using AndroidUI.Extensions;
using AndroidUITestFramework;
using SkiaSharp;

namespace AndroidUITest
{
    internal class Skia : TestGroup
    {
        class PngChunkReader : TestGroup
        {
            class _1_test : Test
            {
                class I
                {
                    public int c = 0;
                }
                class Reader : SKPngChunkReader
                {
                    public override unsafe bool ReadChunk(string tag, void* data, IntPtr length)
                    {
                        Console.WriteLine("READ CHUNK");
                        return true;
                    }
                }
                public override void Run(TestGroup nullableInstance)
                {
                    Reader r = new();
                    Tools.ExpectTrue(r.ReadChunk("tag", IntPtr.Zero, IntPtr.Zero), "failed to read chunk");
                }
            }
        }

        class PixelRef : TestGroup
        {
            class _1_test : Test
            {
                class I
                {
                    public int c = 0;
                }
                class TestListener : SKIDChangeListener
                {
                    public TestListener(I ptr)
                    {
                        fPtr = ptr;
                    }
                    public override void Changed()
                    {
                        Console.WriteLine("CHANGED!");
                        fPtr.c++;
                    }
                    private I fPtr;
                };
                public override void Run(TestGroup nullableInstance)
                {
                    SKPixelRef pixelRef = new(0, 0, IntPtr.Zero, (IntPtr)0);

                    // Register a listener.
                    I count = new();
                    pixelRef.AddGenIDChangeListener(new TestListener(count));
                    Console.WriteLine("Count: " + count.c);
                    Tools.AssertEqual(count.c, 0);

                    // No one has looked at our pixelRef's generation ID, so invalidating it doesn't make sense.
                    // (An SkPixelRef tree falls in the forest but there's nobody around to hear it.  Do we care?)
                    pixelRef.NotifyPixelsChanged();
                    Console.WriteLine("Count: " + count.c);
                    Tools.AssertEqual(count.c, 0);

                    // Force the generation ID to be calculated.
                    uint id = pixelRef.GenerationID;
                    Console.WriteLine("ID: " + id);
                    Tools.AssertEqual(id, 2);

                    // Our listener was dropped in the first call to notifyPixelsChanged().  This is a no-op.
                    pixelRef.NotifyPixelsChanged();
                    Console.WriteLine("Count: " + count.c);
                    Tools.AssertEqual(count.c, 0);

                    // Force the generation ID to be recalculated, then add a listener.
                    id = pixelRef.GenerationID;
                    Console.WriteLine("ID: " + id);
                    Tools.AssertEqual(id, 4);
                    pixelRef.AddGenIDChangeListener(new TestListener(count));
                    pixelRef.NotifyPixelsChanged();
                    Console.WriteLine("Count: " + count.c);
                    Tools.AssertEqual(count.c, 1);
                }
            }
        }

        class IDChangeListener : TestGroup
        {
            class Changed : Test
            {
                class a : SKIDChangeListener
                {
                    public override void Changed()
                    {
                        Console.WriteLine("Changed");
                    }
                }
                public override void Run(TestGroup nullableInstance)
                {
                    SKIDChangeListener.List list = new();
                    list.Add(new a());
                    list.Changed();
                }
            }
        }

