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

using AndroidUI.Applications;
using AndroidUI.Exceptions;
using AndroidUI.Utils.Arrays;

namespace AndroidUI.Widgets
{

    /**
     * Contains methods to standard constants used in the UI for timeouts, sizes, and distances.
     */
    public class ViewConfiguration
    {
        private const String TAG = "ViewConfiguration";

        /**
         * Defines the width of the horizontal scrollbar and the height of the vertical scrollbar in
         * dips
         */
        private const int SCROLL_BAR_SIZE = 4;

        /**
         * Duration of the fade when scrollbars fade away in milliseconds
         */
        private const int SCROLL_BAR_FADE_DURATION = 250;

        /**
         * Default delay before the scrollbars fade in milliseconds
         */
        private const int SCROLL_BAR_DEFAULT_DELAY = 300;

        /**
         * Defines the length of the fading edges in dips
         */
        private const int FADING_EDGE_LENGTH = 12;

        /**
         * Defines the duration in milliseconds of the pressed state in child
         * components.
         */
        private const int PRESSED_STATE_DURATION = 64;

        /**
         * Defines the default duration in milliseconds before a press turns into
         * a long press
         * @hide
         */
        public const int DEFAULT_LONG_PRESS_TIMEOUT = 400;

        /**
         * Defines the default duration in milliseconds between the first tap's up event and the second
         * tap's down event for an interaction to be considered part of the same multi-press.
         */
        private const int DEFAULT_MULTI_PRESS_TIMEOUT = 300;

        /**
         * Defines the time between successive key repeats in milliseconds.
         */
        private const int KEY_REPEAT_DELAY = 50;

        /**
         * Defines the duration in milliseconds a user needs to hold down the
         * appropriate button to bring up the global actions dialog (power off,
         * lock screen, etc).
         */
        private const int GLOBAL_ACTIONS_KEY_TIMEOUT = 500;

        /**
         * Defines the duration in milliseconds a user needs to hold down the
         * appropriate buttons (power + volume down) to trigger the screenshot chord.
         */
        private const int SCREENSHOT_CHORD_KEY_TIMEOUT = 0;

        /**
         * Defines the duration in milliseconds a user needs to hold down the
         * appropriate button to bring up the accessibility shortcut for the first time
         */
        private const int A11Y_SHORTCUT_KEY_TIMEOUT = 3000;

        /**
         * Defines the duration in milliseconds a user needs to hold down the
         * appropriate button to enable the accessibility shortcut once it's configured.
         */
        private const int A11Y_SHORTCUT_KEY_TIMEOUT_AFTER_CONFIRMATION = 1000;

        /**
         * Defines the duration in milliseconds we will wait to see if a touch event
         * is a tap or a scroll. If the user does not move within this interval, it is
         * considered to be a tap.
         */
        private const int TAP_TIMEOUT = 100;

        /**
         * Defines the duration in milliseconds we will wait to see if a touch event
         * is a jump tap. If the user does not complete the jump tap within this interval, it is
         * considered to be a tap.
         */
        private const int JUMP_TAP_TIMEOUT = 500;

        /**
         * Defines the duration in milliseconds between the first tap's up event and
         * the second tap's down event for an interaction to be considered a
         * double-tap.
         */
        private const int DOUBLE_TAP_TIMEOUT = 300;

        /**
         * Defines the minimum duration in milliseconds between the first tap's up event and
         * the second tap's down event for an interaction to be considered a
         * double-tap.
         */
        private const int DOUBLE_TAP_MIN_TIME = 40;

        /**
         * Defines the maximum duration in milliseconds between a touch pad
         * touch and release for a given touch to be considered a tap (click) as
         * opposed to a hover movement gesture.
         */
        private const int HOVER_TAP_TIMEOUT = 150;

        /**
         * Defines the maximum distance in pixels that a touch pad touch can move
         * before being released for it to be considered a tap (click) as opposed
         * to a hover movement gesture.
         */
        private const int HOVER_TAP_SLOP = 20;

        /**
         * Defines the duration in milliseconds we want to display zoom controls in response
         * to a user panning within an application.
         */
        private const int ZOOM_CONTROLS_TIMEOUT = 3000;

        /**
         * Inset in dips to look for touchable content when the user touches the edge of the screen
         */
        private const int EDGE_SLOP = 12;

        /**
         * Distance a touch can wander before we think the user is scrolling in dips.
         * Note that this value defined here is only used as a fallback by legacy/misbehaving
         * applications that do not provide a Context for determining density/configuration-dependent
         * values.
         *
         * To alter this value, see the configuration resource config_viewConfigurationTouchSlop
         * in frameworks/base/core/res/res/values/config.xml or the appropriate device resource overlay.
         * It may be appropriate to tweak this on a device-specific basis in an overlay based on
         * the characteristics of the touch panel and firmware.
         */
        private const int TOUCH_SLOP = 8;

