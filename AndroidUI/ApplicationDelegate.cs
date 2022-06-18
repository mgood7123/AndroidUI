using AndroidUI.Extensions;
using SkiaSharp;

namespace AndroidUI
{
    public class ApplicationDelegate
    {
        public void INTERNAL_ERROR(string error)
        {
            // TODO: display error to screen
            throw new ApplicationException(error);
        }

        public void OnCreate()
        {
            Application?.OnCreate();
        }

        private Application application;

        public Touch multiTouch;

        SKCanvas canvas;

        public Application Application
        {
            get => application;
            set
            {
                if (application != null)
                {
                    application.OnPause();
                    application.SetDelegate(null);
                    multiTouch.onTouch -= application.onTouch;
                }
                application = value;
                if (application != null)
                {
                    application.SetDelegate(this);
                    multiTouch.onTouch += application.onTouch;
                }
            }
        }

        public void OnScreenDensityChanged()
        {
            application?.OnScreenDensityChanged();
        }

        public ApplicationDelegate()
        {
            multiTouch = new Touch();
            multiTouch.MaxSupportedTouches = 10;
            multiTouch.throw_on_error = false;
        }

        public void OnPaintSurface(GRContext GRContext, GRBackendRenderTarget r, SKSurface surface)
        {
            if (Application != null)
            {
                int w = r.Width;
                int h = r.Height;

                bool canvas_exists = canvas != null;
                bool canvas_needs_creation = !canvas_exists
                    || canvas.getWidth() != w
                    || canvas.getHeight() != h;

                if (canvas_needs_creation)
                {
                    if (canvas_exists)
                    {
                        canvas.Dispose();
                        canvas.DisposeSurface();
                        canvas = null;
                    }
                    if (w != 0 && h != 0)
                    {
                        canvas = SKCanvasExtensions.CreateHardwareAcceleratedCanvas(null, GRContext, w, h);
                    }
                    Application.onSizeChanged(w, h);
                }
                if (canvas != null)
                {
                    canvas.Clear(SKColors.Black);

                    Application.Draw(canvas);

                    canvas.Flush();

                    canvas.DrawToCanvas(surface.Canvas, 0, 0);
                }
            }
            else
            {
                // no application, dispose of canvas
                if (canvas != null)
                {
                    canvas.Dispose();
                    canvas.DisposeSurface();
                    canvas = null;
                }
                surface.Canvas.Clear(SKColors.Black);
            }
        }

        public void onVisibilityChanged(bool isVisible)
        {
            Application?.handleAppVisibility(isVisible);
        }

        public void onTouch()
        {
            Application?.onTouch(multiTouch);
        }

        Action invalidateCallback;

        public void SetInvalidateCallback(Action invalidate)
        {
            invalidateCallback = invalidate;
        }

        public void invalidate()
        {
            invalidateCallback?.Invoke();
        }
    }
}
