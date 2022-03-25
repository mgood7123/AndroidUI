﻿using SkiaSharp;

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
            get => application; set
            {
                application?.OnPause();
                application?.SetDelegate(null);
                application = value;
                application?.SetDelegate(this);
            }
        }

        public void OnScreenDensityChanged()
        {
            application?.OnScreenDensityChanged();
        }

        public ApplicationDelegate()
        {
            multiTouch = new Touch();
            multiTouch.setMaxSupportedTouches(10);
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

                    //SKPaint paint = new() { Color = new SKColor(50, 50, 50) };

                    //canvas.DrawRect(new SKRect(0, 0, 200, 200), paint);

                    //Topten_RichTextKit_TextView tv = new();
                    //tv.setTextSize(100);
                    //tv.setTextColor(SKColors.AliceBlue);
                    //tv.setText("Rich text");
                    //tv.measure(
                    //    View.MeasureSpec.makeMeasureSpec(w, View.MeasureSpec.EXACTLY),
                    //    View.MeasureSpec.makeMeasureSpec(h, View.MeasureSpec.EXACTLY)
                    //);
                    //tv.layout(0, 0, w, h);
                    //tv.draw(canvas);

                    //Topten.RichTextKit.TextBlock textBlock = new();
                    //Topten.RichTextKit.Style style = new();

                    //style.FontSize = 100;
                    //style.TextColor = SKColors.AliceBlue;
                    //string text = "Rich text";
                    //textBlock.Clear();
                    //textBlock.AddText(text, style);

                    //textBlock.MaxWidth = w;
                    //textBlock.MaxHeight = h;

                    //textBlock.Paint(canvas);

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