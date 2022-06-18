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

using AndroidUI.AnimationFramework.Interpolators;
using AndroidUI.Exceptions;
using AndroidUI.Execution;

namespace AndroidUI.AnimationFramework
{
    /**
     * Abstraction for an Animation that can be applied to Views, Surfaces, or
     * other objects. See the {@link android.view.animation animation package
     * description file}.
     */
    public class Animation : ICloneable
    {
        internal Context context;

        virtual public Animation Clone()
        {
            Animation animation = (Animation)ICloneable.Clone(this);
            // this state is dependant on animation runtime progress
            animation.mEnded = false;
            animation.mStarted = false;
            animation.mCycleFlip = false;
            animation.mRepeated = 0;
            animation.mMore = true;
            animation.mOneMoreTime = true;
            // these can safely be copied without affecting state
            animation.mInitialized = mInitialized;
            animation.mFillBefore = mFillBefore;
            animation.mFillAfter = mFillAfter;
            animation.mFillEnabled = mFillEnabled;
            animation.mStartTime = mStartTime;
            animation.mStartOffset = mStartOffset;
            animation.mDuration = mDuration;
            animation.mRepeatCount = mRepeatCount;
            animation.mRepeatMode = mRepeatMode;
            // do not deep clone the interpolator
            animation.mInterpolator = mInterpolator;
            // do not clone the listener
            animation.mZAdjustment = mZAdjustment;
            animation.mBackgroundColor = mBackgroundColor;
            animation.mScaleFactor = mScaleFactor;
            animation.mShowWallpaper = mShowWallpaper;
            animation.mHasRoundedCorners = mHasRoundedCorners;
            animation.mPreviousRegion = new RectF();
            animation.mRegion = new RectF();
            animation.mTransformation = new Transformation();
            animation.mPreviousTransformation = new Transformation();
            // do not deep clone the context
            animation.context = context;
            return animation;
        }

        /**
         * Repeat the animation indefinitely.
         */
        public const int INFINITE = -1;

        /**
         * When the animation reaches the end and the repeat count is INFINTE_REPEAT
         * or a positive value, the animation restarts from the beginning.
         */
        public const int RESTART = 1;

        /**
         * When the animation reaches the end and the repeat count is INFINTE_REPEAT
         * or a positive value, the animation plays backward (and then forward again).
         */
        public const int REVERSE = 2;

        /**
         * Can be used as the start time to indicate the start time should be the current
         * time when {@link #getTransformation(long, Transformation)} is invoked for the
         * first animation frame. This can is useful for short animations.
         */
        public const int START_ON_FIRST_FRAME = -1;

        /**
         * The specified dimension is an absolute number of pixels.
         */
        public const int ABSOLUTE = 0;

        /**
         * The specified dimension holds a float and should be multiplied by the
         * height or width of the object being animated.
         */
        public const int RELATIVE_TO_SELF = 1;

        /**
         * The specified dimension holds a float and should be multiplied by the
         * height or width of the parent of the object being animated.
         */
        public const int RELATIVE_TO_PARENT = 2;

        /**
         * Requests that the content being animated be kept in its current Z
         * order.
         */
        public const int ZORDER_NORMAL = 0;

        /**
         * Requests that the content being animated be forced on top of all other
         * content for the duration of the animation.
         */
        public const int ZORDER_TOP = 1;

        /**
         * Requests that the content being animated be forced under all other
         * content for the duration of the animation.
         */
        public const int ZORDER_BOTTOM = -1;

        /**
         * Set by {@link #getTransformation(long, Transformation)} when the animation ends.
         */
        protected bool mEnded = false;

        /**
         * Set by {@link #getTransformation(long, Transformation)} when the animation starts.
         */
        protected bool mStarted = false;

        /**
         * Set by {@link #getTransformation(long, Transformation)} when the animation repeats
         * in REVERSE mode.
         */
        protected bool mCycleFlip = false;

        /**
         * This value must be set to true by {@link #initialize(int, int, int, int)}. It
         * indicates the animation was successfully initialized and can be played.
         */
        protected bool mInitialized = false;

