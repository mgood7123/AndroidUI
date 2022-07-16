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

using AndroidUI.Exceptions;
using AndroidUI.Utils;
using AndroidUI.Utils.Graphics;
using AndroidUI.Utils.Lists;
using AndroidUI.Widgets;

namespace AndroidUI.Utils.Widgets
{

    /**
     * A view tree observer is used to register listeners that can be notified of global
     * changes in the view tree. Such global events include, but are not limited to,
     * layout of the whole tree, beginning of the drawing pass, touch mode change....
     *
     * A ViewTreeObserver should never be instantiated by applications as it is provided
     * by the views hierarchy. Refer to {@link android.view.View#getViewTreeObserver()}
     * for more information.
     */
    public sealed class ViewTreeObserver
    {
        // Recursive listeners use CopyOnWriteList
        private CopyOnWriteList<OnWindowFocusChangeListener> mOnWindowFocusListeners;
        private CopyOnWriteList<OnWindowAttachListener> mOnWindowAttachListeners;
        private CopyOnWriteList<OnGlobalFocusChangeListener> mOnGlobalFocusListeners;
        private CopyOnWriteList<OnTouchModeChangeListener> mOnTouchModeChangeListeners;
        private CopyOnWriteList<OnEnterAnimationCompleteListener> mOnEnterAnimationCompleteListeners;

        // Non-recursive listeners use CopyOnWriteArray
        // Any listener invoked from ViewRootImpl.performTraversals() should not be recursive
        private CopyOnWriteArray<OnGlobalLayoutListener> mOnGlobalLayoutListeners;
        private CopyOnWriteArray<OnComputeInternalInsetsListener> mOnComputeInternalInsetsListeners;
        private CopyOnWriteArray<OnScrollChangedListener> mOnScrollChangedListeners;
        private CopyOnWriteArray<OnPreDrawListener> mOnPreDrawListeners;
        private CopyOnWriteArray<OnWindowShownListener> mOnWindowShownListeners;
        //private CopyOnWriteArray<Consumer<List<Rect>>> mGestureExclusionListeners;

        // These listeners cannot be mutated during dispatch
        private bool mInDispatchOnDraw;
        private List<OnDrawListener> mOnDrawListeners;
        private static bool sIllegalOnDrawModificationIsFatal;

        // These listeners are one-shot
        private List<Runnable> mOnFrameCommitListeners;

        /** Remains false until #dispatchOnWindowShown() is called. If a listener registers after
         * that the listener will be immediately called. */
        private bool mWindowShown;

        private bool mAlive = true;

        /**
         * Interface definition for a callback to be invoked when the view hierarchy is
         * attached to and detached from its window.
         */
        public interface OnWindowAttachListener
        {
            /**
             * Callback method to be invoked when the view hierarchy is attached to a window
             */
            public void onWindowAttached();

            /**
             * Callback method to be invoked when the view hierarchy is detached from a window
             */
            public void onWindowDetached();
        }

        /**
         * Interface definition for a callback to be invoked when the view hierarchy's window
         * focus state changes.
         */
        public interface OnWindowFocusChangeListener
        {
            /**
             * Callback method to be invoked when the window focus changes in the view tree.
             *
             * @param hasFocus Set to true if the window is gaining focus, false if it is
             * losing focus.
             */
            public void onWindowFocusChanged(bool hasFocus);
        }

        /**
         * Interface definition for a callback to be invoked when the focus state within
         * the view tree changes.
         */
        public interface OnGlobalFocusChangeListener
        {
            /**
             * Callback method to be invoked when the focus changes in the view tree. When
             * the view tree transitions from touch mode to non-touch mode, oldFocus is null.
             * When the view tree transitions from non-touch mode to touch mode, newFocus is
             * null. When focus changes in non-touch mode (without transition from or to
             * touch mode) either oldFocus or newFocus can be null.
             *
             * @param oldFocus The previously focused view, if any.
             * @param newFocus The newly focused View, if any.
             */
            public void onGlobalFocusChanged(View oldFocus, View newFocus);
        }

        /**
         * Interface definition for a callback to be invoked when the global layout state
         * or the visibility of views within the view tree changes.
         */
        public interface OnGlobalLayoutListener
        {
            /**
             * Callback method to be invoked when the global layout state or the visibility of views
             * within the view tree changes
             */
            public void onGlobalLayout();
        }

        /**
         * Interface definition for a callback to be invoked when the view tree is about to be drawn.
         */
        public interface OnPreDrawListener
        {
            /**
             * Callback method to be invoked when the view tree is about to be drawn. At this point, all
             * views in the tree have been measured and given a frame. Clients can use this to adjust
             * their scroll bounds or even to request a new layout before drawing occurs.
             *
             * @return Return true to proceed with the current drawing pass, or false to cancel.
             *
             * @see android.view.View#onMeasure
             * @see android.view.View#onLayout
             * @see android.view.View#onDraw
             */
            public bool onPreDraw();
        }

