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

using AndroidUI.Extensions;
using AndroidUI.Skia;
using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;
using AndroidUI.Utils.Graphics;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    /**
     * The NinePatch class permits drawing a bitmap in nine or more sections.
     * Essentially, it allows the creation of custom graphics that will scale the
     * way that you define, when content added within the image exceeds the normal
     * bounds of the graphic. For a thorough explanation of a NinePatch image, 
     * read the discussion in the 
     * <a href="{@docRoot}guide/topics/graphics/2d-graphics.html#nine-patch">2D
     * Graphics</a> document.
     * <p>
     * The <a href="{@docRoot}guide/developing/tools/draw9patch.html">Draw 9-Patch</a> 
     * tool offers an extremely handy way to create your NinePatch images,
     * using a WYSIWYG graphics editor.
     * </p>
     */
    public unsafe class NinePatch
    {
        /**
         * Struct of inset information attached to a 9 patch bitmap.
         *
         * Present on a 9 patch bitmap if it optical insets were manually included,
         * or if outline insets were automatically included by aapt.
         *
         * @hide For use by NinePatchDrawable, but must not be used outside the module.
         */
        public class InsetStruct
        {
            internal InsetStruct(int opticalLeft, int opticalTop, int opticalRight, int opticalBottom,
                    int outlineLeft, int outlineTop, int outlineRight, int outlineBottom,
                    float outlineRadius, int outlineAlpha, float decodeScale)
            {
                opticalRect = new Rect(opticalLeft, opticalTop, opticalRight, opticalBottom);
                opticalRect.scale(decodeScale);

                outlineRect = scaleInsets(outlineLeft, outlineTop,
                        outlineRight, outlineBottom, decodeScale);

                this.outlineRadius = outlineRadius * decodeScale;
                this.outlineAlpha = outlineAlpha / 255.0f;
            }

            public readonly Rect opticalRect;
            public readonly Rect outlineRect;
            public readonly float outlineRadius;
            public readonly float outlineAlpha;

            /**
             * Scales up the rect by the given scale, ceiling values, so actual outline Rect
             * grows toward the inside.
             */
            public static Rect scaleInsets(int left, int top, int right, int bottom, float scale)
            {
                if (scale == 1.0f)
                {
                    return new Rect(left, top, right, bottom);
                }

                Rect result = new();
                result.left = (int)Math.Ceiling(left * scale);
                result.top = (int)Math.Ceiling(top * scale);
                result.right = (int)Math.Ceiling(right * scale);
                result.bottom = (int)Math.Ceiling(bottom * scale);
                return result;
            }
        }

        private readonly Bitmap mBitmap;

        /**
         * Used by native code. This pointer is an instance of Res_png_9patch*.
         *
         * @hide for use by android.graphics, but must not be used outside the module.
         */
        internal sbyte* mNativeChunk;

        private Paint mPaint;
        private string mSrcName;

        /**
         * Create a drawable projection from a bitmap to nine patches.
         *
         * @param bitmap The bitmap describing the patches.
         * @param chunk The 9-patch data chunk describing how the underlying bitmap
         *              is split apart and drawn.
         */
        public NinePatch(Bitmap bitmap, byte[] chunk)
            : this(bitmap, chunk, null)
        {
        }

        /** 
         * Create a drawable projection from a bitmap to nine patches.
         *
         * @param bitmap The bitmap describing the patches.
         * @param chunk The 9-patch data chunk describing how the underlying
         *              bitmap is split apart and drawn.
         * @param srcName The name of the source for the bitmap. Might be null.
         */
        public NinePatch(Bitmap bitmap, byte[] chunk, string srcName)
        {
            mBitmap = bitmap;
            mSrcName = srcName;
            mNativeChunk = validateNinePatchChunk(chunk);
        }

        ~NinePatch()
        {
            if (mNativeChunk != null)
            {
                // only attempt to destroy correctly initilized chunks
                nativeFinalize(mNativeChunk);
                mNativeChunk = null;
            }
        }

        /**
         * Returns the name of this NinePatch object if one was specified
         * when calling the constructor.
         */
        public string getName()
        {
            return mSrcName;
        }

        /**
         * Returns the paint used to draw this NinePatch. The paint can be null.
         *
         * @see #setPaint(Paint)
         * @see #draw(Canvas, Rect)
         * @see #draw(Canvas, RectF)
         */
        public Paint getPaint()
        {
            return mPaint;
        }

        /**
         * Sets the paint to use when drawing the NinePatch.
         *
         * @param p The paint that will be used to draw this NinePatch.
         *
         * @see #getPaint()
         * @see #draw(Canvas, Rect)
         * @see #draw(Canvas, RectF)
         */
        public void setPaint(Paint p)
        {
            mPaint = p;
        }

        /**
         * Returns the bitmap used to draw this NinePatch.
         */
        public Bitmap getBitmap()
        {
            return mBitmap;
        }

        /** 
         * Draws the NinePatch. This method will use the paint returned by {@link #getPaint()}.
         *
         * @param canvas A container for the current matrix and clip used to draw the NinePatch.
         * @param location Where to draw the NinePatch.
         */
        public void draw(Canvas canvas, RectF location)
        {
            canvas.DrawNinePatch(this, location, mPaint.getNativeInstance());
        }

        /** 
         * Draws the NinePatch. This method will use the paint returned by {@link #getPaint()}.
         *
         * @param canvas A container for the current matrix and clip used to draw the NinePatch.
         * @param location Where to draw the NinePatch.
         */
        public void draw(Canvas canvas, Rect location)
        {
            canvas.DrawNinePatch(this, location, mPaint.getNativeInstance());
        }

        /** 
         * Draws the NinePatch. This method will ignore the paint returned
         * by {@link #getPaint()} and use the specified paint instead.
         *
         * @param canvas A container for the current matrix and clip used to draw the NinePatch.
         * @param location Where to draw the NinePatch.
         * @param paint The Paint to draw through.
         */
        public void draw(Canvas canvas, Rect location, Paint paint)
        {
            canvas.DrawNinePatch(this, location, paint.getNativeInstance());
        }

        /**
         * Return the underlying bitmap's density, as per
         * {@link Bitmap#getDensity() Bitmap.getDensity()}.
         */
        public int getDensity()
        {
            return mBitmap.mDensity;
        }

        /**
         * Returns the intrinsic width, in pixels, of this NinePatch. This is equivalent
         * to querying the width of the underlying bitmap returned by {@link #getBitmap()}.
         */
        public int getWidth()
        {
            return mBitmap.getWidth();
        }

        /**
         * Returns the intrinsic height, in pixels, of this NinePatch. This is equivalent
         * to querying the height of the underlying bitmap returned by {@link #getBitmap()}.
         */
        public int getHeight()
        {
            return mBitmap.getHeight();
        }

        /**
         * Indicates whether this NinePatch contains transparent or translucent pixels.
         * This is equivalent to calling <code>getBitmap().hasAlpha()</code> on this
         * NinePatch.
         */
        public bool hasAlpha()
        {
            return mBitmap.hasAlpha();
        }

        /**
         * Returns a {@link Region} representing the parts of the NinePatch that are
         * completely transparent.
         *
         * @param bounds The location and size of the NinePatch.
         *
         * @return null if the NinePatch has no transparent region to
         * report, else a {@link Region} holding the parts of the specified bounds
         * that are transparent.
         */
        public Region getTransparentRegion(Rect bounds)
        {
            ArgumentNullException.ThrowIfNull(bounds);
            SKRegion r = nativeGetTransparentRegion(mBitmap.getNativeInstance(), mNativeChunk, ref bounds);
            return r != null ? new Region(r) : null;
        }

        /**
         * Verifies that the specified byte array is a valid 9-patch data chunk.
         *
         * @param chunk A byte array representing a 9-patch data chunk.
         *
         * @return True if the specified byte array represents a 9-patch data chunk,
         *         false otherwise.
         */
        public static unsafe bool isNinePatchChunk(byte[] chunk)
        {
            fixed (byte* c = chunk)
            {
                return Native.Additional.SkNinePatchGlue_isNinePatchChunk((sbyte*)c, chunk == null ? 0 : chunk.Length);
            }
        }

        /**
         * Validates the 9-patch chunk and throws an exception if the chunk is invalid.
         * If validation is successful, this method returns a native Res_png_9patch*
         * object used by the renderers.
         */
        private static unsafe sbyte* validateNinePatchChunk(byte[] chunk)
        {
            fixed (byte* c = chunk)
            {
                return Native.Additional.SkNinePatchGlue_validateNinePatchChunk((sbyte*)c, chunk.Length);
            }
        }
        private static unsafe void nativeFinalize(sbyte* chunk) => Native.Additional.SkNinePatchGlue_finalize(chunk);

        internal static byte NumXDivs(sbyte* chunk)
        {
            unsafe
            {
                byte v;
                Native.Additional.SkNinePatchGlue_getNumXDivs(chunk, &v);
                return v;
            }
        }

        internal static byte NumYDivs(sbyte* chunk)
        {
            unsafe
            {
                byte v;
                Native.Additional.SkNinePatchGlue_getNumYDivs(chunk, &v);
                return v;
            }
        }

        internal static byte NumColors(sbyte* chunk)
        {
            unsafe
            {
                byte v;
                Native.Additional.SkNinePatchGlue_getNumColors(chunk, &v);
                return v;
            }
        }

        internal static int* XDivs(sbyte* chunk)
        {
            unsafe
            {
                int* v;
                Native.Additional.SkNinePatchGlue_getXDivs(chunk, &v);
                return v;
            }
        }

        internal static int* YDivs(sbyte* chunk)
        {
            unsafe
            {
                int* v;
                Native.Additional.SkNinePatchGlue_getYDivs(chunk, &v);
                return v;
            }
        }

        internal static uint* Colors(sbyte* chunk)
        {
            unsafe
            {
                uint* v;
                Native.Additional.SkNinePatchGlue_getColors(chunk, &v);
                return v;
            }
        }

        internal static int[] XDivsAsArray(sbyte* chunk, int truncate = 0) => Arrays.FromNative<int>(XDivs(chunk), NumXDivs(chunk) - truncate);

        internal static int[] YDivsAsArray(sbyte* chunk, int truncate = 0) => Arrays.FromNative<int>(YDivs(chunk), NumYDivs(chunk) - truncate);

        internal static uint[] ColorsAsArray(sbyte* chunk, int truncate = 0) => Arrays.FromNative<uint>(Colors(chunk), NumColors(chunk) - truncate);

        internal static void SetLatticeDivs(ref SKLattice lattice, sbyte* chunk, int width, int height)
        {
            byte XCount = NumXDivs(chunk);
            byte YCount = NumYDivs(chunk);
            lattice.XDivs = XDivsAsArray(chunk);
            lattice.YDivs = XDivsAsArray(chunk);
            // We'll often see ninepatches where the last div is equal to the width or height.
            // This doesn't provide any additional information and is not supported by Skia.
            if (XCount > 0 && width == lattice.XDivs[XCount - 1])
            {
                lattice.XDivs = XDivsAsArray(chunk, 1);
            }
            if (YCount > 0 && height == lattice.YDivs[YCount - 1])
            {
                lattice.XDivs = XDivsAsArray(chunk, 1);
            }
        }

        internal static int NumDistinctRects(ref SKLattice lattice, sbyte* chunk)
        {
            int xRects;
            byte XCount = NumXDivs(chunk);
            byte YCount = NumYDivs(chunk);
            if (XCount > 0)
            {
                xRects = 0 == lattice.XDivs[0] ? XCount : XCount + 1;
            }
            else
            {
                xRects = 1;
            }

            int yRects;
            if (YCount > 0)
            {
                yRects = 0 == lattice.YDivs[0] ? YCount : YCount + 1;
            }
            else
            {
                yRects = 1;
            }
            return xRects * yRects;
        }

        internal static void SetLatticeFlags(ref SKLattice lattice, MemoryPointer<SKLatticeRectType> flags,
                                           int numFlags, sbyte* chunk, MemoryPointer<SKColor> colors)
        {
            lattice.RectTypes = (SKLatticeRectType[])flags.GetArray();
            lattice.Colors = (SKColor[])colors.GetArray();
            flags.Fill(0, numFlags);
            colors.Fill(0, numFlags);

            byte XCount = NumXDivs(chunk);
            byte YCount = NumYDivs(chunk);

            bool needPadRow = YCount > 0 && 0 == lattice.YDivs[0];
            bool needPadCol = XCount > 0 && 0 == lattice.XDivs[0];

            int yCount = YCount;
            if (needPadRow)
            {
                // Skip flags for the degenerate first row of rects.
                flags += XCount + 1;
                colors += XCount + 1;
                yCount--;
            }

            int i = 0;
            bool setFlags = false;
            uint[] colors_ = ColorsAsArray(chunk);
            for (int y = 0; y < yCount + 1; y++)
            {
                for (int x = 0; x < XCount + 1; x++)
                {
                    if (0 == x && needPadCol)
                    {
                        // First rect of each column is degenerate, skip the flag.
                        flags++;
                        colors++;
                        continue;
                    }

                    uint currentColor = colors_[i++];
                    if (0 == currentColor)
                    {
                        flags[0] = SKLatticeRectType.Transparent;
                        setFlags = true;
                    }
                    else if (1 != currentColor)
                    {
                        flags[0] = SKLatticeRectType.FixedColor;
                        colors[0] = currentColor;
                        setFlags = true;
                    }

                    flags++;
                    colors++;
                }
            }

            if (!setFlags)
            {
                lattice.RectTypes = null;
                lattice.Colors = null;
            }
        }


        private static unsafe SKRegion nativeGetTransparentRegion(SKBitmap bitmapHandle, sbyte* chunk, ref Rect location)
        {
            SKRect dst = location.ToSKRect();

            SKLattice lattice = new();
            SKRectI src = SKRectI.Create(bitmapHandle.Width, bitmapHandle.Height);
            lattice.Bounds = src;
            SetLatticeDivs(ref lattice, chunk, bitmapHandle.Width, bitmapHandle.Height);
            lattice.RectTypes = null;
            lattice.Colors = null;

            SKRegion region = null;
            if (SKLatticeIter.Valid(bitmapHandle.Width, bitmapHandle.Height, ref lattice))
            {
                SKLatticeIter iter = new(ref lattice, ref dst);
                if (iter.numRectsToDraw == NumColors(chunk))
                {
                    SKRect dummy = new();
                    SKRect iterDst = new();
                    int index = 0;
                    uint[] colors = ColorsAsArray(chunk);
                    while (iter.next(ref dummy, ref iterDst))
                    {
                        if (0 == colors[index++] && !iterDst.IsEmpty)
                        {
                            if (region == null)
                            {
                                region = new SKRegion();
                            }

                            region.Op(iterDst.Round(), SKRegionOperation.Union);
                        }
                    }
                }
            }
            return region;
        }
    }
}