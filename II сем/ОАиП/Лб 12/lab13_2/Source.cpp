#include "Header.h"
#include <iostream>
#include <iomanip>
#include<fstream>
using namespace std;
void AAA::print()

{
	std::cout << x;
}
int AAA::getPriority() const
{
	return x;
}
namespace heap
{
	Heap create(int maxsize, CMP(*f)(void*, void*))
	{
		return *(new Heap(maxsize, f));
	}
	int Heap::left(int ix)
	{
		return (2 * ix + 1 >= size) ? -1 : (2 * ix + 1);
	}
	int Heap::right(int ix)
	{
		return (2 * ix + 2 >= size) ? -1 : (2 * ix + 2);
	}
	int Heap::parent(int ix)
	{
		return (ix + 1) / 2 - 1;
	}
	void Heap::swap(int i, int j)
	{
		void* buf = storage[i];
		storage[i] = storage[j];
		storage[j] = buf;
	}
	void Heap::heapify(int ix)
	{
		int l = left(ix), r = right(ix), irl = ix;
		if (l > 0)
		{
			if (isGreat(storage[l], storage[ix])) irl = l;
			if (r > 0 && isGreat(storage[r], storage[irl])) irl = r;
			if (irl != ix)
			{
				swap(ix, irl);
				heapify(irl);
			}
		}
	}
	void Heap::insert(void* x)
	{
		int i;
		if (!isFull())
		{
			storage[i = ++size - 1] = x;
			while (i > 0 && isLess(storage[parent(i)], storage[i]))
			{
				swap(parent(i), i);
				i = parent(i);
			}
		}
	}

	void Heap::scan(int i) const {
		int probel = 20;
		std::cout << '\n';
		if (size == 0)
			std::cout << " уча пуста";
		for (int u = 0, y = 0; u < size; u++)
		{
			std::cout << std::setw(probel + 10) << std::setfill(' ');
			((AAA*)storage[u])->print();
			if (u == y)
			{
				std::cout << '\n';
				if (y == 0)
					y = 2;
				else
					y += y * 2;
			}
			probel /= 2;
		}
		std::cout << '\n';
	}
	void Heap::extractMin()
	{
		int min = 100, a;
		int** A = (int**)storage;
		void* rc = nullptr;
		if (!isEmpty())
		{
			for (int i = 0; i < size; i++)
			{
				if (*A[i] < min)
				{
					min = *A[i];
					a = i;
				}
			}
			int i = parent(a);
			for (int i = a; i < size - 1; i++)
			{
				storage[i] = storage[i + 1];
			}
			size--;
			heapify(0);
		}

	}
	void Heap::extractI()
	{
		int i;
		cout << "¬ведите i:";
		cin >> i;
		cout << endl;
		if (!isEmpty())
		{
			for (int j = i; j < size - 1; j++)
			{
				storage[j] = storage[j + 1];
			}
			size--;
			heapify(0);
		}
	}
	void Heap::unionHeap(void* x)
	{
		int i;
		for (i = 0; i < size - 1; i++)
		{
			storage[i] = storage[i + 1];
		}
		heapify(0);

		storage[i = ++size - 1] = x;
		while (i > 0 && isLess(storage[parent(i)], storage[i]))
		{
			swap(parent(i), i);
			i = parent(i);
		}
	}


}