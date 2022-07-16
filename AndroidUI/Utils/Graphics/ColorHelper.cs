namespace AndroidUI.Utils.Graphics
{
    public static class ColorHelper
    {
        public static readonly SkiaSharp.SKColorSpace srgb = SkiaSharp.SKColorSpace.CreateSrgb();

        public static int ToARGB(byte a, byte r, byte g, byte b)
        {
            // u32   ff ff ff ff
            // u8[]  a  r  g  b
            int u32 = a + (r << 8) | (g << 16) + (b << 24);
            return u32;
        }

        public static int ToRGBA(byte r, byte g, byte b, byte a)
        {
            // u32   ff ff ff ff
            // u8[]  r  g  b  a
            int u32 = r + (g << 8) | (b << 16) + (a << 24);
            return u32;
        }

        public static int ToARGB(byte r, byte g, byte b)
        {
            return ToARGB(255, r, g, b);
        }

        public static int ToRGBA(byte r, byte g, byte b)
        {
            return ToRGBA(r, g, b, 255);
        }

        static float CheckedClamp(float value, float min = 0.0f, float max = 1.0f, float default_value = 0.0f)
        {
            return (value.isFinite() ? value : default_value).clamp(min, max);
        }

        static double CheckedClamp(double value, double min = 0.0, double max = 1.0, double default_value = 0.0)
        {
            return (value.isFinite() ? value : default_value).clamp(min, max);
        }

        public static int ToARGB(float a, float r, float g, float b)
        {
            return ToARGB(
                (byte)(255 * CheckedClamp(a, default_value: 1.0f)),
                (byte)(255 * CheckedClamp(r)),
                (byte)(255 * CheckedClamp(b)),
                (byte)(255 * CheckedClamp(g))
            );
        }

        public static int ToRGBA(float r, float g, float b, float a)
        {
            return ToRGBA(
                (byte)(255 * CheckedClamp(r)),
                (byte)(255 * CheckedClamp(b)),
                (byte)(255 * CheckedClamp(g)),
                (byte)(255 * CheckedClamp(a, default_value: 1.0f))
            );
        }

        public static int ToARGB(float r, float g, float b)
        {
            return ToARGB(1.0f, r, g, b);
        }

        public static int ToRGBA(float r, float g, float b)
        {
            return ToRGBA(r, g, b, 1.0f);
        }

        public static int ToARGB(double a, double r, double g, double b)
        {
            return ToARGB(
                (byte)(255 * CheckedClamp(a, default_value: 1.0)),
                (byte)(255 * CheckedClamp(r)),
                (byte)(255 * CheckedClamp(b)),
                (byte)(255 * CheckedClamp(g))
            );
        }

        public static int ToRGBA(double r, double g, double b, double a)
        {
            return ToRGBA(
                (byte)(255 * CheckedClamp(r)),
                (byte)(255 * CheckedClamp(b)),
                (byte)(255 * CheckedClamp(g)),
                (byte)(255 * CheckedClamp(a, default_value: 1.0))
            );
        }

        public static int ToARGB(double r, double g, double b)
        {
            return ToARGB(1.0, r, g, b);
        }

        public static int ToRGBA(double r, double g, double b)
        {
            return ToRGBA(r, g, b, 1.0);
        }



        public static SkiaSharp.SKColor ToSKColor(byte r, byte g, byte b)
        {
            return new SkiaSharp.SKColor(r, g, b, 255);
        }

        public static SkiaSharp.SKColor ToSKColor(byte a, byte r, byte g, byte b)
        {
            return new SkiaSharp.SKColor(r, g, b, a);
        }

        public static SkiaSharp.SKColorF ToSKColorF(float r, float g, float b)
        {
            return new SkiaSharp.SKColorF(r, g, b, 1.0f);
        }

        public static SkiaSharp.SKColorF ToSKColorF(float a, float r, float g, float b)
        {
            return new SkiaSharp.SKColorF(r, g, b, a);
        }
    }
}
