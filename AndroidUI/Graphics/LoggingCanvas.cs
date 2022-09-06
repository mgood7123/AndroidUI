using SkiaSharp;

namespace AndroidUI.Graphics
{
    public class LoggingCanvas : SKCanvasForwarder
    {
        Utils.LogTag Log = new("LoggingCanvas");

        private bool log_methods;

        public bool LogMethods { get => log_methods; set => log_methods = value; }

        public LoggingCanvas()
        {
        }

        public LoggingCanvas(SKCanvas canvas) : base(canvas)
        {
            if (log_methods) Log.d("LoggingCanvas() ");
        }

        public LoggingCanvas(bool log_methods)
        {
            this.log_methods = log_methods;
            if (log_methods) Log.d("LoggingCanvas() ");
        }

        public LoggingCanvas(SKCanvas canvas, bool owns_canvas) : base(canvas, owns_canvas)
        {
            if (log_methods) Log.d("LoggingCanvas() ");
        }

        public LoggingCanvas(SKCanvas canvas, bool owns_canvas, bool log_methods) : base(canvas, owns_canvas)
        {
            this.log_methods = log_methods;
            if (log_methods) Log.d("LoggingCanvas() ");
        }

        public override bool IsClipEmpty
        {
            get
            {
                if (log_methods) Log.d("IsClipEmpty");
                return base.IsClipEmpty;
            }
        }

        public override bool IsClipRect
        {
            get
            {
                if (log_methods) Log.d("IsClipRect");
                return base.IsClipRect;
            }
        }

        public override SKMatrix TotalMatrix
        {
            get
            {
                if (log_methods) Log.d("get TotalMatrix");
                return base.TotalMatrix;
            }
        }

        public override int SaveCount
        {
            get
            {
                if (log_methods) Log.d("get SaveCount");
                return base.SaveCount;
            }
        }

        public override void Clear(SKColor color)
        {
            if (log_methods) Log.d("Clear(" + color.ToString() + ")");
            base.Clear(color);
        }

        public override void Clear(SKColorF color)
        {
            if (log_methods) Log.d("Clear(" + color.ToString() + ")");
            base.Clear(color);
        }

        public override void ClipPath(SKPath path, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            if (log_methods) Log.d("ClipPath");
            base.ClipPath(path, operation, antialias);
        }

        public override void ClipRect(SKRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            if (log_methods) Log.d("ClipRect");
            base.ClipRect(rect, operation, antialias);
        }

        public override void ClipRegion(SKRegion region, SKClipOperation operation = SKClipOperation.Intersect)
        {
            if (log_methods) Log.d("ClipRegion");
            base.ClipRegion(region, operation);
        }

        public override void ClipRoundRect(SKRoundRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            if (log_methods) Log.d("ClipRoundRect");
            base.ClipRoundRect(rect, operation, antialias);
        }

        public override void Concat(ref SKMatrix m)
        {
            if (log_methods) Log.d("Concat m: " + Utils.Arrays.Arrays.toString(m.Values));
            base.Concat(ref m);
        }

        public override void Discard()
        {
            if (log_methods) Log.d("Discard");
            base.Discard();
        }

        public override void DrawAnnotation(SKRect rect, string key, SKData value)
        {
            if (log_methods) Log.d("DrawAnnotation");
            base.DrawAnnotation(rect, key, value);
        }

        public override void DrawArc(SKRect oval, float startAngle, float sweepAngle, bool useCenter, SKPaint paint)
        {
            if (log_methods) Log.d("DrawArc");
            base.DrawArc(oval, startAngle, sweepAngle, useCenter, paint);
        }

        public override unsafe void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKColor[] colors, SKBlendMode mode, SKRect* cullRect, SKPaint paint)
        {
            if (log_methods) Log.d("DrawAtlas");
            base.DrawAtlas(atlas, sprites, transforms, colors, mode, cullRect, paint);
        }

        public override void DrawCircle(float cx, float cy, float radius, SKPaint paint)
        {
            if (log_methods) Log.d("DrawCircle");
            base.DrawCircle(cx, cy, radius, paint);
        }

        public override void DrawColor(SKColor color, SKBlendMode mode = SKBlendMode.Src)
        {
            if (log_methods) Log.d("DrawColor");
            base.DrawColor(color, mode);
        }

        public override void DrawColor(SKColorF color, SKBlendMode mode = SKBlendMode.Src)
        {
            if (log_methods) Log.d("DrawColor");
            base.DrawColor(color, mode);
        }

