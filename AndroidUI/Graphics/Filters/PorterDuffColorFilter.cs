﻿/*
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
using AndroidUI.Extensions;

namespace AndroidUI.Graphics.Filters
{
    /**
     * A color filter that can be used to tint the source pixels using a single
     * color and a specific {@link PorterDuff Porter-Duff composite mode}.
     */
    public class PorterDuffColorFilter : ColorFilter
    {
        private int mColor;
        private PorterDuff.Mode mMode;

        /**
         * Create a color filter that uses the specified color and Porter-Duff mode.
         *
         * @param color The ARGB source color used with the specified Porter-Duff mode
         * @param mode The porter-duff mode that is applied
         *
         * @see Color
         */
        public PorterDuffColorFilter(int color, PorterDuff.Mode mode)
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
         * @hide
         */
        public int getColor()
        {
            return mColor;
        }

        /**
         * Returns the Porter-Duff mode used to composite this color filter's
         * color with the source pixel when this filter is applied.
         *
         * @see PorterDuff
         *
         * @hide
         */
        public PorterDuff.Mode getMode()
        {
            return mMode;
        }

        public override SkiaSharp.SKColorFilter createNativeInstance()
        {
            return native_CreateBlendModeFilter(mColor, mMode.nativeInt);
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
            PorterDuffColorFilter other = (PorterDuffColorFilter)obj;
            return mColor == other.mColor && mMode.nativeInt == other.mMode.nativeInt;
        }

        public override int GetHashCode()
        {
            return 31 * mMode.GetHashCode() + mColor;
        }
    }
}