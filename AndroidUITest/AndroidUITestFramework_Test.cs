using AndroidUITestFramework;

namespace AndroidUITestFramework_Test
{
    public class AndroidUITestFramework_Tests : TestGroup
    {
        public override void OnCreate()
        {
            Console.WriteLine("Inside OnCreate");
        }

        public override void OnDestroy()
        {
            Console.WriteLine("Inside OnDestroy");
        }

        public class UnitTest_1 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("Inside Test_1");
            }
        }

        public class UnitTest_2 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("Inside Test_2");
            }
        }
    }
}