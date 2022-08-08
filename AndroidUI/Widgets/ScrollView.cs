using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Input;
using AndroidUI.Utils;
using AndroidUI.Utils.Input;
using SkiaSharp;
using static SkiaSharp.HarfBuzz.SKShaper;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace AndroidUI.Widgets
{
    public class ScrollView : FrameLayout, Utils.Widgets.ScrollHost
    {
        Utils.Widgets.ScrollViewHostInstance host;

        public Utils.Widgets.ScrollViewHostInstance ScrollHostGetInstance() => host;

        public void ScrollHostOnSetWillDraw(bool smoothScroll)
        {
            setWillDraw(showDebugText || smoothScroll);
        }

        public void ScrollHostOnCancelled()
        {
            if (showDebugText)
            {
                text.invalidate();
            }
        }

        public bool ScrollHostHasChildrenToScroll()
        {
            return mChildrenCount == 2;
        }

        public int ScrollHostGetMeasuredWidth()
        {
            return getMeasuredWidth();
        }

        public int ScrollHostGetMeasuredHeight()
        {
            return getMeasuredHeight();
        }

        public int ScrollHostGetChildTotalMeasuredWidth()
        {
            return getChildAt(1).getMeasuredWidth();
        }

        public int ScrollHostGetChildTotalMeasuredHeight()
        {
            return getChildAt(1).getMeasuredHeight();
        }

        Topten_RichTextKit_TextView text = new();

        bool showDebugText;
        bool first_draw = true;
        bool text_set;

        public bool ShowDebugText
        {
            get => showDebugText;

            set
            {
                showDebugText = value;
                text.setVisibility(showDebugText ? VISIBLE : GONE);
                if (showDebugText)
                {
                    text.invalidate();
                }
                setWillDraw(showDebugText || host.SmoothScroll);
            }
        }

        public bool AutoScroll { get => host.autoScroll; set => host.autoScroll = value; }

        public bool SmoothScroll { get => host.SmoothScroll; set => host.SmoothScroll = value; }

        public bool LimitScrollingToChildViewBounds { get => host.limitScrollingToViewBounds; set => host.limitScrollingToViewBounds = value; }

        public ScrollView() : base()
        {
            host = new(this);

            addView(text, MATCH_PARENT_W__MATCH_PARENT_H);
            text.setZ(float.PositiveInfinity);
            text.setText("FlywheelScrollView");
            var c = new Graphics.Drawables.ColorDrawable(Graphics.Color.GRAY);
            c.setAlpha((0.25f).ToColorByte());
            text.setBackground(c);
            ShowDebugText = false;
            host.SmoothScroll = false;
        }

        public override void onConfigureTouch(Touch touch)
        {
            host.onConfigureTouch(touch);
        }

        public override bool onInterceptTouchEvent(Touch ev)
        {
            return host.InterceptTouch(this, t => base.onInterceptTouchEvent(t), getChildAt(1), ev);
        }

        protected override void dispatchDraw(SKCanvas canvas)
        {
            base.dispatchDraw(canvas);
            if (first_draw)
            {
                first_draw = false;
                if (showDebugText)
                {
                    text.invalidate();
                }
            }
            else
            {
                if (showDebugText)
                {
                    host.flywheel.AquireLock();
                    if (text_set && host.flywheel.Spinning)
                    {
                        text_set = false;
                        text.invalidate();
                    }
                    host.flywheel.ReleaseLock();
                }
            }
        }

        protected override void onDraw(SKCanvas canvas)
        {
            base.onDraw(canvas);
            host.flywheel.AquireLock();
            host.OnDraw(this, getChildAt(1));
            if (showDebugText)
            {
                string s = "Flywheel\n";
                s += "  Spinning: " + host.flywheel.Spinning + "\n";
                s += "  Friction: " + host.flywheel.Friction + "\n";
                s += "  Distance: \n";
                s += "    x: " + host.flywheel.Distance.X + " pixels\n";
                s += "    y: " + host.flywheel.Distance.Y + " pixels\n";
                s += "    time: " + host.Time + " ms\n";
                s += "  Total Distance: \n";
                s += "    x: " + host.flywheel.TotalDistance.X + " pixels\n";
                s += "    y: " + host.flywheel.TotalDistance.Y + " pixels\n";
                s += "    time: " + host.flywheel.SpinTime + " ms\n";
                s += "  Velocity: \n";
                s += "    x: " + host.flywheel.Velocity.X + "\n";
                s += "    y: " + host.flywheel.Velocity.Y + "\n";
                s += "  Position: \n";
                s += "    x: " + host.flywheel.Position.X + " pixels\n";
                s += "    y: " + host.flywheel.Position.Y + " pixels\n";
                text.setText(s);
                text_set = true;
            }
            host.flywheel.ReleaseLock();
        }

        public override void onBeforeAddView(View child, int index, View.LayoutParams layout_params)
        {
            if (mChildrenCount == 2)
            {
                throw new Exception("ScrollView can only have a single child");
            }
        }

        public override void removeViewAt(int index)
        {
            if (index == 0)
            {
                throw new Exception("ScrollView cannot remove internal text view");
            }
            base.removeViewAt(index);
        }

        public override void removeViews(int start, int count)
        {
            if (start == 0)
            {
                throw new Exception("ScrollView cannot remove internal text view");
            }
            base.removeViews(start, count);
        }

        public override void removeViewsInLayout(int start, int count)
        {
            if (start == 0)
            {
                throw new Exception("ScrollView cannot remove internal text view");
            }
            base.removeViewsInLayout(start, count);
        }

        protected override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.onMeasure(widthMeasureSpec, heightMeasureSpec);

            int heightMode = MeasureSpec.getMode(heightMeasureSpec);
            if (heightMode == MeasureSpec.UNSPECIFIED)
            {
                return;
            }

            if (mChildrenCount == 2)
            {
                View child = getChildAt(1);

                // remeasure child according to its layout params

                LayoutParams childLayoutParams = (LayoutParams)child.getLayoutParams();


                int widthPadding = mPaddingLeft + mPaddingRight + childLayoutParams.leftMargin + childLayoutParams.rightMargin;
                int heightPadding = mPaddingTop + mPaddingBottom + childLayoutParams.topMargin + childLayoutParams.bottomMargin;

                if (DEBUG_MEASURE_CHILD) Log.d("RE-MEASURING CHILD TO RESPECT THEIR LAYOUT PARAMETERS");
                int this_spec_w;
                switch (childLayoutParams.width)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_w = MeasureSpec.makeMeasureSpec(getMeasuredWidth(), MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size
                        this_spec_w = MeasureSpec.makeMeasureSpec(getMeasuredWidth(), MeasureSpec.UNSPECIFIED);
                        break;
                    default:
                        // child wants to be exact size
                        this_spec_w = MeasureSpec.makeMeasureSpec(getMeasuredWidth(), MeasureSpec.EXACTLY);
                        break;
                }
                int this_spec_h;
                switch (childLayoutParams.height)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_h = MeasureSpec.makeMeasureSpec(getMeasuredHeight(), MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size
                        this_spec_h = MeasureSpec.makeMeasureSpec(getMeasuredHeight(), MeasureSpec.UNSPECIFIED);
                        break;
                    default:
                        // child wants to be exact size
                        this_spec_h = MeasureSpec.makeMeasureSpec(getMeasuredHeight(), MeasureSpec.EXACTLY);
                        break;
                }
                int widthMeasureSpec1 = getChildMeasureSpec(this, child, true, this_spec_w, widthPadding, childLayoutParams.width);
                int heightMeasureSpec1 = getChildMeasureSpec(this, child, false, this_spec_h, heightPadding, childLayoutParams.height);
                child.measure(
                    widthMeasureSpec1,
                    heightMeasureSpec1
                );
                if (DEBUG_MEASURE_CHILD) Log.d("RE-MEASURED CHILD");

                if (AutoScroll)
                {
                    host.AutoScrollOnMeasure(this, child);
                    ShowDebugText = true;
                }
            }
        }

        protected override void measureChild(View child, int parentWidthMeasureSpec, int parentHeightMeasureSpec)
        {
            View.LayoutParams lp = child.getLayoutParams();

            int childWidthMeasureSpec;
            int childHeightMeasureSpec;

            int horizontalPadding = mPaddingLeft + mPaddingRight;
            int verticalPadding = mPaddingTop + mPaddingBottom;
            childWidthMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                    Math.Max(0, MeasureSpec.getSize(parentWidthMeasureSpec) - horizontalPadding),
                    MeasureSpec.UNSPECIFIED);
            childHeightMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                    Math.Max(0, MeasureSpec.getSize(parentHeightMeasureSpec) - verticalPadding),
                    MeasureSpec.UNSPECIFIED);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }

        protected override void measureChildWithMargins(View child, int parentWidthMeasureSpec, int widthUsed,
                                               int parentHeightMeasureSpec, int heightUsed)
        {
            MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

            int usedTotalW = getPaddingLeft() + getPaddingRight() + lp.leftMargin + lp.rightMargin +
                    widthUsed;
            int usedTotalH = getPaddingTop() + getPaddingBottom() + lp.topMargin + lp.bottomMargin +
                    heightUsed;
            int childWidthMeasureSpec = MeasureSpec.makeMeasureSpec(
                    Math.Max(0, MeasureSpec.getSize(parentWidthMeasureSpec) - usedTotalW),
                    MeasureSpec.UNSPECIFIED);
            int childHeightMeasureSpec = MeasureSpec.makeMeasureSpec(
                    Math.Max(0, MeasureSpec.getSize(parentHeightMeasureSpec) - usedTotalH),
                    MeasureSpec.UNSPECIFIED);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }
    }
}
