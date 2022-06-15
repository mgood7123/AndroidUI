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

namespace AndroidUI
{
    using Extensions;
    using SkiaSharp;

    /**
    * The Paint class holds the style and color information about how to draw
    * geometries, text and bitmaps.
    */
    public class Paint
    {

        private SKPaint mNativePaint;
        private SKShader mNativeShader;
        private SKColorFilter mNativeColorFilter;

        private long mColor;
        private ColorFilter mColorFilter;
        private MaskFilter mMaskFilter;
        private PathEffect mPathEffect;
        private Shader mShader;
        private Xfermode mXfermode;

        private bool mHasCompatScaling;
        private float mCompatScaling;
        private float mInvCompatScaling;

        private float mShadowLayerRadius;
        private float mShadowLayerDx;
        private float mShadowLayerDy;
        private long mShadowLayerColor;

        static readonly Style[] sStyleArray = {
            new Style(Style.FILL), new Style(Style.STROKE), new Style(Style.FILL_AND_STROKE)
        };
        static readonly Cap[] sCapArray = {
            new Cap(Cap.BUTT), new Cap(Cap.ROUND), new Cap(Cap.SQUARE)
        };
        static readonly Join[] sJoinArray = {
            new Join(Join.MITER), new Join(Join.ROUND), new Join(Join.BEVEL)
        };

        /**
         * Paint flag that enables antialiasing when drawing.
         *
         * <p>Enabling this flag will cause all draw operations that support
         * antialiasing to use it.</p>
         *
         * @see #Paint(int)
         * @see #setFlags(int)
         */
        public const int ANTI_ALIAS_FLAG = 0x01;
        /**
         * Paint flag that enables bilinear sampling on scaled bitmaps.
         *
         * <p>If cleared, scaled bitmaps will be drawn with nearest neighbor
         * sampling, likely resulting in artifacts. This should generally be on
         * when drawing bitmaps, unless performance-bound (rendering to software
         * canvas) or preferring pixelation artifacts to blurriness when scaling
         * significantly.</p>
         *
         * <p>If bitmaps are scaled for device density at creation time (as
         * resource bitmaps often are) the filtering will already have been
         * done.</p>
         *
         * <p>On devices running {@link Build.VERSION_CODES#O} and below, hardware
         * accelerated drawing always uses bilinear sampling on scaled bitmaps,
         * regardless of this flag. On devices running {@link Build.VERSION_CODES#Q}
         * and above, this flag defaults to being set on a new {@code Paint}. It can
         * be cleared with {@link #setFlags} or {@link #setFilterBitmap}.</p>
         *
         * @see #Paint()
         * @see #Paint(int)
         * @see #setFlags(int)
         */
        public const int FILTER_BITMAP_FLAG = 0x02;
        /**
         * Paint flag that enables dithering when blitting.
         *
         * <p>Enabling this flag applies a dither to any blit operation where the
         * target's colour space is more constrained than the source.
         *
         * @see #Paint(int)
         * @see #setFlags(int)
         */
        public const int DITHER_FLAG = 0x04;

        // These flags are always set on a new/reset paint, even if flags 0 is passed.
        const int HIDDEN_DEFAULT_PAINT_FLAGS = FILTER_BITMAP_FLAG;


        /**
         * The Style specifies if the primitive being drawn is filled, stroked, or
         * both (in the same color). The default is FILL.
         */
        public class Style
        {
            /**
             * Geometry and text drawn with this style will be filled, ignoring all
             * stroke-related settings in the paint.
             */
            public const int FILL = 0;
            /**
             * Geometry and text drawn with this style will be stroked, respecting
             * the stroke-related fields on the paint.
             */
            public const int STROKE = 1;
            /**
             * Geometry and text drawn with this style will be both filled and
             * stroked at the same time, respecting the stroke-related fields on
             * the paint. This mode can give unexpected results if the geometry
             * is oriented counter-clockwise. This restriction does not apply to
             * either FILL or STROKE.
             */
            public const int FILL_AND_STROKE = 2;

            internal Style(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }
            internal int nativeInt;

            public static implicit operator SKPaintStyle(Style value) => value.ToSKPaintStyle();

