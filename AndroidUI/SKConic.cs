using AndroidUI.Extensions;
using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI
{
    class SKConic
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static Sk2s from_point(SKPoint point) {
            return Sk2s.Load(new float[] { point.X, point.Y });
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static SKPoint to_point(Sk2s x) {
            SKPoint point = new SKPoint();
            float[] a = x.Store();
            point.Set(a[0], a[1]);
            return point;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static Sk2s times_2(Sk2s value) {
            return value + value;
        }

        SKConic() { }
        SKConic(SKPoint p0, SKPoint p1, SKPoint p2, float w) {
            fPts[0] = p0;
            fPts[1] = p1;
            fPts[2] = p2;
            fW = w;
        }
        SKConic(SKPoint[] pts, float w)
        {
            set(pts, w);
        }

        internal SKPoint[] fPts = new SKPoint[3];
        internal float fW;

        void set(SKPoint[] pts, float w)
        {
            if (pts.Length < 3)
            {
                throw new Exception("SKConic point array size must be at least 3");
            }
            fPts[0] = pts[0];
            fPts[1] = pts[1];
            fPts[2] = pts[2];
            fW = w;
        }

        void set(SKPoint p0, SKPoint p1, SKPoint p2, float w)
        {
            fPts[0] = p0;
            fPts[1] = p1;
            fPts[2] = p2;
            fW = w;
        }

        /**
         *  Given a t-value [0...1] return its position and/or tangent.
         *  If pos is not null, return its position at the t-value.
         *  If tangent is not null, return its tangent at the t-value. NOTE the
         *  tangent value's length is arbitrary, and only its direction should
         *  be used.
         */
        void evalAt(float t, ValueHolder<SKPoint> pos, ValueHolder<SKPoint> tangent = null)
        {
            if (!(t >= 0 && t <= 1.0f))
            {
                throw new Exception("t must be between 0.0 and 1.0");
            }

            if (pos != null)
            {
                pos.value = evalAt(t);
            }
            if (tangent != null)
            {
                tangent.value = evalTangentAt(t);
            }

        }

        /** Linearly interpolate between A and B, based on t.
            If t is 0, return A
            If t is 1, return B
            else interpolate.
            t must be [0..1]
        */
        static float SkScalarInterp(float A, float B, float t)
        {
            if (!(t >= 0 && t <= 1.0f))
            {
                throw new Exception("t must be between 0.0 and 1.0");
            }
            return A + (B - A) * t;
        }

        // We only interpolate one dimension at a time (the first, at +0, +3, +6).
        static void p3d_interp(float src0, float src1, float src2, out float dst0, out float dst1, out float dst2, float t) {
            float ab = SkScalarInterp(src0, src1, t);
            float bc = SkScalarInterp(src1, src2, t);
            dst0 = ab;
            dst1 = SkScalarInterp(ab, bc, t);
            dst2 = bc;
        }

        static void ratquad_mapTo3D(SKPoint[] src, float w, SKPoint3[] dst) {
            dst[0].Set(src[0].X * 1, src[0].Y * 1, 1);
            dst[1].Set(src[1].X * w, src[1].Y * w, w);
            dst[2].Set(src[2].X * 1, src[2].Y * 1, 1);
        }

        static SKPoint project_down(SKPoint3 src) {
            return new SKPoint(src.X / src.Z, src.Y / src.Z);
        }

        bool isFinite() {
            float prod = 0;
            prod *= fPts[0].X;
            prod *= fPts[1].X;
            prod *= fPts[2].X;
            prod *= fPts[0].Y;
            prod *= fPts[1].Y;
            prod *= fPts[2].Y;
            // At this point, prod will either be NaN or 0
            return prod == 0;   // if prod is NaN, this check will return false
        }

        bool chopAt(float t, SKConic[] dst)
        {
            if (dst.Length < 2)
            {
                throw new Exception("SKConic chopAt destination array size must be at least 2");
            }

            SKPoint3[] tmp = new SKPoint3[3], tmp2 = new SKPoint3[3];

            ratquad_mapTo3D(fPts, fW, tmp);

            float tmp_0, tmp_1, tmp_2;

            tmp_0 = tmp2[0].X;
            tmp_1 = tmp2[1].X;
            tmp_2 = tmp2[2].X;
            p3d_interp(
                tmp[0].X, tmp[1].X, tmp[2].X,
                out tmp_0, out tmp_1, out tmp_2,
                t);
            tmp2[0].X = tmp_0;
            tmp2[1].X = tmp_1;
            tmp2[2].X = tmp_2;
            tmp_0 = tmp2[0].Y;
            tmp_1 = tmp2[1].Y;
            tmp_2 = tmp2[2].Y;
            p3d_interp(
                tmp[0].Y, tmp[1].Y, tmp[2].Y,
                out tmp_0, out tmp_1, out tmp_2,
                t);
            tmp2[0].Y = tmp_0;
            tmp2[1].Y = tmp_1;
            tmp2[2].Y = tmp_2;
            tmp_0 = tmp2[0].Z;
            tmp_1 = tmp2[1].Z;
            tmp_2 = tmp2[2].Z;
            p3d_interp(
                tmp[0].Z, tmp[1].Z, tmp[2].Z,
                out tmp_0, out tmp_1, out tmp_2,
                t);
            tmp2[0].Z = tmp_0;
            tmp2[1].Z = tmp_1;
            tmp2[2].Z = tmp_2;

            dst[0].fPts[0] = fPts[0];
            dst[0].fPts[1] = project_down(tmp2[0]);
            dst[0].fPts[2] = project_down(tmp2[1]); dst[1].fPts[0] = dst[0].fPts[2];
            dst[1].fPts[1] = project_down(tmp2[2]);
            dst[1].fPts[2] = fPts[2];

            // to put in "standard form", where w0 and w2 are both 1, we compute the
            // new w1 as sqrt(w1*w1/w0*w2)
            // or
            // w1 /= sqrt(w0*w2)
            //
            // However, in our case, we know that for dst[0]:
            //     w0 == 1, and for dst[1], w2 == 1
            //
            float root = MathUtils.sqrtf(tmp2[1].Z);
            dst[0].fW = tmp2[0].Z / root;
            dst[1].fW = tmp2[2].Z / root;
            return dst[0].isFinite() && dst[1].isFinite();
        }

        void chopAt(float t1, float t2, out SKConic dst)
        {
            if (0 == t1 || 1 == t2)
            {
                if (0 == t1 && 1 == t2)
                {
                    dst = this;
                    return;
                }
                else
                {
                    SKConic[] pair = new SKConic[2];
                    if (chopAt(t1.toBool() ? t1 : t2, pair))
                    {
                        dst = pair[t1.toBool().toInt()];
                        return;
                    }
                }
            }
            SKConicCoeff coeff = new(this);
            Sk2s tt1 = new(t1);
            Sk2s aXY = coeff.fNumer.eval(tt1);
            Sk2s aZZ = coeff.fDenom.eval(tt1);
            Sk2s midTT = new((t1 + t2) / 2);
            Sk2s dXY = coeff.fNumer.eval(midTT);
            Sk2s dZZ = coeff.fDenom.eval(midTT);
            Sk2s tt2 = new(t2);
            Sk2s cXY = coeff.fNumer.eval(tt2);
            Sk2s cZZ = coeff.fDenom.eval(tt2);
            Sk2s bXY = times_2(dXY) - (aXY + cXY) * new Sk2s(0.5f);
            Sk2s bZZ = times_2(dZZ) - (aZZ + cZZ) * new Sk2s(0.5f);
            dst = new();
            dst.fPts[0] = to_point(aXY / aZZ);
            dst.fPts[1] = to_point(bXY / bZZ);
            dst.fPts[2] = to_point(cXY / cZZ);
            Sk2s ww = bZZ / (aZZ * cZZ).Sqrt();
            dst.fW = ww[0];
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        static float SkScalarInvert(float x)
        {
            return 1.0f / x;
        }

        static float subdivide_w_value(float w)
        {
            return MathUtils.sqrtf(0.5f + w * 0.5f);
        }

        void chop(out SKConic[] dst)
        {
            Sk2s scale = new(SkScalarInvert(1.0f + fW));
            float newW = subdivide_w_value(fW);

            Sk2s p0 = from_point(fPts[0]);
            Sk2s p1 = from_point(fPts[1]);
            Sk2s p2 = from_point(fPts[2]);
            Sk2s ww = new(fW);

            Sk2s wp1 = ww * p1;
            Sk2s m = (p0 + times_2(wp1) + p2) * scale * new Sk2s(0.5f);
            SKPoint mPt = to_point(m);
            if (!mPt.isFinite())
            {
                double w_d = fW;
                double w_2 = w_d * 2;
                double scale_half = 1 / (1 + w_d) * 0.5;
                mPt.X = (float)((fPts[0].X + w_2 * fPts[1].X + fPts[2].X) * scale_half);
                mPt.Y = (float)((fPts[0].Y + w_2 * fPts[1].Y + fPts[2].Y) * scale_half);
            }
            dst = new SKConic[2];
            dst[0].fPts[0] = fPts[0];
            dst[0].fPts[1] = to_point((p0 + wp1) * scale);
            dst[0].fPts[2] = dst[1].fPts[0] = mPt;
            dst[1].fPts[1] = to_point((wp1 + p2) * scale);
            dst[1].fPts[2] = fPts[2];

            dst[0].fW = dst[1].fW = newW;
        }

        SKPoint evalAt(float t)
        {
            return to_point(new SKConicCoeff(this).eval(t));
        }

        SKPoint evalTangentAt(float t)
        {
            // The derivative equation returns a zero tangent vector when t is 0 or 1,
            // and the control point is equal to the end point.
            // In this case, use the conic endpoints to compute the tangent.
            if ((t == 0 && fPts[0] == fPts[1]) || (t == 1 && fPts[1] == fPts[2]))
            {
                return fPts[2] - fPts[0];
            }
            Sk2s p0 = from_point(fPts[0]);
            Sk2s p1 = from_point(fPts[1]);
            Sk2s p2 = from_point(fPts[2]);
            Sk2s ww = new(fW);

            Sk2s p20 = p2 - p0;
            Sk2s p10 = p1 - p0;

            Sk2s C = ww * p10;
            Sk2s A = ww * p20 - p20;
            Sk2s B = p20 - C - C;

            return to_point(new SKQuadCoeff(A, B, C).eval(t));
        }

        void computeAsQuadError(ref SKPoint err)
        {
            float a = fW - 1;
            float k = a / (4 * (2 + a));
            float x = k * (fPts[0].X - 2 * fPts[1].X + fPts[2].X);
            float y = k * (fPts[0].Y - 2 * fPts[1].Y + fPts[2].Y);
            err.Set(x, y);
        }

        bool asQuadTol(float tol)
        {
            float a = fW - 1;
            float k = a / (4 * (2 + a));
            float x = k * (fPts[0].X - 2 * fPts[1].X + fPts[2].X);
            float y = k * (fPts[0].Y - 2 * fPts[1].Y + fPts[2].Y);
            return (x * x + y * y) <= tol * tol;
        }

        // Limit the number of suggested quads to approximate a conic
        const byte kMaxConicToQuadPOW2 = 5;

        bool SkPointsAreFinite(SKPoint[] fPts, int count)
        {
            float prod = 0;
            for (int i = 0; i < count; ++i)
            {
                prod *= fPts[i].X;
                prod *= fPts[i].Y;
            }
            // At this point, prod will either be NaN or 0
            return prod == 0;   // if prod is NaN, this check will return false
        }

        static int SkScalarCeilToInt(float fpow2)
        {
            float x = MathF.Ceiling(fpow2);

            /**
             *  Return the closest int for the given float. Returns SK_MaxS32FitsInFloat for NaN.
             */
            x = x < 2147483520 ? x : 2147483520;
            x = x > -2147483520 ? x : -2147483520;
            return (int)x;
        }

        /**
         *  return the power-of-2 number of quads needed to approximate this conic
         *  with a sequence of quads. Will be >= 0.
         */
        int computeQuadPOW2(float tol)
        {
            if (tol < 0 || !tol.isFinite() || !SkPointsAreFinite(fPts, 3))
            {
                return 0;
            }

            float a = fW - 1;
            float k = a / (4 * (2 + a));
            float x = k * (fPts[0].X - 2 * fPts[1].X + fPts[2].X);
            float y = k * (fPts[0].Y - 2 * fPts[1].Y + fPts[2].Y);

            float error = MathUtils.sqrtf(x * x + y * y);
            int pow2;
            for (pow2 = 0; pow2 < kMaxConicToQuadPOW2; ++pow2)
            {
                if (error <= tol)
                {
                    break;
                }
                error *= 0.25f;
            }
            // float version -- using ceil gives the same results as the above.
            if (false)
            {
                float err = MathUtils.sqrtf(x * x + y * y);
                if (err <= tol)
                {
                    return 0;
                }
                float tol2 = tol * tol;
                if (tol2 == 0)
                {
                    return kMaxConicToQuadPOW2;
                }
                float fpow2 = MathF.Log2((x * x + y * y) / tol2) * 0.25f;
                int altPow2 = SkScalarCeilToInt(fpow2);
                if (altPow2 != pow2)
                {
                    throw new Exception(string.Format("pow2 %d altPow2 %d fbits %g err %g tol %g\n", pow2, altPow2, fpow2, err, tol);
                }
                pow2 = altPow2;
            }
            return pow2;
        }

        // This was originally developed and tested for pathops: see SkOpTypes.h
        // returns true if (a <= b <= c) || (a >= b >= c)
        static bool between(float a, float b, float c) {
            return (a - b) * (c - b) <= 0;
        }

        static SKPoint[] subdivide(SKConic src, MemoryPointer<SKPoint> pts, int level) {
            if (!(level >= 0)) throw new Exception("subdivide level must not be less than 0");

            if (0 == level) {
                pts.memcpy(0, src.fPts, 1, 2);
                return pts + 2;
            } else {
                SkConic dst[2];
                src.chop(dst);
                const SkScalar startY = src.fPts[0].fY;
                SkScalar endY = src.fPts[2].fY;
                if (between(startY, src.fPts[1].fY, endY)) {
                    // If the input is monotonic and the output is not, the scan converter hangs.
                    // Ensure that the chopped conics maintain their y-order.
                    SkScalar midY = dst[0].fPts[2].fY;
                    if (!between(startY, midY, endY)) {
                        // If the computed midpoint is outside the ends, move it to the closer one.
                        SkScalar closerY = SkTAbs(midY - startY) < SkTAbs(midY - endY) ? startY : endY;
                        dst[0].fPts[2].fY = dst[1].fPts[0].fY = closerY;
                    }
                    if (!between(startY, dst[0].fPts[1].fY, dst[0].fPts[2].fY)) {
                        // If the 1st control is not between the start and end, put it at the start.
                        // This also reduces the quad to a line.
                        dst[0].fPts[1].fY = startY;
                    }
                    if (!between(dst[1].fPts[0].fY, dst[1].fPts[1].fY, endY)) {
                        // If the 2nd control is not between the start and end, put it at the end.
                        // This also reduces the quad to a line.
                        dst[1].fPts[1].fY = endY;
                    }
                    // Verify that all five points are in order.
                    SkASSERT(between(startY, dst[0].fPts[1].fY, dst[0].fPts[2].fY));
                    SkASSERT(between(dst[0].fPts[1].fY, dst[0].fPts[2].fY, dst[1].fPts[1].fY));
                    SkASSERT(between(dst[0].fPts[2].fY, dst[1].fPts[1].fY, endY));
                }
                --level;
                pts = subdivide(dst[0], pts, level);
                return subdivide(dst[1], pts, level);
            }
        }

        /**
         *  Chop this conic into N quads, stored continguously in pts[], where
         *  N = 1 << pow2. The amount of storage needed is (1 + 2 * N)
         */
        int chopIntoQuadsPOW2(SKPoint[] pts, int pow2)
        {
            if (!(pow2 >= 0)) throw new Exception("chopIntoQuadsPOW2 pow2 must be greater than 0");
            pts[0] = fPts[0];
            
            SKPoint endPts;

            if (pow2 == kMaxConicToQuadPOW2)
            {  // If an extreme weight generates many quads ...
                SKConic[] dst;
                chop(out dst);
                // check to see if the first chop generates a pair of lines
                if (EqualsWithinTolerance(dst[0].fPts[1], dst[0].fPts[2]) &&
                        EqualsWithinTolerance(dst[1].fPts[0], dst[1].fPts[1]))
                {
                    pts[1] = pts[2] = pts[3] = dst[0].fPts[1];  // set ctrl == end to make lines
                    pts[4] = dst[1].fPts[2];
                    pow2 = 1;
                    endPts = pts[5];
                    goto commonFinitePtCheck;
                }
            }
            endPts = subdivide(this, pts + 1, pow2);
        commonFinitePtCheck:
            int quadCount = 1 << pow2;
            int ptCount = 2 * quadCount + 1;
            if (!SkPointPriv::AreFinite(pts, ptCount))
            {
                // if we generated a non-finite, pin ourselves to the middle of the hull,
                // as our first and last are already on the first/last pts of the hull.
                for (int i = 1; i < ptCount - 1; ++i)
                {
                    pts[i] = fPts[1];
                }
            }
            return 1 << pow2;
        }

        private bool EqualsWithinTolerance(SKPoint sKPoint1, SKPoint sKPoint2)
        {
            throw new NotImplementedException();
        }

        //    float findMidTangent() const;
        //    bool findXExtrema(float* t) const;
        //    bool findYExtrema(float* t) const;
        //    bool chopAtXExtrema(SKConic dst[2]) const;
        //    bool chopAtYExtrema(SKConic dst[2]) const;

        //    void computeTightBounds(SkRect* bounds) const;
        //    void computeFastBounds(SkRect* bounds) const;

        //    /** Find the parameter value where the conic takes on its maximum curvature.
        //     *
        //     *  @param t   output scalar for max curvature.  Will be unchanged if
        //     *             max curvature outside 0..1 range.
        //     *
        //     *  @return  true if max curvature found inside 0..1 range, false otherwise
        //     */
        //    //    bool findMaxCurvature(float* t) const;  // unimplemented

        //    static float TransformW(const SKPoint[3], float w, const SkMatrix&);

        //    enum {
        //        kMaxConicsForArc = 5
        //    };
        //    static int BuildUnitArc(const SkVector& start, const SkVector& stop, SkRotationDirection,
        //                            const SkMatrix*, SKConic conics[kMaxConicsForArc]);
    }
}