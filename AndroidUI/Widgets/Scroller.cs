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

using AndroidUI.AnimationFramework.Animation;
using AndroidUI.AnimationFramework.Interpolators;
using AndroidUI.Applications;
using AndroidUI.Utils;
using AndroidUI.Utils.Const;

namespace AndroidUI.Widgets
{
    /**
     * <p>This class encapsulates scrolling. You can use scrollers ({@link Scroller}
     * or {@link OverScroller}) to collect the data you need to produce a scrolling
     * animation&mdash;for example, in response to a fling gesture. Scrollers track
     * scroll offsets for you over time, but they don't automatically apply those
     * positions to your view. It's your responsibility to get and apply new
     * coordinates at a rate that will make the scrolling animation look smooth.</p>
     *
     * <p>Here is a simple example:</p>
     *
     * <pre> internal Scroller mScroller = new Scroller(context);
     * ...
     * public void zoomIn() {
     *     // Revert any animation currently in progress
     *     mScroller.forceFinished(true);
     *     // Start scrolling by providing a starting point and
     *     // the distance to travel
     *     mScroller.startScroll(0, 0, 100, 0);
     *     // Invalidate to request a redraw
     *     invalidate();
     * }</pre>
     *
     * <p>To track the changing positions of the x/y coordinates, use
     * {@link #computeScrollOffset}. The method returns a bool to indicate
     * whether the scroller is finished. If it isn't, it means that a fling or
     * programmatic pan operation is still in progress. You can use this method to
     * find the current offsets of the x and y coordinates, for example:</p>
     *
     * <pre>if (mScroller.computeScrollOffset()) {
     *     // Get current x and y positions
     *     int currX = mScroller.getCurrX();
     *     int currY = mScroller.getCurrY();
     *    ...
     * }</pre>
     */
    public class Scroller
    {
        Context context;
        internal readonly Interpolator mInterpolator;

        internal int mMode;

        internal int mStartX;
        internal int mStartY;
        internal int mFinalX;
        internal int mFinalY;

        internal int mMinX;
        internal int mMaxX;
        internal int mMinY;
        internal int mMaxY;

        internal int mCurrX;
        internal int mCurrY;
        internal long mStartTime;
        internal int mDuration;
        internal float mDurationReciprocal;
        internal float mDeltaX;
        internal float mDeltaY;
        internal bool mFinished;
        internal bool mFlywheel;

        internal float mVelocity;
        internal float mCurrVelocity;
        internal int mDistance;

        internal float mFlingFriction = ViewConfiguration.getScrollFriction();

        internal const int DEFAULT_DURATION = 250;
        internal const int SCROLL_MODE = 0;
        internal const int FLING_MODE = 1;

        internal static float DECELERATION_RATE = (float)(Math.Log(0.78) / Math.Log(0.9));
        internal const float INFLEXION = 0.35f; // Tension lines cross at (INFLEXION, 1)
        internal const float START_TENSION = 0.5f;
        internal const float END_TENSION = 1.0f;
        internal const float P1 = START_TENSION * INFLEXION;
        internal const float P2 = 1.0f - END_TENSION * (1.0f - INFLEXION);

        internal const int NB_SAMPLES = 100;
        internal static readonly float[] SPLINE_POSITION = new float[NB_SAMPLES + 1];
        internal static readonly float[] SPLINE_TIME = new float[NB_SAMPLES + 1];

        internal float mDeceleration;
        internal float mPpi;

        // A context-specific coefficient adjusted to physical values.
        internal float mPhysicalCoeff;

