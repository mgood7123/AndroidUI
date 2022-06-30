﻿/*
 * Copyright (C) 2015 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace AndroidUI.Execution
{

    /**
     * Class used to enqueue pending work from Views when no Handler is attached.
     *
     * @hide Exposed for test framework only.
     */
    internal class HandlerActionQueue
    {
        private object LOCK = new();
        private HandlerAction[] mActions;
        private int mCount;

        public void post(Runnable action)
        {
            postDelayed(action, 0);
        }

        public void postDelayed(Runnable action, long delayMillis)
        {
            HandlerAction handlerAction = new(action, delayMillis);

            lock (LOCK)
            {
                if (mActions == null)
                {
                    mActions = new HandlerAction[4];
                }
                mActions = GrowingArrayUtils.append(mActions, mCount, handlerAction);
                mCount++;
            }
        }

        public void removeCallbacks(Runnable action)
        {
            lock (LOCK)
            {
                int count = mCount;
                int j = 0;

                HandlerAction[] actions = mActions;
                for (int i = 0; i < count; i++)
                {
                    if (actions[i].matches(action))
                    {
                        // Remove this action by overwriting it within
                        // this loop or nulling it out later.
                        continue;
                    }

                    if (j != i)
                    {
                        // At least one previous entry was removed, so
                        // this one needs to move to the "new" list.
                        actions[j] = actions[i];
                    }

                    j++;
                }

                // The "new" list only has j entries.
                mCount = j;

                // Null out any remaining entries.
                for (; j < count; j++)
                {
                    actions[j] = null;
                }
            }
        }

        public void executeActions(Handler handler)
        {
            lock (LOCK)
            {
                HandlerAction[] actions = mActions;
                for (int i = 0, count = mCount; i < count; i++)
                {
                    HandlerAction handlerAction = actions[i];
                    handler.postDelayed(handlerAction.action, handlerAction.delay);
                }

                mActions = null;
                mCount = 0;
            }
        }

        public int size()
        {
            return mCount;
        }

        public Runnable getRunnable(int index)
        {
            if (index >= mCount)
            {
                throw new IndexOutOfRangeException();
            }
            return mActions[index].action;
        }

        public long getDelay(int index)
        {
            if (index >= mCount)
            {
                throw new IndexOutOfRangeException();
            }
            return mActions[index].delay;
        }

        private class HandlerAction
        {
            public Runnable action;
            public long delay;

            public HandlerAction() { }

            public HandlerAction(Runnable action, long delay)
            {
                this.action = action;
                this.delay = delay;
            }

            public bool matches(Runnable otherAction)
            {
                return otherAction == null && action == null
                        || action != null && action.Equals(otherAction);
            }
        }
    }
}