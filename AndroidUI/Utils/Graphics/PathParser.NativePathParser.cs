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

using AndroidUI.Utils;
using AndroidUI.Utils.Graphics;

namespace AndroidUI
{

    internal partial class PathParser
    {
        internal class NativePathParser
        {
            internal class ParseResult
            {
                internal bool failureOccurred = false;
                internal string failureMessage = "";
            };

            static int nextStart(string s, int length, int startIndex) {
                int index = startIndex;
                while (index < length) {
                    char c = s[index];
                    // Note that 'e' or 'E' are not valid path commands, but could be
                    // used for floating point numbers' scientific notation.
                    // Therefore, when searching for next command, we should ignore 'e'
                    // and 'E'.
                    if ((((c - 'A') * (c - 'Z') <= 0) || ((c - 'a') * (c - 'z') <= 0)) && c != 'e' &&
                        c != 'E') {
                        return index;
                    }
                    index++;
                }
                return index;
            }

            /**
             * Calculate the position of the next comma or space or negative sign
             * @param s the string to search
             * @param start the position to start searching
             * @param result the result of the extraction, including the position of the
             * the starting position of next number, whether it is ending with a '-'.
             */
            static void extract(ref int outEndPosition, out bool outEndWithNegOrDot, string s, int start,
                                int end) {
                // Now looking for ' ', ',', '.' or '-' from the start.
                int currentIndex = start;
                bool foundSeparator = false;
                outEndWithNegOrDot = false;
                bool secondDot = false;
                bool isExponential = false;
                for (; currentIndex < end; currentIndex++) {
                    bool isPrevExponential = isExponential;
                    isExponential = false;
                    char currentChar = s[currentIndex];
                    switch (currentChar) {
                        case ' ':
                        case ',':
                            foundSeparator = true;
                            break;
                        case '-':
                            // The negative sign following a 'e' or 'E' is not a separator.
                            if (currentIndex != start && !isPrevExponential) {
                                foundSeparator = true;
                                outEndWithNegOrDot = true;
                            }
                            break;
                        case '.':
                            if (!secondDot) {
                                secondDot = true;
                            } else {
                                // This is the second dot, and it is considered as a separator.
                                foundSeparator = true;
                                outEndWithNegOrDot = true;
                            }
                            break;
                        case 'e':
                        case 'E':
                            isExponential = true;
                            break;
                    }
                    if (foundSeparator) {
                        break;
                    }
                }
                // In the case where nothing is found, we put the end position to the end of
                // our extract range. Otherwise, end position will be where separator is found.
                outEndPosition = currentIndex;
            }

            static float parseFloat(ParseResult result, string startPtr,
                                    int expectedLength) {
                string endPtr = null;
                float currentValue;
                bool r = float.TryParse(startPtr, out currentValue);
                if (float.IsInfinity(currentValue)) {
                    result.failureOccurred = true;
                    System.Text.StringBuilder b = new("Float out of range:  ");
                    b.Append(startPtr, 0, expectedLength);
                    result.failureMessage = b.ToString();
                }
                if (!r || endPtr == startPtr) {
                    // No conversion is done.
                    result.failureOccurred = true;
                    System.Text.StringBuilder b = new("Float format error when parsing:  ");
                    b.Append(startPtr, 0, expectedLength);
                    result.failureMessage = b.ToString();
                }
                return currentValue;
            }

            /**
             * Parse the floats in the string.
             *
             * @param s the string containing a command and list of floats
             * @return true on success
             */
            static void getFloats(out List<float> outPoints, ParseResult result,
                                  string pathStr, int start, int end) {
                outPoints = new List<float>();
                if (pathStr[start] == 'z' || pathStr[start] == 'Z') {
                    return;
                }
                int startPosition = start + 1;
                int endPosition = start;

                // The startPosition should always be the first character of the
                // current number, and endPosition is the character after the current
                // number.
                while (startPosition < end) {
                    bool endWithNegOrDot;
                    extract(ref endPosition, out endWithNegOrDot, pathStr, startPosition, end);

                    if (startPosition < endPosition) {
                        float currentValue = parseFloat(result, pathStr.Substring(startPosition), end - startPosition);
                        if (result.failureOccurred) {
                            return;
                        }
                        outPoints.Add(currentValue);
                    }

                    if (endWithNegOrDot) {
                        // Keep the '-' or '.' sign with next number.
                        startPosition = endPosition;
                    } else {
                        startPosition = endPosition + 1;
                    }
                }
                return;
            }

