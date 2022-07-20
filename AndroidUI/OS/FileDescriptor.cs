using System.Runtime.InteropServices;

namespace AndroidUI.OS
{
    public class FileDescriptor : IDisposable
    {
        IDisposable owner;
        SafeHandle handle;
        bool leaveOpen;
        bool disposeOwner;
        private bool disposedValue;

        public FileDescriptor(IDisposable owner, SafeHandle handle, bool leaveOpen, bool disposeOwner)
        {
            this.owner = owner;
            this.handle = handle;
            this.leaveOpen = leaveOpen;
            this.disposeOwner = disposeOwner;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!leaveOpen)
                    {
                        if (handle != null)
                        {
                            handle.Dispose();
                            handle = null;
                        }
                    }
                    if (DisposeOwner)
                    {
                        if (owner != null)
                        {
                            owner.Dispose();
                            owner = null;
                        }
                    }
                }
                disposedValue = true;
            }
        }

        ~FileDescriptor()
        {
            Dispose(disposing: false);
        }

        /// <summary>
        /// obtains the Handle referred to by this FileDescriptor
        /// </summary>
        public SafeHandle Handle => handle;

        /// <summary>
        /// obtains the Owner referred to by this FileDescriptor
        /// </summary>
        public IDisposable Owner => owner;

        /// <summary>
        /// returns true if the Handle will be disposed when Dispose is called
        /// </summary>
        public bool LeaveOpen => leaveOpen;

        /// <summary>
        /// returns true if the Owner will be disposed when Dispose is called
        /// </summary>
        public bool DisposeOwner => disposeOwner;

        /// <summary>
        /// returns false if there is no Handle, the Handle is invalid, or the Handle is closed
        /// </summary>
        public bool IsValid => handle != null && !handle.IsInvalid && !handle.IsClosed;

        /// <summary>
        /// closes the Handle
        /// </summary>
        public void Close()
        {
            if (IsValid)
            {
                handle.Close();
            }
        }

        /// <summary>
        /// disposes the Handle if FileDescriptor was created with owns = true
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public FileStream ToFileInputStream(bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.Read, takeOwnership);
        public FileStream ToFileInputStream(int bufferSize, bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.Read, bufferSize, takeOwnership);


        public FileStream ToFileOutputStream(bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.Write, takeOwnership);
        public FileStream ToFileOutputStream(int bufferSize, bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.Write, bufferSize, takeOwnership);

        public FileStream ToFileInputOutputStream(bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.ReadWrite, takeOwnership);
        public FileStream ToFileInputOutputStream(int bufferSize, bool takeOwnership = false) => !IsValid ? null : new SafeFileStream(handle, FileAccess.ReadWrite, bufferSize, takeOwnership);

        internal int getInt()
        {
            return !IsValid ? 0 : Handle.DangerousGetHandle().ToInt32();
        }

        internal long getLong()
        {
            return !IsValid ? 0 : Handle.DangerousGetHandle().ToInt64();
        }
    }
}