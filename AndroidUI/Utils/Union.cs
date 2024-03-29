﻿namespace AndroidUI.Utils
{
    /// <summary>
    /// creates a new Union with the specified types
    /// </summary>
    class Union
    {
        public static unsafe TDest ReinterpretCast<TSource, TDest>(TSource source)
        {
            var sourceRef = __makeref(source);
            var dest = default(TDest);
            var destRef = __makeref(dest);
            *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
            return __refvalue(destRef, TDest);
        }

        object storage;
        List<Type> stored_types;

        private static string TypesToString(List<Type> types)
        {
            if (types == null)
                return "null";

            int iMax = types.Count - 1;
            if (iMax == -1)
                return "[]";

            System.Text.StringBuilder b = new();
            b.Append('[');
            for (int i = 0; ; i++)
            {
                Type c = types[i];
                b.Append(c == null ? "<null>" : c.Name);
                if (i == iMax)
                    return b.Append(']').ToString();
                b.Append(", ");
            }
        }

        /// <summary>
        /// creates a new Union with the specified types
        /// </summary>
        public Union(params object[] types)
        {
            stored_types = new();
            foreach (object obj in types)
            {
                Type type;
                if (obj is Type)
                {
                    type = (Type)obj;
                }
                else
                {
                    type = obj.GetType();
                }
                stored_types.Add(type);
            }
        }

        /// <summary>
        /// creates a new union with the same stored types as the specified union
        /// <br></br>
        /// <br></br>
        /// DOES NOT copy the contents of the specified union
        /// <br></br>
        /// </summary>
        public Union(Union union)
        {
            stored_types = new(union.stored_types);
        }

        public interface IBindable
        {
            void SetValue(object value);
            T GetValue<T>();
        }

        public class ValueBindable<T> : IBindable
        {
            byte[] data;
            List<Type> valid_types;

            public ValueBindable(int size_in_bytes)
            {
                data = new byte[size_in_bytes];
            }

            public ValueBindable(int size_in_bytes, List<Type> valid_types)
            {
                data = new byte[size_in_bytes];
                this.valid_types = valid_types;
            }

            public void SetValue(object value)
            {
                T[] a = ReinterpretCast<object, T[]>(data);
                a[0] = (T)value;
            }

            public T GetValue<T>()
            {
                if (valid_types != null)
                {
                    if (!valid_types.Contains(typeof(T)))
                    {
                        throw new InvalidCastException("T must be one of the following types that where specified when this object was created: " + TypesToString(valid_types));
                    }
                }

                T value;

                if (typeof(T).IsArray)
                {
                    value = ReinterpretCast<object, T>(data);
                }
                else
                {
                    unsafe
                    {
                        fixed (byte* array = data)
                        {
                            if (typeof(T) == typeof(sbyte))
                            {
                                sbyte source = *(sbyte*)array;
                                value = ReinterpretCast<sbyte, T>(source);
                            }
                            else if (typeof(T) == typeof(byte))
                            {
                                byte source = *array;
                                value = ReinterpretCast<byte, T>(source);
                            }
                            else if (typeof(T) == typeof(bool))
                            {
                                bool source = *(bool*)array;
                                value = ReinterpretCast<bool, T>(source);
                            }
                            else if (typeof(T) == typeof(ushort))
                            {
                                ushort source = *(ushort*)array;
                                value = ReinterpretCast<ushort, T>(source);
                            }
                            else if (typeof(T) == typeof(short))
                            {
                                short source = *(short*)array;
                                value = ReinterpretCast<short, T>(source);
                            }
                            else if (typeof(T) == typeof(char))
                            {
                                char source = *(char*)array;
                                value = ReinterpretCast<char, T>(source);
                            }
                            else if (typeof(T) == typeof(uint))
                            {
                                uint source = *(uint*)array;
                                value = ReinterpretCast<uint, T>(source);
                            }
                            else if (typeof(T) == typeof(int))
                            {
                                int source = *(int*)array;
                                value = ReinterpretCast<int, T>(source);
                            }
                            else if (typeof(T) == typeof(float))
                            {
                                float source = *(float*)array;
                                value = ReinterpretCast<float, T>(source);
                            }
                            else if (typeof(T) == typeof(ulong))
                            {
                                ulong source = *(ulong*)array;
                                value = ReinterpretCast<ulong, T>(source);
                            }
                            else if (typeof(T) == typeof(long))
                            {
                                long source = *(long*)array;
                                value = ReinterpretCast<long, T>(source);
                            }
                            else if (typeof(T) == typeof(double))
                            {
                                double source = *(double*)array;
                                value = ReinterpretCast<double, T>(source);
                            }
                            else if (typeof(T) == typeof(decimal))
                            {
                                decimal source = *(decimal*)array;
                                value = ReinterpretCast<decimal, T>(source);
                            }
                            else
                            {
                                throw new InvalidCastException("T must be an Unmanaged Type excluding nint and nuint");
                            }
                        }
                    }
                }
                return value;
            }
        }

        public class ReferenceBindable<T> : IBindable
        {
            object data;
            List<Type> valid_types;

            public ReferenceBindable()
            {
            }

            public ReferenceBindable(List<Type> valid_types)
            {
                this.valid_types = valid_types;
            }

            public void SetValue(object value)
            {
                data = value;
            }

            public T GetValue<T>()
            {
                if (valid_types != null)
                {
                    if (!valid_types.Contains(typeof(T)))
                    {
                        throw new InvalidCastException("T must be one of the following types that where specified when this object was created: " + TypesToString(valid_types));
                    }
                }

                return ReinterpretCast<object, T>(data);
            }
        }

        /// <summary>
        /// sets the current value
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if given invalid type
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        public void set<T>(T obj)
        {
            if (stored_types.Contains(typeof(T)))
            {
                IBindable bind;
                if (!typeof(T).IsArray && !typeof(T).IsClass && typeof(T).IsPrimitive && typeof(T).IsValueType)
                {
                    // wrap primative in array object
                    bind = new ValueBindable<T>(CastUtils.SizeOfUnmanagedType<T>(), stored_types);
                }
                else
                {
                    bind = new ReferenceBindable<T>(stored_types);
                }

                bind.SetValue(obj);
                storage = bind;
            }
            else
            {
                throw new InvalidCastException("T must be one of the following types that where specified when this object was created: " + TypesToString(stored_types));
            }
        }

        /// <summary>
        /// obtains the current value, reinterpreted as the specified type
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if given invalid type
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        public T get<T>()
        {
            if (stored_types.Contains(typeof(T)))
            {
                if (storage == null)
                {
                    return (T)storage;
                }
                IBindable bind = getBindable<T>();
                return bind.GetValue<T>();
            }
            else
            {
                throw new InvalidCastException("T must be one of the following types that where specified when this object was created: " + TypesToString(stored_types));
            }
        }

        /// <summary>
        /// use get&lt;T&gt; instead of using this directly
        /// <br></br>
        /// <br></br>
        /// throws InvalidCastException if given invalid type
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        internal IBindable getBindable<T>()
        {
            if (stored_types.Contains(typeof(T)))
            {
                return (IBindable)storage;
            }
            else
            {
                throw new InvalidCastException("T must be one of the following types that where specified when this object was created: " + TypesToString(stored_types));
            }
        }
    }
}