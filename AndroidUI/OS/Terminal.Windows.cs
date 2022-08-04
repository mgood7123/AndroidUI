using AndroidUI.Utils;
using Microsoft.Win32.SafeHandles;
using System.CommandLine;
using static AndroidUI.OS.Terminal.Windows.Native.ConsoleApi;
using static AndroidUI.OS.Terminal.Windows.ProcessAPI;
using static AndroidUI.OS.Terminal.Windows.Psuedo;

namespace AndroidUI.OS
{
    public static partial class Terminal
    {
        partial class Windows
        {
            /// <summary>
            /// The UI of the terminal. It's just a normal console window, but we're managing the input/output.
            /// In a "real" project this could be some other UI.
            /// </summary>
            internal sealed class Terminal : ITerminal
            {
                private static string ExitCommand => "exit" + Environment.NewLine;
                private const string CtrlC_Command = "\x3";

                static uint originalOutConsoleMode;

                public Terminal()
                {
                }

                protected override void OnInit()
                {
                    EnableVirtualTerminalSequenceProcessing();
                }

                protected override void OnDispose()
                {
                    RestoreVirtualTerminalSequenceProcessing();
                }

                /// <summary>
                /// Newer versions of the windows console support interpreting virtual terminal sequences, we just have to opt-in
                /// </summary>
                private static void EnableVirtualTerminalSequenceProcessing()
                {
                    var hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (!GetConsoleMode(hStdOut, out uint outConsoleMode))
                    {
                        throw new InvalidOperationException("Could not get console mode");
                    }
                    originalOutConsoleMode = outConsoleMode;
                    outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                    if (!SetConsoleMode(hStdOut, outConsoleMode))
                    {
                        throw new InvalidOperationException("Could not enable virtual terminal processing");
                    }
                }

                /// <summary>
                /// Newer versions of the windows console support interpreting virtual terminal sequences, we just have to opt-in
                /// </summary>
                private static void RestoreVirtualTerminalSequenceProcessing()
                {
                    var hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (!GetConsoleMode(hStdOut, out uint outConsoleMode))
                    {
                        throw new InvalidOperationException("Could not get console mode");
                    }
                    outConsoleMode = originalOutConsoleMode;
                    if (!SetConsoleMode(hStdOut, outConsoleMode))
                    {
                        throw new InvalidOperationException("Could not restore virtual terminal processing");
                    }
                }

                class ProcessPackage : IProcessPackage
                {
                    Stream terminalOutput;
                    PseudoConsolePipe inputPipe;
                    PseudoConsolePipe outputPipe;
                    PseudoConsole pseudoConsole;
                    Process process;
                    private FileStream pseudoConsoleOutput;
                    private StreamWriter pseudoConsoleInput;
                    bool alive;
                    Task stdinTask, stdoutTask;

                    ReadWriteStream memory;
                    private readonly object LOCK = new();

                    protected override void OnDispose()
                    {
                        WaitForExit();
                        if (memory != null)
                        {
                            memory.Dispose();
                            memory = null;
                        }
                    }

                    public ProcessPackage(
                        ITerminal parent,
                        bool needsInput,
                        bool redirectInput,
                        bool redirectOutput,
                        Stream terminalOutput,
                        PseudoConsolePipe inputPipe,
                        PseudoConsolePipe outputPipe, string command
                    ) : base(parent, needsInput, redirectInput, redirectOutput)
                    {
                        if (redirectOutput)
                        {
                            terminalOutput.Dispose();
                            memory = new(new MemoryStream(), true);
                            this.terminalOutput = memory;
                        }
                        else
                        {
                            this.terminalOutput = terminalOutput;
                        }

                        alive = true;

                        this.inputPipe = inputPipe;
                        this.outputPipe = outputPipe;
                        pseudoConsole = PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, (short)Console.WindowWidth, (short)Console.WindowHeight);
                        process = ProcessFactory.Start(command, PseudoConsole.PseudoConsoleThreadAttribute, pseudoConsole.Handle);

                        // copy all pseudoconsole output to stdout
                        stdoutTask = Task.Run(() => CopyPipeToOutput());
                        // prompt for stdin input and send the result to the pseudoconsole
                        pseudoConsoleInput = new StreamWriter(new FileStream(inputPipe.WriteSide, FileAccess.Write));
                        if (!redirectInput)
                        {
                            ForwardCtrlC(pseudoConsoleInput);
                        }
                        pseudoConsoleInput.AutoFlush = true;
                        stdinTask = Task.Run(() => CopyInputToPipe());

                        // free resources in case the console is ungracefully closed (e.g. by the 'x' in the window titlebar)
                        // must dispose in reverse order
                        OnClose(() =>
                        {
                            DisposeResources(process, pseudoConsole, outputPipe, inputPipe);
                            if (!redirectOutput)
                            {
                                terminalOutput.Dispose();
                            }
                        });

                        if (redirectInput || redirectOutput)
                        {
                            return;
                        }

                        WaitForExit();
                    }

                    protected override void OnSendInput(char input)
                    {
                        lock (LOCK)
                        {
                            if (alive)
                            {
                                pseudoConsoleInput.Write(input);
                            }
                        }
                    }

                    protected override void OnSendInput(string input)
                    {
                        lock (LOCK)
                        {
                            if (alive)
                            {
                                pseudoConsoleInput.Write(input);
                            }
                        }
                    }

