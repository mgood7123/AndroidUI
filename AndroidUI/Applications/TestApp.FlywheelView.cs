/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.Graphics;
using AndroidUI.Input;
using AndroidUI.Utils.Input;
using AndroidUI.Widgets;

namespace AndroidUI.Applications
{
    public partial class TestApp
    {
        class FlywheelView : Topten_RichTextKit_TextView
        {
            public FlywheelView() : base()
            {
                setText("FlywheelView");
            }

            Flywheel flywheel = new();

            protected override void onDraw(Canvas canvas)
            {
                flywheel.AquireLock();
                flywheel.Spin();
                string s = "Flywheel\n";
                s += "  Spinning: " + flywheel.Spinning + "\n";
                s += "  Friction: " + flywheel.Friction + "\n";
                s += "  Distance: \n";
                s += "    x: " + flywheel.Distance.First.value + " pixels\n";
                s += "    y: " + flywheel.Distance.Second.value + " pixels\n";
                s += "    time: " + flywheel.TimeSinceLastMovement + " ms\n";
                s += "  Total Distance: \n";
                s += "    x: " + flywheel.TotalDistance.First.value + " pixels\n";
                s += "    y: " + flywheel.TotalDistance.Second.value + " pixels\n";
                s += "    time: " + flywheel.SpinTime + " ms\n";
                s += "  Velocity: \n";
                s += "    x: " + flywheel.Velocity.First.value + "\n";
                s += "    y: " + flywheel.Velocity.Second.value + "\n";
                s += "  Position: \n";
                s += "    x: " + flywheel.Position.First.value + " pixels\n";
                s += "    y: " + flywheel.Position.Second.value + " pixels\n";
                setText(s);
                base.onDraw(canvas);
                if (flywheel.Spinning) invalidate();
                flywheel.ReleaseLock();
            }

            public override bool onTouch(Touch touch)
            {
                Touch.Data data = touch.getTouchAtCurrentIndex();
                var st = data.state;
                flywheel.AquireLock();
                switch (st)
                {
                    case Touch.State.TOUCH_CANCELLED:
                    case Touch.State.TOUCH_DOWN:
                        flywheel.Reset();
                        break;
                    case Touch.State.TOUCH_MOVE:
                        flywheel.AddMovement(data.timestamp, data.location.x, data.location.y);
                        break;
                    case Touch.State.TOUCH_UP:
                        flywheel.FinalizeMovement();
                        break;
                    default:
                        break;
                }
                flywheel.ReleaseLock();
                invalidate();
                return true;
            }
        }
    }
}
