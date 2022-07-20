using System.Drawing;

namespace AndroidUI
{
    public partial class Touch
    {
        public partial class Data : ICloneable
        {
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

                timestamp = timestamp_milliseconds;
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
    }
}
