using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidUI
{
    public partial class Touch
    {
        internal bool TryPurgeTouch(TouchContainer touchContainer)
        {
            if (
                (touchContainer.used && touchContainer.touch.state == State.TOUCH_UP)
                || (!touchContainer.used && touchContainer.touch.state != State.NONE)
            )
            {
                touchContainer.used = false;
                if (touchContainer.touch.state == State.TOUCH_CANCELLED)
                {
                    //
                    // do not reset touch count
                    // touch count is SUPPOSED to be > 0
                    //
                    // touch count can be > 0 if the index that
                    // has been reused for TOUCH_DOWN is less
                    // than the index of this TOUCH_CANCELLED
                    //
                    // before cancel, touch count 1
                    // 0: TOUCH_DOWN
                    // 1: TOUCH_MOVE
                    //
                    // after cancel, touch count 0
                    // 0: TOUCH_NONE
                    // 1: TOUCH_CANCELLED
                    //
                    // on addTouch, touch count 1
                    // 0: TOUCH_DOWN
                    // 1: TOUCH_CANCELLED
                    //
                    //
                    touchContainer.touch.resetAndKeepIdentity();
                }
                else
                {
                    touchContainer.touch.resetAndKeepIdentity();
                    if (touchCount > 0)
                    {
                        touchCount--;
                    }
                    else
                    {
                        if (throw_on_error)
                        {
                            throw new InvalidOperationException("touch cannot be purged with a touch count of zero");
                        }
                        else
                        {
                            Console.WriteLine("touch cannot be purged with a touch count of zero, cancelling touch");
                            cancelTouch();
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
