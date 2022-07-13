using AndroidUI.Extensions;
using SkiaSharp;
using static AndroidUI.Native;
using static AndroidUI.SKUtils;

namespace AndroidUI
{
    class SKConic
    {
        public SKConic() { }
        public SKConic(SKPoint p0, SKPoint p1, SKPoint p2, float w) {
            fPts[0] = p0;
            fPts[1] = p1;
            fPts[2] = p2;
            fW = w;
        }
        public SKConic(SKPoint[] pts, float w)
        {
            set(pts, w);
        }

        internal SKPoint[] fPts = new SKPoint[3];
        internal float fW;

        public void set(MemoryPointer<SKPoint> pts, float w)
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

        public void set(SKPoint p0, SKPoint p1, SKPoint p2, float w)
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
        public void evalAt(float t, MemoryPointer<SKPoint> pos, MemoryPointer<SKPoint> tangent = null)
        {
            if (!(t >= 0 && t <= 1.0f))
            {
                throw new Exception("t must be between 0.0 and 1.0");
            }

            if (pos != null)
            {
                pos[0] = evalAt(t);
            }
            if (tangent != null)
            {
                pos[0] = evalTangentAt(t);
            }

        }

        /** Linearly interpolate between A and B, based on t.
            If t is 0, return A
            If t is 1, return B
            else interpolate.
            t must be [0..1]
        */
        public static float SkScalarInterp(float A, float B, float t)
        {
            if (!(t >= 0 && t <= 1.0f))
            {
                throw new Exception("t must be between 0.0 and 1.0");
            }
            return A + (B - A) * t;
        }

        // We only interpolate one dimension at a time (the first, at +0, +3, +6).
        public static void p3d_interp(MemoryPointer<float> src, MemoryPointer<float> dst, float t) {
            float ab = SkScalarInterp(src[0], src[3], t);
            float bc = SkScalarInterp(src[3], src[6], t);
            dst[0] = ab;
            dst[3] = SkScalarInterp(ab, bc, t);
            dst[6] = bc;
        }

        public static void ratquad_mapTo3D(SKPoint[] src, float w, SKPoint3[] dst) {
            dst[0].Set(src[0].X * 1, src[0].Y * 1, 1);
            dst[1].Set(src[1].X * w, src[1].Y * w, w);
            dst[2].Set(src[2].X * 1, src[2].Y * 1, 1);
        }

        public static SKPoint project_down(SKPoint3 src) {
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

        public bool chopAt(float t, out SKConic[] dst)
        {
            SKPoint3[] tmp = new SKPoint3[3];
            ratquad_mapTo3D(fPts, fW, tmp);

            MemoryPointer<float> tmp1 = createSKPoint3Mapper(tmp);
            MemoryPointer<float> tmp2 = createSKPoint3Mapper(new SKPoint3[3]);

            p3d_interp(tmp1, tmp2, t);

            tmp1++;
            tmp2++;
            p3d_interp(tmp1, tmp2, t);

            tmp1++;
            tmp2++;
            p3d_interp(tmp1, tmp2, t);

            tmp1.ResetPointerOffset();
            tmp2.ResetPointerOffset();

            dst = new SKConic[2];
            for (int i = 0; i < 2; i++)
            {
                dst[i] = new();
            }
            dst[0].fPts[0] = fPts[0];
            dst[0].fPts[1] = project_down(tmp2.ToSKPoint3());
            dst[0].fPts[2] = project_down((tmp2 + 3).ToSKPoint3()); dst[1].fPts[0] = dst[0].fPts[2];
            dst[1].fPts[1] = project_down((tmp2 + 6).ToSKPoint3());
            dst[1].fPts[2] = fPts[2];

            // to put in "standard form", where w0 and w2 are both 1, we compute the
            // new w1 as sqrt(w1*w1/w0*w2)
            // or
            // w1 /= sqrt(w0*w2)
            //
            // However, in our case, we know that for dst[0]:
            //     w0 == 1, and for dst[1], w2 == 1
            //
            float root = MathUtils.sqrtf(tmp2[5]);
            dst[0].fW = tmp2[2] / root;
            dst[1].fW = tmp2[8] / root;
            return dst[0].isFinite() && dst[1].isFinite();
        }

        public void chopAt(float t1, float t2, out SKConic dst)
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
                    SKConic[] pair;
                    if (chopAt(t1.toBool() ? t1 : t2, out pair))
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

        public void chop(out SKConic[] dst)
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
            for (int i = 0; i < 2; i++)
            {
                dst[i] = new();
            }
            dst[0].fPts[0] = fPts[0];
            dst[0].fPts[1] = to_point((p0 + wp1) * scale);
            dst[0].fPts[2] = dst[1].fPts[0] = mPt;
            dst[1].fPts[1] = to_point((wp1 + p2) * scale);
            dst[1].fPts[2] = fPts[2];

            dst[0].fW = dst[1].fW = newW;
        }

