/*
 * Copyright (C) 2013 The Android Open Source Project
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
    public interface ITypeConverter
    {
        public Type getTargetType();
        public Type getSourceType();
        public object convert(object value);
    }
    /**
     * Abstract base class used convert type T to another type V. This
     * is necessary when the value types of in animation are different
     * from the property type.
     * @see PropertyValuesHolder#setConverter(TypeConverter)
     */
    public abstract class TypeConverter<T, V> : ITypeConverter
    {
        private Type mFromClass;
        private Type mToClass;

        public TypeConverter(Type fromClass, Type toClass)
        {
            mFromClass = fromClass;
            mToClass = toClass;
        }

        /**
         * Returns the target converted type. Used by the animation system to determine
         * the proper setter function to call.
         * @return The Class to convert the input to.
         */
        public Type getTargetType()
        {
            return mToClass;
        }

        /**
         * Returns the source conversion type.
         */
        public Type getSourceType()
        {
            return mFromClass;
        }

        /**
         * Converts a value from one type to another.
         * @param value The Object to convert.
         * @return A value of type V, converted from <code>value</code>.
         */
        public abstract object convert(object value);
    }
}