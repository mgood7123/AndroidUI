using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using MainApp;
using SkiaSharp.Views.Android;

namespace Android
{
    class SkiaGL : SKGLSurfaceView
    {
        readonly AndroidUI_Application_Host host = new AndroidUI_Application_Host();

        public SkiaGL(Context context) : base(context) => main();

        public SkiaGL(Context context, IAttributeSet attrs) : base(context, attrs) => main();

        void main()
        {
            host.SetInvalidateCallback(Invalidate);
            DisplayMetrics m = new DisplayMetrics();
            Context.Display.GetRealMetrics(m);
            host.setDensity(m.Density);
            host.OnCreate();
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);
            host.OnPaint(GRContext, e.BackendRenderTarget, e.Surface);
        }

        protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            host.OnVisibilityChanged(visibility == ViewStates.Visible);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            int[] xy = new int[2];
            xy[0] = 0;
            xy[1] = 0;
            //GetLocationInWindow(xy);

            //DisplayMetrics m = new DisplayMetrics();
            //Context.Display.GetRealMetrics(m);

            //int width = m.WidthPixels;
            //int height = m.HeightPixels;

            int width = Width;
            int height = Height;

            int viewX = xy[0];
            int viewY = xy[1];

            MotionEventActions maskedAction_ = e.ActionMasked;

            int pointerIndex = e.ActionIndex;

            switch (maskedAction_)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        long identity = e.GetPointerId(pointerIndex);
                        long time = e.EventTime;
                        float x = viewX + e.GetX(pointerIndex);
                        float y = viewY + e.GetY(pointerIndex);
                        float normalized_X = x / width;
                        float normalized_Y = y / height;
                        float size = e.GetSize(pointerIndex);
                        float pressure = e.GetPressure(pointerIndex);
                        host.getMultiTouch().addTouch(
                            identity, time,
                            x, y,
                            normalized_X, normalized_Y,
                            size, pressure
                        );
                        break;
                    }
                case MotionEventActions.Move:
                    {
                        int historySize = e.HistorySize;
                        int pointerCount = e.PointerCount;
                        for (int h = 0; h < historySize; h++)
                        {
                            long time = e.GetHistoricalEventTime(h);
                            for (int p = 0; p < pointerCount; p++)
                            {
                                long identity = e.GetPointerId(p);
                                float x = viewX + e.GetHistoricalX(p, h);
                                float y = viewY + e.GetHistoricalY(p, h);
                                float normalized_X = x / width;
                                float normalized_Y = y / height;
                                float size = e.GetHistoricalSize(p, h);
                                float pressure = e.GetHistoricalPressure(p, h);
                                host.getMultiTouch().moveTouchBatched(
                                    identity, time,
                                    x, y,
                                    normalized_X, normalized_Y,
                                    size, pressure
                                );
                            }
                        }

                        long time_ = e.EventTime;
                        for (int p = 0; p < pointerCount; p++)
                        {
                            long identity = e.GetPointerId(p);
                            float x = viewX + e.GetX(p);
                            float y = viewY + e.GetY(p);
                            float normalized_X = x / width;
                            float normalized_Y = y / height;
                            float size = e.GetSize(p);
                            float pressure = e.GetPressure(p);
                            host.getMultiTouch().moveTouchBatched(
                                identity, time_,
                                x, y,
                                normalized_X, normalized_Y,
                                size, pressure
                            );
                        }
                        break;
                    }
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    {
                        long identity = e.GetPointerId(pointerIndex);
                        long time = e.EventTime;
                        float x = viewX + e.GetX(pointerIndex);
                        float y = viewY + e.GetY(pointerIndex);
                        float normalized_X = x / width;
                        float normalized_Y = y / height;
                        float size = e.GetSize(pointerIndex);
                        float pressure = e.GetPressure(pointerIndex);
                        host.getMultiTouch().removeTouch(
                            identity, time,
                            x, y,
                            normalized_X, normalized_Y,
                            size, pressure
                        );
                        break;
                    }
                case MotionEventActions.Cancel:
                    {
                        long identity = e.GetPointerId(pointerIndex);
                        long time = e.EventTime;
                        float x = viewX + e.GetX(pointerIndex);
                        float y = viewY + e.GetY(pointerIndex);
                        float normalized_X = x / width;
                        float normalized_Y = y / height;
                        float size = e.GetSize(pointerIndex);
                        float pressure = e.GetPressure(pointerIndex);
                        host.getMultiTouch().cancelTouch(
                            identity, time,
                            x, y,
                            normalized_X, normalized_Y,
                            size, pressure
                        );
                        break;
                    }
            }
            return true;
        }
    }
}