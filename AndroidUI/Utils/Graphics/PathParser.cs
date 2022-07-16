/*
 * Copyright (C) 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

using AndroidUI.Exceptions;
using AndroidUI.Graphics;
using AndroidUI.Utils.Graphics;

namespace AndroidUI
{

    /**
     * @hide
     */
    internal partial class PathParser
    {
        static String LOGTAG = typeof(PathParser).Name;

        /**
         * @param pathString The string representing a path, the same as "d" string in svg file.
         * @return the generated Path object.
         */
        public static Graphics.Path createPathFromPathData(String pathString)
        {
            if (pathString == null)
            {
                throw new IllegalArgumentException("Path string can not be null.");
            }
            Graphics.Path path = new Graphics.Path();
            nParseStringForPath(path.mNativePath, pathString, pathString.Length);
            return path;
        }

        /**
         * Interpret PathData as path commands and insert the commands to the given path.
         *
         * @param data The source PathData to be converted.
         * @param outPath The Path object where path commands will be inserted.
         */
        public static void createPathFromPathData(Graphics.Path outPath, PathData data)
        {
            nCreatePathFromPathData(outPath.mNativePath, data.mNativePathData);
        }

        /**
         * @param pathDataFrom The source path represented in PathData
         * @param pathDataTo The target path represented in PathData
         * @return whether the <code>nodesFrom</code> can morph into <code>nodesTo</code>
         */
        public static bool canMorph(PathData pathDataFrom, PathData pathDataTo)
        {
            return nCanMorph(pathDataFrom.mNativePathData, pathDataTo.mNativePathData);
        }

        /**
         * PathData class is a wrapper around the native PathData object, which contains
         * the result of parsing a path string. Specifically, there are verbs and points
         * associated with each verb stored in PathData. This data can then be used to
         * generate commands to manipulate a Path.
         */
        public class PathData
        {
            internal VectorDrawableUtils.Data mNativePathData = null;
            public PathData()
            {
                mNativePathData = nCreateEmptyPathData();
            }

            public PathData(PathData data)
            {
                mNativePathData = nCreatePathData(data.mNativePathData);
            }

            public PathData(String pathString)
            {
                mNativePathData = nCreatePathDataFromString(pathString, pathString.Length);
                if (mNativePathData == null)
                {
                    throw new IllegalArgumentException("Invalid pathData: " + pathString);
                }
            }

            public VectorDrawableUtils.Data getNativePtr()
            {
                return mNativePathData;
            }

            /**
             * Update the path data to match the source.
             * Before calling this, make sure canMorph(target, source) is true.
             *
             * @param source The source path represented in PathData
             */
            public void setPathData(PathData source)
            {
                nSetPathData(mNativePathData, source.mNativePathData);
            }

            ~PathData()
            {
                if (mNativePathData != null)
                {
                    nFinalize(mNativePathData);
                    mNativePathData = null;
                }
            }
        }

        /**
         * Interpolate between the <code>fromData</code> and <code>toData</code> according to the
         * <code>fraction</code>, and put the resulting path data into <code>outData</code>.
         *
         * @param outData The resulting PathData of the interpolation
         * @param fromData The start value as a PathData.
         * @param toData The end value as a PathData
         * @param fraction The fraction to interpolate.
         */
        public static bool interpolatePathData(PathData outData, PathData fromData, PathData toData,
                float fraction)
        {
            return nInterpolatePathData(outData.mNativePathData, fromData.mNativePathData,
                    toData.mNativePathData, fraction);
        }

        // Native functions are defined below.
        private static void nParseStringForPath(SkiaSharp.SKPath pathPtr, String pathString,
                int stringLength)
        {
            NativePathParser.ParseResult parseResult = new();
            NativePathParser.parseAsciiStringForSkPath(pathPtr, parseResult, pathString, stringLength);
            if (parseResult.failureOccurred)
            {
                throw new IllegalArgumentException(parseResult.failureMessage);
            }
        }

        private static VectorDrawableUtils.Data nCreatePathDataFromString(String pathString, int stringLength)
        {
            NativePathParser.ParseResult parseResult = new();
            NativePathParser.getPathDataFromAsciiString(out var data, parseResult, pathString, stringLength);
            if (!parseResult.failureOccurred)
            {
                return data;
            }
            throw new IllegalArgumentException(parseResult.failureMessage);
        }

        // ----------------- @FastNative -----------------------

        private static void nCreatePathFromPathData(SkiaSharp.SKPath outPathPtr, VectorDrawableUtils.Data pathData) {
            VectorDrawableUtils.verbsToPath(outPathPtr, pathData);
        }
        private static VectorDrawableUtils.Data nCreateEmptyPathData() {
            return new();
        }
        private static VectorDrawableUtils.Data nCreatePathData(VectorDrawableUtils.Data nativePtr) {
            return new(nativePtr);
        }
        private static bool nInterpolatePathData(VectorDrawableUtils.Data outDataPtr, VectorDrawableUtils.Data fromDataPtr,
                VectorDrawableUtils.Data toDataPtr, float fraction)
        {
            return VectorDrawableUtils.interpolatePathData(outDataPtr, fromDataPtr, toDataPtr, fraction);
        }
        private static void nFinalize(VectorDrawableUtils.Data nativePtr) {
            nativePtr.verbs = null;
            nativePtr.points = null;
            nativePtr.verbSizes = null;
        }
        private static bool nCanMorph(VectorDrawableUtils.Data fromDataPtr, VectorDrawableUtils.Data toDataPtr) {
            return VectorDrawableUtils.canMorph(fromDataPtr, toDataPtr);
        }
        private static void nSetPathData(VectorDrawableUtils.Data outDataPtr, VectorDrawableUtils.Data fromDataPtr) {
            outDataPtr.SetFrom(fromDataPtr);
        }
    }
}