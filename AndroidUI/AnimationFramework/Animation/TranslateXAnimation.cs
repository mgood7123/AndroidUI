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
     * Special case of TranslateAnimation that translates only horizontally, picking up the
     * vertical values from whatever is set on the Transformation already. When used in
     * conjunction with a TranslateYAnimation, allows independent animation of x and y
     * position.
     * @hide
     */
    public class TranslateXAnimation : TranslateAnimation
    {
        float[] mTmpValues = new float[9];

        public override TranslateXAnimation Clone()
        {
            TranslateXAnimation animation = (TranslateXAnimation)base.Clone();
            animation.mTmpValues = new float[9];
            return animation;
        }

        /**
         * Constructor. Passes in 0 for the y parameters of TranslateAnimation
         */
        public TranslateXAnimation(Context context, float fromXDelta, float toXDelta)
        : base(context, fromXDelta, toXDelta, 0, 0)
        {
        }

        /**
         * Constructor. Passes in 0 for the y parameters of TranslateAnimation
         */
        public TranslateXAnimation(Context context, int fromXType, float fromXValue, int toXType, float toXValue)
            : base(context, fromXType, fromXValue, toXType, toXValue, ABSOLUTE, 0, ABSOLUTE, 0)
        {
        }

        /**
         * Calculates and sets x translation values on given transformation.
         */
        override public void applyTransformation(float interpolatedTime, Transformation t)
        {
            SkiaSharp.SKMatrix m = t.getMatrix();
            m.GetValues(mTmpValues);
            float dx = mFromXDelta + (mToXDelta - mFromXDelta) * interpolatedTime;
            t.getMatrix().Value.SetTranslate(dx, mTmpValues[(int)SkiaSharp.SKMatrixRowMajorMask.TransY]);
        }
    }
}
