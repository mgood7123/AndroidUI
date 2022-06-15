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

        private static R[] internal_reinterpret_array<T, R>(T[] v)
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
            where R : unmanaged => internal_reinterpret_array<T, R>(O);

        // IMPORTANT: storing typeof in a variable prevents optimization

        public enum CompareValue
        {
            INVALID_TYPE, LESS, SAME, GREATER
        }

        /// <summary>
        /// compares a and b in a generic way, only accepts integer types
        /// </summary>
        /// <returns>
        /// CompareValue.INVALID_TYPE if either of the given values are not an integer type
        /// <br></br> CompareValue.SAME if a and b are equal
        /// <br></br> CompareValue.GREATER if a is greater than b
        /// <br></br> CompareValue.LESS if a is less than b
        /// </returns>
        public unsafe static CompareValue Compare<T1, T2>(T1 a, T2 b)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            if (typeof(T1) == typeof(nint) || typeof(T1) == typeof(nuint))
            {
                if (typeof(T2) == typeof(nint) || typeof(T2) == typeof(nuint))
                {
                    nuint v1 = *(nuint*)&a;
                    nuint v2 = *(nuint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(sbyte) || typeof(T2) == typeof(byte))
                {
                    byte v1 = (byte)*(nuint*)&a;
                    byte v2 = *(byte*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(short) || typeof(T2) == typeof(ushort))
                {
                    ushort v1 = (ushort)*(nuint*)&a;
                    ushort v2 = *(ushort*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(int) || typeof(T2) == typeof(uint))
                {
                    uint v1 = (uint)*(nuint*)&a;
                    uint v2 = *(uint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(long) || typeof(T2) == typeof(ulong))
                {
                    ulong v1 = (ulong)*(nuint*)&a;
                    ulong v2 = *(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
            }
            else if (typeof(T1) == typeof(sbyte) || typeof(T1) == typeof(byte))
            {
                if (typeof(T2) == typeof(nint) || typeof(T2) == typeof(nuint))
                {
                    byte v1 = *(byte*)&a;
                    byte v2 = (byte)*(nuint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(sbyte) || typeof(T2) == typeof(byte))
                {
                    byte v1 = *(byte*)&a;
                    byte v2 = *(byte*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(short) || typeof(T2) == typeof(ushort))
                {
                    ushort v1 = (ushort)*(byte*)&a;
                    ushort v2 = *(ushort*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(int) || typeof(T2) == typeof(uint))
                {
                    uint v1 = (uint)*(byte*)&a;
                    uint v2 = *(uint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(long) || typeof(T2) == typeof(ulong))
                {
                    ulong v1 = (ulong)*(byte*)&a;
                    ulong v2 = *(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
            }
            else if (typeof(T1) == typeof(short) || typeof(T1) == typeof(ushort))
            {
                if (typeof(T2) == typeof(nint) || typeof(T2) == typeof(nuint))
                {
                    ushort v1 = *(ushort*)&a;
                    ushort v2 = (ushort)*(nuint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(sbyte) || typeof(T2) == typeof(byte))
                {
                    ushort v1 = *(ushort*)&a;
                    ushort v2 = (ushort)*(byte*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(short) || typeof(T2) == typeof(ushort))
                {
                    ushort v1 = *(ushort*)&a;
                    ushort v2 = *(ushort*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(int) || typeof(T2) == typeof(uint))
                {
                    uint v1 = (uint)*(ushort*)&a;
                    uint v2 = *(uint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(long) || typeof(T2) == typeof(ulong))
                {
                    ulong v1 = (ulong)*(ushort*)&a;
                    ulong v2 = *(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
            }
            else if (typeof(T1) == typeof(int) || typeof(T1) == typeof(uint))
            {
                if (typeof(T2) == typeof(nint) || typeof(T2) == typeof(nuint))
                {
                    uint v1 = *(uint*)&a;
                    uint v2 = (uint)*(nuint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(sbyte) || typeof(T2) == typeof(byte))
                {
                    uint v1 = *(uint*)&a;
                    uint v2 = (uint)*(byte*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(short) || typeof(T2) == typeof(ushort))
                {
                    uint v1 = *(uint*)&a;
                    uint v2 = (uint)*(ushort*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(int) || typeof(T2) == typeof(uint))
                {
                    uint v1 = *(uint*)&a;
                    uint v2 = *(uint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(long) || typeof(T2) == typeof(ulong))
                {
                    ulong v1 = (ulong)*(uint*)&a;
                    ulong v2 = *(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
            }
            else if (typeof(T1) == typeof(int) || typeof(T1) == typeof(uint))
            {
                if (typeof(T2) == typeof(nint) || typeof(T2) == typeof(nuint))
                {
                    ulong v1 = *(ulong*)&a;
                    ulong v2 = (ulong)*(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(sbyte) || typeof(T2) == typeof(byte))
                {
                    ulong v1 = *(ulong*)&a;
                    ulong v2 = (ulong)*(byte*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(short) || typeof(T2) == typeof(ushort))
                {
                    ulong v1 = *(ulong*)&a;
                    ulong v2 = (ulong)*(ushort*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(int) || typeof(T2) == typeof(uint))
                {
                    ulong v1 = *(ulong*)&a;
                    ulong v2 = (ulong)*(uint*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
                else if (typeof(T2) == typeof(long) || typeof(T2) == typeof(ulong))
                {
                    ulong v1 = *(ulong*)&a;
                    ulong v2 = *(ulong*)&b;
                    return v1 == v2 ? CompareValue.SAME : v1 > v2 ? CompareValue.GREATER : CompareValue.LESS;
                }
            }
            return CompareValue.INVALID_TYPE;
        }

        public unsafe static T MinValue<T>()
            where T : unmanaged
        {
            if (typeof(T) == typeof(nint)) { nint v = nint.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(nuint)) { nuint v = nuint.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(sbyte)) { sbyte v = sbyte.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(byte)) { byte v = byte.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(bool)) { bool v = false; return *(T*)&v; }
            else if (typeof(T) == typeof(short)) { short v = short.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(ushort)) { ushort v = ushort.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(int)) { int v = int.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(uint)) { uint v = uint.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(long)) { long v = long.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(ulong)) { ulong v = ulong.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(char)) { char v = char.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(float)) { float v = float.MinValue; return *(T*)&v; }
            else if (typeof(T) == typeof(double)) { double v = double.MinValue; return *(T*)&v; }
            else { decimal v = decimal.MinValue; return *(T*)&v; }
        }

        public unsafe static T MaxValue<T>()
            where T : unmanaged
        {
            if (typeof(T) == typeof(nint)) { nint v = nint.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(nuint)) { nuint v = nuint.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(sbyte)) { sbyte v = sbyte.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(byte)) { byte v = byte.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(bool)) { bool v = true; return *(T*)&v; }
            else if (typeof(T) == typeof(short)) { short v = short.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(ushort)) { ushort v = ushort.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(int)) { int v = int.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(uint)) { uint v = uint.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(long)) { long v = long.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(ulong)) { ulong v = ulong.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(char)) { char v = char.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(float)) { float v = float.MaxValue; return *(T*)&v; }
            else if (typeof(T) == typeof(double)) { double v = double.MaxValue; return *(T*)&v; }
            else { decimal v = decimal.MaxValue; return *(T*)&v; }
        }

        public static bool IsUnmanagedType<T>() 
            => 
            typeof(T) == typeof(nint) || typeof(T) == typeof(nuint) || typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(byte) || typeof(T) == typeof(bool) || typeof(T) == typeof(short)
            || typeof(T) == typeof(ushort) || typeof(T) == typeof(int) || typeof(T) == typeof(uint)
            || typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(char)
            || typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal);

        /// <summary>
        /// 0 = managed
        /// <br></br>
        /// 1 = sbyte, byte, bool
        /// <br></br>
        /// 2 = short, ushort, char
        /// <br></br>
        /// 4 = int, uint, float
        /// <br></br>
        /// 8 = long, ulong, double
        /// <br></br>
        /// 16 = decimal
        /// <br></br>
        /// platform-specific size for nint and nuint
        /// <br></br>
        /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/sizeof
        /// </summary>
        public static int SizeOfUnmanagedType<T>()
        {
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(bool))
            {
                return 1;
            }
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort) || typeof(T) == typeof(char))
            {
                return 2;
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float))
            {
                return 4;
            }
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double))
            {
                return 8;
            }
            else if (typeof(T) == typeof(decimal))
            {
                return 16;
            }
            else
            {
                if (typeof(T) == typeof(nint)) unsafe { return sizeof(nint); }
                else if (typeof(T) == typeof(nuint)) unsafe { return sizeof(nuint); }
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
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types
        /// <br></br>
        /// see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code#pointer-types
        /// </summary>
        public unsafe static R* reinterpret_pointer<POINTER, R>(POINTER* P)
            where R : unmanaged
            where POINTER : unmanaged
        {
            return internal_reinterpret_pointer<POINTER, R>(P);
        }
    }
}