        /**
         * Indicates whether the animation transformation should be applied before the
         * animation starts. The value of this variable is only relevant if mFillEnabled is true;
         * otherwise it is assumed to be true.
         */
        protected bool mFillBefore = true;

        /**
         * Indicates whether the animation transformation should be applied after the
         * animation ends.
         */
        protected bool mFillAfter = false;

        /**
         * Indicates whether fillBefore should be taken into account.
         */
        protected bool mFillEnabled = false;

        /**
         * The time in milliseconds at which the animation must start;
         */
        protected long mStartTime = -1;

        /**
         * The delay in milliseconds after which the animation must start. When the
         * start offset is > 0, the start time of the animation is startTime + startOffset.
         */
        protected long mStartOffset;

        /**
         * The duration of one animation cycle in milliseconds.
         */
        protected long mDuration;

        /**
         * The number of times the animation must repeat. By default, an animation repeats
         * indefinitely.
         */
        protected int mRepeatCount = 0;

        /**
         * Indicates how many times the animation was repeated.
         */
        protected int mRepeated = 0;

        /**
         * The behavior of the animation when it repeats. The repeat mode is either
         * {@link #RESTART} or {@link #REVERSE}.
         *
         */
        protected int mRepeatMode = RESTART;

        /**
         * The interpolator used by the animation to smooth the movement.
         */
        internal Interpolator mInterpolator;

        /**
         * An animation listener to be notified when the animation starts, ends or repeats.
         */
        // If you need to chain the AnimationListener, wrap the existing Animation into an AnimationSet
        // and add your new listener to that set
        private AnimationListener mListener;

        /**
         * Desired Z order mode during animation.
         */
        private int mZAdjustment;

        /**
         * Desired background color behind animation.
         */
        private int mBackgroundColor;

        /**
         * scalefactor to apply to pivot points, etc. during animation. Subclasses retrieve the
         * value via getScaleFactor().
         */
        private float mScaleFactor = 1f;

        private bool mShowWallpaper;
        private bool mHasRoundedCorners;

        private bool mMore = true;
        private bool mOneMoreTime = true;


        protected RectF mPreviousRegion = new RectF();

        protected RectF mRegion = new RectF();

        protected Transformation mTransformation = new Transformation();

        protected Transformation mPreviousTransformation = new Transformation();

        private Handler mListenerHandler;
        private Runnable mOnStart;
        private Runnable mOnRepeat;
        private Runnable mOnEnd;

        /**
         * Creates a new animation with a duration of 0ms, the default interpolator, with
         * fillBefore set to true and fillAfter set to false
         */
        public Animation(Context context)
        {
            ensureInterpolator();
            this.context = context;
        }

        /**
         * Reset the initialization state of this animation.
         *
         * @see #initialize(int, int, int, int)
         */
        virtual public void reset()
        {
            mPreviousRegion.setEmpty();
            mPreviousTransformation.clear();
            mInitialized = false;
            mCycleFlip = false;
            mRepeated = 0;
            mMore = true;
            mOneMoreTime = true;
            mListenerHandler = null;
        }

        /**
         * Cancel the animation. Cancelling an animation invokes the animation
         * listener, if set, to notify the end of the animation.
         *
         * If you cancel an animation manually, you must call {@link #reset()}
         * before starting the animation again.
         *
         * @see #reset()
         * @see #start()
         * @see #startNow()
         */
        public void cancel()
        {
            if (mStarted && !mEnded)
            {
                fireAnimationEnd();
                mEnded = true;
            }
            // Make sure we move the animation to the end
            mStartTime = long.MinValue;
            mMore = mOneMoreTime = false;
        }

        /**
         * @hide
         */

        internal void detach()
        {
            if (mStarted && !mEnded)
            {
                mEnded = true;
                fireAnimationEnd();
            }
        }

        /**
         * Whether or not the animation has been initialized.
         *
         * @return Has this animation been initialized.
         * @see #initialize(int, int, int, int)
         */
        public bool isInitialized()
        {
            return mInitialized;
        }

