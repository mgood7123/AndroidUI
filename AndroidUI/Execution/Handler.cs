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

using AndroidUI.Applications;
using AndroidUI.Exceptions;
using AndroidUI.Extensions;
using AndroidUI.OS;
using AndroidUI.Utils;
using System.Text;

namespace AndroidUI.Execution
{

    /**
     * A Handler allows you to send and process {@link Message} and Runnable
     * objects associated with a thread's {@link MessageQueue}.  Each Handler
     * instance is associated with a single thread and that thread's message
     * queue. When you create a new Handler it is bound to a {@link Looper}.
     * It will deliver messages and runnables to that Looper's message
     * queue and execute them on that Looper's thread.
     *
     * <p>There are two main uses for a Handler: (1) to schedule messages and
     * runnables to be executed at some point in the future; and (2) to enqueue
     * an action to be performed on a different thread than your own.
     *
     * <p>Scheduling messages is accomplished with the
     * {@link #post}, {@link #postAtTime(Runnable, long)},
     * {@link #postDelayed}, {@link #sendEmptyMessage},
     * {@link #sendMessage}, {@link #sendMessageAtTime}, and
     * {@link #sendMessageDelayed} methods.  The <em>post</em> versions allow
     * you to enqueue Runnable objects to be called by the message queue when
     * they are received; the <em>sendMessage</em> versions allow you to enqueue
     * a {@link Message} object containing a bundle of data that will be
     * processed by the Handler's {@link #handleMessage} method (requiring that
     * you implement a subclass of Handler).
     * 
     * <p>When posting or sending to a Handler, you can either
     * allow the item to be processed as soon as the message queue is ready
     * to do so, or specify a delay before it gets processed or absolute time for
     * it to be processed.  The latter two allow you to implement timeouts,
     * ticks, and other timing-based behavior.
     * 
     * <p>When a
     * process is created for your application, its main thread is dedicated to
     * running a message queue that takes care of managing the top-level
     * application objects (activities, broadcast receivers, etc) and any windows
     * they create.  You can create your own threads, and communicate back with
     * the main application thread through a Handler.  This is done by calling
     * the same <em>post</em> or <em>sendMessage</em> methods as before, but from
     * your new thread.  The given Runnable or Message will then be scheduled
     * in the Handler's message queue and processed when appropriate.
     */
    public class Handler
    {
        /*
         * Set this flag to true to detect anonymous, local or member classes
         * that extend this Handler class and that are not static. These kind
         * of classes can potentially create leaks.
         */
        internal static bool FIND_POTENTIAL_LEAKS = false;
        private const string TAG = "Handler";

        /**
         * Callback interface you can use when instantiating a Handler to avoid
         * having to implement your own subclass of Handler.
         */
        public interface Callback
        {
            /**
             * @param msg A {@link android.os.Message Message} object
             * @return True if no further handling is desired
             */
            abstract bool handleMessage(Message msg);
        }

        public class ActionCallback : Callback
        {
            RunnableWithReturn<Message, bool> func;

            public ActionCallback(RunnableWithReturn<Message, bool> func)
            {
                ArgumentNullException.ThrowIfNull(func, nameof(func));
                this.func = func;
            }

            public bool handleMessage(Message msg)
            {
                return func.Invoke(msg);
            }
        }

        /**
         * Subclasses must implement this to receive messages.
         */
        virtual public void handleMessage(Message msg)
        {
        }

        /**
         * Handle system messages here.
         */
        public void dispatchMessage(Message msg)
        {
            if (msg.callback != null)
            {
                handleCallback(msg);
            }
            else
            {
                if (mCallback != null)
                {
                    if (mCallback.handleMessage(msg))
                    {
                        return;
                    }
                }
                handleMessage(msg);
            }
        }

