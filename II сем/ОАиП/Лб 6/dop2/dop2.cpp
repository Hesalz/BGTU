#include <iostream>
#include <string>
#include<fstream>
#include <vector>

using namespace std; // Вариант 1

struct list
{
	int number;
	list* next;
};

void insert(list*&, int);


int genRand(int from, int to) {
	return from + rand() % (to - from + 1);
}

void createMany(list*& p, int n) {
	for (int i = 0; i < n; i++)
	{
		insert(p, genRand(-20, 20));
	}
}


void insert(list*& p, int value)
{
	list* newP = new list;
	if (newP != NULL)
	{
		newP->number = value;
		newP->next = p;
		p = newP;
	}
	else
		cout << "Операция добавления не выполнена" << endl;
}

int del(list*& p, int value)
{
	list* previous, * current, * temp;
	if (value == p->number)
	{
		temp = p;
		p = p->next;
		delete temp;
		return value;
	}
	else
	{
		previous = p;
		current = p->next;
		while (current != NULL && current->number != value)
		{
			previous = current;
			current = current->next;
		}
		if (current != NULL)
		{
			temp = current;
			previous->next = current->next;
			free(temp);
			return value;
		}
	}
	return 0;
}

int IsEmpty(list* p)
{
	return p == NULL;
}



void printList(list* p)
{
	if (p == NULL)
		cout << "Список пуст" << endl;
	else
	{
		cout << "Список:" << endl;
		while (p != NULL)
		{
			cout << " --> " << p->number;
			p = p->next;
		}
		cout << " --> NULL" << endl;
	}
}

void toFile(list*& p)
{
	list* temp = p;
	ofstream file1("mList.txt");
	if (file1.fail())
	{
		cout << "\n Ошибка открытия файла";
		exit(1);
	}
	while (temp)
	{
		int nn = temp->number;
		file1 << nn << "\n";
		temp = temp->next;
	}
	file1.close();
	cout << "Список записан в файл\n";
}

void fromFile(list*& p){

	char buf[10];
	vector<int> container;
	ifstream file1("mList.txt");
	if (file1.fail())
	{
		cout << "\n Ошибка открытия файла";
		exit(1);
	}
	while (!file1.eof())
	{
		file1.getline(buf, 10);
		if (strlen(buf))
			container.push_back(atoi(buf));
		cout << " --> " << buf;
	}
	cout << " --> NULL" << endl;

	for (int i = container.size() - 1; i >= 0; i--)
	{
		insert(p, container[i]);
	}

	file1.close();
}
void find(list* p)
{
	bool isFnd = false;
	int idx = 1;
	int element;
	cout << "Введите элемент: ";
	cin >> element;
	list* fnd = p;
	while (fnd)
	{
		if (fnd->number == element)
		{
			isFnd = true;
			printf("Индекс элемента %d\n", idx);
			break;
		}
		idx++;
		fnd = fnd->next;
	}
	if (!isFnd)
		printf("Элемент не найден\n");

}

void menu3()
{
	cout << "Сделайте выбор:" << endl;
	cout << " 1 - Ввод числа" << endl;
	cout << " 2 - Вывод числа" << endl;
	cout << " 3 - Удаление числа" << endl;
	cout << " 4 - Вычисление суммы положительных чисел" << endl;
	cout << " 5 - Поиск элемента" << endl;
	cout << " 6 - Запись в файл" << endl;
	cout << " 7 - Вывод из файла" << endl;
	cout << " 8 - Выход" << endl;
}



void sumP(list* p)
{
	int sm = 0;
	if (p == NULL)
		cout << "Список пуст" << endl;
	else
	{
		while (p != NULL)
		{
			if (p->number >= 0)

				sm = sm + (p->number);
			p = p->next;


		}
		if (sm == 0) {
			cout << "Таких элементов нет" << sm << endl;
		}
		else
		{
			cout << "Сумма = " << sm << endl;
		}


	}

}

int main()
{
	setlocale(LC_CTYPE, "Russian");
	list* first = NULL;
	int choice;
	int value;
	menu3();
	cout << " Введите ваш выбор: ";
	cin >> choice;
	while (choice != 8)
	{
		switch (choice)
		{
		case 1: cout << "Введите число элементов ";
			cin >> value;
			createMany(first, value);
			break;
		case 2:
			printList(first);
			break;
		case 3: if (!IsEmpty(first))
		{
			cout << "Введите удаляемое число ";
			cin >> value;
			if (del(first, value))
			{
				cout << "Число " << value << "удалено" << endl;
				printList(first);
			}
			else
				cout << "Число не найдено" << endl;
		}
			  else
			cout << "Список пуст" << endl;
			break;
		case 4: sumP(first);
			break;
		case 5: find(first);
			break;
		case 6: toFile(first);
			break;
		case 7: fromFile(first);
			break;
		default: cout << "Неправильный выбор" << endl;
			menu3();
			break;
		}
		cout << "Введите ваш выбор: ";
		cin >> choice;
	}
	return 0;
}