        /**
         * Initialize this animation with the dimensions of the object being
         * animated as well as the objects parents. (This is to support animation
         * sizes being specified relative to these dimensions.)
         *
         * <p>Objects that interpret Animations should call this method when
         * the sizes of the object being animated and its parent are known, and
         * before calling {@link #getTransformation}.
         *
         *
         * @param width Width of the object being animated
         * @param height Height of the object being animated
         * @param parentWidth Width of the animated object's parent
         * @param parentHeight Height of the animated object's parent
         */
        virtual public void initialize(int width, int height, int parentWidth, int parentHeight)
        {
            reset();
            mInitialized = true;
        }

        /**
         * Sets the handler used to invoke listeners.
         *
         * @hide
         */
        internal void setListenerHandler(Handler handler)
        {
            if (mListenerHandler == null)
            {
                mOnStart = Runnable.Create(() => dispatchAnimationStart());
                mOnRepeat = Runnable.Create(() => dispatchAnimationRepeat());
                mOnEnd = Runnable.Create(() => dispatchAnimationEnd());
            };
            mListenerHandler = handler;
        }

        /**
         * Sets the acceleration curve for this animation. Defaults to a linear
         * interpolation.
         *
         * @param i The interpolator which defines the acceleration curve
         * @attr ref android.R.styleable#Animation_interpolator
         */
        virtual public void setInterpolator(Interpolator i)
        {
            mInterpolator = i;
        }

        /**
         * When this animation should start relative to the start time. This is most
         * useful when composing complex animations using an {@link AnimationSet }
         * where some of the animations components start at different times.
         *
         * @param startOffset When this Animation should start, in milliseconds from
         *                    the start time of the root AnimationSet.
         * @attr ref android.R.styleable#Animation_startOffset
         */
        virtual public void setStartOffset(long startOffset)
        {
            mStartOffset = startOffset;
        }

        /**
         * How long this animation should last. The duration cannot be negative.
         *
         * @param durationMillis Duration in milliseconds
         *
         * @throws java.lang.IllegalArgumentException if the duration is < 0
         *
         * @attr ref android.R.styleable#Animation_duration
         */
        virtual public void setDuration(long durationMillis)
        {
            if (durationMillis < 0)
            {
                throw new IllegalArgumentException("Animation duration cannot be negative");
            }
            mDuration = durationMillis;
        }

        /**
         * Ensure that the duration that this animation will run is not longer
         * than <var>durationMillis</var>.  In addition to adjusting the duration
         * itself, this ensures that the repeat count also will not make it run
         * longer than the given time.
         *
         * @param durationMillis The maximum duration the animation is allowed
         * to run.
         */
        virtual public void restrictDuration(long durationMillis)
        {
            // If we start after the duration, then we just won't run.
            if (mStartOffset > durationMillis)
            {
                mStartOffset = durationMillis;
                mDuration = 0;
                mRepeatCount = 0;
                return;
            }

            long dur = mDuration + mStartOffset;
            if (dur > durationMillis)
            {
                mDuration = durationMillis - mStartOffset;
                dur = durationMillis;
            }
            // If the duration is 0 or less, then we won't run.
            if (mDuration <= 0)
            {
                mDuration = 0;
                mRepeatCount = 0;
                return;
            }
            // Reduce the number of repeats to keep below the maximum duration.
            // The comparison between mRepeatCount and duration is to catch
            // overflows after multiplying them.
            if (mRepeatCount < 0 || mRepeatCount > durationMillis
                    || (dur * mRepeatCount) > durationMillis)
            {
                // Figure out how many times to do the animation.  Subtract 1 since
                // repeat count is the number of times to repeat so 0 runs once.
                mRepeatCount = (int)(durationMillis / dur) - 1;
                if (mRepeatCount < 0)
                {
                    mRepeatCount = 0;
                }
            }
        }

        /**
         * How much to scale the duration by.
         *
         * @param scale The amount to scale the duration.
         */
        virtual public void scaleCurrentDuration(float scale)
        {
            mDuration = (long)(mDuration * scale);
            mStartOffset = (long)(mStartOffset * scale);
        }

