namespace AndroidUI
{
    using AndroidUI.Extensions;
    public static class Generics
    {
        public static class Operators
        {
            public static int UnaryMinusAsInt<T>(T op) where T : unmanaged
            {
                return -Convert.ToInt32(op);
            }

            public static long UnaryMinusAsLong<T>(T op) where T : unmanaged
            {
                return -Convert.ToInt64(op);
            }

            public static float UnaryMinusAsFloat<T>(T op) where T : unmanaged
            {
                return -Convert.ToSingle(op);
            }

            public static double UnaryMinusAsDouble<T>(T op) where T : unmanaged
            {
                return -Convert.ToDouble(op);
            }

            public static decimal UnaryMinusAsDecimal<T>(T op) where T : unmanaged
            {
                return -Convert.ToDecimal(op);
            }

            public static T UnaryMinus<T>(T op) where T : unmanaged
            {
                Type t = typeof(T);
                int s = CastUtils.SizeOfUnmanagedType(t);
                switch (s)
                {
                    case <= 4:
                        switch (op)
                        {
                            case float:
                                return CastUtils.reinterpret_cast<T>(UnaryMinusAsFloat(op));
                            default:
                                return CastUtils.reinterpret_cast<T>(UnaryMinusAsInt(op));
                        }
                    case 8:
                        switch (op)
                        {
                            case double:
                                return CastUtils.reinterpret_cast<T>(UnaryMinusAsDouble(op));
                            default:
                                return CastUtils.reinterpret_cast<T>(UnaryMinusAsLong(op));
                        }
                    case 16:
                        return CastUtils.reinterpret_cast<T>(UnaryMinusAsDecimal(op));
                    default:
                        throw new InvalidCastException(t.FullName + " cannot be given");
                }
            }




            public static int UnaryAddAsInt<T>(T op) where T : unmanaged
            {
                return +Convert.ToInt32(op);
            }

            public static long UnaryAddAsLong<T>(T op) where T : unmanaged
            {
                return +Convert.ToInt64(op);
            }

            public static float UnaryAddAsFloat<T>(T op) where T : unmanaged
            {
                return +Convert.ToSingle(op);
            }

            public static double UnaryAddAsDouble<T>(T op) where T : unmanaged
            {
                return +Convert.ToDouble(op);
            }

            public static decimal UnaryAddAsDecimal<T>(T op) where T : unmanaged
            {
                return +Convert.ToDecimal(op);
            }

            public static T UnaryAdd<T>(T op) where T : unmanaged
            {
                Type t = typeof(T);
                int s = CastUtils.SizeOfUnmanagedType(t);
                switch (s)
                {
                    case <= 4:
                        switch (op)
                        {
                            case float:
                                return CastUtils.reinterpret_cast<T>(UnaryAddAsFloat(op));
                            default:
                                return CastUtils.reinterpret_cast<T>(UnaryAddAsInt(op));
                        }
                    case 8:
                        switch (op)
                        {
                            case double:
                                return CastUtils.reinterpret_cast<T>(UnaryAddAsDouble(op));
                            default:
                                return CastUtils.reinterpret_cast<T>(UnaryAddAsLong(op));
                        }
                    case 16:
                        return CastUtils.reinterpret_cast<T>(UnaryAddAsDecimal(op));
                    default:
                        throw new InvalidCastException(t.FullName + " cannot be given");
                }
            }




            public static int TidalAsInt<T>(T op) where T : unmanaged
            {
                return ~Convert.ToInt32(op);
            }

            public static long TidalAsLong<T>(T op) where T : unmanaged
            {
                return ~Convert.ToInt64(op);
            }

            public static T Tidal<T>(T op) where T : unmanaged
            {
                Type t = typeof(T);
                int s = CastUtils.SizeOfUnmanagedType(t);
                switch (s)
                {
                    case <= 4:
                        switch (op)
                        {
                            case float:
                                return CastUtils.reinterpret_cast<T>(TidalAsInt(CastUtils.reinterpret_cast<uint>(op)));
                            default:
                                return CastUtils.reinterpret_cast<T>(TidalAsInt(op));
                        }
                    case 8:
                        switch (op)
                        {
                            case double:
                                return CastUtils.reinterpret_cast<T>(TidalAsLong(CastUtils.reinterpret_cast<ulong>(op)));
                            default:
                                return CastUtils.reinterpret_cast<T>(TidalAsLong(op));
                        }
                    default:
                        throw new InvalidCastException(t.FullName + " cannot be given");
                }
            }


