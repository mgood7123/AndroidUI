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
    class ContainerHelpers
    {

        // This is Arrays.binarySearch(), but doesn't do any argument validation.
        public static int binarySearch(int[] array, int size, int value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                int mid = (int)((uint)(lo + hi) >> 1);
                int midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }
            return ~lo;  // value not present
        }

        public static int binarySearch(long[] array, int size, long value)
        {
            int lo = 0;
            int hi = size - 1;

            while (lo <= hi)
            {
                int mid = (int)((uint)(lo + hi) >> 1);
                long midVal = array[mid];

                if (midVal < value)
                {
                    lo = mid + 1;
                }
                else if (midVal > value)
                {
                    hi = mid - 1;
                }
                else
                {
                    return mid;  // value found
                }
            }
            return ~lo;  // value not present
        }
    }
}
