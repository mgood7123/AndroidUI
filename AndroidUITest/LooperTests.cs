using AndroidUI.Applications;
using AndroidUI.Execution;
using AndroidUITestFramework;

namespace AndroidUITest
{
    class LooperTests : TestGroup
    {
        class _1_TrySetLooper : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                Context c = new();
                Thread t = new(() =>
                {
                    Looper.prepare(c);
                    Looper m = Looper.myLooper(c);
                    Tools.AssertInstanceNotEqual(m, null);
                });
                t.Start();
                t.Join();
            }
        }

        class _2_TrySendToLooper : Test
        {
            class HC : Handler.Callback
            {
                public bool handleMessage(Message msg)
                {
                    Console.WriteLine("HANDLE_MESSAGE");
                    return true;
                }
            }

            public override void Run(TestGroup nullableInstance)
            {
                //Tools.SKIP();
                Context c = new();
                Looper m = null;
                Handler h = null;
                Thread t = new(() =>
                {
                    Console.WriteLine("preparing looper...");
                    Looper.prepare(c);
                    Console.WriteLine("prepared looper");
                    m = Looper.myLooper(c);
                    m.setMessageLogging(Console.Out);
                    h = new Handler(m, new HC());
                    Console.WriteLine("obtained handle");
                    Console.WriteLine("looping...");
                    Looper.loop(c);
                    Console.WriteLine("finished looping, sleeping for 5 seconds");
                    Thread.Sleep(5000);
                });
                Console.WriteLine("joining for 1 second...");
                t.Start();
                t.Join(1000);
                Console.WriteLine("joined for 1 second");
                Console.WriteLine("posting a runnable...");
                h.post(() => Console.WriteLine("POSTED"));
                Console.WriteLine("posted a runnable");
                Console.WriteLine("posting another runnable...");
                h.post(() => Console.WriteLine("POSTED 2"));
                Console.WriteLine("posted another runnable");
                Console.WriteLine("joining for 1 second...");
                t.Join(1000);
                Console.WriteLine("joined for 1 second");
                Console.WriteLine("quiting looper...");
                m.quitSafely();
                Console.WriteLine("quit looper");
                Console.WriteLine("joining until finished");
                t.Join();
                Console.WriteLine("joined and finished");
            }
        }

        class _3_TryMain : Test
        {
            class HC : Handler.Callback
            {
                public bool handleMessage(Message msg)
                {
                    Console.WriteLine("HANDLE_MESSAGE");
                    return true;
                }
            }

            public override void Run(TestGroup nullableInstance)
            {
                Context c = new();
                Thread t = new(() =>
                {
                    Looper m = null;
                    Handler h = null;
                    Console.WriteLine("preparing looper...");
                    Looper.prepareMainLooper(c);
                    Console.WriteLine("prepared looper");
                    m = Looper.getMainLooper(c);
                    m.setMessageLogging(Console.Out);
                    h = new Handler(m, new HC());
                    Console.WriteLine("obtained handle");

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");

                    Console.WriteLine("posting a runnable...");
                    h.post(() => Console.WriteLine("POSTED"));
                    Console.WriteLine("posted a runnable");

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");

                    Console.WriteLine("posting runnables...");
                    h.post(() => Console.WriteLine("POSTED"));
                    h.post(() => Console.WriteLine("POSTED"));
                    h.post(() => Console.WriteLine("POSTED"));
                    Console.WriteLine("posted runnables");

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");

                    m.quitSafely();

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");
                });
                t.Start();
                t.Join();
            }
        }
    }
}
