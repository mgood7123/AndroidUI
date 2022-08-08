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

using AndroidUI.Graphics;
using AndroidUI.Graphics.Drawables;
using AndroidUI.Graphics.Filters;
using AndroidUI.Utils;
using SkiaSharp;

namespace AndroidUI.Widgets
{

    /**
     * Displays image resources, for example {@link android.graphics.Bitmap}
     * or {@link android.graphics.drawable.Drawable} resources.
     * ImageView is also commonly used to
     * <a href="#setImageTintMode(android.graphics.PorterDuff.Mode)">apply tints to an image</a> and
     * handle <a href="#setScaleType(android.widget.ImageView.ScaleType)">image scaling</a>.
     *
     * <p>
     * The following XML snippet is a common example of using an ImageView to display an image resource:
     * </p>
     * <pre>
     * &lt;LinearLayout
     *     xmlns:android="http://schemas.android.com/apk/res/android"
     *     android:layout_width="match_parent"
     *     android:layout_height="match_parent"&gt;
     *     &lt;ImageView
     *         android:layout_width="wrap_content"
     *         android:layout_height="wrap_content"
     *         android:src="@drawable/my_image"
     *         android:contentDescription="@string/my_image_description"
     *         /&gt;
     * &lt;/LinearLayout&gt;
     * </pre>
     *
     * <p>
     * To learn more about Drawables, see: <a href="{@docRoot}guide/topics/resources/drawable-resource.html">Drawable Resources</a>.
     * To learn more about working with Bitmaps, see: <a href="{@docRoot}topic/performance/graphics/index.html">Handling Bitmaps</a>.
     * </p>
     *
     * @attr ref android.R.styleable#ImageView_adjustViewBounds
     * @attr ref android.R.styleable#ImageView_src
     * @attr ref android.R.styleable#ImageView_maxWidth
     * @attr ref android.R.styleable#ImageView_maxHeight
     * @attr ref android.R.styleable#ImageView_tint
     * @attr ref android.R.styleable#ImageView_scaleType
     * @attr ref android.R.styleable#ImageView_cropToPadding
     */
    public class ImageView : View
    {
        private const string LOG_TAG = "ImageView";

        // settable by the client
        private Uri mUri;
        private SKMatrix mMatrix = SKMatrix.Identity;
        private ScaleType mScaleType;
        private bool mHaveFrame = false;
        private bool mAdjustViewBounds = false;
        private int mMaxWidth = int.MaxValue;
        private int mMaxHeight = int.MaxValue;

        // these are applied to the drawable
        private ColorFilter mColorFilter = null;
        private bool mHasColorFilter = false;
        private Xfermode mXfermode;
        private bool mHasXfermode = false;
        private int mAlpha = 255;
        private bool mHasAlpha = false;
        private readonly int mViewAlphaScale = 256;

        private Drawable mDrawable = null;
        private BitmapDrawable mRecycleableBitmapDrawable = null;
        private ColorStateList mDrawableTintList = null;
        private BlendMode mDrawableBlendMode = null;
        private bool mHasDrawableTint = false;
        private bool mHasDrawableBlendMode = false;

        private int[] mState = null;
        private bool mMergeState = false;
        private int mLevel = 0;
        private int mDrawableWidth;
        private int mDrawableHeight;
        private ValueHolder<SKMatrix> mDrawMatrix = null;

        // Avoid allocations...
        private readonly RectF mTempSrc = new();
        private readonly RectF mTempDst = new();

        private bool mCropToPadding;

        private int mBaseline = -1;
        private bool mBaselineAlignBottom = false;
        /** AdjustViewBounds behavior will be in compatibility mode for older apps. */
        private static bool sCompatAdjustViewBounds = false;

        /** Whether to pass Resources when creating the source from a stream. */
        private static bool sCompatUseCorrectStreamDensity = true;

        /** Whether to use pre-Nougat drawable visibility dispatching conditions. */
        private static bool sCompatDrawableVisibilityDispatch = false;

        private static readonly ScaleType[] sScaleTypeArray = {
                ScaleType.MATRIX,
                ScaleType.FIT_XY,
                ScaleType.FIT_START,
                ScaleType.FIT_CENTER,
                ScaleType.FIT_END,
                ScaleType.CENTER,
                ScaleType.CENTER_CROP,
                ScaleType.CENTER_INSIDE
        };

        public ImageView() : base()
        {
            setWillDraw(true);
            initImageView();

            mBaselineAlignBottom = false;
            mBaseline = -1;

            setAdjustViewBounds(false);
            setMaxWidth(int.MaxValue);
            setMaxHeight(int.MaxValue);

            int index = -1;
            if (index >= 0)
            {
                setScaleType(sScaleTypeArray[index]);
            }

            applyImageTint();

            mCropToPadding = false;

            //need inflate syntax/reader for matrix
        }

        private void initImageView()
        {
            mMatrix = SKMatrix.Identity;
            mScaleType = ScaleType.CENTER_INSIDE;


            // By default, ImageView is not important for autofill but important for content capture.
            // Developers can override these defaults via the corresponding attributes.
            //if (getImportantForAutofill() == IMPORTANT_FOR_AUTOFILL_AUTO)
            //{
            //    setImportantForAutofill(IMPORTANT_FOR_AUTOFILL_NO);
            //}
            //if (getImportantForContentCapture() == IMPORTANT_FOR_CONTENT_CAPTURE_AUTO)
            //{
            //    setImportantForContentCapture(IMPORTANT_FOR_CONTENT_CAPTURE_YES);
            //}
        }

        override
            protected bool verifyDrawable(Drawable dr)
        {
            return mDrawable == dr || base.verifyDrawable(dr);
        }

        override
            public void jumpDrawablesToCurrentState()
        {
            base.jumpDrawablesToCurrentState();
            if (mDrawable != null) mDrawable.jumpToCurrentState();
        }

