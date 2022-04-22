#pragma once

/*
 * Copyright 2015 Google Inc.
 *
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */

#ifndef SkNx_DEFINED
#define SkNx_DEFINED

#include "SkScalar.h"
#include "SkTypes.h"
#include "SkSafe_math.h"
#include "TmpPtr.h"

#include <algorithm>
#include <limits>
#include <type_traits>

 // load(IntPtr a); // C#
 // load(void* a); // C

 // load(ref IntPtr a); // C#
 // load(void** a); // C

// -Xclang -ast-print
// one file https://godbolt.org/z/zs19Yjea9
// one file https://gist.github.com/mgood7123/b2ac7dcf2ccb1f9e0d288c32c12902f9

#define SKNX_TEMPLATE(N, T) SkNx<N, T>
#define AS_SKNX(N, T, ptr) reinterpret_cast<SKNX_TEMPLATE(N, T) *>(ptr)
#define AS_SKNX_REF(N, T, ptr) reinterpret_cast<SKNX_TEMPLATE(N, T) **>(ptr)

 // Every single SkNx method wants to be fully inlined.  (We know better than MSVC).
#define AI SK_ALWAYS_INLINE

namespace {  // NOLINT(google-build-namespaces)

// The default SkNx<N,T> just proxies down to a pair of SkNx<N/2, T>.
    template <int N, typename T>
    struct SkNx {
        typedef SkNx<N / 2, T> Half;

        Half fLo, fHi;

        AI SkNx() = default;
        AI SkNx(const Half& lo, const Half& hi) : fLo(lo), fHi(hi) {}

        AI SkNx(T v) : fLo(v), fHi(v) {}

        AI SkNx(T a, T b) : fLo(a), fHi(b) { static_assert(N == 2, ""); }
        AI SkNx(T a, T b, T c, T d) : fLo(a, b), fHi(c, d) { static_assert(N == 4, ""); }
        AI SkNx(T a, T b, T c, T d, T e, T f, T g, T h) : fLo(a, b, c, d), fHi(e, f, g, h) {
            static_assert(N == 8, "");
        }
        AI SkNx(T a, T b, T c, T d, T e, T f, T g, T h,
            T i, T j, T k, T l, T m, T n, T o, T p)
            : fLo(a, b, c, d, e, f, g, h), fHi(i, j, k, l, m, n, o, p) {
            static_assert(N == 16, "");
        }

        AI T operator[](int k) const {
            SkASSERT(0 <= k && k < N);
            return k < N / 2 ? fLo[k] : fHi[k - N / 2];
        }

        AI static TmpPtr<SkNx<N, T>>  Load(const void* vptr) {
            auto ptr = (const char*)vptr;
            return new SkNx(TmpPtr<Half>(Half::Load(ptr)), TmpPtr<Half>(Half::Load(ptr + N / 2 * sizeof(T))));
        }
        AI void store(void* vptr) const {
            auto ptr = (char*)vptr;
            fLo.store(ptr);
            fHi.store(ptr + N / 2 * sizeof(T));
        }

        AI static void Load4(const void* vptr, SkNx** a, SkNx** b, SkNx** c, SkNx** d) {
            auto ptr = (const char*)vptr;
            TmpPtr<Half> al, bl, cl, dl,
                ah, bh, ch, dh;
            Half::Load4(ptr, al.address(), bl.address(), cl.address(), dl.address());
            Half::Load4(ptr + 4 * N / 2 * sizeof(T), ah.address(), bh.address(), ch.address(), dh.address());
            *a = new SkNx{ al, ah };
            *b = new SkNx{ bl, bh };
            *c = new SkNx{ cl, ch };
            *d = new SkNx{ dl, dh };
        }
        AI static void Load3(const void* vptr, SkNx** a, SkNx** b, SkNx** c) {
            auto ptr = (const char*)vptr;
            TmpPtr<Half> al, bl, cl,
                ah, bh, ch;
            Half::Load3(ptr, al.address(), bl.address(), cl.address());
            Half::Load3(ptr + 3 * N / 2 * sizeof(T), ah.address(), bh.address(), ch.address());
            *a = new SkNx{ al, ah };
            *b = new SkNx{ bl, bh };
            *c = new SkNx{ cl, ch };
        }
        AI static void Load2(const void* vptr, SkNx** a, SkNx** b) {
            auto ptr = (const char*)vptr;
            TmpPtr<Half> al, bl,
                ah, bh;
            Half::Load2(ptr, al.address(), bl.address());
            Half::Load2(ptr + 2 * N / 2 * sizeof(T), ah.address(), bh.address());
            *a = new SkNx{ al, ah };
            *b = new SkNx{ bl, bh };
        }
        AI static void Store4(void* vptr, const SkNx& a, const SkNx& b, const SkNx& c, const SkNx& d) {
            auto ptr = (char*)vptr;
            Half::Store4(ptr, a.fLo, b.fLo, c.fLo, d.fLo);
            Half::Store4(ptr + 4 * N / 2 * sizeof(T), a.fHi, b.fHi, c.fHi, d.fHi);
        }
        AI static void Store3(void* vptr, const SkNx& a, const SkNx& b, const SkNx& c) {
            auto ptr = (char*)vptr;
            Half::Store3(ptr, a.fLo, b.fLo, c.fLo);
            Half::Store3(ptr + 3 * N / 2 * sizeof(T), a.fHi, b.fHi, c.fHi);
        }
        AI static void Store2(void* vptr, const SkNx& a, const SkNx& b) {
            auto ptr = (char*)vptr;
            Half::Store2(ptr, a.fLo, b.fLo);
            Half::Store2(ptr + 2 * N / 2 * sizeof(T), a.fHi, b.fHi);
        }

        AI T min() const { return std::min(fLo.min(), fHi.min()); }
        AI T max() const { return std::max(fLo.max(), fHi.max()); }
        AI bool anyTrue() const { return fLo.anyTrue() || fHi.anyTrue(); }
        AI bool allTrue() const { return fLo.allTrue() && fHi.allTrue(); }

        AI TmpPtr<SkNx<N, T>>    abs() const { return new SkNx{ TmpPtr<Half>(fLo.abs()), TmpPtr<Half>(fHi.abs()) }; }
        AI TmpPtr<SkNx<N, T>>   sqrt() const { return new SkNx{ TmpPtr<Half>(fLo.sqrt()), TmpPtr<Half>(fHi.sqrt()) }; }
        AI TmpPtr<SkNx<N, T>>  floor() const { return new SkNx{ TmpPtr<Half>(fLo.floor()), TmpPtr<Half>(fHi.floor()) }; }

