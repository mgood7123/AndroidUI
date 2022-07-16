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
using AndroidUI.OS;
using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;

namespace AndroidUI.Execution
{
    /**
     * Low-level class holding the list of messages to be dispatched by a
     * {@link Looper}.  Messages are not added directly to a MessageQueue,
     * but rather through {@link Handler} objects associated with the Looper.
     *
     * <p>You can retrieve the MessageQueue for the current thread with
     * {@link Looper#myQueue() Looper.myQueue()}.
     */
    public sealed class MessageQueue
    {
        private readonly object LOCK = new();
        private const string TAG = "MessageQueue";
        private const bool DEBUG = false;

        // True if the message queue can be quit.
        private readonly bool mQuitAllowed;

        private IntPtr mPtr; // used by native code

        Message mMessages;
        private List<IdleHandler> mIdleHandlers = new();
        private SparseArray<FileDescriptorRecord> mFileDescriptorRecords;
        private IdleHandler[] mPendingIdleHandlers;
        private bool mQuitting;

        // Indicates whether next() is blocked waiting in pollOnce() with a non-zero timeout.
        private bool mBlocked;

        // The next barrier token.
        // Barriers are indicated by messages with a null target whose arg1 field carries the token.
        private int mNextBarrierToken;

        private static IntPtr nativeInit()
        {
            //var s = new HPSocket.Udp.UdpServer();

            //var sock = new System.Net.Sockets.Socket(
            //    System.Net.Sockets.AddressFamily.Unix,
            //    System.Net.Sockets.SocketType.Dgram,
            //    System.Net.Sockets.ProtocolType.Udp
            //);
            //return sock;
            return new IntPtr(1);
        }
        private static void nativeDestroy(IntPtr ptr)
        {
        }

        /*non-static for callbacks*/
        private void nativePollOnce(IntPtr ptr, int timeoutMillis)
        {
            //throw new NotImplementedException();
        }
        private static void nativeWake(IntPtr ptr)
        {
            //throw new NotImplementedException();
        }
        private static bool nativeIsPolling(IntPtr ptr)
        {
            return false;
            //throw new NotImplementedException();
        }
        private static void nativeSetFileDescriptorEvents(IntPtr ptr, int fd, int events)
        {
            //throw new NotImplementedException();
        }

        internal MessageQueue(bool quitAllowed)
        {
            mQuitAllowed = quitAllowed;
            mPtr = nativeInit();
        }

        ~MessageQueue()
        {
            dispose();
        }

        // Disposes of the underlying message queue.
        // Must only be called on the looper thread or the finalizer.
        private void dispose()
        {
            if (mPtr != null)
            {
                nativeDestroy(mPtr);
                mPtr = IntPtr.Zero;
            }
        }

        /**
         * Returns true if the looper has no pending messages which are due to be processed.
         *
         * <p>This method is safe to call from any thread.
         *
         * @return True if the looper is idle.
         */
        public bool isIdle()
        {
            lock (LOCK) {
                long now = NanoTime.currentTimeMillis();
                return mMessages == null || now < mMessages.when;
            }
        }

        /**
         * Add a new {@link IdleHandler} to this message queue.  This may be
         * removed automatically for you by returning false from
         * {@link IdleHandler#queueIdle IdleHandler.queueIdle()} when it is
         * invoked, or explicitly removing it with {@link #removeIdleHandler}.
         *
         * <p>This method is safe to call from any thread.
         *
         * @param handler The IdleHandler to be added.
         */
        public void addIdleHandler(IdleHandler handler)
        {
            if (handler == null)
            {
                throw new NullReferenceException("Can't add a null IdleHandler");
            }
            lock (LOCK) {
                mIdleHandlers.Add(handler);
            }
        }

        /**
         * Remove an {@link IdleHandler} from the queue that was previously added
         * with {@link #addIdleHandler}.  If the given object is not currently
         * in the idle list, nothing is done.
         *
         * <p>This method is safe to call from any thread.
         *
         * @param handler The IdleHandler to be removed.
         */
        public void removeIdleHandler(IdleHandler handler)
        {
            lock (LOCK) {
                mIdleHandlers.Remove(handler);
            }
        }

