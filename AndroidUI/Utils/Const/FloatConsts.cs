namespace AndroidUI.Utils.Const
{
    public static class FloatConsts
    {
        /**
         * A constant holding the positive infinity of type
         * {@code float}. It is equal to the value returned by
         * {@code Float.intBitsToFloat(0x7f800000)}.
         */
        public const float POSITIVE_INFINITY = float.PositiveInfinity;

        /**
         * A constant holding the negative infinity of type
         * {@code float}. It is equal to the value returned by
         * {@code Float.intBitsToFloat(0xff800000)}.
         */
        public const float NEGATIVE_INFINITY = float.NegativeInfinity;

        /**
         * A constant holding a Not-a-Number (NaN) value of type
         * {@code float}.  It is equivalent to the value returned by
         * {@code Float.intBitsToFloat(0x7fc00000)}.
         */
        public const float NAN = float.NaN;

        /**
         * A constant holding the largest positive finite value of type
         * {@code float}, (2-2<sup>-23</sup>)&middot;2<sup>127</sup>.
         * It is equal to the hexadecimal floating-point literal
         * {@code 0x1.fffffeP+127f} and also equal to
         * {@code Float.intBitsToFloat(0x7f7fffff)}.
         */
        public const float MAX_VALUE = float.MaxValue;

        /**
         * A constant holding the smallest positive nonzero value of type
         * {@code float}, 2<sup>-149</sup>. It is equal to the
         * hexadecimal floating-point literal {@code 0x0.000002P-126f}
         * and also equal to {@code Float.intBitsToFloat(0x1)}.
         */
        public const float MIN_VALUE = float.MinValue;

        /**
         * A constant holding the smallest positive normal value of type
         * {@code float}, 2<sup>-126</sup>.  It is equal to the
         * hexadecimal floating-point literal {@code 0x1.0p-126f} and also
         * equal to {@code Float.intBitsToFloat(0x00800000)}.
         *
         * @since 1.6
         */
        public const float MIN_NORMAL = 1.17549435E-38f;

        /**
         * The number of bits used to represent a {@code float} value.
         *
         * @since 1.5
         */
        public const int SIZE = 32;

        /**
         * The number of bytes used to represent a {@code float} value.
         *
         * @since 1.8
         */
        public const int BYTES = SIZE / 8;

        /**
         * The number of logical bits in the significand of a
         * {@code float} number, including the implicit bit.
         */
        public const int SIGNIFICAND_WIDTH = 24;

        /**
         * Maximum exponent a finite <code>float</code> number may have.
         * It is equal to the value returned by
         * <code>Math.ilogb(Float.MAX_VALUE)</code>.
         */
        public const int MAX_EXPONENT = 127;

        /**
         * Minimum exponent a normalized <code>float</code> number may
         * have.  It is equal to the value returned by
         * <code>Math.ilogb(Float.MIN_NORMAL)</code>.
         */
        public const int MIN_EXPONENT = -126;

        /**
         * The exponent the smallest positive {@code float} subnormal
         * value would have if it could be normalized.
         */
        public const int MIN_SUB_EXPONENT = MIN_EXPONENT - (SIGNIFICAND_WIDTH - 1);

        /**
         * Bias used in representing a {@code float} exponent.
         */
        public const int EXP_BIAS = 127;

        /**
         * Bit mask to isolate the sign bit of a {@code float}.
         */
        public const uint SIGN_BIT_MASK = 0x80000000;

        /**
         * Bit mask to isolate the exponent field of a
         * {@code float}.
         */
        public const int EXP_BIT_MASK = 0x7F800000;

        /**
         * Bit mask to isolate the significand field of a
         * {@code float}.
         */
        public const int SIGNIF_BIT_MASK = 0x007FFFFF;

    }
}