            public SKPaintStyle ToSKPaintStyle()
            {
                switch (nativeInt)
                {
                    case FILL:
                        return SKPaintStyle.Fill;
                    case STROKE:
                        return SKPaintStyle.Stroke;
                    case FILL_AND_STROKE:
                        return SKPaintStyle.StrokeAndFill;
                    default:
                        throw new Exceptions.IllegalStateException();
                }
            }

        }

        /**
         * The Cap specifies the treatment for the beginning and ending of
         * stroked lines and paths. The default is BUTT.
         */
        public class Cap
        {
            /**
             * The stroke ends with the path, and does not project beyond it.
             */
            public const int BUTT = 0;
            /**
             * The stroke projects out as a semicircle, with the center at the
             * end of the path.
             */
            public const int ROUND = 1;
            /**
             * The stroke projects out as a square, with the center at the end
             * of the path.
             */
            public const int SQUARE = 2;

            internal Cap(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }
            internal int nativeInt;
            public static implicit operator SKStrokeCap(Cap value) => value.ToSKStrokeCap();

            public SKStrokeCap ToSKStrokeCap()
            {
                switch (nativeInt)
                {
                    case BUTT:
                        return SKStrokeCap.Butt;
                    case ROUND:
                        return SKStrokeCap.Round;
                    case SQUARE:
                        return SKStrokeCap.Square;
                    default:
                        throw new Exceptions.IllegalStateException();
                }
            }
        }

        /**
         * The Join specifies the treatment where lines and curve segments
         * join on a stroked path. The default is MITER.
         */
        public class Join
        {
            /**
             * The outer edges of a join meet at a sharp angle
             */
            public const int MITER = 0;
            /**
             * The outer edges of a join meet in a circular arc.
             */
            public const int ROUND = 1;
            /**
             * The outer edges of a join meet with a straight line
             */
            public const int BEVEL = 2;

            internal Join(int nativeInt)
            {
                this.nativeInt = nativeInt;
            }
            internal int nativeInt;
            public static implicit operator SKStrokeJoin(Join value) => value.ToSKStrokeJoin();

            public SKStrokeJoin ToSKStrokeJoin()
            {
                switch (nativeInt)
                {
                    case MITER:
                        return SKStrokeJoin.Miter;
                    case ROUND:
                        return SKStrokeJoin.Round;
                    case BEVEL:
                        return SKStrokeJoin.Bevel;
                    default:
                        throw new Exceptions.IllegalStateException();
                }
            }
        }

        /**
         * Create a new paint with default settings.
         *
         * <p>On devices running {@link Build.VERSION_CODES#O} and below, hardware
         * accelerated drawing always acts as if {@link #FILTER_BITMAP_FLAG} is set.
         * On devices running {@link Build.VERSION_CODES#Q} and above,
         * {@code FILTER_BITMAP_FLAG} is set by this constructor, and it can be
         * cleared with {@link #setFlags} or {@link #setFilterBitmap}.
         * On devices running {@link Build.VERSION_CODES#S} and above, {@code ANTI_ALIAS_FLAG}
         * is set by this constructor, and it can be cleared with {@link #setFlags} or
         * {@link #setAntiAlias}.</p>
         */
        public Paint() : this(ANTI_ALIAS_FLAG)
        {
        }

        /**
         * Create a new paint with the specified flags. Use setFlags() to change
         * these after the paint is created.
         *
         * <p>On devices running {@link Build.VERSION_CODES#O} and below, hardware
         * accelerated drawing always acts as if {@link #FILTER_BITMAP_FLAG} is set.
         * On devices running {@link Build.VERSION_CODES#Q} and above,
         * {@code FILTER_BITMAP_FLAG} is always set by this constructor, regardless
         * of the value of {@code flags}. It can be cleared with {@link #setFlags} or
         * {@link #setFilterBitmap}.</p>
         *
         * @param flags initial flag bits, as if they were passed via setFlags().
         */
        public Paint(int flags)
        {
            mNativePaint = new SKPaint();
            setFlags(flags | HIDDEN_DEFAULT_PAINT_FLAGS);
            // TODO: Turning off hinting has undesirable side effects, we need to
            //       revisit hinting once we add support for subpixel positioning
            // setHinting(DisplayMetrics.DENSITY_DEVICE >= DisplayMetrics.DENSITY_TV
            //        ? HINTING_OFF : HINTING_ON);
            mCompatScaling = mInvCompatScaling = 1;
            mColor = Color.pack(Color.BLACK);
        }

