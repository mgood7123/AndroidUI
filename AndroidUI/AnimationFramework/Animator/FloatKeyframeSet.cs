/*
 * Copyright (C) 2010 The Android Open Source Project
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
using static AndroidUI.AnimationFramework.Animator.Keyframe;

namespace AndroidUI.AnimationFramework.Animator
{
    /**
     * This class holds a collection of FloatKeyframe objects and is called by ValueAnimator to calculate
     * values between those keyframes for a given animation. The class internal to the animation
     * package because it is an implementation detail of how Keyframes are stored and used.
     *
     * <p>This type-specific subclass of KeyframeSet, along with the other type-specific subclass for
     * int, exists to speed up the getValue() method when there is no custom
     * TypeEvaluator set for the animation, so that values can be calculated without autoboxing to the
     * Object equivalents of these primitive types.</p>
     */
    class FloatKeyframeSet : KeyframeSet, Keyframes.FloatKeyframes
    {
        public FloatKeyframeSet(params FloatKeyframe[] keyframes) : base(keyframes)
        {
        }

        override
        public Object getValue(float fraction)
        {
            return getFloatValue(fraction);
        }

        override
        public FloatKeyframeSet Clone()
        {
            List<Keyframe> keyframes = mKeyframes;
            int numKeyframes = mKeyframes.Count;
            FloatKeyframe[] newKeyframes = new FloatKeyframe[numKeyframes];
            for (int i = 0; i < numKeyframes; ++i)
            {
                newKeyframes[i] = (FloatKeyframe)keyframes.ElementAt(i).Clone();
            }
            FloatKeyframeSet newSet = new FloatKeyframeSet(newKeyframes);
            return newSet;
        }

        public float getFloatValue(float fraction)
        {
            FloatKeyframe prevKeyframe;
            FloatKeyframe nextKeyframe;
            if (fraction <= 0f)
            {
                prevKeyframe = (FloatKeyframe)mKeyframes.ElementAt(0);
                nextKeyframe = (FloatKeyframe)mKeyframes.ElementAt(1);
                float prevValue = prevKeyframe.getFloatValue();
                float nextValue = nextKeyframe.getFloatValue();
                float prevFraction = prevKeyframe.getFraction();
                float nextFraction = nextKeyframe.getFraction();
                TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                if (interpolator != null)
                {
                    fraction = interpolator.getInterpolation(fraction);
                }
                float intervalFraction = (fraction - prevFraction) / (nextFraction - prevFraction);
                return mEvaluator == null ?
                        prevValue + intervalFraction * (nextValue - prevValue) :
                        ((float)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
            }
            else if (fraction >= 1f)
            {
                prevKeyframe = (FloatKeyframe)mKeyframes.ElementAt(mNumKeyframes - 2);
                nextKeyframe = (FloatKeyframe)mKeyframes.ElementAt(mNumKeyframes - 1);
                float prevValue = prevKeyframe.getFloatValue();
                float nextValue = nextKeyframe.getFloatValue();
                float prevFraction = prevKeyframe.getFraction();
                float nextFraction = nextKeyframe.getFraction();
                TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                if (interpolator != null)
                {
                    fraction = interpolator.getInterpolation(fraction);
                }
                float intervalFraction = (fraction - prevFraction) / (nextFraction - prevFraction);
                return mEvaluator == null ?
                        prevValue + intervalFraction * (nextValue - prevValue) :
                        ((float)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
            }
            prevKeyframe = (FloatKeyframe)mKeyframes.ElementAt(0);
            for (int i = 1; i < mNumKeyframes; ++i)
            {
                nextKeyframe = (FloatKeyframe)mKeyframes.ElementAt(i);
                if (fraction < nextKeyframe.getFraction())
                {
                    TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                    float intervalFraction = (fraction - prevKeyframe.getFraction()) /
                        (nextKeyframe.getFraction() - prevKeyframe.getFraction());
                    float prevValue = prevKeyframe.getFloatValue();
                    float nextValue = nextKeyframe.getFloatValue();
                    // Apply interpolator on the proportional duration.
                    if (interpolator != null)
                    {
                        intervalFraction = interpolator.getInterpolation(intervalFraction);
                    }
                    return mEvaluator == null ?
                            prevValue + intervalFraction * (nextValue - prevValue) :
                            ((float)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
                }
                prevKeyframe = nextKeyframe;
            }
            // shouldn't get here
            return ((float)mKeyframes.ElementAt(mNumKeyframes - 1).getValue());
        }

        override
        public Type getType()
        {
            return typeof(float);
        }
    }

}