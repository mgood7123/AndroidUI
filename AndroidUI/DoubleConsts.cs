namespace AndroidUI
{
    public static class DoubleConsts
    {
        /**
         * A constant holding the positive infinity of type
         * {@code double}. It is equal to the value returned by
         * {@code Double.longBitsToDouble(0x7ff0000000000000L)}.
         */
        public const double POSITIVE_INFINITY = double.PositiveInfinity;

        /**
         * A constant holding the negative infinity of type
         * {@code double}. It is equal to the value returned by
         * {@code Double.longBitsToDouble(0xfff0000000000000L)}.
         */
        public const double NEGATIVE_INFINITY = double.NegativeInfinity;

        /**
         * A constant holding a Not-a-Number (NaN) value of type
         * {@code double}. It is equivalent to the value returned by
         * {@code Double.longBitsToDouble(0x7ff8000000000000L)}.
         */
        public const double NaN = double.NaN;

        /**
         * A constant holding the largest positive finite value of type
         * {@code double},
         * (2-2<sup>-52</sup>)&middot;2<sup>1023</sup>.  It is equal to
         * the hexadecimal floating-point literal
         * {@code 0x1.fffffffffffffP+1023} and also equal to
         * {@code Double.longBitsToDouble(0x7fefffffffffffffL)}.
         */
        public const double MAX_VALUE = double.MaxValue;

        /**
         * A constant holding the smallest positive nonzero value of type
         * {@code double}, 2<sup>-1074</sup>. It is equal to the
         * hexadecimal floating-point literal
         * {@code 0x0.0000000000001P-1022} and also equal to
         * {@code Double.longBitsToDouble(0x1L)}.
         */
        public const double MIN_VALUE = double.MinValue;

        /**
         * A constant holding the smallest positive normal value of type
         * <code>double</code>, 2<sup>-1022</sup>.  It is equal to the
         * value returned by
         * <code>Double.longBitsToDouble(0x0010000000000000L)</code>.
         *
         * @since 1.5
         */
        public const double MIN_NORMAL = 2.2250738585072014E-308;

        /**
         * The number of bits used to represent a {@code double} value.
         *
         * @since 1.5
         */
        public const int SIZE = 64;

        /**
         * The number of bytes used to represent a {@code double} value.
         *
         * @since 1.8
         */
        public const int BYTES = SIZE / 8;

        /**
         * The number of logical bits in the significand of a
         * {@code double} number, including the implicit bit.
         */
        public const int SIGNIFICAND_WIDTH = 53;

        /**
         * Maximum exponent a finite <code>double</code> number may have.
         * It is equal to the value returned by
         * <code>Math.ilogb(Double.MAX_VALUE)</code>.
         */
        public const int MAX_EXPONENT = 1023;

        /**
         * Minimum exponent a normalized <code>double</code> number may
         * have.  It is equal to the value returned by
         * <code>Math.ilogb(Double.MIN_NORMAL)</code>.
         */
        public const int MIN_EXPONENT = -1022;

        /**
         * The exponent the smallest positive {@code double}
         * subnormal value would have if it could be normalized..
         */
        public const int MIN_SUB_EXPONENT = MIN_EXPONENT - (SIGNIFICAND_WIDTH - 1);

        /**
         * Bias used in representing a {@code double} exponent.
         */
        public const int EXP_BIAS = 1023;

        /**
         * Bit mask to isolate the sign bit of a {@code double}.
         */
        public const ulong SIGN_BIT_MASK = 0x8000000000000000L;

        /**
         * Bit mask to isolate the exponent field of a
         * {@code double}.
         */
        public const long EXP_BIT_MASK = 0x7FF0000000000000L;

        /**
         * Bit mask to isolate the significand field of a
         * {@code double}.
         */
        public const long SIGNIF_BIT_MASK = 0x000FFFFFFFFFFFFFL;
    }
}