        override
            public void invalidateDrawable(Drawable dr)
        {
            if (dr == mDrawable)
            {
                if (dr != null)
                {
                    // update cached drawable dimensions if they've changed
                    int w = dr.getIntrinsicWidth();
                    int h = dr.getIntrinsicHeight();
                    if (w != mDrawableWidth || h != mDrawableHeight)
                    {
                        mDrawableWidth = w;
                        mDrawableHeight = h;
                        // updates the matrix, which is dependent on the bounds
                        configureBounds();
                    }
                }
                /* we invalidate the whole view in this case because it's very
                 * hard to know where the drawable actually is. This is made
                 * complicated because of the offsets and transformations that
                 * can be applied. In theory we could get the drawable's bounds
                 * and run them through the transformation and offsets, but this
                 * is probably not worth the effort.
                 */
                invalidate();
            }
            else
            {
                base.invalidateDrawable(dr);
            }
        }

        override
            public bool hasOverlappingRendering()
        {
            return getBackground() != null && getBackground().getCurrent() != null;
        }

        /**
         * True when ImageView is adjusting its bounds
         * to preserve the aspect ratio of its drawable
         *
         * @return whether to adjust the bounds of this view
         * to preserve the original aspect ratio of the drawable
         *
         * @see #setAdjustViewBounds(bool)
         *
         * @attr ref android.R.styleable#ImageView_adjustViewBounds
         */
        public bool getAdjustViewBounds()
        {
            return mAdjustViewBounds;
        }

        /**
         * Set this to true if you want the ImageView to adjust its bounds
         * to preserve the aspect ratio of its drawable.
         *
         * <p><strong>Note:</strong> If the application targets API level 17 or lower,
         * adjustViewBounds will allow the drawable to shrink the view bounds, but not grow
         * to fill available measured space in all cases. This is for compatibility with
         * legacy {@link android.view.View.MeasureSpec MeasureSpec} and
         * {@link android.widget.RelativeLayout RelativeLayout} behavior.</p>
         *
         * @param adjustViewBounds Whether to adjust the bounds of this view
         * to preserve the original aspect ratio of the drawable.
         *
         * @see #getAdjustViewBounds()
         *
         * @attr ref android.R.styleable#ImageView_adjustViewBounds
         */
        public void setAdjustViewBounds(bool adjustViewBounds)
        {
            mAdjustViewBounds = adjustViewBounds;
            if (adjustViewBounds)
            {
                setScaleType(ScaleType.FIT_CENTER);
            }
        }

        /**
         * The maximum width of this view.
         *
         * @return The maximum width of this view
         *
         * @see #setMaxWidth(int)
         *
         * @attr ref android.R.styleable#ImageView_maxWidth
         */
        public int getMaxWidth()
        {
            return mMaxWidth;
        }

        /**
         * An optional argument to supply a maximum width for this view. Only valid if
         * {@link #setAdjustViewBounds(bool)} has been set to true. To set an image to be a maximum
         * of 100 x 100 while preserving the original aspect ratio, do the following: 1) set
         * adjustViewBounds to true 2) set maxWidth and maxHeight to 100 3) set the height and width
         * layout params to WRAP_CONTENT.
         *
         * <p>
         * Note that this view could be still smaller than 100 x 100 using this approach if the original
         * image is small. To set an image to a fixed size, specify that size in the layout params and
         * then use {@link #setScaleType(android.widget.ImageView.ScaleType)} to determine how to fit
         * the image within the bounds.
         * </p>
         *
         * @param maxWidth maximum width for this view
         *
         * @see #getMaxWidth()
         *
         * @attr ref android.R.styleable#ImageView_maxWidth
         */
        public void setMaxWidth(int maxWidth)
        {
            mMaxWidth = maxWidth;
        }

        /**
         * The maximum height of this view.
         *
         * @return The maximum height of this view
         *
         * @see #setMaxHeight(int)
         *
         * @attr ref android.R.styleable#ImageView_maxHeight
         */
        public int getMaxHeight()
        {
            return mMaxHeight;
        }

        /**
         * An optional argument to supply a maximum height for this view. Only valid if
         * {@link #setAdjustViewBounds(bool)} has been set to true. To set an image to be a
         * maximum of 100 x 100 while preserving the original aspect ratio, do the following: 1) set
         * adjustViewBounds to true 2) set maxWidth and maxHeight to 100 3) set the height and width
         * layout params to WRAP_CONTENT.
         *
         * <p>
         * Note that this view could be still smaller than 100 x 100 using this approach if the original
         * image is small. To set an image to a fixed size, specify that size in the layout params and
         * then use {@link #setScaleType(android.widget.ImageView.ScaleType)} to determine how to fit
         * the image within the bounds.
         * </p>
         *
         * @param maxHeight maximum height for this view
         *
         * @see #getMaxHeight()
         *
         * @attr ref android.R.styleable#ImageView_maxHeight
         */
        public void setMaxHeight(int maxHeight)
        {
            mMaxHeight = maxHeight;
        }

        /**
         * Gets the current Drawable, or null if no Drawable has been
         * assigned.
         *
         * @return the view's drawable, or null if no drawable has been
         * assigned.
         */
        public Drawable getDrawable()
        {
            if (mDrawable == mRecycleableBitmapDrawable)
            {
                // Consider our cached version dirty since app code now has a reference to it
                mRecycleableBitmapDrawable = null;
            }
            return mDrawable;
        }

        private class ImageDrawableCallback
        {
            ImageView outer;
            private readonly Drawable drawable;
            Runnable runnable;

            public ImageDrawableCallback(ImageView outer, Drawable drawable)
            {
                this.drawable = drawable;
                this.outer = outer;
                runnable = () => outer.setImageDrawable(drawable);
            }
        }

