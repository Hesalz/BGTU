#include "tests.h"

namespace tests
{
	BOOL test1(ht::HtHandle* htHandle)
	{
		ht::Element* insertEl = new ht::Element("test1", 6, "test1", 6);

		ht::insert(htHandle, insertEl);
		ht::Element* getEl = ht::get(htHandle, new ht::Element("test1", 6));

		if (
			getEl == NULL ||
			insertEl->keyLength != getEl->keyLength ||
			memcmp(insertEl->key, getEl->key, insertEl->keyLength) != NULL ||
			insertEl->payloadLength != getEl->payloadLength ||
			memcmp(insertEl->payload, getEl->payload, insertEl->payloadLength) != NULL
			)
			return false;

		return true;
	}

	BOOL test2(ht::HtHandle* htHandle)
	{
		ht::Element* element = new ht::Element("test2", 6, "test2", 6);

		ht::insert(htHandle, element);
		ht::removeOne(htHandle, element);
		if (ht::get(htHandle, element) != NULL)
			return false;

		return true;
	}

	BOOL test3(ht::HtHandle* htHandle)
	{
		ht::Element* element = new ht::Element("test3", 6, "test3", 6);

		ht::insert(htHandle, element);
		if (ht::insert(htHandle, element))
			return false;

		return true;
	}

	BOOL test4(ht::HtHandle* htHandle)
	{
		ht::Element* element = new ht::Element("test3", 6, "test3", 6);

		ht::insert(htHandle, element);
		ht::removeOne(htHandle, element);
		if (ht::removeOne(htHandle, element))
			return false;

		return true;
	}
}