            public enum BitShiftDir { left, right };

            public static int BitShiftAsInt<T>(T op, int bits, BitShiftDir direction) where T : unmanaged
            {
                int v = Convert.ToInt32(op);
                return direction == BitShiftDir.left ? v << bits : v >> bits;
            }

            public static long BitShiftAsLong<T>(T op, int bits, BitShiftDir direction) where T : unmanaged
            {
                return +Convert.ToInt64(op);
            }

            public static T BitShift<T>(T op, int bits, BitShiftDir direction) where T : unmanaged
            {
                Type t = typeof(T);
                int s = CastUtils.SizeOfUnmanagedType(t);
                switch (s)
                {
                    case <= 4:
                        switch (op)
                        {
                            case float:
                                return CastUtils.reinterpret_cast<T>(BitShiftAsInt(CastUtils.reinterpret_cast<uint>(op), bits, direction));
                            default:
                                return CastUtils.reinterpret_cast<T>(BitShiftAsInt(op, bits, direction));
                        }
                    case 8:
                        switch (op)
                        {
                            case double:
                                return CastUtils.reinterpret_cast<T>(BitShiftAsLong(CastUtils.reinterpret_cast<ulong>(op), bits, direction));
                            default:
                                return CastUtils.reinterpret_cast<T>(BitShiftAsLong(op, bits, direction));
                        }
                    default:
                        throw new InvalidCastException(t.FullName + " cannot be given");
                }
            }




            public static int BinaryAddAsInt<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt32(left) + Convert.ToInt32(right);
            }

            public static long BinaryAddAsLong<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt64(left) + Convert.ToInt64(right);
            }

