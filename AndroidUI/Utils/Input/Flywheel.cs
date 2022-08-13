using AndroidUI.Extensions;

namespace AndroidUI.Utils.Input
{
    public class Flywheel
    {
        struct PositionInfo
        {
            public long timestamp;
            public FloatingPointPair<float> pos;

            public PositionInfo(long time, float x, float y)
            {
                timestamp = time;
                pos = new(x, y);
            }
        }

        public const float DEFAULT_FRICTION = 1.0f;

        readonly object LOCK = new();

        class Info : ICloneable
        {
            public Lists.RingBuffer<PositionInfo> pos = new(2);
            public FloatingPointPair<float> distance, totalDistance, velocity, position;
            public long startTime, endTime, timestamp, time_since_last_movement;
            public bool spinning;

            public Info Clone()
            {
                Info c = (Info)ICloneable.Clone(this);
                c.pos = new(pos.Capacity);
                c.pos.AddRange(pos);
                c.distance = distance;
                c.totalDistance = totalDistance;
                c.velocity = velocity;
                c.position = position;
                c.startTime = startTime;
                c.endTime = endTime;
                c.timestamp = timestamp;
                c.time_since_last_movement = time_since_last_movement;
                c.spinning = spinning;
                return c;
            }
        }

        Info current;

        readonly Queue<Info> queue;

        int maxHistory = 1;

        public Flywheel()
        {
            current = new Info();
            queue = new(Arrays.Arrays.AsArray(current));
        }

        void push()
        {
            if (maxHistory > 1)
            {
                if (queue.Count == maxHistory)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(current);
                current = current.Clone();
            }
        }

        public bool Undo()
        {
            if (queue.Count > 1)
            {
                queue.Dequeue();
                current = queue.Peek();
                return true;
            }
            return false;
        }

        private float friction = DEFAULT_FRICTION;
        public bool Spinning => current.spinning;

        public FloatingPointPair<float> Distance => current.distance;
        public FloatingPointPair<float> TotalDistance => current.totalDistance;
        public FloatingPointPair<float> Velocity
        {
            get => current.velocity; set
            {
                current.velocity = value;
                if (current.velocity == FloatingPointPair<float>.Zero)
                {
                    current.spinning = false;
                }
            }
        }

        public FloatingPointPair<float> Position { get => current.position; set => current.position = value; }

        public long TimeSinceLastMovement => current.time_since_last_movement;

        public long SpinTime => current.pos.Count >= 2 ? (current.endTime - current.startTime) : 0;

        public float Friction { get => friction; set => friction = value; }
        public int MaxHistory { get => maxHistory; set => maxHistory = value; }

        public void Spin()
        {
            if (current.spinning)
            {
                bool pushed = false;
                if (current.velocity.First > 0f)
                {
                    push();
                    pushed = true;
                    current.velocity.First -= friction;
                    if (current.velocity.First < 0f)
                    {
                        current.velocity.First = 0f;
                    }
                }
                else if (current.velocity.First < 0f)
                {
                    push();
                    pushed = true;
                    current.velocity.First += friction;
                    if (current.velocity.First > 0f)
                    {
                        current.velocity.First = 0f;
                    }
                }

                if (current.velocity.Second > 0f)
                {
                    if (!pushed)
                    {
                        push();
                        pushed = true;
                    }
                    current.velocity.Second -= friction;
                    if (current.velocity.Second < 0f)
                    {
                        current.velocity.Second = 0f;
                    }
                }
                else if (current.velocity.Second < 0f)
                {
                    if (!pushed)
                    {
                        push();
                        pushed = true;
                    }
                    current.velocity.Second += friction;
                    if (current.velocity.Second > 0f)
                    {
                        current.velocity.Second = 0f;
                    }
                }

                if (!pushed)
                {
                    push();
                    pushed = true;
                }
                current.position += current.velocity;
                if (current.velocity == FloatingPointPair<float>.Zero)
                {
                    current.spinning = false;
                }
            }
        }

        public void AquireLock()
        {
            LOCK.Lock();
        }

        public void ReleaseLock()
        {
            LOCK.Unlock();
        }

        public void Reset()
        {
            ResetButKeepPosition();
            current.position = FloatingPointPair<float>.Zero;
        }

        public void ResetButKeepPosition()
        {
            push();
            current.pos.Clear();
            current.distance = FloatingPointPair<float>.Zero;
            current.totalDistance = FloatingPointPair<float>.Zero;
            current.velocity = FloatingPointPair<float>.Zero;
            current.startTime = 0;
            current.endTime = 0;
            current.timestamp = 0;
            current.time_since_last_movement = 0;
            current.spinning = false;
        }

        public void AddMovement(long timestamp, float x, float y)
        {
            push();
            long last_time_previous = current.timestamp;
            current.timestamp = timestamp;
            if (current.pos.Count == 0)
            {
                current.startTime = timestamp;
            }
            current.pos.Add(new(timestamp, x, y));
            int count = current.pos.Count;
            if (count >= 2)
            {
                current.endTime = timestamp;
                current.time_since_last_movement = last_time_previous == 0 ? 0 : (timestamp - last_time_previous);
                current.distance = current.pos[count - 1].pos - current.pos[0].pos;
                current.position += current.distance;
                current.totalDistance += current.distance;
                current.velocity = current.distance;
            }
        }

        public void FinalizeMovement()
        {
            push();
            current.spinning = true;
        }
    }
}
