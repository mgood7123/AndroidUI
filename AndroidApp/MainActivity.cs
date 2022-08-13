using static Android.Views.ViewGroup;

namespace AndroidApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var host = new AndroidUI.Applications.TestApp();

            AndroidUI.Widgets.View.DEBUG_VIEW_TRACKING = true;

            var layout_params = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            base.SetContentView(
                new AndroidUI.Hosts.Android.SkiaGL(
                    this, host
                ),
                layout_params
            );
        }
    }
}