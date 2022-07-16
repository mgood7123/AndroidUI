/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace AndroidUI.Utils.Widgets
{
    /**
    * An Insets instance holds four integer offsets which describe changes to the four
    * edges of a Rectangle. By convention, positive values move edges towards the
    * centre of the rectangle.
    * <p>
    * Insets are immutable so may be treated as values.
    *
    */
    public sealed class Insets
    {
        public static readonly Insets NONE = new(0, 0, 0, 0);

        public readonly int left;
        public readonly int top;
        public readonly int right;
        public readonly int bottom;

        private Insets(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        // Factory methods

        /**
        * Return an Insets instance with the appropriate values.
        *
        * @param left the left inset
        * @param top the top inset
        * @param right the right inset
        * @param bottom the bottom inset
        *
        * @return Insets instance with the appropriate values
        */
        public static Insets of(int left, int top, int right, int bottom)
        {
            if (left == 0 && top == 0 && right == 0 && bottom == 0)
            {
                return NONE;
            }
            return new Insets(left, top, right, bottom);
        }

        /**
        * Return an Insets instance with the appropriate values.
        *
        * @param r the rectangle from which to take the values
        *
        * @return an Insets instance with the appropriate values
        */
        public static Insets of(Rect r)
        {
            return r == null ? NONE : of(r.left, r.top, r.right, r.bottom);
        }

        /**
        * Returns a Rect instance with the appropriate values.
        *
        * @hide
        */
        public Rect toRect()
        {
            return new Rect(left, top, right, bottom);
        }

        /**
        * Add two Insets.
        *
        * @param a The first Insets to add.
        * @param b The second Insets to add.
        * @return a + b, i. e. all insets on every side are added together.
        */
        public static Insets add(Insets a, Insets b)
        {
            return of(a.left + b.left, a.top + b.top, a.right + b.right, a.bottom + b.bottom);
        }

        /**
        * Subtract two Insets.
        *
        * @param a The minuend.
        * @param b The subtrahend.
        * @return a - b, i. e. all insets on every side are subtracted from each other.
        */
        public static Insets subtract(Insets a, Insets b)
        {
            return of(a.left - b.left, a.top - b.top, a.right - b.right, a.bottom - b.bottom);
        }

        /**
        * Retrieves the maximum of two Insets.
        *
        * @param a The first Insets.
        * @param b The second Insets.
        * @return max(a, b), i. e. the larger of every inset on every side is taken for the result.
        */
        public static Insets max(Insets a, Insets b)
        {
            return of(Math.Max(a.left, b.left), Math.Max(a.top, b.top),
                    Math.Max(a.right, b.right), Math.Max(a.bottom, b.bottom));
        }

        /**
        * Retrieves the minimum of two Insets.
        *
        * @param a The first Insets.
        * @param b The second Insets.
        * @return min(a, b), i. e. the smaller of every inset on every side is taken for the result.
        */
        public static Insets min(Insets a, Insets b)
        {
            return of(Math.Min(a.left, b.left), Math.Min(a.top, b.top),
                    Math.Min(a.right, b.right), Math.Min(a.bottom, b.bottom));
        }

        /**
        * Two Insets instances are equal iff they belong to the same class and their fields are
        * pairwise equal.
        *
        * @param o the object to compare this instance with.
        *
        * @return true iff this object is equal {@code o}
        */
        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;

            Insets insets = (Insets)o;

            if (bottom != insets.bottom) return false;
            if (left != insets.left) return false;
            if (right != insets.right) return false;
            if (top != insets.top) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int result = left;
            result = 31 * result + top;
            result = 31 * result + right;
            result = 31 * result + bottom;
            return result;
        }



        public override string ToString()
        {
            return "Insets{" +
                    "left=" + left +
                    ", top=" + top +
                    ", right=" + right +
                    ", bottom=" + bottom +
                    '}';
        }
    }
}