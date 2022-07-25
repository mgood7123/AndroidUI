using AndroidUI.OS;

namespace AndroidUI.Input
{
    using static NanoTime;

    public partial class Touch
    {
        internal class Batcher
        {
            internal Queue<Data> events = new();
            internal long batchTime = 0;
            internal long pumpTime = 0;
            internal long batch_time_ms = 20;
            internal bool pumpActive = false;

            internal void addBatch(Data touchData)
            {
                if (events.Count == 0)
                {
                    batchTime = currentTimeMillis();
                }
                events.Enqueue(touchData);
            }

            internal bool pump(in Touch touch, State state = State.NONE)
            {
                return pump(touch, false, state);
            }

            internal bool pump(in Touch touch, bool force_pump, State state = State.NONE)
            {
                if (pumpActive)
                {
                    throw new Exception("Attempting to pump while already pumping a batch");
                }

                pumpActive = true;
                int c = events.Count;
                pumpTime = currentTimeMillis();

                bool handled = false;

                if (c != 0 && (force_pump || (pumpTime - batchTime) >= batch_time_ms))
                {
                    if (touch.debug) Console.WriteLine("batch time : " + batchTime);
                    if (touch.debug) Console.WriteLine("pump time  : " + pumpTime);

                    string s = c == 1 ? "" : "s";

                    if (touch.debug) Console.WriteLine("batched " + c + " event" + s);
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        touch.batching = true;
                        Data t;
                        while (events.Count > 1)
                        {
                            t = events.Dequeue();
                            t.batchSource = state;
                            touch.moveTouch(t);
                        }
                        touch.batching = false;
                        t = events.Dequeue();
                        t.batchSource = state;
                        touch.moveTouch(t);
                    }
                    else
                    {
                        try
                        {
                            touch.batching = true;
                            Data t;
                            while (events.Count > 1)
                            {
                                t = events.Dequeue();
                                t.batchSource = state;
                                touch.moveTouch(t);
                            }
                            touch.batching = false;
                            t = events.Dequeue();
                            t.batchSource = state;
                            touch.moveTouch(t);
                        }
                        catch (Exception e)
                        {
                            touch.batching = false;
                            touch.history.Clear();
                            pumpActive = false;
                            // rethrow exception and hope we can be recovered earlier up
                            throw e;
                        }
                    }
                    if (touch.debug) Console.WriteLine("processed " + c + " queued event" + s);
                    handled = true;
                }
                pumpActive = false;
                return handled;
            }

            internal void Clear(Touch touch)
            {
                if (pumpActive)
                {
                    throw new Exception("Attempting to clear while already pumping a batch");
                }

                pumpActive = true;
                int c = events.Count;
                pumpTime = currentTimeMillis();
                if (c != 0)
                {
                    if (touch.debug) Console.WriteLine("batch time : " + batchTime);
                    if (touch.debug) Console.WriteLine("pump time  : " + pumpTime);
                    events.Clear();
                    if (touch.debug) Console.WriteLine("cleared " + c + " queued events");
                }
                pumpActive = false;
            }
        }
    }
}
