using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUITestFramework;
using SkiaSharp;

namespace AndroidUITest
{
    internal class Skia : TestGroup
    {
        internal class PngChunkReader : TestGroup
        {
            internal class _1_test : Test
            {
                internal class Reader : SKPngChunkReader
                {
                    public virtual bool OnReadChunk(string tag, IntPtr data, IntPtr length)
                    {
                        Console.WriteLine("READ CHUNK");
                        return true;
                    }

                    protected override bool ReadChunk(string tag, IntPtr data, IntPtr length)
                    {
                        return OnReadChunk(tag, data, length);
                    }
                }
                public override void Run(TestGroup nullableInstance)
                {
                    using Reader r = new();
                    Tools.ExpectTrue(r.OnReadChunk("tag", IntPtr.Zero, IntPtr.Zero), "failed to read chunk");
                }
            }
        }

        internal class PixelRef : TestGroup
        {
            internal class _1_test : Test
            {
                internal class I
                {
                    public int c = 0;
                }
                internal class TestListener : SKIDChangeListener
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

        internal class IDChangeListener : TestGroup
        {
            internal class Changed : Test
            {
                internal class a : SKIDChangeListener
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

        internal class SKSafeMath : TestGroup
        {
            public static void test_signed_with_max<T>(bool flip = false) where T : unmanaged
            {
                AndroidUI.Utils.Integer<T> max = AndroidUI.Utils.Integer<T>.MaxValue;
                AndroidUI.Utils.Integer<T> one = AndroidUI.Utils.Integer<T>.ConvertFrom(1);
                AndroidUI.Utils.Integer<T> two = AndroidUI.Utils.Integer<T>.ConvertFrom(2);
                {
                    A(flip, max, one, two);
                }

                {
                    B(max);
                }

                {
                    AndroidUI.Utils.Integer<T> maxSqrtFloor = AndroidUI.Utils.Integer<T>.ConvertFrom(Math.Floor(Math.Sqrt(max.ConvertTo<double>())));
                    AndroidUI.Utils.Integer<T> maxSqrtFloorPlus1 = maxSqrtFloor + one;
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

            private static void C<T>(AndroidUI.Utils.Integer<T> max) where T : unmanaged
            {
                SkiaSharp.SKSafeMath safe = new();
                safe.Mul<T>(max, max);
                Tools.AssertTrue(!safe);
            }

            private static void B<T>(AndroidUI.Utils.Integer<T> max) where T : unmanaged
            {
                SkiaSharp.SKSafeMath safe = new();
                safe.Add<T>(max, max);
                Tools.AssertTrue(!safe);
            }

            private static void A<T>(bool flip, AndroidUI.Utils.Integer<T> max, AndroidUI.Utils.Integer<T> one, AndroidUI.Utils.Integer<T> two) where T : unmanaged
            {
                AndroidUI.Utils.Integer<T> halfMax = max >> 1;
                AndroidUI.Utils.Integer<T> halfMaxPlus1 = halfMax + one;
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
                AndroidUI.Utils.Integer<T> max = AndroidUI.Utils.Integer<T>.MaxValue;
                AndroidUI.Utils.Integer<T> one = AndroidUI.Utils.Integer<T>.ConvertFrom(1);
                AndroidUI.Utils.Integer<T> two = AndroidUI.Utils.Integer<T>.ConvertFrom(2);

                {
                    A(flip, max, one, two);
                }

                {
                    B(max);
                }

                {
                    int bits = sizeof(T) * 8;
                    int halfBits = bits / 2;
                    AndroidUI.Utils.Integer<T> sqrtMax = max >> halfBits;
                    AndroidUI.Utils.Integer<T> sqrtMaxPlus1 = sqrtMax + one;
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

            internal class test_int : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<int>();
                }
            }

            internal class test_uint : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<uint>();
                }
            }

            internal class test_long : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<long>();
                }
            }

            internal class test_ulong : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<ulong>();
                }
            }

            internal class test_int_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<int>(true);
                }
            }