        /**
         * Interface definition for a callback to be invoked when the view tree is about to be drawn.
         */
        public interface OnDrawListener
        {
            /**
             * <p>Callback method to be invoked when the view tree is about to be drawn. At this point,
             * views cannot be modified in any way.</p>
             * 
             * <p>Unlike with {@link OnPreDrawListener}, this method cannot be used to cancel the
             * current drawing pass.</p>
             * 
             * <p>An {@link OnDrawListener} listener <strong>cannot be added or removed</strong>
             * from this method.</p>
             *
             * @see android.view.View#onMeasure
             * @see android.view.View#onLayout
             * @see android.view.View#onDraw
             */
            public void onDraw();
        }

        /**
         * Interface definition for a callback to be invoked when the touch mode changes.
         */
        public interface OnTouchModeChangeListener
        {
            /**
             * Callback method to be invoked when the touch mode changes.
             *
             * @param isInTouchMode True if the view hierarchy is now in touch mode, false  otherwise.
             */
            public void onTouchModeChanged(bool isInTouchMode);
        }

        /**
         * Interface definition for a callback to be invoked when
         * something in the view tree has been scrolled.
         */
        public interface OnScrollChangedListener
        {
            /**
             * Callback method to be invoked when something in the view tree
             * has been scrolled.
             */
            public void onScrollChanged();
        }

        /**
         * Interface definition for a callback noting when a system window has been displayed.
         * This is only used for non-Activity windows. Activity windows can use
         * Activity.onEnterAnimationComplete() to get the same signal.
         * @hide
         */
        public interface OnWindowShownListener
        {
            /**
             * Callback method to be invoked when a non-activity window is fully shown.
             */
            void onWindowShown();
        }

        /**
         * Parameters used with OnComputeInternalInsetsListener.
         * 
         * We are not yet ready to commit to this API and support it, so
         * @hide
         */
        internal sealed class InternalInsetsInfo
        {

            public InternalInsetsInfo()
            {
            }

            /**
             * Offsets from the frame of the window at which the content of
             * windows behind it should be placed.
             */
            public Rect contentInsets = new Rect();

            /**
             * Offsets from the frame of the window at which windows behind it
             * are visible.
             */
            public Rect visibleInsets = new Rect();

            /**
             * Touchable region defined relative to the origin of the frame of the window.
             * Only used when {@link #setTouchableInsets(int)} is called with
             * the option {@link #TOUCHABLE_INSETS_REGION}.
             */
            public Region touchableRegion = new Region();

            /**
             * Option for {@link #setTouchableInsets(int)}: the entire window frame
             * can be touched.
             */
            public const int TOUCHABLE_INSETS_FRAME = 0;

            /**
             * Option for {@link #setTouchableInsets(int)}: the area inside of
             * the content insets can be touched.
             */
            public const int TOUCHABLE_INSETS_CONTENT = 1;

            /**
             * Option for {@link #setTouchableInsets(int)}: the area inside of
             * the visible insets can be touched.
             */
            public const int TOUCHABLE_INSETS_VISIBLE = 2;

            /**
             * Option for {@link #setTouchableInsets(int)}: the area inside of
             * the provided touchable region in {@link #touchableRegion} can be touched.
             */
            public const int TOUCHABLE_INSETS_REGION = 3;

            /**
             * Set which parts of the window can be touched: either
             * {@link #TOUCHABLE_INSETS_FRAME}, {@link #TOUCHABLE_INSETS_CONTENT},
             * {@link #TOUCHABLE_INSETS_VISIBLE}, or {@link #TOUCHABLE_INSETS_REGION}.
             */
            public void setTouchableInsets(int val)
            {
                mTouchableInsets = val;
            }

            int mTouchableInsets;

            void reset()
            {
                contentInsets.setEmpty();
                visibleInsets.setEmpty();
                touchableRegion.setEmpty();
                mTouchableInsets = TOUCHABLE_INSETS_FRAME;
            }

            bool isEmpty()
            {
                return contentInsets.isEmpty()
                        && visibleInsets.isEmpty()
                        && touchableRegion.isEmpty()
                        && mTouchableInsets == TOUCHABLE_INSETS_FRAME;
            }

            override
            public int GetHashCode()
            {
                int result = contentInsets.GetHashCode();
                result = 31 * result + visibleInsets.GetHashCode();
                result = 31 * result + touchableRegion.GetHashCode();
                result = 31 * result + mTouchableInsets;
                return result;
            }

            override
            public bool Equals(object o)
            {
                if (this == o) return true;
                if (o == null || GetType() != o.GetType()) return false;

                InternalInsetsInfo other = (InternalInsetsInfo)o;
                return mTouchableInsets == other.mTouchableInsets &&
                        contentInsets.Equals(other.contentInsets) &&
                        visibleInsets.Equals(other.visibleInsets) &&
                        touchableRegion.Equals(other.touchableRegion);
            }

            void set(InternalInsetsInfo other)
            {
                contentInsets.set(other.contentInsets);
                visibleInsets.set(other.visibleInsets);
                touchableRegion.set(other.touchableRegion);
                mTouchableInsets = other.mTouchableInsets;
            }
        }

