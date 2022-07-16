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
using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;
using AndroidUI.Utils.Skia;
using SkiaSharp;

namespace AndroidUI.Skia
{
    /**
     *  Disect a lattice request into an sequence of src-rect / dst-rect pairs
     */
    public class SKLatticeIter
    {
        /**
         *  Divs must be in increasing order with no duplicates.
         */
        static bool valid_divs(MemoryPointer<int> divs, int divCount, int start, int end)
        {
            int prev = start - 1;
            for (int i = 0; i < divCount; i++)
            {
                if (prev >= divs[i] || divs[i] >= end)
                {
                    return false;
                }
                prev = divs[i];
            }

            return true;
        }

        public static bool Valid(int imageWidth, int imageHeight, ref SKLattice lattice)
        {
            SKRectI totalBounds = SKRectI.Create(imageWidth, imageHeight);

            if (!lattice.Bounds.HasValue)
            {
                throw new Exceptions.IllegalStateException("lattive bounds must contain a value");
            }

            SKRectI latticeBounds = lattice.Bounds.Value;
            if (!totalBounds.Contains(latticeBounds))
            {
                return false;
            }

            int[] xDivs = lattice.XDivs;
            int[] yDivs = lattice.YDivs;

            bool zeroXDivs = xDivs.Length <= 0 || 1 == xDivs.Length && latticeBounds.Left == xDivs[0];
            bool zeroYDivs = yDivs.Length <= 0 || 1 == yDivs.Length && latticeBounds.Top == yDivs[0];

            if (zeroXDivs && zeroYDivs)
            {
                return false;
            }

            return valid_divs(xDivs, xDivs.Length, latticeBounds.Left, latticeBounds.Right)
                && valid_divs(yDivs, yDivs.Length, latticeBounds.Top, latticeBounds.Bottom);

        }

        /**
         *  Count the number of pixels that are in "scalable" patches.
         */
        static int count_scalable_pixels(MemoryPointer<int> divs, int divCount, bool firstIsScalable, int start, int end)
        {
            if (0 == divCount)
            {
                return firstIsScalable ? end - start : 0;
            }

            int i;
            int count;
            if (firstIsScalable)
            {
                count = divs[0] - start;
                i = 1;
            }
            else
            {
                count = 0;
                i = 0;
            }

            for (; i < divCount; i += 2)
            {
                // Alternatively, we could use |top| and |bottom| as variable names, instead of
                // |left| and |right|.
                int left = divs[i];
                int right = i + 1 < divCount ? divs[i + 1] : end;
                count += right - left;
            }

            return count;
        }

        /**
            *  Set points for the src and dst rects on subsequent draw calls.
            */
        static void set_points(MemoryPointer<float> dst, MemoryPointer<int> src, MemoryPointer<int> divs, int divCount, int srcFixed,
                                int srcScalable, int srcStart, int srcEnd, float dstStart, float dstEnd,
                                bool isScalable)
        {
            float dstLen = dstEnd - dstStart;
            float scale;
            if (srcFixed <= dstLen)
            {
                // This is the "normal" case, where we scale the "scalable" patches and leave
                // the other patches fixed.
                scale = (dstLen - srcFixed) / srcScalable;
            }
            else
            {
                // In this case, we eliminate the "scalable" patches and scale the "fixed" patches.
                scale = dstLen / srcFixed;
            }

            src[0] = srcStart;
            dst[0] = dstStart;
            for (int i = 0; i < divCount; i++)
            {
                src[i + 1] = divs[i];
                int srcDelta = src[i + 1] - src[i];
                float dstDelta;
                if (srcFixed <= dstLen)
                {
                    dstDelta = isScalable ? scale * srcDelta : srcDelta;
                }
                else
                {
                    dstDelta = isScalable ? 0.0f : scale * srcDelta;
                }
                dst[i + 1] = dst[i] + dstDelta;

                // Alternate between "scalable" and "fixed" patches.
                isScalable = !isScalable;
            }

            src[divCount + 1] = srcEnd;
            dst[divCount + 1] = dstEnd;
        }

