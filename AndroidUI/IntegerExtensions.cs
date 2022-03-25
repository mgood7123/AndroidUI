namespace AndroidUI
{
    public static class IntegerExtensions
    {
        public static int dipToPx(this int dip)
        {
            return (int)(DensityManager.ScreenDensityAsFloat * dip + 0.5f);
        }
    }
}