        AI TmpPtr<SkNx<N, T>> operator!() const { return new SkNx{ TmpPtr<Half>(!fLo), TmpPtr<Half>(!fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator-() const { return new SkNx{ TmpPtr<Half>(-fLo), TmpPtr<Half>(-fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator~() const { return new SkNx{ TmpPtr<Half>(~fLo), TmpPtr<Half>(~fHi) }; }

        AI TmpPtr<SkNx<N, T>> operator<<(int bits) const { return new SkNx{ TmpPtr<Half>(fLo << bits), TmpPtr<Half>(fHi << bits) }; }
        AI TmpPtr<SkNx<N, T>> operator>>(int bits) const { return new SkNx{ TmpPtr<Half>(fLo >> bits), TmpPtr<Half>(fHi >> bits) }; }

        AI TmpPtr<SkNx<N, T>> operator+(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo + y.fLo), TmpPtr<Half>(fHi + y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator-(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo - y.fLo), TmpPtr<Half>(fHi - y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator*(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo * y.fLo), TmpPtr<Half>(fHi * y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator/(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo / y.fLo), TmpPtr<Half>(fHi / y.fHi) }; }

        AI TmpPtr<SkNx<N, T>> operator&(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo & y.fLo), TmpPtr<Half>(fHi & y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator|(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo | y.fLo), TmpPtr<Half>(fHi | y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator^(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo ^ y.fLo), TmpPtr<Half>(fHi ^ y.fHi) }; }

        AI TmpPtr<SkNx<N, T>> operator==(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo == y.fLo), TmpPtr<Half>(fHi == y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator!=(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo != y.fLo), TmpPtr<Half>(fHi != y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator<=(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo <= y.fLo), TmpPtr<Half>(fHi <= y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator>=(const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo >= y.fLo), TmpPtr<Half>(fHi >= y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator< (const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo < y.fLo), TmpPtr<Half>(fHi < y.fHi) }; }
        AI TmpPtr<SkNx<N, T>> operator> (const SkNx& y) const { return new SkNx{ TmpPtr<Half>(fLo > y.fLo), TmpPtr<Half>(fHi > y.fHi) }; }

        AI TmpPtr<SkNx<N, T>> saturatedAdd(const SkNx& y) const {
            return new SkNx{ TmpPtr<Half>(fLo.saturatedAdd(y.fLo)), TmpPtr<Half>(fHi.saturatedAdd(y.fHi)) };
        }

        AI TmpPtr<SkNx<N, T>> mulHi(const SkNx& m) const {
            return new SkNx{ TmpPtr<Half>(fLo.mulHi(m.fLo)), TmpPtr<Half>(fHi.mulHi(m.fHi)) };
        }
        AI TmpPtr<SkNx<N, T>> thenElse(const SkNx& t, const SkNx& e) const {
            return new SkNx{ TmpPtr<Half>(fLo.thenElse(t.fLo, e.fLo)), TmpPtr<Half>(fHi.thenElse(t.fHi, e.fHi)) };
        }
        AI static TmpPtr<SkNx<N, T>> Min(const SkNx& x, const SkNx& y) {
            return new SkNx{ TmpPtr<Half>(Half::Min(x.fLo, y.fLo)), TmpPtr<Half>(Half::Min(x.fHi, y.fHi)) };
        }
        AI static TmpPtr<SkNx<N, T>> Max(const SkNx& x, const SkNx& y) {
            return new SkNx{ TmpPtr<Half>(Half::Max(x.fLo, y.fLo)), TmpPtr<Half>(Half::Max(x.fHi, y.fHi)) };
        }
    };

    // The N -> N/2 recursion bottoms out at N == 1, a scalar value.
    template <typename T>
    struct SkNx<1, T> {
        T fVal;

        AI SkNx() = default;
        AI SkNx(T v) : fVal(v) {}

        // Android complains against unused parameters, so we guard it
        AI T operator[](int SkDEBUGCODE(k)) const {
            SkASSERT(k == 0);
            return fVal;
        }

        AI static TmpPtr<SkNx<1, T>> Load(const void* ptr) {
            SkNx* v = new SkNx();
            memcpy(v, ptr, sizeof(T));
            return v;
        }

        AI void store(void* ptr) const { memcpy(ptr, &fVal, sizeof(T)); }

        AI static void Load4(const void* vptr, SkNx** a, SkNx** b, SkNx** c, SkNx** d) {
            auto ptr = (const char*)vptr;
            *a = Load(ptr + 0 * sizeof(T)).release();
            *b = Load(ptr + 1 * sizeof(T)).release();
            *c = Load(ptr + 2 * sizeof(T)).release();
            *d = Load(ptr + 3 * sizeof(T)).release();
        }
        AI static void Load3(const void* vptr, SkNx** a, SkNx** b, SkNx** c) {
            auto ptr = (const char*)vptr;
            *a = Load(ptr + 0 * sizeof(T)).release();
            *b = Load(ptr + 1 * sizeof(T)).release();
            *c = Load(ptr + 2 * sizeof(T)).release();
        }
        AI static void Load2(const void* vptr, SkNx** a, SkNx** b) {
            auto ptr = (const char*)vptr;
            *a = Load(ptr + 0 * sizeof(T)).release();
            *b = Load(ptr + 1 * sizeof(T)).release();
        }
        AI static void Store4(void* vptr, const SkNx& a, const SkNx& b, const SkNx& c, const SkNx& d) {
            auto ptr = (char*)vptr;
            a.store(ptr + 0 * sizeof(T));
            b.store(ptr + 1 * sizeof(T));
            c.store(ptr + 2 * sizeof(T));
            d.store(ptr + 3 * sizeof(T));
        }
        AI static void Store3(void* vptr, const SkNx& a, const SkNx& b, const SkNx& c) {
            auto ptr = (char*)vptr;
            a.store(ptr + 0 * sizeof(T));
            b.store(ptr + 1 * sizeof(T));
            c.store(ptr + 2 * sizeof(T));
        }
        AI static void Store2(void* vptr, const SkNx& a, const SkNx& b) {
            auto ptr = (char*)vptr;
            a.store(ptr + 0 * sizeof(T));
            b.store(ptr + 1 * sizeof(T));
        }

        AI T min() const { return fVal; }
        AI T max() const { return fVal; }
        AI bool anyTrue() const { return fVal != 0; }
        AI bool allTrue() const { return fVal != 0; }

        AI TmpPtr<SkNx<1, T>>    abs() const { return new SkNx(Abs(fVal)); }
        AI TmpPtr<SkNx<1, T>>   sqrt() const { return new SkNx(Sqrt(fVal)); }
        AI TmpPtr<SkNx<1, T>>  floor() const { return new SkNx(Floor(fVal)); }

        AI TmpPtr<SkNx<1, T>> operator!() const { return new SkNx(!fVal); }
        AI TmpPtr<SkNx<1, T>> operator-() const { return new SkNx(-fVal); }
        AI TmpPtr<SkNx<1, T>> operator~() const { return new SkNx(FromBits(~ToBits(fVal))); }

        AI TmpPtr<SkNx<1, T>> operator<<(int bits) const { return new SkNx(fVal << bits); }
        AI TmpPtr<SkNx<1, T>> operator>>(int bits) const { return new SkNx(fVal >> bits); }

        AI TmpPtr<SkNx<1, T>> operator+(const SkNx& y) const { return new SkNx(fVal + y.fVal); }
        AI TmpPtr<SkNx<1, T>> operator-(const SkNx& y) const { return new SkNx(fVal - y.fVal); }
        AI TmpPtr<SkNx<1, T>> operator*(const SkNx& y) const { return new SkNx(fVal * y.fVal); }
        AI TmpPtr<SkNx<1, T>> operator/(const SkNx& y) const { return new SkNx(fVal / y.fVal); }

        AI TmpPtr<SkNx<1, T>> operator&(const SkNx& y) const { return new SkNx(FromBits(ToBits(fVal) & ToBits(y.fVal))); }
        AI TmpPtr<SkNx<1, T>> operator|(const SkNx& y) const { return new SkNx(FromBits(ToBits(fVal) | ToBits(y.fVal))); }
        AI TmpPtr<SkNx<1, T>> operator^(const SkNx& y) const { return new SkNx(FromBits(ToBits(fVal) ^ ToBits(y.fVal))); }

        AI TmpPtr<SkNx<1, T>> operator==(const SkNx& y) const { return new SkNx(FromBits(fVal == y.fVal ? ~0 : 0)); }
        AI TmpPtr<SkNx<1, T>> operator!=(const SkNx& y) const { return new SkNx(FromBits(fVal != y.fVal ? ~0 : 0)); }
        AI TmpPtr<SkNx<1, T>> operator<=(const SkNx& y) const { return new SkNx(FromBits(fVal <= y.fVal ? ~0 : 0)); }
        AI TmpPtr<SkNx<1, T>> operator>=(const SkNx& y) const { return new SkNx(FromBits(fVal >= y.fVal ? ~0 : 0)); }
        AI TmpPtr<SkNx<1, T>> operator< (const SkNx& y) const { return new SkNx(FromBits(fVal < y.fVal ? ~0 : 0)); }
        AI TmpPtr<SkNx<1, T>> operator> (const SkNx& y) const { return new SkNx(FromBits(fVal > y.fVal ? ~0 : 0)); }

        AI static TmpPtr<SkNx<1, T>> Min(const SkNx& x, const SkNx& y) { return new SkNx(x.fVal < y.fVal ? x : y); }
        AI static TmpPtr<SkNx<1, T>> Max(const SkNx& x, const SkNx& y) { return new SkNx(x.fVal > y.fVal ? x : y); }

        AI TmpPtr<SkNx<1, T>> saturatedAdd(const SkNx& y) const {
            static_assert(std::is_unsigned<T>::value, "cannot be instantiated for signed T");
            T sum = fVal + y.fVal;
            return new SkNx(sum < fVal ? std::numeric_limits<T>::max() : sum);
        }

        AI TmpPtr<SkNx<1, T>> mulHi(const SkNx& m) const {
            static_assert(std::is_unsigned<T>::value, "cannot be instantiated for signed T");
            static_assert(sizeof(T) <= 4, "cannot be instantiated for T with a sizeof(T) > 4");
            return new SkNx(static_cast<T>((static_cast<uint64_t>(fVal) * m.fVal) >> (sizeof(T) * 8)));
        }

        AI TmpPtr<SkNx<1, T>> thenElse(const SkNx& t, const SkNx& e) const { return new SkNx(fVal != 0 ? t : e); }

    private:
        // Helper functions to choose the right float/double methods.  (In <cmath> madness lies...)
        AI static int     Abs(int val) { return  val < 0 ? -val : val; }

        AI static float   Abs(float val) { return  ::fabsf(val); }
        AI static float  Sqrt(float val) { return  ::sqrtf(val); }
        AI static float Floor(float val) { return ::floorf(val); }

        AI static double   Abs(double val) { return  ::fabs(val); }
        AI static double  Sqrt(double val) { return  ::sqrt(val); }
        AI static double Floor(double val) { return ::floor(val); }

        // Helper functions for working with floats/doubles as bit patterns.
        template <typename U>
        AI static U ToBits(U v) { return v; }
        AI static int32_t ToBits(float  v) { int32_t bits; memcpy(&bits, &v, sizeof(v)); return bits; }
        AI static int64_t ToBits(double v) { int64_t bits; memcpy(&bits, &v, sizeof(v)); return bits; }

        template <typename Bits>
        AI static T FromBits(Bits bits) {
            static_assert(std::is_pod<T   >::value &&
                std::is_pod<Bits>::value &&
                sizeof(T) <= sizeof(Bits), "sizeof T is greater than sizeof Bits");
            T val;
            memcpy(&val, &bits, sizeof(T));
            return val;
        }
    };

    // Allow scalars on the left or right of binary operators, and things like +=, &=, etc.
#define V template <int N, typename T> AI static TmpPtr<SkNx<N,T>>
    V operator+ (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) + y; }
    V operator- (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) - y; }
    V operator* (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) * y; }
    V operator/ (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) / y; }
    V operator& (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) & y; }
    V operator| (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) | y; }
    V operator^ (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) ^ y; }
    V operator==(T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) == y; }
    V operator!=(T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) != y; }
    V operator<=(T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) <= y; }
    V operator>=(T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) >= y; }
    V operator< (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) < y; }
    V operator> (T x, const SkNx<N, T>& y) { return SkNx<N, T>(x) > y; }

    V operator+ (const SkNx<N, T>& x, T y) { return x + SkNx<N, T>(y); }
    V operator- (const SkNx<N, T>& x, T y) { return x - SkNx<N, T>(y); }
    V operator* (const SkNx<N, T>& x, T y) { return x * SkNx<N, T>(y); }
    V operator/ (const SkNx<N, T>& x, T y) { return x / SkNx<N, T>(y); }
    V operator& (const SkNx<N, T>& x, T y) { return x & SkNx<N, T>(y); }
    V operator| (const SkNx<N, T>& x, T y) { return x | SkNx<N, T>(y); }
    V operator^ (const SkNx<N, T>& x, T y) { return x ^ SkNx<N, T>(y); }
    V operator==(const SkNx<N, T>& x, T y) { return x == SkNx<N, T>(y); }
    V operator!=(const SkNx<N, T>& x, T y) { return x != SkNx<N, T>(y); }
    V operator<=(const SkNx<N, T>& x, T y) { return x <= SkNx<N, T>(y); }
    V operator>=(const SkNx<N, T>& x, T y) { return x >= SkNx<N, T>(y); }
    V operator< (const SkNx<N, T>& x, T y) { return x < SkNx<N, T>(y); }
    V operator> (const SkNx<N, T>& x, T y) { return x > SkNx<N, T>(y); }