        public SKLatticeIter(ref SKLattice lattice, ref SKRect dst)
        {
            MemoryPointer<int> xDivs = lattice.XDivs;
            int origXCount = xDivs.Length;
            MemoryPointer<int> yDivs = lattice.YDivs;
            int origYCount = yDivs.Length;
            if (!lattice.Bounds.HasValue)
            {
                throw new Exceptions.IllegalStateException("lattive bounds must contain a value");
            }

            SKRectI src = lattice.Bounds.Value;

            // In the x-dimension, the first rectangle always starts at x = 0 and is "scalable".
            // If xDiv[0] is 0, it indicates that the first rectangle is degenerate, so the
            // first real rectangle "scalable" in the x-direction.
            //
            // The same interpretation applies to the y-dimension.
            //
            // As we move left to right across the image, alternating patches will be "fixed" or
            // "scalable" in the x-direction.  Similarly, as move top to bottom, alternating
            // patches will be "fixed" or "scalable" in the y-direction.
            int xCount = origXCount;
            int yCount = origYCount;
            bool xIsScalable = xCount > 0 && src.Left == xDivs[0];
            if (xIsScalable)
            {
                // Once we've decided that the first patch is "scalable", we don't need the
                // xDiv.  It is always implied that we start at the edge of the bounds.
                xDivs++;
                xCount--;
            }
            bool yIsScalable = yCount > 0 && src.Top == yDivs[0];
            if (yIsScalable)
            {
                // Once we've decided that the first patch is "scalable", we don't need the
                // yDiv.  It is always implied that we start at the edge of the bounds.
                yDivs++;
                yCount--;
            }

            // Count "scalable" and "fixed" pixels in each dimension.
            int xCountScalable = count_scalable_pixels(xDivs, xCount, xIsScalable, src.Left, src.Right);
            int xCountFixed = src.Width - xCountScalable;
            int yCountScalable = count_scalable_pixels(yDivs, yCount, yIsScalable, src.Top, src.Bottom);
            int yCountFixed = src.Height - yCountScalable;
            fSrcX = new int[xCount + 2];
            fDstX = new float[xCount + 2];
            set_points(fDstX, fSrcX, xDivs, xCount, xCountFixed, xCountScalable,
                       src.Left, src.Right, dst.Left, dst.Right, xIsScalable);

            fSrcY = new int[yCount + 2];
            fDstY = new float[yCount + 2];
            set_points(fDstY, fSrcY, yDivs, yCount, yCountFixed, yCountScalable,
                       src.Top, src.Bottom, dst.Top, dst.Bottom, yIsScalable);

            fCurrX = fCurrY = 0;
            fNumRectsInLattice = (xCount + 1) * (yCount + 1);
            fNumRectsToDraw = fNumRectsInLattice;

            if (lattice.RectTypes != null && lattice.RectTypes.Length != 0)
            {
                fRectTypes.Add((SKLatticeRectType)fNumRectsInLattice);
                fColors.Add(new SKColor((uint)fNumRectsInLattice));

                MemoryPointer<SKLatticeRectType> flags = lattice.RectTypes;
                MemoryPointer<SKColor> colors = lattice.Colors;

                bool hasPadRow = yCount != origYCount;
                bool hasPadCol = xCount != origXCount;
                if (hasPadRow)
                {
                    // The first row of rects are all empty, skip the first row of flags.
                    flags += origXCount + 1;
                    colors += origXCount + 1;
                }

                int i = 0;
                for (int y = 0; y < yCount + 1; y++)
                {
                    for (int x = 0; x < origXCount + 1; x++)
                    {
                        if (0 == x && hasPadCol)
                        {
                            // The first column of rects are all empty.  Skip a rect.
                            flags++;
                            colors++;
                            continue;
                        }

                        fRectTypes[i] = flags[0];
                        fColors[i] = SKLatticeRectType.FixedColor == flags[0] ? colors[0] : 0;
                        flags++;
                        colors++;
                        i++;
                    }
                }

                for (int j = 0; j < fRectTypes.Count; j++)
                {
                    if (SKLatticeRectType.Transparent == fRectTypes[j])
                    {
                        fNumRectsToDraw--;
                    }
                }
            }
        }

        public static bool Valid(int imageWidth, int imageHeight, ref SKRectI center)
        {
            return !center.IsEmpty && SKRectI.Create(imageWidth, imageHeight).Contains(center);
        }

