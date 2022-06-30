/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AndroidUI.Exceptions;

namespace AndroidUI.Execution
{
    /**
      * Class used to run a message loop for a thread.  Threads by default do
      * not have a message loop associated with them; to create one, call
      * {@link #prepare} in the thread that is to run the loop, and then
      * {@link #loop} to have it process messages until the loop is stopped.
      *
      * <p>Most interaction with a message loop is through the
      * {@link Handler} class.
      *
      * <p>This is a typical example of the implementation of a Looper thread,
      * using the separation of {@link #prepare} and {@link #loop} to create an
      * initial Handler to communicate with the Looper.
      *
      * <pre>
      *  class LooperThread extends Thread {
      *      public Handler mHandler;
      *
      *      public void run() {
      *          Looper.prepare();
      *
      *          mHandler = new Handler(Looper.myLooper()) {
      *              public void handleMessage(Message msg) {
      *                  // process incoming messages here
      *              }
      *          };
      *
      *          Looper.loop();
      *      }
      *  }</pre>
      */
    public sealed class Looper
    {
        internal Context context;

        /*
         * API Implementation Note:
         *
         * This class contains the code required to set up and manage an event loop
         * based on MessageQueue.  APIs that affect the state of the queue should be
         * defined on MessageQueue or Handler rather than on Looper itself.  For example,
         * idle handlers and sync barriers are defined on the queue whereas preparing the
         * thread, looping, and quitting are defined on the looper.
         */

        private const string TAG = "Looper";

        internal MessageQueue mQueue;
        internal Thread mThread;
        private bool mInLoop;

        private TextWriter mLogging;
        private long mTraceTag;

        /**
         * If set, the looper will show a warning log if a message dispatch takes longer than this.
         */
        private long mSlowDispatchThresholdMs;

        /**
         * If set, the looper will show a warning log if a message delivery (actual delivery time -
         * post time) takes longer than this.
         */
        private long mSlowDeliveryThresholdMs;

        /**
         * True if a message delivery takes longer than {@link #mSlowDeliveryThresholdMs}.
         */
        private bool mSlowDeliveryDetected;

        /** Initialize the current thread as a looper.
          * This gives you a chance to create handlers that then reference
          * this looper, before actually starting the loop. Be sure to call
          * {@link #loop()} after calling this method, and end it by calling
          * {@link #quit()}.
          */
        public static void prepare(Context context)
        {
            prepare(context, true, Thread.CurrentThread);
        }

        private static void prepare(Context context, bool quitAllowed, Thread thread = null)
        {
            var looper = context.storage.Get<Looper>(StorageKeys.LOOPER, thread);
            if (looper != null)
            {
                throw new Exception("Only one Looper may be created per thread");
            }
            Looper l = new(context, quitAllowed);
            context.storage.SetOrCreate<Looper>(StorageKeys.LOOPER, l, () => l, thread);
        }

        /**
         * Initialize the current thread as a looper, marking it as an
         * application's main looper. See also: {@link #prepare()}
         *
         * @deprecated The main looper for your application is created by the Android environment,
         *   so you should never need to call this function yourself.
         */
        internal static void prepareMainLooper(Context context)
        {
            var looper = context.storage.Get<Looper>(StorageKeys.LOOPER);
            if (looper != null)
            {
                throw new IllegalStateException("The main Looper has already been prepared.");
            }
            prepare(context, true);
        }

        /**
         * Returns the application's main looper, which lives in the main thread of the application.
         */
        public static Looper getMainLooper(Context context)
        {
            return context.storage.Get<Looper>(StorageKeys.LOOPER)?.Value;
        }

        /**
         * Set the transaction observer for all Loopers in this process.
         *
         * @hide
         */
        internal static void setObserver(Context context, Observer observer)
        {
            context.storage.SetOrCreate<Observer>(StorageKeys.LOOPER_OBSERVER, (ValueHolder<Observer>)observer, () => observer);
        }

        static void LogEnter(TextWriter logging, string msg)
        {
            if (logging != null)
            {
                logging.WriteLine(">>>>> " + msg);
            }
        }