#define OP(a, OP, b)     TmpPtr<SkNx<N, T>> tmp = a OP b; a = tmp; return tmp

    V& operator<<=(SkNx<N, T>& x, int bits) { OP(x, << , bits); }
    V& operator>>=(SkNx<N, T>& x, int bits) { OP(x, >> , bits); }

    V& operator +=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, +, y); }
    V& operator -=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, -, y); }
    V& operator *=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, *, y); }
    V& operator /=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, / , y); }
    V& operator &=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, &, y); }
    V& operator |=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, | , y); }
    V& operator ^=(SkNx<N, T>& x, const SkNx<N, T>& y) { OP(x, ^, y); }

#define OP2(a, OP, b)     TmpPtr<SkNx<N, T>> tmp = a OP SkNx<N, T>(b); a = tmp; return tmp

    V& operator +=(SkNx<N, T>& x, T y) { OP2(x, +, y); }
    V& operator -=(SkNx<N, T>& x, T y) { OP2(x, -, y); }
    V& operator *=(SkNx<N, T>& x, T y) { OP2(x, *, y); }
    V& operator /=(SkNx<N, T>& x, T y) { OP2(x, &, y); }
    V& operator &=(SkNx<N, T>& x, T y) { OP2(x, &, y); }
    V& operator |=(SkNx<N, T>& x, T y) { OP2(x, | , y); }
    V& operator ^=(SkNx<N, T>& x, T y) { OP2(x, ^, y); }
