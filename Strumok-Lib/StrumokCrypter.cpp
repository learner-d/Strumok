#include "pch.h"
#include "StrumokCrypter.h"

#include "strumok-dstu8845/strumok.h"

using namespace System::Text;
using namespace System;

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
        if (m_disposed)
            return;

        if (m_pCtx) {
            dstu8845_free(m_pCtx);
            m_pCtx = nullptr;
        }
        m_disposed = true;
    }

    StrumokCrypter::~StrumokCrypter()
    {
        this->!StrumokCrypter();
    }

    array<uint8_t>^ StrumokCrypter::Crypt(array<uint8_t>^ in, int64_t count) {
        CancellationToken cancellationToken;
        return CryptAsync(in, count, cancellationToken)->Result;
    }

    void StrumokCrypter::Crypt(array<uint8_t>^ in, array<uint8_t>^ out, int64_t count)
    {
        CancellationToken cancellationToken;
        CryptAsync(in, out, count, cancellationToken)->Wait();
    }

    Task<array<uint8_t>^>^ StrumokCrypter::CryptAsync(array<uint8_t>^ in, int64_t count,
        CancellationToken cancellationToken) {
        if (m_disposed)
            throw gcnew ObjectDisposedException(STRINGIFY(StrumokCrypter));
        if (in == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(in));

        int64_t bufferSize = count > -1 ? count : in->LongLength;
        array<uint8_t>^ out = gcnew array<uint8_t>(bufferSize);
        if (in->LongLength < 1) {
            return Task::FromResult(out);
        }

        // Ініціалізація асинхронного шифрування
        TaskCompletionSource<array<uint8_t>^>^ taskCompletionSource = gcnew TaskCompletionSource<array<uint8_t>^>();
        CryptOnCompleteHandler^ onCompleteHandler = gcnew CryptOnCompleteHandler(out, taskCompletionSource);
        CryptAsync(in, out, bufferSize, cancellationToken)->ContinueWith(gcnew Action<Task^>(onCompleteHandler, &CryptOnCompleteHandler::OnComplete));
        return taskCompletionSource->Task;
    }

    StrumokCrypter::CryptOnCompleteHandler::CryptOnCompleteHandler(array<uint8_t>^ result, TaskCompletionSource<array<uint8_t>^>^ taskCompletionSource)
    {
        _result = result;
        _taskCompletionSource = taskCompletionSource;
    }

    void StrumokCrypter::CryptOnCompleteHandler::OnComplete(Task^ task)
    {
        if (task->IsCanceled)
            _taskCompletionSource->TrySetCanceled();
        else if (task->IsFaulted) {
            Exception^ ex = task->Exception;
            if (ex != nullptr)
                ex = ex->GetBaseException();
            _taskCompletionSource->TrySetException(ex);
        }
        else
            _taskCompletionSource->TrySetResult(_result);
    }

    Task^ StrumokCrypter::CryptAsync(array<uint8_t>^ in, array<uint8_t>^ out, int64_t count, CancellationToken cancellationToken)
    {
        if (m_disposed)
            throw gcnew ObjectDisposedException(STRINGIFY(StrumokCrypter));
        if (in == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(in));
        if (out == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(out));

        if(count < 0)
            count = in->LongLength;

        // Ініціалізація асинхронного шифрування
        TaskCompletionSource<array<uint8_t>^>^ taskCompletionSource = gcnew TaskCompletionSource<array<uint8_t>^>();
        StrumokEncryptionWorker^ encryptionWorker = gcnew StrumokEncryptionWorker(m_pCtx, in, out, count);
        // Можливість скасування
        cancellationToken.Register(gcnew Action(encryptionWorker, &StrumokEncryptionWorker::Cancel));
        // Запуск асинхронного завдання
        return Task::Run(gcnew Action(encryptionWorker, &StrumokEncryptionWorker::Crypt), cancellationToken);
    }

    StrumokCrypter::StrumokEncryptionWorker::StrumokEncryptionWorker(Dstu8845Ctx* strumokContext,
        array<uint8_t>^ inputBuffer, array<uint8_t>^ outputBuffer, int64_t encryptionByteCount)
    {
        _strumokContext = strumokContext;
        _inputBuffer = inputBuffer;
        _outputBuffer = outputBuffer;
        _encryptionByteCount = encryptionByteCount > -1 ? encryptionByteCount : _inputBuffer->LongLength;
    }

    void StrumokCrypter::StrumokEncryptionWorker::Crypt() {
        pin_ptr<uint8_t> pin_inputArray = &_inputBuffer[0];
        pin_ptr<uint8_t> pin_outputArray = &_outputBuffer[0];

        dstu8845_crypt(_strumokContext, pin_inputArray, _encryptionByteCount, pin_outputArray);
    }

    void StrumokCrypter::StrumokEncryptionWorker::Cancel() {
        dstu8845_abort(_strumokContext);
    }

    String^ StrumokCrypter::Crypt(String^ in)
    {
        if (m_disposed)
            throw gcnew ObjectDisposedException(STRINGIFY(StrumokCrypter));
        if (in == nullptr)
            throw gcnew ArgumentNullException(STRINGIFY(in));

        array<uint8_t>^ inBuffer = Encoding::Unicode->GetBytes(in);
        array<uint8_t>^ outBuffer = Crypt(inBuffer, inBuffer->Length);
        return Encoding::Unicode->GetString(outBuffer);
    }


    bool StrumokCrypter::Disposed::get() {
        return m_disposed;
    }
}