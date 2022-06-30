/*
 * Copyright (C) 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

using AndroidUI.Extensions;

namespace AndroidUI
{
    public class VectorDrawableUtils
    {
        public class Data
        {
            internal List<char> verbs;
            internal List<int> verbSizes;
            internal List<float> points;

            public Data()
            {
                verbs = new();
                verbSizes = new();
                points = new();
            }

            public Data(Data data)
            {
                SetFrom(data);
            }

            public void SetFrom(Data data)
            {
                verbs = new(data.verbs);
                verbSizes = new(data.verbSizes);
                points = new(data.points);
            }

            public override bool Equals(object obj)
            {
                return obj is Data data &&
                       EqualityComparer<List<char>>.Default.Equals(verbs, data.verbs) &&
                       EqualityComparer<List<int>>.Default.Equals(verbSizes, data.verbSizes) &&
                       EqualityComparer<List<float>>.Default.Equals(points, data.points);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(verbs, verbSizes, points);
            }
        }

        internal class PathResolver
        {
            public float currentX = 0;
            public float currentY = 0;
            public float ctrlPointX = 0;
            public float ctrlPointY = 0;
            public float currentSegmentStartX = 0;
            public float currentSegmentStartY = 0;
            public void addCommand(SkiaSharp.SKPath outPath, char previousCmd, char cmd, List<float> points,
                            int start, int end)
            {
                int incr = 2;
                float reflectiveCtrlPointX;
                float reflectiveCtrlPointY;

                switch (cmd)
                {
                    case 'z':
                    case 'Z':
                        outPath.Close();
                        // Path is closed here, but we need to move the pen to the
                        // closed position. So we cache the segment's starting position,
                        // and restore it here.
                        currentX = currentSegmentStartX;
                        currentY = currentSegmentStartY;
                        ctrlPointX = currentSegmentStartX;
                        ctrlPointY = currentSegmentStartY;
                        outPath.MoveTo(currentX, currentY);
                        break;
                    case 'm':
                    case 'M':
                    case 'l':
                    case 'L':
                    case 't':
                    case 'T':
                        incr = 2;
                        break;
                    case 'h':
                    case 'H':
                    case 'v':
                    case 'V':
                        incr = 1;
                        break;
                    case 'c':
                    case 'C':
                        incr = 6;
                        break;
                    case 's':
                    case 'S':
                    case 'q':
                    case 'Q':
                        incr = 4;
                        break;
                    case 'a':
                    case 'A':
                        incr = 7;
                        break;
                }

                for (int k = start; k < end; k += incr)
                {
                    switch (cmd)
                    {
                        case 'm':  // moveto - Start a new sub-path (relative)
                            currentX += points.ElementAt(k + 0);
                            currentY += points.ElementAt(k + 1);
                            if (k > start)
                            {
                                // According to the spec, if a moveto is followed by multiple
                                // pairs of coordinates, the subsequent pairs are treated as
                                // implicit lineto commands.
                                outPath.RLineTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                            }
                            else
                            {
                                outPath.RMoveTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                                currentSegmentStartX = currentX;
                                currentSegmentStartY = currentY;
                            }
                            break;
                        case 'M':  // moveto - Start a new sub-path
                            currentX = points.ElementAt(k + 0);
                            currentY = points.ElementAt(k + 1);
                            if (k > start)
                            {
                                // According to the spec, if a moveto is followed by multiple
                                // pairs of coordinates, the subsequent pairs are treated as
                                // implicit lineto commands.
                                outPath.LineTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                            }
                            else
                            {
                                outPath.MoveTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                                currentSegmentStartX = currentX;
                                currentSegmentStartY = currentY;
                            }
                            break;
                        case 'l':  // lineto - Draw a line from the current point (relative)
                            outPath.RLineTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                            currentX += points.ElementAt(k + 0);
                            currentY += points.ElementAt(k + 1);
                            break;
                        case 'L':  // lineto - Draw a line from the current point
                            outPath.LineTo(points.ElementAt(k + 0), points.ElementAt(k + 1));
                            currentX = points.ElementAt(k + 0);
                            currentY = points.ElementAt(k + 1);
                            break;
                        case 'h':  // horizontal lineto - Draws a horizontal line (relative)
                            outPath.RLineTo(points.ElementAt(k + 0), 0);
                            currentX += points.ElementAt(k + 0);
                            break;
                        case 'H':  // horizontal lineto - Draws a horizontal line
                            outPath.LineTo(points.ElementAt(k + 0), currentY);
                            currentX = points.ElementAt(k + 0);
                            break;
                        case 'v':  // vertical lineto - Draws a vertical line from the current point (r)
                            outPath.RLineTo(0, points.ElementAt(k + 0));
                            currentY += points.ElementAt(k + 0);
                            break;
                        case 'V':  // vertical lineto - Draws a vertical line from the current point
                            outPath.LineTo(currentX, points.ElementAt(k + 0));
                            currentY = points.ElementAt(k + 0);
                            break;
                        case 'c':  // curveto - Draws a cubic Bézier curve (relative)
                            outPath.RCubicTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                              points.ElementAt(k + 3), points.ElementAt(k + 4), points.ElementAt(k + 5));

                            ctrlPointX = currentX + points.ElementAt(k + 2);
                            ctrlPointY = currentY + points.ElementAt(k + 3);
                            currentX += points.ElementAt(k + 4);
                            currentY += points.ElementAt(k + 5);

                            break;
                        case 'C':  // curveto - Draws a cubic Bézier curve
                            outPath.CubicTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                             points.ElementAt(k + 3), points.ElementAt(k + 4), points.ElementAt(k + 5));
                            currentX = points.ElementAt(k + 4);
                            currentY = points.ElementAt(k + 5);
                            ctrlPointX = points.ElementAt(k + 2);
                            ctrlPointY = points.ElementAt(k + 3);
                            break;
                        case 's':  // smooth curveto - Draws a cubic Bézier curve (reflective cp)
                            reflectiveCtrlPointX = 0;
                            reflectiveCtrlPointY = 0;
                            if (previousCmd == 'c' || previousCmd == 's' || previousCmd == 'C' ||
                                previousCmd == 'S')
                            {
                                reflectiveCtrlPointX = currentX - ctrlPointX;
                                reflectiveCtrlPointY = currentY - ctrlPointY;
                            }
                            outPath.RCubicTo(reflectiveCtrlPointX, reflectiveCtrlPointY, points.ElementAt(k + 0),
                                              points.ElementAt(k + 1), points.ElementAt(k + 2), points.ElementAt(k + 3));
                            ctrlPointX = currentX + points.ElementAt(k + 0);
                            ctrlPointY = currentY + points.ElementAt(k + 1);
                            currentX += points.ElementAt(k + 2);
                            currentY += points.ElementAt(k + 3);
                            break;
                        case 'S':  // shorthand/smooth curveto Draws a cubic Bézier curve(reflective cp)
                            reflectiveCtrlPointX = currentX;
                            reflectiveCtrlPointY = currentY;
                            if (previousCmd == 'c' || previousCmd == 's' || previousCmd == 'C' ||
                                previousCmd == 'S')
                            {
                                reflectiveCtrlPointX = 2 * currentX - ctrlPointX;
                                reflectiveCtrlPointY = 2 * currentY - ctrlPointY;
                            }
                            outPath.CubicTo(reflectiveCtrlPointX, reflectiveCtrlPointY, points.ElementAt(k + 0),
                                             points.ElementAt(k + 1), points.ElementAt(k + 2), points.ElementAt(k + 3));
                            ctrlPointX = points.ElementAt(k + 0);
                            ctrlPointY = points.ElementAt(k + 1);
                            currentX = points.ElementAt(k + 2);
                            currentY = points.ElementAt(k + 3);
                            break;
                        case 'q':  // Draws a quadratic Bézier (relative)
                            outPath.RQuadTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                             points.ElementAt(k + 3));
                            ctrlPointX = currentX + points.ElementAt(k + 0);
                            ctrlPointY = currentY + points.ElementAt(k + 1);
                            currentX += points.ElementAt(k + 2);
                            currentY += points.ElementAt(k + 3);
                            break;
                        case 'Q':  // Draws a quadratic Bézier
                            outPath.QuadTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                            points.ElementAt(k + 3));
                            ctrlPointX = points.ElementAt(k + 0);
                            ctrlPointY = points.ElementAt(k + 1);
                            currentX = points.ElementAt(k + 2);
                            currentY = points.ElementAt(k + 3);
                            break;
                        case 't':  // Draws a quadratic Bézier curve(reflective control point)(relative)
                            reflectiveCtrlPointX = 0;
                            reflectiveCtrlPointY = 0;
                            if (previousCmd == 'q' || previousCmd == 't' || previousCmd == 'Q' ||
                                previousCmd == 'T')
                            {
                                reflectiveCtrlPointX = currentX - ctrlPointX;
                                reflectiveCtrlPointY = currentY - ctrlPointY;
                            }
                            outPath.RQuadTo(reflectiveCtrlPointX, reflectiveCtrlPointY, points.ElementAt(k + 0),
                                             points.ElementAt(k + 1));
                            ctrlPointX = currentX + reflectiveCtrlPointX;
                            ctrlPointY = currentY + reflectiveCtrlPointY;
                            currentX += points.ElementAt(k + 0);
                            currentY += points.ElementAt(k + 1);
                            break;
                        case 'T':  // Draws a quadratic Bézier curve (reflective control point)
                            reflectiveCtrlPointX = currentX;
                            reflectiveCtrlPointY = currentY;
                            if (previousCmd == 'q' || previousCmd == 't' || previousCmd == 'Q' ||
                                previousCmd == 'T')
                            {
                                reflectiveCtrlPointX = 2 * currentX - ctrlPointX;
                                reflectiveCtrlPointY = 2 * currentY - ctrlPointY;
                            }
                            outPath.QuadTo(reflectiveCtrlPointX, reflectiveCtrlPointY, points.ElementAt(k + 0),
                                            points.ElementAt(k + 1));
                            ctrlPointX = reflectiveCtrlPointX;
                            ctrlPointY = reflectiveCtrlPointY;
                            currentX = points.ElementAt(k + 0);
                            currentY = points.ElementAt(k + 1);
                            break;
                        case 'a':  // Draws an elliptical arc
                                   // (rx ry x-axis-rotation large-arc-flag sweep-flag x y)
                            outPath.ArcTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                           (SkiaSharp.SKPathArcSize)(points.ElementAt(k + 3) != 0).toInt(),
                                           (SkiaSharp.SKPathDirection)(points.ElementAt(k + 4) == 0).toInt(),
                                           points.ElementAt(k + 5) + currentX, points.ElementAt(k + 6) + currentY);
                            currentX += points.ElementAt(k + 5);
                            currentY += points.ElementAt(k + 6);
                            ctrlPointX = currentX;
                            ctrlPointY = currentY;
                            break;
                        case 'A':  // Draws an elliptical arc
                            outPath.ArcTo(points.ElementAt(k + 0), points.ElementAt(k + 1), points.ElementAt(k + 2),
                                           (SkiaSharp.SKPathArcSize)(points.ElementAt(k + 3) != 0).toInt(),
                                           (SkiaSharp.SKPathDirection)(points.ElementAt(k + 4) == 0).toInt(),
                                           points.ElementAt(k + 5), points.ElementAt(k + 6));
                            currentX = points.ElementAt(k + 5);
                            currentY = points.ElementAt(k + 6);
                            ctrlPointX = currentX;
                            ctrlPointY = currentY;
                            break;
                        default:
                            Console.WriteLine("Unsupported command: " + cmd);
                            break;
                    }
                    previousCmd = cmd;
                }
            }
        };


        /**
        * Convert an array of PathVerb to Path.
        */
        public static void verbsToPath(SkiaSharp.SKPath outPath, Data data)
        {
            PathResolver resolver = new();
            char previousCommand = 'm';
            int start = 0;
            outPath.Reset();
            for (int i = 0; i < data.verbs.Count; i++)
            {
                int verbSize = data.verbSizes[i];
                resolver.addCommand(outPath, previousCommand, data.verbs[i], data.points, start,
                                    start + verbSize);
                previousCommand = data.verbs[i];
                start += verbSize;
            }
        }

        public static bool canMorph(Data morphFrom, Data morphTo)
        {
            if (morphFrom.verbs.Count != morphTo.verbs.Count)
            {
                return false;
            }

            for (int i = 0; i < morphFrom.verbs.Count; i++)
            {
                if (morphFrom.verbs[i] != morphTo.verbs[i] ||
                    morphFrom.verbSizes[i] != morphTo.verbSizes[i])
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * The current PathVerb will be interpolated between the
         * <code>nodeFrom</code> and <code>nodeTo</code> according to the
         * <code>fraction</code>.
         *
         * @param nodeFrom The start value as a PathVerb.
         * @param nodeTo The end value as a PathVerb
         * @param fraction The fraction to interpolate.
         */
        public static void interpolatePaths(Data outData, Data from,
                                                   Data to, float fraction)
        {
            outData.points.EnsureCapacity(from.points.Count);
            outData.verbSizes = from.verbSizes;
            outData.verbs = from.verbs;

            for (int i = 0; i < from.points.Count; i++)
            {
                outData.points[i] = from.points[i] * (1 - fraction) + to.points[i] * fraction;
            }
        }

        public static bool interpolatePathData(Data outData, Data morphFrom, Data morphTo, float fraction)
        {
            if (!canMorph(morphFrom, morphTo))
            {
                return false;
            }
            interpolatePaths(outData, morphFrom, morphTo, fraction);
            return true;
        }
    }
}