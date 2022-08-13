using System.Runtime.CompilerServices;

namespace AndroidUI.Utils
{
    public struct IntegerPair<T> where T : unmanaged
    {
        public Integer<T> First;
        public Integer<T> Second;

        public static readonly IntegerPair<T> Zero = new(default(T), default(T));

        public override string ToString()
        {
            return "" + First + ", " + Second;
        }

        public IntegerPair(T first, T second)
        {
            First = first;
            Second = second;
        }
        public IntegerPair(ref T first, ref T second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(ref T first, T second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(T first, ref T second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(Integer<T> first, Integer<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(ref Integer<T> first, Integer<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(Integer<T> first, ref Integer<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerPair(ref Integer<T> first, ref Integer<T> second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator (T, T)(IntegerPair<T> IntegerPair) { return (IntegerPair.First, IntegerPair.Second); }
        public static implicit operator (Integer<T>, Integer<T>)(IntegerPair<T> IntegerPair) { return (IntegerPair.First, IntegerPair.Second); }
        public static implicit operator IntegerPair<T>((T first, T second) value) { return new IntegerPair<T>(ref value.first, ref value.second); }
        public static implicit operator IntegerPair<T>((Integer<T> first, Integer<T> second) value) { return new IntegerPair<T>(ref value.first, ref value.second); }

        public override bool Equals(object obj)
        {
            if (obj is IntegerPair<T>) return Equals((IntegerPair<T>)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntegerPair<T> left, IntegerPair<T> right) => left.First == right.First && left.Second == right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntegerPair<T> left, IntegerPair<T> right) => left.First != right.First && left.Second != right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(IntegerPair<T> right) => First.Equals(right.First) && Second.Equals(right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator +(IntegerPair<T> left, IntegerPair<T> right) => new(left.First + right.First, left.Second + right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator -(IntegerPair<T> left, IntegerPair<T> right) => new(left.First - right.First, left.Second - right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator *(IntegerPair<T> left, IntegerPair<T> right) => new(left.First * right.First, left.Second * right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator /(IntegerPair<T> left, IntegerPair<T> right) => new(left.First / right.First, left.Second / right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator >>(IntegerPair<T> left, int right) => new(left.First >> right, left.Second >> right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerPair<T> operator <<(IntegerPair<T> left, int right) => new(left.First << right, left.Second << right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(IntegerPair<T> left, IntegerPair<T> right) => left.First > right.First && left.Second > right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(IntegerPair<T> left, IntegerPair<T> right) => left.First < right.First && left.Second < right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(IntegerPair<T> left, IntegerPair<T> right) => left.First >= right.First && left.Second >= right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(IntegerPair<T> left, IntegerPair<T> right) => left.First <= right.First && left.Second <= right.Second;
    }
}