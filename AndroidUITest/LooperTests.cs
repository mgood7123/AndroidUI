﻿using AndroidUI.Execution;
using AndroidUITestFramework;

namespace AndroidUITest
{
    class LooperTests : TestGroup
    {
        class TrySetLooper : Test
        {
            public override void Run(TestGroup nullableInstance)
            {
                AndroidUI.Context c = new();
                Thread t = new Thread(() =>
                {
                    Looper.prepare(c);
                    Looper m = Looper.myLooper(c);
                    Tools.AssertInstanceNotEqual(m, null);
                });
                t.Start();
                t.Join();
            }
        }

        class TrySendToLooper : Test
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
                AndroidUI.Context c = new();
                Looper m = null;
                Handler h = null;
                Thread t = new Thread(() =>
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
                h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED")));
                Console.WriteLine("posted a runnable");
                Console.WriteLine("posting another runnable...");
                h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED 2")));
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

        class TryMain : Test
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
                AndroidUI.Context c = new();
                Thread t = new Thread(() =>
                {
                    Looper m = null;
                    Handler h = null;
                    Console.WriteLine("preparing looper...");
                    Looper.prepare(c);
                    Console.WriteLine("prepared looper");
                    m = Looper.myLooper(c);
                    m.setMessageLogging(Console.Out);
                    h = new Handler(m, new HC());
                    Console.WriteLine("obtained handle");

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");

                    Console.WriteLine("posting a runnable...");
                    h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED")));
                    Console.WriteLine("posted a runnable");

                    Console.WriteLine("looping UI");
                    Looper.loopUI(c);
                    Console.WriteLine("finished looping");

                    Console.WriteLine("posting runnables...");
                    h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED")));
                    h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED")));
                    h.post(AndroidUI.Runnable.Create(() => Console.WriteLine("POSTED")));
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