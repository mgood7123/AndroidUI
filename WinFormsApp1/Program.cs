using AndroidUI.Utils;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(Form1.UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(Form1.NonUIThreadException);

            TryToSwitchToHighestDpi();
            Application.Run(new Form1());
        }

        private static void TryToSwitchToHighestDpi()
        {
            HighDpiMode highDpiMode = Application.HighDpiMode;
            if (highDpiMode == HighDpiMode.PerMonitorV2)
            {
                Log.WriteLine("current HighDpiMode is " + Application.HighDpiMode);
            }
            else
            {
                if (Application.SetHighDpiMode(HighDpiMode.PerMonitorV2))
                {
                    Log.WriteLine("set HighDpiMode to " + Application.HighDpiMode);
                }
                else
                {
                    Log.WriteLine("Failed to set HighDpiMode to " + HighDpiMode.PerMonitorV2);
                    if (highDpiMode == HighDpiMode.PerMonitor)
                    {
                        Log.WriteLine("current HighDpiMode is " + Application.HighDpiMode);
                    }
                    else
                    {
                        if (Application.SetHighDpiMode(HighDpiMode.PerMonitor))
                        {
                            Log.WriteLine("set HighDpiMode to " + Application.HighDpiMode);
                        }
                        else
                        {
                            Log.WriteLine("Failed to set HighDpiMode to " + HighDpiMode.PerMonitor);
                            if (highDpiMode == HighDpiMode.SystemAware)
                            {
                                Log.WriteLine("current HighDpiMode is " + Application.HighDpiMode);
                            }
                            else
                            {
                                if (Application.SetHighDpiMode(HighDpiMode.SystemAware))
                                {
                                    Log.WriteLine("set HighDpiMode to " + Application.HighDpiMode);
                                }
                                else
                                {
                                    Log.WriteLine("Failed to set HighDpiMode to " + HighDpiMode.SystemAware);
                                    Log.WriteLine("current HighDpiMode is " + Application.HighDpiMode);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}