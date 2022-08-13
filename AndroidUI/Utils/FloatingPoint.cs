using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.Unsafe;

namespace AndroidUI.Utils
{
    // https://sharplab.io/#gist:b3f3c702c579d1f4a7f423be5dc3232f
    public readonly struct FloatingPoint<T> : IEquatable<FloatingPoint<T>>, IComparable<FloatingPoint<T>> where T : unmanaged
    {
        public readonly T value;

        public override string ToString()
        {
            return value.ToString();
        }

        public static FloatingPoint<T> ConvertFrom<T1>(T1 value)
        {
            object obj = Convert.ChangeType(value, typeof(T));

            return obj == null
                ? throw new InvalidCastException("could not convert T1 (" + typeof(T1).FullName + ") to T (" + typeof(T).FullName + ")")
                : (new((T)obj));
        }

        public T ConvertTo<T>()
        {
            object obj = Convert.ChangeType(value, typeof(T));

            return obj == null
                ? throw new InvalidCastException("could not convert " + value.GetType().FullName + " to T (" + typeof(T).FullName + ")")
                : (T)obj;
        }

        public static FloatingPoint<T> MinValue => GetMinValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FloatingPoint<T> GetMinValue()
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef(float.MinValue));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef(double.MinValue));
            else
                return THROW();
        }

        public static FloatingPoint<T> MaxValue => GetMaxValue();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FloatingPoint<T> GetMaxValue()
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef(float.MaxValue));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef(double.MaxValue));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> Parse(string s)
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef(float.Parse(s)));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef(double.Parse(s)));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(string s, out FloatingPoint<T> result)
        {
            if (typeof(T) == typeof(float))
            {
                float r;
                bool ret = float.TryParse(s, out r);
                result = As<float, T>(ref r);
                return ret;
            }
            else if (typeof(T) == typeof(double))
            {
                double r;
                bool ret = double.TryParse(s, out r);
                result = As<double, T>(ref r);
                return ret;
            }
            else
            {
                result = default;
                return THROW_BOOL();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is FloatingPoint<T>) return Equals((FloatingPoint<T>)obj);
            else if (obj is T) return Equals((T)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(T b) => Equals(value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FloatingPoint<T> b) => Equals(value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Equals(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a).Equals(As<T, float>(ref b));
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a).Equals(As<T, double>(ref b));
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatingPoint<T> a, T b) => InstanceEquals(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatingPoint<T> a, FloatingPoint<T> b) => InstanceEquals(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool InstanceEquals(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) == As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) == As<T, double>(ref b);
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatingPoint<T> a, T b) => InstanceNotEquals(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatingPoint<T> a, FloatingPoint<T> b) => InstanceNotEquals(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool InstanceNotEquals(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) != As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) != As<T, double>(ref b);
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AssertFloatingPoint()
        {
            if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
            {
                return;
            }
            throw new InvalidCastException(typeof(T).FullName + " is not an Floating Point type");
        }

        public FloatingPoint(T value) { AssertFloatingPoint(); this.value = value; }

        public static implicit operator FloatingPoint<T>(T value) => new(value);

        public static implicit operator T(FloatingPoint<T> number) => number.value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator +(FloatingPoint<T> a, T b) => Add(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator +(FloatingPoint<T> a, FloatingPoint<T> b) => Add(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Add(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef((float)(As<T, float>(ref a) + As<T, float>(ref b))));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef((double)(As<T, double>(ref a) + As<T, double>(ref b))));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator -(FloatingPoint<T> a, T b) => Sub(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator -(FloatingPoint<T> a, FloatingPoint<T> b) => Sub(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Sub(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef((float)(As<T, float>(ref a) - As<T, float>(ref b))));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef((double)(As<T, double>(ref a) - As<T, double>(ref b))));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator *(FloatingPoint<T> a, T b) => Mul(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator *(FloatingPoint<T> a, FloatingPoint<T> b) => Mul(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Mul(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef((float)(As<T, float>(ref a) * As<T, float>(ref b))));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef((double)(As<T, double>(ref a) * As<T, double>(ref b))));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator /(FloatingPoint<T> a, T b) => Div(a.value, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPoint<T> operator /(FloatingPoint<T> a, FloatingPoint<T> b) => Div(a.value, b.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T Div(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<float, T>(ref AsRef((float)(As<T, float>(ref a) / As<T, float>(ref b))));
            else if (typeof(T) == typeof(double))
                return As<double, T>(ref AsRef((double)(As<T, double>(ref a) / As<T, double>(ref b))));
            else
                return THROW();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(FloatingPoint<T> other) => Comparer<T>.Default.Compare(value, other.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(T other) => Comparer<T>.Default.Compare(value, other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FloatingPoint<T> left, int right) => LessThan(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FloatingPoint<T> left, T right) => LessThan(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FloatingPoint<T> left, FloatingPoint<T> right) => LessThan(left.value, right.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThan(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) < As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) < As<T, double>(ref b);
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThan(T a, int b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) < b;
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) < b;
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FloatingPoint<T> left, int right) => LessThanOrEqualTo(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FloatingPoint<T> left, T right) => LessThanOrEqualTo(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FloatingPoint<T> left, FloatingPoint<T> right) => LessThanOrEqualTo(left.value, right.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThanOrEqualTo(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) <= As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) <= As<T, double>(ref b);
            else
                return THROW_BOOL();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool LessThanOrEqualTo(T a, int b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) <= b;
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) <= b;
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FloatingPoint<T> left, int right) => GreaterThan(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FloatingPoint<T> left, T right) => GreaterThan(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FloatingPoint<T> left, FloatingPoint<T> right) => GreaterThan(left.value, right.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThan(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) > As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) > As<T, double>(ref b);
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThan(T a, int b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) > b;
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) > b;
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FloatingPoint<T> left, int right) => GreaterThanOrEqualTo(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FloatingPoint<T> left, T right) => GreaterThanOrEqualTo(left.value, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FloatingPoint<T> left, FloatingPoint<T> right) => GreaterThanOrEqualTo(left.value, right.value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThanOrEqualTo(T a, T b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) >= As<T, float>(ref b);
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) >= As<T, double>(ref b);
            else
                return THROW_BOOL();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GreaterThanOrEqualTo(T a, int b)
        {
            if (typeof(T) == typeof(float))
                return As<T, float>(ref a) >= b;
            else if (typeof(T) == typeof(double))
                return As<T, double>(ref a) >= b;
            else
                return THROW_BOOL();
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
