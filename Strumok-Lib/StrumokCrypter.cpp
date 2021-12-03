#include "pch.h"
#include "StrumokCrypter.h"

#include "strumok-dstu8845/strumok.h"

using namespace System::Text;

namespace StrumokLib {
    StrumokCrypter::StrumokCrypter(array<uint64_t>^ key, array<uint64_t>^ iv)
    {
        if (key == nullptr)
            throw gcnew System::ArgumentNullException(STRINGIFY(key));
        
        m_pCtx = dstu8845_alloc();

        pin_ptr<uint64_t> pkey = &key[0];
        pin_ptr<uint64_t> pIv =  &iv[0];

        dstu8845_init(m_pCtx, pkey, sizeof(uint64_t) * key->Length, pIv);
    }

    StrumokCrypter::!StrumokCrypter()
    {
        if (m_pCtx) {
            dstu8845_free(m_pCtx);
            m_pCtx = nullptr;
        }
    }

    StrumokCrypter::~StrumokCrypter()
    {
        this->!StrumokCrypter();
    }

    array<uint8_t>^ StrumokCrypter::Crypt(array<uint8_t>^ in)
    {
        if (in == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(in));

        int64_t origSize = in->LongLength;
        /*int64_t alignedSize = Math::Ceiling((double)origSize / 4096) * 4096;
        Array::Resize(in, alignedSize);
        array<uint8_t>^ out = gcnew array<uint8_t>(alignedSize);*/
        array<uint8_t>^ out = gcnew array<uint8_t>(origSize);
        if (in->LongLength < 1)
            return out;

        pin_ptr<uint8_t> pin_in = &in[0];
        pin_ptr<uint8_t> pin_out = &out[0];

        dstu8845_crypt(m_pCtx, pin_in, in->LongLength, pin_out);
        /*Array::Resize(out, origSize);*/
        return out;
    }

    String^ StrumokCrypter::Crypt(String^ in)
    {
        if (in == nullptr)
            throw gcnew ArgumentNullException("in");

        array<uint8_t>^ inBuffer = Encoding::Unicode->GetBytes(in);
        array<uint8_t>^ outBuffer = Crypt(inBuffer);
        return Encoding::Unicode->GetString(outBuffer);
    }
}