        /**
         * Defines the minimum size of the touch target for a scrollbar in dips
         */
        private const int MIN_SCROLLBAR_TOUCH_TARGET = 48;

        /**
         * Distance the first touch can wander before we stop considering this event a double tap
         * (in dips)
         */
        private const int DOUBLE_TAP_TOUCH_SLOP = TOUCH_SLOP;

        /**
         * Distance a touch can wander before we think the user is attempting a paged scroll
         * (in dips)
         *
         * Note that this value defined here is only used as a fallback by legacy/misbehaving
         * applications that do not provide a Context for determining density/configuration-dependent
         * values.
         *
         * See the note above on {@link #TOUCH_SLOP} regarding the dimen resource
         * config_viewConfigurationTouchSlop. ViewConfiguration will report a paging touch slop of
         * config_viewConfigurationTouchSlop * 2 when provided with a Context.
         */
        private const int PAGING_TOUCH_SLOP = TOUCH_SLOP * 2;

        /**
         * Distance in dips between the first touch and second touch to still be considered a double tap
         */
        private const int DOUBLE_TAP_SLOP = 100;

        /**
         * Distance in dips a touch needs to be outside of a window's bounds for it to
         * count as outside for purposes of dismissing the window.
         */
        private const int WINDOW_TOUCH_SLOP = 16;

        /**
         * Minimum velocity to initiate a fling, as measured in dips per second
         */
        private const int MINIMUM_FLING_VELOCITY = 50;

        /**
         * Maximum velocity to initiate a fling, as measured in dips per second
         */
        private const int MAXIMUM_FLING_VELOCITY = 8000;

        /**
         * Delay before dispatching a recurring accessibility event in milliseconds.
         * This delay guarantees that a recurring event will be send at most once
         * during the {@link #SEND_RECURRING_ACCESSIBILITY_EVENTS_INTERVAL_MILLIS} time
         * frame.
         */
        private const long SEND_RECURRING_ACCESSIBILITY_EVENTS_INTERVAL_MILLIS = 100;

        /**
         * The maximum size of View's drawing cache, expressed in bytes. This size
         * should be at least equal to the size of the screen in ARGB888 format.
         */
        [Obsolete]
        private const int MAXIMUM_DRAWING_CACHE_SIZE = 480 * 800 * 4; // ARGB8888

        /**
         * The coefficient of friction applied to flings/scrolls.
         */
        private const float SCROLL_FRICTION = 0.015f;

        /**
         * Max distance in dips to overscroll for edge effects
         */
        private const int OVERSCROLL_DISTANCE = 0;

        /**
         * Max distance in dips to overfling for edge effects
         */
        private const int OVERFLING_DISTANCE = 6;

        /**
         * Amount to scroll in response to a horizontal {@link MotionEvent#ACTION_SCROLL} event,
         * in dips per axis value.
         */
        private const float HORIZONTAL_SCROLL_FACTOR = 64;

        /**
         * Amount to scroll in response to a vertical {@link MotionEvent#ACTION_SCROLL} event,
         * in dips per axis value.
         */
        private const float VERTICAL_SCROLL_FACTOR = 64;

        /**
         * Default duration to hide an action mode for.
         */
        private const long ACTION_MODE_HIDE_DURATION_DEFAULT = 2000;

        /**
         * Defines the duration in milliseconds before an end of a long press causes a tooltip to be
         * hidden.
         */
        private const int LONG_PRESS_TOOLTIP_HIDE_TIMEOUT = 1500;

        /**
         * Defines the duration in milliseconds before a hover event causes a tooltip to be shown.
         */
        private const int HOVER_TOOLTIP_SHOW_TIMEOUT = 500;

        /**
         * Defines the duration in milliseconds before mouse inactivity causes a tooltip to be hidden.
         * (default variant to be used when {@link View#SYSTEM_UI_FLAG_LOW_PROFILE} is not set).
         */
        private const int HOVER_TOOLTIP_HIDE_TIMEOUT = 15000;

        /**
         * Defines the duration in milliseconds before mouse inactivity causes a tooltip to be hidden
         * (short version to be used when {@link View#SYSTEM_UI_FLAG_LOW_PROFILE} is set).
         */
        private const int HOVER_TOOLTIP_HIDE_SHORT_TIMEOUT = 3000;

        /**
         * Configuration values for overriding {@link #hasPermanentMenuKey()} behavior.
         * These constants must match the definition in res/values/config.xml.
         */
        private const int HAS_PERMANENT_MENU_KEY_AUTODETECT = 0;
        private const int HAS_PERMANENT_MENU_KEY_TRUE = 1;
        private const int HAS_PERMANENT_MENU_KEY_FALSE = 2;

        /**
         * The multiplication factor for inhibiting default gestures.
         */
        private const float AMBIGUOUS_GESTURE_MULTIPLIER = 2f;

