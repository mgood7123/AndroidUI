using System.IO.Pipes;

namespace AndroidUI.OS
{
    public static class Functions
    {
        /// <returns>
        /// a pair of FileDescriptor's connected to a new pipe
        /// <br></br>fd[0] is used to read from the pipe
        /// <br></br>fd[1] is used to write to the pipe
        /// </returns>
        public static FileDescriptor[] pipe()
        {
            AnonymousPipeServerStream r = new(PipeDirection.In);
            AnonymousPipeClientStream w = new(PipeDirection.Out, r.GetClientHandleAsString());

            return new FileDescriptor[2] {
                new(r, r.SafePipeHandle, false, true),
                new(w, w.SafePipeHandle, false, true)
            };
        }

        /// <summary>
        /// calls the C api `memcpy`
        /// </summary>
        public static unsafe void memcpy(void* dst, void* src, nuint length)
            => Native.Memcpy(dst, src, length);

        /// <summary>
        /// calls the C api `memcpy`
        /// </summary>
        public static unsafe void mempy(IntPtr dst, IntPtr src, nuint length)
            => Native.Memcpy(dst, src, length);

        /// <summary>
        /// calls the C api `memcpy`, the input src and dst arrays are not copied
        /// </summary>
        public static unsafe void memcpy<T1, T2>(T1[] dst, T2[] src, nuint length)
            where T1 : unmanaged
            where T2 : unmanaged
            => Native.Memcpy(dst, src, length);

        /// <summary>
        /// calls the C api `memcmp`
        /// </summary>
        public static unsafe int memcmp(void* buf1, void* buf2, nuint length)
            => Native.Memcmp(buf1, buf2, length);

        /// <summary>
        /// calls the C api `memcmp`
        /// </summary>
        public static unsafe void memcmp(IntPtr buf1, IntPtr buf2, nuint length)
            => Native.Memcmp(buf1, buf2, length);

        /// <summary>
        /// calls the C api `memcmp`, the input buf1 and buf2 arrays are not copied
        /// </summary>
        public static unsafe void memcmp<T1, T2>(T1[] buf1, T2[] buf2, nuint length)
            where T1 : unmanaged
            where T2 : unmanaged
            => Native.Memcmp(buf1, buf2, length);

        /// <summary>
        /// calls the C api `memset`
        /// </summary>
        /// <returns>the input pointer</returns>
        public static unsafe void* memset(void* buf, int value, nuint length)
            => Native.Memset(buf, value, length);

        /// <summary>
        /// calls the C api `memset`
        /// </summary>
        /// <returns>the input pointer</returns>
        public static IntPtr memset(IntPtr buf, int value, nuint length)
            => Native.Memset(buf, value, length);

        /// <summary>
        /// calls the C api `memset`, the input array is not copied
        /// </summary>
        /// <returns>the input array</returns>
        public static T1[] memset<T1>(T1[] buf, int value, nuint length)
            where T1 : unmanaged
            => Native.Memset(buf, value, length);
    }
}
