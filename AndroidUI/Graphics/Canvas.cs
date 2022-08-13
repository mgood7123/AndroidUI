using AndroidUI.Applications;
using AndroidUI.Extensions;
using AndroidUI.Utils;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public class Canvas : BaseCanvas
    {
        public Canvas()
        {
        }

        public Canvas(Context context, SKCanvas canvas) : base(context, canvas)
        {
        }

        /// <summary>Draws a canvas on this canvas.</summary>
        public void DrawCanvas(BaseCanvas canvas, SKPaint paint = null) => DrawToCanvas(canvas, 0, 0, paint);

        /// <summary>Draws a canvas on this canvas.</summary>
        public void DrawCanvas(BaseCanvas canvas, int x, int y, SKPaint paint = null) => DrawToCanvas(canvas, x, y, paint);

        /// <summary>Draws a canvas on this canvas.</summary>
        public void DrawCanvas(BaseCanvas canvas, SKPoint point, SKPaint paint = null) => DrawToCanvas(canvas, point, paint);

        /// <summary>Draws this canvas to a canvas.</summary>
        public void DrawToCanvas(BaseCanvas canvas, SKPaint paint = null) => DrawToCanvas(canvas, 0, 0, paint);

        /// <summary>Draws this canvas to a canvas.</summary>
        public virtual void DrawToCanvas(BaseCanvas canvas, int x, int y, SKPaint paint = null)
        {
            if (!isHardwareAccelerated())
            {
                Log.e("SKCanvas", "this canvas is not hardware accelerated, DrawToCanvas does not support a suftware canvas");
                return;
            }
            if (surface == null)
            {
                Log.e("SKCanvas", "this canvas is hardware accelerated but it has no surface");
                return;
            }
            canvas.DrawSurface(surface, x, y, paint);
        }

        /// <summary>Draws this canvas to a canvas.</summary>
        public virtual void DrawToCanvas(BaseCanvas canvas, SKPoint point, SKPaint paint = null)
        {
            if (!isHardwareAccelerated())
            {
                Log.e("SKCanvas", "this canvas is not hardware accelerated, DrawToCanvas does not support a suftware canvas");
                return;
            }
            if (surface == null)
            {
                Log.e("SKCanvas", "this canvas is hardware accelerated but it has no surface");
                return;
            }
            canvas.DrawSurface(surface, point, paint);
        }

        public SKRectI GetDeviceClipBounds() => DeviceClipBounds;

        public SKRect GetLocalClipBounds() => LocalClipBounds;

        //static int only_axis_aligned_saveBehind(SKRect bounds)
        //{
        //	if (!GetLocalClipBounds().IntersectsWith(bounds))
        //	{
        //		// Assuming clips never expand, if the request bounds is outside of the current clip
        //		// there is no need to copy/restore the area, so just devolve back to a regular save.
        //		Save();
        //	}
        //	else
        //          {
        //		bool doTheWork = false;
        //		internalSave();
        //		if (doTheWork)
        //		{
        //			internalSaveBehind(bounds);
        //		}
        //	}
        //	return SaveCount - 1;
        //}

        //static int SaveBehind(SKRect subset)
        //      {
        //	return only_axis_aligned_saveBehind(subset);
        //}


        //public int saveUnclippedLayer(int left, int top, int right, int bottom)
        //{
        //    SKRect bounds = SKRect.Create(left, top, right, bottom);
        //    return SaveBehind(bounds);
        //}

        //public void restoreUnclippedLayer(int restoreCount, SKPaint paint)
        //{

        //    while (SaveCount > restoreCount + 1)
        //    {
        //        Restore();
        //    }

        //    if (getSaveCount() == restoreCount + 1) {
        //        SkCanvasPriv::DrawBehind(filterPaint(paint));
        //        restore();
        //    }
        //}

        public void DrawRectLTRB(float left, float top, float right, float bottom, SKPaint paint)
        {
            DrawRect(SKRect.Create(left, top, right - left, bottom - top), paint);
        }

        public void ClipRect(float left, float top, float right, float bottom)
        {
            ClipRect(new SKRect(left, top, right, bottom));
        }

        /**
		 * Draw a series of lines. Each line is taken from 4 consecutive values in the pts array. Thus
		 * to draw 1 line, the array must contain at least 4 values. This is logically the same as
		 * drawing the array as follows: drawLine(pts[0], pts[1], pts[2], pts[3]) followed by
		 * drawLine(pts[4], pts[5], pts[6], pts[7]) and so on.
		 *
		 * @param pts Array of points to draw [x0 y0 x1 y1 x2 y2 ...]
		 * @param offset Number of values in the array to skip before drawing.
		 * @param count The number of values in the array to process, after skipping "offset" of them.
		 *            Since each line uses 4 values, the number of "lines" that are drawn is really
		 *            (count >> 2).
		 * @param paint The paint used to draw the points
		 */
        public void DrawLines(float[] points, int offset, int count, SKPaint paint)
        {
            if ((offset | count) < 0 || count < 2 || offset + count > points.Length)
            {
                return;
            }

            int countDoubled = count * 2;

            // convert the floats into SkPoints
            count >>= 1;  // now it is the number of points

            SKPoint[] pts = new SKPoint[count];

            int pi = offset;
            for (int i = 0; i < count; i++)
            {
                if (pi >= countDoubled) break;

                pts[i].Set(points[pi + 0] + 0.5f, points[pi + 1] + 0.5f);

                pi += 2;
            }

            DrawPoints(SKPointMode.Lines, pts, paint);
        }

        public void DrawLines(float[] points, SKPaint paint)
        {
            DrawLines(points, 0, points.Length, paint);
        }

        void throwIfCannotDraw(Bitmap bitmap)
        {
            if (bitmap.isRecycled())
            {
                throw new Exception("Canvas: trying to use a recycled bitmap " + bitmap);
            }
            if (!bitmap.isPremultiplied() && bitmap.getConfig() == Bitmap.Config.ARGB_8888 &&
                    bitmap.hasAlpha())
            {
                Log.w("AndroidUI", "Canvas: trying to use a non-premultiplied bitmap "
                        + bitmap);
            }
        }

        public unsafe void DrawNinePatch(NinePatch patch, Rect dst, SKPaint paint)
        {
            Bitmap bitmap = patch.getBitmap();
            throwIfCannotDraw(bitmap);
            DrawNinePatch(bitmap.getNativeInstance(), patch.mNativeChunk,
                    dst.left, dst.top, dst.right, dst.bottom, paint,
                    Bitmap.DENSITY_NONE, patch.getDensity());
        }

        public unsafe void DrawNinePatch(NinePatch patch, RectF dst, SKPaint paint)
        {
            Bitmap bitmap = patch.getBitmap();
            throwIfCannotDraw(bitmap);
            DrawNinePatch(bitmap.getNativeInstance(), patch.mNativeChunk,
                    dst.left, dst.top, dst.right, dst.bottom, paint,
                    Bitmap.DENSITY_NONE, patch.getDensity());
        }

        public unsafe void DrawNinePatch(
            SKBitmap bitmap, sbyte* nativeChunk,
            float left, float top, float right, float bottom,
            SKPaint paint, int dstDensity, int srcDensity
        )
        {
            if (dstDensity == srcDensity || dstDensity == 0 || srcDensity == 0)
            {
                DrawNinePatch(bitmap, nativeChunk, left, top, right, bottom, paint);
            }
            else
            {
                Save();
                float scale = dstDensity / (float)srcDensity;
                Translate(left, top);
                Scale(scale, scale);

                SKPaint filteredPaint = paint == null ? new SKPaint() : paint;
                filteredPaint.FilterQuality = SKFilterQuality.Low;

                DrawNinePatch(bitmap, nativeChunk, 0, 0, (right - left) / scale, (bottom - top) / scale, filteredPaint);
                Restore();
            }
        }

        public unsafe void DrawNinePatch(
            SKBitmap bitmap, sbyte* chunk,
            float dstLeft, float dstTop, float dstRight, float dstBottom,
            SKPaint paint
        )
        {
            SKLattice lattice = new();
            NinePatch.SetLatticeDivs(ref lattice, chunk, bitmap.Width, bitmap.Height);

            lattice.RectTypes = null;
            lattice.Colors = null;
            int numFlags = 0;
            int numColors = NinePatch.NumColors(chunk);
            byte XCount = NinePatch.NumXDivs(chunk);
            byte YCount = NinePatch.NumYDivs(chunk);
            if (numColors > 0 && numColors == NinePatch.NumDistinctRects(ref lattice, chunk))
            {
                // We can expect the framework to give us a color for every distinct rect.
                // Skia requires a flag for every rect.
                numFlags = (XCount + 1) * (YCount + 1);
            }

            SKLatticeRectType[] flags = new SKLatticeRectType[numFlags];
            SKColor[] colors = new SKColor[numColors];
            if (numFlags > 0)
            {
                NinePatch.SetLatticeFlags(ref lattice, flags, numFlags, chunk, colors);
            }

            lattice.Bounds = null;
            SKRect dst = SKRect.Create(dstLeft, dstTop, dstRight, dstBottom);
            var image = bitmap.AsImage();
            DrawImageLattice(image, lattice, dst, paint);
        }


        public void DrawBitmap(Bitmap bitmap, float left, float top, Paint paint)
        {
            throwIfCannotDraw(bitmap);
            drawBitmap(
                bitmap.getNativeInstance(), left, top,
                paint?.getNativeInstance(), DensityDPI, 
                context.densityManager.ScreenDpi, bitmap.mDensity
            );
        }

        public void DrawBitmap(Bitmap bitmap, SKMatrix matrix, Paint paint)
        {
            drawBitmapMatrix(
                bitmap.getNativeInstance(), ref matrix, paint?.getNativeInstance()
            );
        }

        public void DrawBitmap(Bitmap bitmap, Rect src, Rect dst, Paint paint)
        {
            if (dst == null)
            {
                throw new ArgumentNullException(nameof(dst));
            }
            throwIfCannotDraw(bitmap);
            SKPaint nativePaint = paint?.getNativeInstance();

            int left, top, right, bottom;
            if (src == null)
            {
                left = top = 0;
                right = bitmap.getWidth();
                bottom = bitmap.getHeight();
            }
            else
            {
                left = src.left;
                right = src.right;
                top = src.top;
                bottom = src.bottom;
            }

            drawBitmapRect(
                bitmap.getNativeInstance(), left, top, right, bottom,
                dst.left, dst.top, dst.right, dst.bottom, nativePaint,
                context.densityManager.ScreenDpi, bitmap.mDensity
            );
        }

        public void DrawBitmap(Bitmap bitmap, Rect src, RectF dst, Paint paint)
        {
            if (dst == null)
            {
                throw new ArgumentNullException(nameof(dst));
            }
            throwIfCannotDraw(bitmap);
            SKPaint nativePaint = paint?.getNativeInstance();

            float left, top, right, bottom;
            if (src == null)
            {
                left = top = 0;
                right = bitmap.getWidth();
                bottom = bitmap.getHeight();
            }
            else
            {
                left = src.left;
                right = src.right;
                top = src.top;
                bottom = src.bottom;
            }

            drawBitmapRect(
                bitmap.getNativeInstance(), left, top, right, bottom,
                dst.left, dst.top, dst.right, dst.bottom, nativePaint,
                context.densityManager.ScreenDpi, bitmap.mDensity
            );
        }

        void drawBitmap(
            SKBitmap bitmap,
            float left, float top, SKPaint paint, int canvasDensity,
            int screenDensity, int bitmapDensity
        )
        {

            if (canvasDensity == bitmapDensity || canvasDensity == 0 || bitmapDensity == 0)
            {
                if (screenDensity != 0 && screenDensity != bitmapDensity)
                {
                    SKPaint filteredPaint = new();
                    if (paint != null)
                    {
                        filteredPaint = paint;
                    }
                    filteredPaint.FilterQuality = SKFilterQuality.Low;
                    DrawImage(bitmap.AsImage(), left, top, filteredPaint);
                }
                else
                {
                    DrawImage(bitmap.AsImage(), left, top, paint);
                }
            }
            else
            {
                Save();
                float scale = canvasDensity / (float)bitmapDensity;
                Translate(left, top);
                Scale(scale, scale);

                SKPaint filteredPaint = new();
                if (paint != null)
                {
                    filteredPaint = paint;
                }
                filteredPaint.FilterQuality = SKFilterQuality.Low;

                DrawImage(bitmap.AsImage(), 0, 0, filteredPaint);
                Restore();
            }
        }

        void drawBitmap(SKBitmap bitmap, ref SKMatrix matrix, SKPaint paint)
        {
            var image = bitmap.AsImage();
            using AutoCanvasRestoreWrapper acr = new(this, true);
            Concat(ref matrix);
            DrawImage(image, 0, 0, paint);
        }


        void drawBitmapMatrix(SKBitmap bitmap,
                                     ref SKMatrix matrix, SKPaint paint)
        {
            drawBitmap(bitmap, ref matrix, paint);
        }

        void drawBitmap(
            SKBitmap bitmap,
            float srcLeft, float srcTop, float srcRight,
            float srcBottom, float dstLeft, float dstTop, float dstRight,
            float dstBottom, SKPaint paint
        )
        {
            var image = bitmap.AsImage();
            SKRect srcRect = SKRect.Create(srcLeft, srcTop, srcRight, srcBottom);
            SKRect dstRect = SKRect.Create(dstLeft, dstTop, dstRight, dstBottom);
            DrawImage(image, srcRect, dstRect, paint);
        }

        void drawBitmapRect(
            SKBitmap bitmap,
            float srcLeft, float srcTop, float srcRight, float srcBottom,
            float dstLeft, float dstTop, float dstRight, float dstBottom,
            SKPaint paint, int screenDensity, int bitmapDensity
        )
        {
            if (screenDensity != 0 && screenDensity != bitmapDensity)
            {
                SKPaint filteredPaint = new();
                if (paint != null)
                {
                    filteredPaint = paint;
                }
                filteredPaint.FilterQuality = SKFilterQuality.Low;
                drawBitmap(bitmap, srcLeft, srcTop, srcRight, srcBottom,
                                   dstLeft, dstTop, dstRight, dstBottom, filteredPaint);
            }
            else
            {
                drawBitmap(bitmap, srcLeft, srcTop, srcRight, srcBottom,
                                   dstLeft, dstTop, dstRight, dstBottom, paint);
            }
        }

        // TODO: investigate if this would be useful
        void drawBitmapArray(
            int[] jcolors, int offset, int stride,
            float x, float y, int width, int height,
            bool hasAlpha, SKPaint paint
        )
        {
            // Note: If hasAlpha is false, kRGB_565_SkColorType will be used, which will
            // correct the alphaType to kOpaque_SkAlphaType.
            SKImageInfo info = new SKImageInfo(
                width, height,
                hasAlpha ? SKImageInfo.PlatformColorType : SKColorType.Rgb565,
                SKAlphaType.Premul
            );
            using SKBitmap bitmap = new();
            bitmap.SetInfo(info);
            using SKBitmap androidBitmap = BitmapFactory.allocateHeapBitmap(bitmap);
            if (androidBitmap != null)
            {
                return;
            }

            if (!BitmapFactory.SetPixels(jcolors, offset, stride, 0, 0, width, height, bitmap))
            {
                return;
            }

            DrawImage(androidBitmap.AsImage(), x, y, paint);
        }
    }
}