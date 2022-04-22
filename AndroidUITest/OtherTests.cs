using AndroidUITestFramework;
using System.Runtime.CompilerServices;

class NATIVE_TEST : Test
{
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