#undef V
}  // namespace

// header
#define DEFINE_ALLOCATION0(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__0();
#define DEFINE_ALLOCATION1(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__1(T value);
#define DEFINE_ALLOCATION2(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__2(T a, T b);
#define DEFINE_ALLOCATION2_(N, HALF_N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__2HALF(void* a, void* b);
#define DEFINE_ALLOCATION4(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__4(T a, T b, T c, T d);
#define DEFINE_ALLOCATION8(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__8(T a, T b, T c, T d, T e, T f, T g, T h);
#define DEFINE_ALLOCATION16(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__16(T a, T b, T c, T d, T e, T f, T g, T h, T i, T j, T k, T l, T m, T n, T o, T p);
#define DEFINE_DELETE(N, T, NAME) extern "C" SK_API void delete_Sk##N##NAME(void * ptr);

#define DEFINE_FUNCTION_CALL0(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL);
#define DEFINE_FUNCTION_CALL0_NO_RELEASE(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL);
#define DEFINE_FUNCTION_CALL1_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL, T1) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value);
#define DEFINE_FUNCTION_CALL1(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value);
#define DEFINE_FUNCTION_CALL1_NO_RELEASE(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value);
#define DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value);
#define DEFINE_FUNCTION_CALL2_SELF(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value1, void* value2);

#define DEFINE_STATIC_FUNCTION_CALL1(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void* value1);
#define DEFINE_STATIC_FUNCTION_CALL2_SELF(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2);
#define DEFINE_STATIC_FUNCTION_CALL3V_SELF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3);
#define DEFINE_STATIC_FUNCTION_CALL3V_SELF_REF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3);
#define DEFINE_STATIC_FUNCTION_CALL4V_SELF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3, void** value4);
#define DEFINE_STATIC_FUNCTION_CALL4V_SELF_REF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3, void* value4);
#define DEFINE_STATIC_FUNCTION_CALL5V_SELF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3, void** value4, void** value5);
#define DEFINE_STATIC_FUNCTION_CALL5V_SELF_REF_RETURN_VOID(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3, void* value4, void* value5);

#define DEFINE_FUNCTION_CALL1S(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value);
#define DEFINE_FUNCTION_CALL1S2(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(T1 value, void * ptr);
#define DEFINE_FUNCTION_CALL1SA(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value);
#define DEFINE_FUNCTION_CALL2_SELFS(N, HALF_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value1, void* value2);
#define DEFINE_FUNCTION_CALL2_SELFJ(N, HALF_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2);
#define DEFINE_FUNCTION_CALL3_SELFFMA(N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3);

#define DEFINE_SUFFLE2(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##2(void * ptr, int Ix1, int Ix2);
#define DEFINE_SUFFLE4(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##4(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4);
#define DEFINE_SUFFLE8(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##8(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8);
#define DEFINE_SUFFLE16(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##16(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16);
#define DEFINE_SUFFLE_ALL(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE2(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE4(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE8(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE16(N, T, NAME, FUNC_C_NAME)

// impl
#define DEFINE_ALLOCATION0_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__0() { return new SkNx<N, T>(); }
#define DEFINE_ALLOCATION1_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__1(T value) { return new SkNx<N, T>(value); }
#define DEFINE_ALLOCATION2_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__2(T a, T b) { return new SkNx<N, T>(a, b); }
#define DEFINE_ALLOCATION2__IMPL(N, HALF_N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__2HALF(void* a, void* b) { return new SkNx<N, T>(AS_SKNX(HALF_N, T, a)[0], AS_SKNX(HALF_N, T, b)[0]); }
#define DEFINE_ALLOCATION4_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__4(T a, T b, T c, T d) { return new SkNx<N, T>(a, b, c, d); }
#define DEFINE_ALLOCATION8_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__8(T a, T b, T c, T d, T e, T f, T g, T h) { return new SkNx<N, T>(a, b, c, d, e, f, g, h); }
#define DEFINE_ALLOCATION16_IMPL(N, T, NAME) extern "C" SK_API void * new_Sk##N##NAME##__16(T a, T b, T c, T d, T e, T f, T g, T h, T i, T j, T k, T l, T m, T n, T o, T p) { return new SkNx<N, T>(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p); }
#define DEFINE_DELETE_IMPL(N, T, NAME) extern "C" SK_API void delete_Sk##N##NAME(void * ptr) { delete AS_SKNX(N, T, ptr); }

