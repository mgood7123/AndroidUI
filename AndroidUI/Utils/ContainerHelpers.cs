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

using System.Globalization;

namespace AndroidUI.Utils
{
    class ContainerHelpers
    {
        static bool floating_point_type_value_equals<T>(ref object a, ref object b, RunnableWithReturn<T, bool> isNaN, RunnableWithReturn<T, bool> isInfinity)
        {
            T TA = (T)Convert.ChangeType(a, typeof(T), CultureInfo.InvariantCulture);
            T TB = (T)Convert.ChangeType(b, typeof(T), CultureInfo.InvariantCulture);
            return isNaN(TA) ? isNaN(TB) : isInfinity(TA) ? isInfinity(TB) : TA.Equals(TB);
        }

        public static bool float_type_value_equals(ref object a, ref object b)
        {
            return floating_point_type_value_equals<float>(ref a, ref b, float.IsNaN, float.IsInfinity);
        }

        public static bool double_type_value_equals(ref object a, ref object b)
        {
            return floating_point_type_value_equals<double>(ref a, ref b, double.IsNaN, double.IsInfinity);
        }

        public static bool float_type_value_equals(object a, object b)
        {
            return floating_point_type_value_equals<float>(ref a, ref b, float.IsNaN, float.IsInfinity);
        }

        public static bool double_type_value_equals(object a, object b)
        {
            return floating_point_type_value_equals<double>(ref a, ref b, double.IsNaN, double.IsInfinity);
        }

        static bool promote_and_equals<T>(ref object a, ref object b)
        {
            if (a is float)
            {
                return float_type_value_equals(ref a, ref b);
            }
            else if (a is double)
            {
                return double_type_value_equals(ref a, ref b);
            }
            else
            {
                T TA = (T)Convert.ChangeType(a, typeof(T), CultureInfo.InvariantCulture);
                T TB = (T)Convert.ChangeType(b, typeof(T), CultureInfo.InvariantCulture);
                return TA.Equals(TB);
            }
        }

        public static bool value_type_equals(ref object a, ref object b)
        {
            if (a is short)
            {
                if (b is short)
                {
                    return promote_and_equals<short>(ref a, ref b);
                }
                else if (b is int)
                {
                    return promote_and_equals<int>(ref a, ref b);
                }
                else if (b is long)
                {
                    return promote_and_equals<long>(ref a, ref b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(ref a, ref b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is int)
            {
                if (b is short || b is int)
                {
                    return promote_and_equals<int>(ref a, ref b);
                }
                else if (b is long)
                {
                    return promote_and_equals<long>(ref a, ref b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(ref a, ref b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is long)
            {
                if (b is short || b is int || b is long)
                {
                    return promote_and_equals<long>(ref a, ref b);
                }
                else if (b is float)
                {
                    return promote_and_equals<float>(ref a, ref b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is float)
            {
                if (b is short || b is int || b is long || b is float)
                {
                    return promote_and_equals<float>(ref a, ref b);
                }
                else if (b is double)
                {
                    return promote_and_equals<double>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is double)
            {
                if (b is short || b is int || b is long || b is float || b is double)
                {
                    return promote_and_equals<double>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is char)
            {
                if (b is char)
                {
                    return promote_and_equals<char>(ref a, ref b);
                }
                else if (b is string)
                {
                    object tmp = "" + a;
                    return promote_and_equals<string>(ref tmp, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else if (a is string)
            {
                if (b is char)
                {
                    object tmp = "" + b;
                    return promote_and_equals<string>(ref a, ref b);
                }
                else if (b is string)
                {
                    return promote_and_equals<string>(ref a, ref b);
                }
                else
                {
                    return a.Equals(a);
                }
            }
            else
            {
                return a.Equals(a);
            }
        }
    }
}