                    public override long GetOutputLength()
                    {
                        if (memory == null) return -1;
                        return memory.Length;
                    }

                    protected override byte? OnReadOutputByte()
                    {
                        if (memory == null) return null;
                        int b = memory.ReadByte();
                        return b == -1 ? null : (byte)b;
                    }

                    protected override int? OnReadOutput(byte[] buffer, int offset, int count)
                    {
                        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                        if (memory == null) return null;
                        if (memory.Length == 0) return null;
                        return memory.Read(buffer, offset, count);
                    }

                    protected override string OnReadOutput(int max_string_length)
                    {
                        if (memory == null) return null;
                        if (memory.Length == 0) return null;

                        int l = Math.Min((int)GetOutputLength(), max_string_length);

                        char[] buffer = new char[l+1];
                        for (int i = 0; i < l; i++)
                        {
                            int b = memory.ReadByte();
                            if (b == -1)
                            {
                                buffer[i] = '\0';
                                return new string(buffer, 0, i);
                            }
                            buffer[i] = (char)b;
                        }
                        return new string(buffer, 0, buffer.Length);
                    }

                    /// <summary>
                    /// Reads terminal input and copies it to the PseudoConsole
                    /// </summary>
                    /// <param name="inputWriteSide">the "write" side of the pseudo console input pipe</param>
                    private void CopyInputToPipe()
                    {
                        while (true)
                        {
                            lock (LOCK)
                            {
                                if (!alive)
                                {
                                    break;
                                }
                            }
                            if (redirectInput || !Console.KeyAvailable)
                            {
                                Thread.Sleep(Utils.Const.Constants.THREAD_LOOP_SLEEP_TIME);
                            }
                            else
                            {
                                // send input character-by-character to the pipe
                                char key = Console.ReadKey(intercept: true).KeyChar;
                                OnSendInput(key);
                            }
                        }
                        pseudoConsoleInput.Dispose();
                    }

                    /// <summary>
                    /// Reads PseudoConsole output and copies it to the terminal's standard out.
                    /// </summary>
                    /// <param name="outputReadSide">the "read" side of the pseudo console output pipe</param>
                    private void CopyPipeToOutput()
                    {
                        pseudoConsoleOutput = new FileStream(outputPipe.ReadSide, FileAccess.Read);

                        // blocks until either outputPipe or terminalOutput is disposed
                        pseudoConsoleOutput.CopyTo(terminalOutput);

                        pseudoConsoleOutput.Dispose();
                    }

                    /// <summary>
                    /// disposes resources when exited
                    /// </summary>
                    public override void OnWaitForExit()
                    {
                        if (process != null && alive)
                        {
                            WaitForExit(process).WaitOne(Timeout.Infinite);
                            Native.ProcessApi.GetExitCodeProcess(process.ProcessInfo.hProcess, out var exitCode);
                            SetProcessExitCode(exitCode);
                            lock (LOCK)
                            {
                                alive = false;
                            }
                            stdinTask.Wait();
                            DisposeResources(process, pseudoConsole, outputPipe, inputPipe);
                            if (!redirectOutput)
                            {
                                terminalOutput.Dispose();
                            }
                            stdoutTask.Wait();

                            process = null;
                            pseudoConsole = null;
                            outputPipe = null;
                            inputPipe = null;
                            stdinTask = null;
                            stdoutTask = null;
                        }
                    }

                    /// <summary>
                    /// Get an AutoResetEvent that signals when the process exits
                    /// </summary>
                    private static AutoResetEvent WaitForExit(Process process) =>
                        new AutoResetEvent(false)
                        {
                            SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false)
                        };

                    /// <summary>
                    /// Set a callback for when the terminal is closed (e.g. via the "X" window decoration button).
                    /// Intended for resource cleanup logic.
                    /// </summary>
                    private static void OnClose(Action handler)
                    {
                        SetConsoleCtrlHandler(eventType =>
                        {
                            if (eventType == CtrlTypes.CTRL_CLOSE_EVENT)
                            {
                                handler();
                            }
                            return false;
                        }, true);
                    }

                    private void DisposeResources(params IDisposable[] disposables)
                    {
                        foreach (var disposable in disposables)
                        {
                            disposable.Dispose();
                        }
                    }


                    /// <summary>
                    /// Don't let ctrl-c kill the terminal, it should be sent to the process in the terminal.
                    /// </summary>
                    private void ForwardCtrlC(StreamWriter writer)
                    {
                        Console.CancelKeyPress += (sender, e) =>
                        {
                            lock (LOCK)
                            {
                                if (alive)
                                {
                                    e.Cancel = true;
                                    writer.Write(CtrlC_Command);
                                }
                            }
                        };
                    }
                }

                /// <summary>
                /// Start the pseudoconsole and run the process as shown in 
                /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#creating-the-pseudoconsole
                /// </summary>
                /// <param name="command">the command to run, e.g. cmd.exe</param>
                protected override IProcessPackage CreatePackage(string command)
                {
                    return new ProcessPackage(
                        this,
                        NeedsInput, RedirectInput, RedirectOutput,
                        Console.OpenStandardOutput(),
                        new PseudoConsolePipe(), new PseudoConsolePipe(),
                        command
                    );
                }
            }
        }
    }
}