        /**
         * Create a new paint, initialized with the attributes in the specified
         * paint parameter.
         *
         * @param paint Existing paint used to initialized the attributes of the
         *              new paint.
         */
        public Paint(Paint paint)
        {
            mNativePaint = paint.mNativePaint.Clone();
            setClassVariablesFrom(paint);
        }

        /** Restores the paint to its default settings. */
        public void reset()
        {
            mNativePaint.Reset();
            setFlags(HIDDEN_DEFAULT_PAINT_FLAGS | ANTI_ALIAS_FLAG);

            // TODO: Turning off hinting has undesirable side effects, we need to
            //       revisit hinting once we add support for subpixel positioning
            // setHinting(DisplayMetrics.DENSITY_DEVICE >= DisplayMetrics.DENSITY_TV
            //        ? HINTING_OFF : HINTING_ON);

            mColor = Color.pack(Color.BLACK);
            mColorFilter = null;
            mMaskFilter = null;
            mPathEffect = null;
            mShader = null;
            mNativeShader = null;
            mXfermode = null;

            mHasCompatScaling = false;
            mCompatScaling = 1;
            mInvCompatScaling = 1;

            mShadowLayerRadius = 0.0f;
            mShadowLayerDx = 0.0f;
            mShadowLayerDy = 0.0f;
            mShadowLayerColor = Color.pack(0);
        }

        /**
         * Copy the fields from src into this paint. This is equivalent to calling
         * get() on all of the src fields, and calling the corresponding set()
         * methods on this.
         */
        public void set(Paint src)
        {
            if (this != src)
            {
                // copy over the native settings
                mNativePaint = src.mNativePaint.Clone();
                setClassVariablesFrom(src);
            }
        }

        /**
         * Set all class variables using current values from the given
         * {@link Paint}.
         */
        private void setClassVariablesFrom(Paint paint)
        {
            mColor = paint.mColor;
            mColorFilter = paint.mColorFilter;
            mMaskFilter = paint.mMaskFilter;
            mPathEffect = paint.mPathEffect;
            mShader = paint.mShader;
            mNativeShader = paint.mNativeShader;
            mXfermode = paint.mXfermode;

            mHasCompatScaling = paint.mHasCompatScaling;
            mCompatScaling = paint.mCompatScaling;
            mInvCompatScaling = paint.mInvCompatScaling;

            mShadowLayerRadius = paint.mShadowLayerRadius;
            mShadowLayerDx = paint.mShadowLayerDx;
            mShadowLayerDy = paint.mShadowLayerDy;
            mShadowLayerColor = paint.mShadowLayerColor;
        }

        /** @hide */
        public void setCompatibilityScaling(float factor)
        {
            if (factor == 1.0)
            {
                mHasCompatScaling = false;
                mCompatScaling = mInvCompatScaling = 1.0f;
            }
            else
            {
                mHasCompatScaling = true;
                mCompatScaling = factor;
                mInvCompatScaling = 1.0f / factor;
            }
        }

        static readonly object LOCK = new object();

        /**
         * Return the pointer to the native object while ensuring that any
         * mutable objects that are attached to the paint are also up-to-date.
         *
         * Note: Although this method is |synchronized|, this is simply so it
         * is not thread-hostile to multiple threads calling this method. It
         * is still unsafe to attempt to change the Shader/ColorFilter while
         * another thread attempts to access the native object.
         *
         * @hide
         */
        public SKPaint getNativeInstance()
        {
            lock (LOCK)
            {
                bool filter = isFilterBitmap();
                SKShader newNativeShader = mShader?.getNativeInstance(filter);
                if (newNativeShader != mNativeShader)
                {
                    mNativeShader = newNativeShader;
                    mNativePaint.Shader = mNativeShader;
                }
                SKColorFilter newNativeColorFilter = mColorFilter?.getNativeInstance();
                if (newNativeColorFilter != mNativeColorFilter)
                {
                    mNativeColorFilter = newNativeColorFilter;
                    mNativePaint.ColorFilter = mNativeColorFilter;
                }
                return mNativePaint;
            }
        }

