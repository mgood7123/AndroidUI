namespace AndroidUI_Application_Windows
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

            AndroidUI.Hosts.Windows.ApplicationHost.InstallHandlers();
            AndroidUI.Hosts.Windows.ApplicationHost.TryToSwitchToHighestDpi();
            Application.Run(new AndroidUI.Hosts.Windows.ApplicationHost());
        }
    }
}