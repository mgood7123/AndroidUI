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

namespace AndroidUI.AnimationFramework.Animation
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

        static ValueHolder<AnimationState> sAnimationState(Context context)
        {
            if (context == null)
            {
                return null;
            }
            return context.storage.GetOrCreate<AnimationState>(StorageKeys.AnimationFrameworkAnimationState, () => new());
        }

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
        internal static void lockAnimationClock(Context context, long vsyncMillis)
        {
            AnimationState state = sAnimationState(context).Value;
            state.animationClockLocked = true;
            state.currentVsyncTimeMillis = vsyncMillis;
        }

        /**
         * Frees the time lock set in place by {@link #lockAnimationClock(long)}. Must be called
         * to allow the animation clock to self-update.
         *
         * @hide
         */
        internal static void unlockAnimationClock(Context context)
        {
            sAnimationState(context).Value.animationClockLocked = false;
        }

        // Choreographer start

        // The default amount of time in ms between animation frames.
        // When vsync is not enabled, we want to have some idea of how long we should
        // wait before posting the next animation message.  It is important that the
        // default value be less than the true inter-frame delay on all devices to avoid
        // situations where we might skip frames by waiting too long (we must compensate
        // for jitter and hardware variations).  Regardless of this value, the animation
        // and display loop is ultimately rate-limited by how fast new graphics buffers can
        // be dequeued.
        private const uint DEFAULT_FRAME_DELAY = 10;

        // The number of milliseconds between animation frames.
        private static volatile uint sFrameDelay = DEFAULT_FRAME_DELAY;

        /**
         * The amount of time, in milliseconds, between each frame of the animation.
         * <p>
         * This is a requested time that the animation will attempt to honor, but the actual delay
         * between frames may be different, depending on system load and capabilities. This is a static
         * function because the same delay will be applied to all animations, since they are all
         * run off of a single timing loop.
         * </p><p>
         * The frame delay may be ignored when the animation system uses an external timing
         * source, such as the display refresh rate (vsync), to govern animations.
         * </p>
         *
         * @return the requested time between frames, in milliseconds
         * @hide
         */
        internal static uint getFrameDelay()
        {
            return sFrameDelay;
        }

        /**
         * The amount of time, in milliseconds, between each frame of the animation.
         * <p>
         * This is a requested time that the animation will attempt to honor, but the actual delay
         * between frames may be different, depending on system load and capabilities. This is a static
         * function because the same delay will be applied to all animations, since they are all
         * run off of a single timing loop.
         * </p><p>
         * The frame delay may be ignored when the animation system uses an external timing
         * source, such as the display refresh rate (vsync), to govern animations.
         * </p>
         *
         * @param frameDelay the requested time between frames, in milliseconds
         * @hide
         */
        internal static void setFrameDelay(uint frameDelay)
        {
            sFrameDelay = frameDelay;
        }

        /**
          * Subtracts typical frame delay time from a delay interval in milliseconds.
          * <p>
          * This method can be used to compensate for animation delay times that have baked
          * in assumptions about the frame delay.  For example, it's quite common for code to
          * assume a 60Hz frame time and bake in a 16ms delay.  When we call
          * {@link #postAnimationCallbackDelayed} we want to know how long to wait before
          * posting the animation callback but let the animation timer take care of the remaining
          * frame delay time.
          * </p><p>
          * This method is somewhat conservative about how much of the frame delay it
          * subtracts.  It uses the same value returned by {@link #getFrameDelay} which by
          * default is 10ms even though many parts of the system assume 16ms.  Consequently,
          * we might still wait 6ms before posting an animation callback that we want to run
          * on the next frame, but this is much better than waiting a whole 16ms and likely
          * missing the deadline.
          * </p>
          *
          * @param delayMillis The original delay time including an assumed frame delay.
          * @return The adjusted delay time with the assumed frame delay subtracted out.
          * @hide
          */
        public static long subtractFrameDelay(long delayMillis)
        {
            long frameDelay = sFrameDelay;
            return delayMillis <= frameDelay ? 0 : delayMillis - frameDelay;
        }

        // Choreographer end

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
        public static long currentAnimationTimeMillis(Context context)
        {
            AnimationState state = sAnimationState(context).Value;
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