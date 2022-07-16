/*
 * Copyright (C) 2011 The Android Open Source Project
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
using AndroidUI.Utils;
using AndroidUI.Widgets;

namespace AndroidUI.AnimationFramework.Animator
{

    /**
     * This class enables automatic and optimized animation of select properties on View objects.
     * If only one or two properties on a View object are being animated, then using an
     * {@link android.animation.ObjectAnimator} is fine; the property setters called by ObjectAnimator
     * are well equipped to do the right thing to set the property and invalidate the view
     * appropriately. But if several properties are animated simultaneously, or if you just want a
     * more convenient syntax to animate a specific property, then ViewPropertyAnimator might be
     * more well-suited to the task.
     *
     * <p>This class may provide better performance for several simultaneous animations, because
     * it will optimize invalidate calls to take place only once for several properties instead of each
     * animated property independently causing its own invalidation. Also, the syntax of using this
     * class could be easier to use because the caller need only tell the View object which
     * property to animate, and the value to animate either to or by, and this class handles the
     * details of configuring the underlying Animator class and starting it.</p>
     *
     * <p>This class is not constructed by the caller, but rather by the View whose properties
     * it will animate. Calls to {@link android.view.View#animate()} will return a reference
     * to the appropriate ViewPropertyAnimator object for that View.</p>
     *
     */
    public class ViewPropertyAnimator
    {

        /**
         * The View whose properties are being animated by this class. This is set at
         * construction time.
         */
        View mView;

        /**
         * The duration of the underlying Animator object. By default, we don't set the duration
         * on the Animator and just use its default duration. If the duration is ever set on this
         * Animator, then we use the duration that it was set to.
         */
        private long mDuration;

        /**
         * A flag indicating whether the duration has been set on this object. If not, we don't set
         * the duration on the underlying Animator, but instead just use its default duration.
         */
        private bool mDurationSet = false;

        /**
         * The startDelay of the underlying Animator object. By default, we don't set the startDelay
         * on the Animator and just use its default startDelay. If the startDelay is ever set on this
         * Animator, then we use the startDelay that it was set to.
         */
        private long mStartDelay = 0;

        /**
         * A flag indicating whether the startDelay has been set on this object. If not, we don't set
         * the startDelay on the underlying Animator, but instead just use its default startDelay.
         */
        private bool mStartDelaySet = false;

        /**
         * The interpolator of the underlying Animator object. By default, we don't set the interpolator
         * on the Animator and just use its default interpolator. If the interpolator is ever set on
         * this Animator, then we use the interpolator that it was set to.
         */
        private TimeInterpolator mInterpolator;

        /**
         * A flag indicating whether the interpolator has been set on this object. If not, we don't set
         * the interpolator on the underlying Animator, but instead just use its default interpolator.
         */
        private bool mInterpolatorSet = false;

        /**
         * Listener for the lifecycle events of the underlying ValueAnimator object.
         */
        private Animator.AnimatorListener mListener = null;

        /**
         * Listener for the update events of the underlying ValueAnimator object.
         */
        private ValueAnimator.AnimatorUpdateListener mUpdateListener = null;

        /**
         * A lazily-created ValueAnimator used in order to get some default animator properties
         * (duration, start delay, interpolator, etc.).
         */
        private ValueAnimator mTempValueAnimator;

        /**
         * This listener is the mechanism by which the underlying Animator causes changes to the
         * properties currently being animated, as well as the cleanup after an animation is
         * complete.
         */
        private AnimatorEventListener mAnimatorEventListener;

        /**
         * This list holds the properties that have been asked to animate. We allow the caller to
         * request several animations prior to actually starting the underlying animator. This
         * enables us to run one single animator to handle several properties in parallel. Each
         * property is tossed onto the pending list until the animation actually starts (which is
         * done by posting it onto mView), at which time the pending list is cleared and the properties
         * on that list are added to the list of properties associated with that animator.
         */
        List<NameValuesHolder> mPendingAnimations = new();
        private Runnable mPendingSetupAction;
        private Runnable mPendingCleanupAction;
        private Runnable mPendingOnStartAction;
        private Runnable mPendingOnEndAction;

        /**
         * Constants used to associate a property being requested and the mechanism used to set
         * the property (this class calls directly into View to set the properties in question).
         */
        const int NONE = 0x0000;
        const int TRANSLATION_X = 0x0001;
        const int TRANSLATION_Y = 0x0002;
        const int TRANSLATION_Z = 0x0004;
        //const int SCALE_X = 0x0008;
        //const int SCALE_Y = 0x0010;
        //const int ROTATION = 0x0020;
        //const int ROTATION_X = 0x0040;
        //const int ROTATION_Y = 0x0080;
        const int X = 0x0100;
        const int Y = 0x0200;
        //const int Z = 0x0400;
        const int ALPHA = 0x0800;

        private const int TRANSFORM_MASK = 
            TRANSLATION_X | TRANSLATION_Y | TRANSLATION_Z |
            //SCALE_X | SCALE_Y | 
            //ROTATION | ROTATION_X | ROTATION_Y | 
            X | Y //| Z
        ;

        /**
         * The mechanism by which the user can request several properties that are then animated
         * together works by posting this Runnable to start the underlying Animator. Every time
         * a property animation is requested, we cancel any previous postings of the Runnable
         * and re-post it. This means that we will only ever run the Runnable (and thus start the
         * underlying animator) after the caller is done setting the properties that should be
         * animated together.
         */
        private Runnable mAnimationStarter;

        /**
         * This class holds information about the overall animation being run on the set of
         * properties. The mask describes which properties are being animated and the
         * values holder is the list of all property/value objects.
         */
        private class PropertyBundle
        {
            internal int mPropertyMask;
            internal List<NameValuesHolder> mNameValuesHolder;

            internal PropertyBundle(int propertyMask, List<NameValuesHolder> nameValuesHolder)
            {
                mPropertyMask = propertyMask;
                mNameValuesHolder = nameValuesHolder;
            }

            /**
             * Removes the given property from being animated as a part of this
             * PropertyBundle. If the property was a part of this bundle, it returns
             * true to indicate that it was, in fact, canceled. This is an indication
             * to the caller that a cancellation actually occurred.
             *
             * @param propertyConstant The property whose cancellation is requested.
             * @return true if the given property is a part of this bundle and if it
             * has therefore been canceled.
             */
            internal bool cancel(int propertyConstant)
            {
                if ((mPropertyMask & propertyConstant) != 0 && mNameValuesHolder != null)
                {
                    int count = mNameValuesHolder.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        NameValuesHolder nameValuesHolder = mNameValuesHolder.ElementAt(i);
                        if (nameValuesHolder.mNameConstant == propertyConstant)
                        {
                            mNameValuesHolder.RemoveAt(i);
                            mPropertyMask &= ~propertyConstant;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        /**
         * This list tracks the list of properties being animated by any particular animator.
         * In most situations, there would only ever be one animator running at a time. But it is
         * possible to request some properties to animate together, then while those properties
         * are animating, to request some other properties to animate together. The way that
         * works is by having this map associate the group of properties being animated with the
         * animator handling the animation. On every update event for an Animator, we ask the
         * map for the associated properties and set them accordingly.
         */
        private Dictionary<Animator, PropertyBundle> mAnimatorMap = new();
        private Dictionary<Animator, Runnable> mAnimatorSetupMap;
        private Dictionary<Animator, Runnable> mAnimatorCleanupMap;
        private Dictionary<Animator, Runnable> mAnimatorOnStartMap;
        private Dictionary<Animator, Runnable> mAnimatorOnEndMap;

        /**
         * This is the information we need to set each property during the animation.
         * mNameConstant is used to set the appropriate field in View, and the from/delta
         * values are used to calculate the animated value for a given animation fraction
         * during the animation.
         */
        internal class NameValuesHolder
        {
            internal int mNameConstant;
            internal float mFromValue;
            internal float mDeltaValue;
            internal NameValuesHolder(int nameConstant, float fromValue, float deltaValue)
            {
                mNameConstant = nameConstant;
                mFromValue = fromValue;
                mDeltaValue = deltaValue;
            }
        }

        /**
         * Constructor, called by View. This is private by design, as the user should only
         * get a ViewPropertyAnimator by calling View.animate().
         *
         * @param view The View associated with this ViewPropertyAnimator
         */
        internal ViewPropertyAnimator(View view)
        {
            mAnimatorEventListener = new AnimatorEventListener(this);
            mAnimationStarter = Runnable.Create(() => startAnimation());
            mView = view;
            view.ensureTransformationInfo();
        }

        /**
         * Sets the duration for the underlying animator that animates the requested properties.
         * By default, the animator uses the default value for ValueAnimator. Calling this method
         * will cause the declared value to be used instead.
         * @param duration The length of ensuing property animations, in milliseconds. The value
         * cannot be negative.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator setDuration(long duration)
        {
            if (duration < 0)
            {
                throw new IllegalArgumentException("Animators cannot have negative duration: " +
                        duration);
            }
            mDurationSet = true;
            mDuration = duration;
            return this;
        }

        /**
         * Returns the current duration of property animations. If the duration was set on this
         * object, that value is returned. Otherwise, the default value of the underlying Animator
         * is returned.
         *
         * @see #setDuration(long)
         * @return The duration of animations, in milliseconds.
         */
        public long getDuration()
        {
            if (mDurationSet)
            {
                return mDuration;
            }
            else
            {
                // Just return the default from ValueAnimator, since that's what we'd get if
                // the value has not been set otherwise
                if (mTempValueAnimator == null)
                {
                    mTempValueAnimator = new ValueAnimator(mView.Context);
                }
                return mTempValueAnimator.getDuration();
            }
        }

        /**
         * Returns the current startDelay of property animations. If the startDelay was set on this
         * object, that value is returned. Otherwise, the default value of the underlying Animator
         * is returned.
         *
         * @see #setStartDelay(long)
         * @return The startDelay of animations, in milliseconds.
         */
        public long getStartDelay()
        {
            if (mStartDelaySet)
            {
                return mStartDelay;
            }
            else
            {
                // Just return the default from ValueAnimator (0), since that's what we'd get if
                // the value has not been set otherwise
                return 0;
            }
        }

        /**
         * Sets the startDelay for the underlying animator that animates the requested properties.
         * By default, the animator uses the default value for ValueAnimator. Calling this method
         * will cause the declared value to be used instead.
         * @param startDelay The delay of ensuing property animations, in milliseconds. The value
         * cannot be negative.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator setStartDelay(long startDelay)
        {
            if (startDelay < 0)
            {
                throw new IllegalArgumentException("Animators cannot have negative start " +
                    "delay: " + startDelay);
            }
            mStartDelaySet = true;
            mStartDelay = startDelay;
            return this;
        }

        /**
         * Sets the interpolator for the underlying animator that animates the requested properties.
         * By default, the animator uses the default interpolator for ValueAnimator. Calling this method
         * will cause the declared object to be used instead.
         *
         * @param interpolator The TimeInterpolator to be used for ensuing property animations. A value
         * of <code>null</code> will result in linear interpolation.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator setInterpolator(TimeInterpolator interpolator)
        {
            mInterpolatorSet = true;
            mInterpolator = interpolator;
            return this;
        }

        /**
         * Returns the timing interpolator that this animation uses.
         *
         * @return The timing interpolator for this animation.
         */
        public TimeInterpolator getInterpolator()
        {
            if (mInterpolatorSet)
            {
                return mInterpolator;
            }
            else
            {
                // Just return the default from ValueAnimator, since that's what we'd get if
                // the value has not been set otherwise
                if (mTempValueAnimator == null)
                {
                    mTempValueAnimator = new ValueAnimator(mView.Context);
                }
                return mTempValueAnimator.getInterpolator();
            }
        }

        /**
         * Sets a listener for events in the underlying Animators that run the property
         * animations.
         *
         * @see Animator.AnimatorListener
         *
         * @param listener The listener to be called with AnimatorListener events. A value of
         * <code>null</code> removes any existing listener.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator setListener(Animator.AnimatorListener listener)
        {
            mListener = listener;
            return this;
        }

        Animator.AnimatorListener getListener()
        {
            return mListener;
        }

        /**
         * Sets a listener for update events in the underlying ValueAnimator that runs
         * the property animations. Note that the underlying animator is animating between
         * 0 and 1 (these values are then turned into the actual property values internally
         * by ViewPropertyAnimator). So the animator cannot give information on the current
         * values of the properties being animated by this ViewPropertyAnimator, although
         * the view object itself can be queried to get the current values.
         *
         * @see android.animation.ValueAnimator.AnimatorUpdateListener
         *
         * @param listener The listener to be called with update events. A value of
         * <code>null</code> removes any existing listener.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator setUpdateListener(ValueAnimator.AnimatorUpdateListener listener)
        {
            mUpdateListener = listener;
            return this;
        }

        ValueAnimator.AnimatorUpdateListener getUpdateListener()
        {
            return mUpdateListener;
        }

        /**
         * Starts the currently pending property animations immediately. Calling <code>start()</code>
         * is optional because all animations start automatically at the next opportunity. However,
         * if the animations are needed to start immediately and synchronously (not at the time when
         * the next event is processed by the hierarchy, which is when the animations would begin
         * otherwise), then this method can be used.
         */
        public void start()
        {
            mView.removeCallbacks(mAnimationStarter);
            startAnimation();
        }

        /**
         * Cancels all property animations that are currently running or pending.
         */
        public void cancel()
        {
            if (mAnimatorMap.Count > 0)
            {
                Dictionary<Animator, PropertyBundle> mAnimatorMapCopy = new(mAnimatorMap);
                var animatorSet = mAnimatorMapCopy.Keys;
                foreach (Animator runningAnim in animatorSet)
                {
                    runningAnim.cancel();
                }
            }
            mPendingAnimations.Clear();
            mPendingSetupAction = null;
            mPendingCleanupAction = null;
            mPendingOnStartAction = null;
            mPendingOnEndAction = null;
            mView.removeCallbacks(mAnimationStarter);
        }

        /**
         * This method will cause the View's <code>x</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setX(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator x(float value)
        {
            animateProperty(X, value);
            return this;
        }

        /**
         * This method will cause the View's <code>x</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setX(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator xBy(float value)
        {
            animatePropertyBy(X, value);
            return this;
        }

        /**
         * This method will cause the View's <code>y</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setY(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator y(float value)
        {
            animateProperty(Y, value);
            return this;
        }

        /**
         * This method will cause the View's <code>y</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setY(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator yBy(float value)
        {
            animatePropertyBy(Y, value);
            return this;
        }

        ///**
        // * This method will cause the View's <code>z</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setZ(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator z(float value)
        //{
        //    animateProperty(Z, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>z</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setZ(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator zBy(float value)
        //{
        //    animatePropertyBy(Z, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotation</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setRotation(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotation(float value)
        //{
        //    animateProperty(ROTATION, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotation</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setRotation(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotationBy(float value)
        //{
        //    animatePropertyBy(ROTATION, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotationX</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setRotationX(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotationX(float value)
        //{
        //    animateProperty(ROTATION_X, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotationX</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setRotationX(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotationXBy(float value)
        //{
        //    animatePropertyBy(ROTATION_X, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotationY</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setRotationY(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotationY(float value)
        //{
        //    animateProperty(ROTATION_Y, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>rotationY</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setRotationY(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator rotationYBy(float value)
        //{
        //    animatePropertyBy(ROTATION_Y, value);
        //    return this;
        //}

        /**
         * This method will cause the View's <code>translationX</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setTranslationX(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationX(float value)
        {
            animateProperty(TRANSLATION_X, value);
            return this;
        }

        /**
         * This method will cause the View's <code>translationX</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setTranslationX(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationXBy(float value)
        {
            animatePropertyBy(TRANSLATION_X, value);
            return this;
        }

        /**
         * This method will cause the View's <code>translationY</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setTranslationY(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationY(float value)
        {
            animateProperty(TRANSLATION_Y, value);
            return this;
        }

        /**
         * This method will cause the View's <code>translationY</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setTranslationY(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationYBy(float value)
        {
            animatePropertyBy(TRANSLATION_Y, value);
            return this;
        }

        /**
         * This method will cause the View's <code>translationZ</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setTranslationZ(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationZ(float value)
        {
            animateProperty(TRANSLATION_Z, value);
            return this;
        }

        /**
         * This method will cause the View's <code>translationZ</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setTranslationZ(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator translationZBy(float value)
        {
            animatePropertyBy(TRANSLATION_Z, value);
            return this;
        }
        ///**
        // * This method will cause the View's <code>scaleX</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setScaleX(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator scaleX(float value)
        //{
        //    animateProperty(SCALE_X, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>scaleX</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setScaleX(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator scaleXBy(float value)
        //{
        //    animatePropertyBy(SCALE_X, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>scaleY</code> property to be animated to the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The value to be animated to.
        // * @see View#setScaleY(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator scaleY(float value)
        //{
        //    animateProperty(SCALE_Y, value);
        //    return this;
        //}

        ///**
        // * This method will cause the View's <code>scaleY</code> property to be animated by the
        // * specified value. Animations already running on the property will be canceled.
        // *
        // * @param value The amount to be animated by, as an offset from the current value.
        // * @see View#setScaleY(float)
        // * @return This object, allowing calls to methods in this class to be chained.
        // */
        //public ViewPropertyAnimator scaleYBy(float value)
        //{
        //    animatePropertyBy(SCALE_Y, value);
        //    return this;
        //}

        /**
         * This method will cause the View's <code>alpha</code> property to be animated to the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The value to be animated to.
         * @see View#setAlpha(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator alpha(float value)
        {
            animateProperty(ALPHA, Math.Clamp(value, 0.0f, 1.0f));
            return this;
        }

        /**
         * This method will cause the View's <code>alpha</code> property to be animated by the
         * specified value. Animations already running on the property will be canceled.
         *
         * @param value The amount to be animated by, as an offset from the current value.
         * @see View#setAlpha(float)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator alphaBy(float value)
        {
            animatePropertyBy(ALPHA, Math.Clamp(value, 0.0f, 1.0f));
            return this;
        }

        /**
         * The View associated with this ViewPropertyAnimator will have its
         * {@link View#setLayerType(int, android.graphics.Paint) layer type} set to
         * {@link View#LAYER_TYPE_HARDWARE} for the duration of the next animation.
         * As stated in the documentation for {@link View#LAYER_TYPE_HARDWARE},
         * the actual type of layer used internally depends on the runtime situation of the
         * view. If the activity and this view are hardware-accelerated, then the layer will be
         * accelerated as well. If the activity or the view is not accelerated, then the layer will
         * effectively be the same as {@link View#LAYER_TYPE_SOFTWARE}.
         *
         * <p>This state is not persistent, either on the View or on this ViewPropertyAnimator: the
         * layer type of the View will be restored when the animation ends to what it was when this
         * method was called, and this setting on ViewPropertyAnimator is only valid for the next
         * animation. Note that calling this method and then independently setting the layer type of
         * the View (by a direct call to {@link View#setLayerType(int, android.graphics.Paint)}) will
         * result in some inconsistency, including having the layer type restored to its pre-withLayer()
         * value when the animation ends.</p>
         *
         * @see View#setLayerType(int, android.graphics.Paint)
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator withLayer()
        {
            mPendingSetupAction = Runnable.Create(() =>
            {
                //mView.setLayerType(View.LAYER_TYPE_HARDWARE, null);
                //if (mView.isAttachedToWindow())
                //{
                //    mView.buildLayer();
                //}
            });
            //int currentLayerType = mView.getLayerType();
            mPendingCleanupAction = Runnable.Create(() =>
                {
                    //mView.setLayerType(currentLayerType, null);
                }
            );
            if (mAnimatorSetupMap == null)
            {
                mAnimatorSetupMap = new Dictionary<Animator, Runnable>();
            }
            if (mAnimatorCleanupMap == null)
            {
                mAnimatorCleanupMap = new Dictionary<Animator, Runnable>();
            }

            return this;
        }

        /**
         * Specifies an action to take place when the next animation runs. If there is a
         * {@link #setStartDelay(long) startDelay} set on this ViewPropertyAnimator, then the
         * action will run after that startDelay expires, when the actual animation begins.
         * This method, along with {@link #withEndAction(Runnable)}, is intended to help facilitate
         * choreographing ViewPropertyAnimator animations with other animations or actions
         * in the application.
         *
         * @param runnable The action to run when the next animation starts.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator withStartAction(Runnable runnable)
        {
            mPendingOnStartAction = runnable;
            if (runnable != null && mAnimatorOnStartMap == null)
            {
                mAnimatorOnStartMap = new Dictionary<Animator, Runnable>();
            }
            return this;
        }

        /**
         * Specifies an action to take place when the next animation ends. The action is only
         * run if the animation ends normally; if the ViewPropertyAnimator is canceled during
         * that animation, the runnable will not run.
         * This method, along with {@link #withStartAction(Runnable)}, is intended to help facilitate
         * choreographing ViewPropertyAnimator animations with other animations or actions
         * in the application.
         *
         * <p>For example, the following code animates a view to x=200 and then back to 0:</p>
         * <pre>
         *     Runnable endAction = new Runnable() {
         *         public void run() {
         *             view.animate().x(0);
         *         }
         *     };
         *     view.animate().x(200).withEndAction(endAction);
         * </pre>
         *
         * @param runnable The action to run when the next animation ends.
         * @return This object, allowing calls to methods in this class to be chained.
         */
        public ViewPropertyAnimator withEndAction(Runnable runnable)
        {
            mPendingOnEndAction = runnable;
            if (runnable != null && mAnimatorOnEndMap == null)
            {
                mAnimatorOnEndMap = new Dictionary<Animator, Runnable>();
            }
            return this;
        }

        bool hasActions()
        {
            return mPendingSetupAction != null
                    || mPendingCleanupAction != null
                    || mPendingOnStartAction != null
                    || mPendingOnEndAction != null;
        }

        /**
         * Starts the underlying Animator for a set of properties. We use a single animator that
         * simply runs from 0 to 1, and then use that fractional value to set each property
         * value accordingly.
         */
        private void startAnimation()
        {
            mView.setHasTransientState(true);
            ValueAnimator animator = ValueAnimator.ofFloat(mView.Context, 1.0f);
            List<NameValuesHolder> nameValueList = new(mPendingAnimations);
            mPendingAnimations.Clear();
            int propertyMask = 0;
            int propertyCount = nameValueList.Count;
            for (int i = 0; i < propertyCount; ++i)
            {
                NameValuesHolder nameValuesHolder = nameValueList.ElementAt(i);
                propertyMask |= nameValuesHolder.mNameConstant;
            }
            mAnimatorMap[animator] = new PropertyBundle(propertyMask, nameValueList);
            if (mPendingSetupAction != null)
            {
                mAnimatorSetupMap[animator] = mPendingSetupAction;
                mPendingSetupAction = null;
            }
            if (mPendingCleanupAction != null)
            {
                mAnimatorCleanupMap[animator] = mPendingCleanupAction;
                mPendingCleanupAction = null;
            }
            if (mPendingOnStartAction != null)
            {
                mAnimatorOnStartMap[animator] = mPendingOnStartAction;
                mPendingOnStartAction = null;
            }
            if (mPendingOnEndAction != null)
            {
                mAnimatorOnEndMap[animator] = mPendingOnEndAction;
                mPendingOnEndAction = null;
            }
            animator.addUpdateListener(mAnimatorEventListener);
            animator.addListener(mAnimatorEventListener);
            if (mStartDelaySet)
            {
                animator.setStartDelay(mStartDelay);
            }
            if (mDurationSet)
            {
                animator.setDuration(mDuration);
            }
            if (mInterpolatorSet)
            {
                animator.setInterpolator(mInterpolator);
            }
            animator.start();
        }

        /**
         * Utility function, called by the various x(), y(), etc. methods. This stores the
         * constant name for the property along with the from/delta values that will be used to
         * calculate and set the property during the animation. This structure is added to the
         * pending animations, awaiting the eventual start() of the underlying animator. A
         * Runnable is posted to start the animation, and any pending such Runnable is canceled
         * (which enables us to end up starting just one animator for all of the properties
         * specified at one time).
         *
         * @param constantName The specifier for the property being animated
         * @param toValue The value to which the property will animate
         */
        private void animateProperty(int constantName, float toValue)
        {
            float fromValue = getValue(constantName);
            float deltaValue = toValue - fromValue;
            animatePropertyBy(constantName, fromValue, deltaValue);
        }

        /**
         * Utility function, called by the various xBy(), yBy(), etc. methods. This method is
         * just like animateProperty(), except the value is an offset from the property's
         * current value, instead of an absolute "to" value.
         *
         * @param constantName The specifier for the property being animated
         * @param byValue The amount by which the property will change
         */
        private void animatePropertyBy(int constantName, float byValue)
        {
            float fromValue = getValue(constantName);
            animatePropertyBy(constantName, fromValue, byValue);
        }

        /**
         * Utility function, called by animateProperty() and animatePropertyBy(), which handles the
         * details of adding a pending animation and posting the request to start the animation.
         *
         * @param constantName The specifier for the property being animated
         * @param startValue The starting value of the property
         * @param byValue The amount by which the property will change
         */
        private void animatePropertyBy(int constantName, float startValue, float byValue)
        {
            // First, cancel any existing animations on this property
            if (mAnimatorMap.Count > 0)
            {
                Animator animatorToCancel = null;
                var animatorSet = mAnimatorMap.Keys;
                foreach (Animator runningAnim in animatorSet)
                {
                    PropertyBundle bundle = mAnimatorMap[runningAnim];
                    if (bundle.cancel(constantName))
                    {
                        // property was canceled - cancel the animation if it's now empty
                        // Note that it's safe to break out here because every new animation
                        // on a property will cancel a previous animation on that property, so
                        // there can only ever be one such animation running.
                        if (bundle.mPropertyMask == NONE)
                        {
                            // the animation is no longer changing anything - cancel it
                            animatorToCancel = runningAnim;
                            break;
                        }
                    }
                }
                if (animatorToCancel != null)
                {
                    animatorToCancel.cancel();
                }
            }

            NameValuesHolder nameValuePair = new(constantName, startValue, byValue);
            mPendingAnimations.Add(nameValuePair);
            mView.removeCallbacks(mAnimationStarter);
            mView.postOnAnimation(mAnimationStarter);
        }

        /**
         * This method handles setting the property values directly in the View object's fields.
         * propertyConstant tells it which property should be set, value is the value to set
         * the property to.
         *
         * @param propertyConstant The property to be set
         * @param value The value to set the property to
         */
        private void setValue(int propertyConstant, float value)
        {
            //final RenderNode renderNode = mView.mRenderNode;
            switch (propertyConstant)
            {
                case TRANSLATION_X:
                    mView.setTranslationX(value);
                    break;
                case TRANSLATION_Y:
                    mView.setTranslationY(value);
                    break;
                case TRANSLATION_Z:
                    mView.setTranslationZ(value);
                    break;
                //case ROTATION:
                //    mView.setRotationZ(value);
                //    break;
                //case ROTATION_X:
                //    mView.setRotationX(value);
                //    break;
                //case ROTATION_Y:
                //    mView.setRotationY(value);
                //    break;
                //case SCALE_X:
                //    mView.setScaleX(value);
                //    break;
                //case SCALE_Y:
                //    mView.setScaleY(value);
                //    break;
                case X:
                    mView.setTranslationX(value - mView.mLeft);
                    break;
                case Y:
                    mView.setTranslationY(value - mView.mTop);
                    break;
                    //case Z:
                    //            mView.setTranslationZ(value - renderNode.getElevation());
                    break;
                case ALPHA:
                    mView.setAlphaInternal(value);
                    //renderNode.setAlpha(value);
                    break;
            }
        }

        /**
         * This method gets the value of the named property from the View object.
         *
         * @param propertyConstant The property whose value should be returned
         * @return float The value of the named property
         */
        private float getValue(int propertyConstant)
        {
            //final RenderNode node = mView.mRenderNode;
            switch (propertyConstant)
            {
                case TRANSLATION_X:
                    return mView.getTranslationX();
                case TRANSLATION_Y:
                    return mView.getTranslationY();
                case TRANSLATION_Z:
                    return mView.getTranslationZ();
                //case ROTATION:
                //    return mView.getRotationZ();
                //case ROTATION_X:
                //    return mView.getRotationX();
                //case ROTATION_Y:
                //    return mView.getRotationY();
                //case SCALE_X:
                //    return mView.getScaleX();
                //case SCALE_Y:
                //    return mView.getScaleY();
                case X:
                    return mView.mLeft + mView.getTranslationX();
                case Y:
                    return mView.mTop + mView.getTranslationY();
                //case Z:
                //    return node.getElevation() + node.getTranslationZ();
                case ALPHA:
                    return mView.getAlpha();
            }
            return 0;
        }

        /**
         * Utility class that handles the various Animator events. The only ones we care
         * about are the end event (which we use to clean up the animator map when an animator
         * finishes) and the update event (which we use to calculate the current value of each
         * property and then set it on the view object).
         */
        private class AnimatorEventListener
                    : Animator.AnimatorListener, ValueAnimator.AnimatorUpdateListener
        {
            ViewPropertyAnimator self;

            public AnimatorEventListener(ViewPropertyAnimator self)
            {
                this.self = self;
            }

            public void onAnimationStart(Animator animation)
            {
                if (self.mAnimatorSetupMap != null)
                {
                    Runnable r = self.mAnimatorSetupMap.GetValueOrDefault(animation, null); ;
                    if (r != null)
                    {
                        r.run();
                    }
                    self.mAnimatorSetupMap.Remove(animation);
                }
                if (self.mAnimatorOnStartMap != null)
                {
                    Runnable r = self.mAnimatorOnStartMap.GetValueOrDefault(animation, null); ;
                    if (r != null)
                    {
                        r.run();
                    }
                    self.mAnimatorOnStartMap.Remove(animation);
                }
                if (self.mListener != null)
                {
                    self.mListener.onAnimationStart(animation);
                }
            }

            public void onAnimationCancel(Animator animation)
            {
                if (self.mListener != null)
                {
                    self.mListener.onAnimationCancel(animation);
                }
                if (self.mAnimatorOnEndMap != null)
                {
                    self.mAnimatorOnEndMap.Remove(animation);
                }
            }

            public void onAnimationRepeat(Animator animation)
            {
                if (self.mListener != null)
                {
                    self.mListener.onAnimationRepeat(animation);
                }
            }

            public void onAnimationEnd(Animator animation)
            {
                self.mView.setHasTransientState(false);
                if (self.mAnimatorCleanupMap != null)
                {
                    Runnable r = self.mAnimatorCleanupMap.GetValueOrDefault(animation, null);
                    if (r != null)
                    {
                        r.run();
                    }
                    self.mAnimatorCleanupMap.Remove(animation);
                }
                if (self.mListener != null)
                {
                    self.mListener.onAnimationEnd(animation);
                }
                if (self.mAnimatorOnEndMap != null)
                {
                    Runnable r = self.mAnimatorOnEndMap.GetValueOrDefault(animation, null);
                    if (r != null)
                    {
                        r.run();
                    }
                    self.mAnimatorOnEndMap.Remove(animation);
                }
                self.mAnimatorMap.Remove(animation);
            }

            /**
             * Calculate the current value for each property and set it on the view. Invalidate
             * the view object appropriately, depending on which properties are being animated.
             *
             * @param animation The animator associated with the properties that need to be
             * set. This animator holds the animation fraction which we will use to calculate
             * the current value of each property.
             */
            public void onAnimationUpdate(ValueAnimator animation)
            {
                PropertyBundle propertyBundle = self.mAnimatorMap.GetValueOrDefault(animation, null);
                if (propertyBundle == null)
                {
                    // Shouldn't happen, but just to play it safe
                    return;
                }

                bool hardwareAccelerated = self.mView.isHardwareAccelerated();

                // alpha requires slightly different treatment than the other (transform) properties.
                // The logic in setAlpha() is not simply setting mAlpha, plus the invalidation
                // logic is dependent on how the view handles an internal call to onSetAlpha().
                // We track what kinds of properties are set, and how alpha is handled when it is
                // set, and perform the invalidation steps appropriately.
                bool alphaHandled = false;
                if (!hardwareAccelerated)
                {
                    self.mView.invalidateParentCaches();
                }
                float fraction = animation.getAnimatedFraction();
                int propertyMask = propertyBundle.mPropertyMask;
                if ((propertyMask & TRANSFORM_MASK) != 0)
                {
                    self.mView.invalidateViewProperty(hardwareAccelerated, false);
                }
                List<NameValuesHolder> valueList = propertyBundle.mNameValuesHolder;
                if (valueList != null)
                {
                    int count = valueList.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        NameValuesHolder values = valueList.ElementAt(i);
                        float value = values.mFromValue + fraction * values.mDeltaValue;
                        if (values.mNameConstant == ALPHA)
                        {
                            alphaHandled = self.mView.setAlphaNoInvalidation(value);
                        }
                        else
                        {
                            self.setValue(values.mNameConstant, value);
                        }
                    }
                }
                if ((propertyMask & TRANSFORM_MASK) != 0)
                {
                    if (!hardwareAccelerated)
                    {
                        self.mView.mPrivateFlags |= View.PFLAG_DRAWN; // force another invalidation
                    }
                }
                // invalidate(false) in all cases except if alphaHandled gets set to true
                // via the call to setAlphaNoInvalidation(), above
                if (alphaHandled)
                {
                    self.mView.invalidate(true);
                }
                else
                {
                    self.mView.invalidateViewProperty(false, false);
                }
                if (self.mUpdateListener != null)
                {
                    self.mUpdateListener.onAnimationUpdate(animation);
                }
            }
        }
    }
}
