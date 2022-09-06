using System.Buffers;
using System.Text;

namespace AndroidUI.Graphics
{
    public partial class RecordingCanvas2
    {
        public class TypeCheckingBinaryReader : BinaryReader
        {
            public TypeCheckingBinaryReader(Stream input) : base(input)
            {
            }

            public TypeCheckingBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
            {
            }

            public TypeCheckingBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
            {
            }

            public override int Read()
            {
                throw new NotSupportedException();
            }

            public override int PeekChar()
            {
                throw new NotSupportedException();
            }

            public void AssertExpectedType(byte expected_type, byte this_type)
            {
                if (expected_type != this_type)
                {
                    throw new InvalidOperationException("Expected " + TypeCheckingBinaryWriter.TYPE_TO_STRING(expected_type) + ", attempted to read as type " + TypeCheckingBinaryWriter.TYPE_TO_STRING(this_type));
                }
            }

            public void AssertExpectedIndex(int expected_index, int this_index)
            {
                if (expected_index != this_index)
                {
                    throw new InvalidOperationException("Expected index " + expected_index + ", attempted to read index " + this_index);
                }
            }

            public void AssertExpectedLength(int expected_length, int this_length)
            {
                if (expected_length != this_length)
                {
                    throw new InvalidOperationException("Expected length " + expected_length + ", attempted to read length " + this_length);
                }
            }

            public override int Read(byte[] buffer, int index, int count)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_BYTE_ARRAY);
                AssertExpectedIndex(base.ReadInt32(), index);
                AssertExpectedLength(base.ReadInt32(), count);
                return base.Read(buffer, index, count);
            }

