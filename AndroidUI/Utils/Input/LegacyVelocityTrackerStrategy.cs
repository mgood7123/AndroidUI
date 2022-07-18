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
     * Velocity tracker strategy used prior to ICS.
     */
    class LegacyVelocityTrackerStrategy : VelocityTrackerStrategy
    {
        internal LegacyVelocityTrackerStrategy()
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
            if (++mIndex == HISTORY_SIZE)
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
        public override bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator)
        {
            outEstimator.clear();

            Movement newestMovement = mMovements[mIndex];
            if (!newestMovement.idBits.Contains(id))
            {
                return false; // no data
            }

            // Find the oldest sample that contains the pointer and that is not older than HORIZON.
            long minTime = newestMovement.eventTime - HORIZON;
            int oldestIndex = mIndex;
            int numTouches = 1;
            do
            {
                int nextOldestIndex = (oldestIndex == 0 ? HISTORY_SIZE : oldestIndex) - 1;
                Movement nextOldestMovement = mMovements[nextOldestIndex];
                if (!nextOldestMovement.idBits.Contains(id)
                        || nextOldestMovement.eventTime < minTime)
                {
                    break;
                }
                oldestIndex = nextOldestIndex;
            } while (++numTouches < HISTORY_SIZE);

            // Calculate an exponentially weighted moving average of the velocity estimate
            // at different points in time measured relative to the oldest sample.
            // This is essentially an IIR filter.  Newer samples are weighted more heavily
            // than older samples.  Samples at equal time points are weighted more or less
            // equally.
            //
            // One tricky problem is that the sample data may be poorly conditioned.
            // Sometimes samples arrive very close together in time which can cause us to
            // overestimate the velocity at that time point.  Most samples might be measured
            // 16ms apart but some consecutive samples could be only 0.5sm apart because
            // the hardware or driver reports them irregularly or in bursts.
            float accumVx = 0;
            float accumVy = 0;
            int index = oldestIndex;
            int samplesUsed = 0;
            Movement oldestMovement = mMovements[oldestIndex];
            NativeVelocityTracker.Position oldestPosition = oldestMovement.getPosition(id);
            long lastDuration = 0;

            while (numTouches-- > 1)
            {
                if (++index == HISTORY_SIZE)
                {
                    index = 0;
                }
                Movement movement = mMovements[index];
                long duration = movement.eventTime - oldestMovement.eventTime;

                // If the duration between samples is small, we may significantly overestimate
                // the velocity.  Consequently, we impose a minimum duration constraint on the
                // samples that we include in the calculation.
                if (duration >= MIN_DURATION)
                {
                    NativeVelocityTracker.Position position = movement.getPosition(id);
                    float scale = 1000000000.0f / duration; // one over time delta in seconds
                    float vx = (position.x - oldestPosition.x) * scale;
                    float vy = (position.y - oldestPosition.y) * scale;
                    accumVx = (accumVx * lastDuration + vx * duration) / (duration + lastDuration);
                    accumVy = (accumVy * lastDuration + vy * duration) / (duration + lastDuration);
                    lastDuration = duration;
                    samplesUsed += 1;
                }
            }

            // Report velocity.
            NativeVelocityTracker.Position newestPosition = newestMovement.getPosition(id);
            outEstimator.time = newestMovement.eventTime;
            outEstimator.confidence = 1;
            outEstimator.xCoeff[0] = newestPosition.x;
            outEstimator.yCoeff[0] = newestPosition.y;
            if (samplesUsed.toBool())
            {
                outEstimator.xCoeff[1] = accumVx;
                outEstimator.yCoeff[1] = accumVy;
                outEstimator.degree = 1;
            }
            else
            {
                outEstimator.degree = 0;
            }
            return true;
        }

        // Oldest sample to consider when calculating the velocity.
        const long HORIZON = 200 * 1000000; // 100 ms

        // Number of samples to keep.
        const int HISTORY_SIZE = 20;

        // The minimum duration between samples when estimating velocity.
        const long MIN_DURATION = 10 * 1000000; // 10 ms

        int mIndex;
        Movement[] mMovements = new Movement[HISTORY_SIZE];
    };
}