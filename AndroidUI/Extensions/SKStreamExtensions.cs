using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKStreamExtensions
    {
        public static bool ReadFloat(this SKStream stream, out float buffer)
        {
            buffer = 0.0f;
            uint tmp;
            if (!stream.ReadUInt32(out tmp))
            {
                return false;
            }

            buffer = tmp.BitsToFloat();
            return true;
        }
    }
}