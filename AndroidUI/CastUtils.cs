namespace AndroidUI
{
    public static class CastUtils
    {
        private static unsafe R internal_reinterpret_cast<T, R>(T v)
            where T : unmanaged
            where R : unmanaged
        {
            return *(R*)&v;
        }

        private static unsafe R* internal_reinterpret_pointer<T, R>(T* v)
            where R : unmanaged
            where T : unmanaged
        {
            return (R*)v;
        }

        private static unsafe R[] internal_reinterpret_array<T, R>(T[] v)
            where R : unmanaged
            where T : unmanaged
        {
            return (R[])(Array)v;
        }

        /// <summary>
        /// reinterprets the given object as type R without doing value conversion
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if the given object is a Managed Type, a Pointer, or an Enum
        /// <br></br>
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        /// </summary>
        public static R reinterpret_cast<R>(object O) where R : unmanaged
        {
            Type t = O.GetType();

            return t.IsEnum
                ? throw new InvalidCastException("reinterpret_enum must be used for this type: " + t)
                : t.IsPointer
                    ? throw new InvalidCastException("reinterpret_pointer must be used for this type: " + t)
                    : O switch
                    {
                        sbyte => internal_reinterpret_cast<sbyte, R>((sbyte)O),
                        byte => internal_reinterpret_cast<byte, R>((byte)O),
                        ushort => internal_reinterpret_cast<ushort, R>((ushort)O),
                        short => internal_reinterpret_cast<short, R>((short)O),
                        int => internal_reinterpret_cast<int, R>((int)O),
                        uint => internal_reinterpret_cast<uint, R>((uint)O),
                        long => internal_reinterpret_cast<long, R>((long)O),
                        ulong => internal_reinterpret_cast<ulong, R>((ulong)O),
                        char => internal_reinterpret_cast<char, R>((char)O),
                        float => internal_reinterpret_cast<float, R>((float)O),
                        double => internal_reinterpret_cast<double, R>((double)O),
                        decimal => internal_reinterpret_cast<decimal, R>((decimal)O),
                        bool => internal_reinterpret_cast<bool, R>((bool)O),
                        _ => throw new InvalidCastException("Managed Type given: " + t)
                    };
        }

        /// <summary>
        /// reinterprets the given array object as type R[] without doing value conversion
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if the given object is a Managed Type array
        /// <br></br>
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        /// </summary>
        public static R[] reinterpret_array<T, R>(T[] O)
            where T : unmanaged
            where R : unmanaged
        {
            Type t = typeof(T);
            if (!IsUnanagedType(t))
            {
                return internal_reinterpret_array<T, R>(O);
            }
            else
            {
                throw new InvalidCastException("Managed Type given: " + t);
            }
        }

        public static readonly Type NINT = typeof(nint);
        public static readonly Type NUINT = typeof(nuint);
        public static readonly Type SBYTE = typeof(sbyte);
        public static readonly Type BYTE = typeof(byte);
        public static readonly Type BOOL = typeof(bool);
        public static readonly Type SHORT = typeof(short);
        public static readonly Type USHORT = typeof(ushort);
        public static readonly Type CHAR = typeof(char);
        public static readonly Type INT = typeof(int);
        public static readonly Type UINT = typeof(uint);
        public static readonly Type FLOAT = typeof(float);
        public static readonly Type LONG = typeof(long);
        public static readonly Type ULONG = typeof(ulong);
        public static readonly Type DOUBLE = typeof(double);
        public static readonly Type DECIMAL = typeof(decimal);

        public static bool IsUnanagedType(Type t)
        {
            return SizeOfUnmanagedType(t) != 0;
        }

        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/sizeof
        public static int SizeOfUnmanagedType(Type t)
        {
            if (t == SBYTE || t == BYTE || t == BOOL)
            {
                return 1;
            }
            else if (t == SHORT || t == USHORT || t == CHAR)
            {
                return 2;
            }
            else if (t == INT || t == UINT || t == FLOAT)
            {
                return 4;
            }
            else if (t == LONG || t == ULONG || t == DOUBLE)
            {
                return 8;
            }
            else if (t == DECIMAL)
            {
                return 16;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// reinterprets the given ENUM as type R without doing value conversion
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if ENUM is not a valid enum
        /// <br></br>
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
        /// </summary>
        public unsafe static R reinterpret_enum<ENUM, R>(ENUM E)
            where R : unmanaged
            where ENUM : unmanaged
        {
            return !E.GetType().IsEnum
                ? throw new InvalidCastException("E must be an enum")
                : internal_reinterpret_cast<ENUM, R>(E);
        }

        /// <summary>
        /// reinterprets the given POINTER as type R without doing value conversion
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if POINTER is not a Managed Type
        /// <br></br>
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code#pointer-types
        /// </summary>
        public unsafe static R* reinterpret_pointer<POINTER, R>(POINTER* P)
            where R : unmanaged
            where POINTER : unmanaged
        {
            Type t = typeof(POINTER);
            if (
                t == typeof(sbyte) || t == typeof(byte)
                || t == typeof(ushort) || t == typeof(short)
                || t == typeof(int) || t == typeof(uint)
                || t == typeof(long) || t == typeof(ulong)
                || t == typeof(char)
                || t == typeof(float) || t == typeof(double) || t == typeof(decimal)
                || t == typeof(bool))
            {
                return internal_reinterpret_pointer<POINTER, R>(P);
            }
            else
            {
                throw new InvalidCastException("Managed Type given: " + t);
            }
        }
    }
}