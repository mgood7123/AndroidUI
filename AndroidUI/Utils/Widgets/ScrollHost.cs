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
        public int ScrollHostGetChildTotalMeasuredWidth();
        public int ScrollHostGetChildTotalMeasuredHeight();
    }
}