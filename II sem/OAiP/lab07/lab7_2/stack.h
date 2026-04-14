#pragma once
struct Stack
{
	char data;             //информационный элемент
	Stack* head;   //вершина стека 
	Stack* next;   //указатель на следующий элемент
};
void show(Stack* myStk);         //прототип
int pop(Stack* myStk);           //прототип
void push(char x, Stack* myStk);  //прототип
