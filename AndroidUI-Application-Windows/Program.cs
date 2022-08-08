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

            SkiaSharp.SKNativeObject.LOG_ALLOCATION_CONSTRUCTOR_ENTER_CALLBACK = stacktrace =>
            {
                Console.WriteLine("SKNativeObject() entering from: " + (stacktrace == null ? "<NULL>" : stacktrace));
            };

            SkiaSharp.SKNativeObject.LOG_ALLOCATION_CONSTRUCTOR_EXIT_CALLBACK = stacktrace =>
            {
                Console.WriteLine("SKNativeObject() exiting from stack trace:" + (stacktrace == null ? "<NULL>" : stacktrace));
            };

            SkiaSharp.SKNativeObject.LOG_ALLOCATION_DESTRUCTOR_ENTER_CALLBACK = stacktrace =>
            {
                Console.WriteLine("~SKNativeObject() entering from stack trace:" + (stacktrace == null ? "<NULL>" : stacktrace));
            };

            SkiaSharp.SKNativeObject.LOG_ALLOCATION_DESTRUCTOR_EXIT_CALLBACK = stacktrace =>
            {
                Console.WriteLine("~SKNativeObject() exiting from stack trace:" + (stacktrace == null ? "<NULL>" : stacktrace));
            };

            Application.Run(
                new AndroidUI.Hosts.Windows.ApplicationHost(

                    // replace this with your own instance of AndroidUI.Applications.Application
                    new TestApp()
                )
            );
        }
    }
}