/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.AnimationFramework.Animator;
using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Graphics.Drawables;
using AndroidUI.Input;
using AndroidUI.Utils;
using AndroidUI.Utils.Input;
using AndroidUI.Utils.Lists;
using AndroidUI.Utils.Widgets;
using AndroidUI.Widgets;
using SkiaSharp;
using static AndroidUI.Widgets.View;
using Application = AndroidUI.Applications.Application;
using Color = AndroidUI.Graphics.Color;
using View = AndroidUI.Widgets.View;

namespace AndroidUI_Application_Windows
{
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

        class MyAdapterView : AdapterView<ArrayAdapter<string>, string>
        {
            ArrayAdapter<string> mAdapter;

            private int mSelectedPosition;
            private int mFirstPosition;

            public override ArrayAdapter<string> getAdapter()
            {
                Log.d("getAdapter");
                return mAdapter;
            }

            public override View getSelectedView()
            {
                Log.d("getSelectedView");
                if (getCount() > 0 && mSelectedPosition >= 0)
                {
                    return getChildAt(mSelectedPosition - mFirstPosition);
                }
                else
                {
                    return null;
                }
            }

            public override void setAdapter(ArrayAdapter<string> adapter)
            {
                Log.d("setAdapter: " + adapter);
                mAdapter = adapter;
            }

            public override void setSelection(int position)
            {
                Log.d("setSelection: " + position);
            }
        }

        class TabView : LinearLayout
        {
            //FlywheelScrollView tabContainer;
            LinearLayout tabs;
            FrameLayout tabContent;

            public TabView()
            {
                setOrientation(HORIZONTAL);
                InitTabView();
            }

            public void addTab(string title, View content)
            {
                Topten_RichTextKit_TextView a = new();
                a.setText(title);
                a.setTextSize(16);
                a.setTag(content);
                a.setOnClickListener(v =>
                {
                    tabContent.removeAllViews();
                    tabContent.addView((View)v.getTag(), MATCH_PARENT__MATCH_PARENT);
                });
                tabs.addView(a, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT));
            }

            public void addTab(string title, Func<View> builder)
            {
                addTab(title, builder.Invoke());
            }

            private void InitTabView()
            {
                tabs = new();
                //tabContainer = new();
                tabContent = new();

                tabs.setOrientation(VERTICAL);

                //tabContainer.SmoothScroll = false;
                //tabContainer.addView(tabs);
                addView(tabs, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.MATCH_PARENT));
                addView(tabContent, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
                tabs.setZ(1);
                post(new Runnable.ActionRunnable(() => tabs.setBackgroundColor(Color.BLACK)));
            }
        }

        public override void OnCreate()
        {
            TabView tabView = new();

            tabView.addTab("View", () => new View());
            tabView.addTab("Touch Info Linear Layout", () =>
            {
                LinearLayout linearLayout = new();

                linearLayout.setOrientation(LinearLayout.VERTICAL);

                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                return linearLayout;
            });
            tabView.addTab("Bitmap", () => new A());
            tabView.addTab("ColorView", () =>
            {
                ColorView colorView = new ColorView();
                colorView.setOnClickListener(v =>
                {
                    colorView.Color = new SKColor(
                        (byte)Random.Shared.Next(255),
                        (byte)Random.Shared.Next(255),
                        (byte)Random.Shared.Next(255)
                    );
                });
                return colorView;
            });
            tabView.addTab("Animation", () => 
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
                return image;
            });
            tabView.addTab("Z Ordering", () =>
            {
                FrameLayout f = new FrameLayout();

                var a = new ColorView();
                var t = new Topten_RichTextKit_TextView();
                a.setZ(0);
                t.setZ(1);
                f.addView(a);
                f.addView(t);
                return f;
            });
            tabView.addTab("Flywheel", () => new FlywheelView());
            tabView.addTab("Scrolling", () =>
            {
                var image = new ImageView();
                //image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);
                var bm = BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                image.setImageBitmap(bm);

                var s = new FlywheelScrollView();
                s.SmoothScroll = false;
                s.LimitScrollingToChildViewBounds = false;
                s.addView(image);
                return s;
            });
            tabView.addTab("Clicking", () =>
            {
                var t = new Topten_RichTextKit_TextView();
                t.setOnClickListener(v => t.setText("Clicked!"));
                t.setOnLongClickListener(v => { t.setText("Long clicked!"); return true; });
                return t;
            });
            tabView.addTab("Adapter", () =>
            {
                var av = new MyAdapterView();
                av.setAdapter(new(() => new(), "1", "2", "3"));
                av.getAdapter().notifyDataSetChanged();
                av.getAdapter().notifyDataSetInvalidated();
                return av;
            });
            tabView.addTab("Scrolling 2", () =>
            {
                var image = new ImageView();
                var bm = BitmapFactory.decodeFile("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg");
                image.setImageBitmap(bm);
                image.setScaleType(ImageView.ScaleType.MATRIX);

                var s = new FlywheelScrollView();
                s.SmoothScroll = true;
                s.addView(image);
                return s;
            });
            tabView.addTab("TabView", () =>
            {
                TabView tabView = new();

                var a = new ImageView();
                a.setImageDrawable(new ColorDrawable(Color.MAGENTA));
                tabView.addTab("magenta", a);

                var b = new ImageView();
                b.setImageDrawable(new ColorDrawable(Color.BLUE));
                tabView.addTab("blue", b);

                return tabView;
            });
            tabView.addTab("Linear Layout", () =>
            {
                LinearLayout linearLayout = new();

                linearLayout.setOrientation(LinearLayout.VERTICAL);

                linearLayout.addView(
                    new Topten_RichTextKit_TextView(),
                    new LinearLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT
                    )
                );
                linearLayout.addView(
                    new ColorView(Color.MAGENTA),
                    new LinearLayout.LayoutParams(
                        View.LayoutParams.MATCH_PARENT,
                        View.LayoutParams.MATCH_PARENT,
                        1
                    )
                );

                return linearLayout;
            });
            SetContentView(tabView);

            //ColorView colorView = new ColorView();
            //colorView.setOnClickListener(v =>
            //{
            //    colorView.Color = new SKColor(
            //        (byte)Random.Shared.Next(255),
            //        (byte)Random.Shared.Next(255),
            //        (byte)Random.Shared.Next(255)
            //    );
            //});

            //ColorView colorViewB = new ColorView();
            //colorViewB.setOnClickListener(v =>
            //{
            //    colorViewB.Color = new SKColor(
            //        (byte)Random.Shared.Next(255),
            //        (byte)Random.Shared.Next(255),
            //        (byte)Random.Shared.Next(255)
            //    );
            //});

            //var l = new LinearLayout();
            //l.setOrientation(LinearLayout.HORIZONTAL);
            //l.addView(colorView, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
            //VV tabContent = new();
            //tabContent.addView(colorViewB, MATCH_PARENT__MATCH_PARENT);
            //l.addView(tabContent, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
            //SetContentView(l);
        }

        class VV : View
        {
            protected override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
            {
                base.onMeasure(widthMeasureSpec, heightMeasureSpec);
            }
            protected override void onLayout(bool changed, int l, int t, int r, int b)
            {
                base.onLayout(changed, l, t, r, b);
            }

            protected override void onDraw(SKCanvas canvas)
            {
                base.onDraw(canvas);
            }
        }
    }
}
