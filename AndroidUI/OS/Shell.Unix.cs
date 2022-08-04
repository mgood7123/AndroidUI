using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static AndroidUI.OS.Shell.Interfaces;

namespace AndroidUI.OS
{
    internal static partial class Shell
    {
        internal static class Unix
        {
            internal class NativeDelegates
            {
                [DllImport("libdl.so.2", EntryPoint = "dlopen")]
                private static extern IntPtr dlopen_lin(string path, int flags);

                [DllImport("libdl.so.2", EntryPoint = "dlsym")]
                private static extern IntPtr dlsym_lin(IntPtr handle, string symbol);

                [DllImport("libSystem.dylib", EntryPoint = "dlopen")]
                private static extern IntPtr dlopen_mac(string path, int flags);

                [DllImport("libSystem.dylib", EntryPoint = "dlsym")]
                private static extern IntPtr dlsym_mac(IntPtr handle, string symbol);

                public static T GetProc<T>()
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        var dl = dlopen_mac("libSystem.dylib", 2);

                        var name = typeof(T).Name;
                        var a = dlsym_mac(dl, name);
                        return Marshal.GetDelegateForFunctionPointer<T>(a);
                    }
                    else
                    {
                        var dl = dlopen_lin("libc.6.so", 2);
                        var a = dlsym_lin(dl, typeof(T).Name);
                        return Marshal.GetDelegateForFunctionPointer<T>(a);
                    }
                }

