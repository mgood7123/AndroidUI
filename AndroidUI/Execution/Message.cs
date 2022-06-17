﻿/*
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
using System.Text;

namespace AndroidUI.Execution
{

    /**
     *
     * Defines a message containing a description and arbitrary data object that can be
     * sent to a {@link Handler}.  This object contains two extra int fields and an
     * extra object field that allow you to not do allocations in many cases.
     *
     * <p class="note">While the constructor of Message is public, the best way to get
     * one of these is to call {@link #obtain Message.obtain()} or one of the
     * {@link Handler#obtainMessage Handler.obtainMessage()} methods, which will pull
     * them from a pool of recycled objects.</p>
     */
    public sealed class Message
    {
        /**
         * User-defined message code so that the recipient can identify
         * what this message is about. Each {@link Handler} has its own name-space
         * for message codes, so you do not need to worry about yours conflicting
         * with other handlers.
         */
        public int what;

        /**
         * arg1 and arg2 are lower-cost alternatives to using
         * {@link #setData(Bundle) setData()} if you only need to store a
         * few integer values.
         */
        public int arg1;

        /**
         * arg1 and arg2 are lower-cost alternatives to using
         * {@link #setData(Bundle) setData()} if you only need to store a
         * few integer values.
         */
        public int arg2;

        /**
         * An arbitrary object to send to the recipient.  When using
         * {@link Messenger} to send the message across processes this can only
         * be non-null if it contains a Parcelable of a framework class (not one
         * implemented by the application).   For other data transfer use
         * {@link #setData}.
         *
         * <p>Note that Parcelable objects here are not supported prior to
         * the {@link android.os.Build.VERSION_CODES#FROYO} release.
         */
        public Object obj;

        /**
         * Optional Messenger where replies to this message can be sent.  The
         * semantics of exactly how this is used are up to the sender and
         * receiver.
         */
        public Messenger replyTo;

        /**
         * Indicates that the uid is not set;
         *
         * @hide Only for use within the system server.
         */
        internal const int UID_NONE = -1;

        /**
         * Optional field indicating the uid that sent the message.  This is
         * only valid for messages posted by a {@link Messenger}; otherwise,
         * it will be -1.
         */
        public int sendingUid = UID_NONE;

        /**
         * Optional field indicating the uid that caused this message to be enqueued.
         *
         * @hide Only for use within the system server.
         */
        public int workSourceUid = UID_NONE;

        /** If set message is in use.
         * This flag is set when the message is enqueued and remains set while it
         * is delivered and afterwards when it is recycled.  The flag is only cleared
         * when a new message is created or obtained since that is the only time that
         * applications are allowed to modify the contents of the message.
         *
         * It is an error to attempt to enqueue or recycle a message that is already in use.
         */
        /*package*/
        const int FLAG_IN_USE = 1 << 0;

        /** If set message is asynchronous */
        /*package*/
        const int FLAG_ASYNCHRONOUS = 1 << 1;

        /** Flags to clear in the copyFrom method */
        /*package*/
        const int FLAGS_TO_CLEAR_ON_COPY_FROM = FLAG_IN_USE;

        internal int flags;

        /**
         * The targeted delivery time of this message. The time-base is
         * {@link SystemClock#uptimeMillis}.
         * @hide Only for use within the tests.
         */
        public long when;

        //Bundle data;

        /*package*/
        internal Handler target;

        /*package*/
        internal Runnable callback;

        // sometimes we store linked lists of these things
        /*package*/
        internal Message next;


        /** @hide */
        internal static readonly object sPoolSync = new Object();
        private static Message sPool;
        private static int sPoolSize = 0;

        private const int MAX_POOL_SIZE = 50;

        private static bool gCheckRecycle = true;

        internal bool waiting = false;

        /**
         * Return a new Message instance from the global pool. Allows us to
         * avoid allocating new objects in many cases.
         */
        public static Message obtain()
        {
            lock (sPoolSync)
            {
                if (sPool != null)
                {
                    Message m = sPool;
                    sPool = m.next;
                    m.next = null;
                    m.flags = 0; // clear in-use flag
                    sPoolSize--;
                    return m;
                }
            }
            return new Message();
        }

        /**
         * Same as {@link #obtain()}, but copies the values of an existing
         * message (including its target) into the new one.
         * @param orig Original message to copy.
         * @return A Message object from the global pool.
         */
        public static Message obtain(Message orig)
        {
            Message m = obtain();
            m.what = orig.what;
            m.arg1 = orig.arg1;
            m.arg2 = orig.arg2;
            m.obj = orig.obj;
            m.replyTo = orig.replyTo;
            m.sendingUid = orig.sendingUid;
            m.workSourceUid = orig.workSourceUid;
            //if (orig.data != null)
            //{
            //    m.data = new Bundle(orig.data);
            //}
            m.target = orig.target;
            m.callback = orig.callback;
            m.waiting = orig.waiting;

            return m;
        }

        /**
         * Same as {@link #obtain()}, but sets the value for the <em>target</em> member on the Message returned.
         * @param h  Handler to assign to the returned Message object's <em>target</em> member.
         * @return A Message object from the global pool.
         */
        public static Message obtain(Handler h)
        {
            Message m = obtain();
            m.target = h;

            return m;
        }

        /**
         * Same as {@link #obtain(Handler)}, but assigns a callback Runnable on
         * the Message that is returned.
         * @param h  Handler to assign to the returned Message object's <em>target</em> member.
         * @param callback Runnable that will execute when the message is handled.
         * @return A Message object from the global pool.
         */
        public static Message obtain(Handler h, Runnable callback)
        {
            Message m = obtain();
            m.target = h;
            m.callback = callback;

            return m;
        }

        /**
         * Same as {@link #obtain()}, but sets the values for both <em>target</em> and
         * <em>what</em> members on the Message.
         * @param h  Value to assign to the <em>target</em> member.
         * @param what  Value to assign to the <em>what</em> member.
         * @return A Message object from the global pool.
         */
        public static Message obtain(Handler h, int what)
        {
            Message m = obtain();
            m.target = h;
            m.what = what;

            return m;
        }

        /**
         * Same as {@link #obtain()}, but sets the values of the <em>target</em>, <em>what</em>, and <em>obj</em>
         * members.
         * @param h  The <em>target</em> value to set.
         * @param what  The <em>what</em> value to set.
         * @param obj  The <em>object</em> method to set.
         * @return  A Message object from the global pool.
         */
        public static Message obtain(Handler h, int what, Object obj)
        {
            Message m = obtain();
            m.target = h;
            m.what = what;
            m.obj = obj;

            return m;
        }

        /**
         * Same as {@link #obtain()}, but sets the values of the <em>target</em>, <em>what</em>,
         * <em>arg1</em>, and <em>arg2</em> members.
         *
         * @param h  The <em>target</em> value to set.
         * @param what  The <em>what</em> value to set.
         * @param arg1  The <em>arg1</em> value to set.
         * @param arg2  The <em>arg2</em> value to set.
         * @return  A Message object from the global pool.
         */
        public static Message obtain(Handler h, int what, int arg1, int arg2)
        {
            Message m = obtain();
            m.target = h;
            m.what = what;
            m.arg1 = arg1;
            m.arg2 = arg2;

            return m;
        }

        /**
         * Same as {@link #obtain()}, but sets the values of the <em>target</em>, <em>what</em>,
         * <em>arg1</em>, <em>arg2</em>, and <em>obj</em> members.
         *
         * @param h  The <em>target</em> value to set.
         * @param what  The <em>what</em> value to set.
         * @param arg1  The <em>arg1</em> value to set.
         * @param arg2  The <em>arg2</em> value to set.
         * @param obj  The <em>obj</em> value to set.
         * @return  A Message object from the global pool.
         */
        public static Message obtain(Handler h, int what,
                int arg1, int arg2, Object obj)
        {
            Message m = obtain();
            m.target = h;
            m.what = what;
            m.arg1 = arg1;
            m.arg2 = arg2;
            m.obj = obj;

            return m;
        }

        /**
         * Return a Message instance to the global pool.
         * <p>
         * You MUST NOT touch the Message after calling this function because it has
         * effectively been freed.  It is an error to recycle a message that is currently
         * enqueued or that is in the process of being delivered to a Handler.
         * </p>
         */
        public void recycle()
        {
            if (isInUse())
            {
                if (gCheckRecycle)
                {
                    throw new IllegalStateException("This message cannot be recycled because it "
                            + "is still in use.");
                }
                return;
            }
            recycleUnchecked();
        }

        /**
         * Recycles a Message that may be in-use.
         * Used internally by the MessageQueue and Looper when disposing of queued Messages.
         */
        internal void recycleUnchecked()
        {
            // Mark the message as in use while it remains in the recycled object pool.
            // Clear out all other details.
            flags = FLAG_IN_USE;
            what = 0;
            arg1 = 0;
            arg2 = 0;
            obj = null;
            replyTo = null;
            sendingUid = UID_NONE;
            workSourceUid = UID_NONE;
            when = 0;
            target = null;
            callback = null;
            //data = null;
            waiting = false;

            lock (sPoolSync)
            {
                if (sPoolSize < MAX_POOL_SIZE)
                {
                    next = sPool;
                    sPool = this;
                    sPoolSize++;
                }
            }
        }

        /**
         * Make this message like o.  Performs a shallow copy of the data field.
         * Does not copy the linked list fields, nor the timestamp or
         * target/callback of the original message.
         */
        public void copyFrom(Message o)
        {
            this.flags = o.flags & ~FLAGS_TO_CLEAR_ON_COPY_FROM;
            this.what = o.what;
            this.arg1 = o.arg1;
            this.arg2 = o.arg2;
            this.obj = o.obj;
            this.replyTo = o.replyTo;
            this.sendingUid = o.sendingUid;
            this.workSourceUid = o.workSourceUid;
            this.waiting = o.waiting;

            //if (o.data != null)
            //{
            //    this.data = (Bundle)o.data.clone();
            //}
            //else
            //{
            //    this.data = null;
            //}
        }

        /**
         * Return the targeted delivery time of this message, in milliseconds.
         */
        public long getWhen()
        {
            return when;
        }

        public void setTarget(Handler target)
        {
            this.target = target;
        }

        /**
         * Retrieve the {@link android.os.Handler Handler} implementation that
         * will receive this message. The object must implement
         * {@link android.os.Handler#handleMessage(android.os.Message)
         * Handler.handleMessage()}. Each Handler has its own name-space for
         * message codes, so you do not need to
         * worry about yours conflicting with other handlers.
         */
        public Handler getTarget()
        {
            return target;
        }

        /**
         * Retrieve callback object that will execute when this message is handled.
         * This object must implement Runnable. This is called by
         * the <em>target</em> {@link Handler} that is receiving this Message to
         * dispatch it.  If
         * not set, the message will be dispatched to the receiving Handler's
         * {@link Handler#handleMessage(Message)}.
         */
        public Runnable getCallback()
        {
            return callback;
        }

        /** @hide */
        internal Message setCallback(Runnable r)
        {
            callback = r;
            return this;
        }

        /**
         * Obtains a Bundle of arbitrary data associated with this
         * event, lazily creating it if necessary. Set this value by calling
         * {@link #setData(Bundle)}.  Note that when transferring data across
         * processes via {@link Messenger}, you will need to set your ClassLoader
         * on the Bundle via {@link Bundle#setClassLoader(ClassLoader)
         * Bundle.setClassLoader()} so that it can instantiate your objects when
         * you retrieve them.
         * @see #peekData()
         * @see #setData(Bundle)
         */
        //public Bundle getData()
        //{
        //    if (data == null)
        //    {
        //        data = new Bundle();
        //    }

        //    return data;
        //}

        /**
         * Like getData(), but does not lazily create the Bundle.  A null
         * is returned if the Bundle does not already exist.  See
         * {@link #getData} for further information on this.
         * @see #getData()
         * @see #setData(Bundle)
         */
        //public Bundle peekData()
        //{
        //    return data;
        //}

        /**
         * Sets a Bundle of arbitrary data values. Use arg1 and arg2 members
         * as a lower cost way to send a few simple integer values, if you can.
         * @see #getData()
         * @see #peekData()
         */
        //public void setData(Bundle data)
        //{
        //    this.data = data;
        //}

        /**
         * Chainable setter for {@link #what}
         *
         * @hide
         */
        internal Message setWhat(int what)
        {
            this.what = what;
            return this;
        }

        /**
         * Sends this Message to the Handler specified by {@link #getTarget}.
         * Throws a null pointer exception if this field has not been set.
         */
        public void sendToTarget()
        {
            target.sendMessage(this);
        }

        /**
         * Returns true if the message is asynchronous, meaning that it is not
         * subject to {@link Looper} synchronization barriers.
         *
         * @return True if the message is asynchronous.
         *
         * @see #setAsynchronous(bool)
         */
        public bool isAsynchronous()
        {
            return (flags & FLAG_ASYNCHRONOUS) != 0;
        }

        /**
         * Sets whether the message is asynchronous, meaning that it is not
         * subject to {@link Looper} synchronization barriers.
         * <p>
         * Certain operations, such as view invalidation, may introduce synchronization
         * barriers into the {@link Looper}'s message queue to prevent subsequent messages
         * from being delivered until some condition is met.  In the case of view invalidation,
         * messages which are posted after a call to {@link android.view.View#invalidate}
         * are suspended by means of a synchronization barrier until the next frame is
         * ready to be drawn.  The synchronization barrier ensures that the invalidation
         * request is completely handled before resuming.
         * </p><p>
         * Asynchronous messages are exempt from synchronization barriers.  They typically
         * represent interrupts, input events, and other signals that must be handled independently
         * even while other work has been suspended.
         * </p><p>
         * Note that asynchronous messages may be delivered out of order with respect to
         * synchronous messages although they are always delivered in order among themselves.
         * If the relative order of these messages matters then they probably should not be
         * asynchronous in the first place.  Use with caution.
         * </p>
         *
         * @param async True if the message is asynchronous.
         *
         * @see #isAsynchronous()
         */
        public void setAsynchronous(bool async)
        {
            if (async)
            {
                flags |= FLAG_ASYNCHRONOUS;
            }
            else
            {
                flags &= ~FLAG_ASYNCHRONOUS;
            }
        }

        /*package*/
        internal bool isInUse()
        {
            return ((flags & FLAG_IN_USE) == FLAG_IN_USE);
        }

        /*package*/
        internal void markInUse()
        {
            flags |= FLAG_IN_USE;
        }

        /** Constructor (but the preferred way to get a Message is to call {@link #obtain() Message.obtain()}).
        */
        public Message()
        {
        }

        override public string ToString()
        {
            return toString(NanoTime.currentTimeMillis());
        }

        string toString(long now)
        {
            StringBuilder b = new StringBuilder();
            b.Append("{ when=");
            TimeUtils.formatDuration(when - now, b);

            b.Append(" waiting=");
            b.Append(waiting);

            if (target != null)
            {
                if (callback != null)
                {
                    b.Append(" callback=");
                    b.Append(callback.GetType().Name);
                }
                else
                {
                    b.Append(" what=");
                    b.Append(what);
                }

                if (arg1 != 0)
                {
                    b.Append(" arg1=");
                    b.Append(arg1);
                }

                if (arg2 != 0)
                {
                    b.Append(" arg2=");
                    b.Append(arg2);
                }

                if (obj != null)
                {
                    b.Append(" obj=");
                    b.Append(obj);
                }

                b.Append(" target=");
                b.Append(target.GetType().Name);
            }
            else
            {
                b.Append(" barrier=");
                b.Append(arg1);
            }

            b.Append(" }");
            return b.ToString();
        }
    }
}