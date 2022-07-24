using AndroidUI;
using AndroidUI.AnimationFramework.Animator;
using AndroidUI.Applications;
using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Graphics.Drawables;
using AndroidUI.Utils;
using AndroidUI.Utils.Input;
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

            public override bool onTouch(Touch touch)
            {
                var st = touch.getTouchAtCurrentIndex().state;
                switch (st)
                {
                    case Touch.State.TOUCH_DOWN:
                    case Touch.State.TOUCH_MOVE:
                    case Touch.State.TOUCH_UP:
                        {
                            Log.d(GetType().Name, "onTouch invalidating");
                            string s = "";
                            s += "view x     : " + getX() + "\n";
                            s += "view y     : " + getY() + "\n";
                            s += "view width : " + getWidth() + "\n";
                            s += "view height: " + getHeight() + "\n";
                            s += touch.ToString();
                            Log.d(GetType().Name, "onTouch: " + s);
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

        class FlywheelView : Topten_RichTextKit_TextView
        {
            public FlywheelView() : base()
            {
                setText("FlywheelView");
            }

            Flywheel flywheel = new();

            protected override void onDraw(SKCanvas canvas)
            {
                flywheel.AquireLock();
                flywheel.Spin();
                string s = "Flywheel\n";
                s += "  Spinning: " + flywheel.Spinning + "\n";
                s += "  Friction: " + flywheel.Friction + "\n";
                s += "  Distance: \n";
                s += "    x: " + flywheel.Distance.X + " pixels\n";
                s += "    y: " + flywheel.Distance.Y + " pixels\n";
                s += "  Total Distance: \n";
                s += "    x: " + flywheel.TotalDistance.X + " pixels\n";
                s += "    y: " + flywheel.TotalDistance.Y + " pixels\n";
                s += "    time: " + flywheel.SpinTime + " ms\n";
                s += "  Velocity: \n";
                s += "    x: " + flywheel.Velocity.X + "\n";
                s += "    y: " + flywheel.Velocity.Y + "\n";
                s += "  Position: \n";
                s += "    x: " + flywheel.Position.X + " pixels\n";
                s += "    y: " + flywheel.Position.Y + " pixels\n";
                setText(s);
                base.onDraw(canvas);
                if (flywheel.Spinning) invalidate();
                flywheel.ReleaseLock();
            }

            public override bool onTouch(Touch touch)
            {
                Touch.Data data = touch.getTouchAtCurrentIndex();
                var st = data.state;
                flywheel.AquireLock();
                switch (st)
                {
                    case Touch.State.TOUCH_CANCELLED:
                    case Touch.State.TOUCH_DOWN:
                        flywheel.Reset();
                        break;
                    case Touch.State.TOUCH_MOVE:
                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                        break;
                    case Touch.State.TOUCH_UP:
                        flywheel.FinalizeMovement();
                        break;
                    default:
                        break;
                }
                flywheel.ReleaseLock();
                invalidate();
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

            class AL : AnimatorListenerAdapter
            {
                public override void onAnimationEnd(Animator animation)
                {
                    animation.start();
                }
            }

            public override void OnCreate()
            {
                int num = 7;
                switch(num)
                {
                    case 0:
                        {
                            SetContentView(new View());
                        }
                        break;
                    case 1:
                        {
                            LinearLayout linearLayout = new();

                            linearLayout.setOrientation(LinearLayout.VERTICAL);

                            linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                            linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                            linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                            linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));

                            SetContentView(linearLayout);
                        }
                        break;
                    case 2:
                        {
                            SetContentView(new A());
                        }
                        break;
                    case 3:
                        {
                            SetContentView(new BoxView());
                        }
                        break;
                    case 4:
                        {
                            var image = new ImageView();
                            image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);
                            image.setImageDrawable(new ColorDrawable(Color.MAGENTA));

                            ObjectAnimator oa = ObjectAnimator.ofInt(Context, image, "x", new int[] { 0, 100 });
                            oa.setRepeatMode(ValueAnimator.REVERSE);
                            oa.setDuration(2200);

                            ObjectAnimator ob = ObjectAnimator.ofInt(Context, image, "y", new int[] { 0, 100 });
                            ob.setRepeatMode(ValueAnimator.REVERSE);
                            ob.setDuration(1100);

                            ObjectAnimator oc = ObjectAnimator.ofInt(Context, image, "y", new int[] { 100, 200 });
                            oc.setRepeatMode(ValueAnimator.REVERSE);
                            oc.setDuration(1100);

                            AnimatorSet s = new(Context);
                            s.play(oa).with(ob).before(oc);
                            s.addListener(new AL());
                            s.start();
                            SetContentView(image);
                        }
                        break;
                    case 5:
                        {
                            var image = new ImageView();
                            //image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);
                            var bm = BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                            image.setImageBitmap(bm);

                            var s = new ScrollView(Context);
                            s.addView(image);
                            SetContentView(s);
                        }
                        break;
                    case 6:
                        {
                            SetContentView(new FlywheelView());
                            break;
                        }
                    case 7:
                        {
                            var image = new ImageView();
                            //image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);
                            var bm = BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                            image.setImageBitmap(bm);

                            var s = new FlywheelScrollView();
                            s.addView(image);
                            SetContentView(s);
                        }
                        break;
                    case 8:
                        {
                            FrameLayout f = new FrameLayout();
                            var t = new Topten_RichTextKit_TextView();
                            var b = new BoxView();
                            t.setZ(1);
                            b.setZ(0);
                            f.addView(t);
                            f.addView(b);
                            SetContentView(f);
                        }
                        break;
                    default:
                        break;
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
