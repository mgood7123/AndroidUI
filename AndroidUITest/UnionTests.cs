using AndroidUITestFramework;

namespace AndroidUITest
{
    internal class UnionTests : TestGroup
    {
        private class test : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union u = new AndroidUI.Union(typeof(System.Collections.Hashtable), typeof(Action<object>));

                System.Collections.Hashtable a;
                Action<object> action;
                
                a = u.get<System.Collections.Hashtable>();
                action = u.get<Action<object>>();

                Tools.ExpectInstanceEqual(a, null);
                Tools.ExpectInstanceEqual(action, null);
                Tools.ExpectInstanceEqual(a, action);

                u.set(new System.Collections.Hashtable());
                a = u.get<System.Collections.Hashtable>();
                action = u.get<Action<object>>();
                a.Add(5, 5);

                Tools.ExpectInstanceNotEqual(a, null);
                Tools.ExpectInstanceNotEqual(action, null);
                Tools.ExpectInstanceEqual(a, action);
                Tools.ExpectEqual(a.Count, 1);

                u.set<Action<object>>((i) => { });
                a = u.get<System.Collections.Hashtable>();
                action = u.get<Action<object>>();

                Tools.ExpectInstanceNotEqual(a, null);
                Tools.ExpectInstanceNotEqual(action, null);
                Tools.ExpectInstanceEqual(a, action);
            }
        }

        private class Test2 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(byte[]), typeof(int[]), typeof(int));
                a.set(5000);
                byte[] bytes = a.get<byte[]>();
                var i = a.getBindable<int>();
                int[] ints = a.get<int[]>();
                Tools.ExpectEqual(i.GetValue<int>(), 5000, "i.GetValue<int>()");
                Tools.ExpectEqual(ints.Length, 4, "ints.Length");
                Tools.ExpectEqual(ints[0], 5000, "ints[0]");
                Tools.ExpectEqual(bytes.Length, 4, "bytes.Length");
                Tools.ExpectEqual(bytes[0], 136, "bytes[0]");
                Tools.ExpectEqual(bytes[1], 19, "bytes[1]");
                Tools.ExpectEqual(bytes[2], 0, "bytes[2]");
                Tools.ExpectEqual(bytes[3], 0, "bytes[3]");
                ints[0] = int.MaxValue;
                Tools.ExpectEqual(i.GetValue<int>(), int.MaxValue, "i.GetValue<int>()");
                Tools.ExpectEqual(ints.Length, 4, "ints.Length");
                Tools.ExpectEqual(ints[0], int.MaxValue, "ints[0]");
                Tools.ExpectEqual(bytes.Length, 4, "bytes.Length");
                Tools.ExpectEqual(bytes[0], byte.MaxValue, "bytes[0]");
                Tools.ExpectEqual(bytes[1], byte.MaxValue, "bytes[1]");
                Tools.ExpectEqual(bytes[2], byte.MaxValue, "bytes[2]");
                Tools.ExpectEqual(bytes[3], sbyte.MaxValue, "bytes[3]");
            }
        }

        private class Test3 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(int[]), typeof(int));
                a.set(5000);

                var bindable_i = a.getBindable<int>();
                var bindable_i_array = a.getBindable<int[]>();
                int[] i_array = a.get<int[]>();

                Tools.ExpectEqual(a.get<int>(), 5000, "a.get<int>()");
                Tools.ExpectEqual(a.getBindable<int>().GetValue<int>(), 5000, "a.getBindable<int>().GetValue<int>()");
                Tools.ExpectEqual(a.get<int[]>()[0], 5000, "a.get<int[]>()[0]");
                Tools.ExpectEqual(bindable_i.GetValue<int>(), 5000, "bindable_i.GetValue<int>()");
                Tools.ExpectEqual(bindable_i.GetValue<int[]>()[0], 5000, "bindable_i.GetValue<int[]>()[0]");
                Tools.ExpectEqual(bindable_i_array.GetValue<int[]>()[0], 5000, "bindable_i_array.GetValue<int[]>()[0]]");
                Tools.ExpectEqual(i_array[0], 5000, "i_array[0]");

                bindable_i.SetValue(66);

                Tools.ExpectEqual(a.get<int>(), 66, "a.get<int>()");
                Tools.ExpectEqual(a.getBindable<int>().GetValue<int>(), 66, "a.getBindable<int>().GetValue<int>()");
                Tools.ExpectEqual(a.get<int[]>()[0], 66, "a.get<int[]>()[0]");
                Tools.ExpectEqual(bindable_i.GetValue<int>(), 66, "bindable_i.GetValue<int>()");
                Tools.ExpectEqual(bindable_i.GetValue<int[]>()[0], 66, "bindable_i.GetValue<int[]>()[0]");
                Tools.ExpectEqual(bindable_i_array.GetValue<int[]>()[0], 66, "bindable_i_array.GetValue<int[]>()[0]]");
                Tools.ExpectEqual(i_array[0], 66, "i_array[0]");
            }
        }

        private class Test_Wrong_Type : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(int[]), typeof(int));
                a.set(0);
                Tools.ExpectException<InvalidCastException>(() => a.get<object>());
            }
        }
        private class Test_Wrong_Type2 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(int[]), typeof(int));
                a.set(0);
                Tools.ExpectException<InvalidCastException>(() => a.get<short>());
            }
        }
        private class Test_Wrong_Type3 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(int[]), typeof(int));
                a.set(0);
                Tools.ExpectException<InvalidCastException>(() => a.getBindable<int>().GetValue<object>());
            }
        }
        private class Test_Wrong_Type4 : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Union a = new AndroidUI.Union(typeof(int[]), typeof(int));
                a.set(0);
                Tools.ExpectException<InvalidCastException>(() => a.getBindable<int>().GetValue<short>());
            }
        }
    }
}