        static void LogExit(TextWriter logging, string msg)
        {
            if (logging != null)
            {
                logging.WriteLine("<<<<< " + msg);
            }
        }

        /**
         * Run the message queue in this thread. Be sure to call
         * {@link #quit()} to end the loop.
         */
        public static void loopUI(Context context)
        {
            Looper me = getMainLooper(context);
            if (me == null)
            {
                throw new Exception("No Looper; Looper.prepare() wasn't called on this thread.");
            }
            if (!me.mInLoop)
            {
                me.mInLoop = true;

                // Make sure the identity of this thread is that of the local process,
                // and keep track of what that identity token actually is.
                //Binder.clearCallingIdentity();
                long ident = 0; // Binder.clearCallingIdentity();

                // Allow overriding a threshold with a system prop. e.g.
                // adb shell 'setprop log.looper.1000.main.slow 1 && stop && start'
                int thresholdOverride = 0;

                me.mSlowDeliveryDetected = false;
                for (;;)
                {
                    if (!loopOnceUI(me, ident, thresholdOverride))
                    {
                        me.mInLoop = false;
                        return;
                    }
                }
            }
        }

        /**
         * Poll and deliver single message, return true if the outer loop should continue.
         */
        private static bool loopOnceUI(Looper me,
                long ident, int thresholdOverride)
        {
            // This must be in a local variable, in case a UI event sets the logger
            TextWriter logging = me.mLogging;

            Message msg = me.mQueue.nextUI(); // might block

            if (msg == null)
            {
                //Handler h = new(me, new Handler.ActionCallback(msg =>
                //{
                //    Console.WriteLine("Recieved message: " + msg);
                //    return false;
                //}));
                //h.sendEmptyMessageDelayed(0, 1000);
                // No message indicates that the message queue is quitting.
                return false;
            }

            if (msg.waiting)
            {
                return false;
            }

            LogEnter(logging, "Dispatching to " + msg.target + " " + msg.callback + ": " + msg.what);

            // Make sure the observer won't change while processing a transaction.
            Observer observer = me.context.storage.Get<Observer>(StorageKeys.LOOPER_OBSERVER)?.Value;

            long traceTag = me.mTraceTag;
            long slowDispatchThresholdMs = me.mSlowDispatchThresholdMs;
            long slowDeliveryThresholdMs = me.mSlowDeliveryThresholdMs;
            if (thresholdOverride > 0)
            {
                slowDispatchThresholdMs = thresholdOverride;
                slowDeliveryThresholdMs = thresholdOverride;
            }
            bool logSlowDelivery = (slowDeliveryThresholdMs > 0) && (msg.when > 0);
            bool logSlowDispatch = slowDispatchThresholdMs > 0;

            bool needStartTime = logSlowDelivery || logSlowDispatch;
            bool needEndTime = logSlowDispatch;

            //if (traceTag != 0 && Trace.isTagEnabled(traceTag))
            //{
            //    Trace.traceBegin(traceTag, msg.target.getTraceName(msg));
            //}

            long dispatchStart = needStartTime ? NanoTime.currentTimeMillis() : 0;
            long dispatchEnd;
            object token = null;
            if (observer != null)
            {
                token = observer.messageDispatchStarting();
            }

            ThreadLocalWorkSource ThreadLocalWorkSource = me.context.storage.GetOrCreate<ThreadLocalWorkSource>(StorageKeys.TLW, () => new()).Value;

            long origWorkSource = ThreadLocalWorkSource.setUid(msg.workSourceUid);
            try
            {
                msg.target.dispatchMessage(msg);
                if (observer != null)
                {
                    observer.messageDispatched(token, msg);
                }
                dispatchEnd = needEndTime ? NanoTime.currentTimeMillis() : 0;
            }
            catch (Exception exception)
            {
                if (observer != null)
                {
                    observer.dispatchingThrewException(token, msg, exception);
                }
                throw;
            }
            finally
            {
                ThreadLocalWorkSource.restore(origWorkSource);
                //if (traceTag != 0)
                //{
                //    Trace.traceEnd(traceTag);
                //}
            }
            if (logSlowDelivery)
            {
                if (me.mSlowDeliveryDetected)
                {
                    if ((dispatchStart - msg.when) <= 10)
                    {
                        Log.w(TAG, "Drained");
                        me.mSlowDeliveryDetected = false;
                    }
                }
                else
                {
                    if (showSlowLog(slowDeliveryThresholdMs, msg.when, dispatchStart, "delivery",
                            msg))
                    {
                        // Once we write a slow delivery log, suppress until the queue drains.
                        me.mSlowDeliveryDetected = true;
                    }
                }
            }
            if (logSlowDispatch)
            {
                showSlowLog(slowDispatchThresholdMs, dispatchStart, dispatchEnd, "dispatch", msg);
            }

            LogExit(logging, "Finished to " + msg.target + " " + msg.callback);

            // Make sure that during the course of dispatching the
            // identity of the thread wasn't corrupted.
            long newIdent = 0; //Binder.clearCallingIdentity();
            if (ident != newIdent)
            {
                Log.e(TAG, "Thread identity changed from 0x"
                        + Extensions.IntegerExtensions.toHexString((int)ident) + " to 0x"
                        + Extensions.IntegerExtensions.toHexString((int)newIdent) + " while dispatching to "
                        + msg.target.GetType().Name + " "
                        + msg.callback + " what=" + msg.what);
            }

            msg.recycleUnchecked();

            return true;
        }

