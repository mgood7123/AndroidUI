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

using AndroidUI.Exceptions;
using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Utils;
using static AndroidUI.AnimationFramework.Animator.Keyframes;

namespace AndroidUI.AnimationFramework.Animator
{
    /**
     * PathKeyframes relies on approximating the Path as a series of line segments.
     * The line segments are recursively divided until there is less than 1/2 pixel error
     * between the lines and the curve. Each point of the line segment is converted
     * to a Keyframe and a linear interpolation between Keyframes creates a good approximation
     * of the curve.
     * <p>
     * PathKeyframes is optimized to reduce the number of objects created when there are
     * many keyframes for a curve.
     * </p>
     * <p>
     * Typically, the returned type is a SkiaSharp.SKPoint, but the individual components can be extracted
     * as either an IntKeyframes or FloatKeyframes.
     * </p>
     * @hide
     */
    public class PathKeyframes : Keyframes
    {
        private const int FRACTION_OFFSET = 0;
        private const int X_OFFSET = 1;
        private const int Y_OFFSET = 2;
        private const int NUM_COMPONENTS = 3;
        private static readonly List<Keyframe> EMPTY_KEYFRAMES = new List<Keyframe>();

        private SkiaSharp.SKPoint mTempPoint = new SkiaSharp.SKPoint();
        private float[] mKeyframeData;

        public PathKeyframes(Graphics.Path path) : this(path, 0.5f)
        {
        }

        public PathKeyframes(Graphics.Path path, float error)
        {
            if (path == null || path.isEmpty())
            {
                throw new IllegalArgumentException("The path must not be null or empty");
            }
            mKeyframeData = path.approximate(error);
        }

        public List<Keyframe> getKeyframes()
        {
            return EMPTY_KEYFRAMES;
        }

        public Object getValue(float fraction)
        {
            int numPoints = mKeyframeData.Length / 3;
            if (fraction < 0)
            {
                return interpolateInRange(fraction, 0, 1);
            }
            else if (fraction > 1)
            {
                return interpolateInRange(fraction, numPoints - 2, numPoints - 1);
            }
            else if (fraction == 0)
            {
                return pointForIndex(0);
            }
            else if (fraction == 1)
            {
                return pointForIndex(numPoints - 1);
            }
            else
            {
                // Binary search for the correct section
                int low = 0;
                int high = numPoints - 1;

                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    float midFraction = mKeyframeData[(mid * NUM_COMPONENTS) + FRACTION_OFFSET];

                    if (fraction < midFraction)
                    {
                        high = mid - 1;
                    }
                    else if (fraction > midFraction)
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        return pointForIndex(mid);
                    }
                }

