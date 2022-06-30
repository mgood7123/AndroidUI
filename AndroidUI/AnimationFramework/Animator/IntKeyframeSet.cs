﻿/*
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
     * This class holds a collection of IntKeyframe objects and is called by ValueAnimator to calculate
     * values between those keyframes for a given animation. The class internal to the animation
     * package because it is an implementation detail of how Keyframes are stored and used.
     *
     * <p>This type-specific subclass of KeyframeSet, along with the other type-specific subclass for
     * float, exists to speed up the getValue() method when there is no custom
     * TypeEvaluator set for the animation, so that values can be calculated without autoboxing to the
     * Object equivalents of these primitive types.</p>
     */
    class IntKeyframeSet : KeyframeSet, Keyframes.IntKeyframes
    {
        public IntKeyframeSet(params IntKeyframe[] keyframes) : base(keyframes)
        {
        }

        override
        public Object getValue(float fraction)
        {
            return getIntValue(fraction);
        }

        override
        public IntKeyframeSet Clone()
        {
            List<Keyframe> keyframes = mKeyframes;
            int numKeyframes = mKeyframes.Count;
            IntKeyframe[] newKeyframes = new IntKeyframe[numKeyframes];
            for (int i = 0; i < numKeyframes; ++i)
            {
                newKeyframes[i] = (IntKeyframe)keyframes.ElementAt(i).Clone();
            }
            IntKeyframeSet newSet = new IntKeyframeSet(newKeyframes);
            return newSet;
        }

        public int getIntValue(float fraction)
        {
            IntKeyframe prevKeyframe;
            IntKeyframe nextKeyframe;
            if (fraction <= 0f)
            {
                prevKeyframe = (IntKeyframe)mKeyframes.ElementAt(0);
                nextKeyframe = (IntKeyframe)mKeyframes.ElementAt(1);
                int prevValue = prevKeyframe.getIntValue();
                int nextValue = nextKeyframe.getIntValue();
                float prevFraction = prevKeyframe.getFraction();
                float nextFraction = nextKeyframe.getFraction();
                TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                if (interpolator != null)
                {
                    fraction = interpolator.getInterpolation(fraction);
                }
                float intervalFraction = (fraction - prevFraction) / (nextFraction - prevFraction);
                return mEvaluator == null ?
                        prevValue + (int)(intervalFraction * (nextValue - prevValue)) :
                        ((int)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
            }
            else if (fraction >= 1f)
            {
                prevKeyframe = (IntKeyframe)mKeyframes.ElementAt(mNumKeyframes - 2);
                nextKeyframe = (IntKeyframe)mKeyframes.ElementAt(mNumKeyframes - 1);
                int prevValue = prevKeyframe.getIntValue();
                int nextValue = nextKeyframe.getIntValue();
                float prevFraction = prevKeyframe.getFraction();
                float nextFraction = nextKeyframe.getFraction();
                TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                if (interpolator != null)
                {
                    fraction = interpolator.getInterpolation(fraction);
                }
                float intervalFraction = (fraction - prevFraction) / (nextFraction - prevFraction);
                return mEvaluator == null ?
                        prevValue + (int)(intervalFraction * (nextValue - prevValue)) :
                        ((int)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
            }
            prevKeyframe = (IntKeyframe)mKeyframes.ElementAt(0);
            for (int i = 1; i < mNumKeyframes; ++i)
            {
                nextKeyframe = (IntKeyframe)mKeyframes.ElementAt(i);
                if (fraction < nextKeyframe.getFraction())
                {
                    TimeInterpolator interpolator = nextKeyframe.getInterpolator();
                    float intervalFraction = (fraction - prevKeyframe.getFraction()) /
                        (nextKeyframe.getFraction() - prevKeyframe.getFraction());
                    int prevValue = prevKeyframe.getIntValue();
                    int nextValue = nextKeyframe.getIntValue();
                    // Apply interpolator on the proportional duration.
                    if (interpolator != null)
                    {
                        intervalFraction = interpolator.getInterpolation(intervalFraction);
                    }
                    return mEvaluator == null ?
                            prevValue + (int)(intervalFraction * (nextValue - prevValue)) :
                            ((int)mEvaluator.evaluate(intervalFraction, prevValue, nextValue));
                }
                prevKeyframe = nextKeyframe;
            }
            // shouldn't get here
            return (int)mKeyframes.ElementAt(mNumKeyframes - 1).getValue();
        }

        override
        public Type getType()
        {
            return typeof(int);
        }
    }

}