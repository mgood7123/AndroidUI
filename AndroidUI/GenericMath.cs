namespace AndroidUI
{
    /// <summary>
    /// math function that support generic types
    /// </summary>
    public static class GenericMath
    {
        static T cast1<T, intermediate_T>(Func<intermediate_T, intermediate_T> f, in T a)
        {
            return Union.ReinterpretCast<intermediate_T, T>(
                f.Invoke(
                    Union.ReinterpretCast<T, intermediate_T>(a)
                )
            );
        }

        static T cast2<T, intermediate_T>(Func<intermediate_T, intermediate_T, intermediate_T> f, in T a, in T b)
        {
            return Union.ReinterpretCast<intermediate_T, T>(
                f.Invoke(
                    Union.ReinterpretCast<T, intermediate_T>(a),
                    Union.ReinterpretCast<T, intermediate_T>(b)
                )
            );
        }

        public static T Abs<T>(T a)
        {
            Type requestedType = typeof(T);
            if (
                requestedType == CastUtils.USHORT ||
                requestedType == CastUtils.UINT ||
                requestedType == CastUtils.ULONG ||
                requestedType == CastUtils.BYTE
            )
            {
                return a;
            }
            else if (requestedType == CastUtils.SBYTE)
            {
                return cast1<T, sbyte>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.SHORT)
            {
                return cast1<T, short>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.CHAR)
            {
                return cast1<T, short>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.INT)
            {
                return cast1<T, int>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.FLOAT)
            {
                return cast1<T, float>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.LONG)
            {
                return cast1<T, long>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast1<T, double>(Math.Abs, a);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast1<T, decimal>(Math.Abs, a);
            }
            else
            {
                throw new InvalidCastException("T must be an Unmanaged Type excluding nint and nuint");
            }
        }

        static T FloatOrDouble__onlyTakesDouble<T>(Func<double, double> d, T a)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)d.Invoke(
                        (double)Union.ReinterpretCast<T, float>(a)
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast1(d, a);
            }
            else
            {
                throw new InvalidCastException("T must be Float or Double");
            }
        }

        static T FloatOrDoubleOrDecimal__onlyTakesDoubleOrDecimal<T>(Func<double, double> d, Func<decimal, decimal> dec, T a)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)d.Invoke(
                        (double)Union.ReinterpretCast<T, float>(a)
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast1(d, a);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast1(dec, a);
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Decimal");
            }
        }

        static T FloatOrDoubleOrDecimal<T>(Func<float, float> f, Func<double, double> d, Func<decimal, decimal> dec, T a)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return cast1(f, a);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast1(d, a);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast1(dec, a);
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Double");
            }
        }

        static T FloatOrDouble<T>(Func<float, float, float> f, Func<double, double, double> d, T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return cast2(f, a, b);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2(d, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be Float or Double");
            }
        }

        static T FloatOrDouble__onlyTakesDouble<T>(Func<double, double, double> d, T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)d.Invoke(
                        (double)Union.ReinterpretCast<T, float>(a),
                        (double)Union.ReinterpretCast<T, float>(b)
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2(d, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be Float or Double");
            }
        }

        static T FloatOrDoubleOrDecimal__onlyTakesDoubleOrDecimal<T>(Func<double, double, double> d, Func<decimal, decimal, decimal> dec, T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)d.Invoke(
                        (double)Union.ReinterpretCast<T, float>(a),
                        (double)Union.ReinterpretCast<T, float>(b)
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2(d, a, b);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast2(dec, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Decimal");
            }
        }

        static T FloatOrDoubleOrDecimal<T>(Func<float, float, float> f, Func<double, double, double> d, Func<decimal, decimal, decimal> dec, T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return cast2(f, a, b);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2(d, a, b);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast2(dec, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Double");
            }
        }

        public static T Acos<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Acos, a);
        public static T Asin<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Asin, a);
        public static T Atan<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Atan, a);
        public static T Atan2<T>(T a, T b) => FloatOrDouble__onlyTakesDouble(Math.Atan2, a, b);
        public static long BigMul<T>(T a, T b)
        {
            return Math.BigMul(Convert.ToInt32(a), Convert.ToInt32(b));
        }
        public static T Ceiling<T>(T a) => FloatOrDoubleOrDecimal__onlyTakesDoubleOrDecimal(Math.Ceiling, Math.Ceiling, a);
        public static T Cos<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Cos, a);
        public static T Cosh<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Cosh, a);
        public static T Exp<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Exp, a);
        public static int DivRem<T>(T a, T b, out int result)
        {
            int r;
            int x = Math.DivRem(Convert.ToInt32(a), Convert.ToInt32(b), out r);
            result = r;
            return x;

        }
        public static long DivRem<T>(T a, T b, out long result)
        {
            long r;
            long x = Math.DivRem(Convert.ToInt64(a), Convert.ToInt64(b), out r);
            result = r;
            return x;

        }
        public static T Floor<T>(T a) => FloatOrDoubleOrDecimal__onlyTakesDoubleOrDecimal(Math.Floor, Math.Floor, a);
        public static T IEEERemainder<T>(T a, T b) => FloatOrDouble__onlyTakesDouble(Math.IEEERemainder, a, b);
        public static T Log<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Log, a);
        public static T Log<T>(T a, T newBase) => FloatOrDouble__onlyTakesDouble(Math.Log, a, newBase);
        public static T Log10<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Log10, a);
        public static T Min<T>(T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.USHORT)
            {
                return cast2<T, ushort>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.UINT)
            {
                return cast2<T, uint>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.ULONG)
            {
                return cast2<T, ulong>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.BYTE)
            {
                return cast2<T, byte>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.SBYTE)
            {
                return cast2<T, sbyte>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.SHORT)
            {
                return cast2<T, short>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.CHAR)
            {
                return cast2<T, ushort>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.INT)
            {
                return cast2<T, int>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.FLOAT)
            {
                return cast2<T, float>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.LONG)
            {
                return cast2<T, long>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2<T, double>(Math.Min, a, b);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast2<T, decimal>(Math.Min, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be an Unmanaged Type excluding nint and nuint");
            }
        }
        public static T Max<T>(T a, T b)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.USHORT)
            {
                return cast2<T, ushort>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.UINT)
            {
                return cast2<T, uint>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.ULONG)
            {
                return cast2<T, ulong>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.BYTE)
            {
                return cast2<T, byte>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.SBYTE)
            {
                return cast2<T, sbyte>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.SHORT)
            {
                return cast2<T, short>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.CHAR)
            {
                return cast2<T, ushort>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.INT)
            {
                return cast2<T, int>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.FLOAT)
            {
                return cast2<T, float>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.LONG)
            {
                return cast2<T, long>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast2<T, double>(Math.Max, a, b);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast2<T, decimal>(Math.Max, a, b);
            }
            else
            {
                throw new InvalidCastException("T must be an Unmanaged Type excluding nint and nuint");
            }
        }
        public static T Pow<T>(T a, T power) => FloatOrDouble__onlyTakesDouble(Math.Pow, a, power);
        public static T Round<T>(T a)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)Math.Round(
                        (double)Union.ReinterpretCast<T, float>(a)
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return cast1<T, double>(Math.Round, a);
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return cast1<T, decimal>(Math.Round, a);
            }
            else
            {
                throw new InvalidCastException("T must be Float or Double");
            }
        }

        public static T Round<T>(T a, T decimals)
        {
            int decimals_ = Convert.ToInt32(decimals);
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)Math.Round(
                        (double)Union.ReinterpretCast<T, float>(a), decimals_
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return Union.ReinterpretCast<double, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, double>(a), decimals_
                    )
                );
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return Union.ReinterpretCast<decimal, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, decimal>(a), decimals_
                    )
                );
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Decimal");
            }
        }

        public static T Round_<T>(T a, T midpointRounding)
        {
            MidpointRounding midpointRounding_ = (MidpointRounding)Convert.ChangeType(midpointRounding, typeof(MidpointRounding));
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)Math.Round(
                        (double)Union.ReinterpretCast<T, float>(a), midpointRounding_
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return Union.ReinterpretCast<double, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, double>(a), midpointRounding_
                    )
                );
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return Union.ReinterpretCast<decimal, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, decimal>(a), midpointRounding_
                    )
                );
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Decimal");
            }
        }

        public static T Round<T>(T a, T decimals, T midpointRounding)
        {
            int decimals_ = Convert.ToInt32(decimals);
            MidpointRounding midpointRounding_ = (MidpointRounding)Convert.ChangeType(midpointRounding, typeof(MidpointRounding));
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.FLOAT)
            {
                return Union.ReinterpretCast<float, T>(
                    (float)Math.Round(
                        (double)Union.ReinterpretCast<T, float>(a), decimals_, midpointRounding_
                    )
                );
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return Union.ReinterpretCast<double, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, double>(a), decimals_, midpointRounding_
                    )
                );
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return Union.ReinterpretCast<decimal, T>(
                    Math.Round(
                        Union.ReinterpretCast<T, decimal>(a), decimals_, midpointRounding_
                    )
                );
            }
            else
            {
                throw new InvalidCastException("T must be Float, Double, or Decimal");
            }
        }

        public static T Sin<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Sin, a);

        public static int Sign<T>(T a)
        {
            Type requestedType = typeof(T);
            if (requestedType == CastUtils.USHORT)
            {
                return Math.Sign(Union.ReinterpretCast<T, ushort>(a));
            }
            else if (requestedType == CastUtils.UINT)
            {
                return Math.Sign(Union.ReinterpretCast<T, uint>(a));
            }
            else if (requestedType == CastUtils.ULONG)
            {
                return Math.Sign(Union.ReinterpretCast<T, long>(a));
            }
            else if (requestedType == CastUtils.BYTE)
            {
                return Math.Sign(Union.ReinterpretCast<T, byte>(a));
            }
            else if (requestedType == CastUtils.SBYTE)
            {
                return Math.Sign(Union.ReinterpretCast<T, sbyte>(a));
            }
            else if (requestedType == CastUtils.SHORT)
            {
                return Math.Sign(Union.ReinterpretCast<T, short>(a));
            }
            else if (requestedType == CastUtils.CHAR)
            {
                return Math.Sign(Union.ReinterpretCast<T, char>(a));
            }
            else if (requestedType == CastUtils.INT)
            {
                return Math.Sign(Union.ReinterpretCast<T, int>(a));
            }
            else if (requestedType == CastUtils.FLOAT)
            {
                return Math.Sign(Union.ReinterpretCast<T, float>(a));
            }
            else if (requestedType == CastUtils.LONG)
            {
                return Math.Sign(Union.ReinterpretCast<T, long>(a));
            }
            else if (requestedType == CastUtils.DOUBLE)
            {
                return Math.Sign(Union.ReinterpretCast<T, double>(a));
            }
            else if (requestedType == CastUtils.DECIMAL)
            {
                return Math.Sign(Union.ReinterpretCast<T, decimal>(a));
            }
            else
            {
                throw new InvalidCastException("T must be an Unmanaged Type excluding nint and nuint");
            }
        }
        public static T Sinh<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Sinh, a);
        public static T Sqrt<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Sqrt, a);
        public static T Tan<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Tan, a);
        public static T Tanh<T>(T a) => FloatOrDouble__onlyTakesDouble(Math.Tanh, a);
        public static T Truncate<T>(T a) => FloatOrDoubleOrDecimal__onlyTakesDoubleOrDecimal(Math.Truncate, Math.Truncate, a);
    }
}