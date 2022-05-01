/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AndroidUI.Exceptions;
using AndroidUI.Extensions;
using SkiaSharp;

namespace AndroidUI
{
    /**
     * The Path class encapsulates compound (multiple contour) geometric paths
     * consisting of straight line segments, quadratic curves, and cubic curves.
     * It can be drawn with canvas.drawPath(path, paint), either filled or stroked
     * (based on the paint's Style), or it can be used for clipping or to draw
     * text on a path.
     */
    public class Path
    {

        /**
         * @hide
         */
        internal SKPath mNativePath;

        /**
         * @hide
         */
        internal bool isSimplePath = true;
        /**
         * @hide
         */
        internal Region rects;
        private Direction mLastDirection = null;

        /**
         * Create an empty path
         */
        public Path()
        {
            mNativePath = new SKPath();
        }

        /**
         * Create a new path, copying the contents from the src path.
         *
         * @param src The path to copy from when initializing the new path
         */
        public Path(Path src)
        {
            SKPath valNative = null;
            if (src != null)
            {
                valNative = src.mNativePath;
                isSimplePath = src.isSimplePath;
                if (src.rects != null)
                {
                    rects = new Region(src.rects);
                }
            }
            mNativePath = new SKPath(valNative);
        }

        /**
         * Clear any lines and curves from the path, making it empty.
         * This does NOT change the fill-type setting.
         */
        public void reset()
        {
            isSimplePath = true;
            mLastDirection = null;
            if (rects != null) rects.setEmpty();
            // We promised not to change this, so preserve it around the native
            // call, which does now reset fill type.
            FillType fillType = getFillType();
            mNativePath.Reset();
            setFillType(fillType);
        }

        /**
         * Rewinds the path: clears any lines and curves from the path but
         * keeps the internal data structure for faster reuse.
         */
        public void rewind()
        {
            isSimplePath = true;
            mLastDirection = null;
            if (rects != null) rects.setEmpty();
            mNativePath.Rewind();
        }

        /** Replace the contents of this with the contents of src.
        */
        public void set(Path src)
        {
            if (this == src)
            {
                return;
            }
            isSimplePath = src.isSimplePath;
            mNativePath.Dispose();
            mNativePath = src.mNativePath;
            if (!isSimplePath)
            {
                return;
            }

            if (rects != null && src.rects != null)
            {
                rects.set(src.rects);
            }
            else if (rects != null && src.rects == null)
            {
                rects.setEmpty();
            }
            else if (src.rects != null)
            {
                rects = new Region(src.rects);
            }
        }

        /**
         * The logical operations that can be performed when combining two paths.
         *
         * @see #op(Path, android.graphics.Path.Op)
         * @see #op(Path, Path, android.graphics.Path.Op)
         */
        public class Op
        {
            /**
             * Subtract the second path from the first path.
             */
            public static readonly Op DIFFERENCE = new(0);
            /**
             * Intersect the two paths.
             */
            public static readonly Op INTERSECT = new(1);
            /**
             * Union (inclusive-or) the two paths.
             */
            public static readonly Op UNION = new(2);
            /**
             * Exclusive-or the two paths.
             */
            public static readonly Op XOR = new(3);
            /**
             * Subtract the first path from the second path.
             */
            public static readonly Op REVERSE_DIFFERENCE = new(4);

            internal Op(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }

            /**
             * @hide
             */
            internal readonly int nativeInt;
        }

        /**
         * Set this path to the result of applying the Op to this path and the specified path.
         * The resulting path will be constructed from non-overlapping contours.
         * The curve order is reduced where possible so that cubics may be turned
         * into quadratics, and quadratics maybe turned into lines.
         *
         * @param path The second operand (for difference, the subtrahend)
         *
         * @return True if operation succeeded, false otherwise and this path remains unmodified.
         *
         * @see Op
         * @see #op(Path, Path, android.graphics.Path.Op)
         */
        public bool op(Path path, Op op)
        {
            return this.op(this, path, op);
        }