        static Scroller()
        {
            float x_min = 0.0f;
            float y_min = 0.0f;
            for (int i = 0; i < NB_SAMPLES; i++)
            {
                float alpha = (float)i / NB_SAMPLES;

                float x_max = 1.0f;
                float x, tx, coef;
                while (true)
                {
                    x = x_min + (x_max - x_min) / 2.0f;
                    coef = 3.0f * x * (1.0f - x);
                    tx = coef * ((1.0f - x) * P1 + x * P2) + x * x * x;
                    if (Math.Abs(tx - alpha) < 1E-5) break;
                    if (tx > alpha) x_max = x;
                    else x_min = x;
                }
                SPLINE_POSITION[i] = coef * ((1.0f - x) * START_TENSION + x) + x * x * x;

                float y_max = 1.0f;
                float y, dy;
                while (true)
                {
                    y = y_min + (y_max - y_min) / 2.0f;
                    coef = 3.0f * y * (1.0f - y);
                    dy = coef * ((1.0f - y) * START_TENSION + y) + y * y * y;
                    if (Math.Abs(dy - alpha) < 1E-5) break;
                    if (dy > alpha) y_max = y;
                    else y_min = y;
                }
                SPLINE_TIME[i] = coef * ((1.0f - y) * P1 + y * P2) + y * y * y;
            }
            SPLINE_POSITION[NB_SAMPLES] = SPLINE_TIME[NB_SAMPLES] = 1.0f;
        }

        /**
         * Create a Scroller with the default duration and interpolator.
         */
        public Scroller(Context context)
        : this(context, null)
        {
        }

        /**
         * Create a Scroller with the specified interpolator. If the interpolator is
         * null, the default (viscous) interpolator will be used. "Flywheel" behavior will
         * be in effect for apps targeting Honeycomb or newer.
         */
        public Scroller(Context context, Interpolator interpolator)
            : this(context, interpolator, true)
        {
        }

        /**
         * Create a Scroller with the specified interpolator. If the interpolator is
         * null, the default (viscous) interpolator will be used. Specify whether or
         * not to support progressive "flywheel" behavior in flinging.
         */
        public Scroller(Context context, Interpolator interpolator, bool flywheel)
        {
            this.context = context;
            mFinished = true;
            if (interpolator == null)
            {
                mInterpolator = new ViscousFluidInterpolator();
            }
            else
            {
                mInterpolator = interpolator;
            }
            mPpi = DensityManager.ScreenDensityAsFloat * 160.0f;
            mDeceleration = computeDeceleration(ViewConfiguration.getScrollFriction());
            mFlywheel = flywheel;

            mPhysicalCoeff = computeDeceleration(0.84f); // look and feel tuning
        }

        /**
         * The amount of friction applied to flings. The default value
         * is {@link ViewConfiguration#getScrollFriction}.
         * 
         * @param friction A scalar dimension-less value representing the coefficient of
         *         friction.
         */
        public void setFriction(float friction)
        {
            mDeceleration = computeDeceleration(friction);
            mFlingFriction = friction;
        }

        internal float computeDeceleration(float friction)
        {
            return Constants.GRAVITY_EARTH   // g (m/s^2)
                          * 39.37f               // inch/meter
                          * mPpi                 // pixels per inch
                          * friction;
        }

        /**
         * 
         * Returns whether the scroller has finished scrolling.
         * 
         * @return True if the scroller has finished scrolling, false otherwise.
         */
        public bool isFinished()
        {
            return mFinished;
        }

        /**
         * Force the finished field to a particular value.
         *  
         * @param finished The new finished value.
         */
        public void forceFinished(bool finished)
        {
            mFinished = finished;
        }

        /**
         * Returns how long the scroll event will take, in milliseconds.
         * 
         * @return The duration of the scroll in milliseconds.
         */
        public int getDuration()
        {
            return mDuration;
        }

        /**
         * Returns the current X offset in the scroll. 
         * 
         * @return The new X offset as an absolute distance from the origin.
         */
        public int getCurrX()
        {
            return mCurrX;
        }

        /**
         * Returns the current Y offset in the scroll. 
         * 
         * @return The new Y offset as an absolute distance from the origin.
         */
        public int getCurrY()
        {
            return mCurrY;
        }

        /**
         * Returns the current velocity.
         *
         * @return The original velocity less the deceleration. Result may be
         * negative.
         */
        public float getCurrVelocity()
        {
            return mMode == FLING_MODE ?
                    mCurrVelocity : mVelocity - mDeceleration * timePassed() / 2000.0f;
        }

