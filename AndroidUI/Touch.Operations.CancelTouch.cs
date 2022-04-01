namespace AndroidUI
{
    public partial class Touch
    {
        public void cancelTouch()
        {
            tryForcePump();
            long timestamp = NanoTime.currentTimeMillis();
            // cancel the first touch
            if (maxSupportedTouches <= 0)
            {
                Console.WriteLine("the maximum number of supported touches must be greater than zero");
            }
            else
            {
                TouchContainer touchContainer = touchContainerList.ElementAt(0);
                touchContainer.touch.location_moved = false;
                touchContainer.touch.normalized_location_on_input_surface_moved = false;
                touchContainer.touch.location_moved_or_normalized_location_on_input_surface_moved = false;
                touchContainer.touch.state = State.TOUCH_CANCELLED;
                touchContainer.touch.timestamp = timestamp;
                touchContainer.touch.timestamp_TOUCH_CANCELLED = timestamp;
                touchContainer.used = false;
                // invalidate all others
                for (int i = 1; i < maxSupportedTouches; i++)
                {
                    TouchContainer touchContainer_ = touchContainerList[i];
                    touchContainer_.used = false;
                    touchContainer_.touch.resetAndKeepIdentity();
                }
            }
            index = 0;
            if (debug) Console.WriteLine("TOUCH_CANCEL: INDEX: " + 0);
            touchCount = 0;
            onTouch.Invoke(this);
        }

        public void cancelTouch(Data touchData)
        {
            tryForcePump();
            if (debug) Console.WriteLine("cancelling touch");
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = touchContainerList[i];
                // ignore touch identity since we are cancelling a touch
                // the identity may not match at all
                if (touchContainer.used)
                {
                    if (!found)
                    {
                        found = true;
                        touchContainer = touchContainerList[i];

                        Data previous = touchContainer.touch;

                        if (touchData.hasLocation)
                        {
                            touchData.location_moved = previous.location.x != touchData.location.x && previous.location.y != touchData.location.y;
                        }
                        touchData.normalized_location_on_input_surface_moved = previous.normalized_location_on_input_surface.x != touchData.normalized_location_on_input_surface.x || previous.normalized_location_on_input_surface.y != touchData.normalized_location_on_input_surface.y;
                        touchData.location_moved_or_normalized_location_on_input_surface_moved = touchData.location_moved || touchData.normalized_location_on_input_surface_moved;

                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_TOUCH_DOWN = previous.timestamp_TOUCH_DOWN;
                        touchContainer.touch.timestamp_TOUCH_MOVE = previous.timestamp_TOUCH_MOVE;
                        touchContainer.touch.timestamp_TOUCH_UP = previous.timestamp_TOUCH_UP;
                        touchContainer.touch.timestamp_TOUCH_CANCELLED = touchData.timestamp;
                        touchContainer.used = false;
                        index = i;
                        if (debug) Console.WriteLine("TOUCH_CANCEL: INDEX: " + i);
                    }
                    else
                    {
                        touchContainer.used = false;
                        touchContainer.touch.resetAndKeepIdentity();
                    }
                }
            }

            if (!found)
            {
                // if not found, cancel the first touch
                if (maxSupportedTouches <= 0)
                {
                    Console.WriteLine("the maximum number of supported touches must be greater than zero");
                }
                else
                {
                    TouchContainer touchContainer = touchContainerList[0];
                    Data previous = touchContainer.touch;

                    if (touchData.hasLocation)
                    {
                        touchData.location_moved = previous.location.x != touchData.location.x && previous.location.y != touchData.location.y;
                    }
                    touchData.normalized_location_on_input_surface_moved = previous.normalized_location_on_input_surface.x != touchData.normalized_location_on_input_surface.x || previous.normalized_location_on_input_surface.y != touchData.normalized_location_on_input_surface.y;
                    touchData.location_moved_or_normalized_location_on_input_surface_moved = touchData.location_moved || touchData.normalized_location_on_input_surface_moved;

                    touchContainer.touch = touchData;
                    touchContainer.touch.timestamp_TOUCH_DOWN = previous.timestamp_TOUCH_DOWN;
                    touchContainer.touch.timestamp_TOUCH_MOVE = previous.timestamp_TOUCH_MOVE;
                    touchContainer.touch.timestamp_TOUCH_UP = previous.timestamp_TOUCH_UP;
                    touchContainer.touch.timestamp_TOUCH_CANCELLED = touchData.timestamp;
                    touchContainer.touch.state = State.TOUCH_CANCELLED;
                    touchContainer.used = false;
                }
                index = 0;
            }
            touchCount = 0;
            onTouch.Invoke(this);
        }

        public void cancelTouch(object identity, float x, float y, float normalized_X, float normalized_Y)
        {
            cancelTouch(identity, x, y, normalized_X, normalized_Y, 1, 1);
        }
        public void cancelTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size)
        {
            cancelTouch(identity, x, y, normalized_X, normalized_Y, size, 1);
        }
        public void cancelTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            cancelTouch(identity, NanoTime.currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void cancelTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            cancelTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_CANCELLED));
        }

        public static Touch generateCancelTouch()
        {
            Touch touch = new();
            touch.cancelTouch();
            return touch;
        }
    }
}
