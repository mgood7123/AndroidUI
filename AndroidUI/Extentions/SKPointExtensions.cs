using SkiaSharp;
using static AndroidUI.SKUtils;

namespace AndroidUI.Extensions
{
    public static class SKPointExtensions
    {
        public static SKPoint ToSKPoint(this ContiguousArray<float> floats)
        {
            return new SKPoint(floats[0], floats[1]);
        }

        public static SKPointI ToSKPointI(this ContiguousArray<int> ints)
        {
            return new SKPointI(ints[0], ints[1]);
        }

        public static SKPoint3 ToSKPoint3(this ContiguousArray<float> floats)
        {
            return new SKPoint3(floats[0], floats[1], floats[2]);
        }

        public static void Set(this ref SKPoint point, float x, float y)
        {
            point.X = x;
            point.Y = y;
        }

        public static void Set(this ref SKPointI point, int x, int y)
        {
            point.X = x;
            point.Y = y;
        }

        public static void Set(this ref SKPoint3 point, float x, float y, float z)
        {
            point.X = x;
            point.Y = y;
            point.Z = z;
        }

        public static SKPoint Add(this SKPoint p, float v) => p.Add(v, v);
        public static SKPoint Add(this SKPoint p, float x, float y) => new(p.X + x, p.Y + y);
        public static SKPoint Subtract(this SKPoint p, float v) => p.Subtract(v, v);
        public static SKPoint Subtract(this SKPoint p, float x, float y) => new(p.X - x, p.Y - y);
        public static SKPoint Multiply(this SKPoint p, float v) => p.Multiply(v, v);
        public static SKPoint Multiply(this SKPoint p, float x, float y) => new(p.X * x, p.Y * y);
        public static SKPoint Divide(this SKPoint p, float v) => p.Divide(v, v);
        public static SKPoint Divide(this SKPoint p, float x, float y) => new(p.X / x, p.Y / y);

        public static SKPointI Add(this SKPointI p, int v) => p.Add(v, v);
        public static SKPointI Add(this SKPointI p, int x, int y) => new(p.X + x, p.Y + y);
        public static SKPointI Subtract(this SKPointI p, int v) => p.Subtract(v, v);
        public static SKPointI Subtract(this SKPointI p, int x, int y) => new(p.X - x, p.Y - y);
        public static SKPointI Multiply(this SKPointI p, int v) => p.Multiply(v, v);
        public static SKPointI Multiply(this SKPointI p, int x, int y) => new(p.X * x, p.Y * y);
        public static SKPointI Divide(this SKPointI p, int v) => p.Divide(v, v);
        public static SKPointI Divide(this SKPointI p, int x, int y) => new(p.X / x, p.Y / y);

        public static SKPoint3 Add(this SKPoint3 p, float v) => p.Add(v, v, v);
        public static SKPoint3 Add(this SKPoint3 p, float x, float y, float z) => new(p.X + x, p.Y + y, p.Z + z);
        public static SKPoint3 Subtract(this SKPoint3 p, float v) => p.Subtract(v, v, v);
        public static SKPoint3 Subtract(this SKPoint3 p, float x, float y, float z) => new(p.X - x, p.Y - y, p.Z - z);
        public static SKPoint3 Multiply(this SKPoint3 p, float v) => p.Multiply(v, v, v);
        public static SKPoint3 Multiply(this SKPoint3 p, float x, float y, float z) => new(p.X * x, p.Y * y, p.Z * z);
        public static SKPoint3 Divide(this SKPoint3 p, float v) => p.Divide(v, v, v);
        public static SKPoint3 Divide(this SKPoint3 p, float x, float y, float z) => new(p.X / x, p.Y / y, p.Z / z);


        public static SKPoint Negate(this SKPoint point)
        {
            return new SKPoint(-point.X, -point.Y);
        }

        public static SKPointI Negate(this SKPointI point)
        {
            return new SKPointI(-point.X, -point.Y);
        }

        public static SKPoint3 Negate(this SKPoint3 point)
        {
            return new SKPoint3(-point.X, -point.Y, -point.Z);
        }