        /**
         * Return the paint's flags. Use the Flag enum to test flag values.
         *
         * @return the paint's flags (see enums ending in _Flag for bit masks)
         */
        public int getFlags()
        {
            return (int)mNativePaint.ExtensionProperties_GetValue("FLAGS", 0);
        }

        /**
         * Set the paint's flags. Use the Flag enum to specific flag values.
         *
         * @param flags The new flag bits for the paint
         */
        public void setFlags(int flags)
        {
            mNativePaint.ExtensionProperties_SetValue("FLAGS", flags);
        }

        /**
         * Return the paint's hinting mode.  Returns either
         * {@link #HINTING_OFF} or {@link #HINTING_ON}.
         */
        public int getHinting()
        {
            return mNativePaint.IsAutohinted.toInt();
        }

        /**
         * Set the paint's hinting mode.  May be either
         * {@link #HINTING_OFF} or {@link #HINTING_ON}.
         */
        public void setHinting(int mode)
        {
            mNativePaint.IsAutohinted = mode.toBool();
        }

        /**
         * Helper for getFlags(), returning true if ANTI_ALIAS_FLAG bit is set
         * AntiAliasing smooths out the edges of what is being drawn, but is has
         * no impact on the interior of the shape. See setDither() and
         * setFilterBitmap() to affect how colors are treated.
         *
         * @return true if the antialias bit is set in the paint's flags.
         */
        public bool isAntiAlias()
        {
            return (getFlags() & DITHER_FLAG) != 0;
        }

        /**
         * Helper for setFlags(), setting or clearing the ANTI_ALIAS_FLAG bit
         * AntiAliasing smooths out the edges of what is being drawn, but is has
         * no impact on the interior of the shape. See setDither() and
         * setFilterBitmap() to affect how colors are treated.
         *
         * @param aa true to set the antialias bit in the flags, false to clear it
         */
        public void setAntiAlias(bool aa)
        {
            mNativePaint.IsAntialias = aa;
        }

        /**
         * Helper for getFlags(), returning true if DITHER_FLAG bit is set
         * Dithering affects how colors that are higher precision than the device
         * are down-sampled. No dithering is generally faster, but higher precision
         * colors are just truncated down (e.g. 8888 -> 565). Dithering tries to
         * distribute the error inherent in this process, to reduce the visual
         * artifacts.
         *
         * @return true if the dithering bit is set in the paint's flags.
         */
        public bool isDither()
        {
            return (getFlags() & DITHER_FLAG) != 0;
        }

        /**
         * Helper for setFlags(), setting or clearing the DITHER_FLAG bit
         * Dithering affects how colors that are higher precision than the device
         * are down-sampled. No dithering is generally faster, but higher precision
         * colors are just truncated down (e.g. 8888 -> 565). Dithering tries to
         * distribute the error inherent in this process, to reduce the visual
         * artifacts.
         *
         * @param dither true to set the dithering bit in flags, false to clear it
         */
        public void setDither(bool dither)
        {
            mNativePaint.IsAntialias = dither;
        }

        /**
         * Whether or not the bitmap filter is activated.
         * Filtering affects the sampling of bitmaps when they are transformed.
         * Filtering does not affect how the colors in the bitmap are converted into
         * device pixels. That is dependent on dithering and xfermodes.
         *
         * @see #setFilterBitmap(bool) setFilterBitmap()
         * @see #FILTER_BITMAP_FLAG
         */
        public bool isFilterBitmap()
        {
            return (getFlags() & FILTER_BITMAP_FLAG) != 0;
        }

        /**
         * Helper for setFlags(), setting or clearing the FILTER_BITMAP_FLAG bit.
         * Filtering affects the sampling of bitmaps when they are transformed.
         * Filtering does not affect how the colors in the bitmap are converted into
         * device pixels. That is dependent on dithering and xfermodes.
         *
         * @param filter true to set the FILTER_BITMAP_FLAG bit in the paint's
         *               flags, false to clear it.
         * @see #FILTER_BITMAP_FLAG
         */
        public void setFilterBitmap(bool filter)
        {
            mNativePaint.FilterQuality = filter ? SKFilterQuality.Low : SKFilterQuality.None;
        }

