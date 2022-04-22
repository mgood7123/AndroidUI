#pragma once

template <typename T>
class TmpPtr
{
    T* ptr = nullptr;

public:
    TmpPtr() : ptr(nullptr) {}
    TmpPtr(T* ptr) : ptr(ptr) {}

    TmpPtr(TmpPtr& t) { ptr = t.release(); }
    TmpPtr(TmpPtr&& t) { ptr = t.release(); }

    TmpPtr<T>* operator=(T* ptr) {
        delete this->ptr;
        this->ptr = ptr;
        return this;
    }

    TmpPtr<T>* operator=(TmpPtr<T>& ptr) {
        this->ptr = ptr.release();
        return this;
    }

    TmpPtr<T>* operator=(TmpPtr<T>&& ptr) {
        this->ptr = ptr.release();
        return this;
    }

    const T& operator * () const { return *ptr; }
    operator const T*& () const { return ptr; }
    operator const T& () const { return *ptr; }
    operator const T () const { return *ptr; }

    T& operator * () { return *ptr; }
    operator T*& () { return ptr; }
    operator T& () { return *ptr; }
    operator T () { return *ptr; }

    T** address()
    {
        return &ptr;
    }

    T* release()
    {
        T* p = ptr;
        ptr = nullptr;
        return p;
    }

    T value()
    {
        return *ptr;
    }

    const T value() const
    {
        return *ptr;
    }

    T& valueRef()
    {
        return *ptr;
    }

    const T& valueRef() const
    {
        return *ptr;
    }

    ~TmpPtr() { delete ptr; }
};