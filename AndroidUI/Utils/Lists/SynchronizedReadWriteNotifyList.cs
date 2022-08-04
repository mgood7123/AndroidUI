using System.Collections;
using System.Collections.ObjectModel;

namespace AndroidUI.Utils.Lists
{
    public class SynchronizedReadWriteNotifyList<T> : ListWrapper<T>
    {

        LockInfo lockInfo;

        public SynchronizedReadWriteNotifyList() : this(new ReaderWriterLockSlimInfo()) { }
        public SynchronizedReadWriteNotifyList(int capacity) : this(new ReaderWriterLockSlimInfo(), capacity) { }
        public SynchronizedReadWriteNotifyList(IEnumerable<T> collection) : this(new ReaderWriterLockSlimInfo(), collection) { }

        public SynchronizedReadWriteNotifyList(LockInfo lockInfo) : base() =>
            this.lockInfo = lockInfo ?? throw new ArgumentNullException(nameof(lockInfo));

        public SynchronizedReadWriteNotifyList(LockInfo lockInfo, int capacity) : base(capacity) =>
            this.lockInfo = lockInfo ?? throw new ArgumentNullException(nameof(lockInfo));

        public SynchronizedReadWriteNotifyList(LockInfo lockInfo, IEnumerable<T> collection) : base(collection) =>
            this.lockInfo = lockInfo ?? throw new ArgumentNullException(nameof(lockInfo));

        ~SynchronizedReadWriteNotifyList()
        {
            lockInfo.Dispose();
        }

        virtual protected void BeforeReadOperation() { }
        virtual protected void AfterReadOperation() { }
        virtual protected void BeforeWriteOperation() { }
        virtual protected void AfterWriteOperation() { }

        void TryWithRead(Action a, int timeout = -1)
        {
            try
            {
                lockInfo.AcquireReadLock(timeout);
                BeforeReadOperation();
                a.Invoke();
            }
            finally
            {
                AfterReadOperation();
                lockInfo.ReleaseReadLock();
            }
        }

        T TryWithReadReturn<T>(Func<T> a, int timeout = -1)
        {
            T result = default;
            try
            {
                lockInfo.AcquireReadLock(timeout);
                BeforeReadOperation();
                result = a.Invoke();
            }
            finally
            {
                AfterReadOperation();
                lockInfo.ReleaseReadLock();
            }
            return result;
        }

        void TryWithWrite(Action a, int timeout = -1)
        {
            try
            {
                lockInfo.AcquireWriteLock(timeout);
                BeforeWriteOperation();
                a.Invoke();
            }
            finally
            {
                AfterWriteOperation();
                lockInfo.ReleaseWriteLock();
            }
        }

        T TryWithWriteReturn<T>(Func<T> a, int timeout = -1)
        {
            T result = default;
            try
            {
                lockInfo.AcquireWriteLock(timeout);
                BeforeWriteOperation();
                result = a.Invoke();
            }
            finally
            {
                AfterWriteOperation();
                lockInfo.ReleaseWriteLock();
            }
            return result;
        }

        public override T this[int index]
        {
            get => TryWithReadReturn(() => base[index]);
            set => TryWithWrite(() => base[index] = value);
        }

        public override int Capacity
        {
            get => TryWithReadReturn(() => base.Capacity);
            set => TryWithWrite(() => base.Capacity = value);
        }

        public override int Count =>
            TryWithReadReturn(() => base.Count);

        protected override bool IList_IsFixedSize => base.IList_IsFixedSize;

        protected override bool IList_IsReadOnly => base.IList_IsReadOnly;

        protected override bool ICollection_IsSynchronized => true;

        protected override object ICollection_SyncRoot => base.ICollection_SyncRoot;

        public override void Add(T item) =>
            TryWithWrite(() => base.Add(item));

        public override void AddRange(IEnumerable<T> collection) =>
            TryWithWrite(() => base.AddRange(collection));