        /**
         * Default constructor associates this handler with the {@link Looper} for the
         * current thread.
         *
         * If this thread does not have a looper, this handler won't be able to receive messages
         * so an exception is thrown.
         *
         * @deprecated Implicitly choosing a Looper during Handler construction can lead to bugs
         *   where operations are silently lost (if the Handler is not expecting new tasks and quits),
         *   crashes (if a handler is sometimes created on a thread without a Looper active), or race
         *   conditions, where the thread a handler is associated with is not what the author
         *   anticipated. Instead, use an {@link java.util.concurrent.Executor} or specify the Looper
         *   explicitly, using {@link Looper#getMainLooper}, {link android.view.View#getHandler}, or
         *   similar. If the implicit thread local behavior is required for compatibility, use
         *   {@code new Handler(Looper.myLooper())} to make it clear to readers.
         *
         */
        public Handler(Context context) : this(context, null, false)
        {
        }

        /**
         * Constructor associates this handler with the {@link Looper} for the
         * current thread and takes a callback interface in which you can handle
         * messages.
         *
         * If this thread does not have a looper, this handler won't be able to receive messages
         * so an exception is thrown.
         *
         * @param callback The callback interface in which to handle messages, or null.
         *
         * @deprecated Implicitly choosing a Looper during Handler construction can lead to bugs
         *   where operations are silently lost (if the Handler is not expecting new tasks and quits),
         *   crashes (if a handler is sometimes created on a thread without a Looper active), or race
         *   conditions, where the thread a handler is associated with is not what the author
         *   anticipated. Instead, use an {@link java.util.concurrent.Executor} or specify the Looper
         *   explicitly, using {@link Looper#getMainLooper}, {link android.view.View#getHandler}, or
         *   similar. If the implicit thread local behavior is required for compatibility, use
         *   {@code new Handler(Looper.myLooper(), callback)} to make it clear to readers.
         */
        public Handler(Context context, Callback callback) : this(context, callback, false)
        {
        }

        /**
         * Use the provided {@link Looper} instead of the default one.
         *
         * @param looper The looper, must not be null.
         */
        public Handler(Looper looper) : this(looper, null, false)
        {
        }

        /**
         * Use the provided {@link Looper} instead of the default one and take a callback
         * interface in which to handle messages.
         *
         * @param looper The looper, must not be null.
         * @param callback The callback interface in which to handle messages, or null.
         */
        public Handler(Looper looper, Callback callback) : this(looper, callback, false)
        {
        }

        /**
         * Use the {@link Looper} for the current thread
         * and set whether the handler should be asynchronous.
         *
         * Handlers are synchronous by default unless this constructor is used to make
         * one that is strictly asynchronous.
         *
         * Asynchronous messages represent interrupts or events that do not require global ordering
         * with respect to synchronous messages.  Asynchronous messages are not subject to
         * the synchronization barriers introduced by {@link MessageQueue#enqueueSyncBarrier(long)}.
         *
         * @param async If true, the handler calls {@link Message#setAsynchronous(bool)} for
         * each {@link Message} that is sent to it or {@link Runnable} that is posted to it.
         *
         * @hide
         */
        internal Handler(Context context, bool async) : this(context, null, async)
        {
        }

        /**
         * Use the {@link Looper} for the current thread with the specified callback interface
         * and set whether the handler should be asynchronous.
         *
         * Handlers are synchronous by default unless this constructor is used to make
         * one that is strictly asynchronous.
         *
         * Asynchronous messages represent interrupts or events that do not require global ordering
         * with respect to synchronous messages.  Asynchronous messages are not subject to
         * the synchronization barriers introduced by {@link MessageQueue#enqueueSyncBarrier(long)}.
         *
         * @param callback The callback interface in which to handle messages, or null.
         * @param async If true, the handler calls {@link Message#setAsynchronous(bool)} for
         * each {@link Message} that is sent to it or {@link Runnable} that is posted to it.
         *
         * @hide
         */
        internal Handler(Context context, Callback callback, bool async)
        {
            //if (FIND_POTENTIAL_LEAKS)
            //{
            //    Class<? extends Handler> klass = getClass();
            //    if ((klass.isAnonymousClass() || klass.isMemberClass() || klass.isLocalClass()) &&
            //            (klass.getModifiers() & Modifier.STATIC) == 0)
            //    {
            //        Log.w(TAG, "The following Handler class should be static or leaks might occur: " +
            //            klass.getCanonicalName());
            //    }
            //}

            mLooper = Looper.myLooper(context);
            if (mLooper == null)
            {
                throw new Exception(
                    "Can't create handler inside thread " + Thread.CurrentThread
                            + " that has not called Looper.prepare()");
            }
            mQueue = mLooper.mQueue;
            mCallback = callback;
            mAsynchronous = async;
        }

