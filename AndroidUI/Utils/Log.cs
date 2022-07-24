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

namespace AndroidUI.Utils
{
    public class LogTag
    {
        string DEFAULT_TAG = "DEFAULT_TAG";

        public LogTag()
        {
            DEFAULT_TAG = GetType().Name;
        }

        public LogTag(object obj_or_string)
        {
            if (obj_or_string == null)
            {
                DEFAULT_TAG = GetType().Name;
            } else if (obj_or_string is string)
            {
                DEFAULT_TAG = (string)obj_or_string;
            }
            else
            {
                DEFAULT_TAG = obj_or_string.GetType().Name;
            }
        }

        public void WriteLine(string msg)
        {
            v(msg);
        }

        private void log_internal(string tag, string message)
        {
            Console.WriteLine(tag + ": " + message);
        }

        public void v(string message) => log_internal(DEFAULT_TAG, message);
        public void v(string tag, string message) => log_internal(tag, message);

        public void i(string message) => log_internal(DEFAULT_TAG, message);
        public void i(string tag, string message) => log_internal(tag, message);

        public void d(string message) => log_internal(DEFAULT_TAG, message);
        public void d(string tag, string message) => log_internal(tag, message);

        public void w(string message) => log_internal(DEFAULT_TAG, message);
        public void w(string tag, string message) => log_internal(tag, message);

        public void e(string message) => log_internal(DEFAULT_TAG, message);
        public void e(string tag, string message) => log_internal(tag, message);
    }

    public static class Log
    {
        public static void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void log_internal(string tag, string message)
        {
            WriteLine(tag + ": " + message);
        }

        public static void v(string tag, string message) => log_internal(tag, message);
        public static void i(string tag, string message) => log_internal(tag, message);
        public static void d(string tag, string message) => log_internal(tag, message);
        public static void w(string tag, string message) => log_internal(tag, message);
        public static void e(string tag, string message) => log_internal(tag, message);
    }
}