using AndroidUI.Extensions;
using AndroidUI.Utils.Input;
using SkiaSharp;

namespace AndroidUI.Widgets
{
    public class FlywheelScrollView : FrameLayout
    {
        private const int THRESH_HOLD = 90; // 90 ms

        Topten_RichTextKit_TextView text = new();
        Flywheel flywheel = new();

        bool first_draw = true;
        bool text_set;
        long last_time_previous, last_time_current, time;
        bool was_down;

        public FlywheelScrollView() : base()
        {
            addView(text, MATCH_PARENT__MATCH_PARENT);
            text.setZ(float.PositiveInfinity);
            text.setText("FlywheelScrollView");
            setWillDraw(true);
        }

        public override bool onInterceptTouchEvent(Touch ev)
        {
            return true;
        }

        protected override void dispatchDraw(SKCanvas canvas)
        {
            base.dispatchDraw(canvas);
            if (first_draw)
            {
                first_draw = false;
                text.invalidate();
            }
            else
            {
                flywheel.AquireLock();
                if (text_set && flywheel.Spinning)
                {
                    text_set = false;
                    text.invalidate();
                }
                flywheel.ReleaseLock();
            }
        }

        protected override void onDraw(SKCanvas canvas)
        {
            base.onDraw(canvas);
            flywheel.AquireLock();
            flywheel.Spin();
            if (flywheel.Spinning)
            {
                invalidate();

                if (getChildCount() == 2)
                {
                    View view = getChildAt(1);
                    view.scrollTo(-flywheel.Position.X.toPixel(), -flywheel.Position.Y.toPixel());
                }
            }
            string s = "Flywheel\n";
            s += "  Spinning: " + flywheel.Spinning + "\n";
            s += "  Friction: " + flywheel.Friction + "\n";
            s += "  Distance: \n";
            s += "    x: " + flywheel.Distance.X + " pixels\n";
            s += "    y: " + flywheel.Distance.Y + " pixels\n";
            s += "    time: " + time + " ms\n";
            s += "  Total Distance: \n";
            s += "    x: " + flywheel.TotalDistance.X + " pixels\n";
            s += "    y: " + flywheel.TotalDistance.Y + " pixels\n";
            s += "    time: " + flywheel.SpinTime + " ms\n";
            s += "  Velocity: \n";
            s += "    x: " + flywheel.Velocity.X + "\n";
            s += "    y: " + flywheel.Velocity.Y + "\n";
            s += "  Position: \n";
            s += "    x: " + flywheel.Position.X + " pixels\n";
            s += "    y: " + flywheel.Position.Y + " pixels\n";
            text.setText(s);
            text_set = true;
            flywheel.ReleaseLock();
        }

        public override void onConfigureTouch(Touch touch)
        {
            touch.DontBatchOnTouchUpOrTouchCancel = true;
        }

        public override bool onTouch(Touch touch)
        {
            touch.DontBatchOnTouchUpOrTouchCancel = true;
            Touch.Data data = touch.getTouchAtCurrentIndex();
            var st = data.state;
            flywheel.AquireLock();
            switch (st)
            {
                case Touch.State.TOUCH_CANCELLED:
                case Touch.State.TOUCH_DOWN:
                    last_time_previous = 0;
                    last_time_current = data.timestamp;
                    time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                    Log.d("DOWN time: " + time);
                    flywheel.ResetButKeepPosition();
                    flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                    was_down = true;
                    break;
                case Touch.State.TOUCH_MOVE:
                    // protect against batching, touch up emits a touch move if it's location has changed
                    last_time_previous = last_time_current;
                    last_time_current = data.timestamp;
                    time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                    Log.d("MOVE time: " + time);
                    if (was_down || time <= THRESH_HOLD)
                    {
                        was_down = false;
                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                        if (getChildCount() == 2)
                        {
                            View view = getChildAt(1);
                            view.scrollTo(-flywheel.Position.X.toPixel(), -flywheel.Position.Y.toPixel());
                        }
                    }
                    break;
                case Touch.State.TOUCH_UP:
                    last_time_previous = last_time_current;
                    last_time_current = data.timestamp;
                    time = last_time_previous == 0 ? 0 : (last_time_current - last_time_previous);
                    Log.d("UP time: " + time);
                    if (!was_down && time <= THRESH_HOLD)
                    {
                        flywheel.FinalizeMovement();
                    }
                    else
                    {
                        was_down = false;
                    }
                    break;
                default:
                    break;
            }
            flywheel.ReleaseLock();
            text.invalidate();
            invalidate();
            return true;
        }

        public override void onBeforeAddView()
        {
            if (getChildCount() == 2)
            {
                throw new Exception("ScrollView can only have a single child");
            }
        }

        public override void removeViewAt(int index)
        {
            base.removeViewAt(index + 1);
        }

        public override void removeViews(int start, int count)
        {
            base.removeViews(start + 1, count);
        }

        public override View getChildAt(int index)
        {
            return base.getChildAt(index + 1);
        }

        public override int getChildCount()
        {
            return base.getChildCount() - 1;
        }

        public override void addView(View child, int index, View.LayoutParams layout_params)
        {
            base.addView(child, index + 1, layout_params);
        }

        public override void removeViewsInLayout(int start, int count)
        {
            base.removeViewsInLayout(start + 1, count);
        }
    }
}
