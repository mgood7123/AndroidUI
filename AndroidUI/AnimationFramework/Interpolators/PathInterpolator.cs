﻿/*
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

using AndroidUI.Exceptions;

namespace AndroidUI.AnimationFramework.Interpolators
{
    /**
     * An interpolator that can traverse a Path that extends from <code>Point</code>
     * <code>(0, 0)</code> to <code>(1, 1)</code>. The x coordinate along the <code>Path</code>
     * is the input value and the output is the y coordinate of the line at that point.
     * This means that the Path must conform to a function <code>y = f(x)</code>.
     *
     * <p>The <code>Path</code> must not have gaps in the x direction and must not
     * loop back on itself such that there can be two points sharing the same x coordinate.
     * It is alright to have a disjoint line in the vertical direction:</p>
     * <p><blockquote><pre>
     *     Path path = new Path();
     *     path.lineTo(0.25f, 0.25f);
     *     path.moveTo(0.25f, 0.5f);
     *     path.lineTo(1f, 1f);
     * </pre></blockquote></p>
     */
    public class PathInterpolator : BaseInterpolator, ICloneable
    {

        // This governs how accurate the approximation of the Path is.
        private static float PRECISION = 0.002f;

        private float[] mX; // x coordinates in the line

        private float[] mY; // y coordinates in the line

        virtual public PathInterpolator Clone()
        {
            PathInterpolator obj = (PathInterpolator)ICloneable.Clone(this);
            obj.mX = (float[])mX?.Clone();
            obj.mY = (float[])mY?.Clone();
            return obj;
        }


        /**
         * Create an interpolator for an arbitrary <code>Path</code>. The <code>Path</code>
         * must begin at <code>(0, 0)</code> and end at <code>(1, 1)</code>.
         *
         * @param path The <code>Path</code> to use to make the line representing the interpolator.
         */
        public PathInterpolator(Path path)
        {
            initPath(path);
        }

        /**
         * Create an interpolator for a quadratic Bezier curve. The end points
         * <code>(0, 0)</code> and <code>(1, 1)</code> are assumed.
         *
         * @param controlX The x coordinate of the quadratic Bezier control point.
         * @param controlY The y coordinate of the quadratic Bezier control point.
         */
        public PathInterpolator(float controlX, float controlY)
        {
            initQuad(controlX, controlY);
        }

        /**
         * Create an interpolator for a cubic Bezier curve.  The end points
         * <code>(0, 0)</code> and <code>(1, 1)</code> are assumed.
         *
         * @param controlX1 The x coordinate of the first control point of the cubic Bezier.
         * @param controlY1 The y coordinate of the first control point of the cubic Bezier.
         * @param controlX2 The x coordinate of the second control point of the cubic Bezier.
         * @param controlY2 The y coordinate of the second control point of the cubic Bezier.
         */
        public PathInterpolator(float controlX1, float controlY1, float controlX2, float controlY2)
        {
            initCubic(controlX1, controlY1, controlX2, controlY2);
        }

        private void initQuad(float controlX, float controlY)
        {
            Path path = new Path();
            path.moveTo(0, 0);
            path.quadTo(controlX, controlY, 1f, 1f);
            initPath(path);
        }

        private void initCubic(float x1, float y1, float x2, float y2)
        {
            Path path = new Path();
            path.moveTo(0, 0);
            path.cubicTo(x1, y1, x2, y2, 1f, 1f);
            initPath(path);
        }

        private void initPath(Path path)
        {
            float[] pointComponents = path.approximate(PRECISION);

            int numPoints = pointComponents.Length / 3;
            if (pointComponents[1] != 0 || pointComponents[2] != 0
                    || pointComponents[pointComponents.Length - 2] != 1
                    || pointComponents[pointComponents.Length - 1] != 1)
            {
                throw new IllegalArgumentException("The Path must start at (0,0) and end at (1,1)");
            }

            mX = new float[numPoints];
            mY = new float[numPoints];
            float prevX = 0;
            float prevFraction = 0;
            int componentIndex = 0;
            for (int i = 0; i < numPoints; i++)
            {
                float fraction = pointComponents[componentIndex++];
                float x = pointComponents[componentIndex++];
                float y = pointComponents[componentIndex++];
                if (fraction == prevFraction && x != prevX)
                {
                    throw new IllegalArgumentException(
                            "The Path cannot have discontinuity in the X axis.");
                }
                if (x < prevX)
                {
                    throw new IllegalArgumentException("The Path cannot loop back on itself.");
                }
                mX[i] = x;
                mY[i] = y;
                prevX = x;
                prevFraction = fraction;
            }
        }

        /**
         * Using the line in the Path in this interpolator that can be described as
         * <code>y = f(x)</code>, finds the y coordinate of the line given <code>t</code>
         * as the x coordinate. Values less than 0 will always return 0 and values greater
         * than 1 will always return 1.
         *
         * @param t Treated as the x coordinate along the line.
         * @return The y coordinate of the Path along the line where x = <code>t</code>.
         * @see Interpolator#getInterpolation(float)
         */
        public override float getInterpolation(float t)
        {
            if (t <= 0)
            {
                return 0;
            }
            else if (t >= 1)
            {
                return 1;
            }
            // Do a binary search for the correct x to interpolate between.
            int startIndex = 0;
            int endIndex = mX.Length - 1;

            while (endIndex - startIndex > 1)
            {
                int midIndex = (startIndex + endIndex) / 2;
                if (t < mX[midIndex])
                {
                    endIndex = midIndex;
                }
                else
                {
                    startIndex = midIndex;
                }
            }

            float xRange = mX[endIndex] - mX[startIndex];
            if (xRange == 0)
            {
                return mY[startIndex];
            }

            float tInRange = t - mX[startIndex];
            float fraction = tInRange / xRange;

            float startY = mY[startIndex];
            float endY = mY[endIndex];
            return startY + (fraction * (endY - startY));
        }

    }
}
