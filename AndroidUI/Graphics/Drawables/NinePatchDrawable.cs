using AndroidUI.Applications;
using AndroidUI.Graphics.Filters;
using AndroidUI.Utils;
using AndroidUI.Utils.Graphics;
using AndroidUI.Utils.Widgets;

namespace AndroidUI.Graphics.Drawables
{
    /**
     *
     * A resizeable bitmap, with stretchable areas that you define. This type of image
     * is defined in a .png file with a special format.
     *
     * <div class="special reference">
     * <h3>Developer Guides</h3>
     * <p>For more information about how to use a NinePatchDrawable, read the
     * <a href="{@docRoot}guide/topics/graphics/2d-graphics.html#nine-patch">
     * Canvas and Drawables</a> developer guide. For information about creating a NinePatch image
     * file using the draw9patch tool, see the
     * <a href="{@docRoot}guide/developing/tools/draw9patch.html">Draw 9-patch</a> tool guide.</p></div>
     */
    public class NinePatchDrawable : Drawable
    {
        // dithering helps a lot, and is pretty cheap, so default is true
        private const bool DEFAULT_DITHER = false;

        /** Temporary rect used for density scaling. */
        private Rect mTempRect;

        private NinePatchState mNinePatchState;
        private BlendModeColorFilter mBlendModeFilter;
        private Rect mPadding;
        private Insets mOpticalInsets = Insets.NONE;
        private Rect mOutlineInsets;
        private float mOutlineRadius;
        private Paint mPaint;
        private bool mMutated;

        private int mTargetDensity = DensityManager.DEFAULT_SCREEN_DPI;

        // These are scaled to match the target density.
        private int mBitmapWidth = -1;
        private int mBitmapHeight = -1;

        NinePatchDrawable()
        {
            mNinePatchState = new NinePatchState();
        }

        /**
         * Create drawable from raw nine-patch data, not dealing with density.
         *
         * @deprecated Use {@link #NinePatchDrawable(Resources, Bitmap, byte[], Rect, String)}
         *             to ensure that the drawable has correctly set its target density.
         */
        [Obsolete]
        public NinePatchDrawable(Bitmap bitmap, byte[] chunk, Rect padding, String srcName)
            : this(new NinePatchState(new NinePatch(bitmap, chunk, srcName), padding))
        { }

        /**
         * Create drawable from existing nine-patch, not dealing with density.
         *
         * @deprecated Use {@link #NinePatchDrawable(Resources, NinePatch)}
         *             to ensure that the drawable has correctly set its target
         *             density.
         */
        [Obsolete]
        public NinePatchDrawable(NinePatch patch)
            : this(new NinePatchState(patch, new Rect()))
        { }

        /**
         * Set the density scale at which this drawable will be rendered. This
         * method assumes the drawable will be rendered at the same density as the
         * specified canvas.
         *
         * @param canvas The Canvas from which the density scale must be obtained.
         *
         * @see android.graphics.Bitmap#setDensity(int)
         * @see android.graphics.Bitmap#getDensity()
         */
        public void setTargetDensity(Canvas canvas)
        {
            setTargetDensity(canvas.DensityDPI);
        }

        /**
         * Set the density scale at which this drawable will be rendered.
         *
         * @param metrics The DisplayMetrics indicating the density scale for this drawable.
         *
         * @see android.graphics.Bitmap#setDensity(int)
         * @see android.graphics.Bitmap#getDensity()
         */
        public void setTargetDensity(DensityManager densityManager)
        {
            setTargetDensity(densityManager.ScreenDpi);
        }

        /**
         * Set the density at which this drawable will be rendered.
         *
         * @param density The density scale for this drawable.
         *
         * @see android.graphics.Bitmap#setDensity(int)
         * @see android.graphics.Bitmap#getDensity()
         */
        public void setTargetDensity(int density)
        {
            if (density == 0)
            {
                density = DensityManager.DEFAULT_SCREEN_DPI;
            }

            if (mTargetDensity != density)
            {
                mTargetDensity = density;

                computeBitmapSize();
                invalidateSelf();
            }
        }