        /**
         * The timeout value in milliseconds to adjust the selection span and actions for the selected
         * text when TextClassifier has been initialized.
         */
        private const int SMART_SELECTION_INITIALIZED_TIMEOUT_IN_MILLISECOND = 200;

        /**
         * The timeout value in milliseconds to adjust the selection span and actions for the selected
         * text when TextClassifier has not been initialized.
         */
        private const int SMART_SELECTION_INITIALIZING_TIMEOUT_IN_MILLISECOND = 500;

        private readonly bool mConstructedWithContext;
        private readonly int mEdgeSlop;
        private readonly int mFadingEdgeLength;
        private readonly int mMinimumFlingVelocity;
        private readonly int mMaximumFlingVelocity;
        private readonly int mScrollbarSize;
        private readonly int mTouchSlop;
        private readonly int mMinScalingSpan;
        private readonly int mHoverSlop;
        private readonly int mMinScrollbarTouchTarget;
        private readonly int mDoubleTapTouchSlop;
        private readonly int mPagingTouchSlop;
        private readonly int mDoubleTapSlop;
        private readonly int mWindowTouchSlop;
        private readonly float mAmbiguousGestureMultiplier;
        private readonly int mMaximumDrawingCacheSize;
        private readonly int mOverscrollDistance;
        private readonly int mOverflingDistance;
        private readonly bool mFadingMarqueeEnabled;
        private readonly long mGlobalActionsKeyTimeout;
        private readonly float mVerticalScrollFactor;
        private readonly float mHorizontalScrollFactor;
        private readonly bool mShowMenuShortcutsWhenKeyboardPresent;
        private readonly long mScreenshotChordKeyTimeout;
        private readonly int mSmartSelectionInitializedTimeout;
        private readonly int mSmartSelectionInitializingTimeout;

        private bool sHasPermanentMenuKey;
        private bool sHasPermanentMenuKeySet;

        static readonly SparseArray<ViewConfiguration> sConfigurations =
                new SparseArray<ViewConfiguration>(2);

        /**
         * [Obsolete] Use {@link android.view.ViewConfiguration#get(android.content.Context)} instead.
         */
        [Obsolete]
        public ViewConfiguration()
        {
            mConstructedWithContext = false;
            mEdgeSlop = EDGE_SLOP;
            mFadingEdgeLength = FADING_EDGE_LENGTH;
            mMinimumFlingVelocity = MINIMUM_FLING_VELOCITY;
            mMaximumFlingVelocity = MAXIMUM_FLING_VELOCITY;
            mScrollbarSize = SCROLL_BAR_SIZE;
            mTouchSlop = TOUCH_SLOP;
            mHoverSlop = TOUCH_SLOP / 2;
            mMinScrollbarTouchTarget = MIN_SCROLLBAR_TOUCH_TARGET;
            mDoubleTapTouchSlop = DOUBLE_TAP_TOUCH_SLOP;
            mPagingTouchSlop = PAGING_TOUCH_SLOP;
            mDoubleTapSlop = DOUBLE_TAP_SLOP;
            mWindowTouchSlop = WINDOW_TOUCH_SLOP;
            mAmbiguousGestureMultiplier = AMBIGUOUS_GESTURE_MULTIPLIER;
            //noinspection deprecation
            mMaximumDrawingCacheSize = MAXIMUM_DRAWING_CACHE_SIZE;
            mOverscrollDistance = OVERSCROLL_DISTANCE;
            mOverflingDistance = OVERFLING_DISTANCE;
            mFadingMarqueeEnabled = true;
            mGlobalActionsKeyTimeout = GLOBAL_ACTIONS_KEY_TIMEOUT;
            mHorizontalScrollFactor = HORIZONTAL_SCROLL_FACTOR;
            mVerticalScrollFactor = VERTICAL_SCROLL_FACTOR;
            mShowMenuShortcutsWhenKeyboardPresent = false;
            mScreenshotChordKeyTimeout = SCREENSHOT_CHORD_KEY_TIMEOUT;

            // Getter throws if mConstructedWithContext is false so doesn't matter what
            // this value is.
            mMinScalingSpan = 0;
            mSmartSelectionInitializedTimeout = SMART_SELECTION_INITIALIZED_TIMEOUT_IN_MILLISECOND;
            mSmartSelectionInitializingTimeout = SMART_SELECTION_INITIALIZING_TIMEOUT_IN_MILLISECOND;
        }