        /**
         * Return the paint's style, used for controlling how primitives'
         * geometries are interpreted (except for drawBitmap, which always assumes
         * FILL_STYLE).
         *
         * @return the paint's style setting (Fill, Stroke, StrokeAndFill)
         */
        public Style getStyle()
        {
            return sStyleArray[(int)mNativePaint.Style];
        }

        /**
         * Set the paint's style, used for controlling how primitives'
         * geometries are interpreted (except for drawBitmap, which always assumes
         * Fill).
         *
         * @param style The new style to set in the paint
         */
        public void setStyle(Style style)
        {
            mNativePaint.Style = (SKPaintStyle)style.nativeInt;
        }

        /**
         * Return the paint's color in sRGB. Note that the color is a 32bit value
         * containing alpha as well as r,g,b. This 32bit value is not premultiplied,
         * meaning that its alpha can be any value, regardless of the values of
         * r,g,b. See the Color class for more details.
         *
         * @return the paint's color (and alpha).
         */
        public int getColor()
        {
            return Color.toArgb(mColor);
        }

        /**
         * Return the paint's color. Note that the color is a long with an encoded
         * {@link ColorSpace} as well as alpha and r,g,b. These values are not
         * premultiplied, meaning that alpha can be any value, regardless of the
         * values of r,g,b. See the {@link Color} class for more details.
         *
         * @return the paint's color, alpha, and {@code ColorSpace} encoded as a
         *      {@code ColorLong}
         */
        public long getColorLong()
        {
            return mColor;
        }

        /**
         * Set the paint's color. Note that the color is an int containing alpha
         * as well as r,g,b. This 32bit value is not premultiplied, meaning that
         * its alpha can be any value, regardless of the values of r,g,b.
         * See the Color class for more details.
         *
         * @param color The new color (including alpha) to set in the paint.
         */
        public void setColor(int color)
        {
            mNativePaint.Color = color.ToSKColor();
            mColor = Color.pack(color);
        }

        /**
         * Set the paint's color with a {@code ColorLong}. Note that the color is
         * a long with an encoded {@link ColorSpace} as well as alpha and r,g,b.
         * These values are not premultiplied, meaning that alpha can be any value,
         * regardless of the values of r,g,b. See the {@link Color} class for more
         * details.
         *
         * @param color The new color (including alpha and {@link ColorSpace})
         *      to set in the paint.
         * @throws IllegalArgumentException if the color space encoded in the
         *      {@code ColorLong} is invalid or unknown.
         */
        public void setColor(long color)
        {
            ColorSpace cs = Color.colorSpace(color);

            mNativePaint.SetColor(color.ToSKColorF(), cs.getNativeInstance());

            mColor = color;
        }

        /**
         * Helper to getColor() that just returns the color's alpha value. This is
         * the same as calling getColor() >>> 24. It always returns a value between
         * 0 (completely transparent) and 255 (completely opaque).
         *
         * @return the alpha component of the paint's color.
         */
        public int getAlpha()
        {
            return (int)Math.Round(Color.alpha(mColor) * 255.0f);
        }

        /**
         * Helper to setColor(), that only assigns the color's alpha value,
         * leaving its r,g,b values unchanged. Results are undefined if the alpha
         * value is outside of the range [0..255]
         *
         * @param a set the alpha component [0..255] of the paint's color.
         */
        public void setAlpha(int a)
        {
            // FIXME: No need to unpack this. Instead, just update the alpha bits.
            // b/122959599
            ColorSpace cs = Color.colorSpace(mColor);
            float r = Color.red(mColor);
            float g = Color.green(mColor);
            float b = Color.blue(mColor);
            mColor = Color.pack(r, g, b, a * (1.0f / 255), cs);
            mNativePaint.SetColor(mColor.ToSKColorF(), cs.getNativeInstance());
        }

        /**
         * Helper to setColor(), that takes a,r,g,b and constructs the color int
         *
         * @param a The new alpha component (0..255) of the paint's color.
         * @param r The new red component (0..255) of the paint's color.
         * @param g The new green component (0..255) of the paint's color.
         * @param b The new blue component (0..255) of the paint's color.
         */
        public void setARGB(int a, int r, int g, int b)
        {
            setColor((a << 24) | (r << 16) | (g << 8) | b);
        }

