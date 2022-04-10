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

using AndroidUI.Exceptions;

namespace AndroidUI
{
    public sealed class Bitmap
    {
        private static readonly string TAG = "Bitmap";

        /**
         * Indicates that the bitmap was created for an unknown pixel density.
         *
         * @see Bitmap#getDensity()
         * @see Bitmap#setDensity(int)
         */
        public static int DENSITY_NONE = 0;

        // Estimated size of the Bitmap native allocation, not including
        // pixel data.
        private static long NATIVE_ALLOCATION_SIZE = 32;

        // Convenience for JNI access
        private SkiaSharp.SKBitmap mNativePtr;

        /**
         * Represents whether the Bitmap's content is requested to be pre-multiplied.
         * Note that isPremultiplied() does not directly return this value, because
         * isPremultiplied() may never return true for a 565 Bitmap or a bitmap
         * without alpha.
         *
         * setPremultiplied() does directly set the value so that setConfig() and
         * setPremultiplied() aren't order dependent, despite being setters.
         *
         * The native bitmap's premultiplication state is kept up to date by
         * pushing down this preference for every config change.
         */
        private bool mRequestPremultiplied;

        private byte[] mNinePatchChunk; // may be null
        private NinePatch.InsetStruct mNinePatchInsets; // may be null
        private int mWidth;
        private int mHeight;
        private bool mRecycled;

        private ColorSpace mColorSpace;

        /*package*/
        internal int mDensity = getDefaultDensity();

        private static volatile int sDefaultDensity = -1;

        /**
         * For backwards compatibility, allows the app layer to change the default
         * density when running old apps.
         * @hide
         */
        public static void setDefaultDensity(int density)
        {
            sDefaultDensity = density;
        }

        static int getDefaultDensity()
        {
            if (sDefaultDensity >= 0)
            {
                return sDefaultDensity;
            }
            sDefaultDensity = DensityManager.ScreenDpi;
            return sDefaultDensity;
        }

        /**
         * Private constructor that must receive an already allocated native bitmap
         * int (pointer).
         */
        // JNI now calls the version below this one. This is preserved due to UnsupportedAppUsage.
        Bitmap(SkiaSharp.SKBitmap nativeBitmap, int width, int height, int density,
                bool requestPremultiplied, byte[] ninePatchChunk,
                NinePatch.InsetStruct ninePatchInsets)
            : this(nativeBitmap, width, height, density, requestPremultiplied, ninePatchChunk,
                    ninePatchInsets, true)
        {
        }

        // called from JNI and Bitmap_Delegate.
        Bitmap(SkiaSharp.SKBitmap nativeBitmap, int width, int height, int density,
                bool requestPremultiplied, byte[] ninePatchChunk,
                NinePatch.InsetStruct ninePatchInsets, bool fromMalloc)
        {
            if (nativeBitmap == null)
            {
                throw new Exception("internal error: native bitmap is null");
            }

            mWidth = width;
            mHeight = height;
            mRequestPremultiplied = requestPremultiplied;

            mNinePatchChunk = ninePatchChunk;
            mNinePatchInsets = ninePatchInsets;
            if (density >= 0)
            {
                mDensity = density;
            }

            mNativePtr = nativeBitmap;
        }

        /**
         * Return the pointer to the native object.
         *
         * @hide
         * Must be public for access from android.graphics.pdf,
         * but must not be called from outside the UI module.
         */
        internal SkiaSharp.SKBitmap getNativeInstance()
        {
            return mNativePtr;
        }

        /**
         * Native bitmap has been reconfigured, so set premult and cached
         * width/height values
         */
        void reinit(int width, int height, bool requestPremultiplied)
        {
            mWidth = width;
            mHeight = height;
            mRequestPremultiplied = requestPremultiplied;
            mColorSpace = null;
        }

        /**
         * <p>Returns the density for this bitmap.</p>
         *
         * <p>The default density is the same density as the current display,
         * unless the current application does not support different screen
         * densities in which case it is
         * {@link android.util.DisplayMetrics#DENSITY_DEFAULT}.  Note that
         * compatibility mode is determined by the application that was initially
         * loaded into a process -- applications that share the same process should
         * all have the same compatibility, or ensure they explicitly set the
         * density of their bitmaps appropriately.</p>
         *
         * @return A scaling factor of the default density or {@link #DENSITY_NONE}
         *         if the scaling factor is unknown.
         *
         * @see #setDensity(int)
         * @see android.util.DisplayMetrics#DENSITY_DEFAULT
         * @see android.util.DisplayMetrics#densityDpi
         * @see #DENSITY_NONE
         */
        public int getDensity()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getDensity() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mDensity;
        }

        /**
         * <p>Specifies the density for this bitmap.  When the bitmap is
         * drawn to a Canvas that also has a density, it will be scaled
         * appropriately.</p>
         *
         * @param density The density scaling factor to use with this bitmap or
         *        {@link #DENSITY_NONE} if the density is unknown.
         *
         * @see #getDensity()
         * @see android.util.DisplayMetrics#DENSITY_DEFAULT
         * @see android.util.DisplayMetrics#densityDpi
         * @see #DENSITY_NONE
         */
        public void setDensity(int density)
        {
            mDensity = density;
        }

        /**
         * <p>Modifies the bitmap to have a specified width, height, and {@link
         * Config}, without affecting the underlying allocation backing the bitmap.
         * Bitmap pixel data is not re-initialized for the new configuration.</p>
         *
         * <p>This method can be used to avoid allocating a new bitmap, instead
         * reusing an existing bitmap's allocation for a new configuration of equal
         * or lesser size. If the Bitmap's allocation isn't large enough to support
         * the new configuration, an IllegalArgumentException will be thrown and the
         * bitmap will not be modified.</p>
         *
         * <p>The result of {@link #getByteCount()} will reflect the new configuration,
         * while {@link #getAllocationByteCount()} will reflect that of the initial
         * configuration.</p>
         *
         * <p>Note: This may change this result of hasAlpha(). When converting to 565,
         * the new bitmap will always be considered opaque. When converting from 565,
         * the new bitmap will be considered non-opaque, and will respect the value
         * set by setPremultiplied().</p>
         *
         * <p>WARNING: This method should NOT be called on a bitmap currently in use
         * by the view system, Canvas, or the AndroidBitmap NDK API. It does not
         * make guarantees about how the underlying pixel buffer is remapped to the
         * new config, just that the allocation is reused. Additionally, the view
         * system does not account for bitmap properties being modifying during use,
         * e.g. while attached to drawables.</p>
         *
         * <p>In order to safely ensure that a Bitmap is no longer in use by the
         * View system it is necessary to wait for a draw pass to occur after
         * invalidate()'ing any view that had previously drawn the Bitmap in the last
         * draw pass due to hardware acceleration's caching of draw commands. As
         * an example, here is how this can be done for an ImageView:
         * <pre class="prettyprint">
         *      ImageView myImageView = ...;
         *      Bitmap myBitmap = ...;
         *      myImageView.setImageDrawable(null);
         *      myImageView.post(new Runnable() {
         *          public void run() {
         *              // myBitmap is now no longer in use by the ImageView
         *              // and can be safely reconfigured.
         *              myBitmap.reconfigure(...);
         *          }
         *      });
         * </pre></p>
         *
         * @see #setWidth(int)
         * @see #setHeight(int)
         * @see #setConfig(Config)
         */
        public void reconfigure(int width, int height, Config config)
        {
            checkRecycled("Can't call reconfigure() on a recycled bitmap");
            if (width <= 0 || height <= 0)
            {
                throw new IllegalArgumentException("width and height must be > 0");
            }
            if (!isMutable())
            {
                throw new IllegalStateException("only mutable bitmaps may be reconfigured");
            }

            if (mNativePtr != null)
            {

            }
            nativeReconfigure(mNativePtr, width, height, config.nativeInt, mRequestPremultiplied);
            mWidth = width;
            mHeight = height;
            mColorSpace = null;
        }

        /**
         * <p>Convenience method for calling {@link #reconfigure(int, int, Config)}
         * with the current height and config.</p>
         *
         * <p>WARNING: this method should not be used on bitmaps currently used by
         * the view system, see {@link #reconfigure(int, int, Config)} for more
         * details.</p>
         *
         * @see #reconfigure(int, int, Config)
         * @see #setHeight(int)
         * @see #setConfig(Config)
         */
        public void setWidth(int width)
        {
            reconfigure(width, getHeight(), getConfig());
        }

        /**
         * <p>Convenience method for calling {@link #reconfigure(int, int, Config)}
         * with the current width and config.</p>
         *
         * <p>WARNING: this method should not be used on bitmaps currently used by
         * the view system, see {@link #reconfigure(int, int, Config)} for more
         * details.</p>
         *
         * @see #reconfigure(int, int, Config)
         * @see #setWidth(int)
         * @see #setConfig(Config)
         */
        public void setHeight(int height)
        {
            reconfigure(getWidth(), height, getConfig());
        }

        /**
         * <p>Convenience method for calling {@link #reconfigure(int, int, Config)}
         * with the current height and width.</p>
         *
         * <p>WARNING: this method should not be used on bitmaps currently used by
         * the view system, see {@link #reconfigure(int, int, Config)} for more
         * details.</p>
         *
         * @see #reconfigure(int, int, Config)
         * @see #setWidth(int)
         * @see #setHeight(int)
         */
        public void setConfig(Config config)
        {
            reconfigure(getWidth(), getHeight(), config);
        }

        /**
         * Sets the nine patch chunk.
         *
         * @param chunk The definition of the nine patch
         */
        private void setNinePatchChunk(byte[] chunk)
        {
            mNinePatchChunk = chunk;
        }

        /**
         * Free the native object associated with this bitmap, and clear the
         * reference to the pixel data. This will not free the pixel data synchronously;
         * it simply allows it to be garbage collected if there are no other references.
         * The bitmap is marked as "dead", meaning it will throw an exception if
         * getPixels() or setPixels() is called, and will draw nothing. This operation
         * cannot be reversed, so it should only be called if you are sure there are no
         * further uses for the bitmap. This is an advanced call, and normally need
         * not be called, since the normal GC process will free up this memory when
         * there are no more references to this bitmap.
         */
        public void recycle()
        {
            if (!mRecycled)
            {
                mNativePtr.Dispose();
                mNinePatchChunk = null;
                mRecycled = true;
                mNativePtr = null;
            }
        }

