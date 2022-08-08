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

using SkiaSharp;
using System.Text;
using System.Text.Unicode;

namespace AndroidUI.Graphics
{
    public unsafe class NinePatchPeeker : SKPngChunkReader
    {
        public NinePatchPeeker()
        {
            Array.Fill(mOpticalInsets, 0);
            Array.Fill(mOutlineInsets, 0);
        }

        protected override void DisposeNative()
        {
            Native.Additional.SkNinePatchGlue_delete(mPatch);
            base.DisposeNative();
        }

        protected override bool ReadChunk(string tag, IntPtr data, IntPtr length)
        {
            // NPatch
            int byteCount = Encoding.UTF8.GetByteCount(tag);
            byte[] bytes = new byte[checked(byteCount + 1)];
            int written = Encoding.UTF8.GetBytes(tag, bytes);
            bytes[written] = 0;
            fixed (byte* tagPtr = bytes)
            fixed (void** mPatch = &this.mPatch)
            fixed (nuint* mPatchSize = &this.mPatchSize)
            fixed (bool* mHasInsets = &this.mHasInsets)
            fixed (int* mOpticalInsets = this.mOpticalInsets)
            fixed (int* mOutlineInsets = this.mOutlineInsets)
            fixed (float* mOutlineRadius = &this.mOutlineRadius)
            fixed (byte* mOutlineAlpha = &this.mOutlineAlpha)
            {
                return Native.Additional.SkNinePatchGlue_ReadChunk(
                    // ReadChunk
                    tagPtr, (void*)data, length,
                    // NPatch
                    mPatch, mPatchSize, mHasInsets,
                    &mOpticalInsets, &mOutlineInsets,
                    mOutlineRadius, mOutlineAlpha
                );
            }
        }

        public NinePatch.InsetStruct createNinePatchInsets(float scale)
        {
            if (!mHasInsets)
            {
                return null;
            }

            return new NinePatch.InsetStruct(
                    mOpticalInsets[0], mOpticalInsets[1],
                    mOpticalInsets[2], mOpticalInsets[3],
                    mOutlineInsets[0], mOutlineInsets[1],
                    mOutlineInsets[2], mOutlineInsets[3],
                    mOutlineRadius, mOutlineAlpha, scale);
        }

        public int[] Padding
        {
            get
            {
                int[] padding = new int[4];
                fixed (int* mPadding = padding)
                {
                    Native.Additional.SkNinePatchGlue_getPadding(mPatch, &mPadding);
                }
                return padding;
            }
        }

        public void Scale(float scaleX, float scaleY, int scaledWidth, int scaledHeight)
        {
            Native.Additional.SkNinePatchGlue_scale(mPatch, scaleX, scaleY, scaledWidth, scaledHeight);
        }

        public void CopyInto<T>(T[] dest) where T : unmanaged
        {
            fixed (void* data = dest)
            {
                Native.Memcpy(mPatch, data, PatchSize);
            }
        }

        public void CopyInto<T>(T[] dest, nuint length) where T : unmanaged
        {
            fixed (void* data = dest)
            {
                Native.Memcpy(mPatch, data, length);
            }
        }

        public nuint SerializedSize => Native.Additional.SkNinePatchGlue_serializedSize(mPatch);

        private void* mPatch = null;
        private nuint mPatchSize;
        private bool mHasInsets;
        public bool HasPatch => mPatch == null;
        public nuint PatchSize => mPatchSize;
        public bool HasInsets => mHasInsets;
        internal int[] mOpticalInsets = new int[4];
        internal int[] mOutlineInsets = new int[4];
        internal float mOutlineRadius;
        internal byte mOutlineAlpha;
    }
}