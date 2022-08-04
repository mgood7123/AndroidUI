namespace AndroidUI.Utils.Lists
{
    /// <summary>
    /// makes a copy of the internal list BEFORE any method which can modify the list is invoked
    /// </summary>
    public class CopyOnWriteList<T> : SynchronizedReadWriteNotifyList<T>
    {
        public CopyOnWriteList()
        {
        }

        public CopyOnWriteList(int capacity) : base(capacity)
        {
        }

        public CopyOnWriteList(IEnumerable<T> collection) : base(collection)
        {
        }

        public CopyOnWriteList(LockInfo lockInfo) : base(lockInfo)
        {
        }


        public CopyOnWriteList(LockInfo lockInfo, int capacity) : base(lockInfo, capacity)
        {
        }

        public CopyOnWriteList(LockInfo lockInfo, IEnumerable<T> collection) : base(lockInfo, collection)
        {
        }

        protected override void BeforeWriteOperation()
        {
            base.BeforeWriteOperation();
            List<T> copy = new List<T>(list);
            list = copy;
        }
    }
}
