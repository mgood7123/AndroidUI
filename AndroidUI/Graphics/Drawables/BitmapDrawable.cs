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

using SkiaSharp;
using AndroidUI.Extensions;
using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;
using AndroidUI.Graphics.Shaders;
using AndroidUI.Graphics.Filters;
using AndroidUI.Applications;
using System.Drawing;

namespace AndroidUI.Graphics.Drawables
{

    /**
     * A Drawable that wraps a bitmap and can be tiled, stretched, or aligned. You can create a
     * BitmapDrawable from a file path, an input stream, through XML inflation, or from
     * a {@link android.graphics.Bitmap} object.
     * <p>It can be defined in an XML file with the <code>&lt;bitmap></code> element.  For more
     * information, see the guide to <a
     * href="{@docRoot}guide/topics/resources/drawable-resource.html">Drawable Resources</a>.</p>
     * <p>
     * Also see the {@link android.graphics.Bitmap} class, which handles the management and
     * transformation of raw bitmap graphics, and should be used when drawing to a
     * {@link android.graphics.Canvas}.
     * </p>
     *
     * @attr ref android.R.styleable#BitmapDrawable_src
     * @attr ref android.R.styleable#BitmapDrawable_antialias
     * @attr ref android.R.styleable#BitmapDrawable_filter
     * @attr ref android.R.styleable#BitmapDrawable_dither
     * @attr ref android.R.styleable#BitmapDrawable_gravity
     * @attr ref android.R.styleable#BitmapDrawable_mipMap
     * @attr ref android.R.styleable#BitmapDrawable_tileMode
     */
    public class BitmapDrawable : Drawable
    {
        Context context;
        private static readonly int DEFAULT_PAINT_FLAGS =
                Paint.FILTER_BITMAP_FLAG | Paint.DITHER_FLAG;

        // Constants for {@link android.R.styleable#BitmapDrawable_tileMode}.
        private const int TILE_MODE_UNDEFINED = -2;
        private const int TILE_MODE_DISABLED = -1;
        private const int TILE_MODE_CLAMP = 0;
        private const int TILE_MODE_REPEAT = 1;
        private const int TILE_MODE_MIRROR = 2;

        private Context.ContextVariable<Rect> mDstRect = new(StorageKeys.BitmapDrawableDstRect, context => () => new());


        private BitmapState mBitmapState;
        private BlendModeColorFilter mBlendModeFilter;

        private int mTargetDensity;

        private bool mDstRectAndInsetsDirty = true;
        private bool mMutated;

        // These are scaled to match the target density.
        private int mBitmapWidth;
        private int mBitmapHeight;

        /** Optical insets due to gravity. */
        private Insets mOpticalInsets = Insets.NONE;

        // Mirroring matrix for using with Shaders
        private SKMatrix? mMirrorMatrix;

        /**
         * Create an empty drawable, not dealing with density.
         * [Obsolete] Use {@link #BitmapDrawable(android.content.res.Resources, android.graphics.Bitmap)}
         * instead to specify a bitmap to draw with and ensure the correct density is set.
         */
        [Obsolete]
        public BitmapDrawable(Context context)
        {
            this.context = context;
            mTargetDensity = context.densityManager.ScreenDpi;
            init(new BitmapState(context, (Bitmap)null));
        }

        /**
         * Create drawable from a bitmap, not dealing with density.
         * [Obsolete] Use {@link #BitmapDrawable(Resources, Bitmap)} to ensure
         * that the drawable has correctly set its target density.
         */
        [Obsolete]
        public BitmapDrawable(Context context, Bitmap bitmap)
        {
            this.context = context;
            mTargetDensity = context.densityManager.ScreenDpi;
            init(new BitmapState(context, bitmap));
        }

