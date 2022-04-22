namespace OtherPlatforms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if ANDROID
            MainPage = new ContentPage()
            {
                Content = new Platforms.Android.SkiaGLView()
            };
#endif
        }
    }
}