        /**
         * Interface definition for a callback to be invoked when layout has
         * completed and the client can compute its interior insets.
         * 
         * We are not yet ready to commit to this API and support it, so
         * @hide
         */
        internal interface OnComputeInternalInsetsListener
        {
            /**
             * Callback method to be invoked when layout has completed and the
             * client can compute its interior insets.
             *
             * @param inoutInfo Should be filled in by the implementation with
             * the information about the insets of the window.  This is called
             * with whatever values the previous OnComputeInternalInsetsListener
             * returned, if there are multiple such listeners in the window.
             */
            public void onComputeInternalInsets(InternalInsetsInfo inoutInfo);
        }

        /**
         * @hide
         */
        internal interface OnEnterAnimationCompleteListener
        {
            public void onEnterAnimationComplete();
        }

        /**
         * Creates a new ViewTreeObserver. This constructor should not be called
         */
        internal ViewTreeObserver()
        {
            sIllegalOnDrawModificationIsFatal = true;
        }

        /**
         * Merges all the listeners registered on the specified observer with the listeners
         * registered on this object. After this method is invoked, the specified observer
         * will return false in {@link #isAlive()} and should not be used anymore.
         *
         * @param observer The ViewTreeObserver whose listeners must be added to this observer
         */
        internal void merge(ViewTreeObserver observer)
        {
            if (observer.mOnWindowAttachListeners != null)
            {
                if (mOnWindowAttachListeners != null)
                {
                    mOnWindowAttachListeners.AddRange(observer.mOnWindowAttachListeners);
                }
                else
                {
                    mOnWindowAttachListeners = observer.mOnWindowAttachListeners;
                }
            }

            if (observer.mOnWindowFocusListeners != null)
            {
                if (mOnWindowFocusListeners != null)
                {
                    mOnWindowFocusListeners.AddRange(observer.mOnWindowFocusListeners);
                }
                else
                {
                    mOnWindowFocusListeners = observer.mOnWindowFocusListeners;
                }
            }

            if (observer.mOnGlobalFocusListeners != null)
            {
                if (mOnGlobalFocusListeners != null)
                {
                    mOnGlobalFocusListeners.AddRange(observer.mOnGlobalFocusListeners);
                }
                else
                {
                    mOnGlobalFocusListeners = observer.mOnGlobalFocusListeners;
                }
            }

            if (observer.mOnGlobalLayoutListeners != null)
            {
                if (mOnGlobalLayoutListeners != null)
                {
                    mOnGlobalLayoutListeners.AddRange(observer.mOnGlobalLayoutListeners);
                }
                else
                {
                    mOnGlobalLayoutListeners = observer.mOnGlobalLayoutListeners;
                }
            }

            if (observer.mOnPreDrawListeners != null)
            {
                if (mOnPreDrawListeners != null)
                {
                    mOnPreDrawListeners.AddRange(observer.mOnPreDrawListeners);
                }
                else
                {
                    mOnPreDrawListeners = observer.mOnPreDrawListeners;
                }
            }

            if (observer.mOnDrawListeners != null)
            {
                if (mOnDrawListeners != null)
                {
                    mOnDrawListeners.AddRange(observer.mOnDrawListeners);
                }
                else
                {
                    mOnDrawListeners = observer.mOnDrawListeners;
                }
            }

            if (observer.mOnFrameCommitListeners != null)
            {
                if (mOnFrameCommitListeners != null)
                {
                    mOnFrameCommitListeners.AddRange(observer.captureFrameCommitCallbacks());
                }
                else
                {
                    mOnFrameCommitListeners = observer.captureFrameCommitCallbacks();
                }
            }

            if (observer.mOnTouchModeChangeListeners != null)
            {
                if (mOnTouchModeChangeListeners != null)
                {
                    mOnTouchModeChangeListeners.AddRange(observer.mOnTouchModeChangeListeners);
                }
                else
                {
                    mOnTouchModeChangeListeners = observer.mOnTouchModeChangeListeners;
                }
            }

            if (observer.mOnComputeInternalInsetsListeners != null)
            {
                if (mOnComputeInternalInsetsListeners != null)
                {
                    mOnComputeInternalInsetsListeners.AddRange(observer.mOnComputeInternalInsetsListeners);
                }
                else
                {
                    mOnComputeInternalInsetsListeners = observer.mOnComputeInternalInsetsListeners;
                }
            }

            if (observer.mOnScrollChangedListeners != null)
            {
                if (mOnScrollChangedListeners != null)
                {
                    mOnScrollChangedListeners.AddRange(observer.mOnScrollChangedListeners);
                }
                else
                {
                    mOnScrollChangedListeners = observer.mOnScrollChangedListeners;
                }
            }

            if (observer.mOnWindowShownListeners != null)
            {
                if (mOnWindowShownListeners != null)
                {
                    mOnWindowShownListeners.AddRange(observer.mOnWindowShownListeners);
                }
                else
                {
                    mOnWindowShownListeners = observer.mOnWindowShownListeners;
                }
            }

            //if (observer.mGestureExclusionListeners != null)
            //{
            //    if (mGestureExclusionListeners != null)
            //    {
            //        mGestureExclusionListeners.AddRange(observer.mGestureExclusionListeners);
            //    }
            //    else
            //    {
            //        mGestureExclusionListeners = observer.mGestureExclusionListeners;
            //    }
            //}

            observer.kill();
        }

