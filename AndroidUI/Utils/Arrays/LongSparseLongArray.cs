﻿/*
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

namespace AndroidUI.Utils.Arrays
{
    /**
    * Map of {@code long} to {@code long}. Unlike a normal array of longs, there
    * can be gaps in the indices. It is intended to be more memory efficient than using a
    * {@code HashMap}, both because it avoids
    * auto-boxing keys and values and its data structure doesn't rely on an extra entry object
    * for each mapping.
    *
    * <p>Note that this container keeps its mappings in an array data structure,
    * using a binary search to find keys.  The implementation is not intended to be appropriate for
    * data structures
    * that may contain large numbers of items.  It is generally slower than a traditional
    * HashMap, since lookups require a binary search and adds and removes require inserting
    * and deleting entries in the array.  For containers holding up to hundreds of items,
    * the performance difference is not significant, less than 50%.</p>
    *
    * <p>It is possible to iterate over the items in this container using
    * {@link #keyAt(int)} and {@link #valueAt(int)}. Iterating over the keys using
    * <code>keyAt(int)</code> with ascending values of the index will return the
    * keys in ascending order, or the values corresponding to the keys in ascending
    * order in the case of <code>valueAt(int)</code>.</p>
    *
    * @hide
    */
    public class LongSparseLongArray
    {
        private long[] mKeys;
        private long[] mValues;
        private int mSize;

        /**
         * Creates a new SparseLongArray containing no mappings.
         */
        public LongSparseLongArray() : this(10)
        {
        }

        /**
         * Creates a new SparseLongArray containing no mappings that will not
         * require any additional memory allocation to store the specified
         * number of mappings.  If you supply an initial capacity of 0, the
         * sparse array will be initialized with a light-weight representation
         * not requiring any additional array allocations.
         */
        public LongSparseLongArray(int initialCapacity)
        {
            if (initialCapacity == 0)
            {
                mKeys = EmptyArray.LONG;
                mValues = EmptyArray.LONG;
            }
            else
            {
                mKeys = new long[initialCapacity];
                mValues = new long[mKeys.Length];
            }
            mSize = 0;
        }

        public LongSparseLongArray clone()
        {
            LongSparseLongArray clone = new();
            clone.mSize = mSize;
            clone.mKeys = new long[mKeys.Length];
            Array.Copy(mKeys, clone.mKeys, mKeys.Length);
            clone.mValues = new long[mValues.Length];
            Array.Copy(mValues, clone.mValues, mValues.Length);
            return clone;
        }

        /**
         * Gets the long mapped from the specified key, or <code>0</code>
         * if no such mapping has been made.
         */
        public long get(long key)
        {
            return get(key, 0);
        }

        /**
         * Gets the long mapped from the specified key, or the specified value
         * if no such mapping has been made.
         */
        public long get(long key, long valueIfKeyNotFound)
        {
            int i = Arrays.binarySearch(mKeys, mSize, key);

            if (i < 0)
            {
                return valueIfKeyNotFound;
            }
            else
            {
                return mValues[i];
            }
        }

        /**
         * Removes the mapping from the specified key, if there was any.
         */
        public void delete(long key)
        {
            int i = Arrays.binarySearch(mKeys, mSize, key);

            if (i >= 0)
            {
                removeAt(i);
            }
        }

        /**
         * Removes the mapping at the given index.
         */
        public void removeAt(int index)
        {
            Arrays.arraycopy(mKeys, index + 1, mKeys, index, mSize - (index + 1));
            Arrays.arraycopy(mValues, index + 1, mValues, index, mSize - (index + 1));
            mSize--;
        }

        /**
         * Adds a mapping from the specified key to the specified value,
         * replacing the previous mapping from the specified key if there
         * was one.
         */
        public void put(long key, long value)
        {
            int i = Arrays.binarySearch(mKeys, mSize, key);

            if (i >= 0)
            {
                mValues[i] = value;
            }
            else
            {
                i = ~i;

                mKeys = GrowingArrayUtils.insert(mKeys, mSize, i, key);
                mValues = GrowingArrayUtils.insert(mValues, mSize, i, value);
                mSize++;
            }
        }

        /**
         * Returns the number of key-value mappings that this SparseIntArray
         * currently stores.
         */
        public int size()
        {
            return mSize;
        }

        /**
         * Given an index in the range <code>0...size()-1</code>, returns
         * the key from the <code>index</code>th key-value mapping that this
         * SparseLongArray stores.
         *
         * <p>The keys corresponding to indices in ascending order are guaranteed to
         * be in ascending order, e.g., <code>keyAt(0)</code> will return the
         * smallest key and <code>keyAt(size()-1)</code> will return the largest
         * key.</p>
         *
         * <p>For indices outside of the range <code>0...size()-1</code>, the behavior is undefined for
         * apps targeting {@link android.os.Build.VERSION_CODES#P} and earlier, and an
         * {@link ArrayIndexOutOfBoundsException} is thrown for apps targeting
         * {@link android.os.Build.VERSION_CODES#Q} and later.</p>
         */
        public long keyAt(int index)
        {
            if (index >= mSize)
            {
                // The array might be slightly bigger than mSize, in which case, indexing won't fail.
                // Check if exception should be thrown outside of the critical path.
                throw new IndexOutOfRangeException("index");
            }
            return mKeys[index];
        }

        /**
         * Given an index in the range <code>0...size()-1</code>, returns
         * the value from the <code>index</code>th key-value mapping that this
         * SparseLongArray stores.
         *
         * <p>The values corresponding to indices in ascending order are guaranteed
         * to be associated with keys in ascending order, e.g.,
         * <code>valueAt(0)</code> will return the value associated with the
         * smallest key and <code>valueAt(size()-1)</code> will return the value
         * associated with the largest key.</p>
         *
         * <p>For indices outside of the range <code>0...size()-1</code>, the behavior is undefined for
         * apps targeting {@link android.os.Build.VERSION_CODES#P} and earlier, and an
         * {@link ArrayIndexOutOfBoundsException} is thrown for apps targeting
         * {@link android.os.Build.VERSION_CODES#Q} and later.</p>
         */
        public long valueAt(int index)
        {
            if (index >= mSize)
            {
                // The array might be slightly bigger than mSize, in which case, indexing won't fail.
                // Check if exception should be thrown outside of the critical path.
                throw new IndexOutOfRangeException("index");
            }
            return mValues[index];
        }

        /**
         * Returns the index for which {@link #keyAt} would return the
         * specified key, or a negative number if the specified
         * key is not mapped.
         */
        public int indexOfKey(long key)
        {
            return Arrays.binarySearch(mKeys, mSize, key);
        }

        /**
         * Returns an index for which {@link #valueAt} would return the
         * specified key, or a negative number if no keys map to the
         * specified value.
         * Beware that this is a linear search, unlike lookups by key,
         * and that multiple keys can map to the same value and this will
         * find only one of them.
         */
        public int indexOfValue(long value)
        {
            for (int i = 0; i < mSize; i++)
                if (mValues[i] == value)
                    return i;

            return -1;
        }

        /**
         * Removes all key-value mappings from this SparseIntArray.
         */
        public void clear()
        {
            mSize = 0;
        }

        /**
         * Puts a key/value pair into the array, optimizing for the case where
         * the key is greater than all existing keys in the array.
         */
        public void append(long key, long value)
        {
            if (mSize != 0 && key <= mKeys[mSize - 1])
            {
                put(key, value);
                return;
            }

            mKeys = GrowingArrayUtils.append(mKeys, mSize, key);
            mValues = GrowingArrayUtils.append(mValues, mSize, value);
            mSize++;
        }

        /**
         * {@inheritDoc}
         *
         * <p>This implementation composes a string by iterating over its mappings.
         */
        public override string ToString()
        {
            if (size() <= 0)
            {
                return "{}";
            }

            System.Text.StringBuilder buffer = new(mSize * 28);
            buffer.Append('{');
            for (int i = 0; i < mSize; i++)
            {
                if (i > 0)
                {
                    buffer.Append(", ");
                }
                long key = keyAt(i);
                buffer.Append(key);
                buffer.Append('=');
                long value = valueAt(i);
                buffer.Append(value);
            }
            buffer.Append('}');
            return buffer.ToString();
        }

        ///**
        // * @hide
        // */
        //public static class Parcelling implements
        //        com.android.internal.util.Parcelling<LongSparseLongArray> {
        //@Override
        //public void parcel(LongSparseLongArray array, Parcel dest, int parcelFlags)
        //{
        //    if (array == null)
        //    {
        //        dest.writeInt(-1);
        //        return;
        //    }

        //    dest.writeInt(array.mSize);
        //    dest.writeLongArray(array.mKeys);
        //    dest.writeLongArray(array.mValues);
        //}

        //@Override
        //public LongSparseLongArray unparcel(Parcel source)
        //{
        //    int size = source.readInt();
        //    if (size == -1)
        //    {
        //        return null;
        //    }

        //    LongSparseLongArray array = new LongSparseLongArray(0);

        //    array.mSize = size;
        //    array.mKeys = source.createLongArray();
        //    array.mValues = source.createLongArray();

        //    // Make sure array is valid
        //    Preconditions.checkArgument(array.mKeys.length >= size);
        //    Preconditions.checkArgument(array.mValues.length >= size);

        //    if (size > 0)
        //    {
        //        long last = array.mKeys[0];
        //        for (int i = 1; i < size; i++)
        //        {
        //            Preconditions.checkArgument(last < array.mKeys[i]);
        //        }
        //    }

        //    return array;
        //}
    }
}