        public SKLatticeIter(int imageWidth, int imageHeight, ref SKRectI center, ref SKRect dst)
        {
            if (!SKRectI.Create(imageWidth, imageHeight).Contains(center))
            {
                throw new Exceptions.IllegalArgumentException("Center must be contained inside given width and height");
            }

            fSrcX = new int[4];
            fSrcY = new int[4];
            fDstX = new float[4];
            fDstY = new float[4];

            fSrcX[0] = 0;
            fSrcX[1] = center.Left;
            fSrcX[2] = center.Right;
            fSrcX[3] = imageWidth;

            fSrcY[0] = 0;
            fSrcY[1] = center.Top;
            fSrcY[2] = center.Bottom;
            fSrcY[3] = imageHeight;

            fDstX[0] = dst.Left;
            fDstX[1] = dst.Left + SKUtils.SkIntToScalar(center.Left);
            fDstX[2] = dst.Right - SKUtils.SkIntToScalar(imageWidth - center.Right);
            fDstX[3] = dst.Right;

            fDstY[0] = dst.Top;
            fDstY[1] = dst.Top + SKUtils.SkIntToScalar(center.Top);
            fDstY[2] = dst.Bottom - SKUtils.SkIntToScalar(imageHeight - center.Bottom);
            fDstY[3] = dst.Bottom;

            if (fDstX[1] > fDstX[2])
            {
                fDstX[1] = fDstX[0] + (fDstX[3] - fDstX[0]) * center.Left / (imageWidth - center.Width);
                fDstX[2] = fDstX[1];
            }

            if (fDstY[1] > fDstY[2])
            {
                fDstY[1] = fDstY[0] + (fDstY[3] - fDstY[0]) * center.Top / (imageHeight - center.Height);
                fDstY[2] = fDstY[1];
            }

            fCurrX = fCurrY = 0;
            fNumRectsInLattice = 9;
            fNumRectsToDraw = 9;
        }

        /**
         *  While it returns true, use src/dst to draw the image/bitmap. Optional parameters
         *  isFixedColor and fixedColor specify if the rectangle is filled with a fixed color.
         *  If (*isFixedColor) is true, then (*fixedColor) contains the rectangle color.
         */
        public bool next(ref SKRectI src, ref SKRect dst, bool isFixedColor = false, ValueHolder<SKColor> fixedColor = null)
        {
            int currRect = fCurrX + fCurrY * (fSrcX.Length - 1);
            if (currRect == fNumRectsInLattice)
            {
                return false;
            }

            int x = fCurrX;
            int y = fCurrY;
            if (!(x >= 0 && x < fSrcX.Length - 1))
            {
                throw new Exception("Assertion failed: x >= 0 && x < fSrcX.Length - 1)");
            }
            if (!(y >= 0 && y < fSrcY.Length - 1))
            {
                throw new Exception("Assertion failed: y >= 0 && y < fSrcY.Length - 1)");
            }

            if (fSrcX.Length - 1 == ++fCurrX)
            {
                fCurrX = 0;
                fCurrY += 1;
            }

            if (fRectTypes.Count > 0 && SKLatticeRectType.Transparent == fRectTypes[currRect])
            {
                return next(ref src, ref dst, isFixedColor, fixedColor);
            }

            src.SetLTRB(fSrcX[x], fSrcY[y], fSrcX[x + 1], fSrcY[y + 1]);
            dst.SetLTRB(fDstX[x], fDstY[y], fDstX[x + 1], fDstY[y + 1]);
            if (isFixedColor && fixedColor != null)
            {
                isFixedColor = fRectTypes.Count > 0 && SKLatticeRectType.FixedColor == fRectTypes[currRect];
                if (isFixedColor)
                {
                    fixedColor = fColors[currRect];
                }
            }
            return true;
        }

        /** Version of above that converts the integer src rect to a scalar rect. */
        public bool next(ref SKRect src, ref SKRect dst, bool isFixedColor = false, ValueHolder<SKColor> fixedColor = null)
        {
            SKRectI isrcR = new();
            if (next(ref isrcR, ref dst, isFixedColor, fixedColor))
            {
                src = isrcR.ToSKRect();
                return true;
            }
            return false;
        }

        /**
        *  Apply a matrix to the dst points.
        */
        public void mapDstScaleTranslate(ref SKMatrix matrix)
        {
            Console.WriteLine("WARNING: mapDstScaleTranslate - matrix.IsScaleTranslate() is not yet ported, skipping check");
            //if (!matrix.IsScaleTranslate()) {
            //    throw new Exceptions.IllegalArgumentException("matrix must be scale translate");
            //}
            float tx = matrix.TransX;
            float sx = matrix.ScaleX;
            for (int i = 0; i < fDstX.Length; i++)
            {
                fDstX[i] = fDstX[i] * sx + tx;
            }

            float ty = matrix.TransY;
            float sy = matrix.ScaleY;
            for (int i = 0; i < fDstY.Length; i++)
            {
                fDstY[i] = fDstY[i] * sy + ty;
            }
        }

        /**
         *  Returns the number of rects that will actually be drawn.
         */
        public int numRectsToDraw => fNumRectsToDraw;

        int[] fSrcX;
        int[] fSrcY;
        float[] fDstX;
        float[] fDstY;
        List<SKLatticeRectType> fRectTypes;
        List<SKColor> fColors;

        int fCurrX;
        int fCurrY;
        int fNumRectsInLattice;
        int fNumRectsToDraw;
    };
}