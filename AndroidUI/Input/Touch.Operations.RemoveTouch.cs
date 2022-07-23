using AndroidUI.OS;

namespace AndroidUI
{
    public partial class Touch
    {
        public void removeTouch(Data touchData)
        {
            tryForcePump();
            if (debug) Console.WriteLine("removing touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = touchContainerList.ElementAt(i);
                if (!TryPurgeTouch(touchContainer)) found = true;
                if (!found && touchContainer.used)
                {
                    if (touchContainer.touch.identity == touchData.identity)
                    {
                        found = true;
                        Data previous = touchContainer.touch;

                        if (touchData.hasLocation)
                        {
                            // touch up must always be preceeded by a move event if the location has moved
                            if (previous.location.x != touchData.location.x || previous.location.y != touchData.location.y)
                            {
                                moveTouch(touchData);
                                touchData.timestamp = NanoTime.currentTimeMillis();
                            }

                            touchData.location_moved = false;
                        }

                        touchData.normalized_location_on_input_surface_moved = previous.normalized_location_on_input_surface.x != touchData.normalized_location_on_input_surface.x || previous.normalized_location_on_input_surface.y != touchData.normalized_location_on_input_surface.y;
                        touchData.location_moved_or_normalized_location_on_input_surface_moved = touchData.location_moved || touchData.normalized_location_on_input_surface_moved;

                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_TOUCH_DOWN = previous.timestamp_TOUCH_DOWN;
                        touchContainer.touch.timestamp_TOUCH_MOVE = previous.timestamp_TOUCH_MOVE;
                        touchContainer.touch.timestamp_TOUCH_UP = touchData.timestamp;
                        touchContainer.touch.timestamp_TOUCH_CANCELLED = previous.timestamp_TOUCH_CANCELLED;
                        index = i;
                        if (debug) Console.WriteLine("TOUCH_UP: INDEX: " + i);
                    }
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException("cannot remove a touch that has not been registered");
                }
                else
                {
                    Console.WriteLine("cannot remove a touch that has not been registered, cancelling touch");
                    cancelTouch();
                }
            }
            else
            {
                onTouch.Invoke(this);
            }
        }

        public void removeTouch(object identity, float x, float y, float normalized_X, float normalized_Y)
        {
            removeTouch(identity, x, y, normalized_X, normalized_Y, 1, 1);
        }
        public void removeTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size)
        {
            removeTouch(identity, x, y, normalized_X, normalized_Y, size, 1);
        }
        public void removeTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            removeTouch(identity, NanoTime.currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void removeTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            removeTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_UP));
        }
    }
}
