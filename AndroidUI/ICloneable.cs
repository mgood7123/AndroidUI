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

using System.Runtime.Serialization;

namespace AndroidUI
{
    public interface ICloneable
    {
        /// <summary>
        /// returns a new UNINITIALIZED instance of this object
        /// <br></br>
        /// <br></br> this is useful when since there is no way to know what constructors the subclasses will have, nor how to call them
        /// </summary>
        public static object Clone(object obj) => obj == null ? null : FormatterServices.GetUninitializedObject(obj.GetType());

        /// <summary>
        /// when overriding this class, `virtual public YOUR_CLASS_HERE Clone()` if you directly implement ICloneable, call ICloneable.Clone(this)
        /// <br></br><br></br><inheritdoc cref="Clone(object)"/>
        /// </summary>
        public virtual object Clone() => null;
    }
}
