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

// Include platform specific specializations if available.
#if !defined(SKNX_NO_SIMD) && SK_CPU_SSE_LEVEL >= SK_CPU_SSE_LEVEL_SSE2
// #include "SkNx_sse.h"
#elif !defined(SKNX_NO_SIMD) && defined(SK_ARM_HAS_NEON)
// #include "SkNx_neon.h"
#endif

#undef AI

#endif//SkNx_DEFINED