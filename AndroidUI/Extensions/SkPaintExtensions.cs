﻿using AndroidUI.Utils.Graphics;
using SkiaSharp;

namespace AndroidUI.Extensions
{
    using static ColorHelper;

    public static class SkPaintExtensions
    {
        public static SKPaint ToPaint(this SKColor color)
        {
            return new SKPaint
            {
                Color = color
            };
        }

        public static SKPaint ToPaint(this SKColorF color)
        {
            return new SKPaint
            {
                ColorF = color
            };
        }

        public static void SetToDefaultBlendMode(this SKPaint paint)
        {
            paint.BlendMode = SKBlendMode.SrcOver;
        }

        public static void SetColor(this SKPaint paint, SKColor color)
        {
            paint.SetColor(color, srgb);
        }

        public static void SetColor(this SKPaint paint, SKColorF color)
        {
            paint.SetColor(color, srgb);
        }



        public static void SetColor(this SKPaint paint, byte red, byte green, byte blue)
        {
            paint.SetColor(new SKColor(red, green, blue), srgb);
        }

        public static void SetColor(this SKPaint paint, float red, float green, float blue)
        {
            paint.SetColor(new SKColorF(red, green, blue), srgb);
        }



        public static void SetColor(this SKPaint paint, byte red, byte green, byte blue, byte alpha)
        {
            paint.SetColor(new SKColor(red, green, blue, alpha), srgb);
        }

        public static void SetColor(this SKPaint paint, float red, float green, float blue, float alpha)
        {
            paint.SetColor(new SKColorF(red, green, blue, alpha), srgb);
        }


        public static void SetStyle(this SKPaint paint, SKPaintStyle style)
        {
            paint.Style = style;
        }

        public static SKPaintStyle GetStyle(this SKPaint paint)
        {
            return paint.Style;
        }
    }
}
