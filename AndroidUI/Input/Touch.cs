using AndroidUI.Utils.Lists;

namespace AndroidUI
{
    public partial class Touch
    {
        public enum State
        {
            NONE,
            TOUCH_DOWN,
            TOUCH_MOVE,
            TOUCH_UP,
            TOUCH_CANCELLED
        };

        public Action<Touch> onTouch = do_nothing;

        private static void do_nothing(Touch t)
        {
        }

        public bool tryForcePump()
        {
            return batcher.pump(this, true);
        }

        internal List<TouchContainer> touchContainerList = new();

        internal int maxSupportedTouches = 0;
        internal int touchCount = 0;
        internal int index = 0;

        public bool debug;
        public bool printMoved;
        public bool throw_on_error;

        public Data getTouchAt(int index)
        {
            return touchContainerList.ElementAt(index).touch;
        }

        public Data getTouchAtCurrentIndex()
        {
            return touchContainerList.ElementAt(index).touch;
        }

        public int getTouchCount()
        {
            return touchCount;
        }

        public int getTouchIndex()
        {
            return index;
        }

        public Iterator getIterator()
        {
            return new Iterator(this);
        }

        public static void listResize<T>(List<T> list, int size) where T : new()
        {
            if (size == 0)
            {
                list.Clear();
            }
            else
            {
                if (size > list.Count)
                {
                    while (list.Count < size)
                    {
                        list.Add(new T());
                    }
                }
                else if (size < list.Count)
                {
                    while (list.Count > size)
                    {
                        list.RemoveAt(list.Count - 1);
                    }
                }
            }
        }

        public int MaxSupportedTouches
        {
            get => maxSupportedTouches;
            set
            {
                maxSupportedTouches = value;
                listResize(touchContainerList, maxSupportedTouches);
            }
        }

        internal bool batching = false;

        public static string StateToString(State state)
        {
            switch (state)
            {
                case State.NONE: return "NONE";
                case State.TOUCH_DOWN: return "TOUCH_DOWN";
                case State.TOUCH_MOVE: return "TOUCH_MOVE";
                case State.TOUCH_UP: return "TOUCH_UP";
                default: return "TOUCH_CANCELLED";
            }
        }

        public override string ToString()
        {
            string s = "";
            s += "touch count : " + touchCount;
            s += ", current index : " + index;
            for (int touchIndex = 0; touchIndex < maxSupportedTouches; touchIndex++)
            {
                TouchContainer touchContainer = touchContainerList.ElementAt(touchIndex);
                Data touch = touchContainer.touch;
                if (touch.state != State.NONE)
                {
                    s += "\n touch index : " + touchIndex;
                    if (touchIndex == index) s += " [CURRENT]";
                    if (touchContainer.used)
                    {
                        s += ", used : " + (touchContainer.used ? "True" : "False");
                    }
                    else
                    {
                        s += ", used : " + (touchContainer.used ? "True" : "False");
                    }
                    s += ", action : " + StateToString(touch.state);
                    s += ", identity : " + touch.identity;
                    s += ", timestamp : " + touch.timestamp;
                    //s += ", timestamp (TOUCH_DOWN) : " + touch.timestamp_TOUCH_DOWN;
                    //s += ", timestamp (TOUCH_MOVE) : " + touch.timestamp_TOUCH_MOVE;
                    //s += ", timestamp (TOUCH_UP) : " + touch.timestamp_TOUCH_UP;
                    //s += ", timestamp (TOUCH_CANCELLED) : " + touch.timestamp_TOUCH_CANCELLED;
                    s += ", has location : " + (touch.hasLocation ? "True" : "False");
                    if (touch.hasLocation)
                    {
                        s += ", x : " + touch.location.x;
                        s += ", y : " + touch.location.y;
                        //s += ", did touch location move : " + (touch.location_moved ? "True" : "False");
                    }
                    //s += ", normalized location on input surface x : " + touch.normalized_location_on_input_surface.x;
                    //s += ", normalized location on input surface y : " + touch.normalized_location_on_input_surface.y;
                    //s += ", did normalized location on input surface move : " + (touch.normalized_location_on_input_surface_moved ? "True" : "False");
                    //s += ", did touch location or normalized location on input surface move : " + (touch.location_moved_or_normalized_location_on_input_surface_moved ? "True" : "False");
                    //s += ", size : " + touch.size;
                    //s += ", pressure : " + touch.pressure;
                }
            }
            return s;
        }

        /**
         * Gets an integer where each pointer id present in the event is marked as a bit.
         */
        internal BitwiseList<object> getPointerIdBits()
        {
            BitwiseList<object> idBits = new();
            Iterator i = getIterator();
            while (i.hasNext())
            {
                idBits |= i.next().identity;
            }
            return idBits;
        }

        internal void offsetLocation(float offsetX, float offsetY)
        {
            if (offsetX == 0 && offsetY == 0) return;

            Iterator i = getIterator();
            while (i.hasNext())
            {
                Data data = i.next();
                if (data.hasLocation)
                {
                    data.location.x += offsetX;
                    data.location.y += offsetY;
                }
            }
        }

        internal object gSharedTempLock = new();

        /**
         * Splits a motion event such that it includes only a subset of pointer ids.
         * @hide
         */
        internal Touch split(BitwiseList<object> idBits)
        {
            Touch ev = new();
            lock (gSharedTempLock)
            {
                int oldPointerCount = getTouchCount();
                ev.MaxSupportedTouches = oldPointerCount;

                var oldState = getTouchAtCurrentIndex().state;

                int newActionPointerIndex = -1;
                int newPointerCount = 0;

                Iterator i = getIterator();
                while (i.hasNext())
                {
                    Data data = i.next();
                    if ((idBits & data.identity) != BitwiseList<object>.ZERO)
                    {
                        bool cancelled = false;
                        for (int i_ = 0; i_ < ev.maxSupportedTouches; i_++)
                        {
                            TouchContainer touchContainer = ev.touchContainerList.ElementAt(i_);
                            touchContainer.used = true;
                            touchContainer.touch = (Data)data.Clone();
                            ev.index = i_;
                            if (touchContainer.touch.state == State.TOUCH_CANCELLED)
                            {
                                cancelled = true;
                            }
                            if (cancelled)
                            {
                                ev.touchCount = 0;
                            }
                            else
                            {
                                ev.touchCount++;
                            }
                        }
                    }
                }

                if (ev.touchCount == 0)
                {
                    throw new Exception("idBits did not match any ids in the event");
                }

                return ev;
            }
        }

        Batcher batcher = new();
    }
}
