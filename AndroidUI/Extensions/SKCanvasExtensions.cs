using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKCanvasExtensions
    {
        internal static void DisposeSurface(this SKCanvas this_canvas)
        {
            SKSurface s = (SKSurface)this_canvas.ExtensionProperties_GetValue("Surface", null);
            if (s != null)
            {
                s.Dispose();
            }
        }

        internal static void setWidthHeight(this SKCanvas this_canvas, int width, int height)
        {
            this_canvas.ExtensionProperties_SetValue("Width", width);
            this_canvas.ExtensionProperties_SetValue("Height", height);
        }

        /// <summary>Experimental.</summary>
        public static int densityDpi(this SKCanvas this_canvas)
        {
            return (int)this_canvas.ExtensionProperties_GetValue("DensityDpi", DensityManager.ScreenDpi);
        }

        /// <summary>Experimental.</summary>
        public static void setDensityDpi(this SKCanvas this_canvas, int value)
        {
            this_canvas.ExtensionProperties_SetValue("DensityDpi", value);
        }

        /// <summary>Returns true if this canvas is Hardware Accelerated.</summary>
        public static bool isHardwareAccelerated(this SKCanvas this_canvas)
        {
            return (bool)this_canvas.ExtensionProperties_GetValue("HardwareAccelerated", false);
        }

        internal static void setIsHardwareAccelerated(this SKCanvas this_canvas, bool value)
        {
            this_canvas.ExtensionProperties_SetValue("HardwareAccelerated", value);
        }

        /// <summary>Creates a Hardware Accelerated canvas.</summary>
        public static SKCanvas CreateHardwareAcceleratedCanvas(this SKCanvas this_canvas, GRContext context, int width, int height)
        {
            SKSurface s = SKSurface.Create(context, false, new SKImageInfo(width, height));
            SKCanvas c = s.Canvas;
            c.ExtensionProperties_SetValue("GRContext", context);
            c.setWidthHeight(width, height);
            c.ExtensionProperties_SetValue("Surface", s);
            c.ExtensionProperties_SetValue("HardwareAccelerated", true);
            return c;
        }

        /// <summary>Creates a software canvas, this is not Hardware Accelerated.</summary>
        public static SKCanvas CreateSoftwareCanvas(this SKCanvas this_canvas, int width, int height)
        {
            SKCanvas c = new(new SKBitmap(width, height));
            c.setWidthHeight(width, height);
            return c;
        }

        /// <summary>Creates a canvas, whether the created canvas is Hardware Accelerated or not depends
        // on the canvas of which this method is invoked from.</summary>
        public static SKCanvas CreateCanvas(this SKCanvas this_canvas, int width, int height)
        {
            if (this_canvas.isHardwareAccelerated())
            {
                GRContext context = (GRContext)this_canvas.ExtensionProperties_GetValue("GRContext", null);
                if (context == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no GRContext");
                return this_canvas.CreateHardwareAcceleratedCanvas(context, width, height);
            }
            else
            {
                return this_canvas.CreateSoftwareCanvas(width, height);
            }
        }

        public static int getWidth(this SKCanvas this_canvas)
        {
            return (int)this_canvas.ExtensionProperties_GetValue("Width", 0);
        }

        public static int getHeight(this SKCanvas this_canvas)
        {
            return (int)this_canvas.ExtensionProperties_GetValue("Height", 0);
        }

        /// <summary>Draws a canvas on this canvas.</summary>
        public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, 0, 0, paint);

        /// <summary>Draws a canvas on this canvas.</summary>
        public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, int x, int y, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, x, y, paint);

        /// <summary>Draws a canvas on this canvas.</summary>
        public static void DrawCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPoint point, SKPaint paint = null) => canvas.DrawToCanvas(this_canvas, point, paint);

        /// <summary>Draws this canvas to a canvas.</summary>
        public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPaint paint = null) => this_canvas.DrawToCanvas(canvas, 0, 0, paint);

        /// <summary>Draws this canvas to a canvas.</summary>
        public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, int x, int y, SKPaint paint = null)
        {
            if (this_canvas.isHardwareAccelerated())
            {
                SKSurface s = (SKSurface)this_canvas.ExtensionProperties_GetValue("Surface", null);
                if (s == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no surface");
                canvas.DrawSurface(s, x, y, paint);
            }
        }

        /// <summary>Draws this canvas to a canvas.</summary>
        public static void DrawToCanvas(this SKCanvas this_canvas, SKCanvas canvas, SKPoint point, SKPaint paint = null)
        {
            if (this_canvas.isHardwareAccelerated())
            {
                SKSurface s = (SKSurface)this_canvas.ExtensionProperties_GetValue("Surface", null);
                if (s == null) throw new NullReferenceException("the canvas is hardware accelerated but it has no surface");
                canvas.DrawSurface(s, point, paint);
            }
        }

        public static SKRectI GetDeviceClipBounds(this SKCanvas canvas)
        {
            SKRectI bounds;
            canvas.GetDeviceClipBounds(out bounds);
            return bounds;
        }

        public static SKRect GetLocalClipBounds(this SKCanvas canvas)
        {
            SKRect bounds;
            canvas.GetLocalClipBounds(out bounds);
            return bounds;
        }

        //static int only_axis_aligned_saveBehind(SKCanvas canvas, SKRect bounds)
        //{
        //	if (!canvas.GetLocalClipBounds().IntersectsWith(bounds))
        //	{
        //		// Assuming clips never expand, if the request bounds is outside of the current clip
        //		// there is no need to copy/restore the area, so just devolve back to a regular save.
        //		canvas.Save();
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
        //	return canvas.SaveCount - 1;
        //}

        //static int SaveBehind(SKCanvas canvas, SKRect subset)
        //      {
        //	return only_axis_aligned_saveBehind(canvas, subset);
        //}


        public static int saveUnclippedLayer(this SKCanvas canvas, int left, int top, int right, int bottom)
        {
            return canvas.Save();
            //	SKRect bounds = SKRect.Create(left, top, right, bottom);
            //	return SaveBehind(this_canvas, bounds);
        }

        public static void restoreUnclippedLayer(this SKCanvas canvas, int restoreCount, SKPaint paint)
        {

            while (canvas.SaveCount > restoreCount + 1)
            {
                canvas.Restore();
            }

            //	if (mcanvasHandle.getSaveCount() == restoreCount + 1) {
            //		SkCanvasPriv::DrawBehind(mCanvas, filterPaint(paint));

            //		this->restore();

            //	}
        }

        public static void ClipRect(this SKCanvas canvas, float left, float top, float right, float bottom)
        {
            canvas.ClipRect(new SKRect(left, top, right, bottom));
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
        public static void DrawLines(this SKCanvas canvas, float[] points, int offset, int count, SKPaint paint)
        {
            if ((offset | count) < 0 || count < 2 || offset + count > points.Length)
            {
                return;
            }

            int countDoubled = count * 2;

            // convert the floats into SkPoints
            count >>= 1;  // now it is the number of points

            SKPoint[] pts = (new SKPoint[count]);

            int pi = offset;
            for (int i = 0; i < count; i++)
            {
                if (pi >= countDoubled) break;

                pts[i].Set(points[pi + 0] + 0.5f, points[pi + 1] + 0.5f);

                pi += 2;
            }

            canvas.DrawPoints(SKPointMode.Lines, pts, paint);
        }

        public static void DrawLines(this SKCanvas canvas, float[] points, SKPaint paint)
        {
            canvas.DrawLines(points, 0, points.Length, paint);
        }

        public static void DrawRectCoords(this SKCanvas canvas, float x1, float y1, float x2, float y2, SKPaint paint)
        {
            // avoid allocating a new SKRect
            canvas.DrawRect(x1, y1, x2 - x1, y2 - y1, paint);
        }

        static void throwIfCannotDraw(Bitmap bitmap)
        {
            if (bitmap.isRecycled())
            {
                throw new Exception("Canvas: trying to use a recycled bitmap " + bitmap);
            }
            if (!bitmap.isPremultiplied() && bitmap.getConfig() == Bitmap.Config.ARGB_8888 &&
                    bitmap.hasAlpha())
            {
                throw new Exception("Canvas: trying to use a non-premultiplied bitmap "
                        + bitmap);
            }
        }

        public static unsafe void DrawPatch(this SKCanvas canvas, NinePatch patch, Rect dst, SKPaint paint)
        {
            Bitmap bitmap = patch.getBitmap();
            throwIfCannotDraw(bitmap);
            canvas.DrawNinePatch(bitmap.getNativeInstance(), patch.mNativeChunk,
                    dst.left, dst.top, dst.right, dst.bottom, paint,
                    Bitmap.DENSITY_NONE, patch.getDensity());
        }

        public static unsafe void DrawPatch(this SKCanvas canvas, NinePatch patch, RectF dst, SKPaint paint)
        {
            Bitmap bitmap = patch.getBitmap();
            throwIfCannotDraw(bitmap);
            canvas.DrawNinePatch(bitmap.getNativeInstance(), patch.mNativeChunk,
                    dst.left, dst.top, dst.right, dst.bottom, paint,
                    Bitmap.DENSITY_NONE, patch.getDensity());
        }

        public static unsafe void DrawNinePatch(
            this SKCanvas canvas, SKBitmap bitmap, sbyte* nativeChunk,
            float left, float top, float right, float bottom,
            SKPaint paint, int dstDensity, int srcDensity
        )
        {
            if (dstDensity == srcDensity || dstDensity == 0 || srcDensity == 0)
            {
                canvas.DrawNinePatch(bitmap, nativeChunk, left, top, right, bottom, paint);
            }
            else
            {
                canvas.Save();
                float scale = dstDensity / (float)srcDensity;
                canvas.Translate(left, top);
                canvas.Scale(scale, scale);

                SKPaint filteredPaint = paint == null ? new SKPaint() : paint;
                filteredPaint.FilterQuality = SKFilterQuality.Low;

                canvas.DrawNinePatch(bitmap, nativeChunk, 0, 0, (right - left) / scale, (bottom - top) / scale, filteredPaint);
                canvas.Restore();
            }
        }

        public static unsafe void DrawNinePatch(
            this SKCanvas canvas, SKBitmap bitmap, sbyte* chunk,
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
            canvas.DrawImageLattice(image, lattice, dst, paint);
        }


        public static void DrawBitmap(this SKCanvas canvas, Bitmap bitmap, float left, float top, Paint paint)
        {
            throwIfCannotDraw(bitmap);
            drawBitmap(canvas, bitmap.getNativeInstance(), left, top,
                    paint?.getNativeInstance(), densityDpi(canvas), DensityManager.ScreenDpi,
                    bitmap.mDensity);
        }

        public static void DrawBitmap(this SKCanvas canvas, Bitmap bitmap, SKMatrix matrix, Paint paint)
        {
            drawBitmapMatrix(canvas, bitmap.getNativeInstance(), ref matrix,
                    paint?.getNativeInstance());
        }

        public static void DrawBitmap(this SKCanvas canvas, Bitmap bitmap, Rect src, Rect dst, Paint paint)
        {
            if (dst == null)
            {
                throw new NullReferenceException();
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

            drawBitmapRect(canvas, bitmap.getNativeInstance(), left, top, right, bottom,
                    dst.left, dst.top, dst.right, dst.bottom, nativePaint, DensityManager.ScreenDpi,
                    bitmap.mDensity);
        }

        public static void DrawBitmap(this SKCanvas canvas, Bitmap bitmap, Rect src, RectF dst, Paint paint)
        {
            if (dst == null)
            {
                throw new NullReferenceException();
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

            drawBitmapRect(canvas, bitmap.getNativeInstance(), left, top, right, bottom,
                    dst.left, dst.top, dst.right, dst.bottom, nativePaint, DensityManager.ScreenDpi,
                    bitmap.mDensity);
        }

        static void drawBitmap(SKCanvas canvasHandle, SKBitmap bitmapHandle,
                               float left, float top, SKPaint paintHandle, int canvasDensity,
                               int screenDensity, int bitmapDensity)
        {

            if (canvasDensity == bitmapDensity || canvasDensity == 0 || bitmapDensity == 0)
            {
                if (screenDensity != 0 && screenDensity != bitmapDensity)
                {
                    SKPaint filteredPaint = new();
                    if (paintHandle != null)
                    {
                        filteredPaint = paintHandle;
                    }
                    filteredPaint.FilterQuality = SKFilterQuality.Low;
                    canvasHandle.DrawImage(bitmapHandle.AsImage(), left, top, filteredPaint);
                }
                else
                {
                    canvasHandle.DrawImage(bitmapHandle.AsImage(), left, top, paintHandle);
                }
            }
            else
            {
                canvasHandle.Save();
                float scale = canvasDensity / (float)bitmapDensity;
                canvasHandle.Translate(left, top);
                canvasHandle.Scale(scale, scale);

                SKPaint filteredPaint = new();
                if (paintHandle != null)
                {
                    filteredPaint = paintHandle;
                }
                filteredPaint.FilterQuality = SKFilterQuality.Low;

                canvasHandle.DrawImage(bitmapHandle.AsImage(), 0, 0, filteredPaint);
                canvasHandle.Restore();
            }
        }

        static void drawBitmap(SKCanvas canvasHandle, SKBitmap bitmapHandle,
                               ref SKMatrix matrixHandle, SKPaint paintHandle) {
            var image = bitmapHandle.AsImage();
            using SKAutoCanvasRestore acr = new(canvasHandle, true);
            canvasHandle.Concat(ref matrixHandle);
            canvasHandle.DrawImage(image, 0, 0, paintHandle);
        }


        static void drawBitmapMatrix(SKCanvas canvasHandle, SKBitmap bitmapHandle,
                                     ref SKMatrix matrixHandle, SKPaint paintHandle)
        {
            drawBitmap(canvasHandle, bitmapHandle, ref matrixHandle, paintHandle);
        }

        static void drawBitmap(SKCanvas canvasHandle, SKBitmap bitmapHandle,
                               float srcLeft, float srcTop, float srcRight,
                               float srcBottom, float dstLeft, float dstTop, float dstRight,
                               float dstBottom, SKPaint paintHandle) {
            var image = bitmapHandle.AsImage();
            SKRect srcRect = SKRect.Create(srcLeft, srcTop, srcRight, srcBottom);
            SKRect dstRect = SKRect.Create(dstLeft, dstTop, dstRight, dstBottom);
            canvasHandle.DrawImage(image, srcRect, dstRect, paintHandle);
        }

        static void drawBitmapRect(SKCanvas canvasHandle, SKBitmap bitmapHandle,
                                   float srcLeft, float srcTop, float srcRight, float srcBottom,
                                   float dstLeft, float dstTop, float dstRight, float dstBottom,
                                   SKPaint paintHandle, int screenDensity, int bitmapDensity)
        {
            if (screenDensity != 0 && screenDensity != bitmapDensity)
            {
                SKPaint filteredPaint = new();
                if (paintHandle != null)
                {
                    filteredPaint = paintHandle;
                }
                filteredPaint.FilterQuality = SKFilterQuality.Low;
                drawBitmap(canvasHandle, bitmapHandle, srcLeft, srcTop, srcRight, srcBottom,
                                   dstLeft, dstTop, dstRight, dstBottom, filteredPaint);
            }
            else
            {
                drawBitmap(canvasHandle, bitmapHandle, srcLeft, srcTop, srcRight, srcBottom,
                                   dstLeft, dstTop, dstRight, dstBottom, paintHandle);
            }
        }

        static void drawBitmapArray(SKCanvas canvasHandle,
                                    int[] jcolors, int offset, int stride,
                                    float x, float y, int width, int height,
                                    bool hasAlpha, SKPaint paintHandle)
        {
            // Note: If hasAlpha is false, kRGB_565_SkColorType will be used, which will
            // correct the alphaType to kOpaque_SkAlphaType.
            SKImageInfo info = SKImageInfo.Create(width, height,
                                   hasAlpha ? SKImageInfo.PlatformColorType: SKColorType.Rgb565,
                                   SKAlphaType.Premul);
            SKBitmap bitmap = new();
            bitmap.SetInfo(info);
            SKBitmap androidBitmap = BitmapFactory.allocateHeapBitmap(bitmap);
            if (androidBitmap != null)
            {
                return;
            }

            if (!BitmapFactory.SetPixels(jcolors, offset, stride, 0, 0, width, height, bitmap))
            {
                return;
            }

            canvasHandle.DrawImage(androidBitmap.AsImage(), x, y, paintHandle);
        }
    }
}