using System.Collections;

namespace AndroidUI
{
    public abstract class IMapper<T>
    {
        public abstract void setValue(int arrayIndex, int fieldIndex, ref T v);
        public abstract void setValue(int arrayIndex, int fieldIndex, T v);
        public abstract T getValue(int arrayIndex, int fieldIndex);

        public abstract int FieldLength { get; }
        public abstract int ArrayLength { get; }
    }

    public delegate ref Type MapperField<Class, Type>(Class obj, int arrayIndex, int fieldIndex);
    public delegate Type MapperFieldGet<Class, Type>(Class obj, int arrayIndex, int fieldIndex);
    public delegate void MapperFieldSet<Class, Type>(Class obj, int arrayIndex, int fieldIndex, Type value);

    public unsafe class Mapper<C, T> : IMapper<T>, IDisposable where C : class
    {
        C obj;
        private readonly int length;
        private readonly int arrayLength;
        private bool disposedValue;

        private event MapperField<C, T> ev;
        private bool hasGetSet;
        private event MapperFieldGet<C, T> get_ev;
        private event MapperFieldSet<C, T> set_ev;

        public override int FieldLength => length;
        public override int ArrayLength => arrayLength;


        public Mapper(C obj, MapperFieldGet<C, T> getField, MapperFieldSet<C, T> setField, int fieldCount)
        {
            this.obj = obj ?? throw new ArgumentNullException(nameof(obj));

            if (fieldCount <= 0) throw new ArgumentOutOfRangeException(nameof(fieldCount), "fieldCount must be greater than zero");

            length = fieldCount;
            arrayLength = obj is Array ? (obj as Array).Length : 1;
            if (arrayLength == 0) throw new ArgumentOutOfRangeException(nameof(obj), "array length cannot be zero");
            hasGetSet = true;
            get_ev = getField ?? throw new ArgumentNullException(nameof(getField));
            set_ev = setField ?? throw new ArgumentNullException(nameof(setField));
        }

        public Mapper(C obj, MapperField<C, T> field, int fieldCount)
        {
            this.obj = obj ?? throw new ArgumentNullException(nameof(obj));

            if (fieldCount <= 0) throw new ArgumentOutOfRangeException(nameof(fieldCount), "fieldCount must be greater than zero");

            length = fieldCount;
            arrayLength = obj is Array ? (obj as Array).Length : 1;
            if (arrayLength == 0) throw new ArgumentOutOfRangeException(nameof(obj), "array length cannot be zero");
            ev = field ?? throw new ArgumentNullException(nameof(field));
        }

        public override void setValue(int arrayIndex, int fieldIndex, ref T v)
        {
            if (hasGetSet) set_ev.Invoke(obj, arrayIndex, fieldIndex, v);
            else ev.Invoke(obj, arrayIndex, fieldIndex) = v;
        }

        public override void setValue(int arrayIndex, int fieldIndex, T v)
        {
            if (hasGetSet) set_ev.Invoke(obj, arrayIndex, fieldIndex, v);
            else ev.Invoke(obj, arrayIndex, fieldIndex) = v;
        }

        public override T getValue(int arrayIndex, int fieldIndex)
        {
            if (hasGetSet) return get_ev.Invoke(obj, arrayIndex, fieldIndex);
            else return ev.Invoke(obj, arrayIndex, fieldIndex);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                if (hasGetSet)
                {
                    Delegate[] delegates = get_ev.GetInvocationList();
                    foreach (Delegate @delegate in delegates)
                    {
                        get_ev -= (MapperFieldGet<C, T>)@delegate;
                    }
                    delegates = set_ev.GetInvocationList();
                    foreach (Delegate @delegate in delegates)
                    {
                        set_ev -= (MapperFieldSet<C, T>)@delegate;
                    }
                }
                else
                {
                    Delegate[] delegates = ev.GetInvocationList();
                    foreach (Delegate @delegate in delegates)
                    {
                        ev -= (MapperField<C, T>)@delegate;
                    }
                }
                disposedValue = true;
            }
        }

        ~Mapper()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

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
        private bool isMapper;

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
            if (array_type == typeof(IMapper<T>)) {
                isMapper = true;
                if (!isMapperSameLength(array))
                {
                    throw new InvalidDataException("all mappers must have the same length");
                }
            } else {
                Type target_type = typeof(T);
                if (array_type != target_type)
                {
                    throw new InvalidCastException("array of type '" + array_type + "' is not valid for ContiguousArray<" + target_type + ">");
                }
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
                            if (isMapper)
                            {
                                IMapper<T> mapper = (IMapper<T>)item;
                                for (int i_A = 0; i_A < mapper.ArrayLength; i_A++)
                                {
                                    for (int i_F = 0; i_F < mapper.FieldLength; i_F++)
                                    {
                                        if (i == index)
                                        {
                                            return mapper.getValue(i_A, i % mapper.FieldLength);
                                        }
                                        i++;
                                    }
                                }
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
                            if (isMapper)
                            {
                                IMapper<T> mapper = (IMapper<T>)item;
                                for (int i_A = 0; i_A < mapper.ArrayLength; i_A++)
                                {
                                    for (int i_F = 0; i_F < mapper.FieldLength; i_F++)
                                    {
                                        if (i == index)
                                        {
                                            mapper.setValue(i_A, i % mapper.FieldLength, value);
                                            return;
                                        }
                                        i++;
                                    }
                                }
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
            }
            throw new IndexOutOfRangeException();
        }

        bool isMapperSameLength(Array value)
        {
            var stack = new LinkedList<Array>();
            stack.AddFirst(value);
            int i = 0;
            bool first = true;
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
                            int tmp = item switch
                            {
                                IMapper<T> => ((IMapper<T>)item).FieldLength,
                                _ => 1
                            };
                            if (first) i = tmp;
                            else if (i != tmp) return false;
                        }
                    }
                }
            }
            return true;
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
                            return item switch
                            {
                                IMapper<T> => typeof(IMapper<T>),
                                _ => item.GetType()
                            };
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