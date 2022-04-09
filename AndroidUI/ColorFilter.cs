namespace AndroidUI
{
    public abstract class ColorFilter
    {
        SkiaSharp.SKColorFilter nativeInstance;

        public abstract SkiaSharp.SKColorFilter createNativeInstance();
        public SkiaSharp.SKColorFilter getNativeInstance() => nativeInstance;

        protected SkiaSharp.SKColorFilter native_CreateBlendModeFilter(int mColor, BlendMode porterDuffMode)
        {
            return native_CreateBlendModeFilter(Color.toSKColor(mColor), porterDuffMode);
        }

        protected SkiaSharp.SKColorFilter native_CreateBlendModeFilter(Color mColor, BlendMode porterDuffMode)
        {
            return native_CreateBlendModeFilter(mColor.toSKColor(), porterDuffMode);
        }

        protected SkiaSharp.SKColorFilter native_CreateBlendModeFilter(SkiaSharp.SKColor mColor, BlendMode porterDuffMode)
        {
            nativeInstance?.Dispose();
            nativeInstance = SkiaSharp.SKColorFilter.CreateBlendMode(mColor, porterDuffMode);
            return nativeInstance;
        }
    }
}