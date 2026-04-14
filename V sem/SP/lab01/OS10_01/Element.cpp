#include "Element.h"

namespace ht
{
	Element::Element()
	{
		key = nullptr;
		keyLength = 0;
		payload = nullptr;
		payloadLength = 0;
	}

	Element::Element(const void* key, int keyLength) : Element()
	{
		this->key = key;
		this->keyLength = keyLength;
	}

	Element::Element(const void* key, int keyLength, const void* payload, int  payloadLength) : Element()
	{
		this->key = key;
		this->keyLength = keyLength;
		this->payload = payload;
		this->payloadLength = payloadLength;
	}

	Element::Element(const Element* oldElement, const void* newPayload, int  newPayloadLength) : Element()
	{
		this->key = oldElement->key;
		this->keyLength = oldElement->keyLength;
		this->payload = newPayload;
		this->payloadLength = newPayloadLength;

		delete oldElement;
	}

	int getSizeElement(int   maxKeyLength, int   maxPayloadLength)
	{
		return maxKeyLength + maxPayloadLength + sizeof(int) * 2;
	}
}