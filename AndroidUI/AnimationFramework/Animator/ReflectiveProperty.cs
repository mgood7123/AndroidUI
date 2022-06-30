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

using System.Reflection;

namespace AndroidUI.AnimationFramework.Animator
{
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

    /**
     * Internal class to automatically generate a Property for a given class/name pair, given the
     * specification of {@link Property#of(java.lang.Class, java.lang.Class, java.lang.String)}
     */
    class ReflectiveProperty<T, V> : Property<T, V>
    {

        private const string PREFIX_GET = "get";
        private const string PREFIX_IS = "is";
        private const string PREFIX_SET = "set";
        private MethodInfo mSetter;
        private MethodInfo mGetter;
        private FieldInfo mField;

        /**
         * For given property name 'name', look for getName method or 'name' field.
         * Also look for setName method (optional - could be readonly). Failing method getters and
         * field results in throwing InvalidOperationException.
         *
         * @param propertyHolder The class on which the methods or field are found
         * @param name The name of the property, where this name is capitalized and appended to
         * "get" and "is to search for the appropriate methods. If the get/is methods are not found,
         * the constructor will search for a field with that exact name.
         */
        public ReflectiveProperty(Type propertyHolder, Type valueType, string name) : base(valueType, name)
        {
            // TODO: cache reflection info for each new class/name pair
            string Cap = IProperty.CapitalizeName(name);
            TryObtainGettersAndSetters(propertyHolder, valueType, name, Cap);
        }

        private void TryObtainGettersAndSetters(Type propertyHolder, Type valueType, string name, string Cap)
        {
            ObtainGetter(propertyHolder, valueType, name, Cap);
            if (mField != null)
            {
                ObtainSetter(propertyHolder, valueType, Cap);
            }
        }

        private void ObtainGetter(Type propertyHolder, Type valueType, string name, string Cap)
        {
            mGetter = IProperty.TryGetMethod(propertyHolder, "get" + Cap);
            if (mGetter == null)
            {
                mGetter = IProperty.TryGetMethod(propertyHolder, "Get" + Cap);
                if (mGetter == null)
                {
                    // Try public field instead
                    mField = IProperty.TryGetField(propertyHolder, name);
                    if (mField == null)
                    {
                        // no way to access property - throw appropriate exception
                        throw new InvalidOperationException("No accessor method or field found for"
                                + " property with name " + name);
                    }
                    Type fieldType = mField.GetType();
                    if (valueType != fieldType)
                    {
                        throw new InvalidOperationException("Underlying type (" + fieldType + ") " +
                                "does not match Property type (" + valueType + ")");
                    }
                }
            }
        }

        private void ObtainSetter(Type propertyHolder, Type valueType, string Cap)
        {
            Type getterType;
            if (mGetter == null)
            {
                getterType = valueType;
            }
            else
            {
                getterType = mGetter.ReturnType;
                // Check to make sure our getter type matches our valueType
                if (valueType != getterType)
                {
                    throw new InvalidOperationException("Underlying type (" + getterType + ") " +
                            "does not match Property type (" + valueType + ")");
                }
            }
            mSetter = IProperty.TryGetMethod(propertyHolder, "set" + Cap, getterType);
            if (mSetter == null)
            {
                mSetter = IProperty.TryGetMethod(propertyHolder, "Set" + Cap, getterType);
            }
            // Okay to not have a setter - just a property
        }

        public override void set(object obj, object value)
        {
            if (mSetter != null)
            {
                mSetter.Invoke(obj, new object[1] { value });
            }
            else if (mField != null)
            {
                mField.SetValue(obj, value);
            }
            else
            {
                throw new NotSupportedException("Property " + getName() + " is read-only");
            }
        }

        public override object get(object obj)
        {
            if (mGetter != null)
            {
                return mGetter.Invoke(obj, (object[])null);
            }
            else if (mField != null)
            {
                return mField.GetValue(obj);
            }
            // Should not get here: there should always be a non-null getter or field
            throw new InvalidOperationException();
        }

        /**
         * Returns false if there is no setter or public field underlying this Property.
         */
        public override bool isReadOnly()
        {
            return mSetter == null && (mField == null || mField.IsInitOnly || mField.IsLiteral);
        }
    }
}