namespace AndroidUI
{
    public partial class Touch
    {
        internal List<Data> history = new();

        public Data getHistoryTouch(int historyIndex)
        {
            return history.ElementAt(historyIndex);
        }

        public int getHistorySize() => history.Count;

        public void copyAllHistory(Touch destination)
        {
            destination.history.Clear();
            foreach (Data data in history)
            {
                destination.history.Add((Data)data.Clone());
            }
        }

        public void copyHistory(Touch destination, object identityFilter)
        {
            destination.history.Clear();
            foreach (Data data in history)
            {
                if (data.identity == identityFilter)
                {
                    destination.history.Add((Data)data.Clone());
                }
            }
        }
    }
}
