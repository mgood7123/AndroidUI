namespace AndroidUI.Input
{
    public partial class Touch
    {
        internal class TouchContainer
        {
            public bool used;
            public Data touch;
            public TouchContainer()
            {
                used = false;
                touch = new Data();
            }
            public TouchContainer(bool used, Data touch)
            {
                this.used = used;
                this.touch = touch;
            }
        };
    }
}
