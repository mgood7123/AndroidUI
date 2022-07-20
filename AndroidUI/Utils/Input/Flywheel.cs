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

        public const float DEFAULT_FRICTION = 0.00042f;

        object l = new();
        Lists.RingBuffer<PositionInfo> pos = new(2);
        Vector2 distance, totalDistance, velocity, position;
        long startTime, endTime;
        bool spinning;

        private float friction = DEFAULT_FRICTION;
        public bool Spinning => spinning;

        public Vector2 Distance => distance;
        public Vector2 TotalDistance => totalDistance;
        public Vector2 Velocity => velocity;
        public Vector2 Position => position;

        public long SpinTime => endTime - startTime;

        public float Friction { get => friction; set => friction = value; }

        public void Spin()
        {
            if (spinning)
            {
                if (velocity.X > 0)
                {
                    velocity.X -= friction;
                    if (velocity.X < 0)
                    {
                        velocity.X = 0;
                    }
                }
                else if (velocity.X < 0)
                {
                    velocity.X += friction;
                    if (velocity.X > 0)
                    {
                        velocity.X = 0;
                    }
                }

                if (velocity.Y > 0)
                {
                    velocity.Y -= friction;
                    if (velocity.Y < 0)
                    {
                        velocity.Y = 0;
                    }
                }
                else if (velocity.Y < 0)
                {
                    velocity.Y += friction;
                    if (velocity.Y > 0)
                    {
                        velocity.Y = 0;
                    }
                }

                position += velocity;
                if (velocity == Vector2.Zero)
                {
                    spinning = false;
                }
            }
        }

        public void AquireLock()
        {
            l.Lock();
        }

        public void ReleaseLock()
        {
            l.Unlock();
        }

        public void Reset()
        {
            pos.Clear();
            distance = Vector2.Zero;
            totalDistance = Vector2.Zero;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
            startTime = 0;
            endTime = 0;
            spinning = false;
        }

        public void AddMovement(long timestamp, float x, float y)
        {
            if (pos.Count == 0)
            {
                startTime = timestamp;
            }
            pos.Add(new(timestamp, x, y));
            int count = pos.Count;
            if (count >= 2)
            {
                endTime = timestamp;
                distance = pos[count - 1].pos - pos[0].pos;
                position += distance;
                totalDistance += distance;
                long time = pos[count - 1].timestamp - pos[0].timestamp;
                velocity = distance / time;
            }
        }

        public void FinalizeMovement()
        {
            spinning = true;
        }
    }
}
