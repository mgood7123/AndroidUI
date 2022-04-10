namespace AndroidUI
{
    public static class DensityManager
    {
        // SCREEN DENSITY
        // default value is 1
        static double screenDensity = 1.0f;
        static int screenDpi;

        public static double ScreenDensity => screenDensity;
        public static float ScreenDensityAsFloat => (float)screenDensity;
        public static int ScreenDpi => screenDpi;


        // FOR INTERNAL USE
        internal static void INTERNAL_USE_ONLY____SET_DENSITY(double density, int dpi)
        {
            screenDensity = density;
            screenDpi = dpi;
        }
    }
}
