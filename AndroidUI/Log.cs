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
    }
}