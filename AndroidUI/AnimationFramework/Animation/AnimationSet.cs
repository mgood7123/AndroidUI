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

using AndroidUI.Applications;
using AndroidUI.Utils;

namespace AndroidUI.AnimationFramework.Animation
{
    /**
     * Represents a group of Animations that should be played together.
     * The transformation of each individual animation are composed
     * together into a single transform.
     * If AnimationSet sets any properties that its children also set
     * (for example, duration or fillBefore), the values of AnimationSet
     * override the child values.
     *
     * <p>The way that AnimationSet inherits behavior from Animation is important to
     * understand. Some of the Animation attributes applied to AnimationSet affect the
     * AnimationSet itself, some are pushed down to the children, and some are ignored,
     * as follows:
     * <ul>
     *     <li>duration, repeatMode, fillBefore, fillAfter: These properties, when set
     *     on an AnimationSet object, will be pushed down to all child animations.</li>
     *     <li>repeatCount, fillEnabled: These properties are ignored for AnimationSet.</li>
     *     <li>startOffset, shareInterpolator: These properties apply to the AnimationSet itself.</li>
     * </ul>
     * Starting with {@link android.os.Build.VERSION_CODES#ICE_CREAM_SANDWICH},
     * the behavior of these properties is the same in XML resources and at runtime (prior to that
     * release, the values set in XML were ignored for AnimationSet). That is, calling
     * <code>setDuration(500)</code> on an AnimationSet has the same effect as declaring
     * <code>android:duration="500"</code> in an XML resource for an AnimationSet object.</p>
     */
    public class AnimationSet : Animation, Utils.ICloneable
    {
        private const int PROPERTY_FILL_AFTER_MASK = 0x1;
        private const int PROPERTY_FILL_BEFORE_MASK = 0x2;
        private const int PROPERTY_REPEAT_MODE_MASK = 0x4;
        private const int PROPERTY_START_OFFSET_MASK = 0x8;
        private const int PROPERTY_SHARE_INTERPOLATOR_MASK = 0x10;
        private const int PROPERTY_DURATION_MASK = 0x20;
        private const int PROPERTY_MORPH_MATRIX_MASK = 0x40;
        private const int PROPERTY_CHANGE_BOUNDS_MASK = 0x80;

        private int mFlags = 0;
        private bool mDirty;
        private bool mHasAlpha;

        private List<Animation> mAnimations = new();

        private Transformation mTempTransformation = new();

        private long mLastEnd;

        private long[] mStoredOffsets;

        /**
         * Constructor to use when building an AnimationSet from code
         *
         * @param shareInterpolator Pass true if all of the animations in this set
         *        should use the interpolator associated with this AnimationSet.
         *        Pass false if each animation should use its own interpolator.
         */
        public AnimationSet(Context context, bool shareInterpolator) : base(context)
        {
            setFlag(PROPERTY_SHARE_INTERPOLATOR_MASK, shareInterpolator);
            init();
        }

        public override AnimationSet Clone()
        {
            AnimationSet animation = (AnimationSet)base.Clone();
            animation.mFlags = mFlags;
            animation.mDirty = mDirty;
            animation.mHasAlpha = mHasAlpha;

            animation.mTempTransformation = new Transformation();
            animation.mAnimations = new List<Animation>();

            int count = mAnimations.Count;
            List<Animation> animations = mAnimations;

            for (int i = 0; i < count; i++)
            {
                animation.mAnimations.Add(animations.ElementAt(i).Clone());
            }

            return animation;
        }

        private void setFlag(int mask, bool value)
        {
            if (value)
            {
                mFlags |= mask;
            }
            else
            {
                mFlags &= ~mask;
            }
        }

        private void init()
        {
            mStartTime = 0;
        }

        override
            public void setFillAfter(bool fillAfter)
        {
            mFlags |= PROPERTY_FILL_AFTER_MASK;
            base.setFillAfter(fillAfter);
        }

        override
            public void setFillBefore(bool fillBefore)
        {
            mFlags |= PROPERTY_FILL_BEFORE_MASK;
            base.setFillBefore(fillBefore);
        }

        override
            public void setRepeatMode(int repeatMode)
        {
            mFlags |= PROPERTY_REPEAT_MODE_MASK;
            base.setRepeatMode(repeatMode);
        }

        override
            public void setStartOffset(long startOffset)
        {
            mFlags |= PROPERTY_START_OFFSET_MASK;
            base.setStartOffset(startOffset);
        }

        /**
         * @hide
         */
        override
            internal bool hasAlpha()
        {
            if (mDirty)
            {
                mDirty = mHasAlpha = false;

                int count = mAnimations.Count;
                List<Animation> animations = mAnimations;

                for (int i = 0; i < count; i++)
                {
                    if (animations.ElementAt(i).hasAlpha())
                    {
                        mHasAlpha = true;
                        break;
                    }
                }
            }

            return mHasAlpha;
        }

