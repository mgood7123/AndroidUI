/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.AnimationFramework.Animator;
using AndroidUI.Graphics;
using AndroidUI.Graphics.Drawables;
using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;
using AndroidUI.Widgets;
using SkiaSharp;
using static AndroidUI.Widgets.View;

namespace AndroidUI.Applications
{
    public partial class TestApp : Application
    {
        const string image_path = "K:/DESKTOP_BACKUP/Documents/2021-07-25 22.37.22.jpg";
        const string image_path_NPatch = "K:/Images/9PNG/circle.9.png";

        public override void OnCreate()
        {
            TabView tabView = new();

            tabView.addTab("Flywheel", () => new FlywheelView());

            tabView.addTab("View", () => new View());

            tabView.addTab("Touch Info Linear Layout", () =>
            {
                LinearLayout linearLayout = new();

                linearLayout.setOrientation(LinearLayout.OrientationMode.VERTICAL);

                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                linearLayout.addView(new TouchInfoView(), new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1.0f));
                return linearLayout;
            });

            tabView.addTab("Bitmap", () => new A());

            tabView.addTab("NinePatch Bitmap", () =>
            {
                var image = new ImageView();
                image.setScaleType(ImageView.ScaleType.FIT_XY);
                var bm = BitmapFactory.decodeFile(Context, image_path_NPatch);
                var chunks = bm?.getNinePatchChunk();
                if (NinePatch.isNinePatchChunk(chunks))
                {
                    Log.d("NINEPATCH", "IS CHUNK YES");
                    image.setImageDrawable(new NinePatchDrawable(bm, chunks, new Rect(), null));
                } else
                {
                    Log.d("NINEPATCH", "IS CHUNK NO");
                    image.setImageBitmap(bm);
                }
                return image;
            });

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
            tabView.addTab("Layered ColorView", () =>
            {
                FrameLayout f = new FrameLayout();
                View newColorView()
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
                }

                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                f.addView(newColorView(), MATCH_PARENT_W__MATCH_PARENT_H);
                return f;
            });

