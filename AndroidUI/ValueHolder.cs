using System;
using System.Collections.Generic;
using System.Text;

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
		public T value;
		public ValueHolder() { }
		public ValueHolder(T value)
		{
			this.value = value;
		}

		public static implicit operator T(ValueHolder<T> valueHolder) { return valueHolder.value; }
		public static implicit operator ValueHolder<T>(T value) { return new ValueHolder<T>(value); }
	}
}
