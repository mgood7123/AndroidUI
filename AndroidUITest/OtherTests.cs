using AndroidUITestFramework;

namespace NATIVE_TEST
{
    class NATIVE_TEST : Test
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Run(TestGroup nullableInstance)
        {
            var sk2f = AndroidUI.Native.Sk2f.Load(new float[] { 1, 2 });
            Tools.ExpectEqual(sk2f[0], 1);
            Tools.ExpectEqual(sk2f[1], 2);
            sk2f *= sk2f;
            Tools.ExpectEqual(sk2f[0], 1);
            Tools.ExpectEqual(sk2f[1], 4);
        }
    }

    class native_benchmark : XMarkTest
    {
        protected override void prepareBenchmark(XManager runner)
        {
            var m = int.MaxValue / 4;
            runner.AddSession(
                new XSession(
                    m + " Nothings",
                    () =>
                    {
                        for (var i = m - m; i < m; i++) { }
                    }
                )
            );

            runner.AddSession(new XSession(m + " iterations of Nothing", () => { }, m));

            void al()
            {
                var sk2f = AndroidUI.Native.Sk2f.Load(new float[] { 1, 2 });
                Tools.ExpectEqual(sk2f[0], 1);
                Tools.ExpectEqual(sk2f[1], 2);
                sk2f *= sk2f;
                Tools.ExpectEqual(sk2f[0], 1);
                Tools.ExpectEqual(sk2f[1], 4);
            }

            runner.AddSession(new XSession("Native allocation", al, 80));
            runner.AddSession(new XSession(
                "Native allocation MT",
                al,
                TimeSpan.FromSeconds(30).Ticks / 100 / 2 / 2 / 2/2, 10
            ));
        }
    }
}