        /**
         * Set this path to the result of applying the Op to the two specified paths.
         * The resulting path will be constructed from non-overlapping contours.
         * The curve order is reduced where possible so that cubics may be turned
         * into quadratics, and quadratics maybe turned into lines.
         *
         * @param path1 The first operand (for difference, the minuend)
         * @param path2 The second operand (for difference, the subtrahend)
         *
         * @return True if operation succeeded, false otherwise and this path remains unmodified.
         *
         * @see Op
         * @see #op(Path, android.graphics.Path.Op)
         */
        public bool op(Path path1, Path path2, Op op)
        {
            SKPath p1 = path1.mNativePath;
            SKPath p2 = path2.mNativePath;
            SKPathOp op1 = (SKPathOp)op.nativeInt;
            SKPath r = mNativePath;
            bool result = p1.Op(p2, op1, r);


            if (result)
            {
                isSimplePath = false;
                rects = null;
                return true;
            }
            return false;
        }

        /**
         * Returns the path's convexity, as defined by the content of the path.
         * <p>
         * A path is convex if it has a single contour, and only ever curves in a
         * single direction.
         * <p>
         * This function will calculate the convexity of the path from its control
         * points, and cache the result.
         *
         * @return True if the path is convex.
         *
         * @deprecated This method is not reliable. The way convexity is computed may change from
         * release to release, and convexity could change based on a matrix as well. This method was
         * useful when non-convex Paths were unable to be used in certain contexts, but that is no
         * longer the case.
         */
        public bool isConvex()
        {
            return mNativePath.IsConvex;
        }

        /**
         * Enum for the ways a path may be filled.
         */
        public class FillType
        {
            // these must match the values in SKPath.h
            /**
             * Specifies that "inside" is computed by a non-zero sum of signed
             * edge crossings.
             */
            public static readonly FillType WINDING = new(0);
            /**
             * Specifies that "inside" is computed by an odd number of edge
             * crossings.
             */
            public static readonly FillType EVEN_ODD = new(1);
            /**
             * Same as {@link #WINDING}, but draws outside of the path, rather than inside.
             */
            public static readonly FillType INVERSE_WINDING = new(2);
            /**
             * Same as {@link #EVEN_ODD}, but draws outside of the path, rather than inside.
             */
            public static readonly FillType INVERSE_EVEN_ODD = new(3);

            internal FillType(int ni)
            {
                nativeInt = ni;
            }

            internal int nativeInt;
        }

        // these must be in the same order as their native values
        static readonly FillType[] sFillTypeArray = {
        FillType.WINDING, FillType.EVEN_ODD,
        FillType.INVERSE_WINDING, FillType.INVERSE_EVEN_ODD
};

        /**
         * Return the path's fill type. This defines how "inside" is
         * computed. The default value is WINDING.
         *
         * @return the path's fill type
         */
        public FillType getFillType()
        {
            return sFillTypeArray[(int)mNativePath.FillType];
        }

        /**
         * Set the path's fill type. This defines how "inside" is computed.
         *
         * @param ft The new fill type for this path
         */
        public void setFillType(FillType ft)
        {
            mNativePath.FillType = (SKPathFillType)ft.nativeInt;
        }

        /**
         * Returns true if the filltype is one of the INVERSE variants
         *
         * @return true if the filltype is one of the INVERSE variants
         */
        public bool isInverseFillType()
        {
            int ft = (int)mNativePath.FillType;
            return (ft & FillType.INVERSE_WINDING.nativeInt) != 0;
        }

        /**
         * Toggles the INVERSE state of the filltype
         */
        public void toggleInverseFillType()
        {
            int ft = (int)mNativePath.FillType;
            ft ^= FillType.INVERSE_WINDING.nativeInt;
            mNativePath.FillType = (SKPathFillType)ft;
        }

        /**
         * Returns true if the path is empty (contains no lines or curves)
         *
         * @return true if the path is empty (contains no lines or curves)
         */
        public bool isEmpty()
        {
            return mNativePath.IsEmpty;
        }

