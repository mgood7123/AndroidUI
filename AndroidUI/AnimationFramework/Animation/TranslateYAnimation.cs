/*
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

namespace AndroidUI.AnimationFramework.Animation
{
    /**
     * Special case of TranslateAnimation that translates only vertically, picking up the
     * horizontal values from whatever is set on the Transformation already. When used in
     * conjunction with a TranslateXAnimation, allows independent animation of x and y
     * position.
     * @hide
     */
    public class TranslateYAnimation : TranslateAnimation
    {
        float[] mTmpValues = new float[9];

        public override TranslateYAnimation Clone()
        {
            TranslateYAnimation animation = (TranslateYAnimation)base.Clone();
            animation.mTmpValues = new float[9];
            return animation;
        }

        /**
         * Constructor. Passes in 0 for the x parameters of TranslateAnimation
         */
        public TranslateYAnimation(Context context, float fromYDelta, float toYDelta)
        : base(context, 0, 0, fromYDelta, toYDelta)
        {
        }

        /**
         * Constructor. Passes in 0 for the x parameters of TranslateAnimation
         */
        public TranslateYAnimation(Context context, int fromYType, float fromYValue, int toYType, float toYValue)
            : base(context, ABSOLUTE, 0, ABSOLUTE, 0, fromYType, fromYValue, toYType, toYValue)
        {
        }

        /**
         * Calculates and sets y translation values on given transformation.
         */
        override public void applyTransformation(float interpolatedTime, Transformation t)
        {
            SkiaSharp.SKMatrix m = t.getMatrix();
            m.GetValues(mTmpValues);
            float dy = mFromYDelta + (mToYDelta - mFromYDelta) * interpolatedTime;
            t.getMatrix().Value.SetTranslate(mTmpValues[(int)SkiaSharp.SKMatrixRowMajorMask.TransX], dy);
        }
    }
}