        /**
         * Sets a drawable as the content of this ImageView.
         *
         * @param drawable the Drawable to set, or {@code null} to clear the
         *                 content
         */
        public void setImageDrawable(Drawable drawable)
        {
            if (mDrawable != drawable)
            {
                int oldWidth = mDrawableWidth;
                int oldHeight = mDrawableHeight;

                updateDrawable(drawable);

                if (oldWidth != mDrawableWidth || oldHeight != mDrawableHeight)
                {
                    requestLayout();
                }
                invalidate();
            }
        }

        // TODO: RESTORE ME
        ///**
        // * Sets the content of this ImageView to the specified Icon.
        // *
        // * <p class="note">Depending on the Icon type, this may do Bitmap reading
        // * and decoding on the UI thread, which can cause UI jank.  If that's a
        // * concern, consider using
        // * {@link Icon#loadDrawableAsync(Context, Icon.OnDrawableLoadedListener, Handler)}
        // * and then {@link #setImageDrawable(android.graphics.drawable.Drawable)}
        // * instead.</p>
        // *
        // * @param icon an Icon holding the desired image, or {@code null} to clear
        // *             the content
        // */
        //    public void setImageIcon(Icon icon)
        //{
        //    setImageDrawable(icon == null ? null : icon.loadDrawable(mContext));
        //}

        ///** @hide **/
        //public Runnable setImageIconAsync(Icon icon)
        //{
        //    return new ImageDrawableCallback(this, icon == null ? null : icon.loadDrawable(mContext));
        //}

        /**
         * Applies a tint to the image drawable. Does not modify the current tint
         * mode, which is {@link PorterDuff.Mode#SRC_IN} by default.
         * <p>
         * Subsequent calls to {@link #setImageDrawable(Drawable)} will automatically
         * mutate the drawable and apply the specified tint and tint mode using
         * {@link Drawable#setTintList(ColorStateList)}.
         * <p>
         * <em>Note:</em> The default tint mode used by this setter is NOT
         * consistent with the default tint mode used by the
         * {@link android.R.styleable#ImageView_tint android:tint}
         * attribute. If the {@code android:tint} attribute is specified, the
         * default tint mode will be set to {@link PorterDuff.Mode#SRC_ATOP} to
         * ensure consistency with earlier versions of the platform.
         *
         * @param tint the tint to apply, may be {@code null} to clear tint
         *
         * @attr ref android.R.styleable#ImageView_tint
         * @see #getImageTintList()
         * @see Drawable#setTintList(ColorStateList)
         */
        public void setImageTintList(ColorStateList tint)
        {
            mDrawableTintList = tint;
            mHasDrawableTint = true;

            applyImageTint();
        }

        /**
         * Get the current {@link android.content.res.ColorStateList} used to tint the image Drawable,
         * or null if no tint is applied.
         *
         * @return the tint applied to the image drawable
         * @attr ref android.R.styleable#ImageView_tint
         * @see #setImageTintList(ColorStateList)
         */
        public ColorStateList getImageTintList()
        {
            return mDrawableTintList;
        }

        /**
         * Specifies the blending mode used to apply the tint specified by
         * {@link #setImageTintList(ColorStateList)}} to the image drawable. The default
         * mode is {@link PorterDuff.Mode#SRC_IN}.
         *
         * @param tintMode the blending mode used to apply the tint, may be
         *                 {@code null} to clear tint
         * @attr ref android.R.styleable#ImageView_tintMode
         * @see #getImageTintMode()
         * @see Drawable#setTintMode(PorterDuff.Mode)
         */
        public void setImageTintMode(PorterDuff.Mode tintMode)
        {
            setImageTintBlendMode(tintMode != null ? BlendMode.fromValue(tintMode.nativeInt) : null);
        }

        /**
         * Specifies the blending mode used to apply the tint specified by
         * {@link #setImageTintList(ColorStateList)}} to the image drawable. The default
         * mode is {@link BlendMode#SRC_IN}.
         *
         * @param blendMode the blending mode used to apply the tint, may be
         *                 {@code null} to clear tint
         * @attr ref android.R.styleable#ImageView_tintMode
         * @see #getImageTintMode()
         * @see Drawable#setTintBlendMode(BlendMode)
         */
        public void setImageTintBlendMode(BlendMode blendMode)
        {
            mDrawableBlendMode = blendMode;
            mHasDrawableBlendMode = true;

            applyImageTint();
        }

        /**
         * Gets the blending mode used to apply the tint to the image Drawable
         * @return the blending mode used to apply the tint to the image Drawable
         * @attr ref android.R.styleable#ImageView_tintMode
         * @see #setImageTintMode(PorterDuff.Mode)
         */
        public PorterDuff.Mode getImageTintMode()
        {
            return mDrawableBlendMode != null
                    ? BlendMode.blendModeToPorterDuffMode(mDrawableBlendMode) : null;
        }

        /**
         * Gets the blending mode used to apply the tint to the image Drawable
         * @return the blending mode used to apply the tint to the image Drawable
         * @attr ref android.R.styleable#ImageView_tintMode
         * @see #setImageTintBlendMode(BlendMode)
         */
        public BlendMode getImageTintBlendMode()
        {
            return mDrawableBlendMode;
        }

        private void applyImageTint()
        {
            if (mDrawable != null && (mHasDrawableTint || mHasDrawableBlendMode))
            {
                mDrawable = mDrawable.mutate();

                if (mHasDrawableTint)
                {
                    mDrawable.setTintList(mDrawableTintList);
                }

                if (mHasDrawableBlendMode)
                {
                    mDrawable.setTintBlendMode(mDrawableBlendMode);
                }

                // The drawable (or one of its children) may not have been
                // stateful before applying the tint, so let's try again.
                if (mDrawable.isStateful())
                {
                    mDrawable.setState(getDrawableState());
                }
            }
        }

