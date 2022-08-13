using AndroidUI.Utils;
using SkiaSharp;
using System.ComponentModel;
using System.Text;

namespace AndroidUI.Graphics
{

    public class CanvasWrapper : Disposable
    {
        SKCanvas canvas;
        private const double RadiansCircle = 2.0 * Math.PI;
        private const double DegreesCircle = 360.0;

        private bool disposedValue;

        internal bool paramaterlessConstructorCalled;

        protected bool ParamaterlessConstructorCalled => paramaterlessConstructorCalled;

        public CanvasWrapper() {
            paramaterlessConstructorCalled = true;
        }

        public CanvasWrapper(SKCanvas canvas)
        {
            paramaterlessConstructorCalled = false;
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        public virtual void SetNativeObject(SKCanvas canvas)
        {
            if (!paramaterlessConstructorCalled)
            {
                throw new InvalidOperationException("this method must only be called on a paramaterless constructor: CanvasWrapper w = new(); w.SetNativeObject(canvas); ");
            }
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        public SKCanvas GetNativeObject() => canvas;

        /// <summary>
        /// Releases the SKCanvas object, it will not be disposed nor discarded
        /// </summary>
        public virtual SKCanvas ReleaseNativeObject()
        {
            SKCanvas r = canvas;
            canvas = null;
            return r;
        }

        protected virtual void OnDispose() {
            canvas?.Dispose();
        }

        public virtual void Discard() => canvas.Discard();

        public virtual bool QuickReject(SKRect rect) => canvas.QuickReject(rect);

        public virtual bool QuickReject(SKPath path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return path.IsEmpty || QuickReject(path.Bounds);
        }

        public virtual int Save() => canvas.Save();

        public virtual int SaveLayer(SKRect limit, SKPaint paint) => canvas.SaveLayer(limit, paint);

        public virtual int SaveLayer(SKPaint paint) => canvas.SaveLayer(paint);

        public int SaveLayer() => SaveLayer(null);

        public virtual void DrawColor(SKColor color, SKBlendMode mode = SKBlendMode.Src) => canvas.DrawColor(color, mode);

        public virtual void DrawColor(SKColorF color, SKBlendMode mode = SKBlendMode.Src) => canvas.DrawColor(color);

        public void DrawLine(SKPoint p0, SKPoint p1, SKPaint paint)
        {
            DrawLine(p0.X, p0.Y, p1.X, p1.Y, paint);
        }

        public virtual void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint) => canvas.DrawLine(x0, y0, x1, y1, paint);

        public void Clear() => Clear(SKColors.Empty);

        public virtual void Clear(SKColor color) => canvas.Clear(color);

        public virtual void Clear(SKColorF color) => canvas.Clear(color);

        public virtual void Restore() => canvas.Restore();

        public virtual void RestoreToCount(int count) => canvas.RestoreToCount(count);

        public virtual void Translate(float dx, float dy) => canvas.Translate(dx, dy);

        public virtual void Translate(SKPoint point) => canvas.Translate(point);

        public virtual void Scale(float s) => canvas.Scale(s);

        public virtual void Scale(float sx, float sy) => canvas.Scale(sx, sy);

        public virtual void Scale(SKPoint size) => canvas.Scale(size);

        public void Scale(float sx, float sy, float px, float py)
        {
            if (sx == 1 && sy == 1)
                return;

            Translate(px, py);
            Scale(sx, sy);
            Translate(-px, -py);
        }

        public virtual void RotateDegrees(float degrees) => canvas.RotateDegrees(degrees);

        public virtual void RotateRadians(float radians) => canvas.RotateRadians(radians);

        public void RotateDegrees(float degrees, float px, float py)
        {
            if (degrees % DegreesCircle == 0)
                return;

            Translate(px, py);
            RotateDegrees(degrees);
            Translate(-px, -py);
        }

        public void RotateRadians(float radians, float px, float py)
        {
            if (radians % RadiansCircle == 0)
                return;

            Translate(px, py);
            RotateRadians(radians);
            Translate(-px, -py);
        }

        public virtual void Skew(float sx, float sy) => canvas.Skew(sx, sy);

        public virtual void Skew(SKPoint skew) => canvas.Skew(skew);

        public virtual void Concat(ref SKMatrix m) => canvas.Concat(ref m);

        public virtual void ClipRect(SKRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false) => canvas.ClipRect(rect, operation, antialias);

        public virtual void ClipRoundRect(SKRoundRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false) => canvas.ClipRoundRect(rect, operation, antialias);

        public virtual void ClipPath(SKPath path, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false) => canvas.ClipPath(path, operation, antialias);

        public virtual void ClipRegion(SKRegion region, SKClipOperation operation = SKClipOperation.Intersect) => canvas.ClipRegion(region, operation);

        public virtual SKRect LocalClipBounds
        {
            get
            {
                GetLocalClipBounds(out var bounds);
                return bounds;
            }
        }

        public virtual SKRectI DeviceClipBounds
        {
            get
            {
                GetDeviceClipBounds(out var bounds);
                return bounds;
            }
        }

        public virtual bool IsClipEmpty => canvas.IsClipEmpty;

        public virtual bool IsClipRect => canvas.IsClipRect;

        public virtual bool GetLocalClipBounds(out SKRect bounds) => canvas.GetLocalClipBounds(out bounds);

        public virtual bool GetDeviceClipBounds(out SKRectI bounds) => canvas.GetDeviceClipBounds(out bounds);

        public virtual void DrawPaint(SKPaint paint) => canvas.DrawPaint(paint);

        public virtual void DrawRegion(SKRegion region, SKPaint paint) => canvas.DrawRegion(region, paint);

        public void DrawRect(float x, float y, float w, float h, SKPaint paint)
        {
            DrawRect(SKRect.Create(x, y, w, h), paint);
        }

        public virtual void DrawRect(SKRect rect, SKPaint paint) => canvas.DrawRect(rect, paint);

        public virtual void DrawRoundRect(SKRoundRect rect, SKPaint paint) => canvas.DrawRoundRect(rect, paint);

        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry, SKPaint paint)
        {
            DrawRoundRect(SKRect.Create(x, y, w, h), rx, ry, paint);
        }

