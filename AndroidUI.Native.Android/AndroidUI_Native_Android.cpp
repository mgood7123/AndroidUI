#include "AndroidUI_Native_Android.h"

DEFINE_ALLOCATION0_IMPL(2, float, f)
DEFINE_ALLOCATION1_IMPL(2, float, f)
DEFINE_ALLOCATION2__IMPL(2, 1, float, f)
DEFINE_ALLOCATION2_IMPL(2, float, f)

DEFINE_ALLOCATION0_IMPL(4, float, f)
DEFINE_ALLOCATION1_IMPL(4, float, f)
DEFINE_ALLOCATION2__IMPL(4, 2, float, f)
DEFINE_ALLOCATION4_IMPL(4, float, f)

DEFINE_ALLOCATION0_IMPL(8, float, f)
DEFINE_ALLOCATION1_IMPL(8, float, f)
DEFINE_ALLOCATION2__IMPL(8, 4, float, f)
DEFINE_ALLOCATION8_IMPL(8, float, f)

DEFINE_ALLOCATION0_IMPL(16, float, f)
DEFINE_ALLOCATION1_IMPL(16, float, f)
DEFINE_ALLOCATION2__IMPL(16, 8, float, f)
DEFINE_ALLOCATION16_IMPL(16, float, f)

DEFINE_FSK_IMPL(2, 1, float, f)
DEFINE_FSK_IMPL(4, 2, float, f)
DEFINE_FSK_IMPL(8, 4, float, f)
DEFINE_FSK_IMPL(16, 8, float, f)

DEFINE_ALLOCATION0_IMPL(2, float, s)
DEFINE_ALLOCATION1_IMPL(2, float, s)
DEFINE_ALLOCATION2__IMPL(2, 1, float, s)
DEFINE_ALLOCATION2_IMPL(2, float, s)

DEFINE_ALLOCATION0_IMPL(4, float, s)
DEFINE_ALLOCATION1_IMPL(4, float, s)
DEFINE_ALLOCATION2__IMPL(4, 2, float, s)
DEFINE_ALLOCATION4_IMPL(4, float, s)

DEFINE_ALLOCATION0_IMPL(8, float, s)
DEFINE_ALLOCATION1_IMPL(8, float, s)
DEFINE_ALLOCATION2__IMPL(8, 4, float, s)
DEFINE_ALLOCATION8_IMPL(8, float, s)

DEFINE_ALLOCATION0_IMPL(16, float, s)
DEFINE_ALLOCATION1_IMPL(16, float, s)
DEFINE_ALLOCATION2__IMPL(16, 8, float, s)
DEFINE_ALLOCATION16_IMPL(16, float, s)

DEFINE_FSK_IMPL(2, 1, float, s)
DEFINE_FSK_IMPL(4, 2, float, s)
DEFINE_FSK_IMPL(8, 4, float, s)
DEFINE_FSK_IMPL(16, 8, float, s)

DEFINE_ALLOCATION0_IMPL(4, uint8_t, b)
DEFINE_ALLOCATION1_IMPL(4, uint8_t, b)
DEFINE_ALLOCATION2__IMPL(4, 2, uint8_t, b)
DEFINE_ALLOCATION4_IMPL(4, uint8_t, b)
DEFINE_ALLOCATION0_IMPL(8, uint8_t, b)
DEFINE_ALLOCATION1_IMPL(8, uint8_t, b)
DEFINE_ALLOCATION2__IMPL(8, 4, uint8_t, b)
DEFINE_ALLOCATION8_IMPL(8, uint8_t, b)
DEFINE_ALLOCATION0_IMPL(16, uint8_t, b)
DEFINE_ALLOCATION1_IMPL(16, uint8_t, b)
DEFINE_ALLOCATION2__IMPL(16, 8, uint8_t, b)
DEFINE_ALLOCATION16_IMPL(16, uint8_t, b)
DEFINE_USK_IMPL(4, 2, uint8_t, b)
DEFINE_USK_IMPL(8, 4, uint8_t, b)
DEFINE_USK_IMPL(16, 8, uint8_t, b)

DEFINE_ALLOCATION0_IMPL(4, uint16_t, h)
DEFINE_ALLOCATION1_IMPL(4, uint16_t, h)
DEFINE_ALLOCATION2__IMPL(4, 2, uint16_t, h)
DEFINE_ALLOCATION4_IMPL(4, uint16_t, h)
DEFINE_ALLOCATION0_IMPL(8, uint16_t, h)
DEFINE_ALLOCATION1_IMPL(8, uint16_t, h)
DEFINE_ALLOCATION2__IMPL(8, 4, uint16_t, h)
DEFINE_ALLOCATION8_IMPL(8, uint16_t, h)
DEFINE_ALLOCATION0_IMPL(16, uint16_t, h)
DEFINE_ALLOCATION1_IMPL(16, uint16_t, h)
DEFINE_ALLOCATION2__IMPL(16, 8, uint16_t, h)
DEFINE_ALLOCATION16_IMPL(16, uint16_t, h)
DEFINE_USK_IMPL(4, 2, uint16_t, h)
DEFINE_USK_IMPL(8, 4, uint16_t, h)
DEFINE_USK_IMPL(16, 8, uint16_t, h)

DEFINE_ALLOCATION0_IMPL(4, int32_t, i)
DEFINE_ALLOCATION1_IMPL(4, int32_t, i)
DEFINE_ALLOCATION2__IMPL(4, 2, int32_t, i)
DEFINE_ALLOCATION4_IMPL(4, int32_t, i)
DEFINE_ALLOCATION0_IMPL(8, int32_t, i)
DEFINE_ALLOCATION1_IMPL(8, int32_t, i)
DEFINE_ALLOCATION2__IMPL(8, 4, int32_t, i)
DEFINE_ALLOCATION8_IMPL(8, int32_t, i)
DEFINE_ALLOCATION0_IMPL(4, uint32_t, u)
DEFINE_ALLOCATION1_IMPL(4, uint32_t, u)
DEFINE_ALLOCATION2__IMPL(4, 2, uint32_t, u)
DEFINE_ALLOCATION4_IMPL(4, uint32_t, u)
DEFINE_ISK_IMPL(4, 2, int32_t, i)
DEFINE_ISK_IMPL(8, 4, int32_t, i)
DEFINE_USK_IMPL(4, 2, uint32_t, u)

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "AndroidUI_Native_Android", __VA_ARGS__))
#define LOGW(...) ((void)__android_log_print(ANDROID_LOG_WARN, "AndroidUI_Native_Android", __VA_ARGS__))

extern "C" {
	/* This trivial function returns the platform ABI for which this dynamic native library is compiled.*/
	const char * AndroidUI_Native_Android::getPlatformABI()
	{
	#if defined(__arm__)
	#if defined(__ARM_ARCH_7A__)
	#if defined(__ARM_NEON__)
		#define ABI "armeabi-v7a/NEON"
	#else
		#define ABI "armeabi-v7a"
	#endif
	#else
		#define ABI "armeabi"
	#endif
	#elif defined(__i386__)
		#define ABI "x86"
	#else
		#define ABI "unknown"
	#endif
		LOGI("This dynamic shared library is compiled with ABI: %s", ABI);
		return "This native library is compiled with ABI: %s" ABI ".";
	}

	void AndroidUI_Native_Android()
	{
	}

	AndroidUI_Native_Android::AndroidUI_Native_Android()
	{
	}

	AndroidUI_Native_Android::~AndroidUI_Native_Android()
	{
	}
}
