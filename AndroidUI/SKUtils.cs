﻿using AndroidUI.Extensions;
using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI
{
    public static class SKUtils
    {
        /// <summary>
        /// Cast double to float, ignoring any warning about too-large finite values being cast to float.
        /// <br></br>
        /// Clang thinks this is undefined, but it's actually implementation defined to return either
        /// <br></br>
        /// the largest float or infinity (one of the two bracketing representable floats).  Good enough!
        /// <br></br>
        /// this is true for C# as well
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_double_to_float(double v)
        {
            return (float)v;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float SkScalarInvert(float x)
        {
            return 1.0f / x;
        }

        public static float subdivide_w_value(float w)
        {
            return MathUtils.sqrtf(0.5f + w * 0.5f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Sk2s from_point(SKPoint point)
        {
            return Sk2s.Load(new float[] { point.X, point.Y });
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKPoint to_point(Sk2s x)
        {
            SKPoint point = new SKPoint();
            float[] a = x.Store();
            point.Set(a[0], a[1]);
            return point;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static Sk2s times_2(Sk2s value)
        {
            return value + value;
        }

        public static bool SkPointsAreFinite(MemoryPointer<SKPoint> fPts, int count)
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

        public const short SK_MaxS16 = short.MaxValue;
        public const short SK_MinS16 = -SK_MaxS16;

        public const int SK_MaxS32 = int.MaxValue;
        public const int SK_MinS32 = -SK_MaxS32;
        public const int SK_NaN32 = int.MinValue;

        public const long SK_MaxS64 = long.MaxValue;
        public const long SK_MinS64 = -SK_MaxS64;

        public const int SK_MaxS32FitsInFloat = 2147483520;
        public const int SK_MinS32FitsInFloat = -SK_MaxS32FitsInFloat;

        public const long SK_MaxS64FitsInFloat = (SK_MaxS64 >> (63 - 24) << (63 - 24));   // 0x7fffff8000000000
        public const long SK_MinS64FitsInFloat = -SK_MaxS64FitsInFloat;

        public const float SK_FloatSqrt2 = 1.41421356f;
        public const float SK_FloatPI = 3.14159265f;
        public const double SK_DoublePI = 3.14159265358979323846264338327950288;

        /// <summary>
        /// Return the closest int for the given float. Returns SK_MaxS32FitsInFloat for NaN.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_saturate2int(float x)
        {
            x = x < SK_MaxS32FitsInFloat ? x : SK_MaxS32FitsInFloat;
            x = x > SK_MinS32FitsInFloat ? x : SK_MinS32FitsInFloat;
            return (int)x;
        }

        /// <summary>
        /// Return the closest int for the given double. Returns SK_MaxS32 for NaN.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_double_saturate2int(float x)
        {
            x = x < SK_MaxS32 ? x : SK_MaxS32;
            x = x > SK_MinS32 ? x : SK_MinS32;
            return (int)x;
        }

        /**
         *  Return the closest int64_t for the given float. Returns SK_MaxS64FitsInFloat for NaN.
         */
        public static long sk_float_saturate2int64(float x)
        {
            x = x < SK_MaxS64FitsInFloat ? x : SK_MaxS64FitsInFloat;
            x = x > SK_MinS64FitsInFloat ? x : SK_MinS64FitsInFloat;
            return (long)x;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_sqrt(float x) => MathF.Sqrt(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_sin(float x) => MathF.Sin(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_cos(float x) => MathF.Cos(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_tan(float x) => MathF.Tan(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_floor(float x) => MathF.Floor(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_ceil(float x) => MathF.Ceiling(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_trunc(float x) => MathF.Truncate(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_acos(float x) => MathF.Acos(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_asin(float x) => MathF.Asin(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_atan2(float x, float y) => MathF.Atan2(x, y);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_abs(float x) => MathF.Abs(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_copysign(float x, float y) => MathF.CopySign(x, y);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_mod(float number, float denom) => MathUtils.fmod(number, denom);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_exp(float x) => MathF.Exp(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_log(float x) => MathF.Log(x);

        public static float sk_float_degrees_to_radians(float degrees)
        {
            return degrees * (SK_FloatPI / 180);
        }

        public static float sk_float_radians_to_degrees(float radians)
        {
            return radians * (180 / SK_FloatPI);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_round(float x) => sk_float_floor((x) + 0.5f);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_log2(float x) => MathF.Log2(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_float_isfinite(float x)
        {
            return SkFloatBits_IsFinite(SkFloat2Bits(x));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_floats_are_finite(float a, float b)
        {
            return sk_float_isfinite(a) && sk_float_isfinite(b);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_floats_are_finite(float[] array, int count) {
            float prod = 0;
            for (int i = 0; i<count; ++i) {
                prod *= array[i];
            }
            // At this point, prod will either be NaN or 0
            return prod == 0;   // if prod is NaN, this check will return false
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_float_isinf(float x)
        {
            return SkFloatBits_IsInf(SkFloat2Bits(x));
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_float_isnan(float x)
        {
            return !(x == x);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool sk_double_isnan(double x) => sk_float_isnan(sk_double_to_float(x));

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int SkFloat2Bits(float x) => x.ToRawIntBits();

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float SkBits2Float(int x) => x.BitsToFloat();

        const int gFloatBits_exponent_mask = 0x7F800000;
        const int gFloatBits_matissa_mask  = 0x007FFFFF;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool SkFloatBits_IsFinite(int bits)
        {
            return (bits & gFloatBits_exponent_mask) != gFloatBits_exponent_mask;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static bool SkFloatBits_IsInf(int bits)
        {
            return ((bits & gFloatBits_exponent_mask) == gFloatBits_exponent_mask) &&
                    (bits & gFloatBits_matissa_mask) == 0;
        }

        /// <summary>Convert a sign-bit int (i.e. float interpreted as int) into a 2s compliement
        /// int. This also converts -0 (0x80000000) to 0. Doing this to a float allows
        /// it to be compared using normal C operators (<, <=, etc.)</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int SkSignBitTo2sCompliment(int x)
        {
            if (x < 0)
            {
                x &= 0x7FFFFFFF;
                x = -x;
            }
            return x;
        }

        /// <summary>Convert a 2s compliment int to a sign-bit (i.e. int interpreted as float).
        /// This undoes the result of SkSignBitTo2sCompliment().</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int Sk2sComplimentToSignBit(int x)
        {
            int sign = x >> 31;
            // make x positive
            x = (x ^ sign) - sign;
            // set the sign bit as needed
            x |= sign.UnsignedLeftShift(31);
            return x;
        }

        /// <summary>
        /// Return the float as a 2s compliment int. Just to be used to compare floats
        /// to each other or against positive float-bit-constants (like 0). This does
        /// not return the int equivalent of the float, just something cheaper for
        /// compares-only.
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int SkFloatAs2sCompliment(float x)
        {
            return SkSignBitTo2sCompliment(SkFloat2Bits(x));
        }

        /// <summary>Return the 2s compliment int as a float. This undos the result of
        /// SkFloatAs2sCompliment</summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float Sk2sComplimentAsFloat(int x)
        {
            return SkBits2Float(Sk2sComplimentToSignBit(x));
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_floor2int(float x) => sk_float_saturate2int(sk_float_floor(x));

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_round2int(float x) => sk_float_saturate2int(sk_float_floor((x) + 0.5f));
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_ceil2int(float x) => sk_float_saturate2int(sk_float_ceil(x));

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_floor2int_no_saturate(float x) => (int)sk_float_floor(x);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_round2int_no_saturate(float x) => (int)sk_float_floor((x) + 0.5f);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_ceil2int_no_saturate(float x) => (int)sk_float_ceil(x);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double sk_double_floor(double x) => Math.Floor(x);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double sk_double_round(double x) => Math.Floor((x) + 0.5);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double sk_double_ceil(double x) => Math.Ceiling(x);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_double_floor2int(double x) => (int)Math.Floor(x);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_double_round2int(double x) => (int)Math.Floor((x) + 0.5);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_double_ceil2int(double x) => (int)Math.Ceiling(x);



        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_rsqrt_portable(float x) => MathF.ReciprocalSqrtEstimate(x);
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_float_rsqrt(float x) => MathF.ReciprocalSqrtEstimate(x);

        // Returns the log2 of the provided value, were that value to be rounded up to the next power of 2.
        // Returns 0 if value <= 0:
        // Never returns a negative number, even if value is NaN.
        //
        //     sk_float_nextlog2((-inf..1]) -> 0
        //     sk_float_nextlog2((1..2]) -> 1
        //     sk_float_nextlog2((2..4]) -> 2
        //     sk_float_nextlog2((4..8]) -> 3
        //     ...
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int sk_float_nextlog2(float x)
        {
            uint bits = (uint)SkFloat2Bits(x);
            bits += (1u << 23) - 1u;  // Increment the exponent for non-powers-of-2.
            int exp = ((int)bits >> 23) - 127;
            return exp & ~(exp >> 31);  // Return 0 for negative or denormalized floats, and exponents < 0.
        }

        // IEEE defines how float divide behaves for non-finite values and zero-denoms, but C does not
        // so we have a helper that suppresses the possible undefined-behavior warnings.

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_ieee_float_divide(float numer, float denom) {
            return numer / denom;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double sk_ieee_double_divide(double numer, double denom) {
            return numer / denom;
        }

        // While we clean up divide by zero, we'll replace places that do divide by zero with this TODO.
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_ieee_float_divide_TODO_IS_DIVIDE_BY_ZERO_SAFE_HERE(float n, float d)
        {
            return sk_ieee_float_divide(n, d);
        }
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double sk_ieee_double_divide_TODO_IS_DIVIDE_BY_ZERO_SAFE_HERE(double n, double d)
        {
            return sk_ieee_double_divide(n, d);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float sk_fmaf(float f, float m, float a) => MathF.FusedMultiplyAdd(f, m, a);
    }
}