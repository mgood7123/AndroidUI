﻿/*
 * Copyright (C) 2009 The Android Open Source Project
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

namespace AndroidUI.AnimationFramework.Interpolators
{
    /**
     * An interpolator where the change flings forward and overshoots the last value
     * then comes back.
     */
    public class OvershootInterpolator : BaseInterpolator, Utils.ICloneable
    {
        private float mTension;

        virtual public OvershootInterpolator Clone()
        {
            var obj = (OvershootInterpolator)Utils.ICloneable.Clone(this);
            obj.mTension = mTension;
            return obj;
        }

        public OvershootInterpolator()
        {
            mTension = 2.0f;
        }

        public override float getInterpolation(float t)
        {
            // _o(t) = t * t * ((tension + 1) * t + tension)
            // o(t) = _o(t - 1) + 1
            t -= 1.0f;
            return t * t * ((mTension + 1) * t + mTension) + 1.0f;
        }
    }
}