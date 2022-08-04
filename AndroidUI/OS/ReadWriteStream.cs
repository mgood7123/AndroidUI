namespace AndroidUI.OS
{
    /// <summary>
    /// a ReadWrite stream that supports both reading from and writing to
    /// <para>
    /// does not take ownership of the given stream unless `owns` is set to `true`
    /// </para>
    /// </summary>
    class ReadWriteStream : Stream
    {
        private readonly Stream innerStream;
        private long readPosition;
        private long writePosition;
        bool owns;

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

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

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

        public override long Position
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = innerStream.Read(buffer, offset, count);
                readPosition = innerStream.Position;

                return red;
            }
        }

        public override int Read(Span<byte> buffer)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = innerStream.Read(buffer);
                readPosition = innerStream.Position;

                return red;
            }
        }


        public override int ReadByte()
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                int red = innerStream.ReadByte();
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
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer, offset, count);
                writePosition = innerStream.Position;
            }
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.Write(buffer);
                writePosition = innerStream.Position;
            }
        }

        public override void WriteByte(byte value)
        {
            lock (innerStream)
            {
                innerStream.Position = writePosition;
                innerStream.WriteByte(value);
                writePosition = innerStream.Position;
            }
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            lock (innerStream)
            {
                innerStream.Position = readPosition;
                innerStream.CopyTo(destination, bufferSize);
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