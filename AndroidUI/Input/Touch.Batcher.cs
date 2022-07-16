using AndroidUI.OS;

namespace AndroidUI
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

            internal bool pump(in Touch touch)
            {
                return pump(touch, false);
            }

            internal bool pump(in Touch touch, bool force_pump)
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
                    try
                    {
                        touch.batching = true;
                        while (events.Count > 1)
                        {
                            touch.moveTouch(events.Dequeue());
                        }
                        touch.batching = false;
                        touch.moveTouch(events.Dequeue());
                    } catch (Exception e) {
                        touch.batching = false;
                        touch.history.Clear();
                        pumpActive = false;
                        // rethrow exception and hope we can be recovered earlier up
                        throw e;
                    }
                    if (touch.debug) Console.WriteLine("processed " + c + " queued event" + s);
                    handled = true;
                }
                pumpActive = false;
                return handled;
            }
        }
    }
}
