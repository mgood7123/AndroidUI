using AndroidUI.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AndroidUI.OS
{
    internal static partial class Shell
    {
        static public Interfaces.IPsuedoTerminalProvider GetProvider()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new Exception("please call Terminal.Create() instead on Windows");
            }
            else
            {
                return new Unix.UnixPsuedoTerminalProvider() as Interfaces.IPsuedoTerminalProvider;
            }
        }

        public static class Interfaces
        {
            public interface IPsuedoTerminal : IDisposable
            {
                void SetSize(int columns, int rows);

                Task WriteAsync(byte[] buffer, int offset, int count);

                int ReadAsync(byte[] buffer, int offset, int count);

                Process Process { get; }
            }

            public interface IPsuedoTerminalProvider
            {
                public IPsuedoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
                {
                    return CreateInternal(columns, rows, initialDirectory, environment, Env.FindInPath(command), arguments);
                }
                protected IPsuedoTerminal CreateInternal(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments);
            }


            public class DataReceivedEventArgs : EventArgs, IEquatable<DataReceivedEventArgs>
            {
                public byte[] Data { get; set; }

                public bool Equals(DataReceivedEventArgs other)
                {
                    return ReferenceEquals(this, other);
                }
            }

            public interface IConnection
            {
                bool IsConnected { get; }

                event EventHandler<DataReceivedEventArgs> DataReceived;

                event EventHandler<EventArgs> Closed;

                bool Connect();

                void Disconnect();

                void SendData(byte[] data);

                void SetTerminalWindowSize(int columns, int rows, int width, int height);
            }

            public class PsuedoTerminalConnection : IConnection, IDisposable
            {
                ~PsuedoTerminalConnection()
                {
                    Log.d("TERMINAL", "FINALIZE PsuedoTerminalConnection");
                }
                private CancellationTokenSource _cancellationSource;
                private bool _isConnected = false;
                private IPsuedoTerminal _terminal;

                public PsuedoTerminalConnection(IPsuedoTerminal terminal)
                {
                    _terminal = terminal;
                }

                public bool IsConnected => _isConnected;

                public event EventHandler<DataReceivedEventArgs> DataReceived;

                public event EventHandler<EventArgs> Closed;

                public bool Connect()
                {
                    _cancellationSource = new CancellationTokenSource();

                    Task.Run(async () =>
                    {
                        var data = new byte[4096];

                        while (!_cancellationSource.IsCancellationRequested)
                        {
                            var bytesReceived = _terminal.ReadAsync(data, 0, data.Length);

                            if (bytesReceived > 0)
                            {
                                DataReceived?.Invoke(this, new DataReceivedEventArgs { Data = data });
                            }
                            if (bytesReceived == -1)
                            {
                                break;
                            }

                            await Task.Delay(5);
                        }
                    }, _cancellationSource.Token);

                    _isConnected = true;

                    if (!_terminal.Process.HasExited)
                        _terminal.Process.EnableRaisingEvents = true;
                    else
                    {
                        _isConnected = false;
                        _cancellationSource?.Cancel();
                    }

                    if (!_terminal.Process.HasExited)
                        _terminal.Process.Exited += Process_Exited;
                    else
                    {
                        _isConnected = false;
                        _cancellationSource?.Cancel();
                    }

                    return _isConnected;
                }

                private void Process_Exited(object sender, EventArgs e)
                {
                    _terminal.Process.Exited -= Process_Exited;

                    Closed?.Invoke(this, EventArgs.Empty);
                }

                public void Disconnect()
                {
                    _cancellationSource?.Cancel();
                    _terminal?.Dispose();
                }

                public void SendData(byte[] data)
                {
                    _terminal.WriteAsync(data, 0, data.Length);
                }

                public void SetTerminalWindowSize(int columns, int rows, int width, int height)
                {
                    _terminal?.SetSize(columns, rows);
                }

                public void Dispose()
                {
                    _cancellationSource?.Cancel();

                    Disconnect();
                }
            }
        }
    }
}