        /**
         * Create a drawable by opening a given file path and decoding the bitmap.
         */
        public BitmapDrawable(Context context, string filepath)
        {
            this.context = context;
            mTargetDensity = context.densityManager.ScreenDpi;
            Bitmap bitmap = BitmapFactory.decodeFile(context, filepath);
            //try {
            //Stream stream = new StreamReader(filepath);
            //bitmap = ImageDecoder.decodeBitmap(ImageDecoder.createSource(res, stream),
            //        (decoder, info, src)-> {
            //    decoder.setAllocator(ImageDecoder.ALLOCATOR_SOFTWARE);
            //});
            //} catch (Exception e)
            //{
            /*  do nothing. This matches the behavior of BitmapFactory.decodeFile()
                If the exception happened on decode, mBitmapState.mBitmap will be null.
            */
            //}
            //finally
            //{
            init(new BitmapState(context, bitmap));
            if (mBitmapState.mBitmap == null)
            {
                Log.w("BitmapDrawable", "BitmapDrawable cannot decode " + filepath);
            }
            //}
        }

        /**
         * Create a drawable by decoding a bitmap from the given input stream.
         * [Obsolete] Use {@link #BitmapDrawable(Resources, java.io.InputStream)} to ensure
         * that the drawable has correctly set its target density.
         */
        public BitmapDrawable(Context context, Stream stream)
        {
            this.context = context;
            mTargetDensity = context.densityManager.ScreenDpi;
            Bitmap bitmap = BitmapFactory.decodeStream(context, stream);
            init(new BitmapState(context, bitmap));
            if (mBitmapState.mBitmap == null)
            {
                Log.w("BitmapDrawable", "BitmapDrawable cannot decode " + stream);
            }
        }

        /**
         * Returns the paint used to render this drawable.
         */
        public Paint getPaint()
        {
            return mBitmapState.mPaint;
        }

        /**
         * Returns the bitmap used by this drawable to render. May be null.
         */
        public Bitmap getBitmap()
        {
            return mBitmapState.mBitmap;
        }

        private void computeBitmapSize()
        {
            Bitmap bitmap = mBitmapState.mBitmap;
            if (bitmap != null)
            {
                mBitmapWidth = bitmap.getScaledWidth(mTargetDensity);
                mBitmapHeight = bitmap.getScaledHeight(mTargetDensity);
            }
            else
            {
                mBitmapWidth = mBitmapHeight = -1;
            }
        }

        /**
         * Switch to a new Bitmap object.
         */
        public void setBitmap(Bitmap bitmap)
        {
            if (mBitmapState.mBitmap != bitmap)
            {
                mBitmapState.mBitmap = bitmap;
                computeBitmapSize();
                invalidateSelf();
            }
        }

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
            setTargetDensity(canvas.context, canvas.DensityDPI);
        }

        /**
         * Set the density at which this drawable will be rendered.
         *
         * @param density The density scale for this drawable.
         *
         * @see android.graphics.Bitmap#setDensity(int)
         * @see android.graphics.Bitmap#getDensity()
         */
        public void setTargetDensity(Context context, int density)
        {
            if (mTargetDensity != density)
            {
                mTargetDensity = density == 0 ? context.densityManager.ScreenDpi : density;
                if (mBitmapState.mBitmap != null)
                {
                    computeBitmapSize();
                }
                invalidateSelf();
            }
        }

        /** Get the gravity used to position/stretch the bitmap within its bounds.
         * See android.view.Gravity
         * @return the gravity applied to the bitmap
         */
        public int getGravity()
        {
            return mBitmapState.mGravity;
        }

        /** Set the gravity used to position/stretch the bitmap within its bounds.
            See android.view.Gravity
         * @param gravity the gravity
         */
        public void setGravity(int gravity)
        {
            if (mBitmapState.mGravity != gravity)
            {
                mBitmapState.mGravity = gravity;
                mDstRectAndInsetsDirty = true;
                invalidateSelf();
            }
        }

