/*
 * Copyright (C) 2008 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AndroidUI.Input;
using AndroidUI.Widgets;

namespace AndroidUI.Utils.Input
{
    /**
     * Helper class to handle situations where you want a view to have a larger touch area than its
     * actual view bounds. The view whose touch area is changed is called the delegate view. This
     * class should be used by an ancestor of the delegate. To use a TouchDelegate, first create an
     * instance that specifies the bounds that should be mapped to the delegate and the delegate
     * view itself.
     * <p>
     * The ancestor should then forward all of its touch events received in its
     * {@link android.view.View#onTouch(Touch)} to {@link #onTouch(Touch)}.
     * </p>
     */
    public class TouchDelegate
    {

        /**
         * View that should receive forwarded touch events
         */
        private View mDelegateView;

        /**
         * Bounds in local coordinates of the containing view that should be mapped to the delegate
         * view. This rect is used for initial hit testing.
         */
        private Rect mBounds;

        /**
         * mBounds inflated to include some slop. This rect is to track whether the motion events
         * should be considered to be within the delegate view.
         */
        private Rect mSlopBounds;

        /**
         * True if the delegate had been targeted on a down event (intersected mBounds).
         */
        private bool mDelegateTargeted;

        /**
         * The touchable region of the View extends above its actual extent.
         */
        public const int ABOVE = 1;

        /**
         * The touchable region of the View extends below its actual extent.
         */
        public const int BELOW = 2;

        /**
         * The touchable region of the View extends to the left of its actual extent.
         */
        public const int TO_LEFT = 4;

        /**
         * The touchable region of the View extends to the right of its actual extent.
         */
        public const int TO_RIGHT = 8;

        private int mSlop;

        /**
         * Constructor
         *
         * @param bounds Bounds in local coordinates of the containing view that should be mapped to
         *        the delegate view
         * @param delegateView The view that should receive motion events
         */
        public TouchDelegate(Rect bounds, View delegateView)
        {
            mBounds = bounds;

            mSlop = ViewConfiguration.get(delegateView.Context).getScaledTouchSlop();
            mSlopBounds = new Rect(bounds);
            mSlopBounds.inset(-mSlop, -mSlop);
            mDelegateView = delegateView;
        }

        /**
         * Forward touch events to the delegate view if the event is within the bounds
         * specified in the constructor.
         *
         * @param event The touch event to forward
         * @return True if the event was consumed by the delegate, false otherwise.
         */
        public bool onTouch(Touch ev)
        {
            var t = ev.getTouchAtCurrentIndex();
            int x = (int)t.location.x;
            int y = (int)t.location.y;
            bool sendToDelegate = false;
            bool hit = true;
            bool handled = false;

            switch (t.state)
            {
                case Touch.State.TOUCH_DOWN:
                case Touch.State.TOUCH_UP:
                case Touch.State.TOUCH_MOVE:
                    if (ev.touchCount > 1)
                    {
                        mDelegateTargeted = mBounds.contains(x, y);
                        sendToDelegate = mDelegateTargeted;
                        break;
                    }
                    sendToDelegate = mDelegateTargeted;
                    if (sendToDelegate)
                    {
                        Rect slopBounds = mSlopBounds;
                        if (!slopBounds.contains(x, y))
                        {
                            hit = false;
                        }
                    }
                    break;
                case Touch.State.TOUCH_CANCELLED:
                    sendToDelegate = mDelegateTargeted;
                    mDelegateTargeted = false;
                    break;
            }
            if (sendToDelegate)
            {
                if (hit)
                {
                    // Offset event coordinates to be inside the target view
                    //event.setLocation(mDelegateView.getWidth() / 2, mDelegateView.getHeight() / 2);
                }
                else
                {
                    // Offset event coordinates to be outside the target view (in case it does
                    // something like tracking pressed state)
                    int slop = mSlop;
                    //event.setLocation(-(slop * 2), -(slop * 2));
                }
                handled = mDelegateView.dispatchTouchEvent(ev);
            }
            return handled;
        }
    }
}