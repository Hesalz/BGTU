#include <iostream>
#include <fstream>
#include <string>
#include <Windows.h>
using namespace std;
const unsigned int NAME_SIZE = 30;
const unsigned int CITY_SIZE = 20;

struct Address {
	char name[NAME_SIZE];
	string city;
	Address* next;
	Address* prev;
};

void insert(Address* e, Address** phead, Address** plast) {
	Address* p = *plast;
	if (*plast == NULL)
	{
		e->next = NULL;
		e->prev = NULL;
		*plast = e;
		*phead = e;
		return;
	}
	else
	{
		p->next = e;
		e->next = NULL;
		e->prev = p;
		*plast = e;
	}
}

Address* setElement() {
	Address* temp = new Address();
	if (!temp)
	{
		cerr << "Ошибка выделения памяти памяти";
		return NULL;
	}
	cout << "Введите имя: ";
	cin >> temp->name;
	cout << "Введите город: ";
	cin.ignore();
	getline(cin,temp->city);
	temp->next = NULL;
	temp->prev = NULL;
	return temp;
}

void outputList(Address** phead, Address** plast) {
	Address* t = *phead;
	while (t)
	{
		cout << t->name << ' ' << t->city << endl;
		t = t->next;
	}
	cout << "" << endl;
}

void find(char name[NAME_SIZE], Address** phead) {
	Address* t = *phead;
	while (t)
	{
		if (!strcmp(name, t->name)) break;
		t = t->next;
	}
	if (!t)
		cerr << "Имя не найдено" << endl;
	else
		cout << t->name << ' ' << t->city << endl;
}

void delet(char name[NAME_SIZE], Address** phead, Address** plast) {
	struct Address* t = *phead;
	while (t)
	{
		if (!strcmp(name, t->name)) break;
		t = t->next;
	}
	if (!t)
		cerr << "Имя не найдено" << endl;
	else
	{
		if (*phead == t)
		{
			*phead = t->next;
			if (*phead)
				(*phead)->prev = NULL;
			else
				*plast = NULL;
		}
		else
		{
			t->prev->next = t->next;
			if (t != *plast)
				t->next->prev = t->prev;
			else
				*plast = t->prev;
		}
		delete t;
		cout << "Элемент удален" << endl;
	}
}

void writeToFile(Address** phead) {
	struct Address* t = *phead;
	ofstream file1("Lip.txt");
	if (file1.fail())
	{
		cerr << "Файл не открывается" << endl;
		exit(1);
	}
	cout << "Сохранение в файл" << endl;
	while (t)
	{
		file1 << t->name << " " << t->city << endl;
		t = t->next;
	}
	file1.close();
}

void fromFile(Address** phead)
{

	char buf[256];
	ifstream file2("Lip.txt");
	if (file2.fail())
	{
		cout << "\n Ошибка открытия файла";
		exit(1);
	}
	while (!file2.eof())
	{
		file2.getline(buf, 256);
		if (strlen(buf))
			cout  << buf;
		cout << endl;
	}

	file2.close();
}
bool returnN(Address** phead, Address** plast)
{
	return *phead == NULL && *plast == NULL;
}

void Menu() {
	cout << endl;
	cout << "1. Ввод адреса" << endl;
	cout << "2. Удаление имени" << endl;
	cout << "3. Вывод на экран" << endl;
	cout << "4. Поиск" << endl;
	cout << "5. В файл" << endl;
	cout << "6. Из файла" << endl;
	cout << "7. Проверить пуст ли список" << endl;
	cout << "8. Выход" << endl;
	cout << endl;
}
int main()
{
	Address* head = NULL;
	Address* last = NULL;
	setlocale(LC_ALL, "Rus");
	SetConsoleCP(1251);
	SetConsoleOutputCP(1251);
	int choice;
	char s[80]; int c;
	Menu();
		cout << "Ваш выбор: ";
		cin >> choice;
	while (choice != 8) {
		switch (choice) {
		case 1:
			insert(setElement(), &head, &last);
			break;
		case 2: 
			char dname[NAME_SIZE];
			cout << "Введите имя: ";
			cin >> dname;
			delet(dname, &head, &last);
			  break;
		case 3: outputList(&head, &last);
			break;
		case 4: 
			char fname[NAME_SIZE];
			cout << "Введите имя: ";
			cin >> fname;
			find(fname, &head);
			  break;
		case 5:
			writeToFile(&head);
			  break;
		case 6:
			fromFile(&head);
			  break;
		case 7:
			returnN(&head, &last) ? cout << "Список пуст" : cout << "Список не пуст";
			cout << endl;
			  break;
		case 8: exit(0);
		default: cout << "Неверный ввод.";
		}
			Menu();
			cout << "Ваш выбор: ";
			cin >> choice;
	}
	return 0;
}