        /**
         * Returns whether this looper's thread is currently polling for more work to do.
         * This is a good signal that the loop is still alive rather than being stuck
         * handling a callback.  Note that this method is intrinsically racy, since the
         * state of the loop can change before you get the result back.
         *
         * <p>This method is safe to call from any thread.
         *
         * @return True if the looper is currently polling for events.
         * @hide
         */
        internal bool isPolling()
        {
            lock (LOCK) {
                return isPollingLocked();
            }
        }

        private bool isPollingLocked()
        {
            // If the loop is quitting then it must not be idling.
            // We can assume mPtr != 0 when mQuitting is false.
            return !mQuitting && nativeIsPolling(mPtr);
        }

        /**
         * Adds a file descriptor listener to receive notification when file descriptor
         * related events occur.
         * <p>
         * If the file descriptor has already been registered, the specified events
         * and listener will replace any that were previously associated with it.
         * It is not possible to set more than one listener per file descriptor.
         * </p><p>
         * It is important to always unregister the listener when the file descriptor
         * is no longer of use.
         * </p>
         *
         * @param fd The file descriptor for which a listener will be registered.
         * @param events The set of events to receive: a combination of the
         * {@link OnFileDescriptorEventListener#EVENT_INPUT},
         * {@link OnFileDescriptorEventListener#EVENT_OUTPUT}, and
         * {@link OnFileDescriptorEventListener#EVENT_ERROR} event masks.  If the requested
         * set of events is zero, then the listener is unregistered.
         * @param listener The listener to invoke when file descriptor events occur.
         *
         * @see OnFileDescriptorEventListener
         * @see #removeOnFileDescriptorEventListener
         */
        public void addOnFileDescriptorEventListener(FileDescriptor fd,
                int events,
                OnFileDescriptorEventListener listener)
        {
            if (fd == null)
            {
                throw new IllegalArgumentException("fd must not be null");
            }
            if (listener == null)
            {
                throw new IllegalArgumentException("listener must not be null");
            }

            lock (LOCK) {
                updateOnFileDescriptorEventListenerLocked(fd, events, listener);
            }
        }

        /**
         * Removes a file descriptor listener.
         * <p>
         * This method does nothing if no listener has been registered for the
         * specified file descriptor.
         * </p>
         *
         * @param fd The file descriptor whose listener will be unregistered.
         *
         * @see OnFileDescriptorEventListener
         * @see #addOnFileDescriptorEventListener
         */
        public void removeOnFileDescriptorEventListener(FileDescriptor fd)
        {
            if (fd == null)
            {
                throw new IllegalArgumentException("fd must not be null");
            }

            lock (LOCK) {
                updateOnFileDescriptorEventListenerLocked(fd, 0, null);
            }
        }

        private void updateOnFileDescriptorEventListenerLocked(FileDescriptor fd, int events,
                OnFileDescriptorEventListener listener)
        {
            int fdNum = fd.getInt();

            int index = -1;
            FileDescriptorRecord record = null;
            if (mFileDescriptorRecords != null)
            {
                index = mFileDescriptorRecords.indexOfKey(fdNum);
                if (index >= 0)
                {
                    record = mFileDescriptorRecords.valueAt(index);
                    if (record != null && record.mEvents == events)
                    {
                        return;
                    }
                }
            }

            if (events != 0)
            {
                events |= OnFileDescriptorEventListener.EVENT_ERROR;
                if (record == null)
                {
                    if (mFileDescriptorRecords == null)
                    {
                        mFileDescriptorRecords = new SparseArray<FileDescriptorRecord>();
                    }
                    record = new FileDescriptorRecord(fd, events, listener);
                    mFileDescriptorRecords.put(fdNum, record);
                }
                else
                {
                    record.mListener = listener;
                    record.mEvents = events;
                    record.mSeq += 1;
                }
                nativeSetFileDescriptorEvents(mPtr, fdNum, events);
            }
            else if (record != null)
            {
                record.mEvents = 0;
                mFileDescriptorRecords.removeAt(index);
                nativeSetFileDescriptorEvents(mPtr, fdNum, 0);
            }
        }