        /**
         * Returns the start X offset in the scroll. 
         * 
         * @return The start X offset as an absolute distance from the origin.
         */
        public int getStartX()
        {
            return mStartX;
        }

        /**
         * Returns the start Y offset in the scroll. 
         * 
         * @return The start Y offset as an absolute distance from the origin.
         */
        public int getStartY()
        {
            return mStartY;
        }

        /**
         * Returns where the scroll will end. Valid only for "fling" scrolls.
         * 
         * @return The X offset as an absolute distance from the origin.
         */
        public int getFinalX()
        {
            return mFinalX;
        }

        /**
         * Returns where the scroll will end. Valid only for "fling" scrolls.
         * 
         * @return The Y offset as an absolute distance from the origin.
         */
        public int getFinalY()
        {
            return mFinalY;
        }

        /**
         * Call this when you want to know the new location.  If it returns true,
         * the animation is not yet finished.
         */
        public bool computeScrollOffset()
        {
            if (mFinished)
            {
                return false;
            }

            int timePassed = (int)(AnimationUtils.currentAnimationTimeMillis(context) - mStartTime);

            if (timePassed < mDuration)
            {
                switch (mMode)
                {
                    case SCROLL_MODE:
                        float x = mInterpolator.getInterpolation(timePassed * mDurationReciprocal);
                        mCurrX = (int)(mStartX + Math.Round(x * mDeltaX));
                        mCurrY = (int)(mStartY + Math.Round(x * mDeltaY));
                        break;
                    case FLING_MODE:
                        float t = (float)timePassed / mDuration;
                        int index = (int)(NB_SAMPLES * t);
                        float distanceCoef = 1.0f;
                        float velocityCoef = 0.0f;
                        if (index < NB_SAMPLES)
                        {
                            float t_inf = (float)index / NB_SAMPLES;
                            float t_sup = (float)(index + 1) / NB_SAMPLES;
                            float d_inf = SPLINE_POSITION[index];
                            float d_sup = SPLINE_POSITION[index + 1];
                            velocityCoef = (d_sup - d_inf) / (t_sup - t_inf);
                            distanceCoef = d_inf + (t - t_inf) * velocityCoef;
                        }

                        mCurrVelocity = velocityCoef * mDistance / mDuration * 1000.0f;

                        mCurrX = (int)(mStartX + Math.Round(distanceCoef * (mFinalX - mStartX)));
                        // Pin to mMinX <= mCurrX <= mMaxX
                        mCurrX = Math.Min(mCurrX, mMaxX);
                        mCurrX = Math.Max(mCurrX, mMinX);

                        mCurrY = (int)(mStartY + Math.Round(distanceCoef * (mFinalY - mStartY)));
                        // Pin to mMinY <= mCurrY <= mMaxY
                        mCurrY = Math.Min(mCurrY, mMaxY);
                        mCurrY = Math.Max(mCurrY, mMinY);

                        if (mCurrX == mFinalX && mCurrY == mFinalY)
                        {
                            mFinished = true;
                        }

                        break;
                }
            }
            else
            {
                mCurrX = mFinalX;
                mCurrY = mFinalY;
                mFinished = true;
            }
            return true;
        }

        /**
         * Start scrolling by providing a starting point and the distance to travel.
         * The scroll will use the default value of 250 milliseconds for the
         * duration.
         * 
         * @param startX Starting horizontal scroll offset in pixels. Positive
         *        numbers will scroll the content to the left.
         * @param startY Starting vertical scroll offset in pixels. Positive numbers
         *        will scroll the content up.
         * @param dx Horizontal distance to travel. Positive numbers will scroll the
         *        content to the left.
         * @param dy Vertical distance to travel. Positive numbers will scroll the
         *        content up.
         */
        public void startScroll(int startX, int startY, int dx, int dy)
        {
            startScroll(startX, startY, dx, dy, DEFAULT_DURATION);
        }

