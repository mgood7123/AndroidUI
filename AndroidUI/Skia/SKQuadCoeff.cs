using AndroidUI.Utils.Skia;
using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI.Skia
{
    /**
     *  use for : eval(t) == A * t^2 + B * t + C
     */
    class SKQuadCoeff
    {
        public SKQuadCoeff() { }

        public SKQuadCoeff(Sk2s A, Sk2s B, Sk2s C)
        {
            fA = A;
            fB = B;
            fC = C;
        }

        public SKQuadCoeff(SKPoint[] src)
        {
            if (src.Length < 3)
            {
                throw new Exception("SKQuadCoeff point array size must be at least 3");
            }
            fC = SKUtils.from_point(src[0]);
            Sk2s P1 = SKUtils.from_point(src[1]);
            Sk2s P2 = SKUtils.from_point(src[2]);
            fB = SKUtils.times_2(P1 - fC);
            fA = P2 - SKUtils.times_2(P1) + fC;
        }

        public Sk2s eval(float t)
        {
            Sk2s tt = new(t);
            return eval(tt);
        }

        public Sk2s eval(Sk2s tt)
        {
            return (fA * tt + fB) * tt + fC;
        }

        internal Sk2s fA;
        internal Sk2s fB;
        internal Sk2s fC;
    }
}