        ///**
        // * Enables or disables the mipmap hint for this drawable's bitmap.
        // * See {@link Bitmap#setHasMipMap(bool)} for more information.
        // *
        // * If the bitmap is null calling this method has no effect.
        // *
        // * @param mipMap True if the bitmap should use mipmaps, false otherwise.
        // *
        // * @see #hasMipMap()
        // */
        //public void setMipMap(bool mipMap) {
        //    if (mBitmapState.mBitmap != null) {
        //        mBitmapState.mBitmap.setHasMipMap(mipMap);
        //        invalidateSelf();
        //    }
        //}

        ///**
        // * Indicates whether the mipmap hint is enabled on this drawable's bitmap.
        // *
        // * @return True if the mipmap hint is set, false otherwise. If the bitmap
        // *         is null, this method always returns false.
        // *
        // * @see #setMipMap(bool)
        // * @attr ref android.R.styleable#BitmapDrawable_mipMap
        // */
        //public bool hasMipMap() {
        //    return mBitmapState.mBitmap != null && mBitmapState.mBitmap.hasMipMap();
        //}

        /**
         * Enables or disables anti-aliasing for this drawable. Anti-aliasing affects
         * the edges of the bitmap only so it applies only when the drawable is rotated.
         *
         * @param aa True if the bitmap should be anti-aliased, false otherwise.
         *
         * @see #hasAntiAlias()
         */
        public void setAntiAlias(bool aa)
        {
            mBitmapState.mPaint.setAntiAlias(aa);
            invalidateSelf();
        }

        /**
         * Indicates whether anti-aliasing is enabled for this drawable.
         *
         * @return True if anti-aliasing is enabled, false otherwise.
         *
         * @see #setAntiAlias(bool)
         */
        public bool hasAntiAlias()
        {
            return mBitmapState.mPaint.isAntiAlias();
        }

        public override void setFilterBitmap(bool filter)
        {
            mBitmapState.mPaint.setFilterBitmap(filter);
            invalidateSelf();
        }

        public override bool isFilterBitmap()
        {
            return mBitmapState.mPaint.isFilterBitmap();
        }

        public override void setDither(bool dither)
        {
            mBitmapState.mPaint.setDither(dither);
            invalidateSelf();
        }

        /**
         * Indicates the repeat behavior of this drawable on the X axis.
         *
         * @return {@link android.graphics.Shader.TileMode#CLAMP} if the bitmap does not repeat,
         *         {@link android.graphics.Shader.TileMode#REPEAT} or
         *         {@link android.graphics.Shader.TileMode#MIRROR} otherwise.
         */
        public Shader.TileMode getTileModeX()
        {
            return mBitmapState.mTileModeX;
        }

        /**
         * Indicates the repeat behavior of this drawable on the Y axis.
         *
         * @return {@link android.graphics.Shader.TileMode#CLAMP} if the bitmap does not repeat,
         *         {@link android.graphics.Shader.TileMode#REPEAT} or
         *         {@link android.graphics.Shader.TileMode#MIRROR} otherwise.
         */
        public Shader.TileMode getTileModeY()
        {
            return mBitmapState.mTileModeY;
        }

        /**
         * Sets the repeat behavior of this drawable on the X axis. By default, the drawable
         * does not repeat its bitmap. Using {@link android.graphics.Shader.TileMode#REPEAT} or
         * {@link android.graphics.Shader.TileMode#MIRROR} the bitmap can be repeated (or tiled)
         * if the bitmap is smaller than this drawable.
         *
         * @param mode The repeat mode for this drawable.
         *
         * @see #setTileModeY(android.graphics.Shader.TileMode)
         * @see #setTileModeXY(android.graphics.Shader.TileMode, android.graphics.Shader.TileMode)
         * @attr ref android.R.styleable#BitmapDrawable_tileModeX
         */
        public void setTileModeX(Shader.TileMode mode)
        {
            setTileModeXY(mode, mBitmapState.mTileModeY);
        }