        // Called from native code.
        private int dispatchEvents(int fd, int events)
        {
            // Get the file descriptor record and any state that might change.
            FileDescriptorRecord record;
            int oldWatchedEvents;
            OnFileDescriptorEventListener listener;
            int seq;
            lock (LOCK) {
                record = mFileDescriptorRecords.get(fd);
                if (record == null)
                {
                    return 0; // spurious, no listener registered
                }

                oldWatchedEvents = record.mEvents;
                events &= oldWatchedEvents; // filter events based on current watched set
                if (events == 0)
                {
                    return oldWatchedEvents; // spurious, watched events changed
                }

                listener = record.mListener;
                seq = record.mSeq;
            }

            // Invoke the listener outside of the lock.
            int newWatchedEvents = listener.onFileDescriptorEvents(
                    record.mDescriptor, events);
            if (newWatchedEvents != 0)
            {
                newWatchedEvents |= OnFileDescriptorEventListener.EVENT_ERROR;
            }

            // Update the file descriptor record if the listener changed the set of
            // events to watch and the listener itself hasn't been updated since.
            if (newWatchedEvents != oldWatchedEvents)
            {
                lock (LOCK) {
                    int index = mFileDescriptorRecords.indexOfKey(fd);
                    if (index >= 0 && mFileDescriptorRecords.valueAt(index) == record
                            && record.mSeq == seq)
                    {
                        record.mEvents = newWatchedEvents;
                        if (newWatchedEvents == 0)
                        {
                            mFileDescriptorRecords.removeAt(index);
                        }
                    }
                }
            }

            // Return the new set of events to watch for native code to take care of.
            return newWatchedEvents;
        }

        internal Message nextUI()
        {
            // Return here if the message loop has already quit and been disposed.
            // This can happen if the application tries to restart a looper after quit
            // which is not supported.
            IntPtr ptr = mPtr;
            if (ptr == null)
            {
                return null;
            }

            int pendingIdleHandlerCount = -1; // -1 only during first iteration
            int nextPollTimeoutMillis = 0;
            for (; ; )
            {
                if (nextPollTimeoutMillis != 0)
                {
                    //Binder.flushPendingCommands();
                }

                nativePollOnce(ptr, nextPollTimeoutMillis);

                lock (LOCK)
                {
                    // Try to retrieve the next message.  Return if found.
                    long now = NanoTime.currentTimeMillis();
                    Message prevMsg = null;
                    Message msg = mMessages;
                    if (msg != null && msg.target == null)
                    {
                        // Stalled by a barrier.  Find the next asynchronous message in the queue.
                        do
                        {
                            prevMsg = msg;
                            msg = msg.next;
                        } while (msg != null && !msg.isAsynchronous());
                    }
                    if (msg != null)
                    {
                        if (now < msg.when)
                        {
                            // Next message is not ready.  Set a timeout to wake up when it is ready.
                            nextPollTimeoutMillis = (int)Math.Min(msg.when - now, int.MaxValue);
                            msg.waiting = true;
                        }
                        else
                        {
                            // Got a message.
                            mBlocked = false;
                            if (prevMsg != null)
                            {
                                prevMsg.next = msg.next;
                            }
                            else
                            {
                                mMessages = msg.next;
                            }
                            msg.next = null;
                            if (DEBUG) Log.v(TAG, "Returning message: " + msg);
                            msg.markInUse();
                            msg.waiting = false;
                            return msg;
                        }
                    }
                    else
                    {
                        // No more messages.
                        nextPollTimeoutMillis = -1;
                    }

                    // Process the quit message now that all pending messages have been handled.
                    if (mQuitting)
                    {
                        dispose();
                        return null;
                    }

                    // If first time idle, then get the number of idlers to run.
                    // Idle handles only run if the queue is empty or if the first message
                    // in the queue (possibly a barrier) is due to be handled in the future.
                    if (pendingIdleHandlerCount < 0
                            && (mMessages == null || now < mMessages.when))
                    {
                        pendingIdleHandlerCount = mIdleHandlers.Count;
                    }
                    if (pendingIdleHandlerCount <= 0)
                    {
                        // No idle handlers to run.  Loop and wait some more.
                        if (msg == null)
                        {
                            return null;
                        }

                        if (msg.waiting)
                        {
                            return msg;
                        }

                        mBlocked = true;
                        continue;
                    }

                    if (mPendingIdleHandlers == null)
                    {
                        mPendingIdleHandlers = new IdleHandler[Math.Max(pendingIdleHandlerCount, 4)];
                    }
                    mPendingIdleHandlers = mIdleHandlers.ToArray();
                }

                // Run the idle handlers.
                // We only ever reach this code block during the first iteration.
                for (int i = 0; i < pendingIdleHandlerCount; i++)
                {
                    IdleHandler idler = mPendingIdleHandlers[i];
                    mPendingIdleHandlers[i] = null; // release the reference to the handler

                    bool keep = false;
                    try
                    {
                        keep = idler.queueIdle();
                    }
                    catch (Exception t)
                    {
                        Log.e(TAG, "IdleHandler threw exception: " + t);
                    }

                    if (!keep)
                    {
                        lock (LOCK)
                        {
                            mIdleHandlers.Remove(idler);
                        }
                    }
                }

                // Reset the idle handler count to 0 so we do not run them again.
                pendingIdleHandlerCount = 0;

                // While calling an idle handler, a new message could have been delivered
                // so go back and look again for a pending message without waiting.
                nextPollTimeoutMillis = 0;
            }
        }

