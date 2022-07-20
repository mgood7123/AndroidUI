/*
 * Copyright (C) 2007 The Android Open Source Project
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
using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;
using AndroidUI.Utils.Skia;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    /**
     * Creates Bitmap objects from various sources, including files, streams,
     * and byte-arrays.
     */
    public class BitmapFactory
    {

        private const int DECODE_BUFFER_SIZE = 16 * 1024;

        public class Options
        {
            /**
             * Create a default Options object, which if left unchanged will give
             * the same result from the decoder as if null were passed.
             */
            public Options()
            {
                inScaled = true;
                inPremultiplied = true;
            }

            /**
             * If set, decode methods that take the Options object will attempt to
             * reuse this bitmap when loading content. If the decode operation
             * cannot use this bitmap, the decode method will throw an
             * {@link java.lang.IllegalArgumentException}. The
             * current implementation necessitates that the reused bitmap be
             * mutable, and the resulting reused bitmap will continue to remain
             * mutable even when decoding a resource which would normally result in
             * an immutable bitmap.</p>
             *
             * <p>You should still always use the returned Bitmap of the decode
             * method and not assume that reusing the bitmap worked, due to the
             * constraints outlined above and failure situations that can occur.
             * Checking whether the return value matches the value of the inBitmap
             * set in the Options structure will indicate if the bitmap was reused,
             * but in all cases you should use the Bitmap returned by the decoding
             * function to ensure that you are using the bitmap that was used as the
             * decode destination.</p>
             *
             * <h3>Usage with BitmapFactory</h3>
             *
             * <p>As of {@link android.os.Build.VERSION_CODES#KITKAT}, any
             * mutable bitmap can be reused by {@link BitmapFactory} to decode any
             * other bitmaps as long as the resulting {@link Bitmap#getByteCount()
             * byte count} of the decoded bitmap is less than or equal to the {@link
             * Bitmap#getAllocationByteCount() allocated byte count} of the reused
             * bitmap. This can be because the intrinsic size is smaller, or its
             * size post scaling (for density / sample size) is smaller.</p>
             *
             * <p class="note">Prior to {@link android.os.Build.VERSION_CODES#KITKAT}
             * additional constraints apply: The image being decoded (whether as a
             * resource or as a stream) must be in jpeg or png format. Only equal
             * sized bitmaps are supported, with {@link #inSampleSize} set to 1.
             * Additionally, the {@link android.graphics.Bitmap.Config
             * configuration} of the reused bitmap will override the setting of
             * {@link #inPreferredConfig}, if set.</p>
             *
             * <h3>Usage with BitmapRegionDecoder</h3>
             *
             * <p>BitmapRegionDecoder will draw its requested content into the Bitmap
             * provided, clipping if the output content size (post scaling) is larger
             * than the provided Bitmap. The provided Bitmap's width, height, and
             * {@link Bitmap.Config} will not be changed.
             *
             * <p class="note">BitmapRegionDecoder support for {@link #inBitmap} was
             * introduced in {@link android.os.Build.VERSION_CODES#JELLY_BEAN}. All
             * formats supported by BitmapRegionDecoder support Bitmap reuse via
             * {@link #inBitmap}.</p>
             *
             * @see Bitmap#reconfigure(int,int, android.graphics.Bitmap.Config)
             */
            public Bitmap inBitmap;

            /**
             * If set, decode methods will always return a mutable Bitmap instead of
             * an immutable one. This can be used for instance to programmatically apply
             * effects to a Bitmap loaded through BitmapFactory.
             * <p>Can not be set simultaneously with inPreferredConfig =
             * {@link android.graphics.Bitmap.Config#HARDWARE},
             * because hardware bitmaps are always immutable.
             */
            public bool inMutable;

            /**
             * If set to true, the decoder will return null (no bitmap), but
             * the <code>out...</code> fields will still be set, allowing the caller to
             * query the bitmap without having to allocate the memory for its pixels.
             */
            public bool inJustDecodeBounds;

            /**
             * If set to a value > 1, requests the decoder to subsample the original
             * image, returning a smaller image to save memory. The sample size is
             * the number of pixels in either dimension that correspond to a single
             * pixel in the decoded bitmap. For example, inSampleSize == 4 returns
             * an image that is 1/4 the width/height of the original, and 1/16 the
             * number of pixels. Any value <= 1 is treated the same as 1. Note: the
             * decoder uses a final value based on powers of 2, any other value will
             * be rounded down to the nearest power of 2.
             */
            public int inSampleSize;

            /**
             * If this is non-null, the decoder will try to decode into this
             * internal configuration. If it is null, or the request cannot be met,
             * the decoder will try to pick the best matching config based on the
             * system's screen depth, and characteristics of the original image such
             * as if it has per-pixel alpha (requiring a config that also does).
             * 
             * Image are loaded with the {@link Bitmap.Config#ARGB_8888} config by
             * default.
             */
            public Bitmap.Config inPreferredConfig = Bitmap.Config.ARGB_8888;

            /**
             * <p>If this is non-null, the decoder will try to decode into this
             * color space. If it is null, or the request cannot be met,
             * the decoder will pick either the color space embedded in the image
             * or the color space best suited for the requested image configuration
             * (for instance {@link ColorSpace.Named#SRGB sRGB} for
             * {@link Bitmap.Config#ARGB_8888} configuration and
             * {@link ColorSpace.Named#EXTENDED_SRGB EXTENDED_SRGB} for
             * {@link Bitmap.Config#RGBA_F16}).</p>
             *
             * <p class="note">Only {@link ColorSpace.Model#RGB} color spaces are
             * currently supported. An <code>IllegalArgumentException</code> will
             * be thrown by the decode methods when setting a non-RGB color space
             * such as {@link ColorSpace.Named#CIE_LAB Lab}.</p>
             *
             * <p class="note">The specified color space's transfer function must be
             * an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}. An
             * <code>IllegalArgumentException</code> will be thrown by the decode methods
             * if calling {@link ColorSpace.Rgb#getTransferParameters()} on the
             * specified color space returns null.</p>
             *
             * <p>After decode, the bitmap's color space is stored in
             * {@link #outColorSpace}.</p>
             */
            public ColorSpace inPreferredColorSpace = null;

            /**
             * If true (which is the default), the resulting bitmap will have its
             * color channels pre-multipled by the alpha channel.
             *
             * <p>This should NOT be set to false for images to be directly drawn by
             * the view system or through a {@link Canvas}. The view system and
             * {@link Canvas} assume all drawn images are pre-multiplied to simplify
             * draw-time blending, and will throw a RuntimeException when
             * un-premultiplied are drawn.</p>
             *
             * <p>This is likely only useful if you want to manipulate raw encoded
             * image data, e.g. with RenderScript or custom OpenGL.</p>
             *
             * <p>This does not affect bitmaps without an alpha channel.</p>
             *
             * <p>Setting this flag to false while setting {@link #inScaled} to true
             * may result in incorrect colors.</p>
             *
             * @see Bitmap#hasAlpha()
             * @see Bitmap#isPremultiplied()
             * @see #inScaled
             */
            public bool inPremultiplied;

            /**
             * @deprecated As of {@link android.os.Build.VERSION_CODES#N}, this is
             * ignored.
             *
             * In {@link android.os.Build.VERSION_CODES#M} and below, if dither is
             * true, the decoder will attempt to dither the decoded image.
             */
            public bool inDither;

            /**
             * The pixel density to use for the bitmap.  This will always result
             * in the returned bitmap having a density set for it (see
             * {@link Bitmap#setDensity(int) Bitmap.setDensity(int)}).  In addition,
             * if {@link #inScaled} is set (which it is by default} and this
             * density does not match {@link #inTargetDensity}, then the bitmap
             * will be scaled to the target density before being returned.
             * 
             * <p>If this is 0,
             * {@link BitmapFactory#decodeResource(Resources, int)}, 
             * {@link BitmapFactory#decodeResource(Resources, int, android.graphics.BitmapFactory.Options)},
             * and {@link BitmapFactory#decodeResourceStream}
             * will fill in the density associated with the resource.  The other
             * functions will leave it as-is and no density will be applied.
             *
             * @see #inTargetDensity
             * @see #inScreenDensity
             * @see #inScaled
             * @see Bitmap#setDensity(int)
             * @see android.util.DisplayMetrics#densityDpi
             */
            public int inDensity;

            /**
             * The pixel density of the destination this bitmap will be drawn to.
             * This is used in conjunction with {@link #inDensity} and
             * {@link #inScaled} to determine if and how to scale the bitmap before
             * returning it.
             * 
             * <p>If this is 0,
             * {@link BitmapFactory#decodeResource(Resources, int)}, 
             * {@link BitmapFactory#decodeResource(Resources, int, android.graphics.BitmapFactory.Options)},
             * and {@link BitmapFactory#decodeResourceStream}
             * will fill in the density associated the Resources object's
             * DisplayMetrics.  The other
             * functions will leave it as-is and no scaling for density will be
             * performed.
             * 
             * @see #inDensity
             * @see #inScreenDensity
             * @see #inScaled
             * @see android.util.DisplayMetrics#densityDpi
             */
            public int inTargetDensity;

            /**
             * The pixel density of the actual screen that is being used.  This is
             * purely for applications running in density compatibility code, where
             * {@link #inTargetDensity} is actually the density the application
             * sees rather than the real screen density.
             * 
             * <p>By setting this, you
             * allow the loading code to avoid scaling a bitmap that is currently
             * in the screen density up/down to the compatibility density.  Instead,
             * if {@link #inDensity} is the same as {@link #inScreenDensity}, the
             * bitmap will be left as-is.  Anything using the resulting bitmap
             * must also used {@link Bitmap#getScaledWidth(int)
             * Bitmap.getScaledWidth} and {@link Bitmap#getScaledHeight
             * Bitmap.getScaledHeight} to account for any different between the
             * bitmap's density and the target's density.
             * 
             * <p>This is never set automatically for the caller by
             * {@link BitmapFactory} itself.  It must be explicitly set, since the
             * caller must deal with the resulting bitmap in a density-aware way.
             * 
             * @see #inDensity
             * @see #inTargetDensity
             * @see #inScaled
             * @see android.util.DisplayMetrics#densityDpi
             */
            public int inScreenDensity;

            /**
             * When this flag is set, if {@link #inDensity} and
             * {@link #inTargetDensity} are not 0, the
             * bitmap will be scaled to match {@link #inTargetDensity} when loaded,
             * rather than relying on the graphics system scaling it each time it
             * is drawn to a Canvas.
             *
             * <p>BitmapRegionDecoder ignores this flag, and will not scale output
             * based on density. (though {@link #inSampleSize} is supported)</p>
             *
             * <p>This flag is turned on by default and should be turned off if you need
             * a non-scaled version of the bitmap.  Nine-patch bitmaps ignore this
             * flag and are always scaled.
             *
             * <p>If {@link #inPremultiplied} is set to false, and the image has alpha,
             * setting this flag to true may result in incorrect colors.
             */
            public bool inScaled;

            /**
             * @deprecated As of {@link android.os.Build.VERSION_CODES#LOLLIPOP}, this is
             * ignored.
             *
             * In {@link android.os.Build.VERSION_CODES#KITKAT} and below, if this
             * is set to true, then the resulting bitmap will allocate its
             * pixels such that they can be purged if the system needs to reclaim
             * memory. In that instance, when the pixels need to be accessed again
             * (e.g. the bitmap is drawn, getPixels() is called), they will be
             * automatically re-decoded.
             *
             * <p>For the re-decode to happen, the bitmap must have access to the
             * encoded data, either by sharing a reference to the input
             * or by making a copy of it. This distinction is controlled by
             * inInputShareable. If this is true, then the bitmap may keep a shallow
             * reference to the input. If this is false, then the bitmap will
             * explicitly make a copy of the input data, and keep that. Even if
             * sharing is allowed, the implementation may still decide to make a
             * deep copy of the input data.</p>
             *
             * <p>While inPurgeable can help avoid big Dalvik heap allocations (from
             * API level 11 onward), it sacrifices performance predictability since any
             * image that the view system tries to draw may incur a decode delay which
             * can lead to dropped frames. Therefore, most apps should avoid using
             * inPurgeable to allow for a fast and fluid UI. To minimize Dalvik heap
             * allocations use the {@link #inBitmap} flag instead.</p>
             *
             * <p class="note"><strong>Note:</strong> This flag is ignored when used
             * with {@link #decodeResource(Resources, int,
             * android.graphics.BitmapFactory.Options)} or {@link #decodeFile(String,
             * android.graphics.BitmapFactory.Options)}.</p>
             */
            [Obsolete]
            public bool inPurgeable;

            /**
             * @deprecated As of {@link android.os.Build.VERSION_CODES#LOLLIPOP}, this is
             * ignored.
             *
             * In {@link android.os.Build.VERSION_CODES#KITKAT} and below, this
             * field works in conjuction with inPurgeable. If inPurgeable is false,
             * then this field is ignored. If inPurgeable is true, then this field
             * determines whether the bitmap can share a reference to the input
             * data (inputstream, array, etc.) or if it must make a deep copy.
             */
            [Obsolete]
            public bool inInputShareable;

            /**
             * @deprecated As of {@link android.os.Build.VERSION_CODES#N}, this is
             * ignored.  The output will always be high quality.
             *
             * In {@link android.os.Build.VERSION_CODES#M} and below, if
             * inPreferQualityOverSpeed is set to true, the decoder will try to
             * decode the reconstructed image to a higher quality even at the
             * expense of the decoding speed. Currently the field only affects JPEG
             * decode, in the case of which a more accurate, but slightly slower,
             * IDCT method will be used instead.
             */
            [Obsolete]
            public bool inPreferQualityOverSpeed;

            /**
             * The resulting width of the bitmap. If {@link #inJustDecodeBounds} is
             * set to false, this will be width of the output bitmap after any
             * scaling is applied. If true, it will be the width of the input image
             * without any accounting for scaling.
             *
             * <p>outWidth will be set to -1 if there is an error trying to decode.</p>
             */
            public int outWidth;

            /**
             * The resulting height of the bitmap. If {@link #inJustDecodeBounds} is
             * set to false, this will be height of the output bitmap after any
             * scaling is applied. If true, it will be the height of the input image
             * without any accounting for scaling.
             *
             * <p>outHeight will be set to -1 if there is an error trying to decode.</p>
             */
            public int outHeight;

            /**
             * If known, this string is set to the mimetype of the decoded image.
             * If not known, or there is an error, it is set to null.
             */
            public string outMimeType;

            /**
             * If known, the config the decoded bitmap will have.
             * If not known, or there is an error, it is set to null.
             */
            public Bitmap.Config outConfig;

            /**
             * If known, the color space the decoded bitmap will have. Note that the
             * output color space is not guaranteed to be the color space the bitmap
             * is encoded with. If not known (when the config is
             * {@link Bitmap.Config#ALPHA_8} for instance), or there is an error,
             * it is set to null.
             */
            public ColorSpace outColorSpace;

            /**
             * Temp storage size to use for decoding.  Suggest 16K or so.
             */
            public int inTempStorageSize;

            /**
             * @deprecated As of {@link android.os.Build.VERSION_CODES#N}, see
             * comments on {@link #requestCancelDecode()}.
             *
             * Flag to indicate that cancel has been called on this object.  This
             * is useful if there's an intermediary that wants to first decode the
             * bounds and then decode the image.  In that case the intermediary
             * can check, inbetween the bounds decode and the image decode, to see
             * if the operation is canceled.
             */
            [Obsolete]
            public bool mCancel;

            /**
             *  @deprecated As of {@link android.os.Build.VERSION_CODES#N}, this
             *  will not affect the decode, though it will still set mCancel.
             *
             *  In {@link android.os.Build.VERSION_CODES#M} and below, if this can
             *  be called from another thread while this options object is inside
             *  a decode... call. Calling this will notify the decoder that it
             *  should cancel its operation. This is not guaranteed to cancel the
             *  decode, but if it does, the decoder... operation will return null,
             *  or if inJustDecodeBounds is true, will set outWidth/outHeight
             *  to -1
             */
            [Obsolete]
            public void requestCancelDecode()
            {
                mCancel = true;
            }

            internal static void validate(Options opts)
            {
                if (opts == null) return;

                if (opts.inBitmap != null)
                {
                    if (opts.inBitmap.getConfig() == Bitmap.Config.HARDWARE)
                    {
                        throw new IllegalArgumentException(
                                "Bitmaps with Config.HARDWARE are always immutable");
                    }
                    if (opts.inBitmap.isRecycled())
                    {
                        throw new IllegalArgumentException(
                                "Cannot reuse a recycled Bitmap");
                    }
                }

                if (opts.inMutable && opts.inPreferredConfig == Bitmap.Config.HARDWARE)
                {
                    throw new IllegalArgumentException("Bitmaps with Config.HARDWARE cannot be " +
                            "decoded into - they are immutable");
                }

                if (opts.inPreferredColorSpace != null)
                {
                    if (!(opts.inPreferredColorSpace is ColorSpace.Rgb))
                    {
                        throw new IllegalArgumentException("The destination color space must use the " +
                                "RGB color model");
                    }
                    if (((ColorSpace.Rgb)opts.inPreferredColorSpace).getTransferParameters() == null)
                    {
                        throw new IllegalArgumentException("The destination color space must use an " +
                                "ICC parametric transfer function");
                    }
                }
            }

            /**
             *  Helper for passing inBitmap's native pointer to native.
             */
            internal static SKBitmap nativeInBitmap(Options opts)
            {
                if (opts == null || opts.inBitmap == null)
                {
                    return null;
                }

                return opts.inBitmap.getNativeInstance();
            }

            /**
             *  Helper for passing SkColorSpace pointer to native.
             *
             *  @throws IllegalArgumentException if the ColorSpace is not Rgb or does
             *          not have TransferParameters.
             */
            internal static SKColorSpace nativeColorSpace(Options opts)
            {
                if (opts == null || opts.inPreferredColorSpace == null)
                {
                    return null;
                }

                return opts.inPreferredColorSpace.getNativeInstance();
            }

        }

        /**
         * Decode a file path into a bitmap. If the specified file name is null,
         * or cannot be decoded into a bitmap, the function returns null.
         *
         * @param pathName complete path name for the file to be decoded.
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just is size returned.
         * @return The decoded bitmap, or null if the image data could not be
         *         decoded, or, if opts is non-null, if opts requested only the
         *         size be returned (in opts.outWidth and opts.outHeight)
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         */
        public static Bitmap decodeFile(string pathName, Options opts)
        {
            Options.validate(opts);
            Bitmap bm = null;
            FileStream stream = null;
            try
            {
                stream = new FileStream(pathName, FileMode.Open);
                bm = decodeStream(stream, opts);
            }
            catch (Exception e)
            {
                /*  do nothing.
                    If the exception happened on open, bm will be null.
                */
                Log.e("BitmapFactory", "Unable to decode stream: " + e);
            }
            finally
            {
                if (stream != null)
                {
                    try
                    {
                        stream.Dispose();
                    }
                    catch (IOException)
                    {
                        // do nothing here
                    }
                }
            }
            return bm;
        }

        /**
         * Decode a file path into a bitmap. If the specified file name is null,
         * or cannot be decoded into a bitmap, the function returns null.
         *
         * @param pathName complete path name for the file to be decoded.
         * @return the resulting decoded bitmap, or null if it could not be decoded.
         */
        public static Bitmap decodeFile(string pathName)
        {
            return decodeFile(pathName, null);
        }

        /**
         * Decode an immutable bitmap from the specified byte array.
         *
         * @param data byte array of compressed image data
         * @param offset offset into imageData for where the decoder should begin
         *               parsing.
         * @param length the number of bytes, beginning at offset, to parse
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just is size returned.
         * @return The decoded bitmap, or null if the image data could not be
         *         decoded, or, if opts is non-null, if opts requested only the
         *         size be returned (in opts.outWidth and opts.outHeight)
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         */
        public static Bitmap decodeByteArray(byte[] data, int offset, int length, Options opts)
        {
            if ((offset | length) < 0 || data.Length < offset + length)
            {
                throw new IndexOutOfRangeException();
            }
            Options.validate(opts);

            Bitmap bm;

            try
            {
                bm = nativeDecodeByteArray(data, offset, length, opts,
                        Options.nativeInBitmap(opts),
                        Options.nativeColorSpace(opts));

                if (bm == null && opts != null && opts.inBitmap != null)
                {
                    throw new IllegalArgumentException("Problem decoding into existing bitmap");
                }
                setDensityFromOptions(bm, opts);
            }
            finally
            {
            }

            return bm;
        }

        /**
         * Decode an immutable bitmap from the specified byte array.
         *
         * @param data byte array of compressed image data
         * @param offset offset into imageData for where the decoder should begin
         *               parsing.
         * @param length the number of bytes, beginning at offset, to parse
         * @return The decoded bitmap, or null if the image could not be decoded.
         */
        public static Bitmap decodeByteArray(byte[] data, int offset, int length)
        {
            return decodeByteArray(data, offset, length, null);
        }

        /**
         * Set the newly decoded bitmap's density based on the Options.
         */
        private static void setDensityFromOptions(Bitmap outputBitmap, Options opts)
        {
            if (outputBitmap == null || opts == null) return;

            int density = opts.inDensity;
            if (density != 0)
            {
                outputBitmap.setDensity(density);
                int targetDensity = opts.inTargetDensity;
                if (targetDensity == 0 || density == targetDensity || density == opts.inScreenDensity)
                {
                    return;
                }

                byte[] np = outputBitmap.getNinePatchChunk();
                bool isNinePatch = np != null && false; // NinePatch.isNinePatchChunk(np);
                if (opts.inScaled || isNinePatch)
                {
                    outputBitmap.setDensity(targetDensity);
                }
            }
            else if (opts.inBitmap != null)
            {
                // bitmap was reused, ensure density is reset
                outputBitmap.setDensity(Bitmap.getDefaultDensity());
            }
        }

        /**
         * Decode an input stream into a bitmap. If the input stream is null, or
         * cannot be used to decode a bitmap, the function returns null.
         * The stream's position will be where ever it was after the encoded data
         * was read.
         *
         * @param is The input stream that holds the raw data to be decoded into a
         *           bitmap.
         * @param outPadding If not null, return the padding rect for the bitmap if
         *                   it exists, otherwise set padding to [-1,-1,-1,-1]. If
         *                   no bitmap is returned (null) then padding is
         *                   unchanged.
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just is size returned.
         * @return The decoded bitmap, or null if the image data could not be
         *         decoded, or, if opts is non-null, if opts requested only the
         *         size be returned (in opts.outWidth and opts.outHeight)
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         *
         * <p class="note">Prior to {@link android.os.Build.VERSION_CODES#KITKAT},
         * if {@link InputStream#markSupported is.markSupported()} returns true,
         * <code>is.mark(1024)</code> would be called. As of
         * {@link android.os.Build.VERSION_CODES#KITKAT}, this is no longer the case.</p>
         */
        public static Bitmap decodeStream(Stream stream, out Rect outPadding,
            Options opts)
        {
            outPadding = null;
            // we don't throw in this case, thus allowing the caller to only check
            // the cache, and not force the image to be decoded.
            if (stream == null)
            {
                return null;
            }
            Options.validate(opts);

            Bitmap bm = null;

            try
            {
                bm = decodeStreamInternal(stream, out outPadding, opts);

                if (bm == null && opts != null && opts.inBitmap != null)
                {
                    throw new IllegalArgumentException("Problem decoding into existing bitmap");
                }

                setDensityFromOptions(bm, opts);
            }
            finally
            {
            }

            return bm;
        }

        /**
         * Decode an input stream into a bitmap. If the input stream is null, or
         * cannot be used to decode a bitmap, the function returns null.
         * The stream's position will be where ever it was after the encoded data
         * was read.
         *
         * @param is The input stream that holds the raw data to be decoded into a
         *           bitmap.
         * @param outPadding If not null, return the padding rect for the bitmap if
         *                   it exists, otherwise set padding to [-1,-1,-1,-1]. If
         *                   no bitmap is returned (null) then padding is
         *                   unchanged.
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just is size returned.
         * @return The decoded bitmap, or null if the image data could not be
         *         decoded, or, if opts is non-null, if opts requested only the
         *         size be returned (in opts.outWidth and opts.outHeight)
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         *
         * <p class="note">Prior to {@link android.os.Build.VERSION_CODES#KITKAT},
         * if {@link InputStream#markSupported is.markSupported()} returns true,
         * <code>is.mark(1024)</code> would be called. As of
         * {@link android.os.Build.VERSION_CODES#KITKAT}, this is no longer the case.</p>
         */
        public static Bitmap decodeStream(Stream stream, Options opts)
        {
            // we don't throw in this case, thus allowing the caller to only check
            // the cache, and not force the image to be decoded.
            if (stream == null)
            {
                return null;
            }
            Options.validate(opts);

            Bitmap bm = null;

            try
            {
                bm = decodeStreamInternal(stream, opts);

                if (bm == null && opts != null && opts.inBitmap != null)
                {
                    throw new IllegalArgumentException("Problem decoding into existing bitmap");
                }

                setDensityFromOptions(bm, opts);
            }
            finally
            {
            }

            return bm;
        }

        /**
         * Private helper function for decoding an InputStream natively. Buffers the input enough to
         * do a rewind as needed, and supplies temporary storage if necessary. is MUST NOT be null.
         */
        private static Bitmap decodeStreamInternal(Stream stream,
                out Rect outPadding, Options opts)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead)
            {
                throw new UnauthorizedAccessException("the given stream does not support reading");
            }
            int tempStorageSize = opts != null && opts.inTempStorageSize > 0 ? opts.inTempStorageSize : DECODE_BUFFER_SIZE;
            int[] padding;
            Bitmap r = nativeDecodeStream(stream, tempStorageSize, out padding, opts,
                    Options.nativeInBitmap(opts),
                    Options.nativeColorSpace(opts));
            outPadding = new(padding[0], padding[1], padding[2], padding[3]);
            return r;
        }

        /**
         * Private helper function for decoding an InputStream natively. Buffers the input enough to
         * do a rewind as needed, and supplies temporary storage if necessary. is MUST NOT be null.
         */
        private static Bitmap decodeStreamInternal(Stream stream, Options opts)
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead)
            {
                throw new UnauthorizedAccessException("the given stream does not support reading");
            }
            int tempStorageSize = opts != null && opts.inTempStorageSize > 0 ? opts.inTempStorageSize : DECODE_BUFFER_SIZE;
            return nativeDecodeStream(stream, tempStorageSize, opts,
                    Options.nativeInBitmap(opts),
                    Options.nativeColorSpace(opts));
        }

        /**
         * Decode an input stream into a bitmap. If the input stream is null, or
         * cannot be used to decode a bitmap, the function returns null.
         * The stream's position will be where ever it was after the encoded data
         * was read.
         *
         * @param is The input stream that holds the raw data to be decoded into a
         *           bitmap.
         * @return The decoded bitmap, or null if the image data could not be decoded.
         */
        public static Bitmap decodeStream(Stream stream)
        {
            return decodeStream(stream, null);
        }

        /**
         * Decode a bitmap from the file descriptor. If the bitmap cannot be decoded
         * return null. The position within the descriptor will not be changed when
         * this returns, so the descriptor can be used again as-is.
         *
         * @param fd The file descriptor containing the bitmap data to decode
         * @param outPadding If not null, return the padding rect for the bitmap if
         *                   it exists, otherwise set padding to [-1,-1,-1,-1]. If
         *                   no bitmap is returned (null) then padding is
         *                   unchanged.
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just its size returned.
         * @return the decoded bitmap, or null
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         */
        public static Bitmap decodeFileDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle fd, out Rect outPadding, Options opts)
        {
            Options.validate(opts);
            Bitmap bm;

            try
            {
                FileStream fis = new(fd, FileAccess.Read);
                try
                {
                    bm = decodeStreamInternal(fis, out outPadding, opts);
                }
                finally
                {
                    try
                    {
                        fis.Dispose();
                    }
                    catch (Exception) {/* ignore */}
                }

                if (bm == null && opts != null && opts.inBitmap != null)
                {
                    throw new IllegalArgumentException("Problem decoding into existing bitmap");
                }

                setDensityFromOptions(bm, opts);
            }
            finally
            {
            }
            return bm;
        }

        /**
         * Decode a bitmap from the file descriptor. If the bitmap cannot be decoded
         * return null. The position within the descriptor will not be changed when
         * this returns, so the descriptor can be used again as-is.
         *
         * @param fd The file descriptor containing the bitmap data to decode
         * @param outPadding If not null, return the padding rect for the bitmap if
         *                   it exists, otherwise set padding to [-1,-1,-1,-1]. If
         *                   no bitmap is returned (null) then padding is
         *                   unchanged.
         * @param opts null-ok; Options that control downsampling and whether the
         *             image should be completely decoded, or just its size returned.
         * @return the decoded bitmap, or null
         * @throws IllegalArgumentException if {@link BitmapFactory.Options#inPreferredConfig}
         *         is {@link android.graphics.Bitmap.Config#HARDWARE}
         *         and {@link BitmapFactory.Options#inMutable} is set, if the specified color space
         *         is not {@link ColorSpace.Model#RGB RGB}, or if the specified color space's transfer
         *         function is not an {@link ColorSpace.Rgb.TransferParameters ICC parametric curve}
         */
        public static Bitmap decodeFileDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle fd, Options opts)
        {
            Options.validate(opts);
            Bitmap bm;

            try
            {
                FileStream fis = new(fd, FileAccess.Read);
                try
                {
                    bm = decodeStreamInternal(fis, opts);
                }
                finally
                {
                    try
                    {
                        fis.Dispose();
                    }
                    catch (Exception) {/* ignore */}
                }

                if (bm == null && opts != null && opts.inBitmap != null)
                {
                    throw new IllegalArgumentException("Problem decoding into existing bitmap");
                }

                setDensityFromOptions(bm, opts);
            }
            finally
            {
            }
            return bm;
        }

        /**
         * Decode a bitmap from the file descriptor. If the bitmap cannot be decoded
         * return null. The position within the descriptor will not be changed when
         * this returns, so the descriptor can be used again as is.
         *
         * @param fd The file descriptor containing the bitmap data to decode
         * @return the decoded bitmap, or null
         */
        public static Bitmap decodeFileDescriptor(Microsoft.Win32.SafeHandles.SafeFileHandle fd)
        {
            return decodeFileDescriptor(fd, null);
        }

        static string getMimeType(SKEncodedImageFormat format)
        {
            switch (format)
            {
                case SKEncodedImageFormat.Bmp:
                    return "image/bmp";
                case SKEncodedImageFormat.Gif:
                    return "image/gif";
                case SKEncodedImageFormat.Ico:
                    return "image/x-ico";
                case SKEncodedImageFormat.Jpeg:
                    return "image/jpeg";
                case SKEncodedImageFormat.Png:
                    return "image/png";
                case SKEncodedImageFormat.Webp:
                    return "image/webp";
                case SKEncodedImageFormat.Heif:
                    return "image/heif";
                case SKEncodedImageFormat.Avif:
                    return "image/avif";
                case SKEncodedImageFormat.Wbmp:
                    return "image/vnd.wap.wbmp";
                case SKEncodedImageFormat.Dng:
                    return "image/x-adobe-dng";
                default:
                    return null;
            }
        }

        /**
        *  Abstract subclass of SkBitmap's allocator.
        *  Allows the allocator to indicate if the memory it allocates
        *  is zero initialized.
        */
        internal abstract class BRDAllocator : SKBitmap.Allocator
        {
            /**
             *  Indicates if the memory allocated by this allocator is
             *  zero initialized.
             */
            public abstract SKZeroInitialized zeroInit();
        };

        internal class ZeroInitHeapAllocator : BRDAllocator
        {

            public override bool AllocPixelRef(SKBitmap bitmap)
            {
                mStorage = allocateHeapBitmap(bitmap);
                return mStorage != null;
            }

            /**
             * Fetches the backing allocation object. Must be called!
             */
            internal SKBitmap getStorageObjAndReset()
            {
                SKBitmap bm = mStorage;
                mStorage = null;
                return bm;
            }

            public override SKZeroInitialized zeroInit() { return SKZeroInitialized.Yes; }
            SKBitmap mStorage;
        };

        static internal int SKColorTypeBytesPerPixel(SKColorType ct)
        {
            switch (ct)
            {
                case SKColorType.Unknown: return 0;
                case SKColorType.Alpha8: return 1;
                case SKColorType.Rgb565: return 2;
                case SKColorType.Argb4444: return 2;
                case SKColorType.Rgba8888: return 4;
                case SKColorType.Bgra8888: return 4;
                case SKColorType.Rgb888x: return 4;
                case SKColorType.Rgba1010102: return 4;
                case SKColorType.Rgb101010x: return 4;
                case SKColorType.Bgra1010102: return 4;
                case SKColorType.Bgr101010x: return 4;
                case SKColorType.Gray8: return 1;
                case SKColorType.RgbaF16Clamped: return 8;
                case SKColorType.RgbaF16: return 8;
                case SKColorType.RgbaF32: return 16;
                case SKColorType.Rg88: return 2;
                case SKColorType.Alpha16: return 2;
                case SKColorType.Rg1616: return 4;
                case SKColorType.AlphaF16: return 2;
                case SKColorType.RgF16: return 4;
                case SKColorType.Rgba16161616: return 8;
            }
            return 0;
        }

        internal class ScaleCheckingAllocator : SKBitmap.HeapAllocator
        {
            internal ScaleCheckingAllocator(float scale, int size)
            {
                mScale = scale;
                mSize = size;
            }

            readonly float mScale;
            readonly int mSize;

            public override bool AllocPixelRef(SKBitmap bitmap)
            {
                // accounts for scale in final allocation, using eventual size and config
                int bytesPerPixel = SKColorTypeBytesPerPixel(bitmap.ColorType);
                int requestedSize = bytesPerPixel *
                        (int)(bitmap.Width * mScale + 0.5f) *
                        (int)(bitmap.Height * mScale + 0.5f);
                if (requestedSize > mSize)
                {
                    Console.WriteLine("bitmap for alloc reuse (" + mSize + " bytes) can't fit scaled bitmap (" + requestedSize + " bytes)");
                    return false;
                }
                return base.AllocPixelRef(bitmap);
            }
        }

        internal class RecyclingPixelAllocator : SKBitmap.Allocator
        {
            internal RecyclingPixelAllocator(SKBitmap bitmap, uint size)
            {
                mBitmap = bitmap;
                mSize = size;
            }

            public override bool AllocPixelRef(SKBitmap bitmap)
            {
                SKImageInfo info = bitmap.Info;
                if (info.ColorType == SKColorType.Unknown)
                {
                    Console.WriteLine("unable to reuse a bitmap as the target has an unknown bitmap configuration");
                    return false;
                }

                long size = info.BytesSize64;
                if (size > int.MaxValue)
                {
                    Console.WriteLine("bitmap is too large");
                    return false;
                }

                if (size > mSize)
                {
                    Console.WriteLine("bitmap marked for reuse (" + mSize + " bytes) can't fit new bitmap (" + size + " bytes)");
                    return false;
                }

                Bitmap_reconfigure(bitmap, info, mBitmap.PixelRef.Pixels, (IntPtr)bitmap.RowBytes);
                return true;
            }

            SKBitmap mBitmap; // android::Bitmap*
            uint mSize;
        }

        static internal void Bitmap_reconfigure(SKBitmap bitmapHandle,
                int width, int height, Bitmap.Config configHandle, bool requestPremul)
        {
            ArgumentNullException.ThrowIfNull(bitmapHandle);
            SKColorType colorType = configHandle.Native;

            // ARGB_4444 is a deprecated format, convert automatically to 8888
            if (colorType == SKColorType.Argb4444)
            {
                colorType = SKImageInfo.PlatformColorType;
            }
            int requestedSize = width * height * SKColorTypeBytesPerPixel(colorType);
            if (requestedSize > bitmapHandle.ByteCount)
            {
                // done in native as there's no way to get BytesPerPixel in Java
                throw new IllegalArgumentException("Bitmap not large enough to support new configuration");
            }
            SKAlphaType alphaType;
            if (bitmapHandle.Info.ColorType != SKColorType.Rgb565
                    && bitmapHandle.Info.AlphaType == SKAlphaType.Opaque)
            {
                // If the original bitmap was set to opaque, keep that setting, unless it
                // was 565, which is required to be opaque.
                alphaType = SKAlphaType.Opaque;
            }
            else
            {
                // Otherwise respect the premultiplied request.
                alphaType = requestPremul ? SKAlphaType.Premul : SKAlphaType.Unpremul;
            }

            SKImageInfo info = new SKImageInfo(width, height, colorType, alphaType, bitmapHandle.Info.ColorSpace);
            Bitmap_reconfigure(bitmapHandle, info);
        }

        static internal void Bitmap_reconfigure(SKBitmap bitmapHandle, SKImageInfo info)
        {
            Bitmap_reconfigure(bitmapHandle, info, (IntPtr)info.RowBytes);
        }

        static internal void Bitmap_reconfigure(SKBitmap bitmapHandle, SKImageInfo info, IntPtr rowBytes)
        {
            Bitmap_reconfigure(bitmapHandle, info, bitmapHandle.PixelRef.Pixels, rowBytes);
        }

        static internal void Bitmap_reconfigure(SKBitmap bitmapHandle, SKImageInfo info, IntPtr pixels, IntPtr rowBytes)
        {
            SKPixelRef n = new(info.Width, info.Height, pixels, rowBytes);
            bitmapHandle.SetPixelRef(n, 0, 0);
        }

        // Necessary for decodes when the native decoder cannot scale to appropriately match the sampleSize
        // (for example, RAW). If the sampleSize divides evenly into the dimension, we require that the
        // scale matches exactly. If sampleSize does not divide evenly, we allow the decoder to choose how
        // best to round.
        static bool needsFineScale(int fullSize, int decodedSize, int sampleSize)
        {
            if (fullSize % sampleSize == 0 && fullSize / sampleSize != decodedSize)
            {
                return true;
            }
            else if (fullSize / sampleSize + 1 != decodedSize &&
                       fullSize / sampleSize != decodedSize)
            {
                return true;
            }
            return false;
        }

        static bool needsFineScale(SKSizeI fullSize, SKSizeI decodedSize, int sampleSize)
        {
            return needsFineScale(fullSize.Width, decodedSize.Width, sampleSize) ||
                   needsFineScale(fullSize.Height, decodedSize.Height, sampleSize);
        }

        static Bitmap doDecode(SKStreamRewindable stream, Options options,
            SKBitmap bitmapHandle, SKColorSpace colorSpaceHandle)
        {
            return doDecode(stream, false, out var padding, options, bitmapHandle, colorSpaceHandle);
        }

        static Bitmap doDecode(SKStreamRewindable stream, out int[] padding, Options options,
            SKBitmap bitmapHandle, SKColorSpace colorSpaceHandle)
        {
            return doDecode(stream, true, out padding, options, bitmapHandle, colorSpaceHandle);
        }

        private static Bitmap doDecode(SKStreamRewindable stream, bool hasPadding, out int[] padding, Options options,
            SKBitmap bitmapHandle, SKColorSpace colorSpaceHandle)
        {
            padding = null;

            // Set default values for the options parameters.
            int sampleSize = 1;
            bool onlyDecodeSize = false;
            SKColorType prefColorType = SKColorType.Rgba8888;
            bool isHardware = false;
            bool isMutable = false;
            float scale = 1.0f;
            bool requireUnpremultiplied = false;
            Bitmap javaBitmap = null;
            SKColorSpace prefColorSpace = colorSpaceHandle;

            // Update with options supplied by the client.
            if (options != null)
            {
                sampleSize = options.inSampleSize;
                // Correct a non-positive sampleSize.  sampleSize defaults to zero within the
                // options object, which is strange.
                if (sampleSize <= 0)
                {
                    sampleSize = 1;
                }

                if (options.inJustDecodeBounds)
                {
                    onlyDecodeSize = true;
                }

                // initialize these, in case we fail later on
                options.outWidth = -1;
                options.outHeight = -1;
                options.outMimeType = null;
                options.outConfig = null;
                options.outColorSpace = null;

                Bitmap.Config config = options.inPreferredConfig;
                prefColorType = (SKColorType)config.nativeInt;
                isHardware = config.nativeInt == Bitmap.Config.HARDWARE.nativeInt;
                isMutable = options.inMutable;
                requireUnpremultiplied = options.inPremultiplied;
                javaBitmap = options.inBitmap;

                if (options.inScaled)
                {
                    int density = options.inDensity;
                    int targetDensity = options.inTargetDensity;
                    int screenDensity = options.inScreenDensity;
                    if (density != 0 && targetDensity != 0 && density != screenDensity)
                    {
                        scale = (float)targetDensity / density;
                    }
                }
            }

            if (isMutable && isHardware)
            {
                throw new IllegalArgumentException("Bitmaps with Config.HARDWARE are always immutable");
            }

            // Create the codec.
            using NinePatchPeeker peeker = new();
            SKCodecResult result_;

            using SKCodec c = SKCodec.Create(stream, out result_, peeker);

            if (c == null)
            {
                throw new IllegalArgumentException("Failed to create image decoder with message '" + Enum.GetName(typeof(SKCodecResult), result_) + "'");
            }

            using SKAndroidCodec codec = SKAndroidCodec.Create(c);
            if (codec == null)
            {
                throw new IllegalArgumentException("SkAndroidCodec.Create returned null");
            }

            // Do not allow ninepatch decodes to 565.  In the past, decodes to 565
            // would dither, and we do not want to pre-dither ninepatches, since we
            // know that they will be stretched.  We no longer dither 565 decodes,
            // but we continue to prevent ninepatches from decoding to 565, in order
            // to maintain the old behavior.
            if (peeker.HasPatch && SKColorType.Rgb565 == prefColorType)
            {
                prefColorType = SKImageInfo.PlatformColorType;
            }

            // Determine the output size.
            SKSizeI size = codec.GetSampledDimensions(sampleSize);

            int scaledWidth = size.Width;
            int scaledHeight = size.Height;
            bool willScale = false;

            // Apply a fine scaling step if necessary.
            if (needsFineScale(codec.Info.Size, size, sampleSize))
            {
                willScale = true;
                scaledWidth = codec.Info.Width / sampleSize;
                scaledHeight = codec.Info.Height / sampleSize;
            }

            // Set the decode colorType
            SKColorType decodeColorType = codec.ComputeOutputColorType(prefColorType);
            if (decodeColorType == SKColorType.RgbaF16 && isHardware)
            {
                decodeColorType = SKImageInfo.PlatformColorType;
            }

            SKColorSpace decodeColorSpace = codec.ComputeOutputColorSpace(decodeColorType, prefColorSpace);
            // android colorspace
            if (decodeColorSpace != null)
            {
                if (
                    decodeColorSpace.GammaIsCloseToSrgb
                    && !decodeColorSpace.GammaIsLinear
                    && !decodeColorSpace.IsSrgb
                )
                {
                    // android adjusted RGB colorspace
                    decodeColorSpace = ColorSpace.get(ColorSpace.Named.SRGB).getNativeInstance();
                }
            }

            // Set the options and return if the client only wants the size.
            if (options != null)
            {
                options.outMimeType = getMimeType(codec.EncodedFormat);
                options.outWidth = scaledWidth;
                options.outHeight = scaledHeight;
                Bitmap.Config colorTypeToLegacyBitmapConfig(SKColorType colorType)
                {
                    if (colorType == SKImageInfo.PlatformColorType)
                    {
                        return Bitmap.Config.ARGB_8888;
                    }
                    switch (colorType)
                    {
                        case SKColorType.RgbaF16:
                            return Bitmap.Config.RGBA_F16;
                        case SKColorType.Argb4444:
                            return Bitmap.Config.ARGB_4444;
                        case SKColorType.Rgb565:
                            return Bitmap.Config.RGB_565;
                        case SKColorType.Alpha8:
                            return Bitmap.Config.ALPHA_8;
                        case SKColorType.Unknown:
                        default:
                            break;
                    }
                    return null;
                }

                ColorSpace getColorSpace(SKColorSpace decodeColorSpace, SKColorType decodeColorType)
                {
                    if (decodeColorSpace == null || decodeColorType == SKColorType.Alpha8)
                    {
                        return null;
                    }

                    // Special checks for the common sRGB cases and their extended variants.
                    ColorSpace.Named? namedCS = null;
                    using SKColorSpace srgbLinear = SKColorSpace.CreateSrgbLinear();
                    if (decodeColorType == SKColorType.RgbaF16)
                    {
                        // An F16 Bitmap will always report that it is EXTENDED if
                        // it matches a ColorSpace that has an EXTENDED variant.
                        if (decodeColorSpace.IsSrgb)
                        {
                            namedCS = ColorSpace.Named.EXTENDED_SRGB;
                        }
                        else if (decodeColorSpace == srgbLinear)
                        {
                            namedCS = ColorSpace.Named.LINEAR_EXTENDED_SRGB;
                        }
                    }
                    else if (decodeColorSpace.IsSrgb)
                    {
                        namedCS = ColorSpace.Named.SRGB;
                    }
                    else if (decodeColorSpace == srgbLinear)
                    {
                        namedCS = ColorSpace.Named.LINEAR_SRGB;
                    }

                    if (namedCS != null)
                    {
                        return ColorSpace.get((ColorSpace.Named)namedCS);
                    }

                    // Try to match against known RGB color spaces using the CIE XYZ D50
                    // conversion matrix and numerical transfer function parameters
                    SKColorSpaceXyz colorSpaceXyz;
                    if (!decodeColorSpace.ToColorSpaceXyz(out colorSpaceXyz))
                    {
                        throw new Exception("could not decode colorspace");
                    }

                    SKColorSpaceTransferFn transferParams;

                    // We can only handle numerical transfer functions at the moment
                    if (!decodeColorSpace.GetNumericalTransferFunction(out transferParams))
                    {
                        throw new Exception("We can only handle numerical transfer functions at the moment");
                    }

                    ColorSpace.Rgb.TransferParameters transferParameters = new(
                        transferParams.A, transferParams.B, transferParams.C,
                        transferParams.D, transferParams.E, transferParams.F,
                        transferParams.G
                    );

                    ColorSpace colorSpace = ColorSpace.match(colorSpaceXyz.Values, transferParameters);

                    if (colorSpace == null)
                    {
                        // We couldn't find an exact match, let's create a new color space
                        // instance with the 3x3 conversion matrix and transfer function
                        colorSpace = new ColorSpace.Rgb("Unknown", colorSpaceXyz.Values, transferParameters);
                    }

                    return colorSpace;
                }

                Bitmap.Config configID = colorTypeToLegacyBitmapConfig(decodeColorType);
                if (isHardware)
                {
                    configID = Bitmap.Config.HARDWARE;
                }
                options.outConfig = configID;
                options.outColorSpace = getColorSpace(decodeColorSpace, decodeColorType);

                if (onlyDecodeSize)
                {
                    return null;
                }
            }

            // Scale is necessary due to density differences.
            if (scale != 1.0f)
            {
                willScale = true;
                scaledWidth = (int)(scaledWidth * scale + 0.5f);
                scaledHeight = (int)(scaledHeight * scale + 0.5f);
            }


            SKBitmap reuseBitmap = null; // android::Bitmap*
            uint existingBufferSize = 0;
            if (javaBitmap != null)
            {
                Console.WriteLine("WARNING: Bitmap reuse is experimental. BitmapFactory.cs line " + SKUtils.GetLineNumber());
                reuseBitmap = bitmapHandle;
                if (reuseBitmap.IsImmutable)
                {
                    Console.WriteLine("Unable to reuse an immutable bitmap as an image decoder target.");
                    javaBitmap = null;
                    reuseBitmap = null;
                }
                else
                {
                    existingBufferSize = (uint)reuseBitmap.ByteCount;
                }
            }

            using ZeroInitHeapAllocator defaultAllocator = new();
            using RecyclingPixelAllocator recyclingAllocator = new(reuseBitmap, existingBufferSize);
            using ScaleCheckingAllocator scaleCheckingAllocator = new(scale, (int)existingBufferSize);
            using SKBitmap.HeapAllocator heapAllocator = new();
            SKBitmap.Allocator decodeAllocator;

            if (javaBitmap != null && willScale)
            {
                // This will allocate pixels using a HeapAllocator, since there will be an extra
                // scaling step that copies these pixels into Java memory.  This allocator
                // also checks that the recycled javaBitmap is large enough.
                decodeAllocator = scaleCheckingAllocator;
            }
            else if (javaBitmap != null)
            {
                decodeAllocator = recyclingAllocator;
            }
            else if (willScale || isHardware)
            {
                // This will allocate pixels using a HeapAllocator,
                // for scale case: there will be an extra scaling step.
                // for hardware case: there will be extra swizzling & upload to gralloc step.
                decodeAllocator = heapAllocator;
            }
            else
            {
                decodeAllocator = defaultAllocator;
            }

            SKAlphaType alphaType = codec.ComputeOutputAlphaType(requireUnpremultiplied);

            SKImageInfo decodeInfo = new SKImageInfo(size.Width, size.Height,
                    decodeColorType, alphaType, decodeColorSpace);

            SKImageInfo bitmapInfo = decodeInfo;
            if (decodeColorType == SKColorType.Gray8)
            {
                // The legacy implementation of BitmapFactory used kAlpha8 for
                // grayscale images (before kGray8 existed).  While the codec
                // recognizes kGray8, we need to decode into a kAlpha8 bitmap
                // in order to avoid a behavior change.
                bitmapInfo =
                        bitmapInfo.WithColorType(SKColorType.Alpha8).WithAlphaType(SKAlphaType.Premul);
            }

            SKBitmap decodingBitmap = new();
            if (!decodingBitmap.SetInfo(bitmapInfo) ||
                    !decodingBitmap.TryAllocPixels(decodeAllocator))
            {
                // SkAndroidCodec should recommend a valid SkImageInfo, so setInfo()
                // should only only fail if the calculated value for rowBytes is too
                // large.
                // tryAllocPixels() can fail due to OOM on the Java heap, OOM on the
                // native heap, or the recycled javaBitmap being too small to reuse.
                return null;
            }

            // Use SkAndroidCodec to perform the decode.
            SKAndroidCodecOptions codecOptions = new();
            codecOptions.ZeroInitialized = decodeAllocator == defaultAllocator ?
                    SKZeroInitialized.Yes : SKZeroInitialized.No;
            codecOptions.SampleSize = sampleSize;
            SKCodecResult result = codec.GetAndroidPixels(
                decodeInfo, decodingBitmap.GetPixels(), decodingBitmap.RowBytes, codecOptions
            );
            switch (result)
            {
                case SKCodecResult.Success:
                case SKCodecResult.IncompleteInput:
                    break;
                default:
                    Console.WriteLine("codec.GetAndroidPixels() failed.");
                    return null;
            }

            // This is weird so let me explain: we could use the scale parameter
            // directly, but for historical reasons this is how the corresponding
            // Dalvik code has always behaved. We simply recreate the behavior here.
            // The result is slightly different from simply using scale because of
            // the 0.5f rounding bias applied when computing the target image size
            float scaleX = scaledWidth / (float)decodingBitmap.Width;
            float scaleY = scaledHeight / (float)decodingBitmap.Height;

            byte[] ninePatchChunk = null;
            if (peeker.HasPatch)
            {
                if (willScale)
                {
                    peeker.Scale(scaleX, scaleY, scaledWidth, scaledHeight);
                }

                nuint ninePatchArraySize = peeker.SerializedSize;
                if (ninePatchArraySize > peeker.PatchSize)
                {
                    Console.WriteLine("WARNING: ninePatchArraySize (" + ninePatchArraySize + ") is greater than peeker.PatchSize (" + peeker.PatchSize + ")");
                }

                if (ninePatchArraySize > 0)
                {
                    try
                    {
                        ninePatchChunk = new byte[ninePatchArraySize];
                    }
                    catch (OutOfMemoryException)
                    {
                        Console.WriteLine("ninePatchChunk == null");
                        return null;
                    }

                    peeker.CopyInto(ninePatchChunk);
                }
            }

            NinePatch.InsetStruct ninePatchInsets = null;
            if (peeker.HasInsets)
            {
                ninePatchInsets = peeker.createNinePatchInsets(scale);
                if (ninePatchInsets == null)
                {
                    Console.WriteLine("nine patch insets == null");
                    return null;
                }
                if (javaBitmap != null)
                {
                    javaBitmap.mNinePatchInsets = ninePatchInsets;
                }
            }

            SKBitmap outputBitmap = new();
            if (willScale)
            {
                // Set the allocator for the outputBitmap.
                SKBitmap.Allocator outputAllocator;
                if (javaBitmap != null)
                {
                    outputAllocator = recyclingAllocator;
                }
                else
                {
                    outputAllocator = defaultAllocator;
                }

                SKColorType scaledColorType = decodingBitmap.ColorType;
                // FIXME: If the alphaType is kUnpremul and the image has alpha, the
                // colors may not be correct, since Skia does not yet support drawing
                // to/from unpremultiplied bitmaps.
                outputBitmap.SetInfo(
                        bitmapInfo.WithSize(scaledWidth, scaledHeight).WithColorType(scaledColorType));
                if (!outputBitmap.TryAllocPixels(outputAllocator))
                {
                    // This should only fail on OOM.  The recyclingAllocator should have
                    // enough memory since we check this before decoding using the
                    // scaleCheckingAllocator.
                    Console.WriteLine("allocation failed for scaled bitmap");
                    return null;
                }

                using SKPaint paint = new();
                // kSrc_Mode instructs us to overwrite the uninitialized pixels in
                // outputBitmap.  Otherwise we would blend by default, which is not
                // what we want.
                paint.BlendMode = SKBlendMode.Src;

                using SKCanvas canvas = new(outputBitmap);
                canvas.Scale(scaleX, scaleY);
                decodingBitmap.SetImmutable(); // so .asImage() doesn't make a copy

                // new SkSamplingOptions(SkFilterMode::kLinear)
                using SKImage i = decodingBitmap.AsImage();
                /*
                SKRect s = i.Info.Rect;
                SKRectI si = new SKRectI((int)s.Left, (int)s.Top, (int)s.Right, (int)s.Bottom);
                SKRectI u1;
                SKPoint u2;
                SKImage li;
                using (SKImageFilter filter = SKImageFilter.CreateImage(i, s, s, SKFilterQuality.Low)) {
                    li = i.ApplyImageFilter(filter, si, si, out u1, out u2);
                }
                */
                canvas.DrawImage(i, 0.0f, 0.0f, paint);
            }
            else
            {
                SKUtils.Swap(ref outputBitmap, ref decodingBitmap);
            }

            if (hasPadding) padding = peeker.Padding;

            // If we get here, the outputBitmap should have an installed pixelref.
            if (outputBitmap.PixelRef == null)
            {
                Console.WriteLine("Got null SkPixelRef");
                return null;
            }

            if (!isMutable && javaBitmap == null)
            {
                // promise we will never change our pixels (great for sharing and pictures)
                outputBitmap.SetImmutable();
            }

            bool isPremultiplied = !requireUnpremultiplied;
            if (javaBitmap != null)
            {
                SKImageInfo i = outputBitmap.Info;
                javaBitmap.reinit(i.Width, i.Height, isPremultiplied);
                outputBitmap.NotifyPixelsChanged();
                // If a java bitmap was passed in for reuse, pass it back
                return javaBitmap;
            }

            BitmapCreateFlags bitmapCreateFlags = BitmapCreateFlags.kBitmapCreateFlag_None;
            if (isMutable) bitmapCreateFlags |= BitmapCreateFlags.kBitmapCreateFlag_Mutable;
            if (isPremultiplied) bitmapCreateFlags |= BitmapCreateFlags.kBitmapCreateFlag_Premultiplied;

            if (isHardware)
            {
                SKBitmap hardwareBitmap = allocateBitmap(outputBitmap, false);
                if (hardwareBitmap == null)
                {
                    Console.WriteLine("Failed to allocate a hardware bitmap");
                }
                return createBitmap(hardwareBitmap, bitmapCreateFlags, ninePatchChunk, ninePatchInsets, -1, isHardware);
            }

            // now create the java bitmap
            return createBitmap(defaultAllocator.getStorageObjAndReset(), bitmapCreateFlags, ninePatchChunk, ninePatchInsets, -1, isHardware);
        }

        // Assert that bitmap's SkAlphaType is consistent with isPremultiplied.
        static bool assert_premultiplied(SKImageInfo info, bool isPremultiplied)
        {
            // kOpaque_SkAlphaType and kIgnore_SkAlphaType mean that isPremultiplied is
            // irrelevant. This just tests to ensure that the SkAlphaType is not
            // opposite of isPremultiplied.
            if (isPremultiplied)
            {
                if (info.AlphaType == SKAlphaType.Unpremul)
                {
                    Console.WriteLine("bitmap must have alpha type of Premultiplied");
                    return false;
                }
            }
            else
            {
                if (info.AlphaType != SKAlphaType.Premul)
                {
                    Console.WriteLine("bitmap must have alpha type of Unpremultiplied");
                    return false;
                }
            }
            return true;
        }


        internal static Bitmap createBitmap(SKBitmap bitmap, BitmapCreateFlags bitmapCreateFlags)
        {
            return createBitmap(bitmap, bitmapCreateFlags, null, null, -1, false);
        }

        internal static Bitmap createBitmap(
            SKBitmap bitmap, BitmapCreateFlags bitmapCreateFlags,
            byte[] ninePatchChunk, NinePatch.InsetStruct ninePatchInsets,
            int density, bool isHardware
        )
        {
            bool isMutable = (bitmapCreateFlags & BitmapCreateFlags.kBitmapCreateFlag_Mutable) == BitmapCreateFlags.kBitmapCreateFlag_Mutable;
            bool isPremultiplied = (bitmapCreateFlags & BitmapCreateFlags.kBitmapCreateFlag_Premultiplied) == BitmapCreateFlags.kBitmapCreateFlag_Premultiplied;
            // The caller needs to have already set the alpha type properly, so the
            // native SkBitmap stays in sync with the Java Bitmap.
            if (!assert_premultiplied(bitmap.Info, isPremultiplied))
            {
                return null;
            }
            // always from malloc
            if (!isMutable)
            {
                bitmap.SetImmutable();
            }
            return new Bitmap(bitmap, bitmap.Width, bitmap.Height, density, isPremultiplied, ninePatchChunk, ninePatchInsets, isHardware);
        }

        internal static SKBitmap allocateHeapBitmap(SKBitmap bitmap)
        {
            SKImageInfo info = bitmap.Info;
            if (info.ColorType == SKColorType.Unknown)
            {
                Console.WriteLine("unknown bitmap configuration");
                return null;
            }
            return allocateBitmap(bitmap, true);
        }

        internal static SKBitmap allocateBitmap(SKBitmap bitmap, bool respectAlreadySetRowBytes)
        {
            SKImageInfo info = bitmap.Info;
            // skip for now
            /*
            if (respectAlreadySetRowBytes)
            {
                // we must respect the rowBytes value already set on the bitmap instead of
                // attempting to compute our own.
                const size_t rowBytes = bitmap->rowBytes();
                if (!computeAllocationSize(rowBytes, bitmap->height(), ref size)) {
                    return nullptr;
                }
            }
            else
            {
                nuint size = 0;
                if (!computeAllocationSize(info.RowBytes, info.Height, ref size))
                {
                    Console.WriteLine("trying to allocate too large bitmap");
                    return null;
                }
            }
            */

            SKBitmap tmp = new(info, SKBitmapAllocFlags.ZeroPixels);
            bitmap.SetInfo(info, tmp.RowBytes);
            bitmap.SetPixelRef(tmp.PixelRef, 0, 0);
            return tmp;
        }

        /*
        bool computeAllocationSize(int rowBytes, int height, ref nuint size)
        {
            if (0 <= height && (nuint)height <= nuint.MaxValue)
            {
                bool overflowed = false;
                checked
                {
                    size = (nuint)rowBytes * (nuint)height;
                }
                catch (OverflowException)
                {
                    overflowed = true;
                }
                if (!overflowed) return size <= int.MaxValue;
            }
            return false;
        }
        */

        internal enum BitmapCreateFlags
        {
            kBitmapCreateFlag_None = 0x0,
            kBitmapCreateFlag_Mutable = 0x1,
            kBitmapCreateFlag_Premultiplied = 0x2,
        };

        internal static bool SetPixels(SKColor[] srcColors, int srcOffset, int srcStride,
                int x, int y, int width, int height, SKBitmap dstBitmap)
        {
            SKColor[] src;
            if (srcOffset == 0)
            {
                src = srcColors;
            }
            else
            {
                src = (srcColors.ToMemoryPointer<SKColor>() + srcOffset).ToArray();
            }

            SKColorSpace sRGB = SKColorSpace.CreateSrgb();
            SKImageInfo srcInfo = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul, sRGB);
            uint[] tmp = Array.ConvertAll(src, new Converter<SKColor, uint>(v => (uint)v));
            unsafe
            {
                fixed (uint* array = tmp)
                {
                    SKPixmap srcPM = new(srcInfo, (IntPtr)array, srcStride * 4);
                    dstBitmap.WritePixels(srcPM, x, y);
                }
            }
            return true;
        }

        internal static bool SetPixels(uint[] srcColors, int srcOffset, int srcStride,
                int x, int y, int width, int height, SKBitmap dstBitmap)
        {
            uint[] src;
            if (srcOffset == 0)
            {
                src = srcColors;
            }
            else
            {
                src = (srcColors.ToMemoryPointer<uint>() + srcOffset).ToArray();
            }

            SKColorSpace sRGB = SKColorSpace.CreateSrgb();
            SKImageInfo srcInfo = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul, sRGB);
            unsafe
            {
                fixed (uint* array = src)
                {
                    SKPixmap srcPM = new(srcInfo, (IntPtr)array, srcStride * 4);
                    dstBitmap.WritePixels(srcPM, x, y);
                }
            }
            return true;
        }

        internal static bool SetPixels(int[] srcColors, int srcOffset, int srcStride,
                int x, int y, int width, int height, SKBitmap dstBitmap)
        {
            int[] src;
            if (srcOffset == 0)
            {
                src = srcColors;
            }
            else
            {
                src = (srcColors.ToMemoryPointer<int>() + srcOffset).ToArray();
            }

            SKColorSpace sRGB = SKColorSpace.CreateSrgb();
            SKImageInfo srcInfo = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul, sRGB);
            unsafe
            {
                fixed (int* array = src)
                {
                    SKPixmap srcPM = new(srcInfo, (IntPtr)array, srcStride * 4);
                    dstBitmap.WritePixels(srcPM, x, y);
                }
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        internal static BitmapCreateFlags getPremulBitmapCreateFlags(bool isMutable)
        {
            BitmapCreateFlags flags = BitmapCreateFlags.kBitmapCreateFlag_Premultiplied;
            if (isMutable) flags |= BitmapCreateFlags.kBitmapCreateFlag_Mutable;
            return flags;
        }

        internal static Bitmap Bitmap_creator(int offset, int stride, int width, int height,
                                      Bitmap.Config configHandle, bool isMutable,
                                      SKColorSpace colorSpacePtr)
        {
            SKColorType colorType = configHandle.Native;

            // ARGB_4444 is a deprecated format, convert automatically to 8888
            if (colorType == SKColorType.Argb4444)
            {
                colorType = SKImageInfo.PlatformColorType;
            }

            SKColorSpace colorSpace;
            if (colorType == SKColorType.Alpha8)
            {
                colorSpace = null;
            }
            else
            {
                colorSpace = colorSpacePtr;
            }

            SKBitmap bitmap = new();
            bitmap.SetInfo(new SKImageInfo(width, height, colorType, SKAlphaType.Premul,
                        colorSpace));

            SKBitmap nativeBitmap = allocateHeapBitmap(bitmap);
            if (nativeBitmap == null)
            {
                Console.WriteLine("OOM allocating Bitmap with dimensions " + width + " x " + height);
                return null;
            }

            return createBitmap(nativeBitmap, getPremulBitmapCreateFlags(isMutable));
        }

        internal static Bitmap Bitmap_creator(SKColor[] jColors,
                                      int offset, int stride, int width, int height,
                                      Bitmap.Config configHandle, bool isMutable,
                                      SKColorSpace colorSpacePtr)
        {
            SKColorType colorType = configHandle.Native;
            if (null != jColors)
            {
                int n = jColors.Length;
                if (n < (int)((nuint)SKUtils.SKAbs32(stride) * (nuint)height))
                {
                    Console.WriteLine("ERROR");
                    return null;
                }
            }

            // ARGB_4444 is a deprecated format, convert automatically to 8888
            if (colorType == SKColorType.Argb4444)
            {
                colorType = SKImageInfo.PlatformColorType;
            }

            SKColorSpace colorSpace;
            if (colorType == SKColorType.Alpha8)
            {
                colorSpace = null;
            }
            else
            {
                colorSpace = colorSpacePtr;
            }

            SKBitmap bitmap = new();
            bitmap.SetInfo(new SKImageInfo(width, height, colorType, SKAlphaType.Premul,
                        colorSpace));

            SKBitmap nativeBitmap = allocateHeapBitmap(bitmap);
            if (nativeBitmap == null)
            {
                Console.WriteLine("OOM allocating Bitmap with dimensions " + width + " x " + height);
                return null;
            }

            if (jColors != null)
            {
                SetPixels(jColors, offset, stride, 0, 0, width, height, bitmap);
            }

            return createBitmap(nativeBitmap, getPremulBitmapCreateFlags(isMutable));
        }

        internal static Bitmap Bitmap_creator(uint[] jColors,
                                      int offset, int stride, int width, int height,
                                      Bitmap.Config configHandle, bool isMutable,
                                      SKColorSpace colorSpacePtr)
        {
            SKColorType colorType = configHandle.Native;
            if (null != jColors)
            {
                int n = jColors.Length;
                if (n < (int)((nuint)SKUtils.SKAbs32(stride) * (nuint)height))
                {
                    Console.WriteLine("ERROR");
                    return null;
                }
            }

            // ARGB_4444 is a deprecated format, convert automatically to 8888
            if (colorType == SKColorType.Argb4444)
            {
                colorType = SKImageInfo.PlatformColorType;
            }

            SKColorSpace colorSpace;
            if (colorType == SKColorType.Alpha8)
            {
                colorSpace = null;
            }
            else
            {
                colorSpace = colorSpacePtr;
            }

            SKBitmap bitmap = new();
            bitmap.SetInfo(new SKImageInfo(width, height, colorType, SKAlphaType.Premul,
                        colorSpace));

            SKBitmap nativeBitmap = allocateHeapBitmap(bitmap);
            if (nativeBitmap == null)
            {
                Console.WriteLine("OOM allocating Bitmap with dimensions " + width + " x " + height);
                return null;
            }

            if (jColors != null)
            {
                SetPixels(jColors, offset, stride, 0, 0, width, height, bitmap);
            }

            return createBitmap(nativeBitmap, getPremulBitmapCreateFlags(isMutable));
        }

        internal static Bitmap Bitmap_creator(int[] jColors,
                                      int offset, int stride, int width, int height,
                                      Bitmap.Config configHandle, bool isMutable,
                                      SKColorSpace colorSpacePtr)
        {
            SKColorType colorType = configHandle.Native;
            if (null != jColors)
            {
                int n = jColors.Length;
                if (n < (int)((nuint)SKUtils.SKAbs32(stride) * (nuint)height))
                {
                    Console.WriteLine("ERROR");
                    return null;
                }
            }

            // ARGB_4444 is a deprecated format, convert automatically to 8888
            if (colorType == SKColorType.Argb4444)
            {
                colorType = SKImageInfo.PlatformColorType;
            }

            SKColorSpace colorSpace;
            if (colorType == SKColorType.Alpha8)
            {
                colorSpace = null;
            }
            else
            {
                colorSpace = colorSpacePtr;
            }

            SKBitmap bitmap = new();
            bitmap.SetInfo(new SKImageInfo(width, height, colorType, SKAlphaType.Premul,
                        colorSpace));

            SKBitmap nativeBitmap = allocateHeapBitmap(bitmap);
            if (nativeBitmap == null)
            {
                Console.WriteLine("OOM allocating Bitmap with dimensions " + width + " x " + height);
                return null;
            }

            if (jColors != null)
            {
                SetPixels(jColors, offset, stride, 0, 0, width, height, bitmap);
            }

            return createBitmap(nativeBitmap, getPremulBitmapCreateFlags(isMutable));
        }

        static Bitmap nativeDecodeStream(
            Stream stream, int size, Options options, SKBitmap inBitmapHandle, SKColorSpace colorSpaceHandle)
        {
            using SKFrontBufferedManagedStream a = new(stream, size, false);
            using SKFrontBufferedManagedStream b = new(a, SKCodec.MinBufferedBytesNeeded, false);
            return doDecode(b, options, inBitmapHandle, colorSpaceHandle);
        }

        static Bitmap nativeDecodeStream(
            Stream stream, int size, out int[] padding,
            Options options, SKBitmap inBitmapHandle, SKColorSpace colorSpaceHandle)
        {
            using SKFrontBufferedManagedStream a = new(stream, size, false);
            using SKFrontBufferedManagedStream b = new(a, SKCodec.MinBufferedBytesNeeded, false);
            return doDecode(b, out padding, options, inBitmapHandle, colorSpaceHandle);
        }

        static Bitmap nativeDecodeByteArray(
            MemoryPointer<byte> byteArray,
            int offset, int length,
            Options options, SKBitmap inBitmapHandle, SKColorSpace colorSpaceHandle)
        {
            byte[] bytes = offset switch
            {
                // offset is zero, we dont need to copy, grab underlying array
                0 => (byte[])byteArray.GetArray(),
                // offset is not zero, we need to copy
                _ => (byteArray + offset).ToArray(),
            };
            MemoryStream m = new(bytes, 0, length);
            using SKFrontBufferedManagedStream a = new(m, length, false);
            return doDecode(a, options, inBitmapHandle, colorSpaceHandle);
        }
    }
}
