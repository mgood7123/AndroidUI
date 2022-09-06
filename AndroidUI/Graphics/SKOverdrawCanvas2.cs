using AndroidUI.Utils;
using AndroidUI.Widgets;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public class SKOverdrawCanvas2 : SKCanvasForwarder
    {
        LogTag Log;
        SKRuntimeEffect shader_code;
        string shader_error_message;

        private class Layer : Disposable
        {
            public SKBitmap bitmap;
            public SKCanvas canvas;

            public Layer(SKBitmap bitmap, SKCanvas canvas)
            {
                this.bitmap = bitmap;
                this.canvas = canvas;
            }

            protected override void OnDispose()
            {
                canvas?.Dispose();
                bitmap?.Dispose();
            }
        }

        Stack<Layer> layers = new();
        Stack<bool> is_layer = new();

        SKCanvas base_canvas;
        bool base_is_owned;

        public string Shader_error_message { get => shader_error_message; set => shader_error_message = value; }

        public SKOverdrawCanvas2()
        {
            InitShaderCode();
            Log = new(this);
        }

        public SKOverdrawCanvas2(SKCanvas canvas) : base(canvas)
        {
            InitShaderCode();
            Log = new(this);
        }

        public SKOverdrawCanvas2(SKCanvas canvas, bool ownsCanvas) : base(canvas, ownsCanvas)
        {
            InitShaderCode();
            Log = new(this);
        }

        private void InitShaderCode()
        {
            shader_code = SKRuntimeEffect.Create(
                @"
uniform shader src;

half4 main(float2 coords) {
    return half4(0, 0, 0, sample(src, coords).a);
}
                            ",
                out shader_error_message
            );

            base_canvas = GetNativeObject();
            base_is_owned = OwnsNativeObject;
        }

        public static SKColor OverdrawColor = new SKColor(0, 0, 0, 1);
        public static SKColorF OverdrawColorF = new SKColorF(0, 0, 0, (float)(1 / 255.0f));

        public static SKPaint OverdrawPaint(SKPaint paint)
        {
            var p = new SKPaint
            {
                BlendMode = SKBlendMode.Plus,
                Color = OverdrawColor,
            };
            if (paint != null)
            {
                p.Style = paint.Style;
                p.StrokeWidth = paint.StrokeWidth;
            }
            return p;
        }

        public override int Save()
        {
            if (base_canvas.Surface.Context != null)
            {
                // GPU here
                return base.Save();
            }
            // CPU here
            is_layer.Push(false);
            return base.Save();
        }

        public override int SaveLayer(SKRect limit, SKPaint paint)
        {
            if (base_canvas.Surface.Context != null)
            {
                // GPU here
                return base.SaveLayer(limit, paint);
            }
            // CPU here
            if (paint != null && paint.NothingToDraw)
            {
                // no need for the layer (or any of the draws until the matching restore()
                is_layer.Push(false);
                int s = base.Save();
                ClipRect(new SKRect(0, 0, 0, 0));
                return s;
            }
            else
            {
                is_layer.Push(true);
                if (layers.Count == 0)
                {
                    if (ViewRootImpl.LOG_COMMANDS)
                    {
                        Log.d("CPU <- BASE");
                    }
                }
                else
                {
                    if (ViewRootImpl.LOG_COMMANDS)
                    {
                        Log.d("CPU <- CPU");
                    }
                }
                SKBitmap bitmap = new(base_canvas.Info);
                SKCanvas canvas = new SKCanvas(bitmap);
                layers.Push(new(bitmap, canvas));
                ReleaseNativeObject();
                SetNativeObject(canvas, false);
                // increase internal save count
                int s = base.Save();
                ClipRect(limit);
                return s;
            }
        }

        public override int SaveLayer(SKPaint paint)
        {
            if (base_canvas.Surface.Context != null)
            {
                // GPU here
                return base.SaveLayer(paint);
            }
            // CPU here
            return SaveLayer(DeviceClipBounds, paint);
        }

        public override void Restore()
        {
            base.Restore();
            if (base_canvas.Surface.Context != null)
            {
                // GPU here
                return;
            }
            // CPU here
            if (is_layer.Pop())
            {
                if (layers.Count > 0)
                {
                    Layer layer = layers.Pop();
                    ReleaseNativeObject();
                    if (layers.Count == 0)
                    {
                        SetNativeObject(base_canvas, base_is_owned);
                        if (ViewRootImpl.LOG_COMMANDS)
                        {
                            Log.d("CPU -> BASE");
                            var bytes = layer.bitmap.Bytes;
                            for (int i_ = 0; i_ < bytes.Length; i_++)
                            {
                                byte b = bytes[i_];
                                if (b != 0)
                                {
                                    Log.d("bitmap src alpha[" + i_ + "] = " + b);
                                    break;
                                }
                            }
                        }

                        using var srcShader = SKShader.CreateBitmap(layer.bitmap);
                        SKRuntimeEffectChildren children = new(shader_code) { { "src", srcShader } };
                        using var shader = shader_code.ToShader(false, new(shader_code), children);

                        using var shader_paint = new SKPaint();
                        shader_paint.BlendMode = SKBlendMode.Plus;
                        shader_paint.Shader = shader;

                        base.DrawPaint(shader_paint);
                        layer.Dispose();
                    }
                    else
                    {
                        Layer dst = layers.Peek();
                        SetNativeObject(dst.canvas, false);
                        if (ViewRootImpl.LOG_COMMANDS)
                        {
                            Log.d("CPU -> CPU");
                            var bytes = layer.bitmap.Bytes;
                            for (int i_ = 0; i_ < bytes.Length; i_++)
                            {
                                byte b = bytes[i_];
                                if (b != 0)
                                {
                                    Log.d("bitmap src alpha[" + i_ + "] = " + b);
                                    break;
                                }
                            }
                        }

                        using var srcShader = SKShader.CreateBitmap(layer.bitmap);
                        SKRuntimeEffectChildren children = new(shader_code) { { "src", srcShader } };
                        using var shader = shader_code.ToShader(false, new(shader_code), children);

                        using var shader_paint = new SKPaint();
                        shader_paint.BlendMode = SKBlendMode.Plus;
                        shader_paint.Shader = shader;

                        dst.canvas.DrawPaint(shader_paint);
                        layer.Dispose();
                        if (ViewRootImpl.LOG_COMMANDS)
                        {
                            var bytes = dst.bitmap.Bytes;
                            for (int i_ = 0; i_ < bytes.Length; i_++)
                            {
                                byte b = bytes[i_];
                                if (b != 0)
                                {
                                    Log.d("bitmap dst alpha[" + i_ + "] = " + b);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Clear(SKColor color)
        {
            using SKPaint p = new();
            p.Color = color;
            DrawPaint(p);
        }

        public override void Clear(SKColorF color)
        {
            using SKPaint p = new();
            p.ColorF = color;
            DrawPaint(p);
        }

        public override void DrawArc(SKRect oval, float startAngle, float sweepAngle, bool useCenter, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawArc(oval, startAngle, sweepAngle, useCenter, p);
        }

        public override unsafe void DrawAtlas(SKImage atlas, SKRect[] sprites, SKRotationScaleMatrix[] transforms, SKColor[] colors, SKBlendMode mode, SKRect* cullRect, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawAtlas(atlas, sprites, transforms, colors, mode, cullRect, p);
        }

        public override void DrawCircle(float cx, float cy, float radius, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawCircle(cx, cy, radius, p);
        }

        public override void DrawColor(SKColor color, SKBlendMode mode = SKBlendMode.Src)
        {
            using SKPaint p = new();
            p.Color = color;
            DrawPaint(p);
        }

        public override void DrawColor(SKColorF color, SKBlendMode mode = SKBlendMode.Src)
        {
            using SKPaint p = new();
            p.ColorF = color;
            DrawPaint(p);
        }

        public override void DrawImage(SKImage image, float x, float y, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawImage(image, x, y, p);
        }

        public override void DrawImage(SKImage image, SKRect dest, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawImage(image, dest, p);
        }

        public override void DrawImage(SKImage image, SKRect source, SKRect dest, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawImage(image, source, dest, p);
        }

        public override void DrawImageLattice(SKImage image, SKLattice lattice, SKRect dst, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawImageLattice(image, lattice, dst, p);
        }

        public override void DrawImageNinePatch(SKImage image, SKRectI center, SKRect dst, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawImageNinePatch(image, center, dst, p);
        }

        public override void DrawLine(float x0, float y0, float x1, float y1, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawLine(x0, y0, x1, y1, p);
        }

        public override void DrawOval(SKRect rect, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawOval(rect, p);
        }

        public override void DrawPaint(SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPaint(p);
        }

        public override void DrawPatch(SKPoint[] cubics, SKColor[] colors, SKPoint[] texCoords, SKBlendMode mode, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPatch(cubics, colors, texCoords, mode, p);
        }

        public override void DrawPath(SKPath path, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPath(path, p);
        }

        public override void DrawPicture(SKPicture picture, ref SKMatrix matrix, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPicture(picture, ref matrix, p);
        }

        public override void DrawDrawable(SKDrawable drawable, ref SKMatrix matrix)
        {
            using SKCallbackCanvasForwarder f = new(this, false, BaseLayerSize.Width, BaseLayerSize.Height);
            f.DrawDrawable(drawable, ref matrix);
        }

        public override void DrawPicture(SKPicture picture, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            using SKCallbackCanvasForwarder f = new(this, false, BaseLayerSize.Width, BaseLayerSize.Height);
            f.DrawPicture(picture, p);
        }

        public override void DrawPoint(float x, float y, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPoint(x, y, p);
        }

        public override void DrawPoints(SKPointMode mode, SKPoint[] points, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawPoints(mode, points, p);
        }

        public override void DrawRect(float x, float y, float w, float h, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRect(x, y, w, h, p);
        }

        public override void DrawRect(SKRect rect, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRect(rect, p);
        }

        public override void DrawRegion(SKRegion region, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRegion(region, p);
        }

        public override void DrawRoundRect(SKRoundRect rect, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRoundRect(rect, p);
        }

        public override void DrawRoundRect(SKRect rect, float rx, float ry, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRoundRect(rect, rx, ry, p);
        }

        public override void DrawRoundRectDifference(SKRoundRect outer, SKRoundRect inner, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawRoundRectDifference(outer, inner, p);
        }

        public override void DrawSurface(SKSurface surface, float x, float y, SKPaint paint = null)
        {
            using var p = OverdrawPaint(paint);
            base.DrawSurface(surface, x, y, p);
        }

        public override void DrawText(SKTextBlob text, float x, float y, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawText(text, x, y, p);
        }

        public override void DrawVertices(SKVertices vertices, SKBlendMode mode, SKPaint paint)
        {
            using var p = OverdrawPaint(paint);
            base.DrawVertices(vertices, mode, p);
        }
    }
}