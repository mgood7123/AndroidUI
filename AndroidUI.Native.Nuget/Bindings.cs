using System;
using System.Runtime.InteropServices;

namespace AndroidUI
{
    static class ImportResolver
    {
        [System.Runtime.CompilerServices.ModuleInitializer]
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
                    libraryName = "runtimes/win-x64/native/AndroidUI.Native.Windows.dll";
                }
                else if (!OperatingSystem.IsAndroid())
                {
                    libraryName = "runtimes/monoandroid-x64/native/libAndroidUI_Native_Android.so";
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
                //Console.WriteLine("Acquire Handle: " + handle);
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
                //Console.WriteLine("Release Handle: " + handle);
                dispose.Invoke(handle);
                handle = invalid;
                return true;
            }

            public unsafe static implicit operator void*(Pointer a) => a.ToPointer();
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
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
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
                    h => Bindings.Native.delete_Sk16f(h.ToPointer())
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
        }
    }
}
