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

    // This file was generated from the C++ include file: SkXfermode.h
    // Any changes made to this file will be discarded by the build.
    // To change this file, either edit the include, or device/tools/gluemaker/main.cpp, 
    // or one of the auxilary file specifications in device/tools/gluemaker.

    /**
     * Xfermode is the base class for objects that are called to implement custom
     * "transfer-modes" in the drawing pipeline. The static function Create(Modes)
     * can be called to return an instance of any of the predefined subclasses as
     * specified in the Modes enum. When an Xfermode is assigned to an Paint, then
     * objects drawn with that paint have the xfermode applied.
     */
    public class Xfermode
    {
        internal static readonly int DEFAULT = PorterDuff.Mode.SRC_OVER;
        internal int porterDuffMode = DEFAULT;

        public static implicit operator SkiaSharp.SKBlendMode(Xfermode mode)
        {
            return mode.ToSKBlendMode();
        }

        public SkiaSharp.SKBlendMode ToSKBlendMode()
        {
            switch (porterDuffMode)
            {
                case PorterDuff.Mode.CLEAR:
                    return SkiaSharp.SKBlendMode.Clear;
                case PorterDuff.Mode.SRC:
                    return SkiaSharp.SKBlendMode.Src;
                case PorterDuff.Mode.DST:
                    return SkiaSharp.SKBlendMode.Dst;
                case PorterDuff.Mode.SRC_OVER:
                    return SkiaSharp.SKBlendMode.SrcOver;
                case PorterDuff.Mode.DST_OVER:
                    return SkiaSharp.SKBlendMode.DstOver;
                case PorterDuff.Mode.SRC_IN:
                    return SkiaSharp.SKBlendMode.SrcIn;
                case PorterDuff.Mode.DST_IN:
                    return SkiaSharp.SKBlendMode.DstIn;
                case PorterDuff.Mode.SRC_OUT:
                    return SkiaSharp.SKBlendMode.SrcOut;
                case PorterDuff.Mode.DST_OUT:
                    return SkiaSharp.SKBlendMode.DstOut;
                case PorterDuff.Mode.SRC_ATOP:
                    return SkiaSharp.SKBlendMode.SrcATop;
                case PorterDuff.Mode.DST_ATOP:
                    return SkiaSharp.SKBlendMode.DstATop;
                case PorterDuff.Mode.XOR:
                    return SkiaSharp.SKBlendMode.Xor;
                case PorterDuff.Mode.DARKEN:
                    return SkiaSharp.SKBlendMode.Darken;
                case PorterDuff.Mode.LIGHTEN:
                    return SkiaSharp.SKBlendMode.Lighten;
                // b/73224934 PorterDuff Multiply maps to Skia Modulate
                case PorterDuff.Mode.MULTIPLY:
                    return SkiaSharp.SKBlendMode.Modulate;
                case PorterDuff.Mode.SCREEN:
                    return SkiaSharp.SKBlendMode.Screen;
                case PorterDuff.Mode.ADD:
                    return SkiaSharp.SKBlendMode.Plus;
                case PorterDuff.Mode.OVERLAY:
                    return SkiaSharp.SKBlendMode.Overlay;
                default:
                    throw new ArgumentException("unknown porterDuffMode value while converting Xfermode to SKBlendMode: " + porterDuffMode);
            }
        }
    }
}