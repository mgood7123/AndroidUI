using AndroidUI.Extensions;
using AndroidUI.Input;
using AndroidUI.Utils.Input;
using SkiaSharp;

namespace AndroidUI.Widgets
{
    public class FlywheelScrollView : FrameLayout
    {
        private const int THRESH_HOLD = 90; // 90 ms

        Topten_RichTextKit_TextView text = new();
        Flywheel flywheel = new();

        bool first_draw = true;
        bool text_set;
        long last_time_previous, last_time_current, time;
        bool was_down;
        private bool smoothScroll;
        bool limitScrollingToViewBounds = true;

        public bool SmoothScroll { get => smoothScroll; set => smoothScroll = value; }
        public bool LimitScrollingToChildViewBounds { get => limitScrollingToViewBounds; set => limitScrollingToViewBounds = value; }

        public FlywheelScrollView() : base()
        {
            addView(text, MATCH_PARENT__MATCH_PARENT);
            text.setZ(float.PositiveInfinity);
            text.setText("FlywheelScrollView");
            setWillDraw(true);
        }

        public override bool onInterceptTouchEvent(Touch ev)
        {
            return true;
        }

        protected override void dispatchDraw(SKCanvas canvas)
        {
            base.dispatchDraw(canvas);
            if (first_draw)
            {
                first_draw = false;
                text.invalidate();
            }
            else
            {
                flywheel.AquireLock();
                if (text_set && flywheel.Spinning)
                {
                    text_set = false;
                    text.invalidate();
                }
                flywheel.ReleaseLock();
            }
        }

        enum RESULT
        {
            OK, CLAMP_X, CLAMP_Y, CLAMP_XY
        }

        RESULT NeedsClamp(View child, ref int x, ref int y)
        {
            RESULT r = RESULT.OK;
            Log.d("attempting to scroll to: " + x + ", " + y);
            if (limitScrollingToViewBounds)
            {
                if (x < 0)
                {
                    x = 0;
                    r = RESULT.CLAMP_X;
                }
                if (y < 0)
                {
                    y = 0;
                    if (r == RESULT.OK)
                    {
                        r = RESULT.CLAMP_Y;
                    }
                    else if (r == RESULT.CLAMP_X)
                    {
                        r = RESULT.CLAMP_XY;
                    }
                }
                if ((getMeasuredWidth() + x) > child.getMeasuredWidth())
                {
                    x = child.getMeasuredWidth() - getMeasuredWidth();
                    if (r == RESULT.OK)
                    {
                        r = RESULT.CLAMP_X;
                    }
                    else if (r == RESULT.CLAMP_Y)
                    {
                        r = RESULT.CLAMP_XY;
                    }
                }
                if ((getMeasuredHeight() + y) > child.getMeasuredHeight())
                {
                    y = child.getMeasuredHeight() - getMeasuredHeight();
                    if (r == RESULT.OK)
                    {
                        r = RESULT.CLAMP_Y;
                    }
                    else if (r == RESULT.CLAMP_X)
                    {
                        r = RESULT.CLAMP_XY;
                    }
                }
            }
            return r;
        }

        protected override void onDraw(SKCanvas canvas)
        {
            base.onDraw(canvas);
            flywheel.AquireLock();
            if (smoothScroll)
            {
                flywheel.Spin();
                if (flywheel.Spinning)
                {
                    invalidate();

                    if (getChildCount() == 2)
                    {
                        View view = getChildAt(1);
                        int x = -flywheel.Position.X.toPixel();
                        int y = -flywheel.Position.Y.toPixel();
                        RESULT r = NeedsClamp(view, ref x, ref y);
                        if (r != RESULT.OK)
                        {
                            flywheel.Position = new(-x, -y);
                            if (r == RESULT.CLAMP_X)
                            {
                                flywheel.Velocity = new(0, flywheel.Velocity.Y);
                            }
                            else if (r == RESULT.CLAMP_Y)
                            {
                                flywheel.Velocity = new(flywheel.Velocity.X, 0);
                            }
                            else if (r == RESULT.CLAMP_XY)
                            {
                                flywheel.Velocity = System.Numerics.Vector2.Zero;
                            }
                        }
                        view.scrollTo(x, y);
                    }
                }
            }
            string s = "Flywheel\n";
            s += "  Spinning: " + flywheel.Spinning + "\n";
            s += "  Friction: " + flywheel.Friction + "\n";
            s += "  Distance: \n";
            s += "    x: " + flywheel.Distance.X + " pixels\n";
            s += "    y: " + flywheel.Distance.Y + " pixels\n";
            s += "    time: " + time + " ms\n";
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
            text.setText(s);
            text_set = true;
            flywheel.ReleaseLock();
        }

