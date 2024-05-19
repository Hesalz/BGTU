/*15	Разработать функцию, которая удаляет первый
повторяющийся элемент стека.*/
#include <iostream>
#include <fstream>
#include <string>
#include "vector"

const char FOUND[] = "-0123456789";

using std::cout;
using std::endl;
using std::cin;
using std::ifstream;
using std::ofstream;
using std::getline;
using std::string;
using std::vector;

struct STACK {
	int data;
	STACK* head;
	STACK* next;
};

void AddEl(int elem, STACK* stack);
int TakeFromStack(STACK* stack);
void PrintStack(STACK* stack);
void ToFile(STACK* stack);
void FromFile(STACK* stack);
int DeleteSame(STACK* stack);
void clear(STACK* stack);

void DeleteSameHelp(STACK* stack, int point);


void main1();

void menu15()
{
	setlocale(LC_ALL, "rus");
	int choice, newEl, position;
	string ch;
	STACK* stack = new STACK;
	stack->head = NULL;


	while (true)
	{
		cout << "1 - добавить элемент в стек\n";
		cout << "2 - извлечь элемент из стека\n";
		cout << "3 - вывод стека на экран\n";
		cout << "4 - удалить из стека первый повторяющийся элемент\n";
		cout << "5 - запись в файл\n";
		cout << "6 - вывод из файла\n";
		cout << "7 - очистка стека\n";
		cout << "0 - выход\n";
		cin >> choice;

		switch (choice)
		{
		case 1: {
			cout << "\n";
			do
			{
				cout << "Введите число: "; // добавить число в список
				cin >> ch;
			} while (ch.find_first_not_of(FOUND) != string::npos);
			newEl = stoi(ch);
			AddEl(newEl, stack);
			cout << "\n";
			break;
		}

		case 2: {
			cout << TakeFromStack(stack) << endl;
			break;
		}

		case 3: {
			PrintStack(stack);
			break;
		}

		case 4: {
			DeleteSame(stack);
			break;
		}

		case 5: {
			ToFile(stack);
			break;
		}

		case 6: {
			clear(stack);
			FromFile(stack);
			break;
		}

		case 7: {
			clear(stack);
			cout << "\n\nСтек очищен!\n\n";
			break;
		}

		case 0: {
			return;
		}
		}
	}
}

void AddEl(int elem, STACK* stack)
{
	STACK* NewEl = new STACK;
	NewEl->data = elem;
	NewEl->next = stack->head;
	stack->head = NewEl;
}




int TakeFromStack(STACK* stack)
{
	if (stack->head == NULL)
	{
		cout << "Стек пуст";
		return -1;
	}
	else {
		STACK* intermEl = new STACK;
		int elData = stack->head->data;
		stack->head = stack->head->next;
		delete intermEl;
		return elData;
	}



}

void PrintStack(STACK* stack)
{
	STACK* sthead = stack->head;
	int elData;
	if (sthead == NULL)
		cout << "Стек пуст";
	while (sthead != NULL)
	{
		elData = sthead->data;
		cout << elData << " ";
		sthead = sthead->next;
	}
	cout << "\n";
}

void ToFile(STACK* stack)
{
	ofstream getf;
	getf.open("File13.txt");
	if (getf.is_open())
	{
		STACK* StackHead = stack->head;
		while (StackHead != NULL)
		{
			getf << StackHead->data << "\n";
			StackHead = StackHead->next;
		}; cout << "\n";
	}
	getf.close();
	cout << "Информация записана в файл!\n\n";
}

void FromFile(STACK* stack)
{
	char buf[10];
	vector<int> container;
	ifstream file1("File13.txt");
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
	}

	for (int i = container.size() - 1; i >= 0; i--)
	{
		AddEl(container[i],stack);
	}

	file1.close();
}

int DeleteSame(STACK* stack)
{
	STACK* sthead = stack->head;
	int elData, i = 0, point = -1;
	vector<int> arr;
	vector<int> arr2;
	if (sthead == NULL)
		cout << "Стек пуст";
	while (sthead != NULL)
	{
		elData = sthead->data;
		arr.push_back(elData);
		sthead = sthead->next;
	}

	bool first = false;

	for (int i = 0; i < arr.size(); i++)
	{
		for (int j = 0; j  < arr.size(); j++)
		{
			if (j != i && !first && arr[i] == arr[j]) {
				
				point = i;
				first = true;
			}
		}
	}

	arr.erase(arr.begin() + point);

	clear(stack);

	for (int i = arr.size() - 1; i >= 0; i--)
	{
		AddEl(arr[i], stack);
	}

	first = false;

	return 0;
}

void DeleteSameHelp(STACK* stack, int point)
{
	STACK* sthead1 = stack->head;
	for (int k = 0; k < point - 1; k++)
		sthead1 = sthead1->next;

	sthead1->next = sthead1->next->next;
}

void clear(STACK* stack)
{
	if (stack->head == NULL)
		cout << "Стек пуст";
	while (stack->head != NULL)
	{
		STACK* f = stack->head;
		stack->head = stack->head->next;
		delete f;
	}
	cout << "\n";
}