#define DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr) { return AS_SKNX(N, T, ptr)->FUNC_TO_CALL().release(); }
#define DEFINE_FUNCTION_CALL0_NO_RELEASE_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr) { return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(); }
#define DEFINE_FUNCTION_CALL1_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL, T1) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value) { AS_SKNX(N, T, ptr)->FUNC_TO_CALL(value); }
#define DEFINE_FUNCTION_CALL1_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value) { return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(value).release(); }
#define DEFINE_FUNCTION_CALL1_NO_RELEASE_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value) { return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(value); }
#define DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value) { auto * a = AS_SKNX(N, T, value); return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(*a).release(); }
#define DEFINE_FUNCTION_CALL2_SELF_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value1, void* value2) { auto * a = AS_SKNX(N, T, value1); auto * b = AS_SKNX(N, T, value2); return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(*a, *b).release(); }

#define DEFINE_STATIC_FUNCTION_CALL1_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void* value1) { return SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1).release(); }
#define DEFINE_STATIC_FUNCTION_CALL2_SELF_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2) { SkNx<N, T> * a = AS_SKNX(N, T, value1); SkNx<N, T> * b = AS_SKNX(N, T, value2); return SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(*a, *b).release(); }
#define DEFINE_STATIC_FUNCTION_CALL3V_SELF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3) { SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, AS_SKNX_REF(N, T, value2), AS_SKNX_REF(N, T, value3)); }
#define DEFINE_STATIC_FUNCTION_CALL3V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3) { SkNx<N, T> * a = AS_SKNX(N, T, value2); SkNx<N, T> * b = AS_SKNX(N, T, value3); SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, *a, *b); }
#define DEFINE_STATIC_FUNCTION_CALL4V_SELF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3, void** value4) { SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, AS_SKNX_REF(N, T, value2), AS_SKNX_REF(N, T, value3), AS_SKNX_REF(N, T, value4)); }
#define DEFINE_STATIC_FUNCTION_CALL4V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3, void* value4) { SkNx<N, T> * a = AS_SKNX(N, T, value2); SkNx<N, T> * b = AS_SKNX(N, T, value3); SkNx<N, T> * c = AS_SKNX(N, T, value4); SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, *a, *b, *c); }
#define DEFINE_STATIC_FUNCTION_CALL5V_SELF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void** value2, void** value3, void** value4, void** value5) { SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, AS_SKNX_REF(N, T, value2), AS_SKNX_REF(N, T, value3), AS_SKNX_REF(N, T, value4), AS_SKNX_REF(N, T, value5)); }
#define DEFINE_STATIC_FUNCTION_CALL5V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, FUNC_C_NAME, FUNC_TO_CALL) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3, void* value4, void* value5) { SkNx<N, T> * a = AS_SKNX(N, T, value2); SkNx<N, T> * b = AS_SKNX(N, T, value3); SkNx<N, T> * c = AS_SKNX(N, T, value4); SkNx<N, T> * d = AS_SKNX(N, T, value5); SKNX_TEMPLATE(N, T)::FUNC_TO_CALL(value1, *a, *b, *c, *d); }

#define DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value) { return AS_SKNX(N, T, ptr)->FUNC_TO_CALL(value); }
#define DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(T1 value, void * ptr) { return SkNx<N, T>(value).FUNC_TO_CALL(AS_SKNX(N, T, ptr)[0]); }
#define DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, FUNC_C_NAME, RETURN_TYPE, FUNC_TO_CALL, T1) extern "C" SK_API RETURN_TYPE Sk##N##NAME##__##FUNC_C_NAME(void * ptr, T1 value) { auto * t = AS_SKNX(N, T, ptr); *t = *(AS_SKNX(N, T, ptr)->FUNC_TO_CALL(value)); return t; }
#define DEFINE_FUNCTION_CALL2_SELFS_IMPL(N, HALF_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void Sk##N##NAME##__##FUNC_C_NAME(void * ptr, void* value1, void* value2) { AS_SKNX(HALF_N, T, value1)[0] = AS_SKNX(N, T, ptr)->fLo; AS_SKNX(HALF_N, T, value2)[0] = AS_SKNX(N, T, ptr)->fHi; }
#define DEFINE_FUNCTION_CALL2_SELFJ_IMPL(N, HALF_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2) { return new SkNx<N, T>(AS_SKNX(HALF_N, T, value1)[0], AS_SKNX(HALF_N, T, value2)[0]); }
#define DEFINE_FUNCTION_CALL3_SELFFMA_IMPL(N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##N##NAME##__##FUNC_C_NAME(void* value1, void* value2, void* value3) { auto a = AS_SKNX(N, T, value1)[0] + AS_SKNX(N, T, value2)[0]; auto & b = *a; auto c = b + AS_SKNX(N, T, value3)[0]; return new SkNx<N, T>(*c); }

#define DEFINE_SUFFLE2_IMPL(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##2(void * ptr, int Ix1, int Ix2) { SkNx<INPUT_N, T> & v = AS_SKNX(INPUT_N, T, ptr)[0]; return new SkNx<2, T>(v[Ix1], v[Ix2]); }
#define DEFINE_SUFFLE4_IMPL(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##4(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4) { SkNx<INPUT_N, T> & v = AS_SKNX(INPUT_N, T, ptr)[0]; return new SkNx<4, T>(v[Ix1], v[Ix2], v[Ix3], v[Ix4]); }
#define DEFINE_SUFFLE8_IMPL(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##8(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8) { SkNx<INPUT_N, T> & v = AS_SKNX(INPUT_N, T, ptr)[0]; return new SkNx<8, T>(v[Ix1], v[Ix2], v[Ix3], v[Ix4], v[Ix5], v[Ix6], v[Ix7], v[Ix8]); }
#define DEFINE_SUFFLE16_IMPL(INPUT_N, T, NAME, FUNC_C_NAME) extern "C" SK_API void* Sk##INPUT_N##NAME##__##FUNC_C_NAME##16(void * ptr, int Ix1, int Ix2, int Ix3, int Ix4, int Ix5, int Ix6, int Ix7, int Ix8, int Ix9, int Ix10, int Ix11, int Ix12, int Ix13, int Ix14, int Ix15, int Ix16) { SkNx<INPUT_N, T> & v = AS_SKNX(INPUT_N, T, ptr)[0]; return new SkNx<16, T>(v[Ix1], v[Ix2], v[Ix3], v[Ix4], v[Ix5], v[Ix6], v[Ix7], v[Ix8], v[Ix9], v[Ix10], v[Ix11], v[Ix12], v[Ix13], v[Ix14], v[Ix15], v[Ix16]); }
#define DEFINE_SUFFLE_ALL_IMPL(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE2_IMPL(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE4_IMPL(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE8_IMPL(N, T, NAME, FUNC_C_NAME) \
DEFINE_SUFFLE16_IMPL(N, T, NAME, FUNC_C_NAME)

