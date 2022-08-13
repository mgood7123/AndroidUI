using System.Runtime.CompilerServices;

namespace AndroidUI.Utils
{
    public struct IntegerQuad<T>where T : unmanaged
    {
        public IntegerPair<T> First;
        public IntegerPair<T> Second;

        public static readonly IntegerQuad<T> Zero = new(default(T), default(T), default(T), default(T));

        public override string ToString()
        {
            return "" + First + ", " + Second;
        }

        public IntegerQuad(T first, T second, T third, T fourth)
        {
            First = new(first, second);
            Second = new(third, fourth);
        }

        public IntegerQuad(ref T first, T second, T third, T fourth)
        {
            First = new(ref first, second);
            Second = new(third, fourth);
        }

        public IntegerQuad(T first, ref T second, T third, T fourth)
        {
            First = new(first, ref second);
            Second = new(third, fourth);
        }

        public IntegerQuad(ref T first, ref T second, T third, T fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, fourth);
        }

        public IntegerQuad(T first, T second, ref T third, T fourth)
        {
            First = new(first, second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(ref T first, T second, ref T third, T fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(T first, ref T second, ref T third, T fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(ref T first, ref T second, ref T third, T fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(T first, T second, T third, ref T fourth)
        {
            First = new(first, second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(ref T first, T second, T third, ref T fourth)
        {
            First = new(ref first, second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(T first, ref T second, T third, ref T fourth)
        {
            First = new(first, ref second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(ref T first, ref T second, T third, ref T fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(T first, T second, ref T third, ref T fourth)
        {
            First = new(first, second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(ref T first, T second, ref T third, ref T fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(T first, ref T second, ref T third, ref T fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(ref T first, ref T second, ref T third, ref T fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(Integer<T> first, Integer<T> second, Integer<T> third, Integer<T> fourth)
        {
            First = new(first, second);
            Second = new(third, fourth);
        }

        public IntegerQuad(ref Integer<T> first, Integer<T> second, Integer<T> third, Integer<T> fourth)
        {
            First = new(ref first, second);
            Second = new(third, fourth);
        }

        public IntegerQuad(Integer<T> first, ref Integer<T> second, Integer<T> third, Integer<T> fourth)
        {
            First = new(first, ref second);
            Second = new(third, fourth);
        }

        public IntegerQuad(ref Integer<T> first, ref Integer<T> second, Integer<T> third, Integer<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, fourth);
        }

        public IntegerQuad(Integer<T> first, Integer<T> second, ref Integer<T> third, Integer<T> fourth)
        {
            First = new(first, second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(ref Integer<T> first, Integer<T> second, ref Integer<T> third, Integer<T> fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(Integer<T> first, ref Integer<T> second, ref Integer<T> third, Integer<T> fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(ref Integer<T> first, ref Integer<T> second, ref Integer<T> third, Integer<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, fourth);
        }

        public IntegerQuad(Integer<T> first, Integer<T> second, Integer<T> third, ref Integer<T> fourth)
        {
            First = new(first, second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(ref Integer<T> first, Integer<T> second, Integer<T> third, ref Integer<T> fourth)
        {
            First = new(ref first, second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(Integer<T> first, ref Integer<T> second, Integer<T> third, ref Integer<T> fourth)
        {
            First = new(first, ref second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(ref Integer<T> first, ref Integer<T> second, Integer<T> third, ref Integer<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, ref fourth);
        }

        public IntegerQuad(Integer<T> first, Integer<T> second, ref Integer<T> third, ref Integer<T> fourth)
        {
            First = new(first, second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(ref Integer<T> first, Integer<T> second, ref Integer<T> third, ref Integer<T> fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(Integer<T> first, ref Integer<T> second, ref Integer<T> third, ref Integer<T> fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(ref Integer<T> first, ref Integer<T> second, ref Integer<T> third, ref Integer<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, ref fourth);
        }

        public IntegerQuad(IntegerPair<T> first, IntegerPair<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerQuad(ref IntegerPair<T> first, IntegerPair<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerQuad(IntegerPair<T> first, ref IntegerPair<T> second)
        {
            First = first;
            Second = second;
        }

        public IntegerQuad(ref IntegerPair<T> first, ref IntegerPair<T> second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator (T, T, T, T)(IntegerQuad<T> IntegerQuad) { return (IntegerQuad.First.First, IntegerQuad.First.Second, IntegerQuad.Second.First, IntegerQuad.Second.Second); }
        public static implicit operator (Integer<T>, Integer<T>, Integer<T>, Integer<T>)(IntegerQuad<T> IntegerQuad) { return (IntegerQuad.First.First, IntegerQuad.First.Second, IntegerQuad.Second.First, IntegerQuad.Second.Second); }
        public static implicit operator (IntegerPair<T>, IntegerPair<T>)(IntegerQuad<T> IntegerQuad) { return (IntegerQuad.First, IntegerQuad.Second); }
        public static implicit operator IntegerQuad<T>((T first, T second, T third, T fourth) value) { return new IntegerQuad<T>(ref value.first, ref value.second, ref value.third, ref value.fourth); }
        public static implicit operator IntegerQuad<T>((Integer<T> first, Integer<T> second, Integer<T> third, Integer<T> fourth) value) { return new IntegerQuad<T>(ref value.first, ref value.second, ref value.third, ref value.fourth); }
        public static implicit operator IntegerQuad<T>((IntegerPair<T> first, IntegerPair<T> second) value) { return new IntegerQuad<T>(ref value.first, ref value.second); }

        public override bool Equals(object obj)
        {
            if (obj is IntegerQuad<T>) return Equals((IntegerQuad<T>)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(IntegerQuad<T> left, IntegerQuad<T> right) => left.First == right.First && left.Second == right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(IntegerQuad<T> left, IntegerQuad<T> right) => left.First != right.First && left.Second != right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(IntegerQuad<T> right) => First.Equals(right.First) && Second.Equals(right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator +(IntegerQuad<T> left, IntegerQuad<T> right) => new(left.First + right.First, left.Second + right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator -(IntegerQuad<T> left, IntegerQuad<T> right) => new(left.First - right.First, left.Second - right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator *(IntegerQuad<T> left, IntegerQuad<T> right) => new(left.First * right.First, left.Second * right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator /(IntegerQuad<T> left, IntegerQuad<T> right) => new(left.First / right.First, left.Second / right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator >>(IntegerQuad<T> left, int right) => new(left.First >> right, left.Second >> right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntegerQuad<T> operator <<(IntegerQuad<T> left, int right) => new(left.First << right, left.Second << right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(IntegerQuad<T> left, IntegerQuad<T> right) => left.First > right.First && left.Second > right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(IntegerQuad<T> left, IntegerQuad<T> right) => left.First < right.First && left.Second < right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(IntegerQuad<T> left, IntegerQuad<T> right) => left.First >= right.First && left.Second >= right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(IntegerQuad<T> left, IntegerQuad<T> right) => left.First <= right.First && left.Second <= right.Second;
    }
}