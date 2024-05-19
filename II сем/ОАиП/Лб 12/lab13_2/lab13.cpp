/* В проект добавить следующие функции: 
удаление минимального extractMin; удаление
i-ого элемента extractI; объедине-ние 
unionHeap двух куч в одну. */


#include "Header.h"
#include<tchar.h>
#include <iostream>
using namespace std;
heap::CMP cmpAAA(void* a1, void* a2) //Функция сравнения
{
#define A1 ((AAA*)a1)
#define A2 ((AAA*)a2)
	heap::CMP
		rc = heap::EQUAL;
	if (A1->x > A2->x)
		rc = heap::GREAT;
	else
		if (A2->x > A1->x)
			rc = heap::LESS;
	return rc;
#undef A2
#undef A1
}
//-------------------------------
int _tmain(int argc, _TCHAR* argv[])
{
	setlocale(LC_ALL, "rus");
	int k, choice;
	heap::Heap h1 = heap::create(30, cmpAAA);
	heap::Heap h2 = heap::create(30, cmpAAA);
	heap::Heap h3 = heap::create(30, cmpAAA);
	for (;;)
	{
		cout << "1 - вывод 1 кучи на экран" << endl;
		cout << "2 - добавить элемент в 1 кучу" << endl;
		cout << "3 - вывод 2 кучи на экран" << endl;
		cout << "4 - добавить элемент в 2 кучу" << endl;
		cout << "5 - объединение 2 куч" << endl;
		cout << "6 - удалить минимальный элемент 1 кучи" << endl;
		cout << "7 - удалить i-й элемент 1 кучи" << endl;
		cout << "0 - выход" << endl;
		cout << "сделайте выбор" << endl; cin >> choice;
		switch (choice)
		{
		case 0: exit(0);
		case 1: h1.scan(0);
			break;
		case 2:
		{AAA* a = new AAA;
		cout << "введите ключ" << endl; cin >> k;
		a->x = k;
		h1.insert(a);
		}
		break;
		case 3: h2.scan(0);
			break;
		case 4:
		{AAA* a = new AAA;
		cout << "введите ключ" << endl; cin >> k;
		a->x = k;
		h2.insert(a);
		h1.insert(a);
		}
		break;
		case 5:
			h1.scan(0);
			break;
		case 6: h1.extractMin();
			break;
		case 7: h1.extractI();
			break;
		default: cout << endl << "Введена неверная команда!" << endl;
		}
	} return 0;
}
