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

using AndroidUI.Utils;
using AndroidUI.Widgets;

namespace AndroidUI.Applications
{
    public class Context
    {
        public sealed class Storage
        {
            class ObjectMap
            {
                class Item
                {
                    internal WeakReference<Thread> associatedThread;
                    internal string key;
                    internal object value;
                }

                Item[] Map = Array.Empty<Item>();

                object null_object;

                public object NullObj => null_object;
                private readonly object LOCK = new();

                internal void SetValue<T>(string key, ref object value, Func<T> default_value_creator, Thread thread)
                {
                    Item item_;
                    lock (LOCK)
                    {
                        foreach (Item item in Map)
                        {
                            bool is_thread = true;
                            if (item.associatedThread != null && item.associatedThread.TryGetTarget(out Thread associated))
                            {
                                is_thread = associated == thread;
                            }
                            if (is_thread && item.key.Equals(key, StringComparison.Ordinal))
                            {
                                item.value = value;
                                return;
                            }
                        }

                        Array.Resize(ref Map, Map.Length + 1);

                        item_ = new();
                        Map[Map.Length - 1] = item_;
                    }
                    item_.key = key;

                    if (default_value_creator == null)
                    {
                        item_.value = null;
                    }
                    else
                    {
                        item_.value = new ValueHolder<T>(default_value_creator.Invoke());
                    }

                    if (thread != null)
                    {
                        item_.associatedThread = new(thread);
                    }
                }

                internal ref object GetValue(string key, Thread thread)
                {
                    lock (LOCK)
                    {
                        foreach (Item item in Map)
                        {
                            bool is_thread = true;
                            if (item.associatedThread != null && item.associatedThread.TryGetTarget(out Thread associated))
                            {
                                is_thread = associated == thread;
                            }
                            if (is_thread && item.key.Equals(key, StringComparison.Ordinal))
                            {
                                return ref item.value;
                            }
                        }
                    }
                    return ref null_object;
                }

                internal ref object GetOrSetValue<T>(string key, Func<T> default_value_creator, Thread thread)
                {
                    Item item_;
                    lock (LOCK)
                    {
                        foreach (Item item in Map)
                        {
                            bool is_thread = true;
                            if (item.associatedThread != null && item.associatedThread.TryGetTarget(out Thread associated))
                            {
                                is_thread = associated == thread;
                            }
                            if (is_thread && item.key.Equals(key, StringComparison.Ordinal))
                            {
                                return ref item.value;
                            }
                        }

                        Array.Resize(ref Map, Map.Length + 1);

                        item_ = new();
                        Map[Map.Length - 1] = item_;
                    }
                    item_.key = key;
                    if (default_value_creator == null)
                    {
                        item_.value = null;
                    }
                    else
                    {
                        item_.value = new ValueHolder<T>(default_value_creator.Invoke());
                    }

                    if (thread != null)
                    {
                        item_.associatedThread = new(thread);
                    }
                    return ref item_.value;
                }
            }

            readonly ObjectMap map;

            public Storage()
            {
                map = new();
            }

            /// <summary>
            /// this should be able to hold both reference types and value types
            /// <br></br>
            /// <br></br> a reference type is stored inside a reference, and then stored if it's key does not exist.
            /// <br></br> all further retrievals return a reference to the reference type
            /// <br></br>
            /// <br></br> a value type is copied inside a reference to avoid further copies,
            /// and then stored if it's key does not exist.
            /// <br></br> all further retrievals return a reference to the value type, avoiding a copy
            /// </summary>
            public ValueHolder<T> GetOrCreate<T>(string key, Func<T> default_value_creator, Thread thread = null)
            {
                return (ValueHolder<T>)map.GetOrSetValue(key, default_value_creator, thread);
            }

            /// <summary>
            /// this should be able to hold both reference types and value types
            /// <br></br>
            /// <br></br> a reference type is stored inside a reference, and then stored if it's key does not exist.
            /// <br></br> all further retrievals return a reference to the reference type
            /// <br></br>
            /// <br></br> a value type is copied inside a reference to avoid further copies,
            /// and then stored if it's key does not exist.
            /// <br></br> all further retrievals return a reference to the value type, avoiding a copy
            /// </summary>
            public void SetOrCreate<T>(string key, ValueHolder<T> value, Func<T> default_value_creator, Thread thread = null)
            {
                object holder = value;
                map.SetValue(key, ref holder, default_value_creator, thread);
            }

            /// <returns>null if the key does not exist</returns>
            public ValueHolder<T> Get<T>(string key, Thread thread = null)
            {
                ref object obj = ref map.GetValue(key, thread);
                return obj == map.NullObj ? null : (ValueHolder<T>)obj;
            }
        }

        public readonly Application application;
        public readonly Storage storage;

        internal View.AttachInfo mAttachInfo;

        public Context() : this(null)
        {
        }

        public Context(Application application)
        {
            mAttachInfo = new(this);

            if (application != null)
            {
                if (application.context != null)
                {
                    throw new ApplicationException("Application already has a context");
                }
                application.context = this;
                this.application = application;
            }
            storage = new();
        }
    }
}