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

namespace AndroidUI.AnimationFramework.Interpolators
{
    /**
     * An interpolator where the change starts backward then flings forward.
     */
    public class AnticipateInterpolator : BaseInterpolator
    {
        private readonly float mTension;

        public AnticipateInterpolator()
        {
            mTension = 2.0f;
        }

        /**
         * @param tension Amount of anticipation. When tension equals 0.0f, there is
         *                no anticipation and the interpolator becomes a simple
         *                acceleration interpolator.
         */
        public AnticipateInterpolator(float tension)
        {
            mTension = tension;
        }

        public override float getInterpolation(float t)
        {
            // a(t) = t * t * ((tension + 1) * t - tension)
            return t * t * ((mTension + 1) * t - mTension);
        }
    }
}