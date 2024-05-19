#include "List.h"
#include "Hash_Twin_Chain.h"
#include <iostream>

struct AAA
{
	int key;
	const char* mas;
	AAA(int k, char* z)
	{
		key = k;
		mas = z;
	}
};

namespace hashTC
{
	Object create(int size, int(*f)(void*))
	{
		return *(new Object(size, f));
	}
	int Object::hashFunction(int h, int size)
	{
		h ^= (h >> 4) ^ (h >> 6);
		return (h ^ (h >> 3) ^ (h >> 2) % size);

	};
	bool Object::insert(void* data)
	{
		return (Hash[hashFunction(data)].insert(data));
	};
	bool Object::deleteByData(void* data)
	{
		return (Hash[hashFunction(data)].deleteByData(data));
	};
	listx::Element* Object::search(void* data)
	{
		return Hash[hashFunction(data)].search(data);
	};
	void Object::Scan()
	{
		for (int i = 0; i < sizeO; i++)
		{
			std::cout << i << ". ";
			Hash[i].scan();
			std::cout << '\n';
		}
	};
}