        /**
         * Sets the repeat behavior of this drawable on the Y axis. By default, the drawable
         * does not repeat its bitmap. Using {@link android.graphics.Shader.TileMode#REPEAT} or
         * {@link android.graphics.Shader.TileMode#MIRROR} the bitmap can be repeated (or tiled)
         * if the bitmap is smaller than this drawable.
         *
         * @param mode The repeat mode for this drawable.
         *
         * @see #setTileModeX(android.graphics.Shader.TileMode)
         * @see #setTileModeXY(android.graphics.Shader.TileMode, android.graphics.Shader.TileMode)
         * @attr ref android.R.styleable#BitmapDrawable_tileModeY
         */
        public void setTileModeY(Shader.TileMode mode)
        {
            setTileModeXY(mBitmapState.mTileModeX, mode);
        }

        /**
         * Sets the repeat behavior of this drawable on both axis. By default, the drawable
         * does not repeat its bitmap. Using {@link android.graphics.Shader.TileMode#REPEAT} or
         * {@link android.graphics.Shader.TileMode#MIRROR} the bitmap can be repeated (or tiled)
         * if the bitmap is smaller than this drawable.
         *
         * @param xmode The X repeat mode for this drawable.
         * @param ymode The Y repeat mode for this drawable.
         *
         * @see #setTileModeX(android.graphics.Shader.TileMode)
         * @see #setTileModeY(android.graphics.Shader.TileMode)
         */
        public void setTileModeXY(Shader.TileMode xmode, Shader.TileMode ymode)
        {
            BitmapState state = mBitmapState;
            if (state.mTileModeX != xmode || state.mTileModeY != ymode)
            {
                state.mTileModeX = xmode;
                state.mTileModeY = ymode;
                state.mRebuildShader = true;
                mDstRectAndInsetsDirty = true;
                invalidateSelf();
            }
        }

        public override void setAutoMirrored(bool mirrored)
        {
            if (mBitmapState.mAutoMirrored != mirrored)
            {
                mBitmapState.mAutoMirrored = mirrored;
                invalidateSelf();
            }
        }

        public override sealed bool isAutoMirrored()
        {
            return mBitmapState.mAutoMirrored;
        }

        public int getChangingConfigurations()
        {
            return base.getChangingConfigurations() | mBitmapState.getChangingConfigurations();
        }

        private bool needMirroring()
        {
            return isAutoMirrored() && getLayoutDirection() == LayoutDirection.RTL;
        }

        protected override void onBoundsChange(Rect bounds)
        {
            mDstRectAndInsetsDirty = true;

            Bitmap bitmap = mBitmapState.mBitmap;
            Shader shader = mBitmapState.mPaint.getShader();
            if (bitmap != null && shader != null)
            {
                updateShaderMatrix(bitmap, mBitmapState.mPaint, shader, needMirroring());
            }
        }

        public override void draw(Canvas canvas)
        {
            Bitmap bitmap = mBitmapState.mBitmap;
            if (bitmap == null)
            {
                return;
            }

            BitmapState state = mBitmapState;
            Paint paint = state.mPaint;
            if (state.mRebuildShader)
            {
                Shader.TileMode tmx = state.mTileModeX;
                Shader.TileMode tmy = state.mTileModeY;
                if (tmx == null && tmy == null)
                {
                    paint.setShader(null);
                }
                else
                {
                    paint.setShader(new BitmapShader(bitmap,
                            tmx == null ? Shader.TileMode.CLAMP : tmx,
                            tmy == null ? Shader.TileMode.CLAMP : tmy));
                }

                state.mRebuildShader = false;
            }

            int restoreAlpha;
            if (state.mBaseAlpha != 1.0f)
            {
                Paint p = getPaint();
                restoreAlpha = p.getAlpha();
                p.setAlpha((int)(restoreAlpha * state.mBaseAlpha + 0.5f));
            }
            else
            {
                restoreAlpha = -1;
            }

            bool clearColorFilter;
            if (mBlendModeFilter != null && paint.getColorFilter() == null)
            {
                paint.setColorFilter(mBlendModeFilter);
                clearColorFilter = true;
            }
            else
            {
                clearColorFilter = false;
            }

            updateDstRectAndInsetsIfDirty();
            Shader shader = paint.getShader();
            bool needMirroring_ = needMirroring();
            var r = mDstRect.Get(context);
            if (shader == null)
            {
                if (needMirroring_)
                {
                    canvas.Save();
                    // Mirror the bitmap
                    canvas.Translate(r.Value.right - r.Value.left, 0);
                    canvas.Scale(-1.0f, 1.0f);
                }

                canvas.DrawBitmap(bitmap, null, r.Value, paint);

                if (needMirroring_)
                {
                    canvas.Restore();
                }
            }
            else
            {
                updateShaderMatrix(bitmap, paint, shader, needMirroring_);
                canvas.DrawRect(r.Value, paint.getNativeInstance());
            }

            if (clearColorFilter)
            {
                paint.setColorFilter(null);
            }

            if (restoreAlpha >= 0)
            {
                paint.setAlpha(restoreAlpha);
            }
        }