            internal class test_uint_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<uint>(true);
                }
            }

            internal class test_long_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_signed_with_max<long>(true);
                }
            }

            internal class test_ulong_flipped : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_unsigned_with_max<ulong>(true);
                }
            }

            static void test_zero<T>() where T : unmanaged {
                AndroidUI.Utils.Integer<T> max = AndroidUI.Utils.Integer<T>.MaxValue;
                AndroidUI.Utils.Integer<T> one = AndroidUI.Utils.Integer<T>.ConvertFrom(1);
                AndroidUI.Utils.Integer<T> zero = AndroidUI.Utils.Integer<T>.ConvertFrom(0);

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

            internal class test_int_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<int>();
                }
            }

            internal class test_uint_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<uint>();
                }
            }

            internal class test_long_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<long>();
                }
            }

            internal class test_ulong_zero : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    test_zero<ulong>();
                }
            }
        }

        internal class BitmapTests : TestGroup
        {
            const string image_path = "K:/DESKTOP_BACKUP/Documents/2021-07-25 22.37.22.jpg";

            internal class Bitmap_T1_012 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    string[] alphas = { "Unknown", "Opaque", "Premul", "Unpremul" };
                    SKPixmap pixmap = new(new SKImageInfo(16, 32, SKAlphaType.Premul), IntPtr.Zero, 64);
                    Console.WriteLine("alpha type: k" + alphas[(int)pixmap.AlphaType] + "_SkAlphaType");
                    Tools.AssertEqual(alphas[(int)pixmap.AlphaType], "Premul");
                }
            }

            internal class Bitmap_T2_GenerationId : Test
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
            internal class Bitmap_T3_ComputeIsOpaque : Test
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

            internal class Bitmap_T4_Allocator : Test
            {
                internal class MyAlloc : SKBitmap.Allocator
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

            internal class Bitmap_T4_Allocator2 : Test
            {
                internal class MyAlloc : SKBitmap.Allocator
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

            internal class _5_android__0_test_color_conversion : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.toHexString(), "ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.toSKColor(AndroidUI.Graphics.Color.CYAN).ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColorF().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColorF().ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.ToSKColor().ToSKColorF().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.toUnsignedLong().ToSKColor().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN.toUnsignedLong().ToSKColor().ToSKColorF().ToString(), "#ff00ffff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.alpha(AndroidUI.Graphics.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.red(AndroidUI.Graphics.Color.CYAN).toHexString(), "0");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.green(AndroidUI.Graphics.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.blue(AndroidUI.Graphics.Color.CYAN).toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.valueOf(AndroidUI.Graphics.Color.CYAN).toSKColorF().Alpha.ToColorInt().toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.valueOf(AndroidUI.Graphics.Color.CYAN).toSKColorF().Red.ToColorInt().toHexString(), "0");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.valueOf(AndroidUI.Graphics.Color.CYAN).toSKColorF().Green.ToColorInt().toHexString(), "ff");
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.valueOf(AndroidUI.Graphics.Color.CYAN).toSKColorF().Blue.ToColorInt().toHexString(), "ff");

                    // alpha
                    Tools.ExpectEqual(0xff, AndroidUI.Graphics.Color.alpha(AndroidUI.Graphics.Color.RED));
                    Tools.ExpectEqual(0xff, AndroidUI.Graphics.Color.alpha(AndroidUI.Graphics.Color.YELLOW));

                    // argb
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.argb(0xff, 0xff, 0x00, 0x00));
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.YELLOW, AndroidUI.Graphics.Color.argb(0xff, 0xff, 0xff, 0x00));
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.argb(1f, 1f, 0f, 0f));
                    Tools.ExpectEqual(AndroidUI.Graphics.Color.YELLOW, AndroidUI.Graphics.Color.argb(1f, 1f, 1f, 0f));

                    // blue
                    Tools.ExpectEqual(0x00, AndroidUI.Graphics.Color.blue(AndroidUI.Graphics.Color.RED));
                    Tools.ExpectEqual(0x00, AndroidUI.Graphics.Color.blue(AndroidUI.Graphics.Color.YELLOW));

                    // green
                    Tools.ExpectEqual(0x00, AndroidUI.Graphics.Color.green(AndroidUI.Graphics.Color.RED));
                    Tools.ExpectEqual(0xff, AndroidUI.Graphics.Color.green(AndroidUI.Graphics.Color.GREEN));

                    // abnormal case: hsv length less than 3
                    {
                        float[] hsv = new float[2];
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Graphics.Color.HSVToColor(hsv));
                    }

                    // HSVToColor
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Graphics.Color.colorToHSV(AndroidUI.Graphics.Color.RED, hsv);
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.HSVToColor(hsv));
                    }

                    // HSVToColorWithAlpha
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Graphics.Color.colorToHSV(AndroidUI.Graphics.Color.RED, hsv);
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.HSVToColor(0xff, hsv));
                    }

                    // abnormal case: colorString starts with '#' but length is neither 7 nor 9
                    {
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Graphics.Color.parseColor("#ff00ff0"));
                    }

                    // parsecolor
                    {
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.parseColor("#ff0000"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.parseColor("#ffff0000"));

                        Tools.ExpectEqual(AndroidUI.Graphics.Color.BLACK, AndroidUI.Graphics.Color.parseColor("black"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.DKGRAY, AndroidUI.Graphics.Color.parseColor("darkgray"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.GRAY, AndroidUI.Graphics.Color.parseColor("gray"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.LTGRAY, AndroidUI.Graphics.Color.parseColor("lightgray"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.WHITE, AndroidUI.Graphics.Color.parseColor("white"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.parseColor("red"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.GREEN, AndroidUI.Graphics.Color.parseColor("green"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.BLUE, AndroidUI.Graphics.Color.parseColor("blue"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.YELLOW, AndroidUI.Graphics.Color.parseColor("yellow"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.CYAN, AndroidUI.Graphics.Color.parseColor("cyan"));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.MAGENTA, AndroidUI.Graphics.Color.parseColor("magenta"));
                    }

                    // abnormal case: colorString doesn't start with '#' and is unknown color
                    {
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Graphics.Color.parseColor("hello"));
                    }

                    // red
                    {
                        Tools.ExpectEqual(0xff, AndroidUI.Graphics.Color.red(AndroidUI.Graphics.Color.RED));
                        Tools.ExpectEqual(0xff, AndroidUI.Graphics.Color.red(AndroidUI.Graphics.Color.YELLOW));
                    }

                    // rbg
                    {
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.rgb(0xff, 0x00, 0x00));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.YELLOW, AndroidUI.Graphics.Color.rgb(0xff, 0xff, 0x00));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.rgb(1f, 0f, 0f));
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.YELLOW, AndroidUI.Graphics.Color.rgb(1f, 1f, 0f));
                    }

                    // abnormal case: hsv length less than 3
                    {
                        float[] hsv = new float[2];
                        Tools.ExpectException<AndroidUI.Exceptions.IllegalArgumentException>(() => AndroidUI.Graphics.Color.RGBToHSV(0xff, 0x00, 0x00, hsv));
                    }

                    // RGBToHSV
                    {
                        float[] hsv = new float[3];
                        AndroidUI.Graphics.Color.RGBToHSV(0xff, 0x00, 0x00, hsv);
                        Tools.ExpectEqual(AndroidUI.Graphics.Color.RED, AndroidUI.Graphics.Color.HSVToColor(hsv));
                    }

                    // Luminance
                    {
                        Tools.ExpectEqual(0, AndroidUI.Graphics.Color.luminance(AndroidUI.Graphics.Color.BLACK), 0.ToString());
                        float eps = 0.000001f;
                        Tools.ExpectEqual(0.0722, AndroidUI.Graphics.Color.luminance(AndroidUI.Graphics.Color.BLUE), eps.ToString());
                        Tools.ExpectEqual(0.2126, AndroidUI.Graphics.Color.luminance(AndroidUI.Graphics.Color.RED), eps.ToString());
                        Tools.ExpectEqual(0.7152, AndroidUI.Graphics.Color.luminance(AndroidUI.Graphics.Color.GREEN), eps.ToString());
                        Tools.ExpectEqual(1, AndroidUI.Graphics.Color.luminance(AndroidUI.Graphics.Color.WHITE), 0.ToString());
                    }
                }
            }

            internal class _5_android__0_test_get_color : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap bm = Bitmap.createBitmap(context, 1, 1, Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.TRANSPARENT);
                    bm.recycle();
                }
            }

            internal class _5_android__1_test_set_pixel_get_color : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap bm = Bitmap.createBitmap(context, 1, 1, Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    bm.setPixel(0, 0, AndroidUI.Graphics.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.TRANSPARENT);
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.CYAN);
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Graphics.Color.CYAN);
                    bm.recycle();
                }
            }

            internal class _5_android__2_test_set_pixel_get_color_get_pixel_erase_color_get_color_get_pixel : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap bm = Bitmap.createBitmap(context, 1, 1, Bitmap.Config.ARGB_8888);
                    Tools.ExpectInstanceNotEqual(bm.mNativePtr, null);
                    bm.setPixel(0, 0, AndroidUI.Graphics.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.TRANSPARENT, "set pixel");
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.CYAN, "set pixel");
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Graphics.Color.CYAN, "set pixel");
                    bm.eraseColor(AndroidUI.Graphics.Color.CYAN);
                    Tools.ExpectNotEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.TRANSPARENT, "erase color");
                    Tools.ExpectEqual(bm.getColor(0, 0).toArgb(), AndroidUI.Graphics.Color.CYAN, "erase color");
                    Tools.ExpectEqual(bm.getPixel(0, 0), AndroidUI.Graphics.Color.CYAN, "erase color");
                    bm.recycle();
                }
            }
            internal class _5_android__3_test_bitmap_send_over_pipe : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap source = Bitmap.createBitmap(context, 100, 100, Bitmap.Config.ARGB_8888);
                    source.eraseColor(AndroidUI.Graphics.Color.RED);
                    Bitmap result = null;
                    try
                    {
                        result = sendOverPipe(source, Bitmap.CompressFormat.PNG);
                    }
                    catch (Exception e)
                    {
                        Tools.RETHROW_EXCEPTION_IF_NEEDED(e);
                        Tools.FAIL("exception: " + e.ToString());
                    }
                    Tools.AssertTrue(source.sameAs(result), "expected source to be same as result");
                }

                private Bitmap sendOverPipe(Bitmap source, Bitmap.CompressFormat format)
                {
                    AndroidUI.OS.FileDescriptor[] pipeFds = AndroidUI.OS.Functions.pipe();
                    AndroidUI.OS.FileDescriptor readFd = pipeFds[0];
                    AndroidUI.OS.FileDescriptor writeFd = pipeFds[1];

                    Exception[] compressErrors = new Exception[1];

                    // CountDownLatch does not appear to be used

                    Bitmap[] decodedResult = new Bitmap[1];
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
                        AndroidUI.Applications.Context context = new();
                        context.densityManager.Set(1, 96);
                        decodedResult[0] = BitmapFactory.decodeStream(context, readFd.ToFileInputStream());
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
            internal class _5_android__4_test_copy : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap bm = Bitmap.createBitmap(context, 4, 4, Bitmap.Config.ARGB_8888);
                    bm.eraseColor(AndroidUI.Graphics.Color.CYAN);
                    var bm2 = bm.copy(bm.getConfig(), bm.isMutable());
                    Tools.ExpectTrue(bm.sameAs(bm2), "copy is not same");
                    bm.recycle();
                    bm2.recycle();
                }
            }

            internal class _5_android__5_test_file : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    const string Filename = image_path;

                    SKBitmap bm1 = SKBitmap.Decode(Filename);
                    bm1.Dispose();

                    AndroidUI.Applications.Context context = new();
                    context.densityManager.Set(1, 96);
                    Bitmap bm2 = BitmapFactory.decodeFile(context, Filename);
                    bm2.recycle();
                    
                    SKBitmap bm3 = SKBitmap.Decode(Filename);
                    Bitmap bm3_ = new(context, bm3, bm3.Width, bm3.Height);
                    bm3_.recycle();
                }
            }

            internal class _6_codec_dispose_0_before : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    FileStream stream = new(image_path, FileMode.Open);
                    SKCodecResult result_;
                    SKCodec c = SKCodec.Create(stream, out result_);
                    var codec = SKAndroidCodec.Create(c);
                    c.Dispose();
                    codec.Dispose();
                }
            }

            internal class _6_codec_dispose_1_after : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    FileStream stream = new(image_path, FileMode.Open);
                    SKCodecResult result_;
                    SKCodec c = SKCodec.Create(stream, out result_);
                    var codec = SKAndroidCodec.Create(c);
                    codec.Dispose();
                    c.Dispose();
                }
            }

            internal class _7_allocator1 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    defaultAllocator.Dispose();
                    decodingBitmap.Dispose();
                }
            }

            internal class _7_allocator2 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap.HeapAllocator defaultAllocator = new();
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.Dispose();
                    defaultAllocator.Dispose();
                }
            }

            internal class _7_allocator3 : Test
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

            internal class _7_allocator4 : Test
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

            internal class _7_allocator5 : Test
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

            internal class _7_allocator6 : Test
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

            internal class _7_allocator7 : Test
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

            internal class _7_allocator8 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap decodingBitmap = new();
                    decodingBitmap.TryAllocPixels();
                    decodingBitmap.Dispose();
                }
            }

            internal class _8_pixmap0 : Test
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
            internal class _10_test_png_chunk_reader : Test
            {
                internal class Reader : SKPngChunkReader
                {
                    public bool r = false;

                    protected override bool ReadChunk(string tag, IntPtr data, IntPtr length)
                    {
                        r = true;
                        Console.WriteLine("READ CHUNK");
                        return true;
                    }
                }
                public override void Run(TestGroup nullableInstance)
                {
                    using Reader r = new();
                    using FileStream stream = new("C:\\Users\\AndroidUI\\Desktop\\AndroidUI\\AndroidUITest\\png_chunks\\good_itxt.png", FileMode.Open);
                    SKCodecResult result_;
                    using SKCodec c = SKCodec.Create(stream, out result_, r);
                    Tools.ExpectFalse(r.r, "known chunks are not meant to invoke chunk reader");

                    using Reader r2 = new();
                    using FileStream np = new("K:/Images/9PNG/circle.9.png", FileMode.Open);
                    SKCodecResult result2_;
                    using SKCodec c2 = SKCodec.Create(np, out result2_, r2);
                    Tools.ExpectTrue(r2.r, "unknown chunks are meant to invoke chunk reader");
                }
            }
        }
    }
}
