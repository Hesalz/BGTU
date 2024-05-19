#include "Header.h"
#include <iostream>
#include <cstdlib>
#include <ctime>


int mode = 0;

int HashFunction(int key, int size) {
	if (mode == 1)
	{
		int h = 0, a = 314445, b = 37542;
		h = (a * h + key) % size;

		return (h < 0) ? (h + size) : h;
	}


	return key % size;

}

int Next_hash(int hash, int size, int p)
{
	return (hash + 5 * p + 3 * p * p) % size;
}

void setMode(int modeId) {
	mode = modeId;
}

Object create(int size, int(*getkey)(void*))
{
	return *(new Object(size, getkey));
}

Object::Object(int size, int(*getkey)(void*))
{
	N = 0;
	this->size = size;
	this->getKey = getkey;
	this->data = new void* [size];
	for (int i = 0; i < size; ++i)
		data[i] = NULL;
}

bool Object::insert(void* d)
{
	bool b = false;
	if (N != size)
		for (int i = 0, t = getKey(d), j = HashFunction(t, size);
			i != size && !b;  j++) {
		if (data[j] == NULL || data[j] == DEL)
		{
			data[j] = d;
			N++;
			b = true;
			std::cout << "hash = " << j << '\n';
		}
		if (j == size-1)
		{
			j = -1;
		}
	}
			
			
	return b;
}

int Object::searchInd(int key)
{
	int t = -1;
	bool b = false;
	if (N != 0)
		for (int i = 0, j = HashFunction(key, size); data[j] != NULL && i != size && !b; j++) {
			if (data[j] != DEL)
				if (getKey(data[j]) == key)
				{
					t = j; b = true;
					std::cout << "hash = " << j << '\n';
				}

			if (j == size - 1)
			{
				j = -1;
			}
		}
			
	return t;
}

void* Object::search(int key)
{
	int t = searchInd(key);
	return(t >= 0) ? (data[t]) : (NULL);
}

void* Object::searchUnivers(int key)
{
	srand(time(NULL));
	int a = rand();
	int b = rand();
	int t = searchInd(key);
	return(t >= 0) ? (data[t]) : (NULL);
}


void* Object::deleteByKey(int key)
{
	int i = searchInd(key);
	void* t = data[i];
	if (t != NULL)
	{
		data[i] = DEL;
		N--;
	}
	return t;
}

bool Object::deleteByValue(void* d)
{
	return(deleteByKey(getKey(d)) != NULL);
}

void Object::scan(void(*f)(void*))
{
	for (int i = 0; i < this->size; i++)
	{
		std::cout << " Ёлемент" << i;
		if ((this->data)[i] == NULL)
			std::cout << "  пусто" << std::endl;
		else
			if ((this->data)[i] == DEL)
				std::cout << "  удален" << std::endl;
			else
				f((this->data)[i]);
	}
}