        public override ReadOnlyCollection<T> AsReadOnly() =>
            TryWithReadReturn(() => base.AsReadOnly());

        public override int BinarySearch(int index, int count, T item, IComparer<T> comparer) =>
            TryWithReadReturn(() => base.BinarySearch(index, count, item, comparer));

        public override int BinarySearch(T item) =>
            TryWithReadReturn(() => base.BinarySearch(item));

        public override int BinarySearch(T item, IComparer<T> comparer) =>
            TryWithReadReturn(() => base.BinarySearch(item, comparer));

        public override void Clear() =>
            TryWithWrite(() => base.Clear());

        public override bool Contains(T item) =>
            TryWithReadReturn(() => base.Contains(item));

        public override List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) =>
            TryWithReadReturn(() => base.ConvertAll(converter));

        public override void CopyTo(T[] array) =>
            TryWithRead(() => base.CopyTo(array));

        public override void CopyTo(int index, T[] array, int arrayIndex, int count) =>
            TryWithRead(() => base.CopyTo(index, array, arrayIndex, count));

        public override void CopyTo(T[] array, int arrayIndex) =>
            TryWithRead(() => base.CopyTo(array, arrayIndex));

        public override int EnsureCapacity(int min) =>
            TryWithWriteReturn(() => base.EnsureCapacity(min));

        public override bool Equals(object obj) =>
            TryWithReadReturn(() => base.Equals(obj));

        public override bool Exists(Predicate<T> match) =>
            TryWithReadReturn(() => base.Exists(match));

        public override T Find(Predicate<T> match) =>
            TryWithReadReturn(() => base.Find(match));

        public override List<T> FindAll(Predicate<T> match) =>
            TryWithReadReturn(() => base.FindAll(match));

        public override int FindIndex(Predicate<T> match) =>
            TryWithReadReturn(() => base.FindIndex(match));

        public override int FindIndex(int startIndex, Predicate<T> match) =>
            TryWithReadReturn(() => base.FindIndex(startIndex, match));

        public override int FindIndex(int startIndex, int count, Predicate<T> match) =>
            TryWithReadReturn(() => base.FindIndex(startIndex, count, match));

        public override T FindLast(Predicate<T> match) =>
            TryWithReadReturn(() => base.FindLast(match));

        public override int FindLastIndex(Predicate<T> match) =>
            TryWithReadReturn(() => base.FindLastIndex(match));

        public override int FindLastIndex(int startIndex, Predicate<T> match) =>
            TryWithReadReturn(() => base.FindLastIndex(startIndex, match));

        public override int FindLastIndex(int startIndex, int count, Predicate<T> match) =>
            TryWithReadReturn(() => base.FindLastIndex(startIndex, count, match));

        public override void ForEach(Action<T> action) =>
            TryWithWrite(() => base.ForEach(action));

        public override EnumeratorWrapper GetEnumerator() =>
            TryWithReadReturn(() => new SynchronizedReadWriteNotifyListEnumerator(this));

        public override int GetHashCode() =>
            TryWithReadReturn(() => base.GetHashCode());

        public override List<T> GetRange(int index, int count) =>
            TryWithReadReturn(() => base.GetRange(index, count));

        public override int IndexOf(T item) =>
            TryWithReadReturn(() => base.IndexOf(item));

        public override int IndexOf(T item, int index) =>
            TryWithReadReturn(() => base.IndexOf(item, index));

        public override int IndexOf(T item, int index, int count) =>
            TryWithReadReturn(() => base.IndexOf(item, index, count));

        public override void Insert(int index, T item) =>
            TryWithWrite(() => base.Insert(index, item));

        public override void InsertRange(int index, IEnumerable<T> collection) =>
            TryWithWrite(() => base.InsertRange(index, collection));

        public override int LastIndexOf(T item) =>
            TryWithReadReturn(() => base.LastIndexOf(item));

        public override int LastIndexOf(T item, int index) =>
            TryWithReadReturn(() => base.LastIndexOf(item, index));

