﻿namespace AndroidUI.OS
{
    public static class NanoTime
    {
        public const long NANOS_PER_MS = 1000000;

        public static long currentTimeNanos()
        {
            long nano = 10000L * System.Diagnostics.Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        public static long currentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
