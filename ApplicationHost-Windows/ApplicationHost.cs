using AndroidUI.Utils;

namespace AndroidUI.Hosts.Windows
{
    public partial class ApplicationHost : Form
    {
        SkiaGL skia;

        public ApplicationHost(Applications.Application application = null)
        {
            InitializeComponent();

            skia = new(application);

            Load += ApplicationHost_Load;
            ClientSizeChanged += ApplicationHost_Resize;
        }

        private void ApplicationHost_Resize(object? sender, EventArgs e)
        {
            Control? control = sender as Control;
            if (control != null)
            {
                skia.Size = ClientSize;
            }
        }

        private void ApplicationHost_Load(object? sender, EventArgs e)
        {
            ApplicationHost_Resize(sender!, e);
            Controls.Add(skia);
        }

        private static void ShowException(string title, Exception e)
        {
            DialogResult result = DialogResult.Cancel;
            result = MessageBox.Show("Do you want to exit?\n\nAn Exception has occured:\n\n" + e, title, MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (result == DialogResult.Yes)
            {
                Log.d("AndroidUI Application Host", "\n-------- ERROR --------\n\n" + e.ToString() + "\n\n-------- ERROR --------\n");
                Environment.Exit(-1);
            }
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        internal static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            ShowException("UI Exception", t.Exception);
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only
        // log the event, and inform the user about it.
        internal static void NonUIThreadException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowException("Non-UI Exception", (Exception)e.ExceptionObject);
        }

        public static void InstallHandlers()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(AndroidUI.Hosts.Windows.ApplicationHost.UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event.
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(AndroidUI.Hosts.Windows.ApplicationHost.NonUIThreadException);
        }

        public static void TryToSwitchToHighestDpi()
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
