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
                // this should be enough to determine if the user
                // can actually scroll in the case of adapter-like views and non-adapter-like views
                //
                // when dealing with an adapter-like view, the host needs to determine if
                // it can scroll based on its current scroll position and adapter position
                //
                // specifically to deal with zero-length buffered adapter-like views, the host view
                // must return a child bounds of a virtual buffer of 1 only if it's actual buffer is zero size
                //
                // in the case of a non-adapter-like view, the host need only deal with it's child bounds
                // and return `true` from `CanScroll*`
                //
                if (host.ScrollHostCanScrollLeftOrUp())
                {
                    int left = host.ScrollHostGetChildLeft();
                    int top = host.ScrollHostGetChildTop();
                    if (x < left)
                    {
                        x = left;
                        r = RESULT.CLAMP_X;
                    }
                    if (y < top)
                    {
                        y = top;
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
                if (host.ScrollHostCanScrollRightOrDown())
                {
                    int right = host.ScrollHostGetChildRight();
                    int bottom = host.ScrollHostGetChildBottom();

                    int width = host.ScrollHostGetMeasuredWidth();
                    if ((width + x) > right)
                    {
                        x = right - width;
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
                    if ((height + y) > bottom)
                    {
                        y = bottom - height;
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
            }
            return r;
        }

        public bool InterceptTouch(View this_view, RunnableWithReturn<Touch, bool> base_onInterceptTouchEvent, View target_view, Touch ev)
        {
            if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH");

            var data = ev.getTouchAtCurrentIndex();
            var state = data.state;

            /*
            * Shortcut the most recurring case: the user is in the dragging
            * state and they is moving their finger.  We want to intercept this
            * motion.
            */
            if ((state == Touch.State.TOUCH_MOVE) && (mIsBeingDragged))
            {
                if (host.ScrollHostHasChildrenToScroll())
                {
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH MOVE ALREADY DRAGGING");
                    flywheel.AquireLock();
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    DoScroll(target_view);
                    flywheel.ReleaseLock();
                }
                return true;
            }

            if (base_onInterceptTouchEvent.Invoke(ev))
            {
                if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH BASE");
                return true;
            }

            /*
             * Don't intercept if we have no child
             */
            if (!host.ScrollHostHasChildrenToScroll())
            {
                if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH NO CHILD");
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
                    bool can_scroll_down_or_right, can_scroll_up_or_left;
                    CanScroll(out can_scroll_down_or_right, out can_scroll_up_or_left);

                    if (!can_scroll_down_or_right && !can_scroll_up_or_left)
                    {
                        // if we get here, we cannot scroll in either X or Y
                        // in other words, we are larger than (or same size as) our child
                        // and we are not allowed to scroll past our child
                        if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH CANNOT SCROLL");
                        return false;
                    }
                }
            }
            if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH CAN SCROLL");
            // if we get here, we can scroll in either X or Y
            // in other words, our child is larger than us
            // or we are allowed to scroll past our child

            // we could either be a down followed be an up
            // or we could be a down followed by a move


            flywheel.AquireLock();

            switch (state)
            {
                case Touch.State.TOUCH_CANCELLED:
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH CANCELLED");
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
                case Touch.State.TOUCH_DOWN:
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH DOWN");
                    ResetButKeepPosition(data.timestamp);
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    was_down = true;

                    mIsBeingDragged = flywheel.Spinning;
                    break;
                case Touch.State.TOUCH_MOVE:
                    {
                        if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH MOVE");

                        /*
                         * mIsBeingDragged == false, otherwise the shortcut would have caught it. Check
                         * whether the user has moved far enough from their original down touch.
                         */

                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);

                        var slop = ViewConfiguration.getScaledTouchSlop(host.ScrollHostGetContext());
                        if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("slop: " + slop);
                        if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("total distance: " + flywheel.TotalDistance);

                        if (MathF.Abs(flywheel.TotalDistance.First) > slop || Math.Abs(flywheel.TotalDistance.Second) > slop)
                        {
                            was_down = false;
                            mIsBeingDragged = true;
                            has_been_dragged = true;
                            has_been_autoscrolled = false;
                            DoScroll(target_view);
                            var parent = this_view.getParent();
                            if (parent != null)
                            {
                                parent.requestDisallowInterceptTouchEvent(true);
                            }
                        }
                    }
                    break;
                case Touch.State.TOUCH_UP:
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH UP");
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("SMOOTH SCROLL: " + smoothScroll);
                    if (mIsBeingDragged) {
                        flywheel.FinalizeMovement();
                    }
                    break;
            }

            flywheel.ReleaseLock();
            if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("INTERCEPT TOUCH IS BEING DRAGGED: " + mIsBeingDragged);
            return mIsBeingDragged;
        }

        private void CanScroll(out bool can_scroll_down_or_right, out bool can_scroll_up_or_left)
        {
            can_scroll_down_or_right = true;
            can_scroll_up_or_left = true;
            int x = 1;
            int y = 1;
            RESULT r = NeedsClamp(ref x, ref y);
            if (r == RESULT.CLAMP_XY)
            {
                can_scroll_down_or_right = false;
            }
            x = -1;
            y = -1;
            r = NeedsClamp(ref x, ref y);
            if (r == RESULT.CLAMP_XY)
            {
                can_scroll_up_or_left = false;
            }
        }

        /// <summary>
        /// you must aquire a lock and release a lock before and after calling this method
        /// <code>
        ///  host.flywheel.AquireLock();
        ///  
        ///  // optional code here
        ///  
        ///  host.OnDraw(this_view, target_view);
        ///  
        ///  // optional code here
        ///  
        ///  host.flywheel.ReleaseLock();
        /// </code>
        /// </summary>
        /// <param name="this_view"></param>
        /// <param name="target_view"></param>
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
                        DoScroll(target_view);
                    }
                }
            }
        }

        private void DoScroll(View target_view)
        {
            int x = -flywheel.Position.First.value.toPixel();
            int y = -flywheel.Position.Second.value.toPixel();
            host.ScrollHostTryScrollTo(target_view, x, y);
            RESULT result = NeedsClamp(ref x, ref y);
            if (result != RESULT.OK)
            {
                flywheel.Position = new((float)-x, -y);
                if (result == RESULT.CLAMP_X)
                {
                    flywheel.Velocity = new(0, flywheel.Velocity.Second);
                }
                else if (result == RESULT.CLAMP_Y)
                {
                    flywheel.Velocity = new(flywheel.Velocity.First, 0);
                }
                else if (result == RESULT.CLAMP_XY)
                {
                    flywheel.Velocity = FloatingPointPair<float>.Zero;
                }
            }
            if (View.DEBUG_VIEW_TRACKING) target_view.Log.d("scrolling to: " + x + ", " + y);
            target_view.scrollTo(x, y);
        }

        // protect against batching, touch up emits a touch move if it's location has changed
        // this can cause a slight decrease in velocity upon lifting up
        public void onConfigureTouch(Touch touch)
        {
            touch.DontBatchOnTouchUpOrTouchCancel = true;
        }

        internal void AutoScrollOnMeasure(View this_view, View target_view)
        {
            if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("Auto-scrolling");
            flywheel.AquireLock();
            var scrollX = target_view.mScrollX;
            var scrollY = target_view.mScrollY;

            bool can_scroll_down_or_right, can_scroll_up_or_left;
            CanScroll(out can_scroll_down_or_right, out can_scroll_up_or_left);

            if (!can_scroll_down_or_right && !can_scroll_up_or_left)
            {
                // we cannot scroll
                flywheel.ReleaseLock();
                return;
            }
            var x = host.ScrollHostGetChildRight() - host.ScrollHostGetMeasuredHeight();
            int y = host.ScrollHostGetChildBottom() - host.ScrollHostGetMeasuredHeight();

            RESULT r = RESULT.OK;

            if (target_view.mScrollY == lastY || lastY == 0)
            {
                if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("lastY: " + lastY);
                if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("target_view.mScrollY: " + target_view.mScrollY);

                int y1 = flywheel.Position.Second.value.toPixel();
                if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("Auto-scrolling before add movement: " + y1);

                if (has_been_dragged)
                {
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("the user manually scrolled to this point");
                    if (target_view.mScrollY == lastY)
                    {
                        has_been_dragged = false;
                        has_been_autoscrolled = false;
                    }
                }
                else
                {
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("the user did not manually scroll to this point");
                    lastY = y;
                    flywheel.AddMovement(0, -flywheel.Position.First.value, 0);
                    flywheel.AddMovement(0, -flywheel.Position.First.value, -y);
                    int x2 = -flywheel.Position.First.value.toPixel();
                    int y2 = -flywheel.Position.Second.value.toPixel();
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("Auto-scrolling before clamp: " + y2);
                    host.ScrollHostTryScrollTo(target_view, scrollX, y2);
                    r = NeedsClamp(ref x2, ref y2);
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("Auto-scrolling after clamp:  " + y2);
                    if (r != RESULT.OK)
                    {
                        flywheel.Position = new((float)-x2, (float)-y2);
                        if (r == RESULT.CLAMP_X)
                        {
                            flywheel.Velocity = new(0, flywheel.Velocity.Second.value);
                        }
                        else if (r == RESULT.CLAMP_Y)
                        {
                            flywheel.Velocity = new(flywheel.Velocity.First.value, 0);
                        }
                        else if (r == RESULT.CLAMP_XY)
                        {
                            flywheel.Velocity = FloatingPointPair<float>.Zero;
                        }
                    }
                    if (View.DEBUG_VIEW_TRACKING) this_view.Log.d("Auto-scrolling to " + y2);
                    target_view.scrollTo(scrollX, y2);
                    has_been_dragged = false;
                    has_been_autoscrolled = true;
                }
            }
            flywheel.ReleaseLock();
        }
    }
}