        /**
         * Creates a new configuration for the specified visual {@link Context}. The configuration
         * depends on various parameters of the {@link Context}, like the dimension of the display or
         * the density of the display.
         *
         * @param context A visual {@link Context} used to initialize the view configuration. It must
         *                be {@link Activity} or other {@link Context} created with
         *                {@link Context#createWindowContext(int, Bundle)}.
         *
         * @see #get(android.content.Context)
         * @see android.util.DisplayMetrics
         */
        //        private ViewConfiguration(Context context)
        //        {
        //            mConstructedWithContext = true;
        //            readonly Resources res = context.getResources();
        //            readonly DisplayMetrics metrics = res.getDisplayMetrics();
        //            readonly Configuration config = res.getConfiguration();
        //            readonly float density = metrics.density;
        //            readonly float sizeAndDensity;
        //            if (config.isLayoutSizeAtLeast(Configuration.SCREENLAYOUT_SIZE_XLARGE))
        //            {
        //                sizeAndDensity = density * 1.5f;
        //            }
        //            else
        //            {
        //                sizeAndDensity = density;
        //            }

        //            mEdgeSlop = (int)(sizeAndDensity * EDGE_SLOP + 0.5f);
        //            mFadingEdgeLength = (int)(sizeAndDensity * FADING_EDGE_LENGTH + 0.5f);
        //            mScrollbarSize = res.getDimensionPixelSize(
        //                    com.android.internal.R.dimen.config_scrollbarSize);
        //        mDoubleTapSlop = (int) (sizeAndDensity* DOUBLE_TAP_SLOP + 0.5f);
        //        mWindowTouchSlop = (int) (sizeAndDensity* WINDOW_TOUCH_SLOP + 0.5f);

        //        readonly TypedValue multiplierValue = new TypedValue();
        //        res.getValue(
        //                com.android.internal.R.dimen.config_ambiguousGestureMultiplier,
        //                multiplierValue,
        //                true /*resolveRefs*/);
        //        mAmbiguousGestureMultiplier = Math.max(1.0f, multiplierValue.getFloat());

        //        // Size of the screen in bytes, in ARGB_8888 format
        //        readonly Rect maxBounds = config.windowConfiguration.getMaxBounds();
        //        mMaximumDrawingCacheSize = 4 * maxBounds.width()* maxBounds.height();

        //        mOverscrollDistance = (int) (sizeAndDensity* OVERSCROLL_DISTANCE + 0.5f);
        //        mOverflingDistance = (int) (sizeAndDensity* OVERFLING_DISTANCE + 0.5f);

        //        if (!sHasPermanentMenuKeySet) {
        //            readonly int configVal = res.getInteger(
        //                    com.android.internal.R.integer.config_overrideHasPermanentMenuKey);

        //            switch (configVal) {
        //                default:
        //                case HAS_PERMANENT_MENU_KEY_AUTODETECT: {
        //                    IWindowManager wm = WindowManagerGlobal.getWindowManagerService();
        //                    try {
        //                        sHasPermanentMenuKey = !wm.hasNavigationBar(context.getDisplayId());
        //                        sHasPermanentMenuKeySet = true;
        //                    } catch (RemoteException ex) {
        //                        sHasPermanentMenuKey = false;
        //                    }
        //                }
        //                break;

        //                case HAS_PERMANENT_MENU_KEY_TRUE:
        //sHasPermanentMenuKey = true;
        //sHasPermanentMenuKeySet = true;
        //break;

        //                case HAS_PERMANENT_MENU_KEY_FALSE:
        //sHasPermanentMenuKey = false;
        //sHasPermanentMenuKeySet = true;
        //break;
        //            }
        //        }

        //        mFadingMarqueeEnabled = res.getbool(
        //                com.android.internal.R.bool.config_ui_enableFadingMarquee);
        //mTouchSlop = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_viewConfigurationTouchSlop);
        //mHoverSlop = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_viewConfigurationHoverSlop);
        //mMinScrollbarTouchTarget = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_minScrollbarTouchTarget);
        //mPagingTouchSlop = mTouchSlop * 2;

        //mDoubleTapTouchSlop = mTouchSlop;

        //mMinimumFlingVelocity = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_viewMinFlingVelocity);
        //mMaximumFlingVelocity = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_viewMaxFlingVelocity);
        //mGlobalActionsKeyTimeout = res.getInteger(
        //        com.android.internal.R.integer.config_globalActionsKeyTimeout);

        //mHorizontalScrollFactor = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_horizontalScrollFactor);
        //mVerticalScrollFactor = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_verticalScrollFactor);

        //mShowMenuShortcutsWhenKeyboardPresent = res.getbool(
        //    com.android.internal.R.bool.config_showMenuShortcutsWhenKeyboardPresent);

        //mMinScalingSpan = res.getDimensionPixelSize(
        //        com.android.internal.R.dimen.config_minScalingSpan);

        //mScreenshotChordKeyTimeout = res.getInteger(
        //        com.android.internal.R.integer.config_screenshotChordKeyTimeout);

