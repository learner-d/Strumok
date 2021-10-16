#include "pch.h"
#include "StrumokKey.h"
#include "util.h"

using namespace System;


namespace StrumokLib {
    StrumokKey::StrumokKey(bool bit512)
    {
        array<uint64_t>^ key = GetRandomKey(bit512);
        array<uint64_t>^ iv = GetRandomIv();
        this->StrumokKey::StrumokKey(key, iv);
    }
    StrumokKey::StrumokKey(array<uint64_t>^ keyArr, array<uint64_t>^ iv)
    {
        if (keyArr == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(keyArr));

        if (keyArr->Length != 4 && keyArr->Length != 8)
            throw gcnew ArgumentOutOfRangeException(STRINGIFY(keyArr));

        if (keyArr == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(iv));

        if (keyArr->Length != 4)
            throw gcnew ArgumentOutOfRangeException(STRINGIFY(iv));

        m_keyArr = keyArr;
        m_iv = iv;
    }

    array<uint64_t>^ StrumokKey::Key::get() {
        if(m_keyArr == nullptr)
            return nullptr;
        return reinterpret_cast<array<uint64_t>^>(m_keyArr->Clone());
    }

    int32_t StrumokKey::KeyBitLength::get() {
        return m_keyArr->Length * 8 * 8;
    }

    int32_t StrumokKey::KeyByteLength::get() {
        return m_keyArr->Length * 8;
    }

    array<uint64_t>^ StrumokKey::IV::get() {
        if (m_iv == nullptr)
            return nullptr;
        return reinterpret_cast<array<uint64_t>^>(m_iv->Clone());
    }

    array<uint64_t>^ StrumokKey::GetRandomKey(bool bit512) {
        size_t arrLen = bit512 ? 8 : 4;
        array<uint64_t>^ key = gcnew array<uint64_t>(arrLen);
        util::rand_fill_array(key);
        return key;
    }
    array<uint64_t>^ StrumokKey::GetRandomIv()
    {
        array<uint64_t>^ iv = gcnew array<uint64_t>(4);
        util::rand_fill_array(iv);
        return iv;
    }
}