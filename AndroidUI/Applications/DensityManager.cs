namespace AndroidUI.Applications
{
    public class DensityManager
    {
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

        public DensityManager() : this(1.0f, 96)
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
