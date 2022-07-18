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

using AndroidUI.Utils.Lists;

namespace AndroidUI.Utils.Input
{
    class VelocityTrackerState
    {
        const string LOG_TAG = "VelocityTrackerState";
        public VelocityTrackerState(int strategy)
        {
            mVelocityTracker = new((NativeVelocityTracker.Strategy)strategy);
            mActivePointerId = BitwiseList<object>.NEGATIVE_ONE;
        }

        public void clear()
        {
            mVelocityTracker.clear();
            mActivePointerId = BitwiseList<object>.NEGATIVE_ONE;
            mCalculatedIdBits.Clear();
        }
        public void addMovement(Touch ev)
        {
            mVelocityTracker.addMovement(ev);
        }
        public void computeCurrentVelocity(int units, float maxVelocity)
        {
            BitwiseList<object> idBits = new(mVelocityTracker.getCurrentPointerIdBits());
            mCalculatedIdBits = idBits;

            Log.d(LOG_TAG, "idBits count = " + idBits.Count);

            foreach (object id in idBits)
            {
                float vx, vy;
                mVelocityTracker.getVelocity(id, out vx, out vy);
                Log.d(LOG_TAG, "velocity x = " + vx + ", velocity y = " + vy);

                vx = vx * units / 1000;
                vy = vy * units / 1000;

                if (vx > maxVelocity)
                {
                    vx = maxVelocity;
                }
                else if (vx < -maxVelocity)
                {
                    vx = -maxVelocity;
                }
                if (vy > maxVelocity)
                {
                    vy = maxVelocity;
                }
                else if (vy < -maxVelocity)
                {
                    vy = -maxVelocity;
                }

                Velocity velocity = new();
                velocity.vx = vx;
                velocity.vy = vy;
                Log.d(LOG_TAG, "velocity: " + velocity);
                mCalculatedVelocity[id] = velocity;
            }
        }

        // Special constant to request the velocity of the active pointer.
        static readonly object ACTIVE_POINTER_ID = BitwiseList<object>.NEGATIVE_ONE;

        public void getVelocity(object id, out float outVx, out float outVy)
        {
            if (id == ACTIVE_POINTER_ID)
            {
                id = mVelocityTracker.getActivePointerId();
            }

            float vx, vy;
            if (id != null && id != ACTIVE_POINTER_ID && mCalculatedIdBits.Contains(id))
            {
                Velocity velocity = mCalculatedVelocity[id];
                vx = velocity.vx;
                vy = velocity.vy;
            }
            else
            {
                vx = 0;
                vy = 0;
            }

            outVx = vx;
            outVy = vy;
        }
        public bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator)
        {
            return mVelocityTracker.getEstimator(id, outEstimator);
        }

        // private:
        private struct Velocity
        {
            internal float vx, vy;

            public override string ToString()
            {
                return "{x: " + vx + ", y: " + vy + "}";
            }
        }
        public const int MAX_POINTERS = 10;

        private NativeVelocityTracker mVelocityTracker;
        private object mActivePointerId;
        private BitwiseList<object> mCalculatedIdBits = new();
        private Dictionary<object, Velocity> mCalculatedVelocity = new(MAX_POINTERS);
    }
}