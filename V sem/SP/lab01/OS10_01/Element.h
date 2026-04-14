#pragma once

#include <Windows.h>
#include <iostream>

namespace ht
{
	struct Element   // элемент 
	{
		Element();
		Element(const void* key, int keyLength);                                             // for get
		Element(const void* key, int keyLength, const void* payload, int  payloadLength);    // for insert
		Element(const Element* oldElement, const void* newPayload, int  newPayloadLength);   // for update
		const void* key;                 // значение ключа 
		int         keyLength;           // рахмер ключа
		const void* payload;             // данные 
		int         payloadLength;       // размер данных
	};

	int getSizeElement(
		int   maxKeyLength,                // максимальный размер ключа
		int   maxPayloadLength);           // максимальный размер данных
}