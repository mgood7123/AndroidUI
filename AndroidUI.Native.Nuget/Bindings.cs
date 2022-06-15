using System;
using System.Runtime.InteropServices;

namespace AndroidUI
{
    static class ImportResolver
    {
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        [System.Runtime.CompilerServices.ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
        internal static void Initialize()
        {
            NativeLibrary.SetDllImportResolver(typeof(ImportResolver).Assembly, Resolve);
        }

        static IntPtr Resolve(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == "AndroidUI.Native.dll")
            {
                if (OperatingSystem.IsWindows())
                {
                    if (Environment.Is64BitProcess)
                    {
                        libraryName = "runtimes/win-x64/native/AndroidUI.Native.Windows.dll";
                    }
                }
                else if (!OperatingSystem.IsAndroid())
                {
                    if (Environment.Is64BitProcess)
                    {
                        libraryName = "runtimes/monoandroid-x64/native/libAndroidUI_Native_Android.so";
                    }
                }
                // set libraryName to the appropriate one for the platform
            }

            return NativeLibrary.Load(libraryName, assembly, searchPath);
        }
    }

    public static class Native
    {
        internal class Pointer : SafeHandle
        {
            private Action<IntPtr> dispose;
            private IntPtr invalid;

            public unsafe Pointer(void* handle, Action<IntPtr> dispose) : this((IntPtr)handle, dispose) { }
            public Pointer(IntPtr handle, Action<IntPtr> dispose) : this(handle, dispose, IntPtr.Zero, true) { }
            public Pointer(IntPtr handle, Action<IntPtr> dispose, bool ownsHandle) : this(handle, dispose, IntPtr.Zero, ownsHandle) { }
            private Pointer(IntPtr handle, Action<IntPtr> dispose, IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
            {
                SetHandle(handle);
                this.dispose = dispose;
                invalid = invalidHandleValue;
            }

            public unsafe bool PointerEquals(IntPtr a, IntPtr b) => a.ToPointer() == b.ToPointer();

            public unsafe bool PointerEquals(void* a, void* b) => a == b;

            public unsafe void* ToPointer() => handle.ToPointer();

            public override bool IsInvalid => PointerEquals(handle, invalid);

            protected override bool ReleaseHandle()
            {
                dispose.Invoke(handle);
                handle = invalid;
                return true;
            }

            public unsafe static implicit operator void*(Pointer a) => a.ToPointer();
        }

