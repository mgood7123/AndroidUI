//------------------------------------------------------------ 
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------
using System.Security;
using System.Security.Permissions;

namespace AndroidUI.Utils
{

    // A simple synchronized pool would simply lock a stack and push/pop on return/take.
    //
    // This implementation tries to reduce locking by exploiting the case where an item
    // is taken and returned by the same thread, which turns out to be common in our 
    // scenarios.
    // 
    // Initially, all the quota is allocated to a global (non-thread-specific) pool, 
    // which takes locks.  As different threads take and return values, we record their IDs,
    // and if we detect that a thread is taking and returning "enough" on the same thread, 
    // then we decide to "promote" the thread.  When a thread is promoted, we decrease the
    // quota of the global pool by one, and allocate a thread-specific entry for the thread
    // to store it's value.  Once this entry is allocated, the thread can take and return
    // it's value from that entry without taking any locks.  Not only does this avoid 
    // locks, but it affinitizes pooled items to a particular thread.
    // 
    // There are a couple of additional things worth noting: 
    //
    // It is possible for a thread that we have reserved an entry for to exit.  This means 
    // we will still have a entry allocated for it, but the pooled item stored there
    // will never be used.  After a while, we could end up with a number of these, and
    // as a result we would begin to exhaust the quota of the overall pool.  To mitigate this
    // case, we throw away the entire per-thread pool, and return all the quota back to 
    // the global pool if we are unable to promote a thread (due to lack of space).  Then
    // the set of active threads will be re-promoted as they take and return items. 
    // 
    // You may notice that the code does not immediately promote a thread, and does not
    // immediately throw away the entire per-thread pool when it is unable to promote a 
    // thread.  Instead, it uses counters (based on the number of calls to the pool)
    // and a threshold to figure out when to do these operations.  In the case where the
    // pool to misconfigured to have too few items for the workload, this avoids constant
    // promoting and rebuilding of the per thread entries. 
    //
    // You may also notice that we do not use interlocked methods when adjusting statistics. 
    // Since the statistics are a heuristic as to how often something is happening, they 
    // do not need to be perfect.
    // 
    class SynchronizedPool<T> where T : class
    {
        Entry[] entries;
        PendingEntry[] pending;
        GlobalPool globalPool;
        int maxCount;
        int promotionFailures;
        const int maxPendingEntries = 128;
        const int maxReturnsBeforePromotion = 64;
        const int maxPromotionFailures = 64;
        const int maxThreadItemsPerProcessor = 16;

        public SynchronizedPool(int maxCount)
        {
            int threadCount = maxCount;
            int maxThreadCount = maxThreadItemsPerProcessor + SynchronizedPoolHelper.ProcessorCount;
            if (threadCount > maxThreadCount)
                threadCount = maxThreadCount;
            this.maxCount = maxCount;
            entries = new Entry[threadCount];
            pending = new PendingEntry[4];
            globalPool = new GlobalPool(maxCount);
        }

        private readonly object l = new();

        object ThisLock
        {
            get { return l; }
        }

        public void Clear()
        {
            Entry[] entries = this.entries;

            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].value = null;
            }

