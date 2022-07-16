using AndroidUI.Applications;
using AndroidUI.Graphics;
using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class IntegerExtensions
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColor ToSKColor(this SKColorF colorF) => (SKColor)colorF;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColorF ToSKColorF(this SKColor color) => ToSKColorF((uint)color);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColor ToSKColor(this int i) => ToSKColor((uint)i);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColor ToSKColor(this uint i)
        {
            return new SKColor(i);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColorF ToSKColorF(this int i) => ToSKColorF((uint)i);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColorF ToSKColorF(this uint i)
        {
            float r = ((i >> 16) & 0xff) / 255.0f;
            float g = ((i >> 8) & 0xff) / 255.0f;
            float b = ((i) & 0xff) / 255.0f;
            float a = ((i >> 24) & 0xff) / 255.0f;
            return new SKColorF(r, g, b, a);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColor ToSKColor(this long i) => ToSKColor((ulong)i);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColor ToSKColor(this ulong i)
        {
            // TODO: int color CANNOT be converted to long, this requires a colorspace re-encode
            byte r = (byte)((byte)(i >> 16) & 0xff);
            byte g = (byte)((byte)(i >> 8) & 0xff);
            byte b = (byte)((byte)i & 0xff);
            byte a = (byte)((byte)(i >> 24) & 0xff);
            return new SKColor(r, g, b, a);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColorF ToSKColorF(this long i)
        {
            float r = Color.red(i);
            float g = Color.green(i);
            float b = Color.blue(i);
            float a = Color.alpha(i);
            return new SKColorF(r, g, b, a);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static SKColorF ToSKColorF(this ulong i) => ToSKColorF((long)i);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte ToColorByte(this float value)
        {
            return (byte)(value * 255.0f + 0.5f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int ToColorInt(this float value)
        {
            return (int)(value * 255.0f + 0.5f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float ToColorFloat(this byte value)
        {
            return (float)(value / 255.0f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static float ToColorFloat(this int value)
        {
            return (float)(value / 255.0f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte ToColorByte(this double value)
        {
            return (byte)(value * 255.0f + 0.5f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int ToColorInt(this double value)
        {
            return (int)(value * 255.0f + 0.5f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double ToColorDouble(this byte value)
        {
            return (double)(value / 255.0f);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static double ToColorDouble(this int value)
        {
            return (double)(value / 255.0f);
        }

        public static int dipToPx(this int dip)
        {
            return (int)(DensityManager.ScreenDensityAsFloat * dip + 0.5f);
        }

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static short tripleRightShift(this short i, int shift_by) => UnsignedRightShift(i, shift_by);

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int tripleRightShift(this int i, int shift_by) => UnsignedRightShift(i, shift_by);

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long tripleRightShift(this long i, int shift_by) => UnsignedRightShift(i, shift_by);

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static short UnsignedRightShift(this short i, int shift_by)
        {
            return (short)((ushort)i >> shift_by);
        }

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int UnsignedRightShift(this int i, int shift_by)
        {
            return (int)((uint)i >> shift_by);
        }

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `>>>`
        /// <br></br>
        /// example: `i >>> shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long UnsignedRightShift(this long i, int shift_by)
        {
            return (int)((ulong)i >> shift_by);
        }

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator '&lt;&lt;&lt;'
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static short tripleLeftShift(this short i, int shift_by) => UnsignedLeftShift(i, shift_by);

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator `&lt;&lt;&lt;`
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int tripleLeftShift(this int i, int shift_by) => UnsignedLeftShift(i, shift_by);

        /// <summary>
        /// Alias for UnsignedShift - equivilant to java's Unsigned Shift operator `&lt;&lt;&lt;`
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long tripleLeftShift(this long i, int shift_by) => UnsignedLeftShift(i, shift_by);

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `&lt;&lt;&lt;`
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static short UnsignedLeftShift(this short i, int shift_by)
        {
            return (short)((ushort)i << shift_by);
        }

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `&lt;&lt;&lt;`
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int UnsignedLeftShift(this int i, int shift_by)
        {
            return (int)((uint)i << shift_by);
        }

        /// <summary>
        /// equivilant to java's Unsigned Shift operator `&lt;&lt;&lt;`
        /// <br></br>
        /// example: `i &lt;&lt;&lt; shift_by`
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static long UnsignedLeftShift(this long i, int shift_by)
        {
            return (int)((ulong)i << shift_by);
        }

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified {@code int} value.  This function is
        /// sometimes referred to as the <i>population count</i>.
        /// <br></br>
        /// <br></br>
        /// NOTE: this is copied from the following source:
        /// <br></br>
        /// <br></br>
        /// https://github.com/dotnet/corert/blob/c6af4cfc8b625851b91823d9be746c4f7abdc667/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs#L238
        /// </summary>
        /// <param name="i">the value whose bits are to be counted.</param>
        /// <returns>the number of one-bits in the two's complement binary
        /// representation of the specified int value.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int bitCount(this int i) => bitCount((uint)i);

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified {@code uint} value.  This function is
        /// sometimes referred to as the <i>population count</i>.
        /// <br></br>
        /// <br></br>
        /// NOTE: this is copied from the following source:
        /// <br></br>
        /// <br></br>
        /// https://github.com/dotnet/corert/blob/c6af4cfc8b625851b91823d9be746c4f7abdc667/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs#L238
        /// </summary>
        /// <param name="i">the value whose bits are to be counted.</param>
        /// <returns>the number of one-bits in the two's complement binary
        /// representation of the specified uint value.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int bitCount(this uint i) => System.Numerics.BitOperations.PopCount(i);

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified {@code long} value.  This function is
        /// sometimes referred to as the <i>population count</i>.
        /// <br></br>
        /// <br></br>
        /// NOTE: this is copied from the following source:
        /// <br></br>
        /// <br></br>
        /// https://github.com/dotnet/corert/blob/c6af4cfc8b625851b91823d9be746c4f7abdc667/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs#L238
        /// </summary>
        /// <param name="i">the value whose bits are to be counted.</param>
        /// <returns>the number of one-bits in the two's complement binary
        /// representation of the specified long value.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int bitCount(this long i) => bitCount((ulong)i);

        /// <summary>
        /// Returns the number of one-bits in the two's complement binary
        /// representation of the specified {@code ulong} value.  This function is
        /// sometimes referred to as the <i>population count</i>.
        /// <br></br>
        /// <br></br>
        /// NOTE: this is copied from the following source:
        /// <br></br>
        /// <br></br>
        /// https://github.com/dotnet/corert/blob/c6af4cfc8b625851b91823d9be746c4f7abdc667/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs#L238
        /// </summary>
        /// <param name="i">the value whose bits are to be counted.</param>
        /// <returns>the number of one-bits in the two's complement binary
        /// representation of the specified ulong value.</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int bitCount(this ulong i) => System.Numerics.BitOperations.PopCount(i);

        /**
         * Returns the {@code float} value corresponding to a given
         * bit representation.
         * The argument is considered to be a representation of a
         * floating-point value according to the IEEE 754 floating-point
         * "single format" bit layout.
         *
         * <p>If the argument is {@code 0x7f800000}, the result is positive
         * infinity.
         *
         * <p>If the argument is {@code 0xff800000}, the result is negative
         * infinity.
         *
         * <p>If the argument is any value in the range
         * {@code 0x7f800001} through {@code 0x7fffffff} or in
         * the range {@code 0xff800001} through
         * {@code 0xffffffff}, the result is a NaN.  No IEEE 754
         * floating-point operation provided by Java can distinguish
         * between two NaN values of the same type with different bit
         * patterns.  Distinct values of NaN are only distinguishable by
         * use of the {@code Float.floatToRawIntBits} method.
         *
         * <p>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
         * values that can be computed from the argument:
         *
         * <blockquote><pre>{@code
         * int s = ((bits >> 31) == 0) ? 1 : -1;
         * int e = ((bits >> 23) & 0xff);
         * int m = (e == 0) ?
         *                 (bits & 0x7fffff) << 1 :
         *                 (bits & 0x7fffff) | 0x800000;
         * }</pre></blockquote>
         *
         * Then the floating-point result equals the value of the mathematical
         * expression <i>s</i>&middot;<i>m</i>&middot;2<sup><i>e</i>-150</sup>.
         *
         * <p>Note that this method may not be able to return a
         * {@code float} NaN with exactly same bit pattern as the
         * {@code int} argument.  IEEE 754 distinguishes between two
         * kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
         * differences between the two kinds of NaN are generally not
         * visible in Java.  Arithmetic operations on signaling NaNs turn
         * them into quiet NaNs with a different, but often similar, bit
         * pattern.  However, on some processors merely copying a
         * signaling NaN also performs that conversion.  In particular,
         * copying a signaling NaN to return it to the calling method may
         * perform this conversion.  So {@code intBitsToFloat} may
         * not be able to return a {@code float} with a signaling NaN
         * bit pattern.  Consequently, for some {@code int} values,
         * {@code floatToRawIntBits(intBitsToFloat(start))} may
         * <i>not</i> equal {@code start}.  Moreover, which
         * particular bit patterns represent signaling NaNs is platform
         * dependent; although all NaN bit patterns, quiet or signaling,
         * must be in the NaN range identified above.
         *
         * @param   bits   an integer.
         * @return  the {@code float} floating-point value with the same bit
         *          pattern.
         */
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe float BitsToFloat(this int bits) => BitConverter.Int32BitsToSingle(bits);

        /**
         * Returns the {@code float} value corresponding to a given
         * bit representation.
         * The argument is considered to be a representation of a
         * floating-point value according to the IEEE 754 floating-point
         * "single format" bit layout.
         *
         * <p>If the argument is {@code 0x7f800000}, the result is positive
         * infinity.
         *
         * <p>If the argument is {@code 0xff800000}, the result is negative
         * infinity.
         *
         * <p>If the argument is any value in the range
         * {@code 0x7f800001} through {@code 0x7fffffff} or in
         * the range {@code 0xff800001} through
         * {@code 0xffffffff}, the result is a NaN.  No IEEE 754
         * floating-point operation provided by Java can distinguish
         * between two NaN values of the same type with different bit
         * patterns.  Distinct values of NaN are only distinguishable by
         * use of the {@code Float.floatToRawIntBits} method.
         *
         * <p>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
         * values that can be computed from the argument:
         *
         * <blockquote><pre>{@code
         * int s = ((bits >> 31) == 0) ? 1 : -1;
         * int e = ((bits >> 23) & 0xff);
         * int m = (e == 0) ?
         *                 (bits & 0x7fffff) << 1 :
         *                 (bits & 0x7fffff) | 0x800000;
         * }</pre></blockquote>
         *
         * Then the floating-point result equals the value of the mathematical
         * expression <i>s</i>&middot;<i>m</i>&middot;2<sup><i>e</i>-150</sup>.
         *
         * <p>Note that this method may not be able to return a
         * {@code float} NaN with exactly same bit pattern as the
         * {@code int} argument.  IEEE 754 distinguishes between two
         * kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
         * differences between the two kinds of NaN are generally not
         * visible in Java.  Arithmetic operations on signaling NaNs turn
         * them into quiet NaNs with a different, but often similar, bit
         * pattern.  However, on some processors merely copying a
         * signaling NaN also performs that conversion.  In particular,
         * copying a signaling NaN to return it to the calling method may
         * perform this conversion.  So {@code intBitsToFloat} may
         * not be able to return a {@code float} with a signaling NaN
         * bit pattern.  Consequently, for some {@code int} values,
         * {@code floatToRawIntBits(intBitsToFloat(start))} may
         * <i>not</i> equal {@code start}.  Moreover, which
         * particular bit patterns represent signaling NaNs is platform
         * dependent; although all NaN bit patterns, quiet or signaling,
         * must be in the NaN range identified above.
         *
         * @param   bits   an unsigned integer.
         * @return  the {@code float} floating-point value with the same bit
         *          pattern.
         */
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe float BitsToFloat(this uint bits) => BitConverter.UInt32BitsToSingle(bits);

        /**
          * Returns the {@code double} value corresponding to a given
          * bit representation.
          * The argument is considered to be a representation of a
          * floating-point value according to the IEEE 754 floating-point
          * "double format" bit layout.
          *
          * <p>If the argument is {@code 0x7ff0000000000000L}, the result
          * is positive infinity.
          *
          * <p>If the argument is {@code 0xfff0000000000000L}, the result
          * is negative infinity.
          *
          * <p>If the argument is any value in the range
          * {@code 0x7ff0000000000001L} through
          * {@code 0x7fffffffffffffffL} or in the range
          * {@code 0xfff0000000000001L} through
          * {@code 0xffffffffffffffffL}, the result is a NaN.  No IEEE
          * 754 floating-point operation provided by Java can distinguish
          * between two NaN values of the same type with different bit
          * patterns.  Distinct values of NaN are only distinguishable by
          * use of the {@code Double.doubleToRawLongBits} method.
          *
          * <p>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
          * values that can be computed from the argument:
          *
          * <blockquote><pre>{@code
          * int s = ((bits >> 63) == 0) ? 1 : -1;
          * int e = (int)((bits >> 52) & 0x7ffL);
          * long m = (e == 0) ?
          *                 (bits & 0xfffffffffffffL) << 1 :
          *                 (bits & 0xfffffffffffffL) | 0x10000000000000L;
          * }</pre></blockquote>
          *
          * Then the floating-point result equals the value of the mathematical
          * expression <i>s</i>&middot;<i>m</i>&middot;2<sup><i>e</i>-1075</sup>.
          *
          * <p>Note that this method may not be able to return a
          * {@code double} NaN with exactly same bit pattern as the
          * {@code long} argument.  IEEE 754 distinguishes between two
          * kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
          * differences between the two kinds of NaN are generally not
          * visible in Java.  Arithmetic operations on signaling NaNs turn
          * them into quiet NaNs with a different, but often similar, bit
          * pattern.  However, on some processors merely copying a
          * signaling NaN also performs that conversion.  In particular,
          * copying a signaling NaN to return it to the calling method
          * may perform this conversion.  So {@code longBitsToDouble}
          * may not be able to return a {@code double} with a
          * signaling NaN bit pattern.  Consequently, for some
          * {@code long} values,
          * {@code doubleToRawLongBits(longBitsToDouble(start))} may
          * <i>not</i> equal {@code start}.  Moreover, which
          * particular bit patterns represent signaling NaNs is platform
          * dependent; although all NaN bit patterns, quiet or signaling,
          * must be in the NaN range identified above.
          *
          * @param   bits   any {@code long} integer.
          * @return  the {@code double} floating-point value with the same
          *          bit pattern.
          */
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe double BitsToDouble(this long bits) => BitConverter.Int64BitsToDouble(bits);

        /**
          * Returns the {@code double} value corresponding to a given
          * bit representation.
          * The argument is considered to be a representation of a
          * floating-point value according to the IEEE 754 floating-point
          * "double format" bit layout.
          *
          * <p>If the argument is {@code 0x7ff0000000000000L}, the result
          * is positive infinity.
          *
          * <p>If the argument is {@code 0xfff0000000000000L}, the result
          * is negative infinity.
          *
          * <p>If the argument is any value in the range
          * {@code 0x7ff0000000000001L} through
          * {@code 0x7fffffffffffffffL} or in the range
          * {@code 0xfff0000000000001L} through
          * {@code 0xffffffffffffffffL}, the result is a NaN.  No IEEE
          * 754 floating-point operation provided by Java can distinguish
          * between two NaN values of the same type with different bit
          * patterns.  Distinct values of NaN are only distinguishable by
          * use of the {@code Double.doubleToRawLongBits} method.
          *
          * <p>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
          * values that can be computed from the argument:
          *
          * <blockquote><pre>{@code
          * int s = ((bits >> 63) == 0) ? 1 : -1;
          * int e = (int)((bits >> 52) & 0x7ffL);
          * long m = (e == 0) ?
          *                 (bits & 0xfffffffffffffL) << 1 :
          *                 (bits & 0xfffffffffffffL) | 0x10000000000000L;
          * }</pre></blockquote>
          *
          * Then the floating-point result equals the value of the mathematical
          * expression <i>s</i>&middot;<i>m</i>&middot;2<sup><i>e</i>-1075</sup>.
          *
          * <p>Note that this method may not be able to return a
          * {@code double} NaN with exactly same bit pattern as the
          * {@code long} argument.  IEEE 754 distinguishes between two
          * kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
          * differences between the two kinds of NaN are generally not
          * visible in Java.  Arithmetic operations on signaling NaNs turn
          * them into quiet NaNs with a different, but often similar, bit
          * pattern.  However, on some processors merely copying a
          * signaling NaN also performs that conversion.  In particular,
          * copying a signaling NaN to return it to the calling method
          * may perform this conversion.  So {@code longBitsToDouble}
          * may not be able to return a {@code double} with a
          * signaling NaN bit pattern.  Consequently, for some
          * {@code long} values,
          * {@code doubleToRawLongBits(longBitsToDouble(start))} may
          * <i>not</i> equal {@code start}.  Moreover, which
          * particular bit patterns represent signaling NaNs is platform
          * dependent; although all NaN bit patterns, quiet or signaling,
          * must be in the NaN range identified above.
          *
          * @param   bits   any {@code ulong} integer.
          * @return  the {@code double} floating-point value with the same
          *          bit pattern.
          */
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe double BitsToDouble(this ulong bits) => BitConverter.UInt64BitsToDouble(bits);


        /**
         * The minimum radix available for conversion to and from strings.
         * The constant value of this field is the smallest value permitted
         * for the radix argument in radix-conversion methods such as the
         * {@code digit} method, the {@code forDigit} method, and the
         * {@code toString} method of class {@code Integer}.
         *
         * @see     Character#digit(char, int)
         * @see     Character#forDigit(int, int)
         * @see     Integer#toString(int, int)
         * @see     Integer#valueOf(String)
         */
        const int MIN_RADIX = 2; // Character.MIN_RADIX

        /**
         * The maximum radix available for conversion to and from strings.
         * The constant value of this field is the largest value permitted
         * for the radix argument in radix-conversion methods such as the
         * {@code digit} method, the {@code forDigit} method, and the
         * {@code toString} method of class {@code Integer}.
         *
         * @see     Character#digit(char, int)
         * @see     Character#forDigit(int, int)
         * @see     Integer#toString(int, int)
         * @see     Integer#valueOf(String)
         */
        const int MAX_RADIX = 36; // Character.MAX_RADIX

        /**
         * All possible chars for representing a number as a String
         */
        static readonly char[] digits = {
            '0' , '1' , '2' , '3' , '4' , '5' ,
            '6' , '7' , '8' , '9' , 'a' , 'b' ,
            'c' , 'd' , 'e' , 'f' , 'g' , 'h' ,
            'i' , 'j' , 'k' , 'l' , 'm' , 'n' ,
            'o' , 'p' , 'q' , 'r' , 's' , 't' ,
            'u' , 'v' , 'w' , 'x' , 'y' , 'z'
        };

        /**
         * Returns a string representation of the integer argument as an
         * unsigned integer in base&nbsp;16.
         *
         * <p>The unsigned integer value is the argument plus 2<sup>32</sup>
         * if the argument is negative; otherwise, it is equal to the
         * argument.  This value is converted to a string of ASCII digits
         * in hexadecimal (base&nbsp;16) with no extra leading
         * {@code 0}s.
         *
         * <p>The value of the argument can be recovered from the returned
         * string {@code s} by calling {@link
         * Integer#parseUnsignedInt(String, int)
         * Integer.parseUnsignedInt(s, 16)}.
         *
         * <p>If the unsigned magnitude is zero, it is represented by a
         * single zero character {@code '0'} ({@code '\u005Cu0030'});
         * otherwise, the first character of the representation of the
         * unsigned magnitude will not be the zero character. The
         * following characters are used as hexadecimal digits:
         *
         * <blockquote>
         *  {@code 0123456789abcdef}
         * </blockquote>
         *
         * These are the characters {@code '\u005Cu0030'} through
         * {@code '\u005Cu0039'} and {@code '\u005Cu0061'} through
         * {@code '\u005Cu0066'}. If uppercase letters are
         * desired, the {@link java.lang.String#toUpperCase()} method may
         * be called on the result:
         *
         * <blockquote>
         *  {@code Integer.toHexString(n).toUpperCase()}
         * </blockquote>
         *
         * @param   i   an integer to be converted to a string.
         * @return  the string representation of the unsigned integer value
         *          represented by the argument in hexadecimal (base&nbsp;16).
         * @see #parseUnsignedInt(String, int)
         * @see #toUnsignedString(int, int)
         * @since   1.0.2
         */
        public static string toHexString(this int i)
        {
            return toUnsignedString0(i, 4);
        }

        /**
         * Returns a string representation of the integer argument as an
         * unsigned integer in base&nbsp;8.
         *
         * <p>The unsigned integer value is the argument plus 2<sup>32</sup>
         * if the argument is negative; otherwise, it is equal to the
         * argument.  This value is converted to a string of ASCII digits
         * in octal (base&nbsp;8) with no extra leading {@code 0}s.
         *
         * <p>The value of the argument can be recovered from the returned
         * string {@code s} by calling {@link
         * Integer#parseUnsignedInt(String, int)
         * Integer.parseUnsignedInt(s, 8)}.
         *
         * <p>If the unsigned magnitude is zero, it is represented by a
         * single zero character {@code '0'} ({@code '\u005Cu0030'});
         * otherwise, the first character of the representation of the
         * unsigned magnitude will not be the zero character. The
         * following characters are used as octal digits:
         *
         * <blockquote>
         * {@code 01234567}
         * </blockquote>
         *
         * These are the characters {@code '\u005Cu0030'} through
         * {@code '\u005Cu0037'}.
         *
         * @param   i   an integer to be converted to a string.
         * @return  the string representation of the unsigned integer value
         *          represented by the argument in octal (base&nbsp;8).
         * @see #parseUnsignedInt(String, int)
         * @see #toUnsignedString(int, int)
         * @since   1.0.2
         */
        public static string toOctalString(this int i)
        {
            return toUnsignedString0(i, 3);
        }

        /**
         * Returns a string representation of the integer argument as an
         * unsigned integer in base&nbsp;2.
         *
         * <p>The unsigned integer value is the argument plus 2<sup>32</sup>
         * if the argument is negative; otherwise it is equal to the
         * argument.  This value is converted to a string of ASCII digits
         * in binary (base&nbsp;2) with no extra leading {@code 0}s.
         *
         * <p>The value of the argument can be recovered from the returned
         * string {@code s} by calling {@link
         * Integer#parseUnsignedInt(String, int)
         * Integer.parseUnsignedInt(s, 2)}.
         *
         * <p>If the unsigned magnitude is zero, it is represented by a
         * single zero character {@code '0'} ({@code '\u005Cu0030'});
         * otherwise, the first character of the representation of the
         * unsigned magnitude will not be the zero character. The
         * characters {@code '0'} ({@code '\u005Cu0030'}) and {@code
         * '1'} ({@code '\u005Cu0031'}) are used as binary digits.
         *
         * @param   i   an integer to be converted to a string.
         * @return  the string representation of the unsigned integer value
         *          represented by the argument in binary (base&nbsp;2).
         * @see #parseUnsignedInt(String, int)
         * @see #toUnsignedString(int, int)
         * @since   1.0.2
         */
        public static string toBinaryString(this int i)
        {
            return toUnsignedString0(i, 1);
        }

        /**
         * Returns the number of zero bits preceding the highest-order
         * ("leftmost") one-bit in the two's complement binary representation
         * of the specified {@code int} value.  Returns 32 if the
         * specified value has no one-bits in its two's complement representation,
         * in other words if it is equal to zero.
         *
         * <p>Note that this method is closely related to the logarithm base 2.
         * For all positive {@code int} values x:
         * <ul>
         * <li>floor(log<sub>2</sub>(x)) = {@code 31 - numberOfLeadingZeros(x)}
         * <li>ceil(log<sub>2</sub>(x)) = {@code 32 - numberOfLeadingZeros(x - 1)}
         * </ul>
         *
         * @param i the value whose number of leading zeros is to be computed
         * @return the number of zero bits preceding the highest-order
         *     ("leftmost") one-bit in the two's complement binary representation
         *     of the specified {@code int} value, or 32 if the value
         *     is equal to zero.
         * @since 1.5
         */
        public static int numberOfLeadingZeros(this int i)
        {
            // HD, Count leading 0's
            if (i <= 0)
                return i == 0 ? 32 : 0;
            int n = 31;
            if (i >= 1 << 16) { n -= 16; i = i.UnsignedRightShift(16); }
            if (i >= 1 << 8) { n -= 8; i = i.UnsignedRightShift(8); }
            if (i >= 1 << 4) { n -= 4; i = i.UnsignedRightShift(4); }
            if (i >= 1 << 2) { n -= 2; i = i.UnsignedRightShift(2); }
            return n - i.UnsignedRightShift(1);
        }

        public const int SIZE = 32;

        /**
         * Convert the integer to an unsigned number.
         */
        private static string toUnsignedString0(int val, int shift)
        {
            // assert shift > 0 && shift <=5 : "Illegal shift value";
            int mag = SIZE - numberOfLeadingZeros(val);
            int chars = Math.Max((mag + (shift - 1)) / shift, 1);


            // BEGIN Android-changed: Use single-byte chars.
            /*
            if (COMPACT_STRINGS) {
             */
            char[] buf = new char[chars];
            formatUnsignedInt(val, shift, buf, 0, chars);
            /*
                return new String(buf, LATIN1);
            } else {
                byte[] buf = new byte[chars * 2];
                formatUnsignedIntUTF16(val, shift, buf, 0, chars);
                return new String(buf, UTF16);
            }
             */
            return new string(buf);
            // END Android-changed: Use single-byte chars.
        }

        /**
         * Format an {@code int} (treated as unsigned) into a character buffer. If
         * {@code len} exceeds the formatted ASCII representation of {@code val},
         * {@code buf} will be padded with leading zeroes.
         *
         * @param val the unsigned int to format
         * @param shift the log2 of the base to format in (4 for hex, 3 for octal, 1 for binary)
         * @param buf the character buffer to write to
         * @param offset the offset in the destination buffer to start at
         * @param len the number of characters to write
         */
        static void formatUnsignedInt(int val, int shift, char[] buf, int offset, int len)
        {
            // assert shift > 0 && shift <=5 : "Illegal shift value";
            // assert offset >= 0 && offset < buf.length : "illegal offset";
            // assert len > 0 && (offset + len) <= buf.length : "illegal length";
            int charPos = offset + len;
            int radix = 1 << shift;
            int mask = radix - 1;
            do
            {
                buf[--charPos] = digits[val & mask];
                val = val.UnsignedRightShift(shift);
            } while (charPos > offset);
        }

        /** byte[]/LATIN1 version    */
        static void formatUnsignedInt(int val, int shift, byte[] buf, int offset, int len)
        {
            int charPos = offset + len;
            int radix = 1 << shift;
            int mask = radix - 1;
            do
            {
                buf[--charPos] = (byte)digits[val & mask];
                val = val.UnsignedRightShift(shift);
            } while (charPos > offset);
        }

        // BEGIN Android-removed: UTF16 version of formatUnsignedInt().
        /*
        /** byte[]/UTF16 version    *
        private static void formatUnsignedIntUTF16(int val, int shift, byte[] buf, int offset, int len) {
            int charPos = offset + len;
            int radix = 1 << shift;
            int mask = radix - 1;
            do {
                StringUTF16.putChar(buf, --charPos, Integer.digits[val & mask]);
                val >>>= shift;
            } while (charPos > offset);
        }
         */
        // END Android-removed: UTF16 version of formatUnsignedInt().

        // BEGIN Android-changed: Cache the toString() result for small values.
        private static readonly string[] SMALL_NEG_VALUES = new string[100];
        private static readonly string[] SMALL_NONNEG_VALUES = new string[100];
        // END Android-changed: Cache the toString() result for small values.

        static readonly char[] DigitTens = {
            '0', '0', '0', '0', '0', '0', '0', '0', '0', '0',
            '1', '1', '1', '1', '1', '1', '1', '1', '1', '1',
            '2', '2', '2', '2', '2', '2', '2', '2', '2', '2',
            '3', '3', '3', '3', '3', '3', '3', '3', '3', '3',
            '4', '4', '4', '4', '4', '4', '4', '4', '4', '4',
            '5', '5', '5', '5', '5', '5', '5', '5', '5', '5',
            '6', '6', '6', '6', '6', '6', '6', '6', '6', '6',
            '7', '7', '7', '7', '7', '7', '7', '7', '7', '7',
            '8', '8', '8', '8', '8', '8', '8', '8', '8', '8',
            '9', '9', '9', '9', '9', '9', '9', '9', '9', '9',
        };

        static readonly char[] DigitOnes = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        };


        /**
         * Returns a {@code String} object representing the
         * specified integer. The argument is converted to signed decimal
         * representation and returned as a string, exactly as if the
         * argument and radix 10 were given as arguments to the {@link
         * #toString(int, int)} method.
         *
         * @param   i   an integer to be converted.
         * @return  a string representation of the argument in base&nbsp;10.
         */
        public static string toString(this int i)
        {
            // BEGIN Android-changed: Cache the String for small values.
            bool negative = i < 0;
            bool small = negative ? i > -100 : i < 100;
            if (small)
            {
                string[] smallValues = negative ? SMALL_NEG_VALUES : SMALL_NONNEG_VALUES;

                if (negative)
                {
                    i = -i;
                    if (smallValues[i] == null)
                    {
                        smallValues[i] =
                            i < 10 ? new string(new char[] { '-', DigitOnes[i] })
                                   : new string(new char[] { '-', DigitTens[i], DigitOnes[i] });
                    }
                }
                else
                {
                    if (smallValues[i] == null)
                    {
                        smallValues[i] =
                            i < 10 ? new string(new char[] { DigitOnes[i] })
                                   : new string(new char[] { DigitTens[i], DigitOnes[i] });
                    }
                }
                return smallValues[i];
            }
            // END Android-changed: Cache the String for small values.
            int size = stringSize(i);

            // BEGIN Android-changed: Use single-byte chars.
            /*
            if (COMPACT_STRINGS) {
             */
            char[] buf = new char[size];
            getChars(i, size, buf);
            /*
                return new String(buf, LATIN1);
            } else {
                byte[] buf = new byte[size * 2];
                StringUTF16.getChars(i, size, buf);
                return new String(buf, UTF16);
            }
             */
            return new string(buf);
            // END Android-changed: Use single-byte chars.
        }

        /**
         * Returns a string representation of the argument as an unsigned
         * decimal value.
         *
         * The argument is converted to unsigned decimal representation
         * and returned as a string exactly as if the argument and radix
         * 10 were given as arguments to the {@link #toUnsignedString(int,
         * int)} method.
         *
         * @param   i  an integer to be converted to an unsigned string.
         * @return  an unsigned string representation of the argument.
         * @see     #toUnsignedString(int, int)
         * @since 1.8
         */
        public static string toUnsignedString(this int i)
        {
            return toString(toUnsignedLong(i));
        }

        /**
         * Returns a {@code String} object representing the specified
         * {@code long}.  The argument is converted to signed decimal
         * representation and returned as a string, exactly as if the
         * argument and the radix 10 were given as arguments to the {@link
         * #toString(long, int)} method.
         *
         * @param   i   a {@code long} to be converted.
         * @return  a string representation of the argument in base&nbsp;10.
         */
        public static string toString(this long i)
        {
            int size = stringSize(i);
            // BEGIN Android-changed: Always use single-byte buffer.
            /*
            if (COMPACT_STRINGS) {
             */
            char[] buf = new char[size];
            getChars(i, size, buf);
            /*
                return new String(buf, LATIN1);
            } else {
                byte[] buf = new byte[size * 2];
                StringUTF16.getChars(i, size, buf);
                return new String(buf, UTF16);
            }
             */
            return new string(buf);
            // END Android-changed: Always use single-byte buffer.
        }

        /**
         * Converts the argument to a {@code long} by an unsigned
         * conversion.  In an unsigned conversion to a {@code long}, the
         * high-order 32 bits of the {@code long} are zero and the
         * low-order 32 bits are equal to the bits of the integer
         * argument.
         *
         * Consequently, zero and positive {@code int} values are mapped
         * to a numerically equal {@code long} value and negative {@code
         * int} values are mapped to a {@code long} value equal to the
         * input plus 2<sup>32</sup>.
         *
         * @param  x the value to convert to an unsigned {@code long}
         * @return the argument converted to {@code long} by an unsigned
         *         conversion
         * @since 1.8
         */
        public static long toUnsignedLong(this int x)
        {
            return ((long)x) & 0xffffffffL;
        }

        /**
         * Places characters representing the integer i into the
         * character array buf. The characters are placed into
         * the buffer backwards starting with the least significant
         * digit at the specified index (exclusive), and working
         * backwards from there.
         *
         * @implNote This method converts positive inputs into negative
         * values, to cover the Integer.MIN_VALUE case. Converting otherwise
         * (negative to positive) will expose -Integer.MIN_VALUE that overflows
         * integer.
         *
         * @param i     value to convert
         * @param index next index, after the least significant digit
         * @param buf   target buffer, Latin1-encoded
         * @return index of the most significant digit or minus sign, if present
         */
        static int getChars(int i, int index, char[] buf)
        {
            int q, r;
            int charPos = index;

            bool negative = i < 0;
            if (!negative)
            {
                i = -i;
            }

            // Generate two digits per iteration
            while (i <= -100)
            {
                q = i / 100;
                r = (q * 100) - i;
                i = q;
                buf[--charPos] = DigitOnes[r];
                buf[--charPos] = DigitTens[r];
            }

            // We know there are at most two digits left at this point.
            q = i / 10;
            r = (q * 10) - i;
            buf[--charPos] = (char)('0' + r);

            // Whatever left is the remaining digit.
            if (q < 0)
            {
                buf[--charPos] = (char)('0' - q);
            }

            if (negative)
            {
                buf[--charPos] = '-';
            }
            return charPos;
        }

        /**
         * A constant holding the minimum value a {@code long} can
         * have, -2<sup>63</sup>.
         */
        public const ulong LONG_MIN_VALUE = 0x8000000000000000L;

        /**
         * A constant holding the maximum value a {@code long} can
         * have, 2<sup>63</sup>-1.
         */
        public const ulong LONG_MAX_VALUE = 0x7fffffffffffffffL;


        /**
         * Places characters representing the long i into the
         * character array buf. The characters are placed into
         * the buffer backwards starting with the least significant
         * digit at the specified index (exclusive), and working
         * backwards from there.
         *
         * @implNote This method converts positive inputs into negative
         * values, to cover the Long.MIN_VALUE case. Converting otherwise
         * (negative to positive) will expose -Long.MIN_VALUE that overflows
         * long.
         *
         * @param i     value to convert
         * @param index next index, after the least significant digit
         * @param buf   target buffer, Latin1-encoded
         * @return index of the most significant digit or minus sign, if present
         */
        static int getChars(long i, int index, char[] buf)
        {
            long q;
            int r;
            int charPos = index;

            bool negative = i < 0;
            if (!negative)
            {
                i = -i;
            }

            // Get 2 digits/iteration using longs until quotient fits into an int
            while ((ulong)i <= LONG_MIN_VALUE)
            {
                q = i / 100;
                r = (int)((q * 100) - i);
                i = q;
                buf[--charPos] = DigitOnes[r];
                buf[--charPos] = DigitTens[r];
            }

            // Get 2 digits/iteration using ints
            int q2;
            int i2 = (int)i;
            while (i2 <= -100)
            {
                q2 = i2 / 100;
                r = (q2 * 100) - i2;
                i2 = q2;
                buf[--charPos] = DigitOnes[r];
                buf[--charPos] = DigitTens[r];
            }

            // We know there are at most two digits left at this point.
            q2 = i2 / 10;
            r = (q2 * 10) - i2;
            buf[--charPos] = (char)('0' + r);

            // Whatever left is the remaining digit.
            if (q2 < 0)
            {
                buf[--charPos] = (char)('0' - q2);
            }

            if (negative)
            {
                buf[--charPos] = '-';
            }
            return charPos;
        }

        /**
         * Returns the string representation size for a given int value.
         *
         * @param x int value
         * @return string size
         *
         * @implNote There are other ways to compute this: e.g. binary search,
         * but values are biased heavily towards zero, and therefore linear search
         * wins. The iteration results are also routinely inlined in the generated
         * code after loop unrolling.
         */
        static int stringSize(int x)
        {
            int d = 1;
            if (x >= 0)
            {
                d = 0;
                x = -x;
            }
            int p = -10;
            for (int i = 1; i < 10; i++)
            {
                if (x > p)
                    return i + d;
                p = 10 * p;
            }
            return 10 + d;
        }

        /**
         * Returns the string representation size for a given long value.
         *
         * @param x long value
         * @return string size
         *
         * @implNote There are other ways to compute this: e.g. binary search,
         * but values are biased heavily towards zero, and therefore linear search
         * wins. The iteration results are also routinely inlined in the generated
         * code after loop unrolling.
         */
        static int stringSize(long x)
        {
            int d = 1;
            if (x >= 0)
            {
                d = 0;
                x = -x;
            }
            long p = -10;
            for (int i = 1; i < 19; i++)
            {
                if (x > p)
                    return i + d;
                p = 10 * p;
            }
            return 19 + d;
        }

        public static int toInt(this bool b) => b ? 1 : 0;

        public static bool toBool(this byte x) => x != 0;
        public static bool toBool(this short x) => x != 0;
        public static bool toBool(this int x) => x != 0;
        public static bool toBool(this long x) => x != 0;
        public static bool toBool(this ushort x) => x != 0;
        public static bool toBool(this uint x) => x != 0;
        public static bool toBool(this ulong x) => x != 0;
        public static bool toBool(this float x) => x != 0.0f;
        public static bool toBool(this double x) => x != 0.0;
        public static bool toBool(this decimal x) => x != 0;
    }
}