        /**
         * Returns true if the path specifies a rectangle. If so, and if rect is
         * not null, set rect to the bounds of the path. If the path does not
         * specify a rectangle, return false and ignore rect.
         *
         * @param rect If not null, returns the bounds of the path if it specifies
         *             a rectangle
         * @return     true if the path specifies a rectangle
         */
        public bool isRect(RectF rect)
        {
            SKRect r = new SKRect();
            bool isClosed;
            SKPathDirection direction;
            r = mNativePath.GetRect(out isClosed, out direction);

            int currVerb = 0;
            SKPoint[] pts = mNativePath.Points;
            return mNativePath.IsRectContour(false, ref currVerb, ref pts, ref isClosed, ref direction, ref r);
        }

        /**
         * Compute the bounds of the control points of the path, and write the
         * answer into bounds. If the path contains 0 or 1 points, the bounds is
         * set to (0,0,0,0)
         *
         * @param bounds Returns the computed bounds of the path's control points.
         * @param exact This parameter is no longer used.
         */
        public void computeBounds(RectF bounds, bool exact)
        {
            int pointCount = mNativePath.PointCount;
            if (pointCount == 0 || pointCount == 1)
            {
                bounds.setEmpty();
            }
            else
            {
                SKRect b;
                mNativePath.GetBounds(out b);
                bounds.top = b.Top;
                bounds.left = b.Left;
                bounds.right = b.Right;
                bounds.bottom = b.Bottom;
            }
        }

        /**
         * Hint to the path to prepare for adding more points. This can allow the
         * path to more efficiently allocate its storage.
         *
         * @param extraPtCount The number of extra points that may be added to this
         *                     path
         */
        public void incReserve(int extraPtCount)
        {
            Log.w("TAG", "incReserve is not yet implemented by SkiaSharp, this is a NOP");
            //nIncReserve(mNativePath.Inc, extraPtCount);
        }

        /**
         * Set the beginning of the next contour to the point (x,y).
         *
         * @param x The x-coordinate of the start of a new contour
         * @param y The y-coordinate of the start of a new contour
         */
        public void moveTo(float x, float y)
        {
            mNativePath.MoveTo(x, y);
        }

        /**
         * Set the beginning of the next contour relative to the last point on the
         * previous contour. If there is no previous contour, this is treated the
         * same as moveTo().
         *
         * @param dx The amount to add to the x-coordinate of the end of the
         *           previous contour, to specify the start of a new contour
         * @param dy The amount to add to the y-coordinate of the end of the
         *           previous contour, to specify the start of a new contour
         */
        public void rMoveTo(float dx, float dy)
        {
            mNativePath.RMoveTo(dx, dy);
        }

        /**
         * Add a line from the last point to the specified point (x,y).
         * If no moveTo() call has been made for this contour, the first point is
         * automatically set to (0,0).
         *
         * @param x The x-coordinate of the end of a line
         * @param y The y-coordinate of the end of a line
         */
        public void lineTo(float x, float y)
        {
            isSimplePath = false;
            mNativePath.LineTo(x, y);
        }

        /**
         * Same as lineTo, but the coordinates are considered relative to the last
         * point on this contour. If there is no previous point, then a moveTo(0,0)
         * is inserted automatically.
         *
         * @param dx The amount to add to the x-coordinate of the previous point on
         *           this contour, to specify a line
         * @param dy The amount to add to the y-coordinate of the previous point on
         *           this contour, to specify a line
         */
        public void rLineTo(float dx, float dy)
        {
            isSimplePath = false;
            mNativePath.RLineTo(dx, dy);
        }

        /**
         * Add a quadratic bezier from the last point, approaching control point
         * (x1,y1), and ending at (x2,y2). If no moveTo() call has been made for
         * this contour, the first point is automatically set to (0,0).
         *
         * @param x1 The x-coordinate of the control point on a quadratic curve
         * @param y1 The y-coordinate of the control point on a quadratic curve
         * @param x2 The x-coordinate of the end point on a quadratic curve
         * @param y2 The y-coordinate of the end point on a quadratic curve
         */
        public void quadTo(float x1, float y1, float x2, float y2)
        {
            isSimplePath = false;
            mNativePath.QuadTo(x1, y1, x2, y2);
        }

