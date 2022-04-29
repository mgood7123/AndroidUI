using AndroidUITestFramework;

namespace AndroidUITest
{
    class MemoryTests : TestGroup
    {
        class Rank1 : TestGroup
        {
            class test1_initialization : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2 };
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 2);
                }
            }

            class test2_arithmatic : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2 };
                    a += 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 2);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 2);
                }
            }

            class test3_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2 };
                    Tools.AssertEqual(a[0], 1);
                    Tools.AssertEqual(a[1], 2);
                }
            }

            class test4_arithmatic_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2 };
                    a += 1;
                    Tools.AssertEqual(a[0], 2);
                    a -= 1;
                    Tools.AssertEqual(a[0], 1);
                    Tools.AssertEqual(a[1], 2);
                }
            }

            class test5_Copy : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2 };
                    AndroidUI.MemoryPointer<int> b = new int[] { 0, 0 };
                    a.Copy(b, 2);
                    Tools.AssertEqual(b[0], 1);
                    Tools.AssertEqual(b[1], 2);
                }
            }

            class test6_foreach : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[] { 1, 2, 3, 4 };
                    int index = 1;
                    foreach (int value in a)
                    {
                        Tools.AssertEqual(value, index);
                        index++;
                    }
                }
            }
        }

        class Rank2 : TestGroup
        {
            class test1_initialization : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][]{ new int[] { 1, 2 }, new int[] { 3, 4 } };
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 4);
                }
            }

            class test2_arithmatic : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    a += 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 4);
                    a += 1;
                    Tools.AssertEqual(a.offset, 2);
                    Tools.AssertEqual(a.Length, 4);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 4);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 4);
                }
            }

            class test3_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    Tools.AssertEqual(a[0], 1);
                    Tools.AssertEqual(a[1], 2);
                    Tools.AssertEqual(a[2], 3);
                    Tools.AssertEqual(a[3], 4);
                }
            }

            class test4_arithmatic_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    a += 1;
                    Tools.AssertEqual(a[0], 2);
                    a++;
                    Tools.AssertEqual(a[0], 3);
                    a += 1;
                    Tools.AssertEqual(a[0], 4);
                    a -= 1;
                    Tools.AssertEqual(a[0], 3);
                    a -= 1;
                    Tools.AssertEqual(a[0], 2);
                    a -= 1;
                    Tools.AssertEqual(a[0], 1);
                }
            }

            class test5_Copy : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    AndroidUI.MemoryPointer<int> b = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 } };
                    a.Copy(b, 4);
                    Tools.AssertEqual(b[0], 1);
                    Tools.AssertEqual(b[1], 2);
                    Tools.AssertEqual(b[2], 3);
                    Tools.AssertEqual(b[3], 4);
                }
            }

            class test6_foreach : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    int index = 1;
                    foreach (int value in a)
                    {
                        Tools.AssertEqual(value, index);
                        index++;
                    }
                }
            }
        }
    }
}