        /**
         * Return the width for stroking.
         * <p />
         * A value of 0 strokes in hairline mode.
         * Hairlines always draws a single pixel independent of the canvas's matrix.
         *
         * @return the paint's stroke width, used whenever the paint's style is
         *         Stroke or StrokeAndFill.
         */
        public float getStrokeWidth()
        {
            return mNativePaint.StrokeWidth;
        }

        /**
         * Set the width for stroking.
         * Pass 0 to stroke in hairline mode.
         * Hairlines always draws a single pixel independent of the canvas's matrix.
         *
         * @param width set the paint's stroke width, used whenever the paint's
         *              style is Stroke or StrokeAndFill.
         */
        public void setStrokeWidth(float width)
        {
            mNativePaint.StrokeWidth = width;
        }

        /**
         * Return the paint's stroke miter value. Used to control the behavior
         * of miter joins when the joins angle is sharp.
         *
         * @return the paint's miter limit, used whenever the paint's style is
         *         Stroke or StrokeAndFill.
         */
        public float getStrokeMiter()
        {
            return mNativePaint.StrokeMiter;
        }

        /**
         * Set the paint's stroke miter value. This is used to control the behavior
         * of miter joins when the joins angle is sharp. This value must be >= 0.
         *
         * @param miter set the miter limit on the paint, used whenever the paint's
         *              style is Stroke or StrokeAndFill.
         */
        public void setStrokeMiter(float miter)
        {
            mNativePaint.StrokeMiter = miter;
        }

        /**
         * Return the paint's Cap, controlling how the start and end of stroked
         * lines and paths are treated.
         *
         * @return the line cap style for the paint, used whenever the paint's
         *         style is Stroke or StrokeAndFill.
         */
        public Cap getStrokeCap()
        {
            return sCapArray[(int)mNativePaint.StrokeCap];
        }

        /**
         * Set the paint's Cap.
         *
         * @param cap set the paint's line cap style, used whenever the paint's
         *            style is Stroke or StrokeAndFill.
         */
        public void setStrokeCap(Cap cap)
        {
            mNativePaint.StrokeCap = (SKStrokeCap)cap.nativeInt;
        }

        /**
         * Return the paint's stroke join type.
         *
         * @return the paint's Join.
         */
        public Join getStrokeJoin()
        {
            return sJoinArray[(int)mNativePaint.StrokeJoin];
        }

        /**
         * Set the paint's Join.
         *
         * @param join set the paint's Join, used whenever the paint's style is
         *             Stroke or StrokeAndFill.
         */
        public void setStrokeJoin(Join join)
        {
            mNativePaint.StrokeJoin = (SKStrokeJoin)join.nativeInt;
        }

        /**
         * Applies any/all effects (patheffect, stroking) to src, returning the
         * result in dst. The result is that drawing src with this paint will be
         * the same as drawing dst with a default paint (at least from the
         * geometric perspective).
         *
         * @param src input path
         * @param dst output path (may be the same as src)
         * @return    true if the path should be filled, or false if it should be
         *                 drawn with a hairline (width == 0)
         */
        public bool getFillPath(SKPath src, SKPath dst)
        {
            return mNativePaint.GetFillPath(src, dst);
        }

        /**
         * Get the paint's shader object.
         *
         * @return the paint's shader (or null)
         */
        public Shader getShader()
        {
            return mShader;
        }

        /**
         * Set or clear the shader object.
         * <p />
         * Pass null to clear any previous shader.
         * As a convenience, the parameter passed is also returned.
         *
         * @param shader May be null. the new shader to be installed in the paint
         * @return       shader
         */
        public Shader setShader(Shader shader)
        {
            // If mShader changes, cached value of native shader aren't valid, since
            // old shader's pointer may be reused by another shader allocation later
            if (mShader != shader)
            {
                mNativeShader = null;
                // Release any native references to the old shader content
                mNativePaint.Shader = null;
            }
            // Defer setting the shader natively until getNativeInstance() is called
            mShader = shader;
            return shader;
        }

