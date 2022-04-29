namespace AndroidUI.Extensions
{
    using static CastUtils;
    public static class FloatingPointExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int toPixel(this float pixelF) => (int)MathF.Ceiling(pixelF);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long toPixel(this double pixel) => (long)Math.Ceiling(pixel);

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 31 (the bit that is selected by the mask <c>0x80000000</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 30-23 (the bits that are selected by the mask <c>0x7f800000</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 22-0 (the bits that are selected by the mask <c>0x007fffff</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7f800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xff800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is <c>0x7fc00000</c>.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is an integer that, when given to the
        /// #intBitsToFloat(int) method, will produce a floating-point
        /// value the same as the argument to <c>floatToIntBits</c>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// </summary>
        /// <param name="value">a floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int ToIntBits(this float value) => !float.IsNaN(value) ? ToRawIntBits(value) : 0x7fc00000;

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 31 (the bit that is selected by the mask <c>0x80000000</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 30-23 (the bits that are selected by the mask <c>0x7f800000</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 22-0 (the bits that are selected by the mask <c>0x007fffff</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7f800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xff800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is the integer representing
        /// the actual NaN value. Unlike the <c>floatToIntBits</c>
        /// method, <c>floatToRawIntBits</c> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is an integer that, when given to the
        /// #intBitsToFloat(int) method, will produce a floating-point
        /// value the same as the argument to <c>floatToRawIntBits</c>
        /// </summary>
        /// <param name="value">a floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int ToRawIntBits(this float value) => BitConverter.SingleToInt32Bits(value);

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 31 (the bit that is selected by the mask <c>0x80000000</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 30-23 (the bits that are selected by the mask <c>0x7f800000</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 22-0 (the bits that are selected by the mask <c>0x007fffff</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7f800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xff800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is <c>0x7fc00000</c>.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is an integer that, when given to the
        /// #intBitsToFloat(int) method, will produce a floating-point
        /// value the same as the argument to <c>floatToIntBits</c>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// </summary>
        /// <param name="value">a floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static uint ToUIntBits(this float value) => !float.IsNaN(value) ? ToRawUIntBits(value) : 0x7fc00000;

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 31 (the bit that is selected by the mask <c>0x80000000</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 30-23 (the bits that are selected by the mask <c>0x7f800000</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 22-0 (the bits that are selected by the mask <c>0x007fffff</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7f800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xff800000</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is the integer representing
        /// the actual NaN value. Unlike the <c>floatToIntBits</c>
        /// method, <c>floatToRawIntBits</c> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is an integer that, when given to the
        /// #intBitsToFloat(int) method, will produce a floating-point
        /// value the same as the argument to <c>floatToRawIntBits</c>
        /// </summary>
        /// <param name="value">a floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static uint ToRawUIntBits(this float value) => BitConverter.SingleToUInt32Bits(value);

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double format" bit
        /// layout.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 63 (the bit that is selected by the mask <c>0x8000000000000000L</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 62-52 (the bits that are selected by the mask <c>0x7ff0000000000000L</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 51-0 (the bits that are selected by the mask <c>0x000fffffffffffffL</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7ff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xfff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is <c>0x7ff8000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is a <c>long</c> integer that, when given to the
        /// #longBitsToDouble(long) method, will produce a floating-point
        /// value the same as the argument to <c>doubleToLongBits</c>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// </summary>
        /// <param name="value">a <c>double</c> precision floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long ToLongBits(this double value) => !double.IsNaN(value) ? ToRawLongBits(value) : 0x7ff8000000000000L;

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 63 (the bit that is selected by the mask <c>0x8000000000000000L</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 62-52 (the bits that are selected by the mask <c>0x7ff0000000000000L</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 51-0 (the bits that are selected by the mask <c>0x000fffffffffffffL</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7ff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xfff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is the <c>long</c> integer representing
        /// the actual NaN value. Unlike the <c>doubleToLongBits</c>
        /// method, <c>doubleToRawLongBits</c> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is a <c>long</c> integer that, when given to the
        /// #longBitsToDouble(long) method, will produce a floating-point
        /// value the same as the argument to <c>doubleToRawLongBits</c>.
        /// </summary>
        /// <param name="value">a <c>double</c> precision floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long ToRawLongBits(this double value) => BitConverter.DoubleToInt64Bits(value);

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double format" bit
        /// layout.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 63 (the bit that is selected by the mask <c>0x8000000000000000L</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 62-52 (the bits that are selected by the mask <c>0x7ff0000000000000L</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 51-0 (the bits that are selected by the mask <c>0x000fffffffffffffL</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7ff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xfff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is <c>0x7ff8000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is a <c>long</c> integer that, when given to the
        /// #longBitsToDouble(long) method, will produce a floating-point
        /// value the same as the argument to <c>doubleToLongBits</c>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// </summary>
        /// <param name="value">a <c>double</c> precision floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ulong ToULongBits(this double value) => !double.IsNaN(value) ? ToRawULongBits(value) : 0x7ff8000000000000L;

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "double format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        /// <br></br>
        /// <br></br>
        /// <para>Bit 63 (the bit that is selected by the mask <c>0x8000000000000000L</c>)
        /// represents the sign of the floating-point number.
        /// <br></br>
        /// Bits 62-52 (the bits that are selected by the mask <c>0x7ff0000000000000L</c>)
        /// represent the exponent.
        /// <br></br>
        /// Bits 51-0 (the bits that are selected by the mask <c>0x000fffffffffffffL</c>)
        /// represent the significand (sometimes called the mantissa)
        /// of the floating-point number.
        /// </para>
        /// <br></br>
        /// <br></br>
        /// If the argument is positive infinity, the result is <c>0x7ff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is negative infinity, the result is <c>0xfff0000000000000L</c>.
        /// <br></br>
        /// <br></br>
        /// If the argument is NaN, the result is the <c>long</c> integer representing
        /// the actual NaN value. Unlike the <c>doubleToLongBits</c>
        /// method, <c>doubleToRawLongBits</c> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        /// <br></br>
        /// <br></br>
        /// In all cases, the result is a <c>long</c> integer that, when given to the
        /// #longBitsToDouble(long) method, will produce a floating-point
        /// value the same as the argument to <c>doubleToRawLongBits</c>.
        /// </summary>
        /// <param name="value">a <c>double</c> precision floating-point number.</param>
        /// <returns>the bits that represent the floating-point number.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static ulong ToRawULongBits(this double value) => BitConverter.DoubleToUInt64Bits(value);
    }
}
