﻿/*
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

using SkiaSharp;
using System.Text;

namespace AndroidUI.AnimationFramework
{
    /**
     * Defines the transformation to be applied at
     * one point in time of an Animation.
     *
     */
    public class Transformation
    {
        /**
         * Indicates a transformation that has no effect (alpha = 1 and identity matrix.)
         */
        public const int TYPE_IDENTITY = 0x0;
        /**
         * Indicates a transformation that applies an alpha only (uses an identity matrix.)
         */
        public const int TYPE_ALPHA = 0x1;
        /**
         * Indicates a transformation that applies a matrix only (alpha = 1.)
         */
        public const int TYPE_MATRIX = 0x2;
        /**
         * Indicates a transformation that applies an alpha and a matrix.
         */
        public const int TYPE_BOTH = TYPE_ALPHA | TYPE_MATRIX;

        protected ValueHolder<SKMatrix> mMatrix;
        protected float mAlpha;
        protected int mTransformationType;

        private bool mHasClipRect;
        private Rect mClipRect = new Rect();

        /**
         * Creates a new transformation with alpha = 1 and the identity matrix.
         */
        public Transformation()
        {
            clear();
        }

        /**
         * Reset the transformation to a state that leaves the object
         * being animated in an unmodified state. The transformation type is
         * {@link #TYPE_BOTH} by default.
         */
        public void clear()
        {
            if (mMatrix == null)
            {
                mMatrix = new SKMatrix();
            }
            else
            {
                mMatrix.Value.Reset();
            }
            mClipRect.setEmpty();
            mHasClipRect = false;
            mAlpha = 1.0f;
            mTransformationType = TYPE_BOTH;
        }

        /**
         * Indicates the nature of this transformation.
         *
         * @return {@link #TYPE_ALPHA}, {@link #TYPE_MATRIX},
         *         {@link #TYPE_BOTH} or {@link #TYPE_IDENTITY}.
         */
        public int getTransformationType()
        {
            return mTransformationType;
        }

        /**
         * Sets the transformation type.
         *
         * @param transformationType One of {@link #TYPE_ALPHA},
         *        {@link #TYPE_MATRIX}, {@link #TYPE_BOTH} or
         *        {@link #TYPE_IDENTITY}.
         */
        public void setTransformationType(int transformationType)
        {
            mTransformationType = transformationType;
        }

        /**
         * Clones the specified transformation.
         *
         * @param t The transformation to clone.
         */
        public void set(Transformation t)
        {
            mAlpha = t.getAlpha();
            mMatrix.Value = t.getMatrix().Value;
            if (t.mHasClipRect)
            {
                setClipRect(t.getClipRect());
            }
            else
            {
                mHasClipRect = false;
                mClipRect.setEmpty();
            }
            mTransformationType = t.getTransformationType();
        }

        /**
         * Apply this Transformation to an existing Transformation, e.g. apply
         * a scale effect to something that has already been rotated.
         * @param t
         */
        public void compose(Transformation t)
        {
            mAlpha *= t.getAlpha();
            mMatrix.Value.PreConcat(t.getMatrix().Value);
            if (t.mHasClipRect)
            {
                Rect bounds = t.getClipRect();
                if (mHasClipRect)
                {
                    setClipRect(mClipRect.left + bounds.left, mClipRect.top + bounds.top,
                            mClipRect.right + bounds.right, mClipRect.bottom + bounds.bottom);
                }
                else
                {
                    setClipRect(bounds);
                }
            }
        }

        /**
         * Like {@link #compose(Transformation)} but does this.postConcat(t) of
         * the transformation matrix.
         * @hide
         */
        public void postCompose(Transformation t)
        {
            mAlpha *= t.getAlpha();
            mMatrix.Value.PostConcat(t.getMatrix().Value);
            if (t.mHasClipRect)
            {
                Rect bounds = t.getClipRect();
                if (mHasClipRect)
                {
                    setClipRect(mClipRect.left + bounds.left, mClipRect.top + bounds.top,
                            mClipRect.right + bounds.right, mClipRect.bottom + bounds.bottom);
                }
                else
                {
                    setClipRect(bounds);
                }
            }
        }

        /**
         * @return The 3x3 Matrix representing the transformation to apply to the
         * coordinates of the object being animated
         */
        public ValueHolder<SKMatrix> getMatrix()
        {
            return mMatrix;
        }

        /**
         * Sets the degree of transparency
         * @param alpha 1.0 means fully opaqe and 0.0 means fully transparent
         */
        public void setAlpha(float alpha)
        {
            mAlpha = MathUtils.clamp(alpha, 0.0f, 1.0f);
        }

        /**
         * Sets the current Transform's clip rect
         * @hide
         */
        internal void setClipRect(Rect r)
        {
            setClipRect(r.left, r.top, r.right, r.bottom);
        }

        /**
         * Sets the current Transform's clip rect
         * @hide
         */
        internal void setClipRect(int l, int t, int r, int b)
        {
            mClipRect.set(l, t, r, b);
            mHasClipRect = true;
        }

        /**
         * Returns the current Transform's clip rect
         * @hide
         */
        internal Rect getClipRect()
        {
            return mClipRect;
        }

        /**
         * Returns whether the current Transform's clip rect is set
         * @hide
         */
        internal bool hasClipRect()
        {
            return mHasClipRect;
        }

        /**
         * @return The degree of transparency
         */
        public float getAlpha()
        {
            return mAlpha;
        }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("Transformation");
            toShortString(sb);
            return sb.ToString();
        }

        /**
         * Return a string representation of the transformation in a compact form.
         */
        public string toShortString()
        {
            StringBuilder sb = new StringBuilder(64);
            toShortString(sb);
            return sb.ToString();
        }

        /**
         * @hide
         */
        internal void toShortString(StringBuilder sb)
        {
            sb.Append("{alpha="); sb.Append(mAlpha);
            sb.Append(" matrix="); toShortString(mMatrix, sb);
            sb.Append('}');
        }

        internal void toShortString(ValueHolder<SKMatrix> m, StringBuilder sb)
        {
            if (m == null)
            {
                sb.Append("<null>");
                return;
            }
            float[] values = new float[9];
            m.Value.Get9(values);
            sb.Append('[');
            sb.Append(values[0]);
            sb.Append(", ");
            sb.Append(values[1]);
            sb.Append(", ");
            sb.Append(values[2]);
            sb.Append("][");
            sb.Append(values[3]);
            sb.Append(", ");
            sb.Append(values[4]);
            sb.Append(", ");
            sb.Append(values[5]);
            sb.Append("][");
            sb.Append(values[6]);
            sb.Append(", ");
            sb.Append(values[7]);
            sb.Append(", ");
            sb.Append(values[8]);
            sb.Append(']');
        }
    }
}