        public override void onConfigureTouch(Touch touch)
        {
            touch.DontBatchOnTouchUpOrTouchCancel = true;
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
                    ResetButKeepPosition(data.timestamp);
                    Log.d("DOWN time: " + time);
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    was_down = true;
                    break;
                case Touch.State.TOUCH_MOVE:
                    // protect against batching, touch up emits a touch move if it's location has changed
                    last_time_previous = last_time_current;
                    last_time_current = data.timestamp;
                    time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                    Log.d("MOVE time: " + time);
                    if (!smoothScroll || was_down || time <= THRESH_HOLD)
                    {
                        was_down = false;
                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                        if (getChildCount() == 2)
                        {
                            View view = getChildAt(1);
                            int x = -flywheel.Position.X.toPixel();
                            int y = -flywheel.Position.Y.toPixel();
                            RESULT r = NeedsClamp(view, ref x, ref y);
                            if (r != RESULT.OK)
                            {
                                flywheel.Position = new(-x, -y);
                                if (r == RESULT.CLAMP_X)
                                {
                                    flywheel.Velocity = new(0, flywheel.Velocity.Y);
                                }
                                else if (r == RESULT.CLAMP_Y)
                                {
                                    flywheel.Velocity = new(flywheel.Velocity.X, 0);
                                }
                                else if (r == RESULT.CLAMP_XY)
                                {
                                    flywheel.Velocity = System.Numerics.Vector2.Zero;
                                }
                            }
                            view.scrollTo(x, y);
                        }
                    }
                    break;
                case Touch.State.TOUCH_UP:
                    last_time_previous = last_time_current;
                    last_time_current = data.timestamp;
                    time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                    Log.d("UP time: " + time);
                    if (smoothScroll && !was_down && time <= THRESH_HOLD)
                    {
                        if (getChildCount() == 2)
                        {
                            View view = getChildAt(1);
                            int x = -flywheel.Position.X.toPixel();
                            int y = -flywheel.Position.Y.toPixel();
                            RESULT r = NeedsClamp(view, ref x, ref y);
                            if (r != RESULT.OK)
                            {
                                flywheel.Position = new(-x, -y);
                                if (r == RESULT.CLAMP_X)
                                {
                                    flywheel.Velocity = new(0, flywheel.Velocity.Y);
                                }
                                else if (r == RESULT.CLAMP_Y)
                                {
                                    flywheel.Velocity = new(flywheel.Velocity.X, 0);
                                }
                                else if (r == RESULT.CLAMP_XY)
                                {
                                    flywheel.Velocity = System.Numerics.Vector2.Zero;
                                }
                            }
                            flywheel.FinalizeMovement();
                        }
                    }
                    else
                    {
                        was_down = false;
                    }
                    break;
                default:
                    break;
            }
            flywheel.ReleaseLock();
            text.invalidate();
            invalidate();
            return true;
        }

        private void Reset(long timestamp)
        {
            last_time_previous = 0;
            last_time_current = timestamp;
            time = 0;
            flywheel.Reset();
        }

        private void ResetButKeepPosition(long timestamp)
        {
            last_time_previous = 0;
            last_time_current = timestamp;
            time = 0;
            flywheel.ResetButKeepPosition();
        }

        public override void onBeforeAddView(View child, int index, View.LayoutParams layout_params)
        {
            if (getChildCount() == 2)
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

            //if (!mFillViewport)
            //{
            //    return;
            //}

            int heightMode = MeasureSpec.getMode(heightMeasureSpec);
            if (heightMode == MeasureSpec.UNSPECIFIED)
            {
                return;
            }

            if (getChildCount() > 1)
            {
                View child = getChildAt(1);
                int widthPadding;
                int heightPadding;
                //final int targetSdkVersion = getContext().getApplicationInfo().targetSdkVersion;
                FrameLayout.LayoutParams lp = (LayoutParams)child.getLayoutParams();
                //if (targetSdkVersion >= VERSION_CODES.M)
                //{
                    widthPadding = mPaddingLeft + mPaddingRight + lp.leftMargin + lp.rightMargin;
                    heightPadding = mPaddingTop + mPaddingBottom + lp.topMargin + lp.bottomMargin;
                //}
                //else
                //{
                //    widthPadding = mPaddingLeft + mPaddingRight;
                //    heightPadding = mPaddingTop + mPaddingBottom;
                //}

                int desiredWidth = getMeasuredWidth() - widthPadding;
                int desiredHeight = getMeasuredHeight() - heightPadding;
                if (child.getMeasuredHeight() < desiredHeight)
                {
                    int childWidthMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                            desiredWidth, MeasureSpec.EXACTLY);
                    int childHeightMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                            desiredHeight, MeasureSpec.EXACTLY);
                    child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
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
