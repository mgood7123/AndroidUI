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

using AndroidUI.Graphics;
using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;

namespace AndroidUI.Widgets
{

    /**
     * FrameLayout is designed to block out an area on the screen to display
     * a single item. Generally, FrameLayout should be used to hold a single child view, because it can
     * be difficult to organize child views in a way that's scalable to different screen sizes without
     * the children overlapping each other. You can, however, add multiple children to a FrameLayout
     * and control their position within the FrameLayout by assigning gravity to each child, using the
     * <a href="FrameLayout.LayoutParams.html#attr_android:layout_gravity">{@code
     * android:layout_gravity}</a> attribute.
     * <p>Child views are drawn in a stack, with the most recently added child on top.
     * The size of the FrameLayout is the size of its largest child (plus padding), visible
     * or not (if the FrameLayout's parent permits). Views that are {@link android.view.View#GONE} are
     * used for sizing
     * only if {@link #setMeasureAllChildren(bool) setConsiderGoneChildrenWhenMeasuring()}
     * is set to true.
     *
     * @attr ref android.R.styleable#FrameLayout_measureAllChildren
     */
    public class FrameLayout : View
    {
        private const int DEFAULT_CHILD_GRAVITY = Gravity.TOP | Gravity.START;

        bool mMeasureAllChildren = false;

        private int mForegroundPaddingLeft = 0;

        private int mForegroundPaddingTop = 0;

        private int mForegroundPaddingRight = 0;

        private int mForegroundPaddingBottom = 0;

        private readonly List<View> mMatchParentChildren = new List<View>(1);

        /**
         * Describes how the foreground is positioned. Defaults to START and TOP.
         *
         * @param foregroundGravity See {@link android.view.Gravity}
         *
         * @see #getForegroundGravity()
         *
         * @attr ref android.R.styleable#View_foregroundGravity
         */
        public void setForegroundGravity(int foregroundGravity)
        {
            if (getForegroundGravity() != foregroundGravity)
            {
                base.setForegroundGravity(foregroundGravity);

                // calling get* again here because the set above may apply default constraints
                Drawable foreground = getForeground();
                if (getForegroundGravity() == Gravity.FILL && foreground != null)
                {
                    Rect padding = new Rect();
                    if (foreground.getPadding(padding))
                    {
                        mForegroundPaddingLeft = padding.left;
                        mForegroundPaddingTop = padding.top;
                        mForegroundPaddingRight = padding.right;
                        mForegroundPaddingBottom = padding.bottom;
                    }
                }
                else
                {
                    mForegroundPaddingLeft = 0;
                    mForegroundPaddingTop = 0;
                    mForegroundPaddingRight = 0;
                    mForegroundPaddingBottom = 0;
                }

                requestLayout();
            }
        }

        /**
         * Returns a set of layout parameters with a width of
         * {@link android.view.ViewGroup.LayoutParams#MATCH_PARENT},
         * and a height of {@link android.view.ViewGroup.LayoutParams#MATCH_PARENT}.
         */
        protected LayoutParams generateDefaultLayoutParams()
        {
            return new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
        }

        int getPaddingLeftWithForeground()
        {
            return isForegroundInsidePadding() ? Math.Max(mPaddingLeft, mForegroundPaddingLeft) :
                mPaddingLeft + mForegroundPaddingLeft;
        }

        int getPaddingRightWithForeground()
        {
            return isForegroundInsidePadding() ? Math.Max(mPaddingRight, mForegroundPaddingRight) :
                mPaddingRight + mForegroundPaddingRight;
        }

        private int getPaddingTopWithForeground()
        {
            return isForegroundInsidePadding() ? Math.Max(mPaddingTop, mForegroundPaddingTop) :
                mPaddingTop + mForegroundPaddingTop;
        }

        private int getPaddingBottomWithForeground()
        {
            return isForegroundInsidePadding() ? Math.Max(mPaddingBottom, mForegroundPaddingBottom) :
                mPaddingBottom + mForegroundPaddingBottom;
        }

        override
        protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int count = getChildCount();

            bool measureMatchParentChildren =
                    MeasureSpec.getMode(widthMeasureSpec) != MeasureSpec.EXACTLY ||
                    MeasureSpec.getMode(heightMeasureSpec) != MeasureSpec.EXACTLY;
            mMatchParentChildren.Clear();

            int maxHeight = 0;
            int maxWidth = 0;
            int childState = 0;

            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                if (mMeasureAllChildren || child.getVisibility() != GONE)
                {
                    measureChildWithMargins(child, widthMeasureSpec, 0, heightMeasureSpec, 0);
                    LayoutParams lp = (LayoutParams)child.getLayoutParams();
                    maxWidth = Math.Max(maxWidth,
                            child.getMeasuredWidth() + lp.leftMargin + lp.rightMargin);
                    maxHeight = Math.Max(maxHeight,
                            child.getMeasuredHeight() + lp.topMargin + lp.bottomMargin);
                    childState = combineMeasuredStates(childState, child.getMeasuredState());
                    if (measureMatchParentChildren)
                    {
                        if (lp.width == LayoutParams.MATCH_PARENT ||
                                lp.height == LayoutParams.MATCH_PARENT)
                        {
                            mMatchParentChildren.Add(child);
                        }
                    }
                }
            }

