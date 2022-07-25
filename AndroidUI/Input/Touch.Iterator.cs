namespace AndroidUI.Input
{
    public partial class Touch
    {
        public class Iterator
        {
            Touch multiTouch;
            int index = 0;
            public Iterator(Touch multiTouch)
            {
                this.multiTouch = multiTouch;
            }
            public bool hasNext()
            {
                for (int i = index; i < multiTouch.maxSupportedTouches; i++)
                {
                    TouchContainer tc = multiTouch.touchContainerList.ElementAt(i);
                    // a container can be marked as unused but have a touch state != NONE
                    // in this case, it is either freshly removed, or freshly cancelled
                    if (tc.touch.state != State.NONE)
                    {
                        index = i;
                        return true;
                    }
                }
                return false;
            }
            public Data next()
            {
                return multiTouch.touchContainerList.ElementAt(index++).touch;
            }
            public int getIndex()
            {
                return index - 1;
            }
        };
    }
}
