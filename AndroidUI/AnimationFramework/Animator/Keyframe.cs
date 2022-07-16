/*
 * Copyright (C) 2013 The Android Open Source Project
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
using AndroidUI.Utils;

namespace AndroidUI.AnimationFramework.Animator
{
    /**
     * This class holds a time/value pair for an animation. The Keyframe class is used
     * by {@link ValueAnimator} to define the values that the animation target will have over the course
     * of the animation. As the time proceeds from one keyframe to the other, the value of the
     * target object will animate between the value at the previous keyframe and the value at the
     * next keyframe. Each keyframe also holds an optional {@link TimeInterpolator}
     * object, which defines the time interpolation over the intervalue preceding the keyframe.
     *
     * <p>The Keyframe class itself is abstract. The type-specific factory methods will return
     * a subclass of Keyframe specific to the type of value being stored. This is done to improve
     * performance when dealing with the most common cases (e.g., <code>float</code> and
     * <code>int</code> values). Other types will fall into a more general Keyframe class that
     * treats its values as Objects. Unless your animation requires dealing with a custom type
     * or a data structure that needs to be animated directly (and evaluated using an implementation
     * of {@link TypeEvaluator}), you should stick to using float and int as animations using those
     * types have lower runtime overhead than other types.</p>
     */
    public abstract class Keyframe : Utils.ICloneable
    {
        /**
         * Flag to indicate whether this keyframe has a valid value. This flag is used when an
         * animation first starts, to populate placeholder keyframes with real values derived
         * from the target object.
         */
        bool mHasValue;

        /**
         * Flag to indicate whether the value in the keyframe was read from the target object or not.
         * If so, its value will be recalculated if target changes.
         */
        bool mValueWasSetOnStart;


        /**
         * The time at which mValue will hold true.
         */
        float mFraction;

        /**
         * The type of the value in this Keyframe. This type is determined at construction time,
         * based on the type of the <code>value</code> object passed into the constructor.
         */
        Type mValueType;

        /**
         * The optional time interpolator for the interval preceding this keyframe. A null interpolator
         * (the default) results in linear interpolation over the interval.
         */
        private TimeInterpolator mInterpolator = null;



        /**
         * Constructs a Keyframe object with the given time and value. The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         * @param value The value that the object will animate to as the animation time approaches
         * the time in this keyframe, and the value animated from as the time passes the time in
         * this keyframe.
         */
        public static Keyframe ofInt(float fraction, int value)
        {
            return new IntKeyframe(fraction, value);
        }

        /**
         * Constructs a Keyframe object with the given time. The value at this time will be derived
         * from the target object when the animation first starts (note that this implies that keyframes
         * with no initial value must be used as part of an {@link ObjectAnimator}).
         * The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         */
        public static Keyframe ofInt(float fraction)
        {
            return new IntKeyframe(fraction);
        }

        /**
         * Constructs a Keyframe object with the given time and value. The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         * @param value The value that the object will animate to as the animation time approaches
         * the time in this keyframe, and the value animated from as the time passes the time in
         * this keyframe.
         */
        public static Keyframe ofFloat(float fraction, float value)
        {
            return new FloatKeyframe(fraction, value);
        }

        /**
         * Constructs a Keyframe object with the given time. The value at this time will be derived
         * from the target object when the animation first starts (note that this implies that keyframes
         * with no initial value must be used as part of an {@link ObjectAnimator}).
         * The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         */
        public static Keyframe ofFloat(float fraction)
        {
            return new FloatKeyframe(fraction);
        }

        /**
         * Constructs a Keyframe object with the given time and value. The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         * @param value The value that the object will animate to as the animation time approaches
         * the time in this keyframe, and the value animated from as the time passes the time in
         * this keyframe.
         */
        public static Keyframe ofObject(float fraction, object value)
        {
            return new ObjectKeyframe(fraction, value);
        }

        /**
         * Constructs a Keyframe object with the given time. The value at this time will be derived
         * from the target object when the animation first starts (note that this implies that keyframes
         * with no initial value must be used as part of an {@link ObjectAnimator}).
         * The time defines the
         * time, as a proportion of an overall animation's duration, at which the value will hold true
         * for the animation. The value for the animation between keyframes will be calculated as
         * an interpolation between the values at those keyframes.
         *
         * @param fraction The time, expressed as a value between 0 and 1, representing the fraction
         * of time elapsed of the overall animation duration.
         */
        public static Keyframe ofObject(float fraction)
        {
            return new ObjectKeyframe(fraction, null);
        }

        /**
         * Indicates whether this keyframe has a valid value. This method is called internally when
         * an {@link ObjectAnimator} first starts; keyframes without values are assigned values at
         * that time by deriving the value for the property from the target object.
         *
         * @return bool Whether this object has a value assigned.
         */
        public bool hasValue()
        {
            return mHasValue;
        }

        /**
         * If the Keyframe's value was acquired from the target object, this flag should be set so that,
         * if target changes, value will be reset.
         *
         * @return bool Whether this Keyframe's value was retieved from the target object or not.
         */
        internal bool valueWasSetOnStart()
        {
            return mValueWasSetOnStart;
        }

        internal void setValueWasSetOnStart(bool valueWasSetOnStart)
        {
            mValueWasSetOnStart = valueWasSetOnStart;
        }

        /**
         * Gets the value for this Keyframe.
         *
         * @return The value for this Keyframe.
         */
        public abstract object getValue();

        /**
         * Sets the value for this Keyframe.
         *
         * @param value value for this Keyframe.
         */
        public abstract void setValue(object value);

        /**
         * Gets the time for this keyframe, as a fraction of the overall animation duration.
         *
         * @return The time associated with this keyframe, as a fraction of the overall animation
         * duration. This should be a value between 0 and 1.
         */
        public float getFraction()
        {
            return mFraction;
        }

        /**
         * Sets the time for this keyframe, as a fraction of the overall animation duration.
         *
         * @param fraction time associated with this keyframe, as a fraction of the overall animation
         * duration. This should be a value between 0 and 1.
         */
        public void setFraction(float fraction)
        {
            mFraction = fraction;
        }

        /**
         * Gets the optional interpolator for this Keyframe. A value of <code>null</code> indicates
         * that there is no interpolation, which is the same as linear interpolation.
         *
         * @return The optional interpolator for this Keyframe.
         */
        public TimeInterpolator getInterpolator()
        {
            return mInterpolator;
        }

        /**
         * Sets the optional interpolator for this Keyframe. A value of <code>null</code> indicates
         * that there is no interpolation, which is the same as linear interpolation.
         *
         * @return The optional interpolator for this Keyframe.
         */
        public void setInterpolator(TimeInterpolator interpolator)
        {
            mInterpolator = interpolator;
        }

        /**
         * Gets the type of keyframe. This information is used by ValueAnimator to determine the type of
         * {@link TypeEvaluator} to use when calculating values between keyframes. The type is based
         * on the type of Keyframe created.
         *
         * @return The type of the value stored in the Keyframe.
         */
        public Type getType()
        {
            return mValueType;
        }

        public virtual Keyframe Clone()
        {
            var c = (Keyframe)Utils.ICloneable.Clone(this);
            c.mInterpolator = mInterpolator;
            c.mHasValue = mHasValue;
            c.mValueWasSetOnStart = mValueWasSetOnStart;
            c.mFraction = mFraction;
            return c;
        }

        /**
         * This internal subclass is used for all types which are not int or float.
         */
        internal class ObjectKeyframe : Keyframe
        {

            /**
             * The value of the animation at the time mFraction.
             */
            object mValue;

            internal ObjectKeyframe(float fraction, object value)
            {
                mFraction = fraction;
                mValue = value;
                mHasValue = value != null;
                mValueType = mHasValue ? value.GetType() : typeof(object);
            }

            override public object getValue()
            {
                return mValue;
            }

            override public void setValue(object value)
            {
                mValue = value;
                mHasValue = value != null;
            }

            override public ObjectKeyframe Clone()
            {
                ObjectKeyframe kfClone = (ObjectKeyframe)base.Clone();
                kfClone.mValue = mValue;
                return kfClone;
            }
        }

        /**
         * Internal subclass used when the keyframe value is of type int.
         */
        internal class IntKeyframe : Keyframe
        {

            /**
             * The value of the animation at the time mFraction.
             */
            int mValue;

            internal IntKeyframe(float fraction, int value)
            {
                mFraction = fraction;
                mValue = value;
                mValueType = typeof(int);
                mHasValue = true;
            }

            internal IntKeyframe(float fraction)
            {
                mFraction = fraction;
                mValueType = typeof(int);
            }

            public int getIntValue()
            {
                return mValue;
            }

            override public object getValue()
            {
                return mValue;
            }

            override public void setValue(object value)
            {
                if (value != null && value.GetType() == typeof(int))
                {
                    mValue = (int)value;
                    mHasValue = true;
                }
            }

            override public IntKeyframe Clone()
            {
                IntKeyframe kfClone = (IntKeyframe)base.Clone();
                kfClone.mValue = mValue;
                return kfClone;
            }
        }

        /**
         * Internal subclass used when the keyframe value is of type float.
         */
        internal class FloatKeyframe : Keyframe
        {
            /**
             * The value of the animation at the time mFraction.
             */
            float mValue;

            internal FloatKeyframe(float fraction, float value)
            {
                mFraction = fraction;
                mValue = value;
                mValueType = typeof(float);
                mHasValue = true;
            }

            internal FloatKeyframe(float fraction)
            {
                mFraction = fraction;
                mValueType = typeof(float);
            }

            public float getFloatValue()
            {
                return mValue;
            }

            override public object getValue()
            {
                return mValue;
            }

            override public void setValue(object value)
            {
                if (value != null && value.GetType() == typeof(float))
                {
                    mValue = (float)value;
                    mHasValue = true;
                }
            }

            override public FloatKeyframe Clone()
            {
                FloatKeyframe kfClone = (FloatKeyframe)base.Clone();
                kfClone.mValue = mValue;
                return kfClone;
            }
        }
    }
}