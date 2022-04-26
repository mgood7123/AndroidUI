using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI
{
    class SKCubicCoeff
    {
        public SKCubicCoeff(SKPoint[] src) {
            if (src.Length < 4)
            {
                throw new Exception("SKCubicCoeff point array size must be at least 4");
            }
            Sk2s P0 = SKConic.from_point(src[0]);
            Sk2s P1 = SKConic.from_point(src[1]);
            Sk2s P2 = SKConic.from_point(src[2]);
            Sk2s P3 = SKConic.from_point(src[3]);
            Sk2s three = new(3);
            fA = P3 + three * (P1 - P2) - P0;
            fB = three * (P2 - SKConic.times_2(P1) + P0);
            fC = three * (P1 - P0);
            fD = P0;
        }

        public Sk2s eval(float t)
        {
            Sk2s tt = new(t);
            return eval(tt);
        }

        Sk2s eval(Sk2s t)
        {
            return ((fA * t + fB) * t + fC) * t + fD;
        }

        internal Sk2s fA;
        internal Sk2s fB;
        internal Sk2s fC;
        internal Sk2s fD;
    };
}