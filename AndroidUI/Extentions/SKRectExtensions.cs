using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI.Extensions
{
    public static class SKRectExtensions
    {

        public static void SetLTRB(this ref SKRect point, float left, float top, float right, float bottom)
        {
            point.Left = left;
            point.Top = top;
            point.Right = right;
            point.Bottom = bottom;
        }

        /** Sets bounds to the smallest SkRect enclosing SkPoint p0 and p1. The result is
            sorted and may be empty. Does not check to see if values are finite.

            @param p0  corner to include
            @param p1  corner to include
        */
        public static void Set(this ref SKRect point, SKPoint p0, SKPoint p1)
        {
            point.Left = Math.Min(p0.X, p1.X);
            point.Top = Math.Min(p0.Y, p1.Y);
            point.Right = Math.Max(p0.X, p1.X);
            point.Bottom = Math.Max(p0.Y, p1.Y);
        }

        /** Sets to bounds of SkPoint array with count entries. If count is zero or smaller,
            or if SkPoint array contains an infinity or NaN, sets to (0, 0, 0, 0).

            Result is either empty or sorted: fLeft is less than or equal to fRight, and
            fTop is less than or equal to fBottom.

            @param pts    SkPoint array
            @param count  entries in array
        */
        public static void setBounds(this ref SKRect point, SKPoint[] pts, int count) {
            setBoundsCheck(ref point, pts, count);
        }

        /** Sets to bounds of SkPoint array with count entries. Returns false if count is
            zero or smaller, or if SkPoint array contains an infinity or NaN; in these cases
            sets SkRect to (0, 0, 0, 0).

            Result is either empty or sorted: fLeft is less than or equal to fRight, and
            fTop is less than or equal to fBottom.

            @param pts    SkPoint array
            @param count  entries in array
            @return       true if all SkPoint values are finite

            example: https://fiddle.skia.org/c/@Rect_setBoundsCheck
        */
        public static bool setBoundsCheck(this ref SKRect point, MemoryPointer<SKPoint> pts, int count)
        {
            if(!((pts != null && count > 0) || count == 0))
            {
                throw new Exception("((pts != null && count > 0) || count == 0) failed");
            };

            if (count <= 0)
            {
                point = SKRect.Empty;
                return true;
            }

            Sk4s min, max;
            if ((count & 1).toBool())
            {
                min = max = new Sk4s(pts[0].X, pts[0].Y, pts[0].X, pts[0].Y);
                pts += 1;
                count -= 1;
            }
            else
            {
                min = max = Sk4s.Load(pts.MapToArray(SKUtils.createSKPointMapper()));
                pts += 2;
                count -= 2;
            }

            Sk4s accum = min * 0;
            while (count != 0)
            {
                Sk4s xy = Sk4s.Load(pts.MapToArray(SKUtils.createSKPointMapper()));
                accum = accum * xy;
                min = Sk4s.Min(min, xy);
                max = Sk4s.Max(max, xy);
                pts += 2;
                count -= 2;
            }

            bool all_finite = (accum * 0 == 0).AllTrue();
            if (all_finite)
            {
                point.SetLTRB(MathF.Min(min[0], min[2]), MathF.Min(min[1], min[3]),
                              MathF.Max(max[0], max[2]), MathF.Max(max[1], max[3]));
            }
            else
            {
                point = SKRect.Empty;
            }
            return all_finite;
        }
    }
}