        /**
         * Register a callback to be invoked when the view hierarchy is attached to a window.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnWindowAttachListener(OnWindowAttachListener listener)
        {
            checkIsAlive();

            if (mOnWindowAttachListeners == null)
            {
                mOnWindowAttachListeners
                        = new CopyOnWriteList<OnWindowAttachListener>(new ReaderWriterLockSlimInfo());
            }

            mOnWindowAttachListeners.Add(listener);
        }

        /**
         * Remove a previously installed window attach callback.
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnWindowAttachListener(android.view.ViewTreeObserver.OnWindowAttachListener)
         */
        public void removeOnWindowAttachListener(OnWindowAttachListener victim)
        {
            checkIsAlive();
            if (mOnWindowAttachListeners == null)
            {
                return;
            }
            mOnWindowAttachListeners.Remove(victim);
        }

        /**
         * Register a callback to be invoked when the window focus state within the view tree changes.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnWindowFocusChangeListener(OnWindowFocusChangeListener listener)
        {
            checkIsAlive();

            if (mOnWindowFocusListeners == null)
            {
                mOnWindowFocusListeners
                        = new CopyOnWriteList<OnWindowFocusChangeListener>(new ReaderWriterLockSlimInfo());
            }

            mOnWindowFocusListeners.Add(listener);
        }

        /**
         * Remove a previously installed window focus change callback.
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnWindowFocusChangeListener(android.view.ViewTreeObserver.OnWindowFocusChangeListener)
         */
        public void removeOnWindowFocusChangeListener(OnWindowFocusChangeListener victim)
        {
            checkIsAlive();
            if (mOnWindowFocusListeners == null)
            {
                return;
            }
            mOnWindowFocusListeners.Remove(victim);
        }

        /**
         * Register a callback to be invoked when the focus state within the view tree changes.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnGlobalFocusChangeListener(OnGlobalFocusChangeListener listener)
        {
            checkIsAlive();

            if (mOnGlobalFocusListeners == null)
            {
                mOnGlobalFocusListeners = new CopyOnWriteList<OnGlobalFocusChangeListener>(new ReaderWriterLockSlimInfo());
            }

            mOnGlobalFocusListeners.Add(listener);
        }

        /**
         * Remove a previously installed focus change callback.
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnGlobalFocusChangeListener(OnGlobalFocusChangeListener)
         */
        public void removeOnGlobalFocusChangeListener(OnGlobalFocusChangeListener victim)
        {
            checkIsAlive();
            if (mOnGlobalFocusListeners == null)
            {
                return;
            }
            mOnGlobalFocusListeners.Remove(victim);
        }

        /**
         * Register a callback to be invoked when the global layout state or the visibility of views
         * within the view tree changes
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnGlobalLayoutListener(OnGlobalLayoutListener listener)
        {
            checkIsAlive();

            if (mOnGlobalLayoutListeners == null)
            {
                mOnGlobalLayoutListeners = new CopyOnWriteArray<OnGlobalLayoutListener>();
            }

            mOnGlobalLayoutListeners.add(listener);
        }

        /**
         * Remove a previously installed global layout callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         * 
         * @deprecated Use #removeOnGlobalLayoutListener instead
         *
         * @see #addOnGlobalLayoutListener(OnGlobalLayoutListener)
         */
        [Obsolete]
        public void removeGlobalOnLayoutListener(OnGlobalLayoutListener victim)
        {
            removeOnGlobalLayoutListener(victim);
        }

        /**
         * Remove a previously installed global layout callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         * 
         * @see #addOnGlobalLayoutListener(OnGlobalLayoutListener)
         */
        public void removeOnGlobalLayoutListener(OnGlobalLayoutListener victim)
        {
            checkIsAlive();
            if (mOnGlobalLayoutListeners == null)
            {
                return;
            }
            mOnGlobalLayoutListeners.remove(victim);
        }

        /**
         * Register a callback to be invoked when the view tree is about to be drawn
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnPreDrawListener(OnPreDrawListener listener)
        {
            checkIsAlive();

            if (mOnPreDrawListeners == null)
            {
                mOnPreDrawListeners = new CopyOnWriteArray<OnPreDrawListener>();
            }

            mOnPreDrawListeners.add(listener);
        }

        /**
         * Remove a previously installed pre-draw callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnPreDrawListener(OnPreDrawListener)
         */
        public void removeOnPreDrawListener(OnPreDrawListener victim)
        {
            checkIsAlive();
            if (mOnPreDrawListeners == null)
            {
                return;
            }
            mOnPreDrawListeners.remove(victim);
        }