        /**
         * Updates the {@code paint}'s shader matrix to be consistent with the
         * destination size and layout direction.
         *
         * @param bitmap the bitmap to be drawn
         * @param paint the paint used to draw the bitmap
         * @param shader the shader to set on the paint
         * @param needMirroring whether the bitmap should be mirrored
         */
        private void updateShaderMatrix(Bitmap bitmap, Paint paint,
                Shader shader, bool needMirroring)
        {
            int sourceDensity = bitmap.getDensity();
            int targetDensity = mTargetDensity;
            bool needScaling = sourceDensity != 0 && sourceDensity != targetDensity;
            if (needScaling || needMirroring)
            {
                SKMatrix matrix = getOrCreateMirrorMatrix();
                matrix.Reset();

                if (needMirroring)
                {
                    var r = mDstRect.Get(context);
                    int dx = r.Value.right - r.Value.left;
                    matrix.TransX = dx;
                    matrix.TransY = 0;
                    matrix.ScaleX = -1;
                    matrix.ScaleY = -1;
                }

                if (needScaling)
                {
                    float densityScale = targetDensity / (float)sourceDensity;
                    matrix.PostScale(densityScale, densityScale);
                }

                shader.setLocalMatrix(matrix);
            }
            else
            {
                mMirrorMatrix = null;
                shader.setLocalMatrix(SKMatrix.Identity);
            }

            paint.setShader(shader);
        }

        private SKMatrix getOrCreateMirrorMatrix()
        {
            if (mMirrorMatrix == null)
            {
                mMirrorMatrix = SKMatrix.Identity;
            }
            return mMirrorMatrix.Value;
        }

        private void updateDstRectAndInsetsIfDirty()
        {
            if (mDstRectAndInsetsDirty)
            {
                var r = mDstRect.Get(context);
                if (mBitmapState.mTileModeX == null && mBitmapState.mTileModeY == null)
                {
                    Rect bounds = getBounds();
                    int layoutDirection = getLayoutDirection();
                    Gravity.apply(mBitmapState.mGravity, mBitmapWidth, mBitmapHeight,
                            bounds, r.Value, layoutDirection);

                    int left = r.Value.left - bounds.left;
                    int top = r.Value.top - bounds.top;
                    int right = bounds.right - r.Value.right;
                    int bottom = bounds.bottom - r.Value.bottom;
                    mOpticalInsets = Insets.of(left, top, right, bottom);
                }
                else
                {
                    copyBounds(r.Value);
                    mOpticalInsets = Insets.NONE;
                }
            }
            mDstRectAndInsetsDirty = false;
        }

        public override Insets getOpticalInsets()
        {
            updateDstRectAndInsetsIfDirty();
            return mOpticalInsets;
        }

