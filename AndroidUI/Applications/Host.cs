using AndroidUI.Input;
using AndroidUI.Utils;
using AndroidUI.Widgets;
using SkiaSharp;

namespace AndroidUI.Applications
{

    public class Host
    {

        /**
         * This view is visible.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int VISIBLE = View.VISIBLE;

        public void setDensity(float density, int dpi)
        {
            Log.d(ToString(), "density changed: " + density + ", dpi: " + dpi);
            application?.SetDensity(density, dpi);
        }

        /**
         * This view is invisible, but it still takes up space for layout purposes.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int INVISIBLE = View.INVISIBLE;

        /**
         * This view is invisible, and it doesn't take any space for layout
         * purposes. Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int GONE = View.GONE;

        private ApplicationDelegate application = new();

        public void SetApplication(Application application = null)
        {
            this.application.Application = application;
        }

        public void OnCreate()
        {
            application.OnCreate();
        }

        public void SetInvalidateCallback(Action invalidate)
        {
            application.SetInvalidateCallback(invalidate);
        }

        public void OnVisibilityChanged(bool isVisible)
        {
            application.onVisibilityChanged(isVisible);
        }

        public void OnPaint(GRContext context, GRBackendRenderTarget r, SKSurface surface)
        {
            application.OnPaintSurface(context, r, surface);
        }

        public Touch getMultiTouch()
        {
            return application.multiTouch;
        }

        public void onTouch()
        {
            application.onTouch();
        }
    }
}