            globalPool.Clear();
        }

        void HandlePromotionFailure(int thisThreadID)
        {
            int newPromotionFailures = promotionFailures + 1;

            if (newPromotionFailures >= maxPromotionFailures)
            {
                lock (ThisLock)
                {
                    entries = new Entry[entries.Length];

                    globalPool.MaxCount = maxCount;
                }

                PromoteThread(thisThreadID);
            }
            else
            {
                promotionFailures = newPromotionFailures;
            }
        }

        bool PromoteThread(int thisThreadID)
        {
            lock (ThisLock)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    int threadID = entries[i].threadID;

                    if (threadID == thisThreadID)
                    {
                        return true;
                    }
                    else if (threadID == 0)
                    {
                        globalPool.DecrementMaxCount();
                        entries[i].threadID = thisThreadID;
                        return true;
                    }
                }
            }

            return false;
        }

        void RecordReturnToGlobalPool(int thisThreadID)
        {
            PendingEntry[] localPending = pending;

            for (int i = 0; i < localPending.Length; i++)
            {
                int threadID = localPending[i].threadID;

                if (threadID == thisThreadID)
                {
                    int newReturnCount = localPending[i].returnCount + 1;

                    if (newReturnCount >= maxReturnsBeforePromotion)
                    {
                        localPending[i].returnCount = 0;

                        if (!PromoteThread(thisThreadID))
                        {
                            HandlePromotionFailure(thisThreadID);
                        }
                    }
                    else
                    {
                        localPending[i].returnCount = newReturnCount;
                    }
                    break;
                }
                else if (threadID == 0)
                {
                    break;
                }
            }
        }

        void RecordTakeFromGlobalPool(int thisThreadID)
        {
            PendingEntry[] localPending = pending;

            for (int i = 0; i < localPending.Length; i++)
            {
                int threadID = localPending[i].threadID;

                if (threadID == thisThreadID)
                {
                    return;
                }
                else if (threadID == 0)
                {
                    lock (localPending)
                    {
                        if (localPending[i].threadID == 0)
                        {
                            localPending[i].threadID = thisThreadID;
                            return;
                        }
                    }
                }
            }

            if (localPending.Length >= maxPendingEntries)
            {
                pending = new PendingEntry[localPending.Length];
            }
            else
            {
                PendingEntry[] newPending = new PendingEntry[localPending.Length * 2];
                Array.Copy(localPending, newPending, localPending.Length);
                pending = newPending;
            }
        }

        public bool Release(T value)
        {
            int thisThreadID = Thread.CurrentThread.ManagedThreadId;

            if (thisThreadID == 0)
                return false;

            if (ReturnToPerThreadPool(thisThreadID, value))
                return true;

            return ReturnToGlobalPool(thisThreadID, value);
        }

        bool ReturnToPerThreadPool(int thisThreadID, T value)
        {
            Entry[] entries = this.entries;

            for (int i = 0; i < entries.Length; i++)
            {
                int threadID = entries[i].threadID;

                if (threadID == thisThreadID)
                {
                    if (entries[i].value == null)
                    {
                        entries[i].value = value;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (threadID == 0)
                {
                    break;
                }
            }

            return false;
        }

        bool ReturnToGlobalPool(int thisThreadID, T value)
        {
            RecordReturnToGlobalPool(thisThreadID);

            return globalPool.Return(value);
        }

        public T Aquire()
        {
            int thisThreadID = Thread.CurrentThread.ManagedThreadId;

            if (thisThreadID == 0)
                return null;

            T value = TakeFromPerThreadPool(thisThreadID);

            if (value != null)
                return value;

            return TakeFromGlobalPool(thisThreadID);
        }

        T TakeFromPerThreadPool(int thisThreadID)
        {
            Entry[] entries = this.entries;

            for (int i = 0; i < entries.Length; i++)
            {
                int threadID = entries[i].threadID;

                if (threadID == thisThreadID)
                {
                    T value = entries[i].value;

                    if (value != null)
                    {
                        entries[i].value = null;
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (threadID == 0)
                {
                    break;
                }
            }

            return null;
        }

        T TakeFromGlobalPool(int thisThreadID)
        {
            RecordTakeFromGlobalPool(thisThreadID);

            return globalPool.Take();
        }

        struct Entry
        {
            public int threadID;
            public T value;
        }

        struct PendingEntry
        {
            public int threadID;
            public int returnCount;
        }

        class GlobalPool
        {
            Stack<T> items;
            int maxCount;

            public GlobalPool(int maxCount)
            {
                items = new Stack<T>();
                this.maxCount = maxCount;
            }

            public int MaxCount
            {
                get
                {
                    return maxCount;
                }
                set
                {
                    lock (ThisLock)
                    {
                        while (items.Count > value)
                        {
                            items.Pop();
                        }
                        maxCount = value;
                    }
                }
            }

            object ThisLock
            {
                get { return this; }
            }

            public void DecrementMaxCount()
            {
                lock (ThisLock)
                {
                    if (items.Count == maxCount)
                    {
                        items.Pop();
                    }
                    maxCount--;
                }
            }

            public T Take()
            {
                if (items.Count > 0)
                {
                    lock (ThisLock)
                    {
                        if (items.Count > 0)
                        {
                            return items.Pop();
                        }
                    }
                }
                return null;
            }

            public bool Return(T value)
            {
                if (items.Count < MaxCount)
                {
                    lock (ThisLock)
                    {
                        if (items.Count < MaxCount)
                        {
                            items.Push(value);
                            return true;
                        }
                    }
                }
                return false;
            }

            public void Clear()
            {
                lock (ThisLock)
                {
                    items.Clear();
                }
            }
        }
    }

    static class SynchronizedPoolHelper
    {
        static public readonly int ProcessorCount = GetProcessorCount();

        ///  
        /// Critical - Asserts in order to get the processor count from the environment
        /// Safe - this data isn't actually protected so it's ok to leak 
        ///  
        [SecurityCritical, SecurityTreatAsSafe]
        [EnvironmentPermission(SecurityAction.Assert, Read = "NUMBER_OF_PROCESSORS")]
        static int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }
    }
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.