        internal Message next()
        {
            // Return here if the message loop has already quit and been disposed.
            // This can happen if the application tries to restart a looper after quit
            // which is not supported.
            IntPtr ptr = mPtr;
            if (ptr == null)
            {
                return null;
            }

            int pendingIdleHandlerCount = -1; // -1 only during first iteration
            int nextPollTimeoutMillis = 0;
            for (; ; )
            {
                if (nextPollTimeoutMillis != 0)
                {
                    //Binder.flushPendingCommands();
                }

                nativePollOnce(ptr, nextPollTimeoutMillis);

                lock (LOCK) {
                    // Try to retrieve the next message.  Return if found.
                    long now = NanoTime.currentTimeMillis();
                    Message prevMsg = null;
                    Message msg = mMessages;
                    if (msg != null && msg.target == null)
                    {
                        // Stalled by a barrier.  Find the next asynchronous message in the queue.
                        do
                        {
                            prevMsg = msg;
                            msg = msg.next;
                        } while (msg != null && !msg.isAsynchronous());
                    }
                    if (msg != null)
                    {
                        if (now < msg.when)
                        {
                            // Next message is not ready.  Set a timeout to wake up when it is ready.
                            nextPollTimeoutMillis = (int)Math.Min(msg.when - now, int.MaxValue);
                        }
                        else
                        {
                            // Got a message.
                            mBlocked = false;
                            if (prevMsg != null)
                            {
                                prevMsg.next = msg.next;
                            }
                            else
                            {
                                mMessages = msg.next;
                            }
                            msg.next = null;
                            if (DEBUG) Log.v(TAG, "Returning message: " + msg);
                            msg.markInUse();
                            return msg;
                        }
                    }
                    else
                    {
                        // No more messages.
                        nextPollTimeoutMillis = -1;
                    }

                    // Process the quit message now that all pending messages have been handled.
                    if (mQuitting)
                    {
                        dispose();
                        return null;
                    }

                    // If first time idle, then get the number of idlers to run.
                    // Idle handles only run if the queue is empty or if the first message
                    // in the queue (possibly a barrier) is due to be handled in the future.
                    if (pendingIdleHandlerCount < 0
                            && (mMessages == null || now < mMessages.when))
                    {
                        pendingIdleHandlerCount = mIdleHandlers.Count;
                    }
                    if (pendingIdleHandlerCount <= 0)
                    {
                        // No idle handlers to run.  Loop and wait some more.
                        mBlocked = true;
                        continue;
                    }

                    if (mPendingIdleHandlers == null)
                    {
                        mPendingIdleHandlers = new IdleHandler[Math.Max(pendingIdleHandlerCount, 4)];
                    }
                    mPendingIdleHandlers = mIdleHandlers.ToArray();
                }

                // Run the idle handlers.
                // We only ever reach this code block during the first iteration.
                for (int i = 0; i < pendingIdleHandlerCount; i++)
                {
                    IdleHandler idler = mPendingIdleHandlers[i];
                    mPendingIdleHandlers[i] = null; // release the reference to the handler

                    bool keep = false;
                    try
                    {
                        keep = idler.queueIdle();
                    }
                    catch (Exception t)
                    {
                        Log.e(TAG, "IdleHandler threw exception: " + t);
                    }

                    if (!keep)
                    {
                        lock (LOCK) {
                            mIdleHandlers.Remove(idler);
                        }
                    }
                }

                // Reset the idle handler count to 0 so we do not run them again.
                pendingIdleHandlerCount = 0;

                // While calling an idle handler, a new message could have been delivered
                // so go back and look again for a pending message without waiting.
                nextPollTimeoutMillis = 0;
            }
        }

