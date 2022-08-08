namespace AndroidUI.OS
{

    /// <summary>
    /// a ReadWrite stream that supports both reading from and writing to
    /// <para>
    /// does not take ownership of the given stream unless `owns` is set to `true`
    /// </para>
    /// </summary>
    public class ReadWriteStream : Stream
    {
        private readonly Stream innerStream;
        private long readPosition;
        private long writePosition;
        bool owns;

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

        public ReadWriteStream(Stream stream, bool owns = false)
        {
            innerStream = stream;
            this.owns = owns;
            if (!innerStream.CanRead)
            {
                throw new NotSupportedException("the given stream must be readable");
            }

            if (!innerStream.CanWrite)
            {
                throw new NotSupportedException("the given stream must be writable");
            }
        }

        public sealed override bool CanRead { get { return true; } }

        public sealed override bool CanSeek { get { return false; } }

        public sealed override bool CanWrite { get { return true; } }

        public override void Flush()
        {
            lock (innerStream)
            {
                innerStream.Flush();
            }
        }

        public override long Length
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.Length;
                }
            }
        }

        public sealed override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool CanTimeout
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.CanTimeout;
                }
            }
        }

        public override int ReadTimeout
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.ReadTimeout;
                }
            }
            set
            {
                lock (innerStream)
                {
                    innerStream.ReadTimeout = value;
                }
            }
        }
        public override int WriteTimeout
        {
            get
            {
                lock (innerStream)
                {
                    return innerStream.WriteTimeout;
                }
            }

            set
            {
                lock (innerStream)
                {
                    innerStream.WriteTimeout = value;
                }
            }
        }

        public Stream BaseStream => innerStream;

        public long ReadPosition
        {
            get
            {
                lock (innerStream)
                {
                    return readPosition;
                }
            }

            set
            {
                lock (innerStream)
                {
                    readPosition = value;
                }
            }
        }
        public long WritePosition
        {
            get
            {
                lock (innerStream)
                {
                    return writePosition;
                }
            }

            set
            {
                lock (innerStream)
                {
                    writePosition = value;
                }
            }
        }

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int bytes = OnRead(innerStream, buffer, offset, count);
                readPosition = innerStream.Position;
                return bytes;
            }
        }

        public sealed override int Read(Span<byte> buffer)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = OnRead(innerStream, buffer);
                readPosition = innerStream.Position;

                return red;
            }
        }


        public sealed override int ReadByte()
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = OnReadByte(innerStream);
                readPosition = innerStream.Position;

                return red;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public sealed override void Write(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                OnWrite(innerStream, buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }

        public sealed override void Write(ReadOnlySpan<byte> buffer)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                OnWrite(innerStream, buffer);
                writePosition = innerStream.Position;
            }
        }

        public sealed override void WriteByte(byte value)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                OnWriteByte(innerStream, value);
                writePosition = innerStream.Position;
            }
        }

        public sealed override void CopyTo(Stream destination, int bufferSize)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                OnCopyTo(destination, bufferSize);
                readPosition = innerStream.Position;
            }
        }

        public override void Close()
        {
            base.Close();
            lock (innerStream)
            {
                if (owns)
                {
                    innerStream.Close();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            lock (innerStream)
            {
                if (owns)
                {
                    innerStream.Dispose();
                }
            }
        }
    }
}