        /**
         * Register a callback to be invoked when the view tree window has been shown
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         * @hide
         */
        public void addOnWindowShownListener(OnWindowShownListener listener)
        {
            checkIsAlive();

            if (mOnWindowShownListeners == null)
            {
                mOnWindowShownListeners = new CopyOnWriteArray<OnWindowShownListener>();
            }

            mOnWindowShownListeners.add(listener);
            if (mWindowShown)
            {
                listener.onWindowShown();
            }
        }

        /**
         * Remove a previously installed window shown callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnWindowShownListener(OnWindowShownListener)
         * @hide
         */
        public void removeOnWindowShownListener(OnWindowShownListener victim)
        {
            checkIsAlive();
            if (mOnWindowShownListeners == null)
            {
                return;
            }
            mOnWindowShownListeners.remove(victim);
        }

        /**
         * <p>Register a callback to be invoked when the view tree is about to be drawn.</p>
         * <p><strong>Note:</strong> this method <strong>cannot</strong> be invoked from
         * {@link android.view.ViewTreeObserver.OnDrawListener#onDraw()}.</p>
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnDrawListener(OnDrawListener listener)
        {
            checkIsAlive();

            if (mOnDrawListeners == null)
            {
                mOnDrawListeners = new List<OnDrawListener>();
            }

            if (mInDispatchOnDraw)
            {
                IllegalStateException ex = new IllegalStateException(
                        "Cannot call addOnDrawListener inside of onDraw");
                if (sIllegalOnDrawModificationIsFatal)
                {
                    throw ex;
                }
                else
                {
                    Log.e("ViewTreeObserver", ex.ToString());
                }
            }
            mOnDrawListeners.Add(listener);
        }

        /**
         * <p>Remove a previously installed pre-draw callback.</p>
         * <p><strong>Note:</strong> this method <strong>cannot</strong> be invoked from
         * {@link android.view.ViewTreeObserver.OnDrawListener#onDraw()}.</p>
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnDrawListener(OnDrawListener)
         */
        public void removeOnDrawListener(OnDrawListener victim)
        {
            checkIsAlive();
            if (mOnDrawListeners == null)
            {
                return;
            }
            if (mInDispatchOnDraw)
            {
                IllegalStateException ex = new IllegalStateException(
                        "Cannot call removeOnDrawListener inside of onDraw");
                if (sIllegalOnDrawModificationIsFatal)
                {
                    throw ex;
                }
                else
                {
                    Log.e("ViewTreeObserver", ex.ToString());
                }
            }
            mOnDrawListeners.Remove(victim);
        }

        /**
         * Adds a frame commit callback. This callback will be invoked when the current rendering
         * content has been rendered into a frame and submitted to the swap chain. The frame may
         * not currently be visible on the display when this is invoked, but it has been submitted.
         * This callback is useful in combination with {@link PixelCopy} to capture the current
         * rendered content of the UI reliably.
         *
         * Note: Only works with hardware rendering. Does nothing otherwise.
         *
         * @param callback The callback to invoke when the frame is committed.
         */
        public void registerFrameCommitCallback(Runnable callback)
        {
            checkIsAlive();
            if (mOnFrameCommitListeners == null)
            {
                mOnFrameCommitListeners = new List<Runnable>();
            }
            mOnFrameCommitListeners.Add(callback);
        }

        List<Runnable> captureFrameCommitCallbacks()
        {
            List<Runnable> ret = mOnFrameCommitListeners;
            mOnFrameCommitListeners = null;
            return ret;
        }

        /**
         * Attempts to remove the given callback from the list of pending frame complete callbacks.
         *
         * @param callback The callback to remove
         * @return Whether or not the callback was removed. If this returns true the callback will
         *         not be invoked. If false is returned then the callback was either never added
         *         or may already be pending execution and was unable to be removed
         */
        public bool unregisterFrameCommitCallback(Runnable callback)
        {
            checkIsAlive();
            if (mOnFrameCommitListeners == null)
            {
                return false;
            }
            return mOnFrameCommitListeners.Remove(callback);
        }

        /**
         * Register a callback to be invoked when a view has been scrolled.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnScrollChangedListener(OnScrollChangedListener listener)
        {
            checkIsAlive();

            if (mOnScrollChangedListeners == null)
            {
                mOnScrollChangedListeners = new CopyOnWriteArray<OnScrollChangedListener>();
            }

            mOnScrollChangedListeners.add(listener);
        }

        /**
         * Remove a previously installed scroll-changed callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnScrollChangedListener(OnScrollChangedListener)
         */
        public void removeOnScrollChangedListener(OnScrollChangedListener victim)
        {
            checkIsAlive();
            if (mOnScrollChangedListeners == null)
            {
                return;
            }
            mOnScrollChangedListeners.remove(victim);
        }