        /**
         * When this animation should start. When the start time is set to
         * {@link #START_ON_FIRST_FRAME}, the animation will start the first time
         * {@link #getTransformation(long, Transformation)} is invoked. The time passed
         * to this method should be obtained by calling
         * {@link AnimationUtils#currentAnimationTimeMillis()} instead of
         * {@link System#currentTimeMillis()}.
         *
         * @param startTimeMillis the start time in milliseconds
         */
        virtual public void setStartTime(long startTimeMillis)
        {
            mStartTime = startTimeMillis;
            mStarted = mEnded = false;
            mCycleFlip = false;
            mRepeated = 0;
            mMore = true;
        }

        /**
         * Convenience method to start the animation the first time
         * {@link #getTransformation(long, Transformation)} is invoked.
         */
        virtual public void start()
        {
            setStartTime(-1);
        }

        /**
         * Convenience method to start the animation at the current time in
         * milliseconds.
         */
        virtual public void startNow()
        {
            setStartTime(AnimationUtils.currentAnimationTimeMillis(context));
        }

        /**
         * Defines what this animation should do when it reaches the end. This
         * setting is applied only when the repeat count is either greater than
         * 0 or {@link #INFINITE}. Defaults to {@link #RESTART}.
         *
         * @param repeatMode {@link #RESTART} or {@link #REVERSE}
         * @attr ref android.R.styleable#Animation_repeatMode
         */
        virtual public void setRepeatMode(int repeatMode)
        {
            mRepeatMode = repeatMode;
        }

        /**
         * Sets how many times the animation should be repeated. If the repeat
         * count is 0, the animation is never repeated. If the repeat count is
         * greater than 0 or {@link #INFINITE}, the repeat mode will be taken
         * into account. The repeat count is 0 by default.
         *
         * @param repeatCount the number of times the animation should be repeated
         * @attr ref android.R.styleable#Animation_repeatCount
         */
        virtual public void setRepeatCount(int repeatCount)
        {
            if (repeatCount < 0)
            {
                repeatCount = INFINITE;
            }
            mRepeatCount = repeatCount;
        }

        /**
         * If fillEnabled is true, this animation will apply the value of fillBefore.
         *
         * @return true if the animation will take fillBefore into account
         * @attr ref android.R.styleable#Animation_fillEnabled
         */
        public bool isFillEnabled()
        {
            return mFillEnabled;
        }

        /**
         * If fillEnabled is true, the animation will apply the value of fillBefore.
         * Otherwise, fillBefore is ignored and the animation
         * transformation is always applied until the animation ends.
         *
         * @param fillEnabled true if the animation should take the value of fillBefore into account
         * @attr ref android.R.styleable#Animation_fillEnabled
         *
         * @see #setFillBefore(bool)
         * @see #setFillAfter(bool)
         */
        virtual public void setFillEnabled(bool fillEnabled)
        {
            mFillEnabled = fillEnabled;
        }

        /**
         * If fillBefore is true, this animation will apply its transformation
         * before the start time of the animation. Defaults to true if
         * {@link #setFillEnabled(bool)} is not set to true.
         * Note that this applies when using an {@link
         * android.view.animation.AnimationSet AnimationSet} to chain
         * animations. The transformation is not applied before the AnimationSet
         * itself starts.
         *
         * @param fillBefore true if the animation should apply its transformation before it starts
         * @attr ref android.R.styleable#Animation_fillBefore
         *
         * @see #setFillEnabled(bool)
         */
        virtual public void setFillBefore(bool fillBefore)
        {
            mFillBefore = fillBefore;
        }

        /**
         * If fillAfter is true, the transformation that this animation performed
         * will persist when it is finished. Defaults to false if not set.
         * Note that this applies to individual animations and when using an {@link
         * android.view.animation.AnimationSet AnimationSet} to chain
         * animations.
         *
         * @param fillAfter true if the animation should apply its transformation after it ends
         * @attr ref android.R.styleable#Animation_fillAfter
         *
         * @see #setFillEnabled(bool)
         */
        virtual public void setFillAfter(bool fillAfter)
        {
            mFillAfter = fillAfter;
        }

