/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.Input;
using AndroidUI.Widgets;

namespace AndroidUI.Applications
{
    public partial class TestApp
    {
        class TouchInfoView : Topten_RichTextKit_TextView
        {
            public TouchInfoView() : base()
            {
                setText("TouchInfoView");
            }

            public override bool onTouch(Touch touch)
            {
                var st = touch.getTouchAtCurrentIndex().state;
                switch (st)
                {
                    case Touch.State.TOUCH_DOWN:
                    case Touch.State.TOUCH_MOVE:
                    case Touch.State.TOUCH_UP:
                        {
                            Log.d(GetType().Name, "onTouch invalidating");
                            string s = "";
                            s += "view x     : " + getX() + "\n";
                            s += "view y     : " + getY() + "\n";
                            s += "view width : " + getWidth() + "\n";
                            s += "view height: " + getHeight() + "\n";
                            s += touch.ToString();
                            Log.d(GetType().Name, "onTouch: " + s);
                            setText(s);
                            invalidate();
                            break;
                        }

                    default:
                        break;
                }
                return true;
            }
        }
    }
}
