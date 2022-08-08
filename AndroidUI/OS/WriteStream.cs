namespace AndroidUI.OS
{
    /// <summary>
    /// a Write stream that supports writing to
    /// <para>
    /// does not take ownership of the given stream unless `owns` is set to `true`
    /// </para>
    /// </summary>
    public class WriteStream : Stream
    {
        private readonly Stream innerStream;
        bool owns;

        /// <summary>
        /// the default implementation is <code>return stream.Seek(offset, origin);</code>
        /// </summary>
        /// <returns></returns>
        protected virtual long OnSeek(Stream stream, long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        /// <summary>
        /// this gets called in `CopyTo`
        /// the default implementation is <code>return stream.Read(buffer, offset, count);</code>
        /// </summary>
        /// <returns></returns>
        protected virtual int OnReadFromCopyTo(Stream stream, byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// the default implementation is <code>stream.Write(buffer, offset, count);</code>
        /// </summary>
        protected virtual void OnWrite(Stream stream, byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// the default implementation is <code>stream.Write(buffer);</code>
        /// </summary>
        protected virtual void OnWrite(Stream stream, ReadOnlySpan<byte> buffer)
        {
            stream.Write(buffer);
        }

        /// <summary>
        /// the default implementation is <code>stream.WriteByte(value);</code>
        /// </summary>
        protected virtual void OnWriteByte(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        /// <summary>
        /// the default implementation is
        /// <code>
        /// byte[] buffer = new byte[bufferSize];
        /// int read;
        /// while ((read = OnReadFromCopyTo(stream, buffer, 0, buffer.Length)) != 0)
        ///     destination.Write(buffer, 0, read);
        /// </code>
        /// </summary>
        protected virtual void OnCopyTo(Stream stream, Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = OnReadFromCopyTo(stream, buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }

        public WriteStream(Stream stream, bool owns = false)
        {
            innerStream = stream;
            this.owns = owns;
            if (!innerStream.CanRead)
            {
                throw new NotSupportedException("the given stream must be readable for CopyTo");
            }

            if (!innerStream.CanWrite)
            {
                throw new NotSupportedException("the given stream must be writable");
            }
        }

        public sealed override bool CanRead { get { return false; } }

        public override bool CanSeek { get { return true; } }

        public sealed override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override long Length
        {
            get
            {
                return innerStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return innerStream.Position;
            }
            set
            {
                innerStream.Position = value;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return innerStream.CanTimeout;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return innerStream.ReadTimeout;
            }
            set
            {
                innerStream.ReadTimeout = value;
            }
        }
        public override int WriteTimeout
        {
            get
            {
                return innerStream.WriteTimeout;
            }

            set
            {
                innerStream.WriteTimeout = value;
            }
        }

        public Stream BaseStream => innerStream;

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public sealed override int Read(Span<byte> buffer)
        {
            throw new NotSupportedException();
        }

        public sealed override int ReadByte()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return OnSeek(innerStream, offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            OnWrite(innerStream, buffer, offset, count);
        }

        public sealed override void Write(ReadOnlySpan<byte> buffer)
        {
            OnWrite(innerStream, buffer);
        }

        public sealed override void WriteByte(byte value)
        {
            OnWriteByte(innerStream, value);
        }

        public sealed override void CopyTo(Stream destination, int bufferSize)
        {
            OnCopyTo(innerStream, destination, bufferSize);
        }

        public override void Close()
        {
            base.Close();
            if (owns)
            {
                innerStream.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (owns)
            {
                innerStream.Dispose();
            }
        }
    }
}