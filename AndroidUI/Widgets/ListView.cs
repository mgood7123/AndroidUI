using AndroidUI.Graphics;
using AndroidUI.Input;
using AndroidUI.Utils.Widgets;
using SkiaSharp;
using static AndroidUI.Widgets.LinearLayout;

namespace AndroidUI.Widgets
{
    public class ListView : View, ScrollHost
    {
        ScrollViewHostInstance host;
        public bool AutoScroll { get => host.autoScroll; set => host.autoScroll = value; }

        public bool SmoothScroll { get => host.SmoothScroll; set => host.SmoothScroll = value; }

        /**
         * Don't show any dividers.
         */
        public const int SHOW_DIVIDER_NONE = 0;
        /**
         * Show a divider at the beginning of the group.
         */
        public const int SHOW_DIVIDER_BEGINNING = 1;
        /**
         * Show dividers between each item in the group.
         */
        public const int SHOW_DIVIDER_MIDDLE = 2;
        /**
         * Show a divider at the end of the group.
         */
        public const int SHOW_DIVIDER_END = 4;


        /**
         * Whether the children of this layout are baseline aligned.  Only applicable
         * if {@link #mOrientation} is horizontal.
         */
        private bool mBaselineAligned = true;

        /**
         * If this layout is part of another layout that is baseline aligned,
         * use the child at this index as the baseline.
         *
         * Note: this is orthogonal to {@link #mBaselineAligned}, which is concerned
         * with whether the children of this layout are baseline aligned.
         */
        private int mBaselineAlignedChildIndex = -1;

        /**
         * The additional offset to the child's baseline.
         * We'll calculate the baseline of this layout as we measure vertically; for
         * horizontal linear layouts, the offset of 0 is appropriate.
         */
        private int mBaselineChildTop = 0;

        private OrientationMode mOrientation;

        private int mGravity = Gravity.START | Gravity.TOP;

        private int mTotalLength;

        private bool mUseLargestChild;

        private int[] mMaxAscent;
        private int[] mMaxDescent;

        private const int VERTICAL_GRAVITY_COUNT = 4;

        private const int INDEX_CENTER_VERTICAL = 0;
        private const int INDEX_TOP = 1;
        private const int INDEX_BOTTOM = 2;
        private const int INDEX_FILL = 3;

        private Drawable mDivider;
        private int mDividerWidth;
        private int mDividerHeight;
        private int mShowDividers;
        private int mDividerPadding;

        //private int mLayoutDirection = View.LAYOUT_DIRECTION_UNDEFINED;

        public ListView() : base()
        {
            host = new(this);

            SmoothScroll = true;

            setOrientation(OrientationMode.VERTICAL);

            setGravity(0);

            setBaselineAligned(true);

            mBaselineAlignedChildIndex = -1;

            mUseLargestChild = false;

            mShowDividers = SHOW_DIVIDER_NONE;
            mDividerPadding = 0;
            setDividerDrawable(null);
        }

        /**
         * Returns <code>true</code> if this layout is currently configured to show at least one
         * divider.
         */
        private bool isShowingDividers()
        {
            return mShowDividers != SHOW_DIVIDER_NONE && mDivider != null;
        }

        /**
         * Set how dividers should be shown between items in this layout
         *
         * @param showDividers One or more of {@link #SHOW_DIVIDER_BEGINNING},
         *                     {@link #SHOW_DIVIDER_MIDDLE}, or {@link #SHOW_DIVIDER_END}
         *                     to show dividers, or {@link #SHOW_DIVIDER_NONE} to show no dividers.
         */
        public void setShowDividers(int showDividers)
        {
            if (showDividers == mShowDividers)
            {
                return;
            }
            mShowDividers = showDividers;

            bool showingDividers = isShowingDividers();
            setWillDraw(showingDividers || host.SmoothScroll);
            if (showingDividers)
            {
                requestLayout();
                invalidate();
            }
        }

        public bool shouldDelayChildPressedState()
        {
            return false;
        }

        /**
         * @return A flag set indicating how dividers should be shown around items.
         * @see #setShowDividers(int)
         */
        public int getShowDividers()
        {
            return mShowDividers;
        }

        /**
         * @return the divider Drawable that will divide each item.
         *
         * @see #setDividerDrawable(Drawable)
         *
         * @attr ref android.R.styleable#ListView_divider
         */
        public Drawable getDividerDrawable()
        {
            return mDivider;
        }

        /**
         * Set a drawable to be used as a divider between items.
         *
         * @param divider Drawable that will divide each item.
         *
         * @see #setShowDividers(int)
         *
         * @attr ref android.R.styleable#ListView_divider
         */
        public void setDividerDrawable(Drawable divider)
        {
            if (divider == mDivider)
            {
                return;
            }
            mDivider = divider;
            if (divider != null)
            {
                mDividerWidth = divider.getIntrinsicWidth();
                mDividerHeight = divider.getIntrinsicHeight();
            }
            else
            {
                mDividerWidth = 0;
                mDividerHeight = 0;
            }

            bool showingDividers = isShowingDividers();
            setWillDraw(showingDividers || host.SmoothScroll);
            if (showingDividers)
            {
                requestLayout();
                invalidate();
            }
        }

        /**
         * Set padding displayed on both ends of dividers. For a vertical layout, the padding is applied
         * to left and right end of dividers. For a horizontal layout, the padding is applied to top and
         * bottom end of dividers.
         *
         * @param padding Padding value in pixels that will be applied to each end
         *
         * @see #setShowDividers(int)
         * @see #setDividerDrawable(Drawable)
         * @see #getDividerPadding()
         */
        public void setDividerPadding(int padding)
        {
            if (padding == mDividerPadding)
            {
                return;
            }
            mDividerPadding = padding;

            if (isShowingDividers())
            {
                requestLayout();
                invalidate();
            }
        }

