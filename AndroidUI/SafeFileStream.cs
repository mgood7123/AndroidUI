using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace AndroidUI
{
    // on android, passing a FileDescriptor to a FileOutputStream will adopt said FileDescriptor but not take ownership of it
    // on JVM passing a FileDescriptor to a FileOutputStream will take ownership of said FileDescriptor
    public class SafeFileStream : FileStream
    {
        SafeHandle h;
        bool takeOwnership;

        public override void Close()
        {
            if (h != null && takeOwnership)
            {
                if (!h.IsClosed) h.Close();
                if (!h.IsInvalid) h.Dispose();
            }
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (h != null && takeOwnership)
            {
                if (!h.IsClosed) h.Close();
                if (!h.IsInvalid) h.Dispose();

            }
            base.Dispose(disposing);
        }

        public SafeFileStream(SafeHandle handle, FileAccess access, bool takeOwnership = false) : base(new SafeFileHandle(handle.DangerousGetHandle(), takeOwnership), access)
        {
            h = handle;
            this.takeOwnership = takeOwnership;
        }

        public SafeFileStream(SafeHandle handle, FileAccess access, int bufferSize, bool takeOwnership = false) : base(new SafeFileHandle(handle.DangerousGetHandle(), takeOwnership), access, bufferSize)
        {
            h = handle;
            this.takeOwnership = takeOwnership;
        }

        public SafeFileStream(SafeHandle handle, FileAccess access, int bufferSize, bool isAsync, bool takeOwnership = false) : base(new SafeFileHandle(handle.DangerousGetHandle(), takeOwnership), access, bufferSize, isAsync)
        {
            h = handle;
            this.takeOwnership = takeOwnership;
        }
    }
}