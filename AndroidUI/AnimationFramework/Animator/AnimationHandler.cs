/*
 * Copyright (C) 2015 The Android Open Source Project
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

namespace AndroidUI.AnimationFramework.Animator
{
    /**
     * This custom, static handler handles the timing pulse that is shared by all active
     * ValueAnimators. This approach ensures that the setting of animation values will happen on the
     * same thread that animations start on, and that all animations will share the same times for
     * calculating their values, which makes synchronizing animations possible.
     *
     * The handler uses the Choreographer by default for doing periodic callbacks. A custom
     * AnimationFrameCallbackProvider can be set on the handler to provide timing pulse that
     * may be independent of UI frame update. This could be useful in testing.
     *
     * @hide
     */
    internal class AnimationHandler
    {
        Context context;

        internal AnimationHandler(Context context)
        {
            this.context = context;
            mFrameCallback = Application.FrameCallback.Create(this, (this_callback, data, frameTimeNanos) =>
            {
                AnimationHandler a = (AnimationHandler)data;
                a.doAnimationFrame(a.getProvider().getFrameTime());
                if (a.mAnimationCallbacks.Count > 0)
                {
                    a.getProvider().postFrameCallback(this_callback);
                }
            });
        }

        /**
         * Internal per-thread collections used to avoid set collisions as animations start and end
         * while being processed.
         * @hide
         */
        private readonly Dictionary<AnimationFrameCallback, ValueHolder<long>> mDelayedCallbackStartTime = new();
        private readonly List<AnimationFrameCallback> mAnimationCallbacks = new();
        private readonly List<AnimationFrameCallback> mCommitCallbacks = new();
        private AnimationFrameCallbackProvider mProvider;

        private Application.FrameCallback mFrameCallback;

        // should this be per-thread?
        static ValueHolder<AnimationHandler> sAnimatorHandler(Context context)
        {
            if (context == null)
            {
                return null;
            }
            return context.storage.GetOrCreate<AnimationHandler>(StorageKeys.AnimationHandler, () => new(context));
        }

        private bool mListDirty = false;

        public static AnimationHandler getInstance(Context context)
        {
            return sAnimatorHandler(context);
        }

        /**
         * By default, the Choreographer is used to provide timing for frame callbacks. A custom
         * provider can be used here to provide different timing pulse.
         */
        internal void setProvider(AnimationFrameCallbackProvider provider)
        {
            if (provider == null)
            {
                mProvider = new MyFrameCallbackProvider(context);
            }
            else
            {
                mProvider = provider;
            }
        }

        private AnimationFrameCallbackProvider getProvider()
        {
            if (mProvider == null)
            {
                mProvider = new MyFrameCallbackProvider(context);
            }
            return mProvider;
        }

        /**
         * Register to get a callback on the next frame after the delay.
         */
        public void addAnimationFrameCallback(AnimationFrameCallback callback, long delay)
        {
            if (mAnimationCallbacks.Count == 0)
            {
                getProvider().postFrameCallback(mFrameCallback);
            }
            if (!mAnimationCallbacks.Contains(callback))
            {
                mAnimationCallbacks.Add(callback);
            }

            if (delay > 0)
            {
                // .put(
                mDelayedCallbackStartTime[callback] = NanoTime.currentTimeMillis() + delay;
            }
        }

        /**
         * Register to get a one shot callback for frame commit timing. Frame commit timing is the
         * time *after* traversals are done, as opposed to the animation frame timing, which is
         * before any traversals. This timing can be used to adjust the start time of an animation
         * when expensive traversals create big delta between the animation frame timing and the time
         * that animation is first shown on screen.
         *
         * Note this should only be called when the animation has already registered to receive
         * animation frame callbacks. This callback will be guaranteed to happen *after* the next
         * animation frame callback.
         */
        public void addOneShotCommitCallback(AnimationFrameCallback callback)
        {
            if (!mCommitCallbacks.Contains(callback))
            {
                mCommitCallbacks.Add(callback);
            }
        }

        /**
         * Removes the given callback from the list, so it will no longer be called for frame related
         * timing.
         */
        public void removeCallback(AnimationFrameCallback callback)
        {
            mCommitCallbacks.Remove(callback);
            mDelayedCallbackStartTime.Remove(callback);
            int id = mAnimationCallbacks.IndexOf(callback);
            if (id >= 0)
            {
                mAnimationCallbacks[id] = null;
                mListDirty = true;
            }
        }

        private void doAnimationFrame(long frameTime)
        {
            long currentTime = NanoTime.currentTimeMillis();
            int size = mAnimationCallbacks.Count;
            for (int i = 0; i < size; i++)
            {
                AnimationFrameCallback callback = mAnimationCallbacks.ElementAt(i);
                if (callback == null)
                {
                    continue;
                }
                if (isCallbackDue(callback, currentTime))
                {
                    callback.doAnimationFrame(frameTime);
                    if (mCommitCallbacks.Contains(callback))
                    {
                        getProvider().postCommitCallback(Runnable.Create(() =>
                        {
                            commitAnimationFrame(callback, getProvider().getFrameTime());
                        }));
                    }
                }
            }
            cleanUpList();
        }

        private void commitAnimationFrame(AnimationFrameCallback callback, long frameTime)
        {
            if (!mDelayedCallbackStartTime.ContainsKey(callback) &&
                    mCommitCallbacks.Contains(callback))
            {
                callback.commitAnimationFrame(frameTime);
                mCommitCallbacks.Remove(callback);
            }
        }

        /**
         * Remove the callbacks from mDelayedCallbackStartTime once they have passed the initial delay
         * so that they can start getting frame callbacks.
         *
         * @return true if they have passed the initial delay or have no delay, false otherwise.
         */
        private bool isCallbackDue(AnimationFrameCallback callback, long currentTime)
        {
            ValueHolder<long> startTime = mDelayedCallbackStartTime.GetValueOrDefault(callback, null);
            if (startTime == null)
            {
                return true;
            }
            if (startTime < currentTime)
            {
                mDelayedCallbackStartTime.Remove(callback);
                return true;
            }
            return false;
        }

        /**
         * Return the number of callbacks that have registered for frame callbacks.
         */
        public static int getAnimationCount(Context context)
        {
            AnimationHandler handler = sAnimatorHandler(context);
            if (handler == null)
            {
                return 0;
            }
            return handler.getCallbackSize();
        }

        public static void setFrameDelay(Context context, uint delay)
        {
            getInstance(context).getProvider().setFrameDelay(delay);
        }

        public static uint getFrameDelay(Context context)
        {
            return getInstance(context).getProvider().getFrameDelay();
        }

        internal void autoCancelBasedOn(ObjectAnimator objectAnimator)
        {
            for (int i = mAnimationCallbacks.Count - 1; i >= 0; i--)
            {
                AnimationFrameCallback cb = mAnimationCallbacks.ElementAt(i);
                if (cb == null)
                {
                    continue;
                }
                if (objectAnimator.shouldAutoCancel(cb))
                {
                    ((Animator)mAnimationCallbacks.ElementAt(i)).cancel();
                }
            }
        }

        private void cleanUpList()
        {
            if (mListDirty)
            {
                for (int i = mAnimationCallbacks.Count - 1; i >= 0; i--)
                {
                    if (mAnimationCallbacks.ElementAt(i) == null)
                    {
                        mAnimationCallbacks.RemoveAt(i);
                    }
                }
                mListDirty = false;
            }
        }

        private int getCallbackSize()
        {
            int count = 0;
            int size = mAnimationCallbacks.Count;
            for (int i = size - 1; i >= 0; i--)
            {
                if (mAnimationCallbacks.ElementAt(i) != null)
                {
                    count++;
                }
            }
            return count;
        }

        /**
         * Default provider of timing pulse that uses Choreographer for frame callbacks.
         */
        private class MyFrameCallbackProvider : AnimationFrameCallbackProvider
        {
            Context context;

            public MyFrameCallbackProvider(Context context)
            {
                this.context = context;
            }

            //Choreographer mChoreographer = Choreographer.getInstance();

            public void postFrameCallback(Application.FrameCallback callback)
            {
                if (context.mAttachInfo.mViewRootImpl.mNextRtFrameCallbacks == null)
                {
                    context.mAttachInfo.mViewRootImpl.mNextRtFrameCallbacks = new();
                }
                context.mAttachInfo.mViewRootImpl.mNextRtFrameCallbacks.Add(callback);
                //mChoreographer.postFrameCallback(callback);
            }

            public void postCommitCallback(Runnable runnable)
            {
                Console.WriteLine("COMMIT");
                context.mAttachInfo.mHandler.post(runnable);
                //mChoreographer.postCallback(Choreographer.CALLBACK_COMMIT, runnable, null);
            }

            public long getFrameTime()
            {
                return Animation.AnimationUtils.currentAnimationTimeMillis(context);
                //throw new NotSupportedException();
                //return 0; //return mChoreographer.getFrameTime();
            }

            public uint getFrameDelay()
            {
                return Animation.AnimationUtils.getFrameDelay();
                //throw new NotSupportedException();
                //return Choreographer.getFrameDelay();
            }

            public void setFrameDelay(uint delay)
            {
                Animation.AnimationUtils.setFrameDelay(delay);
                //throw new NotSupportedException();
                //Choreographer.setFrameDelay(delay);
            }
        }

        /**
         * Callbacks that receives notifications for animation timing and frame commit timing.
         */
        public interface AnimationFrameCallback
        {
            /**
             * Run animation based on the frame time.
             * @param frameTime The frame start time, in the {@link SystemClock#uptimeMillis()} time
             *                  base.
             * @return if the animation has finished.
             */
            bool doAnimationFrame(long frameTime);

            /**
             * This notifies the callback of frame commit time. Frame commit time is the time after
             * traversals happen, as opposed to the normal animation frame time that is before
             * traversals. This is used to compensate expensive traversals that happen as the
             * animation starts. When traversals take a long time to complete, the rendering of the
             * initial frame will be delayed (by a long time). But since the startTime of the
             * animation is set before the traversal, by the time of next frame, a lot of time would
             * have passed since startTime was set, the animation will consequently skip a few frames
             * to respect the new frameTime. By having the commit time, we can adjust the start time to
             * when the first frame was drawn (after any expensive traversals) so that no frames
             * will be skipped.
             *
             * @param frameTime The frame time after traversals happen, if any, in the
             *                  {@link SystemClock#uptimeMillis()} time base.
             */
            void commitAnimationFrame(long frameTime);
        }

        /**
         * The intention for having this interface is to increase the testability of ValueAnimator.
         * Specifically, we can have a custom implementation of the interface below and provide
         * timing pulse without using Choreographer. That way we could use any arbitrary interval for
         * our timing pulse in the tests.
         *
         * @hide
         */
        internal interface AnimationFrameCallbackProvider
        {
            void postFrameCallback(Application.FrameCallback callback);
            void postCommitCallback(Runnable runnable);
            long getFrameTime();
            uint getFrameDelay();
            void setFrameDelay(uint delay);
        }
    }
}