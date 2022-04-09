using AndroidUITestFramework;

public class Group : TestGroup
{
    public class ASSERTION : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("ASSERTION!");
            Tools.AssertFalse(true);
        }
    }

    public class foo_TEST
    {
        public class TEST_INSIDE_ANOTHER_CLASS : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Console.WriteLine("TEST! 3");
            }
        }
        public class H {
            public class GROUP_INSIDE_ANOTHER_CLASS : TestGroup
            {
                public class HOTDOG : Test
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        Console.WriteLine("TEST! 4");
                    }
                }
            }
        }
    }
}

class BASE_TEST : Test
{
    public override void Run(TestGroup nullableInstance)
    {
        Console.WriteLine("TEST! 1");
    }
}

class inherited_test : BASE_TEST
{
    public override void Run(TestGroup nullableInstance)
    {
        base.Run(nullableInstance);
        Console.WriteLine("TEST! INHERITED");
    }
}

class EXPECTATIONS : Test
{
    public override void Run(TestGroup nullableInstance)
    {
        Console.WriteLine("EXPECTATIONS!");
        Tools.ExpectFalse(true);
        Tools.ExpectTrue(false);
        Tools.ExpectInstanceEqual(this, null);
    }
}

class B_TEST : Test
{
    public override void Run(TestGroup nullableInstance)
    {
        Console.WriteLine("TEST!");
    }
}

namespace foob
{
    class B_TEST : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("TEST! INSIDE NAMESPACE");
        }
    }
}

public class PrintGroup : TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class PrintTest: Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }

    public class SkipTest : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Tools.SKIP();
            Console.WriteLine("TEST");
        }
    }
}

public class SkipGroup : TestGroup
{
    public override void OnCreate()
    {
        Tools.SKIP();
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class SkipTest : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}

public class FailGroup : TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
        Tools.FAIL();
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
    }

    public class Test : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}

public class FailGroup2 : TestGroup
{
    public override void OnCreate()
    {
        Console.WriteLine("TEST GROUP CREATE");
    }

    public override void OnDestroy()
    {
        Console.WriteLine("TEST GROUP DESTROY");
        Tools.FAIL();
    }

    public class Test : AndroidUITestFramework.Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            Console.WriteLine("TEST");
        }
    }
}


public class UNHANDLED_GROUP_EXCEPTION_CREATE : TestGroup
{
    public override void OnCreate()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }
}

public class UNHANDLED_GROUP_EXCEPTION_DESTROY : TestGroup
{
    public override void OnDestroy()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
        }
    }
}

public class UNHANDLED_GROUP_EXCEPTION_DESTROY2 : TestGroup
{
    public override void OnDestroy()
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }

    public class UNHANDLED_TEST_EXCEPTION : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }
}

public class UNHANDLED_TEST_EXCEPTION : Test
{
    public override void Run(TestGroup nullableInstance)
    {
        object a = null;
        Console.WriteLine(a.Equals(this));
    }
}

public class UNHANDLED_GROUP_EXCEPTION_2 : TestGroup
{
    public class UNHANDLED_TEST_EXCEPTION_1 : Test
    {
        public override void Run(TestGroup nullableInstance)
        {
            object a = null;
            Console.WriteLine(a.Equals(this));
        }
    }

    public class UNHANDLED_TEST_EXCEPTION_2 : Test
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