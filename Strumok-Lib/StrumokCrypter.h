#pragma once
#include "StrumokKey.h"
using namespace System;

namespace StrumokLib {
	public ref class StrumokCrypter
	{
	public:
		StrumokCrypter(array<uint64_t>^ key, array<uint64_t>^ iv);
		!StrumokCrypter();
		~StrumokCrypter();

		// Шифрує послідовність байт
		array<uint8_t>^ Crypt(array<uint8_t>^ in);
		// Шифрує текстовий рядок
		String^ Crypt(String^ in);
	private:
		Dstu8845Ctx* m_pCtx;
	};
}