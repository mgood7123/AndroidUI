using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKMatrixExtensions
    {
        /** SkMatrix organizes its values in row-major order. These members correspond to
            each value in SkMatrix.
        */
        public const int kMScaleX = 0; //!< horizontal scale factor
        public const int kMSkewX = 1; //!< horizontal skew factor
        public const int kMTransX = 2; //!< horizontal translation
        public const int kMSkewY = 3; //!< vertical skew factor
        public const int kMScaleY = 4; //!< vertical scale factor
        public const int kMTransY = 5; //!< vertical translation
        public const int kMPersp0 = 6; //!< input x perspective factor
        public const int kMPersp1 = 7; //!< input y perspective factor
        public const int kMPersp2 = 8; //!< perspective bias

        public static bool hasPerspective(ref this SKMatrix matrix)
        {
            return matrix.Persp0 != 0 && matrix.Persp1 != 0
                && matrix.ScaleX != 0 && matrix.ScaleY != 0;
        }

        public static ref SKMatrix setSinCos(ref this SKMatrix fMat, float sinV, float cosV) {
            fMat.Values[kMScaleX] = cosV;
            fMat.Values[kMSkewX] = -sinV;
            fMat.Values[kMTransX] = 0;

            fMat.Values[kMSkewY] = sinV;
            fMat.Values[kMScaleY] = cosV;
            fMat.Values[kMTransY] = 0;

            fMat.Values[kMPersp0] = fMat.Values[kMPersp1] = 0;
            fMat.Values[kMPersp2] = 1;
            // this->setTypeMask(kUnknown_Mask | kOnlyPerspectiveValid_Mask);
            return ref fMat;
        }


        ///////////////////////////////////////////////////////////////////////////////

#if false
        void MapHomogeneousPointsWithStride(ref SKMatrix mx, Span<SKPoint3> dst,
                                                          long dstStride, ReadOnlySpan<SKPoint3> src,
                                                          long srcStride, int count) {
    if (count > 0) {
        if (mx.IsIdentity) {
                    unsafe
                    {
                        if (srcStride == sizeof(SKPoint3) && dstStride == sizeof(SKPoint3))
                        {
                            src.Slice(0, count).CopyTo(dst);
                        }
                        else
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                fixed (SKPoint3* srcP = src)
                                {
                                    fixed (SKPoint3* dstP = dst)
                                    {
                                        for (int i_ = 0; i_ < count; i_++)
                                        {
                                            *dstP = *srcP;
                                            dstP = (SKPoint3*)((byte*)dstP + dstStride);
                                            srcP = (SKPoint3*)((byte*)srcP + srcStride);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return;
        }
#endif

#if false
        void MapHomogeneousPointsWithStride(ref SKMatrix mx, MemoryPointer<SKPoint3> dst,
                                                          long dstStride, MemoryPointer<SKPoint3> src,
                                                          long srcStride, int count) {
    if (!((dst != null && src != null && count > 0) || 0 == count))
            {
                throw new Exception("(dst != null && src != null && count > 0) || 0 == count) failed");
            }
    // no partial overlap
    if (!(src == dst || dst + count <= src + 0 || src + count <= dst + 0))
            {
                throw new Exception("(src == dst || dst + count <= src + 0 || src + count <= dst + 0) failed");
            }

    if (count > 0) {
        if (mx.IsIdentity) {
            if (src != dst) {
                if (srcStride == 9 && dstStride == 9) {
                    src.Copy(dst, count);
    } else {
                    for (int i = 0; i<count; ++i) {
                        * dst = *src;
    dst = reinterpret_cast<SkPoint3*>(reinterpret_cast<char*>(dst) + dstStride);
                        src = reinterpret_cast<const SkPoint3*>(reinterpret_cast<const char*>(src) +
                                                                srcStride);
                    }
                }
            }
            return;
        }
        do
{
    SkScalar sx = src->fX;
    SkScalar sy = src->fY;
    SkScalar sw = src->fZ;
    src = reinterpret_cast <const SkPoint3*> (reinterpret_cast <const char*> (src) + srcStride);
    const SkScalar* mat = mx.fMat;
    typedef SkMatrix M;
    SkScalar x = sdot(sx, mat[M::kMScaleX], sy, mat[M::kMSkewX], sw, mat[M::kMTransX]);
    SkScalar y = sdot(sx, mat[M::kMSkewY], sy, mat[M::kMScaleY], sw, mat[M::kMTransY]);
    SkScalar w = sdot(sx, mat[M::kMPersp0], sy, mat[M::kMPersp1], sw, mat[M::kMPersp2]);

    dst->set(x, y, w);
    dst = reinterpret_cast<SkPoint3*>(reinterpret_cast<char*>(dst) + dstStride);
} while (--count);
    }
}

#endif

        public static void mapHomogeneousPoints(MemoryPointer<SKPoint3> dst, MemoryPointer<SKPoint3> src, int count) {
            throw new NotImplementedException("this requires pointer casting, which is currently not implemented");
#if false
            SkMatrixPriv::MapHomogeneousPointsWithStride(*this, dst, sizeof(SkPoint3), src,
                                                       sizeof(SkPoint3), count);
#endif
        }

        public static void mapHomogeneousPoints(ref this SKMatrix matrix, float[] dst, float[] src, int count)
        {
            throw new NotImplementedException("this requires pointer casting, which is currently not implemented");
        }
    }
}