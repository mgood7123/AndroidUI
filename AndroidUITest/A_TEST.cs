using AndroidUITestFramework;

public class Group : AndroidUITestFramework.TestGroup
{
    public class ASSERTION : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("ASSERTION!");
            AndroidUITestFramework.Tools.AssertFalse(true);
        }
    }

    public class foo_TEST
    {
        public class TEST_INSIDE_ANOTHER_CLASS : AndroidUITestFramework.Test
        {
            public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
            {
                Console.WriteLine("TEST! 3");
            }
        }
        public class H {
            public class GROUP_INSIDE_ANOTHER_CLASS : AndroidUITestFramework.TestGroup
            {
                public class HOTDOG : AndroidUITestFramework.Test
                {
                    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
                    {
                        Console.WriteLine("TEST! 4");
                    }
                }
            }
        }
    }
}

class BASE_TEST : AndroidUITestFramework.Test
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        Console.WriteLine("TEST! 1");
    }
}

class inherited_test : BASE_TEST
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        base.Run(nullableInstance);
        Console.WriteLine("TEST! INHERITED");
    }
}

class EXPECTATIONS : AndroidUITestFramework.Test
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        Console.WriteLine("EXPECTATIONS!");
        AndroidUITestFramework.Tools.ExpectFalse(true);
        AndroidUITestFramework.Tools.ExpectTrue(false);
        AndroidUITestFramework.Tools.ExpectInstanceEqual(this, null);
    }
}

class B_TEST : AndroidUITestFramework.Test
{
    public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
    {
        Console.WriteLine("TEST!");
    }
}

namespace foob
{
    class B_TEST : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("TEST! INSIDE NAMESPACE");
        }
    }
}

public class PrintGroup : AndroidUITestFramework.TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class PrintTest: AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }

    public class SkipTest : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            AndroidUITestFramework.Tools.SKIP();
            Console.WriteLine("TEST");
        }
    }
}

public class SkipGroup : AndroidUITestFramework.TestGroup
{
    public override void OnCreate()
    {
        AndroidUITestFramework.Tools.SKIP();
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class SkipTest : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}

public class FailGroup : AndroidUITestFramework.TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
        AndroidUITestFramework.Tools.FAIL();
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class Test : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}

public class FailGroup2 : AndroidUITestFramework.TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
        AndroidUITestFramework.Tools.FAIL();
    }

    public class Test : AndroidUITestFramework.Test
    {
        public override void Run(AndroidUITestFramework.TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}


public class UNHANDLED_GROUP_EXCEPTION_CREATE : AndroidUITestFramework.TestGroup
{
    public override void OnCreate()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }
}

public class UNHANDLED_GROUP_EXCEPTION_DESTROY : AndroidUITestFramework.TestGroup
{
    public override void OnDestroy()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
        }
    }
}

public class UNHANDLED_GROUP_EXCEPTION_DESTROY2 : AndroidUITestFramework.TestGroup
{
    public override void OnDestroy()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }
}

public class UNHANDLED_TEST_EXCEPTION : AndroidUITestFramework.Test
{
    public override void Run(TestGroup nullableInstance)
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }
}

public class UNHANDLED_GROUP_EXCEPTION_2 : AndroidUITestFramework.TestGroup
{
    public class UNHANDLED_TEST_EXCEPTION_1 : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }

    public class UNHANDLED_TEST_EXCEPTION_2 : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }
}



namespace T
{
    public class a_test_group : TestGroup
    {
        public override void OnCreate()
        {
            // called before a test is ran
        }

        public override void OnDestroy()
        {
            // called after a test is ran
        }

        class foo
        {
            class a_test : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    Console.WriteLine("I AM A TEST INSIDE OF A TEST GROUP AND I AM STILL FOUND");
                }
            }
        }

        class sub
        {
            class group
            {
                class we_are_nested
                {
                    public class a_test_group : TestGroup
                    {
                        public override void OnCreate()
                        {
                            // called before a test is ran
                            Console.WriteLine("I AM A TEST GROUP INSIDE OF A TEST GROUP AND I AM STILL FOUND");
                        }

                        public override void OnDestroy()
                        {
                            // called after a test is ran
                        }

                        class foo
                        {
                            class a_test : Test
                            {
                                public override void Run(TestGroup nullableInstance)
                                {
                                    Console.WriteLine("I AM A TEST INSIDE OF A TEST GROUP INSIDE OF ANOTHER TEST GROUP AND I AM STILL FOUND");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}