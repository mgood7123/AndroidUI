/*
 * Copyright (C) 2007 The Android Open Source Project
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
using AndroidUI.Applications;
using AndroidUI.Widgets;

namespace AndroidUI.AnimationFramework.Animation.Controller
{

    /**
     * A layout animation controller is used to animate the children of a layout or a view
     * group. Each child uses the same animation but for every one of
     * them, the animation starts at a different time. A layout animation controller
     * is used by {@link android.view.ViewGroup} to compute the delay by which each
     * child's animation start must be offset. The delay is computed by using
     * characteristics of each child, like its index in the view group.
     *
     * This standard implementation computes the delay by multiplying a fixed
     * amount of miliseconds by the index of the child in its parent view group.
     * Subclasses are supposed to override
     * {@link #getDelayForView(android.view.View)} to implement a different way
     * of computing the delay. For instance, a
     * {@link android.view.animation.GridLayoutAnimationController} will compute the
     * delay based on the column and row indices of the child in its parent view
     * group.
     *
     * Information used to compute the animation delay of each child are stored
     * in an instance of
     * {@link android.view.animation.LayoutAnimationController.AnimationParameters},
     * itself stored in the {@link android.view.ViewGroup.LayoutParams} of the view.
     *
     * @attr ref android.R.styleable#LayoutAnimation_delay
     * @attr ref android.R.styleable#LayoutAnimation_animationOrder
     * @attr ref android.R.styleable#LayoutAnimation_interpolator
     * @attr ref android.R.styleable#LayoutAnimation_animation
     */
    public class LayoutAnimationController
    {
        Context context;
        /**
         * Distributes the animation delays in the order in which view were added
         * to their view group.
         */
        public const int ORDER_NORMAL = 0;

        /**
         * Distributes the animation delays in the reverse order in which view were
         * added to their view group.
         */
        public const int ORDER_REVERSE = 1;

        /**
         * Randomly distributes the animation delays.
         */
        public const int ORDER_RANDOM = 2;

        /**
         * The animation applied on each child of the view group on which this
         * layout animation controller is set.
         */
        protected Animation mAnimation;

        /**
         * The randomizer used when the order is set to random. Subclasses should
         * use this object to avoid creating their own.
         */
        protected Random mRandomizer;

        /**
         * The interpolator used to interpolate the delays.
         */
        protected Interpolator mInterpolator;

        private float mDelay;
        private int mOrder;

        private long mDuration;
        private long mMaxDelay;

        /**
         * Creates a new layout animation controller with a delay of 50%
         * and the specified animation.
         *
         * @param animation the animation to use on each child of the view group
         */
        public LayoutAnimationController(Context context, Animation animation) : this(context, animation, 0.5f)
        {
        }

        /**
         * Creates a new layout animation controller with the specified delay
         * and the specified animation.
         *
         * @param animation the animation to use on each child of the view group
         * @param delay the delay by which each child's animation must be offset
         */
        public LayoutAnimationController(Context context, Animation animation, float delay)
        {
            this.context = context;
            mDelay = delay;
            setAnimation(animation);
        }

        /**
         * Returns the order used to compute the delay of each child's animation.
         *
         * @return one of {@link #ORDER_NORMAL}, {@link #ORDER_REVERSE} or
         *         {@link #ORDER_RANDOM}
         *
         * @attr ref android.R.styleable#LayoutAnimation_animationOrder
         */
        public int getOrder()
        {
            return mOrder;
        }

        /**
         * Sets the order used to compute the delay of each child's animation.
         *
         * @param order one of {@link #ORDER_NORMAL}, {@link #ORDER_REVERSE} or
         *        {@link #ORDER_RANDOM}
         *
         * @attr ref android.R.styleable#LayoutAnimation_animationOrder
         */
        public void setOrder(int order)
        {
            mOrder = order;
        }


        /**
         * Sets the animation to be run on each child of the view group on which
         * this layout animation controller is .
         *
         * @param animation the animation to run on each child of the view group

         * @see #setAnimation(android.content.Context, int)
         * @see #getAnimation()
         *
         * @attr ref android.R.styleable#LayoutAnimation_animation
         */
        public void setAnimation(Animation animation)
        {
            mAnimation = animation;
            mAnimation.setFillBefore(true);
        }

        /**
         * Returns the animation applied to each child of the view group on which
         * this controller is set.
         *
         * @return an {@link android.view.animation.Animation} instance
         *
         * @see #setAnimation(android.content.Context, int)
         * @see #setAnimation(Animation)
         */
        public Animation getAnimation()
        {
            return mAnimation;
        }

        /**
         * Sets the interpolator used to interpolate the delays between the
         * children.
         *
         * @param interpolator the interpolator
         *
         * @see #getInterpolator()
         * @see #setInterpolator(Interpolator)
         *
         * @attr ref android.R.styleable#LayoutAnimation_interpolator
         */
        public void setInterpolator(Interpolator interpolator)
        {
            mInterpolator = interpolator;
        }

        /**
         * Returns the interpolator used to interpolate the delays between the
         * children.
         *
         * @return an {@link android.view.animation.Interpolator}
         */
        public Interpolator getInterpolator()
        {
            return mInterpolator;
        }

        /**
         * Returns the delay by which the children's animation are offset. The
         * delay is expressed as a fraction of the animation duration.
         *
         * @return a fraction of the animation duration
         *
         * @see #setDelay(float)
         */
        public float getDelay()
        {
            return mDelay;
        }

        /**
         * Sets the delay, as a fraction of the animation duration, by which the
         * children's animations are offset. The general formula is:
         *
         * <pre>
         * child animation delay = child index * delay * animation duration
         * </pre>
         *
         * @param delay a fraction of the animation duration
         *
         * @see #getDelay()
         */
        public void setDelay(float delay)
        {
            mDelay = delay;
        }

        /**
         * Indicates whether two children's animations will overlap. Animations
         * overlap when the delay is lower than 100% (or 1.0).
         *
         * @return true if animations will overlap, false otherwise
         */
        virtual public bool willOverlap()
        {
            return mDelay < 1.0f;
        }

        /**
         * Starts the animation.
         */
        public void start()
        {
            mDuration = mAnimation.getDuration();
            mMaxDelay = long.MinValue;
            mAnimation.setStartTime(-1);
        }

        /**
         * Returns the animation to be applied to the specified view. The returned
         * animation is delayed by an offset computed according to the information
         * provided by
         * {@link android.view.animation.LayoutAnimationController.AnimationParameters}.
         * This method is called by view groups to obtain the animation to set on
         * a specific child.
         *
         * @param view the view to animate
         * @return an animation delayed by the number of milliseconds returned by
         *         {@link #getDelayForView(android.view.View)}
         *
         * @see #getDelay()
         * @see #setDelay(float)
         * @see #getDelayForView(android.view.View)
         */
        public Animation getAnimationForView(View view)
        {
            long delay = getDelayForView(view) + mAnimation.getStartOffset();
            mMaxDelay = Math.Max(mMaxDelay, delay);

            Animation animation = mAnimation.Clone();
            animation.setStartOffset(delay);
            return animation;
        }

        /**
         * Indicates whether the layout animation is over or not. A layout animation
         * is considered done when the animation with the longest delay is done.
         *
         * @return true if all of the children's animations are over, false otherwise
         */
        public bool isDone()
        {
            return AnimationUtils.currentAnimationTimeMillis(context) >
                    mAnimation.getStartTime() + mMaxDelay + mDuration;
        }

        /**
         * Returns the amount of milliseconds by which the specified view's
         * animation must be delayed or offset. Subclasses should override this
         * method to return a suitable value.
         *
         * This implementation returns <code>child animation delay</code>
         * milliseconds where:
         *
         * <pre>
         * child animation delay = child index * delay
         * </pre>
         *
         * The index is retrieved from the
         * {@link android.view.animation.LayoutAnimationController.AnimationParameters}
         * found in the view's {@link android.view.ViewGroup.LayoutParams}.
         *
         * @param view the view for which to obtain the animation's delay
         * @return a delay in milliseconds
         *
         * @see #getAnimationForView(android.view.View)
         * @see #getDelay()
         * @see #getTransformedIndex(android.view.animation.LayoutAnimationController.AnimationParameters)
         * @see android.view.ViewGroup.LayoutParams
         */
        virtual protected long getDelayForView(View view)
        {
            View.LayoutParams lp = view.getLayoutParams();
            AnimationParameters params_ = lp.layoutAnimationParameters;

            if (params_ == null)
            {
                return 0;
            }

            float delay = mDelay * mAnimation.getDuration();
            long viewDelay = (long)(getTransformedIndex(params_) * delay);
            float totalDelay = delay * params_.count;

            if (mInterpolator == null)
            {
                mInterpolator = new LinearInterpolator();
            }

            float normalizedDelay = viewDelay / totalDelay;
            normalizedDelay = mInterpolator.getInterpolation(normalizedDelay);

            return (long)(normalizedDelay * totalDelay);
        }

        /**
         * Transforms the index stored in
         * {@link android.view.animation.LayoutAnimationController.AnimationParameters}
         * by the order returned by {@link #getOrder()}. Subclasses should override
         * this method to provide additional support for other types of ordering.
         * This method should be invoked by
         * {@link #getDelayForView(android.view.View)} prior to any computation. 
         *
         * @param params the animation parameters containing the index
         * @return a transformed index
         */
        protected int getTransformedIndex(AnimationParameters params_)
        {
            switch (getOrder())
            {
                case ORDER_REVERSE:
                    return params_.count - 1 - params_.index;
                case ORDER_RANDOM:
                    if (mRandomizer == null)
                    {
                        mRandomizer = new Random();
                    }
                    return (int)(params_.count * mRandomizer.NextSingle());
                case ORDER_NORMAL:
                default:
                    return params_.index;
            }
        }

        /**
         * The set of parameters that has to be attached to each view contained in
         * the view group animated by the layout animation controller. These
         * parameters are used to compute the start time of each individual view's
         * animation.
         */
        public class AnimationParameters
        {
            /**
             * The number of children in the view group containing the view to which
             * these parameters are attached.
             */
            public int count;

            /**
             * The index of the view to which these parameters are attached in its
             * containing view group.
             */
            public int index;
        }
    }
}