using AndroidUI.Skia;
using AndroidUI.Utils.Arrays;
using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKPathExtensions
    {
        public static void SetLastPt(this SKPath path, float x, float y)
        {
            int count = path.PointCount;
            if (count == 0)
            {
                path.MoveTo(x, y);
            }
            else
            {
                path.Points[count - 1].Set(x, y);
                // TODO: verify that points have been updated
            }
        }

        // https://cs.android.com/android/platform/superproject/+/master:external/skia/src/core/SkPath.cpp;l=3435
        public static bool IsRectContour(this SKPath path, bool allowPartial, ref int currVerb,
                               ref SKPoint[] ptsPtr, ref bool isClosed, ref SKPathDirection direction,
                               ref SKRect rect)
        {
            int corners = 0;
            SKPoint closeXY = new();  // used to determine if final line falls on a diagonal
            SKPoint lineStart = new();  // used to construct line from previous point
            SKPoint firstPt = new(); // first point in the rect (last of first moves)
            SKPoint lastPt = new();  // last point in the rect (last of lines or first if closed)
            SKPoint firstCorner = new();
            SKPoint thirdCorner = new();
            int idx = 0;
            SKPoint[] pts = ptsPtr;
            SKPoint[] savePts = null; // used to allow caller to iterate through a pair of rects
            lineStart.Set(0, 0);
            int[] directions = { -1, -1, -1, -1, -1 };  // -1 to 3; -1 is uninitialized
            bool closedOrMoved = false;
            bool autoClose = false;
            bool insertClose = false;
            int verbCnt = path.VerbCount;
            SKPath.RawIterator iterator = path.CreateRawIterator();

            while (currVerb < verbCnt && (!allowPartial || !autoClose))
            {
                SKPathVerb verb = insertClose ? SKPathVerb.Close : iterator.Next(pts);
                if (verb == SKPathVerb.Close)
                {
                    savePts = pts;
                    autoClose = true;
                    insertClose = false;
                }
                switch (verb)
                {
                    case SKPathVerb.Close:
                    case SKPathVerb.Line:
                        {
                            if (SKPathVerb.Close != verb)
                            {
                                lastPt = pts[idx];
                            }
                            SKPoint lineEnd = SKPathVerb.Close == verb ? firstPt : pts[idx++];
                            SKPoint lineDelta = lineEnd - lineStart;
                            if (lineDelta.X == 0 && lineDelta.Y == 0)
                            {
                                return false; // diagonal
                            }
                            if (!lineDelta.isFinite())
                            {
                                return false; // path contains infinity or NaN
                            }
                            if (lineStart == lineEnd)
                            {
                                break; // single point on side OK
                            }
                            int nextDirection = lineDelta.toDirection(); // 0 to 3
                            if (0 == corners)
                            {
                                directions[0] = nextDirection;
                                corners = 1;
                                closedOrMoved = false;
                                lineStart = lineEnd;
                                break;
                            }
                            if (closedOrMoved)
                            {
                                return false; // closed followed by a line
                            }
                            if (autoClose && nextDirection == directions[0])
                            {
                                break; // colinear with first
                            }
                            closedOrMoved = autoClose;
                            if (directions[corners - 1] == nextDirection)
                            {
                                if (3 == corners && SKPathVerb.Line == verb)
                                {
                                    thirdCorner = lineEnd;
                                }
                                lineStart = lineEnd;
                                break; // colinear segment
                            }
                            directions[corners++] = nextDirection;
                            // opposite lines must point in opposite directions; xoring them should equal 2
                            switch (corners)
                            {
                                case 2:
                                    firstCorner = lineStart;
                                    break;
                                case 3:
                                    if ((directions[0] ^ directions[2]) != 2)
                                    {
                                        return false;
                                    }
                                    thirdCorner = lineEnd;
                                    break;
                                case 4:
                                    if ((directions[1] ^ directions[3]) != 2)
                                    {
                                        return false;
                                    }
                                    break;
                                default:
                                    return false; // too many direction changes
                            }
                            lineStart = lineEnd;
                            break;
                        }
                    case SKPathVerb.Quad:
                    case SKPathVerb.Conic:
                    case SKPathVerb.Cubic:
                        return false; // quadratic, cubic not allowed
                    case SKPathVerb.Move:
                        if (allowPartial && !autoClose && directions[0] >= 0)
                        {
                            insertClose = true;
                            currVerb -= 1;  // try move again afterwards
                            goto addMissingClose;
                        }
                        if (!corners.toBool())
                        {
                            firstPt = pts[idx];
                        }
                        else
                        {
                            closeXY = firstPt - lastPt;
                            if (closeXY.X == 0 && closeXY.Y == 0)
                            {
                                return false;   // we're diagonal, abort
                            }
                        }
                        lineStart = pts[idx++];
                        closedOrMoved = true;
                        break;
                    default:
                        throw new Exception("unexpected verb");
                        break;
                }
                currVerb += 1;
            addMissingClose:
                ;
            }
            // Success if 4 corners and first point equals last
            if (corners < 3 || corners > 4)
            {
                return false;
            }
            if (savePts != null)
            {
                ptsPtr = savePts;
            }
            // check if close generates diagonal
            closeXY = firstPt - lastPt;
            if (closeXY.X == 0 && closeXY.Y == 0)
            {
                return false;
            }
            rect.Set(firstCorner, thirdCorner);
            isClosed = autoClose;
            direction = directions[0] == ((directions[1] + 1) & 3) ?
                SKPathDirection.Clockwise : SKPathDirection.CounterClockwise;
            return true;
        }

        static void addMove(List<SKPoint> segmentPoints, List<float> lengths, in SKPoint point)
        {
            float length = 0;
            if (lengths.Count != 0)
            {
                length = lengths.Last();
            }
            segmentPoints.Add(point);
            lengths.Add(length);
        }

        static void addLine(List<SKPoint> segmentPoints, List<float> lengths, in SKPoint toPoint)
        {
            if (segmentPoints.Count == 0)
            {
                segmentPoints.Add(new SKPoint(0, 0));
                lengths.Add(0);
            }
            else if (segmentPoints.Last() == toPoint)
            {
                return; // Empty line
            }
            float length = lengths.Last() + SKPoint.Distance(segmentPoints.Last(), toPoint);
            segmentPoints.Add(toPoint);
            lengths.Add(length);
        }

        static float cubicCoordinateCalculation(float t, float p0, float p1, float p2, float p3)
        {
            float oneMinusT = 1 - t;
            float oneMinusTSquared = oneMinusT * oneMinusT;
            float oneMinusTCubed = oneMinusTSquared * oneMinusT;
            float tSquared = t * t;
            float tCubed = tSquared * t;
            return (oneMinusTCubed * p0) + (3 * oneMinusTSquared * t * p1)
                    + (3 * oneMinusT * tSquared * p2) + (tCubed * p3);
        }

        static SKPoint cubicBezierCalculation(float t, MemoryPointer<SKPoint> points)
        {
            float x = cubicCoordinateCalculation(t, points[0].X, points[1].X,
                points[2].X, points[3].X);
            float y = cubicCoordinateCalculation(t, points[0].Y, points[1].Y,
                points[2].Y, points[3].Y);
            return new SKPoint(x, y);
        }

        static float quadraticCoordinateCalculation(float t, float p0, float p1, float p2)
        {
            float oneMinusT = 1 - t;
            return oneMinusT * ((oneMinusT * p0) + (t * p1)) + t * ((oneMinusT * p1) + (t * p2));
        }

        static SKPoint quadraticBezierCalculation(float t, MemoryPointer<SKPoint> points)
        {
            float x = quadraticCoordinateCalculation(t, points[0].X, points[1].X, points[2].X);
            float y = quadraticCoordinateCalculation(t, points[0].Y, points[1].Y, points[2].Y);
            return new SKPoint(x, y);
        }

        // Subdivide a section of the Bezier curve, set the mid-point and the mid-t value.
        // Returns true if further subdivision is necessary as defined by errorSquared.
        static bool subdividePoints(MemoryPointer<SKPoint> points, Func<float, MemoryPointer<SKPoint>, SKPoint> bezierFunction,
            float t0, in SKPoint p0, float t1, in SKPoint p1,
            out float midT, out SKPoint midPoint, float errorSquared) {
            midT = (t1 + t0) / 2;
            float midX = (p1.X + p0.X) / 2;
            float midY = (p1.Y + p0.Y) / 2;

            midPoint = bezierFunction.Invoke(midT, points);
            float xError = midPoint.X - midX;
            float yError = midPoint.Y - midY;
            float midErrorSquared = (xError * xError) + (yError * yError);
            return midErrorSquared > errorSquared;
        }

        // Divides Bezier curves until linear interpolation is very close to accurate, using
        // errorSquared as a metric. Cubic Bezier curves can have an inflection point that improperly
        // short-circuit subdivision. If you imagine an S shape, the top and bottom points being the
        // starting and end points, linear interpolation would mark the center where the curve places
        // the point. It is clearly not the case that we can linearly interpolate at that point.
        // doubleCheckDivision forces a second examination between subdivisions to ensure that linear
        // interpolation works.
        static void addBezier(MemoryPointer<SKPoint> points,
                Func<float, MemoryPointer<SKPoint>, SKPoint> bezierFunction, List<SKPoint> segmentPoints,
            List<float> lengths, float errorSquared, bool doubleCheckDivision)
        {
            Dictionary<float, SKPoint> tToPoint = new();

            tToPoint[0] = bezierFunction(0, points);
            tToPoint[1] = bezierFunction(1, points);

            Dictionary<float, SKPoint>.Enumerator iter = tToPoint.GetEnumerator();
            Dictionary<float, SKPoint>.Enumerator next = iter;
            next.MoveNext();
            KeyValuePair<float, SKPoint> keyValuePair = tToPoint.Last();
            while (next.Current.Key != keyValuePair.Key && next.Current.Value != keyValuePair.Value)
            {
                KeyValuePair<float, SKPoint> current = next.Current;
                if (current.Key != keyValuePair.Key && current.Value != keyValuePair.Value) break;
                bool needsSubdivision = true;
                SKPoint midPoint;
                do
                {
                    float midT;
                    needsSubdivision = subdividePoints(points, bezierFunction, iter.Current.Key,
                        iter.Current.Value, next.Current.Key, next.Current.Value, out midT, out midPoint, errorSquared);
                    if (!needsSubdivision && doubleCheckDivision)
                    {
                        SKPoint quarterPoint;
                        float quarterT;
                        needsSubdivision = subdividePoints(points, bezierFunction, iter.Current.Key,
                            iter.Current.Value, midT, midPoint, out quarterT, out quarterPoint, errorSquared);
                        if (needsSubdivision)
                        {
                            // Found an inflection point. No need to double-check.
                            doubleCheckDivision = false;
                        }
                    }
                    if (needsSubdivision)
                    {
                        // iter
                        tToPoint.Add(midT, midPoint);
                    }
                } while (needsSubdivision);
                iter = next;
                next.MoveNext();
            }

            // Now that each division can use linear interpolation with less than the allowed error
            KeyValuePair<float, SKPoint> keyValuePair1 = tToPoint.Last();
            for (iter = tToPoint.GetEnumerator(); iter.Current.Key != keyValuePair1.Key && iter.Current.Value != keyValuePair1.Value; iter.MoveNext())
            {
                addLine(segmentPoints, lengths, iter.Current.Value);
            }
        }

        /**
         *  Help class to allocate storage for approximating a conic with N quads.
         */
        class SkAutoConicToQuads
        {
            public SkAutoConicToQuads()
            {
                fQuadCount = 0;
            }

            /**
             *  Given a conic and a tolerance, return the array of points for the
             *  approximating quad(s). Call countQuads() to know the number of quads
             *  represented in these points.
             *
             *  The quads are allocated to share end-points. e.g. if there are 4 quads,
             *  there will be 9 points allocated as follows
             *      quad[0] == pts[0..2]
             *      quad[1] == pts[2..4]
             *      quad[2] == pts[4..6]
             *      quad[3] == pts[6..8]
             */
            public SKPoint[] computeQuads(SKConic conic, float tol)
            {
                int pow2 = conic.computeQuadPOW2(tol);
                fQuadCount = 1 << pow2;
                SKPoint[] pts = fStorage = new SKPoint[1 + 2 * fQuadCount];
                fQuadCount = conic.chopIntoQuadsPOW2(pts, pow2);
                return pts;
            }

            public SKPoint[] computeQuads(SKPoint[] pts, float weight,
                                        float tol)
            {
                return computeQuads(new(pts, weight), tol);
            }

            public int countQuads() { return fQuadCount; }

            private const int kQuadCount = 8; // should handle most conics
            private const int kPointCount = 1 + 2 * kQuadCount;

            SKPoint[] fStorage = new SKPoint[kPointCount];
            int fQuadCount; // #quads for current usage
        };


        static void createVerbSegments(ref SKPath.Iterator pathIter, SKPathVerb verb,
                    SKPoint[] points, List<SKPoint> segmentPoints,
                    List<float> lengths, float errorSquared, float errorConic)
        {
            switch (verb)
            {
                case SKPathVerb.Move:
                    addMove(segmentPoints, lengths, points[0]);
                    break;
                case SKPathVerb.Close:
                    addLine(segmentPoints, lengths, points[0]);
                    break;
                case SKPathVerb.Line:
                    addLine(segmentPoints, lengths, points[1]);
                    break;
                case SKPathVerb.Quad:
                    addBezier(points, quadraticBezierCalculation, segmentPoints, lengths,
                        errorSquared, false);
                    break;
                case SKPathVerb.Cubic:
                    addBezier(points, cubicBezierCalculation, segmentPoints, lengths,
                        errorSquared, true);
                    break;
                case SKPathVerb.Conic:
                    {
                        SkAutoConicToQuads converter = new();

                        MemoryPointer<SKPoint> quads = converter.computeQuads(
                            points, pathIter.ConicWeight(), errorConic);
                        for (int i = 0; i < converter.countQuads(); i++)
                        {
                            // Note: offset each subsequent quad by 2, since end points are shared
                            MemoryPointer<SKPoint> quad = quads + i * 2;
                            addBezier(quad, quadraticBezierCalculation, segmentPoints, lengths,
                                errorConic, false);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        // Returns a float[] with each point along the path represented by 3 floats
        // * fractional length along the path that the point resides
        // * x coordinate
        // * y coordinate
        // Note that more than one point may have the same length along the path in
        // the case of a move.
        // NULL can be returned if the Path is empty.
        public static float[] Approximate(this SKPath path, float acceptableError)
        {
            SKPath.Iterator pathIter = path.CreateIterator(false);
            SKPathVerb verb;
            SKPoint[] points = new SKPoint[4];
            List<SKPoint> segmentPoints = new();
            List<float> lengths = new();
            float errorSquared = acceptableError * acceptableError;
            float errorConic = acceptableError / 2; // somewhat arbitrary

            while ((verb = pathIter.Next(points)) != SKPathVerb.Done)
            {
                createVerbSegments(ref pathIter, verb, points, segmentPoints, lengths,
                        errorSquared, errorConic);
            }

            if (segmentPoints.Count == 0)
            {
                int numVerbs = path.VerbCount;
                if (numVerbs == 1)
                {
                    addMove(segmentPoints, lengths, path.GetPoint(0));
                }
                else
                {
                    // Invalid or empty path. Fall back to point(0,0)
                    addMove(segmentPoints, lengths, new SKPoint());
                }
            }

            float totalLength = lengths.Last();
            if (totalLength == 0)
            {
                // Lone Move instructions should still be able to animate at the same value.
                segmentPoints.Add(segmentPoints.Last());
                lengths.Add(1);
                totalLength = 1;
            }

            int numPoints = segmentPoints.Count;
            int approximationArraySize = numPoints * 3;

            float[] approximation = new float[approximationArraySize];

            int approximationIndex = 0;
            for (int i = 0; i < numPoints; i++)
            {
                SKPoint point = segmentPoints[i];
                approximation[approximationIndex++] = lengths[i] / totalLength;
                approximation[approximationIndex++] = point.X;
                approximation[approximationIndex++] = point.Y;
            }

            return approximation;
        }
    }
}