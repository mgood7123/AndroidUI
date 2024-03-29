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

using AndroidUI.OS;
using System.Text;

namespace AndroidUI.Utils
{
    public static class TimeUtils
    {
        /** @hide Field length that can hold 999 days of time */
        internal const int HUNDRED_DAY_FIELD_LEN = 19;

        private const int SECONDS_PER_MINUTE = 60;
        private const int SECONDS_PER_HOUR = 60 * 60;
        private const int SECONDS_PER_DAY = 24 * 60 * 60;

        /** @hide */
        internal const long NANOS_PER_MS = 1000000;

        private static readonly object sFormatSync = new();
        private static char[] sFormatStr = new char[HUNDRED_DAY_FIELD_LEN + 10];
        private static char[] sTmpFormatStr = new char[HUNDRED_DAY_FIELD_LEN + 10];

        static private int accumField(int amt, int suffix, bool always, int zeropad)
        {
            if (amt > 999)
            {
                int num = 0;
                while (amt != 0)
                {
                    num++;
                    amt /= 10;
                }
                return num + suffix;
            }
            else
            {
                if (amt > 99 || always && zeropad >= 3)
                {
                    return 3 + suffix;
                }
                if (amt > 9 || always && zeropad >= 2)
                {
                    return 2 + suffix;
                }
                if (always || amt > 0)
                {
                    return 1 + suffix;
                }
            }
            return 0;
        }

        static private int printFieldLocked(char[] formatStr, int amt, char suffix, int pos,
                bool always, int zeropad)
        {
            if (always || amt > 0)
            {
                int startPos = pos;
                if (amt > 999)
                {
                    int tmp = 0;
                    while (amt != 0 && tmp < sTmpFormatStr.Length)
                    {
                        int dig = amt % 10;
                        sTmpFormatStr[tmp] = (char)(dig + '0');
                        tmp++;
                        amt /= 10;
                    }
                    tmp--;
                    while (tmp >= 0)
                    {
                        formatStr[pos] = sTmpFormatStr[tmp];
                        pos++;
                        tmp--;
                    }
                }
                else
                {
                    if (always && zeropad >= 3 || amt > 99)
                    {
                        int dig = amt / 100;
                        formatStr[pos] = (char)(dig + '0');
                        pos++;
                        amt -= dig * 100;
                    }
                    if (always && zeropad >= 2 || amt > 9 || startPos != pos)
                    {
                        int dig = amt / 10;
                        formatStr[pos] = (char)(dig + '0');
                        pos++;
                        amt -= dig * 10;
                    }
                    formatStr[pos] = (char)(amt + '0');
                    pos++;
                }
                formatStr[pos] = suffix;
                pos++;
            }
            return pos;
        }

        private static int formatDurationLocked(long duration, int fieldLen)
        {
            if (sFormatStr.Length < fieldLen)
            {
                sFormatStr = new char[fieldLen];
            }

            char[] formatStr = sFormatStr;

            if (duration == 0)
            {
                int pos_ = 0;
                fieldLen -= 1;
                while (pos_ < fieldLen)
                {
                    formatStr[pos_++] = ' ';
                }
                formatStr[pos_] = '0';
                return pos_ + 1;
            }

            char prefix;
            if (duration > 0)
            {
                prefix = '+';
            }
            else
            {
                prefix = '-';
                duration = -duration;
            }

            int millis = (int)(duration % 1000);
            int seconds = (int)MathF.Floor(duration / 1000);
            int days = 0, hours = 0, minutes = 0;

            if (seconds >= SECONDS_PER_DAY)
            {
                days = seconds / SECONDS_PER_DAY;
                seconds -= days * SECONDS_PER_DAY;
            }
            if (seconds >= SECONDS_PER_HOUR)
            {
                hours = seconds / SECONDS_PER_HOUR;
                seconds -= hours * SECONDS_PER_HOUR;
            }
            if (seconds >= SECONDS_PER_MINUTE)
            {
                minutes = seconds / SECONDS_PER_MINUTE;
                seconds -= minutes * SECONDS_PER_MINUTE;
            }

            int pos = 0;

            if (fieldLen != 0)
            {
                int myLen = accumField(days, 1, false, 0);
                myLen += accumField(hours, 1, myLen > 0, 2);
                myLen += accumField(minutes, 1, myLen > 0, 2);
                myLen += accumField(seconds, 1, myLen > 0, 2);
                myLen += accumField(millis, 2, true, myLen > 0 ? 3 : 0) + 1;
                while (myLen < fieldLen)
                {
                    formatStr[pos] = ' ';
                    pos++;
                    myLen++;
                }
            }

            formatStr[pos] = prefix;
            pos++;

            int start = pos;
            bool zeropad = fieldLen != 0;
            pos = printFieldLocked(formatStr, days, 'd', pos, false, 0);
            pos = printFieldLocked(formatStr, hours, 'h', pos, pos != start, zeropad ? 2 : 0);
            pos = printFieldLocked(formatStr, minutes, 'm', pos, pos != start, zeropad ? 2 : 0);
            pos = printFieldLocked(formatStr, seconds, 's', pos, pos != start, zeropad ? 2 : 0);
            pos = printFieldLocked(formatStr, millis, 'm', pos, true, zeropad && pos != start ? 3 : 0);
            formatStr[pos] = 's';
            return pos + 1;
        }

        /** @hide Just for debugging; not internationalized. */
        internal static void formatDuration(long duration, StringBuilder builder)
        {
            lock (sFormatSync)
            {
                int len = formatDurationLocked(duration, 0);
                builder.Append(sFormatStr, 0, len);
            }
        }

        /** @hide Just for debugging; not internationalized. */
        internal static void formatDuration(long duration, StringBuilder builder, int fieldLen)
        {
            lock (sFormatSync)
            {
                int len = formatDurationLocked(duration, fieldLen);
                builder.Append(sFormatStr, 0, len);
            }
        }

        /** @hide Just for debugging; not internationalized. */
        internal static void formatDuration(long duration, TextWriter pw, int fieldLen)
        {
            lock (sFormatSync)
            {
                int len = formatDurationLocked(duration, fieldLen);
                pw.Write(new string(sFormatStr, 0, len));
            }
        }

        /** @hide Just for debugging; not internationalized. */
        internal static string formatDuration(long duration)
        {
            lock (sFormatSync)
            {
                int len = formatDurationLocked(duration, 0);
                return new string(sFormatStr, 0, len);
            }
        }

        /** @hide Just for debugging; not internationalized. */
        internal static void formatDuration(long duration, TextWriter pw)
        {
            formatDuration(duration, pw, 0);
        }

        /** @hide Just for debugging; not internationalized. */
        internal static void formatDuration(long time, long now, TextWriter pw)
        {
            if (time == 0)
            {
                pw.Write("--");
                return;
            }
            formatDuration(time - now, pw, 0);
        }

        /** @hide Just for debugging; not internationalized. */
        internal static string formatUptime(long time)
        {
            return formatTime(time, NanoTime.currentTimeMillis());
        }

        /** @hide Just for debugging; not internationalized. */
        internal static string formatRealtime(long time)
        {
            return formatTime(time, NanoTime.currentTimeMillis());
        }

        /** @hide Just for debugging; not internationalized. */
        internal static string formatTime(long time, long referenceTime)
        {
            long diff = time - referenceTime;
            if (diff > 0)
            {
                return time + " (in " + diff + " ms)";
            }
            if (diff < 0)
            {
                return time + " (" + -diff + " ms ago)";
            }
            return time + " (now)";
        }
    }
}