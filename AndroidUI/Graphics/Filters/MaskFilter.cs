﻿/*
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

namespace AndroidUI.Graphics.Filters
{
    /**
     * MaskFilter is the base class for object that perform transformations on
     * an alpha-channel mask before drawing it. A subclass of MaskFilter may be
     * installed into a Paint. Blur and emboss are implemented as subclasses of MaskFilter.
     */
    public class MaskFilter
    {
        internal SkiaSharp.SKMaskFilter native_instance;
        ~MaskFilter()
        {
            native_instance.Dispose();
        }
    }
}