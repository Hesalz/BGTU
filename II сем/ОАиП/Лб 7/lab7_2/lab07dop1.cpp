/*1. Дана величина a строкового типа из четного
количества символов. Получить и напечатать величину b,
состоящую из символов первой половины величины a, 
записанных в обратном порядке, после которых идут 
символы второй половины величины a, также записанные
в обратном порядке (например, при а = “привет” b 
должно быть равно «ипртев»).*/

#include <iostream>
#include "stack.h"
#include "string"
using namespace std;



void printStack(Stack* myStk)   
{
	Stack* e = myStk->head;   
	char a;
	if (e == NULL)
		cout << "Стек пуст!" << endl;
	while (e != NULL)
	{
		a = e->data;          
		cout << a;
		e = e->next;
	}
	cout << endl;
}

void main1()
{
	setlocale(LC_ALL, "Rus");
	string str;
	int dl;
	Stack* myStk = new Stack; //выделение памяти для стека
	myStk->head = NULL;       //инициализация первого элемента 

	cout << "Строка с четным количеством символов" << endl;
		
	while (str.length() % 2 != 0 || str.empty())
	{
		if (str.length() % 2 != 0)
		{
			cout << "Строка с четным количеством символов" << endl;
			str.clear();
		}
		
		getline(cin, str);
	}



	
	dl = str.length();


	// do stuff
	

	for (int i = (dl / 2); i < dl; i++) {
		push(str[i], myStk);
	}
	for (int i = 0; i < (dl / 2); i++) {
		push(str[i], myStk);
	}
	printStack(myStk);
	
}
