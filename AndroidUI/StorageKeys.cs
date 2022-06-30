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
    internal class StorageKeys
    {
        internal const string ViewTempRect = "ANDROID_UI_ViewTempRect";
        internal const string ViewRootImplHandlerActionQueue = "ANDROID_UI_ViewRootImplHandlerActionQueue";
        internal const string ViewRootImplFirstDrawHandlers = "ANDROID_UI_ViewRootImplFirstDrawHandlers";
        internal const string ViewRootImplFirstDrawComplete = "ANDROID_UI_ViewRootImplFirstDrawComplete";

        internal const string TLW = "ANDROID_UI_ThreadLocalWorkspace";
        internal const string LOOPER = "ANDROID_UI_LOOPER";
        internal const string LOOPER_OBSERVER = "ANDROID_UI_LOOPER_OBSERVER";
        internal static string ColorUtilsTempArray = "ANDROID_UI_ColorUtilsTempArray";
        internal static string AnimationFrameworkAnimationState = "ANDROID_UI_AnimationFrameworkAnimationState";
        internal static string MAIN_THREAD_HANDLER = "ANDROID_UI_MAIN_THREAD_HANDLER";
        internal static string AnimationHandler = "ANDROID_UI_AnimationHandler";
    }
}