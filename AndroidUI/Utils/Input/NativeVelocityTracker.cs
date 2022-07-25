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

using AndroidUI.Input;
using AndroidUI.Utils.Lists;

namespace AndroidUI.Utils.Input
{
    internal class NativeVelocityTracker
    {
        const string LOG_TAG = "NativeVelocityTracker";

        internal enum Strategy : int
        {
            DEFAULT = -1,
            MIN = 0,
            IMPULSE = 0,
            LSQ1 = 1,
            LSQ2 = 2,
            LSQ3 = 3,
            WLSQ2_DELTA = 4,
            WLSQ2_CENTRAL = 5,
            WLSQ2_RECENT = 6,
            INT1 = 7,
            INT2 = 8,
            LEGACY = 9,
            MAX = LEGACY,
        }

        internal struct Position
        {
            internal float x, y;
        }

        internal class Estimator
        {
            const int MAX_DEGREE = 4;

            // Estimator time base.
            internal long time;

            // Polynomial coefficients describing motion in X and Y.
            internal float[] xCoeff = new float[MAX_DEGREE + 1];
            internal float[] yCoeff = new float[MAX_DEGREE + 1];

            // Polynomial degree (number of coefficients), or zero if no information is
            // available.
            internal int degree;

            // Confidence (coefficient of determination), between 0 (no fit) and 1 (perfect fit).
            internal float confidence;

            internal void clear()
            {
                time = 0;
                degree = 0;
                confidence = 0;
                for (int i = 0; i <= MAX_DEGREE; i++)
                {
                    xCoeff[i] = 0;
                    yCoeff[i] = 0;
                }
            }
        }

        // Nanoseconds per milliseconds.
        internal const long NANOS_PER_MS = 1000000;

        // Threshold for determining that a pointer has stopped moving.
        // Some input devices do not send ACTION_MOVE events in the case where a pointer has
        // stopped.  We need to detect this case so that we can accurately predict the
        // velocity after the pointer starts moving again.
        internal const long ASSUME_POINTER_STOPPED_TIME = 40 * NANOS_PER_MS;

        // Creates a velocity tracker using the specified strategy.
        // If strategy is not provided, uses the default strategy for the platform.
        internal NativeVelocityTracker(Strategy strategy = Strategy.DEFAULT)
        {
            mLastEventTime = 0;
            mCurrentPointerIdBits.Clear();
            mActivePointerId = BitwiseList<object>.NEGATIVE_ONE;
            if (!configureStrategy(strategy))
            {
                Log.e("Unrecognized velocity tracker strategy", strategy.ToString());
                if (!configureStrategy(DEFAULT_STRATEGY))
                {
                    throw new Exception("Could not create the default velocity tracker strategy");
                }
            }
        }

        // Resets the velocity tracker state.
        internal void clear()
        {
            mCurrentPointerIdBits.Clear();
            mActivePointerId = BitwiseList<object>.NEGATIVE_ONE;

            mStrategy.clear();
        }

        // Resets the velocity tracker state for specific pointers.
        // Call this method when some pointers have changed and may be reusing
        // an id that was assigned to a different pointer earlier.
        internal void clearPointers(BitwiseList<object> idBits)
        {
            BitwiseList<object> remainingIdBits = new(mCurrentPointerIdBits & ~idBits);
            mCurrentPointerIdBits = remainingIdBits;

            if (mActivePointerId != BitwiseList<object>.ZERO && idBits.Contains(mActivePointerId))
            {
                mActivePointerId = remainingIdBits.Count != 0 ? remainingIdBits[0] : null;
            }

            mStrategy.clearPointers(idBits);
        }