        /**
         * Same as quadTo, but the coordinates are considered relative to the last
         * point on this contour. If there is no previous point, then a moveTo(0,0)
         * is inserted automatically.
         *
         * @param dx1 The amount to add to the x-coordinate of the last point on
         *            this contour, for the control point of a quadratic curve
         * @param dy1 The amount to add to the y-coordinate of the last point on
         *            this contour, for the control point of a quadratic curve
         * @param dx2 The amount to add to the x-coordinate of the last point on
         *            this contour, for the end point of a quadratic curve
         * @param dy2 The amount to add to the y-coordinate of the last point on
         *            this contour, for the end point of a quadratic curve
         */
        public void rQuadTo(float dx1, float dy1, float dx2, float dy2)
        {
            isSimplePath = false;
            mNativePath.RQuadTo(dx1, dy1, dx2, dy2);
        }

        /**
         * Add a cubic bezier from the last point, approaching control points
         * (x1,y1) and (x2,y2), and ending at (x3,y3). If no moveTo() call has been
         * made for this contour, the first point is automatically set to (0,0).
         *
         * @param x1 The x-coordinate of the 1st control point on a cubic curve
         * @param y1 The y-coordinate of the 1st control point on a cubic curve
         * @param x2 The x-coordinate of the 2nd control point on a cubic curve
         * @param y2 The y-coordinate of the 2nd control point on a cubic curve
         * @param x3 The x-coordinate of the end point on a cubic curve
         * @param y3 The y-coordinate of the end point on a cubic curve
         */
        public void cubicTo(float x1, float y1, float x2, float y2,
                            float x3, float y3)
        {
            isSimplePath = false;
            mNativePath.CubicTo(x1, y1, x2, y2, x3, y3);
        }

        /**
         * Same as cubicTo, but the coordinates are considered relative to the
         * current point on this contour. If there is no previous point, then a
         * moveTo(0,0) is inserted automatically.
         */
        public void rCubicTo(float x1, float y1, float x2, float y2,
                             float x3, float y3)
        {
            isSimplePath = false;
            mNativePath.RCubicTo(x1, y1, x2, y2, x3, y3);
        }

        /**
         * Append the specified arc to the path as a new contour. If the start of
         * the path is different from the path's current last point, then an
         * automatic lineTo() is added to connect the current contour to the
         * start of the arc. However, if the path is empty, then we call moveTo()
         * with the first point of the arc.
         *
         * @param oval        The bounds of oval defining shape and size of the arc
         * @param startAngle  Starting angle (in degrees) where the arc begins
         * @param sweepAngle  Sweep angle (in degrees) measured clockwise, treated
         *                    mod 360.
         * @param forceMoveTo If true, always begin a new contour with the arc
         */
        public void arcTo(RectF oval, float startAngle, float sweepAngle,
                          bool forceMoveTo)
        {
            arcTo(oval.left, oval.top, oval.right, oval.bottom, startAngle, sweepAngle, forceMoveTo);
        }

        /**
         * Append the specified arc to the path as a new contour. If the start of
         * the path is different from the path's current last point, then an
         * automatic lineTo() is added to connect the current contour to the
         * start of the arc. However, if the path is empty, then we call moveTo()
         * with the first point of the arc.
         *
         * @param oval        The bounds of oval defining shape and size of the arc
         * @param startAngle  Starting angle (in degrees) where the arc begins
         * @param sweepAngle  Sweep angle (in degrees) measured clockwise
         */
        public void arcTo(RectF oval, float startAngle, float sweepAngle)
        {
            arcTo(oval.left, oval.top, oval.right, oval.bottom, startAngle, sweepAngle, false);
        }

