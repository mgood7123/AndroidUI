namespace AndroidUI.Utils.Lists
{
    public class RingBuffer<T> : ListWrapper<T>
    {
        public RingBuffer(int capacity) : base(capacity)
        {
        }

        public RingBuffer(int capacity, IEnumerable<T> collection) : base(capacity)
        {
            AddRange(collection);
        }

        public override void Add(T item) {
            if (Count == Capacity && Capacity != 0)
            {
                RemoveAt(0);
            }
            base.Add(item);
        }

        public override void AddRange(IEnumerable<T> collection)
        {
            if (collection != null)
            {
                foreach (T item in collection)
                {
                    Add(item);
                }
            }
        }

        protected override int IList_Add(object item)
        {
            if (Count == Capacity && Capacity != 0)
            {
                ((System.Collections.IList)list).RemoveAt(0);
            }
            return base.IList_Add(item);
        }

    }
}