        /**
         * Get the paint's colorfilter (maybe be null).
         *
         * @return the paint's colorfilter (maybe be null)
         */
        public ColorFilter getColorFilter()
        {
            return mColorFilter;
        }

        /**
         * Set or clear the paint's colorfilter, returning the parameter.
         *
         * @param filter May be null. The new filter to be installed in the paint
         * @return       filter
         */
        public ColorFilter setColorFilter(ColorFilter filter)
        {
            // If mColorFilter changes, cached value of native shader aren't valid, since
            // old shader's pointer may be reused by another shader allocation later
            if (mColorFilter != filter)
            {
                mNativeColorFilter = null;
            }

            // Defer setting the filter natively until getNativeInstance() is called
            mColorFilter = filter;
            return filter;
        }

        /**
         * Get the paint's transfer mode object.
         *
         * @return the paint's transfer mode (or null)
         */
        public Xfermode getXfermode()
        {
            return mXfermode;
        }

        /**
         * Get the paint's blend mode object.
         *
         * @return the paint's blend mode (or null)
         */
        public BlendMode getBlendMode()
        {
            if (mXfermode == null)
            {
                return null;
            }
            else
            {
                return BlendMode.fromValue(mXfermode.porterDuffMode);
            }
        }

        /**
         * Set or clear the transfer mode object. A transfer mode defines how
         * source pixels (generate by a drawing command) are composited with
         * the destination pixels (content of the render target).
         * <p />
         * Pass null to clear any previous transfer mode.
         * As a convenience, the parameter passed is also returned.
         * <p />
         * {@link PorterDuffXfermode} is the most common transfer mode.
         *
         * @param xfermode May be null. The xfermode to be installed in the paint
         * @return         xfermode
         */
        public Xfermode setXfermode(Xfermode xfermode)
        {
            return installXfermode(xfermode);
        }

        private Xfermode installXfermode(Xfermode xfermode)
        {
            int newMode = xfermode != null ? xfermode.porterDuffMode : Xfermode.DEFAULT;
            int curMode = mXfermode != null ? mXfermode.porterDuffMode : Xfermode.DEFAULT;
            if (newMode != curMode)
            {
                mNativePaint.BlendMode = (SKBlendMode)newMode;
            }
            mXfermode = xfermode;
            return xfermode;
        }

        /**
         * Set or clear the blend mode. A blend mode defines how source pixels
         * (generated by a drawing command) are composited with the destination pixels
         * (content of the render target).
         * <p />
         * Pass null to clear any previous blend mode.
         * <p />
         *
         * @see BlendMode
         *
         * @param blendmode May be null. The blend mode to be installed in the paint
         */
        public void setBlendMode(BlendMode blendmode)
        {
            installXfermode(blendmode != null ? blendmode.getXfermode() : null);
        }

        /**
         * Get the paint's patheffect object.
         *
         * @return the paint's patheffect (or null)
         */
        public PathEffect getPathEffect()
        {
            return mPathEffect;
        }

        /**
         * Set or clear the patheffect object.
         * <p />
         * Pass null to clear any previous patheffect.
         * As a convenience, the parameter passed is also returned.
         *
         * @param effect May be null. The patheffect to be installed in the paint
         * @return       effect
         */
        public PathEffect setPathEffect(PathEffect effect)
        {
            SKPathEffect effectNative = null;
            if (effect != null)
            {
                effectNative = effect.native_instance;

            }
            mNativePaint.PathEffect = effectNative;
            mPathEffect = effect;
            return effect;
        }

        /**
         * Get the paint's maskfilter object.
         *
         * @return the paint's maskfilter (or null)
         */
        public MaskFilter getMaskFilter()
        {
            return mMaskFilter;
        }

        /**
         * Set or clear the maskfilter object.
         * <p />
         * Pass null to clear any previous maskfilter.
         * As a convenience, the parameter passed is also returned.
         *
         * @param maskfilter May be null. The maskfilter to be installed in the
         *                   paint
         * @return           maskfilter
         */
        public MaskFilter setMaskFilter(MaskFilter maskfilter)
        {
            SKMaskFilter maskfilterNative = null;
            if (maskfilter != null)
            {
                maskfilterNative = maskfilter.native_instance;
            }
            mNativePaint.MaskFilter = maskfilterNative;
            mMaskFilter = maskfilter;
            return maskfilter;
        }