        override
        public void draw(Canvas canvas)
        {
            NinePatchState state = mNinePatchState;

            Rect bounds = getBounds();
            int restoreToCount = -1;

            bool clearColorFilter;
            if (mBlendModeFilter != null && getPaint().getColorFilter() == null)
            {
                mPaint.setColorFilter(mBlendModeFilter);
                clearColorFilter = true;
            }
            else
            {
                clearColorFilter = false;
            }

            int restoreAlpha;
            if (state.mBaseAlpha != 1.0f)
            {
                restoreAlpha = getPaint().getAlpha();
                mPaint.setAlpha((int)(restoreAlpha * state.mBaseAlpha + 0.5f));
            }
            else
            {
                restoreAlpha = -1;
            }

            bool needsDensityScaling = canvas.DensityDPI == 0
                    && Bitmap.DENSITY_NONE != state.mNinePatch.getDensity();
            if (needsDensityScaling)
            {
                restoreToCount = restoreToCount >= 0 ? restoreToCount : canvas.Save();

                // Apply density scaling.
                float scale = mTargetDensity / (float)state.mNinePatch.getDensity();
                float px = bounds.left;
                float py = bounds.top;
                canvas.Scale(scale, scale, px, py);

                if (mTempRect == null)
                {
                    mTempRect = new Rect();
                }

                // Scale the bounds to match.
                Rect scaledBounds = mTempRect;
                scaledBounds.left = bounds.left;
                scaledBounds.top = bounds.top;
                scaledBounds.right = bounds.left + (int)MathF.Round(bounds.width() / scale);
                scaledBounds.bottom = bounds.top + (int)MathF.Round(bounds.height() / scale);
                bounds = scaledBounds;
            }

            bool needsMirroring_ = needsMirroring();
            if (needsMirroring_)
            {
                restoreToCount = restoreToCount >= 0 ? restoreToCount : canvas.Save();

                // Mirror the 9patch.
                float cx = (bounds.left + bounds.right) / 2.0f;
                float cy = (bounds.top + bounds.bottom) / 2.0f;
                canvas.Scale(-1.0f, 1.0f, cx, cy);
            }

            state.mNinePatch.draw(canvas, bounds, mPaint);

            if (restoreToCount >= 0)
            {
                canvas.RestoreToCount(restoreToCount);
            }

            if (clearColorFilter)
            {
                mPaint.setColorFilter(null);
            }

            if (restoreAlpha >= 0)
            {
                mPaint.setAlpha(restoreAlpha);
            }
        }

        override
        public int getChangingConfigurations()
        {
            return base.getChangingConfigurations() | mNinePatchState.getChangingConfigurations();
        }

        override
        public bool getPadding(Rect padding)
        {
            if (mPadding != null)
            {
                padding.set(mPadding);
                return (padding.left | padding.top | padding.right | padding.bottom) != 0;
            }
            else
            {
                return base.getPadding(padding);
            }
        }

        override
        public void getOutline(Outline outline)
        {
            Rect bounds = getBounds();
            if (bounds.isEmpty())
            {
                return;
            }

            if (mNinePatchState != null && mOutlineInsets != null)
            {
                NinePatch.InsetStruct insets =
                        mNinePatchState.mNinePatch.getBitmap().getNinePatchInsets();
                if (insets != null)
                {
                    outline.setRoundRect(bounds.left + mOutlineInsets.left,
                            bounds.top + mOutlineInsets.top,
                            bounds.right - mOutlineInsets.right,
                            bounds.bottom - mOutlineInsets.bottom,
                            mOutlineRadius);
                    outline.setAlpha(insets.outlineAlpha * (getAlpha() / 255.0f));
                    return;
                }
            }

            base.getOutline(outline);
        }

        override
        public Insets getOpticalInsets()
        {
            Insets opticalInsets = mOpticalInsets;
            if (needsMirroring())
            {
                return Insets.of(opticalInsets.right, opticalInsets.top,
                        opticalInsets.left, opticalInsets.bottom);
            }
            else
            {
                return opticalInsets;
            }
        }

        override
        public void setAlpha(int alpha)
        {
            if (mPaint == null && alpha == 0xFF)
            {
                // Fast common case -- leave at normal alpha.
                return;
            }
            getPaint().setAlpha(alpha);
            invalidateSelf();
        }

        override
        public int getAlpha()
        {
            if (mPaint == null)
            {
                // Fast common case -- normal alpha.
                return 0xFF;
            }
            return getPaint().getAlpha();
        }

        override
        public void setColorFilter(ColorFilter colorFilter)
        {
            if (mPaint == null && colorFilter == null)
            {
                // Fast common case -- leave at no color filter.
                return;
            }
            getPaint().setColorFilter(colorFilter);
            invalidateSelf();
        }