        // Adds movement information for a set of pointers.
        // The idBits bitfield specifies the pointer ids of the pointers whose positions
        // are included in the movement.
        // The positions array contains position information for each pointer in order by
        // increasing id.  Its size should be equal to the number of one bits in idBits.
        internal void addMovement(long eventTime, BitwiseList<object> idBits, Dictionary<object, Position> positions)
        {
            Log.d(LOG_TAG, "addMovement");
            Log.d(LOG_TAG, "idBits count = " + idBits.Count);
            if (idBits.Count != positions.Count)
            {
                throw new Exception("Mismatching number of pointers, idBits=" + idBits.Count + ", positions=" + positions.Count);
            }
            while (idBits.Count > VelocityTrackerState.MAX_POINTERS)
            {
                idBits.RemoveAt(idBits.Count - 1);
            }

            if ((mCurrentPointerIdBits & idBits) == BitwiseList<object>.ZERO
                    && eventTime >= mLastEventTime + ASSUME_POINTER_STOPPED_TIME)
            {
                if (VelocityTrackerStrategy.DEBUG_VELOCITY)
                {
                    Log.d(LOG_TAG, "VelocityTracker: stopped for " + (eventTime - mLastEventTime) * 0.000001f + " ms, clearing state.");
                }
                // We have not received any movements for too long.  Assume that all pointers
                // have stopped.
                mStrategy.clear();
            }
            mLastEventTime = eventTime;

            mCurrentPointerIdBits = idBits;
            if (mActivePointerId == BitwiseList<object>.NEGATIVE_ONE || !idBits.Contains(mActivePointerId))
            {
                mActivePointerId = idBits.Count == 0 ? BitwiseList<object>.NEGATIVE_ONE : idBits[0];
            }

            mStrategy.addMovement(eventTime, idBits, positions);

            if (VelocityTrackerStrategy.DEBUG_VELOCITY)
            {
                Log.d(LOG_TAG, "VelocityTracker: addMovement eventTime=" + eventTime + ", activePointerId=" + mActivePointerId);
                foreach (object bit in idBits)
                {
                    Estimator estimator = new();
                    getEstimator(bit, estimator);
                    var p = positions[bit];
                    unsafe
                    {
                        fixed (float* a = estimator.xCoeff)
                        fixed (float* b = estimator.xCoeff)
                        {
                            Log.d(LOG_TAG, "  " + bit + ": position (" + p.x + ", " + p.y + "), "
                                    + "estimator (degree=" + estimator.degree
                                    + ", xCoeff=" + VelocityTrackerStrategy.vectorToString(a, estimator.degree + 1)
                                    + ", yCoeff=" + VelocityTrackerStrategy.vectorToString(b, estimator.degree + 1)
                                    + ", confidence=" + estimator.confidence + ")");
                        }
                    }
                }
            }
        }

        // Adds movement information for all pointers in a MotionEvent, including historical samples.
        internal void addMovement(Touch ev)
        {
            Log.d(LOG_TAG, "addMovement (Touch)");
            var touch = ev.getTouchAtCurrentIndex();
            var actionMasked = touch.state;

            switch (actionMasked)
            {
                case Touch.State.TOUCH_DOWN:
                    if (ev.touchCount == 1)
                    {
                        // Clear all pointers on down before adding the new movement.
                        clear();
                    }
                    else
                    {
                        // Start a new movement trace for a pointer that just went down.
                        // We do this on down instead of on up because the client may want to query the
                        // final velocity for a pointer that just went up.
                        clearPointers(new(touch.identity));
                    }
                    break;
                case Touch.State.TOUCH_MOVE:
                    break;
                default:
                    // Ignore all other actions because they do not convey any new information about
                    // pointer movement.  We also want to preserve the last known velocity of the pointers.
                    // Note that ACTION_UP and ACTION_POINTER_UP always report the last known position
                    // of the pointers that went up.  ACTION_POINTER_UP does include the new position of
                    // pointers that remained down but we will also receive an ACTION_MOVE with this
                    // information if any of them actually moved.  Since we don't know how many pointers
                    // will be going up at once it makes sense to just wait for the following ACTION_MOVE
                    // before adding the movement.
                    return;
            }

            int pointerCount = ev.touchCount;
            if (pointerCount > ev.maxSupportedTouches)
            {
                pointerCount = ev.maxSupportedTouches;
            }
            Log.d(LOG_TAG, "pointerCount = " + pointerCount);

            BitwiseList<object> idBits = ev.getPointerIdBits();
            Log.d(LOG_TAG, "Touch idBits count = " + idBits.Count);

            int[] pointerIndex = new int[ev.maxSupportedTouches];
            for (int i = 0; i < pointerCount; i++)
            {
                pointerIndex[i] = idBits.IndexOf(ev.getTouchAt(i).identity);
                Log.d(LOG_TAG, "pointerIndex["+i+"] = " + pointerIndex[i]);
            }

            Dictionary<object, Position> positions = new(pointerCount);

            int historySize = ev.getHistorySize();
            Log.d(LOG_TAG, "historySize = " + historySize);
            for (int h = 0; h < historySize; h++)
            {
                var hi = ev.history[h];
                long eventTime = hi.timestamp;
                for (int i = 0; i < pointerCount; i++)
                {
                    Position p = new();
                    p.x = hi.location.x;
                    p.y = hi.location.y;
                    positions[hi.identity] = p;
                }
                Log.d(LOG_TAG, "TOUCH add movement");
                addMovement(eventTime, idBits, positions);
            }

            long eventTime_ = touch.timestamp;
            for (int i = 0; i < pointerCount; i++)
            {
                var t = ev.getTouchAt(i);
                Position p = new();
                p.x = t.location.x;
                p.y = t.location.y;
                positions[t.identity] = p;
            }
            Log.d(LOG_TAG, "TOUCH add movement");
            addMovement(eventTime_, idBits, positions);
        }

