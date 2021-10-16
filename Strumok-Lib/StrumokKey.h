#pragma once
#include <stdint.h>
#include "strumok-dstu8845/strumok.h"

namespace StrumokLib {
    public ref class StrumokKey
    {
    public:
        StrumokKey() : StrumokKey(false) {};
        StrumokKey(bool bit512);
        StrumokKey(array<uint64_t>^ keyArr, array<uint64_t>^ iv);
        property array<uint64_t>^ Key {
            array<uint64_t>^ get();
        }
        property int32_t KeyBitLength {
            int32_t get();
        }
        property int32_t KeyByteLength {
            int32_t get();
        }
        property array<uint64_t>^ IV {
            array<uint64_t>^ get();
        }
    
    
    private:
        array<uint64_t>^ m_keyArr;
        array<uint64_t>^ m_iv;

        array<uint64_t>^ GetRandomKey(bool bit512);
        array<uint64_t>^ GetRandomIv();
    };
}