        override
        public void setTintList(ColorStateList tint)
        {
            mNinePatchState.mTint = tint;
            mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, tint,
                    mNinePatchState.mBlendMode);
            invalidateSelf();
        }

        override
        public void setTintBlendMode(BlendMode blendMode)
        {
            mNinePatchState.mBlendMode = blendMode;
            mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, mNinePatchState.mTint,
                    blendMode);
            invalidateSelf();
        }

        override
        public void setDither(bool dither)
        {
            //noinspection PointlessboolExpression
            if (mPaint == null && dither == DEFAULT_DITHER)
            {
                // Fast common case -- leave at default dither.
                return;
            }

            getPaint().setDither(dither);
            invalidateSelf();
        }

        override
        public void setAutoMirrored(bool mirrored)
        {
            mNinePatchState.mAutoMirrored = mirrored;
        }

        private bool needsMirroring()
        {
            return isAutoMirrored() && getLayoutDirection() == LayoutDirection.RTL;
        }

        override
        public bool isAutoMirrored()
        {
            return mNinePatchState.mAutoMirrored;
        }

        override
        public void setFilterBitmap(bool filter)
        {
            getPaint().setFilterBitmap(filter);
            invalidateSelf();
        }

        override
        public bool isFilterBitmap()
        {
            return mPaint != null && getPaint().isFilterBitmap();
        }


        public Paint getPaint()
        {
            if (mPaint == null)
            {
                mPaint = new Paint();
                mPaint.setDither(DEFAULT_DITHER);
            }
            return mPaint;
        }

        override
        public int getIntrinsicWidth()
        {
            return mBitmapWidth;
        }

        override
        public int getIntrinsicHeight()
        {
            return mBitmapHeight;
        }

        override
        public PixelFormat getOpacity()
        {
            return mNinePatchState.mNinePatch.hasAlpha()
                    || (mPaint != null && mPaint.getAlpha() < 255) ?
                            PixelFormat.TRANSLUCENT : PixelFormat.OPAQUE;
        }

        override
        public Region getTransparentRegion()
        {
            return mNinePatchState.mNinePatch.getTransparentRegion(getBounds());
        }

        override
        public ConstantState getConstantState()
        {
            mNinePatchState.mChangingConfigurations = getChangingConfigurations();
            return mNinePatchState;
        }

        override
        public Drawable mutate()
        {
            if (!mMutated && base.mutate() == this)
            {
                mNinePatchState = new NinePatchState(mNinePatchState);
                mMutated = true;
            }
            return this;
        }

        /**
         * @hide
         */
        public void clearMutated()
        {
            base.clearMutated();
            mMutated = false;
        }

        override
        protected bool onStateChange(int[] stateSet)
        {
            NinePatchState state = mNinePatchState;
            if (state.mTint != null && state.mBlendMode != null)
            {
                mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, state.mTint,
                        state.mBlendMode);
                return true;
            }

            return false;
        }

        override
        public bool isStateful()
        {
            NinePatchState s = mNinePatchState;
            return base.isStateful() || (s.mTint != null && s.mTint.isStateful());
        }

        override
        public bool hasFocusStateSpecified()
        {
            return mNinePatchState.mTint != null && mNinePatchState.mTint.hasFocusStateSpecified();
        }

        sealed class NinePatchState : ConstantState
        {
            public int mChangingConfigurations;

            // Values loaded during inflation.
            public NinePatch mNinePatch = null;
            public ColorStateList mTint = null;
            public BlendMode mBlendMode = DEFAULT_BLEND_MODE;
            public Rect mPadding = null;
            public Insets mOpticalInsets = Insets.NONE;
            public float mBaseAlpha = 1.0f;
            public bool mDither = DEFAULT_DITHER;
            public bool mAutoMirrored = false;

            public int[] mThemeAttrs;

            public NinePatchState()
            {
                // Empty constructor.
            }

            public NinePatchState(NinePatch ninePatch, Rect padding)
                : this(ninePatch, padding, null, DEFAULT_DITHER, false)
            { }

            public NinePatchState(NinePatch ninePatch, Rect padding,
                    Rect opticalInsets)
                : this(ninePatch, padding, opticalInsets, DEFAULT_DITHER, false)
            { }

            public NinePatchState(NinePatch ninePatch, Rect padding,
                    Rect opticalInsets, bool dither, bool autoMirror)
            {
                mNinePatch = ninePatch;
                mPadding = padding;
                mOpticalInsets = Insets.of(opticalInsets);
                mDither = dither;
                mAutoMirrored = autoMirror;
            }

            public NinePatchState(NinePatchState orig)
            {
                mChangingConfigurations = orig.mChangingConfigurations;
                mNinePatch = orig.mNinePatch;
                mTint = orig.mTint;
                mBlendMode = orig.mBlendMode;
                mPadding = orig.mPadding;
                mOpticalInsets = orig.mOpticalInsets;
                mBaseAlpha = orig.mBaseAlpha;
                mDither = orig.mDither;
                mAutoMirrored = orig.mAutoMirrored;
                mThemeAttrs = orig.mThemeAttrs;
            }

            override
            public bool canApplyTheme()
            {
                return mThemeAttrs != null
                        || (mTint != null && mTint.canApplyTheme())
                        || base.canApplyTheme();
            }

            override
            public Drawable newDrawable()
            {
                return new NinePatchDrawable(this);
            }

            override
            public int getChangingConfigurations()
            {
                return mChangingConfigurations
                        | (mTint != null ? mTint.getChangingConfigurations() : 0);
            }
        }

        private void computeBitmapSize()
        {
            NinePatch ninePatch = mNinePatchState.mNinePatch;
            if (ninePatch == null)
            {
                return;
            }

            int targetDensity = mTargetDensity;
            int sourceDensity = ninePatch.getDensity() == Bitmap.DENSITY_NONE ?
                targetDensity : ninePatch.getDensity();

            Insets sourceOpticalInsets = mNinePatchState.mOpticalInsets;
            if (sourceOpticalInsets != Insets.NONE)
            {
                int left = Drawable.scaleFromDensity(
                        sourceOpticalInsets.left, sourceDensity, targetDensity, true);
                int top = Drawable.scaleFromDensity(
                        sourceOpticalInsets.top, sourceDensity, targetDensity, true);
                int right = Drawable.scaleFromDensity(
                        sourceOpticalInsets.right, sourceDensity, targetDensity, true);
                int bottom = Drawable.scaleFromDensity(
                        sourceOpticalInsets.bottom, sourceDensity, targetDensity, true);
                mOpticalInsets = Insets.of(left, top, right, bottom);
            }
            else
            {
                mOpticalInsets = Insets.NONE;
            }

            Rect sourcePadding = mNinePatchState.mPadding;
            if (sourcePadding != null)
            {
                if (mPadding == null)
                {
                    mPadding = new Rect();
                }
                mPadding.left = Drawable.scaleFromDensity(
                        sourcePadding.left, sourceDensity, targetDensity, true);
                mPadding.top = Drawable.scaleFromDensity(
                        sourcePadding.top, sourceDensity, targetDensity, true);
                mPadding.right = Drawable.scaleFromDensity(
                        sourcePadding.right, sourceDensity, targetDensity, true);
                mPadding.bottom = Drawable.scaleFromDensity(
                        sourcePadding.bottom, sourceDensity, targetDensity, true);
            }
            else
            {
                mPadding = null;
            }

            mBitmapHeight = Drawable.scaleFromDensity(
                    ninePatch.getHeight(), sourceDensity, targetDensity, true);
            mBitmapWidth = Drawable.scaleFromDensity(
                    ninePatch.getWidth(), sourceDensity, targetDensity, true);

            NinePatch.InsetStruct insets = ninePatch.getBitmap().getNinePatchInsets();
            if (insets != null)
            {
                Rect outlineRect = insets.outlineRect;
                mOutlineInsets = NinePatch.InsetStruct.scaleInsets(outlineRect.left, outlineRect.top,
                        outlineRect.right, outlineRect.bottom, targetDensity / (float)sourceDensity);
                mOutlineRadius = Drawable.scaleFromDensity(
                        insets.outlineRadius, sourceDensity, targetDensity);
            }
            else
            {
                mOutlineInsets = null;
            }
        }

        /**
         * The one constructor to rule them all. This is called by all public
         * constructors to set the state and initialize local properties.
         *
         * @param state constant state to assign to the new drawable
         */
        private NinePatchDrawable(NinePatchState state)
        {
            mNinePatchState = state;
        }
    }
}