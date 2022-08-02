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

        bool showDebugText;

        bool first_draw = true;
        bool text_set;
        long last_time_previous, last_time_current, time;
        bool was_down;
        private bool smoothScroll;
        bool limitScrollingToViewBounds = true;

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

        public FlywheelScrollView() : base()
        {
            addView(text, MATCH_PARENT__MATCH_PARENT);
            text.setZ(float.PositiveInfinity);
            text.setText("FlywheelScrollView");
            var c = new Graphics.Drawables.ColorDrawable(Graphics.Color.GRAY);
            c.setAlpha((0.25f).ToColorByte());
            post(new Utils.Runnable.ActionRunnable(() => text.setBackground(c)));
            ShowDebugText = false;
            SmoothScroll = false;
        }

        bool mIsBeingDragged = false;
        float lastX;
        float lastY;

        public override bool onInterceptTouchEvent(Touch ev)
        {
            Log.d("INTERCEPT TOUCH");
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
                Log.d("INTERCEPT TOUCH MOVE ALREADY DRAGGING");
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
                Log.d("INTERCEPT TOUCH BASE");
                return true;
            }

            /*
             * Don't intercept if we have no child
             */
            if (getChildCount() == 1)
            {
                Log.d("INTERCEPT TOUCH NO CHILD");
                return false;
            }

            /*
             * Don't try to intercept touch if we can't scroll anyway.
             */
            View child = getChildAt(1);

            if (mScrollX == 0 && mScrollY == 0)
            {
                // if we get here then we HAVE NOT yet scrolled anywhere
                bool p = limitScrollingToViewBounds;
                limitScrollingToViewBounds = true;
                int x = 1;
                int y = 1;
                RESULT r = NeedsClamp(child, ref x, ref y);
                limitScrollingToViewBounds = p;
                if (r == RESULT.CLAMP_XY)
                {
                    Log.d("INTERCEPT TOUCH CANNOT SCROLL");
                    // if we get here, we cannot scroll in either X or Y
                    // in other words, we are larger than our child
                    return false;
                }
                Log.d("INTERCEPT TOUCH CAN SCROLL");
                // if we get here, we can scroll in either X or Y
                // in other words, our child is larger than us

                // we could either be a down followed be an up


                // or we could be a down followed by a move

                flywheel.AquireLock();

                switch (state)
                {
                    case Touch.State.TOUCH_DOWN:
                        Log.d("INTERCEPT TOUCH DOWN");
                        // remember last down
                        lastX = data.location.x;
                        lastY = data.location.y;

                        ResetButKeepPosition(data.timestamp);
                        flywheel.AddMovement(data.timestamp, lastX, lastY);
                        was_down = true;

                        mIsBeingDragged = flywheel.Spinning;
                        break;
                    case Touch.State.TOUCH_CANCELLED:
                        Log.d("INTERCEPT TOUCH CANCELLED");
                        ResetButKeepPosition(0);
                        was_down = false;
                        if (mIsBeingDragged)
                        {
                            mIsBeingDragged = false;
                            flywheel.ReleaseLock();
                            return true;
                        }
                        mIsBeingDragged = false;
                        break;
                    case Touch.State.TOUCH_UP:
                        {
                            Log.d("INTERCEPT TOUCH UP");
                            last_time_previous = last_time_current;
                            last_time_current = data.timestamp;
                            time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                            if (smoothScroll && !was_down && time <= THRESH_HOLD)
                            {
                                mIsBeingDragged = true;
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
                        }
                        break;
                    case Touch.State.TOUCH_MOVE:
                        {
                            Log.d("INTERCEPT TOUCH MOVE");
                            /*
                             * mIsBeingDragged == false, otherwise the shortcut would have caught it. Check
                             * whether the user has moved far enough from their original down touch.
                             */

                            float x_ = data.location.x;
                            float y_ = data.location.y;

                            float xDiff = MathF.Abs(x - lastX);
                            float yDiff = MathF.Abs(y - lastY);

                            if (xDiff > 30 || yDiff > 30)
                            {
                                last_time_previous = last_time_current;
                                last_time_current = data.timestamp;
                                time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                                if (!smoothScroll || was_down || time <= THRESH_HOLD)
                                {
                                    was_down = false;
                                    mIsBeingDragged = true;
                                    lastX = x_;
                                    lastY = y_;
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
                            }

                            break;
                        }
                }
                flywheel.ReleaseLock();
            }
            Log.d("INTERCEPT TOUCH IS BEING DRAGGED: " + mIsBeingDragged);
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