        class SKSafeMath : TestGroup
        {
            public static void test_signed_with_max<T>(bool flip = false) where T : unmanaged
            {
                AndroidUI.Integer<T> max = AndroidUI.Integer<T>.MaxValue;
                AndroidUI.Integer<T> one = AndroidUI.Integer<T>.ConvertFrom(1);
                AndroidUI.Integer<T> two = AndroidUI.Integer<T>.ConvertFrom(2);
                {
                    A(flip, max, one, two);
                }

                {
                    B(max);
                }

                {
                    AndroidUI.Integer<T> maxSqrtFloor = AndroidUI.Integer<T>.ConvertFrom(Math.Floor(Math.Sqrt(max.ConvertTo<double>())));
                    AndroidUI.Integer<T> maxSqrtFloorPlus1 = maxSqrtFloor + one;
                    SkiaSharp.SKSafeMath safe = new();
                    Tools.AssertTrue(safe.Mul<T>(maxSqrtFloor, maxSqrtFloor) == maxSqrtFloor * maxSqrtFloor);
                    Tools.AssertTrue(safe);
                    if (flip)
                    {
                        Tools.AssertTrue(safe.Mul<T>(maxSqrtFloorPlus1, maxSqrtFloor) == maxSqrtFloor * maxSqrtFloorPlus1);
                    }
                    else
                    {
                        Tools.AssertTrue(safe.Mul<T>(maxSqrtFloor, maxSqrtFloorPlus1) == maxSqrtFloor * maxSqrtFloorPlus1);
                    }
                    Tools.AssertTrue(safe);
                    if (flip)
                    {
                        safe.Mul<T>(maxSqrtFloorPlus1, maxSqrtFloorPlus1);
                    }
                    else
                    {
                        safe.Mul<T>(maxSqrtFloorPlus1, maxSqrtFloorPlus1);
                    }
                    Tools.AssertTrue(!safe);
                }

                {
                    C(max);
                }
            }

            private static void C<T>(AndroidUI.Integer<T> max) where T : unmanaged
            {
                SkiaSharp.SKSafeMath safe = new();
                safe.Mul<T>(max, max);
                Tools.AssertTrue(!safe);
            }

            private static void B<T>(AndroidUI.Integer<T> max) where T : unmanaged
            {
                SkiaSharp.SKSafeMath safe = new();
                safe.Add<T>(max, max);
                Tools.AssertTrue(!safe);
            }

            private static void A<T>(bool flip, AndroidUI.Integer<T> max, AndroidUI.Integer<T> one, AndroidUI.Integer<T> two) where T : unmanaged
            {
                AndroidUI.Integer<T> halfMax = max >> 1;
                AndroidUI.Integer<T> halfMaxPlus1 = halfMax + one;
                SkiaSharp.SKSafeMath safe = new();
                Tools.AssertTrue(safe.Add<T>(halfMax, halfMax) == two * halfMax);
                Tools.AssertTrue(safe);
                if (flip)
                {
                    Tools.AssertTrue(safe.Add<T>(halfMaxPlus1, halfMax) == max);
                }
                else
                {
                    Tools.AssertTrue(safe.Add<T>(halfMax, halfMaxPlus1) == max);
                }
                Tools.AssertTrue(safe);
                if (flip)
                {
                    safe.Add<T>(one, max);
                }
                else
                {
                    safe.Add<T>(max, one);
                }
                Tools.AssertTrue(!safe);
            }

            public static unsafe void test_unsigned_with_max<T>(bool flip = false) where T : unmanaged
            {
                AndroidUI.Integer<T> max = AndroidUI.Integer<T>.MaxValue;
                AndroidUI.Integer<T> one = AndroidUI.Integer<T>.ConvertFrom(1);
                AndroidUI.Integer<T> two = AndroidUI.Integer<T>.ConvertFrom(2);

                {
                    A(flip, max, one, two);
                }

                {
                    B(max);
                }

                {
                    int bits = sizeof(T) * 8;
                    int halfBits = bits / 2;
                    AndroidUI.Integer<T> sqrtMax = max >> halfBits;
                    AndroidUI.Integer<T> sqrtMaxPlus1 = sqrtMax + one;
                    SkiaSharp.SKSafeMath safe = new();
                    Tools.AssertTrue(safe.Mul<T>(sqrtMax, sqrtMax) == sqrtMax * sqrtMax);
                    Tools.AssertTrue(safe);
                    if (flip)
                    {
                        Tools.AssertTrue(safe.Mul<T>(sqrtMaxPlus1, sqrtMax) == sqrtMax << halfBits);
                    }
                    else
                    {
                        Tools.AssertTrue(safe.Mul<T>(sqrtMax, sqrtMaxPlus1) == sqrtMax << halfBits);
                    }
                    Tools.AssertTrue(safe);
                    safe.Mul<T>(sqrtMaxPlus1, sqrtMaxPlus1);
                    Tools.AssertTrue(!safe);
                }

                {
                    C(max);
                }
            }

