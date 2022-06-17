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

namespace AndroidUI.AnimationFramework
{
    /**
     * Defines common utilities for working with animations.
     *
     */
    public class AnimationUtils
    {

        /**
         * These flags are used when parsing AnimatorSet objects
         */
        private const int TOGETHER = 0;
        private const int SEQUENTIALLY = 1;

        private class AnimationState
        {
            internal bool animationClockLocked;
            internal long currentVsyncTimeMillis;
            internal long lastReportedTimeMillis;
        };

        private static ThreadLocal<AnimationState> sAnimationState
                = new ThreadLocal<AnimationState>(() => new AnimationState());

        /**
         * Locks AnimationUtils{@link #currentAnimationTimeMillis()} to a fixed value for the current
         * thread. This is used by {@link android.view.Choreographer} to ensure that all accesses
         * during a vsync update are synchronized to the timestamp of the vsync.
         *
         * It is also exposed to tests to allow for rapid, flake-free headless testing.
         *
         * Must be followed by a call to {@link #unlockAnimationClock()} to allow time to
         * progress. Failing to do this will result in stuck animations, scrolls, and flings.
         *
         * Note that time is not allowed to "rewind" and must perpetually flow forward. So the
         * lock may fail if the time is in the past from a previously returned value, however
         * time will be frozen for the duration of the lock. The clock is a thread-local, so
         * ensure that {@link #lockAnimationClock(long)}, {@link #unlockAnimationClock()}, and
         * {@link #currentAnimationTimeMillis()} are all called on the same thread.
         *
         * This is also not reference counted in any way. Any call to {@link #unlockAnimationClock()}
         * will unlock the clock for everyone on the same thread. It is therefore recommended
         * for tests to use their own thread to ensure that there is no collision with any existing
         * {@link android.view.Choreographer} instance.
         *
         * @hide
         * */
        internal static void lockAnimationClock(long vsyncMillis)
        {
            AnimationState state = sAnimationState.Value;
            state.animationClockLocked = true;
            state.currentVsyncTimeMillis = vsyncMillis;
        }

        /**
         * Frees the time lock set in place by {@link #lockAnimationClock(long)}. Must be called
         * to allow the animation clock to self-update.
         *
         * @hide
         */
        internal static void unlockAnimationClock()
        {
            sAnimationState.Value.animationClockLocked = false;
        }

        /**
         * Returns the current animation time in milliseconds. This time should be used when invoking
         * {@link Animation#setStartTime(long)}. Refer to {@link android.os.SystemClock} for more
         * information about the different available clocks. The clock used by this method is
         * <em>not</em> the "wall" clock (it is not {@link System#currentTimeMillis}).
         *
         * @return the current animation time in milliseconds
         *
         * @see android.os.SystemClock
         */
        public static long currentAnimationTimeMillis()
        {
            AnimationState state = sAnimationState.Value;
            if (state.animationClockLocked)
            {
                // It's important that time never rewinds
                return Math.Max(state.currentVsyncTimeMillis,
                        state.lastReportedTimeMillis);
            }
            state.lastReportedTimeMillis = NanoTime.currentTimeMillis();
            return state.lastReportedTimeMillis;
        }
    }
}