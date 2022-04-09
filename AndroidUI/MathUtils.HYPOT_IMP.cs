namespace AndroidUI
{
	using static CastUtils;
	public static partial class MathUtils
    {
		internal class HYPOT_IMP
		{
			// if ever needed
			const int FLT_EVAL_METHOD = -1;
			const int FLT_RADIX = 2;
			const int FLT_MANT_DIG = 24;
			const int LDBL_MANT_DIG = 53;



			// in C# FLT_EVAL_METHOD may be 0, 1, or 2 at the discretion of the compiler/JIT/runtime
			// C# FLT_EVAL_METHOD is -1 : the default precision is not known

			// for FLT_RADIX gcc states
			// "The value is 2 on all machines we know of except the IBM 360 and derivatives."
			//
			// it is assumed C# will EMULATE a radix of 2 if the system radix is not 2
			// C# FLT_RADIX is 2

			// for FLT_MANT_DIG gcc states
			// "This is the number of base-FLT_RADIX digits in the floating point mantissa
			// for the float data type"
			//
			// we assume this to be equivilant to the following
			// C# FLT_MANT_DIG is 24 and C# LDBL_MANT_DIG is 53
			//
			// The mantissa is stored as a binary fraction greater than or equal to 1 and
			// less than 2. For types float and double, there is an implied leading 1 in
			// the mantissa in the most-significant bit position, so the mantissas are
			// actually 24 and 53 bits long, respectively, even though the
			// most-significant bit is never stored in memory.

			// NOTE:
			// [22:54] <smallville7123_> "The mantissa is stored as a binary fraction greater than or equal to 1 and less than 2. For types float and double, there is an implied leading 1 in the mantissa in the most-significant bit position, so the mantissas are actually 24 and 53 bits long, respectively, even though the most-significant bit is never stored in memory."
			// [22:58] <smallville7123_> "Instead of the storage method just described, the floating-point package can store binary floating-point numbers as denormalized numbers. "Denormalized numbers" are nonzero floating-point numbers with reserved exponent values in which the most-significant bit of the mantissa is 0"

			// #if FLT_EVAL_METHOD > 1U && LDBL_MANT_DIG == 64
			// #define SPLIT (0x1p32 + 1)
			// #else
			// #define SPLIT (0x1p27 + 1)
			// #endif

			// if is selected as evel method is -1 and according to gcc, -1 > 1U is true

			static readonly double SPLIT = FPN_P(0x1, 32) + 1;

			static void sq(ref double hi, ref double lo, double x)
			{
				double xh, xl, xc;

				xc = x * SPLIT;
				xh = x - xc + xc;
				xl = x - xh;
				hi = x * x;
				lo = xh * xh - hi + 2 * xh * xl + xl * xl;
			}

			static readonly uint MINUS_ONE_U = reinterpret_cast<uint>(-1U);
			static readonly ulong MINUS_ONE_UL = reinterpret_cast<ulong>(-reinterpret_cast<long>(1UL));

			public static double hypot(double x, double y)
			{
				DOUBLE_ULONG ux = x, uy = y, ut;
                double hx = 0, lx = 0, hy = 0, ly = 0, z = 0;

				/* arrange |x| >= |y| */
				ux.i &= MINUS_ONE_UL >> 1;
				uy.i &= MINUS_ONE_UL >> 1;
				if (ux.i < uy.i)
				{
					ut = ux;
					ux = uy;
					uy = ut;
				}

				/* special cases */
				int ex = reinterpret_cast<int>(ux.i >> 52);
				int ey = reinterpret_cast<int>(uy.i >> 52);
				x = ux.f;
				y = uy.f;               /* note: hypot(inf,nan) == inf */
				if (ey == 0x7ff)
					return y;
				if (ex == 0x7ff || uy.i == 0)
					return x;
				/* note: hypot(x,y) ~= x + y*y/x/2 with inexact for small y/x */
				/* 64 difference is enough for ld80 double_t */
				if (ex - ey > 64)
					return x + y;

				/* precise sqrt argument in nearest rounding mode without overflow */
				/* xh*xh must not overflow and xl*xl must not underflow in sq */
				z = 1;
				if (ex > 0x3ff + 510)
				{
					double b = FPN_P(0x1, 700);
					double a = FPN_P(0x1, -700);
					z = b;
					x *= a;
					y *= a;
				}
				else if (ey < 0x3ff - 450)
				{
					double a = FPN_P(0x1, -700);
					double b = FPN_P(0x1, 700);
					z = a;
					x *= b;
					y *= b;
				}
				sq(ref hx, ref lx, x);
				sq(ref hy, ref ly, y);
				return z * Math.Sqrt(ly + lx + hy + hx);
			}

			public static float hypotf(float x, float y)
			{
				FLOAT_UINT ux = x, uy = y, ut;

				ux.i &= MINUS_ONE_U >> 1;
				uy.i &= MINUS_ONE_U >> 1;
				if (ux.i < uy.i)
				{
					ut = ux;
					ux = uy;
					uy = ut;
				}

				x = ux.f;
				y = uy.f;

				if (uy.i == 0xff << 23)
					return y;
				if (ux.i >= 0xff << 23 || uy.i == 0 || ux.i - uy.i >= 25 << 23)
					return x + y;

				float z = 1;

				if (ux.i >= (0x7f + 60) << 23)
				{
					float a = (float)FPN_P(0x1, -90);
					float b = (float)FPN_P(0x1, 90);
					z = b;
					x *= a;
 					y *= a;
				}
				else if (uy.i < (0x7f - 60) << 23)
				{
					float a = (float)FPN_P(0x1, -90);
					float b = (float)FPN_P(0x1, 90);
					z = a;
					x *= b;
					y *= b;
				}

				return z * sqrtf((float)((double)x * x + (double)y * y));
			}

            // we assume this to be equivilant to the following
            // C# FLT_MANT_DIG is 24 and C# LDBL_MANT_DIG is 53

            // C# LDBL_MAX_EXP is 1024

            // C# has no concept of long double
            public static double hypotl(double x, double y) => hypot(x, y);
        }
	}
}