        public SKPoint evalAt(float t)
        {
            return to_point(new SKConicCoeff(this).eval(t));
        }

        public SKPoint evalTangentAt(float t)
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

        public bool asQuadTol(float tol)
        {
            float a = fW - 1;
            float k = a / (4 * (2 + a));
            float x = k * (fPts[0].X - 2 * fPts[1].X + fPts[2].X);
            float y = k * (fPts[0].Y - 2 * fPts[1].Y + fPts[2].Y);
            return (x * x + y * y) <= tol * tol;
        }

        // Limit the number of suggested quads to approximate a conic
        const int kMaxConicToQuadPOW2 = 5;

        /**
         *  return the power-of-2 number of quads needed to approximate this conic
         *  with a sequence of quads. Will be >= 0.
         */
        public int computeQuadPOW2(float tol)
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
                int altPow2 = sk_float_ceil2int(fpow2);
                if (altPow2 != pow2)
                {
                    throw new Exception(string.Format("pow2 %d altPow2 %d fbits %g err %g tol %g\n", pow2, altPow2, fpow2, err, tol));
                }
                pow2 = altPow2;
            }
            return pow2;
        }

        // This was originally developed and tested for pathops: see SkOpTypes.h
        // returns true if (a <= b <= c) || (a >= b >= c)
        public static bool between(float a, float b, float c) {
            return (a - b) * (c - b) <= 0;
        }

        public static SKPoint[] subdivide(SKConic src, MemoryPointer<SKPoint> pts, int level)
        {
            if (!(level >= 0)) throw new Exception("subdivide level must not be less than 0");

            if (0 == level)
            {
                MemoryPointer<SKPoint> tmp = src.fPts;
                pts.Copy(tmp + 1, 2);
                return (pts + 2).ToArray();
            }
            else
            {
                SKConic[] dst;
                src.chop(out dst);
                float startY = src.fPts[0].Y;
                float endY = src.fPts[2].Y;
                if (between(startY, src.fPts[1].Y, endY))
                {
                    // If the input is monotonic and the output is not, the scan converter hangs.
                    // Ensure that the chopped conics maintain their y-order.
                    float midY = dst[0].fPts[2].Y;
                    if (!between(startY, midY, endY))
                    {
                        // If the computed midpoint is outside the ends, move it to the closer one.
                        float closerY = MathF.Abs(midY - startY) < MathF.Abs(midY - endY) ? startY : endY;
                        dst[0].fPts[2].Y = dst[1].fPts[0].Y = closerY;
                    }
                    if (!between(startY, dst[0].fPts[1].Y, dst[0].fPts[2].Y))
                    {
                        // If the 1st control is not between the start and end, put it at the start.
                        // This also reduces the quad to a line.
                        dst[0].fPts[1].Y = startY;
                    }
                    if (!between(dst[1].fPts[0].Y, dst[1].fPts[1].Y, endY))
                    {
                        // If the 2nd control is not between the start and end, put it at the end.
                        // This also reduces the quad to a line.
                        dst[1].fPts[1].Y = endY;
                    }
                    // Verify that all five points are in order.
                    if (!between(startY, dst[0].fPts[1].Y, dst[0].fPts[2].Y))
                    {
                        throw new ArithmeticException("between(startY, dst[0].fPts[1].Y, dst[0].fPts[2].Y)");
                    }
                    if (!between(dst[0].fPts[1].Y, dst[0].fPts[2].Y, dst[1].fPts[1].Y))
                    {
                        throw new ArithmeticException("between(dst[0].fPts[1].Y, dst[0].fPts[2].Y, dst[1].fPts[1].Y)");
                    }
                    if (!between(dst[0].fPts[2].Y, dst[1].fPts[1].Y, endY))
                    {
                        throw new ArithmeticException("between(dst[0].fPts[2].Y, dst[1].fPts[1].Y, endY)");
                    }
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
        public int chopIntoQuadsPOW2(MemoryPointer<SKPoint> pts, int pow2)
        {
            if (!(pow2 >= 0)) throw new Exception("chopIntoQuadsPOW2 pow2 must be greater than 0");
            pts[0] = fPts[0];

            SKPoint[] endPts;

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
                    endPts = (pts + 5).ToArray();
                    goto commonFinitePtCheck;
                }
            }
            endPts = subdivide(this, pts + 1, pow2);
        commonFinitePtCheck:
            int quadCount = 1 << pow2;
            int ptCount = 2 * quadCount + 1;
            if (!SkPointsAreFinite(pts, ptCount))
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

        static bool CanNormalize(float dx, float dy)
        {
            return sk_floats_are_finite(dx, dy) && (dx != 0 || dy != 0);
        }

        static bool EqualsWithinTolerance(SKPoint p1, SKPoint p2)
        {
            return !CanNormalize(p1.X - p2.X, p1.Y - p2.Y);
        }

        public float findMidTangent()
        {
            // Tangents point in the direction of increasing T, so tan0 and -tan1 both point toward the
            // midtangent. The bisector of tan0 and -tan1 is orthogonal to the midtangent:
            //
            //     bisector dot midtangent = 0
            //
            SKPoint tan0 = fPts[1] - fPts[0];
            SKPoint tan1 = fPts[2] - fPts[1];
            SKPoint bisector = SkFindBisector(tan0, tan1.Negate());

            // Start by finding the tangent function's power basis coefficients. These define a tangent
            // direction (scaled by some uniform value) as:
            //                                                |T^2|
            //     Tangent_Direction(T) = dx,dy = |A  B  C| * |T  |
            //                                    |.  .  .|   |1  |
            //
            // The derivative of a conic has a cumbersome order-4 denominator. However, this isn't necessary
            // if we are only interested in a vector in the same *direction* as a given tangent line. Since
            // the denominator scales dx and dy uniformly, we can throw it out completely after evaluating
            // the derivative with the standard quotient rule. This leaves us with a simpler quadratic
            // function that we use to find a tangent.
            SKPoint A = (fPts[2] - fPts[0]).Multiply(fW - 1);
            SKPoint B = (fPts[2] - fPts[0] - (fPts[1] - fPts[0])).Multiply(fW * 2);
            SKPoint C = (fPts[1] - fPts[0]).Multiply(fW);

            // Now solve for "bisector dot midtangent = 0":
            //
            //                            |T^2|
            //     bisector * |A  B  C| * |T  | = 0
            //                |.  .  .|   |1  |
            //
            float a = bisector.Dot(A);
            float b = bisector.Dot(B);
            float c = bisector.Dot(C);
            return solve_quadratic_equation_for_midtangent(a, b, c);
        }

        // Finds the root nearest 0.5. Returns 0.5 if the roots are undefined or outside 0..1.
        public static float solve_quadratic_equation_for_midtangent(float a, float b, float c, float discr)
        {
            // Quadratic formula from Numerical Recipes in C:
            float q = -.5f * (b + MathF.CopySign(MathF.Sqrt(discr), b));
            // The roots are q/a and c/q. Pick the midtangent closer to T=.5.
            float _5qa = -.5f * q * a;
            float T = MathF.Abs(q * q + _5qa) < MathF.Abs(a * c + _5qa) ? sk_ieee_float_divide(q, a)
                                                            : sk_ieee_float_divide(c, q);
            if (!(T > 0 && T < 1))
            {  // Use "!(positive_logic)" so T=NaN will take this branch.
               // Either the curve is a flat line with no rotation or FP precision failed us. Chop at .5.
                T = .5f;
            }
            return T;
        }

        public static float solve_quadratic_equation_for_midtangent(float a, float b, float c)
        {
            return solve_quadratic_equation_for_midtangent(a, b, c, b * b - 4 * a * c);
        }

        public SKPoint SkFindBisector(SKPoint a, SKPoint b)
        {
            float[] v = new float[4];
            if (a.Dot(b) >= 0)
            {
                // a,b are within +/-90 degrees apart.
                v[0] = a.X;
                v[1] = a.Y;
                v[2] = b.X;
                v[3] = b.Y;
            }
            else if (a.Cross(b) >= 0)
            {
                // a,b are >90 degrees apart. Find the bisector of their interior normals instead. (Above 90
                // degrees, the original vectors start cancelling each other out which eventually becomes
                // unstable.)
                v[0] = -a.Y;
                v[1] = +a.X;
                v[2] = +b.Y;
                v[3] = -b.X;
            }
            else
            {
                // a,b are <-90 degrees apart. Find the bisector of their interior normals instead. (Below
                // -90 degrees, the original vectors start cancelling each other out which eventually
                // becomes unstable.)
                v[0] = +a.Y;
                v[1] = -a.X;
                v[2] = -b.Y;
                v[3] = +b.X;
            }
            // Return "normalize(v[0]) + normalize(v[1])".
            Sk2f x0_x1, y0_y1;
            Sk2f.Load2(v, out x0_x1, out y0_y1);
            Sk2f invLengths = 1.0f / (x0_x1 * x0_x1 + y0_y1 * y0_y1).Sqrt();
            x0_x1 *= invLengths;
            y0_y1 *= invLengths;
            return new SKPoint(x0_x1[0] + x0_x1[1], y0_y1[0] + y0_y1[1]);
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // NURB representation for conics.  Helpful explanations at:
        //
        // http://citeseerx.ist.psu.edu/viewdoc/
        //   download?doi=10.1.1.44.5740&rep=rep1&type=ps
        // and
        // http://www.cs.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/NURBS/RB-conics.html
        //
        // F = (A (1 - t)^2 + C t^2 + 2 B (1 - t) t w)
        //     ------------------------------------------
        //         ((1 - t)^2 + t^2 + 2 (1 - t) t w)
        //
        //   = {t^2 (P0 + P2 - 2 P1 w), t (-2 P0 + 2 P1 w), P0}
        //     ------------------------------------------------
        //             {t^2 (2 - 2 w), t (-2 + 2 w), 1}
        //
        //
        // F' = 2 (C t (1 + t (-1 + w)) - A (-1 + t) (t (-1 + w) - w) + B (1 - 2 t) w)
        //
        //  t^2 : (2 P0 - 2 P2 - 2 P0 w + 2 P2 w)
        //  t^1 : (-2 P0 + 2 P2 + 4 P0 w - 4 P1 w)
        //  t^0 : -2 P0 w + 2 P1 w
        //
        //  We disregard magnitude, so we can freely ignore the denominator of F', and
        //  divide the numerator by 2
        //
        //    coeff[0] for t^2
        //    coeff[1] for t^1
        //    coeff[2] for t^0
        //
        public static void conic_deriv_coeff(MemoryPointer<float> src, float w, float[] coeff)
        {
            float P20 = src[4] - src[0];
            float P10 = src[2] - src[0];
            float wP10 = w * P10;
            coeff[0] = w * P20 - P20;
            coeff[1] = P20 - 2 * wP10;
            coeff[2] = wP10;
        }

        public static bool conic_find_extrema(MemoryPointer<float> src, float w, ref float t) {
            float[] coeff = new float[3];

            conic_deriv_coeff(src, w, coeff);

            float[] tValues = new float[2];
            int roots = SkFindUnitQuadRoots(coeff[0], coeff[1], coeff[2], tValues);
            if (!(0 == roots || 1 == roots))
            {
                throw new Exception("(0 == roots || 1 == roots) failed");
            };

            if (1 == roots)
            {
                t = tValues[0];
                return true;
            }
            return false;
        }

        public static int valid_unit_divide(float numerator, float denominator, MemoryPointer<float> ratio)
        {
            if (numerator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            if (denominator == 0 || numerator == 0 || numerator >= denominator)
            {
                return 0;
            }

            float r = numerator / denominator;
            if (sk_float_isnan(r))
            {
                return 0;
            }
            if (!(r >= 0 && r < SK_Scalar1)) {
                throw new Exception("numer " + numerator + ", denom " + denominator + ", r " + r);
            };
            if (r == 0)
            { // catch underflow if numer <<<< denom
                return 0;
            }
            ratio[0] = r;
            return 1;
        }

        // Just returns its argument, but makes it easy to set a break-point to know when
        // SkFindUnitQuadRoots is going to return 0 (an error).
        public static int return_check_zero(int value)
        {
            if (value == 0)
            {
                return 0;
            }
            return value;
        }

        /** From Numerical Recipes in C.

            Q = -1/2 (B + sign(B) sqrt[B*B - 4*A*C])
            x1 = Q / A
            x2 = C / Q
        */
        public static int SkFindUnitQuadRoots(float A, float B, float C, MemoryPointer<float> roots)
        {
            ArgumentNullException.ThrowIfNull(roots, nameof(roots));

            if (A == 0)
            {
                return return_check_zero(valid_unit_divide(-C, B, roots));
            }

            MemoryPointer<float> r = roots;

            // use doubles so we don't overflow temporarily trying to compute R
            double dr = (double)B * B - 4 * (double)A * C;
            if (dr < 0)
            {
                return return_check_zero(0);
            }
            dr = Math.Sqrt(dr);
            float R = sk_double_to_float(dr);
            if (!sk_float_isfinite(R))
            {
                return return_check_zero(0);
            }

            float Q = (B < 0) ? -(B - R) / 2 : -(B + R) / 2;
            r += valid_unit_divide(Q, A, r);
            r += valid_unit_divide(C, Q, r);
            if (r - roots == 2)
            {
                if (roots[0] > roots[1])
                {
                    roots.Swap(0, 1);
                }
                else if (roots[0] == roots[1])
                {
                    // nearly-equal?
                    r -= 1; // skip the double root
                }
            }
            return return_check_zero((int)(r - roots));
        }

        public bool findXExtrema(out float t) {
            t = 0;
            return conic_find_extrema(createSKPointMapper(fPts), fW, ref t);

        }
        public bool findYExtrema(out float t)
        {
            t = 0;
            return conic_find_extrema(new MemoryPointer<float>(createSKPointMapper(fPts)) + 1, fW, ref t);
        }

        public bool chopAtXExtrema(out SKConic[] dst) {
            float t;
            if (findXExtrema(out t)) {
                if (!chopAt(t, out dst))
                {
                    // if chop can't return finite values, don't chop
                    return false;
                }
                // now clean-up the middle, since we know t was meant to be at
                // an X-extrema
                float value = dst[0].fPts[2].X;
                dst[0].fPts[1].X = value;
                dst[1].fPts[0].X = value;
                dst[1].fPts[1].X = value;
                return true;
            }
            dst = null;
            return false;
        }

        public bool chopAtYExtrema(out SKConic[] dst) {
            float t;
            if (findYExtrema(out t)) {
                if (!chopAt(t, out dst))
                {
                    // if chop can't return finite values, don't chop
                    return false;
                }
                // now clean-up the middle, since we know t was meant to be at
                // an Y-extrema
                float value = dst[0].fPts[2].Y;
                dst[0].fPts[1].Y = value;
                dst[1].fPts[0].Y = value;
                dst[1].fPts[1].Y = value;
                return true;
            }
            dst = null;
            return false;
        }

        public void computeTightBounds(ref SKRect bounds) {
            MemoryPointer<SKPoint> pts = new SKPoint[4];
            pts[0] = fPts[0];
            pts[1] = fPts[2];
            int count = 2;

            float t;
            if (findXExtrema(out t)) {
                evalAt(t, pts + count);
                count++;
            }
            if (findYExtrema(out t)) {
                evalAt(t, pts + count);
                count++;
            }
            bounds.setBounds((SKPoint[])pts.GetArray(), count);
        }

        public void computeFastBounds(ref SKRect bounds) {
            bounds.setBounds(fPts, 3);
        }

        /** Find the parameter value where the conic takes on its maximum curvature.
         *
         *  @param t   output scalar for max curvature.  Will be unchanged if
         *             max curvature outside 0..1 range.
         *
         *  @return  true if max curvature found inside 0..1 range, false otherwise
         */
        //    bool findMaxCurvature(float* t) const;  // unimplemented

        public static float TransformW(SKPoint[] pts, float w, ref SKMatrix matrix) {
            if (!matrix.HasPerspective)
            {
                return w;
            }

            SKPoint3[] src = new SKPoint3[3], dst = new SKPoint3[3];

            ratquad_mapTo3D(pts, w, src);

            matrix.MapHomogeneousPoints(dst, src, 3);

            // w' = sqrt(w1*w1/w0*w2)
            // use doubles temporarily, to handle small numer/denom
            double w0 = dst[0].Z;
            double w1 = dst[1].Z;
            double w2 = dst[2].Z;
            return sk_double_to_float(Math.Sqrt(sk_ieee_double_divide(w1 * w1, w0 * w2)));
        }

        public const int kMaxConicsForArc = 5;
        public static int BuildUnitArc(ref SKPoint uStart, ref SKPoint uStop,
            SKPathDirection dir, ref SKMatrix userMatrix, out SKConic[] dst) {
            dst = new SKConic[kMaxConicsForArc];
            for (int i = 0; i < kMaxConicsForArc; i++)
            {
                dst[i] = new();
            }
            // rotate by x,y so that uStart is (1.0)
            float x = uStart.DotProduct(uStop);
            float y = uStart.CrossProduct(uStop);

            float absY = sk_float_abs(y);

            // check for (effectively) coincident vectors
            // this can happen if our angle is nearly 0 or nearly 180 (y == 0)
            // ... we use the dot-prod to distinguish between 0 and 180 (x > 0)
            if (absY <= SK_ScalarNearlyZero && x > 0 && ((y >= 0 && SKPathDirection.Clockwise == dir) ||
                                                         (y <= 0 && SKPathDirection.CounterClockwise == dir))) {
                return 0;
            }

            if (dir == SKPathDirection.CounterClockwise) {
                y = -y;
            }

            // We decide to use 1-conic per quadrant of a circle. What quadrant does [xy] lie in?
            //      0 == [0  .. 90)
            //      1 == [90 ..180)
            //      2 == [180..270)
            //      3 == [270..360)
            //
            int quadrant = 0;
            if (0 == y) {
                quadrant = 2;        // 180
                if (!(sk_float_abs(x + SK_Scalar1) <= SK_ScalarNearlyZero))
                {
                    throw new Exception("(sk_float_abs(x + SK_Scalar1) <= SK_ScalarNearlyZero) failed");
                }
            } else if (0 == x) {
                if (!(absY - SK_Scalar1 <= SK_ScalarNearlyZero))
                {
                    throw new Exception("(absY - SK_Scalar1 <= SK_ScalarNearlyZero) failed");
                }
                quadrant = y > 0 ? 1 : 3; // 90 : 270
            } else {
                if (y < 0) {
                    quadrant += 2;
                }
                if ((x < 0) != (y < 0)) {
                    quadrant += 1;
                }
            }

            MemoryPointer<SKPoint> quadrantPts = new SKPoint[] {
                new (1, 0), new (1, 1), new (0, 1), new (-1, 1),
                new (-1, 0), new (-1, -1), new (0, -1), new (1, -1)
            };
            const float quadrantWeight = SK_ScalarRoot2Over2;

            int conicCount = quadrant;
            for (int i = 0; i < conicCount; ++i) {
                dst[i].set(quadrantPts + (i * 2), quadrantWeight);
            }

            // Now compute any remaing (sub-90-degree) arc for the last conic
            SKPoint finalP = new(x, y);
            SKPoint lastQ = quadrantPts[quadrant * 2];  // will already be a unit-vector
            float dot = lastQ.DotProduct(finalP);
            if (!(0 <= dot && dot <= SK_Scalar1 + SK_ScalarNearlyZero))
            {
                throw new Exception("(0 <= dot && dot <= SK_Scalar1 + SK_ScalarNearlyZero) failed");
            }
            
            if (dot < 1) {
                SKPoint offCurve = new(lastQ.X + x, lastQ.Y + y);
                // compute the bisector vector, and then rescale to be the off-curve point.
                // we compute its length from cos(theta/2) = length / 1, using half-angle identity we get
                // length = sqrt(2 / (1 + cos(theta)). We already have cos() when to computed the dot.
                // This is nice, since our computed weight is cos(theta/2) as well!
                //
                float cosThetaOver2 = sk_float_sqrt((1 + dot) / 2);
                offCurve.setLength(SkScalarInvert(cosThetaOver2));
                if (!EqualsWithinTolerance(lastQ, offCurve)) {
                    dst[conicCount].set(lastQ, offCurve, finalP, cosThetaOver2);
                    conicCount += 1;
                }
            }

            // now handle counter-clockwise and the initial unitStart rotation
            SKMatrix matrix = SKMatrix.CreateSinCos(uStart.Y, uStart.X);
            if (dir == SKPathDirection.Clockwise) {
                matrix.PreScale(SK_Scalar1, -SK_Scalar1);
            }
            if (userMatrix != null) {
                matrix.PostConcat(userMatrix);
            }
            for (int i = 0; i < conicCount; ++i) {
                matrix.MapPoints(dst[i].fPts);
            }
            return conicCount;
        }
    }
}