namespace AndroidUI
{
    // this does nothing, only here to link classes together
    public interface Parent
    {
        public void clearChildFocus(View child); // no op

        /**
         * Returns the parent if it exists, or null.
         *
         * @return a ViewParent or null if this ViewParent does not have a parent
         */
        public abstract Parent getParent();

        /**
         * Unbuffered dispatch has been requested by a child of this view parent.
         * This method is called by the View hierarchy to signal ancestors that a View needs to
         * request unbuffered dispatch.
         *
         * @see View#requestUnbufferedDispatch(int)
         * @hide
         */
        void onDescendantUnbufferedRequested(); // parent

        void requestChildFocus(View view1, View view2);
        View focusSearch(View view, int direction);
        void focusableViewAvailable(View view);
        bool isLayoutRequested();
        void requestLayout();
        void invalidate();
        bool canResolveLayoutDirection(); // false

        bool isLayoutDirectionResolved();
        int getLayoutDirection();
        void requestDisallowInterceptTouchEvent(bool disallowIntercept); // no op

        void onDescendantInvalidated(View view, View target);

        void invalidateChild(View view, Rect damage); // no op

        /**
         * All or part of a child is dirty and needs to be redrawn.
         *
         * <p>The location array is an array of two int values which respectively
         * define the left and the top position of the dirty child.</p>
         *
         * <p>This method must return the parent of this ViewParent if the specified
         * rectangle must be invalidated in the parent. If the specified rectangle
         * does not require invalidation in the parent or if the parent does not
         * exist, this method must return null.</p>
         *
         * <p>When this method returns a non-null value, the location array must
         * have been updated with the left and top coordinates of this ViewParent.</p>
         *
         * @param location An array of 2 ints containing the left and top
         *        coordinates of the child to invalidate
         * @param r The area within the child that is invalid
         *
         * @return the parent of this ViewParent or null
         *
         * @deprecated Use {@link #onDescendantInvalidated(View, View)} instead.
         */
        Parent invalidateChildInParent(int[] location, Rect r);

        void OnScreenDensityChanged();

        /**
         * This method is called on the parent when a child's drawable state
         * has changed.
         *
         * @param child The child whose drawable state has changed.
         */
        public void childDrawableStateChanged(View child); // no op
    }
}