        /**
         * <p>Sets the duration of every child animation.</p>
         *
         * @param durationMillis the duration of the animation, in milliseconds, for
         *        every child in this set
         */
        override
            public void setDuration(long durationMillis)
        {
            mFlags |= PROPERTY_DURATION_MASK;
            base.setDuration(durationMillis);
            mLastEnd = mStartOffset + mDuration;
        }

        /**
         * Add a child animation to this animation set.
         * The transforms of the child animations are applied in the order
         * that they were added
         * @param a Animation to add.
         */
        public void addAnimation(Animation a)
        {
            mAnimations.Add(a);

            bool noMatrix = (mFlags & PROPERTY_MORPH_MATRIX_MASK) == 0;
            if (noMatrix && a.willChangeTransformationMatrix())
            {
                mFlags |= PROPERTY_MORPH_MATRIX_MASK;
            }

            bool changeBounds = (mFlags & PROPERTY_CHANGE_BOUNDS_MASK) == 0;


            if (changeBounds && a.willChangeBounds())
            {
                mFlags |= PROPERTY_CHANGE_BOUNDS_MASK;
            }

            if ((mFlags & PROPERTY_DURATION_MASK) == PROPERTY_DURATION_MASK)
            {
                mLastEnd = mStartOffset + mDuration;
            }
            else
            {
                if (mAnimations.Count == 1)
                {
                    mDuration = a.getStartOffset() + a.getDuration();
                    mLastEnd = mStartOffset + mDuration;
                }
                else
                {
                    mLastEnd = Math.Max(mLastEnd, mStartOffset + a.getStartOffset() + a.getDuration());
                    mDuration = mLastEnd - mStartOffset;
                }
            }

            mDirty = true;
        }

        /**
         * Sets the start time of this animation and all child animations
         *
         * @see android.view.animation.Animation#setStartTime(long)
         */
        override
            public void setStartTime(long startTimeMillis)
        {
            base.setStartTime(startTimeMillis);

            int count = mAnimations.Count;
            List<Animation> animations = mAnimations;

            for (int i = 0; i < count; i++)
            {
                Animation a = animations.ElementAt(i);
                a.setStartTime(startTimeMillis);
            }
        }

        override
            public long getStartTime()
        {
            long startTime = long.MaxValue;

            int count = mAnimations.Count;
            List<Animation> animations = mAnimations;

            for (int i = 0; i < count; i++)
            {
                Animation a = animations.ElementAt(i);
                startTime = Math.Min(startTime, a.getStartTime());
            }

            return startTime;
        }

        override
            public void restrictDuration(long durationMillis)
        {
            base.restrictDuration(durationMillis);

            List<Animation> animations = mAnimations;
            int count = animations.Count;

            for (int i = 0; i < count; i++)
            {
                animations.ElementAt(i).restrictDuration(durationMillis);
            }
        }

        /**
         * The duration of an AnimationSet is defined to be the
         * duration of the longest child animation.
         *
         * @see android.view.animation.Animation#getDuration()
         */
        override
            public long getDuration()
        {
            List<Animation> animations = mAnimations;
            int count = animations.Count;
            long duration = 0;

            bool durationSet = (mFlags & PROPERTY_DURATION_MASK) == PROPERTY_DURATION_MASK;
            if (durationSet)
            {
                duration = mDuration;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    duration = Math.Max(duration, animations.ElementAt(i).getDuration());
                }
            }

            return duration;
        }

        /**
         * The duration hint of an animation set is the maximum of the duration
         * hints of all of its component animations.
         *
         * @see android.view.animation.Animation#computeDurationHint
         */
        public override long computeDurationHint()
        {
            long duration = 0;
            int count = mAnimations.Count;
            List<Animation> animations = mAnimations;
            for (int i = count - 1; i >= 0; --i)
            {
                long d = animations.ElementAt(i).computeDurationHint();
                if (d > duration) duration = d;
            }
            return duration;
        }

        /**
         * @hide
         */
        internal override void initializeInvalidateRegion(int left, int top, int right, int bottom)
        {
            RectF region = mPreviousRegion;
            region.set(left, top, right, bottom);
            region.inset(-1.0f, -1.0f);

            if (mFillBefore)
            {
                int count = mAnimations.Count;
                List<Animation> animations = mAnimations;
                Transformation temp = mTempTransformation;

                Transformation previousTransformation = mPreviousTransformation;

                for (int i = count - 1; i >= 0; --i)
                {
                    Animation a = animations.ElementAt(i);
                    if (!a.isFillEnabled() || a.getFillBefore() || a.getStartOffset() == 0)
                    {
                        temp.clear();
                        Interpolators.Interpolator interpolator = a.mInterpolator;
                        a.applyTransformation(interpolator != null ? interpolator.getInterpolation(0.0f)
                                : 0.0f, temp);
                        previousTransformation.compose(temp);
                    }
                }
            }
        }

