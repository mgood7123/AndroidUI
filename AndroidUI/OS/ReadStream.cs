namespace AndroidUI.OS
{
    /// <summary>
    /// a stream that supports reading from
    /// <para>
    /// does not take ownership of the given stream unless `owns` is set to `true`
    /// </para>
    /// </summary>
    public class ReadStream : Stream
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
        /// the default implementation is <code>return stream.Read(buffer, offset, count);</code>
        /// </summary>
        /// <returns></returns>
        protected virtual int OnRead(Stream stream, byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// the default implementation is <code>return stream.Read(buffer);</code>
        /// </summary>
        /// <returns></returns>
        protected virtual int OnRead(Stream stream, Span<byte> buffer)
        {
            return stream.Read(buffer);
        }

        /// <summary>
        /// the default implementation is <code>return stream.ReadByte();</code>
        /// </summary>
        /// <returns></returns>
        protected virtual int OnReadByte(Stream stream)
        {
            return stream.ReadByte();
        }

        /// <summary>
        /// the default implementation is
        /// <code>
        /// byte[] buffer = new byte[bufferSize];
        /// int read;
        /// while ((read = Read(buffer, 0, buffer.Length)) != 0)
        ///     destination.Write(buffer, 0, read);
        /// </code>
        /// </summary>
        protected virtual void OnCopyTo(Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read;
            while ((read = Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }

        public ReadStream(Stream stream, bool owns = false)
        {
            innerStream = stream;
            this.owns = owns;
            if (!innerStream.CanRead)
            {
                throw new NotSupportedException("the given stream must be readable");
            }
        }

        public sealed override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return true; } }

        public sealed override bool CanWrite { get { return false; } }

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
            return OnRead(innerStream, buffer, offset, count);
        }

        public sealed override int Read(Span<byte> buffer)
        {
            return OnRead(innerStream, buffer);
        }


        public sealed override int ReadByte()
        {
            return OnReadByte(innerStream);
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
            throw new NotSupportedException();
        }

        public sealed override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotSupportedException();
        }

        public sealed override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        public sealed override void CopyTo(Stream destination, int bufferSize)
        {
            OnCopyTo(destination, bufferSize);
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