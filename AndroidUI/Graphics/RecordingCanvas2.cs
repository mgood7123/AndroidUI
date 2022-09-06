using AndroidUI.Applications;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public partial class RecordingCanvas2 : Canvas
    {
        protected MemoryWriter mem = new(new MemoryStream(), false);

        public RecordingCanvas2()
        {
            SetNativeObject(new SKNoDrawCanvas(0, 0));
        }

        public RecordingCanvas2(int w, int h)
        {
            SetNativeObject(new SKNoDrawCanvas(w, h));
        }

        public RecordingCanvas2(Context context) : base(context, new SKNoDrawCanvas(0, 0))
        {
        }

        public RecordingCanvas2(Context context, int w, int h) : base(context, new SKNoDrawCanvas(w, h))
        {
            width = w;
            height = h;
        }

        protected override void OnDispose()
        {
            mem.Dispose();
            base.OnDispose();
        }

        public override void Clear(SKColor color)
        {
            mem.Write((byte)COMMANDS.CLEAR);
            mem.WriteSKColor(color);
        }

        public override void Clear(SKColorF color)
        {
            mem.Write((byte)COMMANDS.CLEARF);
            mem.WriteSKColorF(color);
        }

        public override void DrawAnnotation(SKRect rect, string key, SKData value)
        {
            mem.Write((byte)COMMANDS.DRAW_ANNOTATION);
            mem.WriteSKRect(rect);
            mem.Write(key);
            mem.WriteSKData(value);
        }

        public override void DrawArc(SKRect oval, float startAngle, float sweepAngle, bool useCenter, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_ARC);
            mem.WriteSKRect(oval);
            mem.Write(startAngle);
            mem.Write(sweepAngle);
            mem.Write(useCenter);
            mem.WriteSKPaint(paint);
        }

        public override unsafe void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKColor[] colors, SKBlendMode mode, SKRect* cullRect, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_ATLAS);
            mem.WriteSKImage(atlas);
            mem.WriteSKRectArray(sprites);
            mem.WriteSKSKRotationScaleMatrixArray(transforms);
            mem.WriteSKColorArray(colors);

            mem.Write((byte)mode);

            if (mem.WriteNullable(cullRect))
            {
                mem.WriteSKRect(*cullRect);
            }

            mem.WriteSKPaint(paint);
        }

        public override void DrawCircle(float cx, float cy, float radius, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_CIRCLE);
            mem.Write(cx);
            mem.Write(cy);
            mem.Write(radius);
            mem.WriteSKPaint(paint);
        }

        public override void DrawColor(SKColor color, SKBlendMode mode = SKBlendMode.Src)
        {
            mem.Write((byte)COMMANDS.DRAW_COLOR);
            mem.WriteSKColor(color);
            mem.Write((byte)mode);
        }

        public override void DrawColor(SKColorF color, SKBlendMode mode = SKBlendMode.Src)
        {
            mem.Write((byte)COMMANDS.DRAW_COLORF);
            mem.WriteSKColorF(color);
            mem.Write((byte)mode);
        }

        public override void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix)
        {
            mem.Write((byte)COMMANDS.DRAW_DRAWABLE);
            mem.WriteSKDrawable(drawable);
            mem.WriteSKMatrix(ref matrix);
        }

        public override void DrawImage(SKImage image, SKRect dest, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_IMAGE_SKRECT_SKPAINT);
            mem.WriteSKImage(image);
            mem.WriteSKRect(dest);
            mem.WriteSKPaint(paint);
        }

        public override void DrawImage(SKImage image, SKRect source, SKRect dest, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_IMAGE_SKRECT_SKRECT_SKPAINT);
            mem.WriteSKImage(image);
            mem.WriteSKRect(source);
            mem.WriteSKRect(dest);
            mem.WriteSKPaint(paint);
        }

        public override void DrawImage(SKImage image, float x, float y, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_IMAGE_FLOAT_FLOAT_SKPAINT);
            mem.WriteSKImage(image);
            mem.Write(x);
            mem.Write(y);
            mem.WriteSKPaint(paint);
        }

        public override void DrawImageLattice(SKImage image, SKLattice lattice, SKRect dst, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_IMAGE_LATTICE);
            mem.WriteSKImage(image);
            mem.WriteSKLattice(lattice);
            mem.WriteSKRect(dst);
            mem.WriteSKPaint(paint);
        }

        public override void DrawImageNinePatch(SKImage image, SKRectI center, SKRect dst, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_IMAGE_NINEPATCH);
            mem.WriteSKImage(image);
            mem.WriteSKRectI(center);
            mem.WriteSKRect(dst);
            mem.WriteSKPaint(paint);
        }

        public override void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_LINE);
            mem.Write(x0);
            mem.Write(y0);
            mem.Write(x1);
            mem.Write(y1);
            mem.WriteSKPaint(paint);
        }

        public override void DrawLinkDestinationAnnotation(SKRect rect, SKData value)
        {
            mem.Write((byte)COMMANDS.DRAW_LINK_DESTINATION_ANNOTATION);
            mem.WriteSKRect(rect);
            mem.WriteSKData(value);
        }

        public override void DrawNamedDestinationAnnotation(SKPoint point, SKData value)
        {
            mem.Write((byte)COMMANDS.DRAW_NAMED_DESTINATION_ANNOTATION);
            mem.WriteSKPoint(point);
            mem.WriteSKData(value);
        }

        public override void DrawOval(SKRect rect, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_OVAL);
            mem.WriteSKRect(rect);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPaint(SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_PAINT);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPatch(SKPoint[] cubics, SKColor[] colors, SKPoint[] texCoords, SKBlendMode mode, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_PATCH);
            mem.WriteSKPointArray(cubics);
            mem.WriteSKColorArray(colors);
            mem.WriteSKPointArray(texCoords);
            mem.Write((byte)mode);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPath(SKPath path, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_PATH);
            mem.WriteSKPath(path);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_PICTURE_WITH_MATRIX);
            mem.WriteSKPicture(picture);
            mem.WriteSKMatrix(ref matrix);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPicture(SKPicture picture, SKPaint paint = null)
        {
            mem.Write((byte)COMMANDS.DRAW_PICTURE);
            mem.WriteSKPicture(picture);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPoint(float x, float y, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_POINT);
            mem.Write(x);
            mem.Write(y);
            mem.WriteSKPaint(paint);
        }

        public override void DrawPoints(SKPointMode mode, SKPoint[] points, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_POINTS);
            mem.Write((byte)mode);
            mem.WriteSKPointArray(points);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRect(float x, float y, float w, float h, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_RECT__XYWH);
            mem.Write(x);
            mem.Write(y);
            mem.Write(w);
            mem.Write(h);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRect(SKRect rect, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_RECT__RECT);
            mem.WriteSKRect(rect);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRegion(SKRegion region, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_REGION);
            mem.WriteSKRegion(region);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_ROUNDED_RECT__RECT_XY);
            mem.WriteSKRect(rect);
            mem.Write(rx);
            mem.Write(ry);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRoundRect(SKRoundRect rect, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_ROUNDED_RECT);
            mem.WriteSKRoundRect(rect);
            mem.WriteSKPaint(paint);
        }

        public override void DrawRoundRectDifference(SKRoundRect outer, SKRoundRect inner, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.DRAW_ROUNDED_RECT_DIFFERENCE);
            mem.WriteSKRoundRect(outer);
            mem.WriteSKRoundRect(inner);
            mem.WriteSKPaint(paint);
        }

        public override void DrawSurface(SKSurface surface, float x, float y, SKPaint paint = null)
        {
            // we cannot record the surface to draw it later cus GPU
            // so attempt to record it as an Image snapshot instead
            var image = surface.Snapshot();
            DrawImage(image, x, y, paint);
        }

        public override void DrawText(SKTextBlob text, float x, float y, SKPaint paint)
        {
            mem.Write(value: (byte)COMMANDS.DRAW_TEXTBLOB);
            mem.WriteSKTextBlob(text);
            mem.Write(x);
            mem.Write(y);
            mem.WriteSKPaint(paint);
        }

        public override void DrawToCanvas(BaseCanvas canvas, int x, int y, SKPaint paint = null)
        {
            throw new NotSupportedException();
        }

        public override void DrawToCanvas(BaseCanvas canvas, SKPoint point, SKPaint paint = null)
        {
            throw new NotSupportedException();
        }

        public override void DrawUrlAnnotation(SKRect rect, SKData value)
        {
            mem.Write(value: (byte)COMMANDS.DRAW_URL_ANNOTATION);
            mem.WriteSKRect(rect);
            mem.WriteSKData(value);
        }

        public override void DrawVertices(SKVertices vertices, SKBlendMode mode, SKPaint paint)
        {
            mem.Write(value: (byte)COMMANDS.DRAW_VERTICES);
            mem.WriteSKVertices(vertices);
            mem.Write((byte)mode);
            mem.WriteSKPaint(paint);
        }

        public override void Discard()
        {
            mem.Write((byte)COMMANDS.DISCARD);
            base.Discard();
        }

        public override void Flush()
        {
            mem.Write((byte)COMMANDS.FLUSH);
        }

        public override void ClipPath(SKPath path, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            mem.Write((byte)COMMANDS.CLIP_PATH);
            mem.WriteSKPath(path);
            mem.Write((byte)operation);
            mem.Write(antialias);
            base.ClipPath(path, operation, antialias);
        }

        public override void ClipRect(SKRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            mem.Write((byte)COMMANDS.CLIP_RECT);
            mem.WriteSKRect(rect);
            mem.Write((byte)operation);
            mem.Write(antialias);
            base.ClipRect(rect, operation, antialias);
        }

        public override void ClipRegion(SKRegion region, SKClipOperation operation = SKClipOperation.Intersect)
        {
            mem.Write((byte)COMMANDS.CLIP_REGION);
            mem.WriteSKRegion(region);
            mem.Write((byte)operation);
            base.ClipRegion(region, operation);
        }

        public override void ClipRoundRect(SKRoundRect rect, SKClipOperation operation = SKClipOperation.Intersect, bool antialias = false)
        {
            mem.Write((byte)COMMANDS.CLIP_ROUND_RECT);
            mem.WriteSKRoundRect(rect);
            mem.Write((byte)operation);
            mem.Write(antialias);
            base.ClipRoundRect(rect, operation, antialias);
        }

        public override void Concat(ref SKMatrix m)
        {
            mem.Write((byte)COMMANDS.CONCAT);
            mem.WriteSKMatrix(ref m);
            base.Concat(ref m);
        }

        public override bool GetDeviceClipBounds(out SKRectI bounds)
        {
            return base.GetDeviceClipBounds(out bounds);
        }

        public override bool GetLocalClipBounds(out SKRect bounds)
        {
            return base.GetLocalClipBounds(out bounds);
        }

        public override bool IsClipEmpty => base.IsClipEmpty;
        public override bool IsClipRect => base.IsClipRect;

        public override bool QuickReject(SKRect rect)
        {
            return base.QuickReject(rect);
        }

        public override void ResetMatrix()
        {
            mem.Write((byte)COMMANDS.RESET_MATRIX);
            base.ResetMatrix();
        }

        public override void Restore()
        {
            mem.Write((byte)COMMANDS.RESTORE);
            base.Restore();
        }

        public override void RestoreToCount(int count)
        {
            mem.Write((byte)COMMANDS.RESTORE_TO_COUNT);
            mem.Write(count);
            base.RestoreToCount(count);
        }

        public override void RotateDegrees(float degrees)
        {
            mem.Write((byte)COMMANDS.ROTATE_DEGREES);
            mem.Write(degrees);
            base.RotateDegrees(degrees);
        }

        public override void RotateRadians(float radians)
        {
            mem.Write((byte)COMMANDS.ROTATE_RADIANS);
            mem.Write(radians);
            base.RotateRadians(radians);
        }

        public override int Save()
        {
            mem.Write((byte)COMMANDS.SAVE);
            return base.Save();
        }

        public override int SaveCount => base.SaveCount;

        public override int SaveLayer(SKPaint paint)
        {
            mem.Write((byte)COMMANDS.SAVE_LAYER);
            mem.WriteSKPaint(paint);
            return base.SaveLayer(paint);
        }

        public override int SaveLayer(SKRect limit, SKPaint paint)
        {
            mem.Write((byte)COMMANDS.SAVE_LAYER_RECT);
            mem.WriteSKRect(limit);
            mem.WriteSKPaint(paint);
            return base.SaveLayer(limit, paint);
        }

        public override void Scale(float s)
        {
            mem.Write((byte)COMMANDS.SCALE);
            mem.Write(s);
            base.Scale(s);
        }

        public override void Scale(float sx, float sy)
        {
            mem.Write((byte)COMMANDS.SCALE_XY);
            mem.Write(sx);
            mem.Write(sy);
            base.Scale(sx, sy);
        }

        public override void Scale(SKPoint size)
        {
            mem.Write((byte)COMMANDS.SCALE_POINT);
            mem.WriteSKPoint(size);
            base.Scale(size);
        }

        public override void SetMatrix(SKMatrix matrix)
        {
            mem.Write((byte)COMMANDS.SET_MATRIX);
            mem.WriteSKMatrix(ref matrix);
            base.SetMatrix(matrix);
        }

        public override void Skew(float sx, float sy)
        {
            mem.Write((byte)COMMANDS.SKEW_XY);
            mem.Write(sx);
            mem.Write(sy);
            base.Skew(sx, sy);
        }

        public override void Skew(SKPoint skew)
        {
            mem.Write((byte)COMMANDS.SKEW_POINT);
            mem.WriteSKPoint(skew);
            base.Skew(skew);
        }

        public override SKMatrix TotalMatrix => base.TotalMatrix;

        public override void Translate(float dx, float dy)
        {
            mem.Write((byte)COMMANDS.TRANSLATE_XY);
            mem.Write(dx);
            mem.Write(dy);
            base.Translate(dx, dy);
        }

        public override void Translate(SKPoint point)
        {
            mem.Write((byte)COMMANDS.TRANSLATE_POINT);
            mem.WriteSKPoint(point);
            base.Translate(point);
        }

        public void ResetRecording()
        {
            mem.Dispose();
            mem = new(new MemoryStream(), false);
        }

        /// <summary>
        /// creates a Command Buffer from the current recording
        /// </summary>
        public CommandBuffer GetCommandBuffer()
        {
            MemoryStream src = (MemoryStream)mem.BaseStream;
            MemoryStream dst = new();

            var old = src.Position;
            src.Position = 0;
            src.CopyTo(dst);
            src.Position = old;

            return new CommandBuffer(new(dst, false));
        }
    }
}