        /// <summary>
        /// Returns the dot product of vector a and vector b.
        /// </summary>
        /// <param name="a">left side of dot product</param>
        /// <param name="b">right side of dot product</param>
        /// <returns>product of input magnitudes and cosine of the angle between them</returns>
        public static float DotProduct(this SKPoint a, SKPoint b) {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Returns the cross product of vector a and vector b.
        /// <br></br>
        /// a and b form three-dimensional vectors with z-axis value equal to zero.
        /// <br></br>
        /// The cross product is a three-dimensional vector with x-axis and y-axis values
        /// <br></br>
        /// equal to zero.The cross product z-axis component is returned.
        /// </summary>
        /// <param name="a">left side of cross product</param>
        /// <param name="b">right side of cross product</param>
        /// <returns>area spanned by vectors signed by angle direction</returns>
        public static float CrossProduct(this SKPoint a, SKPoint b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// Returns the cross product of vector and vec.
        /// <br></br>
        /// Vector and vec form three-dimensional vectors with z-axis value equal to zero.
        /// <br></br>
        /// The cross product is a three-dimensional vector with x-axis and y-axis values
        /// <br></br>
        /// equal to zero.The cross product z-axis component is returned.
        /// </summary>
        /// <param name="vec">right side of cross product</param>
        /// <returns>area spanned by vectors signed by angle direction</returns>
        public static float Cross(this SKPoint point, SKPoint vec)
        {
            return CrossProduct(point, vec);
        }

        /// <summary>
        /// Returns the dot product of vector and vector vec.
        /// </summary>
        /// <param name="vec">right side of dot product</param>
        /// <returns>product of input magnitudes and cosine of the angle between them</returns>
        public static float Dot(this SKPoint point, SKPoint vec)
        {
            return DotProduct(point, vec);
        }

        /*
         *  We have to worry about 2 tricky conditions:
         *  1. underflow of mag2 (compared against nearlyzero^2)
         *  2. overflow of mag2 (compared w/ isfinite)
         *
         *  If we underflow, we return false. If we overflow, we compute again using
         *  doubles, which is much slower (3x in a desktop test) but will not overflow.
         */
        static bool set_point_length(bool use_rsqrt, ref SKPoint pt, float x, float y, float length,
                                                        float[] orig_length = null)
        {
            if (!(!use_rsqrt || (orig_length == null)))
            {
                throw new Exception("!use_rsqrt || (orig_length == null)");
            }

            // our mag2 step overflowed to infinity, so use doubles instead.
            // much slower, but needed when x or y are very large, other wise we
            // divide by inf. and return (0,0) vector.
            double xx = x;
            double yy = y;
            double dmag = Math.Sqrt(xx * xx + yy * yy);
            double dscale = sk_ieee_double_divide(length, dmag);
            x = (float)(x * dscale);
            y = (float)(y * dscale);
            // check if we're not finite, or we're zero-length
            if (!sk_float_isfinite(x) || !sk_float_isfinite(y) || (x == 0 && y == 0))
            {
                pt.Set(0, 0);
                return false;
            }
            float mag = 0;
            if (orig_length != null)
            {
                mag = sk_double_to_float(dmag);
            }
            pt.Set(x, y);
            if (orig_length != null)
            {
                orig_length[0] = mag;
            }
            return true;
        }

        /// <summary>
        /// Scales (X, Y) so that length() returns one, while preserving ratio of X to Y,
        /// <br></br>
        /// if possible. If prior length is nearly zero, sets vector to (0, 0) and returns
        /// <br></br>
        /// false; otherwise returns true.
        /// </summary>
        /// <returns>true if former length is not zero or nearly zero</returns>
        public static bool Normalize1(this ref SKPoint pt)
        {
            return pt.setLength(pt.X, pt.Y, SK_Scalar1);
        }

        /// <summary>
        /// Sets vector to (x, y) scaled so length() returns one, and so that
        /// <br></br>
        /// (fX, fY) is proportional to (x, y).  If (x, y) length is nearly zero,
        /// <br></br>
        /// sets vector to (0, 0) and returns false; otherwise returns true.
        /// </summary>
        /// <param name="x">proportional value for X</param>
        /// <param name="y">proportional value for Y</param>
        /// <returns>true if (x, y) length is not zero or nearly zero</returns>
        public static bool Normalize2(this ref SKPoint pt, float x, float y)
        {
            return pt.setLength(x, y, SK_Scalar1);
        }

        /// <summary>
        /// Scales (X, Y) so that length() returns one, while preserving ratio of X
        /// <br></br>
        /// to Y, if possible. If original length is nearly zero, sets vec to (0, 0) and returns
        /// <br></br>
        /// zero; otherwise, returns length of vec before vec is scaled.
        /// <br></br>
        /// <br></br>
        /// Returned prior length may be SK_ScalarInfinity if it can not be represented by SkScalar.
        /// <br></br>
        /// <br></br>
        /// Note that normalize() is faster if prior length is not required.
        /// </summary>
        /// <param name="vec">normalized to unit length</param>
        /// <returns>original vec length</returns>
        public static float NormalizeStatic(this ref SKPoint pt)
        {
            float[] mag = new float[1];
            if (set_point_length(false, ref pt, pt.X, pt.Y, 1.0f, mag))
            {
                return mag[0];
            }
            return 0;
        }

        public static bool setLength(this ref SKPoint pt, float length)
        {
            return set_point_length(false, ref pt, pt.X, pt.Y, length);
        }

        public static bool setLength(this ref SKPoint pt, float x, float y, float length)
        {
            return set_point_length(false, ref pt, x, y, length);
        }

        /// <summary>
        /// Returns the Euclidean distance from origin, computed as:
        /// <br></br>
        /// <br></br>
        /// sqrt(x * x + y * y)
        /// <br></br>
        /// <br></br>
        /// .
        /// </summary>
        /// <param name="dx">component of length</param>
        /// <param name="dy">component of length</param>
        /// <returns>straight-line distance to origin</returns>
        static float SkPointLength(this SKPoint unused, float dx, float dy)
        {
            float mag2 = dx * dx + dy * dy;
            if (mag2.isFinite())
            {
                return MathF.Sqrt(mag2);
            }
            else
            {
                double xx = dx;
                double yy = dy;
                return SKUtils.sk_double_to_float(Math.Sqrt(xx * xx + yy * yy));
            }
        }

        /// <summary>
        /// Returns the Euclidean distance from origin, computed as:
        /// <br></br>
        /// <br></br>
        /// sqrt(x * x + y * y)
        /// <br></br>
        /// <br></br>
        /// .
        /// </summary>
        /// <returns>straight-line distance to origin</returns>
        static float SkPointLength(this SKPoint point)
        {
            return point.SkPointLength(point.X, point.Y);
        }

        /// <summary>
        /// Returns the Euclidean distance between a and b.
        /// </summary>
        /// <param name="a">line end point</param>
        /// <param name="b">line end point</param>
        /// <returns>straight-line distance from a to b</returns>
        static float Distance(SKPoint a, SKPoint b)
        {
            return a.SkPointLength(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Returns the dot product of a and b, treating them as 3D vectors
        /// </summary>
        public static float DotProduct(this SKPoint3 a, SKPoint3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Returns the cross product of a and b, treating them as 3D vectors
        /// </summary>
        static SKPoint3 CrossProduct(SKPoint3 a, SKPoint3 b)
        {
            SKPoint3 result = new();
            result.X = a.Y * b.Z - a.Z * b.Y;
            result.Y = a.Z * b.X - a.X * b.Z;
            result.Z = a.X * b.Y - a.Y * b.X;

            return result;
        }

        /// <summary>
        /// Returns the cross product of a and b, treating them as 3D vectors
        /// </summary>
        public static SKPoint3 Cross(this SKPoint3 point, SKPoint3 vec)
        {
            return CrossProduct(point, vec);
        }

        /// <summary>
        /// Returns the dot product of vector and vector vec.
        /// </summary>
        /// <param name="vec">right side of dot product</param>
        /// <returns>product of input magnitudes and cosine of the angle between them</returns>
        public static float Dot(this SKPoint3 point, SKPoint3 vec)
        {
            return DotProduct(point, vec);
        }

        public static SKPointI Dot(this SKPointI point)
        {
            return new SKPointI(-point.X, -point.Y);
        }

        public static SKPoint3 Dot(this SKPoint3 point)
        {
            return new SKPoint3(-point.X, -point.Y, -point.Z);
        }

        public static bool isFinite(this ref SKPoint point)
        {
            float accum = 0;
            accum *= point.X;
            accum *= point.Y;
            bool n = float.IsNaN(accum);
            if (!(accum == 0 || n))
            {
                throw new Exception("accum must be either NaN or finite (zero).");
            }
            return !n;
        }

        /*
         Determines if path is a rect by keeping track of changes in direction
         and looking for a loop either clockwise or counterclockwise.

         The direction is computed such that:
          0: vertical up
          1: horizontal left
          2: vertical down
          3: horizontal right

        A rectangle cycles up/right/down/left or up/left/down/right.

        The test fails if:
          The path is closed, and followed by a line.
          A second move creates a new endpoint.
          A diagonal line is parsed.
          There's more than four changes of direction.
          There's a discontinuity on the line (e.g., a move in the middle)
          The line reverses direction.
          The path contains a quadratic or cubic.
          The path contains fewer than four points.
         *The rectangle doesn't complete a cycle.
         *The final point isn't equal to the first point.

          *These last two conditions we relax if we have a 3-edge path that would
           form a rectangle if it were closed (as we do when we fill a path)

        It's OK if the path has:
          Several colinear line segments composing a rectangle side.
          Single points on the rectangle side.

        The direction takes advantage of the corners found since opposite sides
        must travel in opposite directions.

        FIXME: Allow colinear quads and cubics to be treated like lines.
        FIXME: If the API passes fill-only, return true if the filled stroke
               is a rectangle, though the caller failed to close the path.

         directions values:
            0x1 is set if the segment is horizontal
            0x2 is set if the segment is moving to the right or down
         thus:
            two directions are opposites iff (dirA ^ dirB) == 0x2
            two directions are perpendicular iff (dirA ^ dirB) == 0x1

         */
        public static int toDirection(this ref SKPoint point)
        {
            return ((0 != point.X).toInt() << 0) | ((point.X > 0 || point.Y > 0).toInt() << 1);
        }
    }
}