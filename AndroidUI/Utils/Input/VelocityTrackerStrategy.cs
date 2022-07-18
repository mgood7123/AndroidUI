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

using AndroidUI.Extensions;
using AndroidUI.Utils.Lists;

namespace AndroidUI.Utils.Input
{
    /*
     * Implements a particular velocity tracker algorithm.
     */
    abstract class VelocityTrackerStrategy
    {
        // Log debug messages about velocity tracking.
        internal const bool DEBUG_VELOCITY = false;

        // Log debug messages about the progress of the algorithm itself.
        internal const bool DEBUG_STRATEGY = false;

        internal const int MAX_POINTERS = 10;

        protected VelocityTrackerStrategy() { }

        public abstract void clear();
        public abstract void clearPointers(BitwiseList<object> idBits);
        public abstract void addMovement(long eventTime, BitwiseList<object> idBits, Dictionary<object, NativeVelocityTracker.Position> positions);
        public abstract bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator);

        internal class Movement
        {
            internal long eventTime;
            internal BitwiseList<object> idBits = new(MAX_POINTERS);
            internal Dictionary<object, NativeVelocityTracker.Position> positions = new(MAX_POINTERS);

            internal NativeVelocityTracker.Position getPosition(object id)
            {
                return positions[id];
            }
        }

        internal static unsafe float vectorDot(float* a, float* b, int m)
        {
            float r = 0;
            for (int i = 0; i < m; i++)
            {
                r += *a++ * *b++;
            }
            return r;
        }

        internal static unsafe float vectorNorm(float* a, int m)
        {
            float r = 0;
            for (int i = 0; i < m; i++)
            {
                float t = *a++;
                r += t * t;
            }
            return MathF.Sqrt(r);
        }

        internal static unsafe string vectorToString(float* a, int m)
        {
            if (DEBUG_STRATEGY || DEBUG_VELOCITY)
            {
                string str = "[";
                for (int i = 0; i < m; i++)
                {
                    if (i.toBool())
                    {
                        str += ",";
                    }
                    str += *a++;
                }
                str += " ]";
                return str;
            }
            return "";
        }

        internal static unsafe string vectorToString(float[] a, int m)
        {
            if (DEBUG_STRATEGY || DEBUG_VELOCITY)
            {
                string str = "[";
                for (int i = 0; i < m; i++)
                {
                    if (i.toBool())
                    {
                        str += ",";
                    }
                    str += a[i];
                }
                str += " ]";
                return str;
            }
            return "";
        }

        internal static string vectorToString(List<float> a, int m)
        {
            if (DEBUG_STRATEGY || DEBUG_VELOCITY)
            {
                string str = "[";
                for (int i = 0; i < m; i++)
                {
                    if (i.toBool())
                    {
                        str += ",";
                    }
                    str += a[i];
                }
                str += " ]";
                return str;
            }
            return "";
        }

        internal static unsafe string matrixToString(float* a, int m, int n, bool rowMajor)
        {
            if (DEBUG_STRATEGY)
            {
                string str = "[";
                for (int i = 0; i < m; i++)
                {
                    if (i.toBool())
                    {
                        str += ",";
                    }
                    str = "[";
                    for (int j = 0; j < n; j++)
                    {
                        if (j.toBool())
                        {
                            str += ",";
                        }
                        str += a[rowMajor ? i * n + j : j * m + i];
                    }
                    str += " ]";
                }
                str += " ]";
                return str;
            }
            return "";
        }
    }
}