        /**
         * Use the provided {@link Looper} instead of the default one and take a callback
         * interface in which to handle messages.  Also set whether the handler
         * should be asynchronous.
         *
         * Handlers are synchronous by default unless this constructor is used to make
         * one that is strictly asynchronous.
         *
         * Asynchronous messages represent interrupts or events that do not require global ordering
         * with respect to synchronous messages.  Asynchronous messages are not subject to
         * the synchronization barriers introduced by conditions such as display vsync.
         *
         * @param looper The looper, must not be null.
         * @param callback The callback interface in which to handle messages, or null.
         * @param async If true, the handler calls {@link Message#setAsynchronous(bool)} for
         * each {@link Message} that is sent to it or {@link Runnable} that is posted to it.
         *
         * @hide
         */
        internal Handler(Looper looper, Callback callback, bool async)
        {
            mLooper = looper;
            mQueue = looper.mQueue;
            mCallback = callback;
            mAsynchronous = async;
        }

        /**
         * Create a new Handler whose posted messages and runnables are not subject to
         * synchronization barriers such as display vsync.
         *
         * <p>Messages sent to an async handler are guaranteed to be ordered with respect to one another,
         * but not necessarily with respect to messages from other Handlers.</p>
         *
         * @see #createAsync(Looper, Callback) to create an async Handler with custom message handling.
         *
         * @param looper the Looper that the new Handler should be bound to
         * @return a new async Handler instance
         */
        public static Handler createAsync(Looper looper)
        {
            if (looper == null) throw new NullReferenceException("looper must not be null");
            return new Handler(looper, null, true);
        }

        /**
         * Create a new Handler whose posted messages and runnables are not subject to
         * synchronization barriers such as display vsync.
         *
         * <p>Messages sent to an async handler are guaranteed to be ordered with respect to one another,
         * but not necessarily with respect to messages from other Handlers.</p>
         *
         * @see #createAsync(Looper) to create an async Handler without custom message handling.
         *
         * @param looper the Looper that the new Handler should be bound to
         * @return a new async Handler instance
         */
        public static Handler createAsync(Looper looper, Callback callback)
        {
            if (looper == null) throw new NullReferenceException("looper must not be null");
            if (callback == null) throw new NullReferenceException("callback must not be null");
            return new Handler(looper, callback, true);
        }

        /** @hide */
        internal static Handler getMain(Context context)
        {
            return context.storage.GetOrCreate<Handler>(StorageKeys.MAIN_THREAD_HANDLER, () => new Handler(Looper.getMainLooper(context))).Value;
        }

        /** @hide */
        internal static Handler mainIfNull(Context context, Handler handler)
        {
            return handler == null ? getMain(context) : handler;
        }

        /** {@hide} */
        internal string getTraceName(Message message)
        {
            StringBuilder sb = new();
            sb.Append(GetType().Name).Append(": ");
            if (message.callback != null)
            {
                sb.Append(message.callback.GetType().Name);
            }
            else
            {
                sb.Append("#").Append(message.what);
            }
            return sb.ToString();
        }

        /**
         * Returns a string representing the name of the specified message.
         * The default implementation will either return the class name of the
         * message callback if any, or the hexadecimal representation of the
         * message "what" field.
         *  
         * @param message The message whose name is being queried 
         */
        virtual public string getMessageName(Message message)
        {
            if (message.callback != null)
            {
                return message.callback.GetType().Name;
            }
            return "0x" + IntegerExtensions.toHexString(message.what);
        }