            public static float BinaryAddAsFloat<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToSingle(left) + Convert.ToSingle(right);
            }

            public static double BinaryAddAsDouble<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDouble(left) + Convert.ToDouble(right);
            }

            public static decimal BinaryAddAsDecimal<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDecimal(left) + Convert.ToDecimal(right);
            }

            public static object BinaryAdd<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                Type t1 = typeof(T1);
                int s1 = CastUtils.SizeOfUnmanagedType(t1);
                Type t2 = typeof(T2);
                int s2 = CastUtils.SizeOfUnmanagedType(t2);
                switch (s1)
                {
                    case <= 4:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case float:
                                        return BinaryAddAsFloat(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return BinaryAddAsFloat(left, right);
                                            default:
                                                return BinaryAddAsInt(left, right);
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case float:
                                    case double:
                                        return BinaryAddAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case double:
                                                return BinaryAddAsDouble(left, right);
                                            default:
                                                return BinaryAddAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryAddAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 8:
                        switch (s2)
                        {
                            case <= 8:
                                switch (left)
                                {
                                    case double:
                                        return BinaryAddAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                            case double:
                                                return BinaryAddAsDouble(left, right);
                                            default:
                                                return BinaryAddAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryAddAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 16:
                        return BinaryAddAsDecimal(left, right);
                    default:
                        throw new InvalidCastException(t1.FullName + " cannot be given");
                }
            }




            public static int BinarySubtractAsInt<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt32(left) - Convert.ToInt32(right);
            }

            public static long BinarySubtractAsLong<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt64(left) - Convert.ToInt64(right);
            }

            public static float BinarySubtractAsFloat<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToSingle(left) - Convert.ToSingle(right);
            }

            public static double BinarySubtractAsDouble<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDouble(left) - Convert.ToDouble(right);
            }

            public static decimal BinarySubtractAsDecimal<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDecimal(left) - Convert.ToDecimal(right);
            }

            public static object BinarySubtract<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                Type t1 = typeof(T1);
                int s1 = CastUtils.SizeOfUnmanagedType(t1);
                Type t2 = typeof(T2);
                int s2 = CastUtils.SizeOfUnmanagedType(t2);
                switch (s1)
                {
                    case <= 4:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case float:
                                        return BinarySubtractAsFloat(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return BinarySubtractAsFloat(left, right);
                                            default:
                                                return BinarySubtractAsInt(left, right);
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case float:
                                    case double:
                                        return BinarySubtractAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case double:
                                                return BinarySubtractAsDouble(left, right);
                                            default:
                                                return BinaryAddAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinarySubtractAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 8:
                        switch (s2)
                        {
                            case <= 8:
                                switch (left)
                                {
                                    case double:
                                        return BinarySubtractAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                            case double:
                                                return BinarySubtractAsDouble(left, right);
                                            default:
                                                return BinarySubtractAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinarySubtractAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 16:
                        return BinarySubtractAsDecimal(left, right);
                    default:
                        throw new InvalidCastException(t1.FullName + " cannot be given");
                }
            }




            public static int BinaryMultiplyAsInt<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt32(left) * Convert.ToInt32(right);
            }

            public static long BinaryMultiplyAsLong<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt64(left) * Convert.ToInt64(right);
            }

            public static float BinaryMultiplyAsFloat<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToSingle(left) * Convert.ToSingle(right);
            }

            public static double BinaryMultiplyAsDouble<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDouble(left) * Convert.ToDouble(right);
            }

            public static decimal BinaryMultiplyAsDecimal<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDecimal(left) * Convert.ToDecimal(right);
            }

            public static object BinaryMultiply<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                Type t1 = typeof(T1);
                int s1 = CastUtils.SizeOfUnmanagedType(t1);
                Type t2 = typeof(T2);
                int s2 = CastUtils.SizeOfUnmanagedType(t2);
                switch (s1)
                {
                    case <= 4:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case float:
                                        return BinaryMultiplyAsFloat(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return BinaryMultiplyAsFloat(left, right);
                                            default:
                                                return BinaryMultiplyAsInt(left, right);
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case float:
                                    case double:
                                        return BinaryMultiplyAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case double:
                                                return BinaryMultiplyAsDouble(left, right);
                                            default:
                                                return BinaryMultiplyAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryMultiplyAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 8:
                        switch (s2)
                        {
                            case <= 8:
                                switch (left)
                                {
                                    case double:
                                        return BinaryMultiplyAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                            case double:
                                                return BinaryMultiplyAsDouble(left, right);
                                            default:
                                                return BinaryMultiplyAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryMultiplyAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 16:
                        return BinaryMultiplyAsDecimal(left, right);
                    default:
                        throw new InvalidCastException(t1.FullName + " cannot be given");
                }
            }




            public static int BinaryDivideAsInt<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt32(left) / Convert.ToInt32(right);
            }

            public static long BinaryDivideAsLong<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToInt64(left) / Convert.ToInt64(right);
            }

            public static float BinaryDivideAsFloat<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToSingle(left) / Convert.ToSingle(right);
            }

            public static double BinaryDivideAsDouble<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDouble(left) / Convert.ToDouble(right);
            }

            public static decimal BinaryDivideAsDecimal<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                return Convert.ToDecimal(left) / Convert.ToDecimal(right);
            }

            public static object BinaryDivide<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                Type t1 = typeof(T1);
                int s1 = CastUtils.SizeOfUnmanagedType(t1);
                Type t2 = typeof(T2);
                int s2 = CastUtils.SizeOfUnmanagedType(t2);
                switch (s1)
                {
                    case <= 4:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case float:
                                        return BinaryDivideAsFloat(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return BinaryDivideAsFloat(left, right);
                                            default:
                                                return BinaryDivideAsInt(left, right);
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case float:
                                    case double:
                                        return BinaryDivideAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case double:
                                                return BinaryDivideAsDouble(left, right);
                                            default:
                                                return BinaryDivideAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryDivideAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 8:
                        switch (s2)
                        {
                            case <= 8:
                                switch (left)
                                {
                                    case double:
                                        return BinaryDivideAsDouble(left, right);
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                            case double:
                                                return BinaryDivideAsDouble(left, right);
                                            default:
                                                return BinaryDivideAsLong(left, right);
                                        }
                                }
                            case 16:
                                return BinaryDivideAsDecimal(left, right);
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 16:
                        return BinaryDivideAsDecimal(left, right);
                    default:
                        throw new InvalidCastException(t1.FullName + " cannot be given");
                }
            }




            public static object BitwiseAND<T1, T2>(T1 left, T2 right) where T1 : unmanaged where T2 : unmanaged
            {
                Type t1 = typeof(T1);
                int s1 = CastUtils.SizeOfUnmanagedType(t1);
                Type t2 = typeof(T2);
                int s2 = CastUtils.SizeOfUnmanagedType(t2);
                switch (s1)
                {
                    case <= 4:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case float:
                                        switch (right)
                                        {
                                            case float:
                                                return (
                                                    CastUtils.reinterpret_cast<float>(left)
                                                        .ToRawUIntBits() &
                                                    CastUtils.reinterpret_cast<float>(right)
                                                        .ToRawUIntBits()
                                                ).BitsToFloat();
                                            default:
                                                return (
                                                    CastUtils.reinterpret_cast<float>(left)
                                                        .ToRawUIntBits() &
                                                    Convert.ToUInt32(right)
                                                ).BitsToFloat();
                                        }
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return (
                                                    Convert.ToUInt32(left) &
                                                    CastUtils.reinterpret_cast<float>(right)
                                                        .ToRawUIntBits()
                                                ).BitsToFloat();
                                            default:
                                                return (
                                                    Convert.ToUInt32(left) &
                                                    Convert.ToUInt32(right)
                                                ).BitsToFloat();
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case float:
                                    case double:
                                        switch (right)
                                        {
                                            case float:
                                            case Double:
                                                return (
                                                    Convert.ToDouble(left)
                                                        .ToRawULongBits() &
                                                    Convert.ToDouble(right)
                                                        .ToRawULongBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    Convert.ToDouble(left)
                                                        .ToRawULongBits() &
                                                    Convert.ToUInt64(right)
                                                ).BitsToDouble();
                                        }
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                            case Double:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    Convert.ToDouble(right)
                                                        .ToRawULongBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    Convert.ToUInt64(right)
                                                ).BitsToDouble();
                                        }
                                }
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    case 8:
                        switch (s2)
                        {
                            case <= 4:
                                switch (left)
                                {
                                    case double:
                                        switch (right)
                                        {
                                            case float:
                                                return (
                                                    CastUtils.reinterpret_cast<double>(left)
                                                        .ToRawULongBits() &
                                                    CastUtils.reinterpret_cast<float>(right)
                                                        .ToRawUIntBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    CastUtils.reinterpret_cast<double>(left)
                                                        .ToRawULongBits() &
                                                    Convert.ToUInt32(right)
                                                ).BitsToDouble();
                                        }
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    CastUtils.reinterpret_cast<float>(right)
                                                        .ToRawUIntBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    Convert.ToUInt32(right)
                                                ).BitsToDouble();
                                        }
                                }
                            case 8:
                                switch (left)
                                {
                                    case double:
                                        switch (right)
                                        {
                                            case double:
                                                return (
                                                    CastUtils.reinterpret_cast<double>(left)
                                                        .ToRawULongBits() &
                                                    CastUtils.reinterpret_cast<double>(right)
                                                        .ToRawULongBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    CastUtils.reinterpret_cast<double>(left)
                                                        .ToRawULongBits() &
                                                    Convert.ToUInt64(right)
                                                ).BitsToDouble();
                                        }
                                    default:
                                        switch (right)
                                        {
                                            case float:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    CastUtils.reinterpret_cast<double>(right)
                                                        .ToRawULongBits()
                                                ).BitsToDouble();
                                            default:
                                                return (
                                                    Convert.ToUInt64(left) &
                                                    Convert.ToUInt64(right)
                                                ).BitsToDouble();
                                        }
                                }
                            default:
                                throw new InvalidCastException(t2.FullName + " cannot be given");
                        }
                    default:
                        throw new InvalidCastException(t1.FullName + " cannot be given");
                }
            }
        }

        public static bool ToBool<T>(T a)
        {
            Type f = typeof(T);
            Type t = typeof(bool);
            bool c = false;
            try
            {
                return (bool) Convert.ChangeType(a, t);
            }
            catch (Exception e)
            {
                string m = "could not convert type " + f.FullName + " to type " + t.FullName + " = " + c + "\n" + e;
                Console.WriteLine(m + "\n");
                throw new InvalidCastException(m, e);
            }
        }
    }
}