        /**
         * Register a callback to be invoked when the invoked when the touch mode changes.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         */
        public void addOnTouchModeChangeListener(OnTouchModeChangeListener listener)
        {
            checkIsAlive();

            if (mOnTouchModeChangeListeners == null)
            {
                mOnTouchModeChangeListeners = new CopyOnWriteList<OnTouchModeChangeListener>(new ReaderWriterLockSlimInfo());
            }

            mOnTouchModeChangeListeners.Add(listener);
        }

        /**
         * Remove a previously installed touch mode change callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnTouchModeChangeListener(OnTouchModeChangeListener)
         */
        public void removeOnTouchModeChangeListener(OnTouchModeChangeListener victim)
        {
            checkIsAlive();
            if (mOnTouchModeChangeListeners == null)
            {
                return;
            }
            mOnTouchModeChangeListeners.Remove(victim);
        }

        /**
         * Register a callback to be invoked when the invoked when it is time to
         * compute the window's internal insets.
         *
         * @param listener The callback to add
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         * 
         * We are not yet ready to commit to this API and support it, so
         * @hide
         */
        internal void addOnComputeInternalInsetsListener(OnComputeInternalInsetsListener listener)
        {
            checkIsAlive();

            if (mOnComputeInternalInsetsListeners == null)
            {
                mOnComputeInternalInsetsListeners =
                        new CopyOnWriteArray<OnComputeInternalInsetsListener>();
            }

            mOnComputeInternalInsetsListeners.add(listener);
        }

        /**
         * Remove a previously installed internal insets computation callback
         *
         * @param victim The callback to remove
         *
         * @throws IllegalStateException If {@link #isAlive()} returns false
         *
         * @see #addOnComputeInternalInsetsListener(OnComputeInternalInsetsListener)
         * 
         * We are not yet ready to commit to this API and support it, so
         * @hide
         */
        internal void removeOnComputeInternalInsetsListener(OnComputeInternalInsetsListener victim)
        {
            checkIsAlive();
            if (mOnComputeInternalInsetsListeners == null)
            {
                return;
            }
            mOnComputeInternalInsetsListeners.remove(victim);
        }

        /**
         * @hide
         */
        internal void addOnEnterAnimationCompleteListener(OnEnterAnimationCompleteListener listener)
        {
            checkIsAlive();
            if (mOnEnterAnimationCompleteListeners == null)
            {
                mOnEnterAnimationCompleteListeners =
                        new CopyOnWriteList<OnEnterAnimationCompleteListener>(new ReaderWriterLockSlimInfo());
            }
            mOnEnterAnimationCompleteListeners.Add(listener);
        }

        /**
         * @hide
         */
        internal void removeOnEnterAnimationCompleteListener(OnEnterAnimationCompleteListener listener)
        {
            checkIsAlive();
            if (mOnEnterAnimationCompleteListeners == null)
            {
                return;
            }
            mOnEnterAnimationCompleteListeners.Remove(listener);
        }

        ///**
        // * Add a listener to be notified when the tree's <em>transformed</em> gesture exclusion rects
        // * change. This could be the result of an animation or other layout change, or a view calling
        // * {@link View#setSystemGestureExclusionRects(List)}.
        // *
        // * @param listener listener to add
        // * @see View#setSystemGestureExclusionRects(List)
        // */
        //public void addOnSystemGestureExclusionRectsChangedListener(
        //        @NonNull Consumer<List<Rect>> listener)
        //{
        //    checkIsAlive();
        //    if (mGestureExclusionListeners == null)
        //    {
        //        mGestureExclusionListeners = new CopyOnWriteArray<>();
        //    }
        //    mGestureExclusionListeners.add(listener);
        //}

        ///**
        // * Unsubscribe the given listener from gesture exclusion rect changes.
        // * @see #addOnSystemGestureExclusionRectsChangedListener(Consumer)
        // * @see View#setSystemGestureExclusionRects(List)
        // */
        //public void removeOnSystemGestureExclusionRectsChangedListener(
        //        @NonNull Consumer<List<Rect>> listener)
        //{
        //    checkIsAlive();
        //    if (mGestureExclusionListeners == null)
        //    {
        //        return;
        //    }
        //    mGestureExclusionListeners.remove(listener);
        //}

        private void checkIsAlive()
        {
            if (!mAlive)
            {
                throw new IllegalStateException("This ViewTreeObserver is not alive, call "
                        + "getViewTreeObserver() again");
            }
        }

        /**
         * Indicates whether this ViewTreeObserver is alive. When an observer is not alive,
         * any call to a method (except this one) will throw an exception.
         *
         * If an application keeps a long-lived reference to this ViewTreeObserver, it should
         * always check for the result of this method before calling any other method.
         *
         * @return True if this object is alive and be used, false otherwise.
         */
        public bool isAlive()
        {
            return mAlive;
        }

        /**
         * Marks this ViewTreeObserver as not alive. After invoking this method, invoking
         * any other method but {@link #isAlive()} and {@link #kill()} will throw an Exception.
         *
         * @hide
         */
        private void kill()
        {
            mAlive = false;
        }