        /**
         * Set the Z ordering mode to use while running the animation.
         *
         * @param zAdjustment The desired mode, one of {@link #ZORDER_NORMAL},
         * {@link #ZORDER_TOP}, or {@link #ZORDER_BOTTOM}.
         * @attr ref android.R.styleable#Animation_zAdjustment
         */
        virtual public void setZAdjustment(int zAdjustment)
        {
            mZAdjustment = zAdjustment;
        }

        /**
         * Set background behind animation.
         *
         * @param bg The background color.  If 0, no background.  Currently must
         * be black, with any desired alpha level.
         *
         * @deprecated None of window animations are running with background color.
         */
        [Obsolete]
        public void setBackgroundColor(int bg)
        {
            // The background color is not needed any more, do nothing.
        }

        /**
         * The scale factor is set by the call to <code>getTransformation</code>. Overrides of
         * {@link #getTransformation(long, Transformation, float)} will get this value
         * directly. Overrides of {@link #applyTransformation(float, Transformation)} can
         * call this method to get the value.
         *
         * @return float The scale factor that should be applied to pre-scaled values in
         * an Animation such as the pivot points in {@link ScaleAnimation} and {@link RotateAnimation}.
         */
        protected float getScaleFactor()
        {
            return mScaleFactor;
        }

        /**
         * If detachWallpaper is true, and this is a window animation of a window
         * that has a wallpaper background, then the window will be detached from
         * the wallpaper while it runs.  That is, the animation will only be applied
         * to the window, and the wallpaper behind it will remain static.
         *
         * @param detachWallpaper true if the wallpaper should be detached from the animation
         * @attr ref android.R.styleable#Animation_detachWallpaper
         *
         * @deprecated All window animations are running with detached wallpaper.
         */
        [Obsolete]
        public void setDetachWallpaper(bool detachWallpaper)
        {
        }

        /**
         * If this animation is run as a window animation, this will make the wallpaper visible behind
         * the animation.
         *
         * @param showWallpaper Whether the wallpaper should be shown during the animation.
         * @attr ref android.R.styleable#Animation_detachWallpaper
         * @hide
         */
        internal void setShowWallpaper(bool showWallpaper)
        {
            mShowWallpaper = showWallpaper;
        }

        /**
         * If this is a window animation, the window will have rounded corners matching the display
         * corner radius.
         *
         * @param hasRoundedCorners Whether the window should have rounded corners or not.
         * @attr ref android.R.styleable#Animation_hasRoundedCorners
         * @see com.android.internal.policy.ScreenDecorationsUtils#getWindowCornerRadius(Resources)
         * @hide
         */
        internal void setHasRoundedCorners(bool hasRoundedCorners)
        {
            mHasRoundedCorners = hasRoundedCorners;
        }

        /**
         * Gets the acceleration curve type for this animation.
         *
         * @return the {@link Interpolator} associated to this animation
         * @attr ref android.R.styleable#Animation_interpolator
         */
        virtual public Interpolator getInterpolator()
        {
            return mInterpolator;
        }

        /**
         * When this animation should start. If the animation has not startet yet,
         * this method might return {@link #START_ON_FIRST_FRAME}.
         *
         * @return the time in milliseconds when the animation should start or
         *         {@link #START_ON_FIRST_FRAME}
         */
        virtual public long getStartTime()
        {
            return mStartTime;
        }

        /**
         * How long this animation should last
         *
         * @return the duration in milliseconds of the animation
         * @attr ref android.R.styleable#Animation_duration
         */
        virtual public long getDuration()
        {
            return mDuration;
        }

        /**
         * When this animation should start, relative to StartTime
         *
         * @return the start offset in milliseconds
         * @attr ref android.R.styleable#Animation_startOffset
         */
        virtual public long getStartOffset()
        {
            return mStartOffset;
        }

        /**
         * Defines what this animation should do when it reaches the end.
         *
         * @return either one of {@link #REVERSE} or {@link #RESTART}
         * @attr ref android.R.styleable#Animation_repeatMode
         */
        public int getRepeatMode()
        {
            return mRepeatMode;
        }

        /**
         * Defines how many times the animation should repeat. The default value
         * is 0.
         *
         * @return the number of times the animation should repeat, or {@link #INFINITE}
         * @attr ref android.R.styleable#Animation_repeatCount
         */
        virtual public int getRepeatCount()
        {
            return mRepeatCount;
        }