        /**
         * Get the padding size used to inset dividers in pixels
         *
         * @see #setShowDividers(int)
         * @see #setDividerDrawable(Drawable)
         * @see #setDividerPadding(int)
         */
        public int getDividerPadding()
        {
            return mDividerPadding;
        }

        /**
         * Get the width of the current divider drawable.
         *
         * @hide Used internally by framework.
         */
        public int getDividerWidth()
        {
            return mDividerWidth;
        }

        public override void onConfigureTouch(Touch touch)
        {
            host.onConfigureTouch(touch);
        }

        public override bool onInterceptTouchEvent(Touch ev)
        {
            return host.InterceptTouch(this, t => base.onInterceptTouchEvent(t), this, ev);
        }

        protected override void onDraw(SKCanvas canvas)
        {
            base.onDraw(canvas);
            host.flywheel.AquireLock();
            host.OnDraw(this, this);
            host.flywheel.ReleaseLock();

            if (mDivider == null)
            {
                return;
            }

            if (mOrientation == OrientationMode.VERTICAL)
            {
                drawDividersVertical(canvas);
            }
            else
            {
                drawDividersHorizontal(canvas);
            }
        }

        void drawDividersVertical(SKCanvas canvas)
        {
            int count = getVirtualChildCount();
            for (int i = 0; i < count; i++)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    if (hasDividerBeforeChildAt(i))
                    {
                        MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();
                        int top_margin = 0;
                        if (lp is MarginLayoutParams)
                        {
                            top_margin = lp.topMargin;
                        }
                        int top = child.getTop() - top_margin - mDividerHeight;
                        drawHorizontalDivider(canvas, top);
                    }
                }
            }

