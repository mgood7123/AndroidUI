using System.Collections;

namespace AndroidUI.Utils.Lists
{
    class ArrayList<T> : IList<T>, System.Collections.IList, IReadOnlyList<T>
    {
        T[] array = Array.Empty<T>();
        int size;

        ArrayList() {
        }

        ArrayList(int capacity)
        {
            EnsureCapacity(capacity);
        }

        ArrayList<T> Clone()
        {
            ArrayList<T> values = new ArrayList<T>();
            values.array = (T[])array.Clone();
            values.size = size;
            return values;
        }

        public T this[int index] { get => array[index]; set => array[index] = value; }
        object IList.this[int index] { get => array[index]; set => array[index] = (T)value; }

        public int Count => array.Length;

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        // TODO: increase capacity by 2
        public int increaseSize()
        {
            int old = size;
            size++;
            Array.Resize(ref array, size);
            return old;
        }

        // TODO: decrease capacity by 2
        public void decreaseSize()
        {
            size--;
            Array.Resize(ref array, size);
        }

        public void EnsureCapacity(int capacity)
        {
            size = capacity;
            Array.Resize(ref array, capacity);
        }

        public void Add(T item)
        {
            array[increaseSize()] = item;
        }

        public int Add(object value)
        {
            T item = (T)value;
            int r = increaseSize();
            array[size] = item;
            return r;
        }

        public void Clear()
        {
            Array.Clear(array);
        }

        public bool Contains(T item)
        {
            return array.Contains(item);
        }

        public bool Contains(object value)
        {
            return array.Contains((T)value);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            array.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            array.CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)array.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(array, item);
        }

        public int IndexOf(object value)
        {
            return Array.IndexOf(array, value);
        }

        public void Insert(int index, T item)
        {
            int oldSize = increaseSize();
            if (index < oldSize)
            {
                Array.Copy(array, index, array, index + 1, oldSize - index);
            }
            array[index] = item;
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1) return false;
            Array.Copy(array, index+1, array, index - 1, size - index);
            decreaseSize();
            return true;
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            Array.Copy(array, index + 1, array, index - 1, size - index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }
}
