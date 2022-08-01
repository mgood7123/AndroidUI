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
            ApplicationConfiguration.Initialize();

            AndroidUI.Hosts.Windows.ApplicationHost.InstallHandlers(); // exception handling

            AndroidUI.Hosts.Windows.ApplicationHost.TryToSwitchToHighestDpi();

            Application.Run(
                new AndroidUI.Hosts.Windows.ApplicationHost(

                    // replace this with your own instance of AndroidUI.Applications.Application
                    new TestApp()
                )
            );
        }
    }
}