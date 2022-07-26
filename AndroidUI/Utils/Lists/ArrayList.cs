namespace AndroidUI.Utils.Lists
{
    public class ArrayList<T> : ListWrapper<T>
    {
        public ArrayList()
        {
        }

        public ArrayList(int capacity) : base(capacity)
        {
            Resize(capacity);
        }

        public ArrayList(IEnumerable<T> collection) : base(collection)
        {
        }

        public override int Capacity
        {
            get => base.Capacity;
            set
            {
                base.Capacity = value;
                Resize(value);
            }
        }

        public ArrayList<T> Clone()
        {
            ArrayList<T> values = new ArrayList<T>(Capacity);
            values.AddRange(this);
            return values;
        }

        public override int EnsureCapacity(int min)
        {
            int newCap = base.EnsureCapacity(min);
            Resize(newCap);
            return newCap;
        }

        public void Resize(int capacity)
        {
            if (capacity == 0)
            {
                Clear();
            }
            else
            {
                if (capacity > Count)
                {
                    while (Count < capacity)
                    {
                        Add(default(T));
                    }
                }
                else if (capacity < Count)
                {
                    while (Count > capacity)
                    {
                        RemoveAt(Count - 1);
                    }
                }
            }
        }
    }
}
