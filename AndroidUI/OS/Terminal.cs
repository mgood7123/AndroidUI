﻿using System.Runtime.InteropServices;

namespace AndroidUI.OS
{
    public static partial class Terminal
    {
#pragma warning disable CA1063 // Implement IDisposable Correctly

        public abstract class IProcessPackage : IDisposable
        {
            protected readonly ITerminal parent;
            protected readonly bool needsInput;
            protected readonly bool redirectInput;
            protected readonly bool redirectOutput;
            private bool disposedValue;
            Utils.NewlineConvertingWriter converter;
            private uint processExitCode;

            public uint ProcessExitCode => processExitCode;

            protected IProcessPackage(ITerminal parent, bool needsInput, bool redirectInput, bool redirectOutput)
            {
                this.parent = parent;
                this.needsInput = needsInput;
                this.redirectInput = redirectInput;
                this.redirectOutput = redirectOutput;
                converter = new("\r");
            }

            protected abstract void OnSendInput(char input);
            protected abstract void OnSendInput(string input);


            public void SetNewLineConversionTargetForSendInput(string target_newline)
            {
                if (target_newline == null) throw new ArgumentNullException(nameof(target_newline));
                lock (converter)
                {
                    converter.SetNewLineConversionTarget(target_newline);
                }
            }

            public void SendInput(char input, int delayMillisecondsBeforeSending = 0, string override_target_newline = null)
            {
                if (needsInput && redirectInput)
                {
                    if (delayMillisecondsBeforeSending > 0) Thread.Sleep(delayMillisecondsBeforeSending);
                    string c = null;
                    lock (converter)
                    {
                        if (override_target_newline != null)
                        {
                            c = converter.GetNewLineConversionTarget();
                            converter.SetNewLineConversionTarget(override_target_newline);
                        }
                        converter.Write(input);
                        if (override_target_newline != null)
                        {
                            converter.SetNewLineConversionTarget(c);
                        }
                    }
                    OnSendInput(converter.GetConversionResult());
                    converter.ClearConversionResult();
                }
            }

            public void SendInput(string input, int delayMillisecondsBeforeSending = 0, string override_target_newline = null)
            {
                if (needsInput && redirectInput)
                {
                    if (delayMillisecondsBeforeSending > 0) Thread.Sleep(delayMillisecondsBeforeSending);
                    string c = null;
                    lock (converter)
                    {
                        if (override_target_newline != null)
                        {
                            c = converter.GetNewLineConversionTarget();
                            converter.SetNewLineConversionTarget(override_target_newline);
                        }
                        converter.Write(input);
                        if (override_target_newline != null)
                        {
                            converter.SetNewLineConversionTarget(c);
                        }
                    }
                    OnSendInput(converter.GetConversionResult());
                    converter.ClearConversionResult();
                }
            }

            /// <summary>
            /// this should return -1 if there is no output
            /// </summary>
            public abstract long GetOutputLength();

            protected abstract byte? OnReadOutputByte();
            protected abstract int? OnReadOutput(byte[] buffer, int offset, int count);

            public byte? ReadOutputByte()
            {
                return redirectOutput ? OnReadOutputByte() : null;
            }

            public int? ReadOutput(byte[] buffer, int offset, int count)
            {
                return redirectOutput ? OnReadOutput(buffer, offset, count) : null;
            }

            public string ReadOutput(int max_string_length = 4096)
            {
                return redirectOutput ? OnReadOutput(max_string_length) : null;
            }

            protected abstract string OnReadOutput(int max_string_length);

            public abstract void OnWaitForExit();

            public abstract bool HasExited();

            public IProcessPackage WaitForExit()
            {
                OnWaitForExit();
                return this;
            }

            protected void SetProcessExitCode(uint code)
            {
                processExitCode = code;
            }

            protected virtual void OnDispose() { }

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                    lock (parent.children)
                    {
                        parent.children.Remove(this);
                    }
                    OnDispose();
                }
            }

            // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            ~IProcessPackage()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public abstract class ITerminal : IDisposable
        {
            internal Utils.Lists.CopyOnWriteList<IProcessPackage> children = new();
            private bool disposedValue;

            public bool NeedsInput { get; set; } = true;

            public bool RedirectInput { get; set; }
            public bool RedirectOutput { get; set; }

            protected abstract void OnInit();
            protected abstract void OnDispose();

            internal void Init()
            {
                lock (terminals)
                {
                    if (terminals.Count == 0)
                    {
                        OnInit();
                    }
                    terminals.Add(this);
                }
            }

            void DisposeInternal()
            {
                if (!disposedValue)
                {
                    lock (terminals)
                    {
                        lock (children)
                        {
                            foreach (var child in children)
                            {
                                child.Dispose();
                            }
                        }
                        terminals.Remove(this);
                        if (terminals.Count == 0)
                        {
                            OnDispose();
                        }
                    }
                    disposedValue = true;
                }
            }

            protected abstract IProcessPackage CreatePackage(Stream outputRedirectionStream, string commandline);

            public IProcessPackage Run(string commandline, string newLineConversionTargetForSendInput = null)
            {
                return Run(null, commandline, newLineConversionTargetForSendInput);
            }

            public IProcessPackage Run(Stream outputRedirectionStream, string commandline, string newLineConversionTargetForSendInput = null)
            {
                if (outputRedirectionStream == null)
                {
                    outputRedirectionStream = new ReadWriteStream(new MemoryStream(), true);
                }
                var package = CreatePackage(outputRedirectionStream, commandline);
                if (newLineConversionTargetForSendInput != null)
                {
                    package.SetNewLineConversionTargetForSendInput(newLineConversionTargetForSendInput);
                } else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        package.SetNewLineConversionTargetForSendInput("\r");
                    }
                    else
                    {
                        package.SetNewLineConversionTargetForSendInput("\n");
                    }
                }
                children.Add(package);
                return package;
            }

            ~ITerminal()
            {
                DisposeInternal();
            }

            public void Dispose()
            {
                DisposeInternal();
                GC.SuppressFinalize(this);
            }
        }
#pragma warning restore CA1063 // Implement IDisposable Correctly

        static Utils.Lists.CopyOnWriteList<ITerminal> terminals = new();

        public static ITerminal Create()
        {
            ITerminal term;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                term = new Windows.Terminal();
            } else
            {
                throw new PlatformNotSupportedException("Terminal does not yet support linux");
            }
            term.Init();
            return term;
        }
    }
}
