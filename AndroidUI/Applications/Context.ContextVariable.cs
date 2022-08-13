/*
 * Copyright (C) 2006 The Android Open Source Project
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

using AndroidUI.Utils;

namespace AndroidUI.Applications
{
    public partial class Context
    {
        public class ContextVariable<T>
        {
            string key;
            RunnableWithReturn<Context, RunnableWithReturn<T>> default_value_creator;

            public ContextVariable(string key, RunnableWithReturn<Context, RunnableWithReturn<T>> default_value_creator)
            {
                this.key = key;
                this.default_value_creator = default_value_creator;
            }

            public ValueHolder<T> Get(Context context, Thread thread = null)
            {
                if (context == null)
                {
                    return null;
                }
                return context.storage.GetOrCreate<T>(key, default_value_creator.Invoke(context), thread);
            }
        }
    }
}