#define DEFINE_BASIC_SK(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL1_NO_RELEASE(N, T, NAME, index, T, operator[], int) \
DEFINE_STATIC_FUNCTION_CALL1(N, T, NAME, Load, void*, Load) \
DEFINE_FUNCTION_CALL1_RETURN_VOID(N, T, NAME, store, store, void*) \
DEFINE_STATIC_FUNCTION_CALL5V_SELF_RETURN_VOID(N, T, NAME, Load4, Load4) \
DEFINE_STATIC_FUNCTION_CALL4V_SELF_RETURN_VOID(N, T, NAME, Load3, Load3) \
DEFINE_STATIC_FUNCTION_CALL3V_SELF_RETURN_VOID(N, T, NAME, Load2, Load2) \
DEFINE_STATIC_FUNCTION_CALL5V_SELF_REF_RETURN_VOID(N, T, NAME, Store4, Store4) \
DEFINE_STATIC_FUNCTION_CALL4V_SELF_REF_RETURN_VOID(N, T, NAME, Store3, Store3) \
DEFINE_STATIC_FUNCTION_CALL3V_SELF_REF_RETURN_VOID(N, T, NAME, Store2, Store2) \
DEFINE_FUNCTION_CALL0_NO_RELEASE(N, T, NAME, min, T, min) \
DEFINE_FUNCTION_CALL0_NO_RELEASE(N, T, NAME, max, T, max) \
DEFINE_FUNCTION_CALL0_NO_RELEASE(N, T, NAME, anyTrue, bool, anyTrue) \
DEFINE_FUNCTION_CALL0_NO_RELEASE(N, T, NAME, allTrue, bool, allTrue) \
DEFINE_FUNCTION_CALL0(N, T, NAME, operator_logical_not, void*, operator!) \
DEFINE_FUNCTION_CALL0(N, T, NAME, operator_binary_ones_complement, void*, operator~) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_add, void*, operator+) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_subtract, void*, operator-) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_multiply, void*, operator*) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_divide, void*, operator/) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_bitwise_AND, void*, operator&) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_bitwise_OR, void*, operator|) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_bitwise_XOR, void*, operator^) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_equal_to, void*, operator==) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_not_equal_to, void*, operator!=) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_less_than_or_equal_to, void*, operator<=) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_greater_than_or_equal_to, void*, operator>=) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_less_than, void*, operator<) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, operator_greater_than, void*, operator>) \
DEFINE_STATIC_FUNCTION_CALL2_SELF(N, T, NAME, Min, void*, Min) \
DEFINE_STATIC_FUNCTION_CALL2_SELF(N, T, NAME, Max, void*, Max) \
DEFINE_FUNCTION_CALL2_SELF(N, T, NAME, thenElse, void*, thenElse) \
/* Allow scalars on the left or right of binary operators, and things like +=, &=, etc. */ \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_add__scalar_rhs, void*, operator+, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_subtract__scalar_rhs, void*, operator-, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_multiply__scalar_rhs, void*, operator*, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_divide__scalar_rhs, void*, operator/, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_bitwise_AND__scalar_rhs, void*, operator&, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_bitwise_OR__scalar_rhs, void*, operator|, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_bitwise_XOR__scalar_rhs, void*, operator^, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_equal_to__scalar_rhs, void*, operator==, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_not_equal_to__scalar_rhs, void*, operator!=, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_less_than_or_equal_to__scalar_rhs, void*, operator<=, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_greater_than_or_equal_to__scalar_rhs, void*, operator>=, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_less_than__scalar_rhs, void*, operator<, T) \
DEFINE_FUNCTION_CALL1S(N, T, NAME, operator_greater_than__scalar_rhs, void*, operator>, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_add__scalar_lhs, void*, operator+, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_subtract__scalar_lhs, void*, operator-, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_multiply__scalar_lhs, void*, operator*, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_divide__scalar_lhs, void*, operator/, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_bitwise_AND__scalar_lhs, void*, operator&, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_bitwise_OR__scalar_lhs, void*, operator|, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_bitwise_XOR__scalar_lhs, void*, operator^, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_equal_to__scalar_lhs, void*, operator==, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_not_equal_to__scalar_lhs, void*, operator!=, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_less_than_or_equal_to__scalar_lhs, void*, operator<=, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_greater_than_or_equal_to__scalar_lhs, void*, operator>=, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_less_than__scalar_lhs, void*, operator<, T) \
DEFINE_FUNCTION_CALL1S2(N, T, NAME, operator_greater_than__scalar_lhs, void*, operator>, T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_add__scalar, void*, operator+, T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_subtract__scalar, void*, operator-, T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_multiply__scalar, void*, operator*, T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_divide__scalar, void*, operator/ , T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_bitwise_AND__scalar, void*, operator&, T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_bitwise_OR__scalar, void*, operator| , T) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_bitwise_XOR__scalar, void*, operator^, T) \
DEFINE_FUNCTION_CALL3_SELFFMA(N, T, NAME, fma) \
DEFINE_FUNCTION_CALL2_SELFS(N, HALF_N, T, NAME, split) \
DEFINE_FUNCTION_CALL2_SELFJ(N, HALF_N, T, NAME, join) \
DEFINE_SUFFLE_ALL(N, T, NAME, suffle) \
DEFINE_DELETE(N, T, NAME)

#define DEFINE_FSK(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL0(N, T, NAME, abs, void*, abs) \
DEFINE_FUNCTION_CALL0(N, T, NAME, operator_unary_minus, void*, operator-)