        /**
         * Run the message queue in this thread. Be sure to call
         * {@link #quit()} to end the loop.
         */
        public static void loop(Context context)
        {
            Looper me = myLooper(context);
            if (me == null)
            {
                throw new Exception("No Looper; Looper.prepare() wasn't called on this thread.");
            }
            if (me.mInLoop)
            {
                Log.w(TAG, "Loop again would have the queued messages be executed"
                        + " before this one completed.");
            }

            me.mInLoop = true;

            // Make sure the identity of this thread is that of the local process,
            // and keep track of what that identity token actually is.
            //Binder.clearCallingIdentity();
            long ident = 0; // Binder.clearCallingIdentity();

            // Allow overriding a threshold with a system prop. e.g.
            // adb shell 'setprop log.looper.1000.main.slow 1 && stop && start'
            int thresholdOverride = 0;

            me.mSlowDeliveryDetected = false;

            // This must be in a local variable, in case a UI event sets the logger
            TextWriter logging = me.mLogging;
            for (;;)
            {
                LogEnter(logging, "Looping Once");
                if (!loopOnce(me, ident, thresholdOverride))
                {
                    LogExit(logging, "Finished Looping Once, Quitting Loop");
                    return;
                }
                LogExit(logging, "Finished Looping Once");
            }
        }

