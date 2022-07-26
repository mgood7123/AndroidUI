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

using AndroidUI.Widgets;

namespace AndroidUI.Utils.Widgets
{
    /**
     * Common base class of common implementation for an {@link Adapter} that can be
     * used in both {@link ListView} (by implementing the specialized
     * {@link ListAdapter} interface) and {@link Spinner} (by implementing the
     * specialized {@link SpinnerAdapter} interface).
     */
    public abstract class BaseAdapter<T> : Adapter<T>
    {
        private readonly DataSetObservable mDataSetObservable = new DataSetObservable();

        public bool hasStableIds()
        {
            return false;
        }

        public void registerDataSetObserver(DataSetObserver observer)
        {
            mDataSetObservable.registerObserver(observer);
        }

        public void unregisterDataSetObserver(DataSetObserver observer)
        {
            mDataSetObservable.unregisterObserver(observer);
        }

        /**
         * Notifies the attached observers that the underlying data has been changed
         * and any View reflecting the data set should refresh itself.
         */
        virtual public void notifyDataSetChanged()
        {
            mDataSetObservable.notifyChanged();
        }

        /**
         * Notifies the attached observers that the underlying data is no longer valid
         * or available. Once invoked this adapter is no longer valid and should
         * not report further data set changes.
         */
        public void notifyDataSetInvalidated()
        {
            mDataSetObservable.notifyInvalidated();
        }

        public bool isEnabled(int position)
        {
            return true;
        }

        public int getItemViewType(int position)
        {
            return 0;
        }

        public int getViewTypeCount()
        {
            return 1;
        }

        public bool isEmpty()
        {
            return getCount() == 0;
        }

        public abstract int getCount();
        public abstract T getItem(int position);
        public abstract long getItemId(int position);
        public abstract View getView(int position, View convertView, View parent);
    }
}