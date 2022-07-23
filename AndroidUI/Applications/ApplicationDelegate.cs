using AndroidUI.Extensions;
using SkiaSharp;

namespace AndroidUI.Applications
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

        int w, h;
        bool needsSizeChange;
        
        public void OnPaintSurface(GRContext graphicsContext, GRBackendRenderTarget r, SKSurface surface)
        {
            if (w != r.Width || h != r.Height)
            {
                w = r.Width;
                h = r.Height;
                needsSizeChange = true;
            }
            if (Application != null)
            {
                if (needsSizeChange)
                {
                    needsSizeChange = false;
                    if (canvas != null)
                    {
                        canvas.DisposeSurface();
                        canvas.Dispose();
                        canvas = null;
                    }
                    canvas = surface.Canvas.CreateHardwareAcceleratedCanvas(graphicsContext, r.Width, r.Height);
                    Application.onSizeChanged(r.Width, r.Height);
                }
                canvas.Clear(SKColors.Black);
                Application.Draw(canvas);
                canvas.Flush();
                canvas.DrawToCanvas(surface.Canvas, 0, 0);
                surface.Canvas.Flush();
            }
            else
            {
                // no application, dispose of canvas
                if (canvas != null)
                {
                    canvas.DisposeSurface();
                    canvas.Dispose();
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