        /**
         * Sets a Bitmap as the content of this ImageView.
         *
         * @param bm The bitmap to set
         */
        public void setImageBitmap(Bitmap bm)
        {
            // Hacky fix to force setImageDrawable to do a full setImageDrawable
            // instead of doing an object reference comparison
            mDrawable = null;
            if (mRecycleableBitmapDrawable == null)
            {
                mRecycleableBitmapDrawable = new BitmapDrawable(bm);
            }
            else
            {
                mRecycleableBitmapDrawable.setBitmap(bm);
            }
            setImageDrawable(mRecycleableBitmapDrawable);
        }

        /**
         * Set the state of the current {@link android.graphics.drawable.StateListDrawable}.
         * For more information about State List Drawables, see: <a href="https://developer.android.com/guide/topics/resources/drawable-resource.html#StateList">the Drawable Resource Guide</a>.
         *
         * @param state the state to set for the StateListDrawable
         * @param merge if true, merges the state values for the state you specify into the current state
         */
        public void setImageState(int[] state, bool merge)
        {
            mState = state;
            mMergeState = merge;
            if (mDrawable != null)
            {
                refreshDrawableState();
                resizeFromDrawable();
            }
        }

        override
            public void setSelected(bool selected)
        {
            base.setSelected(selected);
            resizeFromDrawable();
        }

        /**
         * Sets the image level, when it is constructed from a
         * {@link android.graphics.drawable.LevelListDrawable}.
         *
         * @param level The new level for the image.
         */
        public void setImageLevel(int level)
        {
            mLevel = level;
            if (mDrawable != null)
            {
                mDrawable.setLevel(level);
                resizeFromDrawable();
            }
        }

        /**
         * Options for scaling the bounds of an image to the bounds of this view.
         */
        public class ScaleType
        {
            /**
             * Scale using the image matrix when drawing. The image matrix can be set using
             * {@link ImageView#setImageMatrix(Matrix)}. From XML, use this syntax:
             * <code>android:scaleType="matrix"</code>.
             */
            public static ScaleType MATRIX = new(0);
            /**
             * Scale the image using {@link Matrix.ScaleToFit#FILL}.
             * From XML, use this syntax: <code>android:scaleType="fitXY"</code>.
             */
            public static ScaleType FIT_XY = new(1);
            /**
             * Scale the image using {@link Matrix.ScaleToFit#START}.
             * From XML, use this syntax: <code>android:scaleType="fitStart"</code>.
             */
            public static ScaleType FIT_START = new(2);
            /**
             * Scale the image using {@link Matrix.ScaleToFit#CENTER}.
             * From XML, use this syntax:
             * <code>android:scaleType="fitCenter"</code>.
             */
            public static ScaleType FIT_CENTER = new(3);
            /**
             * Scale the image using {@link Matrix.ScaleToFit#END}.
             * From XML, use this syntax: <code>android:scaleType="fitEnd"</code>.
             */
            public static ScaleType FIT_END = new(4);
            /**
             * Center the image in the view, but perform no scaling.
             * From XML, use this syntax: <code>android:scaleType="center"</code>.
             */
            public static ScaleType CENTER = new(5);
            /**
             * Scale the image uniformly (maintain the image's aspect ratio) so
             * that both dimensions (width and height) of the image will be equal
             * to or larger than the corresponding dimension of the view
             * (minus padding). The image is then centered in the view.
             * From XML, use this syntax: <code>android:scaleType="centerCrop"</code>.
             */
            public static ScaleType CENTER_CROP = new(6);
            /**
             * Scale the image uniformly (maintain the image's aspect ratio) so
             * that both dimensions (width and height) of the image will be equal
             * to or less than the corresponding dimension of the view
             * (minus padding). The image is then centered in the view.
             * From XML, use this syntax: <code>android:scaleType="centerInside"</code>.
             */
            public static ScaleType CENTER_INSIDE = new(7);

            ScaleType(int ni)
            {
                nativeInt = ni;
            }
            internal readonly int nativeInt;
        }

        /**
         * Controls how the image should be resized or moved to match the size
         * of this ImageView.
         *
         * @param scaleType The desired scaling mode.
         *
         * @attr ref android.R.styleable#ImageView_scaleType
         */
        public void setScaleType(ScaleType scaleType)
        {
            if (scaleType == null)
            {
                throw new NullReferenceException();
            }

            if (mScaleType != scaleType)
            {
                mScaleType = scaleType;

                requestLayout();
                invalidate();
            }
        }

        /**
         * Returns the current ScaleType that is used to scale the bounds of an image to the bounds of the ImageView.
         * @return The ScaleType used to scale the image.
         * @see ImageView.ScaleType
         * @attr ref android.R.styleable#ImageView_scaleType
         */
        public ScaleType getScaleType()
        {
            return mScaleType;
        }

        /** Returns the view's optional matrix. This is applied to the
            view's drawable when it is drawn. If there is no matrix,
            this method will return an identity matrix.
            Do not change this matrix in place but make a copy.
            If you want a different matrix applied to the drawable,
            be sure to call setImageMatrix().
*/
        public SKMatrix getImageMatrix()
        {
            if (mDrawMatrix != null)
            {
                return SKMatrix.Identity;
            }
            return mDrawMatrix;
        }

        /**
         * Adds a transformation {@link Matrix} that is applied
         * to the view's drawable when it is drawn.  Allows custom scaling,
         * translation, and perspective distortion.
         *
         * @param matrix The transformation parameters in matrix form.
         */
        public void setImageMatrix(SKMatrix? matrix)
        {
            // collapse null and identity to just null
            if (matrix != null && matrix.Value.IsIdentity)
            {
                matrix = null;
            }

            // don't invalidate unless we're actually changing our matrix
            if (matrix == null && !mMatrix.IsIdentity ||
                    matrix != null && !mMatrix.Equals(matrix.Value))
            {
                mMatrix = matrix == null ? SKMatrix.Identity : matrix.Value;
                configureBounds();
                invalidate();
            }
        }

