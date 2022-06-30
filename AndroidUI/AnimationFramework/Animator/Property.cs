/*
 * Copyright (C) 2011 The Android Open Source Project
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

namespace AndroidUI.AnimationFramework.Animator
{
    public interface IProperty {

        public static System.Reflection.MethodInfo TryGetMethod(Type holder, string full_name, Type t)
        {
            return holder.GetMethod(full_name, new Type[1] { t });
        }

        public static System.Reflection.MethodInfo TryGetMethod(Type holder, string full_name, Type[] t = null)
        {
            return holder.GetMethod(full_name, t ?? Type.EmptyTypes);
        }

        public static System.Reflection.FieldInfo TryGetField(Type holder, string name)
        {
            return holder.GetField(name);
        }

        public static string CapitalizeName(string name)
        {
            return char.ToUpper(name.ElementAt(0)) + name.Substring(1);
        }

        public static string Prepend(string prefix, string name)
        {
            return prefix + CapitalizeName(name);
        }
        private static Type FindGenericType(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return toCheck;
                }
                toCheck = toCheck.BaseType;
            }
            return null;
        }

        public static bool InheritsProperty(Type t)
        {
            return FindGenericType(typeof(Property<,>), t) == null;
        }
        public static bool InheritsPropertyAndFirstTypeIs<T>(Type t)
        {
            Type gen = FindGenericType(typeof(Property<,>), t);
            if (gen == null) return false;
            return gen.GenericTypeArguments[0] == typeof(T);
        }
        public static bool InheritsPropertyAndSecondTypeIs<T>(Type t)
        {
            Type gen = FindGenericType(typeof(Property<,>), t);
            if (gen == null) return false;
            return gen.GenericTypeArguments[1] == typeof(T);
        }

        public bool isReadOnly();
        public void set(object obj, object value);
        public object get(object obj);
        public string getName();
        public Type getType();
    }


    /**
     * A property is an abstraction that can be used to represent a <emb>mutable</em> value that is held
     * in a <em>host</em> object. The Property's {@link #set(Object, Object)} or {@link #get(Object)}
     * methods can be implemented in terms of the private fields of the host object, or via "setter" and
     * "getter" methods or by some other mechanism, as appropriate.
     *
     * @param <T> The class on which the property is declared.
     * @param <V> The type that this property represents.
     */
    public abstract class Property<T, V> : IProperty
    {
        private readonly string mName;
        private readonly Type mType;

        /**
         * This factory method creates and returns a Property given the <code>class</code> and
         * <code>name</code> parameters, where the <code>"name"</code> parameter represents either:
         * <ul>
         *     <li>a public <code>getName()</code> method on the class which takes no arguments, plus an
         *     optional public <code>setName()</code> method which takes a value of the same type
         *     returned by <code>getName()</code>
         *     <li>a public <code>isName()</code> method on the class which takes no arguments, plus an
         *     optional public <code>setName()</code> method which takes a value of the same type
         *     returned by <code>isName()</code>
         *     <li>a public <code>name</code> field on the class
         * </ul>
         *
         * <p>If either of the get/is method alternatives is found on the class, but an appropriate
         * <code>setName()</code> method is not found, the <code>Property</code> will be
         * {@link #isReadOnly() readOnly}. Calling the {@link #set(Object, Object)} method on such
         * a property is allowed, but will have no effect.</p>
         *
         * <p>If neither the methods nor the field are found on the class a
         * {@link NoSuchPropertyException} exception will be thrown.</p>
         */
        public static Property<T, V> of<T, V>(Type hostType, Type valueType, string name)
        {
            return new ReflectiveProperty<T, V>(hostType, valueType, name);
        }

        /**
         * A constructor that takes an identifying name and {@link #getType() type} for the property.
         */
        public Property(Type type, string name)
        {
            mName = name;
            mType = type;
        }

        /**
         * Returns true if the {@link #set(Object, Object)} method does not set the value on the target
         * object (in which case the {@link #set(Object, Object) set()} method should throw a {@link
         * NoSuchPropertyException} exception). This may happen if the Property wraps functionality that
         * allows querying the underlying value but not setting it. For example, the {@link #of(Class,
         * Class, String)} factory method may return a Property with name "foo" for an object that has
         * only a <code>getFoo()</code> or <code>isFoo()</code> method, but no matching
         * <code>setFoo()</code> method.
         */
        virtual public bool isReadOnly()
        {
            return false;
        }

        /**
         * Sets the value on <code>object</code> which this property represents. If the method is unable
         * to set the value on the target object it will throw an {@link UnsupportedOperationException}
         * exception.
         */
        virtual public void set(object obj, object value)
        {
            throw new NotSupportedException("Property " + getName() + " is read-only");
        }

        /**
         * Returns the current value that this property represents on the given <code>object</code>.
         */
        public virtual object get(object obj) { return null; }

        /**
         * Returns the name for this property.
         */
        public string getName()
        {
            return mName;
        }

        /**
         * Returns the type for this property.
         */
        public Type getType()
        {
            return mType;
        }
    }
}