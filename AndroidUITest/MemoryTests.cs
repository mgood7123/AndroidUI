﻿using AndroidUITestFramework;

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

            class t : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    X x1 = new X(5);
                    X x2 = new X(5);
                    Tools.ExpectEqual(x1.x, 5, "x1 5");
                    Tools.ExpectEqual(x2.x, 5, "x2 5");
                    AndroidUI.ContiguousArray<int> a = new AndroidUI.Mapper<X, int>[2] {
                        new(x1, (obj, arrayIndex, index) => ref obj.x, 1),
                        new(x2, (obj, arrayIndex, index) => ref obj.x, 1)
                    };
                    a[0] = 1;
                    a[1] = 2;
                    Tools.ExpectEqual(x1.x, 1, "x1 1");
                    Tools.ExpectEqual(x2.x, 2, "x2 2");

                    AndroidUI.ValueHolder<XY> x3 = new XY(55, 66);
                    Tools.ExpectEqual(x3.value.x, 55, "x3 55");
                    Tools.ExpectEqual(x3.value.y, 66, "x3 66");
                    AndroidUI.ContiguousArray<int> b = new AndroidUI.Mapper<AndroidUI.ValueHolder<XY>, int>[1] {
                        new(x3, (obj, arrayIndex, index) => ref index == 0 ? ref obj.value.x : ref obj.value.y, 2)
                    };
                    b[0] = 5577;
                    b[1] = 6677;
                    Tools.ExpectEqual(x3.value.x, 5577, "x3 5577");
                    Tools.ExpectEqual(x3.value.y, 6677, "x3 6677");

                    XY[] x4 = new XY[2] { new(55, 66), new(77, 88) };
                    Tools.ExpectEqual(x4[0].x, 55, "x4 55");
                    Tools.ExpectEqual(x4[0].y, 66, "x4 66");
                    Tools.ExpectEqual(x4[1].x, 77, "x4 77");
                    Tools.ExpectEqual(x4[1].y, 88, "x4 88");
                    AndroidUI.ContiguousArray<int> c = new AndroidUI.Mapper<XY[], int>[1] {
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
                }
            }
        }
    }
}
