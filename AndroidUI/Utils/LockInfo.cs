namespace AndroidUI.Utils
{
    /// <summary>
    /// Contains required information for lock creation, lock manipulation, and lock disposal
    /// <br></br><br></br>
    /// a timeout of -1 should specify infinite duration if supported by the lock implementation
    /// <br></br><br></br>
    /// This class is NOT thread-safe
    /// </summary>
    public abstract class LockInfo : IDisposable
    {
        private object lock_object;
        private bool disposedValue;

        /// <summary>
        /// Creates a lock, the lock can be destroyed with Dispose()
        /// <br></br><br></br>
        /// This is automatically called by the constructor
        /// <br></br><br></br>
        /// This method is NOT thread-safe
        /// </summary>
        public void ObtainLock()
        {
            if (lock_object == null)
            {
                disposedValue = false;
                lock_object = CreateLock();
                GC.ReRegisterForFinalize(this);
            }
        }

        public abstract object CreateLock();

        // read
        protected abstract void AcquireReadLockImpl(object lock_object, int timeout);
        protected abstract void ReleaseReadLockImpl(object lock_object);

        // write
        protected abstract void AcquireWriteLockImpl(object lock_object, int timeout);
        protected abstract void ReleaseWriteLockImpl(object lock_object);

        /// <summary>
        /// Acquires a read lock with a specified timeout
        /// <br></br>
        /// the exact behaviour depends on the lock implementation provided by a derived class
        /// <br></br><br></br>
        /// a timeout of -1 should specify infinite duration if supported by the lock implementation
        /// </summary>
        public void AcquireReadLock(int timeout) => AcquireReadLockImpl(lock_object, timeout);

        /// <summary>
        /// Releases a read lock
        /// <br></br>
        /// the exact behaviour depends on the lock implementation provided by a derived class
        /// </summary>
        public void ReleaseReadLock() => ReleaseReadLockImpl(lock_object);

        /// <summary>
        /// Acquires a write lock with a specified timeout
        /// <br></br>
        /// the exact behaviour depends on the lock implementation provided by a derived class
        /// <br></br><br></br>
        /// a timeout of -1 should specify infinite duration if supported by the lock implementation
        /// </summary>
        public void AcquireWriteLock(int timeout) => AcquireWriteLockImpl(lock_object, timeout);

        /// <summary>
        /// Releases a write lock
        /// <br></br>
        /// the exact behaviour depends on the lock implementation provided by a derived class
        /// </summary>
        public void ReleaseWriteLock() => ReleaseWriteLockImpl(lock_object);

        /// <summary>
        /// this method should be implemented if your lock uses native resources or Disposable objects
        /// </summary>
        protected virtual void DisposeLock(object lock_object) { }

        private void Dispose(bool fromFinalizer)
        {
            if (!disposedValue)
            {
                DisposeLock(lock_object);
                // incase caller does nothing, set lock object to null to allow it to be GC'd
                lock_object = null;
                disposedValue = true;
            }
        }

        /// <summary>
        /// calls the ObtainLock method, if you would like to defer creation of the lock, pass `false` to this constructor
        /// </summary>
        public LockInfo() : this(ObtainLockNow: true) { }

        /// <summary>
        /// calls the ObtainLock method if true is passed, if you would like to defer creation of the lock, pass false instead
        /// </summary>
        public LockInfo(bool ObtainLockNow)
        {
            if (ObtainLockNow) ObtainLock();
        }

        ~LockInfo()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(fromFinalizer: true);
        }

        /// <summary>
        /// Destroys a lock, the lock can be created again with ObtainLock
        /// <br></br><br></br>
        /// This method is NOT thread-safe
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(fromFinalizer: false);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// implements a basic `Monitor` lock, Single-Reader/Single-Writer
    /// </summary>
    public class BasicLockInfo : LockInfo
    {
        public override object CreateLock()
        {
            return new object();
        }

        protected override void AcquireReadLockImpl(object lock_object, int timeout)
        {
            Monitor.TryEnter(lock_object, timeout);
        }

        protected override void ReleaseReadLockImpl(object lock_object)
        {
            Monitor.Exit(lock_object);
        }

        protected override void AcquireWriteLockImpl(object lock_object, int timeout)
        {
            Monitor.TryEnter(lock_object, timeout);
        }

        protected override void ReleaseWriteLockImpl(object lock_object)
        {
            Monitor.Exit(lock_object);
        }
    }

    /// <summary>
    /// implements a basic `ReaderWriterLock` lock, Multi-Reader/Single-Writer
    /// </summary>
    public class ReaderWriterLockInfo : LockInfo
    {
        public override object CreateLock()
        {
            return new ReaderWriterLock();
        }

        protected override void AcquireReadLockImpl(object lock_object, int timeout)
        {
            ((ReaderWriterLock)lock_object).AcquireReaderLock(timeout);
        }

        protected override void ReleaseReadLockImpl(object lock_object)
        {
            ((ReaderWriterLock)lock_object).ReleaseReaderLock();
        }

        protected override void AcquireWriteLockImpl(object lock_object, int timeout)
        {
            ((ReaderWriterLock)lock_object).AcquireWriterLock(timeout);
        }

        protected override void ReleaseWriteLockImpl(object lock_object)
        {
            ((ReaderWriterLock)lock_object).ReleaseWriterLock();
        }
    }

    /// <summary>
    /// implements a basic `ReaderWriterLockSlim` lock, Multi-Reader/Single-Writer
    /// </summary>
    public class ReaderWriterLockSlimInfo : LockInfo
    {
        public override object CreateLock()
        {
            return new ReaderWriterLockSlim();
        }

        protected override void AcquireReadLockImpl(object lock_object, int timeout)
        {
            ((ReaderWriterLockSlim)lock_object).TryEnterReadLock(timeout);
        }

        protected override void ReleaseReadLockImpl(object lock_object)
        {
            ((ReaderWriterLockSlim)lock_object).ExitReadLock();
        }

        protected override void AcquireWriteLockImpl(object lock_object, int timeout)
        {
            ((ReaderWriterLockSlim)lock_object).TryEnterWriteLock(timeout);
        }

        protected override void ReleaseWriteLockImpl(object lock_object)
        {
            ((ReaderWriterLockSlim)lock_object).ExitWriteLock();
        }

        protected override void DisposeLock(object lock_object)
        {
            ((ReaderWriterLockSlim)lock_object).Dispose();
        }
    }
}