        /**
         * Append the specified arc to the path as a new contour. If the start of
         * the path is different from the path's current last point, then an
         * automatic lineTo() is added to connect the current contour to the
         * start of the arc. However, if the path is empty, then we call moveTo()
         * with the first point of the arc.
         *
         * @param startAngle  Starting angle (in degrees) where the arc begins
         * @param sweepAngle  Sweep angle (in degrees) measured clockwise, treated
         *                    mod 360.
         * @param forceMoveTo If true, always begin a new contour with the arc
         */
        public void arcTo(float left, float top, float right, float bottom, float startAngle,
                float sweepAngle, bool forceMoveTo)
        {
            isSimplePath = false;
            SKRect oval = new SKRect(left, top, right, bottom);
            mNativePath.ArcTo(oval, startAngle, sweepAngle, forceMoveTo);
        }

        /**
         * Close the current contour. If the current point is not equal to the
         * first point of the contour, a line segment is automatically added.
         */
        public void close()
        {
            isSimplePath = false;
            mNativePath.Close();
        }

        /**
         * Specifies how closed shapes (e.g. rects, ovals) are oriented when they
         * are added to a path.
         */
        public class Direction
        {
            /** clockwise */
            public static readonly Direction CW = new(0);    // must match enum in SKPath.h
            /** counter-clockwise */
            public static readonly Direction CCW = new(1);    // must match enum in SKPath.h

            Direction(int ni)
            {
                nativeInt = ni;
            }
            internal int nativeInt;
        }

        private void detectSimplePath(float left, float top, float right, float bottom, Direction dir)
        {
            if (mLastDirection == null)
            {
                mLastDirection = dir;
            }
            if (mLastDirection != dir)
            {
                isSimplePath = false;
            }
            else
            {
                if (rects == null) rects = new Region();
                rects.op((int)left, (int)top, (int)right, (int)bottom, Region.Op.UNION);
            }
        }

        /**
         * Add a closed rectangle contour to the path
         *
         * @param rect The rectangle to add as a closed contour to the path
         * @param dir  The direction to wind the rectangle's contour
         */
        public void addRect(RectF rect, Direction dir)
        {
            addRect(rect.left, rect.top, rect.right, rect.bottom, dir);
        }

        /**
         * Add a closed rectangle contour to the path
         *
         * @param left   The left side of a rectangle to add to the path
         * @param top    The top of a rectangle to add to the path
         * @param right  The right side of a rectangle to add to the path
         * @param bottom The bottom of a rectangle to add to the path
         * @param dir    The direction to wind the rectangle's contour
         */
        public void addRect(float left, float top, float right, float bottom, Direction dir)
        {
            detectSimplePath(left, top, right, bottom, dir);
            SKRect rect = new SKRect(left, top, right, bottom);
            mNativePath.AddRect(rect, (SKPathDirection)dir.nativeInt);
        }

        /**
         * Add a closed oval contour to the path
         *
         * @param oval The bounds of the oval to add as a closed contour to the path
         * @param dir  The direction to wind the oval's contour
         */
        public void addOval(RectF oval, Direction dir)
        {
            addOval(oval.left, oval.top, oval.right, oval.bottom, dir);
        }

        /**
         * Add a closed oval contour to the path
         *
         * @param dir The direction to wind the oval's contour
         */
        public void addOval(float left, float top, float right, float bottom, Direction dir)
        {
            isSimplePath = false;
            SKRect oval = new SKRect(left, top, right, bottom);
            mNativePath.AddOval(oval, (SKPathDirection)dir.nativeInt);
        }

        /**
         * Add a closed circle contour to the path
         *
         * @param x   The x-coordinate of the center of a circle to add to the path
         * @param y   The y-coordinate of the center of a circle to add to the path
         * @param radius The radius of a circle to add to the path
         * @param dir    The direction to wind the circle's contour
         */
        public void addCircle(float x, float y, float radius, Direction dir)
        {
            isSimplePath = false;
            mNativePath.AddCircle(x, y, radius, (SKPathDirection)dir.nativeInt);
        }

