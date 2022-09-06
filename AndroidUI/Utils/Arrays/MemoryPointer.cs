using System.Collections;

namespace AndroidUI.Utils.Arrays
{
    /// <summary>
    /// equivilant to T*
    /// <br></br>
    /// <inheritdoc cref="ContiguousArray{T}"/>
    /// </summary>
    public class MemoryPointer<T> : ContiguousArray<T>, IEnumerable<T>
    {
        internal int offset = 0;

        int Offset => offset;

        public override int Length => offset == 0 ? base.Length : base.Length - offset;

        public MemoryPointer(Array a) : base(a)
        {
        }

        public MemoryPointer(ContiguousArray<T> contiguousArray) : base(contiguousArray)
        {
        }

        public void AssignFrom(MemoryPointer<T> memoryPointer)
        {
            ArgumentNullException.ThrowIfNull(memoryPointer, nameof(memoryPointer));
            base.AssignFrom(memoryPointer);
            offset = memoryPointer.offset;
        }

        public MemoryPointer(MemoryPointer<T> memoryPointer)
        {
            AssignFrom(memoryPointer);
        }

        public void Copy(MemoryPointer<T> dest)
        {
            if (length != dest.Length)
            {
                throw new ArgumentOutOfRangeException("this length (" + length + ") differs from dest length (" + dest.Length + ")");
            }

            for (int i = 0; i < Length; i++)
            {
                dest[i] = this[i];
            }
        }

        public void Copy(MemoryPointer<T> dest, int length)
        {
            if (Length < length)
            {
                throw new ArgumentOutOfRangeException("given length (" + length + ") is greater than this.length (" + Length + ")");
            }

            if (dest.Length < length)
            {
                throw new ArgumentOutOfRangeException("given length (" + length + ") is greater than dest length (" + dest.Length + ")");
            }

            for (int i = 0; i < length; i++)
            {
                dest[i] = this[i];
            }
        }

        public override T this[int index]
        {
            get => base[offset + index];
            set => base[offset + index] = value;
        }

        /// <summary>
        /// increments the pointer offset by the specified value
        /// </summary>
        public static MemoryPointer<T> operator +(MemoryPointer<T> memoryPointer, int value)
        {
            return memoryPointer.Increment(value);
        }

        /// <summary>
        /// increments the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Increment(int value)
        {
            if (value == 0)
            {
                return this;
            }

            if (value < 0)
            {
                throw new IndexOutOfRangeException("negative indexes are not allowed");
            }

            MemoryPointer<T> tmp = new(this);
            tmp.offset += value;
            return tmp;
        }

        /// <summary>
        /// increments the pointer offset by the specified value
        /// </summary>
        public static MemoryPointer<T> operator +(MemoryPointer<T> memoryPointer, MemoryPointer<T> value)
        {
            return memoryPointer.Increment(value);
        }

        /// <summary>
        /// increments the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Increment(MemoryPointer<T> value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return Increment(value.offset);
        }

        /// <summary>
        /// decrements the pointer offset by the specified value
        /// </summary>
        public static MemoryPointer<T> operator -(MemoryPointer<T> memoryPointer, int value)
        {
            return memoryPointer.Decrement(value);
        }

        /// <summary>
        /// decrements the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Decrement(int value)
        {
            if (value == 0)
            {
                return this;
            }

            if (value < 0)
            {
                throw new IndexOutOfRangeException("negative indexes are not allowed");
            }

            MemoryPointer<T> tmp = new(this);
            tmp.offset -= value;
            return tmp;
        }

        /// <summary>
        /// decrements the pointer offset by the specified pointer
        /// </summary>
        public static MemoryPointer<T> operator -(MemoryPointer<T> memoryPointer, MemoryPointer<T> value)
        {
            return memoryPointer.Decrement(value);
        }

        /// <summary>
        /// decrements the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Decrement(MemoryPointer<T> value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            return Decrement(value.offset);
        }

        /// <summary>
        /// increments the pointer offset by one
        /// </summary>
        public static MemoryPointer<T> operator ++(MemoryPointer<T> memoryPointer)
        {
            return memoryPointer.Increment();
        }

        /// <summary>
        /// increments the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Increment()
        {
            offset += 1;
            return this;
        }

        /// <summary>
        /// decrements the pointer offset by one
        /// </summary>
        public static MemoryPointer<T> operator --(MemoryPointer<T> memoryPointer)
        {
            return memoryPointer.Decrement();
        }

        /// <summary>
        /// decrements the pointer offset by the specified value
        /// </summary>
        public MemoryPointer<T> Decrement()
        {
            offset -= 1;
            return this;
        }

        public T Dereference()
        {
            return this[0];
        }

        public T DereferenceAndIncrement()
        {
            T r = this[0];
            Increment();
            return r;
        }

        public T DereferenceAndDecrement()
        {
            T r = this[0];
            Decrement();
            return r;
        }

        private class MemoryPointerEnumerator<T> : IEnumerator<T>
        {
            MemoryPointer<T> memoryPointer;
            private int current_index;
            private T current_item;

            public MemoryPointerEnumerator(MemoryPointer<T> memoryPointer)
            {
                this.memoryPointer = memoryPointer;
                current_index = -1;
                current_item = default;
            }

            public bool MoveNext()
            {
                //Avoids going beyond the end of the collection.
                if (++current_index >= memoryPointer.Length)
                {
                    return false;
                }
                else
                {
                    // Set current box to next item in collection.
                    current_item = memoryPointer[current_index];
                }
                return true;
            }

            public void Reset() { current_index = -1; }

            void IDisposable.Dispose() { }

            public T Current => current_item;

            object IEnumerator.Current => Current;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new MemoryPointerEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MemoryPointerEnumerator<T>(this);
        }

        public static implicit operator MemoryPointer<T>(Array array)
        {
            return new MemoryPointer<T>(array);
        }

        /// <summary>
        /// returns the internal pointer offset
        /// </summary>
        public static implicit operator int(MemoryPointer<T> v)
        {
            return v.offset;
        }

        /// <summary>
        /// resets the pointer offset to zero
        /// </summary>
        public void ResetPointerOffset()
        {
            offset = 0;
        }

        public MemoryPointer<R> CastToMemoryPointer<R>()
        {
            var m = new MemoryPointer<R>(CastToContiguousArray<R>());
            m.offset = offset;
            return m;
        }
    }

    public static class MemoryPointer
    {
        public static MemoryPointer<T> ToMemoryPointer<T>(this Array a)
        {
            return new MemoryPointer<T>(a);
        }

        public static MemoryPointer<T> ToMemoryPointer<T>(this ContiguousArray<T> a)
        {
            return new MemoryPointer<T>(a);
        }

        unsafe public static MemoryPointer<T> ToMemoryPointer<T>(this IntPtr ptr, int len) where T : unmanaged
        {
            return Arrays.AsArray<T>(ptr, len);
        }
    }
}