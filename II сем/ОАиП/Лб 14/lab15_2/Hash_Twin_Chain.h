#pragma once
#include "List.h"
namespace hashTC
{
	struct Object
	{
		int sizeO;
		int(*FunKey)(void*);
		listx::Object* Hash;
		Object(int size, int(*f)(void*))
		{
			sizeO = size;         
			FunKey = f;
			Hash = new listx::Object[size];
		};
		int hashFunction(void* data)
		{
			int h = 0, a = 314445, b = 37542;
			h = (a * h + FunKey(data)) % sizeO;
			return (h < 0) ? (h + sizeO) : h;

		};
		int hashFunction(int h, int size);
		bool insert(void* data);
		listx::Element* search(void* data);
		bool deleteByData(void* data);
		void Scan();
	};
	Object create(int size, int(*f)(void*));
}