        /**
         * Start scrolling by providing a starting point, the distance to travel,
         * and the duration of the scroll.
         * 
         * @param startX Starting horizontal scroll offset in pixels. Positive
         *        numbers will scroll the content to the left.
         * @param startY Starting vertical scroll offset in pixels. Positive numbers
         *        will scroll the content up.
         * @param dx Horizontal distance to travel. Positive numbers will scroll the
         *        content to the left.
         * @param dy Vertical distance to travel. Positive numbers will scroll the
         *        content up.
         * @param duration Duration of the scroll in milliseconds.
         */
        public void startScroll(int startX, int startY, int dx, int dy, int duration)
        {
            mMode = SCROLL_MODE;
            mFinished = false;
            mDuration = duration;
            mStartTime = AnimationUtils.currentAnimationTimeMillis(context);
            mStartX = startX;
            mStartY = startY;
            mFinalX = startX + dx;
            mFinalY = startY + dy;
            mDeltaX = dx;
            mDeltaY = dy;
            mDurationReciprocal = 1.0f / (float)mDuration;
        }

        /**
         * Start scrolling based on a fling gesture. The distance travelled will
         * depend on the initial velocity of the fling.
         * 
         * @param startX Starting point of the scroll (X)
         * @param startY Starting point of the scroll (Y)
         * @param velocityX Initial velocity of the fling (X) measured in pixels per
         *        second.
         * @param velocityY Initial velocity of the fling (Y) measured in pixels per
         *        second
         * @param minX Minimum X value. The scroller will not scroll past this
         *        point.
         * @param maxX Maximum X value. The scroller will not scroll past this
         *        point.
         * @param minY Minimum Y value. The scroller will not scroll past this
         *        point.
         * @param maxY Maximum Y value. The scroller will not scroll past this
         *        point.
         */
        public void fling(int startX, int startY, int velocityX, int velocityY,
                int minX, int maxX, int minY, int maxY)
        {
            // Continue a scroll or fling in progress
            if (mFlywheel && !mFinished)
            {
                float oldVel = getCurrVelocity();

                float dx = (float)(mFinalX - mStartX);
                float dy = (float)(mFinalY - mStartY);
                float hyp = (float)MathUtils.hypot(dx, dy);

                float ndx = dx / hyp;
                float ndy = dy / hyp;

                float oldVelocityX = ndx * oldVel;
                float oldVelocityY = ndy * oldVel;
                if (MathUtils.signum(velocityX) == MathUtils.signum(oldVelocityX) &&
                        MathUtils.signum(velocityY) == MathUtils.signum(oldVelocityY))
                {
                    velocityX = (int)(velocityX + oldVelocityX);
                    velocityY = (int)(velocityY + oldVelocityY);
                }
            }

            mMode = FLING_MODE;
            mFinished = false;

            float velocity = (float)MathUtils.hypot(velocityX, velocityY);

            mVelocity = velocity;
            mDuration = getSplineFlingDuration(velocity);
            mStartTime = AnimationUtils.currentAnimationTimeMillis(context);
            mStartX = startX;
            mStartY = startY;

            float coeffX = velocity == 0 ? 1.0f : velocityX / velocity;
            float coeffY = velocity == 0 ? 1.0f : velocityY / velocity;

            double totalDistance = getSplineFlingDistance(velocity);
            mDistance = (int)(totalDistance * MathUtils.signum(velocity));

            mMinX = minX;
            mMaxX = maxX;
            mMinY = minY;
            mMaxY = maxY;

            mFinalX = startX + (int)Math.Round(totalDistance * coeffX);
            // Pin to mMinX <= mFinalX <= mMaxX
            mFinalX = Math.Min(mFinalX, mMaxX);
            mFinalX = Math.Max(mFinalX, mMinX);

            mFinalY = startY + (int)Math.Round(totalDistance * coeffY);
            // Pin to mMinY <= mFinalY <= mMaxY
            mFinalY = Math.Min(mFinalY, mMaxY);
            mFinalY = Math.Max(mFinalY, mMinY);
        }

        internal double getSplineDeceleration(float velocity)
        {
            return Math.Log(INFLEXION * Math.Abs(velocity) / (mFlingFriction * mPhysicalCoeff));
        }