        public unsafe class Additional {
            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern bool SkNinePatchGlue_isNinePatchChunk(sbyte* array, int length);

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern sbyte* SkNinePatchGlue_validateNinePatchChunk(sbyte* array, int length);

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_finalize(sbyte* patch);

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern bool SkNinePatchGlue_ReadChunk(
                // ReadChunk
                void* tag, void* data, IntPtr length,
                // NPatch
                void** mPatch, nuint* mPatchSize, bool* mHasInsets,
                int** mOpticalInsets, int** mOutlineInsets,
                float* mOutlineRadius, byte* mOutlineAlpha
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_delete(
                void* mPatch
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getPadding(
                void* mPatch, int** outPadding
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getNumXDivs(
                void* mPatch, byte* outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getNumYDivs(
                void* mPatch, byte* outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getNumColors(
                void* mPatch, byte* outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getXDivs(
                void* mPatch, int** outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getYDivs(
                void* mPatch, int** outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_getColors(
                void* mPatch, uint** outValue
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern void SkNinePatchGlue_scale(
                void* mPatch,
                float scaleX, float scaleY, int scaledWidth, int scaledHeight
            );

            [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            public static extern nuint SkNinePatchGlue_serializedSize(void* mPatch);
        }

        /// <summary>
        /// calls the C api `memcpy`
        /// </summary>
        /// <param name="dst">destination</param>
        /// <param name="src">source</param>
        /// <param name="length">length</param>
        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_memcpy")]
        public static unsafe extern void Memcpy(void* dst, void* src, nuint length);

        /// <summary>
        /// calls the C api `memcpy`
        /// </summary>
        public static unsafe void Memcpy(IntPtr dst, IntPtr src, nuint length)
        {
            Memcpy((void*)dst, (void*)src, length);
        }

        /// <summary>
        /// calls the C api `memcpy`, the input src and dst arrays are not copied
        /// </summary>
        /// <param name="dst">destination</param>
        /// <param name="src">source</param>
        /// <param name="length">length</param>
        public static unsafe void Memcpy<T1, T2>(T1[] dst, T2[] src, nuint length)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            fixed (void* destination = dst)
            fixed (void* source = src)
            {
                Memcpy(destination, source, length);
            }
        }

        /// <summary>
        /// calls the C api `memcmp`
        /// </summary>
        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_memcmp")]
        public static unsafe extern int Memcmp(void* buf1, void* buf2, nuint length);

        /// <summary>
        /// calls the C api `memcmp`
        /// </summary>
        public static unsafe void Memcmp(IntPtr buf1, IntPtr buf2, nuint length)
        {
            Memcmp((void*)buf1, (void*)buf2, length);
        }

        /// <summary>
        /// calls the C api `memcmp`, the input buf1 and buf2 arrays are not copied
        /// </summary>
        public static unsafe void Memcmp<T1, T2>(T1[] buf1, T2[] buf2, nuint length)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            fixed (void* bufferA = buf1)
            fixed (void* bufferB = buf2)
            {
                Memcmp(bufferA, bufferB, length);
            }
        }

        /// <summary>
        /// calls the C api `memset`
        /// </summary>
        /// <returns>the input pointer</returns>
        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_memset")]
        public static unsafe extern void* Memset(void* buf, int value, nuint length);

        /// <summary>
        /// calls the C api `memset`
        /// </summary>
        /// <returns>the input pointer</returns>
        public static unsafe IntPtr Memset(IntPtr buf, int value, nuint length)
        {
            return (IntPtr)Memset((void*)buf, value, length);
        }

        /// <summary>
        /// calls the C api `memset`, the input array is not copied
        /// </summary>
        /// <returns>the input array</returns>
        public static unsafe T1[] Memset<T1>(T1[] buf, int value, nuint length)
            where T1 : unmanaged
        {
            fixed (void* bufferA = buf)
            {
                Memset(bufferA, value, length);
            }
            return buf;
        }

        public static unsafe Sk2f fma(Sk2f a, Sk2f b, Sk2f c)
        {
            return new Sk2f(
                Bindings.Native.Sk2f__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk4f fma(Sk4f a, Sk4f b, Sk4f c)
        {
            return new Sk4f(
                Bindings.Native.Sk4f__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8f fma(Sk8f a, Sk8f b, Sk8f c)
        {
            return new Sk8f(
                Bindings.Native.Sk8f__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk16f fma(Sk16f a, Sk16f b, Sk16f c)
        {
            return new Sk16f(
                Bindings.Native.Sk16f__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk4f join(Sk2f a, Sk2f b)
        {
            return new Sk4f(
                Bindings.Native.Sk4f__join(a._native, b._native)
            );
        }

        public static unsafe Sk8f join(Sk4f a, Sk4f b)
        {
            return new Sk8f(
                Bindings.Native.Sk8f__join(a._native, b._native)
            );
        }

        public static unsafe Sk16f join(Sk8f a, Sk8f b)
        {
            return new Sk16f(
                Bindings.Native.Sk16f__join(a._native, b._native)
            );
        }

        public unsafe class Sk2f
        {
            internal Pointer _native;
            public Sk2f()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2f__0(),
                    h => Bindings.Native.delete_Sk2f(h.ToPointer())
                );
            }

            internal unsafe Sk2f(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk2f(h.ToPointer())
                );
            }

            public Sk2f(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2f__1(a),
                    h => Bindings.Native.delete_Sk2f(h.ToPointer())
                );
            }

            public Sk2f(float a, float b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2f__2(a, b),
                    h => Bindings.Native.delete_Sk2f(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk2f__index(_native, index);
                }
            }

            static public Sk2f Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk2f(Bindings.Native.Sk2f__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk2f__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk2f a, out Sk2f b, out Sk2f c, out Sk2f d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk2f a, out Sk2f b, out Sk2f c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk2f a, out Sk2f b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk2f a, Sk2f b, Sk2f c, Sk2f d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk2f a, Sk2f b, Sk2f c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk2f a, Sk2f b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2f__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk2f operator +(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_add(left._native, right._native)
                );
            }

            public static Sk2f operator -(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_subtract(left._native, right._native)
                );
            }

            public static Sk2f operator *(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_multiply(left._native, right._native)
                );
            }

            public static Sk2f operator /(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_divide(left._native, right._native)
                );
            }

            public static Sk2f operator &(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk2f operator |(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk2f operator ^(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk2f operator ==(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk2f operator !=(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk2f operator <=(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk2f operator >=(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk2f operator <(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than(left._native, right._native)
                );
            }

            public static Sk2f operator >(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk2f Min(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__Min(left._native, right._native)
                );
            }

            public static Sk2f Max(Sk2f left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__Max(left._native, right._native)
                );
            }

            public Sk2f thenElse(Sk2f a, Sk2f b)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk2f operator +(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator -(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator *(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator /(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator &(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator |(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator ^(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator ==(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator !=(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator <=(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator >=(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator <(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator >(Sk2f left, float right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk2f operator +(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator -(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator *(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator /(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator &(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator |(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator ^(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator ==(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator !=(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator <=(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator >=(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator <(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk2f operator >(float left, Sk2f right)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk2f shuffle(int a, int b)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__suffle2(_native, a, b)
                );
            }

            public Sk4f shuffle(int a, int b, int c, int d)
            {
                return new Sk4f(
                    Bindings.Native.Sk2f__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8f shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8f(
                    Bindings.Native.Sk2f__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16f shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16f(
                    Bindings.Native.Sk2f__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk2f__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk2f__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk2f__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk2f__allTrue(_native) != 0;
            }

            public static Sk2f operator !(Sk2f left)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_logical_not(left._native)
                );
            }

            public static Sk2f operator ~(Sk2f left)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk2f operator -(Sk2f left)
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__operator_unary_minus(left._native)
                );
            }

            public Sk2f Abs()
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__abs(_native)
                );
            }

            public Sk2f Sqrt()
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__sqrt(_native)
                );
            }

            public Sk2f Floor()
            {
                return new Sk2f(
                    Bindings.Native.Sk2f__floor(_native)
                );
            }
        }

        public unsafe class Sk4f
        {
            internal Pointer _native;
            public Sk4f()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4f__0(),
                    h => Bindings.Native.delete_Sk4f(h.ToPointer())
                );
            }

            internal unsafe Sk4f(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4f(h.ToPointer())
                );
            }

            public Sk4f(Sk2f a, Sk2f b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4f__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk4f(h.ToPointer())
                );
            }

            public Sk4f(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4f__1(a),
                    h => Bindings.Native.delete_Sk4f(h.ToPointer())
                );
            }

            public Sk4f(float a, float b, float c, float d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4f__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4f(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4f__index(_native, index);
                }
            }

            static public Sk4f Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk4f(Bindings.Native.Sk4f__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4f__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk4f a, out Sk4f b, out Sk4f c, out Sk4f d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk4f a, out Sk4f b, out Sk4f c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk4f a, out Sk4f b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk4f a, Sk4f b, Sk4f c, Sk4f d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk4f a, Sk4f b, Sk4f c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk4f a, Sk4f b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4f__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4f operator +(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_add(left._native, right._native)
                );
            }

            public static Sk4f operator -(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4f operator *(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4f operator /(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_divide(left._native, right._native)
                );
            }

            public static Sk4f operator &(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4f operator |(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4f operator ^(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4f operator ==(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4f operator !=(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4f operator <=(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4f operator >=(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4f operator <(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4f operator >(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4f Min(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__Min(left._native, right._native)
                );
            }

            public static Sk4f Max(Sk4f left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__Max(left._native, right._native)
                );
            }

            public Sk4f thenElse(Sk4f a, Sk4f b)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4f operator +(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator -(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator *(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator /(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator &(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator |(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator ^(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator ==(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator !=(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator <=(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator >=(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator <(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator >(Sk4f left, float right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4f operator +(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator -(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator *(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator /(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator &(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator |(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator ^(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator ==(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator !=(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator <=(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator >=(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator <(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4f operator >(float left, Sk4f right)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk2f b, Sk2f c)
            {
                Bindings.Native.Sk4f__split(_native, b._native, c._native);
            }

            public Sk2f shuffle(int a, int b)
            {
                return new Sk2f(
                    Bindings.Native.Sk4f__suffle2(_native, a, b)
                );
            }

            public Sk4f shuffle(int a, int b, int c, int d)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8f shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8f(
                    Bindings.Native.Sk4f__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16f shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16f(
                    Bindings.Native.Sk4f__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk4f__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk4f__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4f__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4f__allTrue(_native) != 0;
            }

            public static Sk4f operator !(Sk4f left)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_logical_not(left._native)
                );
            }

            public static Sk4f operator ~(Sk4f left)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk4f operator -(Sk4f left)
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__operator_unary_minus(left._native)
                );
            }

            public Sk4f Abs()
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__abs(_native)
                );
            }

            public Sk4f Sqrt()
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__sqrt(_native)
                );
            }

            public Sk4f Floor()
            {
                return new Sk4f(
                    Bindings.Native.Sk4f__floor(_native)
                );
            }
        }

        public unsafe class Sk8f
        {
            internal Pointer _native;
            public Sk8f()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8f__0(),
                    h => Bindings.Native.delete_Sk8f(h.ToPointer())
                );
            }

            internal unsafe Sk8f(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk8f(h.ToPointer())
                );
            }

            public Sk8f(Sk4f a, Sk4f b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8f__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk8f(h.ToPointer())
                );
            }

            public Sk8f(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8f__1(a),
                    h => Bindings.Native.delete_Sk8f(h.ToPointer())
                );
            }

            public Sk8f(float a, float b, float c, float d, float e, float f, float g, float h)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8f__8(a, b, c, d, e, f, g, h),
                    h => Bindings.Native.delete_Sk8f(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk8f__index(_native, index);
                }
            }

            static public Sk8f Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk8f(Bindings.Native.Sk8f__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk8f__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk8f a, out Sk8f b, out Sk8f c, out Sk8f d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk8f a, out Sk8f b, out Sk8f c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk8f a, out Sk8f b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk8f a, Sk8f b, Sk8f c, Sk8f d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk8f a, Sk8f b, Sk8f c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk8f a, Sk8f b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8f__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk8f operator +(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_add(left._native, right._native)
                );
            }

            public static Sk8f operator -(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_subtract(left._native, right._native)
                );
            }

            public static Sk8f operator *(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_multiply(left._native, right._native)
                );
            }

            public static Sk8f operator /(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_divide(left._native, right._native)
                );
            }

            public static Sk8f operator &(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk8f operator |(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk8f operator ^(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk8f operator ==(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk8f operator !=(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk8f operator <=(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8f operator >=(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8f operator <(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than(left._native, right._native)
                );
            }

            public static Sk8f operator >(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk8f Min(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__Min(left._native, right._native)
                );
            }

            public static Sk8f Max(Sk8f left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__Max(left._native, right._native)
                );
            }

            public Sk8f thenElse(Sk8f a, Sk8f b)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk8f operator +(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator -(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator *(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator /(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator &(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator |(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator ^(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator ==(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator !=(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator <=(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator >=(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator <(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator >(Sk8f left, float right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8f operator +(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator -(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator *(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator /(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator &(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator |(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator ^(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator ==(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator !=(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator <=(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator >=(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator <(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk8f operator >(float left, Sk8f right)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk4f b, Sk4f c)
            {
                Bindings.Native.Sk8f__split(_native, b._native, c._native);
            }

            public Sk2f shuffle(int a, int b)
            {
                return new Sk2f(
                    Bindings.Native.Sk8f__suffle2(_native, a, b)
                );
            }

            public Sk4f shuffle(int a, int b, int c, int d)
            {
                return new Sk4f(
                    Bindings.Native.Sk8f__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8f shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16f shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16f(
                    Bindings.Native.Sk8f__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk8f__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk8f__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk8f__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk8f__allTrue(_native) != 0;
            }

            public static Sk8f operator !(Sk8f left)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_logical_not(left._native)
                );
            }

            public static Sk8f operator ~(Sk8f left)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk8f operator -(Sk8f left)
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__operator_unary_minus(left._native)
                );
            }

            public Sk8f Abs()
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__abs(_native)
                );
            }

            public Sk8f Sqrt()
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__sqrt(_native)
                );
            }

            public Sk8f Floor()
            {
                return new Sk8f(
                    Bindings.Native.Sk8f__floor(_native)
                );
            }
        }

        public unsafe class Sk16f
        {
            internal Pointer _native;
            public Sk16f()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16f__0(),
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
                );
            }

            internal unsafe Sk16f(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
                );
            }

            public Sk16f(Sk8f a, Sk8f b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16f__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
                );
            }

            public Sk16f(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16f__1(a),
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
                );
            }

            public Sk16f(float a, float b, float c, float d, float e, float f, float g, float h, float i, float j, float k, float l, float m, float n, float o, float p)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16f__16(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p),
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk16f__index(_native, index);
                }
            }

            static public Sk16f Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk16f(Bindings.Native.Sk16f__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk16f__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk16f a, out Sk16f b, out Sk16f c, out Sk16f d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk16f a, out Sk16f b, out Sk16f c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk16f a, out Sk16f b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk16f a, Sk16f b, Sk16f c, Sk16f d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk16f a, Sk16f b, Sk16f c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk16f a, Sk16f b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16f__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk16f operator +(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_add(left._native, right._native)
                );
            }

            public static Sk16f operator -(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_subtract(left._native, right._native)
                );
            }

            public static Sk16f operator *(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_multiply(left._native, right._native)
                );
            }

            public static Sk16f operator /(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_divide(left._native, right._native)
                );
            }

            public static Sk16f operator &(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk16f operator |(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk16f operator ^(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk16f operator ==(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk16f operator !=(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk16f operator <=(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16f operator >=(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16f operator <(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than(left._native, right._native)
                );
            }

            public static Sk16f operator >(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk16f Min(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__Min(left._native, right._native)
                );
            }

            public static Sk16f Max(Sk16f left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__Max(left._native, right._native)
                );
            }

            public Sk16f thenElse(Sk16f a, Sk16f b)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk16f operator +(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator -(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator *(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator /(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator &(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator |(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator ^(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator ==(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator !=(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator <=(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator >=(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator <(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator >(Sk16f left, float right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16f operator +(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator -(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator *(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator /(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator &(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator |(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator ^(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator ==(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator !=(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator <=(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator >=(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator <(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk16f operator >(float left, Sk16f right)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk8f b, Sk8f c)
            {
                Bindings.Native.Sk16f__split(_native, b._native, c._native);
            }

            public Sk2f shuffle(int a, int b)
            {
                return new Sk2f(
                    Bindings.Native.Sk16f__suffle2(_native, a, b)
                );
            }

            public Sk4f shuffle(int a, int b, int c, int d)
            {
                return new Sk4f(
                    Bindings.Native.Sk16f__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8f shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8f(
                    Bindings.Native.Sk16f__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16f shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk16f__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk16f__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk16f__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk16f__allTrue(_native) != 0;
            }

            public static Sk16f operator !(Sk16f left)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_logical_not(left._native)
                );
            }

            public static Sk16f operator ~(Sk16f left)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk16f operator -(Sk16f left)
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__operator_unary_minus(left._native)
                );
            }

            public Sk16f Abs()
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__abs(_native)
                );
            }

            public Sk16f Sqrt()
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__sqrt(_native)
                );
            }

            public Sk16f Floor()
            {
                return new Sk16f(
                    Bindings.Native.Sk16f__floor(_native)
                );
            }
        }

        public static unsafe Sk2s fma(Sk2s a, Sk2s b, Sk2s c)
        {
            return new Sk2s(
                Bindings.Native.Sk2s__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk4s fma(Sk4s a, Sk4s b, Sk4s c)
        {
            return new Sk4s(
                Bindings.Native.Sk4s__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8s fma(Sk8s a, Sk8s b, Sk8s c)
        {
            return new Sk8s(
                Bindings.Native.Sk8s__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk16s fma(Sk16s a, Sk16s b, Sk16s c)
        {
            return new Sk16s(
                Bindings.Native.Sk16s__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk4s join(Sk2s a, Sk2s b)
        {
            return new Sk4s(
                Bindings.Native.Sk4s__join(a._native, b._native)
            );
        }

        public static unsafe Sk8s join(Sk4s a, Sk4s b)
        {
            return new Sk8s(
                Bindings.Native.Sk8s__join(a._native, b._native)
            );
        }

        public static unsafe Sk16s join(Sk8s a, Sk8s b)
        {
            return new Sk16s(
                Bindings.Native.Sk16s__join(a._native, b._native)
            );
        }

        public unsafe class Sk2s
        {
            internal Pointer _native;
            public Sk2s()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2s__0(),
                    h => Bindings.Native.delete_Sk2s(h.ToPointer())
                );
            }

            internal unsafe Sk2s(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk2s(h.ToPointer())
                );
            }

            public Sk2s(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2s__1(a),
                    h => Bindings.Native.delete_Sk2s(h.ToPointer())
                );
            }

            public Sk2s(float a, float b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk2s__2(a, b),
                    h => Bindings.Native.delete_Sk2s(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk2s__index(_native, index);
                }
            }

            static public Sk2s Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk2s(Bindings.Native.Sk2s__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk2s__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk2s a, out Sk2s b, out Sk2s c, out Sk2s d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk2s a, out Sk2s b, out Sk2s c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk2s a, out Sk2s b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk2s a, Sk2s b, Sk2s c, Sk2s d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk2s a, Sk2s b, Sk2s c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk2s a, Sk2s b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk2s__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk2s operator +(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_add(left._native, right._native)
                );
            }

            public static Sk2s operator -(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_subtract(left._native, right._native)
                );
            }

            public static Sk2s operator *(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_multiply(left._native, right._native)
                );
            }

            public static Sk2s operator /(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_divide(left._native, right._native)
                );
            }

            public static Sk2s operator &(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk2s operator |(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk2s operator ^(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk2s operator ==(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk2s operator !=(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk2s operator <=(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk2s operator >=(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk2s operator <(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than(left._native, right._native)
                );
            }

            public static Sk2s operator >(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk2s Min(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__Min(left._native, right._native)
                );
            }

            public static Sk2s Max(Sk2s left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__Max(left._native, right._native)
                );
            }

            public Sk2s thenElse(Sk2s a, Sk2s b)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk2s operator +(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator -(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator *(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator /(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator &(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator |(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator ^(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator ==(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator !=(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator <=(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator >=(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator <(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator >(Sk2s left, float right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk2s operator +(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator -(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator *(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator /(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator &(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator |(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator ^(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator ==(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator !=(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator <=(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator >=(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator <(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk2s operator >(float left, Sk2s right)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk2s shuffle(int a, int b)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__suffle2(_native, a, b)
                );
            }

            public Sk4s shuffle(int a, int b, int c, int d)
            {
                return new Sk4s(
                    Bindings.Native.Sk2s__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8s shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8s(
                    Bindings.Native.Sk2s__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16s shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16s(
                    Bindings.Native.Sk2s__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk2s__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk2s__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk2s__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk2s__allTrue(_native) != 0;
            }

            public static Sk2s operator !(Sk2s left)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_logical_not(left._native)
                );
            }

            public static Sk2s operator ~(Sk2s left)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk2s operator -(Sk2s left)
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__operator_unary_minus(left._native)
                );
            }

            public Sk2s Abs()
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__abs(_native)
                );
            }

            public Sk2s Sqrt()
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__sqrt(_native)
                );
            }

            public Sk2s Floor()
            {
                return new Sk2s(
                    Bindings.Native.Sk2s__floor(_native)
                );
            }
        }

        public unsafe class Sk4s
        {
            internal Pointer _native;
            public Sk4s()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4s__0(),
                    h => Bindings.Native.delete_Sk4s(h.ToPointer())
                );
            }

            internal unsafe Sk4s(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4s(h.ToPointer())
                );
            }

            public Sk4s(Sk2s a, Sk2s b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4s__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk4s(h.ToPointer())
                );
            }

            public Sk4s(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4s__1(a),
                    h => Bindings.Native.delete_Sk4s(h.ToPointer())
                );
            }

            public Sk4s(float a, float b, float c, float d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4s__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4s(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4s__index(_native, index);
                }
            }

            static public Sk4s Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk4s(Bindings.Native.Sk4s__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4s__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk4s a, out Sk4s b, out Sk4s c, out Sk4s d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk4s a, out Sk4s b, out Sk4s c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk4s a, out Sk4s b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk4s a, Sk4s b, Sk4s c, Sk4s d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk4s a, Sk4s b, Sk4s c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk4s a, Sk4s b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk4s__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4s operator +(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_add(left._native, right._native)
                );
            }

            public static Sk4s operator -(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4s operator *(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4s operator /(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_divide(left._native, right._native)
                );
            }

            public static Sk4s operator &(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4s operator |(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4s operator ^(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4s operator ==(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4s operator !=(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4s operator <=(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4s operator >=(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4s operator <(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4s operator >(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4s Min(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__Min(left._native, right._native)
                );
            }

            public static Sk4s Max(Sk4s left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__Max(left._native, right._native)
                );
            }

            public Sk4s thenElse(Sk4s a, Sk4s b)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4s operator +(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator -(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator *(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator /(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator &(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator |(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator ^(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator ==(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator !=(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator <=(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator >=(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator <(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator >(Sk4s left, float right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4s operator +(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator -(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator *(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator /(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator &(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator |(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator ^(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator ==(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator !=(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator <=(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator >=(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator <(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4s operator >(float left, Sk4s right)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk2s b, Sk2s c)
            {
                Bindings.Native.Sk4s__split(_native, b._native, c._native);
            }

            public Sk2s shuffle(int a, int b)
            {
                return new Sk2s(
                    Bindings.Native.Sk4s__suffle2(_native, a, b)
                );
            }

            public Sk4s shuffle(int a, int b, int c, int d)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8s shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8s(
                    Bindings.Native.Sk4s__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16s shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16s(
                    Bindings.Native.Sk4s__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk4s__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk4s__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4s__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4s__allTrue(_native) != 0;
            }

            public static Sk4s operator !(Sk4s left)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_logical_not(left._native)
                );
            }

            public static Sk4s operator ~(Sk4s left)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk4s operator -(Sk4s left)
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__operator_unary_minus(left._native)
                );
            }

            public Sk4s Abs()
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__abs(_native)
                );
            }

            public Sk4s Sqrt()
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__sqrt(_native)
                );
            }

            public Sk4s Floor()
            {
                return new Sk4s(
                    Bindings.Native.Sk4s__floor(_native)
                );
            }
        }

        public unsafe class Sk8s
        {
            internal Pointer _native;
            public Sk8s()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8s__0(),
                    h => Bindings.Native.delete_Sk8s(h.ToPointer())
                );
            }

            internal unsafe Sk8s(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk8s(h.ToPointer())
                );
            }

            public Sk8s(Sk4s a, Sk4s b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8s__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk8s(h.ToPointer())
                );
            }

            public Sk8s(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8s__1(a),
                    h => Bindings.Native.delete_Sk8s(h.ToPointer())
                );
            }

            public Sk8s(float a, float b, float c, float d, float e, float f, float g, float h)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8s__8(a, b, c, d, e, f, g, h),
                    h => Bindings.Native.delete_Sk8s(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk8s__index(_native, index);
                }
            }

            static public Sk8s Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk8s(Bindings.Native.Sk8s__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk8s__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk8s a, out Sk8s b, out Sk8s c, out Sk8s d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk8s a, out Sk8s b, out Sk8s c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk8s a, out Sk8s b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk8s a, Sk8s b, Sk8s c, Sk8s d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk8s a, Sk8s b, Sk8s c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk8s a, Sk8s b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk8s__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk8s operator +(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_add(left._native, right._native)
                );
            }

            public static Sk8s operator -(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_subtract(left._native, right._native)
                );
            }

            public static Sk8s operator *(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_multiply(left._native, right._native)
                );
            }

            public static Sk8s operator /(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_divide(left._native, right._native)
                );
            }

            public static Sk8s operator &(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk8s operator |(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk8s operator ^(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk8s operator ==(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk8s operator !=(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk8s operator <=(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8s operator >=(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8s operator <(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than(left._native, right._native)
                );
            }

            public static Sk8s operator >(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk8s Min(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__Min(left._native, right._native)
                );
            }

            public static Sk8s Max(Sk8s left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__Max(left._native, right._native)
                );
            }

            public Sk8s thenElse(Sk8s a, Sk8s b)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk8s operator +(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator -(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator *(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator /(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator &(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator |(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator ^(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator ==(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator !=(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator <=(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator >=(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator <(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator >(Sk8s left, float right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8s operator +(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator -(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator *(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator /(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator &(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator |(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator ^(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator ==(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator !=(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator <=(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator >=(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator <(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk8s operator >(float left, Sk8s right)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk4s b, Sk4s c)
            {
                Bindings.Native.Sk8s__split(_native, b._native, c._native);
            }

            public Sk2s shuffle(int a, int b)
            {
                return new Sk2s(
                    Bindings.Native.Sk8s__suffle2(_native, a, b)
                );
            }

            public Sk4s shuffle(int a, int b, int c, int d)
            {
                return new Sk4s(
                    Bindings.Native.Sk8s__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8s shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16s shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16s(
                    Bindings.Native.Sk8s__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk8f__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk8s__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk8s__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk8s__allTrue(_native) != 0;
            }

            public static Sk8s operator !(Sk8s left)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_logical_not(left._native)
                );
            }

            public static Sk8s operator ~(Sk8s left)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk8s operator -(Sk8s left)
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__operator_unary_minus(left._native)
                );
            }

            public Sk8s Abs()
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__abs(_native)
                );
            }

            public Sk8s Sqrt()
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__sqrt(_native)
                );
            }

            public Sk8s Floor()
            {
                return new Sk8s(
                    Bindings.Native.Sk8s__floor(_native)
                );
            }
        }

        public unsafe class Sk16s
        {
            internal Pointer _native;
            public Sk16s()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16s__0(),
                    h => Bindings.Native.delete_Sk16s(h.ToPointer())
                );
            }

            internal unsafe Sk16s(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk16s(h.ToPointer())
                );
            }

            public Sk16s(Sk8s a, Sk8s b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16s__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk16s(h.ToPointer())
                );
            }

            public Sk16s(float a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16s__1(a),
                    h => Bindings.Native.delete_Sk16s(h.ToPointer())
                );
            }

            public Sk16s(float a, float b, float c, float d, float e, float f, float g, float h, float i, float j, float k, float l, float m, float n, float o, float p)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16s__16(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p),
                    h => Bindings.Native.delete_Sk16s(h.ToPointer())
                );
            }

            public float this[int index]
            {
                get
                {
                    return Bindings.Native.Sk16s__index(_native, index);
                }
            }

            static public Sk16s Load(float[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        return new Sk16s(Bindings.Native.Sk16s__Load(ptr));
                    }
                }
            }

            public float[] Store()
            {
                float[] array = new float[2];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk16s__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(float[] array, out Sk16s a, out Sk16s b, out Sk16s c, out Sk16s d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(float[] array, out Sk16s a, out Sk16s b, out Sk16s c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(float[] array, out Sk16s a, out Sk16s b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public float[] Store4(Sk16s a, Sk16s b, Sk16s c, Sk16s d)
            {
                float[] array = new float[8];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public float[] Store3(Sk16s a, Sk16s b, Sk16s c)
            {
                float[] array = new float[6];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public float[] Store2(Sk16s a, Sk16s b)
            {
                float[] array = new float[4];
                unsafe
                {
                    fixed (float* ptr = array)
                    {
                        Bindings.Native.Sk16s__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk16s operator +(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_add(left._native, right._native)
                );
            }

            public static Sk16s operator -(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_subtract(left._native, right._native)
                );
            }

            public static Sk16s operator *(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_multiply(left._native, right._native)
                );
            }

            public static Sk16s operator /(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_divide(left._native, right._native)
                );
            }

            public static Sk16s operator &(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk16s operator |(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk16s operator ^(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk16s operator ==(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk16s operator !=(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk16s operator <=(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16s operator >=(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16s operator <(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than(left._native, right._native)
                );
            }

            public static Sk16s operator >(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk16s Min(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__Min(left._native, right._native)
                );
            }

            public static Sk16s Max(Sk16s left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__Max(left._native, right._native)
                );
            }

            public Sk16s thenElse(Sk16s a, Sk16s b)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk16s operator +(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator -(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator *(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator /(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator &(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator |(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator ^(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator ==(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator !=(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator <=(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator >=(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator <(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator >(Sk16s left, float right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16s operator +(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator -(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator *(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator /(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator &(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator |(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator ^(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator ==(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator !=(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator <=(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator >=(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator <(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk16s operator >(float left, Sk16s right)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk8s b, Sk8s c)
            {
                Bindings.Native.Sk16s__split(_native, b._native, c._native);
            }

            public Sk2s shuffle(int a, int b)
            {
                return new Sk2s(
                    Bindings.Native.Sk16s__suffle2(_native, a, b)
                );
            }

            public Sk4s shuffle(int a, int b, int c, int d)
            {
                return new Sk4s(
                    Bindings.Native.Sk16s__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8s shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8s(
                    Bindings.Native.Sk16s__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16s shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public float min()
            {
                return Bindings.Native.Sk16s__min(_native);
            }

            public float max()
            {
                return Bindings.Native.Sk16s__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk16s__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk16s__allTrue(_native) != 0;
            }

            public static Sk16s operator !(Sk16s left)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_logical_not(left._native)
                );
            }

            public static Sk16s operator ~(Sk16s left)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_binary_ones_complement(left._native)
                );
            }

            public static Sk16s operator -(Sk16s left)
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__operator_unary_minus(left._native)
                );
            }

            public Sk16s Abs()
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__abs(_native)
                );
            }

            public Sk16s Sqrt()
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__sqrt(_native)
                );
            }

            public Sk16s Floor()
            {
                return new Sk16s(
                    Bindings.Native.Sk16s__floor(_native)
                );
            }
        }

        public static unsafe Sk4b fma(Sk4b a, Sk4b b, Sk4b c)
        {
            return new Sk4b(
                Bindings.Native.Sk4b__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8b fma(Sk8b a, Sk8b b, Sk8b c)
        {
            return new Sk8b(
                Bindings.Native.Sk8b__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk16b fma(Sk16b a, Sk16b b, Sk16b c)
        {
            return new Sk16b(
                Bindings.Native.Sk16b__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8b join(Sk4b a, Sk4b b)
        {
            return new Sk8b(
                Bindings.Native.Sk8b__join(a._native, b._native)
            );
        }

        public static unsafe Sk16b join(Sk8b a, Sk8b b)
        {
            return new Sk16b(
                Bindings.Native.Sk16b__join(a._native, b._native)
            );
        }

        public unsafe class Sk4b
        {
            internal Pointer _native;
            public Sk4b()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4b__0(),
                    h => Bindings.Native.delete_Sk4b(h.ToPointer())
                );
            }

            internal unsafe Sk4b(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4b(h.ToPointer())
                );
            }

            public Sk4b(byte a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4b__1(a),
                    h => Bindings.Native.delete_Sk4b(h.ToPointer())
                );
            }

            public Sk4b(byte a, byte b, byte c, byte d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4b__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4b(h.ToPointer())
                );
            }

            public byte this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4b__index(_native, index);
                }
            }

            static public Sk4b Load(byte[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        return new Sk4b(Bindings.Native.Sk4b__Load(ptr));
                    }
                }
            }

            public byte[] Store()
            {
                byte[] array = new byte[2];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4b__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(byte[] array, out Sk4b a, out Sk4b b, out Sk4b c, out Sk4b d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(byte[] array, out Sk4b a, out Sk4b b, out Sk4b c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(byte[] array, out Sk4b a, out Sk4b b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public byte[] Store4(Sk4b a, Sk4b b, Sk4b c, Sk4b d)
            {
                byte[] array = new byte[8];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public byte[] Store3(Sk4b a, Sk4b b, Sk4b c)
            {
                byte[] array = new byte[6];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public byte[] Store2(Sk4b a, Sk4b b)
            {
                byte[] array = new byte[4];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk4b__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4b operator +(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_add(left._native, right._native)
                );
            }

            public static Sk4b operator -(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4b operator *(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4b operator /(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_divide(left._native, right._native)
                );
            }

            public static Sk4b operator &(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4b operator |(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4b operator ^(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4b operator ==(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4b operator !=(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4b operator <=(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4b operator >=(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4b operator <(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4b operator >(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4b Min(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__Min(left._native, right._native)
                );
            }

            public static Sk4b Max(Sk4b left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__Max(left._native, right._native)
                );
            }

            public Sk4b thenElse(Sk4b a, Sk4b b)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4b operator +(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator -(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator *(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator /(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator &(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator |(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator ^(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator ==(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator !=(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator <=(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator >=(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator <(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator >(Sk4b left, byte right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4b operator +(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator -(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator *(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator /(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator &(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator |(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator ^(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator ==(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator !=(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator <=(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator >=(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator <(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4b operator >(byte left, Sk4b right)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk4b shuffle(int a, int b, int c, int d)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8b shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8b(
                    Bindings.Native.Sk4b__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16b shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16b(
                    Bindings.Native.Sk4b__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk4b SaturatedAdd(Sk4b value)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__saturatedAdd(_native, value._native)
                );
            }

            public Sk4b MulHi(Sk4b value)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__mulHi(_native, value._native)
                );
            }

            public static Sk4b operator <<(Sk4b left, int value)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk4b operator >>(Sk4b left, int value)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public byte min()
            {
                return Bindings.Native.Sk4b__min(_native);
            }

            public byte max()
            {
                return Bindings.Native.Sk4b__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4b__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4b__allTrue(_native) != 0;
            }

            public static Sk4b operator !(Sk4b left)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_logical_not(left._native)
                );
            }

            public static Sk4b operator ~(Sk4b left)
            {
                return new Sk4b(
                    Bindings.Native.Sk4b__operator_binary_ones_complement(left._native)
                );
            }

        }

        public unsafe class Sk8b
        {
            internal Pointer _native;
            public Sk8b()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8b__0(),
                    h => Bindings.Native.delete_Sk8b(h.ToPointer())
                );
            }

            internal unsafe Sk8b(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk8b(h.ToPointer())
                );
            }

            public Sk8b(Sk4b a, Sk4b b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8b__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk8b(h.ToPointer())
                );
            }

            public Sk8b(byte a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8b__1(a),
                    h => Bindings.Native.delete_Sk8b(h.ToPointer())
                );
            }

            public Sk8b(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8b__8(a, b, c, d, e, f, g, h),
                    h => Bindings.Native.delete_Sk8b(h.ToPointer())
                );
            }

            public byte this[int index]
            {
                get
                {
                    return Bindings.Native.Sk8b__index(_native, index);
                }
            }

            static public Sk8b Load(byte[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        return new Sk8b(Bindings.Native.Sk8b__Load(ptr));
                    }
                }
            }

            public byte[] Store()
            {
                byte[] array = new byte[2];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk8b__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(byte[] array, out Sk8b a, out Sk8b b, out Sk8b c, out Sk8b d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(byte[] array, out Sk8b a, out Sk8b b, out Sk8b c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(byte[] array, out Sk8b a, out Sk8b b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public byte[] Store4(Sk8b a, Sk8b b, Sk8b c, Sk8b d)
            {
                byte[] array = new byte[8];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public byte[] Store3(Sk8b a, Sk8b b, Sk8b c)
            {
                byte[] array = new byte[6];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public byte[] Store2(Sk8b a, Sk8b b)
            {
                byte[] array = new byte[4];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk8b__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk8b operator +(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_add(left._native, right._native)
                );
            }

            public static Sk8b operator -(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_subtract(left._native, right._native)
                );
            }

            public static Sk8b operator *(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_multiply(left._native, right._native)
                );
            }

            public static Sk8b operator /(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_divide(left._native, right._native)
                );
            }

            public static Sk8b operator &(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk8b operator |(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk8b operator ^(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk8b operator ==(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk8b operator !=(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk8b operator <=(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8b operator >=(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8b operator <(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than(left._native, right._native)
                );
            }

            public static Sk8b operator >(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk8b Min(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__Min(left._native, right._native)
                );
            }

            public static Sk8b Max(Sk8b left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__Max(left._native, right._native)
                );
            }

            public Sk8b thenElse(Sk8b a, Sk8b b)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk8b operator +(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator -(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator *(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator /(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator &(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator |(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator ^(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator ==(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator !=(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator <=(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator >=(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator <(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator >(Sk8b left, byte right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8b operator +(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator -(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator *(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator /(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator &(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator |(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator ^(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator ==(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator !=(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator <=(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator >=(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator <(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk8b operator >(byte left, Sk8b right)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk4b b, Sk4b c)
            {
                Bindings.Native.Sk8b__split(_native, b._native, c._native);
            }

            public Sk4b shuffle(int a, int b, int c, int d)
            {
                return new Sk4b(
                    Bindings.Native.Sk8b__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8b shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16b shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16b(
                    Bindings.Native.Sk8b__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk8b SaturatedAdd(Sk8b value)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__saturatedAdd(_native, value._native)
                );
            }

            public Sk8b MulHi(Sk8b value)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__mulHi(_native, value._native)
                );
            }

            public static Sk8b operator <<(Sk8b left, int value)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk8b operator >>(Sk8b left, int value)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public byte min()
            {
                return Bindings.Native.Sk8b__min(_native);
            }

            public byte max()
            {
                return Bindings.Native.Sk8b__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk8b__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk8b__allTrue(_native) != 0;
            }

            public static Sk8b operator !(Sk8b left)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_logical_not(left._native)
                );
            }

            public static Sk8b operator ~(Sk8b left)
            {
                return new Sk8b(
                    Bindings.Native.Sk8b__operator_binary_ones_complement(left._native)
                );
            }
        }

        public unsafe class Sk16b
        {
            internal Pointer _native;
            public Sk16b()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16b__0(),
                    h => Bindings.Native.delete_Sk16b(h.ToPointer())
                );
            }

            internal unsafe Sk16b(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk16b(h.ToPointer())
                );
            }

            public Sk16b(Sk8b a, Sk8b b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16b__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk16b(h.ToPointer())
                );
            }

            public Sk16b(byte a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16b__1(a),
                    h => Bindings.Native.delete_Sk16b(h.ToPointer())
                );
            }

            public Sk16b(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k, byte l, byte m, byte n, byte o, byte p)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16b__16(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p),
                    h => Bindings.Native.delete_Sk16b(h.ToPointer())
                );
            }

            public byte this[int index]
            {
                get
                {
                    return Bindings.Native.Sk16b__index(_native, index);
                }
            }

            static public Sk16b Load(byte[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        return new Sk16b(Bindings.Native.Sk16b__Load(ptr));
                    }
                }
            }

            public byte[] Store()
            {
                byte[] array = new byte[2];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk16b__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(byte[] array, out Sk16b a, out Sk16b b, out Sk16b c, out Sk16b d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(byte[] array, out Sk16b a, out Sk16b b, out Sk16b c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(byte[] array, out Sk16b a, out Sk16b b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public byte[] Store4(Sk16b a, Sk16b b, Sk16b c, Sk16b d)
            {
                byte[] array = new byte[8];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public byte[] Store3(Sk16b a, Sk16b b, Sk16b c)
            {
                byte[] array = new byte[6];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public byte[] Store2(Sk16b a, Sk16b b)
            {
                byte[] array = new byte[4];
                unsafe
                {
                    fixed (byte* ptr = array)
                    {
                        Bindings.Native.Sk16b__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk16b operator +(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_add(left._native, right._native)
                );
            }

            public static Sk16b operator -(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_subtract(left._native, right._native)
                );
            }

            public static Sk16b operator *(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_multiply(left._native, right._native)
                );
            }

            public static Sk16b operator /(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_divide(left._native, right._native)
                );
            }

            public static Sk16b operator &(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk16b operator |(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk16b operator ^(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk16b operator ==(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk16b operator !=(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk16b operator <=(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16b operator >=(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16b operator <(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than(left._native, right._native)
                );
            }

            public static Sk16b operator >(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk16b Min(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__Min(left._native, right._native)
                );
            }

            public static Sk16b Max(Sk16b left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__Max(left._native, right._native)
                );
            }

            public Sk16b thenElse(Sk16b a, Sk16b b)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk16b operator +(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator -(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator *(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator /(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator &(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator |(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator ^(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator ==(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator !=(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator <=(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator >=(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator <(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator >(Sk16b left, byte right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16b operator +(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator -(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator *(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator /(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator &(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator |(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator ^(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator ==(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator !=(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator <=(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator >=(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator <(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk16b operator >(byte left, Sk16b right)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk8b b, Sk8b c)
            {
                Bindings.Native.Sk16b__split(_native, b._native, c._native);
            }

            public Sk4b shuffle(int a, int b, int c, int d)
            {
                return new Sk4b(
                    Bindings.Native.Sk16b__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8b shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8b(
                    Bindings.Native.Sk16b__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16b shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk16b SaturatedAdd(Sk16b value)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__saturatedAdd(_native, value._native)
                );
            }

            public Sk16b MulHi(Sk16b value)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__mulHi(_native, value._native)
                );
            }

            public static Sk16b operator <<(Sk16b left, int value)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk16b operator >>(Sk16b left, int value)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public byte min()
            {
                return Bindings.Native.Sk16b__min(_native);
            }

            public byte max()
            {
                return Bindings.Native.Sk16b__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk16b__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk16b__allTrue(_native) != 0;
            }

            public static Sk16b operator !(Sk16b left)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_logical_not(left._native)
                );
            }

            public static Sk16b operator ~(Sk16b left)
            {
                return new Sk16b(
                    Bindings.Native.Sk16b__operator_binary_ones_complement(left._native)
                );
            }
        }

        public static unsafe Sk4h fma(Sk4h a, Sk4h b, Sk4h c)
        {
            return new Sk4h(
                Bindings.Native.Sk4h__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8h fma(Sk8h a, Sk8h b, Sk8h c)
        {
            return new Sk8h(
                Bindings.Native.Sk8h__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk16h fma(Sk16h a, Sk16h b, Sk16h c)
        {
            return new Sk16h(
                Bindings.Native.Sk16h__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8h join(Sk4h a, Sk4h b)
        {
            return new Sk8h(
                Bindings.Native.Sk8h__join(a._native, b._native)
            );
        }

        public static unsafe Sk16h join(Sk8h a, Sk8h b)
        {
            return new Sk16h(
                Bindings.Native.Sk16h__join(a._native, b._native)
            );
        }

        public unsafe class Sk4h
        {
            internal Pointer _native;
            public Sk4h()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4h__0(),
                    h => Bindings.Native.delete_Sk4h(h.ToPointer())
                );
            }

            internal unsafe Sk4h(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4h(h.ToPointer())
                );
            }

            public Sk4h(ushort a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4h__1(a),
                    h => Bindings.Native.delete_Sk4h(h.ToPointer())
                );
            }

            public Sk4h(ushort a, ushort b, ushort c, ushort d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4h__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4h(h.ToPointer())
                );
            }

            public ushort this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4h__index(_native, index);
                }
            }

            static public Sk4h Load(ushort[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        return new Sk4h(Bindings.Native.Sk4h__Load(ptr));
                    }
                }
            }

            public ushort[] Store()
            {
                ushort[] array = new ushort[2];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4h__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(ushort[] array, out Sk4h a, out Sk4h b, out Sk4h c, out Sk4h d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(ushort[] array, out Sk4h a, out Sk4h b, out Sk4h c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(ushort[] array, out Sk4h a, out Sk4h b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public ushort[] Store4(Sk4h a, Sk4h b, Sk4h c, Sk4h d)
            {
                ushort[] array = new ushort[8];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public ushort[] Store3(Sk4h a, Sk4h b, Sk4h c)
            {
                ushort[] array = new ushort[6];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public ushort[] Store2(Sk4h a, Sk4h b)
            {
                ushort[] array = new ushort[4];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk4h__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4h operator +(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_add(left._native, right._native)
                );
            }

            public static Sk4h operator -(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4h operator *(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4h operator /(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_divide(left._native, right._native)
                );
            }

            public static Sk4h operator &(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4h operator |(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4h operator ^(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4h operator ==(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4h operator !=(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4h operator <=(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4h operator >=(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4h operator <(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4h operator >(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4h Min(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__Min(left._native, right._native)
                );
            }

            public static Sk4h Max(Sk4h left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__Max(left._native, right._native)
                );
            }

            public Sk4h thenElse(Sk4h a, Sk4h b)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4h operator +(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator -(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator *(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator /(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator &(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator |(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator ^(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator ==(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator !=(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator <=(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator >=(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator <(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator >(Sk4h left, ushort right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4h operator +(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator -(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator *(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator /(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator &(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator |(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator ^(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator ==(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator !=(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator <=(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator >=(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator <(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4h operator >(ushort left, Sk4h right)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk4h shuffle(int a, int b, int c, int d)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8h shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8h(
                    Bindings.Native.Sk4h__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16h shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16h(
                    Bindings.Native.Sk4h__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk4h SaturatedAdd(Sk4h value)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__saturatedAdd(_native, value._native)
                );
            }

            public Sk4h MulHi(Sk4h value)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__mulHi(_native, value._native)
                );
            }

            public static Sk4h operator <<(Sk4h left, int value)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk4h operator >>(Sk4h left, int value)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public ushort min()
            {
                return Bindings.Native.Sk4h__min(_native);
            }

            public ushort max()
            {
                return Bindings.Native.Sk4h__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4h__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4h__allTrue(_native) != 0;
            }

            public static Sk4h operator !(Sk4h left)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_logical_not(left._native)
                );
            }

            public static Sk4h operator ~(Sk4h left)
            {
                return new Sk4h(
                    Bindings.Native.Sk4h__operator_binary_ones_complement(left._native)
                );
            }
        }

        public unsafe class Sk8h
        {
            internal Pointer _native;
            public Sk8h()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8h__0(),
                    h => Bindings.Native.delete_Sk8h(h.ToPointer())
                );
            }

            internal unsafe Sk8h(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk8h(h.ToPointer())
                );
            }

            public Sk8h(Sk4h a, Sk4h b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8h__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk8h(h.ToPointer())
                );
            }

            public Sk8h(ushort a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8h__1(a),
                    h => Bindings.Native.delete_Sk8h(h.ToPointer())
                );
            }

            public Sk8h(ushort a, ushort b, ushort c, ushort d, ushort e, ushort f, ushort g, ushort h)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8h__8(a, b, c, d, e, f, g, h),
                    h => Bindings.Native.delete_Sk8h(h.ToPointer())
                );
            }

            public ushort this[int index]
            {
                get
                {
                    return Bindings.Native.Sk8h__index(_native, index);
                }
            }

            static public Sk8h Load(ushort[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        return new Sk8h(Bindings.Native.Sk8h__Load(ptr));
                    }
                }
            }

            public ushort[] Store()
            {
                ushort[] array = new ushort[2];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk8h__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(ushort[] array, out Sk8h a, out Sk8h b, out Sk8h c, out Sk8h d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(ushort[] array, out Sk8h a, out Sk8h b, out Sk8h c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(ushort[] array, out Sk8h a, out Sk8h b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public ushort[] Store4(Sk8h a, Sk8h b, Sk8h c, Sk8h d)
            {
                ushort[] array = new ushort[8];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public ushort[] Store3(Sk8h a, Sk8h b, Sk8h c)
            {
                ushort[] array = new ushort[6];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public ushort[] Store2(Sk8h a, Sk8h b)
            {
                ushort[] array = new ushort[4];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk8h__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk8h operator +(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_add(left._native, right._native)
                );
            }

            public static Sk8h operator -(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_subtract(left._native, right._native)
                );
            }

            public static Sk8h operator *(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_multiply(left._native, right._native)
                );
            }

            public static Sk8h operator /(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_divide(left._native, right._native)
                );
            }

            public static Sk8h operator &(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk8h operator |(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk8h operator ^(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk8h operator ==(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk8h operator !=(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk8h operator <=(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8h operator >=(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8h operator <(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than(left._native, right._native)
                );
            }

            public static Sk8h operator >(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk8h Min(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__Min(left._native, right._native)
                );
            }

            public static Sk8h Max(Sk8h left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__Max(left._native, right._native)
                );
            }

            public Sk8h thenElse(Sk8h a, Sk8h b)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk8h operator +(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator -(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator *(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator /(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator &(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator |(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator ^(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator ==(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator !=(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator <=(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator >=(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator <(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator >(Sk8h left, ushort right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8h operator +(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator -(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator *(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator /(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator &(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator |(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator ^(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator ==(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator !=(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator <=(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator >=(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator <(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk8h operator >(ushort left, Sk8h right)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk4h b, Sk4h c)
            {
                Bindings.Native.Sk8h__split(_native, b._native, c._native);
            }

            public Sk4h shuffle(int a, int b, int c, int d)
            {
                return new Sk4h(
                    Bindings.Native.Sk8h__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8h shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16h shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16h(
                    Bindings.Native.Sk8h__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk8h SaturatedAdd(Sk8h value)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__saturatedAdd(_native, value._native)
                );
            }

            public Sk8h MulHi(Sk8h value)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__mulHi(_native, value._native)
                );
            }

            public static Sk8h operator <<(Sk8h left, int value)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk8h operator >>(Sk8h left, int value)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public ushort min()
            {
                return Bindings.Native.Sk8h__min(_native);
            }

            public ushort max()
            {
                return Bindings.Native.Sk8h__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk8h__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk8h__allTrue(_native) != 0;
            }

            public static Sk8h operator !(Sk8h left)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_logical_not(left._native)
                );
            }

            public static Sk8h operator ~(Sk8h left)
            {
                return new Sk8h(
                    Bindings.Native.Sk8h__operator_binary_ones_complement(left._native)
                );
            }
        }

        public unsafe class Sk16h
        {
            internal Pointer _native;
            public Sk16h()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16h__0(),
                    h => Bindings.Native.delete_Sk16h(h.ToPointer())
                );
            }

            internal unsafe Sk16h(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk16h(h.ToPointer())
                );
            }

            public Sk16h(Sk8h a, Sk8h b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16h__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk16h(h.ToPointer())
                );
            }

            public Sk16h(ushort a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16h__1(a),
                    h => Bindings.Native.delete_Sk16h(h.ToPointer())
                );
            }

            public Sk16h(ushort a, ushort b, ushort c, ushort d, ushort e, ushort f, ushort g, ushort h, ushort i, ushort j, ushort k, ushort l, ushort m, ushort n, ushort o, ushort p)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk16h__16(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p),
                    h => Bindings.Native.delete_Sk16h(h.ToPointer())
                );
            }

            public ushort this[int index]
            {
                get
                {
                    return Bindings.Native.Sk16h__index(_native, index);
                }
            }

            static public Sk16h Load(ushort[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        return new Sk16h(Bindings.Native.Sk16h__Load(ptr));
                    }
                }
            }

            public ushort[] Store()
            {
                ushort[] array = new ushort[2];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk16h__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(ushort[] array, out Sk16h a, out Sk16h b, out Sk16h c, out Sk16h d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(ushort[] array, out Sk16h a, out Sk16h b, out Sk16h c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(ushort[] array, out Sk16h a, out Sk16h b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public ushort[] Store4(Sk16h a, Sk16h b, Sk16h c, Sk16h d)
            {
                ushort[] array = new ushort[8];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public ushort[] Store3(Sk16h a, Sk16h b, Sk16h c)
            {
                ushort[] array = new ushort[6];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public ushort[] Store2(Sk16h a, Sk16h b)
            {
                ushort[] array = new ushort[4];
                unsafe
                {
                    fixed (ushort* ptr = array)
                    {
                        Bindings.Native.Sk16h__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk16h operator +(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_add(left._native, right._native)
                );
            }

            public static Sk16h operator -(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_subtract(left._native, right._native)
                );
            }

            public static Sk16h operator *(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_multiply(left._native, right._native)
                );
            }

            public static Sk16h operator /(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_divide(left._native, right._native)
                );
            }

            public static Sk16h operator &(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk16h operator |(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk16h operator ^(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk16h operator ==(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk16h operator !=(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk16h operator <=(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16h operator >=(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk16h operator <(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than(left._native, right._native)
                );
            }

            public static Sk16h operator >(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk16h Min(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__Min(left._native, right._native)
                );
            }

            public static Sk16h Max(Sk16h left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__Max(left._native, right._native)
                );
            }

            public Sk16h thenElse(Sk16h a, Sk16h b)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk16h operator +(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator -(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator *(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator /(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator &(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator |(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator ^(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator ==(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator !=(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator <=(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator >=(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator <(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator >(Sk16h left, ushort right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk16h operator +(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator -(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator *(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator /(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator &(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator |(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator ^(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator ==(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator !=(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator <=(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator >=(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator <(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk16h operator >(ushort left, Sk16h right)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk8h b, Sk8h c)
            {
                Bindings.Native.Sk16h__split(_native, b._native, c._native);
            }

            public Sk4h shuffle(int a, int b, int c, int d)
            {
                return new Sk4h(
                    Bindings.Native.Sk16h__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8h shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8h(
                    Bindings.Native.Sk16h__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk16h shuffle(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o, int p)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__suffle16(_native, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p)
                );
            }

            public Sk16h SaturatedAdd(Sk16h value)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__saturatedAdd(_native, value._native)
                );
            }

            public Sk16h MulHi(Sk16h value)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__mulHi(_native, value._native)
                );
            }

            public static Sk16h operator <<(Sk16h left, int value)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk16h operator >>(Sk16h left, int value)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public ushort min()
            {
                return Bindings.Native.Sk16h__min(_native);
            }

            public ushort max()
            {
                return Bindings.Native.Sk16h__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk16h__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk16h__allTrue(_native) != 0;
            }

            public static Sk16h operator !(Sk16h left)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_logical_not(left._native)
                );
            }

            public static Sk16h operator ~(Sk16h left)
            {
                return new Sk16h(
                    Bindings.Native.Sk16h__operator_binary_ones_complement(left._native)
                );
            }
        }

        public static unsafe Sk4i fma(Sk4i a, Sk4i b, Sk4i c)
        {
            return new Sk4i(
                Bindings.Native.Sk4i__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8i fma(Sk8i a, Sk8i b, Sk8i c)
        {
            return new Sk8i(
                Bindings.Native.Sk8i__fma(a._native, b._native, c._native)
            );
        }

        public static unsafe Sk8i join(Sk4i a, Sk4i b)
        {
            return new Sk8i(
                Bindings.Native.Sk8i__join(a._native, b._native)
            );
        }

        public unsafe class Sk4i
        {
            internal Pointer _native;
            public Sk4i()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4i__0(),
                    h => Bindings.Native.delete_Sk4i(h.ToPointer())
                );
            }

            internal unsafe Sk4i(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4i(h.ToPointer())
                );
            }

            public Sk4i(int a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4i__1(a),
                    h => Bindings.Native.delete_Sk4i(h.ToPointer())
                );
            }

            public Sk4i(int a, int b, int c, int d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4i__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4i(h.ToPointer())
                );
            }

            public int this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4i__index(_native, index);
                }
            }

            static public Sk4i Load(int[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        return new Sk4i(Bindings.Native.Sk4i__Load(ptr));
                    }
                }
            }

            public int[] Store()
            {
                int[] array = new int[2];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4i__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(int[] array, out Sk4i a, out Sk4i b, out Sk4i c, out Sk4i d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(int[] array, out Sk4i a, out Sk4i b, out Sk4i c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(int[] array, out Sk4i a, out Sk4i b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public int[] Store4(Sk4i a, Sk4i b, Sk4i c, Sk4i d)
            {
                int[] array = new int[8];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public int[] Store3(Sk4i a, Sk4i b, Sk4i c)
            {
                int[] array = new int[6];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public int[] Store2(Sk4i a, Sk4i b)
            {
                int[] array = new int[4];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk4i__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4i operator +(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_add(left._native, right._native)
                );
            }

            public static Sk4i operator -(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4i operator *(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4i operator /(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_divide(left._native, right._native)
                );
            }

            public static Sk4i operator &(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4i operator |(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4i operator ^(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4i operator ==(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4i operator !=(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4i operator <=(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4i operator >=(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4i operator <(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4i operator >(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4i Min(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__Min(left._native, right._native)
                );
            }

            public static Sk4i Max(Sk4i left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__Max(left._native, right._native)
                );
            }

            public Sk4i thenElse(Sk4i a, Sk4i b)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4i operator +(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator -(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator *(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator /(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator &(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator |(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator ^(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator ==(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator !=(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator <=(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator >=(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator <(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator >(Sk4i left, int right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4i operator +(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator -(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator *(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator /(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator &(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator |(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator ^(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator ==(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator !=(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator <=(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator >=(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator <(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4i operator >(int left, Sk4i right)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk4i shuffle(int a, int b, int c, int d)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8i shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8i(
                    Bindings.Native.Sk4i__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk4i Abs()
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__abs(_native)
                );
            }

            public static Sk4i operator <<(Sk4i left, int value)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk4i operator >>(Sk4i left, int value)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public int min()
            {
                return Bindings.Native.Sk4i__min(_native);
            }

            public int max()
            {
                return Bindings.Native.Sk4i__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4i__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4i__allTrue(_native) != 0;
            }

            public static Sk4i operator !(Sk4i left)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_logical_not(left._native)
                );
            }

            public static Sk4i operator ~(Sk4i left)
            {
                return new Sk4i(
                    Bindings.Native.Sk4i__operator_binary_ones_complement(left._native)
                );
            }
        }

        public unsafe class Sk8i
        {
            internal Pointer _native;
            public Sk8i()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8i__0(),
                    h => Bindings.Native.delete_Sk8i(h.ToPointer())
                );
            }

            internal unsafe Sk8i(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk8i(h.ToPointer())
                );
            }

            public Sk8i(Sk4i a, Sk4i b)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8i__2HALF(a._native.ToPointer(), b._native.ToPointer()),
                    h => Bindings.Native.delete_Sk8i(h.ToPointer())
                );
            }

            public Sk8i(int a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8i__1(a),
                    h => Bindings.Native.delete_Sk8i(h.ToPointer())
                );
            }

            public Sk8i(int a, int b, int c, int d, int e, int f, int g, int h)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk8i__8(a, b, c, d, e, f, g, h),
                    h => Bindings.Native.delete_Sk8i(h.ToPointer())
                );
            }

            public int this[int index]
            {
                get
                {
                    return Bindings.Native.Sk8i__index(_native, index);
                }
            }

            static public Sk8i Load(int[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        return new Sk8i(Bindings.Native.Sk8i__Load(ptr));
                    }
                }
            }

            public int[] Store()
            {
                int[] array = new int[2];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk8i__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(int[] array, out Sk8i a, out Sk8i b, out Sk8i c, out Sk8i d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(int[] array, out Sk8i a, out Sk8i b, out Sk8i c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(int[] array, out Sk8i a, out Sk8i b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public int[] Store4(Sk8i a, Sk8i b, Sk8i c, Sk8i d)
            {
                int[] array = new int[8];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public int[] Store3(Sk8i a, Sk8i b, Sk8i c)
            {
                int[] array = new int[6];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public int[] Store2(Sk8i a, Sk8i b)
            {
                int[] array = new int[4];
                unsafe
                {
                    fixed (int* ptr = array)
                    {
                        Bindings.Native.Sk8i__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk8i operator +(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_add(left._native, right._native)
                );
            }

            public static Sk8i operator -(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_subtract(left._native, right._native)
                );
            }

            public static Sk8i operator *(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_multiply(left._native, right._native)
                );
            }

            public static Sk8i operator /(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_divide(left._native, right._native)
                );
            }

            public static Sk8i operator &(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk8i operator |(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk8i operator ^(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk8i operator ==(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk8i operator !=(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk8i operator <=(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8i operator >=(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk8i operator <(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than(left._native, right._native)
                );
            }

            public static Sk8i operator >(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk8i Min(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__Min(left._native, right._native)
                );
            }

            public static Sk8i Max(Sk8i left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__Max(left._native, right._native)
                );
            }

            public Sk8i thenElse(Sk8i a, Sk8i b)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk8i operator +(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator -(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator *(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator /(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator &(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator |(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator ^(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator ==(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator !=(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator <=(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator >=(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator <(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator >(Sk8i left, int right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk8i operator +(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator -(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator *(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator /(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator &(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator |(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator ^(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator ==(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator !=(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator <=(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator >=(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator <(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk8i operator >(int left, Sk8i right)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public void split(Sk4i b, Sk4i c)
            {
                Bindings.Native.Sk8i__split(_native, b._native, c._native);
            }

            public Sk4i shuffle(int a, int b, int c, int d)
            {
                return new Sk4i(
                    Bindings.Native.Sk8i__suffle4(_native, a, b, c, d)
                );
            }

            public Sk8i shuffle(int a, int b, int c, int d, int e, int f, int j, int h)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__suffle8(_native, a, b, c, d, e, f, j, h)
                );
            }

            public Sk8i Abs()
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__abs(_native)
                );
            }

            public static Sk8i operator <<(Sk8i left, int value)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk8i operator >>(Sk8i left, int value)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public int min()
            {
                return Bindings.Native.Sk8i__min(_native);
            }

            public int max()
            {
                return Bindings.Native.Sk8i__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk8i__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk8i__allTrue(_native) != 0;
            }

            public static Sk8i operator !(Sk8i left)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_logical_not(left._native)
                );
            }

            public static Sk8i operator ~(Sk8i left)
            {
                return new Sk8i(
                    Bindings.Native.Sk8i__operator_binary_ones_complement(left._native)
                );
            }
        }

        public static unsafe Sk4u fma(Sk4u a, Sk4u b, Sk4u c)
        {
            return new Sk4u(
                Bindings.Native.Sk4u__fma(a._native, b._native, c._native)
            );
        }

        public unsafe class Sk4u
        {
            internal Pointer _native;
            public Sk4u()
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4u__0(),
                    h => Bindings.Native.delete_Sk4u(h.ToPointer())
                );
            }

            internal unsafe Sk4u(void* ptr)
            {
                _native = new Pointer(
                    ptr,
                    h => Bindings.Native.delete_Sk4u(h.ToPointer())
                );
            }

            public Sk4u(uint a)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4u__1(a),
                    h => Bindings.Native.delete_Sk4u(h.ToPointer())
                );
            }

            public Sk4u(uint a, uint b, uint c, uint d)
            {
                _native = new Pointer(
                    Bindings.Native.new_Sk4u__4(a, b, c, d),
                    h => Bindings.Native.delete_Sk4u(h.ToPointer())
                );
            }

            public uint this[int index]
            {
                get
                {
                    return Bindings.Native.Sk4u__index(_native, index);
                }
            }

            static public Sk4u Load(uint[] array)
            {
                if (array.Length < 2) throw new IndexOutOfRangeException("array must have a length of 2 or greater");
                unsafe
                {
                    fixed (uint* ptr = array)
                    {
                        return new Sk4u(Bindings.Native.Sk4u__Load(ptr));
                    }
                }
            }

            public uint[] Store()
            {
                uint[] array = new uint[2];
                unsafe
                {
                    fixed (uint* ptr = array)
                    {
                        // store the contents of _native into the pointer ptr
                        Bindings.Native.Sk4u__store(_native, ptr);
                    }
                }
                return array;
            }

            static public void Load4(uint[] array, out Sk4u a, out Sk4u b, out Sk4u c, out Sk4u d)
            {
                if (array.Length < 8) throw new IndexOutOfRangeException("array must have a length of 8 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;
                    void* vd;

                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Load4(ptr, &va, &vb, &vc, &vd);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                    d = new(vd);
                }
            }

            static public void Load3(uint[] array, out Sk4u a, out Sk4u b, out Sk4u c)
            {
                if (array.Length < 6)
                    throw new IndexOutOfRangeException("array must have a length of 6 or greater");
                unsafe
                {
                    void* va;
                    void* vb;
                    void* vc;

                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Load3(ptr, &va, &vb, &vc);
                    }
                    a = new(va);
                    b = new(vb);
                    c = new(vc);
                }
            }

            static public void Load2(uint[] array, out Sk4u a, out Sk4u b)
            {
                if (array.Length < 4)
                    throw new IndexOutOfRangeException("array must have a length of 4 or greater");
                unsafe
                {
                    void* va;
                    void* vb;

                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Load2(ptr, &va, &vb);
                    }
                    a = new(va);
                    b = new(vb);
                }
            }

            static public uint[] Store4(Sk4u a, Sk4u b, Sk4u c, Sk4u d)
            {
                uint[] array = new uint[8];
                unsafe
                {
                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Store4(ptr, a._native, b._native, c._native, d._native);
                    }
                }
                return array;
            }

            static public uint[] Store3(Sk4u a, Sk4u b, Sk4u c)
            {
                uint[] array = new uint[6];
                unsafe
                {
                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Store3(ptr, a._native, b._native, c._native);
                    }
                }
                return array;
            }

            static public uint[] Store2(Sk4u a, Sk4u b)
            {
                uint[] array = new uint[4];
                unsafe
                {
                    fixed (uint* ptr = array)
                    {
                        Bindings.Native.Sk4u__Store2(ptr, a._native, b._native);
                    }
                }
                return array;
            }

            public static Sk4u operator +(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_add(left._native, right._native)
                );
            }

            public static Sk4u operator -(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_subtract(left._native, right._native)
                );
            }

            public static Sk4u operator *(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_multiply(left._native, right._native)
                );
            }

            public static Sk4u operator /(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_divide(left._native, right._native)
                );
            }

            public static Sk4u operator &(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_AND(left._native, right._native)
                );
            }

            public static Sk4u operator |(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_OR(left._native, right._native)
                );
            }

            public static Sk4u operator ^(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_XOR(left._native, right._native)
                );
            }

            public static Sk4u operator ==(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_equal_to(left._native, right._native)
                );
            }

            public static Sk4u operator !=(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_not_equal_to(left._native, right._native)
                );
            }

            public static Sk4u operator <=(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4u operator >=(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than_or_equal_to(left._native, right._native)
                );
            }

            public static Sk4u operator <(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than(left._native, right._native)
                );
            }

            public static Sk4u operator >(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than(left._native, right._native)
                );
            }

            public static Sk4u Min(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__Min(left._native, right._native)
                );
            }

            public static Sk4u Max(Sk4u left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__Max(left._native, right._native)
                );
            }

            public Sk4u thenElse(Sk4u a, Sk4u b)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__thenElse(_native, a._native, b._native)
                );
            }

            public static Sk4u operator +(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_add__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator -(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_subtract__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator *(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_multiply__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator /(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_divide__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator &(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_AND__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator |(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_OR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator ^(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_XOR__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator ==(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator !=(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_not_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator <=(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator >=(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than_or_equal_to__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator <(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator >(Sk4u left, uint right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than__scalar_rhs(left._native, right)
                );
            }

            public static Sk4u operator +(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_add__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator -(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_subtract__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator *(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_multiply__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator /(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_divide__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator &(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_AND__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator |(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_OR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator ^(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_bitwise_XOR__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator ==(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator !=(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_not_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator <=(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator >=(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than_or_equal_to__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator <(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_less_than__scalar_lhs(left, right._native)
                );
            }

            public static Sk4u operator >(uint left, Sk4u right)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_greater_than__scalar_lhs(left, right._native)
                );
            }

            public Sk4u shuffle(int a, int b, int c, int d)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__suffle4(_native, a, b, c, d)
                );
            }

            public Sk4u SaturatedAdd(Sk4u value)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__saturatedAdd(_native, value._native)
                );
            }

            public Sk4u MulHi(Sk4u value)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__mulHi(_native, value._native)
                );
            }

            public static Sk4u operator <<(Sk4u left, int value)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__assign_operator_binary_left_shift(left._native, value)
                );
            }

            public static Sk4u operator >>(Sk4u left, int value)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__assign_operator_binary_right_shift(left._native, value)
                );
            }

            public uint min()
            {
                return Bindings.Native.Sk4u__min(_native);
            }

            public uint max()
            {
                return Bindings.Native.Sk4u__min(_native);
            }

            public bool AnyTrue()
            {
                return Bindings.Native.Sk4u__anyTrue(_native) != 0;
            }

            public bool AllTrue()
            {
                return Bindings.Native.Sk4u__allTrue(_native) != 0;
            }

            public static Sk4u operator !(Sk4u left)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_logical_not(left._native)
                );
            }

            public static Sk4u operator ~(Sk4u left)
            {
                return new Sk4u(
                    Bindings.Native.Sk4u__operator_binary_ones_complement(left._native)
                );
            }
        }
    }
}