        /**
         * If fillBefore is true, this animation will apply its transformation
         * before the start time of the animation. If fillBefore is false and
         * {@link #isFillEnabled() fillEnabled} is true, the transformation will not be applied until
         * the start time of the animation.
         *
         * @return true if the animation applies its transformation before it starts
         * @attr ref android.R.styleable#Animation_fillBefore
         */
        public bool getFillBefore()
        {
            return mFillBefore;
        }

        /**
         * If fillAfter is true, this animation will apply its transformation
         * after the end time of the animation.
         *
         * @return true if the animation applies its transformation after it ends
         * @attr ref android.R.styleable#Animation_fillAfter
         */
        public bool getFillAfter()
        {
            return mFillAfter;
        }

        /**
         * Returns the Z ordering mode to use while running the animation as
         * previously set by {@link #setZAdjustment}.
         *
         * @return Returns one of {@link #ZORDER_NORMAL},
         * {@link #ZORDER_TOP}, or {@link #ZORDER_BOTTOM}.
         * @attr ref android.R.styleable#Animation_zAdjustment
         */
        public int getZAdjustment()
        {
            return mZAdjustment;
        }

        /**
         * Returns the background color behind the animation.
         *
         * @deprecated None of window animations are running with background color.
         */
        [Obsolete]
        public int getBackgroundColor()
        {
            return 0;
        }

        /**
         * Return value of {@link #setDetachWallpaper(bool)}.
         * @attr ref android.R.styleable#Animation_detachWallpaper
         *
         * @deprecated All window animations are running with detached wallpaper.
         */
        [Obsolete]
        public bool getDetachWallpaper()
        {
            return true;
        }

        /**
         * @return If run as a window animation, returns whether the wallpaper will be shown behind
         *         during the animation.
         * @attr ref android.R.styleable#Animation_showWallpaper
         * @hide
         */
        internal bool getShowWallpaper()
        {
            return mShowWallpaper;
        }

        /**
         * @return if a window animation should have rounded corners or not.
         *
         * @attr ref android.R.styleable#Animation_hasRoundedCorners
         * @hide
         */
        internal bool hasRoundedCorners()
        {
            return mHasRoundedCorners;
        }

        /**
         * <p>Indicates whether or not this animation will affect the transformation
         * matrix. For instance, a fade animation will not affect the matrix whereas
         * a scale animation will.</p>
         *
         * @return true if this animation will change the transformation matrix
         */
        virtual public bool willChangeTransformationMatrix()
        {
            // assume we will change the matrix
            return true;
        }

        /**
         * <p>Indicates whether or not this animation will affect the bounds of the
         * animated view. For instance, a fade animation will not affect the bounds
         * whereas a 200% scale animation will.</p>
         *
         * @return true if this animation will change the view's bounds
         */
        virtual public bool willChangeBounds()
        {
            // assume we will change the bounds
            return true;
        }

        private bool hasAnimationListener()
        {
            return mListener != null;
        }

        /**
         * <p>Binds an animation listener to this animation. The animation listener
         * is notified of animation events such as the end of the animation or the
         * repetition of the animation.</p>
         *
         * @param listener the animation listener to be notified
         */
        public void setAnimationListener(AnimationListener listener)
        {
            mListener = listener;
        }

        /**
         * Gurantees that this animation has an interpolator. Will use
         * a AccelerateDecelerateInterpolator is nothing else was specified.
         */
        protected void ensureInterpolator()
        {
            if (mInterpolator == null)
            {
                mInterpolator = new AccelerateDecelerateInterpolator();
            }
        }

        /**
         * Compute a hint at how long the entire animation may last, in milliseconds.
         * Animations can be written to cause themselves to run for a different
         * duration than what is computed here, but generally this should be
         * accurate.
         */
        virtual public long computeDurationHint()
        {
            return (getStartOffset() + getDuration()) * (getRepeatCount() + 1);
        }

