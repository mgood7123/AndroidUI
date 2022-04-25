using SkiaSharp;
using static AndroidUI.Native;

namespace AndroidUI
{
    /**
     *  use for : eval(t) == A * t^2 + B * t + C
     */
    class SkQuadCoeff
    {
        public SkQuadCoeff() { }

        public SkQuadCoeff(Sk2s A, Sk2s B, Sk2s C)
        {
            fA = A;
            fB = B;
            fC = C;
        }

        public SkQuadCoeff(SKPoint[] src)
        {
            if (src.Length < 3)
            {
                throw new Exception("SkQuadCoeff point array size must be at least 3");
            }
            fC = SKConic.from_point(src[0]);
            Sk2s P1 = SKConic.from_point(src[1]);
            Sk2s P2 = SKConic.from_point(src[2]);
            fB = SKConic.times_2(P1 - fC);
            fA = P2 - SKConic.times_2(P1) + fC;
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