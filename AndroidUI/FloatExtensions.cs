namespace AndroidUI
{
    public static class FloatExtensions
    {
        public static int toPixel(this float pixelF)
        {
            return (int)Math.Ceiling(pixelF);
        }
    }
}
