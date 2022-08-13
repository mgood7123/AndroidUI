namespace AndroidUI.Utils
{
    /// <summary>
    /// contains the current T as value member
    /// <br></br>
    /// <br></br>
    /// can be used to wrap a Value Type as a reference
    /// </summary>
    public class ValueHolder<T>
    {
        public T Value;
        public static readonly ValueHolder<T> Zero = new(default);
        public ValueHolder() { }
        public ValueHolder(T value)
        {
            Value = value;
        }

        public ValueHolder(ref T value)
        {
            Value = value;
        }

        public static implicit operator T(ValueHolder<T> valueHolder) { return valueHolder.Value; }
        public static implicit operator ValueHolder<T>(T value) { return new ValueHolder<T>(value); }
    }

    /// <summary>
    /// contains the current T1 and T2 as value members
    /// <br></br>
    /// <br></br>
    /// can be used to wrap a Value Type's as references
    /// </summary>
    public class ValueHolderPair<T1, T2>
    {
        public T1 First;
        public T2 Second;
        public static readonly ValueHolderPair<T1, T2> Zero = new(default, default);

        public ValueHolderPair() { }
        public ValueHolderPair(T1 first)
        {
            First = first;
        }
        public ValueHolderPair(T2 second)
        {
            Second = second;
        }
        public ValueHolderPair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
        public ValueHolderPair(ref T1 first)
        {
            First = first;
        }
        public ValueHolderPair(ref T2 second)
        {
            Second = second;
        }
        public ValueHolderPair(ref T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderPair(ref T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderPair(T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }

        public static implicit operator T1(ValueHolderPair<T1,T2> pair) { return pair.First; }
        public static implicit operator T2(ValueHolderPair<T1, T2> pair) { return pair.Second; }
        public static implicit operator (T1,T2)(ValueHolderPair<T1, T2> pair) { return (pair.First, pair.Second); }
        public static implicit operator ValueHolderPair<T1, T2>(T1 first) { return new ValueHolderPair<T1, T2>(ref first); }
        public static implicit operator ValueHolderPair<T1, T2>(T2 second) { return new ValueHolderPair<T1, T2>(ref second); }
        public static implicit operator ValueHolderPair<T1, T2>((T1 first, T2 second) value) { return new ValueHolderPair<T1, T2>(ref value.first, ref value.second); }
    }

    /// <summary>
    /// contains the current T1 and T2 as value members
    /// <br></br>
    /// <br></br>
    /// can be used to wrap a Value Type's as references
    /// </summary>
    public class ValueHolderQuad<T1, T2, T3, T4>
    {
        public T1 First;
        public T2 Second;
        public T3 Third;
        public T4 Forth;

        public static readonly ValueHolderQuad<T1, T2, T3, T4> Zero = new(default, default, default, default);

        public ValueHolderQuad() { }
        public ValueHolderQuad(T1 first)
        {
            First = first;
        }
        public ValueHolderQuad(T2 second)
        {
            Second = second;
        }
        public ValueHolderQuad(T3 third)
        {
            Third = third;
        }
        public ValueHolderQuad(T4 forth)
        {
            Forth = forth;
        }
        public ValueHolderQuad(ref T1 first)
        {
            First = first;
        }
        public ValueHolderQuad(ref T2 second)
        {
            Second = second;
        }
        public ValueHolderQuad(ref T3 third)
        {
            Third = third;
        }
        public ValueHolderQuad(ref T4 forth)
        {
            Forth = forth;
        }
        public ValueHolderQuad(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderQuad(ref T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderQuad(T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderQuad(ref T1 first, ref T2 second)
        {
            First = first;
            Second = second;
        }

        public ValueHolderQuad(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(ref T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(T1 first, ref T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(T1 first, T2 second, ref T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(ref T1 first, ref T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(ref T1 first, T2 second, ref T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(T1 first, ref T2 second, ref T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(ref T1 first, ref T2 second, ref T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public ValueHolderQuad(T1 first, T2 second, T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(ref T1 first, T2 second, T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(T1 first, ref T2 second, T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(T1 first, T2 second, ref T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(T1 first, T2 second, T3 third, ref T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(ref T1 first, ref T2 second, T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(ref T1 first, T2 second, ref T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(ref T1 first, T2 second, T3 third, ref T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(T1 first, ref T2 second, ref T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(T1 first, ref T2 second, T3 third, ref T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }

        public ValueHolderQuad(ref T1 first, ref T2 second, ref T3 third, T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }
        public ValueHolderQuad(ref T1 first, ref T2 second, T3 third, ref T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }
        public ValueHolderQuad(ref T1 first, ref T2 second, ref T3 third, ref T4 forth)
        {
            First = first;
            Second = second;
            Third = third;
            Forth = forth;
        }
        public static implicit operator T1(ValueHolderQuad<T1, T2, T3, T4> pair) { return pair.First; }
        public static implicit operator T2(ValueHolderQuad<T1, T2, T3, T4> pair) { return pair.Second; }
        public static implicit operator T3(ValueHolderQuad<T1, T2, T3, T4> pair) { return pair.Third; }
        public static implicit operator T4(ValueHolderQuad<T1, T2, T3, T4> pair) { return pair.Forth; }
        public static implicit operator (T1, T2)(ValueHolderQuad<T1, T2, T3, T4> pair) { return (pair.First, pair.Second); }
        public static implicit operator (T1, T2, T3)(ValueHolderQuad<T1, T2, T3, T4> pair) { return (pair.First, pair.Second, pair.Third); }
        public static implicit operator (T1, T2, T3, T4)(ValueHolderQuad<T1, T2, T3, T4> pair) { return (pair.First, pair.Second, pair.Third, pair.Forth); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>(T1 first) { return new ValueHolderQuad<T1, T2, T3, T4>(ref first); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>(T2 second) { return new ValueHolderQuad<T1, T2, T3, T4>(ref second); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>(T3 third) { return new ValueHolderQuad<T1, T2, T3, T4>(ref third); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>(T4 fourth) { return new ValueHolderQuad<T1, T2, T3, T4>(ref fourth); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>((T1 first, T2 second) value) { return new ValueHolderQuad<T1, T2, T3, T4>(ref value.first, ref value.second); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>((T1 first, T2 second, T3 third) value) { return new ValueHolderQuad<T1, T2, T3, T4>(ref value.first, ref value.second, ref value.third); }
        public static implicit operator ValueHolderQuad<T1, T2, T3, T4>((T1 first, T2 second, T3 third, T4 fourth) value) { return new ValueHolderQuad<T1, T2, T3, T4>(ref value.first, ref value.second, ref value.third, ref value.fourth); }
    }
}
