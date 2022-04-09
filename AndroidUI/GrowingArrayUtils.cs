/*
 * Copyright (C) 2007 The Android Open Source Project
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

namespace AndroidUI
{
    /**
     * A helper class that aims to provide comparable growth performance to ArrayList, but on primitive
     * arrays. Common array operations are implemented for efficient use in dynamic containers.
     *
     * All methods in this class assume that the length of an array is equivalent to its capacity and
     * NOT the number of elements in the array. The current size of the array is always passed in as a
     * parameter.
     *
     * @hide
     */
    public sealed class GrowingArrayUtils
    {

        /**
         * Appends an element to the end of the array, growing the array if there is no more room.
         * @param array The array to which to append the element. This must NOT be null.
         * @param currentSize The number of elements in the array. Must be less than or equal to
         *                    array.Length.
         * @param element The element to append.
         * @return the array to which the element was appended. This may be different than the given
         *         array.
         */
        public static T[] append<T>(T[] array, int currentSize, T element) where T : new()
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 > array.Length)
                {
                    T[] newArray = new T[growSize(currentSize)];
                    Arrays.arraycopy(array, 0, newArray, 0, currentSize);
                    array = newArray;
                }
                array[currentSize] = element;
                return array;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive int version of {@link #append(Object[], int, Object)}.
         */
        public static int[] append(int[] array, int currentSize, int element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 > array.Length)
                {
                    int[] newArray = new int[growSize(currentSize)];
                    Arrays.arraycopy(array, 0, newArray, 0, currentSize);
                    array = newArray;
                }
                array[currentSize] = element;
                return array;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive long version of {@link #append(Object[], int, Object)}.
         */
        public static long[] append(long[] array, int currentSize, long element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 > array.Length)
                {
                    long[] newArray = new long[growSize(currentSize)];
                    Arrays.arraycopy(array, 0, newArray, 0, currentSize);
                    array = newArray;
                }
                array[currentSize] = element;
                return array;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive bool version of {@link #append(Object[], int, Object)}.
         */
        public static bool[] append(bool[] array, int currentSize, bool element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 > array.Length)
                {
                    bool[] newArray = new bool[growSize(currentSize)];
                    Arrays.arraycopy(array, 0, newArray, 0, currentSize);
                    array = newArray;
                }
                array[currentSize] = element;
                return array;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive float version of {@link #append(Object[], int, Object)}.
         */
        public static float[] append(float[] array, int currentSize, float element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 > array.Length)
                {
                    float[] newArray = new float[growSize(currentSize)];
                    Arrays.arraycopy(array, 0, newArray, 0, currentSize);
                    array = newArray;
                }
                array[currentSize] = element;
                return array;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Inserts an element into the array at the specified index, growing the array if there is no
         * more room.
         *
         * @param array The array to which to append the element. Must NOT be null.
         * @param currentSize The number of elements in the array. Must be less than or equal to
         *                    array.Length.
         * @param element The element to insert.
         * @return the array to which the element was appended. This may be different than the given
         *         array.
         */
        public static T[] insert<T>(T[] array, int currentSize, int index, T element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 <= array.Length)
                {
                    Arrays.arraycopy(array, index, array, index + 1, currentSize - index);
                    array[index] = element;
                    return array;
                }

                T[] newArray = new T[growSize(currentSize)];
                Arrays.arraycopy(array, 0, newArray, 0, index);
                newArray[index] = element;
                Arrays.arraycopy(array, index, newArray, index + 1, array.Length - index);
                return newArray;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive int version of {@link #insert(Object[], int, int, Object)}.
         */
        public static int[] insert(int[] array, int currentSize, int index, int element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 <= array.Length)
                {
                    Arrays.arraycopy(array, index, array, index + 1, currentSize - index);
                    array[index] = element;
                    return array;
                }

                int[] newArray = new int[growSize(currentSize)];
                Arrays.arraycopy(array, 0, newArray, 0, index);
                newArray[index] = element;
                Arrays.arraycopy(array, index, newArray, index + 1, array.Length - index);
                return newArray;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive long version of {@link #insert(Object[], int, int, Object)}.
         */
        public static long[] insert(long[] array, int currentSize, int index, long element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 <= array.Length)
                {
                    Arrays.arraycopy(array, index, array, index + 1, currentSize - index);
                    array[index] = element;
                    return array;
                }

                long[] newArray = new long[growSize(currentSize)];
                Arrays.arraycopy(array, 0, newArray, 0, index);
                newArray[index] = element;
                Arrays.arraycopy(array, index, newArray, index + 1, array.Length - index);
                return newArray;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Primitive bool version of {@link #insert(Object[], int, int, Object)}.
         */
        public static bool[] insert(bool[] array, int currentSize, int index, bool element)
        {
            if (currentSize <= array.Length)
            {
                if (currentSize + 1 <= array.Length)
                {
                    Arrays.arraycopy(array, index, array, index + 1, currentSize - index);
                    array[index] = element;
                    return array;
                }

                bool[] newArray = new bool[growSize(currentSize)];
                Arrays.arraycopy(array, 0, newArray, 0, index);
                newArray[index] = element;
                Arrays.arraycopy(array, index, newArray, index + 1, array.Length - index);
                return newArray;
            }
            else
            {
                throw new Exception();
            }
        }

        /**
         * Given the current size of an array, returns an ideal size to which the array should grow.
         * This is typically double the given size, but should not be relied upon to do so in the
         * future.
         */
        public static int growSize(int currentSize)
        {
            return currentSize <= 4 ? 8 : currentSize * 2;
        }

        // Uninstantiable
        private GrowingArrayUtils() { }
    }
}