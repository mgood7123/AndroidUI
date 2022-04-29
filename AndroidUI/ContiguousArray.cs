using System.Collections;

namespace AndroidUI
{
	/// <summary>
	/// wraps an Array in a 1-dimensional array iterator
	/// <br></br>
	/// does not copy the given array
	/// <br></br>
	/// any nested arrays in the input array cannot be null
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ContiguousArray<T> : IEnumerable<T>
	{
        protected Array value;
        protected int length;

        public int Length => length;

		/// <summary>
		/// this exists purely for decleration as a field
		/// <br></br>
		/// please use another constructor or call AssignFrom
		/// </summary>
        public ContiguousArray()
        {
        }

		/// <summary>
		/// returns the wrapped array
		/// </summary>
		public Array GetArray() => value;

		public void AssignFrom(Array array)
        {
			ArgumentNullException.ThrowIfNull(array, nameof(array));
			Type array_type = getArrayType(array);
			Type target_type = typeof(T);
			if (array_type != target_type)
			{
				throw new InvalidCastException("array of type '" + array_type + "' is not valid for ContiguousArray<" + target_type + ">");
			}
			value = array;
			length = getArrayLength();
		}

		public void AssignFrom(ContiguousArray<T> contiguousArray)
		{
			ArgumentNullException.ThrowIfNull(contiguousArray, nameof(contiguousArray));
			AssignFrom(contiguousArray.value);
		}

		public ContiguousArray(Array a)
		{
			AssignFrom(a);
		}

		public ContiguousArray(ContiguousArray<T> contiguousArray)
		{
			AssignFrom(contiguousArray);
		}

		public void Copy(ContiguousArray<T> dest)
        {
			if (length != dest.length)
            {
				throw new ArgumentOutOfRangeException("this length (" + length + ") differs from dest length (" + dest.length + ")");
            }

            for (int i = 0; i < length; i++)
            {
				dest[i] = this[i];
            }
        }

		public void Copy(ContiguousArray<T> dest, int length)
		{
			if (this.length < length)
			{
				throw new ArgumentOutOfRangeException("given length (" + length + ") differs from this.length (" + this.length + ")");
			}

            if (dest.length < length)
			{
				throw new ArgumentOutOfRangeException("given length (" + length + ") differs from dest length (" + dest.length + ")");
			}

			for (int i = 0; i < length; i++)
			{
				dest[i] = this[i];
			}
		}

		public void Copy(T[] dest)
		{
			if (length != dest.Length)
			{
				throw new ArgumentOutOfRangeException("this length (" + length + ") differs from dest length (" + dest.Length + ")");
			}

			for (int i = 0; i < length; i++)
			{
				dest[i] = this[i];
			}
		}

		public void Copy(T[] dest, int length)
		{
			if (this.length < length)
			{
				throw new ArgumentOutOfRangeException("given length (" + length + ") differs from this.length (" + this.length + ")");
			}

			if (dest.Length < length)
			{
				throw new ArgumentOutOfRangeException("given length (" + length + ") differs from dest length (" + dest.Length + ")");
			}

			for (int i = 0; i < length; i++)
			{
				dest[i] = this[i];
			}
		}

		public virtual T this[int index]
		{
			get => getArrayValue(index);
			set => setArrayValue(index, value);
		}

		int getArrayLength()
		{
			var stack = new LinkedList<Array>();
			stack.AddFirst(value);
			int i = 0;
			while (true)
			{
				if (stack.Count == 0)
				{
					break;
				}
				Array array = stack.Last.Value;
				stack.RemoveLast();
				foreach (var item in array)
				{
					if (item != null)
					{
						if (item.GetType().IsArray)
						{
							stack.AddFirst((Array)item);
						}
						else
						{
							i++;
						}
					}
				}
			}
			return i;
		}

		T getArrayValue(int index)
		{
			var stack = new LinkedList<Array>();
			stack.AddFirst(value);
			int i = 0;
			while (true)
			{
				if (stack.Count == 0)
				{
					break;
				}
				Array array = stack.Last.Value;
				stack.RemoveLast();
				foreach (var item in array)
				{
					if (item != null)
					{
						if (item.GetType().IsArray)
						{
							stack.AddFirst((Array)item);
						}
						else
						{
							if (i == index)
							{
								return (T)item;
							}
							i++;
						}
					}
				}
			}
			throw new IndexOutOfRangeException();
		}

		void setArrayValue(int index, T value)
		{
			var stack = new LinkedList<Array>();
			stack.AddFirst(this.value);
			int i = 0;
			while (true)
			{
				if (stack.Count == 0)
				{
					break;
				}
				Array array = stack.Last.Value;
				stack.RemoveLast();
                IList list = array;
				for (int i1 = 0; i1 < list.Count; i1++)
				{
					object item = list[i1];
					if (item != null)
					{
						if (item.GetType().IsArray)
						{
							stack.AddFirst((Array)item);
						}
						else
						{
							if (i == index)
							{
								list[i1] = value;
								return;
							}
							i++;
						}
					}
				}
			}
			throw new IndexOutOfRangeException();
		}

		Type getArrayType(Array value)
		{
			var stack = new LinkedList<Array>();
			stack.AddFirst(value);
			int i = 0;
			while (true)
			{
				if (stack.Count == 0)
				{
					break;
				}
				Array array = stack.Last.Value;
				stack.RemoveLast();
				foreach (var item in array)
				{
					if (item != null)
					{
						if (item.GetType().IsArray)
						{
							stack.AddFirst((Array)item);
						}
						else
						{
							return item.GetType();
						}
					}
				}
			}
			return null;
		}

		// can this be used to optimize above getters/setters?
		static Dictionary<Array, int> getArrayLengths(Array value)
		{
			Dictionary<Array, int> map = new();
			var stack = new LinkedList<Array>();
			stack.AddFirst(value);
			while (true)
			{
				if (stack.Count == 0)
				{
					break;
				}
				Array array = stack.Last.Value;
				stack.RemoveLast();
				map[array] = array.Length;
				foreach (var item in array)
				{
					if (item != null)
					{
						if (item.GetType().IsArray)
						{
							stack.AddFirst((Array)item);
						}
					}
				}
			}
			return map;
		}

		private class ContiguousArrayEnumerator<T> : IEnumerator<T>
		{
			ContiguousArray<T> contiguousArray;
			private int current_index;
			private T current_item;

			public ContiguousArrayEnumerator(ContiguousArray<T> contiguousArray)
			{
				this.contiguousArray = contiguousArray;
				current_index = -1;
				current_item = default(T);
			}

			public bool MoveNext()
			{
				//Avoids going beyond the end of the collection.
				if (++current_index >= contiguousArray.length)
				{
					return false;
				}
				else
				{
					// Set current box to next item in collection.
					current_item = contiguousArray[current_index];
				}
				return true;
			}

			public void Reset() { current_index = -1; }

			void IDisposable.Dispose() { }

            public T Current => current_item;

            object IEnumerator.Current => Current;
        }
		
		public virtual IEnumerator<T> GetEnumerator()
        {
			return new ContiguousArrayEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
			return new ContiguousArrayEnumerator<T>(this);
		}

		public static implicit operator ContiguousArray<T>(Array array)
		{
			return new ContiguousArray<T>(array);
		}
	}

	public static class ContiguousArray
	{
		public static ContiguousArray<T> ToContiguousArray<T>(this Array a)
		{
			return new ContiguousArray<T>(a);
		}
	}
}