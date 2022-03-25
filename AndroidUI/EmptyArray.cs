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

namespace AndroidUI
{
    /**
    * Empty array is immutable. Use a shared empty array to avoid allocation.
    *
    * @hide
    */
    public sealed class EmptyArray
    {
        private EmptyArray() { }

        /** @hide */
        public static readonly bool[] BOOLEAN = new bool[0];

        /** @hide */
        public static readonly byte[] BYTE = new byte[0];

        /** @hide */
        public static readonly char[] CHAR = new char[0];

        /** @hide */
        public static readonly double[] DOUBLE = new double[0];

        /** @hide */
        public static readonly float[] FLOAT = new float[0];

        /** @hide */
        public static readonly int[] INT = new int[0];

        /** @hide */
        public static readonly long[] LONG = new long[0];

        /** @hide */
        public static readonly object[] OBJECT = new object[0];

        /** @hide */
        public static readonly string[] STRING = new string[0];

        /** @hide */
        public static readonly Exception[] EXCEPTION = new Exception[0];

        /** @hide */
        public static readonly Type[] TYPE = new Type[0];
    }
}