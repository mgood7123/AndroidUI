using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AndroidUI
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

        public static implicit operator SafeFileHandle(FileDescriptor descriptor)
        {
            return descriptor == null ? null : descriptor.ToSafeFileHandle();
        }

        public SafeFileHandle ToSafeFileHandle()
        {
            return !IsValid ? null : new SafeFileHandle(handle.DangerousGetHandle(), true);
        }

        public FileStream ToFileInputStream() => !IsValid ? null : new FileStream(this, FileAccess.Read);
        public FileStream ToFileInputStream(int bufferSize) => !IsValid ? null : new FileStream(this, FileAccess.Read, bufferSize);


        public FileStream ToFileOutputStream() => !IsValid ? null : new FileStream(this, FileAccess.Write);
        public FileStream ToFileOutputStream(int bufferSize) => !IsValid ? null : new FileStream(this, FileAccess.Write, bufferSize);

        public FileStream ToFileInputOutputStream() => !IsValid ? null : new FileStream(this, FileAccess.ReadWrite);
        public FileStream ToFileInputOutputStream(int bufferSize) => !IsValid ? null : new FileStream(this, FileAccess.ReadWrite, bufferSize);
    }
}