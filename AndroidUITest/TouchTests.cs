using AndroidUI;
using AndroidUITestFramework;

namespace AndroidUITest
{
    internal class TouchTests : TestGroup
    {
        Touch touch;

        public override void OnCreate()
        {
            touch = new Touch();
        }

        public override void OnDestroy()
        {
            touch = null;
        }

        private class Core : TouchTests
        {
            class Data : Test
            {
                protected Touch touch;
                public override void Run(TestGroup nullableInstance)
                {
                    TouchTests touchTests = (TouchTests)nullableInstance;
                    touch = touchTests.touch;
                }
            }

            class InitializationTests : TouchTests
            {
                class init_1 : Data
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        Tools.ExpectEqual(touch.index, 0);
                        Tools.ExpectEqual(touch.touchCount, 0);
                        Tools.ExpectEqual(touch.MaxSupportedTouches, 0);
                    }
                }

                class init_2 : Data
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.MaxSupportedTouches = 2;
                        Tools.ExpectEqual(touch.MaxSupportedTouches, 2, "max supported touched has not been updated");
                        Tools.ExpectEqual(touch.touchContainerList.Count, 2, "touch container has not been resized");
                    }
                }
            }

            class Data2 : Data
            {
                public override void Run(TestGroup nullableInstance)
                {
                    base.Run(nullableInstance);
                    touch.MaxSupportedTouches = 2;
                }
            }

            static Touch.TouchContainer expectContainer(Touch touch, string touch_number_string, int touch_index, string touch_index_name, int touch_count, string touch_count_string, int current_touch_index, string current_touch_index_name)
            {
                Tools.ExpectEqual(touch.touchCount, touch_count, touch_number_string + " touch must have a touch count of " + touch_count_string);
                Tools.ExpectEqual(touch.index, current_touch_index, "the current index of the " + touch_number_string + " touch must have an index of " + current_touch_index_name);
                Touch.TouchContainer touchData = touch.touchContainerList.ElementAt(touch_index);
                Tools.ExpectInstanceEqual(touchData.touch, touch.getTouchAt(touch_index), touch_number_string + " touch must have an index of " + touch_index_name);
                Touch.TouchContainer currentTouchData = current_touch_index == touch_index ? touchData : touch.touchContainerList.ElementAt(current_touch_index);
                Tools.ExpectInstanceEqual(currentTouchData.touch, touch.getTouchAtCurrentIndex(), touch_number_string + " touch must match touch at current index");
                return touchData;
            }

            static void expectContainerUsed(Touch touch, string touch_number_string, int touch_index, string touch_index_name, int touch_count, string touch_count_string, int current_touch_index, string current_touch_index_name)
            {
                Touch.TouchContainer touchData = expectContainer(touch, touch_number_string, touch_index, touch_index_name, touch_count, touch_count_string, current_touch_index, current_touch_index_name);
                Tools.ExpectTrue(touchData.used, "touch container at index " + touch_index + " must be used");
            }

            static void expectContainerNotUsed(Touch touch, string touch_number_string, int touch_index, string touch_index_name, int touch_count, string touch_count_string, int current_touch_index, string current_touch_index_name)
            {
                Touch.TouchContainer touchData = expectContainer(touch, touch_number_string, touch_index, touch_index_name, touch_count, touch_count_string, current_touch_index, current_touch_index_name);
                Tools.ExpectFalse(touchData.used, "touch container at index " + touch_index + " must not be used");
            }

            static void expectTouchData(Touch.Data touchData, string touch_number_name, object identity, float x, float y, float nx, float ny, float p, float s, Touch.State state)
            {
                Tools.ExpectEqual(touchData.identity, identity, "incorrect identity for " + touch_number_name + "touch");
                Tools.ExpectTrue(touchData.hasLocation, touch_number_name + " location");

                Tools.ExpectEqual(touchData.location.x, x, touch_number_name + " x");
                Tools.ExpectEqual(touchData.location.y, y, touch_number_name + " y");

                Tools.ExpectEqual(touchData.normalized_location_on_input_surface.x, nx, touch_number_name + " norm x");
                Tools.ExpectEqual(touchData.normalized_location_on_input_surface.y, ny, touch_number_name + " norm y");

                Tools.ExpectEqual(touchData.pressure, p, touch_number_name + " pressure");
                Tools.ExpectEqual(touchData.size, s, touch_number_name + " size");

                Tools.ExpectEqual(touchData.state, state, "incorrect " + touch_number_name + " touch state");
            }

            static void expectTouchDataDidMove(Touch.Data touchData, string touch_number_name, object identity, float x, float y, float nx, float ny, float p, float s, Touch.State state)
            {
                expectTouchData(touchData, touch_number_name, identity, x, y, nx, ny, p, s, state);
                Tools.ExpectTrue(touchData.location_moved, touch_number_name + " location is meant to have moved on " + Touch.StateToString(state));
                Tools.ExpectTrue(touchData.normalized_location_on_input_surface_moved, "norm location is meant to have moved on touch add");
                Tools.ExpectTrue(touchData.location_moved_or_normalized_location_on_input_surface_moved, "location and norm location is meant to have moved on touch add");
            }

            static void expectTouchDataDidNotMove(Touch.Data touchData, string touch_number_name, object identity, float x, float y, float nx, float ny, float p, float s, Touch.State state)
            {
                expectTouchData(touchData, touch_number_name, identity, x, y, nx, ny, p, s, state);
                Tools.ExpectFalse(touchData.location_moved, touch_number_name + " location is not meant to have moved on " + Touch.StateToString(state));
                Tools.ExpectFalse(touchData.normalized_location_on_input_surface_moved, "norm location is not meant to have moved on " + Touch.StateToString(state));
                Tools.ExpectFalse(touchData.location_moved_or_normalized_location_on_input_surface_moved, "location and norm location is not meant to have moved on " + Touch.StateToString(state));
            }

            class OperationTests : TouchTests
            {
                class _0_add__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        expectContainerUsed(touch, "first", 0, "zero", 1, "one", 0, "zero");
                    }
                }

                class _1_add__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        Touch.Data touchData = touch.getTouchAtCurrentIndex();
                        expectTouchDataDidNotMove(touchData, "first", 0, 5, 5, 0, 0, 1, 1, Touch.State.TOUCH_DOWN);
                    }
                }

                class _2_add__touchData_Clone : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        Touch.Data touchData = (Touch.Data)touch.getTouchAtCurrentIndex().Clone();
                        expectTouchDataDidNotMove(touchData, "cloned first", 0, 5, 5, 0, 0, 1, 1, Touch.State.TOUCH_DOWN);
                    }
                }

                class _3_move__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.moveTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "moved first", 0, "zero", 1, "one", 0, "zero");
                    }
                }

                class _4_move__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.moveTouch(0, 6, 6, 1, 1);
                        var touchData = touch.getTouchAtCurrentIndex();
                        expectTouchDataDidMove(touchData, "moved first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_MOVE);
                    }
                }

                class _5_remove__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "removed first", 0, "zero", 1, "one", 0, "one");
                    }
                }

                class _6_remove__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        var touchData = touch.getTouchAtCurrentIndex();
                        expectTouchDataDidMove(touchData, "removed first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_UP);
                    }
                }

                class _7_add_remove_purge__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        var touchData = touch.touchContainerList.ElementAt(0);
                        touch.TryPurgeTouch(touchData);
                        expectContainerNotUsed(touch, "purged first", 0, "zero", 0, "zero", 0, "zero");
                    }
                }
            }

            class OperationTests_2 : TouchTests
            {
                class _0_add__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        expectContainerUsed(touch, "first", 0, "zero", 1, "one", 0, "zero");
                        touch.addTouch(1, 3, 3, 3, 3);
                        expectContainerUsed(touch, "first", 0, "zero", 2, "two", 1, "one");
                        expectContainerUsed(touch, "second", 1, "one", 2, "two", 1, "one");
                    }
                }

                class _1_add__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.addTouch(1, 3, 3, 3, 3);
                        Touch.Data touchData = touch.getTouchAt(0);
                        expectTouchDataDidNotMove(touchData, "first", 0, 5, 5, 0, 0, 1, 1, Touch.State.TOUCH_DOWN);
                        Touch.Data touchData2 = touch.getTouchAt(1);
                        expectTouchDataDidNotMove(touchData2, "second", 1, 3, 3, 3, 3, 1, 1, Touch.State.TOUCH_DOWN);
                    }
                }

                class _2_add__touchData_Clone : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.addTouch(1, 3, 3, 3, 3);
                        Touch.Data touchData = (Touch.Data)touch.getTouchAt(0).Clone();
                        expectTouchDataDidNotMove(touchData, "Cloned first", 0, 5, 5, 0, 0, 1, 1, Touch.State.TOUCH_DOWN);
                        Touch.Data touchData2 = (Touch.Data)touch.getTouchAt(1).Clone();
                        expectTouchDataDidNotMove(touchData2, "Cloned second", 1, 3, 3, 3, 3, 1, 1, Touch.State.TOUCH_DOWN);
                    }
                }

                class _3_move__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.moveTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "moved first", 0, "zero", 1, "one", 0, "zero");
                        touch.addTouch(1, 3, 3, 3, 3);
                        expectContainerUsed(touch, "added first", 1, "one", 2, "two", 1, "one");
                        touch.moveTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "moved first after added second", 0, "zero", 2, "two", 0, "zero");
                        touch.moveTouch(1, 68, 68, 81, 81);
                        expectContainerUsed(touch, "moved first", 1, "one", 2, "two", 1, "one");
                    }
                }

                class _4_move__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.moveTouch(0, 6, 6, 1, 1);
                        expectTouchDataDidMove(touch.getTouchAtCurrentIndex(), "moved first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_MOVE);
                        touch.addTouch(1, 3, 3, 3, 3);
                        expectTouchDataDidMove(touch.getTouchAt(0), "moved first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_MOVE);
                        expectTouchDataDidNotMove(touch.getTouchAt(1), "added second", 1, 3, 3, 3, 3, 1, 1, Touch.State.TOUCH_DOWN);
                        touch.moveTouch(1, 68, 68, 81, 81);
                        expectTouchDataDidMove(touch.getTouchAt(0), "moved first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_MOVE);
                        expectTouchDataDidMove(touch.getTouchAt(1), "moved second", 1, 68, 68, 81, 81, 1, 1, Touch.State.TOUCH_DOWN);
                    }
                }

                class _5_remove__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "removed first", 0, "zero", 1, "one", 0, "zero");

                        touch.addTouch(0, 5, 5, 0, 0);
                        expectContainerUsed(touch, "added first", 0, "zero", 1, "one", 0, "zero");
                        touch.removeTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "removed first", 0, "zero", 1, "one", 0, "zero");

                        touch.addTouch(0, 5, 5, 0, 0);
                        expectContainerUsed(touch, "added first", 0, "zero", 1, "one", 0, "zero");

                        touch.addTouch(1, 51, 51, 10, 10);
                        expectContainerUsed(touch, "added second", 1, "one", 2, "two", 1, "one");
                        touch.removeTouch(0, 6, 6, 1, 1);
                        expectContainerUsed(touch, "removed first", 0, "zero", 2, "two", 0, "zero");

                        expectContainerUsed(touch, "added second", 0, "zero", 2, "two", 0, "zero");

                        touch.removeTouch(1, 6, 6, 1, 1);
                        expectContainerUsed(touch, "removed second", 1, "one", 1, "one", 1, "one");
                    }
                }

                class _6_remove__touchData : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.addTouch(1, 15, 15, 10, 10);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        var touchData = touch.getTouchAt(0);
                        expectTouchDataDidMove(touchData, "removed first", 0, 6, 6, 1, 1, 1, 1, Touch.State.TOUCH_UP);
                    }
                }

                class _7_add_remove_purge__touch_container : Data2
                {
                    public override void Run(TestGroup nullableInstance)
                    {
                        base.Run(nullableInstance);
                        touch.addTouch(0, 5, 5, 0, 0);
                        touch.addTouch(1, 15, 15, 10, 10);
                        touch.removeTouch(0, 6, 6, 1, 1);
                        var touchData = touch.touchContainerList.ElementAt(0);
                        touch.TryPurgeTouch(touchData);
                        expectContainerNotUsed(touch, "purged first", 0, "zero", 1, "one", 0, "zero");
                    }
                }
            }
        }
    }
}