                // now high is below the fraction and low is above the fraction
                return interpolateInRange(fraction, high, low);
            }
        }

        private SkiaSharp.SKPoint interpolateInRange(float fraction, int startIndex, int endIndex)
        {
            int startBase = (startIndex * NUM_COMPONENTS);
            int endBase = (endIndex * NUM_COMPONENTS);

            float startFraction = mKeyframeData[startBase + FRACTION_OFFSET];
            float endFraction = mKeyframeData[endBase + FRACTION_OFFSET];

            float intervalFraction = (fraction - startFraction) / (endFraction - startFraction);

            float startX = mKeyframeData[startBase + X_OFFSET];
            float endX = mKeyframeData[endBase + X_OFFSET];
            float startY = mKeyframeData[startBase + Y_OFFSET];
            float endY = mKeyframeData[endBase + Y_OFFSET];

            float x = interpolate(intervalFraction, startX, endX);
            float y = interpolate(intervalFraction, startY, endY);

            mTempPoint.Set(x, y);
            return mTempPoint;
        }

        virtual public void setEvaluator(ITypeEvaluator evaluator)
        {
        }

        virtual public Type getType()
        {
            return typeof(SkiaSharp.SKPoint);
        }

        virtual public PathKeyframes Clone()
        {
            PathKeyframes clone = (PathKeyframes)Utils.ICloneable.Clone(this);
            clone.mTempPoint = new SkiaSharp.SKPoint();
            if (mKeyframeData != null)
            {
                clone.mKeyframeData = (float[])mKeyframeData.Clone();
            }
            return clone;
        }

        private SkiaSharp.SKPoint pointForIndex(int index)
        {
            int base_ = (index * NUM_COMPONENTS);
            int xOffset = base_ + X_OFFSET;
            int yOffset = base_ + Y_OFFSET;
            mTempPoint.Set(mKeyframeData[xOffset], mKeyframeData[yOffset]);
            return mTempPoint;
        }

        private static float interpolate(float fraction, float startValue, float endValue)
        {
            float diff = endValue - startValue;
            return startValue + (diff * fraction);
        }

        class XI : IntKeyframes
        {
            PathKeyframes outer;

            public XI(PathKeyframes outer)
            {
                this.outer = outer;
            }

            public XI Clone()
            {
                XI clone = (XI)Utils.ICloneable.Clone(this);
                clone.outer = outer.Clone();
                return clone;
            }

            public int getIntValue(float fraction)
            {
                SkiaSharp.SKPoint pointF = (SkiaSharp.SKPoint)outer.getValue(fraction);
                return (int)Math.Round(pointF.X);
            }

            public List<Keyframe> getKeyframes()
            {
                throw new NotImplementedException();
            }

            public Type getType()
            {
                throw new NotImplementedException();
            }

            public object getValue(float fraction)
            {
                throw new NotImplementedException();
            }

            public void setEvaluator(ITypeEvaluator evaluator)
            {
                throw new NotImplementedException();
            }
        }

        class YI : IntKeyframes
        {
            PathKeyframes outer;

            public YI(PathKeyframes outer)
            {
                this.outer = outer;
            }

            public YI Clone()
            {
                YI clone = (YI)Utils.ICloneable.Clone(this);
                clone.outer = outer.Clone();
                return clone;
            }

            public int getIntValue(float fraction)
            {
                SkiaSharp.SKPoint pointF = (SkiaSharp.SKPoint)outer.getValue(fraction);
                return (int)Math.Round(pointF.Y);
            }

            public List<Keyframe> getKeyframes()
            {
                throw new NotImplementedException();
            }

            public Type getType()
            {
                throw new NotImplementedException();
            }

            public object getValue(float fraction)
            {
                throw new NotImplementedException();
            }

            public void setEvaluator(ITypeEvaluator evaluator)
            {
                throw new NotImplementedException();
            }
        }

        class XF : FloatKeyframes
        {
            PathKeyframes outer;

            public XF(PathKeyframes outer)
            {
                this.outer = outer;
            }

            public XF Clone()
            {
                XF clone = (XF)Utils.ICloneable.Clone(this);
                clone.outer = outer.Clone();
                return clone;
            }

            public float getFloatValue(float fraction)
            {
                SkiaSharp.SKPoint pointF = (SkiaSharp.SKPoint)outer.getValue(fraction);
                return pointF.X;
            }

            public List<Keyframe> getKeyframes()
            {
                throw new NotImplementedException();
            }

            public Type getType()
            {
                throw new NotImplementedException();
            }

            public object getValue(float fraction)
            {
                throw new NotImplementedException();
            }

            public void setEvaluator(ITypeEvaluator evaluator)
            {
                throw new NotImplementedException();
            }
        }

        class YF : FloatKeyframes
        {
            PathKeyframes outer;

            public YF(PathKeyframes outer)
            {
                this.outer = outer;
            }

            public YF Clone()
            {
                YF clone = (YF)Utils.ICloneable.Clone(this);
                clone.outer = outer.Clone();
                return clone;
            }

            public float getFloatValue(float fraction)
            {
                SkiaSharp.SKPoint pointF = (SkiaSharp.SKPoint)outer.getValue(fraction);
                return pointF.Y;
            }

            public List<Keyframe> getKeyframes()
            {
                throw new NotImplementedException();
            }

            public Type getType()
            {
                throw new NotImplementedException();
            }

            public object getValue(float fraction)
            {
                throw new NotImplementedException();
            }

            public void setEvaluator(ITypeEvaluator evaluator)
            {
                throw new NotImplementedException();
            }
        }

        /**
         * Returns a FloatKeyframes for the X component of the Path.
         * @return a FloatKeyframes for the X component of the Path.
         */
        public FloatKeyframes createXFloatKeyframes()
        {
            return new XF(this);
        }

        /**
         * Returns a FloatKeyframes for the Y component of the Path.
         * @return a FloatKeyframes for the Y component of the Path.
         */
        public FloatKeyframes createYFloatKeyframes()
        {
            return new YF(this);
        }

        /**
         * Returns an IntKeyframes for the X component of the Path.
         * @return an IntKeyframes for the X component of the Path.
         */
        public IntKeyframes createXIntKeyframes()
        {
            return new XI(this);
        }

        /**
         * Returns an IntKeyframeSet for the Y component of the Path.
         * @return an IntKeyframeSet for the Y component of the Path.
         */
        public IntKeyframes createYIntKeyframes()
        {
            return new YI(this);
        }

        internal abstract class SimpleKeyframes : Keyframes
        {
            public void setEvaluator(ITypeEvaluator evaluator)
            {
            }

            public List<Keyframe> getKeyframes()
            {
                return EMPTY_KEYFRAMES;
            }

            public Keyframes Clone()
            {
                Keyframes clone = (Keyframes)Utils.ICloneable.Clone(this);
                return clone;
            }

            public abstract Type getType();
            public abstract object getValue(float fraction);
        }

        internal abstract class IntKeyframesBase : SimpleKeyframes, IntKeyframes
        {
            public abstract int getIntValue(float fraction);

            override
            public Type getType()
            {
                return typeof(int);
            }

            override
            public Object getValue(float fraction)
            {
                return getIntValue(fraction);
            }
        }

        internal abstract class FloatKeyframesBase : SimpleKeyframes, FloatKeyframes
        {
            public abstract float getFloatValue(float fraction);

            override
            public Type getType()
            {
                return typeof(float);
            }

            override
            public Object getValue(float fraction)
            {
                return getFloatValue(fraction);
            }
        }
    }
}