        public virtual void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint) => canvas.DrawRoundRect(rect, rx, ry, paint);

        public void DrawRoundRect(SKRect rect, SKSize r, SKPaint paint)
        {
            DrawRoundRect(rect, r.Width, r.Height, paint);
        }

        public void DrawOval(float cx, float cy, float rx, float ry, SKPaint paint)
        {
            DrawOval(new SKRect(cx - rx, cy - ry, cx + rx, cy + ry), paint);
        }

        public void DrawOval(SKPoint c, SKSize r, SKPaint paint)
        {
            DrawOval(c.X, c.Y, r.Width, r.Height, paint);
        }

        public virtual void DrawOval(SKRect rect, SKPaint paint) => canvas.DrawOval(rect, paint);

        public virtual void DrawCircle(float cx, float cy, float radius, SKPaint paint) => canvas.DrawCircle(cx, cy, radius, paint);

        public void DrawCircle(SKPoint c, float radius, SKPaint paint)
        {
            DrawCircle(c.X, c.Y, radius, paint);
        }

        public virtual void DrawPath(SKPath path, SKPaint paint) => canvas.DrawPath(path, paint);

        public virtual void DrawPoints(SKPointMode mode, SKPoint[] points, SKPaint paint) => canvas.DrawPoints(mode, points, paint);

        public void DrawPoint(SKPoint p, SKPaint paint)
        {
            DrawPoint(p.X, p.Y, paint);
        }

        public virtual void DrawPoint(float x, float y, SKPaint paint) => canvas.DrawPoint(x, y, paint);

        public void DrawPoint(SKPoint p, SKColor color)
        {
            DrawPoint(p.X, p.Y, color);
        }

        public void DrawPoint(float x, float y, SKColor color)
        {
            using (var paint = new SKPaint { Color = color, BlendMode = SKBlendMode.Src })
            {
                DrawPoint(x, y, paint);
            }
        }

        public void DrawImage(SKImage image, SKPoint p, SKPaint paint = null)
        {
            DrawImage(image, p.X, p.Y, paint);
        }