        /**
         * This draws a shadow layer below the main layer, with the specified
         * offset and color, and blur radius. If radius is 0, then the shadow
         * layer is removed.
         * <p>
         * Can be used to create a blurred shadow underneath text. Support for use
         * with other drawing operations is constrained to the software rendering
         * pipeline.
         * <p>
         * The alpha of the shadow will be the paint's alpha if the shadow color is
         * opaque, or the alpha from the shadow color if not.
         */
        public void setShadowLayer(float radius, float dx, float dy, int shadowColor)
        {
            setShadowLayer(radius, dx, dy, Color.pack(shadowColor));
        }

        /**
         * This draws a shadow layer below the main layer, with the specified
         * offset and color, and blur radius. If radius is 0, then the shadow
         * layer is removed.
         * <p>
         * Can be used to create a blurred shadow underneath text. Support for use
         * with other drawing operations is constrained to the software rendering
         * pipeline.
         * <p>
         * The alpha of the shadow will be the paint's alpha if the shadow color is
         * opaque, or the alpha from the shadow color if not.
         *
         * @throws IllegalArgumentException if the color space encoded in the
         *      {@code ColorLong} is invalid or unknown.
         */
        public void setShadowLayer(float radius, float dx, float dy, long shadowColor)
        {
            SKColorSpace cs = Color.colorSpace(shadowColor).getNativeInstance();

            SKColorF color = shadowColor.ToSKColorF();
            if (radius <= 0)
            {
                mNativePaint.ExtensionProperties_SetValue("LOOPER", null);
            }
            else
            {
                var sigma = SKMaskFilter.ConvertRadiusToSigma(radius);
                if (!cs.IsSrgb)
                {
                    SKPaint tmp = new();
                    tmp.SetColor(color, cs);  // converts color to sRGB
                    color = tmp.Color;
                    tmp.Dispose();
                }
                mNativePaint.SetColor(color);
                if (sigma > 0)
                {
                    mNativePaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, sigma, true);
                }
            }

            mShadowLayerRadius = radius;
            mShadowLayerDx = dx;
            mShadowLayerDy = dy;
            mShadowLayerColor = shadowColor;
        }

        /**
         * Clear the shadow layer.
         */
        public void clearShadowLayer()
        {
            setShadowLayer(0, 0, 0, 0);
        }

        /**
         * Checks if the paint has a shadow layer attached
         *
         * @return true if the paint has a shadow layer attached and false otherwise
         * @hide
         */
        public bool hasShadowLayer()
        {
            return (bool)mNativePaint.ExtensionProperties_GetValue("LOOPER", false);
        }

        /**
         * Returns the blur radius of the shadow layer.
         * @see #setShadowLayer(float,float,float,int)
         * @see #setShadowLayer(float,float,float,long)
         */
        public float getShadowLayerRadius()
        {
            return mShadowLayerRadius;
        }

        /**
         * Returns the x offset of the shadow layer.
         * @see #setShadowLayer(float,float,float,int)
         * @see #setShadowLayer(float,float,float,long)
         */
        public float getShadowLayerDx()
        {
            return mShadowLayerDx;
        }

        /**
         * Returns the y offset of the shadow layer.
         * @see #setShadowLayer(float,float,float,int)
         * @see #setShadowLayer(float,float,float,long)
         */
        public float getShadowLayerDy()
        {
            return mShadowLayerDy;
        }

        /**
         * Returns the color of the shadow layer.
         * @see #setShadowLayer(float,float,float,int)
         * @see #setShadowLayer(float,float,float,long)
         */
        public int getShadowLayerColor()
        {
            return Color.toArgb(mShadowLayerColor);
        }

        /**
         * Returns the color of the shadow layer.
         *
         * @return the shadow layer's color encoded as a {@link ColorLong}.
         * @see #setShadowLayer(float,float,float,int)
         * @see #setShadowLayer(float,float,float,long)
         * @see Color
         */
        public long getShadowLayerColorLong()
        {
            return mShadowLayerColor;
        }
    }
}