        /**
         * Return whether this ImageView crops to padding.
         *
         * @return whether this ImageView crops to padding
         *
         * @see #setCropToPadding(bool)
         *
         * @attr ref android.R.styleable#ImageView_cropToPadding
         */
        public bool getCropToPadding()
        {
            return mCropToPadding;
        }

        /**
         * Sets whether this ImageView will crop to padding.
         *
         * @param cropToPadding whether this ImageView will crop to padding
         *
         * @see #getCropToPadding()
         *
         * @attr ref android.R.styleable#ImageView_cropToPadding
         */
        public void setCropToPadding(bool cropToPadding)
        {
            if (mCropToPadding != cropToPadding)
            {
                mCropToPadding = cropToPadding;
                requestLayout();
                invalidate();
            }
        }

        override
        protected int[] onCreateDrawableState(int extraSpace)
        {
            if (mState == null)
            {
                return base.onCreateDrawableState(extraSpace);
            }
            else if (!mMergeState)
            {
                return mState;
            }
            else
            {
                return mergeDrawableStates(
                        base.onCreateDrawableState(extraSpace + mState.Length), mState);
            }
        }

        private void updateDrawable(Drawable d)
        {
            if (d != mRecycleableBitmapDrawable && mRecycleableBitmapDrawable != null)
            {
                mRecycleableBitmapDrawable.setBitmap(null);
            }

            bool sameDrawable = false;

            if (mDrawable != null)
            {
                sameDrawable = mDrawable == d;
                mDrawable.setCallback(null);
                unscheduleDrawable(mDrawable);
                if (!sCompatDrawableVisibilityDispatch && !sameDrawable && isAttachedToWindow())
                {
                    mDrawable.setVisible(false, false);
                }
            }

            mDrawable = d;

            if (d != null)
            {
                d.setCallback(this);
                d.setLayoutDirection(getLayoutDirection());
                if (d.isStateful())
                {
                    d.setState(getDrawableState());
                }
                if (!sameDrawable || sCompatDrawableVisibilityDispatch)
                {
                    bool visible = sCompatDrawableVisibilityDispatch
                            ? getVisibility() == VISIBLE
                            : isAttachedToWindow() && getWindowVisibility() == VISIBLE && isShown();
                    d.setVisible(visible, true);
                }
                d.setLevel(mLevel);
                mDrawableWidth = d.getIntrinsicWidth();
                mDrawableHeight = d.getIntrinsicHeight();
                applyImageTint();
                applyColorFilter();
                applyAlpha();
                applyXfermode();

                configureBounds();
            }
            else
            {
                mDrawableWidth = mDrawableHeight = -1;
            }
        }

        private void resizeFromDrawable()
        {
            Drawable d = mDrawable;
            if (d != null)
            {
                int w = d.getIntrinsicWidth();
                if (w < 0) w = mDrawableWidth;
                int h = d.getIntrinsicHeight();
                if (h < 0) h = mDrawableHeight;
                if (w != mDrawableWidth || h != mDrawableHeight)
                {
                    mDrawableWidth = w;
                    mDrawableHeight = h;
                    requestLayout();
                }
            }
        }

        override
            public void onRtlPropertiesChanged(int layoutDirection)
        {
            base.onRtlPropertiesChanged(layoutDirection);

            if (mDrawable != null)
            {
                mDrawable.setLayoutDirection(layoutDirection);
            }
        }

        private static readonly SKMatrixScaleToFit[] sS2FArray = {
            SKMatrixScaleToFit.Fill,
            SKMatrixScaleToFit.Start,
            SKMatrixScaleToFit.Center,
            SKMatrixScaleToFit.End
        };

        private static SKMatrixScaleToFit scaleTypeToScaleToFit(ScaleType st)
        {
            // ScaleToFit enum to their corresponding Matrix.ScaleToFit values
            return sS2FArray[st.nativeInt - 1];
        }

        override
            protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int w;
            int h;

            // Desired aspect ratio of the view's contents (not including padding)
            float desiredAspect = 0.0f;

            // We are allowed to change the view's width
            bool resizeWidth = false;

            // We are allowed to change the view's height
            bool resizeHeight = false;

            int widthSpecMode = MeasureSpec.getMode(widthMeasureSpec);
            int heightSpecMode = MeasureSpec.getMode(heightMeasureSpec);

            if (mDrawable == null)
            {
                // If no drawable, its intrinsic size is 0.
                mDrawableWidth = -1;
                mDrawableHeight = -1;
                w = h = 0;
            }
            else
            {
                w = mDrawableWidth;
                h = mDrawableHeight;
                if (w <= 0) w = 1;
                if (h <= 0) h = 1;

                // We are supposed to adjust view bounds to match the aspect
                // ratio of our drawable. See if that is possible.
                if (mAdjustViewBounds)
                {
                    resizeWidth = widthSpecMode != MeasureSpec.EXACTLY;
                    resizeHeight = heightSpecMode != MeasureSpec.EXACTLY;

                    desiredAspect = w / (float)h;
                }
            }

            int pleft = mPaddingLeft;
            int pright = mPaddingRight;
            int ptop = mPaddingTop;
            int pbottom = mPaddingBottom;

            int widthSize;
            int heightSize;

