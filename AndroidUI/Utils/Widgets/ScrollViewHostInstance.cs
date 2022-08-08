using AndroidUI.Extensions;
using AndroidUI.Input;
using AndroidUI.Utils.Input;
using AndroidUI.Widgets;

namespace AndroidUI.Utils.Widgets
{
    public class ScrollViewHostInstance
    {
        ScrollHost host;

        public ScrollViewHostInstance(ScrollHost host)
        {
            this.host = host;
        }

        /// <summary>
        /// the maximum amount of time that the scroll host will allow for a movement to be considered a scroll
        /// </summary>
        public const int THRESH_HOLD = 100; // 100 ms

        public readonly Flywheel flywheel = new();

        bool mIsBeingDragged = false;

        long last_time_previous, last_time_current, time;
        bool was_down;
        private bool smoothScroll;
        public bool limitScrollingToViewBounds = true;
        public bool autoScroll;
        private int lastY;

        bool has_been_dragged;
        bool has_been_autoscrolled;
        public bool SmoothScroll
        {
            get => smoothScroll; set
            {
                smoothScroll = value;
                host.ScrollHostOnSetWillDraw(smoothScroll);
            }
        }
        public bool LimitScrollingToChildViewBounds { get => limitScrollingToViewBounds; set => limitScrollingToViewBounds = value; }
        public long Time => time;

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

        enum RESULT
        {
            OK, CLAMP_X, CLAMP_Y, CLAMP_XY
        }

        RESULT NeedsClamp(ref int x, ref int y)
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
                int width = host.ScrollHostGetMeasuredWidth();
                int childWidth = host.ScrollHostGetChildTotalMeasuredWidth();
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
                int height = host.ScrollHostGetMeasuredHeight();
                int childHeight = host.ScrollHostGetChildTotalMeasuredHeight();
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

        public bool InterceptTouch(View this_view, RunnableWithReturn<Touch, bool> base_onInterceptTouchEvent, View target_view, Touch ev)
        {
            if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH");
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
                if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH MOVE ALREADY DRAGGING");
                flywheel.AquireLock();
                last_time_previous = last_time_current;
                last_time_current = data.timestamp;
                time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                if (!smoothScroll || was_down || time > 0)
                {
                    was_down = false;
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    if (host.ScrollHostHasChildrenToScroll())
                    {
                        int x = -flywheel.Position.X.toPixel();
                        int y = -flywheel.Position.Y.toPixel();
                        RESULT r = NeedsClamp(ref x, ref y);
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
                        target_view.scrollTo(x, y);
                    }
                }
                flywheel.ReleaseLock();
                return true;
            }

            if (base_onInterceptTouchEvent.Invoke(ev))
            {
                if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH BASE");
                return true;
            }

            /*
             * Don't intercept if we have no child
             */
            if (!host.ScrollHostHasChildrenToScroll())
            {
                if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH NO CHILD");
                return false;
            }

            /*
             * Don't try to intercept touch if we can't scroll anyway.
             */
            if (this_view.mScrollX == 0 && this_view.mScrollY == 0)
            {
                // if we get here then we HAVE NOT yet scrolled anywhere
                if (limitScrollingToViewBounds)
                {
                    int x = 1;
                    int y = 1;
                    RESULT r = NeedsClamp(ref x, ref y);
                    if (r == RESULT.CLAMP_XY)
                    {
                        if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH CANNOT SCROLL");
                        // if we get here, we cannot scroll in either X or Y
                        // in other words, we are larger than our child
                        // and we are not allowed to scroll past our child
                        return false;
                    }
                }
            }
            if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH CAN SCROLL");
            // if we get here, we can scroll in either X or Y
            // in other words, our child is larger than us
            // or we are allowed to scroll past our child

            // we could either be a down followed be an up
            // or we could be a down followed by a move

            flywheel.AquireLock();

