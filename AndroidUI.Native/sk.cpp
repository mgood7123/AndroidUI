#include "SkTypes.h"

void sk_abort_no_print() {
#if defined(SK_BUILD_FOR_WIN) && defined(SK_IS_BOT)
    // do not display a system dialog before aborting the process
    _set_abort_behavior(0, _WRITE_ABORT_MSG);
#endif
#if defined(SK_DEBUG) && defined(SK_BUILD_FOR_WIN)
    __fastfail(FAST_FAIL_FATAL_APP_EXIT);
#elif defined(__clang__)
    __builtin_trap();
#else
    abort();
#endif
}

#if defined(SK_BUILD_FOR_WIN)
#include <stdarg.h>
#include <stdio.h>
static const size_t kBufferSize = 2048;
void SkDebugf(const char format[], ...) {
    char    buffer[kBufferSize + 1];
    va_list args;

    va_start(args, format);
    vfprintf(stderr, format, args);
    va_end(args);
    fflush(stderr);  // stderr seems to be buffered on Windows.

    va_start(args, format);
    vsnprintf(buffer, kBufferSize, format, args);
    va_end(args);

    OutputDebugStringA(buffer);
}
#endif

#if !defined(SK_BUILD_FOR_WIN) && !defined(SK_BUILD_FOR_ANDROID)
#include <stdarg.h>
#include <stdio.h>
void SkDebugf(const char format[], ...) {
    va_list args;
    va_start(args, format);
    vfprintf(stderr, format, args);
    va_end(args);
}
#endif//!defined(SK_BUILD_FOR_WIN) && !defined(SK_BUILD_FOR_ANDROID)

#if defined(SK_BUILD_FOR_ANDROID)
#include <stdio.h>

#define LOG_TAG "skia"
#include <android/log.h>

// Print debug output to stdout as well.  This is useful for command line
// applications (e.g. skia_launcher).
bool gSkDebugToStdOut = false;

void SkDebugf(const char format[], ...) {
    va_list args1, args2;
    va_start(args1, format);

    if (gSkDebugToStdOut) {
        va_copy(args2, args1);
        vprintf(format, args2);
        va_end(args2);
    }

    __android_log_vprint(ANDROID_LOG_DEBUG, LOG_TAG, format, args1);

    va_end(args1);
}

#endif//defined(SK_BUILD_FOR_ANDROID)
