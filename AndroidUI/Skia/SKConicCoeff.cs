using AndroidUI.Utils.Skia;
using static AndroidUI.Native;

namespace AndroidUI.Skia
{
    struct SKConicCoeff
    {
        public SKConicCoeff(SKConic conic)
        {
            Sk2s p0 = SKUtils.from_point(conic.fPts[0]);
            Sk2s p1 = SKUtils.from_point(conic.fPts[1]);
            Sk2s p2 = SKUtils.from_point(conic.fPts[2]);
            Sk2s ww = new(conic.fW);

            Sk2s p1w = p1 * ww;
            fNumer.fC = p0;
            fNumer.fA = p2 - SKUtils.times_2(p1w) + p0;
            fNumer.fB = SKUtils.times_2(p1w - p0);

            fDenom.fC = new Sk2s(1);
            fDenom.fB = SKUtils.times_2(ww - fDenom.fC);
            fDenom.fA = new Sk2s(0) - fDenom.fB;
        }

        public Sk2s eval(float t)
        {
            Sk2s tt = new(t);
            Sk2s numer = fNumer.eval(tt);
            Sk2s denom = fDenom.eval(tt);
            return numer / denom;
        }

        internal SKQuadCoeff fNumer = new();
        internal SKQuadCoeff fDenom = new();
    };
}