        internal void quit(bool safe)
        {
            if (!mQuitAllowed)
            {
                throw new IllegalStateException("Main thread not allowed to quit.");
            }

            lock (LOCK) {
                if (mQuitting)
                {
                    return;
                }
                mQuitting = true;

                if (safe)
                {
                    removeAllFutureMessagesLocked();
                }
                else
                {
                    removeAllMessagesLocked();
                }

                // We can assume mPtr != 0 because mQuitting was previously false.
                nativeWake(mPtr);
            }
        }

        /**
         * Posts a synchronization barrier to the Looper's message queue.
         *
         * Message processing occurs as usual until the message queue encounters the
         * synchronization barrier that has been posted.  When the barrier is encountered,
         * later synchronous messages in the queue are stalled (prevented from being executed)
         * until the barrier is released by calling {@link #removeSyncBarrier} and specifying
         * the token that identifies the synchronization barrier.
         *
         * This method is used to immediately postpone execution of all subsequently posted
         * synchronous messages until a condition is met that releases the barrier.
         * Asynchronous messages (see {@link Message#isAsynchronous} are exempt from the barrier
         * and continue to be processed as usual.
         *
         * This call must be always matched by a call to {@link #removeSyncBarrier} with
         * the same token to ensure that the message queue resumes normal operation.
         * Otherwise the application will probably hang!
         *
         * @return A token that uniquely identifies the barrier.  This token must be
         * passed to {@link #removeSyncBarrier} to release the barrier.
         *
         * @hide
         */
        internal int postSyncBarrier()
        {
            return postSyncBarrier(NanoTime.currentTimeMillis());
        }

        private int postSyncBarrier(long when)
        {
            // Enqueue a new sync barrier token.
            // We don't need to wake the queue because the purpose of a barrier is to stall it.
            lock (LOCK) {
                int token = mNextBarrierToken++;
                Message msg = Message.obtain();
                msg.markInUse();
                msg.when = when;
                msg.arg1 = token;

                Message prev = null;
                Message p = mMessages;
                if (when != 0)
                {
                    while (p != null && p.when <= when)
                    {
                        prev = p;
                        p = p.next;
                    }
                }
                if (prev != null)
                { // invariant: p == prev.next
                    msg.next = p;
                    prev.next = msg;
                }
                else
                {
                    msg.next = p;
                    mMessages = msg;
                }
                return token;
            }
        }

        /**
         * Removes a synchronization barrier.
         *
         * @param token The synchronization barrier token that was returned by
         * {@link #postSyncBarrier}.
         *
         * @throws IllegalStateException if the barrier was not found.
         *
         * @hide
         */
        internal void removeSyncBarrier(int token)
        {
            // Remove a sync barrier token from the queue.
            // If the queue is no longer stalled by a barrier then wake it.
            lock (LOCK) {
                Message prev = null;
                Message p = mMessages;
                while (p != null && (p.target != null || p.arg1 != token))
                {
                    prev = p;
                    p = p.next;
                }
                if (p == null)
                {
                    throw new IllegalStateException("The specified message queue synchronization "
                            + " barrier token has not been posted or has already been removed.");
                }
                bool needWake;
                if (prev != null)
                {
                    prev.next = p.next;
                    needWake = false;
                }
                else
                {
                    mMessages = p.next;
                    needWake = mMessages == null || mMessages.target != null;
                }
                p.recycleUnchecked();

                // If the loop is quitting then it is already awake.
                // We can assume mPtr != 0 when mQuitting is false.
                if (needWake && !mQuitting)
                {
                    nativeWake(mPtr);
                }
            }
        }