        /**
         * Gets the transformation to apply at a specified point in time. Implementations of this
         * method should always replace the specified Transformation or document they are doing
         * otherwise.
         *
         * @param currentTime Where we are in the animation. This is wall clock time.
         * @param outTransformation A transformation object that is provided by the
         *        caller and will be filled in by the animation.
         * @return True if the animation is still running
         */
        virtual public bool getTransformation(long currentTime, Transformation outTransformation)
        {
            if (mStartTime == -1)
            {
                mStartTime = currentTime;
            }

            long startOffset = getStartOffset();
            long duration = mDuration;
            float normalizedTime;
            if (duration != 0)
            {
                normalizedTime = ((float)(currentTime - (mStartTime + startOffset))) /
                        (float)duration;
            }
            else
            {
                // time is a step-change with a zero duration
                normalizedTime = currentTime < mStartTime ? 0.0f : 1.0f;
            }

            bool expired = normalizedTime >= 1.0f || isCanceled();
            mMore = !expired;

            if (!mFillEnabled) normalizedTime = Math.Max(Math.Min(normalizedTime, 1.0f), 0.0f);

            if ((normalizedTime >= 0.0f || mFillBefore) && (normalizedTime <= 1.0f || mFillAfter))
            {
                if (!mStarted)
                {
                    fireAnimationStart();
                    mStarted = true;
                }

                if (mFillEnabled) normalizedTime = Math.Max(Math.Min(normalizedTime, 1.0f), 0.0f);

                if (mCycleFlip)
                {
                    normalizedTime = 1.0f - normalizedTime;
                }

                float interpolatedTime = mInterpolator.getInterpolation(normalizedTime);
                applyTransformation(interpolatedTime, outTransformation);
            }

            if (expired)
            {
                if (mRepeatCount == mRepeated || isCanceled())
                {
                    if (!mEnded)
                    {
                        mEnded = true;
                        fireAnimationEnd();
                    }
                }
                else
                {
                    if (mRepeatCount > 0)
                    {
                        mRepeated++;
                    }

                    if (mRepeatMode == REVERSE)
                    {
                        mCycleFlip = !mCycleFlip;
                    }

                    mStartTime = -1;
                    mMore = true;

                    fireAnimationRepeat();
                }
            }

            if (!mMore && mOneMoreTime)
            {
                mOneMoreTime = false;
                return true;
            }

            return mMore;
        }

        private bool isCanceled()
        {
            return mStartTime == long.MinValue;
        }

        private void fireAnimationStart()
        {
            if (hasAnimationListener())
            {
                if (mListenerHandler == null) dispatchAnimationStart();
                else mListenerHandler.postAtFrontOfQueue(mOnStart);
            }
        }

        private void fireAnimationRepeat()
        {
            if (hasAnimationListener())
            {
                if (mListenerHandler == null) dispatchAnimationRepeat();
                else mListenerHandler.postAtFrontOfQueue(mOnRepeat);
            }
        }

        private void fireAnimationEnd()
        {
            if (hasAnimationListener())
            {
                if (mListenerHandler == null) dispatchAnimationEnd();
                else mListenerHandler.postAtFrontOfQueue(mOnEnd);
            }
        }

        internal void dispatchAnimationStart()
        {
            if (mListener != null)
            {
                mListener.onAnimationStart(this);
            }
        }

        internal void dispatchAnimationRepeat()
        {
            if (mListener != null)
            {
                mListener.onAnimationRepeat(this);
            }
        }

        internal void dispatchAnimationEnd()
        {
            if (mListener != null)
            {
                mListener.onAnimationEnd(this);
            }
        }

        /**
         * Gets the transformation to apply at a specified point in time. Implementations of this
         * method should always replace the specified Transformation or document they are doing
         * otherwise.
         *
         * @param currentTime Where we are in the animation. This is wall clock time.
         * @param outTransformation A transformation object that is provided by the
         *        caller and will be filled in by the animation.
         * @param scale Scaling factor to apply to any inputs to the transform operation, such
         *        pivot points being rotated or scaled around.
         * @return True if the animation is still running
         */
        public bool getTransformation(long currentTime, Transformation outTransformation,
                float scale)
        {
            mScaleFactor = scale;
            return getTransformation(currentTime, outTransformation);
        }