        // Gets the velocity of the specified pointer id in position units per second.
        // Returns false and sets the velocity components to zero if there is
        // insufficient movement information for the pointer.
        internal bool getVelocity(object id, out float outVx, out float outVy)
        {
            Estimator estimator = new();
            if (getEstimator(id, estimator) && estimator.degree >= 1)
            {
                outVx = estimator.xCoeff[1];
                outVy = estimator.yCoeff[1];
                return true;
            }
            outVx = 0;
            outVy = 0;
            return false;
        }

        // Gets an estimator for the recent movements of the specified pointer id.
        // Returns false and clears the estimator if there is no information available
        // about the pointer.
        internal bool getEstimator(object id, Estimator outEstimator)
        {
            return mStrategy.getEstimator(id, outEstimator);
        }

        // Gets the active pointer id, or null if none.
        internal object getActivePointerId() { return mActivePointerId; }

        // Gets a bitset containing all pointer ids from the most recent movement.
        internal BitwiseList<object> getCurrentPointerIdBits() { return mCurrentPointerIdBits; }

        // The default velocity tracker strategy.
        // Although other strategies are available for testing and comparison purposes,
        // this is the strategy that applications will actually use.  Be very careful
        // when adjusting the default strategy because it can dramatically affect
        // (often in a bad way) the user experience.
        private const Strategy DEFAULT_STRATEGY = Strategy.LSQ2;

        private long mLastEventTime;
        private BitwiseList<object> mCurrentPointerIdBits = new BitwiseList<object>();
        private object mActivePointerId;
        private VelocityTrackerStrategy mStrategy;

        private bool configureStrategy(Strategy strategy)
        {
            if (strategy == Strategy.DEFAULT)
            {
                mStrategy = createStrategy(DEFAULT_STRATEGY);
            }
            else
            {
                mStrategy = createStrategy(strategy);
            }
            return mStrategy != null;
        }

        private static VelocityTrackerStrategy createStrategy(Strategy strategy)
        {
            switch (strategy)
            {
                case Strategy.IMPULSE:
                    return new ImpulseVelocityTrackerStrategy();

                case Strategy.LSQ1:
                    return new LeastSquaresVelocityTrackerStrategy(1);

                case Strategy.LSQ2:
                    return new LeastSquaresVelocityTrackerStrategy(2);

                case Strategy.LSQ3:
                    return new LeastSquaresVelocityTrackerStrategy(3);

                case Strategy.WLSQ2_DELTA:
                    return new LeastSquaresVelocityTrackerStrategy(2, LeastSquaresVelocityTrackerStrategy.Weighting.WEIGHTING_DELTA);
                case Strategy.WLSQ2_CENTRAL:
                    return new LeastSquaresVelocityTrackerStrategy(2, LeastSquaresVelocityTrackerStrategy.Weighting.WEIGHTING_CENTRAL);
                case Strategy.WLSQ2_RECENT:
                    return new LeastSquaresVelocityTrackerStrategy(2, LeastSquaresVelocityTrackerStrategy.Weighting.WEIGHTING_RECENT);

                case Strategy.INT1:
                    return new IntegratingVelocityTrackerStrategy(1);

                case Strategy.INT2:
                    return new IntegratingVelocityTrackerStrategy(2);

                case Strategy.LEGACY:
                    return new LegacyVelocityTrackerStrategy();

                default:
                    break;
            }
            return null;
        }
    }
}