#define DEFINE_ISK(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL0(N, T, NAME, abs, void*, abs) \
DEFINE_FUNCTION_CALL0(N, T, NAME, operator_unary_minus, void*, operator-) \
DEFINE_FUNCTION_CALL1(N, T, NAME, operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1(N, T, NAME, operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_binary_right_shift, void*, operator>>, int)

#define DEFINE_USK(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL1(N, T, NAME, operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1(N, T, NAME, operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1SA(N, T, NAME, assign_operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, saturatedAdd, void*, saturatedAdd) \
DEFINE_FUNCTION_CALL1_SELF(N, T, NAME, mulHi, void*, mulHi)



#define DEFINE_BASIC_SK_IMPL(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL1_NO_RELEASE_IMPL(N, T, NAME, index, T, operator[], int) \
DEFINE_STATIC_FUNCTION_CALL1_IMPL(N, T, NAME, Load, void*, Load) \
DEFINE_FUNCTION_CALL1_RETURN_VOID_IMPL(N, T, NAME, store, store, void*) \
DEFINE_STATIC_FUNCTION_CALL5V_SELF_RETURN_VOID_IMPL(N, T, NAME, Load4, Load4) \
DEFINE_STATIC_FUNCTION_CALL4V_SELF_RETURN_VOID_IMPL(N, T, NAME, Load3, Load3) \
DEFINE_STATIC_FUNCTION_CALL3V_SELF_RETURN_VOID_IMPL(N, T, NAME, Load2, Load2) \
DEFINE_STATIC_FUNCTION_CALL5V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, Store4, Store4) \
DEFINE_STATIC_FUNCTION_CALL4V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, Store3, Store3) \
DEFINE_STATIC_FUNCTION_CALL3V_SELF_REF_RETURN_VOID_IMPL(N, T, NAME, Store2, Store2) \
DEFINE_FUNCTION_CALL0_NO_RELEASE_IMPL(N, T, NAME, min, T, min) \
DEFINE_FUNCTION_CALL0_NO_RELEASE_IMPL(N, T, NAME, max, T, max) \
DEFINE_FUNCTION_CALL0_NO_RELEASE_IMPL(N, T, NAME, anyTrue, bool, anyTrue) \
DEFINE_FUNCTION_CALL0_NO_RELEASE_IMPL(N, T, NAME, allTrue, bool, allTrue) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, operator_logical_not, void*, operator!) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, operator_binary_ones_complement, void*, operator~) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_add, void*, operator+) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_subtract, void*, operator-) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_multiply, void*, operator*) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_divide, void*, operator/) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_bitwise_AND, void*, operator&) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_bitwise_OR, void*, operator|) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_bitwise_XOR, void*, operator^) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_equal_to, void*, operator==) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_not_equal_to, void*, operator!=) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_less_than_or_equal_to, void*, operator<=) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_greater_than_or_equal_to, void*, operator>=) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_less_than, void*, operator<) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, operator_greater_than, void*, operator>) \
DEFINE_STATIC_FUNCTION_CALL2_SELF_IMPL(N, T, NAME, Min, void*, Min) \
DEFINE_STATIC_FUNCTION_CALL2_SELF_IMPL(N, T, NAME, Max, void*, Max) \
DEFINE_FUNCTION_CALL2_SELF_IMPL(N, T, NAME, thenElse, void*, thenElse) \
/* Allow scalars on the left or right of binary operators, and things like +=, &=, etc. */ \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_add__scalar_rhs, void*, operator+, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_subtract__scalar_rhs, void*, operator-, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_multiply__scalar_rhs, void*, operator*, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_divide__scalar_rhs, void*, operator/, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_bitwise_AND__scalar_rhs, void*, operator&, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_bitwise_OR__scalar_rhs, void*, operator|, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_bitwise_XOR__scalar_rhs, void*, operator^, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_equal_to__scalar_rhs, void*, operator==, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_not_equal_to__scalar_rhs, void*, operator!=, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_less_than_or_equal_to__scalar_rhs, void*, operator<=, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_greater_than_or_equal_to__scalar_rhs, void*, operator>=, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_less_than__scalar_rhs, void*, operator<, T) \
DEFINE_FUNCTION_CALL1S_IMPL(N, T, NAME, operator_greater_than__scalar_rhs, void*, operator>, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_add__scalar_lhs, void*, operator+, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_subtract__scalar_lhs, void*, operator-, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_multiply__scalar_lhs, void*, operator*, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_divide__scalar_lhs, void*, operator/, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_bitwise_AND__scalar_lhs, void*, operator&, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_bitwise_OR__scalar_lhs, void*, operator|, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_bitwise_XOR__scalar_lhs, void*, operator^, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_equal_to__scalar_lhs, void*, operator==, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_not_equal_to__scalar_lhs, void*, operator!=, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_less_than_or_equal_to__scalar_lhs, void*, operator<=, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_greater_than_or_equal_to__scalar_lhs, void*, operator>=, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_less_than__scalar_lhs, void*, operator<, T) \
DEFINE_FUNCTION_CALL1S2_IMPL(N, T, NAME, operator_greater_than__scalar_lhs, void*, operator>, T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_add__scalar, void*, operator+, T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_subtract__scalar, void*, operator-, T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_multiply__scalar, void*, operator*, T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_divide__scalar, void*, operator/ , T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_bitwise_AND__scalar, void*, operator&, T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_bitwise_OR__scalar, void*, operator| , T) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_bitwise_XOR__scalar, void*, operator^, T) \
DEFINE_FUNCTION_CALL3_SELFFMA_IMPL(N, T, NAME, fma) \
DEFINE_FUNCTION_CALL2_SELFS_IMPL(N, HALF_N, T, NAME, split) \
DEFINE_FUNCTION_CALL2_SELFJ_IMPL(N, HALF_N, T, NAME, join) \
DEFINE_SUFFLE_ALL_IMPL(N, T, NAME, suffle) \
DEFINE_DELETE_IMPL(N, T, NAME)

#define DEFINE_FSK_IMPL(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK_IMPL(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, abs, void*, abs) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, operator_unary_minus, void*, operator-)

