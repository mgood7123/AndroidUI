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

using AndroidUI.Widgets;

namespace AndroidUI.Applications
{
    public partial class Context
    {
        public readonly Application application;
        public readonly Storage storage;
        public readonly DensityManager densityManager;

        internal View.AttachInfo mAttachInfo;

        public Context() : this(null)
        {
        }

        public Context(Application application)
        {
            mAttachInfo = new(this);

            if (application != null)
            {
                if (application.context != null)
                {
                    throw new ApplicationException("Application already has a context");
                }
                application.context = this;
                this.application = application;
            }
            storage = new();
            densityManager = new();
        }
    }
}