            if (hasDividerBeforeChildAt(count))
            {
                View child = getLastNonGoneChild();
                int bottom = 0;
                if (child == null)
                {
                    bottom = getHeight() - getPaddingBottom() - mDividerHeight;
                }
                else
                {
                    MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();
                    bottom = child.getBottom() + lp.bottomMargin;
                }
                drawHorizontalDivider(canvas, bottom);
            }
        }

        /**
         * Finds the last child that is not gone. The last child will be used as the reference for
         * where the end divider should be drawn.
         */
        private View getLastNonGoneChild()
        {
            for (int i = getVirtualChildCount() - 1; i >= 0; i--)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    return child;
                }
            }
            return null;
        }

        void drawDividersHorizontal(SKCanvas canvas)
        {
            int count = getVirtualChildCount();
            bool isLayoutRtl_ = isLayoutRtl();
            for (int i = 0; i < count; i++)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    if (hasDividerBeforeChildAt(i))
                    {
                        MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();
                        int position;
                        if (isLayoutRtl_)
                        {
                            position = child.getRight() + lp.rightMargin;
                        }
                        else
                        {
                            position = child.getLeft() - lp.rightMargin - mDividerWidth;
                        }
                        drawVerticalDivider(canvas, position);
                    }
                }
            }

            if (hasDividerBeforeChildAt(count))
            {
                View child = getLastNonGoneChild();
                int position;
                if (child == null)
                {
                    if (isLayoutRtl_)
                    {
                        position = getPaddingLeft();
                    }
                    else
                    {
                        position = getWidth() - getPaddingRight() - mDividerWidth;
                    }
                }
                else
                {
                    MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();
                    if (isLayoutRtl_)
                    {
                        position = child.getLeft() - lp.leftMargin - mDividerWidth;
                    }
                    else
                    {
                        position = child.getRight() + lp.rightMargin;
                    }
                }
                drawVerticalDivider(canvas, position);
            }
        }

        void drawHorizontalDivider(SKCanvas canvas, int top)
        {
            //mDivider.setBounds(getPaddingLeft() + mDividerPadding, top,
            //getWidth() - getPaddingRight() - mDividerPadding, top + mDividerHeight);
            //mDivider.draw(canvas);
        }

        void drawVerticalDivider(SKCanvas canvas, int left)
        {
            //mDivider.setBounds(left, getPaddingTop() + mDividerPadding,
            //left + mDividerWidth, getHeight() - getPaddingBottom() - mDividerPadding);
            //mDivider.draw(canvas);
        }

        /**
         * <p>Indicates whether widgets contained within this layout are aligned
         * on their baseline or not.</p>
         *
         * @return true when widgets are baseline-aligned, false otherwise
         */
        public bool isBaselineAligned()
        {
            return mBaselineAligned;
        }

        /**
         * <p>Defines whether widgets contained in this layout are
         * baseline-aligned or not.</p>
         *
         * @param baselineAligned true to align widgets on their baseline,
         *         false otherwise
         *
         * @attr ref android.R.styleable#ListView_baselineAligned
         */
        public void setBaselineAligned(bool baselineAligned)
        {
            mBaselineAligned = baselineAligned;
        }

        /**
         * When true, all children with a weight will be considered having
         * the minimum size of the largest child. If false, all children are
         * measured normally.
         *
         * @return True to measure children with a weight using the minimum
         *         size of the largest child, false otherwise.
         *
         * @attr ref android.R.styleable#ListView_measureWithLargestChild
         */
        public bool isMeasureWithLargestChildEnabled()
        {
            return mUseLargestChild;
        }

        /**
         * When set to true, all children with a weight will be considered having
         * the minimum size of the largest child. If false, all children are
         * measured normally.
         *
         * Disabled by default.
         *
         * @param enabled True to measure children with a weight using the
         *        minimum size of the largest child, false otherwise.
         *
         * @attr ref android.R.styleable#ListView_measureWithLargestChild
         */
        public void setMeasureWithLargestChildEnabled(bool enabled)
        {
            mUseLargestChild = enabled;
        }

        public override int getBaseline()
        {
            if (mBaselineAlignedChildIndex < 0)
            {
                return base.getBaseline();
            }

            if (mChildrenCount <= mBaselineAlignedChildIndex)
            {
                throw new Exception("mBaselineAlignedChildIndex of ListView "
                        + "set to an index that is out of bounds.");
            }

            View child = getChildAt(mBaselineAlignedChildIndex);
            int childBaseline = child.getBaseline();

            if (childBaseline == -1)
            {
                if (mBaselineAlignedChildIndex == 0)
                {
                    // this is just the default case, safe to return -1
                    return -1;
                }
                // the user picked an index that points to something that doesn't
                // know how to calculate its baseline.
                throw new Exception("mBaselineAlignedChildIndex of ListView "
                        + "points to a View that doesn't know how to get its baseline.");
            }

            // TODO: This should try to take into account the virtual offsets
            // (See getNextLocationOffset and getLocationOffset)
            // We should add to childTop:
            // sum([getNextLocationOffset(getChildAt(i)) / i < mBaselineAlignedChildIndex])
            // and also add:
            // getLocationOffset(child)
            int childTop = mBaselineChildTop;

            if (mOrientation == OrientationMode.VERTICAL)
            {
                int majorGravity = mGravity & Gravity.VERTICAL_GRAVITY_MASK;
                if (majorGravity != Gravity.TOP)
                {
                    switch (majorGravity)
                    {
                        case Gravity.BOTTOM:
                            childTop = mBottom - mTop - mPaddingBottom - mTotalLength;
                            break;

                        case Gravity.CENTER_VERTICAL:
                            childTop += (mBottom - mTop - mPaddingTop - mPaddingBottom -
                                    mTotalLength) / 2;
                            break;
                    }
                }
            }

            MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();
            return childTop + lp.topMargin + childBaseline;
        }

        /**
         * @return The index of the child that will be used if this layout is
         *   part of a larger layout that is baseline aligned, or -1 if none has
         *   been set.
         */
        public int getBaselineAlignedChildIndex()
        {
            return mBaselineAlignedChildIndex;
        }

        /**
         * @param i The index of the child that will be used if this layout is
         *          part of a larger layout that is baseline aligned.
         *
         * @attr ref android.R.styleable#ListView_baselineAlignedChildIndex
         */
        public void setBaselineAlignedChildIndex(int i)
        {
            if (i < 0 || i >= mChildrenCount)
            {
                throw new Exception("base aligned child index out "
                        + "of range (0, " + mChildrenCount + ")");
            }
            mBaselineAlignedChildIndex = i;
        }

        /**
         * <p>Returns the view at the specified index. This method can be overridden
         * to take into account virtual children. Refer to
         * {@link android.widget.TableLayout} and {@link android.widget.TableRow}
         * for an example.</p>
         *
         * @param index the child's index
         * @return the child at the specified index, may be {@code null}
         */
        View getVirtualChildAt(int index)
        {
            return getChildAt(index);
        }

        /**
         * <p>Returns the virtual number of children. This number might be different
         * than the actual number of children if the layout can hold virtual
         * children. Refer to
         * {@link android.widget.TableLayout} and {@link android.widget.TableRow}
         * for an example.</p>
         *
         * @return the virtual number of children
         */
        int getVirtualChildCount()
        {
            return mChildrenCount;
        }

        protected override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (mOrientation == OrientationMode.VERTICAL)
            {
                measureVertical(widthMeasureSpec, heightMeasureSpec);
            }
            else
            {
                measureHorizontal(widthMeasureSpec, heightMeasureSpec);
            }
            if (AutoScroll)
            {
                host.AutoScrollOnMeasure(this, this);
            }
        }

        /**
         * Determines where to position dividers between children.
         *
         * @param childIndex Index of child to check for preceding divider
         * @return true if there should be a divider before the child at childIndex
         * @hide Pending API consideration. Currently only used internally by the system.
         */
        protected bool hasDividerBeforeChildAt(int childIndex)
        {
            if (childIndex == getVirtualChildCount())
            {
                // Check whether the end divider should draw.
                return (mShowDividers & SHOW_DIVIDER_END) != 0;
            }
            bool allViewsAreGoneBefore_ = allViewsAreGoneBefore(childIndex);
            if (allViewsAreGoneBefore_)
            {
                // This is the first view that's not gone, check if beginning divider is enabled.
                return (mShowDividers & SHOW_DIVIDER_BEGINNING) != 0;
            }
            else
            {
                return (mShowDividers & SHOW_DIVIDER_MIDDLE) != 0;
            }
        }

        /**
         * Checks whether all (virtual) child views before the given index are gone.
         */
        private bool allViewsAreGoneBefore(int childIndex)
        {
            for (int i = childIndex - 1; i >= 0; i--)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * Measures the children when the orientation of this ListView is set
         * to {@link #VERTICAL}.
         *
         * @param widthMeasureSpec Horizontal space requirements as imposed by the parent.
         * @param heightMeasureSpec Vertical space requirements as imposed by the parent.
         *
         * @see #getOrientation()
         * @see #setOrientation(int)
         * @see #onMeasure(int, int)
         */
        void measureVertical(int widthMeasureSpec, int heightMeasureSpec)
        {
            mTotalLength = 0;
            int maxWidth = 0;
            int childState = 0;
            int alternativeMaxWidth = 0;
            bool allFillParent = true;

            int count = getVirtualChildCount();

            int widthMode = MeasureSpec.getMode(widthMeasureSpec);

            bool matchWidth = false;

            int baselineChildIndex = mBaselineAlignedChildIndex;
            int nonSkippedChildCount = 0;

            // See how tall everyone is. Also remember max width.
            for (int i = 0; i < count; ++i)
            {
                View child = getVirtualChildAt(i);
                if (child == null)
                {
                    mTotalLength += measureNullChild(i);
                    continue;
                }

                if (child.getVisibility() == GONE)
                {
                    i += getChildrenSkipCount(child, i);
                    continue;
                }

                nonSkippedChildCount++;
                if (hasDividerBeforeChildAt(i))
                {
                    mTotalLength += mDividerHeight;
                }

                MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

                // Determine how big this child would like to be. If this or
                // previous children have given a weight, then we allow it to
                // use all available space (and we will shrink things later
                // if needed).
                int usedHeight = mTotalLength;
                measureChildBeforeLayout(child, i, widthMeasureSpec, 0,
                        heightMeasureSpec, usedHeight);

                int childHeight = child.getMeasuredHeight();

                int totalLength = mTotalLength;
                mTotalLength = Math.Max(totalLength, totalLength + childHeight + lp.topMargin +
                        lp.bottomMargin + getNextLocationOffset(child));

                /**
                 * If applicable, compute the additional offset to the child's baseline
                 * we'll need later when asked {@link #getBaseline}.
                 */
                if (baselineChildIndex >= 0 && baselineChildIndex == i + 1)
                {
                    mBaselineChildTop = mTotalLength;
                }

                bool matchWidthLocally = false;
                if (widthMode != MeasureSpec.EXACTLY && lp.width == View.LayoutParams.MATCH_PARENT)
                {
                    // The width of the linear layout will scale, and at least one
                    // child said it wanted to match our width. Set a flag
                    // indicating that we need to remeasure at least that view when
                    // we know our width.
                    matchWidth = true;
                    matchWidthLocally = true;
                }

                int margin = lp.leftMargin + lp.rightMargin;
                int measuredWidth = child.getMeasuredWidth() + margin;
                maxWidth = Math.Max(maxWidth, measuredWidth);
                childState = combineMeasuredStates(childState, child.getMeasuredState());

                allFillParent = allFillParent && lp.width == View.LayoutParams.MATCH_PARENT;
                alternativeMaxWidth = Math.Max(alternativeMaxWidth,
                        matchWidthLocally ? margin : measuredWidth);

                i += getChildrenSkipCount(child, i);
            }

            if (nonSkippedChildCount > 0 && hasDividerBeforeChildAt(count))
            {
                mTotalLength += mDividerHeight;
            }

            // Add in our padding
            mTotalLength += mPaddingTop + mPaddingBottom;

            int heightSize = mTotalLength;

            // Check against our minimum height
            heightSize = Math.Max(heightSize, getSuggestedMinimumHeight());

            // Reconcile our calculated size with the heightMeasureSpec
            int heightSizeAndState = resolveSizeAndState(heightSize, heightMeasureSpec, 0);
            heightSize = heightSizeAndState & MEASURED_SIZE_MASK;
            // Either expand children with weight to take up available space or
            // shrink them if they extend beyond our current bounds. If we skipped
            // measurement on any children, we need to measure them now.

            if (!allFillParent && widthMode != MeasureSpec.EXACTLY)
            {
                maxWidth = alternativeMaxWidth;
            }

            maxWidth += mPaddingLeft + mPaddingRight;

            // Check against our minimum width
            maxWidth = Math.Max(maxWidth, getSuggestedMinimumWidth());

            setMeasuredDimension(resolveSizeAndState(maxWidth, widthMeasureSpec, childState),
                    heightSizeAndState);

            if (matchWidth)
            {
                forceUniformWidth(count, heightMeasureSpec);
            }
        }

        private void forceUniformWidth(int count, int heightMeasureSpec)
        {
            // Pretend that the linear layout has an exact size.
            int uniformMeasureSpec = MeasureSpec.makeMeasureSpec(getMeasuredWidth(),
                    MeasureSpec.EXACTLY);
            for (int i = 0; i < count; ++i)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

                    if (lp.width == View.LayoutParams.MATCH_PARENT)
                    {
                        // Temporarily force children to reuse their old measured height
                        // FIXME: this may not be right for something like wrapping text?
                        int oldHeight = lp.height;
                        lp.height = child.getMeasuredHeight();

                        // Remeasue with new dimensions
                        measureChildWithMargins(child, uniformMeasureSpec, 0, heightMeasureSpec, 0);
                        lp.height = oldHeight;
                    }
                }
            }
        }

        /**
         * Measures the children when the orientation of this ListView is set
         * to {@link #HORIZONTAL}.
         *
         * @param widthMeasureSpec Horizontal space requirements as imposed by the parent.
         * @param heightMeasureSpec Vertical space requirements as imposed by the parent.
         *
         * @see #getOrientation()
         * @see #setOrientation(int)
         * @see #onMeasure(int, int)
         */
        void measureHorizontal(int widthMeasureSpec, int heightMeasureSpec)
        {
            mTotalLength = 0;
            int maxHeight = 0;
            int childState = 0;
            int alternativeMaxHeight = 0;
            int weightedMaxHeight = 0;
            bool allFillParent = true;
            float totalWeight = 0;

            int count = getVirtualChildCount();

            int widthMode = MeasureSpec.getMode(widthMeasureSpec);
            int heightMode = MeasureSpec.getMode(heightMeasureSpec);

            bool matchHeight = false;

            if (mMaxAscent == null || mMaxDescent == null)
            {
                mMaxAscent = new int[VERTICAL_GRAVITY_COUNT];
                mMaxDescent = new int[VERTICAL_GRAVITY_COUNT];
            }

            int[] maxAscent = mMaxAscent;
            int[] maxDescent = mMaxDescent;

            maxAscent[0] = maxAscent[1] = maxAscent[2] = maxAscent[3] = -1;
            maxDescent[0] = maxDescent[1] = maxDescent[2] = maxDescent[3] = -1;

            bool baselineAligned = mBaselineAligned;
            bool useLargestChild = mUseLargestChild;

            bool isExactly = widthMode == MeasureSpec.EXACTLY;
            int usedExcessSpace = 0;

            int nonSkippedChildCount = 0;

            // See how wide everyone is. Also remember max height.
            for (int i = 0; i < count; ++i)
            {
                View child = getVirtualChildAt(i);
                if (child == null)
                {
                    mTotalLength += measureNullChild(i);
                    continue;
                }

                if (child.getVisibility() == GONE)
                {
                    i += getChildrenSkipCount(child, i);
                    continue;
                }

                nonSkippedChildCount++;
                if (hasDividerBeforeChildAt(i))
                {
                    mTotalLength += mDividerWidth;
                }

                MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

                // Determine how big this child would like to be. If this or
                // previous children have given a weight, then we allow it to
                // use all available space (and we will shrink things later
                // if needed).
                int usedWidth = totalWeight == 0 ? mTotalLength : 0;
                measureChildBeforeLayout(child, i, widthMeasureSpec, usedWidth,
                        heightMeasureSpec, 0);

                int childWidth = child.getMeasuredWidth();
                if (isExactly)
                {
                    mTotalLength += childWidth + lp.leftMargin + lp.rightMargin
                            + getNextLocationOffset(child);
                }
                else
                {
                    int totalLength = mTotalLength;
                    mTotalLength = Math.Max(totalLength, totalLength + childWidth + lp.leftMargin
                            + lp.rightMargin + getNextLocationOffset(child));
                }

                bool matchHeightLocally = false;
                if (heightMode != MeasureSpec.EXACTLY && lp.height == View.LayoutParams.MATCH_PARENT)
                {
                    // The height of the linear layout will scale, and at least one
                    // child said it wanted to match our height. Set a flag indicating that
                    // we need to remeasure at least that view when we know our height.
                    matchHeight = true;
                    matchHeightLocally = true;
                }

                int margin = lp.topMargin + lp.bottomMargin;
                int childHeight = child.getMeasuredHeight() + margin;
                childState = combineMeasuredStates(childState, child.getMeasuredState());

                maxHeight = Math.Max(maxHeight, childHeight);

                allFillParent = allFillParent && lp.height == View.LayoutParams.MATCH_PARENT;
                alternativeMaxHeight = Math.Max(alternativeMaxHeight,
                        matchHeightLocally ? margin : childHeight);

                i += getChildrenSkipCount(child, i);
            }

            if (nonSkippedChildCount > 0 && hasDividerBeforeChildAt(count))
            {
                mTotalLength += mDividerWidth;
            }

            // Check mMaxAscent[INDEX_TOP] first because it maps to Gravity.TOP,
            // the most common case
            if (maxAscent[INDEX_TOP] != -1 ||
                    maxAscent[INDEX_CENTER_VERTICAL] != -1 ||
                    maxAscent[INDEX_BOTTOM] != -1 ||
                    maxAscent[INDEX_FILL] != -1)
            {
                int ascent = Math.Max(maxAscent[INDEX_FILL],
                        Math.Max(maxAscent[INDEX_CENTER_VERTICAL],
                        Math.Max(maxAscent[INDEX_TOP], maxAscent[INDEX_BOTTOM])));
                int descent = Math.Max(maxDescent[INDEX_FILL],
                        Math.Max(maxDescent[INDEX_CENTER_VERTICAL],
                        Math.Max(maxDescent[INDEX_TOP], maxDescent[INDEX_BOTTOM])));
                maxHeight = Math.Max(maxHeight, ascent + descent);
            }

            // Add in our padding
            mTotalLength += mPaddingLeft + mPaddingRight;

            int widthSize = mTotalLength;

            // Check against our minimum width
            widthSize = Math.Max(widthSize, getSuggestedMinimumWidth());

            // Reconcile our calculated size with the widthMeasureSpec
            int widthSizeAndState = resolveSizeAndState(widthSize, widthMeasureSpec, 0);
            widthSize = widthSizeAndState & MEASURED_SIZE_MASK;

            if (!allFillParent && heightMode != MeasureSpec.EXACTLY)
            {
                maxHeight = alternativeMaxHeight;
            }

            maxHeight += mPaddingTop + mPaddingBottom;

            // Check against our minimum height
            maxHeight = Math.Max(maxHeight, getSuggestedMinimumHeight());

            setMeasuredDimension((int)((uint)widthSizeAndState | childState & MEASURED_STATE_MASK),
                    resolveSizeAndState(maxHeight, heightMeasureSpec,
                            childState << MEASURED_HEIGHT_STATE_SHIFT));

            if (matchHeight)
            {
                forceUniformHeight(count, widthMeasureSpec);
            }
        }

        private void forceUniformHeight(int count, int widthMeasureSpec)
        {
            // Pretend that the linear layout has an exact size. This is the measured height of
            // ourselves. The measured height should be the max height of the children, changed
            // to accommodate the heightMeasureSpec from the parent
            int uniformMeasureSpec = MeasureSpec.makeMeasureSpec(getMeasuredHeight(),
                    MeasureSpec.EXACTLY);
            for (int i = 0; i < count; ++i)
            {
                View child = getVirtualChildAt(i);
                if (child != null && child.getVisibility() != GONE)
                {
                    MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

                    if (lp.height == View.LayoutParams.MATCH_PARENT)
                    {
                        // Temporarily force children to reuse their old measured width
                        // FIXME: this may not be right for something like wrapping text?
                        int oldWidth = lp.width;
                        lp.width = child.getMeasuredWidth();

                        // Remeasure with new dimensions
                        measureChildWithMargins(child, widthMeasureSpec, 0, uniformMeasureSpec, 0);
                        lp.width = oldWidth;
                    }
                }
            }
        }

        /**
         * <p>Returns the number of children to skip after measuring/laying out
         * the specified child.</p>
         *
         * @param child the child after which we want to skip children
         * @param index the index of the child after which we want to skip children
         * @return the number of children to skip, 0 by default
         */
        int getChildrenSkipCount(View child, int index)
        {
            return 0;
        }

        /**
         * <p>Returns the size (width or height) that should be occupied by a null
         * child.</p>
         *
         * @param childIndex the index of the null child
         * @return the width or height of the child depending on the orientation
         */
        int measureNullChild(int childIndex)
        {
            return 0;
        }

        /**
         * <p>Measure the child according to the parent's measure specs. This
         * method should be overridden by subclasses to force the sizing of
         * children. This method is called by {@link #measureVertical(int, int)} and
         * {@link #measureHorizontal(int, int)}.</p>
         *
         * @param child the child to measure
         * @param childIndex the index of the child in this view
         * @param widthMeasureSpec horizontal space requirements as imposed by the parent
         * @param totalWidth extra space that has been used up by the parent horizontally
         * @param heightMeasureSpec vertical space requirements as imposed by the parent
         * @param totalHeight extra space that has been used up by the parent vertically
         */
        protected virtual void measureChildBeforeLayout(View child, int childIndex,
                int widthMeasureSpec, int totalWidth, int heightMeasureSpec,
                int totalHeight)
        {
            measureChildWithMargins(child, widthMeasureSpec, totalWidth,
                    heightMeasureSpec, totalHeight);
        }

        protected override void measureChildWithMargins(View child, int parentWidthMeasureSpec, int widthUsed,
                                               int parentHeightMeasureSpec, int heightUsed)
        {
            MarginLayoutParams childLayoutParams = (MarginLayoutParams)child.getLayoutParams();

            int widthPadding = mPaddingLeft + mPaddingRight + childLayoutParams.leftMargin + childLayoutParams.rightMargin;
            int heightPadding = mPaddingTop + mPaddingBottom + childLayoutParams.topMargin + childLayoutParams.bottomMargin;

            int usedTotalW = widthPadding + widthUsed;
            int usedTotalH = heightPadding + heightUsed;

            int width = MeasureSpec.getSize(parentWidthMeasureSpec);
            int height = MeasureSpec.getSize(parentHeightMeasureSpec);

            int this_spec_w;
            int this_spec_h;
            if (mOrientation == OrientationMode.VERTICAL)
            {
                switch (childLayoutParams.width)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size, but it cant be bigger than us
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.AT_MOST);
                        break;
                    default:
                        // child wants to be exact size, but it cant be bigger than us
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.AT_MOST);
                        break;
                }
                switch (childLayoutParams.height)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.UNSPECIFIED);
                        break;
                    default:
                        // child wants to be exact size
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY);
                        break;
                }
                this_spec_w = getChildMeasureSpec(this, child, true, this_spec_w, widthPadding, childLayoutParams.width);
                this_spec_h = getChildMeasureSpec(this, child, false, this_spec_h, heightPadding, childLayoutParams.height);
            }
            else
            {
                switch (childLayoutParams.width)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.UNSPECIFIED);
                        break;
                    default:
                        // child wants to be exact size
                        this_spec_w = MeasureSpec.makeMeasureSpec(width, MeasureSpec.EXACTLY);
                        break;
                }
                switch (childLayoutParams.height)
                {
                    case View.LayoutParams.MATCH_PARENT:
                        // child wants to be our size
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY);
                        break;
                    case View.LayoutParams.WRAP_CONTENT:
                        // child wants to be its own size, but it cant be bigger than us
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.AT_MOST);
                        break;
                    default:
                        // child wants to be exact size, but it cant be bigger than us
                        this_spec_h = MeasureSpec.makeMeasureSpec(height, MeasureSpec.AT_MOST);
                        break;
                }
                this_spec_w = getChildMeasureSpec(this, child, true, this_spec_w, widthPadding, childLayoutParams.width);
                this_spec_h = getChildMeasureSpec(this, child, false, this_spec_h, heightPadding, childLayoutParams.height);
            }
            child.measure(this_spec_w, this_spec_h);
        }

        /**
         * <p>Return the location offset of the specified child. This can be used
         * by subclasses to change the location of a given widget.</p>
         *
         * @param child the child for which to obtain the location offset
         * @return the location offset in pixels
         */
        protected virtual int getLocationOffset(View child)
        {
            return 0;
        }

        /**
         * <p>Return the size offset of the next sibling of the specified child.
         * This can be used by subclasses to change the location of the widget
         * following <code>child</code>.</p>
         *
         * @param child the child whose next sibling will be moved
         * @return the location offset of the next child in pixels
         */
        int getNextLocationOffset(View child)
        {
            return 0;
        }

        protected override void onLayout(bool changed, int l, int t, int r, int b)
        {
            if (mOrientation == OrientationMode.VERTICAL)
            {
                layoutVertical(l, t, r, b);
            }
            else
            {
                layoutHorizontal(l, t, r, b);
            }
        }

        /**
         * Position the children during a layout pass if the orientation of this
         * ListView is set to {@link #VERTICAL}.
         *
         * @see #getOrientation()
         * @see #setOrientation(int)
         * @see #onLayout(bool, int, int, int, int)
         * @param left
         * @param top
         * @param right
         * @param bottom
         */
        void layoutVertical(int left, int top, int right, int bottom)
        {
            int paddingLeft = mPaddingLeft;

            int childTop;
            int childLeft;

            // Where right end of child should go
            int width = right - left;
            int childRight = width - mPaddingRight;

            // Space available for child
            int childSpace = width - paddingLeft - mPaddingRight;

            int count = getVirtualChildCount();

            int majorGravity = mGravity & Gravity.VERTICAL_GRAVITY_MASK;
            int minorGravity = mGravity & Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK;

            switch (majorGravity)
            {
                case Gravity.BOTTOM:
                    // mTotalLength contains the padding already
                    childTop = mPaddingTop + bottom - top - mTotalLength;
                    break;

                // mTotalLength contains the padding already
                case Gravity.CENTER_VERTICAL:
                    childTop = mPaddingTop + (bottom - top - mTotalLength) / 2;
                    break;

                case Gravity.TOP:
                default:
                    childTop = mPaddingTop;
                    break;
            }

            for (int i = 0; i < count; i++)
            {
                View child = getVirtualChildAt(i);
                if (child == null)
                {
                    childTop += measureNullChild(i);
                }
                else if (child.getVisibility() != GONE)
                {
                    int childWidth = child.getMeasuredWidth();
                    int childHeight = child.getMeasuredHeight();

                    MarginLayoutParams lp =
                            (MarginLayoutParams)child.getLayoutParams();

                    int gravity = minorGravity;
                    int layoutDirection = getLayoutDirection();
                    int absoluteGravity = Gravity.getAbsoluteGravity(gravity, layoutDirection);
                    switch (absoluteGravity & Gravity.HORIZONTAL_GRAVITY_MASK)
                    {
                        case Gravity.CENTER_HORIZONTAL:
                            childLeft = paddingLeft + (childSpace - childWidth) / 2
                                    + lp.leftMargin - lp.rightMargin;
                            break;

                        case Gravity.RIGHT:
                            childLeft = childRight - childWidth - lp.rightMargin;
                            break;

                        case Gravity.LEFT:
                        default:
                            childLeft = paddingLeft + lp.leftMargin;
                            break;
                    }

                    if (hasDividerBeforeChildAt(i))
                    {
                        childTop += mDividerHeight;
                    }

                    childTop += lp.topMargin;
                    setChildFrame(child, childLeft, childTop + getLocationOffset(child),
                            childWidth, childHeight);
                    childTop += childHeight + lp.bottomMargin + getNextLocationOffset(child);

                    i += getChildrenSkipCount(child, i);
                }
            }
        }

        public override void onRtlPropertiesChanged(int layoutDirection)
        {
            base.onRtlPropertiesChanged(layoutDirection);
            if (layoutDirection != mLayoutDirection)
            {
                mLayoutDirection = layoutDirection;
                if (mOrientation == OrientationMode.HORIZONTAL)
                {
                    requestLayout();
                }
            }
        }

        /**
         * Position the children during a layout pass if the orientation of this
         * ListView is set to {@link #HORIZONTAL}.
         *
         * @see #getOrientation()
         * @see #setOrientation(int)
         * @see #onLayout(bool, int, int, int, int)
         * @param left
         * @param top
         * @param right
         * @param bottom
         */
        void layoutHorizontal(int left, int top, int right, int bottom)
        {
            bool isLayoutRtl_ = isLayoutRtl();
            int paddingTop = mPaddingTop;

            int childTop;
            int childLeft;

            // Where bottom of child should go
            int height = bottom - top;
            int childBottom = height - mPaddingBottom;

            // Space available for child
            int childSpace = height - paddingTop - mPaddingBottom;

            int count = getVirtualChildCount();

            int majorGravity = mGravity & Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK;
            int minorGravity = mGravity & Gravity.VERTICAL_GRAVITY_MASK;

            bool baselineAligned = mBaselineAligned;

            int[] maxAscent = mMaxAscent;
            int[] maxDescent = mMaxDescent;

            int layoutDirection = getLayoutDirection();
            switch (Gravity.getAbsoluteGravity(majorGravity, layoutDirection))
            {
                case Gravity.RIGHT:
                    // mTotalLength contains the padding already
                    childLeft = mPaddingLeft + right - left - mTotalLength;
                    break;

                case Gravity.CENTER_HORIZONTAL:
                    // mTotalLength contains the padding already
                    childLeft = mPaddingLeft + (right - left - mTotalLength) / 2;
                    break;

                case Gravity.LEFT:
                default:
                    childLeft = mPaddingLeft;
                    break;
            }

            int start = 0;
            int dir = 1;
            //In case of RTL, start drawing from the last child.
            if (isLayoutRtl_)
            {
                start = count - 1;
                dir = -1;
            }

            for (int i = 0; i < count; i++)
            {
                int childIndex = start + dir * i;
                View child = getVirtualChildAt(childIndex);
                if (child == null)
                {
                    childLeft += measureNullChild(childIndex);
                }
                else if (child.getVisibility() != GONE)
                {
                    int childWidth = child.getMeasuredWidth();
                    int childHeight = child.getMeasuredHeight();
                    int childBaseline = -1;

                    MarginLayoutParams lp =
                            (MarginLayoutParams)child.getLayoutParams();

                    if (baselineAligned && lp.height != View.LayoutParams.MATCH_PARENT)
                    {
                        childBaseline = child.getBaseline();
                    }

                    int gravity = minorGravity;

                    switch (gravity & Gravity.VERTICAL_GRAVITY_MASK)
                    {
                        case Gravity.TOP:
                            childTop = paddingTop + lp.topMargin;
                            if (childBaseline != -1)
                            {
                                childTop += maxAscent[INDEX_TOP] - childBaseline;
                            }
                            break;

                        case Gravity.CENTER_VERTICAL:
                            // Removed support for baseline alignment when layout_gravity or
                            // gravity == center_vertical. See bug #1038483.
                            // Keep the code around if we need to re-enable this feature
                            // if (childBaseline != -1) {
                            //     // Align baselines vertically only if the child is smaller than us
                            //     if (childSpace - childHeight > 0) {
                            //         childTop = paddingTop + (childSpace / 2) - childBaseline;
                            //     } else {
                            //         childTop = paddingTop + (childSpace - childHeight) / 2;
                            //     }
                            // } else {
                            childTop = paddingTop + (childSpace - childHeight) / 2
                                    + lp.topMargin - lp.bottomMargin;
                            break;

                        case Gravity.BOTTOM:
                            childTop = childBottom - childHeight - lp.bottomMargin;
                            if (childBaseline != -1)
                            {
                                int descent = child.getMeasuredHeight() - childBaseline;
                                childTop -= maxDescent[INDEX_BOTTOM] - descent;
                            }
                            break;
                        default:
                            childTop = paddingTop;
                            break;
                    }

                    if (hasDividerBeforeChildAt(childIndex))
                    {
                        childLeft += mDividerWidth;
                    }

                    childLeft += lp.leftMargin;
                    setChildFrame(child, childLeft + getLocationOffset(child), childTop,
                            childWidth, childHeight);
                    childLeft += childWidth + lp.rightMargin +
                            getNextLocationOffset(child);

                    i += getChildrenSkipCount(child, childIndex);
                }
            }
        }

        private void setChildFrame(View child, int left, int top, int width, int height)
        {
            child.layout(left, top, left + width, top + height);
        }

        /**
         * Should the layout be a column or a row.
         * @param orientation Pass {@link #HORIZONTAL} or {@link #VERTICAL}. Default
         * value is {@link #HORIZONTAL}.
         *
         * @attr ref android.R.styleable#ListView_orientation
         */
        public void setOrientation(OrientationMode orientation)
        {
            if (mOrientation != orientation)
            {
                mOrientation = orientation;
                requestLayout();
            }
        }

        /**
         * Returns the current orientation.
         *
         * @return either {@link #HORIZONTAL} or {@link #VERTICAL}
         */
        public OrientationMode getOrientation()
        {
            return mOrientation;
        }

        /**
         * Describes how the child views are positioned. Defaults to GRAVITY_TOP. If
         * this layout has a VERTICAL orientation, this controls where all the child
         * views are placed if there is extra vertical space. If this layout has a
         *  HORIZONTAL orientation, this controls the alignment of the children.
         *
         * @param gravity See {@link android.view.Gravity}
         *
         * @attr ref android.R.styleable#ListView_gravity
         */
        public void setGravity(int gravity)
        {
            if (mGravity != gravity)
            {
                if ((gravity & Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK) == 0)
                {
                    gravity |= Gravity.START;
                }

                if ((gravity & Gravity.VERTICAL_GRAVITY_MASK) == 0)
                {
                    gravity |= Gravity.TOP;
                }

                mGravity = gravity;
                requestLayout();
            }
        }

        /**
         * Returns the current gravity. See {@link android.view.Gravity}
         *
         * @return the current gravity.
         * @see #setGravity
         */
        public int getGravity()
        {
            return mGravity;
        }

        public void setHorizontalGravity(int horizontalGravity)
        {
            int gravity = horizontalGravity & Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK;
            if ((mGravity & Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK) != gravity)
            {
                mGravity = mGravity & ~Gravity.RELATIVE_HORIZONTAL_GRAVITY_MASK | gravity;
                requestLayout();
            }
        }

        public void setVerticalGravity(int verticalGravity)
        {
            int gravity = verticalGravity & Gravity.VERTICAL_GRAVITY_MASK;
            if ((mGravity & Gravity.VERTICAL_GRAVITY_MASK) != gravity)
            {
                mGravity = mGravity & ~Gravity.VERTICAL_GRAVITY_MASK | gravity;
                requestLayout();
            }
        }

        override
        public MarginLayoutParams generateLayoutParams()
        {
            return new MarginLayoutParams();
        }

        /**
         * Returns a set of layout parameters with a width of
         * {@link android.view.ViewGroup.LayoutParams#MATCH_PARENT}
         * and a height of {@link android.view.ViewGroup.LayoutParams#WRAP_CONTENT}
         * when the layout's orientation is {@link #VERTICAL}. When the orientation is
         * {@link #HORIZONTAL}, the width is set to {@link LayoutParams#WRAP_CONTENT}
         * and the height to {@link LayoutParams#WRAP_CONTENT}.
         */
        protected override View.MarginLayoutParams generateDefaultLayoutParams()
        {
            if (mOrientation == OrientationMode.HORIZONTAL)
            {
                return new MarginLayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT);
            }
            else if (mOrientation == OrientationMode.VERTICAL)
            {
                return new MarginLayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT);
            }
            return null;
        }

        protected override View.MarginLayoutParams generateLayoutParams(View.LayoutParams lp)
        {
            if (sPreserveMarginParamsInLayoutParamConversion)
            {
                if (lp is LayoutParams)
                {
                    return new MarginLayoutParams(lp);
                }
                else if (lp is MarginLayoutParams)
                {
                    return (MarginLayoutParams)lp;
                }
            }
            return new MarginLayoutParams(lp);
        }


        // Override to allow type-checking of LayoutParams.
        protected override bool checkLayoutParams(View.LayoutParams p)
        {
            return p is MarginLayoutParams;
        }

        public ScrollViewHostInstance ScrollHostGetInstance()
        {
            return host;
        }

        public void ScrollHostOnSetWillDraw(bool smoothScroll)
        {
            setWillDraw(isShowingDividers() || smoothScroll);
        }

        public void ScrollHostOnCancelled()
        {
        }

        public bool ScrollHostHasChildrenToScroll()
        {
            return mChildrenCount != 0;
        }

        public int ScrollHostGetMeasuredWidth()
        {
            return getMeasuredWidth();
        }

        public int ScrollHostGetMeasuredHeight()
        {
            return getMeasuredHeight();
        }

        public int ScrollHostGetChildTotalMeasuredWidth()
        {
            if (mOrientation == OrientationMode.VERTICAL)
            {
                return getMeasuredWidth();
            }
            else
            {
                return mTotalLength;
            }
        }

        public int ScrollHostGetChildTotalMeasuredHeight()
        {
            if (mOrientation == OrientationMode.HORIZONTAL)
            {
                return getMeasuredHeight();
            }
            else
            {
                return mTotalLength;
            }
        }
    }
}