            // Account for padding too
            maxWidth += getPaddingLeftWithForeground() + getPaddingRightWithForeground();
            maxHeight += getPaddingTopWithForeground() + getPaddingBottomWithForeground();

            // Check against our minimum height and width
            maxHeight = Math.Max(maxHeight, getSuggestedMinimumHeight());
            maxWidth = Math.Max(maxWidth, getSuggestedMinimumWidth());

            // Check against our foreground's minimum height and width
            Drawable drawable = getForeground();
            if (drawable != null)
            {
                maxHeight = Math.Max(maxHeight, drawable.getMinimumHeight());
                maxWidth = Math.Max(maxWidth, drawable.getMinimumWidth());
            }

            setMeasuredDimension(resolveSizeAndState(maxWidth, widthMeasureSpec, childState),
                    resolveSizeAndState(maxHeight, heightMeasureSpec,
                            childState << MEASURED_HEIGHT_STATE_SHIFT));

            count = mMatchParentChildren.Count;
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    View child = mMatchParentChildren.ElementAt(i);
                    MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

                    int childWidthMeasureSpec;
                    if (lp.width == LayoutParams.MATCH_PARENT)
                    {
                        int width = Math.Max(0, getMeasuredWidth()
                                - getPaddingLeftWithForeground() - getPaddingRightWithForeground()
                                - lp.leftMargin - lp.rightMargin);
                        childWidthMeasureSpec = MeasureSpec.makeMeasureSpec(
                                width, MeasureSpec.EXACTLY);
                    }
                    else
                    {
                        childWidthMeasureSpec = getChildMeasureSpec(this, child, true, widthMeasureSpec,
                                getPaddingLeftWithForeground() + getPaddingRightWithForeground() +
                                lp.leftMargin + lp.rightMargin,
                                lp.width);
                    }

                    int childHeightMeasureSpec;
                    if (lp.height == LayoutParams.MATCH_PARENT)
                    {
                        int height = Math.Max(0, getMeasuredHeight()
                                - getPaddingTopWithForeground() - getPaddingBottomWithForeground()
                                - lp.topMargin - lp.bottomMargin);
                        childHeightMeasureSpec = MeasureSpec.makeMeasureSpec(
                                height, MeasureSpec.EXACTLY);
                    }
                    else
                    {
                        childHeightMeasureSpec = getChildMeasureSpec(this, child, false, heightMeasureSpec,
                                getPaddingTopWithForeground() + getPaddingBottomWithForeground() +
                                lp.topMargin + lp.bottomMargin,
                                lp.height);
                    }

