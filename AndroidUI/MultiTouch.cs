using System.Drawing;

namespace AndroidUI
{
    using static NanoTime;

    public class Touch
    {
        private const string TAG = "MultiTouch";

        /// <summary>
        /// an Identity class useful for comparing/identifying an object by reference or value
        /// <br/>
        /// read the description for Identity.Equals for more information
        /// </summary>
        public class Identity : IEquatable<Identity>
        {
            private object identity;

            public Identity(object identity)
            {
                this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            }

            public Identity(Identity identity)
            {
                if (identity == null) throw new ArgumentNullException(nameof(identity));
                this.identity = identity.identity ?? throw new ArgumentNullException(nameof(identity) + ".identity ( THIS SHOULD NEVER HAPPEN!!! )");
            }

            /// <summary>
            /// compares the identity of this.identity and obj
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public override bool Equals(object obj)
            {
                return Equals(obj is Identity identity ? identity : new Identity(obj));
            }

            bool floating_point_type_value_equals<T>(ref object a, ref object b, Func<T, bool> isNaN, Func<T, bool> isInfinity)
            {
                T va = (T)a;
                T vb = (T)b;
                return isNaN(va) ? isNaN(vb) : isInfinity(va) ? isInfinity(vb) : va.Equals(vb);
            }

            bool type_value_equals<T>(ref object a, ref object b)
            {
                switch (a)
                {
                    case float:
                        return floating_point_type_value_equals<float>(ref a, ref b, float.IsNaN, float.IsInfinity);
                    case double:
                        return floating_point_type_value_equals<double>(ref a, ref b, double.IsNaN, double.IsInfinity);
                    default:
                        {
                            T va = (T)a;
                            T vb = (T)b;
                            return va.Equals(vb);
                        }
                }
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public bool Equals(Identity other)
            {
                Type a = identity.GetType();
                Type b = other.identity.GetType();

                if (a != b) return false;

                switch (identity)
                {
                    case short:
                        return type_value_equals<short>(ref identity, ref other.identity);
                    case int:
                        return type_value_equals<int>(ref identity, ref other.identity);
                    case long:
                        return type_value_equals<long>(ref identity, ref other.identity);
                    case float:
                        return type_value_equals<float>(ref identity, ref other.identity);
                    case double:
                        return type_value_equals<double>(ref identity, ref other.identity);
                    case char:
                        return type_value_equals<char>(ref identity, ref other.identity);
                    case string:
                        return type_value_equals<string>(ref identity, ref other.identity);
                    default:
                        bool sameInstance = (a.IsClass && b.IsClass && identity == other.identity);
                        return sameInstance || identity.Equals(other.identity);
                }
            }

            public override int GetHashCode()
            {
                return 1963189203 + EqualityComparer<object>.Default.GetHashCode(identity);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public static bool operator ==(Identity left, Identity right)
            {
                return EqualityComparer<Identity>.Default.Equals(left, right);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public static bool operator ==(Identity left, object right)
            {
                return EqualityComparer<Identity>.Default.Equals(left, right is Identity r_id ? r_id : new Identity(right));
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types matchs, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the inverse result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type
            /// then it checks the result of a instance equality via the == operator and returns false upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the inverse result of value equality comparison
            /// <br/>
            /// an inverse result of an equality comparison is the following:
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <term>Inverse Value</term>
            /// </listheader>
            /// <item>
            /// <term>True</term>
            /// <term>False</term>
            /// </item>
            /// <item>
            /// <term>False</term>
            /// <term>True</term>
            /// </item>
            /// </list>
            /// </summary>
            public static bool operator !=(Identity left, Identity right)
            {
                return !(left == right);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types matchs, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the inverse result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type
            /// then it checks the result of a instance equality via the == operator and returns false upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the inverse result of value equality comparison
            /// <br/>
            /// an inverse result of an equality comparison is the following:
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <term>Inverse Value</term>
            /// </listheader>
            /// <item>
            /// <term>True</term>
            /// <term>False</term>
            /// </item>
            /// <item>
            /// <term>False</term>
            /// <term>True</term>
            /// </item>
            /// </list>
            /// </summary>
            public static bool operator !=(Identity left, object right)
            {
                return !(left == right);
            }

            public override string ToString()
            {
                return "[ this: " + base.ToString() + ", identity: " + identity + " ]";
            }
        }

        public enum State
        {
            NONE,
            TOUCH_DOWN,
            TOUCH_MOVE,
            TOUCH_UP,
            TOUCH_CANCELLED
        };

        public class Data : ICloneable
        {
            public class Position : ICloneable
            {
                public float x, y;

                public Position(float x, float y)
                {
                    this.x = x;
                    this.y = y;
                }

                public Position()
                {
                }

                public object Clone()
                {
                    return new Position(x, y);
                }

                public override string ToString()
                {
                    return ToString(x, y);
                }

                internal static string ToString(float x, float y)
                {
                    return "" + x + ", " + y;
                }
            }
            public Identity identity;
            public long timestamp;
            public long timestamp_TOUCH_DOWN;
            public long timestamp_TOUCH_MOVE;
            public long timestamp_TOUCH_UP;
            public long timestamp_TOUCH_CANCELLED;
            public Position location;
            public bool hasLocation;
            public bool location_moved;
            public float size;
            public float pressure;
            public State state;
            public Position normalized_location_on_input_surface;
            public bool normalized_location_on_input_surface_moved;
            public bool location_moved_or_normalized_location_on_input_surface_moved;

            internal PointF MouseToPointF() => new(location.x, location.y);
            internal PointF NormalizedToPointF() => new(normalized_location_on_input_surface.y, normalized_location_on_input_surface.y);

            public Data()
                : this(0, 0, float.NaN, float.NaN, 0, 0, 1, 1, State.NONE) { }
            public Data(object identity, long timestamp_milliseconds, float x, float y, float normalized_X, float normalized_Y, State state)
                : this(identity, timestamp_milliseconds, x, y, normalized_X, normalized_Y, 1, 1, state) { }
            public Data(object identity, long timestamp_milliseconds, float x, float y, float normalized_X, float normalized_Y, float size, State state)
                : this(identity, timestamp_milliseconds, x, y, normalized_X, normalized_Y, size, 1, state) { }
            public Data(object identity, long timestamp_milliseconds, float x, float y, float normalized_X, float normalized_Y, float size, float pressure, State state)
            {
                this.identity = identity is Identity id ? id : new Identity(identity);

                this.timestamp = timestamp_milliseconds;
                timestamp_TOUCH_UP = 0;
                timestamp_TOUCH_MOVE = 0;
                timestamp_TOUCH_DOWN = 0;
                timestamp_TOUCH_CANCELLED = 0;

                if (float.IsNaN(x) || float.IsNaN(y) || float.IsInfinity(x) || float.IsInfinity(y))
                {
                    hasLocation = false;
                    location = null;
                }
                else
                {
                    hasLocation = true;
                    location = new(x, y);
                }
                location_moved = false;

                normalized_location_on_input_surface = new(normalized_X, normalized_Y);
                normalized_location_on_input_surface_moved = false;

                location_moved_or_normalized_location_on_input_surface_moved = false;

                this.size = size;
                this.pressure = pressure;
                this.state = state;
            }

            internal void resetAndKeepIdentity()
            {
                timestamp = 0;
                timestamp_TOUCH_UP = 0;
                timestamp_TOUCH_MOVE = 0;
                timestamp_TOUCH_DOWN = 0;
                timestamp_TOUCH_CANCELLED = 0;

                hasLocation = false;
                location = null;
                location_moved = false;

                normalized_location_on_input_surface = null;
                normalized_location_on_input_surface_moved = false;

                location_moved_or_normalized_location_on_input_surface_moved = false;

                size = 0;
                pressure = 0;
                state = State.NONE;
            }

            internal void resetAndLooseIdentity()
            {
                identity = null;
                resetAndKeepIdentity();
            }

            public object Clone()
            {
                Data tmp = new();

                tmp.identity = identity;

                tmp.timestamp = timestamp;
                tmp.timestamp_TOUCH_DOWN = timestamp_TOUCH_DOWN;
                tmp.timestamp_TOUCH_MOVE = timestamp_TOUCH_MOVE;
                tmp.timestamp_TOUCH_UP = timestamp_TOUCH_UP;
                tmp.timestamp_TOUCH_CANCELLED = timestamp_TOUCH_CANCELLED;

                tmp.hasLocation = hasLocation;
                if (hasLocation)
                {
                    tmp.location = (Position)location.Clone();
                }
                tmp.location_moved = location_moved;

                tmp.normalized_location_on_input_surface = (Position)normalized_location_on_input_surface.Clone();
                tmp.normalized_location_on_input_surface_moved = normalized_location_on_input_surface_moved;

                tmp.location_moved_or_normalized_location_on_input_surface_moved = location_moved_or_normalized_location_on_input_surface_moved;

                tmp.size = size;
                tmp.pressure = pressure;
                tmp.state = state;

                return tmp;
            }
        }

        public Action<Touch> onTouch = do_nothing;

        private static void do_nothing(Touch t)
        {
        }

        public bool tryForcePump()
        {
            return batcher.pump(this, true);
        }

        private class TouchContainer
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

        List<TouchContainer> data = new();

        int maxSupportedTouches = 0;
        int touchCount = 0;
        int index = 0;

        public bool debug = false;
        public bool printMoved = false;
        public bool throw_on_error = false;

        public Data getTouchAt(int index)
        {
            return data.ElementAt(index).touch;
        }

        public Data getTouchAtCurrentIndex()
        {
            return data.ElementAt(index).touch;
        }

        public int getTouchCount()
        {
            return touchCount;
        }

        public int getTouchIndex()
        {
            return index;
        }

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
                    TouchContainer tc = multiTouch.data.ElementAt(i);
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
                return multiTouch.data.ElementAt(index++).touch;
            }
            public int getIndex()
            {
                return index - 1;
            }
        };

        public Iterator getIterator()
        {
            return new Iterator(this);
        }

        private void listResize<T>(List<T> list, int size) where T : new()
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

        public void setMaxSupportedTouches(int supportedTouches)
        {
            maxSupportedTouches = supportedTouches;
            listResize(data, maxSupportedTouches);
        }

        public int getMaxSupportedTouches()
        {
            return maxSupportedTouches;
        }

        bool tryPurgeTouch(TouchContainer touchContainer)
        {
            if (
                (touchContainer.used && touchContainer.touch.state == State.TOUCH_UP)
                || (!touchContainer.used && touchContainer.touch.state != State.NONE)
            )
            {
                touchContainer.used = false;
                if (touchContainer.touch.state == State.TOUCH_CANCELLED)
                {
                    //
                    // do not reset touch count
                    // touch count is SUPPOSED to be > 0
                    //
                    // touch count can be > 0 if the index that
                    // has been reused for TOUCH_DOWN is less
                    // than the index of this TOUCH_CANCELLED
                    //
                    // before cancel, touch count 1
                    // 0: TOUCH_DOWN
                    // 1: TOUCH_MOVE
                    //
                    // after cancel, touch count 0
                    // 0: TOUCH_NONE
                    // 1: TOUCH_CANCELLED
                    //
                    // on addTouch, touch count 1
                    // 0: TOUCH_DOWN
                    // 1: TOUCH_CANCELLED
                    //
                    //
                    touchContainer.touch.resetAndKeepIdentity();
                }
                else
                {
                    touchContainer.touch.resetAndKeepIdentity();
                    if (touchCount > 0)
                    {
                        touchCount--;
                    }
                    else
                    {
                        if (throw_on_error)
                        {
                            throw new InvalidOperationException("touch cannot be purged with a touch count of zero");
                        }
                        else
                        {
                            Console.WriteLine("touch cannot be purged with a touch count of zero, cancelling touch");
                            cancelTouch();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void addTouch(Data touchData)
        {
            tryForcePump();
            if (debug) Console.WriteLine("adding touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                if (!tryPurgeTouch(touchContainer)) found = true;
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
            addTouch(identity, currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void addTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            addTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_DOWN));
        }

        bool batching = false;

        List<Data> history = new();

        Data getHistoryTouch(int historyIndex)
        {
            return history.ElementAt(historyIndex);
        }

        int getHistorySize => history.Count;

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

        public void moveTouch(Data touchData)
        {
            if (debug && printMoved) Console.WriteLine("moving touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                if (!tryPurgeTouch(touchContainer)) found = true;
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
            moveTouch(identity, currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
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
            return moveTouchBatched(identity, currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public bool moveTouchBatched(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            return moveTouchBatched(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_MOVE));
        }

        public void removeTouch(Data touchData)
        {
            tryForcePump();
            if (debug) Console.WriteLine("removing touch with identity: " + touchData.identity);
            bool found = false;
            for (int i = 0; i < maxSupportedTouches; i++)
            {
                TouchContainer touchContainer = data.ElementAt(i);
                if (!tryPurgeTouch(touchContainer)) found = true;
                if (!found && touchContainer.used)
                {
                    if (touchContainer.touch.identity == touchData.identity)
                    {
                        found = true;
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
            removeTouch(identity, currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
        }
        public void removeTouch(object identity, long time, float x, float y, float normalized_X, float normalized_Y, float size, float pressure)
        {
            removeTouch(new Data(identity, time, x, y, normalized_X, normalized_Y, size, pressure, State.TOUCH_UP));
        }

        public void cancelTouch()
        {
            tryForcePump();
            long timestamp = currentTimeMillis();
            // cancel the first touch
            if (maxSupportedTouches <= 0)
            {
                Console.WriteLine("the maximum number of supported touches must be greater than zero");
            }
            else
            {
                TouchContainer touchContainer = data.ElementAt(0);
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
                    TouchContainer touchContainer_ = data[i];
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
                TouchContainer touchContainer = data[i];
                // ignore touch identity since we are cancelling a touch
                // the identity may not match at all
                if (touchContainer.used)
                {
                    if (!found)
                    {
                        found = true;
                        touchContainer = data[i];

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
                    TouchContainer touchContainer = data[0];
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
            cancelTouch(identity, currentTimeMillis(), x, y, normalized_X, normalized_Y, size, pressure);
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
                TouchContainer touchContainer = data.ElementAt(touchIndex);
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

        object gSharedTempLock = new();

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
                ev.setMaxSupportedTouches(oldPointerCount);

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
                            TouchContainer touchContainer = ev.data.ElementAt(i_);
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

        internal class Batcher
        {
            internal Queue<Data> events = new();
            long batchTime = 0;
            long pumpTime = 0;
            long batch_time_ms = 20;
            bool pumpActive = false;

            internal void addBatch(Data touchData)
            {
                if (events.Count == 0)
                {
                    batchTime = currentTimeMillis();
                }
                events.Enqueue(touchData);
            }

            internal bool pump(in Touch touch)
            {
                return pump(touch, false);
            }

            internal bool pump(in Touch touch, bool force_pump)
            {
                if (pumpActive)
                {
                    throw new Exception("Attempting to pump while already pumping a batch");
                }

                pumpActive = true;
                int c = events.Count;
                pumpTime = currentTimeMillis();

                bool handled = false;

                if (c != 0 && (force_pump || (pumpTime - batchTime) >= batch_time_ms))
                {
                    if (touch.debug) Console.WriteLine("batch time : " + batchTime);
                    if (touch.debug) Console.WriteLine("pump time  : " + pumpTime);

                    string s = c == 1 ? "" : "s";

                    if (touch.debug) Console.WriteLine("batched " + c + " event" + s);
                    try
                    {
                        touch.batching = true;
                        while (events.Count > 1)
                        {
                            touch.moveTouch(events.Dequeue());
                        }
                        touch.batching = false;
                        touch.moveTouch(events.Dequeue());
                    } catch (Exception e) {
                        touch.batching = false;
                        touch.history.Clear();
                        pumpActive = false;
                        // rethrow exception and hope we can be recovered earlier up
                        throw e;
                    }
                    if (touch.debug) Console.WriteLine("processed " + c + " queued event" + s);
                    handled = true;
                }
                pumpActive = false;
                return handled;
            }
        }
    }
}