            public override int Read(char[] buffer, int index, int count)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_CHAR_ARRAY);
                AssertExpectedIndex(base.ReadInt32(), index);
                AssertExpectedLength(base.ReadInt32(), count);
                return base.Read(buffer, index, count);
            }

            public override int Read(Span<byte> buffer)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_SPAN_BYTE);
                AssertExpectedLength(base.ReadInt32(), buffer.Length);
                return base.Read(buffer);
            }

            public override int Read(Span<char> buffer)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_SPAN_CHAR);
                AssertExpectedLength(base.ReadInt32(), buffer.Length);
                return base.Read(buffer);
            }

            public override bool ReadBoolean()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_BOOL);
                return base.ReadBoolean();
            }

            public override byte ReadByte()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_BYTE);
                return base.ReadByte();
            }

            public override byte[] ReadBytes(int count)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_BYTE_ARRAY);
                AssertExpectedLength(base.ReadInt32(), count);
                return base.ReadBytes(count);
            }

            public override char ReadChar()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_CHAR);
                return base.ReadChar();
            }

            public override char[] ReadChars(int count)
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_CHAR_ARRAY);
                AssertExpectedLength(base.ReadInt32(), count);
                return base.ReadChars(count);
            }

            public override decimal ReadDecimal()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_DECIMAL);
                return base.ReadDecimal();
            }

            public override double ReadDouble()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_DOUBLE);
                return base.ReadDouble();
            }

            public override Half ReadHalf()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_HALF);
                return base.ReadHalf();
            }

            public override short ReadInt16()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_SHORT);
                return base.ReadInt16();
            }

            public override int ReadInt32()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_INT);
                return base.ReadInt32();
            }

            public override long ReadInt64()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_LONG);
                return base.ReadInt64();
            }

            public override sbyte ReadSByte()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_SBYTE);
                return base.ReadSByte();
            }

            public override float ReadSingle()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_FLOAT);
                return base.ReadSingle();
            }

            public override string ReadString()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_STRING);
                return base.ReadString();
            }

            public override ushort ReadUInt16()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_USHORT);
                return base.ReadUInt16();
            }

            public override uint ReadUInt32()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_UINT);
                return base.ReadUInt32();
            }

            public override ulong ReadUInt64()
            {
                AssertExpectedType(base.ReadByte(), TypeCheckingBinaryWriter.TYPE_ULONG);
                return base.ReadUInt64();
            }
        }

        public class TypeCheckingBinaryWriter : BinaryWriter
        {
            internal const byte TYPE_UNDEFINED = 0;
            internal const byte TYPE_BOOL = 1;
            internal const byte TYPE_BYTE = 2;
            internal const byte TYPE_BYTE_ARRAY = 3;
            internal const byte TYPE_CHAR = 4;
            internal const byte TYPE_CHAR_ARRAY = 5;
            internal const byte TYPE_DECIMAL = 6;
            internal const byte TYPE_DOUBLE = 7;
            internal const byte TYPE_HALF = 8;
            internal const byte TYPE_SHORT = 9;
            internal const byte TYPE_INT = 10;
            internal const byte TYPE_LONG = 11;
            internal const byte TYPE_SPAN_BYTE = 12;
            internal const byte TYPE_SPAN_CHAR = 13;
            internal const byte TYPE_SBYTE = 14;
            internal const byte TYPE_FLOAT = 15;
            internal const byte TYPE_STRING = 16;
            internal const byte TYPE_USHORT = 17;
            internal const byte TYPE_UINT = 18;
            internal const byte TYPE_ULONG = 19;

            internal static string TYPE_TO_STRING(byte TYPE)
            {
                return TYPE switch
                {
                    TYPE_UNDEFINED => "UNDEFINED",
                    TYPE_BOOL => "BOOL",
                    TYPE_BYTE => "BYTE",
                    TYPE_BYTE_ARRAY => "BYTE ARRAY",
                    TYPE_CHAR => "CHAR",
                    TYPE_CHAR_ARRAY => "CHAR ARRAY",
                    TYPE_DECIMAL => "DECIMAL",
                    TYPE_DOUBLE => "DOUBLE",
                    TYPE_HALF => "HALF",
                    TYPE_SHORT => "SHORT",
                    TYPE_INT => "INT",
                    TYPE_LONG => "LONG",
                    TYPE_SPAN_BYTE => "SPAN<BYTE>",
                    TYPE_SPAN_CHAR => "SPAN<CHAR>",
                    TYPE_SBYTE => "SBYTE",
                    TYPE_FLOAT => "FLOAT",
                    TYPE_STRING => "STRING",
                    TYPE_USHORT => "USHORT",
                    TYPE_UINT => "UINT",
                    TYPE_ULONG => "ULONG",
                    _ => "INVALID TYPE (" + TYPE + ")"
                };
            }

            public TypeCheckingBinaryWriter(Stream output) : base(output)
            {
            }

            public TypeCheckingBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
            {
            }

            public TypeCheckingBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
            {
            }

            protected TypeCheckingBinaryWriter()
            {
            }

            public override void Write(bool value)
            {
                base.Write(TYPE_BOOL);
                base.Write(value);
            }

            public override void Write(byte value)
            {
                base.Write(TYPE_BYTE);
                base.Write(value);
            }

            public override void Write(byte[] buffer)
            {
                base.Write(TYPE_BYTE_ARRAY);
                base.Write(0);
                base.Write(buffer.Length);
                base.Write(buffer);
            }

            public override void Write(byte[] buffer, int index, int count)
            {
                base.Write(TYPE_BYTE_ARRAY);
                base.Write(index);
                base.Write(count - index);
                base.Write(buffer, index, count);
            }

            public override void Write(char ch)
            {
                base.Write(TYPE_CHAR);
                base.Write(ch);
            }

            public override void Write(char[] chars)
            {
                base.Write(TYPE_CHAR_ARRAY);
                base.Write(0);
                base.Write(chars.Length);
                base.Write(chars);
            }

            public override void Write(char[] chars, int index, int count)
            {
                base.Write(TYPE_CHAR_ARRAY);
                base.Write(index);
                base.Write(count - index);
                base.Write(chars, index, count);
            }

            public override void Write(decimal value)
            {
                base.Write(TYPE_DECIMAL);
                base.Write(value);
            }

            public override void Write(double value)
            {
                base.Write(TYPE_DOUBLE);
                base.Write(value);
            }

            public override void Write(Half value)
            {
                base.Write(TYPE_HALF);
                base.Write(value);
            }

            public override void Write(short value)
            {
                base.Write(TYPE_SHORT);
                base.Write(value);
            }

            public override void Write(int value)
            {
                base.Write(TYPE_INT);
                base.Write(value);
            }

            public override void Write(long value)
            {
                base.Write(TYPE_LONG);
                base.Write(value);
            }

            public override void Write(ReadOnlySpan<byte> buffer)
            {
                base.Write(TYPE_SPAN_BYTE);
                base.Write(buffer.Length);
                byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
                try
                {
                    buffer.CopyTo(array);
                    base.Write(array, 0, buffer.Length);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(array);
                }
            }

            public override void Write(ReadOnlySpan<char> chars)
            {
                base.Write(TYPE_SPAN_CHAR);
                base.Write(chars.Length);
                char[] array = ArrayPool<char>.Shared.Rent(chars.Length);
                try
                {
                    chars.CopyTo(array);
                    base.Write(array, 0, chars.Length);
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(array);
                }
            }

            public override void Write(sbyte value)
            {
                base.Write(TYPE_SBYTE);
                base.Write(value);
            }

            public override void Write(float value)
            {
                base.Write(TYPE_FLOAT);
                base.Write(value);
            }

            public override void Write(string value)
            {
                base.Write(TYPE_STRING);
                base.Write(value);
            }

            public override void Write(ushort value)
            {
                base.Write(TYPE_USHORT);
                base.Write(value);
            }

            public override void Write(uint value)
            {
                base.Write(TYPE_UINT);
                base.Write(value);
            }

            public override void Write(ulong value)
            {
                base.Write(TYPE_ULONG);
                base.Write(value);
            }
        }
    }
}