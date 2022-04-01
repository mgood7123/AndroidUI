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
        void OnScreenDensityChanged();
    }
}