using System.Runtime.CompilerServices;

namespace AndroidUI.Utils
{
    public struct FloatingPointPair<T> where T : unmanaged
    {
        public FloatingPoint<T> First;
        public FloatingPoint<T> Second;

        public static readonly FloatingPointPair<T> Zero = new(default(T), default(T));

        public override string ToString()
        {
            return "" + First + ", " + Second;
        }

        public FloatingPointPair(T first, T second)
        {
            First = first;
            Second = second;
        }
        public FloatingPointPair(ref T first, ref T second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(ref T first, T second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(T first, ref T second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(FloatingPoint<T> first, FloatingPoint<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(ref FloatingPoint<T> first, FloatingPoint<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(FloatingPoint<T> first, ref FloatingPoint<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointPair(ref FloatingPoint<T> first, ref FloatingPoint<T> second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator (T, T)(FloatingPointPair<T> FloatingPointPair) { return (FloatingPointPair.First, FloatingPointPair.Second); }
        public static implicit operator (FloatingPoint<T>, FloatingPoint<T>)(FloatingPointPair<T> FloatingPointPair) { return (FloatingPointPair.First, FloatingPointPair.Second); }
        public static implicit operator FloatingPointPair<T>((T first, T second) value) { return new FloatingPointPair<T>(ref value.first, ref value.second); }
        public static implicit operator FloatingPointPair<T>((FloatingPoint<T> first, FloatingPoint<T> second) value) { return new FloatingPointPair<T>(ref value.first, ref value.second); }

        public override bool Equals(object obj)
        {
            if (obj is FloatingPointPair<T>) return Equals((FloatingPointPair<T>)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First == right.First && left.Second == right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First != right.First && left.Second != right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FloatingPointPair<T> right) => First.Equals(right.First) && Second.Equals(right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointPair<T> operator +(FloatingPointPair<T> left, FloatingPointPair<T> right) => new(left.First + right.First, left.Second + right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointPair<T> operator -(FloatingPointPair<T> left, FloatingPointPair<T> right) => new(left.First - right.First, left.Second - right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointPair<T> operator *(FloatingPointPair<T> left, FloatingPointPair<T> right) => new(left.First * right.First, left.Second * right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointPair<T> operator /(FloatingPointPair<T> left, FloatingPointPair<T> right) => new(left.First / right.First, left.Second / right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First > right.First && left.Second > right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First < right.First && left.Second < right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First >= right.First && left.Second >= right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FloatingPointPair<T> left, FloatingPointPair<T> right) => left.First <= right.First && left.Second <= right.Second;
    }
}