        /**
         * Poll and deliver single message, return true if the outer loop should continue.
         */
        private static bool loopOnce(Looper me,
                long ident, int thresholdOverride)
        {
            // This must be in a local variable, in case a UI event sets the logger
            TextWriter logging = me.mLogging;

            Message msg = me.mQueue.next(); // might block

            if (msg == null)
            {
                // No message indicates that the message queue is quitting.
                return false;
            }

            LogEnter(logging, "Dispatching to " + msg.target + " " + msg.callback + ": " + msg.what);

            // Make sure the observer won't change while processing a transaction.
            Observer observer = me.context.storage.Get<Observer>(StorageKeys.LOOPER_OBSERVER)?.Value;

            long traceTag = me.mTraceTag;
            long slowDispatchThresholdMs = me.mSlowDispatchThresholdMs;
            long slowDeliveryThresholdMs = me.mSlowDeliveryThresholdMs;
            if (thresholdOverride > 0)
            {
                slowDispatchThresholdMs = thresholdOverride;
                slowDeliveryThresholdMs = thresholdOverride;
            }
            bool logSlowDelivery = (slowDeliveryThresholdMs > 0) && (msg.when > 0);
            bool logSlowDispatch = slowDispatchThresholdMs > 0;

            bool needStartTime = logSlowDelivery || logSlowDispatch;
            bool needEndTime = logSlowDispatch;

            //if (traceTag != 0 && Trace.isTagEnabled(traceTag))
            //{
            //    Trace.traceBegin(traceTag, msg.target.getTraceName(msg));
            //}

            long dispatchStart = needStartTime ? NanoTime.currentTimeMillis() : 0;
            long dispatchEnd;
            object token = null;
            if (observer != null)
            {
                token = observer.messageDispatchStarting();
            }
            ThreadLocalWorkSource ThreadLocalWorkSource = me.context.storage.GetOrCreate<ThreadLocalWorkSource>(StorageKeys.TLW, () => new()).Value;

            long origWorkSource = ThreadLocalWorkSource.setUid(msg.workSourceUid);
            try
            {
                msg.target.dispatchMessage(msg);
                if (observer != null)
                {
                    observer.messageDispatched(token, msg);
                }
                dispatchEnd = needEndTime ? NanoTime.currentTimeMillis() : 0;
            }
            catch (Exception exception)
            {
                if (observer != null)
                {
                    observer.dispatchingThrewException(token, msg, exception);
                }
                throw exception;
            }
            finally
            {
                ThreadLocalWorkSource.restore(origWorkSource);
                //if (traceTag != 0)
                //{
                //    Trace.traceEnd(traceTag);
                //}
            }
            if (logSlowDelivery)
            {
                if (me.mSlowDeliveryDetected)
                {
                    if ((dispatchStart - msg.when) <= 10)
                    {
                        Log.w(TAG, "Drained");
                        me.mSlowDeliveryDetected = false;
                    }
                }
                else
                {
                    if (showSlowLog(slowDeliveryThresholdMs, msg.when, dispatchStart, "delivery",
                            msg))
                    {
                        // Once we write a slow delivery log, suppress until the queue drains.
                        me.mSlowDeliveryDetected = true;
                    }
                }
            }
            if (logSlowDispatch)
            {
                showSlowLog(slowDispatchThresholdMs, dispatchStart, dispatchEnd, "dispatch", msg);
            }

            LogExit(logging, "Finished to " + msg.target + " " + msg.callback);

            // Make sure that during the course of dispatching the
            // identity of the thread wasn't corrupted.
            long newIdent = 0; //Binder.clearCallingIdentity();
            if (ident != newIdent)
            {
                Log.e(TAG, "Thread identity changed from 0x"
                        + Extensions.IntegerExtensions.toHexString((int)ident) + " to 0x"
                        + Extensions.IntegerExtensions.toHexString((int)newIdent) + " while dispatching to "
                        + msg.target.GetType().Name + " "
                        + msg.callback + " what=" + msg.what);
            }

            msg.recycleUnchecked();

            return true;
        }

        private static bool showSlowLog(long threshold, long measureStart, long measureEnd,
                string what, Message msg)
        {
            long actualTime = measureEnd - measureStart;
            if (actualTime < threshold)
            {
                return false;
            }
            // For slow delivery, the current message isn't really important, but log it anyway.
            Log.w(TAG, "Slow " + what + " took " + actualTime + "ms "
                    + Thread.CurrentThread.Name + " h="
                    + msg.target.GetType().Name + " c=" + msg.callback + " m=" + msg.what);
            return true;
        }

        /**
         * Return the Looper object associated with the current thread.  Returns
         * null if the calling thread is not associated with a Looper.
         */
        public static Looper myLooper(Context context)
        {
            return context.storage.Get<Looper>(StorageKeys.LOOPER, Thread.CurrentThread)?.Value;
        }

        /**
         * Return the {@link MessageQueue} object associated with the current
         * thread.  This must be called from a thread running a Looper, or a
         * NullPointerException will be thrown.
         */
        public static MessageQueue myQueue(Context context)
        {
            return myLooper(context).mQueue;
        }

