using AndroidUI.OS;

namespace AndroidUI
{
    public partial class Touch
    {
        public void moveTouch(Data touchData)
        {
            if (debug && printMoved) Console.WriteLine("moving touch with identity: " + touchData.identity);
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

                        // always update this container's data so that the current batch is correctly updated

                        Data previous = touchContainer.touch;

                        if (touchData.hasLocation)
                        {
                            touchData.location_moved = previous.location.x != touchData.location.x && previous.location.y != touchData.location.y;
                        }
                        touchData.normalized_location_on_input_surface_moved = previous.normalized_location_on_input_surface.x != touchData.normalized_location_on_input_surface.x || previous.normalized_location_on_input_surface.y != touchData.normalized_location_on_input_surface.y;
                        touchData.location_moved_or_normalized_location_on_input_surface_moved = touchData.location_moved || touchData.normalized_location_on_input_surface_moved;

                        touchContainer.touch = touchData;
                        touchContainer.touch.timestamp_TOUCH_DOWN = previous.timestamp_TOUCH_DOWN;
                        touchContainer.touch.timestamp_TOUCH_MOVE = touchData.timestamp;
                        touchContainer.touch.timestamp_TOUCH_UP = previous.timestamp_TOUCH_UP;
                        touchContainer.touch.timestamp_TOUCH_CANCELLED = previous.timestamp_TOUCH_CANCELLED;
                        index = i;

                        if (batching)
                        {
                            history.Add(touchContainer.touch);
                        }

                        if (debug && printMoved) Console.WriteLine("TOUCH_MOVE: INDEX: " + i);
                    }
                }
            }
            if (!found)
            {
                if (throw_on_error)
                {
                    throw new InvalidOperationException("cannot move a touch that has not been registered");
                }
                else
                {
                    Console.WriteLine("cannot move a touch that has not been registered, cancelling touch");
                    cancelTouch();
                }
            }
            else
            {
                if (!batching)
                {
                    onTouch.Invoke(this);
                    history.Clear();
                }
            }
        }

        public void moveTouch(object identity, float x, float y, float normalized_X, float normalized_Y)
        {
            moveTouch(identity, x, y, normalized_X, normalized_Y, 1, 1);
        }
        public void moveTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size)
        {
            moveTouch(identity, x, y, normalized_X, normalized_Y, size, 1);
        }
        public void moveTouch(object identity, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            moveTouch(identity, NanoTime.currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void moveTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            moveTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_MOVE));
        }

        public bool moveTouchBatched(Data touchData)
        {
            batcher.addBatch(touchData);
            return batcher.pump(this);
        }

        public bool moveTouchBatched(object identity, float x, float y, float normalized_X, float normalized_Y)
        {
            return moveTouchBatched(identity, x, y, normalized_X, normalized_Y, 1, 1);
        }
        public bool moveTouchBatched(object identity, float x, float y, float normalized_X, float normalized_Y, float size)
        {
            return moveTouchBatched(identity, x, y, normalized_X, normalized_Y, size, 1);
        }
        public bool moveTouchBatched(object identity, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            return moveTouchBatched(identity, NanoTime.currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public bool moveTouchBatched(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            return moveTouchBatched(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_MOVE));
        }
    }
}
