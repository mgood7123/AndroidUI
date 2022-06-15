namespace AndroidUI
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
		public ValueHolder() { }
		public ValueHolder(T value)
		{
			this.Value = value;
		}

		public static implicit operator T(ValueHolder<T> valueHolder) { return valueHolder.Value; }
		public static implicit operator ValueHolder<T>(T value) { return new ValueHolder<T>(value); }
	}
}
