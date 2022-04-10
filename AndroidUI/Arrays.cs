using AndroidUI.Extensions;
using System.Text;

namespace AndroidUI
{
    public static class Arrays
    {
        // This is Arrays.binarySearch(), but doesn't do any argument validation.
        public static int binarySearch(int[] array, int size, int value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi).UnsignedShift(1);
                int midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }
            return ~lo;  // value not present
        }

        public static int binarySearch(long[] array, int size, long value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi).UnsignedShift(1);
                long midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }
            return ~lo;  // value not present
        }

        /**
         * Returns {@code true} if the two specified arrays of object are
         * <i>equal</i> to one another.  Two arrays are considered equal if both
         * arrays contain the same number of elements, and all corresponding pairs
         * of elements in the two arrays are equal.  In other words, two arrays
         * are equal if they contain the same elements in the same order.  Also,
         * two array references are considered equal if both are {@code null}.
         *
         * @param a one array to be tested for equality
         * @param a2 the other array to be tested for equality
         * @return {@code true} if the two arrays are equal
         */
        public static bool equals<T>(T[] a, T[] a2)
        {
            if (a == a2)
                return true;
            if (a == null || a2 == null)
                return false;

            int length = a.Length;
            if (a2.Length != length)
                return false;

            for (int i = 0; i < length; i++)
            {
                object a_ = a[i];
                object a2_ = a2[i];
                if (!ContainerHelpers.value_type_equals(ref a_, ref a2_))
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Copies an array from the specified source array, beginning at the
         * specified position, to the specified position of the destination array.
         * A subsequence of array components are copied from the source
         * array referenced by <code>src</code> to the destination array
         * referenced by <code>dest</code>. The number of components copied is
         * equal to the <code>length</code> argument. The components at
         * positions <code>srcPos</code> through
         * <code>srcPos+length-1</code> in the source array are copied into
         * positions <code>destPos</code> through
         * <code>destPos+length-1</code>, respectively, of the destination
         * array.
         * <p>
         * If the <code>src</code> and <code>dest</code> arguments refer to the
         * same array object, then the copying is performed as if the
         * components at positions <code>srcPos</code> through
         * <code>srcPos+length-1</code> were first copied to a temporary
         * array with <code>length</code> components and then the contents of
         * the temporary array were copied into positions
         * <code>destPos</code> through <code>destPos+length-1</code> of the
         * destination array.
         * <p>
         * If <code>dest</code> is <code>null</code>, then a
         * <code>NullReferenceException</code> is thrown.
         * <p>
         * If <code>src</code> is <code>null</code>, then a
         * <code>NullReferenceException</code> is thrown and the destination
         * array is not modified.
         * <p>
         * Otherwise, if any of the following is true, an
         * <code>ArrayStoreException</code> is thrown and the destination is
         * not modified:
         * <ul>
         * <li>The <code>src</code> argument refers to an object that is not an
         *     array.
         * <li>The <code>dest</code> argument refers to an object that is not an
         *     array.
         * <li>The <code>src</code> argument and <code>dest</code> argument refer
         *     to arrays whose component types are different primitive types.
         * <li>The <code>src</code> argument refers to an array with a primitive
         *    component type and the <code>dest</code> argument refers to an array
         *     with a reference component type.
         * <li>The <code>src</code> argument refers to an array with a reference
         *    component type and the <code>dest</code> argument refers to an array
         *     with a primitive component type.
         * </ul>
         * <p>
         * Otherwise, if any of the following is true, an
         * <code>IndexOutOfBoundsException</code> is
         * thrown and the destination is not modified:
         * <ul>
         * <li>The <code>srcPos</code> argument is negative.
         * <li>The <code>destPos</code> argument is negative.
         * <li>The <code>length</code> argument is negative.
         * <li><code>srcPos+length</code> is greater than
         *     <code>src.Length</code>, the length of the source array.
         * <li><code>destPos+length</code> is greater than
         *     <code>dest.Length</code>, the length of the destination array.
         * </ul>
         * <p>
         * Otherwise, if any actual component of the source array from
         * position <code>srcPos</code> through
         * <code>srcPos+length-1</code> cannot be converted to the component
         * type of the destination array by assignment conversion, an
         * <code>ArrayStoreException</code> is thrown. In this case, let
         * <b><i>k</i></b> be the smallest nonnegative integer less than
         * length such that <code>src[srcPos+</code><i>k</i><code>]</code>
         * cannot be converted to the component type of the destination
         * array; when the exception is thrown, source array components from
         * positions <code>srcPos</code> through
         * <code>srcPos+</code><i>k</i><code>-1</code>
         * will already have been copied to destination array positions
         * <code>destPos</code> through
         * <code>destPos+</code><i>k</I><code>-1</code> and no other
         * positions of the destination array will have been modified.
         * (Because of the restrictions already itemized, this
         * paragraph effectively applies only to the situation where both
         * arrays have component types that are reference types.)
         *
         * @param      src      the source array.
         * @param      srcPos   starting position in the source array.
         * @param      dest     the destination array.
         * @param      destPos  starting position in the destination data.
         * @param      length   the number of array elements to be copied.
         * @exception  IndexOutOfBoundsException  if copying would cause
         *               access of data outside array bounds.
         * @exception  ArrayStoreException  if an element in the <code>src</code>
         *               array could not be stored into the <code>dest</code> array
         *               because of a type mismatch.
         * @exception  NullReferenceException if either <code>src</code> or
         *               <code>dest</code> is <code>null</code>.
         */
        public static void arraycopy(Array src, int srcPos,
                                            Array dest, int destPos,
                                            int length)
        {
            Array.ConstrainedCopy(src, srcPos, dest, destPos, length);
        }

        /**
         * Copies the specified array, truncating or padding with zeros (if necessary)
         * so the copy has the specified length.  For all indices that are
         * valid in both the original array and the copy, the two arrays will
         * contain identical values.  For any indices that are valid in the
         * copy but not the original, the copy will contain {@code 0f}.
         * Such indices will exist if and only if the specified length
         * is greater than that of the original array.
         *
         * @param original the array to be copied
         * @param newLength the length of the copy to be returned
         * @return a copy of the original array, truncated or padded with zeros
         *     to obtain the specified length
         * @throws NegativeArraySizeException if {@code newLength} is negative
         * @throws NullReferenceException if {@code original} is null
         * @since 1.6
         */
        public static float[] copyOf(float[] original, int newLength)
        {
            float[] copy = new float[newLength];
            arraycopy(original, 0, copy, 0,
                             Math.Min(original.Length, newLength));
            return copy;
        }


        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code long} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Long}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(long[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (long element in a) {
                int elementHash = (int)(element ^ (element.UnsignedShift(32)));
                result = 31 * result + elementHash;
            }

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two non-null {@code int} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Integer}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(int[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (int element in a)
                result = 31 * result + element;

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code short} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Short}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(short[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (short element in a)
                result = 31 * result + element;

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code char} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Character}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(char[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (char element in a)
                result = 31 * result + element;

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code byte} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Byte}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(byte[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (byte element in a)
                result = 31 * result + element;

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code bool} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link bool}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(bool[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (bool element in a)
                result = 31 * result + (element ? 1231 : 1237);

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code float} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Float}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(float[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (float element in a)
                result = 31 * result + element.ToIntBits();

            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.
         * For any two {@code double} arrays {@code a} and {@code b}
         * such that {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is the same value that would be
         * obtained by invoking the {@link List#hashCode() hashCode}
         * method on a {@link List} containing a sequence of {@link Double}
         * instances representing the elements of {@code a} in the same order.
         * If {@code a} is {@code null}, this method returns 0.
         *
         * @param a the array whose hash value to compute
         * @return a content-based hash code for {@code a}
         * @since 1.5
         */
        public static int hashCode(double[] a) {
            if (a == null)
                return 0;

            int result = 1;
            foreach (double element in a) {
                long bits = element.ToLongBits();
                result = 31 * result + (int)(bits ^ (bits.UnsignedShift(32)));
            }
            return result;
        }

        /**
         * Returns a hash code based on the contents of the specified array.  If
         * the array contains other arrays as elements, the hash code is based on
         * their identities rather than their contents.  It is therefore
         * acceptable to invoke this method on an array that contains itself as an
         * element,  either directly or indirectly through one or more levels of
         * arrays.
         *
         * <p>For any two arrays {@code a} and {@code b} such that
         * {@code Arrays.equals(a, b)}, it is also the case that
         * {@code Arrays.hashCode(a) == Arrays.hashCode(b)}.
         *
         * <p>The value returned by this method is equal to the value that would
         * be returned by {@code Arrays.asList(a).hashCode()}, unless {@code a}
         * is {@code null}, in which case {@code 0} is returned.
         *
         * @param a the array whose content-based hash code to compute
         * @return a content-based hash code for {@code a}
         * @see #deepHashCode(Object[])
         * @since 1.5
         */
        public static int hashCode(object[] a) {
            if (a == null)
                return 0;

            int result = 1;

            foreach (object element in a)
                result = 31 * result + (element == null ? 0 : element.GetHashCode());

            return result;
        }

        /**
         * Returns a hash code based on the "deep contents" of the specified
         * array.  If the array contains other arrays as elements, the
         * hash code is based on their contents and so on, ad infinitum.
         * It is therefore unacceptable to invoke this method on an array that
         * contains itself as an element, either directly or indirectly through
         * one or more levels of arrays.  The behavior of such an invocation is
         * undefined.
         *
         * <p>For any two arrays {@code a} and {@code b} such that
         * {@code Arrays.deepEquals(a, b)}, it is also the case that
         * {@code Arrays.deepHashCode(a) == Arrays.deepHashCode(b)}.
         *
         * <p>The computation of the value returned by this method is similar to
         * that of the value returned by {@link List#hashCode()} on a list
         * containing the same elements as {@code a} in the same order, with one
         * difference: If an element {@code e} of {@code a} is itself an array,
         * its hash code is computed not by calling {@code e.hashCode()}, but as
         * by calling the appropriate overloading of {@code Arrays.hashCode(e)}
         * if {@code e} is an array of a primitive type, or as by calling
         * {@code Arrays.deepHashCode(e)} recursively if {@code e} is an array
         * of a reference type.  If {@code a} is {@code null}, this method
         * returns 0.
         *
         * @param a the array whose deep-content-based hash code to compute
         * @return a deep-content-based hash code for {@code a}
         * @see #hashCode(Object[])
         * @since 1.5
         */
        public static int deepHashCode(object[] a) {
            if (a == null)
                return 0;

            int result = 1;

            foreach (object element in a) {
                int elementHash;
                Type cl;
                if (element == null)
                    elementHash = 0;
                else if ((cl = element.GetType().GetElementType()) == null)
                    elementHash = element.GetHashCode();
                else if (element is object[])
                    elementHash = deepHashCode((object[])element);
                else
                    elementHash = primitiveArrayHashCode(element, cl);

                result = 31 * result + elementHash;
            }

            return result;
        }

        private static int primitiveArrayHashCode(object a, Type cl) {
            return
                (cl == typeof(byte)) ? hashCode((byte[])a) :
                (cl == typeof(int)) ? hashCode((int[])a) :
                (cl == typeof(long)) ? hashCode((long[])a) :
                (cl == typeof(char)) ? hashCode((char[])a) :
                (cl == typeof(short)) ? hashCode((short[])a) :
                (cl == typeof(bool)) ? hashCode((bool[])a) :
                (cl == typeof(double)) ? hashCode((double[])a) :
                // If new primitive types are ever added, this method must be
                // expanded or we will fail here with ClassCastException.
                hashCode((float[])a);
        }

        /**
         * Returns {@code true} if the two specified arrays are <i>deeply
         * equal</i> to one another.  Unlike the {@link #equals(Object[],Object[])}
         * method, this method is appropriate for use with nested arrays of
         * arbitrary depth.
         *
         * <p>Two array references are considered deeply equal if both
         * are {@code null}, or if they refer to arrays that contain the same
         * number of elements and all corresponding pairs of elements in the two
         * arrays are deeply equal.
         *
         * <p>Two possibly {@code null} elements {@code e1} and {@code e2} are
         * deeply equal if any of the following conditions hold:
         * <ul>
         *    <li> {@code e1} and {@code e2} are both arrays of object reference
         *         types, and {@code Arrays.deepEquals(e1, e2) would return true}
         *    <li> {@code e1} and {@code e2} are arrays of the same primitive
         *         type, and the appropriate overloading of
         *         {@code Arrays.equals(e1, e2)} would return true.
         *    <li> {@code e1 == e2}
         *    <li> {@code e1.equals(e2)} would return true.
         * </ul>
         * Note that this definition permits {@code null} elements at any depth.
         *
         * <p>If either of the specified arrays contain themselves as elements
         * either directly or indirectly through one or more levels of arrays,
         * the behavior of this method is undefined.
         *
         * @param a1 one array to be tested for equality
         * @param a2 the other array to be tested for equality
         * @return {@code true} if the two arrays are equal
         * @see #equals(Object[],Object[])
         * @see Objects#deepEquals(Object, Object)
         * @since 1.5
         */
        public static bool deepEquals(object[] a1, object[] a2) {
            if (a1 == a2)
                return true;
            if (a1 == null || a2 == null)
                return false;
            int length = a1.Length;
            if (a2.Length != length)
                return false;

            for (int i = 0; i < length; i++) {
                object e1 = a1[i];
                object e2 = a2[i];

                if (e1 == e2)
                    continue;
                if (e1 == null)
                    return false;

                // Figure out whether the two elements are equal
                bool eq = deepEquals0(e1, e2);

                if (!eq)
                    return false;
            }
            return true;
        }

        static bool deepEquals0(object e1, object e2) {
            if (e1 == null) throw new ArgumentNullException();
            bool eq;
            if (e1 is object[] && e2 is object[])
                eq = deepEquals((object[])e1, (object[])e2);
            else if (e1 is byte[] && e2 is byte[])
                eq = equals((byte[])e1, (byte[])e2);
            else if (e1 is short[] && e2 is short[])
                eq = equals((short[])e1, (short[])e2);
            else if (e1 is int[] && e2 is int[])
                eq = equals((int[])e1, (int[])e2);
            else if (e1 is long[] && e2 is long[])
                eq = equals((long[])e1, (long[])e2);
            else if (e1 is char[] && e2 is char[])
                eq = equals((char[])e1, (char[])e2);
            else if (e1 is float[] && e2 is float[])
                eq = equals((float[])e1, (float[])e2);
            else if (e1 is double[] && e2 is double[])
                eq = equals((double[])e1, (double[])e2);
            else if (e1 is bool[] && e2 is bool[])
                eq = equals((bool[])e1, (bool[])e2);
            else
                eq = e1.Equals(e2);
            return eq;
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(long)}.  Returns {@code "null"} if {@code a}
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(long[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(int)}.  Returns {@code "null"} if {@code a} is
         * {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(int[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(short)}.  Returns {@code "null"} if {@code a}
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(short[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(char)}.  Returns {@code "null"} if {@code a}
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(char[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements
         * are separated by the characters {@code ", "} (a comma followed
         * by a space).  Elements are converted to strings as by
         * {@code String.valueOf(byte)}.  Returns {@code "null"} if
         * {@code a} is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(byte[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(bool)}.  Returns {@code "null"} if
         * {@code a} is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(bool[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(float)}.  Returns {@code "null"} if {@code a}
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(float[] a) {
            if (a == null)
                return "null";

            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * The string representation consists of a list of the array's elements,
         * enclosed in square brackets ({@code "[]"}).  Adjacent elements are
         * separated by the characters {@code ", "} (a comma followed by a
         * space).  Elements are converted to strings as by
         * {@code String.valueOf(double)}.  Returns {@code "null"} if {@code a}
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @since 1.5
         */
        public static string toString(double[] a) {
            if (a == null)
                return "null";
            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                b.Append(a[i]);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the contents of the specified array.
         * If the array contains other arrays as elements, they are converted to
         * strings by the {@link Object#toString} method inherited from
         * {@code Object}, which describes their <i>identities</i> rather than
         * their contents.
         *
         * <p>The value returned by this method is equal to the value that would
         * be returned by {@code Arrays.asList(a).toString()}, unless {@code a}
         * is {@code null}, in which case {@code "null"} is returned.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @see #deepToString(Object[])
         * @since 1.5
         */
        public static string toString(object[] a) {
            if (a == null)
                return "null";

            int iMax = a.Length - 1;
            if (iMax == -1)
                return "[]";

            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; ; i++) {
                object c = a[i];
                b.Append(c == null ? "<null>" : c.ToString());
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /**
         * Returns a string representation of the "deep contents" of the specified
         * array.  If the array contains other arrays as elements, the string
         * representation contains their contents and so on.  This method is
         * designed for converting multidimensional arrays to strings.
         *
         * <p>The string representation consists of a list of the array's
         * elements, enclosed in square brackets ({@code "[]"}).  Adjacent
         * elements are separated by the characters {@code ", "} (a comma
         * followed by a space).  Elements are converted to strings as by
         * {@code String.valueOf(Object)}, unless they are themselves
         * arrays.
         *
         * <p>If an element {@code e} is an array of a primitive type, it is
         * converted to a string as by invoking the appropriate overloading of
         * {@code Arrays.toString(e)}.  If an element {@code e} is an array of a
         * reference type, it is converted to a string as by invoking
         * this method recursively.
         *
         * <p>To avoid infinite recursion, if the specified array contains itself
         * as an element, or contains an indirect reference to itself through one
         * or more levels of arrays, the self-reference is converted to the string
         * {@code "[...]"}.  For example, an array containing only a reference
         * to itself would be rendered as {@code "[[...]]"}.
         *
         * <p>This method returns {@code "null"} if the specified array
         * is {@code null}.
         *
         * @param a the array whose string representation to return
         * @return a string representation of {@code a}
         * @see #toString(Object[])
         * @since 1.5
         */
        public static string deepToString(object[] a) {
            if (a == null)
                return "null";

            int bufLen = 20 * a.Length;
            if (a.Length != 0 && bufLen <= 0)
                bufLen = int.MaxValue;
            StringBuilder buf = new StringBuilder(bufLen);
            deepToString(a, buf, new HashSet<object[]>());
            return buf.ToString();
        }

        private static void deepToString(object[] a, StringBuilder buf,
                                         HashSet<object[]> dejaVu) {
            if (a == null) {
                buf.Append("null");
                return;
            }
            int iMax = a.Length - 1;
            if (iMax == -1) {
                buf.Append("[]");
                return;
            }

            dejaVu.Add(a);
            buf.Append('[');
            for (int i = 0; ; i++) {

                object element = a[i];
                if (element == null) {
                    buf.Append("null");
                } else {
                    Type eClass = element.GetType();

                    if (eClass.IsArray) {
                        if (eClass == typeof(byte[]))
                            buf.Append(toString((byte[])element));
                        else if (eClass == typeof(short[]))
                            buf.Append(toString((short[])element));
                        else if (eClass == typeof(int[]))
                            buf.Append(toString((int[])element));
                        else if (eClass == typeof(long[]))
                            buf.Append(toString((long[])element));
                        else if (eClass == typeof(char[]))
                            buf.Append(toString((char[])element));
                        else if (eClass == typeof(float[]))
                            buf.Append(toString((float[])element));
                        else if (eClass == typeof(double[]))
                            buf.Append(toString((double[])element));
                        else if (eClass == typeof(bool[]))
                            buf.Append(toString((bool[])element));
                        else { // element is an array of object references
                            if (dejaVu.Contains(element))
                                buf.Append("[...]");
                            else
                                deepToString((object[])element, buf, dejaVu);
                        }
                    } else {  // element is non-null and not an array
                        buf.Append(element.ToString());
                    }
                }
                if (i == iMax)
                    break;
                buf.Append(", ");
            }
            buf.Append(']');
            dejaVu.Remove(a);
        }
    }
}
