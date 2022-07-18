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
    /*
     * Velocity tracker algorithm that uses an IIR filter.
     */
    class IntegratingVelocityTrackerStrategy : VelocityTrackerStrategy
    {
        // Degree must be 1 or 2.
        internal IntegratingVelocityTrackerStrategy(int degree)
        {
            mDegree = degree;
        }

        public override void clear()
        {
            mPointerIdBits.Clear();
        }
        public override void clearPointers(BitwiseList<object> idBits)
        {
            mPointerIdBits &= ~idBits;
        }
        public override void addMovement(
            long eventTime, BitwiseList<object> idBits, Dictionary<object, NativeVelocityTracker.Position> positions
        )
        {
            foreach (object bit in idBits)
            {
                State state = mPointerState[bit];
                NativeVelocityTracker.Position position = positions[bit];
                if (mPointerIdBits.Contains(bit))
                {
                    updateState(state, eventTime, position.x, position.y);
                }
                else
                {
                    initState(state, eventTime, position.x, position.y);
                }
            }

            mPointerIdBits = idBits;
        }
        public override bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator)
        {
            outEstimator.clear();

            if (mPointerIdBits.Contains(id))
            {
                State state = mPointerState[id];
                populateEstimator(state, outEstimator);
                return true;
            }

            return false;
        }

        // Current state estimate for a particular pointer.
        class State
        {
            internal long updateTime;
            internal int degree;

            internal float xpos, xvel, xaccel;
            internal float ypos, yvel, yaccel;
        };

        int mDegree;
        BitwiseList<object> mPointerIdBits;
        Dictionary<object, State> mPointerState = new(MAX_POINTERS + 1);

        void initState(State state, long eventTime, float xpos, float ypos)
        {
            state.updateTime = eventTime;
            state.degree = 0;

            state.xpos = xpos;
            state.xvel = 0;
            state.xaccel = 0;
            state.ypos = ypos;
            state.yvel = 0;
            state.yaccel = 0;
        }
        void updateState(State state, long eventTime, float xpos, float ypos)
        {
            const long MIN_TIME_DELTA = 2 * NativeVelocityTracker.NANOS_PER_MS;
            const float FILTER_TIME_CONSTANT = 0.010f; // 10 milliseconds

            if (eventTime <= state.updateTime + MIN_TIME_DELTA)
            {
                return;
            }

            float dt = (eventTime - state.updateTime) * 0.000000001f;
            state.updateTime = eventTime;

            float xvel = (xpos - state.xpos) / dt;
            float yvel = (ypos - state.ypos) / dt;
            if (state.degree == 0)
            {
                state.xvel = xvel;
                state.yvel = yvel;
                state.degree = 1;
            }
            else
            {
                float alpha = dt / (FILTER_TIME_CONSTANT + dt);
                if (mDegree == 1)
                {
                    state.xvel += (xvel - state.xvel) * alpha;
                    state.yvel += (yvel - state.yvel) * alpha;
                }
                else
                {
                    float xaccel = (xvel - state.xvel) / dt;
                    float yaccel = (yvel - state.yvel) / dt;
                    if (state.degree == 1)
                    {
                        state.xaccel = xaccel;
                        state.yaccel = yaccel;
                        state.degree = 2;
                    }
                    else
                    {
                        state.xaccel += (xaccel - state.xaccel) * alpha;
                        state.yaccel += (yaccel - state.yaccel) * alpha;
                    }
                    state.xvel += state.xaccel * dt * alpha;
                    state.yvel += state.yaccel * dt * alpha;
                }
            }
            state.xpos = xpos;
            state.ypos = ypos;
        }
        void populateEstimator(State state, NativeVelocityTracker.Estimator outEstimator)
        {
            outEstimator.time = state.updateTime;
            outEstimator.confidence = 1.0f;
            outEstimator.degree = state.degree;
            outEstimator.xCoeff[0] = state.xpos;
            outEstimator.xCoeff[1] = state.xvel;
            outEstimator.xCoeff[2] = state.xaccel / 2;
            outEstimator.yCoeff[0] = state.ypos;
            outEstimator.yCoeff[1] = state.yvel;
            outEstimator.yCoeff[2] = state.yaccel / 2;
        }
    }
}