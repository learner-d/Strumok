#pragma once
#include "StrumokKey.h"
using namespace System;
using namespace System::Threading;
using namespace System::Threading::Tasks;

namespace StrumokLib {
	public ref class StrumokCrypter
	{
	public:
		StrumokCrypter(array<uint64_t>^ key, array<uint64_t>^ iv);
		!StrumokCrypter();
		~StrumokCrypter();

		// Шифрує послідовність байт
		array<uint8_t>^ Crypt(array<uint8_t>^ in, int64_t count);
		void Crypt(array<uint8_t>^ in, array<uint8_t>^ out, int64_t count);
		// Шифрує текстовий рядок
		String^ Crypt(String^ in);
		Task<array<uint8_t>^>^ CryptAsync(array<uint8_t>^ in, int64_t count, CancellationToken cancellationToken);
		Task^ CryptAsync(array<uint8_t>^ in, array<uint8_t>^ out, int64_t count, CancellationToken cancellationToken);

		property bool Disposed { bool get(); };
	private:
		Dstu8845Ctx* m_pCtx;
		volatile bool m_disposed = false;

		ref class StrumokEncryptionWorker
		{
		public:
			StrumokEncryptionWorker(Dstu8845Ctx* strumokContext, array<uint8_t>^ inputArray, array<uint8_t>^ outputArray, int64_t encryptionByteCount);
			void Crypt();
			void Cancel();
		private:
			Dstu8845Ctx* _strumokContext;
			array<uint8_t>^ _inputBuffer;
			array<uint8_t>^ _outputBuffer;
			int64_t _encryptionByteCount;
		};

		ref class CryptOnCompleteHandler {
		public:
			CryptOnCompleteHandler(array<uint8_t>^ result, TaskCompletionSource<array<uint8_t>^>^ taskCompletionSource);
			void OnComplete(Task^ task);
		private:
			array<uint8_t>^ _result;
			TaskCompletionSource<array<uint8_t>^>^ _taskCompletionSource;
		};
	};
}