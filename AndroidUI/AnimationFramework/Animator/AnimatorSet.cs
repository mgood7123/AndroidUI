/*
 * Copyright (C) 2010 The Android Open Source Project
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

using AndroidUI.AnimationFramework.Interpolators;
using AndroidUI.Applications;
using AndroidUI.Exceptions;
using AndroidUI.Execution;
using AndroidUI.Utils;

namespace AndroidUI.AnimationFramework.Animator
{

    /**
     * This class plays a set of {@link Animator} objects in the specified order. Animations
     * can be set up to play together, in sequence, or after a specified delay.
     *
     * <p>There are two different approaches to adding animations to a <code>AnimatorSet</code>:
     * either the {@link AnimatorSet#playTogether(Animator[]) playTogether()} or
     * {@link AnimatorSet#playSequentially(Animator[]) playSequentially()} methods can be called to add
     * a set of animations all at once, or the {@link AnimatorSet#play(Animator)} can be
     * used in conjunction with methods in the {@link AnimatorSet.Builder Builder}
     * class to add animations
     * one by one.</p>
     *
     * <p>It is possible to set up a <code>AnimatorSet</code> with circular dependencies between
     * its animations. For example, an animation a1 could be set up to start before animation a2, a2
     * before a3, and a3 before a1. The results of this configuration are undefined, but will typically
     * result in none of the affected animations being played. Because of this (and because
     * circular dependencies do not make logical sense anyway), circular dependencies
     * should be avoided, and the dependency flow of animations should only be in one direction.
     *
     * <div class="special reference">
     * <h3>Developer Guides</h3>
     * <p>For more information about animating with {@code AnimatorSet}, read the
     * <a href="{@docRoot}guide/topics/graphics/prop-animation.html#choreography">Property
     * Animation</a> developer guide.</p>
     * </div>
     */
    public sealed class AnimatorSet : Animator, AnimationHandler.AnimationFrameCallback
    {

        Context context;

        private const String TAG = "AnimatorSet";
        /**
         * Internal variables
         * NOTE: This object implements the clone() method, making a deep copy of any referenced
         * objects. As other non-trivial fields are added to this class, make sure to add logic
         * to clone() to make deep copies of them.
         */

        /**
         * Tracks animations currently being played, so that we know what to
         * cancel or end when cancel() or end() is called on this AnimatorSet
         */
        private List<Node> mPlayingSet = new List<Node>();

        /**
         * Contains all nodes, mapped to their respective Animators. When new
         * dependency information is added for an Animator, we want to add it
         * to a single node representing that Animator, not create a new Node
         * if one already exists.
         */
        private Dictionary<Animator, Node> mNodeMap = new Dictionary<Animator, Node>();

        /**
         * Contains the start and end events of all the nodes. All these events are sorted in this list.
         */
        private List<AnimationEvent> mEvents = new();

        /**
         * Set of all nodes created for this AnimatorSet. This list is used upon
         * starting the set, and the nodes are placed in sorted order into the
         * sortedNodes collection.
         */
        private List<Node> mNodes = new List<Node>();

        /**
         * Tracks whether any change has been made to the AnimatorSet, which is then used to
         * determine whether the dependency graph should be re-constructed.
         */
        private bool mDependencyDirty = false;

        /**
         * Indicates whether an AnimatorSet has been start()'d, whether or
         * not there is a nonzero startDelay.
         */
        private bool mStarted = false;

        // The amount of time in ms to delay starting the animation after start() is called
        private long mStartDelay = 0;

        // Animator used for a nonzero startDelay
        private ValueAnimator mDelayAnim;

        // Root of the dependency tree of all the animators in the set. In this tree, parent-child
        // relationship captures the order of animation (i.e. parent and child will play sequentially),
        // and sibling relationship indicates "with" relationship, as sibling animators start at the
        // same time.
        private Node mRootNode;

        // How long the child animations should last in ms. The default value is negative, which
        // simply means that there is no duration set on the AnimatorSet. When a real duration is
        // set, it is passed along to the child animations.
        private long mDuration = -1;

        // Records the interpolator for the set. Null value indicates that no interpolator
        // was set on this AnimatorSet, so it should not be passed down to the children.
        private TimeInterpolator mInterpolator = null;

        // The total duration of finishing all the Animators in the set.
        private long mTotalDuration = 0;

        // In pre-N releases, calling end() before start() on an animator set is no-op. But that is not
        // consistent with the behavior for other animator types. In order to keep the behavior
        // consistent within Animation framework, when end() is called without start(), we will start
        // the animator set and immediately end it for N and forward.
        private bool mShouldIgnoreEndWithoutStart;

        // In pre-O releases, calling start() doesn't reset all the animators values to start values.
        // As a result, the start of the animation is inconsistent with what setCurrentPlayTime(0) would
        // look like on O. Also it is inconsistent with what reverse() does on O, as reverse would
        // advance all the animations to the right beginning values for before starting to reverse.
        // From O and forward, we will add an additional step of resetting the animation values (unless
        // the animation was previously seeked and therefore doesn't start from the beginning).
        private bool mShouldResetValuesAtStart;

        // In pre-O releases, end() may never explicitly called on a child animator. As a result, end()
        // may not even be properly implemented in a lot of cases. After a few apps crashing on this,
        // it became necessary to use an sdk target guard for calling end().
        private bool mEndCanBeCalled;

        // The time, in milliseconds, when last frame of the animation came in. -1 when the animation is
        // not running.
        private long mLastFrameTime = -1;

        // The time, in milliseconds, when the first frame of the animation came in. This is the
        // frame before we start counting down the start delay, if any.
        // -1 when the animation is not running.
        private long mFirstFrame = -1;

        // The time, in milliseconds, when the first frame of the animation came in.
        // -1 when the animation is not running.
        private int mLastEventId = -1;

        // Indicates whether the animation is reversing.
        private bool mReversing = false;

        // Indicates whether the animation should register frame callbacks. If false, the animation will
        // passively wait for an AnimatorSet to pulse it.
        private bool mSelfPulse = true;

        // SeekState stores the last seeked play time as well as seek direction.
        private SeekState mSeekState;

        // Indicates where children animators are all initialized with their start values captured.
        private bool mChildrenInitialized = false;

        /**
         * Set on the next frame after pause() is called, used to calculate a new startTime
         * or delayStartTime which allows the animator set to continue from the point at which
         * it was paused. If negative, has not yet been set.
         */
        private long mPauseTime = -1;

        // This is to work around a bug in b/34736819. This needs to be removed once app team
        // fixes their side.
        internal class ALA : AnimatorListenerAdapter
        {
            AnimatorSet outer;

            internal ALA(AnimatorSet o) { outer = o; }

            override
            public void onAnimationEnd(Animator animation)
            {
                if (!outer.mNodeMap.TryGetValue(animation, out Node node))
                {
                    throw new Exception("Error: animation ended is not in the node map");
                }
                node.mEnded = true;
            }
        }

        private AnimatorListenerAdapter mAnimationEndListener;

        public AnimatorSet(Context context)
        {
            this.context = context;
            mDelayAnim = ValueAnimator.ofFloat(context, 0f, 1f).setDuration(0);
            mSeekState = new SeekState(this);
            mRootNode = new Node(mDelayAnim, this);
            mAnimationEndListener = new ALA(this);
            mNodeMap[mDelayAnim] = mRootNode;
            mNodes.Add(mRootNode);
            bool isPreO;
            // we are never pre-N
            // Set the flag to ignore calling end() without start() for pre-N releases
            //Application app = ActivityThread.currentApplication();
            //if (app == null || app.getApplicationInfo() == null) {
            //    mShouldIgnoreEndWithoutStart = true;
            //    isPreO = true;
            //} else {
            //    if (app.getApplicationInfo().targetSdkVersion < Build.VERSION_CODES.N) {
            //        mShouldIgnoreEndWithoutStart = true;
            //    } else {
            mShouldIgnoreEndWithoutStart = false;
            //    }

            isPreO = false; //app.getApplicationInfo().targetSdkVersion < Build.VERSION_CODES.O;
                            //}
            mShouldResetValuesAtStart = !isPreO;
            mEndCanBeCalled = !isPreO;
        }

        /**
         * Sets up this AnimatorSet to play all of the supplied animations at the same time.
         * This is equivalent to calling {@link #play(Animator)} with the first animator in the
         * set and then {@link Builder#with(Animator)} with each of the other animators. Note that
         * an Animator with a {@link Animator#setStartDelay(long) startDelay} will not actually
         * start until that delay elapses, which means that if the first animator in the list
         * supplied to this constructor has a startDelay, none of the other animators will start
         * until that first animator's startDelay has elapsed.
         *
         * @param items The animations that will be started simultaneously.
         */
        public void playTogether(params Animator[] items)
        {
            if (items != null)
            {
                Builder builder = play(items[0]);
                for (int i = 1; i < items.Length; ++i)
                {
                    builder.with(items[i]);
                }
            }
        }

        /**
         * Sets up this AnimatorSet to play all of the supplied animations at the same time.
         *
         * @param items The animations that will be started simultaneously.
         */
        public void playTogether(ICollection<Animator> items)
        {
            if (items != null && items.Count > 0)
            {
                Builder builder = null;
                foreach (Animator anim in items)
                {
                    if (builder == null)
                    {
                        builder = play(anim);
                    }
                    else
                    {
                        builder.with(anim);
                    }
                }
            }
        }

        /**
         * Sets up this AnimatorSet to play each of the supplied animations when the
         * previous animation ends.
         *
         * @param items The animations that will be started one after another.
         */
        public void playSequentially(params Animator[] items)
        {
            if (items != null)
            {
                if (items.Length == 1)
                {
                    play(items[0]);
                }
                else
                {
                    for (int i = 0; i < items.Length - 1; ++i)
                    {
                        play(items[i]).before(items[i + 1]);
                    }
                }
            }
        }

        /**
         * Sets up this AnimatorSet to play each of the supplied animations when the
         * previous animation ends.
         *
         * @param items The animations that will be started one after another.
         */
        public void playSequentially(List<Animator> items)
        {
            if (items != null && items.Count > 0)
            {
                if (items.Count == 1)
                {
                    play(items.ElementAt(0));
                }
                else
                {
                    for (int i = 0; i < items.Count - 1; ++i)
                    {
                        play(items.ElementAt(i)).before(items.ElementAt(i + 1));
                    }
                }
            }
        }

        /**
         * Returns the current list of child Animator objects controlled by this
         * AnimatorSet. This is a copy of the internal list; modifications to the returned list
         * will not affect the AnimatorSet, although changes to the underlying Animator objects
         * will affect those objects being managed by the AnimatorSet.
         *
         * @return List<Animator> The list of child animations of this AnimatorSet.
         */
        public List<Animator> getChildAnimations()
        {
            List<Animator> childList = new List<Animator>();
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                if (node != mRootNode)
                {
                    childList.Add(node.mAnimation);
                }
            }
            return childList;
        }

        /**
         * Sets the target object for all current {@link #getChildAnimations() child animations}
         * of this AnimatorSet that take targets ({@link ObjectAnimator} and
         * AnimatorSet).
         *
         * @param target The object being animated
         */
        override
        public void setTarget(Object target)
        {
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                Animator animation = node.mAnimation;
                if (animation is AnimatorSet)
                {
                    ((AnimatorSet)animation).setTarget(target);
                }
                else if (animation is ObjectAnimator)
                {
                    ((ObjectAnimator)animation).setTarget(target);
                }
            }
        }

        /**
         * @hide
         */
        override
        internal int getChangingConfigurations()
        {
            int conf = base.getChangingConfigurations();
            int nodeCount = mNodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                conf |= mNodes.ElementAt(i).mAnimation.getChangingConfigurations();
            }
            return conf;
        }

        /**
         * Sets the TimeInterpolator for all current {@link #getChildAnimations() child animations}
         * of this AnimatorSet. The default value is null, which means that no interpolator
         * is set on this AnimatorSet. Setting the interpolator to any non-null value
         * will cause that interpolator to be set on the child animations
         * when the set is started.
         *
         * @param interpolator the interpolator to be used by each child animation of this AnimatorSet
         */
        override
        public void setInterpolator(TimeInterpolator interpolator)
        {
            mInterpolator = interpolator;
        }

        override
        public TimeInterpolator getInterpolator()
        {
            return mInterpolator;
        }

        /**
         * This method creates a <code>Builder</code> object, which is used to
         * set up playing constraints. This initial <code>play()</code> method
         * tells the <code>Builder</code> the animation that is the dependency for
         * the succeeding commands to the <code>Builder</code>. For example,
         * calling <code>play(a1).with(a2)</code> sets up the AnimatorSet to play
         * <code>a1</code> and <code>a2</code> at the same time,
         * <code>play(a1).before(a2)</code> sets up the AnimatorSet to play
         * <code>a1</code> first, followed by <code>a2</code>, and
         * <code>play(a1).after(a2)</code> sets up the AnimatorSet to play
         * <code>a2</code> first, followed by <code>a1</code>.
         *
         * <p>Note that <code>play()</code> is the only way to tell the
         * <code>Builder</code> the animation upon which the dependency is created,
         * so successive calls to the various functions in <code>Builder</code>
         * will all refer to the initial parameter supplied in <code>play()</code>
         * as the dependency of the other animations. For example, calling
         * <code>play(a1).before(a2).before(a3)</code> will play both <code>a2</code>
         * and <code>a3</code> when a1 ends; it does not set up a dependency between
         * <code>a2</code> and <code>a3</code>.</p>
         *
         * @param anim The animation that is the dependency used in later calls to the
         * methods in the returned <code>Builder</code> object. A null parameter will result
         * in a null <code>Builder</code> return value.
         * @return Builder The object that constructs the AnimatorSet based on the dependencies
         * outlined in the calls to <code>play</code> and the other methods in the
         * <code>Builder</code object.
         */
        public Builder play(Animator anim)
        {
            if (anim != null)
            {
                return new Builder(anim, this);
            }
            return null;
        }

        /**
         * {@inheritDoc}
         *
         * <p>Note that canceling a <code>AnimatorSet</code> also cancels all of the animations that it
         * is responsible for.</p>
         */
        override
        public void cancel()
        {
            if (Looper.myLooper(context) == null)
            {
                throw new Exception("Animators may only be run on Looper threads");
            }
            if (isStarted())
            {
                List<AnimatorListener> tmpListeners = null;
                if (mListeners != null)
                {
                    tmpListeners = new List<AnimatorListener>(mListeners);
                    int size = tmpListeners.Count;
                    for (int i = 0; i < size; i++)
                    {
                        tmpListeners.ElementAt(i).onAnimationCancel(this);
                    }
                }
                List<Node> playingSet = new(mPlayingSet);
                int setSize = playingSet.Count;
                for (int i = 0; i < setSize; i++)
                {
                    playingSet.ElementAt(i).mAnimation.cancel();
                }
                mPlayingSet.Clear();
                endAnimation();
            }
        }

        // Force all the animations to end when the duration scale is 0.
        private void forceToEnd()
        {
            if (mEndCanBeCalled)
            {
                end();
                return;
            }

            // Note: we don't want to combine this case with the end() method below because in
            // the case of developer calling end(), we still need to make sure end() is explicitly
            // called on the child animators to maintain the old behavior.
            if (mReversing)
            {
                handleAnimationEvents(mLastEventId, 0, getTotalDuration());
            }
            else
            {
                long zeroScalePlayTime = getTotalDuration();
                if (zeroScalePlayTime == DURATION_INFINITE)
                {
                    // Use a large number for the play time.
                    zeroScalePlayTime = int.MaxValue;
                }
                handleAnimationEvents(mLastEventId, mEvents.Count - 1, zeroScalePlayTime);
            }
            mPlayingSet.Clear();
            endAnimation();
        }

        /**
         * {@inheritDoc}
         *
         * <p>Note that ending a <code>AnimatorSet</code> also ends all of the animations that it is
         * responsible for.</p>
         */
        override
        public void end()
        {
            if (Looper.myLooper(context) == null)
            {
                throw new Exception("Animators may only be run on Looper threads");
            }
            if (mShouldIgnoreEndWithoutStart && !isStarted())
            {
                return;
            }
            if (isStarted())
            {
                // Iterate the animations that haven't finished or haven't started, and end them.
                if (mReversing)
                {
                    // Between start() and first frame, mLastEventId would be unset (i.e. -1)
                    mLastEventId = mLastEventId == -1 ? mEvents.Count : mLastEventId;
                    while (mLastEventId > 0)
                    {
                        mLastEventId = mLastEventId - 1;
                        AnimationEvent event_ = mEvents.ElementAt(mLastEventId);
                        Animator anim = event_.mNode.mAnimation;
                        if (mNodeMap[anim].mEnded)
                        {
                            continue;
                        }
                        if (event_.mEvent == AnimationEvent.ANIMATION_END)
                        {
                            anim.reverse();
                        }
                        else if (event_.mEvent == AnimationEvent.ANIMATION_DELAY_ENDED
                                && anim.isStarted())
                        {
                            // Make sure anim hasn't finished before calling end() so that we don't end
                            // already ended animations, which will cause start and end callbacks to be
                            // triggered again.
                            anim.end();
                        }
                    }
                }
                else
                {
                    while (mLastEventId < mEvents.Count - 1)
                    {
                        // Avoid potential reentrant loop caused by child animators manipulating
                        // AnimatorSet's lifecycle (i.e. not a recommended approach).
                        mLastEventId = mLastEventId + 1;
                        AnimationEvent event_ = mEvents.ElementAt(mLastEventId);
                        Animator anim = event_.mNode.mAnimation;
                        if (mNodeMap[anim].mEnded)
                        {
                            continue;
                        }
                        if (event_.mEvent == AnimationEvent.ANIMATION_START)
                        {
                            anim.start();
                        }
                        else if (event_.mEvent == AnimationEvent.ANIMATION_END && anim.isStarted())
                        {
                            // Make sure anim hasn't finished before calling end() so that we don't end
                            // already ended animations, which will cause start and end callbacks to be
                            // triggered again.
                            anim.end();
                        }
                    }
                }
                mPlayingSet.Clear();
            }
            endAnimation();
        }

        /**
         * Returns true if any of the child animations of this AnimatorSet have been started and have
         * not yet ended. Child animations will not be started until the AnimatorSet has gone past
         * its initial delay set through {@link #setStartDelay(long)}.
         *
         * @return Whether this AnimatorSet has gone past the initial delay, and at least one child
         *         animation has been started and not yet ended.
         */
        override
        public bool isRunning()
        {
            if (mStartDelay == 0)
            {
                return mStarted;
            }
            return mLastFrameTime > 0;
        }

        override
        public bool isStarted()
        {
            return mStarted;
        }

        /**
         * The amount of time, in milliseconds, to delay starting the animation after
         * {@link #start()} is called.
         *
         * @return the number of milliseconds to delay running the animation
         */
        override
        public long getStartDelay()
        {
            return mStartDelay;
        }

        /**
         * The amount of time, in milliseconds, to delay starting the animation after
         * {@link #start()} is called. Note that the start delay should always be non-negative. Any
         * negative start delay will be clamped to 0 on N and above.
         *
         * @param startDelay The amount of the delay, in milliseconds
         */
        override
        public void setStartDelay(long startDelay)
        {
            // Clamp start delay to non-negative range.
            if (startDelay < 0)
            {
                Log.w(TAG, "Start delay should always be non-negative");
                startDelay = 0;
            }
            long delta = startDelay - mStartDelay;
            if (delta == 0)
            {
                return;
            }
            mStartDelay = startDelay;
            if (!mDependencyDirty)
            {
                // Dependency graph already constructed, update all the nodes' start/end time
                int size = mNodes.Count;
                for (int i = 0; i < size; i++)
                {
                    Node node = mNodes.ElementAt(i);
                    if (node == mRootNode)
                    {
                        node.mEndTime = mStartDelay;
                    }
                    else
                    {
                        node.mStartTime = node.mStartTime == DURATION_INFINITE ?
                                DURATION_INFINITE : node.mStartTime + delta;
                        node.mEndTime = node.mEndTime == DURATION_INFINITE ?
                                DURATION_INFINITE : node.mEndTime + delta;
                    }
                }
                // Update total duration, if necessary.
                if (mTotalDuration != DURATION_INFINITE)
                {
                    mTotalDuration += delta;
                }
            }
        }

        /**
         * Gets the length of each of the child animations of this AnimatorSet. This value may
         * be less than 0, which indicates that no duration has been set on this AnimatorSet
         * and each of the child animations will use their own duration.
         *
         * @return The length of the animation, in milliseconds, of each of the child
         * animations of this AnimatorSet.
         */
        override
        public long getDuration()
        {
            return mDuration;
        }

        /**
         * Sets the length of each of the current child animations of this AnimatorSet. By default,
         * each child animation will use its own duration. If the duration is set on the AnimatorSet,
         * then each child animation inherits this duration.
         *
         * @param duration The length of the animation, in milliseconds, of each of the child
         * animations of this AnimatorSet.
         */
        override
        public AnimatorSet setDuration(long duration)
        {
            if (duration < 0)
            {
                throw new IllegalArgumentException("duration must be a value of zero or greater");
            }
            mDependencyDirty = true;
            // Just record the value for now - it will be used later when the AnimatorSet starts
            mDuration = duration;
            return this;
        }

        override
        public void setupStartValues()
        {
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                if (node != mRootNode)
                {
                    node.mAnimation.setupStartValues();
                }
            }
        }

        override
        public void setupEndValues()
        {
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                if (node != mRootNode)
                {
                    node.mAnimation.setupEndValues();
                }
            }
        }

        override
        public void pause()
        {
            if (Looper.myLooper(context) == null)
            {
                throw new Exception("Animators may only be run on Looper threads");
            }
            bool previouslyPaused = mPaused;
            base.pause();
            if (!previouslyPaused && mPaused)
            {
                mPauseTime = -1;
            }
        }

        override
        public void resume()
        {
            if (Looper.myLooper(context) == null)
            {
                throw new Exception("Animators may only be run on Looper threads");
            }
            bool previouslyPaused = mPaused;
            base.resume();
            if (previouslyPaused && !mPaused)
            {
                if (mPauseTime >= 0)
                {
                    addAnimationCallback(0);
                }
            }
        }

        /**
         * {@inheritDoc}
         *
         * <p>Starting this <code>AnimatorSet</code> will, in turn, start the animations for which
         * it is responsible. The details of when exactly those animations are started depends on
         * the dependency relationships that have been set up between the animations.
         *
         * <b>Note:</b> Manipulating AnimatorSet's lifecycle in the child animators' listener callbacks
         * will lead to undefined behaviors. Also, AnimatorSet will ignore any seeking in the child
         * animators once {@link #start()} is called.
         */
        override
        public void start()
        {
            start(false, true);
        }

        override
        internal void startWithoutPulsing(bool inReverse)
        {
            start(inReverse, false);
        }

        private void initAnimation()
        {
            if (mInterpolator != null)
            {
                for (int i = 0; i < mNodes.Count; i++)
                {
                    Node node = mNodes.ElementAt(i);
                    node.mAnimation.setInterpolator(mInterpolator);
                }
            }
            updateAnimatorsDuration();
            createDependencyGraph();
        }

        private void start(bool inReverse, bool selfPulse)
        {
            if (Looper.myLooper(context) == null)
            {
                throw new Exception("Animators may only be run on Looper threads");
            }
            mStarted = true;
            mSelfPulse = selfPulse;
            mPaused = false;
            mPauseTime = -1;

            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                node.mEnded = false;
                node.mAnimation.setAllowRunningAsynchronously(false);
            }

            initAnimation();
            if (inReverse && !canReverse())
            {
                throw new NotSupportedException("Cannot reverse infinite AnimatorSet");
            }

            mReversing = inReverse;

            // Now that all dependencies are set up, start the animations that should be started.
            bool isEmptySet_ = isEmptySet(this);
            if (!isEmptySet_)
            {
                startAnimation();
            }

            if (mListeners != null)
            {
                List<AnimatorListener> tmpListeners =
                        new List<AnimatorListener>(mListeners);
                int numListeners = tmpListeners.Count;
                for (int i = 0; i < numListeners; ++i)
                {
                    tmpListeners.ElementAt(i).onAnimationStart(this, inReverse);
                }
            }
            if (isEmptySet_)
            {
                // In the case of empty AnimatorSet, or 0 duration scale, we will trigger the
                // onAnimationEnd() right away.
                end();
            }
        }

        // Returns true if set is empty or contains nothing but animator sets with no start delay.
        private static bool isEmptySet(AnimatorSet set)
        {
            if (set.getStartDelay() > 0)
            {
                return false;
            }
            for (int i = 0; i < set.getChildAnimations().Count; i++)
            {
                Animator anim = set.getChildAnimations().ElementAt(i);
                if (!(anim is AnimatorSet))
                {
                    // Contains non-AnimatorSet, not empty.
                    return false;
                }
                else
                {
                    if (!isEmptySet((AnimatorSet)anim))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void updateAnimatorsDuration()
        {
            if (mDuration >= 0)
            {
                // If the duration was set on this AnimatorSet, pass it along to all child animations
                int size = mNodes.Count;
                for (int i = 0; i < size; i++)
                {
                    Node node = mNodes.ElementAt(i);
                    // TODO: don't set the duration of the timing-only nodes created by AnimatorSet to
                    // insert "play-after" delays
                    node.mAnimation.setDuration(mDuration);
                }
            }
            mDelayAnim.setDuration(mStartDelay);
        }

        override
        internal void skipToEndValue(bool inReverse)
        {
            if (!isInitialized())
            {
                throw new NotSupportedException("Children must be initialized.");
            }

            // This makes sure the animation events are sorted an up to date.
            initAnimation();

            // Calling skip to the end in the sequence that they would be called in a forward/reverse
            // run, such that the sequential animations modifying the same property would have
            // the right value in the end.
            if (inReverse)
            {
                for (int i = mEvents.Count - 1; i >= 0; i--)
                {
                    if (mEvents.ElementAt(i).mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
                    {
                        mEvents.ElementAt(i).mNode.mAnimation.skipToEndValue(true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < mEvents.Count; i++)
                {
                    if (mEvents.ElementAt(i).mEvent == AnimationEvent.ANIMATION_END)
                    {
                        mEvents.ElementAt(i).mNode.mAnimation.skipToEndValue(false);
                    }
                }
            }
        }

        /**
         * Internal only.
         *
         * This method sets the animation values based on the play time. It also fast forward or
         * backward all the child animations progress accordingly.
         *
         * This method is also responsible for calling
         * {@link android.view.animation.Animation.AnimationListener#onAnimationRepeat(Animation)},
         * as needed, based on the last play time and current play time.
         */
        override
        internal void animateBasedOnPlayTime(long currentPlayTime, long lastPlayTime, bool inReverse)
        {
            if (currentPlayTime < 0 || lastPlayTime < 0)
            {
                throw new NotSupportedException("Error: Play time should never be negative.");
            }
            // TODO: take into account repeat counts and repeat callback when repeat is implemented.
            // Clamp currentPlayTime and lastPlayTime

            // TODO: Make this more efficient

            // Convert the play times to the forward direction.
            if (inReverse)
            {
                if (getTotalDuration() == DURATION_INFINITE)
                {
                    throw new NotSupportedException("Cannot reverse AnimatorSet with infinite"
                            + " duration");
                }
                long duration = getTotalDuration() - mStartDelay;
                currentPlayTime = Math.Min(currentPlayTime, duration);
                currentPlayTime = duration - currentPlayTime;
                lastPlayTime = duration - lastPlayTime;
                inReverse = false;
            }

            List<Node> unfinishedNodes = new();
            // Assumes forward playing from here on.
            for (int i = 0; i < mEvents.Count; i++)
            {
                AnimationEvent event_ = mEvents.ElementAt(i);
                if (event_.getTime() > currentPlayTime || event_.getTime() == DURATION_INFINITE)
                {
                    break;
                }

                // This animation started prior to the current play time, and won't finish before the
                // play time, add to the unfinished list.
                if (event_.mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
                {
                    if (event_.mNode.mEndTime == DURATION_INFINITE
                            || event_.mNode.mEndTime > currentPlayTime)
                    {
                        unfinishedNodes.Add(event_.mNode);
                    }
                }
                // For animations that do finish before the play time, end them in the sequence that
                // they would in a normal run.
                if (event_.mEvent == AnimationEvent.ANIMATION_END)
                {
                    // Skip to the end of the animation.
                    event_.mNode.mAnimation.skipToEndValue(false);
                }
            }

            // Seek unfinished animation to the right time.
            for (int i = 0; i < unfinishedNodes.Count; i++)
            {
                Node node = unfinishedNodes.ElementAt(i);
                long playTime = getPlayTimeForNode(currentPlayTime, node, inReverse);
                if (!inReverse)
                {
                    playTime -= node.mAnimation.getStartDelay();
                }
                node.mAnimation.animateBasedOnPlayTime(playTime, lastPlayTime, inReverse);
            }

            // Seek not yet started animations.
            for (int i = 0; i < mEvents.Count; i++)
            {
                AnimationEvent event_ = mEvents.ElementAt(i);
                if (event_.getTime() > currentPlayTime
                        && event_.mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
                {
                    event_.mNode.mAnimation.skipToEndValue(true);
                }
            }

        }

        override
        internal bool isInitialized()
        {
            if (mChildrenInitialized)
            {
                return true;
            }

            bool allInitialized = true;
            for (int i = 0; i < mNodes.Count; i++)
            {
                if (!mNodes.ElementAt(i).mAnimation.isInitialized())
                {
                    allInitialized = false;
                    break;
                }
            }
            mChildrenInitialized = allInitialized;
            return mChildrenInitialized;
        }

        private void skipToStartValue(bool inReverse)
        {
            skipToEndValue(!inReverse);
        }

        /**
         * Sets the position of the animation to the specified point in time. This time should
         * be between 0 and the total duration of the animation, including any repetition. If
         * the animation has not yet been started, then it will not advance forward after it is
         * set to this time; it will simply set the time to this value and perform any appropriate
         * actions based on that time. If the animation is already running, then setCurrentPlayTime()
         * will set the current playing time to this value and continue playing from that point.
         *
         * @param playTime The time, in milliseconds, to which the animation is advanced or rewound.
         *                 Unless the animation is reversing, the playtime is considered the time since
         *                 the end of the start delay of the AnimatorSet in a forward playing direction.
         *
         */
        public void setCurrentPlayTime(long playTime)
        {
            if (mReversing && getTotalDuration() == DURATION_INFINITE)
            {
                // Should never get here
                throw new NotSupportedException("Error: Cannot seek in reverse in an infinite"
                        + " AnimatorSet");
            }

            if ((getTotalDuration() != DURATION_INFINITE && playTime > getTotalDuration() - mStartDelay)
                    || playTime < 0)
            {
                throw new NotSupportedException("Error: Play time should always be in between"
                        + "0 and duration.");
            }

            initAnimation();

            if (!isStarted() || isPaused())
            {
                if (mReversing)
                {
                    throw new NotSupportedException("Error: Something went wrong. mReversing"
                            + " should not be set when AnimatorSet is not started.");
                }
                if (!mSeekState.isActive())
                {
                    findLatestEventIdForTime(0);
                    // Set all the values to start values.
                    initChildren();
                    mSeekState.setPlayTime(0, mReversing);
                }
                animateBasedOnPlayTime(playTime, 0, mReversing);
                mSeekState.setPlayTime(playTime, mReversing);
            }
            else
            {
                // If the animation is running, just set the seek time and wait until the next frame
                // (i.e. doAnimationFrame(...)) to advance the animation.
                mSeekState.setPlayTime(playTime, mReversing);
            }
        }

        /**
         * Returns the milliseconds elapsed since the start of the animation.
         *
         * <p>For ongoing animations, this method returns the current progress of the animation in
         * terms of play time. For an animation that has not yet been started: if the animation has been
         * seeked to a certain time via {@link #setCurrentPlayTime(long)}, the seeked play time will
         * be returned; otherwise, this method will return 0.
         *
         * @return the current position in time of the animation in milliseconds
         */
        public long getCurrentPlayTime()
        {
            if (mSeekState.isActive())
            {
                return mSeekState.getPlayTime();
            }
            if (mLastFrameTime == -1)
            {
                // Not yet started or during start delay
                return 0;
            }
            float durationScale = ValueAnimator.getDurationScale();
            durationScale = durationScale == 0 ? 1 : durationScale;
            if (mReversing)
            {
                return (long)((mLastFrameTime - mFirstFrame) / durationScale);
            }
            else
            {
                return (long)((mLastFrameTime - mFirstFrame - mStartDelay) / durationScale);
            }
        }

        private void initChildren()
        {
            if (!isInitialized())
            {
                mChildrenInitialized = true;
                // Forcefully initialize all children based on their end time, so that if the start
                // value of a child is dependent on a previous animation, the animation will be
                // initialized after the the previous animations have been advanced to the end.
                skipToEndValue(false);
            }
        }

        /**
         * @param frameTime The frame start time, in the {@link SystemClock#uptimeMillis()} time
         *                  base.
         * @return
         * @hide
         */
        public bool doAnimationFrame(long frameTime)
        {
            float durationScale = ValueAnimator.getDurationScale();
            if (durationScale == 0f)
            {
                // Duration scale is 0, end the animation right away.
                forceToEnd();
                return true;
            }

            // After the first frame comes in, we need to wait for start delay to pass before updating
            // any animation values.
            if (mFirstFrame < 0)
            {
                mFirstFrame = frameTime;
            }

            // Handle pause/resume
            if (mPaused)
            {
                // Note: Child animations don't receive pause events. Since it's never a contract that
                // the child animators will be paused when set is paused, this is unlikely to be an
                // issue.
                mPauseTime = frameTime;
                removeAnimationCallback();
                return false;
            }
            else if (mPauseTime > 0)
            {
                // Offset by the duration that the animation was paused
                mFirstFrame += (frameTime - mPauseTime);
                mPauseTime = -1;
            }

            // Continue at seeked position
            if (mSeekState.isActive())
            {
                mSeekState.updateSeekDirection(mReversing);
                if (mReversing)
                {
                    mFirstFrame = (long)(frameTime - mSeekState.getPlayTime() * durationScale);
                }
                else
                {
                    mFirstFrame = (long)(frameTime - (mSeekState.getPlayTime() + mStartDelay)
                            * durationScale);
                }
                mSeekState.reset();
            }

            if (!mReversing && frameTime < mFirstFrame + mStartDelay * durationScale)
            {
                // Still during start delay in a forward playing case.
                return false;
            }

            // From here on, we always use unscaled play time. Note this unscaled playtime includes
            // the start delay.
            long unscaledPlayTime = (long)((frameTime - mFirstFrame) / durationScale);
            mLastFrameTime = frameTime;

            // 1. Pulse the animators that will start or end in this frame
            // 2. Pulse the animators that will finish in a later frame
            int latestId = findLatestEventIdForTime(unscaledPlayTime);
            int startId = mLastEventId;

            handleAnimationEvents(startId, latestId, unscaledPlayTime);

            mLastEventId = latestId;

            // Pump a frame to the on-going animators
            for (int i = 0; i < mPlayingSet.Count; i++)
            {
                Node node = mPlayingSet.ElementAt(i);
                if (!node.mEnded)
                {
                    pulseFrame(node, getPlayTimeForNode(unscaledPlayTime, node));
                }
            }

            // Remove all the finished anims
            for (int i = mPlayingSet.Count - 1; i >= 0; i--)
            {
                if (mPlayingSet.ElementAt(i).mEnded)
                {
                    mPlayingSet.RemoveAt(i);
                }
            }

            bool finished = false;
            if (mReversing)
            {
                if (mPlayingSet.Count == 1 && mPlayingSet.ElementAt(0) == mRootNode)
                {
                    // The only animation that is running is the delay animation.
                    finished = true;
                }
                else if (mPlayingSet.Count == 0 && mLastEventId < 3)
                {
                    // The only remaining animation is the delay animation
                    finished = true;
                }
            }
            else
            {
                finished = mPlayingSet.Count == 0 && mLastEventId == mEvents.Count - 1;
            }

            if (finished)
            {
                endAnimation();
                return true;
            }
            return false;
        }

        /**
         * @hide
         */
        public void commitAnimationFrame(long frameTime)
        {
            // No op.
        }

        override
        internal bool pulseAnimationFrame(long frameTime)
        {
            return doAnimationFrame(frameTime);
        }

        /**
         * When playing forward, we call start() at the animation's scheduled start time, and make sure
         * to pump a frame at the animation's scheduled end time.
         *
         * When playing in reverse, we should reverse the animation when we hit animation's end event,
         * and expect the animation to end at the its delay ended event, rather than start event_.
         */
        private void handleAnimationEvents(int startId, int latestId, long playTime)
        {
            if (mReversing)
            {
                startId = startId == -1 ? mEvents.Count : startId;
                for (int i = startId - 1; i >= latestId; i--)
                {
                    AnimationEvent event_ = mEvents.ElementAt(i);
                    Node node = event_.mNode;
                    if (event_.mEvent == AnimationEvent.ANIMATION_END)
                    {
                        if (node.mAnimation.isStarted())
                        {
                            // If the animation has already been started before its due time (i.e.
                            // the child animator is being manipulated outside of the AnimatorSet), we
                            // need to cancel the animation to reset the internal state (e.g. frame
                            // time tracking) and remove the self pulsing callbacks
                            node.mAnimation.cancel();
                        }
                        node.mEnded = false;
                        mPlayingSet.Add(event_.mNode);
                        node.mAnimation.startWithoutPulsing(true);
                        pulseFrame(node, 0);
                    }
                    else if (event_.mEvent == AnimationEvent.ANIMATION_DELAY_ENDED && !node.mEnded)
                    {
                        // end event:
                        pulseFrame(node, getPlayTimeForNode(playTime, node));
                    }
                }
            }
            else
            {
                for (int i = startId + 1; i <= latestId; i++)
                {
                    AnimationEvent event_ = mEvents.ElementAt(i);
                    Node node = event_.mNode;
                    if (event_.mEvent == AnimationEvent.ANIMATION_START)
                    {
                        mPlayingSet.Add(event_.mNode);
                        if (node.mAnimation.isStarted())
                        {
                            // If the animation has already been started before its due time (i.e.
                            // the child animator is being manipulated outside of the AnimatorSet), we
                            // need to cancel the animation to reset the internal state (e.g. frame
                            // time tracking) and remove the self pulsing callbacks
                            node.mAnimation.cancel();
                        }
                        node.mEnded = false;
                        node.mAnimation.startWithoutPulsing(false);
                        pulseFrame(node, 0);
                    }
                    else if (event_.mEvent == AnimationEvent.ANIMATION_END && !node.mEnded)
                    {
                        // start event:
                        pulseFrame(node, getPlayTimeForNode(playTime, node));
                    }
                }
            }
        }

        /**
         * This method pulses frames into child animations. It scales the input animation play time
         * with the duration scale and pass that to the child animation via pulseAnimationFrame(long).
         *
         * @param node child animator node
         * @param animPlayTime unscaled play time (including start delay) for the child animator
         */
        private void pulseFrame(Node node, long animPlayTime)
        {
            if (!node.mEnded)
            {
                float durationScale = ValueAnimator.getDurationScale();
                durationScale = durationScale == 0 ? 1 : durationScale;
                node.mEnded = node.mAnimation.pulseAnimationFrame(
                        (long)(animPlayTime * durationScale));
            }
        }

        private long getPlayTimeForNode(long overallPlayTime, Node node)
        {
            return getPlayTimeForNode(overallPlayTime, node, mReversing);
        }

        private long getPlayTimeForNode(long overallPlayTime, Node node, bool inReverse)
        {
            if (inReverse)
            {
                overallPlayTime = getTotalDuration() - overallPlayTime;
                return node.mEndTime - overallPlayTime;
            }
            else
            {
                return overallPlayTime - node.mStartTime;
            }
        }

        private void startAnimation()
        {
            addAnimationEndListener();

            // Register animation callback
            addAnimationCallback(0);

            if (mSeekState.getPlayTimeNormalized() == 0 && mReversing)
            {
                // Maintain old behavior, if seeked to 0 then call reverse, we'll treat the case
                // the same as no seeking at all.
                mSeekState.reset();
            }
            // Set the child animators to the right end:
            if (mShouldResetValuesAtStart)
            {
                if (isInitialized())
                {
                    skipToEndValue(!mReversing);
                }
                else if (mReversing)
                {
                    // Reversing but haven't initialized all the children yet.
                    initChildren();
                    skipToEndValue(!mReversing);
                }
                else
                {
                    // If not all children are initialized and play direction is forward
                    for (int i = mEvents.Count - 1; i >= 0; i--)
                    {
                        if (mEvents.ElementAt(i).mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
                        {
                            Animator anim = mEvents.ElementAt(i).mNode.mAnimation;
                            // Only reset the animations that have been initialized to start value,
                            // so that if they are defined without a start value, they will get the
                            // values set at the right time (i.e. the next animation run)
                            if (anim.isInitialized())
                            {
                                anim.skipToEndValue(true);
                            }
                        }
                    }
                }
            }

            if (mReversing || mStartDelay == 0 || mSeekState.isActive())
            {
                long playTime;
                // If no delay, we need to call start on the first animations to be consistent with old
                // behavior.
                if (mSeekState.isActive())
                {
                    mSeekState.updateSeekDirection(mReversing);
                    playTime = mSeekState.getPlayTime();
                }
                else
                {
                    playTime = 0;
                }
                int toId = findLatestEventIdForTime(playTime);
                handleAnimationEvents(-1, toId, playTime);
                for (int i = mPlayingSet.Count - 1; i >= 0; i--)
                {
                    if (mPlayingSet.ElementAt(i).mEnded)
                    {
                        mPlayingSet.RemoveAt(i);
                    }
                }
                mLastEventId = toId;
            }
        }

        // This is to work around the issue in b/34736819, as the old behavior in AnimatorSet had
        // masked a real bug in play movies. TODO: remove this and below once the root cause is fixed.
        private void addAnimationEndListener()
        {
            for (int i = 1; i < mNodes.Count; i++)
            {
                mNodes.ElementAt(i).mAnimation.addListener(mAnimationEndListener);
            }
        }

        private void removeAnimationEndListener()
        {
            for (int i = 1; i < mNodes.Count; i++)
            {
                mNodes.ElementAt(i).mAnimation.removeListener(mAnimationEndListener);
            }
        }

        private int findLatestEventIdForTime(long currentPlayTime)
        {
            int size = mEvents.Count;
            int latestId = mLastEventId;
            // Call start on the first animations now to be consistent with the old behavior
            if (mReversing)
            {
                currentPlayTime = getTotalDuration() - currentPlayTime;
                mLastEventId = mLastEventId == -1 ? size : mLastEventId;
                for (int j = mLastEventId - 1; j >= 0; j--)
                {
                    AnimationEvent event_ = mEvents.ElementAt(j);
                    if (event_.getTime() >= currentPlayTime)
                    {
                        latestId = j;
                    }
                }
            }
            else
            {
                for (int i = mLastEventId + 1; i < size; i++)
                {
                    AnimationEvent event_ = mEvents.ElementAt(i);
                    // TODO: need a function that accounts for infinite duration to compare time
                    if (event_.getTime() != DURATION_INFINITE && event_.getTime() <= currentPlayTime)
                    {
                        latestId = i;
                    }
                }
            }
            return latestId;
        }

        private void endAnimation()
        {
            mStarted = false;
            mLastFrameTime = -1;
            mFirstFrame = -1;
            mLastEventId = -1;
            mPaused = false;
            mPauseTime = -1;
            mSeekState.reset();
            mPlayingSet.Clear();

            // No longer receive callbacks
            removeAnimationCallback();
            // Call end listener
            if (mListeners != null)
            {
                List<AnimatorListener> tmpListeners =
                        new List<AnimatorListener>(mListeners);
                int numListeners = tmpListeners.Count;
                for (int i = 0; i < numListeners; ++i)
                {
                    tmpListeners.ElementAt(i).onAnimationEnd(this, mReversing);
                }
            }
            removeAnimationEndListener();
            mSelfPulse = true;
            mReversing = false;
        }

        private void removeAnimationCallback()
        {
            if (!mSelfPulse)
            {
                return;
            }
            AnimationHandler handler = AnimationHandler.getInstance(context);
            handler.removeCallback(this);
        }

        private void addAnimationCallback(long delay)
        {
            if (!mSelfPulse)
            {
                return;
            }
            AnimationHandler handler = AnimationHandler.getInstance(context);
            handler.addAnimationFrameCallback(this, delay);
        }

        override
        public AnimatorSet Clone()
        {
            AnimatorSet anim = (AnimatorSet)base.Clone();
            /*
             * The basic clone() operation copies all items. This doesn't work very well for
             * AnimatorSet, because it will copy references that need to be recreated and state
             * that may not apply. What we need to do now is put the clone in an uninitialized
             * state, with fresh, empty data structures. Then we will build up the nodes list
             * manually, as we clone each Node (and its animation). The clone will then be sorted,
             * and will populate any appropriate lists, when it is started.
             */
            int nodeCount = mNodes.Count;
            anim.mStarted = false;
            anim.mLastFrameTime = -1;
            anim.mFirstFrame = -1;
            anim.mLastEventId = -1;
            anim.mPaused = false;
            anim.mPauseTime = -1;
            anim.mSeekState = new SeekState(anim);
            anim.mSelfPulse = true;
            anim.mPlayingSet = new List<Node>();
            anim.mNodeMap = new Dictionary<Animator, Node>();
            anim.mNodes = new List<Node>(nodeCount);
            anim.mEvents = new List<AnimationEvent>();
            anim.mAnimationEndListener = new ALA(anim);
            anim.mReversing = false;
            anim.mDependencyDirty = true;

            // Walk through the old nodes list, cloning each node and adding it to the new nodemap.
            // One problem is that the old node dependencies point to nodes in the old AnimatorSet.
            // We need to track the old/new nodes in order to reconstruct the dependencies in the clone.

            Dictionary<Node, Node> clonesMap = new(nodeCount);
            for (int n = 0; n < nodeCount; n++)
            {
                Node node = mNodes.ElementAt(n);
                Node nodeClone = node.Clone();
                // Remove the old internal listener from the cloned child
                nodeClone.mAnimation.removeListener(mAnimationEndListener);
                clonesMap[node] = nodeClone;
                anim.mNodes.Add(nodeClone);
                anim.mNodeMap[nodeClone.mAnimation] = nodeClone;
            }

            anim.mRootNode = clonesMap[mRootNode];
            anim.mDelayAnim = (ValueAnimator)anim.mRootNode.mAnimation;

            // Now that we've cloned all of the nodes, we're ready to walk through their
            // dependencies, mapping the old dependencies to the new nodes
            for (int i = 0; i < nodeCount; i++)
            {
                Node node = mNodes.ElementAt(i);
                // Update dependencies for node's clone
                Node nodeClone = clonesMap[node];
                nodeClone.mLatestParent = node.mLatestParent == null
                        ? null : clonesMap[node.mLatestParent];
                int size = node.mChildNodes == null ? 0 : node.mChildNodes.Count;
                for (int j = 0; j < size; j++)
                {
                    nodeClone.mChildNodes[j] = clonesMap[node.mChildNodes.ElementAt(j)];
                }
                size = node.mSiblings == null ? 0 : node.mSiblings.Count;
                for (int j = 0; j < size; j++)
                {
                    nodeClone.mSiblings[j] = clonesMap[node.mSiblings.ElementAt(j)];
                }
                size = node.mParents == null ? 0 : node.mParents.Count;
                for (int j = 0; j < size; j++)
                {
                    nodeClone.mParents[j] = clonesMap[node.mParents.ElementAt(j)];
                }
            }
            return anim;
        }


        /**
         * AnimatorSet is only reversible when the set contains no sequential animation, and no child
         * animators have a start delay.
         * @hide
         */
        override
        public bool canReverse()
        {
            return getTotalDuration() != DURATION_INFINITE;
        }

        /**
         * Plays the AnimatorSet in reverse. If the animation has been seeked to a specific play time
         * using {@link #setCurrentPlayTime(long)}, it will play backwards from the point seeked when
         * reverse was called. Otherwise, then it will start from the end and play backwards. This
         * behavior is only set for the current animation; future playing of the animation will use the
         * default behavior of playing forward.
         * <p>
         * Note: reverse is not supported for infinite AnimatorSet.
         */
        override
        public void reverse()
        {
            start(true, true);
        }

        override
        public String ToString()
        {
            String returnVal = "AnimatorSet@" + AndroidUI.Extensions.IntegerExtensions.toHexString(GetHashCode()) + "{";
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                returnVal += "\n    " + node.mAnimation.ToString();
            }
            return returnVal + "\n}";
        }

        private void printChildCount()
        {
            // Print out the child count through a level traverse.
            List<Node> list = new(mNodes.Count);
            list.Add(mRootNode);
            Log.d(TAG, "Current tree: ");
            int index = 0;
            while (index < list.Count)
            {
                int listSize = list.Count;
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (; index < listSize; index++)
                {
                    Node node = list.ElementAt(index);
                    int num = 0;
                    if (node.mChildNodes != null)
                    {
                        for (int i = 0; i < node.mChildNodes.Count; i++)
                        {
                            Node child = node.mChildNodes.ElementAt(i);
                            if (child.mLatestParent == node)
                            {
                                num++;
                                list.Add(child);
                            }
                        }
                    }
                    builder.Append(" ");
                    builder.Append(num);
                }
                Log.d(TAG, builder.ToString());
            }
        }

        private void createDependencyGraph()
        {
            if (!mDependencyDirty)
            {
                // Check whether any duration of the child animations has changed
                bool durationChanged = false;
                for (int i = 0; i < mNodes.Count; i++)
                {
                    Animator anim = mNodes.ElementAt(i).mAnimation;
                    if (mNodes.ElementAt(i).mTotalDuration != anim.getTotalDuration())
                    {
                        durationChanged = true;
                        break;
                    }
                }
                if (!durationChanged)
                {
                    return;
                }
            }

            mDependencyDirty = false;
            // Traverse all the siblings and make sure they have all the parents
            int size = mNodes.Count;
            for (int i = 0; i < size; i++)
            {
                mNodes.ElementAt(i).mParentsAdded = false;
            }
            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                if (node.mParentsAdded)
                {
                    continue;
                }

                node.mParentsAdded = true;
                if (node.mSiblings == null)
                {
                    continue;
                }

                // Find all the siblings
                findSiblings(node, node.mSiblings);
                node.mSiblings.Remove(node);

                // Get parents from all siblings
                int siblingSize = node.mSiblings.Count;
                for (int j = 0; j < siblingSize; j++)
                {
                    node.AddParents(node.mSiblings.ElementAt(j).mParents);
                }

                // Now make sure all siblings share the same set of parents
                for (int j = 0; j < siblingSize; j++)
                {
                    Node sibling = node.mSiblings.ElementAt(j);
                    sibling.AddParents(node.mParents);
                    sibling.mParentsAdded = true;
                }
            }

            for (int i = 0; i < size; i++)
            {
                Node node = mNodes.ElementAt(i);
                if (node != mRootNode && node.mParents == null)
                {
                    node.AddParent(mRootNode);
                }
            }

            // Do a DFS on the tree
            List<Node> visited = new List<Node>(mNodes.Count);
            // Assign start/end time
            mRootNode.mStartTime = 0;
            mRootNode.mEndTime = mDelayAnim.getDuration();
            updatePlayTime(mRootNode, visited);

            sortAnimationEvents();
            mTotalDuration = mEvents.ElementAt(mEvents.Count - 1).getTime();
        }

        class C : Comparer<AnimationEvent>
        {
            public override int Compare(AnimationEvent? e1, AnimationEvent? e2)
            {
                long t1 = e1.getTime();
                long t2 = e2.getTime();
                if (t1 == t2)
                {
                    // For events that happen at the same time, we need them to be in the sequence
                    // (end, start, start delay ended)
                    if (e2.mEvent + e1.mEvent == AnimationEvent.ANIMATION_START
                            + AnimationEvent.ANIMATION_DELAY_ENDED)
                    {
                        // Ensure start delay happens after start
                        return e1.mEvent - e2.mEvent;
                    }
                    else
                    {
                        return e2.mEvent - e1.mEvent;
                    }
                }
                if (t2 == DURATION_INFINITE)
                {
                    return -1;
                }
                if (t1 == DURATION_INFINITE)
                {
                    return 1;
                }
                // When neither event_ happens at INFINITE time:
                return (int)(t1 - t2);
            }
        }

        private void sortAnimationEvents()
        {
            // Sort the list of events in ascending order of their time
            // Create the list including the delay animation.
            mEvents.Clear();
            for (int i = 1; i < mNodes.Count; i++)
            {
                Node node = mNodes.ElementAt(i);
                mEvents.Add(new AnimationEvent(node, AnimationEvent.ANIMATION_START, this));
                mEvents.Add(new AnimationEvent(node, AnimationEvent.ANIMATION_DELAY_ENDED, this));
                mEvents.Add(new AnimationEvent(node, AnimationEvent.ANIMATION_END, this));
            }
            mEvents.Sort(new C());

            int eventSize = mEvents.Count;
            // For the same animation, start event_ has to happen before end.
            for (int i = 0; i < eventSize;)
            {
                AnimationEvent event_ = mEvents.ElementAt(i);
                if (event_.mEvent == AnimationEvent.ANIMATION_END)
                {
                    bool needToSwapStart;
                    if (event_.mNode.mStartTime == event_.mNode.mEndTime)
                    {
                        needToSwapStart = true;
                    }
                    else if (event_.mNode.mEndTime == event_.mNode.mStartTime
                            + event_.mNode.mAnimation.getStartDelay())
                    {
                        // Swapping start delay
                        needToSwapStart = false;
                    }
                    else
                    {
                        i++;
                        continue;
                    }

                    int startEventId = eventSize;
                    int startDelayEndId = eventSize;
                    for (int j = i + 1; j < eventSize; j++)
                    {
                        if (startEventId < eventSize && startDelayEndId < eventSize)
                        {
                            break;
                        }
                        if (mEvents.ElementAt(j).mNode == event_.mNode)
                        {
                            if (mEvents.ElementAt(j).mEvent == AnimationEvent.ANIMATION_START)
                            {
                                // Found start event
                                startEventId = j;
                            }
                            else if (mEvents.ElementAt(j).mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
                            {
                                startDelayEndId = j;
                            }
                        }

                    }
                    if (needToSwapStart && startEventId == mEvents.Count)
                    {
                        throw new NotSupportedException("Something went wrong, no start is"
                                + "found after stop for an animation that has the same start and end"
                                + "time.");

                    }
                    if (startDelayEndId == mEvents.Count)
                    {
                        throw new NotSupportedException("Something went wrong, no start"
                                + "delay end is found after stop for an animation");

                    }

                    // We need to make sure start is inserted before start delay ended event,
                    // because otherwise inserting start delay ended events first would change
                    // the start event_ index.
                    if (needToSwapStart)
                    {
                        AnimationEvent startEvent = mEvents.ElementAt(startEventId);
                        mEvents.RemoveAt(startEventId);
                        mEvents.Insert(i, startEvent);
                        i++;
                    }

                    AnimationEvent startDelayEndEvent = mEvents.ElementAt(startDelayEndId);
                    mEvents.RemoveAt(startDelayEndId);
                    mEvents.Insert(i, startDelayEndEvent);
                    i += 2;
                }
                else
                {
                    i++;
                }
            }

            if (mEvents.Count != 0 && mEvents.ElementAt(0).mEvent != AnimationEvent.ANIMATION_START)
            {
                throw new NotSupportedException(
                        "Sorting went bad, the start event_ should always be at index 0");
            }

            // Add AnimatorSet's start delay node to the beginning
            mEvents.Insert(0, new AnimationEvent(mRootNode, AnimationEvent.ANIMATION_START, this));
            mEvents.Insert(1, new AnimationEvent(mRootNode, AnimationEvent.ANIMATION_DELAY_ENDED, this));
            mEvents.Insert(2, new AnimationEvent(mRootNode, AnimationEvent.ANIMATION_END, this));

            if (mEvents.ElementAt(mEvents.Count - 1).mEvent == AnimationEvent.ANIMATION_START
                    || mEvents.ElementAt(mEvents.Count - 1).mEvent == AnimationEvent.ANIMATION_DELAY_ENDED)
            {
                throw new NotSupportedException(
                        "Something went wrong, the last event_ is not an end event");
            }
        }

        /**
         * Based on parent's start/end time, calculate children's start/end time. If cycle exists in
         * the graph, all the nodes on the cycle will be marked to start at {@link #DURATION_INFINITE},
         * meaning they will ever play.
         */
        private void updatePlayTime(Node parent, List<Node> visited)
        {
            if (parent.mChildNodes == null)
            {
                if (parent == mRootNode)
                {
                    // All the animators are in a cycle
                    for (int i = 0; i < mNodes.Count; i++)
                    {
                        Node node = mNodes.ElementAt(i);
                        if (node != mRootNode)
                        {
                            node.mStartTime = DURATION_INFINITE;
                            node.mEndTime = DURATION_INFINITE;
                        }
                    }
                }
                return;
            }

            visited.Add(parent);
            int childrenSize = parent.mChildNodes.Count;
            for (int i = 0; i < childrenSize; i++)
            {
                Node child = parent.mChildNodes.ElementAt(i);
                child.mTotalDuration = child.mAnimation.getTotalDuration();  // Update cached duration.

                int index = visited.IndexOf(child);
                if (index >= 0)
                {
                    // Child has been visited, cycle found. Mark all the nodes in the cycle.
                    for (int j = index; j < visited.Count; j++)
                    {
                        visited.ElementAt(j).mLatestParent = null;
                        visited.ElementAt(j).mStartTime = DURATION_INFINITE;
                        visited.ElementAt(j).mEndTime = DURATION_INFINITE;
                    }
                    child.mStartTime = DURATION_INFINITE;
                    child.mEndTime = DURATION_INFINITE;
                    child.mLatestParent = null;
                    Log.w(TAG, "Cycle found in AnimatorSet: " + this);
                    continue;
                }

                if (child.mStartTime != DURATION_INFINITE)
                {
                    if (parent.mEndTime == DURATION_INFINITE)
                    {
                        child.mLatestParent = parent;
                        child.mStartTime = DURATION_INFINITE;
                        child.mEndTime = DURATION_INFINITE;
                    }
                    else
                    {
                        if (parent.mEndTime >= child.mStartTime)
                        {
                            child.mLatestParent = parent;
                            child.mStartTime = parent.mEndTime;
                        }

                        child.mEndTime = child.mTotalDuration == DURATION_INFINITE
                                ? DURATION_INFINITE : child.mStartTime + child.mTotalDuration;
                    }
                }
                updatePlayTime(child, visited);
            }
            visited.Remove(parent);
        }

        // Recursively find all the siblings
        private void findSiblings(Node node, List<Node> siblings)
        {
            if (!siblings.Contains(node))
            {
                siblings.Add(node);
                if (node.mSiblings == null)
                {
                    return;
                }
                for (int i = 0; i < node.mSiblings.Count; i++)
                {
                    findSiblings(node.mSiblings.ElementAt(i), siblings);
                }
            }
        }

        /**
         * @hide
         * TODO: For animatorSet defined in XML, we can use a flag to indicate what the play order
         * if defined (i.e. sequential or together), then we can use the flag instead of calculating
         * dynamically. Note that when AnimatorSet is empty this method returns true.
         * @return whether all the animators in the set are supposed to play together
         */
        public bool shouldPlayTogether()
        {
            updateAnimatorsDuration();
            createDependencyGraph();
            // All the child nodes are set out to play right after the delay animation
            return mRootNode.mChildNodes == null || mRootNode.mChildNodes.Count == mNodes.Count - 1;
        }

        override
        public long getTotalDuration()
        {
            updateAnimatorsDuration();
            createDependencyGraph();
            return mTotalDuration;
        }

        private Node getNodeForAnimation(Animator anim)
        {
            if (!mNodeMap.TryGetValue(anim, out Node node))
            {
                node = new Node(anim, this);
                mNodeMap.Add(anim, node);
                mNodes.Add(node);
            }
            return node;
        }

        /**
         * A Node is an embodiment of both the Animator that it wraps as well as
         * any dependencies that are associated with that Animation. This includes
         * both dependencies upon other nodes (in the dependencies list) as
         * well as dependencies of other nodes upon this (in the nodeDependents list).
         */
        private class Node : Utils.ICloneable
        {
            internal Animator mAnimation;

            /**
             * Child nodes are the nodes associated with animations that will be played immediately
             * after current node.
             */
            internal List<Node> mChildNodes = null;

            /**
             * Flag indicating whether the animation in this node is finished. This flag
             * is used by AnimatorSet to check, as each animation ends, whether all child animations
             * are mEnded and it's time to send out an end event_ for the entire AnimatorSet.
             */
            internal bool mEnded = false;

            /**
             * Nodes with animations that are defined to play simultaneously with the animation
             * associated with this current node.
             */
            internal List<Node> mSiblings;

            /**
             * Parent nodes are the nodes with animations preceding current node's animation. Parent
             * nodes here are derived from user defined animation sequence.
             */
            internal List<Node> mParents;

            /**
             * Latest parent is the parent node associated with a animation that finishes after all
             * the other parents' animations.
             */
            internal Node mLatestParent = null;

            internal bool mParentsAdded = false;
            internal long mStartTime = 0;
            internal long mEndTime = 0;
            internal long mTotalDuration = 0;

            AnimatorSet outer;

            /**
             * Constructs the Node with the animation that it encapsulates. A Node has no
             * dependencies by default; dependencies are added via the addDependency()
             * method.
             *
             * @param animation The animation that the Node encapsulates.
             */
            public Node(Animator animation, AnimatorSet outer)
            {
                mAnimation = animation;
                this.outer = outer;
            }

            public Node Clone()
            {
                Node node = (Node)Utils.ICloneable.Clone(this);
                node.outer = outer;
                node.mLatestParent = mLatestParent;
                node.mParentsAdded = mParentsAdded;
                node.mStartTime = mStartTime;
                node.mEndTime = mEndTime;
                node.mTotalDuration = mTotalDuration;
                node.mAnimation = mAnimation.Clone();
                if (mChildNodes != null)
                {
                    node.mChildNodes = new(mChildNodes);
                }
                if (mSiblings != null)
                {
                    node.mSiblings = new(mSiblings);
                }
                if (mParents != null)
                {
                    node.mParents = new(mParents);
                }
                node.mEnded = false;
                return node;
            }

            internal void AddChild(Node node)
            {
                if (mChildNodes == null)
                {
                    mChildNodes = new();
                }
                if (!mChildNodes.Contains(node))
                {
                    mChildNodes.Add(node);
                    node.AddParent(this);
                }
            }

            public void AddSibling(Node node)
            {
                if (mSiblings == null)
                {
                    mSiblings = new List<Node>();
                }
                if (!mSiblings.Contains(node))
                {
                    mSiblings.Add(node);
                    node.AddSibling(this);
                }
            }

            public void AddParent(Node node)
            {
                if (mParents == null)
                {
                    mParents = new List<Node>();
                }
                if (!mParents.Contains(node))
                {
                    mParents.Add(node);
                    node.AddChild(this);
                }
            }

            public void AddParents(List<Node> parents)
            {
                if (parents == null)
                {
                    return;
                }
                int size = parents.Count;
                for (int i = 0; i < size; i++)
                {
                    AddParent(parents.ElementAt(i));
                }
            }
        }

        /**
         * This class is a wrapper around a node and an event_ for the animation corresponding to the
         * node. The 3 types of events represent the start of an animation, the end of a start delay of
         * an animation, and the end of an animation. When playing forward (i.e. in the non-reverse
         * direction), start event_ marks when start() should be called, and end event_ corresponds to
         * when the animation should finish. When playing in reverse, start delay will not be a part
         * of the animation. Therefore, reverse() is called at the end event, and animation should end
         * at the delay ended event_.
         */
        private class AnimationEvent
        {
            internal const int ANIMATION_START = 0;
            internal const int ANIMATION_DELAY_ENDED = 1;
            internal const int ANIMATION_END = 2;
            internal Node mNode;
            internal int mEvent;
            AnimatorSet outer;

            internal AnimationEvent(Node node, int ev, AnimatorSet outer)
            {
                mNode = node;
                mEvent = ev;
                this.outer = outer;
            }

            internal long getTime()
            {
                if (mEvent == ANIMATION_START)
                {
                    return mNode.mStartTime;
                }
                else if (mEvent == ANIMATION_DELAY_ENDED)
                {
                    return mNode.mStartTime == DURATION_INFINITE
                            ? DURATION_INFINITE : mNode.mStartTime + mNode.mAnimation.getStartDelay();
                }
                else
                {
                    return mNode.mEndTime;
                }
            }

            override
                public String ToString()
            {
                String eventStr = mEvent == ANIMATION_START ? "start" : (
                        mEvent == ANIMATION_DELAY_ENDED ? "delay ended" : "end");
                return eventStr + " " + mNode.mAnimation.ToString();
            }
        }

        private class SeekState
        {
            AnimatorSet outer;
            private long mPlayTime = -1;
            private bool mSeekingInReverse = false;

            internal SeekState(AnimatorSet outer)
            {
                this.outer = outer;
            }

            internal void reset()
            {
                mPlayTime = -1;
                mSeekingInReverse = false;
            }

            internal void setPlayTime(long playTime, bool inReverse)
            {
                // TODO: This can be simplified.

                // Clamp the play time
                if (outer.getTotalDuration() != DURATION_INFINITE)
                {
                    mPlayTime = Math.Min(playTime, outer.getTotalDuration() - outer.mStartDelay);
                }
                mPlayTime = Math.Max(0, mPlayTime);
                mSeekingInReverse = inReverse;
            }

            internal void updateSeekDirection(bool inReverse)
            {
                // Change seek direction without changing the overall fraction
                if (inReverse && outer.getTotalDuration() == DURATION_INFINITE)
                {
                    throw new NotSupportedException("Error: Cannot reverse infinite animator"
                            + " set");
                }
                if (mPlayTime >= 0)
                {
                    if (inReverse != mSeekingInReverse)
                    {
                        mPlayTime = outer.getTotalDuration() - outer.mStartDelay - mPlayTime;
                        mSeekingInReverse = inReverse;
                    }
                }
            }

            internal long getPlayTime()
            {
                return mPlayTime;
            }

            /**
             * Returns the playtime assuming the animation is forward playing
             */
            internal long getPlayTimeNormalized()
            {
                if (outer.mReversing)
                {
                    return outer.getTotalDuration() - outer.mStartDelay - mPlayTime;
                }
                return mPlayTime;
            }

            internal bool isActive()
            {
                return mPlayTime != -1;
            }
        }

        /**
         * The <code>Builder</code> object is a utility class to facilitate adding animations to a
         * <code>AnimatorSet</code> along with the relationships between the various animations. The
         * intention of the <code>Builder</code> methods, along with the {@link
         * AnimatorSet#play(Animator) play()} method of <code>AnimatorSet</code> is to make it possible
         * to express the dependency relationships of animations in a natural way. Developers can also
         * use the {@link AnimatorSet#playTogether(Animator[]) playTogether()} and {@link
         * AnimatorSet#playSequentially(Animator[]) playSequentially()} methods if these suit the need,
         * but it might be easier in some situations to express the AnimatorSet of animations in pairs.
         * <p/>
         * <p>The <code>Builder</code> object cannot be constructed directly, but is rather constructed
         * internally via a call to {@link AnimatorSet#play(Animator)}.</p>
         * <p/>
         * <p>For example, this sets up a AnimatorSet to play anim1 and anim2 at the same time, anim3 to
         * play when anim2 finishes, and anim4 to play when anim3 finishes:</p>
         * <pre>
         *     AnimatorSet s = new AnimatorSet();
         *     s.play(anim1).with(anim2);
         *     s.play(anim2).before(anim3);
         *     s.play(anim4).after(anim3);
         * </pre>
         * <p/>
         * <p>Note in the example that both {@link Builder#before(Animator)} and {@link
         * Builder#after(Animator)} are used. These are just different ways of expressing the same
         * relationship and are provided to make it easier to say things in a way that is more natural,
         * depending on the situation.</p>
         * <p/>
         * <p>It is possible to make several calls into the same <code>Builder</code> object to express
         * multiple relationships. However, note that it is only the animation passed into the initial
         * {@link AnimatorSet#play(Animator)} method that is the dependency in any of the successive
         * calls to the <code>Builder</code> object. For example, the following code starts both anim2
         * and anim3 when anim1 ends; there is no direct dependency relationship between anim2 and
         * anim3:
         * <pre>
         *   AnimatorSet s = new AnimatorSet();
         *   s.play(anim1).before(anim2).before(anim3);
         * </pre>
         * If the desired result is to play anim1 then anim2 then anim3, this code expresses the
         * relationship correctly:</p>
         * <pre>
         *   AnimatorSet s = new AnimatorSet();
         *   s.play(anim1).before(anim2);
         *   s.play(anim2).before(anim3);
         * </pre>
         * <p/>
         * <p>Note that it is possible to express relationships that cannot be resolved and will not
         * result in sensible results. For example, <code>play(anim1).after(anim1)</code> makes no
         * sense. In general, circular dependencies like this one (or more indirect ones where a depends
         * on b, which depends on c, which depends on a) should be avoided. Only create AnimatorSets
         * that can boil down to a simple, one-way relationship of animations starting with, before, and
         * after other, different, animations.</p>
         */
        public class Builder
        {
            AnimatorSet outer;
            /**
             * This tracks the current node being processed. It is supplied to the play() method
             * of AnimatorSet and passed into the constructor of Builder.
             */
            private Node mCurrentNode;

            /**
             * package-private constructor. Builders are only constructed by AnimatorSet, when the
             * play() method is called.
             *
             * @param anim The animation that is the dependency for the other animations passed into
             * the other methods of this Builder object.
             */
            internal Builder(Animator anim, AnimatorSet outer)
            {
                this.outer = outer;
                outer.mDependencyDirty = true;
                mCurrentNode = outer.getNodeForAnimation(anim);
            }

            /**
             * Sets up the given animation to play at the same time as the animation supplied in the
             * {@link AnimatorSet#play(Animator)} call that created this <code>Builder</code> object.
             *
             * @param anim The animation that will play when the animation supplied to the
             * {@link AnimatorSet#play(Animator)} method starts.
             */
            public Builder with(Animator anim)
            {
                Node node = outer.getNodeForAnimation(anim);
                mCurrentNode.AddSibling(node);
                return this;
            }

            /**
             * Sets up the given animation to play when the animation supplied in the
             * {@link AnimatorSet#play(Animator)} call that created this <code>Builder</code> object
             * ends.
             *
             * @param anim The animation that will play when the animation supplied to the
             * {@link AnimatorSet#play(Animator)} method ends.
             */
            public Builder before(Animator anim)
            {
                Node node = outer.getNodeForAnimation(anim);
                mCurrentNode.AddChild(node);
                return this;
            }

            /**
             * Sets up the given animation to play when the animation supplied in the
             * {@link AnimatorSet#play(Animator)} call that created this <code>Builder</code> object
             * to start when the animation supplied in this method call ends.
             *
             * @param anim The animation whose end will cause the animation supplied to the
             * {@link AnimatorSet#play(Animator)} method to play.
             */
            public Builder after(Animator anim)
            {
                Node node = outer.getNodeForAnimation(anim);
                mCurrentNode.AddParent(node);
                return this;
            }

            /**
             * Sets up the animation supplied in the
             * {@link AnimatorSet#play(Animator)} call that created this <code>Builder</code> object
             * to play when the given amount of time elapses.
             *
             * @param delay The number of milliseconds that should elapse before the
             * animation starts.
             */
            public Builder after(long delay)
            {
                // setup a ValueAnimator just to run the clock
                ValueAnimator anim = ValueAnimator.ofFloat(outer.context, 0f, 1f);
                anim.setDuration(delay);
                after(anim);
                return this;
            }

        }

    }
}
