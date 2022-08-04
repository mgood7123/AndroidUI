using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AndroidUI.OS
{
    public static partial class Terminal
    {
        internal static partial class Windows
        {
            internal static class Native
            {
                /// <summary>
                /// PInvoke signatures for win32 console api
                /// </summary>
                internal static class ConsoleApi
                {
                    internal const int STD_OUTPUT_HANDLE = -11;
                    internal const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
                    internal const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

                    // https://github.com/microsoft/terminal/issues/3765#issuecomment-624936105
                    // The standard output handle is being closed by a ".NET Finalizer" thread.
                    // It looks like the SafeHandle that you used to set the output mode is being
                    // finalized when its scope ends on line 40.
                    // It's closing the only output handle when it does so.
                    // If you replace the use of SafeHandle in GetStdHandle with IntPtr, it works perfectly
                    // fine forever as the handle isn't prematurely slain in its sleep.
                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern IntPtr GetStdHandle(int nStdHandle);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern bool GetConsoleMode(IntPtr handle, out uint mode);

                    internal delegate bool ConsoleEventDelegate(CtrlTypes ctrlType);

                    internal enum CtrlTypes : uint
                    {
                        CTRL_C_EVENT = 0,
                        CTRL_BREAK_EVENT,
                        CTRL_CLOSE_EVENT,
                        CTRL_LOGOFF_EVENT = 5,
                        CTRL_SHUTDOWN_EVENT
                    }

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
                }

                /// <summary>
                /// PInvoke signatures for win32 process api
                /// </summary>
                internal static class ProcessApi
                {
                    internal const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;

                    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
                    internal struct STARTUPINFOEX
                    {
                        public STARTUPINFO StartupInfo;
                        public IntPtr lpAttributeList;
                    }

                    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
                    internal struct STARTUPINFO
                    {
                        public Int32 cb;
                        public string lpReserved;
                        public string lpDesktop;
                        public string lpTitle;
                        public Int32 dwX;
                        public Int32 dwY;
                        public Int32 dwXSize;
                        public Int32 dwYSize;
                        public Int32 dwXCountChars;
                        public Int32 dwYCountChars;
                        public Int32 dwFillAttribute;
                        public Int32 dwFlags;
                        public Int16 wShowWindow;
                        public Int16 cbReserved2;
                        public IntPtr lpReserved2;
                        public IntPtr hStdInput;
                        public IntPtr hStdOutput;
                        public IntPtr hStdError;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    internal struct PROCESS_INFORMATION
                    {
                        public IntPtr hProcess;
                        public IntPtr hThread;
                        public int dwProcessId;
                        public int dwThreadId;
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    internal struct SECURITY_ATTRIBUTES
                    {
                        public int nLength;
                        public IntPtr lpSecurityDescriptor;
                        public int bInheritHandle;
                    }

                    [DllImport("kernel32.dll", SetLastError = true)]
                    [return: MarshalAs(UnmanagedType.Bool)]
                    internal static extern bool InitializeProcThreadAttributeList(
                        IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint ExitCode);
                    
                    [DllImport("kernel32.dll", SetLastError = true)]
                    [return: MarshalAs(UnmanagedType.Bool)]
                    internal static extern bool UpdateProcThreadAttribute(
                        IntPtr lpAttributeList, uint dwFlags, IntPtr attribute, IntPtr lpValue,
                        IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

                    [DllImport("kernel32.dll")]
                    [return: MarshalAs(UnmanagedType.Bool)]
                    internal static extern bool CreateProcess(
                        string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
                        ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags,
                        IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo,
                        out PROCESS_INFORMATION lpProcessInformation);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    [return: MarshalAs(UnmanagedType.Bool)]
                    internal static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern bool CloseHandle(IntPtr hObject);
                }

                /// <summary>
                /// PInvoke signatures for win32 pseudo console api
                /// </summary>
                internal static class PseudoConsoleApi
                {
                    internal const uint PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;

                    [StructLayout(LayoutKind.Sequential)]
                    internal struct COORD
                    {
                        public short X;
                        public short Y;
                    }

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out IntPtr phPC);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

                    [DllImport("kernel32.dll", SetLastError = true)]
                    internal static extern int ClosePseudoConsole(IntPtr hPC);

                    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                    internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, IntPtr lpPipeAttributes, int nSize);
                }
            }
        }
    }
}
