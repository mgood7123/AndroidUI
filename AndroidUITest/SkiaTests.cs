﻿using AndroidUITestFramework;
using SkiaSharp;

namespace AndroidUITest
{
    internal class Skia : TestGroup
    {
        class Bitmap : TestGroup
        {
            class Bitmap_T1_012 : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    string[] alphas = { "Unknown", "Opaque", "Premul", "Unpremul" };
                    SKPixmap pixmap = new(new SKImageInfo(16, 32, SKImageInfo.PlatformColorType), IntPtr.Zero, 64);
                    Console.WriteLine("alpha type: k" + alphas[(int)pixmap.AlphaType] + "_SkAlphaType");
                    Tools.AssertEqual(alphas[(int)pixmap.AlphaType], "Premul");
                }
            }

            class Bitmap_T2_GenerationId : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap bitmap = new();
                    Console.WriteLine("empty id " + bitmap.GenerationId);
                    Tools.ExpectEqual(bitmap.GenerationId, 0);
                    bitmap.TryAllocPixels(new SKImageInfo(64, 64, SKImageInfo.PlatformColorType, SKAlphaType.Opaque));
                    Console.WriteLine("alloc id " + bitmap.GenerationId);
                    Tools.ExpectEqual(bitmap.GenerationId, 2);
                    bitmap.Erase(SKColors.Red);
                    Console.WriteLine("erase id " + bitmap.GenerationId);
                    Tools.ExpectEqual(bitmap.GenerationId, 4);
                }
            }
            class Bitmap_T3_ComputeIsOpaque : Test
            {
                public override void Run(TestGroup nullableInstance)
                {
                    SKBitmap bitmap = new();
                    bitmap.SetInfo(new SKImageInfo(2, 2, SKImageInfo.PlatformColorType, SKAlphaType.Premul));
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
        }
    }
}