            internal static void validateVerbAndPoints(char verb, int points, ParseResult result)
            {
                int numberOfPointsExpected = -1;
                switch (verb)
                {
                    case 'z':
                    case 'Z':
                        numberOfPointsExpected = 0;
                        break;
                    case 'm':
                    case 'l':
                    case 't':
                    case 'M':
                    case 'L':
                    case 'T':
                        numberOfPointsExpected = 2;
                        break;
                    case 'h':
                    case 'v':
                    case 'H':
                    case 'V':
                        numberOfPointsExpected = 1;
                        break;
                    case 'c':
                    case 'C':
                        numberOfPointsExpected = 6;
                        break;
                    case 's':
                    case 'q':
                    case 'S':
                    case 'Q':
                        numberOfPointsExpected = 4;
                        break;
                    case 'a':
                    case 'A':
                        numberOfPointsExpected = 7;
                        break;
                    default:
                        result.failureOccurred = true;
                        result.failureMessage += verb;
                        result.failureMessage += " is not a valid verb. ";
                        return;
                }
                if (numberOfPointsExpected == 0 && points == 0)
                {
                    return;
                }
                if (numberOfPointsExpected > 0 && points % numberOfPointsExpected == 0)
                {
                    return;
                }

                result.failureOccurred = true;
                result.failureMessage += verb;
                result.failureMessage += " needs to be followed by ";
                if (numberOfPointsExpected > 0)
                {
                    result.failureMessage += "a multiple of ";
                }
                result.failureMessage += numberOfPointsExpected + " floats. However, " +
                                          points + " float(s) are found. ";
            }

            internal static void getPathDataFromAsciiString(out VectorDrawableUtils.Data data, ParseResult result,
                                                           string pathStr, int strLen)
            {
                data = new();
                if (pathStr == null)
                {
                    result.failureOccurred = true;
                    result.failureMessage = "Path string cannot be NULL.";
                    return;
                }

                int start = 0;
                // Skip leading spaces.
                while (char.IsWhiteSpace(pathStr[start]) && start < strLen)
                {
                    start++;
                }
                if (start == strLen)
                {
                    result.failureOccurred = true;
                    result.failureMessage = "Path string cannot be empty.";
                    return;
                }
                int end = start + 1;

                while (end < strLen)
                {
                    end = nextStart(pathStr, strLen, end);
                    List<float> points;
                    getFloats(out points, result, pathStr, start, end);
                    validateVerbAndPoints(pathStr[start], points.Count, result);
                    if (result.failureOccurred)
                    {
                        // If either verb or points is not valid, return immediately.
                        result.failureMessage += "Failure occurred at position " + start +
                                                  " of path: " + pathStr;
                        return;
                    }
                    data.verbs.Add(pathStr[start]);
                    data.verbSizes.Add(points.Count);
                    data.points.AddRange(points);
                    start = end;
                    end++;
                }

                if ((end - start) == 1 && start < strLen)
                {
                    validateVerbAndPoints(pathStr[start], 0, result);
                    if (result.failureOccurred)
                    {
                        // If either verb or points is not valid, return immediately.
                        result.failureMessage += "Failure occurred at position " + start +
                                                  " of path: " + pathStr;
                        return;
                    }
                    data.verbs.Add(pathStr[start]);
                    data.verbSizes.Add(0);
                }
            }

            internal static void dump(VectorDrawableUtils.Data data)
            {
                // Print out the path data.
                int start = 0;
                string os;
                for (int i = 0; i < data.verbs.Count; i++)
                {
                    os = "";
                    os += data.verbs[i];
                    os += ", verb size: " + data.verbSizes[i];
                    for (int j = 0; j < data.verbSizes[i]; j++)
                    {
                        os += " " + data.points[start + j];
                    }
                    start += data.verbSizes[i];
                    Log.d("DUMP", os);
                }

                os = "";
                for (int i = 0; i < data.points.Count; i++)
                {
                    os += data.points[i] + ", ";
                }
                Log.d("DUMP", "points are : " + os);
            }

            /**
             * Parse the string literal and create a Skia Path. Return true on success.
             */
            internal static void parseAsciiStringForSkPath(SkiaSharp.SKPath outPath, ParseResult result,
                                                  string pathStr, int strLength)
            {
                VectorDrawableUtils.Data pathData;
                getPathDataFromAsciiString(out pathData, result, pathStr, strLength);
                if (result.failureOccurred)
                {
                    return;
                }
                // Check if there is valid data coming out of parsing the string.
                if (pathData.verbs.Count == 0)
                {
                    result.failureOccurred = true;
                    result.failureMessage = "No verbs found in the string for pathData: ";
                    result.failureMessage += pathStr;
                    return;
                }
                VectorDrawableUtils.verbsToPath(outPath, pathData);
                return;
            }

        }
    }
}