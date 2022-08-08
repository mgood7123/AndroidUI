using AndroidUI.Extensions;
using AndroidUI.Input;
using AndroidUI.Utils.Input;
using SkiaSharp;

namespace AndroidUI.Widgets
{
    public class FlywheelScrollView : FrameLayout
    {
        private const int THRESH_HOLD = 100; // 100 ms

        Topten_RichTextKit_TextView text = new();
        Flywheel flywheel = new();

        bool showDebugText;
        bool mIsBeingDragged = false;

        bool first_draw = true;
        bool text_set;
        long last_time_previous, last_time_current, time;
        bool was_down;
        private bool smoothScroll;
        bool limitScrollingToViewBounds = true;
        private bool autoScroll;
        private int lastY;

        bool has_been_dragged;
        bool has_been_autoscrolled;

        public bool SmoothScroll
        {
            get => smoothScroll; set
            {
                smoothScroll = value;
                setWillDraw(showDebugText || smoothScroll);
            }
        }
        public bool LimitScrollingToChildViewBounds { get => limitScrollingToViewBounds; set => limitScrollingToViewBounds = value; }

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
                setWillDraw(showDebugText || smoothScroll);
            }
        }

        public bool AutoScroll { get => autoScroll; set => autoScroll = value; }

        public FlywheelScrollView() : base()
        {
            addView(text, MATCH_PARENT_W__MATCH_PARENT_H);
            text.setZ(float.PositiveInfinity);
            text.setText("FlywheelScrollView");
            var c = new Graphics.Drawables.ColorDrawable(Graphics.Color.GRAY);
            c.setAlpha((0.25f).ToColorByte());
            text.setBackground(c);
            ShowDebugText = false;
            SmoothScroll = false;
        }

        public override bool onInterceptTouchEvent(Touch ev)
        {
            if (DEBUG) Log.d("INTERCEPT TOUCH");
            /*
             * This method JUST determines whether we want to intercept the motion.
             * If we return true, onMotionEvent will be called and we do the actual
             * scrolling there.
             */

            /*
            * Shortcut the most recurring case: the user is in the dragging
            * state and they is moving their finger.  We want to intercept this
            * motion.
            */
            var data = ev.getTouchAtCurrentIndex();
            var state = data.state;
            if ((state == Touch.State.TOUCH_MOVE) && (mIsBeingDragged))
            {
                if (DEBUG) Log.d("INTERCEPT TOUCH MOVE ALREADY DRAGGING");
                flywheel.AquireLock();
                last_time_previous = last_time_current;
                last_time_current = data.timestamp;
                time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                if (!smoothScroll || was_down || time > 0)
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
                flywheel.ReleaseLock();
                return true;
            }

            if (base.onInterceptTouchEvent(ev))
            {
                if (DEBUG) Log.d("INTERCEPT TOUCH BASE");
                return true;
            }

            /*
             * Don't intercept if we have no child
             */
            if (getChildCount() == 1)
            {
                if (DEBUG) Log.d("INTERCEPT TOUCH NO CHILD");
                return false;
            }

            /*
             * Don't try to intercept touch if we can't scroll anyway.
             */
            View child = getChildAt(1);

            if (mScrollX == 0 && mScrollY == 0)
            {
                // if we get here then we HAVE NOT yet scrolled anywhere
                if (limitScrollingToViewBounds)
                {
                    int x = 1;
                    int y = 1;
                    RESULT r = NeedsClamp(child, ref x, ref y);
                    if (r == RESULT.CLAMP_XY)
                    {
                        if (DEBUG) Log.d("INTERCEPT TOUCH CANNOT SCROLL");
                        // if we get here, we cannot scroll in either X or Y
                        // in other words, we are larger than our child
                        // and we are not allowed to scroll past our child
                        return false;
                    }
                }
                if (DEBUG) Log.d("INTERCEPT TOUCH CAN SCROLL");
                // if we get here, we can scroll in either X or Y
                // in other words, our child is larger than us
                // or we are allowed to scroll past our child

                // we could either be a down followed be an up


                // or we could be a down followed by a move

                flywheel.AquireLock();

                switch (state)
                {
                    case Touch.State.TOUCH_DOWN:
                        if (DEBUG) Log.d("INTERCEPT TOUCH DOWN");
                        ResetButKeepPosition(data.timestamp);
                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                        was_down = true;

                        mIsBeingDragged = flywheel.Spinning;
                        break;
                    case Touch.State.TOUCH_CANCELLED:
                        if (DEBUG) Log.d("INTERCEPT TOUCH CANCELLED");
                        ResetButKeepPosition(0);
                        was_down = false;
                        if (mIsBeingDragged)
                        {
                            mIsBeingDragged = false;
                            flywheel.ReleaseLock();
                            return true;
                        }
                        mIsBeingDragged = false;
                        if (showDebugText)
                        {
                            text.invalidate();
                        }
                        break;
                    case Touch.State.TOUCH_UP:
                            // do not intercept up
                        {
                            if (DEBUG) Log.d("INTERCEPT TOUCH UP");
                            last_time_previous = last_time_current;
                            last_time_current = data.timestamp;
                            time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                            if (smoothScroll && !was_down && time <= THRESH_HOLD)
                            {
                                mIsBeingDragged = true;
                                has_been_dragged = true;
                                has_been_autoscrolled = false;
                                int x2 = -flywheel.Position.X.toPixel();
                                int y2 = -flywheel.Position.Y.toPixel();
                                RESULT r2 = NeedsClamp(child, ref x2, ref y2);
                                if (r2 != RESULT.OK)
                                {
                                    flywheel.Position = new(-x2, -y2);
                                    if (r2 == RESULT.CLAMP_X)
                                    {
                                        flywheel.Velocity = new(0, flywheel.Velocity.Y);
                                    }
                                    else if (r2 == RESULT.CLAMP_Y)
                                    {
                                        flywheel.Velocity = new(flywheel.Velocity.X, 0);
                                    }
                                    else if (r2 == RESULT.CLAMP_XY)
                                    {
                                        flywheel.Velocity = System.Numerics.Vector2.Zero;
                                    }
                                }
                                flywheel.FinalizeMovement();
                            }
                            else
                            {
                                ResetButKeepPosition(0);
                                was_down = false;
                                if (mIsBeingDragged)
                                {
                                    mIsBeingDragged = false;
                                    flywheel.ReleaseLock();
                                    return true;
                                }
                                mIsBeingDragged = false;
                            }
                            if (showDebugText)
                            {
                                text.invalidate();
                            }
                        }
                        break;
                    case Touch.State.TOUCH_MOVE:
                        {
                            if (DEBUG) Log.d("INTERCEPT TOUCH MOVE");
                            /*
                             * mIsBeingDragged == false, otherwise the shortcut would have caught it. Check
                             * whether the user has moved far enough from their original down touch.
                             */

                            float x_ = data.location.x;
                            float y_ = data.location.y;

                            last_time_previous = last_time_current;
                            last_time_current = data.timestamp;
                            time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                            if (!smoothScroll || was_down || time <= THRESH_HOLD)
                            {
                                was_down = false;
                                mIsBeingDragged = true;
                                has_been_dragged = true;
                                has_been_autoscrolled = false;
                                flywheel.AddMovement(last_time_current, x_, y_);
                                int x2 = -flywheel.Position.X.toPixel();
                                int y2 = -flywheel.Position.Y.toPixel();
                                RESULT r2 = NeedsClamp(child, ref x2, ref y2);
                                if (r2 != RESULT.OK)
                                {
                                    flywheel.Position = new(-x2, -y2);
                                    if (r2 == RESULT.CLAMP_X)
                                    {
                                        flywheel.Velocity = new(0, flywheel.Velocity.Y);
                                    }
                                    else if (r2 == RESULT.CLAMP_Y)
                                    {
                                        flywheel.Velocity = new(flywheel.Velocity.X, 0);
                                    }
                                    else if (r2 == RESULT.CLAMP_XY)
                                    {
                                        flywheel.Velocity = System.Numerics.Vector2.Zero;
                                    }
                                }
                                child.scrollTo(x2, y2);

                                var parent = getParent();
                                if (parent != null)
                                {
                                    parent.requestDisallowInterceptTouchEvent(true);
                                }
                            }

                            break;
                        }
                }
                flywheel.ReleaseLock();
            }
            if (DEBUG) Log.d("INTERCEPT TOUCH IS BEING DRAGGED: " + mIsBeingDragged);
            return mIsBeingDragged;
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
                    flywheel.AquireLock();
                    if (text_set && flywheel.Spinning)
                    {
                        text_set = false;
                        text.invalidate();
                    }
                    flywheel.ReleaseLock();
                }
            }
        }

        enum RESULT
        {
            OK, CLAMP_X, CLAMP_Y, CLAMP_XY
        }

        RESULT NeedsClamp(View child, ref int x, ref int y)
        {
            RESULT r = RESULT.OK;
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
                int width = getMeasuredWidth();
                int childWidth = child.getMeasuredWidth();
                if (childWidth > width)
                {
                    if ((width + x) > childWidth)
                    {
                        x = childWidth - width;
                        if (r == RESULT.OK)
                        {
                            r = RESULT.CLAMP_X;
                        }
                        else if (r == RESULT.CLAMP_Y)
                        {
                            r = RESULT.CLAMP_XY;
                        }
                    }
                }
                else
                {
                    // child is same size or smaller than us, dont scroll horizontally
                    x = 0;
                    if (r == RESULT.OK)
                    {
                        r = RESULT.CLAMP_X;
                    }
                    else if (r == RESULT.CLAMP_Y)
                    {
                        r = RESULT.CLAMP_XY;
                    }
                }
                int height = getMeasuredHeight();
                int childHeight = child.getMeasuredHeight();
                if (childHeight > height)
                {
                    if ((height + y) > childHeight)
                    {
                        y = childHeight - height;
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
                else
                {
                    // child is same size or smaller than us, dont scroll vertically
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
            if (showDebugText)
            {
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
            }
            flywheel.ReleaseLock();
        }

        public override void onConfigureTouch(Touch touch)
        {
            // protect against batching, touch up emits a touch move if it's location has changed
            // this can cause a slight decrease in velocity upon lifting up
            touch.DontBatchOnTouchUpOrTouchCancel = true;
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

            int heightMode = MeasureSpec.getMode(heightMeasureSpec);
            if (heightMode == MeasureSpec.UNSPECIFIED)
            {
                return;
            }

            if (getChildCount() == 2)
            {
                View child = getChildAt(1);
                int widthPadding;
                int heightPadding;

                // remeasure child according to its layout params

                LayoutParams childLayoutParams = (LayoutParams)child.getLayoutParams();


                widthPadding = mPaddingLeft + mPaddingRight + childLayoutParams.leftMargin + childLayoutParams.rightMargin;
                heightPadding = mPaddingTop + mPaddingBottom + childLayoutParams.topMargin + childLayoutParams.bottomMargin;

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
                    if (DEBUG) Log.d("Auto-scrolling");
                    flywheel.AquireLock();
                    var scrollX = child.mScrollX;
                    var scrollY = child.mScrollY;

                    int x_ = 1;
                    int y_ = 1;
                    RESULT r = NeedsClamp(child, ref x_, ref y_);
                    if (r == RESULT.CLAMP_XY || r == RESULT.CLAMP_Y)
                    {
                        // we cannot scroll down
                    }
                    else
                    {

                        var x = child.getMeasuredWidth() - getMeasuredHeight();
                        int y = child.getMeasuredHeight() - getMeasuredHeight();

                        if (child.mScrollY == lastY || lastY == 0)
                        {
                            Log.d("lastY: " + lastY);
                            Log.d("child.mScrollY: " + child.mScrollY);

                            int y1 = flywheel.Position.Y.toPixel();
                            Log.d("Auto-scrolling before add movement: " + y1);

                            if (has_been_dragged)
                            {
                                Log.d("the user manually scrolled to this point");
                                if (mScrollX == lastY)
                                {
                                    has_been_dragged = false;
                                    has_been_autoscrolled = false;
                                }
                            }
                            else
                            {
                                Log.d("the user did not manually scroll to this point");
                                lastY = y;
                                flywheel.AddMovement(0, -flywheel.Position.X, 0);
                                flywheel.AddMovement(0, -flywheel.Position.X, -y);
                                int x2 = -flywheel.Position.X.toPixel();
                                int y2 = -flywheel.Position.Y.toPixel();
                                Log.d("Auto-scrolling before clamp: " + y2);
                                r = NeedsClamp(child, ref x2, ref y2);
                                Log.d("Auto-scrolling after clamp:  " + y2);
                                if (r != RESULT.OK)
                                {
                                    flywheel.Position = new(-x2, -y2);
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
                                Log.d("Auto-scrolling to " + y2);
                                child.scrollTo(scrollX, y2);
                                has_been_dragged = false;
                                has_been_autoscrolled = true;
                            }
                        }

                        ShowDebugText = true;
                    }
                    flywheel.ReleaseLock();
                }

                //int desiredWidth = getMeasuredWidth() - widthPadding;
                //int desiredHeight = getMeasuredHeight() - heightPadding;

                //int childWidth = child.getMeasuredWidth();
                //int childHeight = child.getMeasuredHeight();

                //if (childHeight < desiredHeight)
                //{
                //    int childWidthMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                //            desiredWidth, MeasureSpec.EXACTLY);
                //    int childHeightMeasureSpec = MeasureSpec.makeSafeMeasureSpec(
                //            desiredHeight, MeasureSpec.EXACTLY);
                //    child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
                //}
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
