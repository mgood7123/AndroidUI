/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.Utils.Widgets;
using AndroidUI.Widgets;

namespace AndroidUI.Applications
{
    public partial class TestApp
    {
        class MyAdapterView : AdapterView<ArrayAdapter<string>, string>
        {
            ArrayAdapter<string> mAdapter;

            private int mSelectedPosition;
            private int mFirstPosition;

            public override ArrayAdapter<string> getAdapter()
            {
                Log.d("getAdapter");
                return mAdapter;
            }

            public override View getSelectedView()
            {
                Log.d("getSelectedView");
                if (getCount() > 0 && mSelectedPosition >= 0)
                {
                    return getChildAt(mSelectedPosition - mFirstPosition);
                }
                else
                {
                    return null;
                }
            }

            public override void setAdapter(ArrayAdapter<string> adapter)
            {
                Log.d("setAdapter: " + adapter);
                mAdapter = adapter;
            }

            public override void setSelection(int position)
            {
                Log.d("setSelection: " + position);
            }
        }
    }
}
