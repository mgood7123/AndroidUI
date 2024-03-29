﻿/*
 * Copyright (C) 2014 The Android Open Source Project
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
     * An animation that controls the clip of an object. See the
     * {@link android.view.animation full package} description for details and
     * sample code.
     *
     * @hide
     */
    public class ClipRectAnimation : Animation
    {
        protected Rect mFromRect = new();
        protected Rect mToRect = new();

        private int mFromLeftType = ABSOLUTE;
        private int mFromTopType = ABSOLUTE;
        private int mFromRightType = ABSOLUTE;
        private int mFromBottomType = ABSOLUTE;

        private int mToLeftType = ABSOLUTE;
        private int mToTopType = ABSOLUTE;
        private int mToRightType = ABSOLUTE;
        private int mToBottomType = ABSOLUTE;

        private float mFromLeftValue;
        private float mFromTopValue;
        private float mFromRightValue;
        private float mFromBottomValue;

        private float mToLeftValue;
        private float mToTopValue;
        private float mToRightValue;
        private float mToBottomValue;


        public override ClipRectAnimation Clone()
        {
            ClipRectAnimation animation = (ClipRectAnimation)base.Clone();
            animation.mFromRect = mFromRect;
            animation.mToRect = mToRect;

            animation.mFromLeftType = mFromLeftType;
            animation.mFromTopType = mFromTopType;
            animation.mFromRightType = mFromRightType;
            animation.mFromBottomType = mFromBottomType;

            animation.mToLeftType = mToLeftType;
            animation.mToTopType = mToTopType;
            animation.mToRightType = mToRightType;
            animation.mToBottomType = mToBottomType;

            animation.mFromLeftValue = mFromLeftValue;
            animation.mFromTopValue = mFromTopValue;
            animation.mFromRightValue = mFromRightValue;
            animation.mFromBottomValue = mFromBottomValue;

            animation.mToLeftValue = mToLeftValue;
            animation.mToTopValue = mToTopValue;
            animation.mToRightValue = mToRightValue;
            animation.mToBottomValue = mToBottomValue;


            return animation;
        }

        /**
         * Constructor to use when building a ClipRectAnimation from code
         *
         * @param fromClip the clip rect to animate from
         * @param toClip the clip rect to animate to
         */
        public ClipRectAnimation(Context context, Rect fromClip, Rect toClip) : base(context)
        {
            if (fromClip == null || toClip == null)
            {
                throw new Exception("Expected non-null animation clip rects");
            }
            mFromLeftValue = fromClip.left;
            mFromTopValue = fromClip.top;
            mFromRightValue = fromClip.right;
            mFromBottomValue = fromClip.bottom;

            mToLeftValue = toClip.left;
            mToTopValue = toClip.top;
            mToRightValue = toClip.right;
            mToBottomValue = toClip.bottom;
        }

        /**
         * Constructor to use when building a ClipRectAnimation from code
         */
        public ClipRectAnimation(
            Context context,
            int fromL, int fromT, int fromR, int fromB,
            int toL, int toT, int toR, int toB
        ) : this(
            context,
            new Rect(fromL, fromT, fromR, fromB),
            new Rect(toL, toT, toR, toB)
        )
        {
        }

        override
            public void applyTransformation(float it, Transformation tr)
        {
            int l = mFromRect.left + (int)((mToRect.left - mFromRect.left) * it);
            int t = mFromRect.top + (int)((mToRect.top - mFromRect.top) * it);
            int r = mFromRect.right + (int)((mToRect.right - mFromRect.right) * it);
            int b = mFromRect.bottom + (int)((mToRect.bottom - mFromRect.bottom) * it);
            tr.setClipRect(l, t, r, b);
        }

        override
            public bool willChangeTransformationMatrix()
        {
            return false;
        }

        override
            public void initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.initialize(width, height, parentWidth, parentHeight);
            mFromRect.set((int)resolveSize(mFromLeftType, mFromLeftValue, width, parentWidth),
                    (int)resolveSize(mFromTopType, mFromTopValue, height, parentHeight),
                    (int)resolveSize(mFromRightType, mFromRightValue, width, parentWidth),
                    (int)resolveSize(mFromBottomType, mFromBottomValue, height, parentHeight));
            mToRect.set((int)resolveSize(mToLeftType, mToLeftValue, width, parentWidth),
                    (int)resolveSize(mToTopType, mToTopValue, height, parentHeight),
                    (int)resolveSize(mToRightType, mToRightValue, width, parentWidth),
                    (int)resolveSize(mToBottomType, mToBottomValue, height, parentHeight));
        }
    }
}
