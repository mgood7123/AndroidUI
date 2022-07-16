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

using AndroidUI.Applications;
using SkiaSharp;

namespace AndroidUI.Widgets
{
    public class EdgeEffect
    {
        private Context context;

        public EdgeEffect(Context context)
        {
            this.context = context;
        }

        internal void setColor(int color)
        {
            //throw new NotImplementedException();
        }

        internal int getColor()
        {
            return 0; // throw new NotImplementedException();
        }

        internal bool isFinished()
        {
            return true;
            //throw new NotImplementedException();
        }

        internal float onPullDistance(float v, object value)
        {
            return 0;
        }

        internal float getDistance()
        {
            return 0;//throw new NotImplementedException();
        }

        internal void onRelease()
        {
            //throw new NotImplementedException();
        }

        internal void onAbsorb(int v)
        {
            //throw new NotImplementedException();
        }

        internal void setSize(int width, int height)
        {
            //throw new NotImplementedException();
        }

        internal bool draw(SKCanvas canvas)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}