        /**
         * Returns a new {@link android.os.Message Message} from the global message pool. More efficient than
         * creating and allocating new instances. The retrieved message has its handler set to this instance (Message.target == this).
         *  If you don't want that facility, just call Message.obtain() instead.
         */
        public Message obtainMessage()
        {
            return Message.obtain(this);
        }

        /**
         * Same as {@link #obtainMessage()}, except that it also sets the what member of the returned Message.
         * 
         * @param what Value to assign to the returned Message.what field.
         * @return A Message from the global message pool.
         */
        public Message obtainMessage(int what)
        {
            return Message.obtain(this, what);
        }

        /**
         * 
         * Same as {@link #obtainMessage()}, except that it also sets the what and obj members 
         * of the returned Message.
         * 
         * @param what Value to assign to the returned Message.what field.
         * @param obj Value to assign to the returned Message.obj field.
         * @return A Message from the global message pool.
         */
        public Message obtainMessage(int what, object obj)
        {
            return Message.obtain(this, what, obj);
        }

        /**
         * 
         * Same as {@link #obtainMessage()}, except that it also sets the what, arg1 and arg2 members of the returned
         * Message.
         * @param what Value to assign to the returned Message.what field.
         * @param arg1 Value to assign to the returned Message.arg1 field.
         * @param arg2 Value to assign to the returned Message.arg2 field.
         * @return A Message from the global message pool.
         */
        public Message obtainMessage(int what, int arg1, int arg2)
        {
            return Message.obtain(this, what, arg1, arg2);
        }

        /**
         * 
         * Same as {@link #obtainMessage()}, except that it also sets the what, obj, arg1,and arg2 values on the 
         * returned Message.
         * @param what Value to assign to the returned Message.what field.
         * @param arg1 Value to assign to the returned Message.arg1 field.
         * @param arg2 Value to assign to the returned Message.arg2 field.
         * @param obj Value to assign to the returned Message.obj field.
         * @return A Message from the global message pool.
         */
        public Message obtainMessage(int what, int arg1, int arg2, object obj)
        {
            return Message.obtain(this, what, arg1, arg2, obj);
        }

        /**
         * Causes the Runnable r to be added to the message queue.
         * The runnable will be run on the thread to which this handler is 
         * attached. 
         *  
         * @param r The Runnable that will be executed.
         * 
         * @return Returns true if the Runnable was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool post(Runnable r)
        {
            return sendMessageDelayed(getPostMessage(r), 0);
        }

        public bool post(Runnable r, int what)
        {
            return sendMessageDelayed(getPostMessage(r, what), 0);
        }

        /**
         * Causes the Runnable r to be added to the message queue, to be run
         * at a specific time given by <var>uptimeMillis</var>.
         * <b>The time-base is {@link android.os.SystemClock#uptimeMillis}.</b>
         * Time spent in deep sleep will add an additional delay to execution.
         * The runnable will be run on the thread to which this handler is attached.
         *
         * @param r The Runnable that will be executed.
         * @param uptimeMillis The absolute time at which the callback should run,
         *         using the {@link android.os.SystemClock#uptimeMillis} time-base.
         *  
         * @return Returns true if the Runnable was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the Runnable will be processed -- if
         *         the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         */
        public bool postAtTime(Runnable r, long uptimeMillis)
        {
            return sendMessageAtTime(getPostMessage(r), uptimeMillis);
        }

