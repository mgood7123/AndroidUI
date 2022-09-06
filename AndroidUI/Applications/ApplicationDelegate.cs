using AndroidUI.Input;
using SkiaSharp;

namespace AndroidUI.Applications
{
    public class ApplicationDelegate
    {
        public void INTERNAL_ERROR(string error)
        {
            // TODO: display error to screen
            throw new ApplicationException("INTERNAL ERROR: " + error);
        }

        public void OnCreate()
        {
            Application?.OnCreate();
        }

        private Application application;

        public Touch multiTouch;

        int w, h;
        bool needsSizeChange;
        Graphics.Canvas canvas;

        // TODO: set application state correctly
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

            // change this if you want to support more or less touches
            multiTouch.MaxSupportedTouches = 10;

            multiTouch.throw_on_error = false;
        }
        
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
                        canvas.Dispose();
                        canvas = null;
                    }
                    canvas = Graphics.BaseCanvas.CreateHardwareAcceleratedCanvas<Graphics.Canvas>(graphicsContext, surface, w, h);
                    Application.onSizeChanged(r.Width, r.Height);
                }
                if (canvas != null)
                {
                    canvas.Clear(SKColors.Black);
                    Application.Draw(canvas);
                    canvas.Flush();
                }
            }
            else
            {
                // no application, dispose of canvas
                if (canvas != null)
                {
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

        public void SetDensity(float density, int dpi)
        {
            Application?.SetDensity(density, dpi);
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
