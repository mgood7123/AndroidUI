namespace AndroidUI
{
    public static class MathUtils
    {
        public static short Clamp(this short value, short min, short max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static long Clamp(this long value, long min, long max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static double Clamp(this double value, double min, double max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        public static bool IsFinite(this float value)
        {
            return !(float.IsNaN(value) && float.IsInfinity(value));
        }

        public static bool IsFinite(this double value)
        {
            return !(double.IsNaN(value) && double.IsInfinity(value));
        }
    }
}