            if (resizeWidth || resizeHeight)
            {
                /* If we get here, it means we want to resize to match the
                    drawables aspect ratio, and we have the freedom to change at
                    least one dimension.
                */

                // Get the max possible width given our constraints
                widthSize = resolveAdjustedSize(w + pleft + pright, mMaxWidth, widthMeasureSpec);

                // Get the max possible height given our constraints
                heightSize = resolveAdjustedSize(h + ptop + pbottom, mMaxHeight, heightMeasureSpec);

                if (desiredAspect != 0.0f)
                {
                    // See what our actual aspect ratio is
                    float actualAspect = (float)(widthSize - pleft - pright) /
                                            (heightSize - ptop - pbottom);

                    if (Math.Abs(actualAspect - desiredAspect) > 0.0000001)
                    {

                        bool done = false;

                        // Try adjusting width to be proportional to height
                        if (resizeWidth)
                        {
                            int newWidth = (int)(desiredAspect * (heightSize - ptop - pbottom)) +
                                    pleft + pright;

                            // Allow the width to outgrow its original estimate if height is fixed.
                            if (!resizeHeight && !sCompatAdjustViewBounds)
                            {
                                widthSize = resolveAdjustedSize(newWidth, mMaxWidth, widthMeasureSpec);
                            }

                            if (newWidth <= widthSize)
                            {
                                widthSize = newWidth;
                                done = true;
                            }
                        }

                        // Try adjusting height to be proportional to width
                        if (!done && resizeHeight)
                        {
                            int newHeight = (int)((widthSize - pleft - pright) / desiredAspect) +
                                    ptop + pbottom;

                            // Allow the height to outgrow its original estimate if width is fixed.
                            if (!resizeWidth && !sCompatAdjustViewBounds)
                            {
                                heightSize = resolveAdjustedSize(newHeight, mMaxHeight,
                                        heightMeasureSpec);
                            }

                            if (newHeight <= heightSize)
                            {
                                heightSize = newHeight;
                            }
                        }
                    }
                }
            }
            else
            {
                /* We are either don't want to preserve the drawables aspect ratio,
                   or we are not allowed to change view dimensions. Just measure in
                   the normal way.
                */
                w += pleft + pright;
                h += ptop + pbottom;

                w = Math.Max(w, getSuggestedMinimumWidth());
                h = Math.Max(h, getSuggestedMinimumHeight());

                widthSize = resolveSizeAndState(w, widthMeasureSpec, 0);
                heightSize = resolveSizeAndState(h, heightMeasureSpec, 0);
            }

            setMeasuredDimension(widthSize, heightSize);
        }

        private int resolveAdjustedSize(int desiredSize, int maxSize,
                                       int measureSpec)
        {
            int result = desiredSize;
            int specMode = MeasureSpec.getMode(measureSpec);
            int specSize = MeasureSpec.getSize(measureSpec);
            switch (specMode)
            {
                case MeasureSpec.UNSPECIFIED:
                    /* Parent says we can be as big as we want. Just don't be larger
                       than max size imposed on ourselves.
                    */
                    result = Math.Min(desiredSize, maxSize);
                    break;
                case MeasureSpec.AT_MOST:
                    // Parent says we can be as big as we want, up to specSize.
                    // Don't be larger than specSize, and don't be larger than
                    // the max size imposed on ourselves.
                    result = Math.Min(Math.Min(desiredSize, specSize), maxSize);
                    break;
                case MeasureSpec.EXACTLY:
                    // No choice. Do what we are told.
                    result = specSize;
                    break;
            }
            return result;
        }

        override
            internal bool setFrame(int l, int t, int r, int b)
        {
            bool changed = base.setFrame(l, t, r, b);
            mHaveFrame = true;
            configureBounds();
            return changed;
        }

        private void configureBounds()
        {
            if (mDrawable == null || !mHaveFrame)
            {
                return;
            }

            int dwidth = mDrawableWidth;
            int dheight = mDrawableHeight;

            int vwidth = getWidth() - mPaddingLeft - mPaddingRight;
            int vheight = getHeight() - mPaddingTop - mPaddingBottom;

            bool fits = (dwidth < 0 || vwidth == dwidth)
                    && (dheight < 0 || vheight == dheight);

            if (dwidth <= 0 || dheight <= 0 || ScaleType.FIT_XY == mScaleType)
            {
                /* If the drawable has no intrinsic size, or we're told to
                    scaletofit, then we just fill our entire view.
                */
                mDrawable.setBounds(0, 0, vwidth, vheight);
                mDrawMatrix = null;
            }
            else
            {
                // We need to do the scaling ourself, so have the drawable
                // use its native size.
                mDrawable.setBounds(0, 0, dwidth, dheight);

                if (ScaleType.MATRIX == mScaleType)
                {
                    // Use the specified matrix as-is.
                    if (mMatrix.IsIdentity)
                    {
                        mDrawMatrix = null;
                    }
                    else
                    {
                        mDrawMatrix = mMatrix;
                    }
                }
                else if (fits)
                {
                    // The bitmap fits exactly, no transform needed.
                    mDrawMatrix = null;
                }
                else if (ScaleType.CENTER == mScaleType)
                {
                    // Center bitmap in view, no scaling.
                    mDrawMatrix = mMatrix;
                    mDrawMatrix.Value.SetTranslate(MathF.Round((vwidth - dwidth) * 0.5f),
                                             MathF.Round((vheight - dheight) * 0.5f));
                }
                else if (ScaleType.CENTER_CROP == mScaleType)
                {
                    mDrawMatrix = mMatrix;

                    float scale;
                    float dx = 0, dy = 0;

                    if (dwidth * vheight > vwidth * dheight)
                    {
                        scale = vheight / (float)dheight;
                        dx = (vwidth - dwidth * scale) * 0.5f;
                    }
                    else
                    {
                        scale = vwidth / (float)dwidth;
                        dy = (vheight - dheight * scale) * 0.5f;
                    }

                    mDrawMatrix.Value.SetScale(scale, scale);
                    mDrawMatrix.Value.PostTranslate(MathF.Round(dx), MathF.Round(dy));
                }
                else if (ScaleType.CENTER_INSIDE == mScaleType)
                {
                    mDrawMatrix = mMatrix;
                    float scale;
                    float dx;
                    float dy;

                    if (dwidth <= vwidth && dheight <= vheight)
                    {
                        scale = 1.0f;
                    }
                    else
                    {
                        scale = Math.Min(vwidth / (float)dwidth,
                                vheight / (float)dheight);
                    }

                    dx = (float)Math.Round((vwidth - dwidth * scale) * 0.5f);
                    dy = (float)Math.Round((vheight - dheight * scale) * 0.5f);

                    mDrawMatrix.Value.SetScale(scale, scale);
                    mDrawMatrix.Value.PostTranslate(dx, dy);
                }
                else
                {
                    // Generate the required transform.
                    mTempSrc.set(0, 0, dwidth, dheight);
                    mTempDst.set(0, 0, vwidth, vheight);

                    mDrawMatrix = mMatrix;
                    mDrawMatrix.Value.SetRectToRect(mTempSrc, mTempDst, scaleTypeToScaleToFit(mScaleType));
                }
            }
        }

