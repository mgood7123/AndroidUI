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

using SkiaSharp;

namespace AndroidUI
{
    /**
     * Shader used to draw a bitmap as a texture. The bitmap can be repeated or
     * mirrored by setting the tiling mode.
     */
    public class BitmapShader : Shader
    {
        /**
         * Prevent garbage collection.
         */
        Bitmap mBitmap;

        private int mTileX;
        private int mTileY;

        /*
         *  This is cache of the last value from the Paint of bitmap-filtering.
         *  In the future, BitmapShaders will carry their own (expanded) data for this
         *  (e.g. including mipmap options, or bicubic weights)
         *
         *  When that happens, this bool will become those extended values, and we will
         *  need to track whether this Shader was created with those new constructors,
         *  or from the current "legacy" constructor, which (for compatibility) will
         *  still need to know the Paint's setting.
         *
         *  When the filter Paint setting is finally gone, we will be able to remove
         *  the filterFromPaint parameter currently being passed to createNativeInstance()
         *  and shouldDiscardNativeInstance(), as shaders will always know their filter
         *  settings.
         */
        private bool mFilterFromPaint;

        /**
         * Call this to create a new shader that will draw with a bitmap.
         *
         * @param bitmap The bitmap to use inside the shader
         * @param tileX The tiling mode for x to draw the bitmap in.
         * @param tileY The tiling mode for y to draw the bitmap in.
         */
        public BitmapShader(Bitmap bitmap, TileMode tileX, TileMode tileY)
        : this(bitmap, tileX.nativeInt, tileY.nativeInt) {
        }

        private BitmapShader(Bitmap bitmap, int tileX, int tileY)
        {
            if (bitmap == null)
            {
                throw new Exceptions.IllegalArgumentException("Bitmap must be non-null");
            }
            mBitmap = bitmap;
            mTileX = tileX;
            mTileY = tileY;
            mFilterFromPaint = false;
        }

        /** @hide */
        protected override SKShader createNativeInstance(SKMatrix? nativeMatrix, bool filterFromPaint)
        {
            mFilterFromPaint = filterFromPaint;
            return nativeCreate(nativeMatrix, mBitmap.getNativeInstance(), mTileX, mTileY,
                                mFilterFromPaint);
        }

        /** @hide */
        protected override bool shouldDiscardNativeInstance(bool filterFromPaint)
        {
            return mFilterFromPaint != filterFromPaint;
        }

        private static SKShader nativeCreate(SKMatrix? nativeMatrix, SKBitmap bitmapHandle,
                int shaderTileModeX, int shaderTileModeY, bool filter)
        {
            SKImage image = null;
            if (bitmapHandle != null)
            {
                // Only pass a valid SkBitmap object to the constructor if the Bitmap exists. Otherwise,
                // we'll pass an empty SkBitmap to avoid crashing/excepting for compatibility.
                image = bitmapHandle.AsImage();
            }

            if (image != null)
            {
                SKBitmap bitmap = new();
                bitmap.SetImmutable();
                image = SKImage.FromBitmap(bitmap);
            }

            //SkSamplingOptions sampling(filter? SkFilterMode::kLinear : SkFilterMode::kNearest,
            //                   SkMipmapMode::kNone);
            SKShader shader = image.ToShader((SKShaderTileMode)shaderTileModeX, (SKShaderTileMode)shaderTileModeY);
            if (shader == null) throw new Exceptions.IllegalArgumentException();

            if (nativeMatrix != null)
            {
                shader = shader.WithLocalMatrix(nativeMatrix.Value);
            }

            return shader;
        }
    }
}