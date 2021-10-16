#include "util.h"
using namespace System;

void util::rand_fill_array(array<uint64_t>^ in)
{
	if (in == nullptr)
		throw gcnew ArgumentNullException(STRINGIFY(in));

	Random^ r = gcnew Random();
	for (size_t i = 0; i < in->Length; i++)
	{
		in[i] = r->Next() << 32 | r->Next();
	}
}