        /**
         * Causes the Runnable r to be added to the message queue, to be run
         * at a specific time given by <var>uptimeMillis</var>.
         * <b>The time-base is {@link android.os.SystemClock#uptimeMillis}.</b>
         * Time spent in deep sleep will add an additional delay to execution.
         * The runnable will be run on the thread to which this handler is attached.
         *
         * @param r The Runnable that will be executed.
         * @param token An instance which can be used to cancel {@code r} via
         *         {@link #removeCallbacksAndMessages}.
         * @param uptimeMillis The absolute time at which the callback should run,
         *         using the {@link android.os.SystemClock#uptimeMillis} time-base.
         * 
         * @return Returns true if the Runnable was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the Runnable will be processed -- if
         *         the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         *         
         * @see android.os.SystemClock#uptimeMillis
         */
        public bool postAtTime(
                Runnable r, object token, long uptimeMillis)
        {
            return sendMessageAtTime(getPostMessage(r, token), uptimeMillis);
        }

        /**
         * Causes the Runnable r to be added to the message queue, to be run
         * after the specified amount of time elapses.
         * The runnable will be run on the thread to which this handler
         * is attached.
         * <b>The time-base is {@link android.os.SystemClock#uptimeMillis}.</b>
         * Time spent in deep sleep will add an additional delay to execution.
         *  
         * @param r The Runnable that will be executed.
         * @param delayMillis The delay (in milliseconds) until the Runnable
         *        will be executed.
         *        
         * @return Returns true if the Runnable was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the Runnable will be processed --
         *         if the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         */
        public bool postDelayed(Runnable r, long delayMillis)
        {
            return sendMessageDelayed(getPostMessage(r), delayMillis);
        }

        /** @hide */
        internal bool postDelayed(Runnable r, int what, long delayMillis)
        {
            return sendMessageDelayed(getPostMessage(r, what), delayMillis);
        }

        /**
         * Causes the Runnable r to be added to the message queue, to be run
         * after the specified amount of time elapses.
         * The runnable will be run on the thread to which this handler
         * is attached.
         * <b>The time-base is {@link android.os.SystemClock#uptimeMillis}.</b>
         * Time spent in deep sleep will add an additional delay to execution.
         *
         * @param r The Runnable that will be executed.
         * @param token An instance which can be used to cancel {@code r} via
         *         {@link #removeCallbacksAndMessages}.
         * @param delayMillis The delay (in milliseconds) until the Runnable
         *        will be executed.
         *
         * @return Returns true if the Runnable was successfully placed in to the
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the Runnable will be processed --
         *         if the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         */
        public bool postDelayed(
                Runnable r, object token, long delayMillis)
        {
            return sendMessageDelayed(getPostMessage(r, token), delayMillis);
        }

        /**
         * Posts a message to an object that implements Runnable.
         * Causes the Runnable r to executed on the next iteration through the
         * message queue. The runnable will be run on the thread to which this
         * handler is attached.
         * <b>This method is only for use in very special circumstances -- it
         * can easily starve the message queue, cause ordering problems, or have
         * other unexpected side-effects.</b>
         *  
         * @param r The Runnable that will be executed.
         * 
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool postAtFrontOfQueue(Runnable r)
        {
            return sendMessageAtFrontOfQueue(getPostMessage(r));
        }

        /**
         * Runs the specified task synchronously.
         * <p>
         * If the current thread is the same as the handler thread, then the runnable
         * runs immediately without being enqueued.  Otherwise, posts the runnable
         * to the handler and waits for it to complete before returning.
         * </p><p>
         * This method is dangerous!  Improper use can result in deadlocks.
         * Never call this method while any locks are held or use it in a
         * possibly re-entrant manner.
         * </p><p>
         * This method is occasionally useful in situations where a background thread
         * must synchronously await completion of a task that must run on the
         * handler's thread.  However, this problem is often a symptom of bad design.
         * Consider improving the design (if possible) before resorting to this method.
         * </p><p>
         * One example of where you might want to use this method is when you just
         * set up a Handler thread and need to perform some initialization steps on
         * it before continuing execution.
         * </p><p>
         * If timeout occurs then this method returns <code>false</code> but the runnable
         * will remain posted on the handler and may already be in progress or
         * complete at a later time.
         * </p><p>
         * When using this method, be sure to use {@link Looper#quitSafely} when
         * quitting the looper.  Otherwise {@link #runWithScissors} may hang indefinitely.
         * (TODO: We should fix this by making MessageQueue aware of blocking runnables.)
         * </p>
         *
         * @param r The Runnable that will be executed synchronously.
         * @param timeout The timeout in milliseconds, or 0 to wait indefinitely.
         *
         * @return Returns true if the Runnable was successfully executed.
         *         Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         *
         * @hide This method is prone to abuse and should probably not be in the API.
         * If we ever do make it part of the API, we might want to rename it to something
         * less funny like runUnsafe().
         */
        internal bool runWithScissors(Runnable r, long timeout)
        {
            if (r == null)
            {
                throw new IllegalArgumentException("runnable must not be null");
            }
            if (timeout < 0)
            {
                throw new IllegalArgumentException("timeout must be non-negative");
            }

            if (Looper.myLooper(mLooper.context) == mLooper)
            {
                r.Invoke();
                return true;
            }

            BlockingRunnable br = new(r);
            return br.postAndWait(this, timeout);
        }

