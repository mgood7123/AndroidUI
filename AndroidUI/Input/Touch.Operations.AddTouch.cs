using AndroidUI.OS;

namespace AndroidUI
{
    public partial class Touch
    {
        public void addTouch(Data touchData)
        {
            tryForcePump();
            if (debug) Console.WriteLine("adding touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = touchContainerList.ElementAt(i);
                if (!TryPurgeTouch(touchContainer)) found = true;
                if (!found && !touchContainer.used)
                {
                    found = true;
                    // when a touch is added all timestamps should be reset
                    touchContainer.touch = touchData;
                    touchContainer.touch.state = State.TOUCH_DOWN;
                    touchContainer.touch.timestamp_TOUCH_DOWN = touchData.timestamp;
                    touchContainer.used = true;
                    touchCount++;
                    index = i;
                    if (debug) Console.WriteLine("TOUCH_DOWN: INDEX: " + i);
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException(
                            "the maximum number of supported touches of " +
                            maxSupportedTouches + " has been reached.\n" +
                            "please call setMaxSupportedTouches(long)"
                    );
                }
                else
                {
                    Console.WriteLine(
                            "the maximum number of supported touches of " +
                            maxSupportedTouches + " has been reached.\n" +
                            "please call setMaxSupportedTouches(long), cancelling touch"
                    );
                }
            }
            else
            {
                onTouch.Invoke(this);
            }
        }

        public void addTouch(object identity, float x, float y, float normalized_X, float normalized_Y)
        {
            addTouch(identity, x, y, normalized_X, normalized_Y, 1, 1);
        }
        public void addTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size)
        {
            addTouch(identity, x, y, normalized_X, normalized_Y, size, 1);
        }
        public void addTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            addTouch(identity, NanoTime.currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void addTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            addTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_DOWN));
        }
    }
}
