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

namespace AndroidUI
{
    public class Region
    {

        private const int MAX_POOL_SIZE = 10;

        private static readonly SynchronizedPool<Region> sPool =
                new(MAX_POOL_SIZE);

        /**
         * @hide
         */
        internal SkiaSharp.SKRegion mNativeRegion;

        // the native values for these must match up with the enum in SkRegion.h
        public class Op
        {
            public static readonly Op DIFFERENCE = new(0);
            public static readonly Op INTERSECT = new(1);
            public static readonly Op UNION = new(2);
            public static readonly Op XOR = new(3);
            public static readonly Op REVERSE_DIFFERENCE = new(4);
            public static readonly Op REPLACE = new(5);

            internal Op(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }

            /**
             * @hide
             */
            internal readonly int nativeInt;
        }

        /** Create an empty region
*/
        public Region()
        {
            mNativeRegion = new SkiaSharp.SKRegion();
        }

        /** Return a copy of the specified region
*/
        public Region(Region region)
        {
            mNativeRegion = new SkiaSharp.SKRegion();
            mNativeRegion.SetRegion(region.mNativeRegion);
        }

        /** Return a region set to the specified rectangle
*/
        public Region(Rect r)
        {
            mNativeRegion = new SkiaSharp.SKRegion();
            mNativeRegion.SetRect(new SkiaSharp.SKRectI(r.left, r.top, r.right, r.bottom));
        }

        /** Return a region set to the specified rectangle
*/
        public Region(int left, int top, int right, int bottom)
        {
            mNativeRegion = new SkiaSharp.SKRegion();
            mNativeRegion.SetRect(new SkiaSharp.SKRectI(left, top, right, bottom));
        }

        /** Set the region to the empty region
*/
        public void setEmpty()
        {
            mNativeRegion.SetRect(new SkiaSharp.SKRectI(0, 0, 0, 0));
        }

        /** Set the region to the specified region.
*/
        public bool set(Region region)
        {
            mNativeRegion.SetRegion(region.mNativeRegion);
            return true;
        }

        /** Set the region to the specified rectangle
*/
        public bool set(Rect r)
        {
            return mNativeRegion.SetRect(new SkiaSharp.SKRectI(r.left, r.top, r.right, r.bottom));
        }

        /** Set the region to the specified rectangle
*/
        public bool set(int left, int top, int right, int bottom)
        {
            return mNativeRegion.SetRect(new SkiaSharp.SKRectI(left, top, right, bottom));
        }

        /**
         * Set the region to the area described by the path and clip.
         * Return true if the resulting region is non-empty. This produces a region
         * that is identical to the pixels that would be drawn by the path
         * (with no antialiasing).
         */
        public bool setPath(SkiaSharp.SKPath path, Region clip)
        {
            return mNativeRegion.SetPath(path, clip.mNativeRegion);
        }

        /**
         * Return true if this region is empty
         */
        public bool isEmpty()
        {
            return mNativeRegion.IsEmpty;
        }

        /**
         * Return true if the region contains a single rectangle
         */
        public bool isRect()
        {
            return mNativeRegion.IsRect;
        }

        /**
         * Return true if the region contains more than one rectangle
         */
        public bool isComplex()
        {
            return mNativeRegion.IsComplex;
        }