            switch (state)
            {
                case Touch.State.TOUCH_DOWN:
                    if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH DOWN");
                    ResetButKeepPosition(data.timestamp);
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    was_down = true;

                    mIsBeingDragged = flywheel.Spinning;
                    break;
                case Touch.State.TOUCH_CANCELLED:
                    if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH CANCELLED");
                    ResetButKeepPosition(0);
                    was_down = false;
                    if (mIsBeingDragged)
                    {
                        mIsBeingDragged = false;
                        flywheel.ReleaseLock();
                        host.ScrollHostOnCancelled();
                        return true;
                    }
                    mIsBeingDragged = false;
                    host.ScrollHostOnCancelled();
                    break;
                case Touch.State.TOUCH_UP:
                    // do not intercept up
                    {
                        if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH UP");
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
                            RESULT r2 = NeedsClamp(ref x2, ref y2);
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
                        host.ScrollHostOnCancelled();
                    }
                    break;
                case Touch.State.TOUCH_MOVE:
                    {
                        if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH MOVE");
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
                            RESULT r2 = NeedsClamp(ref x2, ref y2);
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
                            target_view.scrollTo(x2, y2);

                            var parent = this_view.getParent();
                            if (parent != null)
                            {
                                parent.requestDisallowInterceptTouchEvent(true);
                            }
                        }

                        break;
                    }
                    flywheel.ReleaseLock();
            }
            if (View.DEBUG) this_view.Log.d("INTERCEPT TOUCH IS BEING DRAGGED: " + mIsBeingDragged);
            return mIsBeingDragged;
        }

        public void OnDraw(View this_view, View target_view)
        {
            if (smoothScroll)
            {
                flywheel.Spin();
                if (flywheel.Spinning)
                {
                    this_view.invalidate();

                    if (host.ScrollHostHasChildrenToScroll())
                    {
                        int x = -flywheel.Position.X.toPixel();
                        int y = -flywheel.Position.Y.toPixel();
                        RESULT r = NeedsClamp(ref x, ref y);
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
                        target_view.scrollTo(x, y);
                    }
                }
            }
        }

        // protect against batching, touch up emits a touch move if it's location has changed
        // this can cause a slight decrease in velocity upon lifting up
        public void onConfigureTouch(Touch touch)
        {
            touch.DontBatchOnTouchUpOrTouchCancel = true;
        }

        internal void AutoScrollOnMeasure(View this_view, View target_view)
        {
            if (View.DEBUG) this_view.Log.d("Auto-scrolling");
            flywheel.AquireLock();
            var scrollX = target_view.mScrollX;
            var scrollY = target_view.mScrollY;

            int x_ = 1;
            int y_ = 1;
            RESULT r = NeedsClamp(ref x_, ref y_);
            if (r == RESULT.CLAMP_XY || r == RESULT.CLAMP_Y)
            {
                // we cannot scroll down
            }
            else
            {

                var x = host.ScrollHostGetChildTotalMeasuredWidth() - host.ScrollHostGetMeasuredHeight();
                int y = host.ScrollHostGetChildTotalMeasuredHeight() - host.ScrollHostGetMeasuredHeight();

                if (target_view.mScrollY == lastY || lastY == 0)
                {
                    if (View.DEBUG) this_view.Log.d("lastY: " + lastY);
                    if (View.DEBUG) this_view.Log.d("target_view.mScrollY: " + target_view.mScrollY);

                    int y1 = flywheel.Position.Y.toPixel();
                    if (View.DEBUG) this_view.Log.d("Auto-scrolling before add movement: " + y1);

                    if (has_been_dragged)
                    {
                        if (View.DEBUG) this_view.Log.d("the user manually scrolled to this point");
                        if (target_view.mScrollY == lastY)
                        {
                            has_been_dragged = false;
                            has_been_autoscrolled = false;
                        }
                    }
                    else
                    {
                        if (View.DEBUG) this_view.Log.d("the user did not manually scroll to this point");
                        lastY = y;
                        flywheel.AddMovement(0, -flywheel.Position.X, 0);
                        flywheel.AddMovement(0, -flywheel.Position.X, -y);
                        int x2 = -flywheel.Position.X.toPixel();
                        int y2 = -flywheel.Position.Y.toPixel();
                        if (View.DEBUG) this_view.Log.d("Auto-scrolling before clamp: " + y2);
                        r = NeedsClamp(ref x2, ref y2);
                        if (View.DEBUG) this_view.Log.d("Auto-scrolling after clamp:  " + y2);
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
                        if (View.DEBUG) this_view.Log.d("Auto-scrolling to " + y2);
                        target_view.scrollTo(scrollX, y2);
                        has_been_dragged = false;
                        has_been_autoscrolled = true;
                    }
                }
            }
            flywheel.ReleaseLock();
        }
    }
}