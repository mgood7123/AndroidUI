/*
 * Copyright (C) 2010 The Android Open Source Project
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
    public interface IBidirectionalTypeConverter {
        public object convertBack(object value);
    }
    /**
     * Abstract base class used convert type T to another type V and back again. This
     * is necessary when the value types of in animation are different from the property
     * type. BidirectionalTypeConverter is needed when only the final value for the
     * animation is supplied to animators.
     * @see PropertyValuesHolder#setConverter(TypeConverter)
     */
    public abstract class BidirectionalTypeConverter<T, V> : TypeConverter<T, V>, IBidirectionalTypeConverter
    {
        private IBidirectionalTypeConverter mInvertedConverter;

        public BidirectionalTypeConverter(Type fromClass, Type toClass) : base(fromClass, toClass)
        {
        }

        /**
         * Does a conversion from the target type back to the source type. The subclass
         * must implement this when a TypeConverter is used in animations and current
         * values will need to be read for an animation.
         * @param value The Object to convert.
         * @return A value of type T, converted from <code>value</code>.
         */
        public abstract object convertBack(object value);

        /**
         * Returns the inverse of this converter, where the from and to classes are reversed.
         * The inverted converter uses this convert to call {@link #convertBack(Object)} for
         * {@link #convert(Object)} calls and {@link #convert(Object)} for
         * {@link #convertBack(Object)} calls.
         * @return The inverse of this converter, where the from and to classes are reversed.
         */
        public BidirectionalTypeConverter<V, T> invert()
        {
            if (mInvertedConverter == null)
            {
                mInvertedConverter = new InvertedConverter<V, T>(this);
            }
            return (BidirectionalTypeConverter<V, T>)mInvertedConverter;
        }

        private class InvertedConverter<From, To> : BidirectionalTypeConverter<From, To>
        {
            private BidirectionalTypeConverter<To, From> mConverter;

            public InvertedConverter(BidirectionalTypeConverter<To, From> converter) : base(converter.getTargetType(), converter.getSourceType())
            {
                mConverter = converter;
            }

            override
                public object convertBack(object value)
            {
                return mConverter.convert(value);
            }

            override
                public object convert(object value)
            {
                return mConverter.convertBack(value);
            }
        }
    }
}