        /**
         * <p>Indicates whether this animation has started or not.</p>
         *
         * @return true if the animation has started, false otherwise
         */
        public bool hasStarted()
        {
            return mStarted;
        }

        /**
         * <p>Indicates whether this animation has ended or not.</p>
         *
         * @return true if the animation has ended, false otherwise
         */
        public bool hasEnded()
        {
            return mEnded;
        }

        /**
         * Helper for getTransformation. Subclasses should implement this to apply
         * their transforms given an interpolation value.  Implementations of this
         * method should always replace the specified Transformation or document
         * they are doing otherwise.
         *
         * @param interpolatedTime The value of the normalized time (0.0 to 1.0)
         *        after it has been run through the interpolation function.
         * @param t The Transformation object to fill in with the current
         *        transforms.
         */
        virtual public void applyTransformation(float interpolatedTime, Transformation t)
        {
        }

        /**
         * Convert the information in the description of a size to an actual
         * dimension
         *
         * @param type One of Animation.ABSOLUTE, Animation.RELATIVE_TO_SELF, or
         *             Animation.RELATIVE_TO_PARENT.
         * @param value The dimension associated with the type parameter
         * @param size The size of the object being animated
         * @param parentSize The size of the parent of the object being animated
         * @return The dimension to use for the animation
         */
        virtual protected float resolveSize(int type, float value, int size, int parentSize) {
            switch (type) {
                case ABSOLUTE:
                    return value;
                case RELATIVE_TO_SELF:
                    return size * value;
                case RELATIVE_TO_PARENT:
                    return parentSize * value;
                default:
                    return value;
            }
        }

        /**
         * @param left
         * @param top
         * @param right
         * @param bottom
         * @param invalidate
         * @param transformation
         *
         * @hide
         */
        virtual internal void getInvalidateRegion(int left, int top, int right, int bottom,
                RectF invalidate, Transformation transformation)
        {

            RectF tempRegion = mRegion;
            RectF previousRegion = mPreviousRegion;

            invalidate.set(left, top, right, bottom);
            transformation.getMatrix().Value.MapRect(invalidate);
            // Enlarge the invalidate region to account for rounding errors
            invalidate.inset(-1.0f, -1.0f);
            tempRegion.set(invalidate);
            invalidate.union(previousRegion);

            previousRegion.set(tempRegion);

            Transformation tempTransformation = mTransformation;
            Transformation previousTransformation = mPreviousTransformation;

            tempTransformation.set(transformation);
            transformation.set(previousTransformation);
            previousTransformation.set(tempTransformation);
        }

        /**
         * @param left
         * @param top
         * @param right
         * @param bottom
         *
         * @hide
         */
        virtual internal void initializeInvalidateRegion(int left, int top, int right, int bottom)
        {
            RectF region = mPreviousRegion;
            region.set(left, top, right, bottom);
            // Enlarge the invalidate region to account for rounding errors
            region.inset(-1.0f, -1.0f);
            if (mFillBefore)
            {
                Transformation previousTransformation = mPreviousTransformation;
                applyTransformation(mInterpolator.getInterpolation(0.0f), previousTransformation);
            }
        }


        /**
         * Return true if this animation changes the view's alpha property.
         *
         * @hide
         */
        virtual internal bool hasAlpha()
        {
            return false;
        }

        /**
         * <p>An animation listener receives notifications from an animation.
         * Notifications indicate animation related events, such as the end or the
         * repetition of the animation.</p>
         */
        public interface AnimationListener
        {
            /**
             * <p>Notifies the start of the animation.</p>
             *
             * @param animation The started animation.
             */
            void onAnimationStart(Animation animation);

            /**
             * <p>Notifies the end of the animation. This callback is not invoked
             * for animations with repeat count set to INFINITE.</p>
             *
             * @param animation The animation which reached its end.
             */
            void onAnimationEnd(Animation animation);

            /**
             * <p>Notifies the repetition of the animation.</p>
             *
             * @param animation The animation which was repeated.
             */
            void onAnimationRepeat(Animation animation);
        }
    }
}