        /**
         * Returns true if this bitmap has been recycled. If so, then it is an error
         * to try to access its pixels, and the bitmap will not draw.
         *
         * @return true if the bitmap has been recycled
         */
        public bool isRecycled() {
            return mRecycled;
        }

        /**
         * Returns the generation ID of this bitmap. The generation ID changes
         * whenever the bitmap is modified. This can be used as an efficient way to
         * check if a bitmap has changed.
         *
         * @return The current generation ID for this bitmap.
         */
        public int getGenerationId()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getGenerationId() on a recycle()'d bitmap! This is undefined behavior!");
            }
            // TODO
            return 0;
        }

        /**
         * This is called by methods that want to throw an exception if the bitmap
         * has already been recycled.
         */
        private void checkRecycled(string errorMessage)
        {
            if (mRecycled)
            {
                throw new IllegalStateException(errorMessage);
            }
        }

        /**
         * This is called by methods that want to throw an exception if the bitmap
         * is {@link Config#HARDWARE}.
         */
        private void checkHardware(string errorMessage)
        {
            if (getConfig() == Config.HARDWARE)
            {
                throw new IllegalStateException(errorMessage);
            }
        }

        /**
         * Common code for checking that x and y are >= 0
         *
         * @param x x coordinate to ensure is >= 0
         * @param y y coordinate to ensure is >= 0
         */
        private static void checkXYSign(int x, int y)
        {
            if (x < 0)
            {
                throw new IllegalArgumentException("x must be >= 0");
            }
            if (y < 0)
            {
                throw new IllegalArgumentException("y must be >= 0");
            }
        }

        /**
         * Common code for checking that width and height are > 0
         *
         * @param width  width to ensure is > 0
         * @param height height to ensure is > 0
         */
        private static void checkWidthHeight(int width, int height)
        {
            if (width <= 0)
            {
                throw new IllegalArgumentException("width must be > 0");
            }
            if (height <= 0)
            {
                throw new IllegalArgumentException("height must be > 0");
            }
        }

        /**
         * Possible bitmap configurations. A bitmap configuration describes
         * how pixels are stored. This affects the quality (color depth) as
         * well as the ability to display transparent/translucent colors.
         */
        public class Config
        {
            // these native values must match up with the enum in SkBitmap.h

            /**
             * Each pixel is stored as a single translucency (alpha) channel.
             * This is very useful to efficiently store masks for instance.
             * No color information is stored.
             * With this configuration, each pixel requires 1 byte of memory.
             */
            public const int ALPHA_8 = 1;

            /**
             * Each pixel is stored on 2 bytes and only the RGB channels are
             * encoded: red is stored with 5 bits of precision (32 possible
             * values), green is stored with 6 bits of precision (64 possible
             * values) and blue is stored with 5 bits of precision.
             *
             * This configuration can produce slight visual artifacts depending
             * on the configuration of the source. For instance, without
             * dithering, the result might show a greenish tint. To get better
             * results dithering should be applied.
             *
             * This configuration may be useful when using opaque bitmaps
             * that do not require high color fidelity.
             *
             * <p>Use this formula to pack into 16 bits:</p>
             * <pre class="prettyprint">
             * short color = (R & 0x1f) << 11 | (G & 0x3f) << 5 | (B & 0x1f);
             * </pre>
             */
            public const int RGB_565 = 3;

            /**
             * Each pixel is stored on 2 bytes. The three RGB color channels
             * and the alpha channel (translucency) are stored with a 4 bits
             * precision (16 possible values.)
             *
             * This configuration is mostly useful if the application needs
             * to store translucency information but also needs to save
             * memory.
             *
             * It is recommended to use {@link #ARGB_8888} instead of this
             * configuration.
             *
             * Note: as of {@link android.os.Build.VERSION_CODES#KITKAT},
             * any bitmap created with this configuration will be created
             * using {@link #ARGB_8888} instead.
             *
             * @deprecated Because of the poor quality of this configuration,
             *             it is advised to use {@link #ARGB_8888} instead.
             */
            public const int ARGB_4444 = 4;

            /**
             * Each pixel is stored on 4 bytes. Each channel (RGB and alpha
             * for translucency) is stored with 8 bits of precision (256
             * possible values.)
             *
             * This configuration is very flexible and offers the best
             * quality. It should be used whenever possible.
             *
             * <p>Use this formula to pack into 32 bits:</p>
             * <pre class="prettyprint">
             * int color = (A & 0xff) << 24 | (B & 0xff) << 16 | (G & 0xff) << 8 | (R & 0xff);
             * </pre>
             */
            public const int ARGB_8888 = 5;

            /**
             * Each pixels is stored on 8 bytes. Each channel (RGB and alpha
             * for translucency) is stored as a
             * {@link android.util.Half half-precision floating point value}.
             *
             * This configuration is particularly suited for wide-gamut and
             * HDR content.
             *
             * <p>Use this formula to pack into 64 bits:</p>
             * <pre class="prettyprint">
             * long color = (A & 0xffff) << 48 | (B & 0xffff) << 32 | (G & 0xffff) << 16 | (R & 0xffff);
             * </pre>
             */
            public const int RGBA_F16 = 6;

            /**
             * Special configuration, when bitmap is stored only in graphic memory.
             * Bitmaps in this configuration are always immutable.
             *
             * It is optimal for cases, when the only operation with the bitmap is to draw it on a
             * screen.
             */
            public const int HARDWARE = 7;

            internal int nativeInt;

            private static readonly Config[] sConfigs = {
                new Config(0), new Config(ALPHA_8),
                new Config(0), new Config(RGB_565),
                new Config(ARGB_4444), new Config(ARGB_8888),
                new Config(RGBA_F16), new Config(HARDWARE)
            };

            Config(int ni) {
                nativeInt = ni;
            }

            internal static Config nativeToConfig(int ni)
            {
                return sConfigs[ni];
            }

            public override bool Equals(object obj)
            {
                return obj is Config config &&
                       nativeInt == config.nativeInt;
            }

            public static bool operator ==(Config left, Config right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Config left, Config right)
            {
                return !(left == right);
            }

            public bool Equals(int config)
            {
                return nativeInt == config;
            }

            public static bool operator ==(Config left, int right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Config left, int right)
            {
                return !(left == right);
            }
        }

        /**
         * <p>Copy the bitmap's pixels into the specified buffer (allocated by the
         * caller). An exception is thrown if the buffer is not large enough to
         * hold all of the pixels (taking into account the number of bytes per
         * pixel) or if the Buffer subclass is not one of the support types
         * (ByteBuffer, ShortBuffer, IntBuffer).</p>
         * <p>The content of the bitmap is copied into the buffer as-is. This means
         * that if this bitmap stores its pixels pre-multiplied
         * (see {@link #isPremultiplied()}, the values in the buffer will also be
         * pre-multiplied. The pixels remain in the color space of the bitmap.</p>
         * <p>After this method returns, the current position of the buffer is
         * updated: the position is incremented by the number of elements written
         * in the buffer.</p>
         * @throws IllegalStateException if the bitmap's config is {@link Config#HARDWARE}
         */
        public void copyPixelsToBuffer(Stream dst)
        {
            checkHardware("unable to copyPixelsToBuffer, "
                    + "pixel access is not supported on Config#HARDWARE bitmaps");
            long elements = dst.Length - dst.Position;
            int shift;

            long pixelSize = getByteCount();

            if (elements < pixelSize)
            {
                throw new Exception("Buffer not large enough for pixels");
            }

            byte[] pixels = mNativePtr.Bytes;
            dst.Write(pixels, 0, pixels.Length);
        }

        /**
         * <p>Copy the pixels from the buffer, beginning at the current position,
         * overwriting the bitmap's pixels. The data in the buffer is not changed
         * in any way (unlike setPixels(), which converts from unpremultipled 32bit
         * to whatever the bitmap's native format is. The pixels in the source
         * buffer are assumed to be in the bitmap's color space.</p>
         * <p>After this method returns, the current position of the buffer is
         * updated: the position is incremented by the number of elements read from
         * the buffer. If you need to read the bitmap from the buffer again you must
         * first rewind the buffer.</p>
         * @throws IllegalStateException if the bitmap's config is {@link Config#HARDWARE}
         */
        public void copyPixelsFromBuffer(Stream src)
        {
            checkRecycled("copyPixelsFromBuffer called on recycled bitmap");
            checkHardware("unable to copyPixelsFromBuffer, Config#HARDWARE bitmaps are immutable");

            long elements = src.Length - src.Position;
            int shift;

            long bitmapBytes = getByteCount();

            if (elements < bitmapBytes)
            {
                throw new Exception("Buffer not large enough for pixels");
            }

            byte[] pixels = mNativePtr.Bytes;
            src.Read(pixels, 0, pixels.Length);
        }

        private void noteHardwareBitmapSlowCall()
        {
            if (getConfig() == Config.HARDWARE)
            {
                Log.w(TAG, "Warning: attempt to read pixels from hardware "
                        + "bitmap, which is very slow operation");
            }
        }

        /**
         * Tries to make a new bitmap based on the dimensions of this bitmap,
         * setting the new bitmap's config to the one specified, and then copying
         * this bitmap's pixels into the new bitmap. If the conversion is not
         * supported, or the allocator fails, then this returns NULL.  The returned
         * bitmap has the same density and color space as the original, except in
         * the following cases. When copying to {@link Config#ALPHA_8}, the color
         * space is dropped. When copying to or from {@link Config#RGBA_F16},
         * EXTENDED or non-EXTENDED variants may be adjusted as appropriate.
         *
         * @param config    The desired config for the resulting bitmap
         * @param isMutable True if the resulting bitmap should be mutable (i.e.
         *                  its pixels can be modified)
         * @return the new bitmap, or null if the copy could not be made.
         * @throws IllegalArgumentException if config is {@link Config#HARDWARE} and isMutable is true
         */
        public Bitmap copy(Config config, bool isMutable)
        {
            checkRecycled("Can't copy a recycled bitmap");
            if (config == Config.HARDWARE && isMutable)
            {
                throw new IllegalArgumentException("Hardware bitmaps are always immutable");
            }
            noteHardwareBitmapSlowCall();
            // TODO
            Bitmap b = nativeCopy(mNativePtr, config.nativeInt, isMutable);
            if (b != null)
            {
                b.setPremultiplied(mRequestPremultiplied);
                b.mDensity = mDensity;
            }
            return b;
        }

        /**
         * Creates a new immutable bitmap backed by ashmem which can efficiently
         * be passed between processes.
         *
         * @hide
         */
        public Bitmap createAshmemBitmap()
        {
            checkRecycled("Can't copy a recycled bitmap");
            noteHardwareBitmapSlowCall();
            // TODO
            Bitmap b = nativeCopyAshmem(mNativePtr);
            if (b != null)
            {
                b.setPremultiplied(mRequestPremultiplied);
                b.mDensity = mDensity;
            }
            return b;
        }

        /**
         * Return an immutable bitmap backed by shared memory which can be
         * efficiently passed between processes via Parcelable.
         *
         * <p>If this bitmap already meets these criteria it will return itself.
         */
        public Bitmap asShared()
        {
            // TODO
            if (nativeIsBackedByAshmem(mNativePtr) && nativeIsImmutable(mNativePtr))
            {
                return this;
            }
            Bitmap shared = createAshmemBitmap();
            if (shared == null)
            {
                throw new RuntimeException("Failed to create shared Bitmap!");
            }
            return shared;
        }

        /**
         * Creates a new bitmap, scaled from an existing bitmap, when possible. If the
         * specified width and height are the same as the current width and height of
         * the source bitmap, the source bitmap is returned and no new bitmap is
         * created.
         *
         * @param src       The source bitmap.
         * @param dstWidth  The new bitmap's desired width.
         * @param dstHeight The new bitmap's desired height.
         * @param filter    Whether or not bilinear filtering should be used when scaling the
         *                  bitmap. If this is true then bilinear filtering will be used when
         *                  scaling which has better image quality at the cost of worse performance.
         *                  If this is false then nearest-neighbor scaling is used instead which
         *                  will have worse image quality but is faster. Recommended default
         *                  is to set filter to 'true' as the cost of bilinear filtering is
         *                  typically minimal and the improved image quality is significant.
         * @return The new scaled bitmap or the source bitmap if no scaling is required.
         * @throws IllegalArgumentException if width is <= 0, or height is <= 0
         */
        public static Bitmap createScaledBitmap(Bitmap src, int dstWidth, int dstHeight,
                bool filter)
        {
            SkiaSharp.SKMatrix m = new SkiaSharp.SKMatrix();

            int width = src.getWidth();
            int height = src.getHeight();
            if (width != dstWidth || height != dstHeight)
            {
                float sx = dstWidth / (float)width;
                float sy = dstHeight / (float)height;
                m.ScaleX = sx;
                m.ScaleY = sy;
            }
            return Bitmap.createBitmap(src, 0, 0, width, height, m, filter);
        }

        /**
         * Returns a bitmap from the source bitmap. The new bitmap may
         * be the same object as source, or a copy may have been made.  It is
         * initialized with the same density and color space as the original bitmap.
         */
        public static Bitmap createBitmap(Bitmap src)
        {
            return createBitmap(src, 0, 0, src.getWidth(), src.getHeight());
        }

        /**
         * Returns a bitmap from the specified subset of the source
         * bitmap. The new bitmap may be the same object as source, or a copy may
         * have been made. It is initialized with the same density and color space
         * as the original bitmap.
         *
         * @param source   The bitmap we are subsetting
         * @param x        The x coordinate of the first pixel in source
         * @param y        The y coordinate of the first pixel in source
         * @param width    The number of pixels in each row
         * @param height   The number of rows
         * @return A copy of a subset of the source bitmap or the source bitmap itself.
         * @throws IllegalArgumentException if the x, y, width, height values are
         *         outside of the dimensions of the source bitmap, or width is <= 0,
         *         or height is <= 0
         */
        public static Bitmap createBitmap(Bitmap source, int x, int y, int width, int height)
        {
            return createBitmap(source, x, y, width, height, null, false);
        }

        /**
         * Returns a bitmap from subset of the source bitmap,
         * transformed by the optional matrix. The new bitmap may be the
         * same object as source, or a copy may have been made. It is
         * initialized with the same density and color space as the original
         * bitmap.
         *
         * If the source bitmap is immutable and the requested subset is the
         * same as the source bitmap itself, then the source bitmap is
         * returned and no new bitmap is created.
         *
         * The returned bitmap will always be mutable except in the following scenarios:
         * (1) In situations where the source bitmap is returned and the source bitmap is immutable
         *
         * (2) The source bitmap is a hardware bitmap. That is {@link #getConfig()} is equivalent to
         * {@link Config#HARDWARE}
         *
         * @param source   The bitmap we are subsetting
         * @param x        The x coordinate of the first pixel in source
         * @param y        The y coordinate of the first pixel in source
         * @param width    The number of pixels in each row
         * @param height   The number of rows
         * @param m        Optional matrix to be applied to the pixels
         * @param filter   true if the source should be filtered.
         *                   Only applies if the matrix contains more than just
         *                   translation.
         * @return A bitmap that represents the specified subset of source
         * @throws IllegalArgumentException if the x, y, width, height values are
         *         outside of the dimensions of the source bitmap, or width is <= 0,
         *         or height is <= 0, or if the source bitmap has already been recycled
         */
        public static Bitmap createBitmap(Bitmap source, int x, int y, int width, int height,
                Matrix m, bool filter)
        {

            checkXYSign(x, y);
            checkWidthHeight(width, height);
            if (x + width > source.getWidth())
            {
                throw new IllegalArgumentException("x + width must be <= bitmap.width()");
            }
            if (y + height > source.getHeight())
            {
                throw new IllegalArgumentException("y + height must be <= bitmap.height()");
            }
            if (source.isRecycled())
            {
                throw new IllegalArgumentException("cannot use a recycled source in createBitmap");
            }

            // check if we can just return our argument unchanged
            if (!source.isMutable() && x == 0 && y == 0 && width == source.getWidth() &&
                    height == source.getHeight() && (m == null || m.isIdentity()))
            {
                return source;
            }

            bool isHardware = source.getConfig() == Config.HARDWARE;
            if (isHardware)
            {
                source.noteHardwareBitmapSlowCall();
                source = nativeCopyPreserveInternalConfig(source.mNativePtr);
            }

            int neww = width;
            int newh = height;
            Bitmap bitmap;
            Paint paint;

            Rect srcR = new Rect(x, y, x + width, y + height);
            RectF dstR = new RectF(0, 0, width, height);
            RectF deviceR = new RectF();

            Config newConfig = Config.ARGB_8888;
            Config config = source.getConfig();
            // GIF files generate null configs, assume ARGB_8888
            if (config != null)
            {
                switch (config)
                {
                    case RGB_565:
                        newConfig = Config.RGB_565;
                        break;
                    case ALPHA_8:
                        newConfig = Config.ALPHA_8;
                        break;
                    case RGBA_F16:
                        newConfig = Config.RGBA_F16;
                        break;
                    //noinspection deprecation
                    case ARGB_4444:
                    case ARGB_8888:
                    default:
                        newConfig = Config.ARGB_8888;
                        break;
                }
            }

            ColorSpace cs = source.getColorSpace();

            if (m == null || m.isIdentity())
            {
                bitmap = createBitmap(null, neww, newh, newConfig, source.hasAlpha(), cs);
                paint = null;   // not needed
            }
            else
            {
                bool transformed = !m.rectStaysRect();

                m.mapRect(deviceR, dstR);

                neww = Math.round(deviceR.width());
                newh = Math.round(deviceR.height());

                Config transformedConfig = newConfig;
                if (transformed)
                {
                    if (transformedConfig != Config.ARGB_8888 && transformedConfig != Config.RGBA_F16)
                    {
                        transformedConfig = Config.ARGB_8888;
                        if (cs == null)
                        {
                            cs = ColorSpace.get(ColorSpace.Named.SRGB);
                        }
                    }
                }

                bitmap = createBitmap(null, neww, newh, transformedConfig,
                        transformed || source.hasAlpha(), cs);

                paint = new Paint();
                paint.setFilterBitmap(filter);
                if (transformed)
                {
                    paint.setAntiAlias(true);
                }
            }

            // The new bitmap was created from a known bitmap source so assume that
            // they use the same density
            bitmap.mDensity = source.mDensity;
            bitmap.setHasAlpha(source.hasAlpha());
            bitmap.setPremultiplied(source.mRequestPremultiplied);

            SkiaSharp.SKCanvas canvas = new SkiaSharp.SKCanvas(bitmap.getNativeInstance());
            canvas.Translate(-deviceR.left, -deviceR.top);
            canvas.Concat(m);
            canvas.DrawBitmap(source, srcR, dstR, paint);
            //canvas.SetBitmap(null);
            if (isHardware)
            {
                return bitmap.copy(Config.HARDWARE, false);
            }
            return bitmap;
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is as per {@link #getDensity}. The newly created
         * bitmap is in the {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         Config is Config.HARDWARE, because hardware bitmaps are always immutable
         */
        public static Bitmap createBitmap(int width, int height, Config config)
        {
            return createBitmap(width, height, config, true);
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is determined from the given {@link DisplayMetrics}.
         * The newly created bitmap is in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.
         *
         * @param display  Display metrics for the display this bitmap will be
         *                 drawn on.
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         Config is Config.HARDWARE, because hardware bitmaps are always immutable
         */
        public static Bitmap createBitmap(DisplayMetrics display, int width,
                int height, Config config)
        {
            return createBitmap(display, width, height, config, true);
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is as per {@link #getDensity}. The newly created
         * bitmap is in the {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @param hasAlpha If the bitmap is ARGB_8888 or RGBA_16F this flag can be used to
         *                 mark the bitmap as opaque. Doing so will clear the bitmap in black
         *                 instead of transparent.
         *
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         Config is Config.HARDWARE, because hardware bitmaps are always immutable
         */
        public static Bitmap createBitmap(int width, int height,
                Config config, bool hasAlpha)
        {
            return createBitmap(null, width, height, config, hasAlpha);
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is as per {@link #getDensity}.
         *
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @param hasAlpha If the bitmap is ARGB_8888 or RGBA_16F this flag can be used to
         *                 mark the bitmap as opaque. Doing so will clear the bitmap in black
         *                 instead of transparent.
         * @param colorSpace The color space of the bitmap. If the config is {@link Config#RGBA_F16}
         *                   and {@link ColorSpace.Named#SRGB sRGB} or
         *                   {@link ColorSpace.Named#LINEAR_SRGB Linear sRGB} is provided then the
         *                   corresponding extended range variant is assumed.
         *
         * @throws IllegalArgumentException if the width or height are <= 0, if
         *         Config is Config.HARDWARE (because hardware bitmaps are always
         *         immutable), if the specified color space is not {@link ColorSpace.Model#RGB RGB},
         *         if the specified color space's transfer function is not an
         *         {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}, or if
         *         the color space is null
         */
        public static Bitmap createBitmap(int width, int height, Config config,
                bool hasAlpha, ColorSpace colorSpace)
        {
            return createBitmap(null, width, height, config, hasAlpha, colorSpace);
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is determined from the given {@link DisplayMetrics}.
         * The newly created bitmap is in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.
         *
         * @param display  Display metrics for the display this bitmap will be
         *                 drawn on.
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @param hasAlpha If the bitmap is ARGB_8888 or RGBA_16F this flag can be used to
         *                 mark the bitmap as opaque. Doing so will clear the bitmap in black
         *                 instead of transparent.
         *
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         Config is Config.HARDWARE, because hardware bitmaps are always immutable
         */
        public static Bitmap createBitmap(DisplayMetrics display, int width, int height,
                Config config, bool hasAlpha)
        {
            return createBitmap(display, width, height, config, hasAlpha,
                    ColorSpace.get(ColorSpace.Named.SRGB));
        }

        /**
         * Returns a mutable bitmap with the specified width and height.  Its
         * initial density is determined from the given {@link DisplayMetrics}.
         * The newly created bitmap is in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.
         *
         * @param display  Display metrics for the display this bitmap will be
         *                 drawn on.
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create.
         * @param hasAlpha If the bitmap is ARGB_8888 or RGBA_16F this flag can be used to
         *                 mark the bitmap as opaque. Doing so will clear the bitmap in black
         *                 instead of transparent.
         * @param colorSpace The color space of the bitmap. If the config is {@link Config#RGBA_F16}
         *                   and {@link ColorSpace.Named#SRGB sRGB} or
         *                   {@link ColorSpace.Named#LINEAR_SRGB Linear sRGB} is provided then the
         *                   corresponding extended range variant is assumed.
         *
         * @throws IllegalArgumentException if the width or height are <= 0, if
         *         Config is Config.HARDWARE (because hardware bitmaps are always
         *         immutable), if the specified color space is not {@link ColorSpace.Model#RGB RGB},
         *         if the specified color space's transfer function is not an
         *         {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}, or if
         *         the color space is null
         */
        public static Bitmap createBitmap(DisplayMetrics display, int width, int height,
                Config config, bool hasAlpha, ColorSpace colorSpace)
        {
            if (width <= 0 || height <= 0)
            {
                throw new IllegalArgumentException("width and height must be > 0");
            }
            if (config == Config.HARDWARE)
            {
                throw new IllegalArgumentException("can't create mutable bitmap with Config.HARDWARE");
            }
            if (colorSpace == null && config != Config.ALPHA_8)
            {
                throw new IllegalArgumentException("can't create bitmap without a color space");
            }

            Bitmap bm = nativeCreate(null, 0, width, width, height, config.nativeInt, true,
                    colorSpace == null ? 0 : colorSpace.getNativeInstance());

            if (display != null)
            {
                bm.mDensity = display.densityDpi;
            }
            bm.setHasAlpha(hasAlpha);
            if ((config == Config.ARGB_8888 || config == Config.RGBA_F16) && !hasAlpha)
            {
                nativeErase(bm.mNativePtr, 0xff000000);
            }
            // No need to initialize the bitmap to zeroes with other configs;
            // it is backed by a VM byte array which is by definition preinitialized
            // to all zeroes.
            return bm;
        }

        /**
         * Returns a immutable bitmap with the specified width and height, with each
         * pixel value set to the corresponding value in the colors array.  Its
         * initial density is as per {@link #getDensity}. The newly created
         * bitmap is in the {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param colors   Array of sRGB {@link Color colors} used to initialize the pixels.
         * @param offset   Number of values to skip before the first color in the
         *                 array of colors.
         * @param stride   Number of colors in the array between rows (must be >=
         *                 width or <= -width).
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create. If the config does not
         *                 support per-pixel alpha (e.g. RGB_565), then the alpha
         *                 bytes in the colors[] will be ignored (assumed to be FF)
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         the color array's length is less than the number of pixels.
         */
        public static Bitmap createBitmap(int[] colors, int offset, int stride,
                int width, int height, Config config)
        {
            return createBitmap(null, colors, offset, stride, width, height, config);
        }

        /**
         * Returns a immutable bitmap with the specified width and height, with each
         * pixel value set to the corresponding value in the colors array.  Its
         * initial density is determined from the given {@link DisplayMetrics}.
         * The newly created bitmap is in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.
         *
         * @param display  Display metrics for the display this bitmap will be
         *                 drawn on.
         * @param colors   Array of sRGB {@link Color colors} used to initialize the pixels.
         * @param offset   Number of values to skip before the first color in the
         *                 array of colors.
         * @param stride   Number of colors in the array between rows (must be >=
         *                 width or <= -width).
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create. If the config does not
         *                 support per-pixel alpha (e.g. RGB_565), then the alpha
         *                 bytes in the colors[] will be ignored (assumed to be FF)
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         the color array's length is less than the number of pixels.
         */
        public static Bitmap createBitmap(DisplayMetrics display,
                int[] colors, int offset, int stride,
                int width, int height, Config config)
        {

            checkWidthHeight(width, height);
            if (Math.Abs(stride) < width)
            {
                throw new IllegalArgumentException("abs(stride) must be >= width");
            }
            int lastScanline = offset + (height - 1) * stride;
            int length = colors.Length;
            if (offset < 0 || (offset + width > length) || lastScanline < 0 ||
                    (lastScanline + width > length))
            {
                throw new IndexOutOfRangeException();
            }
            if (width <= 0 || height <= 0)
            {
                throw new IllegalArgumentException("width and height must be > 0");
            }
            ColorSpace sRGB = ColorSpace.get(ColorSpace.Named.SRGB);
            Bitmap bm = nativeCreate(colors, offset, stride, width, height,
                                config.nativeInt, false, sRGB.getNativeInstance());
            if (display != null)
            {
                bm.mDensity = display.densityDpi;
            }
            return bm;
        }

        /**
         * Returns a immutable bitmap with the specified width and height, with each
         * pixel value set to the corresponding value in the colors array.  Its
         * initial density is as per {@link #getDensity}. The newly created
         * bitmap is in the {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param colors   Array of sRGB {@link Color colors} used to initialize the pixels.
         *                 This array must be at least as large as width * height.
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create. If the config does not
         *                 support per-pixel alpha (e.g. RGB_565), then the alpha
         *                 bytes in the colors[] will be ignored (assumed to be FF)
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         the color array's length is less than the number of pixels.
         */
        public static Bitmap createBitmap(int[] colors,
                int width, int height, Config config)
        {
            return createBitmap(null, colors, 0, width, width, height, config);
        }

        /**
         * Returns a immutable bitmap with the specified width and height, with each
         * pixel value set to the corresponding value in the colors array.  Its
         * initial density is determined from the given {@link DisplayMetrics}.
         * The newly created bitmap is in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.
         *
         * @param display  Display metrics for the display this bitmap will be
         *                 drawn on.
         * @param colors   Array of sRGB {@link Color colors} used to initialize the pixels.
         *                 This array must be at least as large as width * height.
         * @param width    The width of the bitmap
         * @param height   The height of the bitmap
         * @param config   The bitmap config to create. If the config does not
         *                 support per-pixel alpha (e.g. RGB_565), then the alpha
         *                 bytes in the colors[] will be ignored (assumed to be FF)
         * @throws IllegalArgumentException if the width or height are <= 0, or if
         *         the color array's length is less than the number of pixels.
         */
        public static Bitmap createBitmap(DisplayMetrics display,
                int colors[], int width, int height, Config config)
        {
            return createBitmap(display, colors, 0, width, width, height, config);
        }

        /**
         * Creates a Bitmap from the given {@link Picture} source of recorded drawing commands.
         *
         * Equivalent to calling {@link #createBitmap(Picture, int, int, Config)} with
         * width and height the same as the Picture's width and height and a Config.HARDWARE
         * config.
         *
         * @param source The recorded {@link Picture} of drawing commands that will be
         *               drawn into the returned Bitmap.
         * @return An immutable bitmap with a HARDWARE config whose contents are created
         * from the recorded drawing commands in the Picture source.
         */
        public static Bitmap createBitmap(Picture source) {
            return createBitmap(source, source.getWidth(), source.getHeight(), Config.HARDWARE);
        }

        /**
         * Creates a Bitmap from the given {@link Picture} source of recorded drawing commands.
         *
         * The bitmap will be immutable with the given width and height. If the width and height
         * are not the same as the Picture's width & height, the Picture will be scaled to
         * fit the given width and height.
         *
         * @param source The recorded {@link Picture} of drawing commands that will be
         *               drawn into the returned Bitmap.
         * @param width The width of the bitmap to create. The picture's width will be
         *              scaled to match if necessary.
         * @param height The height of the bitmap to create. The picture's height will be
         *              scaled to match if necessary.
         * @param config The {@link Config} of the created bitmap.
         *
         * @return An immutable bitmap with a configuration specified by the config parameter
         */
        public static Bitmap createBitmap(Picture source, int width, int height,
                Config config) {
            if (width <= 0 || height <= 0)
            {
                throw new IllegalArgumentException("width & height must be > 0");
            }
            if (config == null)
            {
                throw new IllegalArgumentException("Config must not be null");
            }
            source.endRecording();
            if (source.requiresHardwareAcceleration() && config != Config.HARDWARE)
            {
                StrictMode.noteSlowCall("GPU readback");
            }
            if (config == Config.HARDWARE || source.requiresHardwareAcceleration())
            {
                RenderNode node = RenderNode.create("BitmapTemporary", null);
                node.setLeftTopRightBottom(0, 0, width, height);
                node.setClipToBounds(false);
                node.setForceDarkAllowed(false);
                RecordingCanvas canvas = node.beginRecording(width, height);
                if (source.getWidth() != width || source.getHeight() != height)
                {
                    canvas.scale(width / (float)source.getWidth(),
                            height / (float)source.getHeight());
                }
                canvas.drawPicture(source);
                node.endRecording();
                Bitmap bitmap = ThreadedRenderer.createHardwareBitmap(node, width, height);
                if (config != Config.HARDWARE)
                {
                    bitmap = bitmap.copy(config, false);
                }
                return bitmap;
            }
            else
            {
                Bitmap bitmap = Bitmap.createBitmap(width, height, config);
                Canvas canvas = new Canvas(bitmap);
                if (source.getWidth() != width || source.getHeight() != height)
                {
                    canvas.scale(width / (float)source.getWidth(),
                            height / (float)source.getHeight());
                }
                canvas.drawPicture(source);
                canvas.setBitmap(null);
                bitmap.setImmutable();
                return bitmap;
            }
        }

        /**
         * Returns an optional array of private data, used by the UI system for
         * some bitmaps. Not intended to be called by applications.
         */
        public byte[] getNinePatchChunk()
        {
            return mNinePatchChunk;
        }

        /**
         * Populates a rectangle with the bitmap's optical insets.
         *
         * @param outInsets Rect to populate with optical insets
         *
         * @hide
         * Must be public for access from android.graphics.drawable,
         * but must not be called from outside the UI module.
         */
        public void getOpticalInsets(Rect outInsets)
        {
            if (mNinePatchInsets == null)
            {
                outInsets.setEmpty();
            }
            else
            {
                outInsets.set(mNinePatchInsets.opticalRect);
            }
        }

        /**
         * @hide
         * Must be public for access from android.graphics.drawable,
         * but must not be called from outside the UI module.
         */
        public NinePatch.InsetStruct getNinePatchInsets()
        {
            return mNinePatchInsets;
        }

        /**
         * Specifies the known formats a bitmap can be compressed into
         */
        public class CompressFormat
        {
            /**
             * Compress to the JPEG format. {@code quality} of {@code 0} means
             * compress for the smallest size. {@code 100} means compress for max
             * visual quality.
             */
            public const int JPEG = 0;
            /**
             * Compress to the PNG format. PNG is lossless, so {@code quality} is
             * ignored.
             */
            public const int PNG = 1;
            /**
             * Compress to the WEBP format. {@code quality} of {@code 0} means
             * compress for the smallest size. {@code 100} means compress for max
             * visual quality. As of {@link android.os.Build.VERSION_CODES#Q}, a
             * value of {@code 100} results in a file in the lossless WEBP format.
             * Otherwise the file will be in the lossy WEBP format.
             *
             * @deprecated in favor of the more explicit
             *             {@link CompressFormat#WEBP_LOSSY} and
             *             {@link CompressFormat#WEBP_LOSSLESS}.
             */
            public const int WEBP = 2;
            /**
             * Compress to the WEBP lossy format. {@code quality} of {@code 0} means
             * compress for the smallest size. {@code 100} means compress for max
             * visual quality.
             */
            public const int WEBP_LOSSY = 3;
            /**
             * Compress to the WEBP lossless format. {@code quality} refers to how
             * much effort to put into compression. A value of {@code 0} means to
             * compress quickly, resulting in a relatively large file size.
             * {@code 100} means to spend more time compressing, resulting in a
             * smaller file.
             */
            public const int WEBP_LOSSLESS = 4;

            CompressFormat(int nativeInt) {
                this.nativeInt = nativeInt;
            }
            internal int nativeInt;
        }

        /**
         * Number of bytes of temp storage we use for communicating between the
         * native compressor and the java OutputStream.
         */
        private static int WORKING_COMPRESS_STORAGE = 4096;

        /**
         * Write a compressed version of the bitmap to the specified outputstream.
         * If this returns true, the bitmap can be reconstructed by passing a
         * corresponding inputstream to BitmapFactory.decodeStream(). Note: not
         * all Formats support all bitmap configs directly, so it is possible that
         * the returned bitmap from BitmapFactory could be in a different bitdepth,
         * and/or may have lost per-pixel alpha (e.g. JPEG only supports opaque
         * pixels).
         *
         * @param format   The format of the compressed image
         * @param quality  Hint to the compressor, 0-100. The value is interpreted
         *                 differently depending on the {@link CompressFormat}.
         * @param stream   The outputstream to write the compressed data.
         * @return true if successfully compressed to the specified stream.
         */
        public bool compress(CompressFormat format, int quality, OutputStream stream)
        {
            checkRecycled("Can't compress a recycled bitmap");
            // do explicit check before calling the native method
            if (stream == null)
            {
                throw new NullReferenceException();
            }
            if (quality < 0 || quality > 100)
            {
                throw new IllegalArgumentException("quality must be 0..100");
            }
            Log.w(TAG, "Compression of a bitmap is slow");
            // TODO
            bool result = nativeCompress(mNativePtr, format.nativeInt,
                    quality, stream, new byte[WORKING_COMPRESS_STORAGE]);
            return result;
        }

        /**
         * Returns true if the bitmap is marked as mutable (i.e.&nbsp;can be drawn into)
         */
        public bool isMutable() {
            return !mNativePtr.IsImmutable;
        }

        /**
         * Marks the Bitmap as immutable. Further modifications to this Bitmap are disallowed.
         * After this method is called, this Bitmap cannot be made mutable again and subsequent calls
         * to {@link #reconfigure(int, int, Config)}, {@link #setPixel(int, int, int)},
         * {@link #setPixels(int[], int, int, int, int, int, int)} and {@link #eraseColor(int)} will
         * fail and throw an IllegalStateException.
         */
        private void setImmutable()
        {
            if (isMutable())
            {
                mNativePtr.SetImmutable();
            }
        }

        /**
         * <p>Indicates whether pixels stored in this bitmaps are stored pre-multiplied.
         * When a pixel is pre-multiplied, the RGB components have been multiplied by
         * the alpha component. For instance, if the original color is a 50%
         * translucent red <code>(128, 255, 0, 0)</code>, the pre-multiplied form is
         * <code>(128, 128, 0, 0)</code>.</p>
         *
         * <p>This method always returns false if {@link #getConfig()} is
         * {@link Bitmap.Config#RGB_565}.</p>
         *
         * <p>The return value is undefined if {@link #getConfig()} is
         * {@link Bitmap.Config#ALPHA_8}.</p>
         *
         * <p>This method only returns true if {@link #hasAlpha()} returns true.
         * A bitmap with no alpha channel can be used both as a pre-multiplied and
         * as a non pre-multiplied bitmap.</p>
         *
         * <p>Only pre-multiplied bitmaps may be drawn by the view system or
         * {@link Canvas}. If a non-pre-multiplied bitmap with an alpha channel is
         * drawn to a Canvas, a RuntimeException will be thrown.</p>
         *
         * @return true if the underlying pixels have been pre-multiplied, false
         *         otherwise
         *
         * @see Bitmap#setPremultiplied(bool)
         * @see BitmapFactory.Options#inPremultiplied
         */
        public bool isPremultiplied() {
            if (mRecycled)
            {
                Log.w(TAG, "Called isPremultiplied() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mNativePtr.Info.AlphaType == SkiaSharp.SKAlphaType.Premul;
        }

        /**
         * Sets whether the bitmap should treat its data as pre-multiplied.
         *
         * <p>Bitmaps are always treated as pre-multiplied by the view system and
         * {@link Canvas} for performance reasons. Storing un-pre-multiplied data in
         * a Bitmap (through {@link #setPixel}, {@link #setPixels}, or {@link
         * BitmapFactory.Options#inPremultiplied BitmapFactory.Options.inPremultiplied})
         * can lead to incorrect blending if drawn by the framework.</p>
         *
         * <p>This method will not affect the behavior of a bitmap without an alpha
         * channel, or if {@link #hasAlpha()} returns false.</p>
         *
         * <p>Calling {@link #createBitmap} or {@link #createScaledBitmap} with a source
         * Bitmap whose colors are not pre-multiplied may result in a RuntimeException,
         * since those functions require drawing the source, which is not supported for
         * un-pre-multiplied Bitmaps.</p>
         *
         * @see Bitmap#isPremultiplied()
         * @see BitmapFactory.Options#inPremultiplied
         */
        public void setPremultiplied(bool premultiplied)
        {
            checkRecycled("setPremultiplied called on a recycled bitmap");
            mRequestPremultiplied = premultiplied;
            if (!mNativePtr.Info.IsOpaque) {
                SkiaSharp.SKImageInfo info = mNativePtr.Info;
                if (premultiplied)
                {
                    info.AlphaType = SkiaSharp.SKAlphaType.Premul;
                }
                else
                {
                    info.AlphaType = SkiaSharp.SKAlphaType.Unpremul;
                }
                // TODO update info
            }
        }

        /** Returns the bitmap's width */
        public int getWidth()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getWidth() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mWidth;
        }

        /** Returns the bitmap's height */
        public int getHeight()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getHeight() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mHeight;
        }

        /**
         * Convenience for calling {@link #getScaledWidth(int)} with the target
         * density of the given {@link Canvas}.
         */
        public int getScaledWidth(Canvas canvas)
        {
            return scaleFromDensity(getWidth(), mDensity, canvas.mDensity);
        }

        /**
         * Convenience for calling {@link #getScaledHeight(int)} with the target
         * density of the given {@link Canvas}.
         */
        public int getScaledHeight(Canvas canvas)
        {
            return scaleFromDensity(getHeight(), mDensity, canvas.mDensity);
        }

        /**
         * Convenience for calling {@link #getScaledWidth(int)} with the target
         * density of the given {@link DisplayMetrics}.
         */
        public int getScaledWidth(DisplayMetrics metrics)
        {
            return scaleFromDensity(getWidth(), mDensity, DensityManager.ScreenDpi);
        }

        /**
         * Convenience for calling {@link #getScaledHeight(int)} with the target
         * density of the given {@link DisplayMetrics}.
         */
        public int getScaledHeight(DisplayMetrics metrics)
        {
            return scaleFromDensity(getHeight(), mDensity, DensityManager.ScreenDpi);
        }

        /**
         * Convenience method that returns the width of this bitmap divided
         * by the density scale factor.
         *
         * Returns the bitmap's width multiplied by the ratio of the target density to the bitmap's
         * source density
         *
         * @param targetDensity The density of the target canvas of the bitmap.
         * @return The scaled width of this bitmap, according to the density scale factor.
         */
        public int getScaledWidth(int targetDensity)
        {
            return scaleFromDensity(getWidth(), mDensity, targetDensity);
        }

        /**
         * Convenience method that returns the height of this bitmap divided
         * by the density scale factor.
         *
         * Returns the bitmap's height multiplied by the ratio of the target density to the bitmap's
         * source density
         *
         * @param targetDensity The density of the target canvas of the bitmap.
         * @return The scaled height of this bitmap, according to the density scale factor.
         */
        public int getScaledHeight(int targetDensity)
        {
            return scaleFromDensity(getHeight(), mDensity, targetDensity);
        }

        /**
         * @hide
         * Must be public for access from android.graphics.drawable,
         * but must not be called from outside the UI module.
         */
        internal static int scaleFromDensity(int size, int sdensity, int tdensity)
        {
            if (sdensity == DENSITY_NONE || tdensity == DENSITY_NONE || sdensity == tdensity)
            {
                return size;
            }

            // Scale by tdensity / sdensity, rounding up.
            return ((size * tdensity) + (sdensity >> 1)) / sdensity;
        }

        /**
         * Return the number of bytes between rows in the bitmap's pixels. Note that
         * this refers to the pixels as stored natively by the bitmap. If you call
         * getPixels() or setPixels(), then the pixels are uniformly treated as
         * 32bit values, packed according to the Color class.
         *
         * <p>As of {@link android.os.Build.VERSION_CODES#KITKAT}, this method
         * should not be used to calculate the memory usage of the bitmap. Instead,
         * see {@link #getAllocationByteCount()}.
         *
         * @return number of bytes between rows of the native bitmap pixels.
         */
        public int getRowBytes()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getRowBytes() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mNativePtr.RowBytes;
        }

        /**
         * Returns the minimum number of bytes that can be used to store this bitmap's pixels.
         *
         * <p>As of {@link android.os.Build.VERSION_CODES#KITKAT}, the result of this method can
         * no longer be used to determine memory usage of a bitmap. See {@link
         * #getAllocationByteCount()}.</p>
         */
        public int getByteCount()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getByteCount() on a recycle()'d bitmap! "
                        + "This is undefined behavior!");
                return 0;
            }
            // int result permits bitmaps up to 46,340 x 46,340
            return getRowBytes() * getHeight();
        }

        /**
         * Returns the size of the allocated memory used to store this bitmap's pixels.
         *
         * <p>This can be larger than the result of {@link #getByteCount()} if a bitmap is reused to
         * decode other bitmaps of smaller size, or by manual reconfiguration. See {@link
         * #reconfigure(int, int, Config)}, {@link #setWidth(int)}, {@link #setHeight(int)}, {@link
         * #setConfig(Bitmap.Config)}, and {@link BitmapFactory.Options#inBitmap
         * BitmapFactory.Options.inBitmap}. If a bitmap is not modified in this way, this value will be
         * the same as that returned by {@link #getByteCount()}.</p>
         *
         * <p>This value will not change over the lifetime of a Bitmap.</p>
         *
         * @see #reconfigure(int, int, Config)
         */
        public int getAllocationByteCount()
        {
            if (mRecycled)
            {
                Log.w(TAG, "Called getAllocationByteCount() on a recycle()'d bitmap! "
                        + "This is undefined behavior!");
                return 0;
            }
            // TODO: verify
            return mNativePtr.ByteCount;
        }

        /**
         * If the bitmap's internal config is in one of the public formats, return
         * that config, otherwise return null.
         */
        public Config getConfig() {
            if (mRecycled)
            {
                Log.w(TAG, "Called getConfig() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return Config.nativeToConfig((int)mNativePtr.ColorType);
        }

        /** Returns true if the bitmap's config supports per-pixel alpha, and
         * if the pixels may contain non-opaque alpha values. For some configs,
         * this is always false (e.g. RGB_565), since they do not support per-pixel
         * alpha. However, for configs that do, the bitmap may be flagged to be
         * known that all of its pixels are opaque. In this case hasAlpha() will
         * also return false. If a config such as ARGB_8888 is not so flagged,
         * it will return true by default.
         */
        public bool hasAlpha() {
            if (mRecycled)
            {
                Log.w(TAG, "Called hasAlpha() on a recycle()'d bitmap! This is undefined behavior!");
            }
            return mNativePtr.Info.IsOpaque;
        }

        /**
         * Tell the bitmap if all of the pixels are known to be opaque (false)
         * or if some of the pixels may contain non-opaque alpha values (true).
         * Note, for some configs (e.g. RGB_565) this call is ignored, since it
         * does not support per-pixel alpha values.
         *
         * This is meant as a drawing hint, as in some cases a bitmap that is known
         * to be opaque can take a faster drawing case than one that may have
         * non-opaque per-pixel alpha values.
         */
        public void setHasAlpha(bool hasAlpha)
        {
            checkRecycled("setHasAlpha called on a recycled bitmap");
            SkiaSharp.SKImageInfo info = mNativePtr.Info;
            if (hasAlpha)
            {
                info.AlphaType = mRequestPremultiplied ? SkiaSharp.SKAlphaType.Premul : SkiaSharp.SKAlphaType.Unpremul;
            }
            else
            {
                info.AlphaType = SkiaSharp.SKAlphaType.Opaque;
            }
            // TODO update info
        }

        /**
         * Returns the color space associated with this bitmap. If the color
         * space is unknown, this method returns null.
         */
        public ColorSpace getColorSpace() {
            checkRecycled("getColorSpace called on a recycled bitmap");
            if (mColorSpace == null)
            {
                object namedCS = null;

                SkiaSharp.SKColorSpace decodedColorSpace = mNativePtr.Info.ColorSpace;
                SkiaSharp.SKColorType decodedColorType = mNativePtr.Info.ColorType;
                if (decodedColorSpace == null || decodedColorType == SkiaSharp.SKColorType.Alpha8)
                {
                    mColorSpace = null;
                }
                else
                {
                    // Special checks for the common sRGB cases and their extended variants.
                    SkiaSharp.SKColorSpace srgbLinear = SkiaSharp.SKColorSpace.CreateSrgbLinear();
                    if (decodedColorType == SkiaSharp.SKColorType.RgbaF16)
                    {
                        // An F16 Bitmap will always report that it is EXTENDED if
                        // it matches a ColorSpace that has an EXTENDED variant.
                        if (decodedColorSpace.IsSrgb)
                        {
                            namedCS = ColorSpace.Named.EXTENDED_SRGB;
                        }
                        else if (decodedColorSpace == srgbLinear)
                        {
                            namedCS = ColorSpace.Named.LINEAR_EXTENDED_SRGB;
                        }
                    }
                    else if (decodedColorSpace.IsSrgb)
                    {
                        namedCS = ColorSpace.Named.SRGB;
                    }
                    else if (decodedColorSpace == srgbLinear)
                    {
                        namedCS = ColorSpace.Named.LINEAR_SRGB;
                    }

                    if (namedCS != null)
                    {
                        mColorSpace = ColorSpace.get((ColorSpace.Named)namedCS);
                    }
                    else
                    {
                        // Try to match against known RGB color spaces using the CIE XYZ D50
                        // conversion matrix and numerical transfer function parameters

                        SkiaSharp.SKColorSpaceXyz xyzMatrix;

                        if (!decodedColorSpace.ToColorSpaceXyz(out xyzMatrix))
                        {
                            throw new Exception("could not obtain xyz matrix");
                        }

                        // We can only handle numerical transfer functions at the moment
                        SkiaSharp.SKColorSpaceTransferFn transferParams;
                        if (!decodedColorSpace.GetNumericalTransferFunction(out transferParams))
                        {
                            throw new Exception("We can only handle numerical transfer functions at the moment");
                        }

                        ColorSpace.Rgb.TransferParameters transfer = new ColorSpace.Rgb.TransferParameters(
                            transferParams.A, transferParams.B, transferParams.C, transferParams.D,
                            transferParams.E, transferParams.F, transferParams.G);

                        float[] xyzArray = xyzMatrix.Values;

                        ColorSpace colorspace = ColorSpace.match(xyzArray, transfer);
                        if (colorspace == null)
                        {
                            // We couldn't find an exact match, let's create a new color space
                            // instance with the 3x3 conversion matrix and transfer function
                            colorspace = new ColorSpace.Rgb("Unknown", xyzArray, transfer);
                        }
                        mColorSpace = colorspace;
                    }
                }
            }
            return mColorSpace;
        }

        /**
         * <p>Modifies the bitmap to have the specified {@link ColorSpace}, without
         * affecting the underlying allocation backing the bitmap.</p>
         *
         * <p>This affects how the framework will interpret the color at each pixel. A bitmap
         * with {@link Config#ALPHA_8} never has a color space, since a color space does not
         * affect the alpha channel. Other {@code Config}s must always have a non-null
         * {@code ColorSpace}.</p>
         *
         * @throws IllegalArgumentException If the specified color space is {@code null}, not
         *         {@link ColorSpace.Model#RGB RGB}, has a transfer function that is not an
         *         {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}, or whose
         *         components min/max values reduce the numerical range compared to the
         *         previously assigned color space.
         *
         * @throws IllegalArgumentException If the {@code Config} (returned by {@link #getConfig()})
         *         is {@link Config#ALPHA_8}.
         *
         * @param colorSpace to assign to the bitmap
         */
        public void setColorSpace(ColorSpace colorSpace)
        {
            checkRecycled("setColorSpace called on a recycled bitmap");
            if (colorSpace == null)
            {
                throw new IllegalArgumentException("The colorSpace cannot be set to null");
            }

            if (getConfig() == Config.ALPHA_8)
            {
                throw new IllegalArgumentException("Cannot set a ColorSpace on ALPHA_8");
            }

            // Keep track of the old ColorSpace for comparison, and so we can reset it in case of an
            // Exception.
            ColorSpace oldColorSpace = getColorSpace();
            SkiaSharp.SKImageInfo info = mNativePtr.Info;
            info.ColorSpace = colorSpace.getNativeInstance();
            // TODO: update info

            // This will update mColorSpace. It may not be the same as |colorSpace|, e.g. if we
            // corrected it because the Bitmap is F16.
            mColorSpace = null;
            ColorSpace newColorSpace = getColorSpace();

            try
            {
                if (oldColorSpace.getComponentCount() != newColorSpace.getComponentCount())
                {
                    throw new IllegalArgumentException("The new ColorSpace must have the same "
                            + "component count as the current ColorSpace");
                }
                else
                {
                    for (int i = 0; i < oldColorSpace.getComponentCount(); i++)
                    {
                        if (oldColorSpace.getMinValue(i) < newColorSpace.getMinValue(i))
                        {
                            throw new IllegalArgumentException("The new ColorSpace cannot increase the "
                                    + "minimum value for any of the components compared to the current "
                                    + "ColorSpace. To perform this type of conversion create a new "
                                    + "Bitmap in the desired ColorSpace and draw this Bitmap into it.");
                        }
                        if (oldColorSpace.getMaxValue(i) > newColorSpace.getMaxValue(i))
                        {
                            throw new IllegalArgumentException("The new ColorSpace cannot decrease the "
                                    + "maximum value for any of the components compared to the current "
                                    + "ColorSpace/ To perform this type of conversion create a new "
                                    + "Bitmap in the desired ColorSpace and draw this Bitmap into it.");
                        }
                    }
                }
            }
            catch (IllegalArgumentException e)
            {
                // Undo the change to the ColorSpace.
                mColorSpace = oldColorSpace;
                SkiaSharp.SKImageInfo info = mNativePtr.Info;
                info.ColorSpace = mColorSpace.getNativeInstance();
                // TODO: update info
                throw e;
            }
        }

        /**
         * Fills the bitmap's pixels with the specified {@link Color}.
         *
         * @throws IllegalStateException if the bitmap is not mutable.
         */
        public void eraseColor(int color)
        {
            checkRecycled("Can't erase a recycled bitmap");
            if (!isMutable())
            {
                throw new IllegalStateException("cannot erase immutable bitmaps");
            }

            SkiaSharp.SKPaint p = new();
            p.SetColor(Color.toSKColor(color), mNativePtr.ColorSpace);
            p.BlendMode = SkiaSharp.SKBlendMode.Src;
            SkiaSharp.SKCanvas canvas = new(mNativePtr);
            canvas.DrawPaint(p);
            canvas.Dispose();
        }

        /**
         * Fills the bitmap's pixels with the specified {@code ColorLong}.
         *
         * @param color The color to fill as packed by the {@link Color} class.
         * @throws IllegalStateException if the bitmap is not mutable.
         * @throws IllegalArgumentException if the color space encoded in the
         *                                  {@code ColorLong} is invalid or unknown.
         *
         */
        public void eraseColor(long color)
        {
            checkRecycled("Can't erase a recycled bitmap");
            if (!isMutable())
            {
                throw new IllegalStateException("cannot erase immutable bitmaps");
            }

            ColorSpace cs = Color.colorSpace(color);
            SkiaSharp.SKPaint p = new();
            p.SetColor(Color.toSKColorF(color), cs.getNativeInstance());
            p.BlendMode = SkiaSharp.SKBlendMode.Src;
            SkiaSharp.SKCanvas canvas = new(mNativePtr);
            canvas.DrawPaint(p);
            canvas.Dispose();
        }

        /**
         * Returns the {@link Color} at the specified location. Throws an exception
         * if x or y are out of bounds (negative or >= to the width or height
         * respectively). The returned color is a non-premultiplied ARGB value in
         * the {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param x    The x coordinate (0...width-1) of the pixel to return
         * @param y    The y coordinate (0...height-1) of the pixel to return
         * @return     The argb {@link Color} at the specified coordinate
         * @throws IllegalArgumentException if x, y exceed the bitmap's bounds
         * @throws IllegalStateException if the bitmap's config is {@link Config#HARDWARE}
         */
        public int getPixel(int x, int y)
        {
            checkRecycled("Can't call getPixel() on a recycled bitmap");
            checkHardware("unable to getPixel(), "
                    + "pixel access is not supported on Config#HARDWARE bitmaps");
            checkPixelAccess(x, y);
            return (int)(uint)mNativePtr.GetPixel(x, y);
        }

        private static float clamp(float value, ColorSpace cs, int index)
        {
            return Math.Max(Math.Min(value, cs.getMaxValue(index)), cs.getMinValue(index));
        }

        /**
         * Returns the {@link Color} at the specified location. Throws an exception
         * if x or y are out of bounds (negative or >= to the width or height
         * respectively).
         *
         * @param x    The x coordinate (0...width-1) of the pixel to return
         * @param y    The y coordinate (0...height-1) of the pixel to return
         * @return     The {@link Color} at the specified coordinate
         * @throws IllegalArgumentException if x, y exceed the bitmap's bounds
         * @throws IllegalStateException if the bitmap's config is {@link Config#HARDWARE}
         *
         */
        public Color getColor(int x, int y)
        {
            checkRecycled("Can't call getColor() on a recycled bitmap");
            checkHardware("unable to getColor(), "
                    + "pixel access is not supported on Config#HARDWARE bitmaps");
            checkPixelAccess(x, y);

            ColorSpace cs = getColorSpace();
            if (cs.Equals(ColorSpace.get(ColorSpace.Named.SRGB)))
            {
                return Color.valueOf((uint)mNativePtr.GetPixel(x, y));
            }
            // The returned value is in kRGBA_F16_SkColorType, which is packed as
            // four half-floats, r,g,b,a.
            long rgba = (uint)mNativePtr.GetPixel(x, y);
            float r = Half.toFloat((short)((rgba >> 0) & 0xffff));
            float g = Half.toFloat((short)((rgba >> 16) & 0xffff));
            float b = Half.toFloat((short)((rgba >> 32) & 0xffff));
            float a = Half.toFloat((short)((rgba >> 48) & 0xffff));

            // Skia may draw outside of the numerical range of the colorSpace.
            // Clamp to get an expected value.
            return Color.valueOf(clamp(r, cs, 0), clamp(g, cs, 1), clamp(b, cs, 2), a, cs);
        }

        /**
         * Returns in pixels[] a copy of the data in the bitmap. Each value is
         * a packed int representing a {@link Color}. The stride parameter allows
         * the caller to allow for gaps in the returned pixels array between
         * rows. For normal packed results, just pass width for the stride value.
         * The returned colors are non-premultiplied ARGB values in the
         * {@link ColorSpace.Named#SRGB sRGB} color space.
         *
         * @param pixels   The array to receive the bitmap's colors
         * @param offset   The first index to write into pixels[]
         * @param stride   The number of entries in pixels[] to skip between
         *                 rows (must be >= bitmap's width). Can be negative.
         * @param x        The x coordinate of the first pixel to read from
         *                 the bitmap
         * @param y        The y coordinate of the first pixel to read from
         *                 the bitmap
         * @param width    The number of pixels to read from each row
         * @param height   The number of rows to read
         *
         * @throws IllegalArgumentException if x, y, width, height exceed the
         *         bounds of the bitmap, or if abs(stride) < width.
         * @throws ArrayIndexOutOfBoundsException if the pixels array is too small
         *         to receive the specified number of pixels.
         * @throws IllegalStateException if the bitmap's config is {@link Config#HARDWARE}
         */
        public void getPixels(int[] pixels, int offset, int stride,
                              int x, int y, int width, int height)
        {
            checkRecycled("Can't call getPixels() on a recycled bitmap");
            checkHardware("unable to getPixels(), "
                    + "pixel access is not supported on Config#HARDWARE bitmaps");
            if (width == 0 || height == 0)
            {
                return; // nothing to do
            }
            checkPixelsAccess(x, y, width, height, offset, stride, pixels);
            SkiaSharp.SKPixmap p = new();
            if (!mNativePtr.PeekPixels(p))
            {
                return;
            }
            unsafe
            {
                fixed (int* ptr = pixels)
                {
                    p.ReadPixels(mNativePtr.Info, (IntPtr)(ptr + offset), stride * 4, x, y);
                }
            }
            p.Dispose();
        }

        /**
         * Shared code to check for illegal arguments passed to getPixel()
         * or setPixel()
         *
         * @param x x coordinate of the pixel
         * @param y y coordinate of the pixel
         */
        private void checkPixelAccess(int x, int y)
        {
            checkXYSign(x, y);
            if (x >= getWidth())
            {
                throw new IllegalArgumentException("x must be < bitmap.width()");
            }
            if (y >= getHeight())
            {
                throw new IllegalArgumentException("y must be < bitmap.height()");
            }
        }

        /**
         * Shared code to check for illegal arguments passed to getPixels()
         * or setPixels()
         *
         * @param x left edge of the area of pixels to access
         * @param y top edge of the area of pixels to access
         * @param width width of the area of pixels to access
         * @param height height of the area of pixels to access
         * @param offset offset into pixels[] array
         * @param stride number of elements in pixels[] between each logical row
         * @param pixels array to hold the area of pixels being accessed
*/
        private void checkPixelsAccess(int x, int y, int width, int height,
                                       int offset, int stride, int[] pixels)
        {
            checkXYSign(x, y);
            if (width < 0)
            {
                throw new IllegalArgumentException("width must be >= 0");
            }
            if (height < 0)
            {
                throw new IllegalArgumentException("height must be >= 0");
            }
            if (x + width > getWidth())
            {
                throw new IllegalArgumentException(
                        "x + width must be <= bitmap.width()");
            }
            if (y + height > getHeight())
            {
                throw new IllegalArgumentException(
                        "y + height must be <= bitmap.height()");
            }
            if (Math.Abs(stride) < width)
            {
                throw new IllegalArgumentException("abs(stride) must be >= width");
            }
            int lastScanline = offset + (height - 1) * stride;
            int length = pixels.Length;
            if (offset < 0 || (offset + width > length)
                    || lastScanline < 0
                    || (lastScanline + width > length))
            {
                throw new IndexOutOfRangeException();
            }
        }

        /**
         * <p>Write the specified {@link Color} into the bitmap (assuming it is
         * mutable) at the x,y coordinate. The color must be a
         * non-premultiplied ARGB value in the {@link ColorSpace.Named#SRGB sRGB}
         * color space.</p>
         *
         * @param x     The x coordinate of the pixel to replace (0...width-1)
         * @param y     The y coordinate of the pixel to replace (0...height-1)
         * @param color The ARGB color to write into the bitmap
         *
         * @throws IllegalStateException if the bitmap is not mutable
         * @throws IllegalArgumentException if x, y are outside of the bitmap's
         *         bounds.
         */
        public void setPixel(int x, int y, int color)
        {
            checkRecycled("Can't call setPixel() on a recycled bitmap");
            if (!isMutable())
            {
                throw new IllegalStateException();
            }
            checkPixelAccess(x, y);
            mNativePtr.SetPixel(x, y, Color.toSKColor(color));
        }

        /**
         * <p>Replace pixels in the bitmap with the colors in the array. Each element
         * in the array is a packed int representing a non-premultiplied ARGB
         * {@link Color} in the {@link ColorSpace.Named#SRGB sRGB} color space.</p>
         *
         * @param pixels   The colors to write to the bitmap
         * @param offset   The index of the first color to read from pixels[]
         * @param stride   The number of colors in pixels[] to skip between rows.
         *                 Normally this value will be the same as the width of
         *                 the bitmap, but it can be larger (or negative).
         * @param x        The x coordinate of the first pixel to write to in
         *                 the bitmap.
         * @param y        The y coordinate of the first pixel to write to in
         *                 the bitmap.
         * @param width    The number of colors to copy from pixels[] per row
         * @param height   The number of rows to write to the bitmap
         *
         * @throws IllegalStateException if the bitmap is not mutable
         * @throws IllegalArgumentException if x, y, width, height are outside of
         *         the bitmap's bounds.
         * @throws ArrayIndexOutOfBoundsException if the pixels array is too small
         *         to receive the specified number of pixels.
         */
        public void setPixels(int[] pixels, int offset, int stride,
                int x, int y, int width, int height)
        {
            checkRecycled("Can't call setPixels() on a recycled bitmap");
            if (!isMutable())
            {
                throw new IllegalStateException();
            }
            if (width == 0 || height == 0)
            {
                return; // nothing to do
            }
            checkPixelsAccess(x, y, width, height, offset, stride, pixels);

            unsafe
            {
                fixed (int* ptr = pixels)
                {
                    int* src = ptr + offset;

                    SkiaSharp.SKColorSpace sRGB = SkiaSharp.SKColorSpace.CreateSrgb();

                    SkiaSharp.SKImageInfo srcInfo = new SkiaSharp.SKImageInfo(width, height, SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Unpremul, sRGB);

                    SkiaSharp.SKPixmap srcPM = new(srcInfo, (IntPtr)src, stride * 4);
                    srcPM.ReadPixels(srcInfo, mNativePtr.GetPixels(), srcInfo.RowBytes);
                    srcPM.Dispose();
                }
            }
        }

        /**
         * Returns a new bitmap that captures the alpha values of the original.
         * This may be drawn with Canvas.drawBitmap(), where the color(s) will be
         * taken from the paint that is passed to the draw call.
         *
         * @return new bitmap containing the alpha channel of the original bitmap.
         */
        public Bitmap extractAlpha()
        {
            return extractAlpha(null, null);
        }

        /**
         * Returns a new bitmap that captures the alpha values of the original.
         * These values may be affected by the optional Paint parameter, which
         * can contain its own alpha, and may also contain a MaskFilter which
         * could change the actual dimensions of the resulting bitmap (e.g.
         * a blur maskfilter might enlarge the resulting bitmap). If offsetXY
         * is not null, it returns the amount to offset the returned bitmap so
         * that it will logically align with the original. For example, if the
         * paint contains a blur of radius 2, then offsetXY[] would contains
         * -2, -2, so that drawing the alpha bitmap offset by (-2, -2) and then
         * drawing the original would result in the blur visually aligning with
         * the original.
         *
         * <p>The initial density of the returned bitmap is the same as the original's.
         *
         * @param paint Optional paint used to modify the alpha values in the
         *              resulting bitmap. Pass null for default behavior.
         * @param offsetXY Optional array that returns the X (index 0) and Y
         *                 (index 1) offset needed to position the returned bitmap
         *                 so that it visually lines up with the original.
         * @return new bitmap containing the (optionally modified by paint) alpha
         *         channel of the original bitmap. This may be drawn with
         *         Canvas.drawBitmap(), where the color(s) will be taken from the
         *         paint that is passed to the draw call.
         */
        public Bitmap extractAlpha(Paint paint, int[] offsetXY)
        {
            checkRecycled("Can't extractAlpha on a recycled bitmap");
            SkiaSharp.SKPaint nativePaint = paint?.getNativeInstance();
            noteHardwareBitmapSlowCall();
            SkiaSharp.SKPointI o;
            SkiaSharp.SKBitmap bitmap = new(mNativePtr.Info);
            if (!mNativePtr.ExtractAlpha(bitmap, nativePaint, out o))
            {
                throw new Exception("Failed to extractAlpha on Bitmap");
            }
            if (offsetXY != null)
            {
                offsetXY[0] = o.X;
                offsetXY[1] = o.Y;
            }
            return new Bitmap(bitmap, mNativePtr.Width, mNativePtr.Height, mDensity, mRequestPremultiplied, null, null);
        }

        /**
         *  Given another bitmap, return true if it has the same dimensions, config,
         *  and pixel data as this bitmap. If any of those differ, return false.
         *  If other is null, return false.
         */
        public bool sameAs(Bitmap other)
        {
            checkRecycled("Can't call sameAs on a recycled bitmap!");
            noteHardwareBitmapSlowCall();
            if (this == other) return true;
            if (other == null) return false;
            other.noteHardwareBitmapSlowCall();
            if (other.isRecycled())
            {
                throw new IllegalArgumentException("Can't compare to a recycled bitmap!");
            }
            SkiaSharp.SKBitmap bm0 = mNativePtr;
            SkiaSharp.SKBitmap bm1 = other.mNativePtr;

            // Paying the price for making Hardware Bitmap as Config:
            // later check for colorType will pass successfully,
            // because Hardware Config internally may be RGBA8888 or smth like that.
            if ((getConfig() == Config.HARDWARE) != (other.getConfig() == Config.HARDWARE))
            {
                return false;
            }
            if (bm0.Width != bm1.Width
        || bm0.Height != bm1.Height
        || bm0.ColorType != bm1.ColorType
        || bm0.AlphaType != bm1.AlphaType
        || !SkiaSharp.SKColorSpace.Equals(bm0.ColorSpace, bm1.ColorSpace))
            {
                return false;
            }

            // if we can't load the pixels, return false
            if (IntPtr.Zero == bm0.GetPixels() || IntPtr.Zero == bm1.GetPixels())
            {
                return false;
            }

            // now compare each scanline. We can't do the entire buffer at once,
            // since we don't care about the pixel values that might extend beyond
            // the width (since the scanline might be larger than the logical width)
            int h = bm0.Height;
            long size = bm0.Width * bm0.BytesPerPixel;
            for (int y = 0; y < h; y++)
            {
                // SkBitmap::getAddr(int, int) may return NULL due to unrecognized config
                // (ex: kRLE_Index8_Config). This will cause memcmp method to crash. Since bm0
                // and bm1 both have pixel data() (have passed NULL == getPixels() check),
                // those 2 bitmaps should be valid (only unrecognized), we return JNI_FALSE
                // to warn user those 2 unrecognized config bitmaps may be different.
                unsafe
                {
                    void* bm0Addr = bm0.GetAddr(0, y).ToPointer();
                    void* bm1Addr = bm1.GetAddr(0, y).ToPointer();

                    if (bm0Addr == null || bm1Addr == null)
                    {
                        return true;
                    }

                    [System.Runtime.InteropServices.DllImport(
                        "msvcrt.dll",
                        CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl
                    )]
                    static extern int memcmp(void* b1, void* b2, long size);

                    if (memcmp(bm0Addr, bm1Addr, size) != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Builds caches associated with the bitmap that are used for drawing it.
         *
         * <p>Starting in {@link android.os.Build.VERSION_CODES#N}, this call initiates an asynchronous
         * upload to the GPU on RenderThread, if the Bitmap is not already uploaded. With Hardware
         * Acceleration, Bitmaps must be uploaded to the GPU in order to be rendered. This is done by
         * default the first time a Bitmap is drawn, but the process can take several milliseconds,
         * depending on the size of the Bitmap. Each time a Bitmap is modified and drawn again, it must
         * be re-uploaded.</p>
         *
         * <p>Calling this method in advance can save time in the first frame it's used. For example, it
         * is recommended to call this on an image decoding worker thread when a decoded Bitmap is about
         * to be displayed. It is recommended to make any pre-draw modifications to the Bitmap before
         * calling this method, so the cached, uploaded copy may be reused without re-uploading.</p>
         *
         * In {@link android.os.Build.VERSION_CODES#KITKAT} and below, for purgeable bitmaps, this call
         * would attempt to ensure that the pixels have been decoded.
         */
        public void prepareToDraw()
        {
            checkRecycled("Can't prepareToDraw on a recycled bitmap!");
            // Kick off an update/upload of the bitmap outside of the normal
            // draw path.
            // TODO
            //nativePrepareToDraw(mNativePtr);
        }
    }
}
