#include <iostream>
#include <fstream>
#include <string>
#include "vector"

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
void zad14(STACK* stack, STACK* stack1, STACK* stack2);
void menu15();
void main1();
int main2();


void menu14()
{
	setlocale(LC_ALL, "rus");
	int choice, newEl, position;
	const char FOUND[] = "-0123456789";
	string ch;

	STACK* stack = new STACK;
	stack->head = NULL;

	STACK* stack1 = new STACK;
	stack1->head = NULL;
	STACK* stack2 = new STACK;
	stack2->head = NULL;


	while (true)
	{
		cout << "1 - добавить элемент в стек1\n";
		cout << "2 - извлечь элемент из стека1\n";
		cout << "3 - добавить элемент в стек2\n";
		cout << "4 - извлечь элемент из стека2\n";
		cout << "5 - вывод стека1 на экран\n";
		cout << "6 - вывод стека2 на экран\n";
		cout << "7 - вывод стека\n";
		cout << "8 - запись в файл\n";
		cout << "9 - вывод из файла\n";
		cout << "10 - очистка стека1\n";
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
			AddEl(newEl, stack1);
			cout << "\n";
			break;
		}

		case 2: {
			cout << TakeFromStack(stack1) << endl;
			break;
		}

		case 3: {
			cout << "\n";
			do
			{
				cout << "Введите число: "; // добавить число в список
				cin >> ch;
			} while (ch.find_first_not_of(FOUND) != string::npos);
			newEl = stoi(ch);

			AddEl(newEl, stack2);
			cout << "\n";
			break;
		}

		case 4: {
			cout << TakeFromStack(stack2) << endl;
			break;
		}

		case 5: {
			PrintStack(stack1);
			break;
		}
		case 6: {
			PrintStack(stack2);
			break;
		}

		case 7: {
			zad14(stack,stack1,stack2);
			break;
		}

		case 8: {
			ToFile(stack1);
			break;
		}

		case 9: {
			FromFile(stack1);
			break;
		}

		case 10: {
			clear(stack1);
			cout << "\n\nСтек очищен!\n\n";
			break;
		}

		case 0: {
			return;
		}
		}
	}
}


void zad14(STACK* stack, STACK* stack1, STACK* stack2) {


	STACK* sthead1 = stack1->head;
	STACK* sthead2 = stack2->head;

	int elData1, elData2;

	while (sthead1 != NULL)
	{
		elData1 = sthead1->data;
		while (sthead2 != NULL)
		{
			elData2 = sthead2->data;
			if (elData1 == elData2) AddEl(elData1, stack);
			sthead2 = sthead2->next;
		}

		sthead2 = stack2->head;
		sthead1 = sthead1->next;
	}
	PrintStack(stack);

	
	while (stack->head != NULL)
	{
		STACK* f = stack->head;
		stack->head = stack->head->next;
		delete f;
	}

		
	
}


void main() {
	/*char x;
	menu14();*/
	menu15();
	/*main1();
	cin >> x;
	main2();*/
}