using AndroidUI.Widgets;

namespace AndroidUI.Utils.Widgets
{
    public interface ScrollHost
    {
        public ScrollViewHostInstance ScrollHostGetInstance();
        public void ScrollHostOnSetWillDraw(bool smoothScroll);
        public void ScrollHostOnCancelled();
        public bool ScrollHostHasChildrenToScroll();
        public int ScrollHostGetMeasuredWidth();
        public int ScrollHostGetMeasuredHeight();
        bool ScrollHostCanScrollLeftOrUp();
        int ScrollHostGetChildLeft();
        int ScrollHostGetChildTop();
        bool ScrollHostCanScrollRightOrDown();
        int ScrollHostGetChildRight();
        int ScrollHostGetChildBottom();
        void ScrollHostTryScrollTo(View target_view, int x, int y);
        Applications.Context ScrollHostGetContext();
    }
}