#define DEFINE_ISK_IMPL(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK_IMPL(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, abs, void*, abs) \
DEFINE_FUNCTION_CALL0_IMPL(N, T, NAME, operator_unary_minus, void*, operator-) \
DEFINE_FUNCTION_CALL1_IMPL(N, T, NAME, operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1_IMPL(N, T, NAME, operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_binary_right_shift, void*, operator>>, int)

#define DEFINE_USK_IMPL(N, HALF_N, T, NAME) \
DEFINE_BASIC_SK_IMPL(N, HALF_N, T, NAME) \
DEFINE_FUNCTION_CALL1_IMPL(N, T, NAME, operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1_IMPL(N, T, NAME, operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_binary_left_shift, void*, operator<<, int) \
DEFINE_FUNCTION_CALL1SA_IMPL(N, T, NAME, assign_operator_binary_right_shift, void*, operator>>, int) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, saturatedAdd, void*, saturatedAdd) \
DEFINE_FUNCTION_CALL1_SELF_IMPL(N, T, NAME, mulHi, void*, mulHi)

/*
ClangSharpPInvokeGenerator.exe -f .\SkXn.h -l "runtimes/win-x64/native/AndroidUI.Native.Windows.dll"           -n Bindings_Windows -m Native -c multi-file -c generate-helper-types -o ..\AndroidUI.Native.Nuget\Bindings_Windows -c compatible-codegen
ClangSharpPInvokeGenerator.exe -f .\SkXn.h -l "runtimes/monoandroid-x64/native/libAndroidUI_Native_Android.so" -n Bindings_Android -m Native -c multi-file -c generate-helper-types -o ..\AndroidUI.Native.Nuget\Bindings_Android -c compatible-codegen
*/

DEFINE_ALLOCATION0(2, float, f)
DEFINE_ALLOCATION1(2, float, f)
DEFINE_ALLOCATION2_(2, 1, float, f)
DEFINE_ALLOCATION2(2, float, f)

DEFINE_ALLOCATION0(4, float, f)
DEFINE_ALLOCATION1(4, float, f)
DEFINE_ALLOCATION2_(4, 2, float, f)
DEFINE_ALLOCATION4(4, float, f)

DEFINE_ALLOCATION0(8, float, f)
DEFINE_ALLOCATION1(8, float, f)
DEFINE_ALLOCATION2_(8, 4, float, f)
DEFINE_ALLOCATION8(8, float, f)

DEFINE_ALLOCATION0(16, float, f)
DEFINE_ALLOCATION1(16, float, f)
DEFINE_ALLOCATION2_(16, 8, float, f)
DEFINE_ALLOCATION16(16, float, f)

DEFINE_FSK(2, 1, float, f)
DEFINE_FSK(4, 2, float, f)
DEFINE_FSK(8, 4, float, f)
DEFINE_FSK(16, 8, float, f)

DEFINE_ALLOCATION0(2, float, s)
DEFINE_ALLOCATION1(2, float, s)
DEFINE_ALLOCATION2_(2, 1, float, s)
DEFINE_ALLOCATION2(2, float, s)

DEFINE_ALLOCATION0(4, float, s)
DEFINE_ALLOCATION1(4, float, s)
DEFINE_ALLOCATION2_(4, 2, float, s)
DEFINE_ALLOCATION4(4, float, s)

DEFINE_ALLOCATION0(8, float, s)
DEFINE_ALLOCATION1(8, float, s)
DEFINE_ALLOCATION2_(8, 4, float, s)
DEFINE_ALLOCATION8(8, float, s)

DEFINE_ALLOCATION0(16, float, s)
DEFINE_ALLOCATION1(16, float, s)
DEFINE_ALLOCATION2_(16, 8, float, s)
DEFINE_ALLOCATION16(16, float, s)

DEFINE_FSK(2, 1, float, s)
DEFINE_FSK(4, 2, float, s)
DEFINE_FSK(8, 4, float, s)
DEFINE_FSK(16, 8, float, s)

DEFINE_ALLOCATION0(4, uint8_t, b)
DEFINE_ALLOCATION1(4, uint8_t, b)
DEFINE_ALLOCATION2_(4, 2, uint8_t, b)
DEFINE_ALLOCATION4(4, uint8_t, b)
DEFINE_ALLOCATION0(8, uint8_t, b)
DEFINE_ALLOCATION1(8, uint8_t, b)
DEFINE_ALLOCATION2_(8, 4, uint8_t, b)
DEFINE_ALLOCATION8(8, uint8_t, b)
DEFINE_ALLOCATION0(16, uint8_t, b)
DEFINE_ALLOCATION1(16, uint8_t, b)
DEFINE_ALLOCATION2_(16, 8, uint8_t, b)
DEFINE_ALLOCATION16(16, uint8_t, b)
DEFINE_USK(4, 2, uint8_t, b)
DEFINE_USK(8, 4, uint8_t, b)
DEFINE_USK(16, 8, uint8_t, b)

DEFINE_ALLOCATION0(4, uint16_t, h)
DEFINE_ALLOCATION1(4, uint16_t, h)
DEFINE_ALLOCATION2_(4, 2, uint16_t, h)
DEFINE_ALLOCATION4(4, uint16_t, h)
DEFINE_ALLOCATION0(8, uint16_t, h)
DEFINE_ALLOCATION1(8, uint16_t, h)
DEFINE_ALLOCATION2_(8, 4, uint16_t, h)
DEFINE_ALLOCATION8(8, uint16_t, h)
DEFINE_ALLOCATION0(16, uint16_t, h)
DEFINE_ALLOCATION1(16, uint16_t, h)
DEFINE_ALLOCATION2_(16, 8, uint16_t, h)
DEFINE_ALLOCATION16(16, uint16_t, h)
DEFINE_USK(4, 2, uint16_t, h)
DEFINE_USK(8, 4, uint16_t, h)
DEFINE_USK(16, 8, uint16_t, h)

DEFINE_ALLOCATION0(4, int32_t, i)
DEFINE_ALLOCATION1(4, int32_t, i)
DEFINE_ALLOCATION2_(4, 2, int32_t, i)
DEFINE_ALLOCATION4(4, int32_t, i)
DEFINE_ALLOCATION0(8, int32_t, i)
DEFINE_ALLOCATION1(8, int32_t, i)
DEFINE_ALLOCATION2_(8, 4, int32_t, i)
DEFINE_ALLOCATION8(8, int32_t, i)
DEFINE_ALLOCATION0(4, uint32_t, u)
DEFINE_ALLOCATION1(4, uint32_t, u)
DEFINE_ALLOCATION2_(4, 2, uint32_t, u)
DEFINE_ALLOCATION4(4, uint32_t, u)
DEFINE_ISK(4, 2, int32_t, i)
DEFINE_ISK(8, 4, int32_t, i)
DEFINE_USK(4, 2, uint32_t, u)


// Include platform specific specializations if available.
#if !defined(SKNX_NO_SIMD) && SK_CPU_SSE_LEVEL >= SK_CPU_SSE_LEVEL_SSE2
// #include "SkNx_sse.h"
#elif !defined(SKNX_NO_SIMD) && defined(SK_ARM_HAS_NEON)
// #include "SkNx_neon.h"
#endif

#undef AI

#endif//SkNx_DEFINED