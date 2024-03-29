﻿using AndroidUITestFramework;

namespace AndroidUITest
{
    internal class math : TestGroup
    {
        internal class hypot : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Tools.AssertEqual(AndroidUI.Utils.MathUtils.hypot(3.0, 4.0), 5.0, "hypot(3.0, 4.0)");

                // If x or y is a NaN, returns NaN.
                Tools.AssertEqual(AndroidUI.Utils.MathUtils.hypot(3.0, double.NaN), double.NaN, "hypot(3.0, double.NaN)");
                Tools.AssertEqual(AndroidUI.Utils.MathUtils.hypot(double.NaN, 4.0), double.NaN, "hypot(double.NaN, 4.0)");
            }
        }
    }
}
