namespace AndroidUI
{
    // this does nothing, only here to link classes together
    public interface ViewParent
    {
        public void clearChildFocus(View child); // no op

        /**
         * Returns the parent if it exists, or null.
         *
         * @return a ViewParent or null if this ViewParent does not have a parent
         */
        public abstract ViewParent getParent();

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
        ViewParent invalidateChildInParent(int[] location, Rect r);

        void OnScreenDensityChanged();

        /**
         * This method is called on the parent when a child's drawable state
         * has changed.
         *
         * @param child The child whose drawable state has changed.
         */
        public void childDrawableStateChanged(View child); // no op

        /**
         * Called when a child view now has or no longer is tracking transient state.
         *
         * <p>"Transient state" is any state that a View might hold that is not expected to
         * be reflected in the data model that the View currently presents. This state only
         * affects the presentation to the user within the View itself, such as the current
         * state of animations in progress or the state of a text selection operation.</p>
         *
         * <p>Transient state is useful for hinting to other components of the View system
         * that a particular view is tracking something complex but encapsulated.
         * A <code>ListView</code> for example may acknowledge that list item Views
         * with transient state should be preserved within their position or stable item ID
         * instead of treating that view as trivially replaceable by the backing adapter.
         * This allows adapter implementations to be simpler instead of needing to track
         * the state of item view animations in progress such that they could be restored
         * in the event of an unexpected recycling and rebinding of attached item views.</p>
         *
         * <p>This method is called on a parent view when a child view or a view within
         * its subtree begins or ends tracking of internal transient state.</p>
         *
         * @param child Child view whose state has changed
         * @param hasTransientState true if this child has transient state
         */
        public void childHasTransientStateChanged(View child, bool hasTransientState); // no-op
    }
}