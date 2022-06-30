using System.Diagnostics;
using System.Collections.ObjectModel;

namespace AndroidUI
{

    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ListWrapper<T> : IList<T>, System.Collections.IList, IReadOnlyList<T>
    {
        protected List<T> list;

        // Constructs a List. The list is initially empty and has a capacity
        // of zero. Upon adding the first element to the list the capacity is
        // increased to 16, and then increased in multiples of two as required.
        public ListWrapper()
        {
            list = new();
        }

        // Constructs a List with a given initial capacity. The list is
        // initially empty, but will have room for the given number of elements
        // before any reallocations are required.
        // 
        public ListWrapper(int capacity)
        {
            list = new(capacity);
        }

        // Constructs a List, copying the contents of the given collection. The
        // size and capacity of the new list will both be equal to the size of the
        // given collection.
        // 
        public ListWrapper(IEnumerable<T> collection)
        {
            list = new(collection);
        }

        // Gets and sets the capacity of this list.  The capacity is the size of
        // the internal array used to hold items.  When set, the internal 
        // array of the list is reallocated to the given capacity.
        // 
        virtual public int Capacity
        {
            get
            {
                return list.Capacity;
            }
            set
            {
                list.Capacity = value;
            }
        }

        // Read-only property describing how many elements are in the List.
        virtual public int Count
        {
            get
            {
                return list.Count;
            }
        }

        protected virtual bool IList_IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return IList_IsFixedSize; }
        }

        protected virtual bool IList_IsReadOnly
        {
            get { return false; }
        }

        // Is this List read-only?
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return IList_IsReadOnly; }
        }

        protected virtual bool ICollection_IsSynchronized
        {
            get { return false; }
        }

        // Is this List synchronized (thread-safe)?
        bool System.Collections.ICollection.IsSynchronized
        {
            get { return ICollection_IsSynchronized; }
        }

        protected virtual object ICollection_SyncRoot
        {
            get { return ((System.Collections.ICollection)list).SyncRoot; }
        }

        // Synchronization root for this object.
        object System.Collections.ICollection.SyncRoot
        {
            get
            {
                return ICollection_SyncRoot;
            }
        }
        // Sets or Gets the element at the given index.
        // 
        public virtual T this[int index]
        {
            get
            {
                return list[index];
            }

            set
            {
                list[index] = value;
            }
        }

        protected virtual object IList_get_this(int index)
        {
            return ((System.Collections.IList)list)[index];
        }

        protected virtual void IList_set_this(int index, object value)
        {
            ((System.Collections.IList)list)[index] = (T)value;
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return IList_get_this(index);
            }
            set
            {
                IList_set_this(index, value);
            }
        }

        // Adds the given object to the end of this list. The size of the list is
        // increased by one. If required, the capacity of the list is doubled
        // before adding the new element.
        //
        public virtual void Add(T item)
        {
            list.Add(item);
        }

        protected virtual int IList_Add(object item)
        {
            return ((System.Collections.IList)list).Add(item);
        }

        int System.Collections.IList.Add(object item)
        {
            return IList_Add(item);
        }


        // Adds the elements of the given collection to the end of this list. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.
        //
        public virtual void AddRange(IEnumerable<T> collection)
        {
            list.AddRange(collection);
        }

        public virtual ReadOnlyCollection<T> AsReadOnly()
        {
            return list.AsReadOnly();
        }

        // Searches a section of the list for a given element using a binary search
        // algorithm. Elements of the list are compared to the search value using
        // the given IComparer interface. If comparer is null, elements of
        // the list are compared to the search value using the IComparable
        // interface, which in that case must be implemented by all elements of the
        // list and the given search value. This method assumes that the given
        // section of the list is already sorted; if this is not the case, the
        // result will be incorrect.
        //
        // The method returns the index of the given value in the list. If the
        // list does not contain the given value, the method returns a negative
        // integer. The bitwise complement operator (~) can be applied to a
        // negative result to produce the index of the first element (if any) that
        // is larger than the given search value. This is also the index at which
        // the search value should be inserted into the list in order for the list
        // to remain sorted.
        // 
        // The method uses the Array.BinarySearch method to perform the
        // search.
        // 
        public virtual int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return list.BinarySearch(index, count, item, comparer);
        }

        public virtual int BinarySearch(T item)
        {
            return list.BinarySearch(item);
        }

        public virtual int BinarySearch(T item, IComparer<T> comparer)
        {
            return list.BinarySearch(item, comparer);
        }


        // Clears the contents of List.
        public virtual void Clear()
        {
            list.Clear();
        }

        // Contains returns true if the specified element is in the List.
        // It does a linear, O(n) search.  Equality is determined by calling
        // item.Equals().
        //
        public virtual bool Contains(T item)
        {
            return list.Contains(item);
        }

        protected virtual bool IList_Contains(object item)
        {
            return ((System.Collections.IList)list).Contains(item);
        }

        bool System.Collections.IList.Contains(object item)
        {
            return IList_Contains(item);
        }

        public virtual List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return list.ConvertAll<TOutput>(converter);
        }

        // Copies this List into array, which must be of a 
        // compatible array type.  
        //
        public virtual void CopyTo(T[] array)
        {
            list.CopyTo(array);
        }

        protected virtual void ICollection_CopyTo(Array array, int arrayIndex)
        {
            ((System.Collections.ICollection)list).CopyTo(array, arrayIndex);
        }


        // Copies this List into array, which must be of a 
        // compatible array type.  
        //
        void System.Collections.ICollection.CopyTo(Array array, int arrayIndex)
        {
            ICollection_CopyTo(array, arrayIndex);
        }

        // Copies a section of this list to the given array at the given index.
        // 
        // The method uses the Array.Copy method to copy the elements.
        // 
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            list.CopyTo(index, array, arrayIndex, count);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        // Ensures that the capacity of this list is at least the given minimum
        // value. If the currect capacity of the list is less than min, the
        // capacity is increased to twice the current capacity or to min,
        // whichever is larger.
        public virtual int EnsureCapacity(int min)
        {
            return list.EnsureCapacity(min);
        }

        public virtual bool Exists(Predicate<T> match)
        {
            return list.Exists(match);
        }

        public virtual T Find(Predicate<T> match)
        {
            return list.Find(match);
        }

        public virtual List<T> FindAll(Predicate<T> match)
        {
            return list.FindAll(match);
        }

        public virtual int FindIndex(Predicate<T> match)
        {
            return list.FindIndex(match);
        }

        public virtual int FindIndex(int startIndex, Predicate<T> match)
        {
            return list.FindIndex(startIndex, match);
        }

        public virtual int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return list.FindIndex(startIndex, count, match);
        }

        public virtual T FindLast(Predicate<T> match)
        {
            return list.FindLast(match);
        }

        public virtual int FindLastIndex(Predicate<T> match)
        {
            return list.FindLastIndex(match);
        }

        public virtual int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return list.FindLastIndex(startIndex, match);
        }

        public virtual int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return list.FindLastIndex(startIndex, count, match);
        }

        public virtual void ForEach(Action<T> action)
        {
            list.ForEach(action);
        }

        // Returns an enumerator for this list with the given
        // permission for removal of elements. If modifications made to the list 
        // while an enumeration is in progress, the MoveNext and 
        // GetObject methods of the enumerator will throw an exception.
        //
        public virtual EnumeratorWrapper GetEnumerator()
        {
            return new EnumeratorWrapper(list);
        }

        protected virtual IEnumerator<T> IEnumerable_T__GetEnumerator()
        {
            return new EnumeratorWrapper(list);
        }

        protected virtual System.Collections.IEnumerator IEnumerable_GetEnumerator()
        {
            return new EnumeratorWrapper(list);
        }

        /// <internalonly/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return IEnumerable_T__GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return IEnumerable_GetEnumerator();
        }

        public virtual List<T> GetRange(int index, int count)
        {
            return list.GetRange(index, count);
        }


        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards from beginning to end.
        // The elements of the list are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public virtual int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        protected virtual int IList_IndexOf(object item)
        {
            return ((System.Collections.IList)list).IndexOf(item);
        }

        int System.Collections.IList.IndexOf(object item)
        {
            return IList_IndexOf(item);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // index and ending at count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public virtual int IndexOf(T item, int index)
        {
            return list.IndexOf(item, index);
        }

        // Returns the index of the first occurrence of a given value in a range of
        // this list. The list is searched forwards, starting at index
        // index and upto count number of elements. The
        // elements of the list are compared to the given value using the
        // Object.Equals method.
        // 
        // This method uses the Array.IndexOf method to perform the
        // search.
        // 
        public virtual int IndexOf(T item, int index, int count)
        {
            return list.IndexOf(item, index, count);
        }

        // Inserts an element into this list at a given index. The size of the list
        // is increased by one. If required, the capacity of the list is doubled
        // before inserting the new element.
        // 
        public virtual void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        protected virtual void IList_Insert(int index, object item)
        {
            ((System.Collections.IList)list).Insert(index, item);
        }

        void System.Collections.IList.Insert(int index, object item)
        {
            IList_Insert(index, item);
        }

        // Inserts the elements of the given collection at a given index. If
        // required, the capacity of the list is increased to twice the previous
        // capacity or the new size, whichever is larger.  Ranges may be added
        // to the end of the list by setting index to the List's size.
        //
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            list.InsertRange(index, collection);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at the end 
        // and ending at the first element in the list. The elements of the list 
        // are compared to the given value using the Object.Equals method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public virtual int LastIndexOf(T item)
        {
            return list.LastIndexOf(item);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // index and ending at the first element in the list. The 
        // elements of the list are compared to the given value using the 
        // Object.Equals method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public virtual int LastIndexOf(T item, int index)
        {
            return list.LastIndexOf(item, index);
        }

        // Returns the index of the last occurrence of a given value in a range of
        // this list. The list is searched backwards, starting at index
        // index and upto count elements. The elements of
        // the list are compared to the given value using the Object.Equals
        // method.
        // 
        // This method uses the Array.LastIndexOf method to perform the
        // search.
        // 
        public virtual int LastIndexOf(T item, int index, int count)
        {
            return list.LastIndexOf(item, index, count);
        }

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        // 
        public virtual bool Remove(T item)
        {
            return list.Remove(item);
        }

        protected virtual void IList_Remove(object item)
        {
            ((System.Collections.IList)list).Remove(item);
        }

        void System.Collections.IList.Remove(object item)
        {
            IList_Remove(item);
        }

        // This method removes all items which matches the predicate.
        // The complexity is O(n).   
        public virtual int RemoveAll(Predicate<T> match)
        {
            return list.RemoveAll(match);
        }

        // Removes the element at the given index. The size of the list is
        // decreased by one.
        // 
        public virtual void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        // Removes a range of elements from this list.
        // 
        public virtual void RemoveRange(int index, int count)
        {
            list.RemoveRange(index, count);
        }

        // Reverses the elements in this list.
        public virtual void Reverse()
        {
            list.Reverse();
        }

        // Reverses the elements in a range of this list. Following a call to this
        // method, an element in the range given by index and count
        // which was previously located at index i will now be located at
        // index index + (index + count - i - 1).
        // 
        // This method uses the Array.Reverse method to reverse the
        // elements.
        // 
        public virtual void Reverse(int index, int count)
        {
            list.Reverse(index, count);
        }

        // Sorts the elements in this list.  Uses the default comparer and 
        // Array.Sort.
        public virtual void Sort()
        {
            list.Sort();
        }

        // Sorts the elements in this list.  Uses Array.Sort with the
        // provided comparer.
        public virtual void Sort(IComparer<T> comparer)
        {
            list.Sort(comparer);
        }

        // Sorts the elements in a section of this list. The sort compares the
        // elements to each other using the given IComparer interface. If
        // comparer is null, the elements are compared to each other using
        // the IComparable interface, which in that case must be implemented by all
        // elements of the list.
        // 
        // This method uses the Array.Sort method to sort the elements.
        // 
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            list.Sort(index, count, comparer);
        }

        public virtual void Sort(Comparison<T> comparison)
        {
            list.Sort(comparison);
        }

        // ToArray returns a new Object array containing the contents of the List.
        // This requires copying the List, which is an O(n) operation.
        public virtual T[] ToArray()
        {
            return list.ToArray();
        }

        // Sets the capacity of this list to the size of the list. This method can
        // be used to minimize a list's memory overhead once it is known that no
        // new elements will be added to the list. To completely clear a list and
        // release all memory referenced by the list, execute the following
        // statements:
        // 
        // list.Clear();
        // list.TrimExcess();
        // 
        public virtual void TrimExcess()
        {
            list.TrimExcess();
        }

        public virtual bool TrueForAll(Predicate<T> match)
        {
            return list.TrueForAll(match);
        }

        public override bool Equals(object obj)
        {
            return list.Equals(obj);
        }

        public override int GetHashCode()
        {
            return list.GetHashCode();
        }

        public override string ToString()
        {
            return list.ToString();
        }

        [Serializable]
        public class EnumeratorWrapper : IEnumerator<T>, System.Collections.IEnumerator
        {
            List<T>.Enumerator e;
            private bool disposedValue;

            public EnumeratorWrapper(List<T> list)
            {
                e = list.GetEnumerator();
            }

            public virtual bool MoveNext()
            {
                return e.MoveNext();
            }

            public virtual T Current
            {
                get
                {
                    return e.Current;
                }
            }

            protected virtual object IEnumerator_Current
            {
                get
                {
                    return ((System.Collections.IEnumerator)e).Current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return IEnumerator_Current;
                }
            }

            protected virtual void IEnumerator_Reset()
            {
                ((System.Collections.IEnumerator)e).Reset();
            }

            void System.Collections.IEnumerator.Reset()
            {
                IEnumerator_Reset();
            }

            protected virtual void OnBeforeDispose()
            {
            }

            protected virtual void OnAfterDispose()
            {
            }

            protected virtual void OnDispose()
            {
                e.Dispose();
            }

#pragma warning disable CA1063 // Implement IDisposable Correctly
            void Dispose(bool disposing)
#pragma warning restore CA1063 // Implement IDisposable Correctly
            {
                OnBeforeDispose();
                if (!disposedValue)
                {
                    OnDispose();
                    disposedValue = true;
                }
                OnAfterDispose();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~EnumeratorWrapper()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(false);
            }
        }
    }
}
