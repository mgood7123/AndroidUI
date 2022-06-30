using AndroidUI.AnimationFramework.Animation;
using AndroidUI.Extensions;
using SkiaSharp;
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

        AndroidUI.ApplicationDelegate application = new();

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
            class A : AndroidUI.View
            {

                public A()
                {
                    setWillDraw(true);
                }

                protected override void onDraw(SKCanvas canvas)
                {
                    base.onDraw(canvas);
                    var bm = AndroidUI.BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                    var p = new AndroidUI.Paint();
                    p.setColor(AndroidUI.Color.WHITE);
                    canvas.DrawBitmap(bm, 0, 0, p);
                    bm.recycle();
                }
            }

            class l : Animation.AnimationListener
            {
                public void onAnimationEnd(Animation animation)
                    => Console.WriteLine("ANIMATION END: " + animation);

                public void onAnimationRepeat(Animation animation)
                    => Console.WriteLine("ANIMATION REPEAT: " + animation);

                public void onAnimationStart(Animation animation)
                    => Console.WriteLine("ANIMATION START: " + animation);
            }

            public override void OnCreate()
            {

                if (false)
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
                    if (false)
                    {
                        if (false)
                        {
                            SetContentView(new A());
                        }
                        else
                        {
                            SetContentView(new AndroidUI.BoxView());
                        }
                    }
                    else
                    {

                        var image = new AndroidUI.ImageView();
                        //var bm = AndroidUI.BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                        //image.setImageBitmap(bm);
                        //image.setBackgroundColor(AndroidUI.Color.MAGENTA);
                        image.setImageDrawable(new AndroidUI.ColorDrawable(AndroidUI.Color.MAGENTA));
                        image.setScaleType(AndroidUI.ImageView.ScaleType.CENTER_INSIDE);
                        
                        //AlphaAnimation anim = new(Context, 0.15f, 1.0f);
                        //RotateAnimation anim = new(Context, 0, 180);
                        TranslateAnimation anim = new(Context, 0, 200, 0, 100);

                        anim.setDuration(2200);
                        anim.setRepeatCount(Animation.INFINITE);
                        anim.setRepeatMode(Animation.REVERSE);
                        anim.setAnimationListener(new l());
                        image.startAnimation(anim);

                        SetContentView(image);
                    }
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

        public void OnPaint(GRContext context, GRBackendRenderTarget r, SKSurface surface)
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