            class test_int : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<int>();
                }
            }

            class test_uint : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<uint>();
                }
            }

            class test_long : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<long>();
                }
            }

            class test_ulong : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<ulong>();
                }
            }

            class test_int_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<int>(true);
                }
            }

            class test_uint_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<uint>(true);
                }
            }

            class test_long_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<long>(true);
                }
            }

            class test_ulong_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<ulong>(true);
                }
            }

            static void test_zero<T>() where T : unmanaged {
                AndroidUI.Integer<T> max = AndroidUI.Integer<T>.MaxValue;
                AndroidUI.Integer<T> one = AndroidUI.Integer<T>.ConvertFrom(1);
                AndroidUI.Integer<T> zero = AndroidUI.Integer<T>.ConvertFrom(0);

                {
                    SkiaSharp.SKSafeMath safe = new();
                    Tools.AssertTrue(safe.Add(one, zero) == one);
                    Tools.AssertTrue(safe);
                    Tools.AssertTrue(safe.Add(zero, zero) == zero);
                    Tools.AssertTrue(safe);
                    Tools.AssertTrue(safe.Add(zero, max) == max);
                    Tools.AssertTrue(safe);
                }

                {
                    SkiaSharp.SKSafeMath safe = new();
                    Tools.AssertTrue(safe.Mul(one, zero) == zero);
                    Tools.AssertTrue(safe);
                    Tools.AssertTrue(safe.Mul(zero, zero) == zero);
                    Tools.AssertTrue(safe);
                    Tools.AssertTrue(safe.Mul(zero, max) == zero);
                    Tools.AssertTrue(safe);
                }
            }

            class test_int_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<int>();
                }
            }

            class test_uint_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<uint>();
                }
            }

            class test_long_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<long>();
                }
            }

            class test_ulong_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<ulong>();
                }
            }
        }

        class BitmapTests : TestGroup
        {
            class Bitmap_T1_012 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    string[] alphas = { "Unknown", "Opaque", "Premul", "Unpremul" };
                    SKPixmap pixmap = new(new SKImageInfo(16, 32, SKAlphaType.Premul), IntPtr.Zero, 64);
                    Console.WriteLine("alpha type: k" + alphas[(int)pixmap.AlphaType] + "_SkAlphaType");
                    Tools.AssertEqual(alphas[(int)pixmap.AlphaType], "Premul");
                }
            }

            class Bitmap_T2_GenerationId : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap bitmap = new();
                    uint old = bitmap.GenerationId;
                    Console.WriteLine("empty id " + old);
                    Tools.ExpectEqual(old, 0);
                    bitmap.TryAllocPixels(new SKImageInfo(64, 64, SKAlphaType.Opaque));
                    Console.WriteLine("alloc id " + bitmap.GenerationId);
                    Tools.ExpectTrue(bitmap.GenerationId > old);
                    old = bitmap.GenerationId;
                    bitmap.Erase(SKColors.Red);
                    Console.WriteLine("erase id " + bitmap.GenerationId);
                    Tools.ExpectTrue(bitmap.GenerationId > old);
                }
            }
            class Bitmap_T3_ComputeIsOpaque : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap bitmap = new();
                    bitmap.SetInfo(new SKImageInfo(2, 2, SKAlphaType.Premul));
                    for (int index = 0; index < 2; ++index)
                    {
                        Tools.AssertNoException(() => bitmap.AllocPixels(), "failed to allocate pixels");
                        bitmap.Erase(0x00000000);
                        bool computed = bitmap.ComputeIsOpaque();
                        Console.WriteLine("computeIsOpaque: " + (computed ? "true" : "false"));
                        Tools.AssertFalse(computed);
                        bitmap.Erase(0xFFFFFFFF);
                        computed = bitmap.ComputeIsOpaque();
                        Console.WriteLine("computeIsOpaque: " + (computed ? "true" : "false"));
                        Tools.AssertTrue(computed);
                        bitmap.SetInfo(bitmap.Info.WithAlphaType(SKAlphaType.Opaque));
                    }
                }
            }

            class Bitmap_T4_Allocator : Test
            {
                class MyAlloc : SKBitmap.Allocator
                {
                    public override bool AllocPixelRef(SKBitmap bitmap)
                    {
                        Console.WriteLine("ALLOC PIXEL REF");

                        return true;
                    }
                }

                public override void Run(TestGroup nullableInstance)
                {
                    MyAlloc m = new();
                    SKBitmap bitmap = new();
                    bitmap.SetInfo(new SKImageInfo(2, 2, SKAlphaType.Premul));
                    bitmap.AllocPixels(m);
                }
            }

            class Bitmap_T4_Allocator2 : Test
            {
                class MyAlloc : SKBitmap.Allocator
                {
                    public override bool AllocPixelRef(SKBitmap bitmap)
                    {
                        Console.WriteLine("ALLOC PIXEL REF");
                        return true;
                    }
                }

                public override void Run(TestGroup nullableInstance)
                {
                    MyAlloc m = new();
                    SKBitmap bitmap = new();
                    bitmap.SetInfo(new SKImageInfo(2, 2, SKAlphaType.Premul));
                    bitmap.AllocPixels(m);
                    bitmap.Erase(SKColors.AliceBlue);
                }
            }

            class _5_android__0_test_color_conversion : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.toHexString(), "ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.toSKColor(AndroidUI.Color.CYAN).ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColorF().ToString(), "#ff00ffff"); ;
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColorF().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColorF().ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.ToSKColor().ToSKColorF().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.toUnsignedLong().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.CYAN.toUnsignedLong().ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Color.alpha(AndroidUI.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Color.red(AndroidUI.Color.CYAN).toHexString(), "0");
                    Tools.ExpectEqual(AndroidUI.Color.green(AndroidUI.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Color.blue(AndroidUI.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Color.valueOf(AndroidUI.Color.CYAN).toSKColorF().Alpha.ToColorInt().toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Color.valueOf(AndroidUI.Color.CYAN).toSKColorF().Red.ToColorInt().toHexString(), "0");
                    Tools.ExpectEqual(AndroidUI.Color.valueOf(AndroidUI.Color.CYAN).toSKColorF().Green.ToColorInt().toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Color.valueOf(AndroidUI.Color.CYAN).toSKColorF().Blue.ToColorInt().toHexString(), "ff");

                    // alpha
                    Tools.ExpectEqual(0xff, AndroidUI.Color.alpha(AndroidUI.Color.RED));
                    Tools.ExpectEqual(0xff, AndroidUI.Color.alpha(AndroidUI.Color.YELLOW));

                    // argb
                    Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.argb(0xff, 0xff, 0x00, 0x00));
                    Tools.ExpectEqual(AndroidUI.Color.YELLOW, AndroidUI.Color.argb(0xff, 0xff, 0xff, 0x00));
                    Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.argb(1f, 1f, 0f, 0f));
                    Tools.ExpectEqual(AndroidUI.Color.YELLOW, AndroidUI.Color.argb(1f, 1f, 1f, 0f));

                    // blue
                    Tools.ExpectEqual(0x00, AndroidUI.Color.blue(AndroidUI.Color.RED));
                    Tools.ExpectEqual(0x00, AndroidUI.Color.blue(AndroidUI.Color.YELLOW));

                    // green
                    Tools.ExpectEqual(0x00, AndroidUI.Color.green(AndroidUI.Color.RED));
                    Tools.ExpectEqual(0xff, AndroidUI.Color.green(AndroidUI.Color.GREEN));

                    // abnormal case: hsv length less than 3
                    {
                        float[] hsv = new float[2];
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Color.HSVToColor(hsv));
                    }

                    // HSVToColor
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Color.colorToHSV(AndroidUI.Color.RED, hsv);
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.HSVToColor(hsv));
                    }

                    // HSVToColorWithAlpha
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Color.colorToHSV(AndroidUI.Color.RED, hsv);
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.HSVToColor(0xff, hsv));
                    }

                    // abnormal case: colorString starts with '#' but length is neither 7 nor 9
                    {
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Color.parseColor("#ff00ff0"));
                    }

                    // parsecolor
                    {
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.parseColor("#ff0000"));
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.parseColor("#ffff0000"));

                        Tools.ExpectEqual(AndroidUI.Color.BLACK, AndroidUI.Color.parseColor("black"));
                        Tools.ExpectEqual(AndroidUI.Color.DKGRAY, AndroidUI.Color.parseColor("darkgray"));
                        Tools.ExpectEqual(AndroidUI.Color.GRAY, AndroidUI.Color.parseColor("gray"));
                        Tools.ExpectEqual(AndroidUI.Color.LTGRAY, AndroidUI.Color.parseColor("lightgray"));
                        Tools.ExpectEqual(AndroidUI.Color.WHITE, AndroidUI.Color.parseColor("white"));
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.parseColor("red"));
                        Tools.ExpectEqual(AndroidUI.Color.GREEN, AndroidUI.Color.parseColor("green"));
                        Tools.ExpectEqual(AndroidUI.Color.BLUE, AndroidUI.Color.parseColor("blue"));
                        Tools.ExpectEqual(AndroidUI.Color.YELLOW, AndroidUI.Color.parseColor("yellow"));
                        Tools.ExpectEqual(AndroidUI.Color.CYAN, AndroidUI.Color.parseColor("cyan"));
                        Tools.ExpectEqual(AndroidUI.Color.MAGENTA, AndroidUI.Color.parseColor("magenta"));
                    }

                    // abnormal case: colorString doesn't start with '#' and is unknown color
                    {
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Color.parseColor("hello"));
                    }

                    // red
                    {
                        Tools.ExpectEqual(0xff, AndroidUI.Color.red(AndroidUI.Color.RED));
                        Tools.ExpectEqual(0xff, AndroidUI.Color.red(AndroidUI.Color.YELLOW));
                    }

                    // rbg
                    {
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.rgb(0xff, 0x00, 0x00));
                        Tools.ExpectEqual(AndroidUI.Color.YELLOW, AndroidUI.Color.rgb(0xff, 0xff, 0x00));
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.rgb(1f, 0f, 0f));
                        Tools.ExpectEqual(AndroidUI.Color.YELLOW, AndroidUI.Color.rgb(1f, 1f, 0f));
                    }

                    // abnormal case: hsv length less than 3
                    {
                        float[] hsv = new float[2];
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Color.RGBToHSV(0xff, 0x00, 0x00, hsv));
                    }

                    // RGBToHSV
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Color.RGBToHSV(0xff, 0x00, 0x00, hsv);
                        Tools.ExpectEqual(AndroidUI.Color.RED, AndroidUI.Color.HSVToColor(hsv));
                    }

                    // Luminance
                    {
                        Tools.ExpectEqual(0, AndroidUI.Color.luminance(AndroidUI.Color.BLACK), 0.ToString());
                        float eps = 0.000001f;
                        Tools.ExpectEqual(0.0722, AndroidUI.Color.luminance(AndroidUI.Color.BLUE), eps.ToString());
                        Tools.ExpectEqual(0.2126, AndroidUI.Color.luminance(AndroidUI.Color.RED), eps.ToString());
                        Tools.ExpectEqual(0.7152, AndroidUI.Color.luminance(AndroidUI.Color.GREEN), eps.ToString());
                        Tools.ExpectEqual(1, AndroidUI.Color.luminance(AndroidUI.Color.WHITE), 0.ToString());
                    }
                }
            }

            class _5_android__0_test : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Bitmap bm = AndroidUI.Bitmap.createBitmap(1, 1, AndroidUI.Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.TRANSPARENT);
                    bm.recycle();
                }
            }

            class _5_android__1_test : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Bitmap bm = AndroidUI.Bitmap.createBitmap(1, 1, AndroidUI.Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    bm.setPixel(0, 0, AndroidUI.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.TRANSPARENT);
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.CYAN);
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Color.CYAN);
                    bm.recycle();
                }
            }

            class _5_android__2_test : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Bitmap bm = AndroidUI.Bitmap.createBitmap(1, 1, AndroidUI.Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    bm.setPixel(0, 0, AndroidUI.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.TRANSPARENT, "set pixel");
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.CYAN, "set pixel");
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Color.CYAN, "set pixel");
                    bm.eraseColor(AndroidUI.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.TRANSPARENT, "erase color");
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Color.CYAN, "erase color");
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Color.CYAN, "erase color");
                    bm.recycle();
                }
            }
            class _5_android__3_test_bitmap_send_over_pipe : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Bitmap source = AndroidUI.Bitmap.createBitmap(100, 100, AndroidUI.Bitmap.Config.ARGB_8888);
                    source.eraseColor(AndroidUI.Color.RED);
                    AndroidUI.Bitmap result = null;
                    try
                    {
                        result = sendOverPipe(source, AndroidUI.Bitmap.CompressFormat.PNG);
                    }
                    catch (Exception e)
                    {
                        Tools.RETHROW_EXCEPTION_IF_NEEDED(e);
                        Tools.FAIL("exception: " + e.ToString());
                    }
                    Tools.AssertTrue(source.sameAs(result), "expected source to be same as result");
                }

                private AndroidUI.Bitmap sendOverPipe(AndroidUI.Bitmap source, AndroidUI.Bitmap.CompressFormat format)
                {
                    AndroidUI.FileDescriptor[] pipeFds = AndroidUI.Os.pipe();
                    AndroidUI.FileDescriptor readFd = pipeFds[0];
                    AndroidUI.FileDescriptor writeFd = pipeFds[1];

                    Exception[] compressErrors = new Exception[1];

                    // CountDownLatch does not appear to be used

                    AndroidUI.Bitmap[] decodedResult = new AndroidUI.Bitmap[1];
                    Thread writeThread = new(() =>
                    {
                        try
                        {
                            FileStream output = writeFd.ToFileOutputStream();
                            source.compress(format, 100, output);
                            Console.WriteLine("disposing writefd");
                            output.Dispose();
                            Console.WriteLine("disposed writefd");
                        }
                        catch (Exception t)
                        {
                            compressErrors[0] = t;
                            // Try closing the FD to unblock the test thread
                            try
                            {
                                Console.WriteLine("disposing writefd");
                                writeFd.Dispose();
                                Console.WriteLine("disposed writefd");
                            }
                            catch (Exception ignore) { }
                        }
                        finally
                        {
                        }
                    });
                    Thread readThread = new(() =>
                    {
                        decodedResult[0] = AndroidUI.BitmapFactory.decodeStream(readFd.ToFileInputStream());
                    });
                    writeThread.Start();
                    readThread.Start();
                    writeThread.Join(1000);
                    readThread.Join(1000);
                    Tools.AssertFalse(writeThread.IsAlive, "write thread must be dead");
                    if (compressErrors[0] != null)
                    {
                        Tools.FAIL("error: " + compressErrors[0].ToString());
                    }
                    if (readThread.IsAlive)
                    {
                        // Test failure, try to clean up
                        Console.WriteLine("disposing writefd");
                        writeFd.Dispose();
                        Console.WriteLine("disposed writefd");
                        readThread.Join(500);
                        Tools.FAIL("Read timed out");
                    }
                    Tools.AssertTrue(readFd.IsValid, "read fd must be valid");
                    Tools.AssertTrue(writeFd.IsValid, "write fd must be valud");
                    Console.WriteLine("disposing readfd");
                    readFd.Dispose();
                    Console.WriteLine("disposed readfd");
                    Console.WriteLine("disposing writefd");
                    writeFd.Dispose();
                    Console.WriteLine("disposed writefd");
                    return decodedResult[0];
                }
            }
            class _5_android__4_test_copy : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Bitmap bm = AndroidUI.Bitmap.createBitmap(4, 4, AndroidUI.Bitmap.Config.ARGB_8888);
                    bm.eraseColor(AndroidUI.Color.CYAN);
                    var bm2 = bm.copy(bm.getConfig(), bm.isMutable());
                    Tools.ExpectTrue(bm.sameAs(bm2), "copy is not same");
                    bm.recycle();
                    bm2.recycle();
                }
            }

            class _5_android__5_test_file : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    const string Filename = "C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg";

                    SKBitmap bm1 = SKBitmap.Decode(Filename);
                    bm1.Dispose();
                    
                    AndroidUI.Bitmap bm2 = AndroidUI.BitmapFactory.decodeFile(Filename);
                    bm2.recycle();
                    
                    SKBitmap bm3 = SKBitmap.Decode(Filename);
                    AndroidUI.Bitmap bm3_ = new(bm3, bm3.Width, bm3.Height);
                    bm3_.recycle();
                }
            }

            class _6_codec_dispose_0_before : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    FileStream stream = new("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg", FileMode.Open);
                    SKCodecResult result_;
                    SKCodec c = SKCodec.Create(stream, out result_);
                    var codec = SKAndroidCodec.Create(c);
                    c.Dispose();
                    codec.Dispose();
                }
            }

            class _6_codec_dispose_1_after : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    FileStream stream = new("C:/Users/small/Pictures/Screenshot 2022-05-19 034147.jpeg", FileMode.Open);
                    SKCodecResult result_;
                    SKCodec c = SKCodec.Create(stream, out result_);
                    var codec = SKAndroidCodec.Create(c);
                    codec.Dispose();
                    c.Dispose();
                }
            }

            class _7_allocator1 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    defaultAllocator.Dispose();
                    decodingBitmap.Dispose();
                }
            }

            class _7_allocator2 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.Dispose();
                    defaultAllocator.Dispose();
                }
            }

            class _7_allocator3 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.SetInfo(new SKImageInfo(1, 1));
                    defaultAllocator.Dispose();
                    decodingBitmap.Dispose();
                }
            }

            class _7_allocator4 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.SetInfo(new SKImageInfo(1, 1));
                    decodingBitmap.Dispose();
                    defaultAllocator.Dispose();
                }
            }

            class _7_allocator5 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.SetInfo(new SKImageInfo(1, 1));
                    decodingBitmap.TryAllocPixels();
                    defaultAllocator.Dispose();
                    decodingBitmap.Dispose();
                }
            }

            class _7_allocator6 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.SetInfo(new SKImageInfo(1, 1));
                    decodingBitmap.TryAllocPixels();
                    decodingBitmap.Dispose();
                    defaultAllocator.Dispose();
                }
            }

            class _7_allocator7 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.SetInfo(new SKImageInfo(1, 1));
                    decodingBitmap.TryAllocPixels(defaultAllocator);
                    defaultAllocator.Dispose();
                    decodingBitmap.Dispose();
                }
            }

            class _7_allocator8 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.TryAllocPixels();
                    decodingBitmap.Dispose();
                }
            }

            class _8_pixmap0 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap decodingBitmap = new();

                    // we have no pixels
                    Tools.ExpectEqual(decodingBitmap.GetPixels(), IntPtr.Zero);

                    // we always return pixmap even if we have no pixels
                    Tools.ExpectInstanceNotEqual(decodingBitmap.Pixmap, null);

                    // pixmap itself may have no pixels
                    Tools.ExpectEqual(decodingBitmap.Pixmap.GetPixels(), IntPtr.Zero);

                    // we have no pixel ref since we have no pixels
                    Tools.ExpectInstanceEqual(decodingBitmap.PixelRef, null);

                    // we return an empty SKColor array if we have no pixels, this wastes an allocation
                    Tools.ExpectInstanceNotEqual(decodingBitmap.Pixels, null);

                    // we have an empty array since we have no pixels
                    Tools.ExpectEqual(decodingBitmap.Pixels.Length, 0);

                    decodingBitmap.Dispose();
                }
            }
        }
    }
}