        internal int getSplineFlingDuration(float velocity)
        {
            double l = getSplineDeceleration(velocity);
            double decelMinusOne = DECELERATION_RATE - 1.0;
            return (int)(1000.0 * Math.Exp(l / decelMinusOne));
        }

        internal double getSplineFlingDistance(float velocity)
        {
            double l = getSplineDeceleration(velocity);
            double decelMinusOne = DECELERATION_RATE - 1.0;
            return mFlingFriction * mPhysicalCoeff * Math.Exp(DECELERATION_RATE / decelMinusOne * l);
        }

        /**
         * Stops the animation. Contrary to {@link #forceFinished(bool)},
         * aborting the animating cause the scroller to move to the x and y
         * position
         *
         * @see #forceFinished(bool)
         */
        public void abortAnimation()
        {
            mCurrX = mFinalX;
            mCurrY = mFinalY;
            mFinished = true;
        }

        /**
         * Extend the scroll animation. This allows a running animation to scroll
         * further and longer, when used with {@link #setFinalX(int)} or {@link #setFinalY(int)}.
         *
         * @param extend Additional time to scroll in milliseconds.
         * @see #setFinalX(int)
         * @see #setFinalY(int)
         */
        public void extendDuration(int extend)
        {
            int passed = timePassed();
            mDuration = passed + extend;
            mDurationReciprocal = 1.0f / mDuration;
            mFinished = false;
        }

        /**
         * Returns the time elapsed since the beginning of the scrolling.
         *
         * @return The elapsed time in milliseconds.
         */
        public int timePassed()
        {
            return (int)(AnimationUtils.currentAnimationTimeMillis(context) - mStartTime);
        }

        /**
         * Sets the position (X) for this scroller.
         *
         * @param newX The new X offset as an absolute distance from the origin.
         * @see #extendDuration(int)
         * @see #setFinalY(int)
         */
        public void setFinalX(int newX)
        {
            mFinalX = newX;
            mDeltaX = mFinalX - mStartX;
            mFinished = false;
        }

        /**
         * Sets the position (Y) for this scroller.
         *
         * @param newY The new Y offset as an absolute distance from the origin.
         * @see #extendDuration(int)
         * @see #setFinalX(int)
         */
        public void setFinalY(int newY)
        {
            mFinalY = newY;
            mDeltaY = mFinalY - mStartY;
            mFinished = false;
        }

        /**
         * @hide
         */
        public bool isScrollingInDirection(float xvel, float yvel)
        {
            return !mFinished && MathUtils.signum(xvel) == MathUtils.signum(mFinalX - mStartX) &&
                    MathUtils.signum(yvel) == MathUtils.signum(mFinalY - mStartY);
        }

        public class ViscousFluidInterpolator : Interpolator
        {
            /** Controls the viscous fluid effect (how much of it). */
            internal const float VISCOUS_FLUID_SCALE = 8.0f;

            internal static readonly float VISCOUS_FLUID_NORMALIZE;
            internal static readonly float VISCOUS_FLUID_OFFSET;

            static ViscousFluidInterpolator()
            {

                // must be set to 1.0 (used in viscousFluid())
                VISCOUS_FLUID_NORMALIZE = 1.0f / viscousFluid(1.0f);
                // account for very small floating-point error
                VISCOUS_FLUID_OFFSET = 1.0f - VISCOUS_FLUID_NORMALIZE * viscousFluid(1.0f);
            }

            internal static float viscousFluid(float x)
            {
                x *= VISCOUS_FLUID_SCALE;
                if (x < 1.0f)
                {
                    x -= (1.0f - (float)Math.Exp(-x));
                }
                else
                {
                    float start = 0.36787944117f;   // 1/e == exp(-1)
                    x = 1.0f - (float)Math.Exp(1.0f - x);
                    x = start + x * (1.0f - start);
                }
                return x;
            }

            public float getInterpolation(float input)
            {
                float interpolated = VISCOUS_FLUID_NORMALIZE * viscousFluid(input);
                if (interpolated > 0)
                {
                    return interpolated + VISCOUS_FLUID_OFFSET;
                }
                return interpolated;
            }
        }
    }
}