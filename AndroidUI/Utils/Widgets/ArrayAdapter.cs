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

using AndroidUI.Widgets;

namespace AndroidUI.Utils.Widgets
{

    /**
     * You can use this adapter to provide views for an {@link AdapterView},
     * Returns a view for each object in a collection of data objects you
     * provide, and can be used with list-based user interface widgets such as
     * {@link ListView} or {@link Spinner}.
     * <p>
     * By default, the array adapter creates a view by calling {@link Object#toString()} on each
     * data object in the collection you provide, and places the result in a TextView.
     * You may also customize what type of view is used for the data object in the collection.
     * To customize what type of view is used for the data object,
     * override {@link #getView(int, View, ViewGroup)}
     * and inflate a view resource.
     * </p>
     * <p>
     * For an example of using an array adapter with a ListView, see the
     * <a href="{@docRoot}guide/topics/ui/declaring-layout.html#AdapterViews">
     * Adapter Views</a> guide.
     * </p>
     * <p>
     * For an example of using an array adapter with a Spinner, see the
     * <a href="{@docRoot}guide/topics/ui/controls/spinner.html">Spinners</a> guide.
     * </p>
     * <p class="note"><strong>Note:</strong>
     * If you are considering using array adapter with a ListView, consider using
     * {@link android.support.v7.widget.RecyclerView} instead.
     * RecyclerView offers similar features with better performance and more flexibility than
     * ListView provides.
     * See the
     * <a href="https://developer.android.com/guide/topics/ui/layout/recyclerview.html">
     * Recycler View</a> guide.</p>
     */
    public class ArrayAdapter<T> : BaseAdapter<T>
    {
        /**
         * Lock used to modify the content of {@link #mObjects}. Any write operation
         * performed on the array should be synchronized on this lock. This lock is also
         * used by the filter (see {@link #getFilter()} to make a synchronized copy of
         * the original array of data.
         */
        private readonly Object mLock = new Object();

        /**
         * Contains the list of objects that represent the data of this ArrayAdapter.
         * The content of this list is referred to as "the array" in the documentation.
         */
        private List<T> mObjects;

        /**
         * Indicates whether or not {@link #notifyDataSetChanged()} must be called whenever
         * {@link #mObjects} is modified.
         */
        private bool mNotifyOnChange = true;

        RunnableWithReturn<Topten_RichTextKit_TextView> creator;

        /**
         * Constructor. This constructor will result in the underlying data collection being
         * immutable, so methods such as {@link #clear()} will throw an exception.
         *
         * @param creator The creator to use to create text views.
         * @param objects The objects to represent in the ListView.
         */
        public ArrayAdapter(RunnableWithReturn<Topten_RichTextKit_TextView> creator, params T[] objects)
            : this(creator, objects.ToList())
        {
        }

        public ArrayAdapter(RunnableWithReturn<Topten_RichTextKit_TextView> creator, List<T> objects)
        {
            mObjects = objects;
            this.creator = creator;
        }

        /**
         * Adds the specified object at the end of the array.
         *
         * @param object The object to add at the end of the array.
         * @throws UnsupportedOperationException if the underlying data collection is immutable
         */
        public void add(T obj)
        {
            lock (mLock)
            {
                mObjects.Add(obj);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Adds the specified Collection at the end of the array.
         *
         * @param collection The Collection to add at the end of the array.
         * @throws UnsupportedOperationException if the <tt>addAll</tt> operation
         *         is not supported by this list
         * @throws ClassCastException if the class of an element of the specified
         *         collection prevents it from being added to this list
         * @throws NullPointerException if the specified collection contains one
         *         or more null elements and this list does not permit null
         *         elements, or if the specified collection is null
         * @throws IllegalArgumentException if some property of an element of the
         *         specified collection prevents it from being added to this list
         */
        public void addAll(ICollection<T> collection)
        {
            lock (mLock)
            {
                mObjects.AddRange(collection);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Adds the specified items at the end of the array.
         *
         * @param items The items to add at the end of the array.
         * @throws UnsupportedOperationException if the underlying data collection is immutable
         */
        public void addAll(params T[] items)
        {
            lock (mLock)
            {
                mObjects.AddRange(items);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Inserts the specified object at the specified index in the array.
         *
         * @param object The object to insert into the array.
         * @param index The index at which the object must be inserted.
         * @throws UnsupportedOperationException if the underlying data collection is immutable
         */
        public void insert(T obj, int index)
        {
            lock (mLock)
            {
                mObjects.Insert(index, obj);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Removes the specified object from the array.
         *
         * @param object The object to remove.
         * @throws UnsupportedOperationException if the underlying data collection is immutable
         */
        public void remove(T obj)
        {
            lock (mLock)
            {
                mObjects.Remove(obj);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Remove all elements from the list.
         *
         * @throws UnsupportedOperationException if the underlying data collection is immutable
         */
        public void clear()
        {
            lock (mLock)
            {
                mObjects.Clear();
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        /**
         * Sorts the content of this adapter using the specified comparator.
         *
         * @param comparator The comparator used to sort the objects contained
         *        in this adapter.
         */
        public void sort(IComparer<T> comparator)
        {
            lock (mLock)
            {
                mObjects.Sort(comparator);
            }
            if (mNotifyOnChange) notifyDataSetChanged();
        }

        override
        public void notifyDataSetChanged()
        {
            base.notifyDataSetChanged();
            mNotifyOnChange = true;
        }

        /**
         * Control whether methods that change the list ({@link #add}, {@link #addAll(Collection)},
         * {@link #addAll(Object[])}, {@link #insert}, {@link #remove}, {@link #clear},
         * {@link #sort(Comparator)}) automatically call {@link #notifyDataSetChanged}.  If set to
         * false, caller must manually call notifyDataSetChanged() to have the changes
         * reflected in the attached view.
         *
         * The default is true, and calling notifyDataSetChanged()
         * resets the flag to true.
         *
         * @param notifyOnChange if true, modifications to the list will
         *                       automatically call {@link
         *                       #notifyDataSetChanged}
         */
        public void setNotifyOnChange(bool notifyOnChange)
        {
            mNotifyOnChange = notifyOnChange;
        }

        override
        public int getCount()
        {
            return mObjects.Count;
        }

        override
        public T getItem(int position)
        {
            return mObjects[position];
        }

        /**
         * Returns the position of the specified item in the array.
         *
         * @param item The item to retrieve the position of.
         *
         * @return The position of the specified item.
         */
        public int getPosition(T item)
        {
            return mObjects.IndexOf(item);
        }

        override
        public long getItemId(int position)
        {
            return position;
        }

        override
        public View getView(int position, View convertView, View parent)
        {
            View view;
            Topten_RichTextKit_TextView text;

            if (convertView == null)
            {
                view = creator.Invoke();
            }
            else
            {
                view = convertView;
            }

            //  If no custom field is assigned, assume the whole resource is a TextView
            text = (Topten_RichTextKit_TextView)view;

            T item = getItem(position);
            if (item is string s)
            {
                text.setText(s);
            }
            else
            {
                text.setText(item.ToString());
            }

            return view;
        }
    }
}