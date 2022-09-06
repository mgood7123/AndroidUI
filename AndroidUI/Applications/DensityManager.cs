namespace AndroidUI.Applications
{
    public class DensityManager
    {
        public const float DEFAULT_SCREEN_DENSITY = 1.0f;
        public const int DEFAULT_SCREEN_DPI = 96;

        private float screenDensity;
        private int screenDpi;

        public float ScreenDensity => screenDensity;
        public int ScreenDpi => screenDpi;

        public int ConvertDPToPX(int dp, float offset = 0f)
        {
            return (int)((dp * ScreenDensity) + offset);
        }

        public int ConvertPXToDP(int px, float offset = 0f)
        {
            return (int)((px / ScreenDensity) - offset);
        }

        public DensityManager() : this(DEFAULT_SCREEN_DENSITY, DEFAULT_SCREEN_DPI)
        {
        }

        public DensityManager(float density, int dpi)
        {
            screenDensity = density;
            screenDpi = dpi;
        }

        internal void Set(float density, int dpi)
        {
            screenDensity = density;
            screenDpi = dpi;
        }
    }
}