        /**
         * Remove any pending posts of Runnable r that are in the message queue.
         */
        public void removeCallbacks(Runnable r)
        {
            mQueue.removeMessages(this, r, null);
        }

        /**
         * Remove any pending posts of Runnable <var>r</var> with Object
         * <var>token</var> that are in the message queue.  If <var>token</var> is null,
         * all callbacks will be removed.
         */
        public void removeCallbacks(Runnable r, object token)
        {
            mQueue.removeMessages(this, r, token);
        }

        /**
         * Remove any pending posts of Runnable <var>r</var> with Object
         * <var>token</var> that are in the message queue.  If <var>token</var> is null,
         * all callbacks will be removed.
         */
        public void removeCallbacks(Runnable r, int what)
        {
            mQueue.removeMessages(this, r, what);
        }

        /**
         * Pushes a message onto the end of the message queue after all pending messages
         * before the current time. It will be received in {@link #handleMessage},
         * in the thread attached to this handler.
         *  
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool sendMessage(Message msg)
        {
            return sendMessageDelayed(msg, 0);
        }

        /**
         * Sends a Message containing only the what value.
         *  
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool sendEmptyMessage(int what)
        {
            return sendEmptyMessageDelayed(what, 0);
        }

        /**
         * Sends a Message containing only the what value, to be delivered
         * after the specified amount of time elapses.
         * @see #sendMessageDelayed(android.os.Message, long) 
         * 
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool sendEmptyMessageDelayed(int what, long delayMillis)
        {
            Message msg = Message.obtain();
            msg.what = what;
            return sendMessageDelayed(msg, delayMillis);
        }

        /**
         * Sends a Message containing only the what value, to be delivered 
         * at a specific time.
         * @see #sendMessageAtTime(android.os.Message, long)
         *  
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */

        public bool sendEmptyMessageAtTime(int what, long uptimeMillis)
        {
            Message msg = Message.obtain();
            msg.what = what;
            return sendMessageAtTime(msg, uptimeMillis);
        }

        /**
         * Enqueue a message into the message queue after all pending messages
         * before (current time + delayMillis). You will receive it in
         * {@link #handleMessage}, in the thread attached to this handler.
         *  
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the message will be processed -- if
         *         the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         */
        public bool sendMessageDelayed(Message msg, long delayMillis)
        {
            if (delayMillis < 0)
            {
                delayMillis = 0;
            }
            return sendMessageAtTime(msg, NanoTime.currentTimeMillis() + delayMillis);
        }

