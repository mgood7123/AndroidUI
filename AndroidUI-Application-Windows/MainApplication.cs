using AndroidUI.Utils;

namespace AndroidUI_Application_Windows
{
    /*
     */
    internal class MainApplication : AndroidUI.Applications.Application
    {
        /*
         * this is called when your app is first constructed
         * 
         * here you would build up your UI and then call SetContentView
         */
        public override void OnCreate()
        {
            Log.WriteLine("OnCreate");
        }

        /*
         * this is called when your application is paused, currently only relevent on android
         */
        public override void OnPause()
        {
            Log.WriteLine("OnPause");
        }

        /*
         * this is called when your application is resumed, currently only relevent on android
         */
        public override void OnResume()
        {
            Log.WriteLine("OnResume");
        }
    }
}