        /**
         * Add the specified arc to the path as a new contour.
         *
         * @param oval The bounds of oval defining the shape and size of the arc
         * @param startAngle Starting angle (in degrees) where the arc begins
         * @param sweepAngle Sweep angle (in degrees) measured clockwise
         */
        public void addArc(RectF oval, float startAngle, float sweepAngle)
        {
            addArc(oval.left, oval.top, oval.right, oval.bottom, startAngle, sweepAngle);
        }

        /**
         * Add the specified arc to the path as a new contour.
         *
         * @param startAngle Starting angle (in degrees) where the arc begins
         * @param sweepAngle Sweep angle (in degrees) measured clockwise
         */
        public void addArc(float left, float top, float right, float bottom, float startAngle,
                float sweepAngle)
        {
            isSimplePath = false;
            SKRect oval = new SKRect(left, top, right, bottom);
            mNativePath.AddArc(oval, startAngle, sweepAngle);
        }

        /**
            * Add a closed round-rectangle contour to the path
         *
         * @param rect The bounds of a round-rectangle to add to the path
         * @param rx   The x-radius of the rounded corners on the round-rectangle
         * @param ry   The y-radius of the rounded corners on the round-rectangle
         * @param dir  The direction to wind the round-rectangle's contour
         */
        public void addRoundRect(RectF rect, float rx, float ry, Direction dir)
        {
            addRoundRect(rect.left, rect.top, rect.right, rect.bottom, rx, ry, dir);
        }

        /**
         * Add a closed round-rectangle contour to the path
         *
         * @param rx   The x-radius of the rounded corners on the round-rectangle
         * @param ry   The y-radius of the rounded corners on the round-rectangle
         * @param dir  The direction to wind the round-rectangle's contour
         */
        public void addRoundRect(float left, float top, float right, float bottom, float rx, float ry,
                Direction dir)
        {
            isSimplePath = false;
            SKRect rect = new SKRect(left, top, right, bottom);
            mNativePath.AddRoundRect(rect, rx, ry, (SKPathDirection)dir.nativeInt);
        }

        /**
         * Add a closed round-rectangle contour to the path. Each corner receives
         * two radius values [X, Y]. The corners are ordered top-left, top-right,
         * bottom-right, bottom-left
         *
         * @param rect The bounds of a round-rectangle to add to the path
         * @param radii Array of 8 values, 4 pairs of [X,Y] radii
         * @param dir  The direction to wind the round-rectangle's contour
         */
        public void addRoundRect(RectF rect, float[] radii, Direction dir)
        {
            if (rect == null)
            {
                throw new NullReferenceException("need rect parameter");
            }
            addRoundRect(rect.left, rect.top, rect.right, rect.bottom, radii, dir);
        }

        /**
         * Add a closed round-rectangle contour to the path. Each corner receives
         * two radius values [X, Y]. The corners are ordered top-left, top-right,
         * bottom-right, bottom-left
         *
         * @param radii Array of 8 values, 4 pairs of [X,Y] radii
         * @param dir  The direction to wind the round-rectangle's contour
         */
        public void addRoundRect(float left, float top, float right, float bottom,
                float[] radii, Direction dir)
        {
            if (radii.Length < 8)
            {
                throw new IndexOutOfRangeException("radii[] needs 8 values");
            }
            isSimplePath = false;
            SKPoint[] radiis = new SKPoint[4];
            for (int i = 0, ii = 0; i < radii.Length; i++)
            {
                radiis[i].X = radii[ii++];
                radiis[i].Y = radii[ii++];
            }
            SKRoundRect rect = new SKRoundRect();
            rect.SetRectRadii(new SKRect(left, top, right, bottom), radiis);
            mNativePath.AddRoundRect(rect, (SKPathDirection)dir.nativeInt);
            rect.Dispose();
        }

