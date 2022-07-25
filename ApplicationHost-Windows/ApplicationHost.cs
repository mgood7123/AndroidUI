using AndroidUI.Utils;
using System.Diagnostics;

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

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        internal static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Windows Forms Error", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only
        // log the event, and inform the user about it.
        internal static void NonUIThreadException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errorMsg = "An application error occurred. Please contact the adminstrator " +
                    "with the following information:\n\n";

                // Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists("ThreadException"))
                {
                    EventLog.CreateEventSource("ThreadException", "Application");
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new();
                myLog.Source = "ThreadException";
                myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        // Creates the error message and displays it.
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred. Please contact the adminstrator " +
                "with the following information:\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
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