                    child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
                }
            }
        }

        override
        protected void onLayout(bool changed, int left, int top, int right, int bottom)
        {
            layoutChildren(left, top, right, bottom, false /* no force left gravity */);
        }

        void layoutChildren(int left, int top, int right, int bottom, bool forceLeftGravity)
        {
            int count = getChildCount();

            int parentLeft = getPaddingLeftWithForeground();
            int parentRight = right - left - getPaddingRightWithForeground();

            int parentTop = getPaddingTopWithForeground();
            int parentBottom = bottom - top - getPaddingBottomWithForeground();

            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                if (child.getVisibility() != GONE)
                {
                    LayoutParams lp = (LayoutParams)child.getLayoutParams();

                    int width = child.getMeasuredWidth();
                    int height = child.getMeasuredHeight();

                    int childLeft;
                    int childTop;

                    int gravity = lp.gravity;
                    if (gravity == -1)
                    {
                        gravity = DEFAULT_CHILD_GRAVITY;
                    }

                    int layoutDirection = getLayoutDirection();
                    int absoluteGravity = Gravity.getAbsoluteGravity(gravity, layoutDirection);
                    int verticalGravity = gravity & Gravity.VERTICAL_GRAVITY_MASK;

                    switch (absoluteGravity & Gravity.HORIZONTAL_GRAVITY_MASK)
                    {
                        case Gravity.CENTER_HORIZONTAL:
                            childLeft = parentLeft + (parentRight - parentLeft - width) / 2 +
                            lp.leftMargin - lp.rightMargin;
                            break;
                        case Gravity.RIGHT:
                            if (!forceLeftGravity)
                            {
                                childLeft = parentRight - width - lp.rightMargin;
                                break;
                            }
                            childLeft = parentLeft + lp.leftMargin;
                            break;
                        default:
                            childLeft = parentLeft + lp.leftMargin;
                            break;
                    }

                    switch (verticalGravity)
                    {
                        case Gravity.TOP:
                            childTop = parentTop + lp.topMargin;
                            break;
                        case Gravity.CENTER_VERTICAL:
                            childTop = parentTop + (parentBottom - parentTop - height) / 2 +
                            lp.topMargin - lp.bottomMargin;
                            break;
                        case Gravity.BOTTOM:
                            childTop = parentBottom - height - lp.bottomMargin;
                            break;
                        default:
                            childTop = parentTop + lp.topMargin;
                            break;
                    }

                    child.layout(childLeft, childTop, childLeft + width, childTop + height);
                }
            }
        }

        /**
         * Sets whether to consider all children, or just those in
         * the VISIBLE or INVISIBLE state, when measuring. Defaults to false.
         *
         * @param measureAll true to consider children marked GONE, false otherwise.
         * Default value is false.
         *
         * @attr ref android.R.styleable#FrameLayout_measureAllChildren
         */
        public void setMeasureAllChildren(bool measureAll)
        {
            mMeasureAllChildren = measureAll;
        }

        /**
         * Determines whether all children, or just those in the VISIBLE or
         * INVISIBLE state, are considered when measuring.
         *
         * @return Whether all children are considered when measuring.
         *
         * @deprecated This method is deprecated in favor of
         * {@link #getMeasureAllChildren() getMeasureAllChildren()}, which was
         * renamed for consistency with
         * {@link #setMeasureAllChildren(bool) setMeasureAllChildren()}.
         */
        [Obsolete]
        public bool getConsiderGoneChildrenWhenMeasuring()
        {
            return getMeasureAllChildren();
        }

        /**
         * Determines whether all children, or just those in the VISIBLE or
         * INVISIBLE state, are considered when measuring.
         *
         * @return Whether all children are considered when measuring.
         */
        public bool getMeasureAllChildren()
        {
            return mMeasureAllChildren;
        }

        override
        public LayoutParams generateLayoutParams()
        {
            return new FrameLayout.LayoutParams();
        }

        override
        public bool shouldDelayChildPressedState()
        {
            return false;
        }

        override
        protected bool checkLayoutParams(View.LayoutParams p)
        {
            return p is LayoutParams;
        }

        override
        protected View.LayoutParams generateLayoutParams(View.LayoutParams lp)
        {
            if (sPreserveMarginParamsInLayoutParamConversion)
            {
                if (lp is LayoutParams)
                {
                    return new LayoutParams((LayoutParams)lp);
                }
                else if (lp is MarginLayoutParams)
                {
                    return new LayoutParams((MarginLayoutParams)lp);
                }
            }
            return new LayoutParams(lp);
        }


        /**
         * Per-child layout information for layouts that support margins.
         * See {@link android.R.styleable#FrameLayout_Layout FrameLayout Layout Attributes}
         * for a list of all child view attributes that this class supports.
         *
         * @attr ref android.R.styleable#FrameLayout_Layout_layout_gravity
         */
        public new class LayoutParams : View.MarginLayoutParams
        {
            /**
             * Value for {@link #gravity} indicating that a gravity has not been
             * explicitly specified.
             */
            public const int UNSPECIFIED_GRAVITY = -1;

            /**
             * The gravity to apply with the View to which these layout parameters
             * are associated.
             * <p>
             * The default value is {@link #UNSPECIFIED_GRAVITY}, which is treated
             * by FrameLayout as {@code Gravity.TOP | Gravity.START}.
             *
             * @see android.view.Gravity
             * @attr ref android.R.styleable#FrameLayout_Layout_layout_gravity
             */
            public int gravity = UNSPECIFIED_GRAVITY;

            public LayoutParams() : base() {}

            public LayoutParams(int width, int height) : base(width, height)
            {
            }

            /**
             * Creates a new set of layout parameters with the specified width, height
             * and gravity.
             *
             * @param width the width, either {@link #MATCH_PARENT},
             *              {@link #WRAP_CONTENT} or a fixed size in pixels
             * @param height the height, either {@link #MATCH_PARENT},
             *               {@link #WRAP_CONTENT} or a fixed size in pixels
             * @param gravity the gravity
             *
             * @see android.view.Gravity
             */
            public LayoutParams(int width, int height, int gravity) : base(width, height)
            {
                this.gravity = gravity;
            }

            public LayoutParams(View.LayoutParams source) : base(source)
            {
            }

            public LayoutParams(View.MarginLayoutParams source) : base(source)
            {
            }

            /**
             * Copy constructor. Clones the width, height, margin values, and
             * gravity of the source.
             *
             * @param source The layout params to copy from.
             */
            public LayoutParams(LayoutParams source) : base(source)
            {
                gravity = source.gravity;
            }
        }
    }
}