        public override void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix)
        {
            if (log_methods) Log.d("DrawDrawable");
            base.DrawDrawable(drawable, ref matrix);
        }

        public override void DrawImage(SKImage image, float x, float y, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawImage");
            base.DrawImage(image, x, y, paint);
        }

        public override void DrawImage(SKImage image, SKRect dest, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawImage");
            base.DrawImage(image, dest, paint);
        }

        public override void DrawImage(SKImage image, SKRect source, SKRect dest, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawImage");
            base.DrawImage(image, source, dest, paint);
        }

        public override void DrawImageLattice(SKImage image, SKLattice lattice, SKRect dst, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawImageLattice");
            base.DrawImageLattice(image, lattice, dst, paint);
        }

        public override void DrawImageNinePatch(SKImage image, SKRectI center, SKRect dst, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawImageNinePatch");
            base.DrawImageNinePatch(image, center, dst, paint);
        }

        public override void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint)
        {
            if (log_methods) Log.d("DrawLine");
            base.DrawLine(x0, y0, x1, y1, paint);
        }

        public override void DrawLinkDestinationAnnotation(SKRect rect, SKData value)
        {
            if (log_methods) Log.d("DrawLinkDestinationAnnotation");
            base.DrawLinkDestinationAnnotation(rect, value);
        }

        public override void DrawNamedDestinationAnnotation(SKPoint point, SKData value)
        {
            if (log_methods) Log.d("DrawNamedDestinationAnnotation");
            base.DrawNamedDestinationAnnotation(point, value);
        }

        public override void DrawOval(SKRect rect, SKPaint paint)
        {
            if (log_methods) Log.d("DrawOval");
            base.DrawOval(rect, paint);
        }

        public override void DrawPaint(SKPaint paint)
        {
            if (log_methods) Log.d("DrawPaint");
            base.DrawPaint(paint);
        }

        public override void DrawPatch(SKPoint[] cubics, SKColor[] colors, SKPoint[] texCoords, SKBlendMode mode, SKPaint paint)
        {
            if (log_methods) Log.d("DrawPatch");
            base.DrawPatch(cubics, colors, texCoords, mode, paint);
        }

        public override void DrawPath(SKPath path, SKPaint paint)
        {
            if (log_methods) Log.d("DrawPath");
            base.DrawPath(path, paint);
        }

        public override void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawPicture");
            base.DrawPicture(picture, ref matrix, paint);
        }

        public override void DrawPicture(SKPicture picture, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawPicture");
            base.DrawPicture(picture, paint);
        }

        public override void DrawPoint(float x, float y, SKPaint paint)
        {
            if (log_methods) Log.d("DrawPoint");
            base.DrawPoint(x, y, paint);
        }

        public override void DrawPoints(SKPointMode mode, SKPoint[] points, SKPaint paint)
        {
            if (log_methods) Log.d("DrawPoints");
            base.DrawPoints(mode, points, paint);
        }

        public override void DrawRect(float x, float y, float w, float h, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRect x: " + x + ", y: " + y + ", w: " + w + ", h: " + h + ", paint: " + paint?.Color);
            base.DrawRect(x, y, w, h, paint);
        }

        public override void DrawRect(SKRect rect, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRect rect: " + rect + ", paint: " + paint?.Color);
            base.DrawRect(rect, paint);
        }

        public override void DrawRegion(SKRegion region, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRegion");
            base.DrawRegion(region, paint);
        }

        public override void DrawRoundRect(SKRoundRect rect, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRoundRect");
            base.DrawRoundRect(rect, paint);
        }

        public override void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRoundRect");
            base.DrawRoundRect(rect, rx, ry, paint);
        }

        public override void DrawRoundRectDifference(SKRoundRect outer, SKRoundRect inner, SKPaint paint)
        {
            if (log_methods) Log.d("DrawRoundRectDifference");
            base.DrawRoundRectDifference(outer, inner, paint);
        }

        public override void DrawSurface(SKSurface surface, float x, float y, SKPaint paint = null)
        {
            if (log_methods) Log.d("DrawSurface");
            base.DrawSurface(surface, x, y, paint);
        }

        public override void DrawText(SKTextBlob text, float x, float y, SKPaint paint)
        {
            if (log_methods) Log.d("DrawText");
            base.DrawText(text, x, y, paint);
        }

        public override void DrawUrlAnnotation(SKRect rect, SKData value)
        {
            if (log_methods) Log.d("DrawUrlAnnotation");
            base.DrawUrlAnnotation(rect, value);
        }

        public override void DrawVertices(SKVertices vertices, SKBlendMode mode, SKPaint paint)
        {
            if (log_methods) Log.d("DrawVertices");
            base.DrawVertices(vertices, mode, paint);
        }

        public override void Flush()
        {
            if (log_methods) Log.d("Flush");
            base.Flush();
        }

        public override bool GetDeviceClipBounds(out SKRectI bounds)
        {
            if (log_methods) Log.d("GetDeviceClipBounds");
            return base.GetDeviceClipBounds(out bounds);
        }

        public override bool GetLocalClipBounds(out SKRect bounds)
        {
            if (log_methods) Log.d("GetLocalClipBounds");
            return base.GetLocalClipBounds(out bounds);
        }

        public override bool QuickReject(SKRect rect)
        {
            if (log_methods) Log.d("QuickReject");
            return base.QuickReject(rect);
        }

        public override SKCanvas ReleaseNativeObject()
        {
            if (log_methods) Log.d("ReleaseNativeObject");
            return base.ReleaseNativeObject();
        }

        public override void ResetMatrix()
        {
            if (log_methods) Log.d("ResetMatrix");
            base.ResetMatrix();
        }

        public override void Restore()
        {
            if (log_methods) Log.d("Restore");
            base.Restore();
        }

        public override void RestoreToCount(int count)
        {
            if (log_methods) Log.d("RestoreToCount");
            base.RestoreToCount(count);
        }

        public override void RotateDegrees(float degrees)
        {
            if (log_methods) Log.d("RotateDegrees");
            base.RotateDegrees(degrees);
        }

        public override void RotateRadians(float radians)
        {
            if (log_methods) Log.d("RotateRadians");
            base.RotateRadians(radians);
        }

        public override int Save()
        {
            if (log_methods) Log.d("Save");
            return base.Save();
        }

        public override int SaveLayer(SKRect limit, SKPaint paint)
        {
            if (log_methods) Log.d("SaveLayer rect: " + limit + ", paint: " + paint?.Color);
            return base.SaveLayer(limit, paint);
        }

        public override int SaveLayer(SKPaint paint)
        {
            if (log_methods) Log.d("SaveLayer paint: " + paint?.Color);
            return base.SaveLayer(paint);
        }

        public override void Scale(float s)
        {
            if (log_methods) Log.d("Scale");
            base.Scale(s);
        }

        public override void Scale(float sx, float sy)
        {
            if (log_methods) Log.d("Scale");
            base.Scale(sx, sy);
        }

        public override void Scale(SKPoint size)
        {
            if (log_methods) Log.d("Scale");
            base.Scale(size);
        }

        public override void SetMatrix(SKMatrix matrix)
        {
            if (log_methods) Log.d("SetMatrix");
            base.SetMatrix(matrix);
        }

        public override void SetNativeObject(SKCanvas canvas)
        {
            if (log_methods) Log.d("SetNativeObject");
            base.SetNativeObject(canvas);
        }

        public override void SetNativeObject(SKCanvas canvas, bool ownsCanvas)
        {
            if (log_methods) Log.d("SetNativeObject");
            base.SetNativeObject(canvas, ownsCanvas);
        }

        public override void Skew(float sx, float sy)
        {
            if (log_methods) Log.d("Skew");
            base.Skew(sx, sy);
        }

        public override void Skew(SKPoint skew)
        {
            if (log_methods) Log.d("Skew");
            base.Skew(skew);
        }

        public override void Translate(float dx, float dy)
        {
            if (log_methods) Log.d("Translate");
            base.Translate(dx, dy);
        }

        public override void Translate(SKPoint point)
        {
            if (log_methods) Log.d("Translate");
            base.Translate(point);
        }

        virtual protected void OnDispose()
        {
        }

        sealed protected override void Dispose(bool disposing)
        {
            if (log_methods) Log.d("Dispose");
            OnDispose();
            base.Dispose(disposing);
        }

        sealed protected override bool OwnsHandle
        {
            get
            {
                if (log_methods) Log.d("OwnsHandle");
                return base.OwnsHandle;
            }
            set => base.OwnsHandle = value;
        }

        sealed public override IntPtr Handle
        {
            get
            {
                if (log_methods) Log.d("Handle");
                return base.Handle;
            }
            protected set => base.Handle = value;
        }

        sealed protected override void DisposeUnownedManaged()
        {
            if (log_methods) Log.d("DisposeUnownedManaged");
            base.DisposeUnownedManaged();
        }

        sealed protected override void DisposeNative()
        {
            if (log_methods) Log.d("DisposeNative");
            base.DisposeNative();
        }
    }
}