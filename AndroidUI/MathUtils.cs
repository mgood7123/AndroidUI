namespace AndroidUI
{
    using AndroidUI.Exceptions;
    using AndroidUI.Extensions;
    using static CastUtils;
    public static partial class MathUtils
    {
        public static short clamp(this short value, short min, short max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static int clamp(this int value, int min, int max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static long clamp(this long value, long min, long max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static float clamp(this float value, float min, float max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static double clamp(this double value, double min, double max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static bool isFinite(this float value)
        {
            return !(float.IsNaN(value) && float.IsInfinity(value));
        }

        public static bool isFinite(this double value)
        {
            return !(double.IsNaN(value) && double.IsInfinity(value));
        }

        public static int constrain(int amount, int low, int high)
        {
            return amount < low ? low : (amount > high ? high : amount);
        }

        public static long constrain(long amount, long low, long high)
        {
            return amount < low ? low : (amount > high ? high : amount);
        }

        public static float constrain(float amount, float low, float high)
        {
            return amount < low ? low : (amount > high ? high : amount);
        }

        /**
         * Returns the first floating-point argument with the sign of the
         * second floating-point argument.  Note that unlike the {@link
         * StrictMath#copySign(double, double) StrictMath.copySign}
         * method, this method does not require NaN {@code sign}
         * arguments to be treated as positive values; implementations are
         * permitted to treat some NaN arguments as positive and other NaN
         * arguments as negative to allow greater performance.
         *
         * @param magnitude  the parameter providing the magnitude of the result
         * @param sign   the parameter providing the sign of the result
         * @return a value with the magnitude of {@code magnitude}
         * and the sign of {@code sign}.
         * @since 1.6
         */
        public static double copySign(double magnitude, double sign)
        {
            return IntegerExtensions.BitsToDouble(sign.ToRawULongBits() &
                FloatConsts.SIGN_BIT_MASK | magnitude.ToRawULongBits() &
                (FloatConsts.EXP_BIT_MASK | FloatConsts.SIGNIF_BIT_MASK)
            );
        }

        /**
         * Returns the first floating-point argument with the sign of the
         * second floating-point argument.  Note that unlike the {@link
         * StrictMath#copySign(float, float) StrictMath.copySign}
         * method, this method does not require NaN {@code sign}
         * arguments to be treated as positive values; implementations are
         * permitted to treat some NaN arguments as positive and other NaN
         * arguments as negative to allow greater performance.
         *
         * @param magnitude  the parameter providing the magnitude of the result
         * @param sign   the parameter providing the sign of the result
         * @return a value with the magnitude of {@code magnitude}
         * and the sign of {@code sign}.
         * @since 1.6
         */
        public static float copySign(float magnitude, float sign)
        {
            return IntegerExtensions.BitsToFloat(sign.ToRawUIntBits() &
                FloatConsts.SIGN_BIT_MASK | magnitude.ToRawUIntBits() &
                (FloatConsts.EXP_BIT_MASK | FloatConsts.SIGNIF_BIT_MASK)
            );
        }

        /**
         * Returns the unbiased exponent used in the representation of a
         * {@code float}.  Special cases:
         *
         * <ul>
         * <li>If the argument is NaN or infinite, then the result is
         * {@link Float#MAX_EXPONENT} + 1.
         * <li>If the argument is zero or subnormal, then the result is
         * {@link Float#MIN_EXPONENT} -1.
         * </ul>
         * @param f a {@code float} value
         * @return the unbiased exponent of the argument
         * @since 1.6
         */
        public static int getExponent(this float f)
        {
            /*
             * Bitwise convert f to integer, mask out exponent bits, shift
             * to the right and then subtract out float's bias adjust to
             * get true exponent value
             */
            return ((f.ToRawIntBits() & FloatConsts.EXP_BIT_MASK) >>
                (FloatConsts.SIGNIFICAND_WIDTH - 1)) - FloatConsts.EXP_BIAS;
        }

        /**
         * Returns the unbiased exponent used in the representation of a
         * {@code double}.  Special cases:
         *
         * <ul>
         * <li>If the argument is NaN or infinite, then the result is
         * {@link Double#MAX_EXPONENT} + 1.
         * <li>If the argument is zero or subnormal, then the result is
         * {@link Double#MIN_EXPONENT} -1.
         * </ul>
         * @param d a {@code double} value
         * @return the unbiased exponent of the argument
         * @since 1.6
         */
        public static int getExponent(this double d)
        {
            /*
             * Bitwise convert d to long, mask out exponent bits, shift
             * to the right and then subtract out double's bias adjust to
             * get true exponent value.
             */
            return (int)(((d.ToLongBits() & DoubleConsts.EXP_BIT_MASK) >>
                          (DoubleConsts.SIGNIFICAND_WIDTH - 1)) - DoubleConsts.EXP_BIAS);
        }

        /**
         * Returns the size of an ulp of the argument.  An ulp, unit in
         * the last place, of a {@code double} value is the positive
         * distance between this floating-point value and the {@code
         * double} value next larger in magnitude.  Note that for non-NaN
         * <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, then the result is NaN.
         * <li> If the argument is positive or negative infinity, then the
         * result is positive infinity.
         * <li> If the argument is positive or negative zero, then the result is
         * {@code Double.MIN_VALUE}.
         * <li> If the argument is &plusmn;{@code Double.MAX_VALUE}, then
         * the result is equal to 2<sup>971</sup>.
         * </ul>
         *
         * @param d the floating-point value whose ulp is to be returned
         * @return the size of an ulp of the argument
         * @author Joseph D. Darcy
         * @since 1.5
         */
        public static double ulp(double d)
        {
            int exp = getExponent(d);

            switch (exp)
            {
                case DoubleConsts.MAX_EXPONENT + 1:       // NaN or infinity
                    return Math.Abs(d);

                case DoubleConsts.MIN_EXPONENT - 1:       // zero or subnormal
                    return DoubleConsts.MIN_VALUE;

                default:
                    if (!(exp <= DoubleConsts.MAX_EXPONENT && exp >= DoubleConsts.MIN_EXPONENT))
                    {
                        throw new ArithmeticException();
                    }

                    // ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
                    exp = exp - (DoubleConsts.SIGNIFICAND_WIDTH - 1);
                    if (exp >= DoubleConsts.MIN_EXPONENT)
                    {
                        return powerOfTwoD(exp);
                    }
                    else
                    {
                        // return a subnormal result; left shift integer
                        // representation of Double.MIN_VALUE appropriate
                        // number of positions
                        return (1L <<
                        (exp - (DoubleConsts.MIN_EXPONENT - (DoubleConsts.SIGNIFICAND_WIDTH - 1))))
                            .BitsToDouble();
                    }
            }
        }

        /**
         * Returns the size of an ulp of the argument.  An ulp, unit in
         * the last place, of a {@code float} value is the positive
         * distance between this floating-point value and the {@code
         * float} value next larger in magnitude.  Note that for non-NaN
         * <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, then the result is NaN.
         * <li> If the argument is positive or negative infinity, then the
         * result is positive infinity.
         * <li> If the argument is positive or negative zero, then the result is
         * {@code Float.MIN_VALUE}.
         * <li> If the argument is &plusmn;{@code Float.MAX_VALUE}, then
         * the result is equal to 2<sup>104</sup>.
         * </ul>
         *
         * @param f the floating-point value whose ulp is to be returned
         * @return the size of an ulp of the argument
         * @author Joseph D. Darcy
         * @since 1.5
         */
        public static float ulp(this float f)
        {
            int exp = getExponent(f);

            switch (exp)
            {
                case FloatConsts.MAX_EXPONENT + 1:        // NaN or infinity
                    return Math.Abs(f);

                case FloatConsts.MIN_EXPONENT - 1:        // zero or subnormal
                    return FloatConsts.MIN_VALUE;

                default:
                    if (!(exp <= FloatConsts.MAX_EXPONENT && exp >= FloatConsts.MIN_EXPONENT))
                    {
                        throw new ArithmeticException();
                    }

                    // ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
                    exp = exp - (FloatConsts.SIGNIFICAND_WIDTH - 1);
                    if (exp >= FloatConsts.MIN_EXPONENT)
                    {
                        return powerOfTwoF(exp);
                    }
                    else
                    {
                        // return a subnormal result; left shift integer
                        // representation of FloatConsts.MIN_VALUE appropriate
                        // number of positions
                        return (1 <<
                        (exp - (FloatConsts.MIN_EXPONENT - (FloatConsts.SIGNIFICAND_WIDTH - 1))))
                            .BitsToFloat();
                    }
            }
        }

        /**
         * Returns the signum function of the argument; zero if the argument
         * is zero, 1.0 if the argument is greater than zero, -1.0 if the
         * argument is less than zero.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, then the result is NaN.
         * <li> If the argument is positive zero or negative zero, then the
         *      result is the same as the argument.
         * </ul>
         *
         * @param d the floating-point value whose signum is to be returned
         * @return the signum function of the argument
         * @author Joseph D. Darcy
         * @since 1.5
         */
        public static double signum(double d)
        {
            return (d == 0.0 || double.IsNaN(d)) ? d : copySign(1.0, d);
        }

        /**
         * Returns the signum function of the argument; zero if the argument
         * is zero, 1.0f if the argument is greater than zero, -1.0f if the
         * argument is less than zero.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, then the result is NaN.
         * <li> If the argument is positive zero or negative zero, then the
         *      result is the same as the argument.
         * </ul>
         *
         * @param f the floating-point value whose signum is to be returned
         * @return the signum function of the argument
         * @author Joseph D. Darcy
         * @since 1.5
         */
        public static float signum(float f)
        {
            return (f == 0.0f || float.IsNaN(f)) ? f : copySign(1.0f, f);
        }
        /**
         * Returns the floating-point number adjacent to the first
         * argument in the direction of the second argument.  If both
         * arguments compare as equal the second argument is returned.
         *
         * <p>
         * Special cases:
         * <ul>
         * <li> If either argument is a NaN, then NaN is returned.
         *
         * <li> If both arguments are signed zeros, {@code direction}
         * is returned unchanged (as implied by the requirement of
         * returning the second argument if the arguments compare as
         * equal).
         *
         * <li> If {@code start} is
         * &plusmn;{@link Double#MIN_VALUE} and {@code direction}
         * has a value such that the result should have a smaller
         * magnitude, then a zero with the same sign as {@code start}
         * is returned.
         *
         * <li> If {@code start} is infinite and
         * {@code direction} has a value such that the result should
         * have a smaller magnitude, {@link Double#MAX_VALUE} with the
         * same sign as {@code start} is returned.
         *
         * <li> If {@code start} is equal to &plusmn;
         * {@link Double#MAX_VALUE} and {@code direction} has a
         * value such that the result should have a larger magnitude, an
         * infinity with same sign as {@code start} is returned.
         * </ul>
         *
         * @param start  starting floating-point value
         * @param direction value indicating which of
         * {@code start}'s neighbors or {@code start} should
         * be returned
         * @return The floating-point number adjacent to {@code start} in the
         * direction of {@code direction}.
         * @since 1.6
         */
        public static double nextAfter(double start, double direction)
        {
            /*
             * The cases:
             *
             * nextAfter(+infinity, 0)  == MAX_VALUE
             * nextAfter(+infinity, +infinity)  == +infinity
             * nextAfter(-infinity, 0)  == -MAX_VALUE
             * nextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            /*
             * IEEE 754 floating-point numbers are lexicographically
             * ordered if treated as signed-magnitude integers.
             * Since Java's integers are two's complement,
             * incrementing the two's complement representation of a
             * logically negative floating-point value *decrements*
             * the signed-magnitude representation. Therefore, when
             * the integer representation of a floating-point value
             * is negative, the adjustment to the representation is in
             * the opposite direction from what would initially be expected.
             */

            // Branch to descending case first as it is more costly than ascending
            // case due to start != 0.0d conditional.
            if (start > direction)
            { // descending
                if (start != 0.0d)
                {
                    long transducer = start.ToRawLongBits();
                    return (transducer + ((transducer > 0L) ? -1L : 1L)).BitsToDouble();
                }
                else
                { // start == 0.0d && direction < 0.0d
                    return -DoubleConsts.MIN_VALUE;
                }
            }
            else if (start < direction)
            { // ascending
              // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
              // then bitwise convert start to integer.
                long transducer = (start + 0.0d).ToRawLongBits();
                return (transducer + ((transducer >= 0L) ? 1L : -1L)).BitsToDouble();
            }
            else if (start == direction)
            {
                return direction;
            }
            else
            { // isNaN(start) || isNaN(direction)
                return start + direction;
            }
        }

        /**
         * Returns the floating-point number adjacent to the first
         * argument in the direction of the second argument.  If both
         * arguments compare as equal a value equivalent to the second argument
         * is returned.
         *
         * <p>
         * Special cases:
         * <ul>
         * <li> If either argument is a NaN, then NaN is returned.
         *
         * <li> If both arguments are signed zeros, a value equivalent
         * to {@code direction} is returned.
         *
         * <li> If {@code start} is
         * &plusmn;{@link Float#MIN_VALUE} and {@code direction}
         * has a value such that the result should have a smaller
         * magnitude, then a zero with the same sign as {@code start}
         * is returned.
         *
         * <li> If {@code start} is infinite and
         * {@code direction} has a value such that the result should
         * have a smaller magnitude, {@link Float#MAX_VALUE} with the
         * same sign as {@code start} is returned.
         *
         * <li> If {@code start} is equal to &plusmn;
         * {@link Float#MAX_VALUE} and {@code direction} has a
         * value such that the result should have a larger magnitude, an
         * infinity with same sign as {@code start} is returned.
         * </ul>
         *
         * @param start  starting floating-point value
         * @param direction value indicating which of
         * {@code start}'s neighbors or {@code start} should
         * be returned
         * @return The floating-point number adjacent to {@code start} in the
         * direction of {@code direction}.
         * @since 1.6
         */
        public static float nextAfter(float start, double direction)
        {
            /*
             * The cases:
             *
             * nextAfter(+infinity, 0)  == MAX_VALUE
             * nextAfter(+infinity, +infinity)  == +infinity
             * nextAfter(-infinity, 0)  == -MAX_VALUE
             * nextAfter(-infinity, -infinity)  == -infinity
             *
             * are naturally handled without any additional testing
             */

            /*
             * IEEE 754 floating-point numbers are lexicographically
             * ordered if treated as signed-magnitude integers.
             * Since Java's integers are two's complement,
             * incrementing the two's complement representation of a
             * logically negative floating-point value *decrements*
             * the signed-magnitude representation. Therefore, when
             * the integer representation of a floating-point value
             * is negative, the adjustment to the representation is in
             * the opposite direction from what would initially be expected.
             */

            // Branch to descending case first as it is more costly than ascending
            // case due to start != 0.0f conditional.
            if (start > direction)
            { // descending
                if (start != 0.0f)
                {
                    int transducer = start.ToRawIntBits();
                    return (transducer + ((transducer > 0) ? -1 : 1)).BitsToFloat();
                }
                else
                { // start == 0.0f && direction < 0.0f
                    return -FloatConsts.MIN_VALUE;
                }
            }
            else if (start < direction)
            { // ascending
              // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0)
              // then bitwise convert start to integer.
                int transducer = (start + 0.0f).ToRawIntBits();
                return (transducer + ((transducer >= 0) ? 1 : -1)).BitsToFloat();
            }
            else if (start == direction)
            {
                return (float)direction;
            }
            else
            { // isNaN(start) || isNaN(direction)
                return start + (float)direction;
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code d} in
         * the direction of positive infinity.  This method is
         * semantically equivalent to {@code nextAfter(d,
         * Double.POSITIVE_INFINITY)}; however, a {@code nextUp}
         * implementation may run faster than its equivalent
         * {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is positive infinity, the result is
         * positive infinity.
         *
         * <li> If the argument is zero, the result is
         * {@link Double#MIN_VALUE}
         *
         * </ul>
         *
         * @param d starting floating-point value
         * @return The adjacent floating-point value closer to positive
         * infinity.
         * @since 1.6
         */
        public static double nextUp(double d)
        {
            // Use a single conditional and handle the likely cases first.
            if (d < DoubleConsts.POSITIVE_INFINITY)
            {
                // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0).
                long transducer = (d + 0.0D).ToRawLongBits();
                return (transducer + ((transducer >= 0L) ? 1L : -1L)).BitsToDouble();
            }
            else
            { // d is NaN or +Infinity
                return d;
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code f} in
         * the direction of positive infinity.  This method is
         * semantically equivalent to {@code nextAfter(f,
         * Float.POSITIVE_INFINITY)}; however, a {@code nextUp}
         * implementation may run faster than its equivalent
         * {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is positive infinity, the result is
         * positive infinity.
         *
         * <li> If the argument is zero, the result is
         * {@link Float#MIN_VALUE}
         *
         * </ul>
         *
         * @param f starting floating-point value
         * @return The adjacent floating-point value closer to positive
         * infinity.
         * @since 1.6
         */
        public static float nextUp(float f)
        {
            // Use a single conditional and handle the likely cases first.
            if (f < FloatConsts.POSITIVE_INFINITY)
            {
                // Add +0.0 to get rid of a -0.0 (+0.0 + -0.0 => +0.0).
                int transducer = (f + 0.0F).ToRawIntBits();
                return (transducer + ((transducer >= 0) ? 1 : -1)).BitsToFloat();
            }
            else
            { // f is NaN or +Infinity
                return f;
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code d} in
         * the direction of negative infinity.  This method is
         * semantically equivalent to {@code nextAfter(d,
         * Double.NEGATIVE_INFINITY)}; however, a
         * {@code nextDown} implementation may run faster than its
         * equivalent {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is negative infinity, the result is
         * negative infinity.
         *
         * <li> If the argument is zero, the result is
         * {@code -Double.MIN_VALUE}
         *
         * </ul>
         *
         * @param d  starting floating-point value
         * @return The adjacent floating-point value closer to negative
         * infinity.
         * @since 1.8
         */
        public static double nextDown(double d)
        {
            if (double.IsNaN(d) || d == DoubleConsts.NEGATIVE_INFINITY)
                return d;
            else
            {
                if (d == 0.0)
                    return -DoubleConsts.MIN_VALUE;
                else
                    return (d.ToRawLongBits() + ((d > 0.0d) ? -1L : +1L)).BitsToDouble();
            }
        }

        /**
         * Returns the floating-point value adjacent to {@code f} in
         * the direction of negative infinity.  This method is
         * semantically equivalent to {@code nextAfter(f,
         * Float.NEGATIVE_INFINITY)}; however, a
         * {@code nextDown} implementation may run faster than its
         * equivalent {@code nextAfter} call.
         *
         * <p>Special Cases:
         * <ul>
         * <li> If the argument is NaN, the result is NaN.
         *
         * <li> If the argument is negative infinity, the result is
         * negative infinity.
         *
         * <li> If the argument is zero, the result is
         * {@code -Float.MIN_VALUE}
         *
         * </ul>
         *
         * @param f  starting floating-point value
         * @return The adjacent floating-point value closer to negative
         * infinity.
         * @since 1.8
         */
        public static float nextDown(float f)
        {
            if (float.IsNaN(f) || f == FloatConsts.NEGATIVE_INFINITY)
                return f;
            else
            {
                if (f == 0.0f)
                    return -FloatConsts.MIN_VALUE;
                else
                    return (f.ToRawIntBits() + ((f > 0.0f) ? -1 : +1)).BitsToFloat();
            }
        }

        /**
         * Returns {@code d} &times;
         * 2<sup>{@code scaleFactor}</sup> rounded as if performed
         * by a single correctly rounded floating-point multiply to a
         * member of the double value set.  See the Java
         * Language Specification for a discussion of floating-point
         * value sets.  If the exponent of the result is between {@link
         * Double#MIN_EXPONENT} and {@link Double#MAX_EXPONENT}, the
         * answer is calculated exactly.  If the exponent of the result
         * would be larger than {@code Double.MAX_EXPONENT}, an
         * infinity is returned.  Note that if the result is subnormal,
         * precision may be lost; that is, when {@code scalb(x, n)}
         * is subnormal, {@code scalb(scalb(x, n), -n)} may not equal
         * <i>x</i>.  When the result is non-NaN, the result has the same
         * sign as {@code d}.
         *
         * <p>Special cases:
         * <ul>
         * <li> If the first argument is NaN, NaN is returned.
         * <li> If the first argument is infinite, then an infinity of the
         * same sign is returned.
         * <li> If the first argument is zero, then a zero of the same
         * sign is returned.
         * </ul>
         *
         * @param d number to be scaled by a power of two.
         * @param scaleFactor power of 2 used to scale {@code d}
         * @return {@code d} &times; 2<sup>{@code scaleFactor}</sup>
         * @since 1.6
         */
        public static double scalb(double d, int scaleFactor)
        {
            /*
             * This method does not need to be declared strictfp to
             * compute the same correct result on all platforms.  When
             * scaling up, it does not matter what order the
             * multiply-store operations are done; the result will be
             * finite or overflow regardless of the operation ordering.
             * However, to get the correct result when scaling down, a
             * particular ordering must be used.
             *
             * When scaling down, the multiply-store operations are
             * sequenced so that it is not possible for two consecutive
             * multiply-stores to return subnormal results.  If one
             * multiply-store result is subnormal, the next multiply will
             * round it away to zero.  This is done by first multiplying
             * by 2 ^ (scaleFactor % n) and then multiplying several
             * times by 2^n as needed where n is the exponent of number
             * that is a covenient power of two.  In this way, at most one
             * real rounding error occurs.  If the double value set is
             * being used exclusively, the rounding will occur on a
             * multiply.  If the double-extended-exponent value set is
             * being used, the products will (perhaps) be exact but the
             * stores to d are guaranteed to round to the double value
             * set.
             *
             * It is _not_ a valid implementation to first multiply d by
             * 2^MIN_EXPONENT and then by 2 ^ (scaleFactor %
             * MIN_EXPONENT) since even in a strictfp program double
             * rounding on underflow could occur; e.g. if the scaleFactor
             * argument was (MIN_EXPONENT - n) and the exponent of d was a
             * little less than -(MIN_EXPONENT - n), meaning the final
             * result would be subnormal.
             *
             * Since exact reproducibility of this method can be achieved
             * without any undue performance burden, there is no
             * compelling reason to allow double rounding on underflow in
             * scalb.
             */

            // magnitude of a power of two so large that scaling a finite
            // nonzero value by it would be guaranteed to over or
            // underflow; due to rounding, scaling down takes an
            // additional power of two which is reflected here
            int MAX_SCALE = DoubleConsts.MAX_EXPONENT + -DoubleConsts.MIN_EXPONENT +
                                  DoubleConsts.SIGNIFICAND_WIDTH + 1;
            int exp_adjust = 0;
            int scale_increment = 0;
            double exp_delta = double.NaN;

            // Make sure scaling factor is in a reasonable range

            if (scaleFactor < 0)
            {
                scaleFactor = Math.Max(scaleFactor, -MAX_SCALE);
                scale_increment = -512;
                exp_delta = twoToTheDoubleScaleDown;
            }
            else
            {
                scaleFactor = Math.Min(scaleFactor, MAX_SCALE);
                scale_increment = 512;
                exp_delta = twoToTheDoubleScaleUp;
            }

            // Calculate (scaleFactor % +/-512), 512 = 2^9, using
            // technique from "Hacker's Delight" section 10-2.
            int t = (scaleFactor >> 9 - 1).UnsignedShift(32 - 9);
            exp_adjust = ((scaleFactor + t) & (512 - 1)) - t;

            d *= powerOfTwoD(exp_adjust);
            scaleFactor -= exp_adjust;

            while (scaleFactor != 0)
            {
                d *= exp_delta;
                scaleFactor -= scale_increment;
            }
            return d;
        }

        /**
         * Returns {@code f} &times;
         * 2<sup>{@code scaleFactor}</sup> rounded as if performed
         * by a single correctly rounded floating-point multiply to a
         * member of the float value set.  See the Java
         * Language Specification for a discussion of floating-point
         * value sets.  If the exponent of the result is between {@link
         * Float#MIN_EXPONENT} and {@link Float#MAX_EXPONENT}, the
         * answer is calculated exactly.  If the exponent of the result
         * would be larger than {@code Float.MAX_EXPONENT}, an
         * infinity is returned.  Note that if the result is subnormal,
         * precision may be lost; that is, when {@code scalb(x, n)}
         * is subnormal, {@code scalb(scalb(x, n), -n)} may not equal
         * <i>x</i>.  When the result is non-NaN, the result has the same
         * sign as {@code f}.
         *
         * <p>Special cases:
         * <ul>
         * <li> If the first argument is NaN, NaN is returned.
         * <li> If the first argument is infinite, then an infinity of the
         * same sign is returned.
         * <li> If the first argument is zero, then a zero of the same
         * sign is returned.
         * </ul>
         *
         * @param f number to be scaled by a power of two.
         * @param scaleFactor power of 2 used to scale {@code f}
         * @return {@code f} &times; 2<sup>{@code scaleFactor}</sup>
         * @since 1.6
         */
        public static float scalb(float f, int scaleFactor)
        {
            // magnitude of a power of two so large that scaling a finite
            // nonzero value by it would be guaranteed to over or
            // underflow; due to rounding, scaling down takes an
            // additional power of two which is reflected here
            int MAX_SCALE = FloatConsts.MAX_EXPONENT + -FloatConsts.MIN_EXPONENT +
                                  FloatConsts.SIGNIFICAND_WIDTH + 1;

            // Make sure scaling factor is in a reasonable range
            scaleFactor = Math.Max(Math.Min(scaleFactor, MAX_SCALE), -MAX_SCALE);

            /*
             * Since + MAX_SCALE for float fits well within the double
             * exponent range and + float -> double conversion is exact
             * the multiplication below will be exact. Therefore, the
             * rounding that occurs when the double product is cast to
             * float will be the correctly rounded float result.  Since
             * all operations other than the multiply will be exact,
             * it is not necessary to declare this method strictfp.
             */
            return (float)((double)f * powerOfTwoD(scaleFactor));
        }

        // Constants used in scalb
        static double twoToTheDoubleScaleUp = powerOfTwoD(512);
        static double twoToTheDoubleScaleDown = powerOfTwoD(-512);

        /**
         * Returns a floating-point power of two in the normal range.
         */
        static double powerOfTwoD(int n)
        {
            if (!(n >= DoubleConsts.MIN_EXPONENT && n <= DoubleConsts.MAX_EXPONENT))
            {
                throw new ArithmeticException();
            }
            return ((((long)n + (long)DoubleConsts.EXP_BIAS) <<
                        (DoubleConsts.SIGNIFICAND_WIDTH - 1))
                        & DoubleConsts.EXP_BIT_MASK).BitsToDouble();
        }

        /**
         * Returns a floating-point power of two in the normal range.
         */
        public static float powerOfTwoF(int n)
        {
            if (!(n >= FloatConsts.MIN_EXPONENT && n <= FloatConsts.MAX_EXPONENT))
            {
                throw new ArithmeticException();
            }
            return (((n + FloatConsts.EXP_BIAS) <<
                        (FloatConsts.SIGNIFICAND_WIDTH - 1))
                    & FloatConsts.EXP_BIT_MASK).BitsToFloat();
        }

        /// <summary>
        /// Floating Point Notation for E
        /// <br></br>
        /// <br></br>
        /// 164e64
        /// <br></br>
        /// <br></br>
        /// part_before_deminal_point = 0x164
        /// <br></br>
        /// part_after_decimal_point = 0
        /// <br></br>
        /// number_of_digits_in_part_after_decimal_point = 0
        /// <br></br>
        /// exponent = 64
        /// <br></br>
        /// <br></br>
        /// use FPN_E(part_before_deminal_point, exponent) instead as it avoids the computation of + 0 * Math.Pow(10, -0) which is equivilant to + 0
        /// <br></br>
        /// <br></br>
        /// <br></br>
        /// <br></br>
        /// 164.05e64
        /// <br></br>
        /// <br></br>
        /// part_before_deminal_point = 0x164
        /// <br></br>
        /// part_after_decimal_point = 5
        /// <br></br>
        /// number_of_digits_in_part_after_decimal_point = 2
        /// <br></br>
        /// exponent = 64
        /// </summary>
        public static double FPN_E(int part_before_deminal_point,
                    int part_after_decimal_point,
                    int number_of_digits_in_part_after_decimal_point,
                    int exponent)
        {
            return (part_before_deminal_point + part_after_decimal_point * Math.Pow(10, -number_of_digits_in_part_after_decimal_point)) * Math.Pow(10, exponent);
        }

        /// <summary>
        /// Floating Point Notation for E
        /// <br></br>
        /// <br></br>
        /// 164e64
        /// <br></br>
        /// whole number = 0x164
        /// <br></br>
        /// exponent = 64
        /// </summary>
        public static double FPN_E(int whole_number, int exponent)
        {
            return whole_number * Math.Pow(10, exponent);
        }

        /// <summary>
        /// Floating Point Notation for P
        /// <br></br>
        /// <br></br>
        /// 164p64
        /// <br></br>
        /// <br></br>
        /// part_before_deminal_point = 0x164
        /// <br></br>
        /// part_after_decimal_point = 0
        /// <br></br>
        /// number_of_digits_in_part_after_decimal_point = 0
        /// <br></br>
        /// power = 64
        /// <br></br>
        /// <br></br>
        /// use FPN_P(part_before_deminal_point, power) instead as it avoids the computation of + scalb(0, -4 * 0) which is equivilant to + 0
        /// <br></br>
        /// <br></br>
        /// <br></br>
        /// <br></br>
        /// <br></br>
        /// 164.05p64
        /// <br></br>
        /// <br></br>
        /// part_before_deminal_point = 0x164
        /// <br></br>
        /// part_after_decimal_point = 5
        /// <br></br>
        /// number_of_digits_in_part_after_decimal_point = 2
        /// <br></br>
        /// power = 64
        /// </summary>
        public static double FPN_P(int part_before_deminal_point,
                    int part_after_decimal_point,
                    int number_of_digits_in_part_after_decimal_point,
                    int power)
        {
            return scalb(part_before_deminal_point + scalb(part_after_decimal_point, -4 * number_of_digits_in_part_after_decimal_point), power);
        }

        /// <summary>
        /// Floating Point Notation for P
        /// <br></br>
        /// <br></br>
        /// 164p64
        /// <br></br>
        /// <br></br>
        /// whole number = 0x164
        /// <br></br>
        /// power = 64
        /// </summary>
        public static double FPN_P(int whole_number, int power)
        {
            return scalb(whole_number, power);
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct FLOAT_UINT
        {
            [System.Runtime.InteropServices.FieldOffset(0)] public float f;
            [System.Runtime.InteropServices.FieldOffset(0)] public uint i;

            public static implicit operator FLOAT_UINT(float v)
            {
                FLOAT_UINT tmp = new FLOAT_UINT();
                tmp.f = v;
                return tmp;
            }

            public static implicit operator FLOAT_UINT(uint v)
            {
                FLOAT_UINT tmp = new FLOAT_UINT();
                tmp.i = v;
                return tmp;
            }
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
        public struct DOUBLE_ULONG
        {
            [System.Runtime.InteropServices.FieldOffset(0)] public double f;
            [System.Runtime.InteropServices.FieldOffset(0)] public ulong i;

            public static implicit operator DOUBLE_ULONG(double v)
            {
                DOUBLE_ULONG tmp = new DOUBLE_ULONG();
                tmp.f = v;
                return tmp;
            }

            public static implicit operator DOUBLE_ULONG(ulong v)
            {
                DOUBLE_ULONG tmp = new DOUBLE_ULONG();
                tmp.i = v;
                return tmp;
            }
        }

        private static class MUSL
        {
            /* origin: FreeBSD /usr/src/lib/msun/src/s_cbrt.c */
            /*
             * ====================================================
             * Copyright (C) 1993 by Sun Microsystems, Inc. All rights reserved.
             *
             * Developed at SunPro, a Sun Microsystems, Inc. business.
             * Permission to use, copy, modify, and distribute this
             * software is freely granted, provided that this notice
             * is preserved.
             * ====================================================
             *
             * Optimized by Bruce D. Evans.
             */
            /* cbrt(x)
             * Return cube root of x
             */
            const uint
            B1 = 715094163, /* B1 = (1023-1023/3-0.03306235651)*2**20 */
            B2 = 696219795; /* B2 = (1023-1023/3-54/3-0.03306235651)*2**20 */

            /* |1/cbrt(x) - p(x)| < 2**-23.5 (~[-7.93e-8, 7.929e-8]). */
            const double
            P0 = 1.87595182427177009643,  /* 0x3ffe03e6, 0x0f61e692 */
            P1 = -1.88497979543377169875,  /* 0xbffe28e0, 0x92f02420 */
            P2 = 1.621429720105354466140, /* 0x3ff9f160, 0x4a49d6c2 */
            P3 = -0.758397934778766047437, /* 0xbfe844cb, 0xbee751d9 */
            P4 = 0.145996192886612446982; /* 0x3fc2b000, 0xd4e4edd7 */

            public static double cbrt(double x)
            {
                DOUBLE_ULONG u = x;
                double r, s, t, w;
                uint hx = reinterpret_cast<uint>(u.i >> 32 & 0x7fffffff);

                if (hx >= 0x7ff00000)  /* cbrt(NaN,INF) is itself */
                    return x + x;

                /*
                 * Rough cbrt to 5 bits:
                 *    cbrt(2**e*(1+m) ~= 2**(e/3)*(1+(e%3+m)/3)
                 * where e is integral and >= 0, m is real and in [0, 1), and "/" and
                 * "%" are integer division and modulus with rounding towards minus
                 * infinity.  The RHS is always >= the LHS and has a maximum relative
                 * error of about 1 in 16.  Adding a bias of -0.03306235651 to the
                 * (e%3+m)/3 term reduces the error to about 1 in 32. With the IEEE
                 * floating point representation, for finite positive normal values,
                 * ordinary integer divison of the value in bits magically gives
                 * almost exactly the RHS of the above provided we first subtract the
                 * exponent bias (1023 for doubles) and later add it back.  We do the
                 * subtraction virtually to keep e >= 0 so that ordinary integer
                 * division rounds towards minus infinity; this is also efficient.
                 */
                if (hx < 0x00100000)
                { /* zero or subnormal? */
                    u.f = x * FPN_P(0x1, 54);
                    hx = (uint)(u.i >> 32 & 0x7fffffff);
                    if (hx == 0)
                        return x;  /* cbrt(0) is itself */
                    hx = hx / 3 + B2;
                }
                else
                    hx = hx / 3 + B1;
                u.i &= 1UL << 63;
                u.i |= (ulong)hx << 32;
                t = u.f;

                /*
                 * New cbrt to 23 bits:
                 *    cbrt(x) = t*cbrt(x/t**3) ~= t*P(t**3/x)
                 * where P(r) is a polynomial of degree 4 that approximates 1/cbrt(r)
                 * to within 2**-23.5 when |r - 1| < 1/10.  The rough approximation
                 * has produced t such than |t/cbrt(x) - 1| ~< 1/32, and cubing this
                 * gives us bounds for r = t**3/x.
                 *
                 * Try to optimize for parallel evaluation as in __tanf.c.
                 */
                r = (t * t) * (t / x);
                t = t * ((P0 + r * (P1 + r * P2)) + ((r * r) * r) * (P3 + r * P4));

                /*
                 * Round t away from zero to 23 bits (sloppily except for ensuring that
                 * the result is larger in magnitude than cbrt(x) but not much more than
                 * 2 23-bit ulps larger).  With rounding towards zero, the error bound
                 * would be ~5/6 instead of ~4/6.  With a maximum error of 2 23-bit ulps
                 * in the rounded t, the infinite-precision error in the Newton
                 * approximation barely affects third digit in the final error
                 * 0.667; the error in the rounded t can be up to about 3 23-bit ulps
                 * before the final error is larger than 0.667 ulps.
                 */
                u.f = t;
                u.i = (u.i + 0x80000000) & 0xffffffffc0000000UL;
                t = u.f;

                /* one step Newton iteration to 53 bits with error < 0.667 ulps */
                s = t * t;         /* t*t is exact */
                r = x / s;         /* error <= 0.5 ulps; |r| < |t| */
                w = t + t;         /* t+t is exact */
                r = (r - t) / (w + r); /* r-t is exact; w+r ~= 3*t */
                t = t + t * r;       /* error <= 0.5 + 0.5/3 + epsilon */
                return t;
            }

            static uint mul32(uint a, uint b)
            {
                return (uint)a * b >> 32;
            }

            static float __math_invalidf(float x)
            {
                return (x - x) / (x - x);
            }

            const uint three = 0xc0000000;
            static readonly ushort[] __rsqrt_tab = {
                0xb451,0xb2f0,0xb196,0xb044,0xaef9,0xadb6,0xac79,0xab43,
                0xaa14,0xa8eb,0xa7c8,0xa6aa,0xa592,0xa480,0xa373,0xa26b,
                0xa168,0xa06a,0x9f70,0x9e7b,0x9d8a,0x9c9d,0x9bb5,0x9ad1,
                0x99f0,0x9913,0x983a,0x9765,0x9693,0x95c4,0x94f8,0x9430,
                0x936b,0x92a9,0x91ea,0x912e,0x9075,0x8fbe,0x8f0a,0x8e59,
                0x8daa,0x8cfe,0x8c54,0x8bac,0x8b07,0x8a64,0x89c4,0x8925,
                0x8889,0x87ee,0x8756,0x86c0,0x862b,0x8599,0x8508,0x8479,
                0x83ec,0x8361,0x82d8,0x8250,0x81c9,0x8145,0x80c2,0x8040,
                0xff02,0xfd0e,0xfb25,0xf947,0xf773,0xf5aa,0xf3ea,0xf234,
                0xf087,0xeee3,0xed47,0xebb3,0xea27,0xe8a3,0xe727,0xe5b2,
                0xe443,0xe2dc,0xe17a,0xe020,0xdecb,0xdd7d,0xdc34,0xdaf1,
                0xd9b3,0xd87b,0xd748,0xd61a,0xd4f1,0xd3cd,0xd2ad,0xd192,
                0xd07b,0xcf69,0xce5b,0xcd51,0xcc4a,0xcb48,0xca4a,0xc94f,
                0xc858,0xc764,0xc674,0xc587,0xc49d,0xc3b7,0xc2d4,0xc1f4,
                0xc116,0xc03c,0xbf65,0xbe90,0xbdbe,0xbcef,0xbc23,0xbb59,
                0xba91,0xb9cc,0xb90a,0xb84a,0xb78c,0xb6d0,0xb617,0xb560,
            };

            /* see sqrt.c for more detailed comments.  */

            public static float sqrtf(float x)
            {
                FLOAT_UINT u1 = x;
                uint ix, m, m1, m0, even, ey;

                ix = u1.i;
                if (ix - 0x00800000 >= 0x7f800000 - 0x00800000)
                {
                    /* x < 0x1p-126 or inf or nan.  */
                    if (ix * 2 == 0)
                        return x;
                    if (ix == 0x7f800000)
                        return x;
                    if (ix > 0x7f800000)
                        return __math_invalidf(x);
                    /* x is subnormal, normalize it.  */
                    u1.f = x * (float)FPN_P(0x1, 23);
                    ix = u1.i;
                    ix -= 23 << 23;
                }

                /* x = 4^e m; with int e and m in [1, 4).  */
                even = ix & 0x00800000;
                m1 = (ix << 8) | 0x80000000;
                m0 = (ix << 7) & 0x7fffffff;
                m = even.toBool() ? m0 : m1;

                /* 2^e is the exponent part of the return value.  */
                ey = ix >> 1;
                ey += 0x3f800000 >> 1;
                ey &= 0x7f800000;

                /* compute r ~ 1/sqrt(m), s ~ sqrt(m) with 2 goldschmidt iterations.  */
                uint r, s, d, u, i;
                i = (ix >> 17) % 128;
                r = (uint)__rsqrt_tab[i] << 16;
                /* |r*sqrt(m) - 1| < 0x1p-8 */
                s = mul32(m, r);
                /* |s/sqrt(m) - 1| < 0x1p-8 */
                d = mul32(s, r);
                u = three - d;
                r = mul32(r, u) << 1;
                /* |r*sqrt(m) - 1| < 0x1.7bp-16 */
                s = mul32(s, u) << 1;
                /* |s/sqrt(m) - 1| < 0x1.7bp-16 */
                d = mul32(s, r);
                u = three - d;
                s = mul32(s, u);
                /* -0x1.03p-28 < s/sqrt(m) - 1 < 0x1.fp-31 */
                s = (s - 1) >> 6;
                /* s < sqrt(m) < s + 0x1.08p-23 */

                /* compute nearest rounded result.  */
                uint d0, d1, d2;
                float y, t;
                d0 = (m << 16) - s * s;
                d1 = s - d0;
                d2 = d1 + s + 1;
                s += d1 >> 31;
                s &= 0x007fffff;
                s |= ey;
                y = s.BitsToFloat();
                /* handle rounding and inexact exception. */
                uint tiny = d2 == 0 ? 0 : reinterpret_cast<uint>(0x01000000);
                tiny |= (d1 ^ d2) & 0x80000000;
                t = tiny.BitsToFloat();
                y = y + t;
                return y;
            }
        }

        public static double cbrt(double x) => MUSL.cbrt(x);
        public static float sqrtf(float x) => MUSL.sqrtf(x);
        public static double hypot(double x, double y) => HYPOT_IMP.hypot(x, y);
        public static float hypotf(float x, float y) => HYPOT_IMP.hypotf(x, y);
        public static double hypotl(double x, double y) => HYPOT_IMP.hypotl(x, y);

        private const float DEG_TO_RAD = 3.1415926f / 180.0f;
        private const float RAD_TO_DEG = 180.0f / 3.1415926f;

        public static float max(float a, float b, float c)
        {
            return a > b ? (a > c ? a : c) : (b > c ? b : c);
        }

        public static float max(int a, int b, int c)
        {
            return a > b ? (a > c ? a : c) : (b > c ? b : c);
        }

        public static float min(float a, float b, float c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
        }

        public static float min(int a, int b, int c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
        }

        public static float dist(float x1, float y1, float x2, float y2)
        {
            float x = (x2 - x1);
            float y = (y2 - y1);
            return (float)hypot(x, y);
        }

        public static float dist(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            float x = (x2 - x1);
            float y = (y2 - y1);
            float z = (z2 - z1);
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public static float mag(float a, float b)
        {
            return (float)hypot(a, b);
        }

        public static float mag(float a, float b, float c)
        {
            return (float)Math.Sqrt(a * a + b * b + c * c);
        }

        public static float sq(float v)
        {
            return v * v;
        }

        public static float dot(float v1x, float v1y, float v2x, float v2y)
        {
            return v1x * v2x + v1y * v2y;
        }

        public static float cross(float v1x, float v1y, float v2x, float v2y)
        {
            return v1x * v2y - v1y * v2x;
        }

        public static float radians(float degrees)
        {
            return degrees * DEG_TO_RAD;
        }

        public static float degrees(float radians)
        {
            return radians * RAD_TO_DEG;
        }

        public static float lerp(float start, float stop, float amount)
        {
            return start + (stop - start) * amount;
        }

        public static float lerp(int start, int stop, float amount)
        {
            return lerp((float)start, (float)stop, amount);
        }

        /**
         * Returns the interpolation scalar (s) that satisfies the equation: {@code value = }{@link
         * #lerp}{@code (a, b, s)}
         *
         * <p>If {@code a == b}, then this function will return 0.
         */
        public static float lerpInv(float a, float b, float value)
        {
            return a != b ? ((value - a) / (b - a)) : 0.0f;
        }

        /** Returns the single argument constrained between [0.0, 1.0]. */
        public static float saturate(float value)
        {
            return constrain(value, 0.0f, 1.0f);
        }

        /** Returns the saturated (constrained between [0, 1]) result of {@link #lerpInv}. */
        public static float lerpInvSat(float a, float b, float value)
        {
            return saturate(lerpInv(a, b, value));
        }

        /**
         * Returns an interpolated angle in degrees between a set of start and end
         * angles.
         * <p>
         * Unlike {@link #lerp(float, float, float)}, the direction and distance of
         * travel is determined by the shortest angle between the start and end
         * angles. For example, if the starting angle is 0 and the ending angle is
         * 350, then the interpolated angle will be in the range [0,-10] rather
         * than [0,350].
         *
         * @param start the starting angle in degrees
         * @param end the ending angle in degrees
         * @param amount the position between start and end in the range [0,1]
         *               where 0 is the starting angle and 1 is the ending angle
         * @return the interpolated angle in degrees
         */
        public static float lerpDeg(float start, float end, float amount)
        {
            float minAngle = (((end - start) + 180) % 360) - 180;
            return minAngle * amount + start;
        }

        public static float norm(float start, float stop, float value)
        {
            return (value - start) / (stop - start);
        }

        public static float map(float minStart, float minStop, float maxStart, float maxStop, float value)
        {
            return maxStart + (maxStop - maxStart) * ((value - minStart) / (minStop - minStart));
        }

        /**
         * Calculates a value in [rangeMin, rangeMax] that maps value in [valueMin, valueMax] to
         * returnVal in [rangeMin, rangeMax].
         * <p>
         * Always returns a constrained value in the range [rangeMin, rangeMax], even if value is
         * outside [valueMin, valueMax].
         * <p>
         * Eg:
         *    constrainedMap(0f, 100f, 0f, 1f, 0.5f) = 50f
         *    constrainedMap(20f, 200f, 10f, 20f, 20f) = 200f
         *    constrainedMap(20f, 200f, 10f, 20f, 50f) = 200f
         *    constrainedMap(10f, 50f, 10f, 20f, 5f) = 10f
         *
         * @param rangeMin minimum of the range that should be returned.
         * @param rangeMax maximum of the range that should be returned.
         * @param valueMin minimum of range to map {@code value} to.
         * @param valueMax maximum of range to map {@code value} to.
         * @param value to map to the range [{@code valueMin}, {@code valueMax}]. Note, can be outside
         *              this range, resulting in a clamped value.
         * @return the mapped value, constrained to [{@code rangeMin}, {@code rangeMax}.
         */
        public static float constrainedMap(
                float rangeMin, float rangeMax, float valueMin, float valueMax, float value)
        {
            return lerp(rangeMin, rangeMax, lerpInvSat(valueMin, valueMax, value));
        }

        /**
         * Perform Hermite interpolation between two values.
         * Eg:
         *   smoothStep(0, 0.5f, 0.5f) = 1f
         *   smoothStep(0, 0.5f, 0.25f) = 0.5f
         *
         * @param start Left edge.
         * @param end Right edge.
         * @param x A value between {@code start} and {@code end}.
         * @return A number between 0 and 1 representing where {@code x} is in the interpolation.
         */
        public static float smoothStep(float start, float end, float x)
        {
            return constrain((x - start) / (end - start), 0f, 1f);
        }

        /**
         * Returns the sum of the two parameters, or throws an exception if the resulting sum would
         * cause an overflow or underflow.
         * @throws IllegalArgumentException when overflow or underflow would occur.
         */
        public static int addOrThrow(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }

            if (b > 0 && a <= (int.MaxValue - b))
            {
                return a + b;
            }

            if (b < 0 && a >= (int.MinValue - b))
            {
                return a + b;
            }
            throw new IllegalArgumentException("Addition overflow: " + a + " + " + b);
        }

        /**
         * Resize a {@link Rect} so one size would be {@param largestSide}.
         *
         * @param outToResize Rectangle that will be resized.
         * @param largestSide Size of the largest side.
         */
        public static void fitRect(Rect outToResize, int largestSide)
        {
            if (outToResize.isEmpty())
            {
                return;
            }
            float maxSize = Math.Max(outToResize.width(), outToResize.height());
            outToResize.scale(largestSide / maxSize);
        }
    }
}