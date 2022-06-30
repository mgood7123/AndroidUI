/*
 * Copyright (C) 2008 The Android Open Source Project
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

namespace AndroidUI
{
    /**
     * A specialized Drawable that fills the Canvas with a specified color.
     * Note that a ColorDrawable ignores the ColorFilter.
     *
     * <p>It can be defined in an XML file with the <code>&lt;color></code> element.</p>
     *
     * @attr ref android.R.styleable#ColorDrawable_color
     */
    public class ColorDrawable : Drawable
    {
        private readonly Paint mPaint = new();

        private ColorState mColorState;
        private BlendModeColorFilter mBlendModeColorFilter;

        private bool mMutated;

        /**
         * Creates a new black ColorDrawable.
         */
        public ColorDrawable()
        {
            mColorState = new ColorState();
        }

        /**
         * Creates a new ColorDrawable with the specified color.
         *
         * @param color The color to draw.
         */
        public ColorDrawable(int color)
        {
            mColorState = new ColorState();

            setColor(color);
        }

        override public int getChangingConfigurations()
        {
            return base.getChangingConfigurations() | mColorState.getChangingConfigurations();
        }

        /**
         * A mutable BitmapDrawable still shares its Bitmap with any other Drawable
         * that comes from the same resource.
         *
         * @return This drawable.
         */
        override public Drawable mutate()
        {
            if (!mMutated && base.mutate() == this)
            {
                mColorState = new ColorState(mColorState);
                mMutated = true;
            }
            return this;
        }

        /**
         * @hide
         */
        override internal void clearMutated()
        {
            base.clearMutated();
            mMutated = false;
        }

        override public void draw(SkiaSharp.SKCanvas canvas)
        {
            ColorFilter colorFilter = mPaint.getColorFilter();
            if (mColorState.mUseColor.UnsignedRightShift(24) != 0 || colorFilter != null
                    || mBlendModeColorFilter != null)
            {
                if (colorFilter == null)
                {
                    mPaint.setColorFilter(mBlendModeColorFilter);
                }

                mPaint.setColor(mColorState.mUseColor);
                canvas.DrawRect(getBounds(), mPaint.getNativeInstance());

                // Restore original color filter.
                mPaint.setColorFilter(colorFilter);
            }
        }

        /**
         * Gets the drawable's color value.
         *
         * @return int The color to draw.
         */
        public int getColor()
        {
            return mColorState.mUseColor;
        }

        /**
         * Sets the drawable's color value. This action will clobber the results of
         * prior calls to {@link #setAlpha(int)} on this object, which side-affected
         * the underlying color.
         *
         * @param color The color to draw.
         */
        public void setColor(int color)
        {
            if (mColorState.mBaseColor != color || mColorState.mUseColor != color)
            {
                mColorState.mBaseColor = mColorState.mUseColor = color;
                invalidateSelf();
            }
        }

        /**
         * Returns the alpha value of this drawable's color. Note this may not be the same alpha value
         * provided in {@link Drawable#setAlpha(int)}. Instead this will return the alpha of the color
         * combined with the alpha provided by setAlpha
         *
         * @return A value between 0 and 255.
         *
         * @see ColorDrawable#setAlpha(int)
         */
        override public int getAlpha()
        {
            return mColorState.mUseColor.UnsignedRightShift(24);
        }

        /**
         * Applies the given alpha to the underlying color. Note if the color already has
         * an alpha applied to it, this will apply this alpha to the existing value instead of
         * overwriting it.
         *
         * @param alpha The alpha value to set, between 0 and 255.
         */
        override public void setAlpha(int alpha)
        {
            alpha += alpha >> 7;   // make it 0..256
            int baseAlpha = mColorState.mBaseColor.UnsignedRightShift(24);
            int useAlpha = baseAlpha * alpha >> 8;
            int useColor = ((mColorState.mBaseColor << 8).UnsignedRightShift(8)) | (useAlpha << 24);
            if (mColorState.mUseColor != useColor)
            {
                mColorState.mUseColor = useColor;
                invalidateSelf();
            }
        }

        /**
         * Sets the color filter applied to this color.
         * <p>
         * Only supported on version {@link android.os.Build.VERSION_CODES#LOLLIPOP} and
         * above. Calling this method has no effect on earlier versions.
         *
         * @see android.graphics.drawable.Drawable#setColorFilter(ColorFilter)
         */
        override public void setColorFilter(ColorFilter colorFilter)
        {
            mPaint.setColorFilter(colorFilter);
        }

        /**
         * Returns the color filter applied to this color configured by
         * {@link #setColorFilter(ColorFilter)}
         *
         * @see android.graphics.drawable.Drawable#getColorFilter()
         */
        override public ColorFilter getColorFilter()
        {
            return mPaint.getColorFilter();
        }

        override public void setTintList(ColorStateList tint)
        {
            mColorState.mTint = tint;
            mBlendModeColorFilter = updateBlendModeFilter(mBlendModeColorFilter, tint,
                    mColorState.mBlendMode);
            invalidateSelf();
        }

        override public void setTintBlendMode(BlendMode blendMode)
        {
            mColorState.mBlendMode = blendMode;
            mBlendModeColorFilter = updateBlendModeFilter(mBlendModeColorFilter, mColorState.mTint,
                    blendMode);
            invalidateSelf();
        }

        override protected bool onStateChange(int[] stateSet)
        {
            ColorState state = mColorState;
            if (state.mTint != null && state.mBlendMode != null)
            {
                mBlendModeColorFilter = updateBlendModeFilter(mBlendModeColorFilter, state.mTint,
                        state.mBlendMode);
                return true;
            }
            return false;
        }

        override public bool isStateful()
        {
            return mColorState.mTint != null && mColorState.mTint.isStateful();
        }

        override public bool hasFocusStateSpecified()
        {
            return mColorState.mTint != null && mColorState.mTint.hasFocusStateSpecified();
        }

        /**
         * @hide
         * @param mode new transfer mode
         */
        internal override void setXfermode(Xfermode mode)
        {
            mPaint.setXfermode(mode);
            invalidateSelf();
        }

        /**
         * @hide
         * @return current transfer mode
         */
        internal Xfermode getXfermode()
        {
            return mPaint.getXfermode();
        }

        override public PixelFormat getOpacity()
        {
            if (mBlendModeColorFilter != null || mPaint.getColorFilter() != null)
            {
                return PixelFormat.TRANSLUCENT;
            }

            switch (mColorState.mUseColor.UnsignedRightShift(24))
            {
                case 255:
                    return PixelFormat.OPAQUE;
                case 0:
                    return PixelFormat.TRANSPARENT;
            }
            return PixelFormat.TRANSLUCENT;
        }

        override public void getOutline(Outline outline)
        {
            outline.setRect(getBounds());
            outline.setAlpha(getAlpha() / 255.0f);
        }


        override public bool canApplyTheme()
        {
            return mColorState.canApplyTheme() || base.canApplyTheme();
        }

        override public ConstantState getConstantState()
        {
            return mColorState;
        }

        internal sealed class ColorState : ConstantState
        {
            internal int[]
        mThemeAttrs;
            internal int mBaseColor; // base color, independent of setAlpha()
            internal int mUseColor;  // basecolor modulated by setAlpha()
            internal int mChangingConfigurations;
            internal ColorStateList mTint = null;
            internal BlendMode mBlendMode = DEFAULT_BLEND_MODE;

            internal ColorState() {
                // Empty constructor.
            }

            internal ColorState(ColorState state) {
                mThemeAttrs = state.mThemeAttrs;
                mBaseColor = state.mBaseColor;
                mUseColor = state.mUseColor;
                mChangingConfigurations = state.mChangingConfigurations;
                mTint = state.mTint;
                mBlendMode = state.mBlendMode;
            }

            override public bool canApplyTheme()
            {
                return mThemeAttrs != null
                        || (mTint != null && mTint.canApplyTheme());
            }

            override public Drawable newDrawable()
            {
                return new ColorDrawable(this);
            }

            override public int getChangingConfigurations()
            {
                return mChangingConfigurations
                        | (mTint != null ? mTint.getChangingConfigurations() : 0);
            }
        }

        private ColorDrawable(ColorState state)
        {
            mColorState = state;
        }
    }
}