        //mSmartSelectionInitializedTimeout = res.getInteger(
        //        com.android.internal.R.integer.config_smartSelectionInitializedTimeoutMillis);
        //mSmartSelectionInitializingTimeout = res.getInteger(
        //        com.android.internal.R.integer.config_smartSelectionInitializingTimeoutMillis);
        //    }

        /**
         * Returns a configuration for the specified visual {@link Context}. The configuration depends
         * on various parameters of the {@link Context}, like the dimension of the display or the
         * density of the display.
         *
         * @param context A visual {@link Context} used to initialize the view configuration. It must
         *                be {@link Activity} or other {@link Context} created with
         *                {@link Context#createWindowContext(int, Bundle)}.
         */
        // TODO(b/182007470): Use @ConfigurationContext instead
        //    public static ViewConfiguration get(@UiContext Context context)
        //{
        //    StrictMode.assertConfigurationContext(context, "ViewConfiguration");

        //    readonly DisplayMetrics metrics = context.getResources().getDisplayMetrics();
        //    readonly int density = (int)(100.0f * metrics.density);

        //    ViewConfiguration configuration = sConfigurations.get(density);
        //    if (configuration == null)
        //    {
        //        configuration = new ViewConfiguration(context);
        //        sConfigurations.put(density, configuration);
        //    }

        //    return configuration;
        //}

        public static ViewConfiguration get(Context context)
        {
            int density = (int)(100.0f * DensityManager.ScreenDensityAsFloat);

            ViewConfiguration configuration = sConfigurations.get(density);
            if (configuration == null)
            {
                configuration = new ViewConfiguration();
                sConfigurations.put(density, configuration);
            }

            return configuration;
        }

        /**
         * @return The width of the horizontal scrollbar and the height of the vertical
         *         scrollbar in dips
         *
         * [Obsolete] Use {@link #getScaledScrollBarSize()} instead.
         */
        [Obsolete]
        public static int getScrollBarSize()
        {
            return SCROLL_BAR_SIZE;
        }

        /**
         * @return The width of the horizontal scrollbar and the height of the vertical
         *         scrollbar in pixels
         */
        public int getScaledScrollBarSize()
        {
            return mScrollbarSize;
        }

        /**
         * @return the minimum size of the scrollbar thumb's touch target in pixels
         * @hide
         */
        public int getScaledMinScrollbarTouchTarget()
        {
            return mMinScrollbarTouchTarget;
        }

        /**
         * @return Duration of the fade when scrollbars fade away in milliseconds
         */
        public static int getScrollBarFadeDuration()
        {
            return SCROLL_BAR_FADE_DURATION;
        }

        /**
         * @return Default delay before the scrollbars fade in milliseconds
         */
        public static int getScrollDefaultDelay()
        {
            return SCROLL_BAR_DEFAULT_DELAY;
        }

        /**
         * @return the length of the fading edges in dips
         *
         * [Obsolete] Use {@link #getScaledFadingEdgeLength()} instead.
         */
        [Obsolete]
        public static int getFadingEdgeLength()
        {
            return FADING_EDGE_LENGTH;
        }

        /**
         * @return the length of the fading edges in pixels
         */
        public int getScaledFadingEdgeLength()
        {
            return mFadingEdgeLength;
        }

        /**
         * @return the duration in milliseconds of the pressed state in child
         * components.
         */
        public static int getPressedStateDuration()
        {
            return PRESSED_STATE_DURATION;
        }

