using AndroidUI.Applications;
using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Utils;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Data.Common;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace AndroidUI.Hosts.Windows
{
    internal class SkiaGL : SKGLControl
    {
        readonly Host host = new();

        public SkiaGL(Applications.Application application = null) : base(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 8), 4, 0, GraphicsContextFlags.Debug)
        {
            MakeCurrent();

            Console.WriteLine("OpenGL Renderer: " + GL.GetString(StringName.Renderer));
            Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("OpenGL Shading Language Version: " + GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("OpenGL Vendor: " + GL.GetString(StringName.Vendor));

            host.SetApplication(application);

            host.SetInvalidateCallback(Invalidate);

            DpiChangedAfterParent += SkiaGL_DpiChangedAfterParent;

            handleDpiChange();

            host.OnCreate();

            // VSync is disabled by default, who knows why, enable it
            VSync = true;
        }

        private void SkiaGL_DpiChangedAfterParent(object? sender, EventArgs e)
        {
            handleDpiChange();
        }

        void handleDpiChange()
        {
            int dpi = DeviceDpi;
            float density = dpi / dpi;
            host.setDensity(density, dpi);
        }

        class Tmp : SKCanvasForwarder
        {
            class Info
            {
                public SKSurface surface;
                public SKCanvas canvas;
                public SKOverdrawCanvas overdraw;
            }

            Stack<Info> info = new();

            void push(SKCanvas c)
            {
                Info i = new();
                if (c != null)
                {
                    i.surface = SKSurface.Create(c.Surface.Context, false, c.Info);
                    if (i.surface != null)
                    {
                        i.canvas = i.surface.Canvas;
                        i.overdraw = new SKOverdrawCanvas(i.canvas);
                    }
                }
                info.Push(i);
                if (i.overdraw != null)
                {
                    ReleaseNativeObject();
                    SetNativeObject(i.overdraw, false);
                }
            }

            public Tmp(SKCanvas canvas) : base(canvas)
            {
                push(canvas);
            }

            protected override void Dispose(bool disposing)
            {
                var i = info.Pop();
                if (i.overdraw != null)
                {
                    i.overdraw.Dispose();
                }
                if (i.surface != null)
                {
                    i.surface.Dispose();
                }
                ReleaseNativeObject();
                base.Dispose(disposing);
            }

            public override int Save()
            {
                push(null);
                return base.Save();
            }

            public override int SaveLayer(SKPaint paint)
            {
                var i = info.Peek();
                if (i.canvas != null) {
                    push(i.canvas);
                }
                return base.SaveLayer(paint);
            }

            public override void Restore()
            {
                var i = info.Pop();
                if (i.overdraw != null)
                {
                    i.overdraw.Dispose();
                }
                if (i.surface != null)
                {
                    i.surface.Dispose();
                }
                ReleaseNativeObject();
                SetNativeObject(info.Peek().overdraw, false);
                base.Restore();
            }
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            if (true)
            {
                host.OnPaint(GRContext, e.BackendRenderTarget, e.Surface);
            }
            else
            {
                Invalidate();

                // empty
                if (e.Info.BytesSize == 0) return;

                SKCanvas canvas = e.Surface.Canvas;
                canvas.DrawColor(SKColors.Black);

                // bitmap
                if (false)
                {
                    SKImageInfo info = new(1, 1, SKColorType.Alpha8);
                    
                    using SKBitmap src = new(info);
                    using SKCanvas srcCanvas = new(src);
                    
                    using SKBitmap dst = new(info);
                    
                    using SKCanvas dstCanvas = new(dst);

                    using SKPaint p = new() { BlendMode = SKBlendMode.Color, Color = new SKColor(0, 0, 0, 1) };
                    srcCanvas.DrawPoint(0, 0, p);
                    srcCanvas.DrawPoint(0, 0, p);

                    Log.d("BITMAP", "b src[0] = " + src.Bytes[0]);
                    Log.d("BITMAP", "b dst[0] = " + dst.Bytes[0]);

                    dstCanvas.DrawBitmap(src, 0, 0, p);

                    Log.d("BITMAP", "a src[0] = " + src.Bytes[0]);
                    Log.d("BITMAP", "a dst[0] = " + dst.Bytes[0]);
                    return;
                }

                // bitmap alpha
                if (true)
                {
                    SKImageInfo info = new(2, 2, SKColorType.Alpha8);


                    using var offscreenSurface = SKSurface.Create(info);

                    //using SKBitmap dst = new(info);
                    //using SKCanvas dstCanvas = new(dst);

                    SKOverdrawCanvas2 srcOverdraw = new(offscreenSurface.Canvas, false);

                    //Log.d("BITMAP", "b bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                    if (true)
                    {
                        srcOverdraw.SaveLayer();
                        srcOverdraw.SaveLayer();
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Restore();
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Restore();
                    }
                    else
                    {
                        // SaveLayer
                        //SKCanvas old = srcOverdraw.GetNativeObject();
                        //bool oldOwn = srcOverdraw.OwnsNativeObject;
                        //srcOverdraw.Save();
                        //srcOverdraw.ReleaseNativeObject();
                        //using SKBitmap src = new(info);
                        //using SKCanvas srcCanvas = new(src);
                        //srcOverdraw.SetNativeObject(srcCanvas, false);

                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Clear(SKColors.White);

                        // Restore
                        //srcOverdraw.ReleaseNativeObject();
                        //srcOverdraw.SetNativeObject(old, oldOwn);
                        //srcOverdraw.Restore();
                        //using var p = SKOverdrawCanvas4.OverdrawPaint(null);
                        //srcOverdraw.DrawBitmap(src, 0, 0, p);

                    }

                    using SKImage snapshot = offscreenSurface.Snapshot();
                    using SKImage raster = snapshot.ToRasterImage(true);
                    using SKBitmap bitmap = SKBitmap.FromImage(raster);
                    using SKBitmap alpha = bitmap.Copy(SKColorType.Alpha8);
                    Log.d("BITMAP", "a bytes = " + Utils.Arrays.Arrays.toString(alpha.Bytes));
                    return;
                }

                // bitmap alpha
                if (true)
                {
                    SKImageInfo info = new(2, 2, SKColorType.Alpha8);


                    using SKBitmap dst = new(info);
                    using SKCanvas dstCanvas = new(dst);

                    SKOverdrawCanvas2 srcOverdraw = new(dstCanvas, false);

                    Log.d("BITMAP", "b bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                    if (true)
                    {
                        srcOverdraw.SaveLayer();
                        srcOverdraw.SaveLayer();
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Restore();
                        srcOverdraw.Clear(SKColors.White);
                        srcOverdraw.Restore();
                        //Log.d("BITMAP", "dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));
                    }
                    else
                    {
                        // SaveLayer
                        SKCanvas old = srcOverdraw.GetNativeObject();
                        bool oldOwn = srcOverdraw.OwnsNativeObject;
                        srcOverdraw.Save();
                        srcOverdraw.ReleaseNativeObject();
                        using SKBitmap src = new(info);
                        using SKCanvas srcCanvas = new(src);
                        srcOverdraw.SetNativeObject(srcCanvas, false);
                        Log.d("BITMAP", "save1 src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "save1 dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                        // SaveLayer

                        srcOverdraw.Save();
                        srcOverdraw.ReleaseNativeObject();
                        using SKBitmap src2 = new(info);
                        using SKCanvas srcCanvas2 = new(src2);
                        srcOverdraw.SetNativeObject(srcCanvas2, false);
                        Log.d("BITMAP", "save2 src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "save2 src2 bytes = " + Utils.Arrays.Arrays.toString(src2.Bytes));
                        Log.d("BITMAP", "save2 dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                        // draw
                        srcOverdraw.Clear(SKColors.White);
                        Log.d("BITMAP", "draw src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "draw src2 bytes = " + Utils.Arrays.Arrays.toString(src2.Bytes));
                        Log.d("BITMAP", "draw dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                        // Restore
                        srcOverdraw.ReleaseNativeObject();
                        srcOverdraw.SetNativeObject(srcCanvas, false);
                        srcOverdraw.Restore();

                        using var p1 = SKOverdrawCanvas2.OverdrawPaint(null);
                        srcOverdraw.DrawBitmap(src2, 0, 0, p1);
                        Log.d("BITMAP", "restore2 src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "restore2 src2 bytes = " + Utils.Arrays.Arrays.toString(src2.Bytes));
                        Log.d("BITMAP", "restore2 dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                        // draw
                        srcOverdraw.Clear(SKColors.White);
                        Log.d("BITMAP", "draw2 src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "draw2 src2 bytes = " + Utils.Arrays.Arrays.toString(src2.Bytes));
                        Log.d("BITMAP", "draw2 dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));

                        // Restore
                        srcOverdraw.ReleaseNativeObject();
                        srcOverdraw.SetNativeObject(old, oldOwn);
                        srcOverdraw.Restore();

                        using var p = SKOverdrawCanvas2.OverdrawPaint(null);
                        //srcOverdraw.DrawBitmap(src, 0, 0, p);

                        // build shader

                        using var sksl = SKRuntimeEffect.Create(
                            @"
                            uniform shader src;

                            half4 main(float2 coords) {
                                return half4(0, 0, 0, sample(src, coords).a);
                            }
                            ",
                            out var shader_error_message
                        );

                        Log.d("SHADER", "message: " + shader_error_message);

                        using var srcShader = src.ToShader();
                        SKRuntimeEffectChildren children = new(sksl) {
                            { "src", srcShader }
                        };
                        using var shader = sksl.ToShader(false, new(sksl), children);

                        using var pp = new SKPaint();
                        pp.BlendMode = SKBlendMode.Plus;
                        pp.Shader = shader;


                        dstCanvas.DrawPaint(pp);

                        Log.d("BITMAP", "restore1 src bytes = " + Utils.Arrays.Arrays.toString(src.Bytes));
                        Log.d("BITMAP", "restore1 src2 bytes = " + Utils.Arrays.Arrays.toString(src2.Bytes));
                        Log.d("BITMAP", "restore1 dst bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));
                    }
                    Log.d("BITMAP", "a bytes = " + Utils.Arrays.Arrays.toString(dst.Bytes));
                    return;
                }

                if (false)
                {
                    SKBitmap DrawAlpha(int count, int w, int h, Action<SKCanvas, SKPaint> drawFunc, Func<SKBitmap, SKCanvas> createCanvas, Func<SKCanvas, SKCanvas> transformCanvas)
                    {

                        using SKPaint alphaPaint = new SKPaint();
                        alphaPaint.BlendMode = SKBlendMode.Plus;
                        float[] kIncrementAlpha = new float[] {
                                0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                                0.0f, 0.0f, 0.0f, 0.0f, 1.0f/255,
                        };
                        using var alphaMatrix = SKColorFilter.CreateColorMatrix(kIncrementAlpha);
                        alphaPaint.ColorFilter = alphaMatrix;

                        var alphaBitmap = new SKBitmap(w, h, SKColorType.Alpha8, SKAlphaType.Premul);
                        using var canvas = transformCanvas(createCanvas(alphaBitmap));
                        for (int i = 0; i < count; i++)
                        {
                            drawFunc(canvas, alphaPaint);
                        }
                        canvas.Flush();
                        return alphaBitmap;
                    }

                    using var AlphaBitmap0 = DrawAlpha(0, 1, 1, (c, p) => c.DrawPoint(0, 0, p), b => new SKCanvas(b), c => c);
                    using var AlphaBitmap1 = DrawAlpha(1, 1, 1, (c, p) => c.DrawPoint(0, 0, p), b => new SKCanvas(b), c => c);
                    return;
                }
                if (false)
                {
                    void drawText(SKCanvas canvas, int n, int x, int y)
                    {
                        Topten.RichTextKit.TextBlock block = new();
                        Topten.RichTextKit.Style style = new();
                        style.TextColor = SKColors.Silver;
                        style.FontSize = 20;
                        for (int i = 0; i < n; i++)
                        {
                            string text = "drawn " + n + " time";
                            if (i != 0) text += "s";
                            block.Clear();
                            block.AddText(text, style);
                            block.Paint(canvas, new SKPoint(x, y));
                            canvas.Flush();
                        }
                    }

                    void drawMatrix(int count, int max_lines, int spacing)
                    {
                        max_lines++;
                        int n = 0;
                        int column = 0;
                        int line = 1;
                        for (int i = 0; i < count; i++)
                        {
                            n = i + 1;
                            if (line == max_lines)
                            {
                                line = 1;
                                column += spacing;
                            }
                            //int s = canvas.Save();
                            drawText(canvas, n, column, 20 * line);
                            //canvas.RestoreToCount(s);
                            line++;
                        }
                    }

                    drawMatrix(20, 20, 50);
                    return;
                }
                if (false)
                {
                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half4 color = sample(input);\n" +
                        "    float alpha = 128 / 255.0;\n" +
                        // R G B A
                        "    vec4 tblue = vec4(alpha * 0, alpha * 0, alpha * 1, alpha * 1);\n" +
                        "    vec4 tgreen = vec4(alpha * 0, alpha * 1, alpha * 0, alpha * 1);\n" +
                        "    return vec4(tblue.rgb + tgreen.rgb, tblue.a * tgreen.a);\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("OVERDRAW", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    using SKPaint colorPaint = new();
                    colorPaint.Shader = sksl.ToShader(false, new(sksl), new(sksl));
                    sksl.Dispose();
                    canvas.DrawPaint(colorPaint);
                    colorPaint.Shader.Dispose();
                    return;
                }
                if (false)
                {
                    using SKPaint colorPaint = new();
                    colorPaint.Shader = SKShader.CreateColor(SKColors.Blue.WithAlpha(128));
                    canvas.DrawPaint(colorPaint);
                    colorPaint.Shader.Dispose();
                    canvas.Flush();

                    using var image = e.Surface.Snapshot();

                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half4 color = sample(input);\n" +
                        // convert float byte to float between 0.0 - 1.0
                        "    float alpha = 128 / 255.0;\n" +
                        // our sampled color is premultiplied so we need to premul our mixing color
                        // R G B A
                        "    vec4 tgreen = vec4(alpha * 0, alpha * 1, alpha * 0, alpha * 1);\n" +
                        // colors are blended via Multiply blend mode, sum rgb and mul alpha
                        "    return vec4(color.rgb + tgreen.rgb, color.a * tgreen.a);\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    var imageShader = image.ToShader();

                    SKRuntimeEffectChildren child = new(sksl) { { "input", imageShader } };

                    var ourShader = sksl.ToShader(false, new(sksl), child);

                    sksl.Dispose();
                    imageShader.Dispose();

                    // we only want to write our paint shader's output pixel to the canvas
                    // this is the same as if the canvas was cleared before painting the shader
                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                if (false)
                {
                    SKImageInfo offscreenInfo = new(canvas.BaseLayerSize.Width, canvas.BaseLayerSize.Height);
                    using var offscreenSurface = SKSurface.Create(offscreenInfo);
                    using SKOverdrawCanvas overdrawCanvas = new(offscreenSurface.Canvas);

                    using SKPaint colorPaint = new();
                    using var silverPaint = new SKPaint();
                    for (int i = 0; i < 128; i++)
                    {
                        overdrawCanvas.DrawText("colorPaint", 50, 50, silverPaint);
                    }
                    overdrawCanvas.Flush();

                    using var image = offscreenSurface.Snapshot();

                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half4 color = sample(input);\n" +
                        // convert float byte to float between 0.0 - 1.0
                        //"    float alpha = 128 / 255.0;\n" +
                        // our sampled color is premultiplied so we need to premul our mixing color
                        // R G B A
                        "    vec4 filter = vec4(color.a, color.a, color.a, 1);\n" +
                        // colors are blended via Multiply blend mode, sum rgb and mul alpha
                        "    return filter;\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    var imageShader = image.ToShader();

                    SKRuntimeEffectChildren child = new(sksl) { { "input", imageShader } };

                    var ourShader = sksl.ToShader(false, new(sksl), child);

                    sksl.Dispose();
                    imageShader.Dispose();

                    // we only want to write our paint shader's output pixel to the canvas
                    // this is the same as if the canvas was cleared before painting the shader
                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                if (false)
                {
                    using var gradientShader = SKShader.CreateLinearGradient(
                        // start
                        new SKPoint(0, 0),
                        // end
                        new SKPoint(0, 255),
                        // colors
                        new SKColor[]
                        {
                            // light blue
                            new SKColorF(0, 0.5f, 0.75f).ToSKColor(),
                            // blueish-whitish
                            new SKColorF(0.37f, 0.5f, 0.75f).ToSKColor(),
                            // light orange
                            new SKColor(243, 158, 95),
                            // orange-redish
                            new SKColorF(1f, 0.28f, 0).ToSKColor(),
                            // red
                            new SKColorF(1f, 0, 0).ToSKColor(),
                        },
                        // distribution (color pos from 0 to 1)
                        new float[] { 0, 0.038f, 0.082f, 0.55f, 1 },
                        SKShaderTileMode.Clamp
                    );
                    using var gradientPaint = new SKPaint();
                    gradientPaint.Shader = gradientShader;
                    canvas.DrawRect(0, 0, 255, 255, gradientPaint);
                    return;
                }
                if (false)
                {
                    // to improve overdraw quality we only apply overdraw to non transparent final output pixels
                    // this means we need to draw twice, once in full color, another in alpha
                    // if the full color pixel has an alpha of zero we discard the result
                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "uniform shader inputAlpha;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half4 color = sample(input);\n" +
                        "    if (color.a == 0) return vec4(0,0,0,0);\n" +
                        "    half alpha = 255.0 * sample(inputAlpha).a;\n" +
                        // our sampled color is premultiplied so we need to premul our mixing color
                        "    if (alpha <= 0) return color;\n" +
                        // simple heatmap
                        // R G B A
                        "    if (alpha <= 1) return half4(0, 0.5, 0.75, 1);\n" +
                        "    if (alpha <= 4) return half4(1, 1, 0, 1);\n" +
                        "    if (alpha <= 7) return half4(1, 0.8, 0, 1);\n" +
                        "    if (alpha <= 12) return half4(1, 0.4, 0, 1);\n" +
                        "    return half4(1, 0, 0, 1);\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    int w = canvas.BaseLayerSize.Width;
                    int h = canvas.BaseLayerSize.Height;
                    SKImageInfo offscreenInfo = new(w, h);
                    SKImageInfo offscreenAlphaInfo = new(w, h, SKColorType.Alpha8);
                    using var offscreenSurface = SKSurface.Create(offscreenInfo);
                    using var offscreenAlphaSurface = SKSurface.Create(offscreenAlphaInfo);
                    using SKCanvas imageCanvas = offscreenSurface.Canvas;
                    using SKOverdrawCanvas overdrawCanvas = new(offscreenAlphaSurface.Canvas);
                    using SKNWayCanvas nWayCanvas = new(w, h);
                    nWayCanvas.AddCanvas(overdrawCanvas);
                    nWayCanvas.AddCanvas(imageCanvas);

                    using SKPaint colorPaint = new();

                    void drawText(SKCanvas canvas, int n, int x, int y) {
                        using var paint = new SKPaint();
                        paint.Color = SKColors.Silver;
                        for (int i = 0; i < n; i++)
                        {
                            string text = "drawn " + n + " time";
                            if (i != 1) text += "s";
                            canvas.DrawText(text, x, y, paint);
                        }
                    }
                    int n = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        n = i + 1;
                        drawText(nWayCanvas, n, 50, 12 * n);
                    }
                    n++;
                    drawText(nWayCanvas, 200, 50, 12*n);
                    nWayCanvas.Flush();

                    using var imageAlpha = offscreenAlphaSurface.Snapshot();
                    using var image = offscreenSurface.Snapshot();
                    var imageAlphaShader = imageAlpha.ToShader();
                    var imageShader = image.ToShader();


                    SKRuntimeEffectChildren children = new(sksl) {
                        { "input", imageShader },
                        { "inputAlpha", imageAlphaShader }
                    };

                    var ourShader = sksl.ToShader(false, new(sksl), children);

                    sksl.Dispose();
                    imageAlphaShader.Dispose();
                    imageShader.Dispose();

                    // we only want to write our paint shader's output pixel to the canvas
                    // this is the same as if the canvas was cleared before painting the shader
                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                if (false)
                {
                    // create an alpha canvas
                    SKImageInfo offscreenAlphaInfo = new(255, 255, SKColorType.Alpha8);
                    using var offscreenAlphaSurface = SKSurface.Create(offscreenAlphaInfo);

                    using var alphaShader = SKShader.CreateLinearGradient(
                        // start
                        new SKPoint(0, 0),
                        // end
                        new SKPoint(0, 255),
                        // colors
                        new SKColor[]
                        {
                            new SKColor(0, 0, 0, 0),
                            new SKColor(0, 0, 0, 255)
                        },
                        SKShaderTileMode.Clamp
                    );
                    using var alphaPaint = new SKPaint();
                    alphaPaint.Shader = alphaShader;
                    offscreenAlphaSurface.Canvas.DrawRect(0, 0, 255, 255, alphaPaint);
                    using var imageAlpha = offscreenAlphaSurface.Snapshot();
                    var imageAlphaShader = imageAlpha.ToShader();

                    // create gradient shader

                    using var gradientShader = SKShader.CreateLinearGradient(
                        // start
                        new SKPoint(0, 0),
                        // end
                        new SKPoint(0, 255),
                        // colors
                        new SKColor[]
                        {
                            // light blue
                            new SKColorF(0, 0.5f, 0.75f).ToSKColor(),
                            // blueish-whitish
                            new SKColorF(0.37f, 0.5f, 0.75f).ToSKColor(),
                            // light orange
                            new SKColor(243, 158, 95),
                            // orange-redish
                            new SKColorF(1f, 0.28f, 0).ToSKColor(),
                            // red
                            new SKColorF(1f, 0, 0).ToSKColor(),
                        },
                        // distribution (color pos from 0 to 1)
                        new float[] { 0, 0.038f, 0.082f, 0.55f, 1 },
                        SKShaderTileMode.Clamp
                    );

                    // apply gradient to alpha shader

                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader inputAlpha;\n" +
                        "uniform shader inputGradient;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half alpha = 255.0 * sample(inputAlpha).a;\n" +
                        // our sampled color is premultiplied so we need to premul our mixing color
                        "    if (alpha <= 0) return half4(0,0,0,0);\n" +
                        "    return sample(inputGradient, float2(0, alpha));\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    SKRuntimeEffectChildren children = new(sksl) {
                        { "inputAlpha", imageAlphaShader },
                        { "inputGradient", gradientShader },
                    };

                    var heatShader = sksl.ToShader(false, new(sksl), children);

                    int w = canvas.BaseLayerSize.Width;
                    int h = canvas.BaseLayerSize.Height;
                    using var heatPaint = new SKPaint();
                    heatPaint.Shader = heatShader;
                    // we only want to write our paint shader's output pixel to the canvas
                    // this is the same as if the canvas was cleared before painting the shader
                    heatPaint.BlendMode = SKBlendMode.Src;
                    canvas.DrawPaint(heatPaint);
                    return;
                }
                // ALPHA8
                if (false)
                {
                    bool RedIsAlpha = canvas.Surface.Context.Should_Convert_Alpha8_To_R8();
                    SKImageInfo _IMAGE_INFO = new(1, 1, SKColorType.Alpha8);
                    // MakeRenderTarget
                    var _SURFACE_GPU = SKSurface.Create(canvas.Surface.Context, false, _IMAGE_INFO);
                    var _color = new SKColor((byte)(RedIsAlpha ? 255 : 0), 0, 0, (byte)(RedIsAlpha ? 0 : 255));
                    _SURFACE_GPU.Canvas.DrawColor(_color);
                    // makeImageSnapshot
                    var _IMAGE_GPU = _SURFACE_GPU.Snapshot();
                    // makeRasterImage
                    var _raster = _IMAGE_GPU.ToRasterImage(true);
                    SKPixmap _pixmap = new();
                    // peekPixels
                    _raster.PeekPixels(_pixmap);
                    // addr
                    unsafe
                    {
                        // addr
                        IntPtr _p = _pixmap.GetPixels();
                        void* _ptr = _p.ToPointer();
                        byte* _bytes = (byte*)_ptr;
                        Console.WriteLine("bytes 0 : " + _bytes[0]);
                    }
                    // addr
                    var _GPU_BYTES = _pixmap.GetPixelSpan();
                    Console.WriteLine("GPU BYTES LENGTH: " + _GPU_BYTES.Length);
                    string _c = "GPU BYTES CONTENT: ";
                    for (int i = 0; i < _GPU_BYTES.Length; i++)
                    {
                        _c += _GPU_BYTES[i];
                        if ((i + 1) != _GPU_BYTES.Length)
                        {
                            _c += ", ";
                        }
                    }
                    Console.WriteLine(_c);
                    _SURFACE_GPU.Dispose();
                    _IMAGE_GPU.Dispose();
                    _pixmap.Dispose();
                    _raster.Dispose();
                    return;


                    SKShader createAlphaGradientShader()
                    {
                        return SKShader.CreateLinearGradient(
                            // start
                            new SKPoint(0, 0),
                            // end
                            new SKPoint(0, 255),
                            // colors
                            new SKColor[]
                            {
                            // lighter orange
                            new SKColor(249, 205, 172),
                            // red
                            new SKColorF(1f, 0, 0).ToSKColor(),
                            },
                            // distribution (color pos from 0 to 1)
                            new float[] { 0, 0.05f },
                            SKShaderTileMode.Clamp
                        );
                    }

                    using var gradient = createAlphaGradientShader();

                    // to improve overdraw quality we only apply overdraw to non transparent final output pixels
                    // this means we need to draw twice, once in full color, another in alpha
                    // if the full color pixel has an alpha of zero we discard the result
                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "uniform shader inputAlpha;\n" +
                        "uniform shader inputGradient;\n" +
                        "\n" +
                        "half4 main(float2 coords) {\n" +
                        "    half4 color = sample(input, coords);\n" +
                        // return zero if alpha is zero
                        "    if (color.a == 0) return vec4(0,0,0,0);\n" +
                        "    int alpha = (int)(sample(inputAlpha, coords).a * 255.0 + 0.5);\n" +
                        // return color if input alpha is 1, this means we only drawn this pixel once
                        // Skia's overdraw canvas 
                        // R G B A
                        "    if (alpha == 1) {\n" +
                        // apply greyscale to the overdraw canvas in order to isolate the overdraw colors
                        "       return half4(vec3((color.r + color.g + color.b) / 3), 1);\n" +
                        "    }\n" +
                        // gradient heatmap
                        "    return sample(inputGradient, float2(0, alpha));\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    int w = canvas.BaseLayerSize.Width;
                    int h = canvas.BaseLayerSize.Height;

                    SKImageInfo offscreenInfo = new(w, h);
                    using var offscreenSurface = SKSurface.Create(GRContext, false, offscreenInfo);
                    using SKCanvas imageCanvas = offscreenSurface.Canvas;

                    SKImageInfo offscreenAlphaInfo = new(w, h, SKColorType.Alpha8);
                    using var offscreenAlphaSurface = SKSurface.Create(GRContext, false, offscreenAlphaInfo);
                    offscreenAlphaSurface.Canvas.Clear(new SKColor(0, 0, 0, 255));
                    offscreenAlphaSurface.Canvas.Flush();
                    offscreenAlphaSurface.Flush(true, true);
                    var a3 = offscreenAlphaSurface.PeekPixels();
                    var a4 = SKImage.FromPixels(a3);
                    var b1 = SKBitmap.FromImage(a4);
                    a3.Dispose();
                    a4.Dispose();
                    var b2 = b1.Bytes;
                    b1.Dispose();


                    //using SKOverdrawCanvas2 overdrawCanvas = new(offscreenAlphaSurface.Canvas);

                    //using var sf = new SKCallbackCanvasForwarder(overdrawCanvas, false, w, h);

                    //offscreenAlphaSurface.Canvas.Translate(0, 12);
                    //offscreenAlphaSurface.Canvas.Restore();
                    //offscreenSurface.Canvas.Translate(0, 12);
                    //offscreenAlphaSurface.Canvas.DrawText("hi", 0, 12, new SKColor(0, 0, 0, 1).ToPaint());
                    //offscreenSurface.Canvas.DrawText("hi", 0, 12, new SKColor(255, 255, 255, 255).ToPaint());
                    //offscreenSurface.Canvas.Restore();
                    //offscreenAlphaSurface.Canvas.Flush();
                    //offscreenSurface.Canvas.Flush();
                    //using SKNWayCanvas nWayCanvas = new(w, h);
                    //nWayCanvas.AddCanvas(sf);
                    //nWayCanvas.AddCanvas(imageCanvas);
                    //nWayCanvas.DrawText("hi", 0, 12, new SKColor(255, 255, 0, 255).ToPaint());
                    //nWayCanvas.Flush();

                    using SKPaint colorPaint = new();

                    //void drawText(int n, int x, int y)
                    //{
                    //    Log.d("TASK", "ENTER drawText(" + n + ", " + x + ", " + y + ")");
                    //    using var paint = new SKPaint();
                    //    paint.Color = SKColors.Silver;
                    //    for (int i = 0; i < n; i++)
                    //    {
                    //        string text = "drawn " + n + " time";
                    //        if (i != 0) text += "s";
                    //        //canvas.Save();
                    //        //canvas.Translate(x, y);
                    //        //canvas.DrawText(text, x, y, paint);
                    //        //canvas.Restore();

                    //        Topten.RichTextKit.TextBlock block = new();
                    //        Topten.RichTextKit.Style style = new();
                    //        block.AddText(text, null);
                    //        //style.TextColor = new SKColor(0, 0, 0, (byte)(i + 1));
                    //        style.FontFamily = "Arial";
                    //        style.FontSize = 20;
                    //        style.TextColor = SKColors.Silver;
                    //        block.ApplyStyle(0, text.Length, style);

                    //        overdrawCanvas.SaveLayer();
                    //        block.Paint(overdrawCanvas, new SKPoint(x, y));
                    //        overdrawCanvas.Restore();

                    //        //block.ApplyStyle(0, text.Length, style);
                    //        offscreenSurface.Canvas.SaveLayer();
                    //        block.Paint(offscreenSurface.Canvas, new SKPoint(x, y));
                    //        offscreenSurface.Canvas.Restore();

                    //        //offscreenAlphaSurface.Canvas.Save();
                    //        //offscreenAlphaSurface.Canvas.Translate(x, y);
                    //        //offscreenAlphaSurface.Canvas.DrawText(text, 0, 0, p);
                    //        //offscreenAlphaSurface.Canvas.Restore();

                    //        //offscreenSurface.Canvas.Save();
                    //        //offscreenSurface.Canvas.Translate(x, y);
                    //        //offscreenSurface.Canvas.DrawText(text, 0, 0, paint);
                    //        //offscreenSurface.Canvas.Restore();
                    //    }
                    //    Log.d("TASK", "EXIT drawText(" + n + ", " + x + ", " + y + ")");
                    //}

                    //void drawMatrix(int count, int max_lines, int spacing)
                    //{
                    //    max_lines++;
                    //    int n = 0;
                    //    int column = 0;
                    //    int line = 1;
                    //    for (int i = 0; i < count; i++)
                    //    {
                    //        n = i + 1;
                    //        if (line == max_lines)
                    //        {
                    //            line = 1;
                    //            column += spacing;
                    //        }
                    //        drawText(n, column, 20 * line);
                    //        line++;
                    //    }
                    //}

                    //drawMatrix(1, 20, 160);
                    //drawText(nWayCanvas, 1, 0, 12 * 1);

                    //nWayCanvas.DrawColor(SKColors.Green);

                    //nWayCanvas.Flush();

                    //offscreenAlphaSurface.Flush(true, true);

                    using var pixmapAlpha = offscreenAlphaSurface.PeekPixels();
                    using var pixmap = offscreenSurface.PeekPixels();

                    using var imageAlpha = pixmapAlpha == null ? offscreenAlphaSurface.Snapshot() : SKImage.FromPixelCopy(pixmapAlpha);
                    using var image = pixmap == null ? offscreenSurface.Snapshot() : SKImage.FromPixelCopy(pixmap);

                    SKBitmap bitmap;
                    byte[] bytes;

                    bitmap = SKBitmap.FromImage(imageAlpha);
                    bytes = bitmap.Bytes;
                    for (int i_ = 0; i_ < bytes.Length; i_++)
                    {
                        byte b = bytes[i_];
                        if (b != 0)
                        {
                            Log.d("SKGL", "IMAGE_ALPHA alpha[" + i_ + "] = " + b);
                            break;
                        }
                    }
                    bitmap.Dispose();

                    bitmap = SKBitmap.FromImage(imageAlpha);
                    bytes = bitmap.Bytes;
                    for (int i_ = 0; i_ < bytes.Length; i_++)
                    {
                        byte b = bytes[i_];
                        if (b != 0)
                        {
                            Log.d("SKGL", "IMAGE alpha[" + i_ + "] = " + b);
                            break;
                        }
                    }
                    bitmap.Dispose();

                    var imageAlphaShader = imageAlpha.ToShader();
                    var imageShader = image.ToShader();

                    SKRuntimeEffectChildren children = new(sksl) {
                        { "input", imageShader },
                        { "inputAlpha", imageAlphaShader },
                        { "inputGradient", gradient },
                    };

                    var ourShader = sksl.ToShader(false, new(sksl), children);

                    sksl.Dispose();
                    imageAlphaShader.Dispose();
                    imageShader.Dispose();

                    // we only want to write our paint shader's output pixel to the canvas
                    // this is the same as if the canvas was cleared before painting the shader
                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                // overdraw2
                if (true)
                {
                    SKShader createAlphaGradientShader()
                    {
                        return SKShader.CreateLinearGradient(
                            // start
                            new SKPoint(0, 0),
                            // end
                            new SKPoint(0, 255),
                            // colors
                            new SKColor[]
                            {
                                // lighter orange
                                new SKColor(249, 205, 172),
                                // red
                                new SKColorF(1f, 0, 0).ToSKColor(),
                            },
                            // distribution (color pos from 0 to 1)
                            new float[] { 0, 0.05f },
                            SKShaderTileMode.Clamp
                        );
                    }

                    using var gradient = createAlphaGradientShader();

                    // to improve overdraw quality we only apply overdraw to non transparent final output pixels
                    // this means we need to draw twice, once in full color, another in alpha
                    // if the full color pixel has an alpha of zero we discard the result
                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "uniform shader inputAlpha;\n" +
                        "uniform shader inputGradient;\n" +
                        "\n" +
                        "half4 main(float2 coords) {\n" +
                        "    half4 color = sample(input, coords);\n" +
                        // return zero if alpha is zero
                        "    if (color.a == 0) return vec4(0,0,0,0);\n" +
                        "    int alpha = (int)(sample(inputAlpha, coords).a * 255.0 + 0.5);\n" +
                        // return color if input alpha is 1, this means we only drawn this pixel once
                        // Skia's overdraw canvas 
                        // R G B A
                        "    if (alpha == 1) {\n" +
                        // apply greyscale to the overdraw canvas in order to isolate the overdraw colors
                        "       return half4(vec3((color.r + color.g + color.b) / 3), 1);\n" +
                        "    }\n" +
                        // gradient heatmap
                        "    return sample(inputGradient, float2(0, alpha));\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    int w = canvas.BaseLayerSize.Width;
                    int h = canvas.BaseLayerSize.Height;
                    SKImageInfo offscreenInfo = new(w, h);
                    SKImageInfo offscreenAlphaInfo = new(w, h, SKColorType.Alpha8);
                    SKSurface offscreenSurface;
                    SKSurface offscreenAlphaSurface;
                    if (true)
                    {
                        offscreenSurface = SKSurface.Create(GRContext, false, offscreenInfo);
                        offscreenAlphaSurface = SKSurface.Create(GRContext, false, offscreenAlphaInfo);
                    }
                    else
                    {
                        offscreenSurface = SKSurface.Create(offscreenInfo);
                        offscreenAlphaSurface = SKSurface.Create(offscreenAlphaInfo);
                    }
                    using SKCanvas imageCanvas = offscreenSurface.Canvas;
                    using SKOverdrawCanvas2 overdrawCanvas = new(offscreenAlphaSurface.Canvas);
                    using var SF = new SKCallbackCanvasForwarder(new LoggingCanvas(overdrawCanvas, false, true), true, w, h);
                    using SKNWayCanvas nWayCanvas = new(w, h);
                    nWayCanvas.AddCanvas(SF);
                    nWayCanvas.AddCanvas(imageCanvas);

                    using SKPaint colorPaint = new();

                    nWayCanvas.SaveLayer(SKColors.White.ToPaint());
                    //nWayCanvas.DrawRect(new SKRect(0, 0, 800, 427), SKColors.DarkGreen.ToPaint());
                    nWayCanvas.Restore();
                    nWayCanvas.SaveLayer(SKColors.White.ToPaint());
                    //nWayCanvas.DrawPaint(SKColors.Black.ToPaint());
                    //nWayCanvas.DrawRect(new SKRect(0, 0, 800, 427), SKColors.DarkGreen.ToPaint());
                    nWayCanvas.DrawRect(new SKRect(0, 0, 800, 427), SKColors.DarkGreen.ToPaint());
                    nWayCanvas.Restore();

                    nWayCanvas.Flush();

                    using var imageAlpha = offscreenAlphaSurface.Snapshot();
                    using var image = offscreenSurface.Snapshot();

                    SKPixmap pixelsAlpha;
                    ReadOnlySpan<byte> readOnlySpan;

                    SKImage dstImage;
                    dstImage = imageAlpha.ToRasterImage(true);
                    pixelsAlpha = dstImage.PeekPixels();
                    readOnlySpan = pixelsAlpha.GetPixelSpan();
                    Log.d("SRC", "src length " + readOnlySpan.Length);
                    for (int i_ = 0; i_ < readOnlySpan.Length; i_++)
                    {
                        byte b = readOnlySpan[i_];
                        if (b != 0)
                        {
                            Log.d("SRC", "src alpha[" + i_ + "] = " + b);
                            break;
                        }
                    }
                    pixelsAlpha.Dispose();
                    dstImage.Dispose();

                    imageCanvas.Dispose();
                    overdrawCanvas.Dispose();
                    SF.Dispose();
                    offscreenAlphaSurface.Dispose();
                    offscreenSurface.Dispose();
                    nWayCanvas.Dispose();

                    var imageAlphaShader = imageAlpha.ToShader();
                    var imageShader = image.ToShader();

                    SKRuntimeEffectChildren children = new(sksl) {
                        { "input", imageShader },
                        { "inputAlpha", imageAlphaShader },
                        { "inputGradient", gradient },
                    };

                    var ourShader = sksl.ToShader(false, new(sksl), children);

                    sksl.Dispose();
                    imageAlphaShader.Dispose();
                    imageShader.Dispose();

                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                // text
                if (true)
                {
                    SKShader createAlphaGradientShader()
                    {
                        return SKShader.CreateLinearGradient(
                            // start
                            new SKPoint(0, 0),
                            // end
                            new SKPoint(0, 255),
                            // colors
                            new SKColor[]
                            {
                            // lighter orange
                            new SKColor(249, 205, 172),
                            // red
                            new SKColorF(1f, 0, 0).ToSKColor(),
                            },
                            // distribution (color pos from 0 to 1)
                            new float[] { 0, 0.05f },
                            SKShaderTileMode.Clamp
                        );
                    }

                    using var gradient = createAlphaGradientShader();

                    // to improve overdraw quality we only apply overdraw to non transparent final output pixels
                    // this means we need to draw twice, once in full color, another in alpha
                    // if the full color pixel has an alpha of zero we discard the result
                    var sksl = SKRuntimeEffect.Create(
                        "uniform shader input;\n" +
                        "uniform shader inputAlpha;\n" +
                        "uniform shader inputGradient;\n" +
                        "\n" +
                        "half4 main() {\n" +
                        "    half4 color = sample(input);\n" +
                        // return zero if alpha is zero
                        "    if (color.a == 0) return vec4(0,0,0,0);\n" +
                        "    int alpha = 255.0 * sample(inputAlpha).a;\n" +
                        // return color if input alpha is 0, this means we only drawn this pixel once
                        // Skia's overdraw canvas increases the alpha of a pixel each time it drawn touched
                        // R G B A
                        "    if (alpha == 0) {\n" +
                        // apply greyscale to the overdraw canvas in order to isolate the overdraw colors
                        "       return half4(vec3((color.r + color.g + color.b) / 3), 1);\n" +
                        "    }\n" +
                        //"    return half4(1,0,0,1);\n" +
                        //// gradient heatmap
                        "    return sample(inputGradient, float2(0, alpha));\n" +
                        "}\n",
                        out string err
                    );

                    if (err != null)
                    {
                        Log.d("SHADER", "runtime effect compiled with errors: " + err);
                        return;
                    }

                    int w = canvas.BaseLayerSize.Width;
                    int h = canvas.BaseLayerSize.Height;
                    SKImageInfo offscreenInfo = new(w, h);
                    SKImageInfo offscreenAlphaInfo = new(w, h, SKColorType.Alpha8);
                    SKSurface offscreenSurface;
                    SKSurface offscreenAlphaSurface;
                    if (false)
                    {
                        offscreenSurface = SKSurface.Create(GRContext, false, offscreenInfo);
                        offscreenAlphaSurface = SKSurface.Create(GRContext, false, offscreenAlphaInfo);
                    }
                    else
                    {
                        offscreenSurface = SKSurface.Create(offscreenInfo);
                        offscreenAlphaSurface = SKSurface.Create(offscreenAlphaInfo);
                    }
                    using SKCanvas imageCanvas = offscreenSurface.Canvas;
                    using SKOverdrawCanvas2 overdrawCanvas = new(offscreenAlphaSurface.Canvas);
                    using var SF = new SKCallbackCanvasForwarder(new LoggingCanvas(overdrawCanvas, false, false), true, w, h);
                    using SKNWayCanvas nWayCanvas = new(w, h);
                    nWayCanvas.AddCanvas(SF);
                    nWayCanvas.AddCanvas(imageCanvas);

                    using SKPaint colorPaint = new();

                    var textSize = 10;
                    int prevY = 0;

                    void drawText(SKCanvas canvas, int n, int x, int y)
                    {
                        if (y == 0)
                        {
                            prevY = 0;
                        }
                        for (int i = 0; i < n; i++)
                        {
                            string text = "drawn " + n + " time";
                            if (i != 0) text += "s";
                            if (true)
                            {
                                Topten.RichTextKit.TextBlock block = new();
                                Topten.RichTextKit.Style style = new();
                                style.TextColor = new SKColor(0, 255, 0, 255);
                                style.FontFamily = "Arial";
                                style.FontSize = textSize + n;
                                block.AddText(text, style);
                                block.Paint(canvas, new SKPoint(x, prevY));
                            }
                            else
                            {
                                SKTypeface t = SKTypeface.FromFamilyName("Arial");
                                SKFont f = t.ToFont();

                                using var paint = new SKPaint(f);
                                paint.Color = new SKColor(0, 255, 0, 255);
                                paint.TextSize = textSize + n;
                                canvas.Save();
                                canvas.DrawText(text, new SKPoint(x, paint.TextSize + prevY), paint);
                                canvas.Restore();
                            }
                        }
                        prevY += textSize + (n - 1);
                    }

                    void drawMatrix(SKCanvas canvas, int count, int max_lines, int spacing)
                    {
                        max_lines++;
                        int n = 0;
                        int column = 0;
                        int line = 1;
                        for (int i = 0; i < count; i++)
                        {
                            n = i + 1;
                            if (line == max_lines)
                            {
                                line = 1;
                                column += spacing;
                            }
                            drawText(canvas, n, column, line-1);
                            line++;
                        }
                    }

                    //Log.d("TAG", "DRAWING TEXT");
                    drawMatrix(nWayCanvas, 20, 20, 50);
                    //drawText(nWayCanvas, 10, 0, 40 * 1);
                    //Log.d("TAG", "DRAWN TEXT");

                    nWayCanvas.Flush();

                    using var imageAlpha = offscreenAlphaSurface.Snapshot();
                    using var image = offscreenSurface.Snapshot();
                    imageCanvas.Dispose();
                    overdrawCanvas.Dispose();
                    SF.Dispose();
                    offscreenAlphaSurface.Dispose();
                    offscreenSurface.Dispose();
                    nWayCanvas.Dispose();

                    var imageAlphaShader = imageAlpha.ToShader();
                    var imageShader = image.ToShader();

                    SKRuntimeEffectChildren children = new(sksl) {
                        { "input", imageShader },
                        { "inputAlpha", imageAlphaShader },
                        { "inputGradient", gradient },
                    };

                    var ourShader = sksl.ToShader(false, new(sksl), children);

                    sksl.Dispose();
                    imageAlphaShader.Dispose();
                    imageShader.Dispose();

                    colorPaint.BlendMode = SKBlendMode.Src;
                    colorPaint.Shader = ourShader;
                    canvas.DrawPaint(colorPaint);
                    ourShader.Dispose();
                    return;
                }
                using SKPaint paint = new();
                paint.Color = SKColors.Red;
                canvas.DrawCircle(50, 50, 50, paint);
                canvas.SaveLayerAlpha(128);
                paint.Color = SKColors.Blue;
                canvas.DrawCircle(100, 50, 50, paint);
                paint.Color = SKColors.Green;
                paint.Alpha = 128;
                canvas.DrawCircle(75, 90, 50, paint);
                canvas.Restore();

                paint.Color = SKColors.Red;
                canvas.DrawCircle(150, 50, 50, paint);
                paint.Color = SKColors.Blue.WithAlpha(128);
                canvas.DrawCircle(200, 50, 50, paint);
                paint.Color = SKColors.Green.WithAlpha(128);
                canvas.DrawCircle(175, 90, 50, paint);
                canvas.Restore();
                paint.Color = SKColors.Blue.WithAlpha(128);
                canvas.DrawPaint(paint);
                paint.Color = SKColors.Green.WithAlpha(128);

                using var rteffect = SKRuntimeEffect.Create(
                    "uniform shader input;\n" +
                    "\n" +
                    "half4 main() {\n" +
                    "    half4 color = sample(input);\n" +
                    "    half4 premul = half4(half3(0, 1, 0) * 0.5, 0.5); // R G B A -> premultiplied A*R A*G A*B A \n" +
                    "    return vec4(1,1,1,1);// * premul; \n" +
                    "}\n",
                    out string errors
                );
                if (errors == null)
                {
                    Log.d("OVERDRAW", "runtime effect compiled succesfully");
                    using var s = SKShader.CreateColor(SKColors.Green.WithAlpha(128));
                    //using var s = rteffect.ToColorFilter(new(rteffect), new(rteffect));
                    if (s != null)
                    {
                        using var p = new SKPaint();
                        //using var tmp = SKShader.CreateEmpty().WithColorFilter(cf);
                        //using var tmp = SKShader.CreateColor(SKColors.Green.WithAlpha(128));
                        p.Shader = s;
                        //p.Shader = s;
                        canvas.DrawPaint(p);
                        //using var i = e.Surface.Snapshot();
                        //canvas.DrawImage(i, 0, 0, p);
                    }
                    else
                    {
                        Log.d("OVERDRAW", "failed to create color filter for overdraw viz.");
                    }
                }
                else
                {
                    Log.d("OVERDRAW", "runtime effect compiled with errors: " + errors);
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            host.OnVisibilityChanged(Visible);
        }

        bool mouse_clicked = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouse_clicked = true;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().addTouch(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!mouse_clicked) return;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().moveTouchBatched(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouse_clicked = false;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().removeTouch(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }


        // MULTI TOUCH

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool GetCurrentInputMessageSource(out INPUT_MESSAGE_SOURCE inputMessageSource);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT_MESSAGE_SOURCE
        {
            public INPUT_MESSAGE_DEVICE_TYPE deviceType;
            public INPUT_MESSAGE_ORIGIN_ID originId;
        }

        public enum INPUT_MESSAGE_DEVICE_TYPE
        {
            IMDT_UNAVAILABLE = 0x00000000,      // not specified
            IMDT_KEYBOARD = 0x00000001,      // from keyboard
            IMDT_MOUSE = 0x00000002,      // from mouse
            IMDT_TOUCH = 0x00000004,      // from touch
            IMDT_PEN = 0x00000008,      // from pen
            IMDT_TOUCHPAD = 0x00000010,      // from touchpad
        }

        public enum INPUT_MESSAGE_ORIGIN_ID
        {
            IMO_UNAVAILABLE = 0x00000000,  // not specified
            IMO_HARDWARE = 0x00000001,  // from a hardware device or injected by a UIAccess app
            IMO_INJECTED = 0x00000002,  // injected via SendInput() by a non-UIAccess app
            IMO_SYSTEM = 0x00000004,  // injected by the system
        }

        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSELAST = 0x020E;
        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYLAST = 0x0109;
        public const int WM_TOUCH = 0x0240;
        public const int WM_POINTERWHEEL = 0x024E;

        protected override void WndProc(ref Message m)
        {
            if (
                m.Msg >= WM_MOUSEFIRST && m.Msg <= WM_MOUSELAST
                || m.Msg >= WM_KEYFIRST && m.Msg <= WM_KEYLAST
                || m.Msg >= WM_TOUCH && m.Msg <= WM_POINTERWHEEL
            )
            {
                INPUT_MESSAGE_SOURCE ims = new();
                GetCurrentInputMessageSource(out ims);
                if (ims.deviceType == INPUT_MESSAGE_DEVICE_TYPE.IMDT_MOUSE)
                {
                    base.WndProc(ref m);
                }
                // not tested
                else if (ims.deviceType == INPUT_MESSAGE_DEVICE_TYPE.IMDT_TOUCHPAD)
                {
                    Log.WriteLine("TOUCHPAD INPUT");
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
            {
                Log.WriteLine("");
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
    }
}