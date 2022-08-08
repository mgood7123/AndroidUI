/*
 * Copyright (C) 2006 The Android Open Source Project
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

using AndroidUI.AnimationFramework.Animator;
using AndroidUI.Applications;
using AndroidUI.Execution;
using AndroidUI.Graphics;
using AndroidUI.Input;
using AndroidUI.OS;
using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;
using SkiaSharp;
using static AndroidUI.Widgets.View.LayoutParams;

namespace AndroidUI.Widgets
{
    public class ViewRootImpl : ViewParent
    {
        private View mContentParent;

        internal bool hasContent() => mContentParent != null;

        public void draw(SKCanvas canvas)
        {
            doTraversal(canvas);
        }

        private static bool DEBUG = false;
        private static bool DEBUG_LAYOUT = false;
        private static bool DEBUG_ORIENTATION = false;
        private static bool DEBUG_INPUT_RESIZE = false;
        private static bool DEBUG_DRAW = false;
        private static bool DEBUG_MEASURE_LAYOUT_DRAW_TIME = false;
        private static bool DEBUG_FPS = false;
        private static bool DEBUG_BLAST = false;

        private const string TAG = "ViewRootImpl";
        private const string mTag = TAG;

        // called by Application wrapper
        internal void setContentView(View view, View.LayoutParams layoutParams)
        {
            if (view == null)
            {
                if (mContentParent != null)
                {
                    mContentParent.removeViewAt(0);
                    holderApp.removeViewAt(0);
                    mContentParent = null;
                }
            }
            else
            {
                if (mContentParent == null)
                {
                    mContentParent = new View();
                    holderApp.addView(mContentParent, new View.LayoutParams(MATCH_PARENT, MATCH_PARENT));
                }
                else
                {
                    mContentParent.removeViewAt(0);
                }
                mContentParent.addView(view, layoutParams);
            }
        }

        internal View getContentView()
        {
            return mContentParent?.getChildAt(0);
        }

        public ViewParent getParent()
        {
            // should we return application?
            return null;
        }

        public void requestChildFocus(View view1, View view2)
        {
        }

        public View focusSearch(View view, int direction)
        {
            return null;
        }

        public void focusableViewAvailable(View view)
        {
        }

        Context context;
        bool mFirst; // true for the first time the view is added
        private bool mWillDrawSoon;
        private bool mNewSurfaceNeeded;
        private bool mActivityRelaunched;
        bool mReportNextDraw;
        bool mFullRedrawNeeded;
        bool mLayoutRequested;
        bool windowSizeMayChange;
        bool mForceNextWindowRelayout;
        bool mIsDrawing;

        int mWidth = -1;
        int mHeight = -1;

        private int canvasWidth;
        private int canvasHeight;

        /**
         * Called by {@link android.view.View#isInLayout()} to determine whether the view hierarchy
         * is currently undergoing a layout pass.
         *
         * @return whether the view hierarchy is currently undergoing a layout pass
         */
        internal bool isInLayout()
        {
            return mInLayout;
        }

        internal void onSizeChanged(int width, int height)
        {
            canvasWidth = width;
            canvasHeight = height;
            frame = new Rect(0, 0, width, height);
            scheduleTraversals();
        }

        public void onTouch(Touch ev)
        {
            if (mView != null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    mView.dispatchTouchEvent(ev);
                }
                else
                {
                    try
                    {
                        mView.dispatchTouchEvent(ev);
                    }
                    catch (Exception e)
                    {
                        Log.v(mTag, "Caught exception while dispatching touch event to view: " + e);
                    }
                }
            }
        }

        // used by Application wrapper
        internal View mView;

        int mViewVisibility;
        bool mAppVisibilityChanged;

        // Variables to track frames per second, enabled via DEBUG_FPS flag
        private long mFpsStartTime = -1;
        private long mFpsPrevTime = -1;
        private int mFpsNumFrames;

        /**
         * Called from draw() when DEBUG_FPS is enabled
         */
        private void trackFPS()
        {
            // Tracks frames per second drawn. First value in a series of draws may be bogus
            // because it down not account for the intervening idle time
            long nowTime = NanoTime.currentTimeMillis();
            if (mFpsStartTime < 0)
            {
                mFpsStartTime = mFpsPrevTime = nowTime;
                mFpsNumFrames = 0;
            }
            else
            {
                ++mFpsNumFrames;
                //long frameTime = nowTime - mFpsPrevTime;
                long totalTime = nowTime - mFpsStartTime;
                //Log.v(mTag, "Frame time:\t" + frameTime);
                mFpsPrevTime = nowTime;
                if (totalTime > 1000)
                {
                    float fps = (float)mFpsNumFrames * 1000 / totalTime;
                    Log.v(mTag, "FPS:\t" + fps);
                    mFpsStartTime = nowTime;
                    mFpsNumFrames = 0;
                }
            }
        }

        int getHostVisibility()
        {
            return !mAppVisible || mView == null ? View.GONE : mView.getVisibility();
        }

        /**
         * Ensure that the touch mode for this window is set, and if it is changing,
         * take the appropriate action.
         * @param inTouchMode Whether we want to be in touch mode.
         * @return True if the touch mode changed and focus changed was changed as a result
         */
        private bool ensureTouchModeLocally(bool inTouchMode)
        {
            if (DEBUG) Log.v("touchmode", "ensureTouchModeLocally(" + inTouchMode + "), current "
                    + "touch mode is " + context.mAttachInfo.mInTouchMode);

            if (context.mAttachInfo.mInTouchMode == inTouchMode) return false;

            context.mAttachInfo.mInTouchMode = inTouchMode;
            context.mAttachInfo.mTreeObserver.dispatchOnTouchModeChanged(inTouchMode);

            return inTouchMode ? enterTouchMode() : leaveTouchMode();
        }

        private bool enterTouchMode()
        {
            if (mView != null && mView.hasFocus())
            {
                // note: not relying on mFocusedView here because this could
                // be when the window is first being added, and mFocused isn't
                // set yet.
                View focused = mView.findFocus();
                if (focused != null && !focused.isFocusableInTouchMode())
                {
                    View ancestorToTakeFocus = findAncestorToTakeFocusInTouchMode(focused);
                    if (ancestorToTakeFocus != null)
                    {
                        // there is an ancestor that wants focus after its
                        // descendants that is focusable in touch mode.. give it
                        // focus
                        return ancestorToTakeFocus.requestFocus();
                    }
                    else
                    {
                        // There's nothing to focus. Clear and propagate through the
                        // hierarchy, but don't attempt to place new focus.
                        focused.clearFocusInternal(null, true, false);
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Find an ancestor of focused that wants focus after its descendants and is
         * focusable in touch mode.
         * @param focused The currently focused view.
         * @return An appropriate view, or null if no such view exists.
         */
        private static View findAncestorToTakeFocusInTouchMode(View focused)
        {
            ViewParent parent = focused.mParent;
            while (parent is View)
            {
                View vgParent = (View)parent;
                if (vgParent.getDescendantFocusability() == View.FOCUS_AFTER_DESCENDANTS
                        && vgParent.isFocusableInTouchMode())
                {
                    return vgParent;
                }
                if (vgParent.isRootNamespace())
                {
                    return null;
                }
                else
                {
                    parent = vgParent.mParent;
                }
            }
            return null;
        }

        private bool leaveTouchMode()
        {
            if (mView != null)
            {
                if (mView.hasFocus())
                {
                    View focusedView = mView.findFocus();
                    if (!(focusedView is View))
                    {
                        // some view has focus, let it keep it
                        return false;
                    }
                    else if (focusedView.getDescendantFocusability() !=
                          View.FOCUS_AFTER_DESCENDANTS)
                    {
                        // some view group has focus, and doesn't prefer its children
                        // over itself for focus, so let them keep it.
                        return false;
                    }
                }

                // find the best view to give focus to in this brave new non-touch-mode
                // world
                return mView.restoreDefaultFocus();
            }
            return false;
        }

        const bool mAddedTouchMode = true;

        /**
         * Figures out the measure spec for the root view in a window based on it's
         * layout params.
         *
         * @param windowSize
         *            The available width or height of the window
         *
         * @param rootDimension
         *            The layout params for one dimension (width or height) of the
         *            window.
         *
         * @return The measure spec to use to measure the root view.
         */
        private static int getRootMeasureSpec(int windowSize, int rootDimension)
        {
            int measureSpec;
            switch (rootDimension)
            {

                case View.LayoutParams.MATCH_PARENT:
                    // Window can't resize. Force root view to be windowSize.
                    measureSpec = View.MeasureSpec.makeMeasureSpec(windowSize, View.MeasureSpec.EXACTLY);
                    break;
                case View.LayoutParams.WRAP_CONTENT:
                    // Window can resize. Set max size for root view.
                    measureSpec = View.MeasureSpec.makeMeasureSpec(windowSize, View.MeasureSpec.AT_MOST);
                    break;
                default:
                    // Window wants to be an exact size. Force root view to be that size.
                    measureSpec = View.MeasureSpec.makeMeasureSpec(rootDimension, View.MeasureSpec.EXACTLY);
                    break;
            }
            return measureSpec;
        }

        private void performMeasure(int childWidthMeasureSpec, int childHeightMeasureSpec)
        {
            if (mView == null)
            {
                return;
            }
            if (System.Diagnostics.Debugger.IsAttached)
            {
                mView.measure(childWidthMeasureSpec, childHeightMeasureSpec);
            }
            else
            {
                try
                {
                    mView.measure(childWidthMeasureSpec, childHeightMeasureSpec);
                }
                catch (Exception e)
                {
                    Log.v(mTag, "Caught exception while measuring view: " + e);
                }
            }
        }

        internal Rect frame;
        Rect mTempRect = new();

        private bool measureHierarchy(View host,
            object lp_unused, // WindowManager.LayoutParams lp,
            object res_unused, //Resources res,
            int desiredWindowWidth, int desiredWindowHeight
        )
        {
            int childWidthMeasureSpec;
            int childHeightMeasureSpec;
            bool windowSizeMayChange = false;

            if (DEBUG_ORIENTATION || DEBUG_LAYOUT) Log.v(mTag, "Measuring " + host + " in display " + desiredWindowWidth
                    + "x" + desiredWindowHeight + "...");

            bool goodMeasure = false;

            if (!goodMeasure)
            {
                childWidthMeasureSpec = getRootMeasureSpec(desiredWindowWidth, View.LayoutParams.MATCH_PARENT); //lp.width);
                childHeightMeasureSpec = getRootMeasureSpec(desiredWindowHeight, View.LayoutParams.MATCH_PARENT); //lp.height);
                performMeasure(childWidthMeasureSpec, childHeightMeasureSpec);
                if (mWidth != host.getMeasuredWidth() || mHeight != host.getMeasuredHeight())
                {
                    windowSizeMayChange = true;
                }
            }

            if (DEBUG)
            {
                Console.WriteLine("======================================");
                Console.WriteLine("performTraversals -- after measure");
                host.debug();
            }

            return windowSizeMayChange;
        }

        bool mDrawingAllowed;
        bool mIsCreating = false;
        bool mStopped = false;
        private bool mScrollMayChange;
        private bool mInLayout;
        private List<View> mLayoutRequesters = new();
        private bool mHandlingLayoutInLayoutRequest;
        private bool sAlwaysAssignFocus;
        private bool mLostWindowFocus = false;
        private bool mHadWindowFocus = true;
        private bool mTraversalScheduled;
        private bool mIsInTraversal;

        private void performTraversals(SKCanvas canvas)
        {
            View host = mView;

            if (DEBUG)
            {
                Console.WriteLine("======================================");
                Console.WriteLine("performTraversals");
                host.debug();
            }

            int w = View.MeasureSpec.makeMeasureSpec(canvasWidth, View.MeasureSpec.EXACTLY);
            int h = View.MeasureSpec.makeMeasureSpec(canvasHeight, View.MeasureSpec.EXACTLY);

            int desiredWindowWidth;
            int desiredWindowHeight;
            int viewVisibility = getHostVisibility();
            bool viewVisibilityChanged = !mFirst
                    && (mViewVisibility != viewVisibility
                    // Also check for possible double visibility update, which will make current
                    // viewVisibility value equal to mViewVisibility and we may miss it.
                    || mAppVisibilityChanged);
            mAppVisibilityChanged = false;
            bool viewUserVisibilityChanged = !mFirst &&
                    mViewVisibility == View.VISIBLE != (viewVisibility == View.VISIBLE);


            if (mFirst)
            {
                windowSizeMayChange = false;
                mFullRedrawNeeded = true;
                mLayoutRequested = true;
                desiredWindowWidth = canvasWidth;
                desiredWindowHeight = canvasHeight;
                if (context == null)
                {
                    throw new Exception("null context");
                }
                mView.dispatchAttachedToWindow(context.mAttachInfo, 0);
            }
            else
            {
                desiredWindowWidth = canvasWidth;
                desiredWindowHeight = canvasHeight;

                if (desiredWindowWidth != mWidth || desiredWindowHeight != mHeight)
                {
                    if (DEBUG_ORIENTATION) Log.v(mTag, "View " + host + " resized to: " + frame);
                    mFullRedrawNeeded = true;
                    mLayoutRequested = true;
                    windowSizeMayChange = true;
                }
            }

            if (viewVisibilityChanged)
            {
                context.mAttachInfo.mWindowVisibility = viewVisibility;
                host.dispatchWindowVisibilityChanged(viewVisibility);
                if (viewUserVisibilityChanged)
                {
                    host.dispatchVisibilityAggregated(viewVisibility == View.VISIBLE);
                }
                if (viewVisibility != View.VISIBLE)// || mNewSurfaceNeeded)
                {
                    //endDragResizing();
                    //destroyHardwareResources();
                }
            }

            // Execute enqueued actions on every traversal in case a detached view enqueued an action
            getRunQueue().executeActions(context.mAttachInfo.mHandler);

            bool layoutRequested = mLayoutRequested && (
                !mStopped ||
                mReportNextDraw
            );
            if (layoutRequested)
            {

                //Resources res = mView.getContext().getResources();

                if (mFirst)
                {
                    // make sure touch mode code executes by setting cached value
                    // to opposite of the added touch mode.
                    context.mAttachInfo.mInTouchMode = true; // !mAddedTouchMode;
                    ensureTouchModeLocally(mAddedTouchMode);
                }
                else
                {
                }

                // Ask host how big it wants to be
                windowSizeMayChange |= measureHierarchy(host, null, 0,
                        desiredWindowWidth, desiredWindowHeight);
            }

            //if (collectViewAttributes())
            //{
            //    params = lp;
            //}

            //if (context.mAttachInfo.mForceReportNewAttributes)
            //{
            //    context.mAttachInfo.mForceReportNewAttributes = false;
            //    params = lp;
            //}

            if (mFirst || context.mAttachInfo.mViewVisibilityChanged)
            {
                context.mAttachInfo.mViewVisibilityChanged = false;
                //int resizeMode = mSoftInputMode & SOFT_INPUT_MASK_ADJUST;
                // If we are in auto resize mode, then we need to determine
                // what mode to use now.
                //if (resizeMode == WindowManager.LayoutParams.SOFT_INPUT_ADJUST_UNSPECIFIED)
                //{
                //    int N = context.mAttachInfo.mScrollContainers.size();
                //    for (int i = 0; i < N; i++)
                //    {
                //        if (context.mAttachInfo.mScrollContainers.get(i).isShown())
                //        {
                //            resizeMode = WindowManager.LayoutParams.SOFT_INPUT_ADJUST_RESIZE;
                //        }
                //    }
                //    if (resizeMode == 0)
                //    {
                //        resizeMode = WindowManager.LayoutParams.SOFT_INPUT_ADJUST_PAN;
                //    }
                //    if ((lp.softInputMode & SOFT_INPUT_MASK_ADJUST) != resizeMode)
                //    {
                //        lp.softInputMode = (lp.softInputMode & ~SOFT_INPUT_MASK_ADJUST) | resizeMode;
                //    params = lp;
                //    }
                //}
            }

            //if (mApplyInsetsRequested && !(mWillMove || mWillResize))
            //{
            //    dispatchApplyInsets(host);
            //    if (mLayoutRequested)
            //    {
            //        // Short-circuit catching a new layout request here, so
            //        // we don't need to go through two layout passes when things
            //        // change due to fitting system windows, which can happen a lot.
            //        windowSizeMayChange |= measureHierarchy(host, lp,
            //                mView.getContext().getResources(),
            //                desiredWindowWidth, desiredWindowHeight);
            //    }
            //}

            if (layoutRequested)
            {
                // Clear this now, so that if anything requests a layout in the
                // rest of this function we will catch it and re-run a full
                // layout pass.
                mLayoutRequested = false;
            }

            bool windowShouldResize = layoutRequested && windowSizeMayChange
                && (mWidth != host.getMeasuredWidth() || mHeight != host.getMeasuredHeight());
            //windowShouldResize |= mDragResizing && mResizeMode == RESIZE_MODE_FREEFORM;

            // If the activity was just relaunched, it might have unfrozen the task bounds (while
            // relaunching), so we need to force a call into window manager to pick up the latest
            // bounds.
            //windowShouldResize |= mActivityRelaunched;

            // Determine whether to compute insets.
            // If there are no inset listeners remaining then we may still need to compute
            // insets in case the old insets were non-empty and must be reset.
            bool computesInternalInsets = false
            //        context.mAttachInfo.mTreeObserver.hasComputeInternalInsetsListeners()
            //        || context.mAttachInfo.mHasNonEmptyGivenInternalInsets
                ;

            bool insetsPending = false;
            int relayoutResult = 0;
            bool updatedConfiguration = false;

            //int surfaceGenerationId = mSurface.getGenerationId();

            bool isViewVisible = viewVisibility == View.VISIBLE;
            bool windowRelayoutWasForced = mForceNextWindowRelayout;
            bool surfaceSizeChanged = false;
            bool surfaceCreated = false;
            bool surfaceDestroyed = false;
            // True if surface generation id changes or relayout result is RELAYOUT_RES_SURFACE_CHANGED.
            bool surfaceReplaced = false;

            //bool windowAttributesChanged = mWindowAttributesChanged;
            //if (windowAttributesChanged)
            //{
            //mWindowAttributesChanged = false;
            //params = lp;
            //}

            //if (params != null) {
            //    if ((host.mPrivateFlags & View.PFLAG_REQUEST_TRANSPARENT_REGIONS) != 0
            //            && !PixelFormat.formatHasAlpha(params.format))
            //    {
            //    params.format = PixelFormat.TRANSLUCENT;
            //    }
            //    adjustLayoutParamsForCompatibility(params);
            //    controlInsetsForCompatibility(params);
            //    if (mDispatchedSystemBarAppearance != params.insetsFlags.appearance) {
            //        mDispatchedSystemBarAppearance = params.insetsFlags.appearance;
            //        mView.onSystemBarAppearanceChanged(mDispatchedSystemBarAppearance);
            //    }
            //}
            bool wasReportNextDraw = mReportNextDraw;

            if (mFirst || windowShouldResize || viewVisibilityChanged
                    // || params != null
                    || mForceNextWindowRelayout)
            {
                mForceNextWindowRelayout = false;

                // If this window is giving internal insets to the window manager, then we want to first
                // make the provided insets unchanged during layout. This avoids it briefly causing
                // other windows to resize/move based on the raw frame of the window, waiting until we
                // can finish laying out this window and get back to the window manager with the
                // ultimately computed insets.
                insetsPending = computesInternalInsets;

                //if (mSurfaceHolder != null)
                //{
                //    mSurfaceHolder.mSurfaceLock.lock () ;
                mDrawingAllowed = true;
                //}

                bool hwInitialized = false;
                bool dispatchApplyInsets = false;
                bool hadSurface = true; // mSurface.isValid();

                try
                {
                    if (DEBUG_LAYOUT)
                    {
                        Log.i(mTag, "host=w:" + host.getMeasuredWidth() + ", h:" +
                                host.getMeasuredHeight()); // + ", params=" + params);
                    }

                    //if (context.mAttachInfo.mThreadedRenderer != null)
                    //{
                    //    // relayoutWindow may decide to destroy mSurface. As that decision
                    //    // happens in WindowManager service, we need to be defensive here
                    //    // and stop using the surface in case it gets destroyed.
                    //    if (context.mAttachInfo.mThreadedRenderer.pause())
                    //    {
                    //        // Animations were running so we need to push a frame
                    //        // to resume them
                    //        mDirty.set(0, 0, mWidth, mHeight);
                    //    }
                    //}
                    if (mFirst || viewVisibilityChanged)
                    {
                        //mViewFrameInfo.flags |= FrameInfo.FLAG_WINDOW_VISIBILITY_CHANGED;
                    }
                    relayoutResult = 0; // relayoutWindow(params, viewVisibility, insetsPending);
                    bool freeformResizing = false; // (relayoutResult
                                                   //& WindowManagerGlobal.RELAYOUT_RES_DRAG_RESIZING_FREEFORM) != 0;
                    bool dockedResizing = false; //(relayoutResult
                                                 //& WindowManagerGlobal.RELAYOUT_RES_DRAG_RESIZING_DOCKED) != 0;
                    bool dragResizing = freeformResizing || dockedResizing;
                    if (
                        windowSizeMayChange
                    //(relayoutResult & WindowManagerGlobal.RELAYOUT_RES_BLAST_SYNC) != 0
                    )
                    {
                        if (DEBUG_BLAST)
                        {
                            Log.d(mTag, "Relayout called with blastSync");
                        }
                        reportNextDraw();
                        if (isHardwareEnabled())
                        {
                            //mNextDrawUseBlastSync = true;
                        }
                    }

                    bool surfaceControlChanged = false;
                    //(relayoutResult & RELAYOUT_RES_SURFACE_CHANGED)
                    //== RELAYOUT_RES_SURFACE_CHANGED;

                    //if (mSurfaceControl.isValid())
                    //{
                    //    updateOpacity(mWindowAttributes, dragResizing,
                    //            surfaceControlChanged /*forceUpdate */);
                    //}

                    if (DEBUG_LAYOUT) Log.v(mTag, "relayout: frame=" + frame.toShortString());

                    // If the pending {@link MergedConfiguration} handed back from
                    // {@link #relayoutWindow} does not match the one last reported,
                    // WindowManagerService has reported back a frame from a configuration not yet
                    // handled by the client. In this case, we need to accept the configuration so we
                    // do not lay out and draw with the wrong configuration.
                    //if (!mPendingMergedConfiguration.equals(mLastReportedMergedConfiguration))
                    //{
                    //    if (DEBUG_CONFIGURATION) Log.v(mTag, "Visible with new config: "
                    //            + mPendingMergedConfiguration.getMergedConfiguration());
                    //    performConfigurationChange(new MergedConfiguration(mPendingMergedConfiguration),
                    //            !mFirst, INVALID_DISPLAY /* same display */);
                    //    updatedConfiguration = true;
                    //}

                    surfaceSizeChanged = false;
                    //if (!mLastSurfaceSize.equals(mSurfaceSize))
                    //{
                    //surfaceSizeChanged = true;
                    //mLastSurfaceSize.set(mSurfaceSize.x, mSurfaceSize.y);
                    //}
                    //bool alwaysConsumeSystemBarsChanged = 
                    //mPendingAlwaysConsumeSystemBars != context.mAttachInfo.mAlwaysConsumeSystemBars;
                    //updateColorModeIfNeeded(lp.getColorMode());
                    //surfaceCreated = !hadSurface && mSurface.isValid();
                    //surfaceDestroyed = hadSurface && !mSurface.isValid();

                    // When using Blast, the surface generation id may not change when there's a new
                    // SurfaceControl. In that case, we also check relayout flag
                    // RELAYOUT_RES_SURFACE_CHANGED since it should indicate that WMS created a new
                    // SurfaceControl.
                    //surfaceReplaced = (surfaceGenerationId != mSurface.getGenerationId()
                    //        || surfaceControlChanged) && mSurface.isValid();
                    //if (surfaceReplaced)
                    //{
                    //    mSurfaceSequenceId++;
                    //}

                    //if (alwaysConsumeSystemBarsChanged)
                    //{
                    //    context.mAttachInfo.mAlwaysConsumeSystemBars = mPendingAlwaysConsumeSystemBars;
                    //    dispatchApplyInsets = true;
                    //}
                    //if (updateCaptionInsets())
                    //{
                    //    dispatchApplyInsets = true;
                    //}
                    //if (dispatchApplyInsets || mLastSystemUiVisibility !=
                    //        context.mAttachInfo.mSystemUiVisibility || mApplyInsetsRequested)
                    //{
                    //    mLastSystemUiVisibility = context.mAttachInfo.mSystemUiVisibility;
                    //    dispatchApplyInsets(host);
                    //    // We applied insets so force contentInsetsChanged to ensure the
                    //    // hierarchy is measured below.
                    //    dispatchApplyInsets = true;
                    //}

                    surfaceCreated = false; // dont completely redraw
                    surfaceDestroyed = false; // surface not removed
                    surfaceSizeChanged = false; // for now
                    surfaceReplaced = false; // never replaced

                    if (surfaceCreated)
                    {
                        // If we are creating a new surface, then we need to
                        // completely redraw it.
                        mFullRedrawNeeded = true;
                        //mPreviousTransparentRegion.setEmpty();

                        // Only initialize up-front if transparent regions are not
                        // requested, otherwise defer to see if the entire window
                        // will be transparent
                        //if (context.mAttachInfo.mThreadedRenderer != null)
                        //{
                        //    try
                        //    {
                        //        hwInitialized = context.mAttachInfo.mThreadedRenderer.initialize(mSurface);
                        //        if (hwInitialized && (host.mPrivateFlags
                        //                        & View.PFLAG_REQUEST_TRANSPARENT_REGIONS) == 0)
                        //        {
                        //            // Don't pre-allocate if transparent regions
                        //            // are requested as they may not be needed
                        //            context.mAttachInfo.mThreadedRenderer.allocateBuffers();
                        //        }
                        //    }
                        //    catch (OutOfResourcesException e)
                        //    {
                        //        handleOutOfResourcesException(e);
                        //        return;
                        //    }
                        //}
                    }
                    else if (surfaceDestroyed)
                    {
                        // If the surface has been removed, then reset the scroll
                        // positions.
                        //if (mLastScrolledFocus != null)
                        //{
                        //    mLastScrolledFocus.clear();
                        //}
                        //mScrollY = mCurScrollY = 0;
                        //if (mView instanceof RootViewSurfaceTaker) {
                        //    ((RootViewSurfaceTaker)mView).onRootViewScrollYChanged(mCurScrollY);
                        //}
                        //if (mScroller != null)
                        //{
                        //    mScroller.abortAnimation();
                        //}
                        //// Our surface is gone
                        //if (isHardwareEnabled())
                        //{
                        //    context.mAttachInfo.mThreadedRenderer.destroy();
                        //}
                    }
                    else if (surfaceReplaced
                          || surfaceSizeChanged || windowRelayoutWasForced
                    //&& mSurfaceHolder == null
                    //&& context.mAttachInfo.mThreadedRenderer != null
                    //&& mSurface.isValid()
                    )
                    {
                        mFullRedrawNeeded = true;
                        try
                        {
                            // Need to do updateSurface (which leads to CanvasContext::setSurface and
                            // re-create the EGLSurface) if either the Surface changed (as indicated by
                            // generation id), or WindowManager changed the surface size. The latter is
                            // because on some chips, changing the consumer side's BufferQueue size may
                            // not take effect immediately unless we create a new EGLSurface.
                            // Note that frame size change doesn't always imply surface size change (eg.
                            // drag resizing uses fullscreen surface), need to check surfaceSizeChanged
                            // flag from WindowManager.
                            //context.mAttachInfo.mThreadedRenderer.updateSurface(mSurface);
                        }
                        catch (OutOfMemoryException e)
                        {
                            //handleOutOfResourcesException(e);
                            return;
                        }
                    }

                    //if (mDragResizing != dragResizing)
                    //{
                    //    if (dragResizing)
                    //    {
                    //        mResizeMode = freeformResizing
                    //                ? RESIZE_MODE_FREEFORM
                    //                : RESIZE_MODE_DOCKED_DIVIDER;
                    //        bool backdropSizeMatchesFrame =
                    //                mWinFrame.width() == mPendingBackDropFrame.width()
                    //                        && mWinFrame.height() == mPendingBackDropFrame.height();
                    //        // TODO: Need cutout?
                    //        startDragResizing(mPendingBackDropFrame, !backdropSizeMatchesFrame,
                    //                context.mAttachInfo.mContentInsets, context.mAttachInfo.mStableInsets, mResizeMode);
                    //    }
                    //    else
                    //    {
                    //        // We shouldn't come here, but if we come we should end the resize.
                    //        endDragResizing();
                    //    }
                    //}
                    //if (!mUseMTRenderer)
                    //{
                    //    if (dragResizing)
                    //    {
                    //        mCanvasOffsetX = mWinFrame.left;
                    //        mCanvasOffsetY = mWinFrame.top;
                    //    }
                    //    else
                    //    {
                    //        mCanvasOffsetX = mCanvasOffsetY = 0;
                    //    }
                    //}
                }
                catch (Exception e)
                {
                }

                if (DEBUG_ORIENTATION) Log.v(
                        TAG, "Relayout returned: frame=" + frame);

                context.mAttachInfo.mWindowLeft = frame.left;
                context.mAttachInfo.mWindowTop = frame.top;

                // !!FIXME!! This next section handles the case where we did not get the
                // window size we asked for. We should avoid this by getting a maximum size from
                // the window session beforehand.
                if (mWidth != frame.width() || mHeight != frame.height())
                {
                    mWidth = frame.width();
                    mHeight = frame.height();
                }
                object mSurfaceHolder = new();
                if (mSurfaceHolder != null)
                {
                    // The app owns the surface; tell it about what is going on.
                    //if (mSurface.isValid())
                    {
                        // XXX .copyFrom() doesn't work!
                        //mSurfaceHolder.mSurface.copyFrom(mSurface);
                        //mSurfaceHolder.mSurface = mSurface;
                        //}
                        //mSurfaceHolder.setSurfaceFrameSize(mWidth, mHeight);
                        //mSurfaceHolder.mSurfaceLock.unlock();
                        //if (surfaceCreated)
                        //{
                        //    mSurfaceHolder.ungetCallbacks();

                        //    mIsCreating = true;
                        //    SurfaceHolder.Callback[] callbacks = mSurfaceHolder.getCallbacks();
                        //    if (callbacks != null)
                        //    {
                        //        for (SurfaceHolder.Callback c : callbacks)
                        //        {
                        //            c.surfaceCreated(mSurfaceHolder);
                        //        }
                        //    }
                    }

                    if (surfaceCreated || surfaceReplaced || surfaceSizeChanged
                    //|| windowAttributesChanged

                    //&& mSurface.isValid()
                    )
                    {
                        //SurfaceHolder.Callback[] callbacks = mSurfaceHolder.getCallbacks();
                        //if (callbacks != null)
                        //{
                        //    for (SurfaceHolder.Callback c : callbacks)
                        //    {
                        //        c.surfaceChanged(mSurfaceHolder, lp.format,
                        //                mWidth, mHeight);
                        //    }
                        //}
                        mIsCreating = false;
                    }

                    if (surfaceDestroyed)
                    {
                        //notifyHolderSurfaceDestroyed();
                        //mSurfaceHolder.mSurfaceLock.lock () ;
                        //try
                        //{
                        //    mSurfaceHolder.mSurface = new Surface();
                        //}
                        //finally
                        //{
                        //    mSurfaceHolder.mSurfaceLock.unlock();
                        //}
                    }
                }

                //ThreadedRenderer threadedRenderer = context.mAttachInfo.mThreadedRenderer;
                //if (threadedRenderer != null && threadedRenderer.isEnabled())
                //{
                //    if (hwInitialized
                //            || mWidth != threadedRenderer.getWidth()
                //            || mHeight != threadedRenderer.getHeight()
                //            || mNeedsRendererSetup)
                //    {
                //        threadedRenderer.setup(mWidth, mHeight, mAttachInfo,
                //                mWindowAttributes.surfaceInsets);
                //        mNeedsRendererSetup = false;
                //    }
                //}

                // TODO: In the CL "ViewRootImpl: Fix issue with early draw report in
                // seamless rotation". We moved processing of RELAYOUT_RES_BLAST_SYNC
                // earlier in the function, potentially triggering a call to
                // reportNextDraw(). That same CL changed this and the next reference
                // to wasReportNextDraw, such that this logic would remain undisturbed
                // (it continues to operate as if the code was never moved). This was
                // done to achieve a more hermetic fix for S, but it's entirely
                // possible that checking the most recent value is actually more
                // correct here.
                if (!mStopped || wasReportNextDraw)
                {
                    bool focusChangedDueToTouchMode = false; //ensureTouchModeLocally(
                                                             //(relayoutResult & WindowManagerGlobal.RELAYOUT_RES_IN_TOUCH_MODE) != 0);
                    if (focusChangedDueToTouchMode || mWidth != host.getMeasuredWidth()
                            || mHeight != host.getMeasuredHeight() || dispatchApplyInsets ||
                            updatedConfiguration)
                    {
                        int childWidthMeasureSpec = getRootMeasureSpec(mWidth, View.LayoutParams.MATCH_PARENT); // lp.width);
                        int childHeightMeasureSpec = getRootMeasureSpec(mHeight, View.LayoutParams.MATCH_PARENT); // lp.height);

                        if (DEBUG_LAYOUT) Log.v(mTag, "Ooops, something changed!  mWidth="
                                + mWidth + " measuredWidth=" + host.getMeasuredWidth()
                                + " mHeight=" + mHeight
                                + " measuredHeight=" + host.getMeasuredHeight()
                                + " dispatchApplyInsets=" + dispatchApplyInsets);

                        // Ask host how big it wants to be
                        performMeasure(childWidthMeasureSpec, childHeightMeasureSpec);

                        // Implementation of weights from WindowManager.LayoutParams
                        // We just grow the dimensions as needed and re-measure if
                        // needs be
                        int width = host.getMeasuredWidth();
                        int height = host.getMeasuredHeight();
                        bool measureAgain = false;

                        //if (lp.horizontalWeight > 0.0f)
                        //{
                        //    width += (int)((mWidth - width) * lp.horizontalWeight);
                        //    childWidthMeasureSpec = MeasureSpec.makeMeasureSpec(width,
                        //            MeasureSpec.EXACTLY);
                        //    measureAgain = true;
                        //}
                        //if (lp.verticalWeight > 0.0f)
                        //{
                        //    height += (int)((mHeight - height) * lp.verticalWeight);
                        //    childHeightMeasureSpec = MeasureSpec.makeMeasureSpec(height,
                        //            MeasureSpec.EXACTLY);
                        //    measureAgain = true;
                        //}

                        if (measureAgain)
                        {
                            if (DEBUG_LAYOUT) Log.v(mTag,
                                    "And hey let's measure once more: width=" + width
                                    + " height=" + height);
                            performMeasure(childWidthMeasureSpec, childHeightMeasureSpec);
                        }

                        layoutRequested = true;
                    }
                }
            }
            else
            {
                // Not the first pass and no window/insets/visibility change but the window
                // may have moved and we need check that and if so to update the left and right
                // in the attach info. We translate only the window frame since on window move
                // the window manager tells us only for the new frame but the insets are the
                // same and we do not want to translate them more than once.
                //maybeHandleWindowMove(frame);
            }

            if (surfaceSizeChanged || surfaceReplaced || surfaceCreated
            //|| windowAttributesChanged
            )
            {
                // If the surface has been replaced, there's a chance the bounds layer is not parented
                // to the new layer. When updating bounds layer, also reparent to the main VRI
                // SurfaceControl to ensure it's correctly placed in the hierarchy.
                //
                // This needs to be done on the client side since WMS won't reparent the children to the
                // new surface if it thinks the app is closing. WMS gets the signal that the app is
                // stopping, but on the client side it doesn't get stopped since it's restarted quick
                // enough. WMS doesn't want to keep around old children since they will leak when the
                // client creates new children.
                //prepareSurfaces();
            }

            bool didLayout = layoutRequested && (!mStopped || wasReportNextDraw);
            bool triggerGlobalLayoutListener = didLayout
                    || context.mAttachInfo.mRecomputeGlobalAttributes;
            if (didLayout)
            {
                performLayout(
                    null,
                    mWidth, mHeight
                );

                // By this point all views have been sized and positioned
                // We can compute the transparent area

                //if ((host.mPrivateFlags & View.PFLAG_REQUEST_TRANSPARENT_REGIONS) != 0)
                //{
                //    // start out transparent
                //    // TODO: AVOID THAT CALL BY CACHING THE RESULT?
                //    host.getLocationInWindow(mTmpLocation);
                //    mTransparentRegion.set(mTmpLocation[0], mTmpLocation[1],
                //            mTmpLocation[0] + host.mRight - host.mLeft,
                //            mTmpLocation[1] + host.mBottom - host.mTop);

                //    host.gatherTransparentRegion(mTransparentRegion);
                //    if (mTranslator != null)
                //    {
                //        mTranslator.translateRegionInWindowToScreen(mTransparentRegion);
                //    }

                //    if (!mTransparentRegion.equals(mPreviousTransparentRegion))
                //    {
                //        mPreviousTransparentRegion.set(mTransparentRegion);
                //        mFullRedrawNeeded = true;
                //        // TODO: Ideally we would do this in prepareSurfaces,
                //        // but prepareSurfaces is currently working under
                //        // the assumption that we paused the render thread
                //        // via the WM relayout code path. We probably eventually
                //        // want to synchronize transparent region hint changes
                //        // with draws.
                //        SurfaceControl sc = getSurfaceControl();
                //        if (sc.isValid())
                //        {
                //            mTransaction.setTransparentRegionHint(sc, mTransparentRegion).apply();
                //        }
                //    }
                //}

                if (DEBUG)
                {
                    Console.WriteLine("======================================");
                    Console.WriteLine("performTraversals -- after setFrame");
                    host.debug();
                }
            }

            // These callbacks will trigger SurfaceView SurfaceHolder.Callbacks and must be invoked
            // after the measure pass. If its invoked before the measure pass and the app modifies
            // the view hierarchy in the callbacks, we could leave the views in a broken state.
            if (surfaceCreated)
            {
                //notifySurfaceCreated();
            }
            else if (surfaceReplaced)
            {
                //notifySurfaceReplaced();
            }
            else if (surfaceDestroyed)
            {
                //notifySurfaceDestroyed();
            }

            if (triggerGlobalLayoutListener)
            {
                context.mAttachInfo.mRecomputeGlobalAttributes = false;
                context.mAttachInfo.mTreeObserver.dispatchOnGlobalLayout();
            }

            if (computesInternalInsets)
            {
                //// Clear the original insets.
                //ViewTreeObserver.InternalInsetsInfo insets = context.mAttachInfo.mGivenInternalInsets;
                //insets.reset();

                //// Compute new insets in place.
                //context.mAttachInfo.mTreeObserver.dispatchOnComputeInternalInsets(insets);
                //context.mAttachInfo.mHasNonEmptyGivenInternalInsets = !insets.isEmpty();

                //// Tell the window manager.
                //if (insetsPending || !mLastGivenInsets.equals(insets))
                //{
                //    mLastGivenInsets.set(insets);

                //    // Translate insets to screen coordinates if needed.
                //    Rect contentInsets;
                //    Rect visibleInsets;
                //    Region touchableRegion;
                //    if (mTranslator != null)
                //    {
                //        contentInsets = mTranslator.getTranslatedContentInsets(insets.contentInsets);
                //        visibleInsets = mTranslator.getTranslatedVisibleInsets(insets.visibleInsets);
                //        touchableRegion = mTranslator.getTranslatedTouchableArea(insets.touchableRegion);
                //    }
                //    else
                //    {
                //        contentInsets = insets.contentInsets;
                //        visibleInsets = insets.visibleInsets;
                //        touchableRegion = insets.touchableRegion;
                //    }

                //    try
                //    {
                //        mWindowSession.setInsets(mWindow, insets.mTouchableInsets,
                //                contentInsets, visibleInsets, touchableRegion);
                //    }
                //    catch (RemoteException e)
                //    {
                //    }
                //}
            }

            if (mFirst)
            {
                if (
                    sAlwaysAssignFocus || !true // isInTouchMode()
                )
                {
                    // handle first focus request
                    if (DEBUG_INPUT_RESIZE)
                    {
                        Log.v(mTag, "First: mView.hasFocus()=" + mView.hasFocus());
                    }
                    if (mView != null)
                    {
                        if (!mView.hasFocus())
                        {
                            mView.restoreDefaultFocus();
                            if (DEBUG_INPUT_RESIZE)
                            {
                                Log.v(mTag, "First: requested focused view=" + mView.findFocus());
                            }
                        }
                        else
                        {
                            if (DEBUG_INPUT_RESIZE)
                            {
                                Log.v(mTag, "First: existing focused view=" + mView.findFocus());
                            }
                        }
                    }
                }
                else
                {
                    // Some views (like ScrollView) won't hand focus to descendants that aren't within
                    // their viewport. Before layout, there's a good change these views are size 0
                    // which means no children can get focus. After layout, this view now has size, but
                    // is not guaranteed to hand-off focus to a focusable child (specifically, the edge-
                    // case where the child has a size prior to layout and thus won't trigger
                    // focusableViewAvailable).
                    View focused = mView.findFocus();
                    if (focused is View
                        && focused.getDescendantFocusability()
                                == View.FOCUS_AFTER_DESCENDANTS)
                    {
                        focused.restoreDefaultFocus();
                    }
                }
            }

            bool changedVisibility = (viewVisibilityChanged || mFirst) && isViewVisible;
            bool hasWindowFocus = context.mAttachInfo.mHasWindowFocus && isViewVisible;
            bool regainedFocus = hasWindowFocus && mLostWindowFocus;
            if (regainedFocus)
            {
                mLostWindowFocus = false;
            }
            else if (!hasWindowFocus && mHadWindowFocus)
            {
                mLostWindowFocus = true;
            }

            if (changedVisibility || regainedFocus)
            {
                // Toasts are presented as notifications - don't present them as windows as well
                //bool isToast = mWindowAttributes.type == TYPE_TOAST;
                //if (!isToast)
                //{
                //host.sendAccessibilityEvent(AccessibilityEvent.TYPE_WINDOW_STATE_CHANGED);
                //}
            }

            mFirst = false;
            mWillDrawSoon = false;
            mNewSurfaceNeeded = false;
            mActivityRelaunched = false;
            mViewVisibility = viewVisibility;
            mHadWindowFocus = hasWindowFocus;

            //mImeFocusController.onTraversal(hasWindowFocus, mWindowAttributes);

            // Remember if we must report the next draw.
            if (
                windowSizeMayChange
            //(relayoutResult & WindowManagerGlobal.RELAYOUT_RES_FIRST_TIME) != 0
            )
            {
                reportNextDraw();
            }

            bool cancelDraw = context.mAttachInfo.mTreeObserver.dispatchOnPreDraw() || !isViewVisible;

            if (!cancelDraw)
            {
                if (mPendingTransitions != null && mPendingTransitions.Count > 0)
                {
                    for (int i = 0; i < mPendingTransitions.Count; ++i)
                    {
                        mPendingTransitions.ElementAt(i).startChangingAnimations();
                    }
                    mPendingTransitions.Clear();
                }
                performDraw(canvas);
            }
            else
            {
                if (isViewVisible)
                {
                    // Try again
                    scheduleTraversals();
                }
                else
                {
                    if (mPendingTransitions != null && mPendingTransitions.Count > 0)
                    {
                        for (int i = 0; i < mPendingTransitions.Count; ++i)
                        {
                            mPendingTransitions.ElementAt(i).endChangingAnimations();
                        }
                        mPendingTransitions.Clear();
                    }

                    // We may never draw since it's not visible. Report back that we're finished
                    // drawing.
                    if (!wasReportNextDraw && mReportNextDraw)
                    {
                        mReportNextDraw = false;
                        pendingDrawFinished();
                    }
                }
            }

            //if (context.mAttachInfo.mContentCaptureEvents != null)
            //{
            //    notifyContentCatpureEvents();
            //}

            mIsInTraversal = false;
        }

        /**
         * A count of the number of calls to pendingDrawFinished we
         * require to notify the WM drawing is complete.
         */
        int mDrawsNeededToReport = 0;
        internal int mScrollY;
        internal int mCurScrollY;
        internal Rect mDirty = new();
        internal int mCanvasOffsetX;
        internal int mCanvasOffsetY;
        internal bool mIsAnimating;
        internal bool mInvalidateRootRequested;
        internal bool mAppVisible = false;
        List<LayoutTransition> mPendingTransitions;

        internal sealed class InvalidateOnAnimationRunnable
        {

            private bool mPosted;
            readonly object LOCK = new();
            private List<View> mViews = new();
            private readonly List<View.AttachInfo.InvalidateInfo> mViewRects = new();
            private View[] mTempViews;
            private View.AttachInfo.InvalidateInfo[] mTempViewRects;
            ViewRootImpl impl;
            Runnable run;

            public InvalidateOnAnimationRunnable(ViewRootImpl impl)
            {
                this.impl = impl;
                run = () => {
                    int viewCount;
                    int viewRectCount;
                    lock (LOCK)
                    {
                        mPosted = false;

                        viewCount = mViews.Count;
                        if (viewCount != 0)
                        {
                            mTempViews = mViews.ToArray();
                            mViews.Clear();
                        }

                        viewRectCount = mViewRects.Count;
                        if (viewRectCount != 0)
                        {
                            mTempViewRects = mViewRects.ToArray();
                            mViewRects.Clear();
                        }
                    }

                    for (int i = 0; i < viewCount; i++)
                    {
                        mTempViews[i].invalidate();
                        mTempViews[i] = null;
                    }

                    for (int i = 0; i < viewRectCount; i++)
                    {
                        View.AttachInfo.InvalidateInfo info = mTempViewRects[i];
                        info.target.invalidate(info.left, info.top, info.right, info.bottom);
                        info.recycle();
                    }
                };
            }

            public void addView(View view)
            {
                lock (LOCK)
                {
                    mViews.Add(view);
                    postIfNeededLocked();
                }
            }

            public void addViewRect(View.AttachInfo.InvalidateInfo info)
            {
                lock (LOCK)
                {
                    mViewRects.Add(info);
                    postIfNeededLocked();
                }
            }

            public void removeView(View view)
            {
                lock (LOCK)
                {
                    mViews.Remove(view);

                    for (int i = mViewRects.Count; i-- > 0;)
                    {
                        View.AttachInfo.InvalidateInfo info = mViewRects.ElementAt(i);
                        if (info.target == view)
                        {
                            mViewRects.RemoveAt(i);
                            info.recycle();
                        }
                    }

                    if (mPosted && mViews.Count == 0 && mViewRects.Count == 0)
                    {
                        // no cheographer
                        impl.mHandler.removeCallbacks(run, View.CALLBACK_ANIMATION);

                        //mChoreographer.removeCallbacks(Choreographer.CALLBACK_ANIMATION, this, null);
                        mPosted = false;
                    }
                }
            }

            private void postIfNeededLocked()
            {
                if (!mPosted)
                {
                    // no cheographer
                    impl.mHandler.post(run, View.CALLBACK_ANIMATION);

                    //mChoreographer.postCallback(Choreographer.CALLBACK_ANIMATION, this, null);
                    mPosted = true;
                }
            }
        }
        internal readonly InvalidateOnAnimationRunnable mInvalidateOnAnimationRunnable;

        private const int MSG_INVALIDATE = 1;
        private const int MSG_INVALIDATE_RECT = 2;

        sealed class ViewRootHandler : Handler
        {
            public ViewRootHandler(Context context) : base(context)
            {
            }

            public ViewRootHandler(Context context, Callback callback) : base(context, callback)
            {
            }

            public ViewRootHandler(Looper looper) : base(looper)
            {
            }

            public ViewRootHandler(Looper looper, Callback callback) : base(looper, callback)
            {
            }

            internal ViewRootHandler(Context context, bool async) : base(context, async)
            {
            }

            internal ViewRootHandler(Context context, Callback callback, bool async) : base(context, callback, async)
            {
            }

            internal ViewRootHandler(Looper looper, Callback callback, bool async) : base(looper, callback, async)
            {
            }

            override
            public string getMessageName(Message message)
            {
                switch (message.what)
                {
                    case MSG_INVALIDATE:
                        return "MSG_INVALIDATE";
                    case MSG_INVALIDATE_RECT:
                        return "MSG_INVALIDATE_RECT";
                }
                return base.getMessageName(message);
            }

            override
            public void handleMessage(Message msg)
            {
                //if (Trace.isTagEnabled(Trace.TRACE_TAG_VIEW)) {
                //    Trace.traceBegin(Trace.TRACE_TAG_VIEW, getMessageName(msg));
                //}
                try
                {
                    handleMessageImpl(msg);
                }
                finally
                {
                    //Trace.traceEnd(Trace.TRACE_TAG_VIEW);
                }
            }

            private void handleMessageImpl(Message msg)
            {
                switch (msg.what)
                {
                    case MSG_INVALIDATE:
                        ((View)msg.obj).invalidate();
                        break;
                    case MSG_INVALIDATE_RECT:
                        View.AttachInfo.InvalidateInfo info =
                                (View.AttachInfo.InvalidateInfo)msg.obj;
                        info.target.invalidate(info.left, info.top, info.right, info.bottom);
                        info.recycle();
                        break;
                }
            }
        }

        ViewRootHandler mHandler;
        internal Thread mThread;

        internal void dispatchInvalidateDelayed(View view, long delayMilliseconds)
        {
            Message msg = mHandler.obtainMessage(MSG_INVALIDATE, view);
            mHandler.sendMessageDelayed(msg, delayMilliseconds);
        }

        internal void dispatchInvalidateRectDelayed(View.AttachInfo.InvalidateInfo info,
                long delayMilliseconds)
        {
            Message msg = mHandler.obtainMessage(MSG_INVALIDATE_RECT, info);
            mHandler.sendMessageDelayed(msg, delayMilliseconds);
        }

        internal void dispatchInvalidateOnAnimation(View view)
        {
            mInvalidateOnAnimationRunnable.addView(view);
        }

        internal void dispatchInvalidateRectOnAnimation(View.AttachInfo.InvalidateInfo info)
        {
            mInvalidateOnAnimationRunnable.addViewRect(info);
        }

        public void cancelInvalidate(View view)
        {
            mHandler.removeMessages(MSG_INVALIDATE, view);
            // fixme: might leak the AttachInfo.InvalidateInfo objects instead of returning
            // them to the pool
            mHandler.removeMessages(MSG_INVALIDATE_RECT, view);
            mInvalidateOnAnimationRunnable.removeView(view);
        }

        LinearLayout holder;
        FrameLayout holderApp;
        bool optionsIsShowing;
        View options_page;

        public ViewRootImpl(Context context, Looper looper)
        {
            this.context = context;
            mFirst = true;
            mView = new()
            {
                mLayoutParams = View.MATCH_PARENT_W__MATCH_PARENT_H
            };
            mView.assignParent(this);
            mView.mAttachInfo = context.mAttachInfo;
            context.mAttachInfo.mRootView = mView;
            mThread = Thread.CurrentThread;
            mViewVisibility = View.GONE;
            mInvalidateOnAnimationRunnable = new InvalidateOnAnimationRunnable(this);
            mHandler = new(looper);
            context.mAttachInfo.mHandler = mHandler;

            holder = new();
            holderApp = new();
            FrameLayout options = new();
            Topten_RichTextKit_TextView optionsText = new();
            var l = new FrameLayout.LayoutParams(WRAP_CONTENT, WRAP_CONTENT);
            l.gravity = Gravity.CENTER;
            options.addView(optionsText, l);
            optionsText.setTextColor(Color.BLACK);
            optionsText.setText("AndroidUI Options");
            options.setOnClickListener(v =>
            {
                if (!optionsIsShowing)
                {
                    optionsIsShowing = true;
                    holder.removeView(holderApp);
                    holder.addView(options_page, View.MATCH_PARENT_W__MATCH_PARENT_H);
                    holder.invalidate();
                }
                else
                {
                    optionsIsShowing = false;
                    holder.removeView(options_page);
                    holder.addView(holderApp, View.MATCH_PARENT_W__MATCH_PARENT_H);
                    holder.invalidate();
                }
            });
            options.setZ(float.PositiveInfinity);
            holder.addView(options, Widgets.View.MATCH_PARENT_W__WRAP_CONTENT_H);
            holder.addView(holderApp, View.MATCH_PARENT_W__MATCH_PARENT_H);
            mView.addView(holder, View.MATCH_PARENT_W__MATCH_PARENT_H);

            LinearLayout CreateSettingsRow(string title, string enabledText, string disabledText, RunnableWithReturn<bool> GetValue, Runnable<bool> SetValue)
            {
                LinearLayout row = new();

                row.setOrientation(LinearLayout.OrientationMode.HORIZONTAL);

                var lp = new View.LayoutParams(MATCH_PARENT, WRAP_CONTENT);
                row.setLayoutParams(lp);

                var titleView = new Topten_RichTextKit_TextView();
                var valueView = new Topten_RichTextKit_TextView();

                titleView.setTextColor(Color.WHITE);
                valueView.setTextColor(Color.WHITE);

                titleView.setText(title);
                titleView.setTextSize(20);
                valueView.setTextSize(20);

                bool value = GetValue.Invoke();
                valueView.setText(value ? enabledText : disabledText);
                valueView.setTextColor(value ? Color.GREEN : Color.RED);

                LinearLayout.LayoutParams layout_params = new LinearLayout.LayoutParams(MATCH_PARENT, WRAP_CONTENT, 1);
                layout_params.gravity = Gravity.CENTER;

                row.addView(titleView, layout_params);

                LinearLayout.LayoutParams layout_params1 = new LinearLayout.LayoutParams(MATCH_PARENT, WRAP_CONTENT, 2);
                layout_params1.gravity = Gravity.CENTER;

                row.addView(valueView, layout_params1);

                row.setOnClickListener(v =>
                {
                    bool value = !GetValue.Invoke();
                    SetValue.Invoke(value);
                    valueView.setText(value ? enabledText : disabledText);
                    valueView.setTextColor(value ? Color.GREEN : Color.RED);
                    invalidate();
                });

                return row;
            }

            LinearLayout optionsPage = new LinearLayout();
            optionsPage.addView(CreateSettingsRow("Show Layout Bounds", "Enabled", "Disabled", () => context.mAttachInfo.mDebugLayout, value => { lock (context.mAttachInfo) { context.mAttachInfo.mDebugLayout = value; } }));
            optionsPage.addView(CreateSettingsRow("Skia: Allocation Logging (expensive)", "Enabled", "Disabled", () => SkiaSharp.SKNativeObject.LOG_ALLOCATIONS, value => SKNativeObject.LOG_ALLOCATIONS = value));
            optionsPage.addView(CreateSettingsRow("Touch: Debug", "Enabled", "Disabled", () => Touch.DEBUG, value => Touch.DEBUG = value));
            optionsPage.addView(CreateSettingsRow("Touch: Debug - show TOUCH_MOVE events\n(requires [Touch: Debug] to be Enabled) (can flood)", "Enabled", "Disabled", () => Touch.PRINT_MOVED, value => Touch.PRINT_MOVED = value).setTagRecursively("TOUCH DEBUG"));
            optionsPage.addView(CreateSettingsRow("Touch Batcher: Debug", "Enabled", "Disabled", () => Touch.Batcher.DEBUG, value => Touch.Batcher.DEBUG = value));
            optionsPage.addView(CreateSettingsRow("View: Debug", "Enabled", "Disabled", () => View.DEBUG, value => View.DEBUG = value));
            optionsPage.addView(CreateSettingsRow("View: Debug View Allocation", "Enabled", "Disabled", () => View.DEBUG_VIEW_ALLOCATION, value => View.DEBUG_VIEW_ALLOCATION = value));
            optionsPage.addView(CreateSettingsRow("View: Debug Measure Child", "Enabled", "Disabled", () => View.DEBUG_MEASURE_CHILD, value => View.DEBUG_MEASURE_CHILD = value));
            optionsPage.addView(CreateSettingsRow("View: Debug Invalidation", "Enabled", "Disabled", () => View.DEBUG_INVALIDATION, value => View.DEBUG_INVALIDATION = value));
            optionsPage.addView(CreateSettingsRow("View: Debug View Tracking", "Enabled", "Disabled", () => View.DEBUG_VIEW_TRACKING, value => View.DEBUG_VIEW_TRACKING = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug (expensive)", "Enabled", "Disabled", () => DEBUG, value => getRunQueue().post(() => DEBUG = value)));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug FPS", "Enabled", "Disabled", () => DEBUG_FPS, value => DEBUG_FPS = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Layout", "Enabled", "Disabled", () => DEBUG_LAYOUT, value => DEBUG_LAYOUT = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Draw", "Enabled", "Disabled", () => DEBUG_DRAW, value => DEBUG_DRAW = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Measure/Layout/Draw Time", "Enabled", "Disabled", () => DEBUG_MEASURE_LAYOUT_DRAW_TIME, value => DEBUG_MEASURE_LAYOUT_DRAW_TIME = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Orientation", "Enabled", "Disabled", () => DEBUG_ORIENTATION, value => DEBUG_ORIENTATION = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Input Resize", "Enabled", "Disabled", () => DEBUG_INPUT_RESIZE, value => DEBUG_INPUT_RESIZE = value));
            optionsPage.addView(CreateSettingsRow("ViewRootImpl: Debug Blast", "Enabled", "Disabled", () => DEBUG_BLAST, value => DEBUG_BLAST = value));

            FlywheelScrollView sv = new FlywheelScrollView();
            sv.addView(optionsPage, View.MATCH_PARENT_W__WRAP_CONTENT_H);
            options_page = sv;

            options.setBackgroundColor(Color.GRAY);
            options_page.setBackgroundColor(Color.BLACK);
        }

        public void DestroyHandler()
        {
            if (mHandler != null)
            {
                context.mAttachInfo.mHandler = null;
                mHandler = null;
            }
        }

        ~ViewRootImpl()
        {
            DestroyHandler();
        }

        internal void handleAppVisibility(bool visible)
        {
            if (mAppVisible != visible)
            {
                mAppVisible = visible;
                mAppVisibilityChanged = true;
                scheduleTraversals();
            }
        }

        /**
         * Delay notifying WM of draw finished until
         * a balanced call to pendingDrawFinished.
         */
        void drawPending()
        {
            mDrawsNeededToReport++;
        }

        void pendingDrawFinished()
        {
            if (mDrawsNeededToReport == 0)
            {
                throw new Exception("Unbalanced drawPending/pendingDrawFinished calls");
            }
            mDrawsNeededToReport--;
            if (mDrawsNeededToReport == 0)
            {
                reportDrawFinished();
            }
            else if (DEBUG_BLAST)
            {
                Log.d(mTag, "pendingDrawFinished. Waiting on draw reported mDrawsNeededToReport="
                        + mDrawsNeededToReport);
            }
        }

        private void reportDrawFinished()
        {
            try
            {
                if (DEBUG_BLAST)
                {
                    Log.d(mTag, "reportDrawFinished");
                }
                mDrawsNeededToReport = 0;
                //mWindowSession.finishDrawing(mWindow, mSurfaceChangedTransaction);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to report draw finished", e);
                //mSurfaceChangedTransaction.apply();
            }
            finally
            {
                //mSurfaceChangedTransaction.clear();
            }
        }

        private void reportNextDraw()
        {
            if (mReportNextDraw == false)
            {
                drawPending();
            }
            mReportNextDraw = true;
        }

        private void performDraw(SKCanvas canvas)
        {
            if (!mReportNextDraw || mView == null)
            {
                return;
            }

            if (DEBUG_DRAW)
            {
                Log.v(mTag, "STARTED DRAWING: " + mView);
            }

            bool fullRedrawNeeded =
                    mFullRedrawNeeded || mReportNextDraw //|| mNextDrawUseBlastSync
                    ;
            mFullRedrawNeeded = false;

            mIsDrawing = true;
            //Trace.traceBegin(Trace.TRACE_TAG_VIEW, "draw");

            //bool usingAsyncReport = addFrameCompleteCallbackIfNeeded();
            //addFrameCallbackIfNeeded();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                bool canUseAsync = draw(canvas, fullRedrawNeeded);
                //if (usingAsyncReport && !canUseAsync)
                //{
                //    context.mAttachInfo.mThreadedRenderer.setFrameCompleteCallback(null);
                //    usingAsyncReport = false;
                //}
            }
            else
            {
                try
                {
                    bool canUseAsync = draw(canvas, fullRedrawNeeded);
                    //if (usingAsyncReport && !canUseAsync)
                    //{
                    //    context.mAttachInfo.mThreadedRenderer.setFrameCompleteCallback(null);
                    //    usingAsyncReport = false;
                    //}
                }
                catch (Exception e)
                {
                    Log.v(mTag, "Caught exception while drawing view: " + e);
                }
            }
            mIsDrawing = false;
            //Trace.traceEnd(Trace.TRACE_TAG_VIEW);

            // For whatever reason we didn't create a HardwareRenderer, end any
            // hardware animations that are now dangling
            if (context.mAttachInfo.mPendingAnimatingRenderNodes != null)
            {
                int count = context.mAttachInfo.mPendingAnimatingRenderNodes.Count;
                for (int i = 0; i < count; i++)
                {
                    //context.mAttachInfo.mPendingAnimatingRenderNodes.ElementAt(i).endAllAnimators();
                }
                context.mAttachInfo.mPendingAnimatingRenderNodes.Clear();
            }

            if (mReportNextDraw)
            {
                mReportNextDraw = false;

                // if we're using multi-thread renderer, wait for the window frame draws
                //if (mWindowDrawCountDown != null)
                //{
                //    try
                //    {
                //        mWindowDrawCountDown.await();
                //    }
                //    catch (InterruptedException e)
                //    {
                //        Log.e(mTag, "Window redraw count down interrupted!");
                //    }
                //    mWindowDrawCountDown = null;
                //}

                //if (context.mAttachInfo.mThreadedRenderer != null)
                //{
                //    context.mAttachInfo.mThreadedRenderer.setStopped(mStopped);
                //}

                if (DEBUG_DRAW)
                {
                    Log.v(mTag, "FINISHED DRAWING: " + mView);
                }

                //if (mSurfaceHolder != null && mSurface.isValid())
                //{
                //    SurfaceCallbackHelper sch = new SurfaceCallbackHelper(this::postDrawFinished);
                //    SurfaceHolder.Callback callbacks[] = mSurfaceHolder.getCallbacks();

                //    sch.dispatchSurfaceRedrawNeededAsync(mSurfaceHolder, callbacks);
                //}
                //else if (!usingAsyncReport)
                //{
                //    if (context.mAttachInfo.mThreadedRenderer != null)
                //    {
                //        context.mAttachInfo.mThreadedRenderer.fence();
                //    }
                //    pendingDrawFinished();
                //}
            }
            //if (mPerformContentCapture)
            //{
            //    performContentCaptureInitialReport();
            //}
        }

        /**
         * Called by {@link android.view.View#requestLayout()} if the view hierarchy is currently
         * undergoing a layout pass. requestLayout() should not generally be called during layout,
         * unless the container hierarchy knows what it is doing (i.e., it is fine as long as
         * all children in that container hierarchy are measured and laid out at the end of the layout
         * pass for that container). If requestLayout() is called anyway, we handle it correctly
         * by registering all requesters during a frame as it proceeds. At the end of the frame,
         * we check all of those views to see if any still have pending layout requests, which
         * indicates that they were not correctly handled by their container hierarchy. If that is
         * the case, we clear all such flags in the tree, to remove the buggy flag state that leads
         * to blank containers, and force a second request/measure/layout pass in this frame. If
         * more requestLayout() calls are received during that second layout pass, we post those
         * requests to the next frame to avoid possible infinite loops.
         *
         * <p>The return value from this method indicates whether the request should proceed
         * (if it is a request during the first layout pass) or should be skipped and posted to the
         * next frame (if it is a request during the second layout pass).</p>
         *
         * @param view the view that requested the layout.
         *
         * @return true if request should proceed, false otherwise.
         */
        internal bool requestLayoutDuringLayout(View view)
        {
            if (view.mParent == null || view.mAttachInfo == null)
            {
                // Would not normally trigger another layout, so just let it pass through as usual
                return true;
            }
            if (!mLayoutRequesters.Contains(view))
            {
                mLayoutRequesters.Add(view);
            }
            if (!mHandlingLayoutInLayoutRequest)
            {
                // Let the request proceed normally; it will be processed in a second layout pass
                // if necessary
                return true;
            }
            else
            {
                // Don't let the request proceed during the second layout pass.
                // It will post to the next frame instead.
                return false;
            }
        }

        private void performLayout(
            object lp_unused //WindowManager.LayoutParams lp
            , int desiredWindowWidth, int desiredWindowHeight)
        {
            mScrollMayChange = true;
            mInLayout = true;

            View host = mView;
            if (host == null)
            {
                return;
            }
            if (DEBUG_ORIENTATION || DEBUG_LAYOUT)
            {
                Log.v(mTag, "Laying out " + host + " to (" +
                        host.getMeasuredWidth() + ", " + host.getMeasuredHeight() + ")");
            }

            //Trace.traceBegin(Trace.TRACE_TAG_VIEW, "layout");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                host.layout(0, 0, host.getMeasuredWidth(), host.getMeasuredHeight());

                mInLayout = false;
                int numViewsRequestingLayout = mLayoutRequesters.Count;
                if (numViewsRequestingLayout > 0)
                {
                    // requestLayout() was called during layout.
                    // If no layout-request flags are set on the requesting views, there is no problem.
                    // If some requests are still pending, then we need to clear those flags and do
                    // a full request/measure/layout pass to handle this situation.
                    List<View> validLayoutRequesters = getValidLayoutRequesters(mLayoutRequesters,
                            false);
                    if (validLayoutRequesters != null)
                    {
                        // Set this flag to indicate that any further requests are happening during
                        // the second pass, which may result in posting those requests to the next
                        // frame instead
                        mHandlingLayoutInLayoutRequest = true;

                        // Process fresh layout requests, then measure and layout
                        int numValidRequests = validLayoutRequesters.Count;
                        for (int i = 0; i < numValidRequests; ++i)
                        {
                            View view = validLayoutRequesters.ElementAt(i);
                            Console.WriteLine(mTag, "requestLayout() improperly called by \"" + view +
                                    "\" during layout: running second layout pass");
                            view.requestLayout();
                        }
                        measureHierarchy(host, null, null,
                                desiredWindowWidth, desiredWindowHeight);
                        mInLayout = true;
                        host.layout(0, 0, host.getMeasuredWidth(), host.getMeasuredHeight());

                        mHandlingLayoutInLayoutRequest = false;

                        // Check the valid requests again, this time without checking/clearing the
                        // layout flags, since requests happening during the second pass get noop'd
                        validLayoutRequesters = getValidLayoutRequesters(mLayoutRequesters, true);
                        if (validLayoutRequesters != null)
                        {
                            List<View> finalRequesters = validLayoutRequesters;
                            // Post second-pass requests to the next frame
                            getRunQueue().post(() =>
                            {
                                int numValidRequests = finalRequesters.Count;
                                for (int i = 0; i < numValidRequests; ++i)
                                {
                                    View view = finalRequesters.ElementAt(i);
                                    Log.w("View", "requestLayout() improperly called by " + view +
                                            " during second layout pass: posting in next frame");
                                    view.requestLayout();
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                try
                {
                    host.layout(0, 0, host.getMeasuredWidth(), host.getMeasuredHeight());

                    mInLayout = false;
                    int numViewsRequestingLayout = mLayoutRequesters.Count;
                    if (numViewsRequestingLayout > 0)
                    {
                        // requestLayout() was called during layout.
                        // If no layout-request flags are set on the requesting views, there is no problem.
                        // If some requests are still pending, then we need to clear those flags and do
                        // a full request/measure/layout pass to handle this situation.
                        List<View> validLayoutRequesters = getValidLayoutRequesters(mLayoutRequesters,
                                false);
                        if (validLayoutRequesters != null)
                        {
                            // Set this flag to indicate that any further requests are happening during
                            // the second pass, which may result in posting those requests to the next
                            // frame instead
                            mHandlingLayoutInLayoutRequest = true;

                            // Process fresh layout requests, then measure and layout
                            int numValidRequests = validLayoutRequesters.Count;
                            for (int i = 0; i < numValidRequests; ++i)
                            {
                                View view = validLayoutRequesters.ElementAt(i);
                                Console.WriteLine(mTag, "requestLayout() improperly called by " + view +
                                        " during layout: running second layout pass");
                                view.requestLayout();
                            }
                            measureHierarchy(host, null, null,
                                    desiredWindowWidth, desiredWindowHeight);
                            mInLayout = true;
                            host.layout(0, 0, host.getMeasuredWidth(), host.getMeasuredHeight());

                            mHandlingLayoutInLayoutRequest = false;

                            // Check the valid requests again, this time without checking/clearing the
                            // layout flags, since requests happening during the second pass get noop'd
                            validLayoutRequesters = getValidLayoutRequesters(mLayoutRequesters, true);
                            if (validLayoutRequesters != null)
                            {
                                List<View> finalRequesters = validLayoutRequesters;
                                // Post second-pass requests to the next frame
                                getRunQueue().post(() =>
                                {
                                    int numValidRequests = finalRequesters.Count;
                                    for (int i = 0; i < numValidRequests; ++i)
                                    {
                                        View view = finalRequesters.ElementAt(i);
                                        Log.w("View", "requestLayout() improperly called by " + view +
                                                " during second layout pass: posting in next frame");
                                        view.requestLayout();
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.v(mTag, "Caught exception while laying out view: " + e);
                }
            }
            //Trace.traceEnd(Trace.TRACE_TAG_VIEW);
            mInLayout = false;
        }

        /**
         * This method is called during layout when there have been calls to requestLayout() during
         * layout. It walks through the list of views that requested layout to determine which ones
         * still need it, based on visibility in the hierarchy and whether they have already been
         * handled (as is usually the case with ListView children).
         *
         * @param layoutRequesters The list of views that requested layout during layout
         * @param secondLayoutRequests Whether the requests were issued during the second layout pass.
         * If so, the FORCE_LAYOUT flag was not set on requesters.
         * @return A list of the actual views that still need to be laid out.
         */
        private List<View> getValidLayoutRequesters(List<View> layoutRequesters,
                bool secondLayoutRequests)
        {

            int numViewsRequestingLayout = layoutRequesters.Count;
            List<View> validLayoutRequesters = null;
            for (int i = 0; i < numViewsRequestingLayout; ++i)
            {
                View view = layoutRequesters.ElementAt(i);
                if (view != null && view.mAttachInfo != null && view.mParent != null &&
                        (secondLayoutRequests || (view.mPrivateFlags & View.PFLAG_FORCE_LAYOUT) ==
                                View.PFLAG_FORCE_LAYOUT))
                {
                    bool gone = false;
                    View parent = view;
                    // Only trigger new requests for views in a non-GONE hierarchy
                    while (parent != null)
                    {
                        if ((parent.mViewFlags & View.VISIBILITY_MASK) == View.GONE)
                        {
                            gone = true;
                            break;
                        }
                        if (parent.mParent is View)
                        {
                            parent = (View)parent.mParent;
                        }
                        else
                        {
                            parent = null;
                        }
                    }
                    if (!gone)
                    {
                        if (validLayoutRequesters == null)
                        {
                            validLayoutRequesters = new List<View>();
                        }
                        validLayoutRequesters.Add(view);
                    }
                }
            }
            if (!secondLayoutRequests)
            {
                // If we're checking the layout flags, then we need to clean them up also
                for (int i = 0; i < numViewsRequestingLayout; ++i)
                {
                    View view = layoutRequesters.ElementAt(i);
                    while (view != null &&
                            (view.mPrivateFlags & View.PFLAG_FORCE_LAYOUT) != 0)
                    {
                        view.mPrivateFlags &= ~View.PFLAG_FORCE_LAYOUT;
                        if (view.mParent is View)
                        {
                            view = (View)view.mParent;
                        }
                        else
                        {
                            view = null;
                        }
                    }
                }
            }
            layoutRequesters.Clear();
            return validLayoutRequesters;
        }

        bool scrollToRectOrFocus(Rect rectangle, bool immediate)
        {
            //Rect ci = context.mAttachInfo.mContentInsets;
            //Rect vi = context.mAttachInfo.mVisibleInsets;
            int scrollY = 0;
            bool handled = false;

            //if (vi.left > ci.left || vi.top > ci.top
            //        || vi.right > ci.right || vi.bottom > ci.bottom)
            //{
            //    // We'll assume that we aren't going to change the scroll
            //    // offset, since we want to avoid that unless it is actually
            //    // going to make the focus visible...  otherwise we scroll
            //    // all over the place.
            //    scrollY = mScrollY;
            //    // We can be called for two different situations: during a draw,
            //    // to update the scroll position if the focus has changed (in which
            //    // case 'rectangle' is null), or in response to a
            //    // requestChildRectangleOnScreen() call (in which case 'rectangle'
            //    // is non-null and we just want to scroll to whatever that
            //    // rectangle is).
            //    View focus = mView.findFocus();
            //    if (focus == null)
            //    {
            //        return false;
            //    }
            //    View lastScrolledFocus = (mLastScrolledFocus != null) ? mLastScrolledFocus.get() : null;
            //    if (focus != lastScrolledFocus)
            //    {
            //        // If the focus has changed, then ignore any requests to scroll
            //        // to a rectangle; first we want to make sure the entire focus
            //        // view is visible.
            //        rectangle = null;
            //    }
            //    if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Eval scroll: focus=" + focus
            //            + " rectangle=" + rectangle + " ci=" + ci
            //            + " vi=" + vi);
            //    if (focus == lastScrolledFocus && !mScrollMayChange && rectangle == null)
            //    {
            //        // Optimization: if the focus hasn't changed since last
            //        // time, and no layout has happened, then just leave things
            //        // as they are.
            //        if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Keeping scroll y="
            //                + mScrollY + " vi=" + vi.toShortString());
            //    }
            //    else
            //    {
            //        // We need to determine if the currently focused view is
            //        // within the visible part of the window and, if not, apply
            //        // a pan so it can be seen.
            //        mLastScrolledFocus = new WeakReference<View>(focus);
            //        mScrollMayChange = false;
            //        if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Need to scroll?");
            //        // Try to find the rectangle from the focus view.
            //        if (focus.getGlobalVisibleRect(mVisRect, null))
            //        {
            //            if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Root w="
            //                    + mView.getWidth() + " h=" + mView.getHeight()
            //                    + " ci=" + ci.toShortString()
            //                    + " vi=" + vi.toShortString());
            //            if (rectangle == null)
            //            {
            //                focus.getFocusedRect(mTempRect);
            //                if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Focus " + focus
            //                        + ": focusRect=" + mTempRect.toShortString());
            //                if (mView instanceof ViewGroup) {
            //                    ((ViewGroup)mView).offsetDescendantRectToMyCoords(
            //                            focus, mTempRect);
            //                }
            //                if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                        "Focus in window: focusRect="
            //                        + mTempRect.toShortString()
            //                        + " visRect=" + mVisRect.toShortString());
            //            }
            //            else
            //            {
            //                mTempRect.set(rectangle);
            //                if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                        "Request scroll to rect: "
            //                        + mTempRect.toShortString()
            //                        + " visRect=" + mVisRect.toShortString());
            //            }
            //            if (mTempRect.intersect(mVisRect))
            //            {
            //                if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                        "Focus window visible rect: "
            //                        + mTempRect.toShortString());
            //                if (mTempRect.height() >
            //                        (mView.getHeight() - vi.top - vi.bottom))
            //                {
            //                    // If the focus simply is not going to fit, then
            //                    // best is probably just to leave things as-is.
            //                    if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                            "Too tall; leaving scrollY=" + scrollY);
            //                }
            //                // Next, check whether top or bottom is covered based on the non-scrolled
            //                // position, and calculate new scrollY (or set it to 0).
            //                // We can't keep using mScrollY here. For example mScrollY is non-zero
            //                // due to IME, then IME goes away. The current value of mScrollY leaves top
            //                // and bottom both visible, but we still need to scroll it back to 0.
            //                else if (mTempRect.top < vi.top)
            //                {
            //                    scrollY = mTempRect.top - vi.top;
            //                    if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                            "Top covered; scrollY=" + scrollY);
            //                }
            //                else if (mTempRect.bottom > (mView.getHeight() - vi.bottom))
            //                {
            //                    scrollY = mTempRect.bottom - (mView.getHeight() - vi.bottom);
            //                    if (DEBUG_INPUT_RESIZE) Log.v(mTag,
            //                            "Bottom covered; scrollY=" + scrollY);
            //                }
            //                else
            //                {
            //                    scrollY = 0;
            //                }
            //                handled = true;
            //            }
            //        }
            //    }
            //}

            if (scrollY != mScrollY)
            {
                if (DEBUG_INPUT_RESIZE) Log.v(mTag, "Pan scroll changed: old="
                        + mScrollY + " , new=" + scrollY);
                if (!immediate)
                {
                    //if (mScroller == null)
                    //{
                    //    mScroller = new Scroller(mView.getContext());
                    //}
                    //mScroller.startScroll(0, mScrollY, 0, scrollY - mScrollY);
                }
                //else if (mScroller != null)
                //{
                //    mScroller.abortAnimation();
                //}
                mScrollY = scrollY;
            }

            return handled;
        }

        private bool draw(SKCanvas canvas, bool fullRedrawNeeded)
        {
            // our surface is always valid

            //Surface surface = mSurface;
            //if (!surface.isValid())
            //{
            //return false;
            //}

            if (DEBUG_FPS)
            {
                trackFPS();
            }

            if (!sFirstDrawComplete)
            {
                var firstdrawHandlers = sFirstDrawHandlers.Value;
                lock (firstdrawHandlers)
                {
                    sFirstDrawComplete.Value = true;
                    int count = firstdrawHandlers.Count;
                    for (int i = 0; i < count; i++)
                    {
                        mHandler.post(firstdrawHandlers.ElementAt(i));
                    }
                }
            }

            scrollToRectOrFocus(null, false);

            if (context.mAttachInfo.mViewScrollChanged)
            {
                context.mAttachInfo.mViewScrollChanged = false;
                context.mAttachInfo.mTreeObserver.dispatchOnScrollChanged();
            }

            bool animating = false; // mScroller != null && mScroller.computeScrollOffset();
            int curScrollY = 0;
            if (animating)
            {
                //curScrollY = mScroller.getCurrY();
            }
            else
            {
                curScrollY = mScrollY;
            }
            if (mCurScrollY != curScrollY)
            {
                mCurScrollY = curScrollY;
                fullRedrawNeeded = true;
                //if (mView in RootViewSurfaceTaker) {
                //((RootViewSurfaceTaker)mView).onRootViewScrollYChanged(mCurScrollY);
                //}
            }

            float appScale = context.mAttachInfo.mApplicationScale;
            bool scalingRequired = context.mAttachInfo.mScalingRequired;

            Rect dirty = mDirty;
            //if (mSurfaceHolder != null)
            //{
            // The app owns the surface, we won't draw.
            //dirty.setEmpty();
            //if (animating && mScroller != null)
            //{
            //mScroller.abortAnimation();
            //}
            //return false;
            //}

            if (fullRedrawNeeded)
            {
                dirty.set(0, 0, (int)(mWidth * appScale + 0.5f), (int)(mHeight * appScale + 0.5f));
            }

            if (DEBUG_ORIENTATION || DEBUG_DRAW)
            {
                Log.v(mTag, "Draw " + mView
                        + ": dirty={" + dirty.left + "," + dirty.top
                        + "," + dirty.right + "," + dirty.bottom + "} , appScale:" +
                        appScale + ", width=" + mWidth + ", height=" + mHeight);
            }

            context.mAttachInfo.mTreeObserver.dispatchOnDraw();

            int xOffset = -mCanvasOffsetX;
            int yOffset = -mCanvasOffsetY + curScrollY;
            //WindowManager.LayoutParams params = mWindowAttributes;
            Rect surfaceInsets = null; //params != null ? params.surfaceInsets: null;
            if (surfaceInsets != null)
            {
                xOffset -= surfaceInsets.left;
                yOffset -= surfaceInsets.top;

                // Offset dirty rect for surface insets.
                dirty.offset(surfaceInsets.left, surfaceInsets.top);
            }

            bool accessibilityFocusDirty = false;
            Drawable drawable = null; // context.mAttachInfo.mAccessibilityFocusDrawable;
            if (drawable != null)
            {
                //Rect bounds = context.mAttachInfo.mTmpInvalRect;
                //bool hasFocus = getAccessibilityFocusedRect(bounds);
                //if (!hasFocus)
                //{
                //    bounds.setEmpty();
                //}
                //if (!bounds.equals(drawable.getBounds()))
                //{
                //    accessibilityFocusDirty = true;
                //}
            }

            context.mAttachInfo.mDrawingTime = NanoTime.currentTimeMillis(); // currentTimeNanos() / NanoTime.NANOS_PER_MS; // mChoreographer.getFrameTimeNanos() / TimeUtils.NANOS_PER_MS;

            bool useAsyncReport = false;
            if (!dirty.isEmpty() || mIsAnimating || accessibilityFocusDirty
            //|| mNextDrawUseBlastSync
            )
            {
                if (isHardwareEnabled())
                {
                    // If accessibility focus moved, always invalidate the root.
                    bool invalidateRoot = accessibilityFocusDirty || mInvalidateRootRequested;
                    mInvalidateRootRequested = false;

                    // Draw with hardware renderer.
                    mIsAnimating = false;

                    //if (mHardwareYOffset != yOffset || mHardwareXOffset != xOffset)
                    //{
                    //    mHardwareYOffset = yOffset;
                    //    mHardwareXOffset = xOffset;
                    //    invalidateRoot = true;
                    //}

                    if (invalidateRoot)
                    {
                        //context.mAttachInfo.mThreadedRenderer.invalidateRoot();
                    }

                    dirty.setEmpty();

                    // Stage the content drawn size now. It will be transferred to the renderer
                    // shortly before the draw commands get send to the renderer.
                    //bool updated = updateContentDrawBounds();

                    if (mReportNextDraw)
                    {
                        // report next draw overrides setStopped()
                        // This value is re-sync'd to the value of mStopped
                        // in the handling of mReportNextDraw post-draw.
                        //context.mAttachInfo.mThreadedRenderer.setStopped(false);
                    }

                    //if (updated)
                    //{
                    //requestDrawWindow();
                    //}

                    useAsyncReport = true;

                    //context.mAttachInfo.mThreadedRenderer.
                    draw(canvas, mView, context.mAttachInfo, this);
                }
                else
                {
                    // If we get here with a disabled & requested hardware renderer, something went
                    // wrong (an invalidate posted right before we destroyed the hardware surface
                    // for instance) so we should just bail out. Locking the surface with software
                    // rendering at this point would lock it forever and prevent hardware renderer
                    // from doing its job when it comes back.
                    // Before we request a new frame we must however attempt to reinitiliaze the
                    // hardware renderer if it's in requested state. This would happen after an
                    // eglTerminate() for instance.
                    //if (context.mAttachInfo.mThreadedRenderer != null &&
                    //        !context.mAttachInfo.mThreadedRenderer.isEnabled() &&
                    //        context.mAttachInfo.mThreadedRenderer.isRequested() &&
                    //        mSurface.isValid())
                    //{

                    //    try
                    //    {
                    //        context.mAttachInfo.mThreadedRenderer.initializeIfNeeded(
                    //                mWidth, mHeight, mAttachInfo, mSurface, surfaceInsets);
                    //    }
                    //    catch (OutOfResourcesException e)
                    //    {
                    //        handleOutOfResourcesException(e);
                    //        return false;
                    //    }

                    //    mFullRedrawNeeded = true;
                    //    scheduleTraversals();
                    //    return false;
                    //}

                    //if (!drawSoftware(surface, mAttachInfo, xOffset, yOffset,
                    //        scalingRequired, dirty, surfaceInsets))
                    //{
                    //    return false;
                    //}
                }
            }

            if (animating)
            {
                mFullRedrawNeeded = true;
                scheduleTraversals();
            }
            return useAsyncReport;
        }

        /**
         * Draws the specified view.
         *
         * @param view The view to draw.
         * @param attachInfo AttachInfo tied to the specified view.
         */
        void draw(SKCanvas canvas, View view, View.AttachInfo attachInfo,
            object //DrawCallbacks
                callbacks)
        {
            //attachInfo.mViewRootImpl.mViewFrameInfo.markDrawStart();

            updateRootDisplayList(canvas, view, callbacks);

            // register animating rendernodes which started animating prior to renderer
            // creation, which is typical for animators started prior to first draw
            if (attachInfo.mPendingAnimatingRenderNodes != null)
            {
                int count = attachInfo.mPendingAnimatingRenderNodes.Count;
                for (int i = 0; i < count; i++)
                {
                    //registerAnimatingRenderNode(
                    //attachInfo.mPendingAnimatingRenderNodes.get(i));
                }
                attachInfo.mPendingAnimatingRenderNodes.Clear();
                // We don't need this anymore as subsequent calls to
                // ViewRootImpl#attachRenderNodeAnimator will go directly to us.
                attachInfo.mPendingAnimatingRenderNodes = null;
            }

            //FrameInfo frameInfo = attachInfo.mViewRootImpl.getUpdatedFrameInfo();

            //int syncResult = syncAndDrawFrame(frameInfo);
            //if ((syncResult & SYNC_LOST_SURFACE_REWARD_IF_FOUND) != 0)
            //{
            //    Log.w("OpenGLRenderer", "Surface lost, forcing relayout");
            //    // We lost our surface. For a relayout next frame which should give us a new
            //    // surface from WindowManager, which hopefully will work.
            //    attachInfo.mViewRootImpl.mForceNextWindowRelayout = true;
            //    attachInfo.mViewRootImpl.requestLayout();
            //}
            //if ((syncResult & SYNC_REDRAW_REQUESTED) != 0)
            //{
            //    attachInfo.mViewRootImpl.invalidate();
            //}
        }

        private void updateViewTreeDisplayList(SKCanvas drawingCanvas, View view)
        {
            view.mPrivateFlags |= View.PFLAG_DRAWN;
            view.mRecreateDisplayList = (view.mPrivateFlags & View.PFLAG_INVALIDATED) == View.PFLAG_INVALIDATED;
            view.mPrivateFlags &= (int)~View.PFLAG_INVALIDATED;
            SKPicture displayList = view.updateDisplayListIfDirty(drawingCanvas);
            if (displayList != null)
            {
                drawingCanvas.DrawPicture(displayList, 0, 0);
            }
            view.mRecreateDisplayList = false;
        }

        internal List<Application.FrameCallback> mNextRtFrameCallbacks;

        private void updateRootDisplayList(SKCanvas canvas, View view,
            object //DrawCallbacks
            callbacks)
        {
            //Trace.traceBegin(Trace.TRACE_TAG_VIEW, "Record View#draw()");
            updateViewTreeDisplayList(canvas, view);

            // Consume and set the frame callback after we dispatch draw to the view above, but before
            // onPostDraw below which may reset the callback for the next frame.  This ensures that
            // updates to the frame callback during scroll handling will also apply in this frame.
            if (mNextRtFrameCallbacks != null)
            {
                List<Application.FrameCallback> frameCallbacks = mNextRtFrameCallbacks;
                mNextRtFrameCallbacks = null;
                //setFrameCallback(frame-> {
                for (int i = 0; i < frameCallbacks.Count; ++i)
                {
                    frameCallbacks.ElementAt(i).doFrame(context.mAttachInfo.mDrawingTime);//onFrameDraw(frame);
                }
                //});
            }

            //if (mRootNodeNeedsUpdate || !mRootNode.hasDisplayList())
            //{
            //    RecordingCanvas canvas = mRootNode.beginRecording(mSurfaceWidth, mSurfaceHeight);
            //    try
            //    {
            //        final int saveCount = canvas.save();
            //        canvas.translate(mInsetLeft, mInsetTop);
            //        callbacks.onPreDraw(canvas);

            //        canvas.enableZ();
            //        canvas.drawRenderNode(view.updateDisplayListIfDirty());
            //        canvas.disableZ();

            //        callbacks.onPostDraw(canvas);
            //        canvas.restoreToCount(saveCount);
            //        mRootNodeNeedsUpdate = false;
            //    }
            //    finally
            //    {
            //        mRootNode.endRecording();
            //    }
            //}
            //Trace.traceEnd(Trace.TRACE_TAG_VIEW);
        }

        private bool isHardwareEnabled()
        {
            return true; // always
        }

        public bool isLayoutRequested()
        {
            return mLayoutRequested;
        }

        public void requestLayout()
        {
            if (!mHandlingLayoutInLayoutRequest)
            {
                //checkThread();
                mLayoutRequested = true;
                scheduleTraversals();
            }
        }

        void scheduleTraversals()
        {
            if (!mTraversalScheduled)
            {
                mTraversalScheduled = true;
                context.application.invalidate();

                //mTraversalBarrier = mHandler.getLooper().getQueue().postSyncBarrier();
                //mChoreographer.postCallback(
                //        Choreographer.CALLBACK_TRAVERSAL, mTraversalRunnable, null);
                //notifyRendererOfFramePending();
                //pokeDrawLockIfNeeded();
            }
        }

        void unscheduleTraversals()
        {
            if (mTraversalScheduled)
            {
                mTraversalScheduled = false;
                //mHandler.getLooper().getQueue().removeSyncBarrier(mTraversalBarrier);
                //mChoreographer.removeCallbacks(
                //Choreographer.CALLBACK_TRAVERSAL, mTraversalRunnable, null);
            }
        }

        void doTraversal(SKCanvas canvas)
        {
            if (mTraversalScheduled)
            {
                mTraversalScheduled = false;
                //mHandler.getLooper().getQueue().removeSyncBarrier(mTraversalBarrier);

                //if (mProfile)
                //{
                //    Debug.startMethodTracing("ViewAncestor");
                //}


                if (System.Diagnostics.Debugger.IsAttached)
                {
                    DO_TRAVERSAL_INTERNAL(canvas);
                }
                else
                {
                    try
                    {
                        DO_TRAVERSAL_INTERNAL(canvas);
                    }
                    catch (Exception e)
                    {
                        Log.v(mTag, "Caught exception while performing traversal: " + e);
                    }
                }

                //if (mProfile)
                //{
                //    Debug.stopMethodTracing();
                //    mProfile = false;
                //}
            }
        }

        private void DO_TRAVERSAL_INTERNAL(SKCanvas canvas)
        {
            if (DEBUG_MEASURE_LAYOUT_DRAW_TIME)
            {
                long s = NanoTime.currentTimeMillis();
                performTraversals(canvas);
                long e = NanoTime.currentTimeMillis();
                Log.d(TAG, "performed traversal in " + (e - s) + " milliseconds");
            }
            else
            {
                performTraversals(canvas);
            }

            // invalidate AFTER traversal to ensure invalidation flag is added back
            mView.invalidate();
        }

        public void invalidate()
        {
            mDirty.set(0, 0, mWidth, mHeight);
            if (!mWillDrawSoon)
            {
                scheduleTraversals();
            }
            else
            {
                Log.d(TAG, "skip traversal scheduling because we are going to draw soon");
            }
        }

        /**
         * Add LayoutTransition to the list of transitions to be started in the next traversal.
         * This list will be cleared after the transitions on the list are start()'ed. These
         * transitionsa re added by LayoutTransition itself when it sets up animations. The setup
         * happens during the layout phase of traversal, which we want to complete before any of the
         * animations are started (because those animations may side-effect properties that layout
         * depends upon, like the bounding rectangles of the affected views). So we add the transition
         * to the list and it is started just prior to starting the drawing phase of traversal.
         *
         * @param transition The LayoutTransition to be started on the next traversal.
         *
         * @hide
         */
        internal void requestTransitionStart(LayoutTransition transition)
        {
            if (mPendingTransitions == null || !mPendingTransitions.Contains(transition))
            {
                if (mPendingTransitions == null)
                {
                    mPendingTransitions = new List<LayoutTransition>();
                }
                mPendingTransitions.Add(transition);
            }
        }

        public bool isLayoutDirectionResolved()
        {
            return true;
        }

        public int getLayoutDirection()
        {
            return View.LAYOUT_DIRECTION_RESOLVED_DEFAULT;
        }

        internal static bool isInTouchMode()
        {
            return true;
        }

        /**
         * Return true if child is an ancestor of parent, (or equal to the parent).
         */
        public static bool isViewDescendantOf(View child, View parent)
        {
            if (child == parent)
            {
                return true;
            }

            ViewParent theParent = child.getParent();
            return theParent is View && isViewDescendantOf((View)theParent, parent);
        }

        public void onDescendantInvalidated(View view, View descendant)
        {
            if ((descendant.mPrivateFlags & View.PFLAG_DRAW_ANIMATION) != 0)
            {
                mIsAnimating = true;
            }
            invalidate();
        }

        public void childDrawableStateChanged(View child)
        {
            // no op
        }

        public void childHasTransientStateChanged(View child, bool hasTransientState)
        {
            // Do nothing.
        }

        ValueHolder<HandlerActionQueue> sRunQueues
        {
            get
            {
                if (context == null)
                {
                    return null;
                }
                return context.storage.GetOrCreate<HandlerActionQueue>(StorageKeys.ViewRootImplHandlerActionQueue, () => new());
            }
        }
        ValueHolder<List<Runnable>> sFirstDrawHandlers
        {
            get
            {
                if (context == null)
                {
                    return null;
                }
                return context.storage.GetOrCreate<List<Runnable>>(StorageKeys.ViewRootImplFirstDrawHandlers, () => new());
            }
        }
        ValueHolder<bool> sFirstDrawComplete
        {
            get
            {
                if (context == null)
                {
                    return null;
                }
                return context.storage.GetOrCreate(StorageKeys.ViewRootImplFirstDrawComplete, () => false);
            }
        }


        internal HandlerActionQueue getRunQueue()
        {
            var s = sRunQueues;
            HandlerActionQueue rq = s.Value;
            if (rq != null)
            {
                return rq;
            }
            rq = new HandlerActionQueue();
            s.Value = rq;
            return rq;
        }

        public void clearChildFocus(View child)
        {
        }

        public void onDescendantUnbufferedRequested()
        {
            if (getParent() != null)
            {
                getParent().onDescendantUnbufferedRequested();
            }
        }

        public bool canResolveLayoutDirection()
        {
            return false;
        }

        public void OnScreenDensityChanged()
        {
            mView.OnScreenDensityChanged();
            scheduleTraversals();
        }

        public void requestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }

        public void invalidateChild(View view, Rect dirty)
        {
            invalidateChildInParent(null, dirty);
        }


        public ViewParent invalidateChildInParent(int[] location, Rect dirty)
        {
            //checkThread();
            if (DEBUG_DRAW) Log.v(mTag, "Invalidate child: " + dirty);

            if (dirty == null)
            {
                invalidate();
                return null;
            }
            else if (dirty.isEmpty() && !mIsAnimating)
            {
                return null;
            }

            if (mCurScrollY != 0)
            {
                mTempRect.set(dirty);
                dirty = mTempRect;
                if (mCurScrollY != 0)
                {
                    dirty.offset(0, -mCurScrollY);
                }
                if (context.mAttachInfo.mScalingRequired)
                {
                    dirty.inset(-1, -1);
                }
            }

            invalidateRectOnScreen(dirty);

            return null;
        }

        private void invalidateRectOnScreen(Rect dirty)
        {
            Rect localDirty = mDirty;

            // Add the new dirty rect to the current one
            localDirty.union(dirty.left, dirty.top, dirty.right, dirty.bottom);
            // Intersect with the bounds of the window to skip
            // updates that lie outside of the visible region
            float appScale = context.mAttachInfo.mApplicationScale;
            bool intersected = localDirty.intersect(0, 0,
                    (int)(mWidth * appScale + 0.5f), (int)(mHeight * appScale + 0.5f));
            if (!intersected)
            {
                localDirty.setEmpty();
            }
            if (!mWillDrawSoon && (intersected || mIsAnimating))
            {
                scheduleTraversals();
            }
        }
    }
}