        /**
         * @return the duration in milliseconds before a press turns into
         * a long press
         */
        public static int getLongPressTimeout()
        {
            return DEFAULT_LONG_PRESS_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds between the first tap's up event and the second tap's
         * down event for an interaction to be considered part of the same multi-press.
         */
        public static int getMultiPressTimeout()
        {
            return DEFAULT_MULTI_PRESS_TIMEOUT;
        }

        /**
         * @return the time before the first key repeat in milliseconds.
         */
        public static int getKeyRepeatTimeout()
        {
            return getLongPressTimeout();
        }

        /**
         * @return the time between successive key repeats in milliseconds.
         */
        public static int getKeyRepeatDelay()
        {
            return KEY_REPEAT_DELAY;
        }

        /**
         * @return the duration in milliseconds we will wait to see if a touch event
         * is a tap or a scroll. If the user does not move within this interval, it is
         * considered to be a tap.
         */
        public static int getTapTimeout()
        {
            return TAP_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds we will wait to see if a touch event
         * is a jump tap. If the user does not move within this interval, it is
         * considered to be a tap.
         */
        public static int getJumpTapTimeout()
        {
            return JUMP_TAP_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds between the first tap's up event and
         * the second tap's down event for an interaction to be considered a
         * double-tap.
         */
        public static int getDoubleTapTimeout()
        {
            return DOUBLE_TAP_TIMEOUT;
        }

        /**
         * @return the minimum duration in milliseconds between the first tap's
         * up event and the second tap's down event for an interaction to be considered a
         * double-tap.
         *
         * @hide
         */
        public static int getDoubleTapMinTime()
        {
            return DOUBLE_TAP_MIN_TIME;
        }

        /**
         * @return the maximum duration in milliseconds between a touch pad
         * touch and release for a given touch to be considered a tap (click) as
         * opposed to a hover movement gesture.
         * @hide
         */
        public static int getHoverTapTimeout()
        {
            return HOVER_TAP_TIMEOUT;
        }

        /**
         * @return the maximum distance in pixels that a touch pad touch can move
         * before being released for it to be considered a tap (click) as opposed
         * to a hover movement gesture.
         * @hide
         */
        public static int getHoverTapSlop()
        {
            return HOVER_TAP_SLOP;
        }

        /**
         * @return Inset in dips to look for touchable content when the user touches the edge of the
         *         screen
         *
         * [Obsolete] Use {@link #getScaledEdgeSlop()} instead.
         */
        [Obsolete]
        public static int getEdgeSlop()
        {
            return EDGE_SLOP;
        }

        /**
         * @return Inset in pixels to look for touchable content when the user touches the edge of the
         *         screen
         */
        public int getScaledEdgeSlop()
        {
            return mEdgeSlop;
        }

        /**
         * @return Distance in dips a touch can wander before we think the user is scrolling
         *
         * [Obsolete] Use {@link #getScaledTouchSlop()} instead.
         */
        [Obsolete]
        public static int getTouchSlop()
        {
            return TOUCH_SLOP;
        }

        /**
         * @return Distance in pixels a touch can wander before we think the user is scrolling
         */
        public int getScaledTouchSlop()
        {
            return mTouchSlop;
        }

        /**
         * @return Distance in pixels a hover can wander while it is still considered "stationary".
         *
         */
        public int getScaledHoverSlop()
        {
            return mHoverSlop;
        }

        /**
         * @return Distance in pixels the first touch can wander before we do not consider this a
         * potential double tap event
         * @hide
         */
        public int getScaledDoubleTapTouchSlop()
        {
            return mDoubleTapTouchSlop;
        }

        /**
         * @return Distance in pixels a touch can wander before we think the user is scrolling a full
         * page
         */
        public int getScaledPagingTouchSlop()
        {
            return mPagingTouchSlop;
        }

        /**
         * @return Distance in dips between the first touch and second touch to still be
         *         considered a double tap
         * [Obsolete] Use {@link #getScaledDoubleTapSlop()} instead.
         * @hide The only client of this should be GestureDetector, which needs this
         *       for clients that still use its deprecated constructor.
         */
        [Obsolete]
        public static int getDoubleTapSlop()
        {
            return DOUBLE_TAP_SLOP;
        }

        /**
         * @return Distance in pixels between the first touch and second touch to still be
         *         considered a double tap
         */
        public int getScaledDoubleTapSlop()
        {
            return mDoubleTapSlop;
        }

        /**
         * Interval for dispatching a recurring accessibility event in milliseconds.
         * This interval guarantees that a recurring event will be send at most once
         * during the {@link #getSendRecurringAccessibilityEventsInterval()} time frame.
         *
         * @return The delay in milliseconds.
         *
         * @hide
         */
        public static long getSendRecurringAccessibilityEventsInterval()
        {
            return SEND_RECURRING_ACCESSIBILITY_EVENTS_INTERVAL_MILLIS;
        }

        /**
         * @return Distance in dips a touch must be outside the bounds of a window for it
         * to be counted as outside the window for purposes of dismissing that
         * window.
         *
         * [Obsolete] Use {@link #getScaledWindowTouchSlop()} instead.
         */
        [Obsolete]
        public static int getWindowTouchSlop()
        {
            return WINDOW_TOUCH_SLOP;
        }

        /**
         * @return Distance in pixels a touch must be outside the bounds of a window for it
         * to be counted as outside the window for purposes of dismissing that window.
         */
        public int getScaledWindowTouchSlop()
        {
            return mWindowTouchSlop;
        }

        /**
         * @return Minimum velocity to initiate a fling, as measured in dips per second.
         *
         * [Obsolete] Use {@link #getScaledMinimumFlingVelocity()} instead.
         */
        [Obsolete]
        public static int getMinimumFlingVelocity()
        {
            return MINIMUM_FLING_VELOCITY;
        }

        /**
         * @return Minimum velocity to initiate a fling, as measured in pixels per second.
         */
        public int getScaledMinimumFlingVelocity()
        {
            return mMinimumFlingVelocity;
        }

        /**
         * @return Maximum velocity to initiate a fling, as measured in dips per second.
         *
         * [Obsolete] Use {@link #getScaledMaximumFlingVelocity()} instead.
         */
        [Obsolete]
        public static int getMaximumFlingVelocity()
        {
            return MAXIMUM_FLING_VELOCITY;
        }

        /**
         * @return Maximum velocity to initiate a fling, as measured in pixels per second.
         */
        public int getScaledMaximumFlingVelocity()
        {
            return mMaximumFlingVelocity;
        }

        /**
         * @return Amount to scroll in response to a {@link MotionEvent#ACTION_SCROLL} event. Multiply
         * this by the event's axis value to obtain the number of pixels to be scrolled.
         *
         * @removed
         */
        public int getScaledScrollFactor()
        {
            return (int)mVerticalScrollFactor;
        }

        /**
         * @return Amount to scroll in response to a horizontal {@link MotionEvent#ACTION_SCROLL} event.
         * Multiply this by the event's axis value to obtain the number of pixels to be scrolled.
         */
        public float getScaledHorizontalScrollFactor()
        {
            return mHorizontalScrollFactor;
        }

        /**
         * @return Amount to scroll in response to a vertical {@link MotionEvent#ACTION_SCROLL} event.
         * Multiply this by the event's axis value to obtain the number of pixels to be scrolled.
         */
        public float getScaledVerticalScrollFactor()
        {
            return mVerticalScrollFactor;
        }

        /**
         * The maximum drawing cache size expressed in bytes.
         *
         * @return the maximum size of View's drawing cache expressed in bytes
         *
         * [Obsolete] Use {@link #getScaledMaximumDrawingCacheSize()} instead.
         */
        [Obsolete]
        public static int getMaximumDrawingCacheSize()
        {
            //noinspection deprecation
            return MAXIMUM_DRAWING_CACHE_SIZE;
        }

        /**
         * The maximum drawing cache size expressed in bytes.
         *
         * @return the maximum size of View's drawing cache expressed in bytes
         */
        public int getScaledMaximumDrawingCacheSize()
        {
            return mMaximumDrawingCacheSize;
        }

        /**
         * @return The maximum distance a View should overscroll by when showing edge effects (in
         * pixels).
         */
        public int getScaledOverscrollDistance()
        {
            return mOverscrollDistance;
        }

        /**
         * @return The maximum distance a View should overfling by when showing edge effects (in
         * pixels).
         */
        public int getScaledOverflingDistance()
        {
            return mOverflingDistance;
        }

        /**
         * The amount of time that the zoom controls should be
         * displayed on the screen expressed in milliseconds.
         *
         * @return the time the zoom controls should be visible expressed
         * in milliseconds.
         */
        public static long getZoomControlsTimeout()
        {
            return ZOOM_CONTROLS_TIMEOUT;
        }

        /**
         * The amount of time a user needs to press the relevant key to bring up
         * the global actions dialog.
         *
         * @return how long a user needs to press the relevant key to bring up
         *   the global actions dialog.
         * [Obsolete] This timeout should not be used by applications
         */
        [Obsolete]
        public static long getGlobalActionKeyTimeout()
        {
            return GLOBAL_ACTIONS_KEY_TIMEOUT;
        }

        /**
         * The amount of time a user needs to press the relevant key to bring up
         * the global actions dialog.
         *
         * @return how long a user needs to press the relevant key to bring up
         *   the global actions dialog.
         * @hide
         */
        public long getDeviceGlobalActionKeyTimeout()
        {
            return mGlobalActionsKeyTimeout;
        }

        /**
         * The amount of time a user needs to press the relevant keys to trigger
         * the screenshot chord.
         *
         * @return how long a user needs to press the relevant keys to trigger
         *   the screenshot chord.
         * @hide
         */
        public long getScreenshotChordKeyTimeout()
        {
            return mScreenshotChordKeyTimeout;
        }

        /**
         * The amount of time a user needs to press the relevant keys to activate the accessibility
         * shortcut.
         *
         * @return how long a user needs to press the relevant keys to activate the accessibility
         *   shortcut.
         * @hide
         */
        public long getAccessibilityShortcutKeyTimeout()
        {
            return A11Y_SHORTCUT_KEY_TIMEOUT;
        }

        /**
         * @return The amount of time a user needs to press the relevant keys to activate the
         *   accessibility shortcut after it's confirmed that accessibility shortcut is used.
         * @hide
         */
        public long getAccessibilityShortcutKeyTimeoutAfterConfirmation()
        {
            return A11Y_SHORTCUT_KEY_TIMEOUT_AFTER_CONFIRMATION;
        }

        /**
         * The amount of friction applied to scrolls and flings.
         *
         * @return A scalar dimensionless value representing the coefficient of
         *         friction.
         */
        public static float getScrollFriction()
        {
            return SCROLL_FRICTION;
        }

        /**
         * @return the default duration in milliseconds for {@link ActionMode#hide(long)}.
         */
        public static long getDefaultActionModeHideDuration()
        {
            return ACTION_MODE_HIDE_DURATION_DEFAULT;
        }

        /**
         * The multiplication factor for inhibiting default gestures.
         *
         * If a MotionEvent has {@link android.view.MotionEvent#CLASSIFICATION_AMBIGUOUS_GESTURE} set,
         * then certain actions, such as scrolling, will be inhibited. However, to account for the
         * possibility of an incorrect classification, existing gesture thresholds (e.g. scrolling
         * touch slop and the long-press timeout) should be scaled by this factor and remain in effect.
         *
         * [Obsolete] Use {@link #getScaledAmbiguousGestureMultiplier()}.
         */
        [Obsolete]
        public static float getAmbiguousGestureMultiplier()
        {
            return AMBIGUOUS_GESTURE_MULTIPLIER;
        }

        /**
         * The multiplication factor for inhibiting default gestures.
         *
         * If a MotionEvent has {@link android.view.MotionEvent#CLASSIFICATION_AMBIGUOUS_GESTURE} set,
         * then certain actions, such as scrolling, will be inhibited. However, to account for the
         * possibility of an incorrect classification, existing gesture thresholds (e.g. scrolling
         * touch slop and the long-press timeout) should be scaled by this factor and remain in effect.
         */
        public float getScaledAmbiguousGestureMultiplier()
        {
            return mAmbiguousGestureMultiplier;
        }

        /**
         * Report if the device has a permanent menu key available to the user.
         *
         * <p>As of Android 3.0, devices may not have a permanent menu key available.
         * Apps should use the action bar to present menu options to users.
         * However, there are some apps where the action bar is inappropriate
         * or undesirable. This method may be used to detect if a menu key is present.
         * If not, applications should provide another on-screen affordance to access
         * functionality.
         *
         * @return true if a permanent menu key is present, false otherwise.
         */
        public bool hasPermanentMenuKey()
        {
            return sHasPermanentMenuKey;
        }

        /**
         * Check if shortcuts should be displayed in menus.
         *
         * @return {@code True} if shortcuts should be displayed in menus.
         */
        public bool shouldShowMenuShortcutsWhenKeyboardPresent()
        {
            return mShowMenuShortcutsWhenKeyboardPresent;
        }

        /**
         * Retrieves the distance in pixels between touches that must be reached for a gesture to be
         * interpreted as scaling.
         *
         * In general, scaling shouldn't start until this distance has been met or surpassed, and
         * scaling should end when the distance in pixels between touches drops below this distance.
         *
         * @return The distance in pixels
         * @throws IllegalStateException if this method is called on a ViewConfiguration that was
         *         instantiated using a constructor with no Context parameter.
         */
        public int getScaledMinimumScalingSpan()
        {
            if (!mConstructedWithContext)
            {
                throw new IllegalStateException("Min scaling span cannot be determined when this "
                        + "method is called on a ViewConfiguration that was instantiated using a "
                        + "constructor with no Context parameter");
            }
            return mMinScalingSpan;
        }

        /**
         * @hide
         * @return Whether or not marquee should use fading edges.
         */
        public bool isFadingMarqueeEnabled()
        {
            return mFadingMarqueeEnabled;
        }

        /**
         * @return the timeout value in milliseconds to adjust the selection span and actions for the
         *         selected text when TextClassifier has been initialized.
         * @hide
         */
        public int getSmartSelectionInitializedTimeout()
        {
            return mSmartSelectionInitializedTimeout;
        }

        /**
         * @return the timeout value in milliseconds to adjust the selection span and actions for the
         *         selected text when TextClassifier has not been initialized.
         * @hide
         */
        public int getSmartSelectionInitializingTimeout()
        {
            return mSmartSelectionInitializingTimeout;
        }

        /**
         * @return the duration in milliseconds before an end of a long press causes a tooltip to be
         * hidden
         * @hide
         */
        public static int getLongPressTooltipHideTimeout()
        {
            return LONG_PRESS_TOOLTIP_HIDE_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds before a hover event causes a tooltip to be shown
         * @hide
         */
        public static int getHoverTooltipShowTimeout()
        {
            return HOVER_TOOLTIP_SHOW_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds before mouse inactivity causes a tooltip to be hidden
         * (default variant to be used when {@link View#SYSTEM_UI_FLAG_LOW_PROFILE} is not set).
         * @hide
         */
        public static int getHoverTooltipHideTimeout()
        {
            return HOVER_TOOLTIP_HIDE_TIMEOUT;
        }

        /**
         * @return the duration in milliseconds before mouse inactivity causes a tooltip to be hidden
         * (shorter variant to be used when {@link View#SYSTEM_UI_FLAG_LOW_PROFILE} is set).
         * @hide
         */
        public static int getHoverTooltipHideShortTimeout()
        {
            return HOVER_TOOLTIP_HIDE_SHORT_TIMEOUT;
        }
    }
}