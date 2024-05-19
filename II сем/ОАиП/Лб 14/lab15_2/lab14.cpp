#include "Hash_Twin_Chain.h"
#include <iostream>
#include <windows.h>
#include <tchar.h>
#include <time.h>
#include <algorithm>
#include <vector>

using namespace std;

struct AAA
{
	int key;
	const char* mas;
	const char* name;
	AAA(int k, const char* z,const char* s)
	{
		key = k;
		mas = z;
		name = s;
	}
	AAA()
	{
		key = 0;
		mas = NULL;
		name = NULL;
	}
};

int hf(void* d) {
	AAA* f = (AAA*)d;
	return f->key;
}

void AAA_print(listx::Element* e)
{
	std::cout << ((AAA*)e->data)->key << "; ФИО "  << ((AAA*)e->data)->mas << "; Название университета " << ((AAA*)e->data)->name;
}

int _tmain(int argc, _TCHAR* argv[]) {
	SetConsoleOutputCP(1251);
	SetConsoleCP(1251);
	srand(time(0));
	int current_size = 0;                                                                                                              
	cout << "Введите размер хеш-таблицы: ";                        
	cin >> current_size;                                                            
	hashTC::Object H = hashTC::create(current_size, hf);
	vector<int> keys;
	clock_t start, end;
	double searching_time;
	int choice;
	int k;
	for (;;)									
	{
		cout << "1 - Показать хеш-таблицу" << endl;
		cout << "2 - Добавить новый элемент" << endl;
		cout << "3 - Удалить элемент" << endl;
		cout << "4 - Поиск элемента" << endl;
		cout << "0 - Выход" << endl;
		cout << "Ваш выбор?" << endl;
		cin >> choice;
		switch (choice)
		{
		case 0:  exit(0);
		case 2: {	  AAA* a = new AAA;                                         
			char* str = new char[100];
			char* str2 = new char[100];
			bool is_correct = false;
			do {
				cout << "Введите год создания: ";
				cin >> k;
				k = k;
				a->key = k;       
				if (std::count(keys.begin(), keys.end(), k) == 0 and k > -1) {
					keys.push_back(k);
					is_correct = true;
				}
				else cout << "Неверный ввод" << endl;
			} while (!is_correct);
			cout << "Введите ФИО: ";
			cin.ignore();
			cin.getline(str, 100);
			a->mas = str;       
			cout << "Введите название университета: ";
			
			cin.getline(str2, 100);
			a->name = str2;
			H.insert(a);
		}
			  break;
		case 1: H.Scan();
			break;
		case 3: {	  AAA* b = new AAA;
			cout << "Введите год создания университета для удаления: ";
			cin >> k;
			b->key = k;
			keys.erase(std::find(keys.begin(), keys.end(), k));
			H.deleteByData(b);
		}
			  break;
		case 4: {AAA* c = new AAA;
			cout << "Введите год создания университета для поиска: ";
			cin >> k;
			c->key = k;
			start = clock();
			if (H.search(c) == NULL)
				cout << "Элемент не найден." << endl;
			else
			{
				cout << "Найденный элемент: ";
				AAA_print(H.search(c));
				cout << endl;
			}
			end = clock();
			searching_time = (double)(end - start);
			cout << "Время поиска: " << searching_time << "ms" << endl;
		}
			  break;
		}
	}
	return 0;
}