        /**
         * Enqueue a message into the message queue after all pending messages
         * before the absolute time (in milliseconds) <var>uptimeMillis</var>.
         * <b>The time-base is {@link android.os.SystemClock#uptimeMillis}.</b>
         * Time spent in deep sleep will add an additional delay to execution.
         * You will receive it in {@link #handleMessage}, in the thread attached
         * to this handler.
         * 
         * @param uptimeMillis The absolute time at which the message should be
         *         delivered, using the
         *         {@link android.os.SystemClock#uptimeMillis} time-base.
         *         
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.  Note that a
         *         result of true does not mean the message will be processed -- if
         *         the looper is quit before the delivery time of the message
         *         occurs then the message will be dropped.
         */
        public bool sendMessageAtTime(Message msg, long uptimeMillis)
        {
            MessageQueue queue = mQueue;
            if (queue == null)
            {
                Exception e = new(this + " sendMessageAtTime() called with no mQueue");
                Log.w("Looper", e.ToString());
                return false;
            }
            return enqueueMessage(queue, msg, uptimeMillis);
        }

        /**
         * Enqueue a message at the front of the message queue, to be processed on
         * the next iteration of the message loop.  You will receive it in
         * {@link #handleMessage}, in the thread attached to this handler.
         * <b>This method is only for use in very special circumstances -- it
         * can easily starve the message queue, cause ordering problems, or have
         * other unexpected side-effects.</b>
         *  
         * @return Returns true if the message was successfully placed in to the 
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         */
        public bool sendMessageAtFrontOfQueue(Message msg)
        {
            MessageQueue queue = mQueue;
            if (queue == null)
            {
                Exception e = new(this + " sendMessageAtTime() called with no mQueue");
                Log.w("Looper", e.ToString());
                return false;
            }
            return enqueueMessage(queue, msg, 0);
        }

        /**
         * Executes the message synchronously if called on the same thread this handler corresponds to,
         * or {@link #sendMessage pushes it to the queue} otherwise
         *
         * @return Returns true if the message was successfully ran or placed in to the
         *         message queue.  Returns false on failure, usually because the
         *         looper processing the message queue is exiting.
         * @hide
         */
        internal bool executeOrSendMessage(Message msg)
        {
            if (mLooper == Looper.myLooper(mLooper.context))
            {
                dispatchMessage(msg);
                return true;
            }
            return sendMessage(msg);
        }

        private bool enqueueMessage(MessageQueue queue, Message msg,
                long uptimeMillis)
        {
            msg.target = this;
            msg.workSourceUid = mLooper.context.storage.GetOrCreate<ThreadLocalWorkSource>(StorageKeys.TLW, () => new()).Value.getUid();

            if (mAsynchronous)
            {
                msg.setAsynchronous(true);
            }
            return queue.enqueueMessage(msg, uptimeMillis);
        }

        /**
         * Remove any pending posts of messages with code 'what' that are in the
         * message queue.
         */
        public void removeMessages(int what)
        {
            mQueue.removeMessages(this, what, null);
        }

        /**
         * Remove any pending posts of messages with code 'what' and whose obj is
         * 'object' that are in the message queue.  If <var>object</var> is null,
         * all messages will be removed.
         */
        public void removeMessages(int what, object obj)
        {
            mQueue.removeMessages(this, what, obj);
        }

        /**
         * Remove any pending posts of messages with code 'what' and whose obj is
         * 'object' that are in the message queue.  If <var>object</var> is null,
         * all messages will be removed.
         *
         *@hide
         */
        internal void removeEqualMessages(int what, object obj)
        {
            mQueue.removeEqualMessages(this, what, obj);
        }

        /**
         * Remove any pending posts of callbacks and sent messages whose
         * <var>obj</var> is <var>token</var>.  If <var>token</var> is null,
         * all callbacks and messages will be removed.
         */
        public void removeCallbacksAndMessages(object token)
        {
            mQueue.removeCallbacksAndMessages(this, token);
        }

        /**
         * Remove any pending posts of callbacks and sent messages whose
         * <var>obj</var> is <var>token</var>.  If <var>token</var> is null,
         * all callbacks and messages will be removed.
         *
         *@hide
         */
        internal void removeCallbacksAndEqualMessages(object token)
        {
            mQueue.removeCallbacksAndEqualMessages(this, token);
        }
        /**
         * Check if there are any pending posts of messages with code 'what' in
         * the message queue.
         */
        public bool hasMessages(int what)
        {
            return mQueue.hasMessages(this, what, null);
        }

