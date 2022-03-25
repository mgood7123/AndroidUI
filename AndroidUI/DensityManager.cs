namespace AndroidUI
{
    public static class DensityManager
    {
        // SCREEN DENSITY
        // default value is 1
        static double screenDensity = 1.0f;

        public static double ScreenDensity => screenDensity;
        public static float ScreenDensityAsFloat => (float)screenDensity;


        // FOR INTERNAL USE
        internal static double INTERNAL_USE_ONLY_SCREEN_DENSITY_SETTER
        {
            set => screenDensity = value;
        }
    }
}
