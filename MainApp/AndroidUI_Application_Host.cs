using System;

namespace MainApp
{
    public class AndroidUI_Application_Host
    {

        /**
         * This view is visible.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int VISIBLE = AndroidUI.View.VISIBLE;

        public void setDensity(float density, int dpi)
        {
            AndroidUI.Log.d(ToString(), "density changed: " + density + ", dpi: " + dpi);
            AndroidUI.Application.SetDensity(density, dpi);
            application?.OnScreenDensityChanged();
        }

        /**
         * This view is invisible, but it still takes up space for layout purposes.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int INVISIBLE = AndroidUI.View.INVISIBLE;

        /**
         * This view is invisible, and it doesn't take any space for layout
         * purposes. Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int GONE = AndroidUI.View.GONE;

        AndroidUI.ApplicationDelegate application = new AndroidUI.ApplicationDelegate();

        class TouchInfoView : AndroidUI.Topten_RichTextKit_TextView
        {
            public TouchInfoView() : base()
            {
                setText("TouchInfoView");
            }

            public override bool onTouch(AndroidUI.Touch touch)
            {
                var st = touch.getTouchAtCurrentIndex().state;
                switch (st)
                {
                    case AndroidUI.Touch.State.TOUCH_DOWN:
                    case AndroidUI.Touch.State.TOUCH_MOVE:
                    case AndroidUI.Touch.State.TOUCH_UP:
                        {
                            Console.WriteLine("onTouch invalidating");
                            string s = "";
                            s += "view x     : " + getX() + "\n";
                            s += "view y     : " + getY() + "\n";
                            s += "view width : " + getWidth() + "\n";
                            s += "view height: " + getHeight() + "\n";
                            s += touch.ToString();
                            Console.WriteLine("onTouch: " + s);
                            setText(s);
                            invalidate();
                            break;
                        }

                    default:
                        break;
                }
                return true;
            }
        }

        class TestApp : AndroidUI.Application
        {
            public override void OnCreate()
            {

                if (true)
                {
                    AndroidUI.LinearLayout linearLayout = new();

                    linearLayout.setOrientation(AndroidUI.LinearLayout.VERTICAL);

                    linearLayout.addView(new TouchInfoView(), new AndroidUI.LinearLayout.LayoutParams(AndroidUI.View.LayoutParams.MATCH_PARENT, AndroidUI.View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new AndroidUI.LinearLayout.LayoutParams(AndroidUI.View.LayoutParams.MATCH_PARENT, AndroidUI.View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new AndroidUI.LinearLayout.LayoutParams(AndroidUI.View.LayoutParams.MATCH_PARENT, AndroidUI.View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new AndroidUI.LinearLayout.LayoutParams(AndroidUI.View.LayoutParams.MATCH_PARENT, AndroidUI.View.LayoutParams.MATCH_PARENT, 1.0f));

                    SetContentView(linearLayout);
                }
                else
                {
                    SetContentView(new AndroidUI.Box());
                }
            }
        }

        public void OnCreate()
        {
            application.Application = new TestApp();
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

        public void OnPaint(SkiaSharp.GRContext context, SkiaSharp.GRBackendRenderTarget r, SkiaSharp.SKSurface surface)
        {
            application.OnPaintSurface(context, r, surface);
        }

        public AndroidUI.Touch getMultiTouch()
        {
            return application.multiTouch;
        }

        public void onTouch()
        {
            application.onTouch();
        }
    }
}
