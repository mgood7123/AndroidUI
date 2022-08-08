namespace AndroidUI.IDE
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

            Hosts.Windows.ApplicationHost.InstallHandlers(); // exception handling

            Hosts.Windows.ApplicationHost.TryToSwitchToHighestDpi();

            Application.Run(
                new Hosts.Windows.ApplicationHost(
                    new App()
                )
            );
        }
    }
}