                public static T GetProc<T>(string function)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        var dl = dlopen_mac("libSystem.dylib", 2);
                        var a = dlsym_mac(dl, function);
                        return Marshal.GetDelegateForFunctionPointer<T>(a);
                    }
                    else
                    {
                        var dl = dlopen_lin("libc.6.so", 2);
                        var a = dlsym_lin(dl, function);
                        return Marshal.GetDelegateForFunctionPointer<T>(a);
                    }
                }

                public delegate void dup2(int oldfd, int newfd);
                public delegate int fork();
                public delegate void setsid();
                public delegate int ioctl(int fd, UInt64 ctl, IntPtr arg);
                public delegate void close(int fd);
                public delegate int open([MarshalAs(UnmanagedType.LPStr)] string file, int flags);
                public delegate int chdir([MarshalAs(UnmanagedType.LPStr)] string path);
                public delegate IntPtr ptsname(int fd);
                public delegate int grantpt(int fd);
                public delegate int unlockpt(int fd);
                public unsafe delegate void execve([MarshalAs(UnmanagedType.LPStr)] string path, [MarshalAs(UnmanagedType.LPArray)] string[] argv, [MarshalAs(UnmanagedType.LPArray)] string[] envp);
                public delegate int read(int fd, IntPtr buffer, int length);
                public delegate int write(int fd, IntPtr buffer, int length);
                public delegate void free(IntPtr ptr);
                public delegate int pipe(IntPtr[] fds);
                public delegate int setpgid(int pid, int pgid);
                public delegate int posix_spawn_file_actions_adddup2(IntPtr file_actions, int fildes, int newfildes);
                public delegate int posix_spawn_file_actions_addclose(IntPtr file_actions, int fildes);
                public delegate int posix_spawn_file_actions_init(IntPtr file_actions);
                public delegate int posix_spawnattr_init(IntPtr attributes);
                public delegate int posix_spawnp(out IntPtr pid, string path, IntPtr fileActions, IntPtr attrib, string[] argv, string[] envp);
                public delegate int dup(int fd);
                public delegate void _exit(int code);
                public delegate int getdtablesize();
            }

            internal static class Native
            {
                public const int O_RDONLY = 0x0000;
                public const int O_WRONLY = 0x0001;
                public const int O_RDWR = 0x0002;
                public const int O_ACCMODE = 0x0003;

                public const int O_CREAT = 0x0100; /* second byte, away from DOS bits */
                public const int O_EXCL = 0x0200;
                public const int O_NOCTTY = 0x0400;
                public const int O_TRUNC = 0x0800;
                public const int O_APPEND = 0x1000;
                public const int O_NONBLOCK = 0x2000;
                public static readonly ulong TIOCSWINSZ = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0x80087467 : 0x5414;

                public const int _SC_OPEN_MAX = 5;

                public const int EAGAIN = 11;  /* Try again */

                public const int EINTR = 4; /* Interrupted system call */

                public const int ENOENT = 2;

                public static readonly ulong TIOCSCTTY = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? (ulong)0x20007484 : 0x540E;

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                public struct winsize
                {
                    public ushort ws_row;   /* rows, in characters */
                    public ushort ws_col;   /* columns, in characters */
                    public ushort ws_xpixel;    /* horizontal size, pixels */
                    public ushort ws_ypixel;    /* vertical size, pixels */
                };

                public static NativeDelegates.open open = NativeDelegates.GetProc<NativeDelegates.open>();
                public static NativeDelegates.chdir chdir = NativeDelegates.GetProc<NativeDelegates.chdir>();
                public static NativeDelegates.write write = NativeDelegates.GetProc<NativeDelegates.write>();
                public static NativeDelegates.grantpt grantpt = NativeDelegates.GetProc<NativeDelegates.grantpt>();
                public static NativeDelegates.unlockpt unlockpt = NativeDelegates.GetProc<NativeDelegates.unlockpt>();
                public static NativeDelegates.ptsname ptsname = NativeDelegates.GetProc<NativeDelegates.ptsname>();
                public static NativeDelegates.posix_spawn_file_actions_init posix_spawn_file_actions_init = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_init>();
                public static NativeDelegates.posix_spawn_file_actions_adddup2 posix_spawn_file_actions_adddup2 = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_adddup2>();
                public static NativeDelegates.posix_spawn_file_actions_addclose posix_spawn_file_actions_addclose = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_addclose>();
                public static NativeDelegates.posix_spawnattr_init posix_spawnattr_init = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_init>();
                public static NativeDelegates.posix_spawnp posix_spawnp = NativeDelegates.GetProc<NativeDelegates.posix_spawnp>();
                public static NativeDelegates.dup dup = NativeDelegates.GetProc<NativeDelegates.dup>();
                public static NativeDelegates.setsid setsid = NativeDelegates.GetProc<NativeDelegates.setsid>();
                public static NativeDelegates.ioctl ioctl = NativeDelegates.GetProc<NativeDelegates.ioctl>();
                public static NativeDelegates.execve execve = NativeDelegates.GetProc<NativeDelegates.execve>();

                public static IntPtr StructToPtr(object obj)
                {
                    var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
                    Marshal.StructureToPtr(obj, ptr, false);
                    return ptr;
                }


            }

            public class UnixPsuedoTerminal : IPsuedoTerminal
            {
                private int _handle;
                private int _cfg;
                private Stream _stdin = null;
                private Stream _stdout = null;
                private Process _process;
                private bool _isDisposed = false;

                public UnixPsuedoTerminal(Process process, int handle, int cfg, Stream stdin, Stream stdout)
                {
                    _process = process;

                    _handle = handle;
                    _stdin = stdin;
                    _stdout = stdout;

                    _cfg = cfg;
                }

                public static void Trampoline(string[] args)
                {
                    if (args.Length > 2 && args[0] == "--trampoline")
                    {
                        Native.setsid();
                        Native.ioctl(0, Native.TIOCSCTTY, IntPtr.Zero);
                        Native.chdir(args[1]);

                        var envVars = new List<string>();
                        var env = Environment.GetEnvironmentVariables();

                        foreach (var variable in env.Keys)
                        {
                            if (variable.ToString() != "TERM")
                            {
                                envVars.Add($"{variable}={env[variable]}");
                            }
                        }

                        envVars.Add("TERM=xterm-256color");
                        envVars.Add(null);

                        var argsArray = args.Skip(3).ToList();
                        argsArray.Add(null);

                        Native.execve(args[2], argsArray.ToArray(), envVars.ToArray());
                    }
                    else
                    {
                        return;
                    }
                }

                public void Dispose()
                {
                    if (!_isDisposed)
                    {
                        _isDisposed = true;
                        _stdin?.Dispose();
                        _stdout?.Dispose();

                        // TODO close file descriptors and terminate processes?
                    }
                }

                public int ReadAsync(byte[] buffer, int offset, int count)
                {
                    return _stdout.Read(buffer, offset, count);
                }

                public async Task WriteAsync(byte[] buffer, int offset, int count)
                {
                    if (buffer.Length == 1 && buffer[0] == 10)
                    {
                        buffer[0] = 13;
                    }

                    await Task.Run(() =>
                    {
                        var buf = Marshal.AllocHGlobal(count);
                        Marshal.Copy(buffer, offset, buf, count);
                        Native.write(_cfg, buf, count);

                        Marshal.FreeHGlobal(buf);
                    });
                }

                public void SetSize(int columns, int rows)
                {
                    Native.winsize size = new Native.winsize();
                    int ret;
                    size.ws_row = (ushort)(rows > 0 ? rows : 24);
                    size.ws_col = (ushort)(columns > 0 ? columns : 80);

                    var ptr = Native.StructToPtr(size);

                    ret = Native.ioctl(_cfg, Native.TIOCSWINSZ, ptr);

                    Marshal.FreeHGlobal(ptr);

                    var error = Marshal.GetLastWin32Error();
                }

                public struct winsize
                {
                    public ushort ws_row;   /* rows, in characters */
                    public ushort ws_col;   /* columns, in characters */
                    public ushort ws_xpixel;    /* horizontal size, pixels */
                    public ushort ws_ypixel;    /* vertical size, pixels */
                };

                public Process Process => _process;
            }

            public class UnixPsuedoTerminalProvider : IPsuedoTerminalProvider
            {
                public IPsuedoTerminal CreateInternal(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
                {
                    var fdm = Native.open("/dev/ptmx", Native.O_RDWR | Native.O_NOCTTY);

                    var res = Native.grantpt(fdm);
                    res = Native.unlockpt(fdm);

                    var namePtr = Native.ptsname(fdm);
                    var name = Marshal.PtrToStringAnsi(namePtr);
                    var fds = Native.open(name, (int)Native.O_RDWR);

                    var fileActions = Marshal.AllocHGlobal(1024);
                    Native.posix_spawn_file_actions_init(fileActions);
                    res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 0);
                    res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 1);
                    res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 2);
                    res = Native.posix_spawn_file_actions_addclose(fileActions, (int)fdm);
                    res = Native.posix_spawn_file_actions_addclose(fileActions, (int)fds);


                    var attributes = Marshal.AllocHGlobal(1024);
                    res = Native.posix_spawnattr_init(attributes);

                    var envVars = new List<string>();
                    var env = Environment.GetEnvironmentVariables();

                    foreach (var variable in env.Keys)
                    {
                        if (variable.ToString() != "TERM")
                        {
                            envVars.Add($"{variable}={env[variable]}");
                        }
                    }

                    envVars.Add("TERM=xterm-256color");
                    envVars.Add(null);

                    var path = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var argsArray = new List<string> { "dotnet", path, "--trampoline", initialDirectory, command };
                    argsArray.AddRange(arguments);
                    argsArray.Add(null);

                    res = Native.posix_spawnp(out var pid, "dotnet", fileActions, attributes, argsArray.ToArray(), envVars.ToArray());

                    var stdin = Native.dup(fdm);
                    var process = Process.GetProcessById((int)pid);
                    return new UnixPsuedoTerminal(process, fds, stdin, new FileStream(new SafeFileHandle(new IntPtr(stdin), true), FileAccess.Write), new FileStream(new SafeFileHandle(new IntPtr(fdm), true), FileAccess.Read));
                }
            }
        }
    }
}
