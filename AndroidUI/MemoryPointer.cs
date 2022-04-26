namespace AndroidUI
{
    /// <summary>
    /// equivilant to T*
    /// </summary>
    class MemoryPointer<T>
    {
        Array ptr;
        int rank;
        int[] indexes;
        int[] lengths;

        public MemoryPointer(T[] value)
        {
            ptr = value;
            rank = value.Rank;
            indexes = new int[rank];
            lengths = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                indexes[i] = 0;
                lengths[i] = ptr.GetLength(i);
            }
        }

        public MemoryPointer(MemoryPointer<T> memoryPointer)
        {
            ptr = memoryPointer.ptr;
            rank = memoryPointer.rank;
            indexes = memoryPointer.indexes;
            lengths = memoryPointer.lengths;
        }

        public void memcpy(MemoryPointer<T> dest, int len)
        {
            //memcpy(0, dest.ptr, dest.index, len);
        }

        public void memcpy(T[] dest, int destOffset, int len)
        {
            //memcpy(0, dest, destOffset, len);
        }

        public void memcpy(int srcOffset, T[] dest, int destOffset, int len)
        {
            //Arrays.arraycopy(ptr, index + srcOffset, dest, destOffset, len);
        }

        public T this[int index]
        {
            get
            {
                return (T)ptr.GetValue(indexes);
            }

            set
            {
                ptr.SetValue(value, indexes);
            }
        }

        public static MemoryPointer<T> operator +(MemoryPointer<T> memoryPointer, int value)
        {
            MemoryPointer<T> tmp = new(memoryPointer);
            bool overflow = true;
            for (int i = tmp.rank - 1; i >= 0; i--)
            {
                if (overflow)
                {
                    tmp.indexes[i]++;
                    overflow = false;
                }
                if (tmp.indexes[i] == tmp.lengths[i])
                {
                    overflow = true;
                    tmp.indexes[i] = 0;
                }
            }
            return tmp;
        }

        public static MemoryPointer<T> operator -(MemoryPointer<T> memoryPointer, int value)
        {
            MemoryPointer<T> tmp = new(memoryPointer);
            bool underflow = true;
            for (int i = tmp.rank - 1; i >= 0; i--)
            {
                if (underflow)
                {
                    tmp.indexes[i]--;
                    underflow = false;
                }
                if (tmp.indexes[i] == 0)
                {
                    underflow = true;
                    tmp.indexes[i] = tmp.lengths[i]-1;
                }
            }
            return tmp;
        }

        public static implicit operator MemoryPointer<T>(T[] array)
        {
            return new MemoryPointer<T>(array);
        }
    }
}