        public override int LastIndexOf(T item, int index, int count) =>
            TryWithReadReturn(() => base.LastIndexOf(item, index, count));

        public override bool Remove(T item) =>
            TryWithWriteReturn(() => base.Remove(item));

        public override int RemoveAll(Predicate<T> match) =>
            TryWithWriteReturn(() => base.RemoveAll(match));

        public override void RemoveAt(int index) =>
            TryWithWrite(() => base.RemoveAt(index));

        public override void RemoveRange(int index, int count) =>
            TryWithWrite(() => base.RemoveRange(index, count));

        public override void Reverse() =>
            TryWithWrite(() => base.Reverse());

        public override void Reverse(int index, int count) =>
            TryWithWrite(() => base.Reverse(index, count));

        public override void Sort() =>
            TryWithWrite(() => base.Sort());

        public override void Sort(IComparer<T> comparer) =>
            TryWithWrite(() => base.Sort(comparer));

        public override void Sort(int index, int count, IComparer<T> comparer) =>
            TryWithWrite(() => base.Sort(index, count, comparer));

        public override void Sort(Comparison<T> comparison) =>
            TryWithWrite(() => base.Sort(comparison));

        public override T[] ToArray() =>
            TryWithReadReturn(() => base.ToArray());

        public override string ToString() =>
            TryWithReadReturn(() => base.ToString());

        public override void TrimExcess() =>
            TryWithWrite(() => base.TrimExcess());

        public override bool TrueForAll(Predicate<T> match) =>
            TryWithReadReturn(() => base.TrueForAll(match));

        protected override void ICollection_CopyTo(Array array, int arrayIndex) =>
            TryWithRead(() => base.ICollection_CopyTo(array, arrayIndex));

        protected override IEnumerator IEnumerable_GetEnumerator() =>
            TryWithReadReturn(() => new SynchronizedReadWriteNotifyListEnumerator(this));

        protected override IEnumerator<T> IEnumerable_T__GetEnumerator() =>
            TryWithReadReturn(() => new SynchronizedReadWriteNotifyListEnumerator(this));

        protected override int IList_Add(object item) =>
            TryWithWriteReturn(() => base.IList_Add(item));

        protected override bool IList_Contains(object item) =>
            TryWithReadReturn(() => base.IList_Contains(item));

        protected override object IList_get_this(int index) =>
            TryWithReadReturn(() => base.IList_get_this(index));

        protected override int IList_IndexOf(object item) =>
            TryWithReadReturn(() => base.IList_IndexOf(item));

        protected override void IList_Insert(int index, object item) =>
            TryWithWrite(() => base.IList_Insert(index, item));

        protected override void IList_Remove(object item) =>
            TryWithWrite(() => base.IList_Remove(item));

        protected override void IList_set_this(int index, object value) =>
            TryWithWrite(() => base.IList_set_this(index, value));

        class SynchronizedReadWriteNotifyListEnumerator : EnumeratorWrapper
        {
            SynchronizedReadWriteNotifyList<T> outer;
            public SynchronizedReadWriteNotifyListEnumerator(SynchronizedReadWriteNotifyList<T> outer) : base(outer.list) =>
                this.outer = outer;

            public override T Current =>
                outer.TryWithReadReturn(() => base.Current);

            protected override object IEnumerator_Current =>
                outer.TryWithReadReturn(() => base.IEnumerator_Current);

            public override bool MoveNext() =>
                outer.TryWithWriteReturn(() => base.MoveNext());

            protected override void IEnumerator_Reset() =>
                outer.TryWithWrite(() => base.IEnumerator_Reset());

            protected override void OnBeforeDispose()
            {
                outer.lockInfo.AcquireWriteLock(-1);
                base.OnBeforeDispose();
            }

            protected override void OnAfterDispose()
            {
                base.OnAfterDispose();
                outer.lockInfo.ReleaseWriteLock();
            }

            // no need to lock in OnDispose()
        }
    }
}
