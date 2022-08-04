﻿using Microsoft.Win32.SafeHandles;
using static AndroidUI.OS.Terminal.Windows.Native.PseudoConsoleApi;

namespace AndroidUI.OS
{
    public static partial class Terminal
    {
        internal static partial class Windows
        {
            internal static class Psuedo
            {
                /// <summary>
                /// Utility functions around the new Pseudo Console APIs
                /// </summary>
                internal sealed class PseudoConsole : IDisposable
                {
                    public static readonly IntPtr PseudoConsoleThreadAttribute = (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE;

                    public IntPtr Handle { get; }

                    private PseudoConsole(IntPtr handle)
                    {
                        this.Handle = handle;
                    }

                    internal static PseudoConsole Create(SafeFileHandle inputReadSide, SafeFileHandle outputWriteSide, int width, int height)
                    {
                        var createResult = CreatePseudoConsole(
                            new COORD { X = (short)width, Y = (short)height },
                            inputReadSide, outputWriteSide,
                            0, out IntPtr hPC);
                        if (createResult != 0)
                        {
                            throw new InvalidOperationException("Could not create pseudo console. Error Code " + createResult);
                        }
                        return new PseudoConsole(hPC);
                    }

                    public void Dispose()
                    {
                        ClosePseudoConsole(Handle);
                    }
                }

                /// <summary>
                /// A pipe used to talk to the pseudoconsole, as described in:
                /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session
                /// </summary>
                /// <remarks>
                /// We'll have two instances of this class, one for input and one for output.
                /// </remarks>
                internal sealed class PseudoConsolePipe : IDisposable
                {
                    public readonly SafeFileHandle ReadSide;
                    public readonly SafeFileHandle WriteSide;

                    public PseudoConsolePipe()
                    {
                        if (!CreatePipe(out ReadSide, out WriteSide, IntPtr.Zero, 0))
                        {
                            throw new InvalidOperationException("failed to create pipe");
                        }
                    }

                    #region IDisposable

                    void Dispose(bool disposing)
                    {
                        if (disposing)
                        {
                            ReadSide?.Dispose();
                            WriteSide?.Dispose();
                        }
                    }

                    public void Dispose()
                    {
                        Dispose(true);
                        GC.SuppressFinalize(this);
                    }

                    #endregion
                }
            }
        }
    }
}
