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

using AndroidUI.Exceptions;
using AndroidUI.Utils;

namespace AndroidUI.Graphics
{
    /**
     * Shader is the base class for objects that return horizontal spans of colors
     * during drawing. A subclass of Shader is installed in a Paint calling
     * paint.setShader(shader). After that any object (other than a bitmap) that is
     * drawn with that paint will get its color(s) from the shader.
     */
    public class Shader
    {

        /**
         * @deprecated Use subclass constructors directly instead.
         */
        [Obsolete]
        public Shader()
        {
            mColorSpace = null;
        }

        /**
         * @hide Only to be used by subclasses in android.graphics.
         */
        protected Shader(ColorSpace colorSpace)
        {
            mColorSpace = colorSpace;
            if (colorSpace == null)
            {
                throw new IllegalArgumentException(
                        "Use Shader() to create a Shader with no ColorSpace");
            }

            // This just ensures that if the ColorSpace is invalid, the Exception will be thrown now.
            mColorSpace.getNativeInstance();
        }

        private readonly ColorSpace mColorSpace;

        /**
         * @hide Only to be used by subclasses in android.graphics.
         */
        protected ColorSpace colorSpace()
        {
            return mColorSpace;
        }

        /**
         * Current native shader instance. Created and updated lazily when {@link #getNativeInstance()}
         * is called - otherwise may be out of date with java setters/properties.
         */
        private SkiaSharp.SKShader mNativeInstance;
        // Runnable to do immediate destruction
        private Runnable mCleaner;

        /**
         * Current matrix - always set to null if local matrix is identity.
         */
        private SkiaSharp.SKMatrix mLocalMatrix;

        public class TileMode
        {
            /**
             * Replicate the edge color if the shader draws outside of its
             * original bounds.
             */
            public static readonly TileMode CLAMP = new(0);
            /**
             * Repeat the shader's image horizontally and vertically.
             */
            public static readonly TileMode REPEAT = new(1);
            /**
             * Repeat the shader's image horizontally and vertically, alternating
             * mirror images so that adjacent images always seam.
             */
            public static readonly TileMode MIRROR = new(2);
            /**
             * Render the shader's image pixels only within its original bounds. If the shader
             * draws outside of its original bounds, transparent black is drawn instead.
             */
            public static readonly TileMode DECAL = new(3);

            TileMode(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }
            internal int nativeInt;

            public static implicit operator SkiaSharp.SKShaderTileMode(TileMode value) => value.ToSKShaderTileMode();

            public SkiaSharp.SKShaderTileMode ToSKShaderTileMode()
            {
                switch (nativeInt)
                {
                    case 0:
                        return SkiaSharp.SKShaderTileMode.Clamp;
                    case 1:
                        return SkiaSharp.SKShaderTileMode.Repeat;
                    case 2:
                        return SkiaSharp.SKShaderTileMode.Mirror;
                    case 3:
                        return SkiaSharp.SKShaderTileMode.Decal;
                    default:
                        throw new IllegalStateException();
                }
            }
        }

        /**
         * Return true if the shader has a non-identity local matrix.
         * @param localM Set to the local matrix of the shader, if the shader's matrix is non-null.
         * @return true if the shader has a non-identity local matrix
         */
        public bool getLocalMatrix(ref SkiaSharp.SKMatrix localM)
        {
            if (mLocalMatrix != null)
            {
                localM = mLocalMatrix;
                return true; // presence of mLocalMatrix means it's not identity
            }
            return false;
        }

        /**
         * Set the shader's local matrix. Passing null will reset the shader's
         * matrix to identity. If the matrix has scale value as 0, the drawing
         * result is undefined.
         *
         * @param localM The shader's new local matrix, or null to specify identity
         */
        public void setLocalMatrix(SkiaSharp.SKMatrix localM)
        {
            if (localM == null || localM.IsIdentity)
            {
                if (mLocalMatrix != null)
                {
                    //mLocalMatrix = null;
                    discardNativeInstance();
                }
            }
            else
            {
                if (mLocalMatrix == null)
                {
                    mLocalMatrix = new SkiaSharp.SKMatrix(localM.Values);
                    discardNativeInstance();
                }
                else if (!mLocalMatrix.Equals(localM))
                {
                    mLocalMatrix = localM;
                    discardNativeInstance();
                }
            }
        }

        /**
         *  @hide Only to be used by subclasses in the graphics package.
         */
        virtual protected SkiaSharp.SKShader createNativeInstance(SkiaSharp.SKMatrix? nativeMatrix, bool filterFromPaint)
        {
            return null;
        }

        static readonly object LOCK = new();

        /**
         *  @hide Only to be used by subclasses in the graphics package.
         */
        protected void discardNativeInstance()
        {
            lock (LOCK)
            {
                discardNativeInstanceLocked();
            }
        }

        // For calling inside a synchronized method.
        private void discardNativeInstanceLocked()
        {
            if (mNativeInstance != null)
            {
                mCleaner?.Invoke();
                mCleaner = null;
                mNativeInstance.Dispose();
                mNativeInstance = null;
            }
        }

        /**
         * Callback for subclasses to specify whether the most recently
         * constructed native instance is still valid.
         *  @hide Only to be used by subclasses in the graphics package.
         */
        virtual protected bool shouldDiscardNativeInstance(bool filterBitmap)
        {
            return false;
        }


        /**
         * @hide so it can be called by android.graphics.drawable but must not be called from outside
         * the module.
         */
        public SkiaSharp.SKShader getNativeInstance(bool filterFromPaint)
        {
            lock (LOCK)
            {
                if (shouldDiscardNativeInstance(filterFromPaint))
                {
                    discardNativeInstanceLocked();
                }

                if (mNativeInstance == null)
                {
                    mNativeInstance = createNativeInstance(mLocalMatrix == null
                            ? SkiaSharp.SKMatrix.Identity : mLocalMatrix, filterFromPaint);
                }
                return mNativeInstance;
            }
        }

        /**
         * @hide so it can be called by android.graphics.drawable but must not be called from outside
         * the module.
         */
        public SkiaSharp.SKShader getNativeInstance()
        {
            // If the caller has no paint flag for filtering bitmaps, we just pass false
            return getNativeInstance(false);
        }

        /**
         * @hide Only to be called by subclasses in the android.graphics package.
         */
        protected static long[] convertColors(int[] colors)
        {
            if (colors.Length < 2)
            {
                throw new IllegalArgumentException("needs >= 2 number of colors");
            }

            long[] colorLongs = new long[colors.Length];
            for (int i = 0; i < colors.Length; ++i)
            {
                colorLongs[i] = Color.pack(colors[i]);
            }

            return colorLongs;
        }

        /**
         * Detect the ColorSpace that the {@code colors} share.
         *
         * @throws IllegalArgumentException if the colors do not all share the same,
         *      valid ColorSpace, or if there are less than 2 colors.
         *
         * @hide Only to be called by subclasses in the android.graphics package.
         */
        protected static ColorSpace detectColorSpace(long[] colors)
        {
            if (colors.Length < 2)
            {
                throw new IllegalArgumentException("needs >= 2 number of colors");
            }
            ColorSpace colorSpace = Color.colorSpace(colors[0]);
            for (int i = 1; i < colors.Length; ++i)
            {
                if (Color.colorSpace(colors[i]) != colorSpace)
                {
                    throw new IllegalArgumentException("All colors must be in the same ColorSpace!");
                }
            }
            return colorSpace;
        }

    }
}