        /**
         * Add a copy of src to the path, offset by (dx,dy)
         *
         * @param src The path to add as a new contour
         * @param dx  The amount to translate the path in X as it is added
         */
        public void addPath(Path src, float dx, float dy)
        {
            isSimplePath = false;
            mNativePath.AddPath(src.mNativePath, dx, dy);
        }

        /**
         * Add a copy of src to the path
         *
         * @param src The path that is appended to the current path
         */
        public void addPath(Path src)
        {
            isSimplePath = false;
            mNativePath.AddPath(src.mNativePath);
        }

        /**
         * Add a copy of src to the path, transformed by matrix
         *
         * @param src The path to add as a new contour
         */
        public void addPath(Path src, SKMatrix matrix)
        {
            if (!src.isSimplePath) isSimplePath = false;
            mNativePath.AddPath(src.mNativePath, ref matrix);
        }

        /**
         * Offset the path by (dx,dy)
         *
         * @param dx  The amount in the X direction to offset the entire path
         * @param dy  The amount in the Y direction to offset the entire path
         * @param dst The translated path is written here. If this is null, then
         *            the original path is modified.
         */
        public void offset(float dx, float dy, Path dst)
        {
            if (dst != null)
            {
                dst.set(this);
            }
            else
            {
                dst = this;
            }
            dst.offset(dx, dy);
        }

        /**
         * Offset the path by (dx,dy)
         *
         * @param dx The amount in the X direction to offset the entire path
         * @param dy The amount in the Y direction to offset the entire path
         */
        public void offset(float dx, float dy)
        {
            if (isSimplePath && rects == null)
            {
                // nothing to offset
                return;
            }
            if (isSimplePath && dx == MathUtils.rint(dx) && dy == MathUtils.rint(dy))
            {
                rects.translate((int)dx, (int)dy);
            }
            else
            {
                isSimplePath = false;
            }
            mNativePath.Offset(dx, dy);
        }

        /**
         * Sets the last point of the path.
         *
         * @param dx The new X coordinate for the last point
         * @param dy The new Y coordinate for the last point
         */
        public void setLastPoint(float dx, float dy)
        {
            isSimplePath = false;
            mNativePath.SetLastPt(dx, dy);
        }

        /**
         * Transform the points in this path by matrix, and write the answer
         * into dst. If dst is null, then the the original path is modified.
         *
         * @param matrix The matrix to apply to the path
         * @param dst    The transformed path is written here. If dst is null,
         *               then the the original path is modified
         */
        public void transform(SKMatrix matrix, Path dst)
        {
            SKPath dstNative = null;
            if (dst != null)
            {
                dst.isSimplePath = false;
                dstNative = dst.mNativePath;
            }
            mNativePath.Transform(matrix, dstNative);
        }

        /**
         * Transform the points in this path by matrix.
         *
         * @param matrix The matrix to apply to the path
         */
        public void transform(SKMatrix matrix)
        {
            isSimplePath = false;
            mNativePath.Transform(matrix);
        }

        /** @hide */
        internal SKPath readOnlyNI()
        {
            return mNativePath;
        }

        internal SKPath mutateNI()
        {
            isSimplePath = false;
            return mNativePath;
        }

        /**
         * Approximate the <code>Path</code> with a series of line segments.
         * This returns float[] with the array containing point components.
         * There are three components for each point, in order:
         * <ul>
         *     <li>Fraction along the length of the path that the point resides</li>
         *     <li>The x coordinate of the point</li>
         *     <li>The y coordinate of the point</li>
         * </ul>
         * <p>Two points may share the same fraction along its length when there is
         * a move action within the Path.</p>
         *
         * @param acceptableError The acceptable error for a line on the
         *                        Path. Typically this would be 0.5 so that
         *                        the error is less than half a pixel.
         * @return An array of components for points approximating the Path.
         */
        public float[] approximate(float acceptableError)
        {
            if (acceptableError < 0)
            {
                throw new IllegalArgumentException("AcceptableError must be greater than or equal to 0");
            }
            return mNativePath.Approximate(acceptableError);
        }

    }
}