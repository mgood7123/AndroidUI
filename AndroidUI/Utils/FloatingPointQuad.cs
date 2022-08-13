using System.Runtime.CompilerServices;

namespace AndroidUI.Utils
{
    public struct FloatingPointQuad<T> where T : unmanaged
    {
        public FloatingPointPair<T> First;
        public FloatingPointPair<T> Second;

        public static readonly FloatingPointQuad<T> Zero = new(default(T), default(T), default(T), default(T));

        public override string ToString()
        {
            return "" + First + ", " + Second;
        }

        public FloatingPointQuad(T first, T second, T third, T fourth)
        {
            First = new(first, second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(ref T first, T second, T third, T fourth)
        {
            First = new(ref first, second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(T first, ref T second, T third, T fourth)
        {
            First = new(first, ref second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(ref T first, ref T second, T third, T fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(T first, T second, ref T third, T fourth)
        {
            First = new(first, second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(ref T first, T second, ref T third, T fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(T first, ref T second, ref T third, T fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(ref T first, ref T second, ref T third, T fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(T first, T second, T third, ref T fourth)
        {
            First = new(first, second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(ref T first, T second, T third, ref T fourth)
        {
            First = new(ref first, second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(T first, ref T second, T third, ref T fourth)
        {
            First = new(first, ref second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(ref T first, ref T second, T third, ref T fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(T first, T second, ref T third, ref T fourth)
        {
            First = new(first, second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(ref T first, T second, ref T third, ref T fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(T first, ref T second, ref T third, ref T fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(ref T first, ref T second, ref T third, ref T fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, FloatingPoint<T> second, FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(first, second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, FloatingPoint<T> second, FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(ref first, second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, ref FloatingPoint<T> second, FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(first, ref second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, ref FloatingPoint<T> second, FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, FloatingPoint<T> second, ref FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(first, second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, FloatingPoint<T> second, ref FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, ref FloatingPoint<T> second, ref FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, ref FloatingPoint<T> second, ref FloatingPoint<T> third, FloatingPoint<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, FloatingPoint<T> second, FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(first, second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, FloatingPoint<T> second, FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(ref first, second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, ref FloatingPoint<T> second, FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(first, ref second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, ref FloatingPoint<T> second, FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(third, ref fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, FloatingPoint<T> second, ref FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(first, second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, FloatingPoint<T> second, ref FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(ref first, second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(FloatingPoint<T> first, ref FloatingPoint<T> second, ref FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(first, ref second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(ref FloatingPoint<T> first, ref FloatingPoint<T> second, ref FloatingPoint<T> third, ref FloatingPoint<T> fourth)
        {
            First = new(ref first, ref second);
            Second = new(ref third, ref fourth);
        }

        public FloatingPointQuad(FloatingPointPair<T> first, FloatingPointPair<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointQuad(ref FloatingPointPair<T> first, FloatingPointPair<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointQuad(FloatingPointPair<T> first, ref FloatingPointPair<T> second)
        {
            First = first;
            Second = second;
        }

        public FloatingPointQuad(ref FloatingPointPair<T> first, ref FloatingPointPair<T> second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator (T, T, T, T)(FloatingPointQuad<T> FloatingPointQuad) { return (FloatingPointQuad.First.First, FloatingPointQuad.First.Second, FloatingPointQuad.Second.First, FloatingPointQuad.Second.Second); }
        public static implicit operator (FloatingPoint<T>, FloatingPoint<T>, FloatingPoint<T>, FloatingPoint<T>)(FloatingPointQuad<T> FloatingPointQuad) { return (FloatingPointQuad.First.First, FloatingPointQuad.First.Second, FloatingPointQuad.Second.First, FloatingPointQuad.Second.Second); }
        public static implicit operator (FloatingPointPair<T>, FloatingPointPair<T>)(FloatingPointQuad<T> FloatingPointQuad) { return (FloatingPointQuad.First, FloatingPointQuad.Second); }
        public static implicit operator FloatingPointQuad<T>((T first, T second, T third, T fourth) value) { return new FloatingPointQuad<T>(ref value.first, ref value.second, ref value.third, ref value.fourth); }
        public static implicit operator FloatingPointQuad<T>((FloatingPoint<T> first, FloatingPoint<T> second, FloatingPoint<T> third, FloatingPoint<T> fourth) value) { return new FloatingPointQuad<T>(ref value.first, ref value.second, ref value.third, ref value.fourth); }
        public static implicit operator FloatingPointQuad<T>((FloatingPointPair<T> first, FloatingPointPair<T> second) value) { return new FloatingPointQuad<T>(ref value.first, ref value.second); }

        public override bool Equals(object obj)
        {
            if (obj is FloatingPointQuad<T>) return Equals((FloatingPointQuad<T>)obj);
            else return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First == right.First && left.Second == right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First != right.First && left.Second != right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FloatingPointQuad<T> right) => First.Equals(right.First) && Second.Equals(right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointQuad<T> operator +(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => new(left.First + right.First, left.Second + right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointQuad<T> operator -(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => new(left.First - right.First, left.Second - right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointQuad<T> operator *(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => new(left.First * right.First, left.Second * right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatingPointQuad<T> operator /(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => new(left.First / right.First, left.Second / right.Second);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First > right.First && left.Second > right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First < right.First && left.Second < right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First >= right.First && left.Second >= right.Second;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FloatingPointQuad<T> left, FloatingPointQuad<T> right) => left.First <= right.First && left.Second <= right.Second;
    }
}