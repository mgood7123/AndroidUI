using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKRectExtensions
    {

        /** Sets bounds to the smallest SkRect enclosing SkPoint p0 and p1. The result is
            sorted and may be empty. Does not check to see if values are finite.

            @param p0  corner to include
            @param p1  corner to include
        */
        public static void Set(this ref SKRect point, SKPoint p0, SKPoint p1)
        {
            point.Left = Math.Min(p0.X, p1.X);
            point.Right = Math.Max(p0.X, p1.X);
            point.Top = Math.Min(p0.Y, p1.Y);
            point.Bottom = Math.Max(p0.Y, p1.Y);
        }
    }
}