            tabView.addTab("Animation", () => 
            {
                var color = new ColorView(Color.MAGENTA);

                ObjectAnimator ox = ObjectAnimator.ofInt(Context, color, "x", new int[] { 0, 60 })
                    .setDuration(1100);
                ObjectAnimator ox_ = ObjectAnimator.ofInt(Context, color, "x", new int[] { 60, 100 })
                    .setDuration(400);
                ObjectAnimator rx = ObjectAnimator.ofInt(Context, color, "x", new int[] { 100, 0 })
                    .setDuration(1100/2);

                ObjectAnimator oy = ObjectAnimator.ofInt(Context, color, "y", new int[] { 0, 100 })
                    .setDuration(1100);
                ObjectAnimator oy_ = ObjectAnimator.ofInt(Context, color, "y", new int[] { 100, 200 })
                    .setDuration(900);
                ObjectAnimator ry = ObjectAnimator.ofInt(Context, color, "y", new int[] { 200, 0 })
                    .setDuration(1100/2);

                AnimatorSet A = new(Context);
                AnimatorSet B = new(Context);
                AnimatorSet C = new(Context);

                A.playTogether(ox, oy);
                B.playTogether(rx, ry);
                C.playSequentially(A, ox_, oy_, B);

                C.addListener(new AL());
                C.addViewAttachmentListener(color);

                return color;
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

                linearLayout.setOrientation(LinearLayout.OrientationMode.VERTICAL);

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

            tabView.addTab("Scrolling (Unbounded)", () =>
            {
                var image = new ImageView();
                //image.setScaleType(ImageView.ScaleType.CENTER_INSIDE);
                var bm = BitmapFactory.decodeFile(Context, image_path);
                image.setImageBitmap(bm);

                var s = new ScrollView();
                s.SmoothScroll = false;
                s.LimitScrollingToChildViewBounds = false;
                s.addView(image);
                return s;
            });

            tabView.addTab("Scrolling 2 (Bounded)", () =>
            {
                var image = new ImageView();
                var bm = BitmapFactory.decodeFile(Context, image_path);
                image.setImageBitmap(bm);
                image.setScaleType(ImageView.ScaleType.MATRIX);

                var scrollView = new ScrollView();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;
                scrollView.addView(image);
                return scrollView;
            });

            tabView.addTab("Scrolling 2 (Unbounded)", () =>
            {
                var image = new ImageView();
                var bm = BitmapFactory.decodeFile(Context, image_path);
                image.setImageBitmap(bm);
                image.setScaleType(ImageView.ScaleType.MATRIX);

                var s = new ScrollView();
                s.ShowDebugText = true;
                s.SmoothScroll = true;
                s.LimitScrollingToChildViewBounds = false;
                s.addView(image);
                return s;
            });

            tabView.addTab("Scrolling 3 (large, clickable)", () =>
            {
                ScrollView scrollView = new();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;

                ColorView colorView = new ColorView();
                colorView.setOnClickListener(v =>
                {
                    colorView.Color = new SKColor(
                        (byte)Random.Shared.Next(255),
                        (byte)Random.Shared.Next(255),
                        (byte)Random.Shared.Next(255)
                    );
                });

                scrollView.addView(colorView, new LayoutParams(100000, 100000));

                return scrollView;
            });

            tabView.addTab("Scrolling 3 (large, non-clickable)", () =>
            {
                ScrollView scrollView = new();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;

                ColorView colorView = new ColorView();

                scrollView.addView(colorView, new LayoutParams(100000, 100000));

                return scrollView;
            });

            tabView.addTab("Scrolling 3 (large, HORIZONTAL)", () =>
            {
                ScrollView scrollView = new();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;
                scrollView.LimitScrollingToChildViewBounds = true;

                ColorView colorView = new ColorView();

                scrollView.addView(colorView, new LayoutParams(100000, 50));

                return scrollView;
            });

            tabView.addTab("Scrolling 3 (large, VERTICAL)", () =>
            {
                ScrollView scrollView = new();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;
                scrollView.LimitScrollingToChildViewBounds = true;

                ColorView colorView = new ColorView();

                scrollView.addView(colorView, new LayoutParams(50, 100000));

                return scrollView;
            });

            tabView.addTab("Gravity", () =>
            {
                FrameLayout frame = new();
                frame.addView(
                    new Topten_RichTextKit_TextView("Left|Top"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.LEFT | Gravity.TOP
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Top|Center"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.TOP | Gravity.CENTER
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Top|Right"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.TOP | Gravity.RIGHT
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Right|Center"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.RIGHT | Gravity.CENTER
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Right|Bottom"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.RIGHT | Gravity.BOTTOM
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Bottom|Center"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.BOTTOM | Gravity.CENTER
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Bottom|Left"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.BOTTOM | Gravity.LEFT
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Left|Center"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.LEFT | Gravity.CENTER
                    )
                );
                frame.addView(
                    new Topten_RichTextKit_TextView("Center"),
                    new FrameLayout.LayoutParams(
                        View.LayoutParams.WRAP_CONTENT,
                        View.LayoutParams.WRAP_CONTENT,
                        Gravity.CENTER
                    )
                );
                return frame;
            });

            tabView.addTab("Scrolling Buttons", () =>
            {
                ScrollView scrollView = new();
                scrollView.ShowDebugText = true;
                scrollView.SmoothScroll = true;
                scrollView.LimitScrollingToChildViewBounds = true;

                LinearLayout list = new();
                list.setOrientation(LinearLayout.OrientationMode.VERTICAL);

                for (int i = 0; i < 50; i++)
                {
                    AddButton((p, b) => p.addView(b, MATCH_PARENT_W__WRAP_CONTENT_H), list, i +1);
                }

                scrollView.addView(list, MATCH_PARENT_W__WRAP_CONTENT_H);

                return scrollView;
            });

            tabView.addTab("ListView Buttons", () =>
            {
                AndroidUI.Widgets.ListView list = new();
                for (int i = 0; i < 100; i++)
                {
                    AddButton((p, b) => p.addView(b, MATCH_PARENT_W__WRAP_CONTENT_H), list, i + 1);
                }
                return list;
            });

            tabView.addTab("RecyclingListView Buttons", () =>
            {
                AdapterListView list = new();
                AdapterListView.Adapter adapter = new();
                list.setAdapter(adapter);
                for (int i = 0; i < 100; i++)
                {
                    AddButton((p, b) => p.views.Add(b), adapter, i + 1);
                }
                return list;
            });

            SetContentView(tabView);
        }

        void AddButton<T>(AndroidUI.Utils.Runnable<T, View> runnable, T parent, int n)
        {
            var button = new Topten_RichTextKit_TextView("Button " + n, 24, SkiaSharp.SKColors.Black);

            FrameLayout frame = new();
            frame.addView(
                button,
                new FrameLayout.LayoutParams(
                    View.LayoutParams.WRAP_CONTENT,
                    View.LayoutParams.WRAP_CONTENT,
                    Gravity.CENTER
                )
            );
            frame.setBackgroundColor((int)(uint)AndroidUI.Utils.Const.Constants.color_code_LineageOS);
            frame.setOnClickListener(v => v.Log.d("Clicked button " + n));
            frame.setTagRecursively(button.getText());
            runnable.Invoke(parent, frame);
        }
    }
}