        override
            protected void drawableStateChanged()
        {
            base.drawableStateChanged();

            Drawable drawable = mDrawable;
            if (drawable != null && drawable.isStateful()
                    && drawable.setState(getDrawableState()))
            {
                invalidateDrawable(drawable);
            }
        }

        override
            public void drawableHotspotChanged(float x, float y)
        {
            base.drawableHotspotChanged(x, y);

            if (mDrawable != null)
            {
                mDrawable.setHotspot(x, y);
            }
        }

        /**
         * Applies a temporary transformation {@link Matrix} to the view's drawable when it is drawn.
         * Allows custom scaling, translation, and perspective distortion during an animation.
         *
         * This method is a lightweight analogue of {@link ImageView#setImageMatrix(Matrix)} to use
         * only during animations as this matrix will be cleared after the next drawable
         * update or view's bounds change.
         *
         * @param matrix The transformation parameters in matrix form.
         */
        public void animateTransform(SKMatrix matrix)
        {
            if (mDrawable == null)
            {
                return;
            }
            if (matrix == null)
            {
                int vwidth = getWidth() - mPaddingLeft - mPaddingRight;
                int vheight = getHeight() - mPaddingTop - mPaddingBottom;
                mDrawable.setBounds(0, 0, vwidth, vheight);
                mDrawMatrix = null;
            }
            else
            {
                mDrawable.setBounds(0, 0, mDrawableWidth, mDrawableHeight);
                if (mDrawMatrix == null)
                {
                    mDrawMatrix = SKMatrix.Identity;
                }
                mDrawMatrix = matrix;
            }
            invalidate();
        }

        override
            protected void onDraw(SKCanvas canvas)
        {
            base.onDraw(canvas);

            if (mDrawable == null)
            {
                return; // couldn't resolve the URI
            }

            if (mDrawableWidth == 0 || mDrawableHeight == 0)
            {
                return;     // nothing to draw (empty bounds)
            }

            if (mDrawMatrix == null && mPaddingTop == 0 && mPaddingLeft == 0)
            {
                mDrawable.draw(canvas);
            }
            else
            {
                int saveCount = canvas.SaveCount;
                canvas.Save();

                if (mCropToPadding)
                {
                    int scrollX = mScrollX;
                    int scrollY = mScrollY;
                    canvas.ClipRect(new(scrollX + mPaddingLeft, scrollY + mPaddingTop,
                            scrollX + mRight - mLeft - mPaddingRight,
                            scrollY + mBottom - mTop - mPaddingBottom));
                }

                canvas.Translate(mPaddingLeft, mPaddingTop);

                if (mDrawMatrix != null)
                {
                    canvas.Concat(ref mDrawMatrix.Value);
                }
                mDrawable.draw(canvas);
                canvas.RestoreToCount(saveCount);
            }
        }

        /**
         * <p>Return the offset of the widget's text baseline from the widget's top
         * boundary. </p>
         *
         * @return the offset of the baseline within the widget's bounds or -1
         *         if baseline alignment is not supported.
         */
        override
            public int getBaseline()
        {
            if (mBaselineAlignBottom)
            {
                return getMeasuredHeight();
            }
            else
            {
                return mBaseline;
            }
        }

        /**
         * <p>Set the offset of the widget's text baseline from the widget's top
         * boundary.  This value is overridden by the {@link #setBaselineAlignBottom(bool)}
         * property.</p>
         *
         * @param baseline The baseline to use, or -1 if none is to be provided.
         *
         * @see #setBaseline(int)
         * @attr ref android.R.styleable#ImageView_baseline
         */
        public void setBaseline(int baseline)
        {
            if (mBaseline != baseline)
            {
                mBaseline = baseline;
                requestLayout();
            }
        }

        /**
         * Sets whether the baseline of this view to the bottom of the view.
         * Setting this value overrides any calls to setBaseline.
         *
         * @param aligned If true, the image view will be baseline aligned by its bottom edge.
         *
         * @attr ref android.R.styleable#ImageView_baselineAlignBottom
         */
        public void setBaselineAlignBottom(bool aligned)
        {
            if (mBaselineAlignBottom != aligned)
            {
                mBaselineAlignBottom = aligned;
                requestLayout();
            }
        }

        /**
         * Checks whether this view's baseline is considered the bottom of the view.
         *
         * @return True if the ImageView's baseline is considered the bottom of the view, false if otherwise.
         * @see #setBaselineAlignBottom(bool)
         */
        public bool getBaselineAlignBottom()
        {
            return mBaselineAlignBottom;
        }

        /**
         * Sets a tinting option for the image.
         *
         * @param color Color tint to apply.
         * @param mode How to apply the color.  The standard mode is
         * {@link PorterDuff.Mode#SRC_ATOP}
         *
         * @attr ref android.R.styleable#ImageView_tint
         */
        public void setColorFilter(int color, PorterDuff.Mode mode)
        {
            setColorFilter(new PorterDuffColorFilter(color, mode));
        }