        internal bool enqueueMessage(Message msg, long when)
        {
            if (msg.target == null)
            {
                throw new IllegalArgumentException("Message must have a target.");
            }

            lock (LOCK) {
                if (msg.isInUse())
                {
                    throw new IllegalStateException(msg + " This message is already in use.");
                }

                if (mQuitting)
                {
                    IllegalStateException e = new(
                            msg.target + " sending message to a Handler on a dead thread");
                    Log.w(TAG, e.ToString());
                    msg.recycle();
                    return false;
                }

                msg.markInUse();
                msg.when = when;
                Message p = mMessages;
                bool needWake;
                if (p == null || when == 0 || when < p.when)
                {
                    // New head, wake up the event queue if blocked.
                    msg.next = p;
                    mMessages = msg;
                    needWake = mBlocked;
                }
                else
                {
                    // Inserted within the middle of the queue.  Usually we don't have to wake
                    // up the event queue unless there is a barrier at the head of the queue
                    // and the message is the earliest asynchronous message in the queue.
                    needWake = mBlocked && p.target == null && msg.isAsynchronous();
                    Message prev;
                    for (; ; )
                    {
                        prev = p;
                        p = p.next;
                        if (p == null || when < p.when)
                        {
                            break;
                        }
                        if (needWake && p.isAsynchronous())
                        {
                            needWake = false;
                        }
                    }
                    msg.next = p; // invariant: p == prev.next
                    prev.next = msg;
                }

                // We can assume mPtr != 0 because mQuitting is false.
                if (needWake)
                {
                    nativeWake(mPtr);
                }
            }
            return true;
        }

        internal bool hasMessages(Handler h, int what, object obj)
        {
            if (h == null)
            {
                return false;
            }

            lock (LOCK) {
                Message p = mMessages;
                while (p != null)
                {
                    if (p.target == h && p.what == what && (obj == null || p.obj == obj))
                    {
                        return true;
                    }
                    p = p.next;
                }
                return false;
            }
        }

        internal bool hasEqualMessages(Handler h, int what, object obj)
        {
            if (h == null)
            {
                return false;
            }

            lock (LOCK) {
                Message p = mMessages;
                while (p != null)
                {
                    if (p.target == h && p.what == what && (obj == null || obj.Equals(p.obj)))
                    {
                        return true;
                    }
                    p = p.next;
                }
                return false;
            }
        }

        internal bool hasMessages(Handler h, Runnable r, object obj)
        {
            if (h == null)
            {
                return false;
            }

            lock (LOCK) {
                Message p = mMessages;
                while (p != null)
                {
                    if (p.target == h && p.callback == r && (obj == null || p.obj == obj))
                    {
                        return true;
                    }
                    p = p.next;
                }
                return false;
            }
        }

        internal bool hasMessages(Handler h)
        {
            if (h == null)
            {
                return false;
            }

            lock (LOCK) {
                Message p = mMessages;
                while (p != null)
                {
                    if (p.target == h)
                    {
                        return true;
                    }
                    p = p.next;
                }
                return false;
            }
        }

        internal void removeMessages(Handler h, int what, object obj)
        {
            if (h == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h && p.what == what
                       && (obj == null || p.obj == obj))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.what == what
                            && (obj == null || n.obj == obj))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        internal void removeEqualMessages(Handler h, int what, object obj)
        {
            if (h == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h && p.what == what
                       && (obj == null || obj.Equals(p.obj)))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.what == what
                            && (obj == null || obj.Equals(n.obj)))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        internal void removeMessages(Handler h, Runnable r, object obj)
        {
            if (h == null || r == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h && p.callback == r
                       && (obj == null || p.obj == obj))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.callback == r
                            && (obj == null || n.obj == obj))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        internal void removeMessages(Handler h, Runnable r, int what)
        {
            if (h == null || r == null)
            {
                return;
            }

            lock (LOCK)
            {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h && p.callback == r
                       && p.what == what)
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.callback == r
                            && n.what == what)
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        internal void removeEqualMessages(Handler h, Runnable r, object obj)
        {
            if (h == null || r == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h && p.callback == r
                       && (obj == null || obj.Equals(p.obj)))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && n.callback == r
                            && (obj == null || obj.Equals(n.obj)))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }


        internal void removeCallbacksAndMessages(Handler h, object obj)
        {
            if (h == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h
                        && (obj == null || p.obj == obj))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && (obj == null || n.obj == obj))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        internal void removeCallbacksAndEqualMessages(Handler h, object obj)
        {
            if (h == null)
            {
                return;
            }

            lock (LOCK) {
                Message p = mMessages;

                // Remove all messages at front.
                while (p != null && p.target == h
                        && (obj == null || obj.Equals(p.obj)))
                {
                    Message n = p.next;
                    mMessages = n;
                    p.recycleUnchecked();
                    p = n;
                }

                // Remove all messages after front.
                while (p != null)
                {
                    Message n = p.next;
                    if (n != null)
                    {
                        if (n.target == h && (obj == null || obj.Equals(n.obj)))
                        {
                            Message nn = n.next;
                            n.recycleUnchecked();
                            p.next = nn;
                            continue;
                        }
                    }
                    p = n;
                }
            }
        }

