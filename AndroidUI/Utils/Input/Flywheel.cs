using AndroidUI.Extensions;
using System.Numerics;

namespace AndroidUI.Utils.Input
{
    public class Flywheel
    {
        struct PositionInfo
        {
            public long timestamp;
            public Vector2 pos;

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
            public Vector2 distance, totalDistance, velocity, position;
            public long startTime, endTime;
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

        public Vector2 Distance => current.distance;
        public Vector2 TotalDistance => current.totalDistance;
        public Vector2 Velocity
        {
            get => current.velocity; set
            {
                current.velocity = value;
                if (current.velocity == Vector2.Zero)
                {
                    current.spinning = false;
                }
            }
        }
        public Vector2 Position { get => current.position; set => current.position = value; }

        public long SpinTime => current.pos.Count >= 2 ? (current.endTime - current.startTime) : 0;

        public float Friction { get => friction; set => friction = value; }
        public int MaxHistory { get => maxHistory; set => maxHistory = value; }

        public void Spin()
        {
            if (current.spinning)
            {
                bool pushed = false;
                if (current.velocity.X > 0)
                {
                    push();
                    pushed = true;
                    current.velocity.X -= friction;
                    if (current.velocity.X < 0)
                    {
                        current.velocity.X = 0;
                    }
                }
                else if (current.velocity.X < 0)
                {
                    push();
                    pushed = true;
                    current.velocity.X += friction;
                    if (current.velocity.X > 0)
                    {
                        current.velocity.X = 0;
                    }
                }

                if (current.velocity.Y > 0)
                {
                    if (!pushed)
                    {
                        push();
                        pushed = true;
                    }
                    current.velocity.Y -= friction;
                    if (current.velocity.Y < 0)
                    {
                        current.velocity.Y = 0;
                    }
                }
                else if (current.velocity.Y < 0)
                {
                    if (!pushed)
                    {
                        push();
                        pushed = true;
                    }
                    current.velocity.Y += friction;
                    if (current.velocity.Y > 0)
                    {
                        current.velocity.Y = 0;
                    }
                }

                if (!pushed)
                {
                    push();
                    pushed = true;
                }
                current.position += current.velocity;
                if (current.velocity == Vector2.Zero)
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
            push();
            current.pos.Clear();
            current.distance = Vector2.Zero;
            current.totalDistance = Vector2.Zero;
            current.velocity = Vector2.Zero;
            current.position = Vector2.Zero;
            current.startTime = 0;
            current.endTime = 0;
            current.spinning = false;
        }

        public void ResetButKeepPosition()
        {
            push();
            current.pos.Clear();
            current.distance = Vector2.Zero;
            current.totalDistance = Vector2.Zero;
            current.velocity = Vector2.Zero;
            current.startTime = 0;
            current.endTime = 0;
            current.spinning = false;
        }

        public void AddMovement(long timestamp, float x, float y)
        {
            push();
            if (current.pos.Count == 0)
            {
                current.startTime = timestamp;
            }
            current.pos.Add(new(timestamp, x, y));
            int count = current.pos.Count;
            if (count >= 2)
            {
                current.endTime = timestamp;
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
