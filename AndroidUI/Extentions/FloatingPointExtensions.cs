namespace AndroidUI.Extensions
{
    using static CastUtils;
    public static class FloatingPointExtensions
    {
        public static int toPixel(this float pixelF)
        {
            return (int)Math.Ceiling(pixelF);
        }

        public static long toPixel(this double pixel)
        {
            return (long)Math.Ceiling(pixel);
        }

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "single format" bit
         * layout.
         *
         * <p>Bit 31 (the bit that is selected by the mask
         * {@code 0x80000000}) represents the sign of the floating-point
         * number.
         * Bits 30-23 (the bits that are selected by the mask
         * {@code 0x7f800000}) represent the exponent.
         * Bits 22-0 (the bits that are selected by the mask
         * {@code 0x007fffff}) represent the significand (sometimes called
         * the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7f800000}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xff800000}.
         *
         * <p>If the argument is NaN, the result is {@code 0x7fc00000}.
         *
         * <p>In all cases, the result is an integer that, when given to the
         * {@link #intBitsToFloat(int)} method, will produce a floating-point
         * value the same as the argument to {@code floatToIntBits}
         * (except all NaN values are collapsed to a single
         * "canonical" NaN value).
         *
         * @param   value   a floating-point number.
         * @return the bits that represent the floating-point number.
         */
        public static int ToIntBits(this float value)
        {
            if (!float.IsNaN(value))
            {
                return ToRawIntBits(value);
            }
            return 0x7fc00000;
        }

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "single format" bit
         * layout, preserving Not-a-Number (NaN) values.
         *
         * <p>Bit 31 (the bit that is selected by the mask
         * {@code 0x80000000}) represents the sign of the floating-point
         * number.
         * Bits 30-23 (the bits that are selected by the mask
         * {@code 0x7f800000}) represent the exponent.
         * Bits 22-0 (the bits that are selected by the mask
         * {@code 0x007fffff}) represent the significand (sometimes called
         * the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7f800000}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xff800000}.
         *
         * <p>If the argument is NaN, the result is the integer representing
         * the actual NaN value.  Unlike the {@code floatToIntBits}
         * method, {@code floatToRawIntBits} does not collapse all the
         * bit patterns encoding a NaN to a single "canonical"
         * NaN value.
         *
         * <p>In all cases, the result is an integer that, when given to the
         * {@link #intBitsToFloat(int)} method, will produce a
         * floating-point value the same as the argument to
         * {@code floatToRawIntBits}.
         *
         * @param   value   a floating-point number.
         * @return the bits that represent the floating-point number.
         * @since 1.3
         */
        public static int ToRawIntBits(this float value) => reinterpret_cast<int>(value);

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "single format" bit
         * layout.
         *
         * <p>Bit 31 (the bit that is selected by the mask
         * {@code 0x80000000}) represents the sign of the floating-point
         * number.
         * Bits 30-23 (the bits that are selected by the mask
         * {@code 0x7f800000}) represent the exponent.
         * Bits 22-0 (the bits that are selected by the mask
         * {@code 0x007fffff}) represent the significand (sometimes called
         * the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7f800000}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xff800000}.
         *
         * <p>If the argument is NaN, the result is {@code 0x7fc00000}.
         *
         * <p>In all cases, the result is an integer that, when given to the
         * {@link #intBitsToFloat(int)} method, will produce a floating-point
         * value the same as the argument to {@code floatToIntBits}
         * (except all NaN values are collapsed to a single
         * "canonical" NaN value).
         *
         * @param   value   a floating-point number.
         * @return the bits that represent the floating-point number.
         */
        public static uint ToUIntBits(this float value)
        {
            if (!float.IsNaN(value))
            {
                return ToRawUIntBits(value);
            }
            return 0x7fc00000;
        }

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "single format" bit
         * layout, preserving Not-a-Number (NaN) values.
         *
         * <p>Bit 31 (the bit that is selected by the mask
         * {@code 0x80000000}) represents the sign of the floating-point
         * number.
         * Bits 30-23 (the bits that are selected by the mask
         * {@code 0x7f800000}) represent the exponent.
         * Bits 22-0 (the bits that are selected by the mask
         * {@code 0x007fffff}) represent the significand (sometimes called
         * the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7f800000}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xff800000}.
         *
         * <p>If the argument is NaN, the result is the integer representing
         * the actual NaN value.  Unlike the {@code floatToIntBits}
         * method, {@code floatToRawIntBits} does not collapse all the
         * bit patterns encoding a NaN to a single "canonical"
         * NaN value.
         *
         * <p>In all cases, the result is an integer that, when given to the
         * {@link #intBitsToFloat(int)} method, will produce a
         * floating-point value the same as the argument to
         * {@code floatToRawIntBits}.
         *
         * @param   value   a floating-point number.
         * @return the bits that represent the floating-point number.
         * @since 1.3
         */
        public static uint ToRawUIntBits(this float value) => reinterpret_cast<uint>(value);

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "double
         * format" bit layout.
         *
         * <p>Bit 63 (the bit that is selected by the mask
         * {@code 0x8000000000000000L}) represents the sign of the
         * floating-point number. Bits
         * 62-52 (the bits that are selected by the mask
         * {@code 0x7ff0000000000000L}) represent the exponent. Bits 51-0
         * (the bits that are selected by the mask
         * {@code 0x000fffffffffffffL}) represent the significand
         * (sometimes called the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7ff0000000000000L}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xfff0000000000000L}.
         *
         * <p>If the argument is NaN, the result is
         * {@code 0x7ff8000000000000L}.
         *
         * <p>In all cases, the result is a {@code long} integer that, when
         * given to the {@link #longBitsToDouble(long)} method, will produce a
         * floating-point value the same as the argument to
         * {@code doubleToLongBits} (except all NaN values are
         * collapsed to a single "canonical" NaN value).
         *
         * @param   value   a {@code double} precision floating-point number.
         * @return the bits that represent the floating-point number.
         */
        public static long ToLongBits(this double value)
        {
            if (!double.IsNaN(value))
            {
                return ToRawLongBits(value);
            }
            return 0x7ff8000000000000L;
        }

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "double
         * format" bit layout, preserving Not-a-Number (NaN) values.
         *
         * <p>Bit 63 (the bit that is selected by the mask
         * {@code 0x8000000000000000L}) represents the sign of the
         * floating-point number. Bits
         * 62-52 (the bits that are selected by the mask
         * {@code 0x7ff0000000000000L}) represent the exponent. Bits 51-0
         * (the bits that are selected by the mask
         * {@code 0x000fffffffffffffL}) represent the significand
         * (sometimes called the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7ff0000000000000L}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xfff0000000000000L}.
         *
         * <p>If the argument is NaN, the result is the {@code long}
         * integer representing the actual NaN value.  Unlike the
         * {@code doubleToLongBits} method,
         * {@code doubleToRawLongBits} does not collapse all the bit
         * patterns encoding a NaN to a single "canonical" NaN
         * value.
         *
         * <p>In all cases, the result is a {@code long} integer that,
         * when given to the {@link #longBitsToDouble(long)} method, will
         * produce a floating-point value the same as the argument to
         * {@code doubleToRawLongBits}.
         *
         * @param   value   a {@code double} precision floating-point number.
         * @return the bits that represent the floating-point number.
         * @since 1.3
         */
        public static long ToRawLongBits(this double value) => reinterpret_cast<long>(value);

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "double
         * format" bit layout.
         *
         * <p>Bit 63 (the bit that is selected by the mask
         * {@code 0x8000000000000000L}) represents the sign of the
         * floating-point number. Bits
         * 62-52 (the bits that are selected by the mask
         * {@code 0x7ff0000000000000L}) represent the exponent. Bits 51-0
         * (the bits that are selected by the mask
         * {@code 0x000fffffffffffffL}) represent the significand
         * (sometimes called the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7ff0000000000000L}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xfff0000000000000L}.
         *
         * <p>If the argument is NaN, the result is
         * {@code 0x7ff8000000000000L}.
         *
         * <p>In all cases, the result is a {@code long} integer that, when
         * given to the {@link #longBitsToDouble(long)} method, will produce a
         * floating-point value the same as the argument to
         * {@code doubleToLongBits} (except all NaN values are
         * collapsed to a single "canonical" NaN value).
         *
         * @param   value   a {@code double} precision floating-point number.
         * @return the bits that represent the floating-point number.
         */
        public static ulong ToULongBits(this double value)
        {
            if (!double.IsNaN(value))
            {
                return ToRawULongBits(value);
            }
            return 0x7ff8000000000000L;
        }

        /**
         * Returns a representation of the specified floating-point value
         * according to the IEEE 754 floating-point "double
         * format" bit layout, preserving Not-a-Number (NaN) values.
         *
         * <p>Bit 63 (the bit that is selected by the mask
         * {@code 0x8000000000000000L}) represents the sign of the
         * floating-point number. Bits
         * 62-52 (the bits that are selected by the mask
         * {@code 0x7ff0000000000000L}) represent the exponent. Bits 51-0
         * (the bits that are selected by the mask
         * {@code 0x000fffffffffffffL}) represent the significand
         * (sometimes called the mantissa) of the floating-point number.
         *
         * <p>If the argument is positive infinity, the result is
         * {@code 0x7ff0000000000000L}.
         *
         * <p>If the argument is negative infinity, the result is
         * {@code 0xfff0000000000000L}.
         *
         * <p>If the argument is NaN, the result is the {@code long}
         * integer representing the actual NaN value.  Unlike the
         * {@code doubleToLongBits} method,
         * {@code doubleToRawLongBits} does not collapse all the bit
         * patterns encoding a NaN to a single "canonical" NaN
         * value.
         *
         * <p>In all cases, the result is a {@code long} integer that,
         * when given to the {@link #longBitsToDouble(long)} method, will
         * produce a floating-point value the same as the argument to
         * {@code doubleToRawLongBits}.
         *
         * @param   value   a {@code double} precision floating-point number.
         * @return the bits that represent the floating-point number.
         * @since 1.3
         */
        public static ulong ToRawULongBits(this double value) => reinterpret_cast<ulong>(value);
    }
}