        private Looper(Context context, bool quitAllowed)
        {
            this.context = context;
            mQueue = new MessageQueue(quitAllowed);
            mThread = Thread.CurrentThread;
        }

        /**
         * Returns true if the current thread is this looper's thread.
         */
        public bool isCurrentThread()
        {
            return Thread.CurrentThread == mThread;
        }

        /**
         * Control logging of messages as they are processed by this Looper.  If
         * enabled, a log message will be written to <var>printer</var>
         * at the beginning and ending of each message dispatch, identifying the
         * target Handler and message contents.
         *
         * @param printer A Printer object that will receive log messages, or
         * null to disable message logging.
         */
        public void setMessageLogging(TextWriter printer)
        {
            mLogging = printer;
        }

        /** {@hide} */
        internal void setTraceTag(long traceTag)
        {
            mTraceTag = traceTag;
        }

        /**
         * Set a thresholds for slow dispatch/delivery log.
         * {@hide}
         */
        internal void setSlowLogThresholdMs(long slowDispatchThresholdMs, long slowDeliveryThresholdMs)
        {
            mSlowDispatchThresholdMs = slowDispatchThresholdMs;
            mSlowDeliveryThresholdMs = slowDeliveryThresholdMs;
        }

        /**
         * Quits the looper.
         * <p>
         * Causes the {@link #loop} method to terminate without processing any
         * more messages in the message queue.
         * </p><p>
         * Any attempt to post messages to the queue after the looper is asked to quit will fail.
         * For example, the {@link Handler#sendMessage(Message)} method will return false.
         * </p><p class="note">
         * Using this method may be unsafe because some messages may not be delivered
         * before the looper terminates.  Consider using {@link #quitSafely} instead to ensure
         * that all pending work is completed in an orderly manner.
         * </p>
         *
         * @see #quitSafely
         */
        public void quit()
        {
            mQueue.quit(false);
        }

        /**
         * Quits the looper safely.
         * <p>
         * Causes the {@link #loop} method to terminate as soon as all remaining messages
         * in the message queue that are already due to be delivered have been handled.
         * However pending delayed messages with due times in the future will not be
         * delivered before the loop terminates.
         * </p><p>
         * Any attempt to post messages to the queue after the looper is asked to quit will fail.
         * For example, the {@link Handler#sendMessage(Message)} method will return false.
         * </p>
         */
        public void quitSafely()
        {
            mQueue.quit(true);
        }

        /**
         * Gets the Thread associated with this Looper.
         *
         * @return The looper's thread.
         */
        public Thread getThread()
        {
            return mThread;
        }

        /**
         * Gets this looper's message queue.
         *
         * @return The looper's message queue.
         */
        public MessageQueue getQueue()
        {
            return mQueue;
        }

        override public string ToString()
        {
            return "Looper (" + mThread.Name + ", tid " + mThread.ManagedThreadId
                    + ") {" + Extensions.IntegerExtensions.toHexString(GetHashCode()) + "}";
        }

        /** {@hide} */
        internal interface Observer
        {
            /**
             * Called right before a message is dispatched.
             *
             * <p> The token type is not specified to allow the implementation to specify its own type.
             *
             * @return a token used for collecting telemetry when dispatching a single message.
             *         The token token must be passed back exactly once to either
             *         {@link Observer#messageDispatched} or {@link Observer#dispatchingThrewException}
             *         and must not be reused again.
             *
             */
            object messageDispatchStarting();

            /**
             * Called when a message was processed by a Handler.
             *
             * @param token Token obtained by previously calling
             *              {@link Observer#messageDispatchStarting} on the same Observer instance.
             * @param msg The message that was dispatched.
             */
            void messageDispatched(object token, Message msg);

            /**
             * Called when an exception was thrown while processing a message.
             *
             * @param token Token obtained by previously calling
             *              {@link Observer#messageDispatchStarting} on the same Observer instance.
             * @param msg The message that was dispatched and caused an exception.
             * @param exception The exception that was thrown.
             */
            void dispatchingThrewException(object token, Message msg, Exception exception);
        }
    }
}