        /**
         * Set a tinting option for the image. Assumes
         * {@link PorterDuff.Mode#SRC_ATOP} blending mode.
         *
         * @param color Color tint to apply.
         * @attr ref android.R.styleable#ImageView_tint
         */
        public void setColorFilter(int color)
        {
            setColorFilter(color, PorterDuff.Mode.SRC_ATOP);
        }

        /**
         * Removes the image's {@link android.graphics.ColorFilter}.
         *
         * @see #setColorFilter(int)
         * @see #getColorFilter()
         */
        public void clearColorFilter()
        {
            setColorFilter(null);
        }

        /**
         * @hide Candidate for future API inclusion
         */
        internal void setXfermode(Xfermode mode)
        {
            if (mXfermode != mode)
            {
                mXfermode = mode;
                mHasXfermode = true;
                applyXfermode();
                invalidate();
            }
        }

        /**
         * Returns the active color filter for this ImageView.
         *
         * @return the active color filter for this ImageView
         *
         * @see #setColorFilter(android.graphics.ColorFilter)
         */
        public ColorFilter getColorFilter()
        {
            return mColorFilter;
        }

        /**
         * Apply an arbitrary colorfilter to the image.
         *
         * @param cf the colorfilter to apply (may be null)
         *
         * @see #getColorFilter()
         */
        public void setColorFilter(ColorFilter cf)
        {
            if (mColorFilter != cf)
            {
                mColorFilter = cf;
                mHasColorFilter = true;
                applyColorFilter();
                invalidate();
            }
        }

        /**
         * Returns the alpha that will be applied to the drawable of this ImageView.
         *
         * @return the alpha value that will be applied to the drawable of this
         * ImageView (between 0 and 255 inclusive, with 0 being transparent and
         * 255 being opaque)
         *
         * @see #setImageAlpha(int)
         */
        public int getImageAlpha()
        {
            return mAlpha;
        }

        /**
         * Sets the alpha value that should be applied to the image.
         *
         * @param alpha the alpha value that should be applied to the image (between
         * 0 and 255 inclusive, with 0 being transparent and 255 being opaque)
         *
         * @see #getImageAlpha()
         */
        public void setImageAlpha(int alpha)
        {
            setAlpha(alpha);
        }

        /**
         * Sets the alpha value that should be applied to the image.
         *
         * @param alpha the alpha value that should be applied to the image
         *
         * @deprecated use #setImageAlpha(int) instead
         */
        internal void setAlpha(int alpha)
        {
            alpha &= 0xFF;          // keep it legal
            if (mAlpha != alpha)
            {
                mAlpha = alpha;
                mHasAlpha = true;
                applyAlpha();
                invalidate();
            }
        }

        private void applyXfermode()
        {
            if (mDrawable != null && mHasXfermode)
            {
                mDrawable = mDrawable.mutate();
                mDrawable.setXfermode(mXfermode);
            }
        }

        private void applyColorFilter()
        {
            if (mDrawable != null && mHasColorFilter)
            {
                mDrawable = mDrawable.mutate();
                mDrawable.setColorFilter(mColorFilter);
            }
        }

        private void applyAlpha()
        {
            if (mDrawable != null && mHasAlpha)
            {
                mDrawable = mDrawable.mutate();
                mDrawable.setAlpha(mAlpha * mViewAlphaScale >> 8);
            }
        }

        override
            public bool isOpaque()
        {
            return base.isOpaque() || mDrawable != null && mXfermode == null
                    && mDrawable.getOpacity() == PixelFormat.OPAQUE
                    && mAlpha * mViewAlphaScale >> 8 == 255
                    && isFilledByImage();
        }

        private bool isFilledByImage()
        {
            if (mDrawable == null)
            {
                return false;
            }

            Rect bounds = mDrawable.getBounds();
            if (mDrawMatrix == null)
            {
                return bounds.left <= 0 && bounds.top <= 0 && bounds.right >= getWidth()
                        && bounds.bottom >= getHeight();
            }
            SKMatrix matrix = mDrawMatrix;
            if (matrix.RectStaysRect)
            {
                RectF boundsSrc = mTempSrc;
                RectF boundsDst = mTempDst;
                boundsSrc.set(bounds);
                boundsDst = matrix.MapRect(boundsSrc);
                return boundsDst.left <= 0 && boundsDst.top <= 0 && boundsDst.right >= getWidth()
                        && boundsDst.bottom >= getHeight();
            }
            else
            {
                // If the matrix doesn't map to a rectangle, assume the worst.
                return false;
            }
        }

        override
            public void onVisibilityAggregated(bool isVisible)
        {
            base.onVisibilityAggregated(isVisible);
            // Only do this for new apps post-Nougat
            if (mDrawable != null && !sCompatDrawableVisibilityDispatch)
            {
                mDrawable.setVisible(isVisible, false);
            }
        }

        override
            public void setVisibility(int visibility)
        {
            base.setVisibility(visibility);
            // Only do this for old apps pre-Nougat; new apps use onVisibilityAggregated
            if (mDrawable != null && sCompatDrawableVisibilityDispatch)
            {
                mDrawable.setVisible(visibility == VISIBLE, false);
            }
        }

        override
            protected void onAttachedToWindow()
        {
            base.onAttachedToWindow();
            // Only do this for old apps pre-Nougat; new apps use onVisibilityAggregated
            if (mDrawable != null && sCompatDrawableVisibilityDispatch)
            {
                mDrawable.setVisible(getVisibility() == VISIBLE, false);
            }
        }

        override
            protected void onDetachedFromWindow()
        {
            base.onDetachedFromWindow();
            // Only do this for old apps pre-Nougat; new apps use onVisibilityAggregated
            if (mDrawable != null && sCompatDrawableVisibilityDispatch)
            {
                mDrawable.setVisible(false, false);
            }
        }
    }
}
