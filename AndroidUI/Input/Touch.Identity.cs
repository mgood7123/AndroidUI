namespace AndroidUI.Input
{
    public partial class Touch
    {
        /// <summary>
        /// an Identity class useful for comparing/identifying an object by reference or value
        /// <br/>
        /// read the description for Identity.Equals for more information
        /// </summary>
        public class Identity : IEquatable<Identity>
        {
            private object identity;

            public Identity(object identity)
            {
                this.identity = identity ?? throw new ArgumentNullException(nameof(identity));
            }

            public Identity(Identity identity)
            {
                if (identity == null) throw new ArgumentNullException(nameof(identity));
                this.identity = identity.identity ?? throw new ArgumentNullException(nameof(identity) + ".identity ( THIS SHOULD NEVER HAPPEN!!! )");
            }

            /// <summary>
            /// compares the identity of this.identity and obj
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public override bool Equals(object obj)
            {
                return Equals(obj is Identity identity ? identity : new Identity(obj));
            }

            bool floating_point_type_value_equals<T>(ref object a, ref object b, Func<T, bool> isNaN, Func<T, bool> isInfinity)
            {
                T va = (T)a;
                T vb = (T)b;
                return isNaN(va) ? isNaN(vb) : isInfinity(va) ? isInfinity(vb) : va.Equals(vb);
            }

            bool type_value_equals<T>(ref object a, ref object b)
            {
                switch (a)
                {
                    case float:
                        return floating_point_type_value_equals<float>(ref a, ref b, float.IsNaN, float.IsInfinity);
                    case double:
                        return floating_point_type_value_equals<double>(ref a, ref b, double.IsNaN, double.IsInfinity);
                    default:
                        {
                            T va = (T)a;
                            T vb = (T)b;
                            return va.Equals(vb);
                        }
                }
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public bool Equals(Identity other)
            {
                Type a = identity.GetType();
                Type b = other.identity.GetType();

                if (a != b) return false;

                switch (identity)
                {
                    case short:
                        return type_value_equals<short>(ref identity, ref other.identity);
                    case int:
                        return type_value_equals<int>(ref identity, ref other.identity);
                    case long:
                        return type_value_equals<long>(ref identity, ref other.identity);
                    case float:
                        return type_value_equals<float>(ref identity, ref other.identity);
                    case double:
                        return type_value_equals<double>(ref identity, ref other.identity);
                    case char:
                        return type_value_equals<char>(ref identity, ref other.identity);
                    case string:
                        return type_value_equals<string>(ref identity, ref other.identity);
                    default:
                        bool sameInstance = a.IsClass && b.IsClass && identity == other.identity;
                        return sameInstance || identity.Equals(other.identity);
                }
            }

            public override int GetHashCode()
            {
                return 1963189203 + EqualityComparer<object>.Default.GetHashCode(identity);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public static bool operator ==(Identity left, Identity right)
            {
                return EqualityComparer<Identity>.Default.Equals(left, right);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types do not match, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type.
            /// then it checks the result of a instance equality via the == operator and returns true upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the result of value equality comparison
            /// </summary>
            public static bool operator ==(Identity left, object right)
            {
                return EqualityComparer<Identity>.Default.Equals(left, right is Identity r_id ? r_id : new Identity(right));
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types matchs, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the inverse result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type
            /// then it checks the result of a instance equality via the == operator and returns false upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the inverse result of value equality comparison
            /// <br/>
            /// an inverse result of an equality comparison is the following:
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <term>Inverse Value</term>
            /// </listheader>
            /// <item>
            /// <term>True</term>
            /// <term>False</term>
            /// </item>
            /// <item>
            /// <term>False</term>
            /// <term>True</term>
            /// </item>
            /// </list>
            /// </summary>
            public static bool operator !=(Identity left, Identity right)
            {
                return !(left == right);
            }

            /// <summary>
            /// compares the identity of left and right
            /// <br/>
            /// if both objects's underlying types matchs, then it returns false
            /// <br/>
            /// if any of the following types
            /// <br/>
            /// short, int, long, float, double, char, string
            /// <br/>
            /// matches the type of the object,
            /// then it returns the inverse result of value equality comparison (with additional checks for floating point cases)
            /// <br/>
            /// otherwise if both objects are a reference type
            /// then it checks the result of a instance equality via the == operator and returns false upon success
            /// <br/>
            /// otherwise if both objects are not a reference type, or the instance equality check fails,
            /// it returns the inverse result of value equality comparison
            /// <br/>
            /// an inverse result of an equality comparison is the following:
            /// <list type="table">
            /// <listheader>
            /// <term>Value</term>
            /// <term>Inverse Value</term>
            /// </listheader>
            /// <item>
            /// <term>True</term>
            /// <term>False</term>
            /// </item>
            /// <item>
            /// <term>False</term>
            /// <term>True</term>
            /// </item>
            /// </list>
            /// </summary>
            public static bool operator !=(Identity left, object right)
            {
                return !(left == right);
            }

            public override string ToString()
            {
                return "[ this: " + base.ToString() + ", identity: " + identity + " ]";
            }
        }
    }
}
