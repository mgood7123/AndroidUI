using AndroidUI.AnimationFramework.Animation;
using AndroidUI.AnimationFramework.Animator;
using AndroidUI.AnimationFramework.Interpolators;
using AndroidUI.Applications;
using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Graphics.Drawables;
using AndroidUI.Utils;
using AndroidUI.Widgets;
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
        public const int VISIBLE = View.VISIBLE;

        public void setDensity(float density, int dpi)
        {
            Log.d(ToString(), "density changed: " + density + ", dpi: " + dpi);
            Application.SetDensity(density, dpi);
            application?.OnScreenDensityChanged();
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

        ApplicationDelegate application = new();

        class TouchInfoView : Topten_RichTextKit_TextView
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

        class TestApp : Application
        {
            class A : View
            {

                public A()
                {
                    setWillDraw(true);
                }

                protected override void onDraw(SKCanvas canvas)
                {
                    base.onDraw(canvas);
                    var bm = BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                    var p = new Paint();
                    p.setColor(Color.WHITE);
                    canvas.DrawBitmap(bm, 0, 0, p);
                    bm.recycle();
                }
            }

            public override void OnCreate()
            {

                if (false)
                {
                    LinearLayout linearLayout = new();

                    linearLayout.setOrientation(LinearLayout.VERTICAL);

                    linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                    linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));

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
                            SetContentView(new BoxView());
                        }
                    }
                    else
                    {

                        var image = new ImageView();
                        //var bm = AndroidUI.BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                        //image.setImageBitmap(bm);
                        //image.setBackgroundColor(AndroidUI.Color.MAGENTA);
                        image.setImageDrawable(new ColorDrawable(Color.MAGENTA));
                        image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);

                        //image.setX(200);
                        //image.setTranslationX(200);
                        //image.setY(200);
                        //image.setTranslationY(200);

                        //AndroidUI.Path path = new AndroidUI.Path();
                        //path.arcTo(0f, 0f, 1000f, 1000f, 270f, -180f, true);
                        //PathInterpolator pathInterpolator = new PathInterpolator(path);

                        //ObjectAnimator animation = ObjectAnimator.ofFloat(Context, image, "x", 100f);
                        //animation.setInterpolator(pathInterpolator);
                        //animation.setDuration(2200);
                        //animation.start();


                        ObjectAnimator oa = ObjectAnimator.ofInt(Context, image, "x", new int[] { 0, 100 });
                        //oa.setRepeatCount(ValueAnimator.INFINITE);
                        oa.setRepeatMode(ValueAnimator.REVERSE);
                        oa.setDuration(2200);

                        ObjectAnimator ob = ObjectAnimator.ofInt(Context, image, "y", new int[] { 0, 100 });
                        //ob.setRepeatCount(ValueAnimator.INFINITE);
                        ob.setRepeatMode(ValueAnimator.REVERSE);
                        ob.setDuration(1100);

                        ObjectAnimator oc = ObjectAnimator.ofInt(Context, image, "y", new int[] { 100, 200 });
                        //ob.setRepeatCount(ValueAnimator.INFINITE);
                        oc.setRepeatMode(ValueAnimator.REVERSE);
                        oc.setDuration(1100);

                        AnimatorSet s = new(Context);
                        s.play(oa).with(ob).before(oc);
                        s.start();

                        //oa.start();
                        //ob.start();

                        //AlphaAnimation anim = new(Context, 0.15f, 1.0f);
                        //RotateAnimation anim = new(Context, 0, 180);
                        //TranslateAnimation anim = new(Context, 0, 200, 0, 100);

                        //anim.setDuration(2200);
                        //anim.setRepeatCount(Animation.INFINITE);
                        //anim.setRepeatMode(Animation.REVERSE);
                        //anim.setAnimationListener(new l());
                        //image.startAnimation(anim);

                        SetContentView(image);
                        //animator.start();
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