        public virtual void DrawImage(SKImage image, float x, float y, SKPaint paint = null) => canvas.DrawImage(image, x, y, paint);

        public virtual void DrawImage(SKImage image, SKRect dest, SKPaint paint = null) => canvas.DrawImage(image, dest, paint);

        public virtual void DrawImage(SKImage image, SKRect source, SKRect dest, SKPaint paint = null) => canvas.DrawImage(image, source, dest, paint);

        public void DrawPicture(SKPicture picture, float x, float y, SKPaint paint = null)
        {
            var matrix = SKMatrix.CreateTranslation(x, y);
            DrawPicture(picture, ref matrix, paint);
        }

        public void DrawPicture(SKPicture picture, SKPoint p, SKPaint paint = null)
        {
            DrawPicture(picture, p.X, p.Y, paint);
        }

        public virtual void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint paint = null) => canvas.DrawPicture(picture, ref matrix, paint);

        public virtual void DrawPicture(SKPicture picture, SKPaint paint = null) => canvas.DrawPicture(picture, paint);

        public virtual void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix) => canvas.DrawDrawable(drawable, ref matrix);

        public void DrawDrawable(SKDrawable drawable, float x, float y)
        {
            if (drawable == null)
                throw new ArgumentNullException(nameof(drawable));
            var matrix = SKMatrix.CreateTranslation(x, y);
            DrawDrawable(drawable, ref matrix);
        }

        public void DrawDrawable(SKDrawable drawable, SKPoint p)
        {
            if (drawable == null)
                throw new ArgumentNullException(nameof(drawable));
            var matrix = SKMatrix.CreateTranslation(p.X, p.Y);
            DrawDrawable(drawable, ref matrix);
        }

        public void DrawBitmap(SKBitmap bitmap, SKPoint p, SKPaint paint = null) =>
        DrawBitmap(bitmap, p.X, p.Y, paint);

        public void DrawBitmap(SKBitmap bitmap, float x, float y, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImage(image, x, y, paint);
        }

        public void DrawBitmap(SKBitmap bitmap, SKRect dest, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImage(image, dest, paint);
        }

        public void DrawBitmap(SKBitmap bitmap, SKRect source, SKRect dest, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImage(image, source, dest, paint);
        }

        // DrawSurface

        public void DrawSurface(SKSurface surface, SKPoint p, SKPaint paint = null)
        {
            DrawSurface(surface, p.X, p.Y, paint);
        }

        public virtual void DrawSurface(SKSurface surface, float x, float y, SKPaint paint = null)
        {
            if (surface == null)
                throw new ArgumentNullException(nameof(surface));

            surface.Draw(canvas, x, y, paint);
        }

        public virtual void DrawText(SKTextBlob text, float x, float y, SKPaint paint) => canvas.DrawText(text, x, y, paint);

        public void DrawText(string text, SKPoint p, SKPaint paint)
        {
            DrawText(text, p.X, p.Y, paint);
        }

        public virtual void DrawText(string text, float x, float y, SKPaint paint) => canvas.DrawText(text, x, y, paint);

        public virtual void DrawText(string text, float x, float y, SKFont font, SKPaint paint) => canvas.DrawText(text, x, y, paint);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public void DrawText(byte[] text, SKPoint p, SKPaint paint)
        {
            DrawText(text, p.X, p.Y, paint);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public virtual void DrawText(byte[] text, float x, float y, SKPaint paint) => canvas.DrawText(text, x, y, paint);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public void DrawText(IntPtr buffer, int length, SKPoint p, SKPaint paint)
        {
            DrawText(buffer, length, p.X, p.Y, paint);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public virtual void DrawText(IntPtr buffer, int length, float x, float y, SKPaint paint) => canvas.DrawText(buffer, length, x, y, paint);

        // DrawPositionedText

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public virtual void DrawPositionedText(string text, SKPoint[] points, SKPaint paint) => canvas.DrawPositionedText(text, points, paint);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public virtual void DrawPositionedText(byte[] text, SKPoint[] points, SKPaint paint) => canvas.DrawPositionedText(text, points, paint);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawText(SKTextBlob, float, float, SKPaint) instead.")]
        public virtual void DrawPositionedText(IntPtr buffer, int length, SKPoint[] points, SKPaint paint) => canvas.DrawPositionedText(buffer, length, points, paint);

        // DrawTextOnPath

        public void DrawTextOnPath(string text, SKPath path, SKPoint offset, SKPaint paint)
        {
            DrawTextOnPath(text, path, offset, true, paint);
        }

        public void DrawTextOnPath(string text, SKPath path, float hOffset, float vOffset, SKPaint paint)
        {
            DrawTextOnPath(text, path, new SKPoint(hOffset, vOffset), true, paint);
        }

        public virtual void DrawTextOnPath(string text, SKPath path, SKPoint offset, bool warpGlyphs, SKPaint paint) => canvas.DrawTextOnPath(text, path, offset, warpGlyphs, paint);

        public virtual void DrawTextOnPath(string text, SKPath path, SKPoint offset, bool warpGlyphs, SKFont font, SKPaint paint) => canvas.DrawTextOnPath(text, path, offset, warpGlyphs, font, paint);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawTextOnPath(string, SKPath, SKPoint, SKPaint) instead.")]
        public void DrawTextOnPath(byte[] text, SKPath path, SKPoint offset, SKPaint paint)
        {
            DrawTextOnPath(text, path, offset.X, offset.Y, paint);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawTextOnPath(string, SKPath, float, float, SKPaint) instead.")]
        public void DrawTextOnPath(byte[] text, SKPath path, float hOffset, float vOffset, SKPaint paint)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (paint == null)
                throw new ArgumentNullException(nameof(paint));

            unsafe
            {
                fixed (byte* t = text)
                {
                    DrawTextOnPath((IntPtr)t, text.Length, path, hOffset, vOffset, paint);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawTextOnPath(string, SKPath, SKPoint, SKPaint) instead.")]
        public void DrawTextOnPath(IntPtr buffer, int length, SKPath path, SKPoint offset, SKPaint paint)
        {
            DrawTextOnPath(buffer, length, path, offset.X, offset.Y, paint);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use DrawTextOnPath(string, SKPath, float, float, SKPaint) instead.")]
        public virtual void DrawTextOnPath(IntPtr buffer, int length, SKPath path, float hOffset, float vOffset, SKPaint paint) => canvas.DrawTextOnPath(buffer, length, path, hOffset, vOffset, paint);

        public virtual void Flush() => canvas.Flush();

        public virtual void DrawAnnotation(SKRect rect, string key, SKData value) => canvas.DrawAnnotation(rect, key, value);

        public virtual void DrawUrlAnnotation(SKRect rect, SKData value) => canvas.DrawUrlAnnotation(rect, value);

        public SKData DrawUrlAnnotation(SKRect rect, string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            var data = SKData.CreateCopy(bytes, (ulong)(bytes.Length + 1)); // + 1 for the terminating char
            DrawUrlAnnotation(rect, data);
            return data;
        }

        public virtual void DrawNamedDestinationAnnotation(SKPoint point, SKData value) => canvas.DrawNamedDestinationAnnotation(point, value);

        public SKData DrawNamedDestinationAnnotation(SKPoint point, string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            var data = SKData.CreateCopy(bytes, (ulong)(bytes.Length + 1)); // + 1 for the terminating char
            DrawNamedDestinationAnnotation(point, data);
            return data;
        }

        public virtual void DrawLinkDestinationAnnotation(SKRect rect, SKData value) => canvas.DrawLinkDestinationAnnotation(rect, value);

        public SKData DrawLinkDestinationAnnotation(SKRect rect, string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            var data = SKData.CreateCopy(bytes, (ulong)(bytes.Length + 1)); // + 1 for the terminating char
            DrawLinkDestinationAnnotation(rect, data);
            return data;
        }

        public void DrawBitmapNinePatch(SKBitmap bitmap, SKRectI center, SKRect dst, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImageNinePatch(image, center, dst, paint);
        }

        public virtual void DrawImageNinePatch(SKImage image, SKRectI center, SKRect dst, SKPaint paint = null) => canvas.DrawImageNinePatch(image, center, dst, paint);

        public void DrawBitmapLattice(SKBitmap bitmap, int[] xDivs, int[] yDivs, SKRect dst, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImageLattice(image, xDivs, yDivs, dst, paint);
        }

        public void DrawImageLattice(SKImage image, int[] xDivs, int[] yDivs, SKRect dst, SKPaint paint = null)
        {
            var lattice = new SKLattice
            {
                XDivs = xDivs,
                YDivs = yDivs
            };
            DrawImageLattice(image, lattice, dst, paint);
        }

        public void DrawBitmapLattice(SKBitmap bitmap, SKLattice lattice, SKRect dst, SKPaint paint = null)
        {
            using var image = SKImage.FromBitmap(bitmap);
            DrawImageLattice(image, lattice, dst, paint);
        }

        public virtual void DrawImageLattice(SKImage image, SKLattice lattice, SKRect dst, SKPaint paint = null) => canvas.DrawImageLattice(image, lattice, dst, paint);

        public virtual void ResetMatrix() => canvas.ResetMatrix();

        public virtual void SetMatrix(SKMatrix matrix) => canvas.SetMatrix(matrix);
        public virtual void SetMatrix(ref SKMatrix matrix) => canvas.SetMatrix(matrix);

        public virtual SKMatrix TotalMatrix => canvas.TotalMatrix;

        public virtual int SaveCount => canvas.SaveCount;

        public void DrawVertices(SKVertexMode vmode, SKPoint[] vertices, SKColor[] colors, SKPaint paint)
        {
            var vert = SKVertices.CreateCopy(vmode, vertices, colors);
            DrawVertices(vert, SKBlendMode.Modulate, paint);
        }

        public void DrawVertices(SKVertexMode vmode, SKPoint[] vertices, SKPoint[] texs, SKColor[] colors, SKPaint paint)
        {
            var vert = SKVertices.CreateCopy(vmode, vertices, texs, colors);
            DrawVertices(vert, SKBlendMode.Modulate, paint);
        }

        public void DrawVertices(SKVertexMode vmode, SKPoint[] vertices, SKPoint[] texs, SKColor[] colors, UInt16[] indices, SKPaint paint)
        {
            var vert = SKVertices.CreateCopy(vmode, vertices, texs, colors, indices);
            DrawVertices(vert, SKBlendMode.Modulate, paint);
        }

        public void DrawVertices(SKVertexMode vmode, SKPoint[] vertices, SKPoint[] texs, SKColor[] colors, SKBlendMode mode, UInt16[] indices, SKPaint paint)
        {
            var vert = SKVertices.CreateCopy(vmode, vertices, texs, colors, indices);
            DrawVertices(vert, mode, paint);
        }

        public virtual void DrawVertices(SKVertices vertices, SKBlendMode mode, SKPaint paint) => canvas.DrawVertices(vertices, mode, paint);

        public virtual void DrawArc(SKRect oval, float startAngle, float sweepAngle, bool useCenter, SKPaint paint) => canvas.DrawArc(oval, startAngle, sweepAngle, useCenter, paint);

        public virtual void DrawRoundRectDifference(SKRoundRect outer, SKRoundRect inner, SKPaint paint) => canvas.DrawRoundRectDifference(outer, inner, paint);

        public virtual void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKPaint paint) => canvas.DrawAtlas(atlas, sprites, transforms, paint);

        public virtual void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKColor[] colors, SKBlendMode mode, SKPaint paint) => canvas.DrawAtlas(atlas, sprites, transforms, colors, mode, paint);

        public virtual void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKColor[] colors, SKBlendMode mode, SKRect cullRect, SKPaint paint) => canvas.DrawAtlas(atlas, sprites, transforms, colors, mode, cullRect, paint);

        public void DrawPatch(SKPoint[] cubics, SKColor[] colors, SKPoint[] texCoords, SKPaint paint) =>
        DrawPatch(cubics, colors, texCoords, SKBlendMode.Modulate, paint);

        public virtual void DrawPatch(SKPoint[] cubics, SKColor[] colors, SKPoint[] texCoords, SKBlendMode mode, SKPaint paint) => canvas.DrawPatch(cubics, colors, texCoords, mode, paint);
    }
}