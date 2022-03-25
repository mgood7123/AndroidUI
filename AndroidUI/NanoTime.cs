namespace AndroidUI
{
    public static class NanoTime
    {
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
