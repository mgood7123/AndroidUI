using AndroidUI.Extensions;
using AndroidUI.Input;
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

        int w, h;
        bool needsSizeChange;

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
                    surface.Canvas.ExtensionProperties_SetValue("GRContext", graphicsContext);
                    surface.Canvas.setWidthHeight(w, h);
                    surface.Canvas.ExtensionProperties_SetValue("Surface", surface);
                    surface.Canvas.ExtensionProperties_SetValue("HardwareAccelerated", true);
                    Application.onSizeChanged(r.Width, r.Height);
                }
                surface.Canvas.Clear(SKColors.Black);
                Application.Draw(surface.Canvas);
                surface.Canvas.Flush();
            }
            else
            {
                // no application, dispose of canvas
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
