using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKPointExtensions
    {
        public static void Set(this ref SKPoint point, float x, float y)
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