using System.Runtime.InteropServices;

namespace Bindings
{
    public static unsafe partial class Native
    {
        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2f__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2f__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2f__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2f__2(float a, float b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4f__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4f__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4f__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4f__4(float a, float b, float c, float d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8f__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8f__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8f__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8f__8(float a, float b, float c, float d, float e, float f, float g, float h);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16f__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16f__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16f__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16f__16(float a, float b, float c, float d, float e, float f, float g, float h, float i, float j, float k, float l, float m, float n, float o, float p);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk2f__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2f__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2f__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk2f(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk4f__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4f__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4f__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4f(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk8f__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8f__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8f__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk8f(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk16f__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16f__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16f__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk16f(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2s__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2s__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2s__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk2s__2(float a, float b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4s__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4s__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4s__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4s__4(float a, float b, float c, float d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8s__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8s__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8s__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8s__8(float a, float b, float c, float d, float e, float f, float g, float h);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16s__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16s__1(float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16s__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16s__16(float a, float b, float c, float d, float e, float f, float g, float h, float i, float j, float k, float l, float m, float n, float o, float p);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk2s__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk2s__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk2s__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk2s(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk4s__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4s__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4s__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4s(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk8s__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8s__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8s__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk8s(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern float Sk16s__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_add__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_subtract__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_multiply__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_divide__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_AND__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_OR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_XOR__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_not_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than__scalar_rhs(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_add__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_subtract__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_multiply__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_divide__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_AND__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_OR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_bitwise_XOR__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_not_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than_or_equal_to__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_less_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__operator_greater_than__scalar_lhs(float value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_add__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_subtract__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_multiply__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_divide__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_bitwise_AND__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_bitwise_OR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__assign_operator_bitwise_XOR__scalar(void* ptr, float value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16s__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16s__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk16s(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4b__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4b__1([NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4b__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4b__4([NativeTypeName("uint8_t")] byte a, [NativeTypeName("uint8_t")] byte b, [NativeTypeName("uint8_t")] byte c, [NativeTypeName("uint8_t")] byte d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8b__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8b__1([NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8b__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8b__8([NativeTypeName("uint8_t")] byte a, [NativeTypeName("uint8_t")] byte b, [NativeTypeName("uint8_t")] byte c, [NativeTypeName("uint8_t")] byte d, [NativeTypeName("uint8_t")] byte e, [NativeTypeName("uint8_t")] byte f, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte h);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16b__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16b__1([NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16b__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16b__16([NativeTypeName("uint8_t")] byte a, [NativeTypeName("uint8_t")] byte b, [NativeTypeName("uint8_t")] byte c, [NativeTypeName("uint8_t")] byte d, [NativeTypeName("uint8_t")] byte e, [NativeTypeName("uint8_t")] byte f, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte h, [NativeTypeName("uint8_t")] byte i, [NativeTypeName("uint8_t")] byte j, [NativeTypeName("uint8_t")] byte k, [NativeTypeName("uint8_t")] byte l, [NativeTypeName("uint8_t")] byte m, [NativeTypeName("uint8_t")] byte n, [NativeTypeName("uint8_t")] byte o, [NativeTypeName("uint8_t")] byte p);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte Sk4b__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_add__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_subtract__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_multiply__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_divide__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_not_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_less_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_greater_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4b__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4b(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4b__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte Sk8b__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_add__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_subtract__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_multiply__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_divide__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_not_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_less_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_greater_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8b__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk8b(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8b__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte Sk16b__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_add__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_subtract__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_multiply__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_divide__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_not_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_less_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_greater_than__scalar_lhs([NativeTypeName("uint8_t")] byte value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint8_t")] byte value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16b__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk16b(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16b__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4h__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4h__1([NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4h__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4h__4([NativeTypeName("uint16_t")] ushort a, [NativeTypeName("uint16_t")] ushort b, [NativeTypeName("uint16_t")] ushort c, [NativeTypeName("uint16_t")] ushort d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8h__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8h__1([NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8h__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8h__8([NativeTypeName("uint16_t")] ushort a, [NativeTypeName("uint16_t")] ushort b, [NativeTypeName("uint16_t")] ushort c, [NativeTypeName("uint16_t")] ushort d, [NativeTypeName("uint16_t")] ushort e, [NativeTypeName("uint16_t")] ushort f, [NativeTypeName("uint16_t")] ushort g, [NativeTypeName("uint16_t")] ushort h);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16h__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16h__1([NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16h__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk16h__16([NativeTypeName("uint16_t")] ushort a, [NativeTypeName("uint16_t")] ushort b, [NativeTypeName("uint16_t")] ushort c, [NativeTypeName("uint16_t")] ushort d, [NativeTypeName("uint16_t")] ushort e, [NativeTypeName("uint16_t")] ushort f, [NativeTypeName("uint16_t")] ushort g, [NativeTypeName("uint16_t")] ushort h, [NativeTypeName("uint16_t")] ushort i, [NativeTypeName("uint16_t")] ushort j, [NativeTypeName("uint16_t")] ushort k, [NativeTypeName("uint16_t")] ushort l, [NativeTypeName("uint16_t")] ushort m, [NativeTypeName("uint16_t")] ushort n, [NativeTypeName("uint16_t")] ushort o, [NativeTypeName("uint16_t")] ushort p);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Sk4h__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_add__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_subtract__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_multiply__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_divide__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_not_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_less_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_greater_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4h__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4h(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4h__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Sk8h__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_add__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_subtract__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_multiply__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_divide__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_not_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_less_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_greater_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8h__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk8h(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8h__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort Sk16h__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_add__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_subtract__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_multiply__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_divide__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_not_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_less_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_greater_than__scalar_lhs([NativeTypeName("uint16_t")] ushort value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint16_t")] ushort value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk16h__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk16h(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk16h__mulHi(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4i__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4i__1([NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4i__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4i__4([NativeTypeName("int32_t")] int a, [NativeTypeName("int32_t")] int b, [NativeTypeName("int32_t")] int c, [NativeTypeName("int32_t")] int d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8i__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8i__1([NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8i__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk8i__8([NativeTypeName("int32_t")] int a, [NativeTypeName("int32_t")] int b, [NativeTypeName("int32_t")] int c, [NativeTypeName("int32_t")] int d, [NativeTypeName("int32_t")] int e, [NativeTypeName("int32_t")] int f, [NativeTypeName("int32_t")] int g, [NativeTypeName("int32_t")] int h);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4u__0();

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4u__1([NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4u__2HALF(void* a, void* b);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* new_Sk4u__4([NativeTypeName("uint32_t")] uint a, [NativeTypeName("uint32_t")] uint b, [NativeTypeName("uint32_t")] uint c, [NativeTypeName("uint32_t")] uint d);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("int32_t")]
        public static extern int Sk4i__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_add__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_add__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_subtract__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_multiply__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_divide__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_AND__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_OR__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_bitwise_XOR__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_not_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_less_than__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_greater_than__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_add__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_divide__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4i__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4i(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4i__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("int32_t")]
        public static extern int Sk8i__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_add__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_add__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_subtract__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_multiply__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_divide__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_AND__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_OR__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_bitwise_XOR__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_not_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_less_than__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_greater_than__scalar_lhs([NativeTypeName("int32_t")] int value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_add__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_divide__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("int32_t")] int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk8i__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk8i(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk8i__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint Sk4u__index(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__Load(void* value1);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__store(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Load4(void* value1, void** value2, void** value3, void** value4, void** value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Load3(void* value1, void** value2, void** value3, void** value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Load2(void* value1, void** value2, void** value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Store4(void* value1, void* value2, void* value3, void* value4, void* value5);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Store3(void* value1, void* value2, void* value3, void* value4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__Store2(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_add(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_subtract(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_multiply(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_divide(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_AND(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_OR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_XOR(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_not_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than_or_equal_to(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__Min(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__Max(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__thenElse(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_add__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_subtract__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_multiply__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_divide__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_AND__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_OR__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_XOR__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_not_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than_or_equal_to__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than__scalar_rhs(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_add__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_subtract__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_multiply__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_divide__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_AND__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_OR__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_bitwise_XOR__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_equal_to__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_not_equal_to__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than_or_equal_to__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than_or_equal_to__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_less_than__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_greater_than__scalar_lhs([NativeTypeName("uint32_t")] uint value, void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_add__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_subtract__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_multiply__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_divide__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_bitwise_AND__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_bitwise_OR__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_bitwise_XOR__scalar(void* ptr, [NativeTypeName("uint32_t")] uint value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__fma(void* value1, void* value2, void* value3);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void Sk4u__split(void* ptr, void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__join(void* value1, void* value2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__suffle2(void* ptr, int Ix1, int Ix2);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__suffle4(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__suffle8(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__suffle16(void* ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void delete_Sk4u(void* ptr);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_binary_left_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__assign_operator_binary_right_shift(void* ptr, int value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__saturatedAdd(void* ptr, void* value);

        [DllImport("AndroidUI.Native.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void* Sk4u__mulHi(void* ptr, void* value);
    }
}