        private void removeAllMessagesLocked()
        {
            Message p = mMessages;
            while (p != null)
            {
                Message n = p.next;
                p.recycleUnchecked();
                p = n;
            }
            mMessages = null;
        }

        private void removeAllFutureMessagesLocked()
        {
            long now = NanoTime.currentTimeMillis();
            Message p = mMessages;
            if (p != null)
            {
                if (p.when > now)
                {
                    removeAllMessagesLocked();
                }
                else
                {
                    Message n;
                    for (; ; )
                    {
                        n = p.next;
                        if (n == null)
                        {
                            return;
                        }
                        if (n.when > now)
                        {
                            break;
                        }
                        p = n;
                    }
                    p.next = null;
                    do
                    {
                        p = n;
                        n = p.next;
                        p.recycleUnchecked();
                    } while (n != null);
                }
            }
        }

        /**
         * Callback interface for discovering when a thread is going to block
         * waiting for more messages.
         */
        public abstract class IdleHandler
        {
            /**
             * Called when the message queue has run out of messages and will now
             * wait for more.  Return true to keep your idle handler active, false
             * to have it removed.  This may be called if there are still messages
             * pending in the queue, but they are all scheduled to be dispatched
             * after the current time.
             */
            public abstract bool queueIdle();
        }

        /**
         * A listener which is invoked when file descriptor related events occur.
         */
        public abstract class OnFileDescriptorEventListener
        {
            /**
             * File descriptor event: Indicates that the file descriptor is ready for input
             * operations, such as reading.
             * <p>
             * The listener should read all available data from the file descriptor
             * then return <code>true</code> to keep the listener active or <code>false</code>
             * to remove the listener.
             * </p><p>
             * In the case of a socket, this event may be generated to indicate
             * that there is at least one incoming connection that the listener
             * should accept.
             * </p><p>
             * This event will only be generated if the {@link #EVENT_INPUT} event mask was
             * specified when the listener was added.
             * </p>
             */
            public const int EVENT_INPUT = 1 << 0;

            /**
             * File descriptor event: Indicates that the file descriptor is ready for output
             * operations, such as writing.
             * <p>
             * The listener should write as much data as it needs.  If it could not
             * write everything at once, then it should return <code>true</code> to
             * keep the listener active.  Otherwise, it should return <code>false</code>
             * to remove the listener then re-register it later when it needs to write
             * something else.
             * </p><p>
             * This event will only be generated if the {@link #EVENT_OUTPUT} event mask was
             * specified when the listener was added.
             * </p>
             */
            public const int EVENT_OUTPUT = 1 << 1;

            /**
             * File descriptor event: Indicates that the file descriptor encountered a
             * fatal error.
             * <p>
             * File descriptor errors can occur for various reasons.  One common error
             * is when the remote peer of a socket or pipe closes its end of the connection.
             * </p><p>
             * This event may be generated at any time regardless of whether the
             * {@link #EVENT_ERROR} event mask was specified when the listener was added.
             * </p>
             */
            public const int EVENT_ERROR = 1 << 2;

            /** @hide */
            internal enum Events {
                EVENT_INPUT,
                EVENT_OUTPUT,
                EVENT_ERROR
            };

            /**
             * Called when a file descriptor receives events.
             *
             * @param fd The file descriptor.
             * @param events The set of events that occurred: a combination of the
             * {@link #EVENT_INPUT}, {@link #EVENT_OUTPUT}, and {@link #EVENT_ERROR} event masks.
             * @return The new set of events to watch, or 0 to unregister the listener.
             *
             * @see #EVENT_INPUT
             * @see #EVENT_OUTPUT
             * @see #EVENT_ERROR
             */
            public abstract int onFileDescriptorEvents(FileDescriptor fd, int events);
        }

        private class FileDescriptorRecord
        {
            public readonly FileDescriptor mDescriptor;
            public int mEvents;
            public OnFileDescriptorEventListener mListener;
            public int mSeq;

            public FileDescriptorRecord(FileDescriptor descriptor,
                    int events, OnFileDescriptorEventListener listener)
            {
                mDescriptor = descriptor;
                mEvents = events;
                mListener = listener;
            }
        }
    }
}