        public override void getOutline(Outline outline)
        {
            updateDstRectAndInsetsIfDirty();
            var r = mDstRect.Get(context);
            outline.setRect(r.Value);

            // Only opaque Bitmaps can report a non-0 alpha,
            // since only they are guaranteed to fill their bounds
            bool opaqueOverShape = mBitmapState.mBitmap != null
                    && !mBitmapState.mBitmap.hasAlpha();
            outline.setAlpha(opaqueOverShape ? getAlpha() / 255.0f : 0.0f);
        }

        public override void setAlpha(int alpha)
        {
            int oldAlpha = mBitmapState.mPaint.getAlpha();
            if (alpha != oldAlpha)
            {
                mBitmapState.mPaint.setAlpha(alpha);
                invalidateSelf();
            }
        }

        public override int getAlpha()
        {
            return mBitmapState.mPaint.getAlpha();
        }

        public override void setColorFilter(ColorFilter colorFilter)
        {
            mBitmapState.mPaint.setColorFilter(colorFilter);
            invalidateSelf();
        }

        public override ColorFilter getColorFilter()
        {
            return mBitmapState.mPaint.getColorFilter();
        }

        public override void setTintList(ColorStateList tint)
        {
            BitmapState state = mBitmapState;
            if (state.mTint != tint)
            {
                state.mTint = tint;
                mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, tint,
                          mBitmapState.mBlendMode);
                invalidateSelf();
            }
        }

        public override void setTintBlendMode(BlendMode blendMode)
        {
            BitmapState state = mBitmapState;
            if (state.mBlendMode != blendMode)
            {
                state.mBlendMode = blendMode;
                mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, mBitmapState.mTint,
                        blendMode);
                invalidateSelf();
            }
        }

        /**
         * No longer needed by ProgressBar, but still here due to UnsupportedAppUsage.
         */

        private ColorStateList getTint()
        {
            return mBitmapState.mTint;
        }

        /**
         * No longer needed by ProgressBar, but still here due to UnsupportedAppUsage.
         */

        private PorterDuff.Mode getTintMode()
        {
            return BlendMode.blendModeToPorterDuffMode(mBitmapState.mBlendMode);
        }

        /**
         * @hide Candidate for future API inclusion
         */
        internal override void setXfermode(Xfermode xfermode)
        {
            mBitmapState.mPaint.setXfermode(xfermode);
            invalidateSelf();
        }

        /**
         * A mutable BitmapDrawable still shares its Bitmap with any other Drawable
         * that comes from the same resource.
         *
         * @return This drawable.
         */
        public override Drawable mutate()
        {
            if (!mMutated && base.mutate() == this)
            {
                mBitmapState = new BitmapState(mBitmapState);
                mMutated = true;
            }
            return this;
        }

        /**
         * @hide
         */
        internal override void clearMutated()
        {
            base.clearMutated();
            mMutated = false;
        }

        protected override bool onStateChange(int[] stateSet)
        {
            BitmapState state = mBitmapState;
            if (state.mTint != null && state.mBlendMode != null)
            {
                mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, state.mTint,
                        state.mBlendMode);
                return true;
            }
            return false;
        }

        public override bool isStateful()
        {
            return mBitmapState.mTint != null && mBitmapState.mTint.isStateful()
                    || base.isStateful();
        }

        public override bool hasFocusStateSpecified()
        {
            return mBitmapState.mTint != null && mBitmapState.mTint.hasFocusStateSpecified();
        }

        private static Shader.TileMode parseTileMode(int tileMode)
        {
            switch (tileMode)
            {
                case TILE_MODE_CLAMP:
                    return Shader.TileMode.CLAMP;
                case TILE_MODE_REPEAT:
                    return Shader.TileMode.REPEAT;
                case TILE_MODE_MIRROR:
                    return Shader.TileMode.MIRROR;
                default:
                    return null;
            }
        }

        public override bool canApplyTheme()
        {
            return mBitmapState != null && mBitmapState.canApplyTheme();
        }

        public override int getIntrinsicWidth()
        {
            return mBitmapWidth;
        }

        public override int getIntrinsicHeight()
        {
            return mBitmapHeight;
        }

        public override PixelFormat getOpacity()
        {
            if (mBitmapState.mGravity != Gravity.FILL)
            {
                return PixelFormat.TRANSLUCENT;
            }

            Bitmap bitmap = mBitmapState.mBitmap;
            return bitmap == null || bitmap.hasAlpha() || mBitmapState.mPaint.getAlpha() < 255 ?
                    PixelFormat.TRANSLUCENT : PixelFormat.OPAQUE;
        }

        public override sealed ConstantState getConstantState()
        {
            mBitmapState.mChangingConfigurations |= getChangingConfigurations();
            return mBitmapState;
        }

        class BitmapState : ConstantState
        {
            internal Paint mPaint;

            // Values loaded during inflation.
            internal int[] mThemeAttrs = null;
            internal Bitmap mBitmap = null;
            internal ColorStateList mTint = null;
            internal BlendMode mBlendMode = DEFAULT_BLEND_MODE;

            internal int mGravity = Gravity.FILL;
            internal float mBaseAlpha = 1.0f;
            internal Shader.TileMode mTileModeX = null;
            internal Shader.TileMode mTileModeY = null;

            // The density to use when looking up the bitmap in Resources. A value of 0 means use
            // the system's density.
            internal int mSrcDensityOverride = 0;

            // The density at which to render the bitmap.
            internal int mTargetDensity;

            internal bool mAutoMirrored = false;

            internal int mChangingConfigurations;
            internal bool mRebuildShader;

            internal BitmapState(Context context, Bitmap bitmap)
            {
                mTargetDensity = context.densityManager.ScreenDpi;
                mBitmap = bitmap;
                mPaint = new Paint(DEFAULT_PAINT_FLAGS);
            }

            internal BitmapState(BitmapState bitmapState)
            {
                mBitmap = bitmapState.mBitmap;
                mTint = bitmapState.mTint;
                mBlendMode = bitmapState.mBlendMode;
                mThemeAttrs = bitmapState.mThemeAttrs;
                mChangingConfigurations = bitmapState.mChangingConfigurations;
                mGravity = bitmapState.mGravity;
                mTileModeX = bitmapState.mTileModeX;
                mTileModeY = bitmapState.mTileModeY;
                mSrcDensityOverride = bitmapState.mSrcDensityOverride;
                mTargetDensity = bitmapState.mTargetDensity;
                mBaseAlpha = bitmapState.mBaseAlpha;
                mPaint = new Paint(bitmapState.mPaint);
                mRebuildShader = bitmapState.mRebuildShader;
                mAutoMirrored = bitmapState.mAutoMirrored;
            }

            public override bool canApplyTheme()
            {
                return mThemeAttrs != null || mTint != null && mTint.canApplyTheme();
            }

            public override Drawable newDrawable()
            {
                return new BitmapDrawable(this);
            }

            public override int getChangingConfigurations()
            {
                return mChangingConfigurations
                        | (mTint != null ? mTint.getChangingConfigurations() : 0);
            }
        }

        private BitmapDrawable(BitmapState state)
        {
            init(state);
        }

        /**
         * The one helper to rule them all. This is called by all public & private
         * constructors to set the state and initialize local properties.
         */
        private void init(BitmapState state)
        {
            mBitmapState = state;
            updateLocalState();

            if (mBitmapState != null)
            {
                mBitmapState.mTargetDensity = mTargetDensity;
            }
        }

        /**
         * Initializes local dynamic properties from state. This should be called
         * after significant state changes, e.g. from the One True Constructor and
         * after inflating or applying a theme.
         */
        private void updateLocalState()
        {
            mTargetDensity = mBitmapState.mTargetDensity;
            mBlendModeFilter = updateBlendModeFilter(mBlendModeFilter, mBitmapState.mTint,
                    mBitmapState.mBlendMode);
            computeBitmapSize();
        }
    }
}