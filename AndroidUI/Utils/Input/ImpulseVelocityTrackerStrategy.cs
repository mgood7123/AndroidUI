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
    class ImpulseVelocityTrackerStrategy : VelocityTrackerStrategy
    {
        const string LOG_TAG = "ImpulseVelocityTrackerStrategy";
        internal ImpulseVelocityTrackerStrategy()
        {
            for (int i = 0; i < mMovements.Length; i++)
            {
                mMovements[i] = new();
            }
            clear();
        }

        public override void clear()
        {
            mIndex = 0;
            mMovements[0].idBits.Clear();
        }
        public override void clearPointers(BitwiseList<object> idBits)
        {
            BitwiseList<object> remainingIdBits = new(mMovements[mIndex].idBits & ~idBits);
            mMovements[mIndex].idBits = remainingIdBits;
        }
        public override void addMovement(
            long eventTime, BitwiseList<object> idBits, Dictionary<object, NativeVelocityTracker.Position> positions
        )
        {
            if (mMovements[mIndex].eventTime != eventTime)
            {
                // When ACTION_POINTER_DOWN happens, we will first receive ACTION_MOVE with the coordinates
                // of the existing pointers, and then ACTION_POINTER_DOWN with the coordinates that include
                // the new pointer. If the eventtimes for both events are identical, just update the data
                // for this time.
                // We only compare against the last value, as it is likely that addMovement is called
                // in chronological order as events occur.
                mIndex++;
            }
            if (mIndex == HISTORY_SIZE)
            {
                mIndex = 0;
            }

            Movement movement = mMovements[mIndex];
            movement.eventTime = eventTime;
            movement.idBits = idBits;
            int count = idBits.Count;
            for (int i = 0; i < count; i++)
            {
                movement.positions[i] = positions[i];
            }
        }

        /**
         * Calculate the total impulse provided to the screen and the resulting velocity.
         *
         * The touchscreen is modeled as a physical object.
         * Initial condition is discussed below, but for now suppose that v(t=0) = 0
         *
         * The kinetic energy of the object at the release is E=0.5*m*v^2
         * Then vfinal = sqrt(2E/m). The goal is to calculate E.
         *
         * The kinetic energy at the release is equal to the total work done on the object by the finger.
         * The total work W is the sum of all dW along the path.
         *
         * dW = F*dx, where dx is the piece of path traveled.
         * Force is change of momentum over time, F = dp/dt = m dv/dt.
         * Then substituting:
         * dW = m (dv/dt) * dx = m * v * dv
         *
         * Summing along the path, we get:
         * W = sum(dW) = sum(m * v * dv) = m * sum(v * dv)
         * Since the mass stays constant, the equation for final velocity is:
         * vfinal = sqrt(2*sum(v * dv))
         *
         * Here,
         * dv : change of velocity = (v[i+1]-v[i])
         * dx : change of distance = (x[i+1]-x[i])
         * dt : change of time = (t[i+1]-t[i])
         * v : instantaneous velocity = dx/dt
         *
         * The final formula is:
         * vfinal = sqrt(2) * sqrt(sum((v[i]-v[i-1])*|v[i]|)) for all i
         * The absolute value is needed to properly account for the sign. If the velocity over a
         * particular segment descreases, then this indicates braking, which means that negative
         * work was done. So for two positive, but decreasing, velocities, this contribution would be
         * negative and will cause a smaller final velocity.
         *
         * Initial condition
         * There are two ways to deal with initial condition:
         * 1) Assume that v(0) = 0, which would mean that the screen is initially at rest.
         * This is not entirely accurate. We are only taking the past X ms of touch data, where X is
         * currently equal to 100. However, a touch event that created a fling probably lasted for longer
         * than that, which would mean that the user has already been interacting with the touchscreen
         * and it has probably already been moving.
         * 2) Assume that the touchscreen has already been moving at a certain velocity, calculate this
         * initial velocity and the equivalent energy, and start with this initial energy.
         * Consider an example where we have the following data, consisting of 3 points:
         *                 time: t0, t1, t2
         *                 x   : x0, x1, x2
         *                 v   : 0 , v1, v2
         * Here is what will happen in each of these scenarios:
         * 1) By directly applying the formula above with the v(0) = 0 boundary condition, we will get
         * vfinal = sqrt(2*(|v1|*(v1-v0) + |v2|*(v2-v1))). This can be simplified since v0=0
         * vfinal = sqrt(2*(|v1|*v1 + |v2|*(v2-v1))) = sqrt(2*(v1^2 + |v2|*(v2 - v1)))
         * since velocity is a real number
         * 2) If we treat the screen as already moving, then it must already have an energy (per mass)
         * equal to 1/2*v1^2. Then the initial energy should be 1/2*v1*2, and only the second segment
         * will contribute to the total kinetic energy (since we can effectively consider that v0=v1).
         * This will give the following expression for the final velocity:
         * vfinal = sqrt(2*(1/2*v1^2 + |v2|*(v2-v1)))
         * This analysis can be generalized to an arbitrary number of samples.
         *
         *
         * Comparing the two equations above, we see that the only mathematical difference
         * is the factor of 1/2 in front of the first velocity term.
         * This boundary condition would allow for the "proper" calculation of the case when all of the
         * samples are equally spaced in time and distance, which should suggest a constant velocity.
         *
         * Note that approach 2) is sensitive to the proper ordering of the data in time, since
         * the boundary condition must be applied to the oldest sample to be accurate.
         */
        static float kineticEnergyToVelocity(float work)
        {
            const float sqrt2 = 1.41421356237f;
            return (float)((work < 0 ? -1.0 : 1.0) * MathF.Sqrt(MathF.Abs(work)) * sqrt2);
        }

        static float calculateImpulseVelocity(long[] t, float[] x, int count)
        {
            // The input should be in reversed time order (most recent sample at index i=0)
            // t[i] is in nanoseconds, but due to FP arithmetic, convert to seconds inside this function
            const float SECONDS_PER_NANO = 1E-9f;

            if (count < 2)
            {
                return 0; // if 0 or 1 points, velocity is zero
            }
            if (t[1] > t[0])
            { // Algorithm will still work, but not perfectly
                Log.d(LOG_TAG, "Samples provided to calculateImpulseVelocity in the wrong order");
            }
            if (count == 2)
            { // if 2 points, basic linear calculation
                if (t[1] == t[0])
                {
                    Log.e(LOG_TAG, "Events have identical time stamps t=" + t[0] + ", setting velocity = 0");
                    return 0;
                }
                return (x[1] - x[0]) / (SECONDS_PER_NANO * (t[1] - t[0]));
            }
            // Guaranteed to have at least 3 points here
            float work = 0;
            for (int i = count - 1; i > 0; i--)
            { // start with the oldest sample and go forward in time
                if (t[i] == t[i - 1])
                {
                    Log.e(LOG_TAG, "Events have identical time stamps t=" + t[i] + ", skipping sample");
                    continue;
                }
                float vprev = kineticEnergyToVelocity(work); // v[i-1]
                float vcurr = (x[i] - x[i - 1]) / (SECONDS_PER_NANO * (t[i] - t[i - 1])); // v[i]
                work += (vcurr - vprev) * MathF.Abs(vcurr);
                if (i == count - 1)
                {
                    work = (float)(work * 0.5); // initial condition, case 2) above
                }
            }
            return kineticEnergyToVelocity(work);
        }

        public override bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator)
        {
            outEstimator.clear();

            // Iterate over movement samples in reverse time order and collect samples.
            float[] x = new float[HISTORY_SIZE];
            float[] y = new float[HISTORY_SIZE];
            long[] time = new long[HISTORY_SIZE];
            int m = 0; // number of points that will be used for fitting
            int index = mIndex;
            Movement newestMovement = mMovements[mIndex];
            do
            {
                Movement movement = mMovements[index];
                if (!movement.idBits.Contains(id))
                {
                    break;
                }

                long age = newestMovement.eventTime - movement.eventTime;
                if (age > HORIZON)
                {
                    break;
                }

                NativeVelocityTracker.Position position = movement.getPosition(id);
                x[m] = position.x;
                y[m] = position.y;
                time[m] = movement.eventTime;
                index = (index == 0 ? HISTORY_SIZE : index) - 1;
            } while (++m < HISTORY_SIZE);

            if (m == 0)
            {
                return false; // no data
            }
            outEstimator.xCoeff[0] = 0;
            outEstimator.yCoeff[0] = 0;
            outEstimator.xCoeff[1] = calculateImpulseVelocity(time, x, m);
            outEstimator.yCoeff[1] = calculateImpulseVelocity(time, y, m);
            outEstimator.xCoeff[2] = 0;
            outEstimator.yCoeff[2] = 0;
            outEstimator.time = newestMovement.eventTime;
            outEstimator.degree = 2; // similar results to 2nd degree fit
            outEstimator.confidence = 1;
            if (DEBUG_STRATEGY)
            {
                Log.d(LOG_TAG, "velocity: (" + outEstimator.xCoeff[1] + ", " + outEstimator.yCoeff[1] + ")");
            }
            return true;
        }

        // Sample horizon.
        // We don't use too much history by default since we want to react to quick
        // changes in direction.
        const long HORIZON = 100 * 1000000; // 100 ms

        // Number of samples to keep.
        const int HISTORY_SIZE = 20;

        int mIndex;
        Movement[] mMovements = new Movement[HISTORY_SIZE];
    }
}