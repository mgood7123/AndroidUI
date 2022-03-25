namespace AndroidUI
{
    public class BitwiseList<T> : List<T>
    {
        public bool shouldCheckInstanceEquality = true;

        public sealed class Zero { };
        public sealed class NegativeOne { };

        /// <summary>
        /// a constant for integer zero: 0
        /// <br/>
        /// this should be used when testing for bits
        /// </summary>
        public static readonly Zero ZERO = new();
        /// <summary>
        /// a constant for integer negative one: -1
        /// <br/>
        /// this should be used when testing for bits
        /// </summary>
        public static readonly NegativeOne NEGATIVE_ONE = new();

        public BitwiseList() : base()
        {
        }

        public BitwiseList(object right) : base(new BitwiseList<T>() | right)
        {
        }

        public BitwiseList(IEnumerable<T> collection) : base(collection)
        {
        }

        public BitwiseList(int capacity) : base(capacity)
        {
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Count; i++)
            {
                str += this[i];
                if (i + 1 != Count) str += ", ";
            }
            return str;
        }



        /// <summary>
        /// bitwise EQUAL
        /// <br/>
        /// use ZERO to explicitly test if all bits are 0 (no bits set)
        /// <br/>
        /// use NEGATIVE_ONE to explicitly test if zero or more bits are set (will always return true)
        /// <br/>
        /// NOTE: unlike a binary bitwise comparison,
        /// comparing against -1 cannot be done for a list since we use -1 to
        /// represent an empty list which also matches ZERO since an empty
        /// list has no "bits" set in it (it has no objects)
        /// <br/>
        /// if you need to explicitly test if ALL bits are set, use the following:
        /// <br/>
        /// list.NotEquals(ZERO)
        /// </summary>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                // check if we compare against 0
                case Zero: return Count == 0;



                // check if we compare against -1
                // NOTE: an empty list is equivilent to -1

                case NegativeOne: return Count >= 0;

                // comparing against another list
                case BitwiseList<T>:
                    {
                        BitwiseList<T> other = (BitwiseList<T>)obj;
                        if (other == null) return false;
                        if (Count != other.Count) return false;
                        for (int i = 0; i < Count; i++)
                        {
                            T a = this.ElementAt(i);
                            T b = other.ElementAt(i);
                            if (shouldCheckInstanceEquality || other.shouldCheckInstanceEquality)
                            {
                                if (a.GetType().IsClass && b.GetType().IsClass)
                                {
                                    if ((object)a != (object)b)
                                    {
                                        return false;
                                    }
                                }
                            }
                            if (!a.Equals(b))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                // comparing against another list
                case IEnumerable<T>:
                    {
                        IEnumerable<T> other = (IEnumerable<T>)obj;
                        if (other == null) return false;
                        if (Count != other.Count()) return false;
                        for (int i = 0; i < Count; i++)
                        {
                            T a = this.ElementAt(i);
                            T b = other.ElementAt(i);
                            if (shouldCheckInstanceEquality)
                            {
                                if (a.GetType().IsClass && b.GetType().IsClass)
                                {
                                    if ((object)a != (object)b)
                                    {
                                        return false;
                                    }
                                }
                            }
                            if (!a.Equals(b))
                            {
                                return false;
                            }
                        }
                        return true;
                    }

                // comparing against single object
                case T:
                    {
                        if (Count != 1) return false;
                        T a = this.ElementAt(0);
                        if (shouldCheckInstanceEquality && a.GetType().IsClass && obj.GetType().IsClass)
                        {
                            if ((object)a != (object)obj)
                            {
                                return false;
                            }
                        }

                        if (!a.Equals(obj))
                        {
                            return false;
                        }

                        return false;
                    }

                default:
                    return false;
            }
        }

        /// <summary>
        /// bitwise NOT EQUAL
        /// <br/>
        /// use ZERO to explicitly test if all bits are set
        /// <br/>
        /// NEGATIVE_ONE will always return false
        /// <br/>
        /// NOTE: unlike a binary bitwise comparison,
        /// comparing against -1 cannot be done for a list since we use -1 to
        /// represent an empty list which also matches ZERO (in a bitwise EQUALS operation)
        /// since an empty list has no "bits" set in it (it has no objects)
        /// <br/>
        /// if you need to explicitly test if ALL bits are set, use the following:
        /// <br/>
        /// list.NotEquals(ZERO)
        /// </summary>
        public bool NotEquals(object obj)
        {
            return !Equals(obj);
        }


        /// <summary>
        /// bitwise EQUAL
        /// <br/>
        /// use ZERO to explicitly test if all bits are 0 (no bits set)
        /// <br/>
        /// use NEGATIVE_ONE to explicitly test if zero or more bits are set (will always return true)
        /// <br/>
        /// NOTE: unlike a binary bitwise comparison,
        /// comparing against -1 cannot be done for a list since we use -1 to
        /// represent an empty list which also matches ZERO since an empty
        /// list has no "bits" set in it (it has no objects)
        /// <br/>
        /// if you need to explicitly test if ALL bits are set, use the following:
        /// <br/>
        /// list != ZERO
        /// </summary>
        public static bool operator ==(BitwiseList<T> left, object right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// bitwise NOT EQUAL
        /// <br/>
        /// use ZERO to explicitly test if all bits are set
        /// <br/>
        /// NEGATIVE_ONE will always return false
        /// <br/>
        /// NOTE: unlike a binary bitwise comparison,
        /// comparing against -1 cannot be done for a list since we use -1 to
        /// represent an empty list which also matches ZERO (in a bitwise EQUALS operation)
        /// since an empty list has no "bits" set in it (it has no objects)
        /// <br/>
        /// if you need to explicitly test if ALL bits are set, use the following:
        /// <br/>
        /// list != ZERO
        /// </summary>
        public static bool operator !=(BitwiseList<T> left, object right)
        {
            return !(left == right);
        }

        static void assertType(ref object obj)
        {
            switch (obj)
            {
                case BitwiseList<T>:
                case IEnumerable<T>:
                case T:
                    break;
                default:
                    throw new ArrayTypeMismatchException();
            }
        }

        bool ContainsElement(ref T element, bool shouldCheckInstanceEquality = false)
        {
            if (Count == 0) return false;

            bool isClass = false;

            if (this.shouldCheckInstanceEquality || shouldCheckInstanceEquality)
            {
                isClass = element.GetType().IsClass;
            }

            for (int i = 0; i < Count; i++)
            {
                T b = this.ElementAt(i);
                if (shouldCheckInstanceEquality || this.shouldCheckInstanceEquality)
                {
                    if (isClass && b.GetType().IsClass)
                    {
                        return (object)element == (object)b;
                    }
                }
                return element.Equals(b);
            }
            return false;
        }

        static bool IEnumerableContainsElement(ref IEnumerable<T> this_, ref T element, bool shouldCheckInstanceEquality = false)
        {
            int count = this_.Count();
            if (count == 0) return false;

            bool isClass = false;

            bool e = false;
            if (this_ is BitwiseList<T>)
            {
                e = ((BitwiseList<T>)this_).shouldCheckInstanceEquality;
            }

            if (e || shouldCheckInstanceEquality)
            {
                isClass = element.GetType().IsClass;
            }

            for (int i = 0; i < count; i++)
            {
                T b = this_.ElementAt(i);
                if (shouldCheckInstanceEquality)
                {
                    if (isClass && b.GetType().IsClass)
                    {
                        return (object)element == (object)b;
                    }
                }
                return element.Equals(b);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -439212823;
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + shouldCheckInstanceEquality.GetHashCode();
            hashCode = hashCode * -1521134295 + IS_NOT.GetHashCode();
            return hashCode;
        }

        bool IS_NOT = false;

        /// <summary>
        /// bitwise NOT ( ~ a )
        /// <br/>
        /// returns a "bit-flipped" list
        /// <br/>
        /// see 'bitwise AND ( a &amp; b )' and 'bitwise OR ( a | b )' for more details on how this affects operations
        /// </summary>
        public static BitwiseList<T> operator ~(BitwiseList<T> left)
        {
            BitwiseList<T> tmp = new(left);
            tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
            tmp.IS_NOT = !tmp.IS_NOT;
            return tmp;
        }

        /// <summary>
        /// bitwise AND ( a &amp; b )
        /// <br/>
        /// returns objects only if they are present in both lists
        /// <br/>
        /// when given a "bit-flipped" list ( 'bitwise NOT ( ~a )' )
        /// <br/>
        /// if the list is empty, returns an empty list with no objects
        /// <br/>
        /// this is because in binary, -1 represents all bits set to 1, and 101 &amp; 111 returns itself which is 101
        /// <br/>
        /// if the list is not empty, returns a list that is the inverse of what would be returned if not given a "bit-flipped" list
        /// <br/>
        /// this is because in binary, 101 &amp; ~011 translates to 101 &amp; 100 which returns 100
        /// </summary>
        public static BitwiseList<T> operator &(BitwiseList<T> left, object right)
        {
            if (right is Zero)
            {
                throw new ArgumentException("ZERO is not valid for a bitwise AND operation");
            }

            if (right is not NegativeOne) assertType(ref right);

            switch (right)
            {
                case NegativeOne:
                    {
                        // an empty list is equivilant to -1
                        // and `a &= -1` simply results in `a`

                        BitwiseList<T> t = new(left);
                        t.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                        return t;
                    }
                case BitwiseList<T>:
                    {
                        BitwiseList<T> rhs = (BitwiseList<T>)right;

                        if (rhs.IS_NOT)
                        {
                            BitwiseList<T> tmp = new();
                            tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality || rhs.shouldCheckInstanceEquality;

                            // an empty list is equivilant to -1
                            // and `a &= ~(-1)` removes ALL bits from `a`

                            if (rhs.Count == 0) return tmp;

                            for (int i = 0; i < left.Count; i++)
                            {
                                T a = left.ElementAt(i);
                                if (!rhs.ContainsElement(ref a, left.shouldCheckInstanceEquality))
                                {
                                    tmp.Add(a);
                                }
                            }
                            return tmp;
                        }
                        else
                        {
                            if (rhs.Count == 0)
                            {
                                // an empty list is equivilant to -1
                                // and `a &= -1` simply results in `a`

                                BitwiseList<T> tmp = new(left);
                                tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                                return tmp;
                            }
                            else
                            {
                                BitwiseList<T> tmp = new();
                                tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality || rhs.shouldCheckInstanceEquality;

                                for (int i = 0; i < left.Count; i++)
                                {
                                    T a = left.ElementAt(i);
                                    if (rhs.ContainsElement(ref a, left.shouldCheckInstanceEquality))
                                    {
                                        tmp.Add(a);
                                    }
                                }
                                return tmp;
                            }
                        }
                    }
                case IEnumerable<T>:
                    {
                        BitwiseList<T> tmp = new();
                        tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                        IEnumerable<T> rhs = (IEnumerable<T>)right;

                        for (int i = 0; i < left.Count; i++)
                        {
                            T a = left.ElementAt(i);
                            if (IEnumerableContainsElement(ref rhs, ref a, left.shouldCheckInstanceEquality))
                            {
                                tmp.Add(a);
                            }
                        }
                        return tmp;
                    }
                case T:
                    {
                        if (left.Count != 1)
                        {
                            BitwiseList<T> t = new();
                            t.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                            return t;
                        }

                        T A = left.ElementAt(0);

                        if (left.shouldCheckInstanceEquality)
                        {
                            if ((object)A != (object)right)
                            {
                                BitwiseList<T> t = new();
                                t.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                                return t;
                            }
                        }

                        if (!A.Equals(right))
                        {
                            BitwiseList<T> t = new();
                            t.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                            return t;
                        }

                        BitwiseList<T> t_ = new(left);
                        t_.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                        return t_;
                    }
                default:
                    throw new ArrayTypeMismatchException();
            }
        }

        /// <summary>
        /// bitwise OR ( a | b )
        /// <br/>
        /// adds object if not present in list
        /// <br/>
        /// when given a "bit-flipped" list ( 'bitwise NOT ( ~a )' )
        /// <br/>
        /// if the list is empty, returns an empty list with no objects
        /// <br/>
        /// this is because in binary, -1 represents all bits set to 1, and INTEGER | -1 returns -1 because INTEGER contains NO bits that are NOT PRESENT in -1
        /// <br/>
        /// if the list is not empty, returns a list with no objects added
        /// <br/>
        /// this is because in binary, 101 | ~011 translates to 101 | 100 which returns 101 as [1]00 is already present in [1]01
        /// </summary>
        public static BitwiseList<T> operator |(BitwiseList<T> left, object right)
        {
            if (right is Zero)
            {
                throw new ArgumentException("ZERO is not valid for a bitwise OR operation");
            }

            if (right is not NegativeOne) assertType(ref right);

            switch (right)
            {
                case NegativeOne:
                    {
                        // an null object is equivilant to empty list
                        // which is equivilant to -1
                        // and `a |= -1` simply results in `-1`
                        // and the above assert ensures that the integer is -1

                        BitwiseList<T> tmp = new();
                        tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                        return tmp;
                    }

                // we need only add any missing elements, merging both lists into one
                case BitwiseList<T>:
                    {
                        BitwiseList<T> rhs = (BitwiseList<T>)right;
                        if (rhs.Count == 0)
                        {
                            // an empty list is equivilant to -1

                            if (rhs.IS_NOT)
                            {
                                // and `a |= ~(-1)` simply results in `a`

                                BitwiseList<T> tmp = new(left);
                                tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality || rhs.shouldCheckInstanceEquality;
                                return tmp;
                            }
                            else
                            {
                                // and `a |= -1` simply results in `-1`

                                BitwiseList<T> tmp = new();
                                tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality || rhs.shouldCheckInstanceEquality;
                                return tmp;
                            }
                        }
                        else
                        {
                            BitwiseList<T> tmp = new();
                            tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality || rhs.shouldCheckInstanceEquality;

                            for (int i = 0; i < left.Count; i++)
                            {
                                tmp.Add(left.ElementAt(i));
                            }

                            for (int i = 0; i < rhs.Count; i++)
                            {
                                T a = left.ElementAt(i);
                                if (!tmp.ContainsElement(ref a))
                                {
                                    tmp.Add(a);
                                }
                            }

                            return tmp;
                        }
                    }
                case IEnumerable<T>:
                    {
                        BitwiseList<T> tmp = new();
                        IEnumerable<T> rhs = (IEnumerable<T>)right;
                        tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;

                        for (int i = 0; i < left.Count; i++)
                        {
                            tmp.Add(left.ElementAt(i));
                        }

                        int c = rhs.Count();

                        for (int i = 0; i < c; i++)
                        {
                            T a = left.ElementAt(i);
                            if (!tmp.ContainsElement(ref a))
                            {
                                tmp.Add(a);
                            }
                        }

                        return tmp;
                    }

                case T:
                    {
                        BitwiseList<T> tmp = new(left);
                        tmp.shouldCheckInstanceEquality = left.shouldCheckInstanceEquality;
                        T r = (T)right;
                        if (!tmp.ContainsElement(ref r))
                        {
                            tmp.Add(r);
                        }
                        return tmp;
                    }
                default:
                    throw new ArrayTypeMismatchException();
            }
        }
    }
}
