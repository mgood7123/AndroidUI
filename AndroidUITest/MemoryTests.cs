using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;
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
                    MemoryPointer<int> a = new int[] { 1, 2 };
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 2);
                }
            }

            class test2_arithmatic : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[] { 1, 2 };
                    a += 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 1);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 2);
                }
            }

            class test3_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[] { 1, 2 };
                    Tools.AssertEqual(a[0], 1);
                    Tools.AssertEqual(a[1], 2);
                }
            }

            class test4_arithmatic_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[] { 1, 2 };
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
                    MemoryPointer<int> a = new int[] { 1, 2 };
                    MemoryPointer<int> b = new int[] { 0, 0 };
                    a.Copy(b, 2);
                    Tools.AssertEqual(b[0], 1);
                    Tools.AssertEqual(b[1], 2);
                }
            }

            class test6_foreach : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[] { 1, 2, 3, 4 };
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
                    MemoryPointer<int> a = new int[][]{ new int[] { 1, 2 }, new int[] { 3, 4 } };
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 4);
                }
            }

            class test2_arithmatic : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    a += 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 3);
                    a += 1;
                    Tools.AssertEqual(a.offset, 2);
                    Tools.AssertEqual(a.Length, 2);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 1);
                    Tools.AssertEqual(a.Length, 3);
                    a -= 1;
                    Tools.AssertEqual(a.offset, 0);
                    Tools.AssertEqual(a.Length, 4);
                }
            }

            class test3_index_access : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
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
                    MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
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
                    MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    MemoryPointer<int> b = new int[][] { new int[] { 0, 0 }, new int[] { 0, 0 } };
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
                    MemoryPointer<int> a = new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } };
                    int index = 1;
                    foreach (int value in a)
                    {
                        Tools.AssertEqual(value, index);
                        index++;
                    }
                }
            }
        }

        class Assignment : TestGroup
        {
            public class X
            {
                public int x;

                public X(int x)
                {
                    this.x = x;
                }
            }

            public struct XY
            {
                public int x;
                public int y;

                public XY(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            class MapperTest : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    X x1 = new(5);
                    X x2 = new(5);
                    Tools.ExpectEqual(x1.x, 5, "x1 5");
                    Tools.ExpectEqual(x2.x, 5, "x2 5");
                    ContiguousArray<int> a = new Mapper<X, int>[2] {
                        new(x1, (obj, arrayIndex, index) => ref obj.x, 1),
                        new(x2, (obj, arrayIndex, index) => ref obj.x, 1)
                    };
                    a[0] = 1;
                    a[1] = 2;
                    Tools.ExpectEqual(x1.x, 1, "x1 1");
                    Tools.ExpectEqual(x2.x, 2, "x2 2");

                    ValueHolder<XY> x3 = new XY(55, 66);
                    ContiguousArray<int> b = new Mapper<ValueHolder<XY>, int>[1] {
                        new(x3, (obj, arrayIndex, index) => ref index == 0 ? ref obj.Value.x : ref obj.Value.y, 2)
                    };
                    b[0] = 5577;
                    b[1] = 6677;
                    Tools.ExpectEqual(x3.Value.x, 5577, "x3 5577");
                    Tools.ExpectEqual(x3.Value.y, 6677, "x3 6677");

                    XY[] x4 = new XY[2] { new(55, 66), new(77, 88) };
                    ContiguousArray<int> c = new Mapper<XY[], int>[1] {
                        new(x4, (obj, arrayIndex, index) => {
                            if (index == 0)
                            {
                                return ref obj[arrayIndex].x;
                            }
                            return ref obj[arrayIndex].y;
                        }, 2)
                    };
                    c[0] = 5577;
                    c[1] = 6677;
                    c[2] = 7777;
                    c[3] = 8877;
                    Tools.ExpectEqual(x4[0].x, 5577, "x4 5577");
                    Tools.ExpectEqual(x4[0].y, 6677, "x4 6677");
                    Tools.ExpectEqual(x4[1].x, 7777, "x4 7777");
                    Tools.ExpectEqual(x4[1].y, 8877, "x4 8877");

                    XY[] x5 = new XY[2] { new(55, 66), new(77, 88) };
                    ContiguousArray<int> c2 = new Mapper<XY[], int>[1] {
                        new(x5, (obj, arrayIndex, index) => {
                            return ref obj[arrayIndex].x;
                        }, 1)
                    };
                    c2[0] = 5577;
                    c2[1] = 6677;
                    Tools.ExpectEqual(x5[0].x, 5577, "x4 5577");
                    Tools.ExpectEqual(x5[0].y, 66, "x4 6677");
                    Tools.ExpectEqual(x5[1].x, 6677, "x4 7777");
                    Tools.ExpectEqual(x5[1].y, 88, "x4 8877");
                }
            }
        }
    }
}
