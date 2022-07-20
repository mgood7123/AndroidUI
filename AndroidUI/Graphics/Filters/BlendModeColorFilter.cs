/*
 * Copyright (C) 2018 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using AndroidUI.Extensions;

namespace AndroidUI.Graphics.Filters
{
    public sealed class BlendModeColorFilter : ColorFilter
    {

        readonly int mColor;
        private readonly BlendMode mMode;

        public BlendModeColorFilter(int color, BlendMode mode)
        {
            mColor = color;
            mMode = mode;
        }


        /**
         * Returns the ARGB color used to tint the source pixels when this filter
         * is applied.
         *
         * @see Color
         *
         */
        public int getColor()
        {
            return mColor;
        }

        /**
         * Returns the Porter-Duff mode used to composite this color filter's
         * color with the source pixel when this filter is applied.
         *
         * @see BlendMode
         *
         */
        public BlendMode getMode()
        {
            return mMode;
        }

        public override SkiaSharp.SKColorFilter createNativeInstance()
        {
            return native_CreateBlendModeFilter(mColor, mMode);
        }


        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || this.getClass() != obj.getClass())
            {
                return false;
            }
            BlendModeColorFilter other = (BlendModeColorFilter)obj;
            return other.mMode == mMode;
        }

        override public int GetHashCode()
        {
            return 31 * mMode.hashCode() + mColor;
        }
    }
}