        /**
         * Return whether there are any messages or callbacks currently scheduled on this handler.
         * @hide
         */
        internal bool hasMessagesOrCallbacks()
        {
            return mQueue.hasMessages(this);
        }

        /**
         * Check if there are any pending posts of messages with code 'what' and
         * whose obj is 'object' in the message queue.
         */
        public bool hasMessages(int what, object obj)
        {
            return mQueue.hasMessages(this, what, obj);
        }

        /**
         * Check if there are any pending posts of messages with code 'what' and
         * whose obj is 'object' in the message queue.
         *
         *@hide
         */
        internal bool hasEqualMessages(int what, object obj)
        {
            return mQueue.hasEqualMessages(this, what, obj);
        }

        /**
         * Check if there are any pending posts of messages with callback r in
         * the message queue.
         */
        public bool hasCallbacks(Runnable r)
        {
            return mQueue.hasMessages(this, r, null);
        }

        // if we can get rid of this method, the handler need not remember its loop
        // we could instead export a getMessageQueue() method... 
        public Looper getLooper()
        {
            return mLooper;
        }

        override
        public string ToString()
        {
            return "Handler (" + GetType().Name + ") {"
            + GetHashCode().toHexString()
            + "}";
        }

        internal IMessenger getIMessenger()
        {
            lock (mQueue) {
                if (mMessenger != null)
                {
                    return mMessenger;
                }
                mMessenger = new MessengerImpl(this);
                return mMessenger;
            }
        }

        private sealed class MessengerImpl : IMessenger
        {
            Handler outer;

            public MessengerImpl(Handler outer)
            {
                this.outer = outer;
            }

            public void send(Message msg)
            {
                //msg.sendingUid = Binder.getCallingUid();
                outer.sendMessage(msg);
            }
        }

        private static Message getPostMessage(Runnable r)
        {
            Message m = Message.obtain();
            m.callback = r;
            return m;
        }

        private static Message getPostMessage(Runnable r, object token)
        {
            Message m = Message.obtain();
            m.obj = token;
            m.callback = r;
            return m;
        }

        private static Message getPostMessage(Runnable r, int what)
        {
            Message m = Message.obtain();
            m.what = what;
            m.callback = r;
            return m;
        }

        private static void handleCallback(Message message)
        {
            message.callback.Invoke();
        }

        Looper mLooper;
        MessageQueue mQueue;
        Callback mCallback;
        bool mAsynchronous;
        IMessenger mMessenger;

        private class BlockingRunnable
        {
            private Runnable mTask;
            private bool mDone;
            private readonly object LOCK = new();
            Runnable runnable;

            public BlockingRunnable(Runnable task)
            {
                mTask = task;
                runnable = () =>
                {
                    try
                    {
                        mTask.Invoke();
                    }
                    finally
                    {
                        lock (LOCK)
                        {
                            mDone = true;
                            LOCK.NotifyAll();
                        }
                    }
                };
            }

            public bool postAndWait(Handler handler, long timeout)
            {
                if (!handler.post(runnable))
                {
                    return false;
                }

                lock (LOCK) {
                    if (timeout > 0)
                    {
                        long expirationTime = NanoTime.currentTimeMillis() + timeout;
                        while (!mDone)
                        {
                            int delay = (int)(expirationTime - NanoTime.currentTimeMillis());
                            if (delay <= 0)
                            {
                                return false; // timeout
                            }
                            try
                            {
                                LOCK.Wait(delay);
                            }
                            catch (ThreadInterruptedException ex)
                            {
                            }
                        }
                    }
                    else
                    {
                        while (!mDone)
                        {
                            try
                            {
                                LOCK.Wait();
                            }
                            catch (ThreadInterruptedException ex)
                            {
                            }
                        }
                    }
                }
                return true;
            }
        }
    }
}