        /**
         * Return a new Rect set to the bounds of the region. If the region is
         * empty, the Rect will be set to [0, 0, 0, 0]
         */
        public Rect getBounds()
        {
            SkiaSharp.SKRectI bounds = mNativeRegion.Bounds;
            return new Rect(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        }

        /**
         * Set the Rect to the bounds of the region. If the region is empty, the
         * Rect will be set to [0, 0, 0, 0]
         */
        public bool getBounds(Rect r)
        {
            if (r == null)
            {
                throw new NullReferenceException();
            }
            SkiaSharp.SKRectI bounds = mNativeRegion.Bounds;
            r.set(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
            return !mNativeRegion.IsEmpty;
        }

        /**
         * Return the boundary of the region as a new Path. If the region is empty,
         * the path will also be empty.
         */
        public Path getBoundaryPath()
        {
            Path path = new();
            // TODO
            //nativeGetBoundaryPath(mNativeRegion, path.Mutate());
            return null;
        }

#if false
        /**
         * Set the path to the boundary of the region. If the region is empty, the
         * path will also be empty.
         */
        public bool getBoundaryPath(Path path)
        {
            throw new NotImplementedException("SkiaSharp does not yet expose SkRegion::getBoundaryPath(SkPath* path)");
            //return mNativeRegion.GetBoundaryPath(path.mutateNI());
        }

#endif

        /**
         * Return true if the region contains the specified point
         */
        public bool contains(int x, int y)
        {
            return mNativeRegion.Contains(x, y);
        }

        /**
         * Return true if the region is a single rectangle (not complex) and it
         * contains the specified rectangle. Returning false is not a guarantee
         * that the rectangle is not contained by this region, but return true is a
         * guarantee that the rectangle is contained by this region.
         */
        public bool quickContains(Rect r)
        {
            return quickContains(r.left, r.top, r.right, r.bottom);
        }

        /**
         * Return true if the region is a single rectangle (not complex) and it
         * contains the specified rectangle. Returning false is not a guarantee
         * that the rectangle is not contained by this region, but return true is a
         * guarantee that the rectangle is contained by this region.
         */
        public bool quickContains(int left, int top, int right,
                                            int bottom)
        {
            return mNativeRegion.QuickContains(new SkiaSharp.SKRectI(left, top, right, bottom));
        }

        /**
         * Return true if the region is empty, or if the specified rectangle does
         * not intersect the region. Returning false is not a guarantee that they
         * intersect, but returning true is a guarantee that they do not.
         */
        public bool quickReject(Rect r)
        {
            return quickReject(r.left, r.top, r.right, r.bottom);
        }

        /**
         * Return true if the region is empty, or if the specified rectangle does
         * not intersect the region. Returning false is not a guarantee that they
         * intersect, but returning true is a guarantee that they do not.
         */
        public bool quickReject(int left, int top, int right, int bottom)
        {
            return mNativeRegion.QuickReject(new SkiaSharp.SKRectI(left, top, right, bottom));
        }

        /**
         * Return true if the region is empty, or if the specified region does not
         * intersect the region. Returning false is not a guarantee that they
         * intersect, but returning true is a guarantee that they do not.
         */
        public bool quickReject(Region rgn)
        {
            return mNativeRegion.QuickReject(rgn.mNativeRegion);
        }

        /**
         * Translate the region by [dx, dy]. If the region is empty, do nothing.
         */
        public void translate(int dx, int dy)
        {
            translate(dx, dy, null);
        }

        /**
         * Set the dst region to the result of translating this region by [dx, dy].
         * If this region is empty, then dst will be set to empty.
         */
        public void translate(int dx, int dy, Region dst)
        {
            dst.mNativeRegion.SetRegion(mNativeRegion);
            dst.mNativeRegion.Translate(dx, dy);
        }

        /**
         * Scale the region by the given scale amount. This re-constructs new region by
         * scaling the rects that this region consists of. New rectis are computed by scaling 
         * coordinates by float, then rounded by roundf() function to integers. This may results
         * in less internal rects if 0 < scale < 1. Zero and Negative scale result in
         * an empty region. If this region is empty, do nothing.
         *
         * @hide
         */
        public void scale(float scale_)
        {
            scale(scale_, null);
        }

        /**
         * Set the dst region to the result of scaling this region by the given scale amount.
         * If this region is empty, then dst will be set to empty.
         * @hide
         */
        public void scale(float scale, Region dst)
        {
            dst.mNativeRegion.SetRegion(mNativeRegion);
            throw new NotSupportedException("region scaling is not yet supported");
        }

        public bool union(Rect r)
        {
            return op(r, new Op(Op.UNION.nativeInt));
        }

        /**
         * Perform the specified Op on this region and the specified rect. Return
         * true if the result of the op is not empty.
         */
        public bool op(Rect r, Op op)
        {
            return mNativeRegion.Op(r.left, r.top, r.right, r.bottom, (SkiaSharp.SKRegionOperation)op.nativeInt);
        }

        /**
         * Perform the specified Op on this region and the specified rect. Return
         * true if the result of the op is not empty.
         */
        public bool op(int left, int top, int right, int bottom, Op op)
        {
            return mNativeRegion.Op(left, top, right, bottom, (SkiaSharp.SKRegionOperation)op.nativeInt);
        }

        /**
         * Perform the specified Op on this region and the specified region. Return
         * true if the result of the op is not empty.
         */
        public bool op(Region region, Op op_)
        {
            return op(this, region, op_);
        }

        /**
         * Set this region to the result of performing the Op on the specified rect
         * and region. Return true if the result is not empty.
         */
        public bool op(Rect rect, Region region, Op op)
        {
            // TODO is this correct?
            mNativeRegion.Op(rect.left, rect.top, rect.right, rect.bottom, (SkiaSharp.SKRegionOperation)op.nativeInt);
            mNativeRegion.Op(region.mNativeRegion, (SkiaSharp.SKRegionOperation)op.nativeInt);
            return !mNativeRegion.IsEmpty;
        }

        /**
         * Set this region to the result of performing the Op on the specified
         * regions. Return true if the result is not empty.
         */
        public bool op(Region region1, Region region2, Op op)
        {
            mNativeRegion.Op(region1.mNativeRegion, (SkiaSharp.SKRegionOperation)op.nativeInt);
            mNativeRegion.Op(region2.mNativeRegion, (SkiaSharp.SKRegionOperation)op.nativeInt);
            return !mNativeRegion.IsEmpty;
        }

        override public string ToString()
        {
            return mNativeRegion.ToString();
        }

        /**
         * @return An instance from a pool if such or a new one.
         *
         * @hide
         */
        internal static Region obtain()
        {
            Region region = sPool.Aquire();
            return (region != null) ? region : new Region();
        }

        /**
         * @return An instance from a pool if such or a new one.
         *
         * @param other Region to copy values from for initialization.
         *
         * @hide
         */
        internal static Region obtain(Region other)
        {
            Region region = obtain();
            region.set(other);
            return region;
        }

        /**
         * Recycles an instance.
         *
         * @hide
         */
        public void recycle()
        {
            setEmpty();
            sPool.Release(this);
        }


        override public bool Equals(object obj)
        {
            if (obj == null || !(obj is Region))
            {
                return false;
            }
            Region peer = (Region)obj;
            return mNativeRegion.Equals(peer.mNativeRegion);
        }

        ~Region()
        {
            mNativeRegion.Dispose();
            mNativeRegion = null;
        }

        internal Region(SkiaSharp.SKRegion ni)
        {
            if (ni == null)
            {
                throw new Exception();
            }
            mNativeRegion = ni;
        }

        /* Add an unused parameter so constructor can be called from jni without
           triggering 'not cloneable' exception */
        private Region(SkiaSharp.SKRegion ni, int unused)
        : this(ni)
        {
        }

        SkiaSharp.SKRegion ni()
        {
            return mNativeRegion;
        }
    }
}