        /**
         * Notifies registered listeners that window has been attached/detached.
         */
        internal void dispatchOnWindowAttachedChange(bool attached)
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteList<OnWindowAttachListener> listeners = mOnWindowAttachListeners;
            if (listeners != null && listeners.Count > 0)
            {
                foreach (OnWindowAttachListener listener in listeners)
                {
                    if (attached) listener.onWindowAttached();
                    else listener.onWindowDetached();
                }
            }
        }

        /**
         * Notifies registered listeners that window focus has changed.
         */
        internal void dispatchOnWindowFocusChange(bool hasFocus)
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteList<OnWindowFocusChangeListener> listeners = mOnWindowFocusListeners;
            if (listeners != null && listeners.Count > 0)
            {
                foreach (OnWindowFocusChangeListener listener in listeners)
                {
                    listener.onWindowFocusChanged(hasFocus);
                }
            }
        }

        /**
         * Notifies registered listeners that focus has changed.
         */
        internal void dispatchOnGlobalFocusChange(View oldFocus, View newFocus)
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteList<OnGlobalFocusChangeListener> listeners = mOnGlobalFocusListeners;
            if (listeners != null && listeners.Count > 0)
            {
                foreach (OnGlobalFocusChangeListener listener in listeners)
                {
                    listener.onGlobalFocusChanged(oldFocus, newFocus);
                }
            }
        }

        /**
         * Notifies registered listeners that a global layout happened. This can be called
         * manually if you are forcing a layout on a View or a hierarchy of Views that are
         * not attached to a Window or in the GONE state.
         */
        public void dispatchOnGlobalLayout()
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteArray<OnGlobalLayoutListener> listeners = mOnGlobalLayoutListeners;
            if (listeners != null && listeners.size() > 0)
            {
                CopyOnWriteArray<OnGlobalLayoutListener>.Access<OnGlobalLayoutListener> access = listeners.start();
                try
                {
                    int count = access.size();
                    for (int i = 0; i < count; i++)
                    {
                        access.get(i).onGlobalLayout();
                    }
                }
                finally
                {
                    listeners.end();
                }
            }
        }

        /**
         * Returns whether there are listeners for on pre-draw events.
         */
        bool hasOnPreDrawListeners()
        {
            return mOnPreDrawListeners != null && mOnPreDrawListeners.size() > 0;
        }

        /**
         * Notifies registered listeners that the drawing pass is about to start. If a
         * listener returns true, then the drawing pass is canceled and rescheduled. This can
         * be called manually if you are forcing the drawing on a View or a hierarchy of Views
         * that are not attached to a Window or in the GONE state.
         *
         * @return True if the current draw should be canceled and rescheduled, false otherwise.
         */
        public bool dispatchOnPreDraw()
        {
            bool cancelDraw = false;
            CopyOnWriteArray<OnPreDrawListener> listeners = mOnPreDrawListeners;
            if (listeners != null && listeners.size() > 0)
            {
                CopyOnWriteArray<OnPreDrawListener>.Access<OnPreDrawListener> access = listeners.start();
                try
                {
                    int count = access.size();
                    for (int i = 0; i < count; i++)
                    {
                        cancelDraw |= !access.get(i).onPreDraw();
                    }
                }
                finally
                {
                    listeners.end();
                }
            }
            return cancelDraw;
        }

        /**
         * Notifies registered listeners that the window is now shown
         * @hide
         */
        public void dispatchOnWindowShown()
        {
            mWindowShown = true;
            CopyOnWriteArray<OnWindowShownListener> listeners = mOnWindowShownListeners;
            if (listeners != null && listeners.size() > 0)
            {
                CopyOnWriteArray<OnWindowShownListener>.Access<OnWindowShownListener> access = listeners.start();
                try
                {
                    int count = access.size();
                    for (int i = 0; i < count; i++)
                    {
                        access.get(i).onWindowShown();
                    }
                }
                finally
                {
                    listeners.end();
                }
            }
        }

        /**
         * Notifies registered listeners that the drawing pass is about to start.
         */
        public void dispatchOnDraw()
        {
            if (mOnDrawListeners != null)
            {
                mInDispatchOnDraw = true;
                List<OnDrawListener> listeners = mOnDrawListeners;
                int numListeners = listeners.Count;
                for (int i = 0; i < numListeners; ++i)
                {
                    listeners.ElementAt(i).onDraw();
                }
                mInDispatchOnDraw = false;
            }
        }

        /**
         * Notifies registered listeners that the touch mode has changed.
         *
         * @param inTouchMode True if the touch mode is now enabled, false otherwise.
         */
        internal void dispatchOnTouchModeChanged(bool inTouchMode)
        {
            CopyOnWriteList<OnTouchModeChangeListener> listeners =
                    mOnTouchModeChangeListeners;
            if (listeners != null && listeners.Count > 0)
            {
                foreach (OnTouchModeChangeListener listener in listeners)
                {
                    listener.onTouchModeChanged(inTouchMode);
                }
            }
        }

        /**
         * Notifies registered listeners that something has scrolled.
         */
        internal void dispatchOnScrollChanged()
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteArray<OnScrollChangedListener> listeners = mOnScrollChangedListeners;
            if (listeners != null && listeners.size() > 0)
            {
                CopyOnWriteArray<OnScrollChangedListener>.Access<OnScrollChangedListener> access = listeners.start();
                try
                {
                    int count = access.size();
                    for (int i = 0; i < count; i++)
                    {
                        access.get(i).onScrollChanged();
                    }
                }
                finally
                {
                    listeners.end();
                }
            }
        }

        /**
         * Returns whether there are listeners for computing internal insets.
         */
        internal bool hasComputeInternalInsetsListeners()
        {
            CopyOnWriteArray<OnComputeInternalInsetsListener> listeners =
                    mOnComputeInternalInsetsListeners;
            return listeners != null && listeners.size() > 0;
        }

        /**
         * Calls all listeners to compute the current insets.
         */
        internal void dispatchOnComputeInternalInsets(InternalInsetsInfo inoutInfo)
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteArray<OnComputeInternalInsetsListener> listeners =
                    mOnComputeInternalInsetsListeners;
            if (listeners != null && listeners.size() > 0)
            {
                CopyOnWriteArray<OnComputeInternalInsetsListener>.Access<OnComputeInternalInsetsListener> access = listeners.start();
                try
                {
                    int count = access.size();
                    for (int i = 0; i < count; i++)
                    {
                        access.get(i).onComputeInternalInsets(inoutInfo);
                    }
                }
                finally
                {
                    listeners.end();
                }
            }
        }

        /**
         * @hide
         */
        internal void dispatchOnEnterAnimationComplete()
        {
            // NOTE: because of the use of CopyOnWriteList, we *must* use an iterator to
            // perform the dispatching. The iterator is a safe guard against listeners that
            // could mutate the list by calling the various add/remove methods. This prevents
            // the array from being modified while we iterate it.
            CopyOnWriteList<OnEnterAnimationCompleteListener> listeners =
                    mOnEnterAnimationCompleteListeners;
            if (listeners != null && listeners.Count != 0)
            {
                foreach (OnEnterAnimationCompleteListener listener in listeners)
                {
                    listener.onEnterAnimationComplete();
                }
            }
        }

        //void dispatchOnSystemGestureExclusionRectsChanged(@NonNull List<Rect> rects)
        //{
        //    final CopyOnWriteArray<Consumer< List < Rect >>> listeners = mGestureExclusionListeners;
        //    if (listeners != null && listeners.Count > 0)
        //    {
        //        CopyOnWriteArray.Access<Consumer<List<Rect>>> access = listeners.start();
        //        try
        //        {
        //            final int count = access.Count;
        //            for (int i = 0; i < count; i++)
        //            {
        //                access.get(i).accept(rects);
        //            }
        //        }
        //        finally
        //        {
        //            listeners.end();
        //        }
        //    }
        //}

        /**
         * Copy on write array. This array is not thread safe, and only one loop can
         * iterate over this array at any given time. This class avoids allocations
         * until a concurrent modification happens.
         * 
         * Usage:
         * 
         * CopyOnWriteArray.Access<MyData> access = array.start();
         * try {
         *     for (int i = 0; i < access.Count; i++) {
         *         MyData d = access.get(i);
         *     }
         * } finally {
         *     access.end();
         * }
         */
        internal class CopyOnWriteArray<T>
        {
            internal List<T> mData = new List<T>();
            internal List<T> mDataCopy;

            internal readonly Access<T> mAccess = new Access<T>();

            internal bool mStart;

            internal class Access<T>
            {
                internal List<T> mData;
                internal int mSize;

                internal T get(int index)
                {
                    return mData.ElementAt(index);
                }

                internal int size()
                {
                    return mSize;
                }
            }

            internal CopyOnWriteArray()
            {
            }

            private List<T> getArray()
            {
                if (mStart)
                {
                    if (mDataCopy == null) mDataCopy = new List<T>(mData);
                    return mDataCopy;
                }
                return mData;
            }

            internal Access<T> start()
            {
                if (mStart) throw new IllegalStateException("Iteration already started");
                mStart = true;
                mDataCopy = null;
                mAccess.mData = mData;
                mAccess.mSize = mData.Count;
                return mAccess;
            }

            internal void end()
            {
                if (!mStart) throw new IllegalStateException("Iteration not started");
                mStart = false;
                if (mDataCopy != null)
                {
                    mData = mDataCopy;
                    mAccess.mData.Clear();
                    mAccess.mSize = 0;
                }
                mDataCopy = null;
            }

            internal int size()
            {
                return getArray().Count;
            }

            internal void add(T item)
            {
                getArray().Add(item);
            }

            internal void AddRange(CopyOnWriteArray<T> array)
            {
                getArray().AddRange(array.mData);
            }

            internal void remove(T item)
            {
                getArray().Remove(item);
            }

            internal void clear()
            {
                getArray().Clear();
            }
        }
    }
}