        /**
         * The transformation of an animation set is the concatenation of all of its
         * component animations.
         *
         * @see android.view.animation.Animation#getTransformation
         */
        override
            public bool getTransformation(long currentTime, Transformation t)
        {
            int count = mAnimations.Count;
            List<Animation> animations = mAnimations;
            Transformation temp = mTempTransformation;

            bool more = false;
            bool started = false;
            bool ended = true;

            t.clear();

            for (int i = count - 1; i >= 0; --i)
            {
                Animation a = animations.ElementAt(i);

                temp.clear();
                more = a.getTransformation(currentTime, temp, getScaleFactor()) || more;
                t.compose(temp);

                started = started || a.hasStarted();
                ended = a.hasEnded() && ended;
            }

            if (started && !mStarted)
            {
                dispatchAnimationStart();
                mStarted = true;
            }

            if (ended != mEnded)
            {
                dispatchAnimationEnd();
                mEnded = ended;
            }

            return more;
        }

        /**
         * @see android.view.animation.Animation#scaleCurrentDuration(float)
         */
        override
            public void scaleCurrentDuration(float scale)
        {
            List<Animation> animations = mAnimations;
            int count = animations.Count;
            for (int i = 0; i < count; i++)
            {
                animations.ElementAt(i).scaleCurrentDuration(scale);
            }
        }

        /**
         * @see android.view.animation.Animation#initialize(int, int, int, int)
         */
        override
            public void initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.initialize(width, height, parentWidth, parentHeight);

            bool durationSet = (mFlags & PROPERTY_DURATION_MASK) == PROPERTY_DURATION_MASK;
            bool fillAfterSet = (mFlags & PROPERTY_FILL_AFTER_MASK) == PROPERTY_FILL_AFTER_MASK;
            bool fillBeforeSet = (mFlags & PROPERTY_FILL_BEFORE_MASK) == PROPERTY_FILL_BEFORE_MASK;
            bool repeatModeSet = (mFlags & PROPERTY_REPEAT_MODE_MASK) == PROPERTY_REPEAT_MODE_MASK;
            bool shareInterpolator = (mFlags & PROPERTY_SHARE_INTERPOLATOR_MASK)
                    == PROPERTY_SHARE_INTERPOLATOR_MASK;
            bool startOffsetSet = (mFlags & PROPERTY_START_OFFSET_MASK)
                    == PROPERTY_START_OFFSET_MASK;

            if (shareInterpolator)
            {
                ensureInterpolator();
            }

            List<Animation> children = mAnimations;
            int count = children.Count;

            long duration = mDuration;
            bool fillAfter = mFillAfter;
            bool fillBefore = mFillBefore;
            int repeatMode = mRepeatMode;
            Interpolators.Interpolator interpolator = mInterpolator;
            long startOffset = mStartOffset;


            long[] storedOffsets = mStoredOffsets;
            if (startOffsetSet)
            {
                if (storedOffsets == null || storedOffsets.Length != count)
                {
                    storedOffsets = mStoredOffsets = new long[count];
                }
            }
            else if (storedOffsets != null)
            {
                storedOffsets = mStoredOffsets = null;
            }

            for (int i = 0; i < count; i++)
            {
                Animation a = children.ElementAt(i);
                if (durationSet)
                {
                    a.setDuration(duration);
                }
                if (fillAfterSet)
                {
                    a.setFillAfter(fillAfter);
                }
                if (fillBeforeSet)
                {
                    a.setFillBefore(fillBefore);
                }
                if (repeatModeSet)
                {
                    a.setRepeatMode(repeatMode);
                }
                if (shareInterpolator)
                {
                    a.setInterpolator(interpolator);
                }
                if (startOffsetSet)
                {
                    long offset = a.getStartOffset();
                    a.setStartOffset(offset + startOffset);
                    storedOffsets[i] = offset;
                }
                a.initialize(width, height, parentWidth, parentHeight);
            }
        }

        override
            public void reset()
        {
            base.reset();
            restoreChildrenStartOffset();
        }

        /**
         * @hide
         */
        void restoreChildrenStartOffset()
        {
            long[] offsets = mStoredOffsets;
            if (offsets == null) return;

            List<Animation> children = mAnimations;
            int count = children.Count;

            for (int i = 0; i < count; i++)
            {
                children.ElementAt(i).setStartOffset(offsets[i]);
            }
        }

        /**
         * @return All the child animations in this AnimationSet. Note that
         * this may include other AnimationSets, which are not expanded.
         */
        public List<Animation> getAnimations()
        {
            return mAnimations;
        }

        override
            public bool willChangeTransformationMatrix()
        {
            return (mFlags & PROPERTY_MORPH_MATRIX_MASK) == PROPERTY_MORPH_MATRIX_MASK;
        }

        override
            public bool willChangeBounds()
        {
            return (mFlags & PROPERTY_CHANGE_BOUNDS_MASK) == PROPERTY_CHANGE_BOUNDS_MASK;
        }
    }
}
