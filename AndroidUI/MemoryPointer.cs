using System.Collections;

namespace AndroidUI
{
    /// <summary>
    /// equivilant to T*
    /// <br></br>
    /// <inheritdoc cref="ContiguousArray{T}"/>
    /// </summary>
    public class MemoryPointer<T> : ContiguousArray<T>, IEnumerable<T>
    {
        internal int offset = 0;

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

        public override T this[int index]
        {
            get => base[offset + index];
            set => base[offset + index] = value;
        }

        public static MemoryPointer<T> operator +(MemoryPointer<T> memoryPointer, int value)
        {
            if (value == 0)
            {
                return memoryPointer;
            }

            if (value < 0)
            {
                throw new IndexOutOfRangeException("negative indexes are not allowed");
            }

            MemoryPointer<T> tmp = new MemoryPointer<T>(memoryPointer);
            tmp.offset++;
            return tmp;
        }

        public static MemoryPointer<T> operator -(MemoryPointer<T> memoryPointer, int value)
        {
            if (value == 0)
            {
                return memoryPointer;
            }

            if (value < 0)
            {
                throw new IndexOutOfRangeException("negative indexes are not allowed");
            }

            MemoryPointer<T> tmp = new MemoryPointer<T>(memoryPointer);
            tmp.offset--;
            return tmp;
        }

        public static MemoryPointer<T> operator ++(MemoryPointer<T> memoryPointer)
        {
            return memoryPointer + 1;
        }

        public static MemoryPointer<T> operator --(MemoryPointer<T> memoryPointer)
        {
            return memoryPointer - 1;
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
                current_item = default(T);
            }

            public bool MoveNext()
            {
                //Avoids going beyond the end of the collection.
                if (++current_index >= memoryPointer.length)
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
    }
}