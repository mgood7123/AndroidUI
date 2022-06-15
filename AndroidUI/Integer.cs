using System.Numerics;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace AndroidUI
{
    // https://sharplab.io/#gist:b2ca424611d820ad8aeb4ad14d763287
    public readonly struct Integer<T> : IEquatable<Integer<T>>, IComparable<Integer<T>> where T : unmanaged
    {
        readonly T _value;

        public static Integer<T> ConvertFrom<T1>(T1 value)
        {
            object? obj = Convert.ChangeType(value, typeof(T));

            return obj == null
                ? throw new InvalidCastException("could not convert T1 (" + typeof(T1).FullName + ") to T (" + typeof(T).FullName + ")")
                : (new((T)obj));
        }

        public T ConvertTo<T>()
        {
            object? obj = Convert.ChangeType(_value, typeof(T));

            return obj == null
                ? throw new InvalidCastException("could not convert " + _value.GetType().FullName + " to T (" + typeof(T).FullName + ")")
                : (T)obj;
        }

        public static Integer<T> MinValue => GetMinValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Integer<T> GetMinValue()
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef(sbyte.MinValue));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef(byte.MinValue));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef(short.MinValue));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef(ushort.MinValue));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef(char.MinValue));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(int.MinValue));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(uint.MinValue));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(long.MinValue));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(ulong.MinValue));
            else
                return GetMinValue2();

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static Integer<T> GetMinValue2()
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(nint.MinValue));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(nuint.MinValue));
                else
                    return THROW();
            }
        }

        public static Integer<T> MaxValue => GetMaxValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Integer<T> GetMaxValue()
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef(sbyte.MaxValue));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef(byte.MaxValue));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef(short.MaxValue));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef(ushort.MaxValue));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef(char.MaxValue));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(int.MaxValue));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(uint.MaxValue));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(long.MaxValue));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(ulong.MaxValue));
            else
                return GetMaxValue2();

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static Integer<T> GetMaxValue2()
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(nint.MaxValue));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(nuint.MaxValue));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> Parse(string s)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef(sbyte.Parse(s)));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef(byte.Parse(s)));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef(short.Parse(s)));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef(ushort.Parse(s)));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef(char.Parse(s)));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(int.Parse(s)));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(uint.Parse(s)));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(long.Parse(s)));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(ulong.Parse(s)));
            else
                return Parse2(s);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static Integer<T> Parse2(string s)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(nint.Parse(s)));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(nuint.Parse(s)));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(BigInteger.Parse(s)));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(string? s, out Integer<T> result)
        {
            if (typeof(T) == typeof(sbyte))
            {
                sbyte r;
                bool ret = sbyte.TryParse(s, out r);
                result = As<sbyte, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(byte))
            {
                byte r;
                bool ret = byte.TryParse(s, out r);
                result = As<byte, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(short))
            {
                short r;
                bool ret = short.TryParse(s, out r);
                result = As<short, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(ushort))
            {
                ushort r;
                bool ret = ushort.TryParse(s, out r);
                result = As<ushort, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(char))
            {
                char r;
                bool ret = char.TryParse(s, out r);
                result = As<char, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(int))
            {
                int r;
                bool ret = int.TryParse(s, out r);
                result = As<int, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(uint))
            {
                uint r;
                bool ret = uint.TryParse(s, out r);
                result = As<uint, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(long))
            {
                long r;
                bool ret = long.TryParse(s, out r);
                result = As<long, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(ulong))
            {
                ulong r;
                bool ret = ulong.TryParse(s, out r);
                result = As<ulong, T>(ref r);
                return ret;
            }
            else
                return TryParse2(s, out result);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool TryParse2(string? s, out Integer<T> result)
            {
                if (typeof(T) == typeof(nint)) {
                    nint r;
                    bool ret = nint.TryParse(s, out r);
                    result = As<nint, T>(ref r);
                    return ret;
                }
                else if (typeof(T) == typeof(nuint))
                {
                    nuint r;
                    bool ret = nuint.TryParse(s, out r);
                    result = As<nuint, T>(ref r);
                    return ret;
                }
                else if (typeof(T) == typeof(BigInteger))
                {
                    BigInteger r;
                    bool ret = BigInteger.TryParse(s, out r);
                    result = As<BigInteger, T>(ref r);
                    return ret;
                }
                else
                {
                    result = default;
                    return THROW_BOOL();
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Integer<T>) return Equals((Integer<T>)obj);
            else if (obj is T) return Equals((T)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T b) => Equals(_value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Integer<T> b) => Equals(_value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Equals(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a).Equals(As<T, sbyte>(ref b));
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a).Equals(As<T, byte>(ref b));
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a).Equals(As<T, short>(ref b));
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a).Equals(As<T, ushort>(ref b));
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a).Equals(As<T, char>(ref b));
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a).Equals(As<T, int>(ref b));
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a).Equals(As<T, uint>(ref b));
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a).Equals(As<T, long>(ref b));
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a).Equals(As<T, ulong>(ref b));
            else
                return Equals2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool Equals2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a).Equals(As<T, nint>(ref b));
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a).Equals(As<T, nuint>(ref b));
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a).Equals(As<T, BigInteger>(ref b));
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Integer<T> a, T b) => InstanceEquals(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Integer<T> a, Integer<T> b) => InstanceEquals(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool InstanceEquals(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) == As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) == As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) == As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) == As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) == As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) == As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) == As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) == As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) == As<T, ulong>(ref b);
            else
                return InstanceEquals2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool InstanceEquals2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) == As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) == As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) == As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Integer<T> a, T b) => InstanceNotEquals(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Integer<T> a, Integer<T> b) => InstanceNotEquals(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool InstanceNotEquals(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) != As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) != As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) != As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) != As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) != As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) != As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) != As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) != As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) != As<T, ulong>(ref b);
            else
                return InstanceNotEquals2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool InstanceNotEquals2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) != As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) != As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) != As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AssertNumaric()
        {
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal) || typeof(T) == typeof(bool))
            {
                throw new InvalidCastException(typeof(T).FullName + " is not an integeral type");
            }
        }

        public Integer(T value) { AssertNumaric(); _value = value; }

        public static implicit operator Integer<T>(T value) => new(value);

        public static implicit operator T(Integer<T> number) => number._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator +(Integer<T> a, T b) => Add(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator +(Integer<T> a, Integer<T> b) => Add(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Add(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) + As<T, sbyte>(ref b))));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) + As<T, byte>(ref b))));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) + As<T, short>(ref b))));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) + As<T, ushort>(ref b))));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) + As<T, char>(ref b))));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) + As<T, int>(ref b)));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) + As<T, uint>(ref b)));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) + As<T, long>(ref b)));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) + As<T, ulong>(ref b)));
            else
                return Add2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T Add2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) + As<T, nint>(ref b)));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) + As<T, nuint>(ref b)));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) + As<T, BigInteger>(ref b)));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator -(Integer<T> a, T b) => Sub(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator -(Integer<T> a, Integer<T> b) => Sub(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Sub(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) - As<T, sbyte>(ref b))));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) - As<T, byte>(ref b))));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) - As<T, short>(ref b))));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) - As<T, ushort>(ref b))));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) - As<T, char>(ref b))));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) - As<T, int>(ref b)));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) - As<T, uint>(ref b)));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) - As<T, long>(ref b)));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) - As<T, ulong>(ref b)));
            else
                return Sub2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T Sub2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) - As<T, nint>(ref b)));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) - As<T, nuint>(ref b)));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) - As<T, BigInteger>(ref b)));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator *(Integer<T> a, T b) => Mul(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator *(Integer<T> a, Integer<T> b) => Mul(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Mul(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) * As<T, sbyte>(ref b))));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) * As<T, byte>(ref b))));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) * As<T, short>(ref b))));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) * As<T, ushort>(ref b))));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) * As<T, char>(ref b))));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) * As<T, int>(ref b)));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) * As<T, uint>(ref b)));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) * As<T, long>(ref b)));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) * As<T, ulong>(ref b)));
            else
                return Mul2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T Mul2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) * As<T, nint>(ref b)));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) * As<T, nuint>(ref b)));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) * As<T, BigInteger>(ref b)));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator /(Integer<T> a, T b) => Div(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator /(Integer<T> a, Integer<T> b) => Div(a._value, b._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Div(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) / As<T, sbyte>(ref b))));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) / As<T, byte>(ref b))));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) / As<T, short>(ref b))));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) / As<T, ushort>(ref b))));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) / As<T, char>(ref b))));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) / As<T, int>(ref b)));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) / As<T, uint>(ref b)));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) / As<T, long>(ref b)));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) / As<T, ulong>(ref b)));
            else
                return Div2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T Div2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) / As<T, nint>(ref b)));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) / As<T, nuint>(ref b)));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) / As<T, BigInteger>(ref b)));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator >>(Integer<T> a, int b) => ShiftRight(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T ShiftRight(T a, int b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) >> b)));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) >> b)));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) >> b)));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) >> b)));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) >> b)));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) >> b));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) >> b));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) >> b));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) >> b));
            else
                return ShiftRight2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T ShiftRight2(T a, int b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) >> b));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) >> b));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) >> b));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Integer<T> operator <<(Integer<T> a, int b) => ShiftLeft(a._value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T ShiftLeft(T a, int b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<sbyte, T>(ref AsRef((sbyte)(As<T, sbyte>(ref a) << b)));
            else if (typeof(T) == typeof(byte))
                return As<byte, T>(ref AsRef((byte)(As<T, byte>(ref a) << b)));
            else if (typeof(T) == typeof(short))
                return As<short, T>(ref AsRef((short)(As<T, short>(ref a) << b)));
            else if (typeof(T) == typeof(ushort))
                return As<ushort, T>(ref AsRef((ushort)(As<T, ushort>(ref a) << b)));
            else if (typeof(T) == typeof(char))
                return As<char, T>(ref AsRef((char)(As<T, char>(ref a) << b)));
            else if (typeof(T) == typeof(int))
                return As<int, T>(ref AsRef(As<T, int>(ref a) << b));
            else if (typeof(T) == typeof(uint))
                return As<uint, T>(ref AsRef(As<T, uint>(ref a) << b));
            else if (typeof(T) == typeof(long))
                return As<long, T>(ref AsRef(As<T, long>(ref a) << b));
            else if (typeof(T) == typeof(ulong))
                return As<ulong, T>(ref AsRef(As<T, ulong>(ref a) << b));
            else
                return ShiftLeft2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static T ShiftLeft2(T a, int b)
            {
                if (typeof(T) == typeof(nint))
                    return As<nint, T>(ref AsRef(As<T, nint>(ref a) << b));
                else if (typeof(T) == typeof(nuint))
                    return As<nuint, T>(ref AsRef(As<T, nuint>(ref a) << b));
                else if (typeof(T) == typeof(BigInteger))
                    return As<BigInteger, T>(ref AsRef(As<T, BigInteger>(ref a) << b));
                else
                    return THROW();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Integer<T> other) => Comparer<T>.Default.Compare(_value, other._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other) => Comparer<T>.Default.Compare(_value, other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Integer<T> left, T right) => LessThan(left._value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Integer<T> left, Integer<T> right) => LessThan(left._value, right._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThan(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) < As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) < As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) < As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) < As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) < As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) < As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) < As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) < As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) < As<T, ulong>(ref b);
            else
                return LessThan2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool LessThan2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) < As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) < As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) < As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Integer<T> left, T right) => LessThanOrEqualTo(left._value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Integer<T> left, Integer<T> right) => LessThanOrEqualTo(left._value, right._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThanOrEqualTo(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) <= As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) <= As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) <= As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) <= As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) <= As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) <= As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) <= As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) <= As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) <= As<T, ulong>(ref b);
            else
                return LessThanOrEqualTo2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool LessThanOrEqualTo2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) <= As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) <= As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) <= As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Integer<T> left, T right) => GreaterThan(left._value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Integer<T> left, Integer<T> right) => GreaterThan(left._value, right._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThan(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) > As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) > As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) > As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) > As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) > As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) > As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) > As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) > As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) > As<T, ulong>(ref b);
            else
                return GreaterThan2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool GreaterThan2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) > As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) > As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) > As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Integer<T> left, T right) => GreaterThanOrEqualTo(left._value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Integer<T> left, Integer<T> right) => GreaterThanOrEqualTo(left._value, right._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThanOrEqualTo(T a, T b)
        {
            if (typeof(T) == typeof(sbyte))
                return As<T, sbyte>(ref a) >= As<T, sbyte>(ref b);
            else if (typeof(T) == typeof(byte))
                return As<T, byte>(ref a) >= As<T, byte>(ref b);
            else if (typeof(T) == typeof(short))
                return As<T, short>(ref a) >= As<T, short>(ref b);
            else if (typeof(T) == typeof(ushort))
                return As<T, ushort>(ref a) >= As<T, ushort>(ref b);
            else if (typeof(T) == typeof(char))
                return As<T, char>(ref a) >= As<T, char>(ref b);
            else if (typeof(T) == typeof(int))
                return As<T, int>(ref a) >= As<T, int>(ref b);
            else if (typeof(T) == typeof(uint))
                return As<T, uint>(ref a) >= As<T, uint>(ref b);
            else if (typeof(T) == typeof(long))
                return As<T, long>(ref a) >= As<T, long>(ref b);
            else if (typeof(T) == typeof(ulong))
                return As<T, ulong>(ref a) >= As<T, ulong>(ref b);
            else
                return GreaterThanOrEqualTo2(a, b);

            // Split remaining branches to improve chances of inlining.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool GreaterThanOrEqualTo2(T a, T b)
            {
                if (typeof(T) == typeof(nint))
                    return As<T, nint>(ref a) >= As<T, nint>(ref b);
                else if (typeof(T) == typeof(nuint))
                    return As<T, nuint>(ref a) >= As<T, nuint>(ref b);
                else if (typeof(T) == typeof(BigInteger))
                    return As<T, BigInteger>(ref a) >= As<T, BigInteger>(ref b);
                else
                    return THROW_BOOL();
            }
        }

        // Split remaining branches to improve chances of inlining.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T THROW()
        {
            throw new InvalidOperationException();
        }

        // Split remaining branches to improve chances of inlining.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool THROW_BOOL()
        {
            throw new InvalidOperationException();
        }
    }
}
