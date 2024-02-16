#include <iostream>
#include <Windows.h>
#include <stdio.h>
using namespace std;

int main()
{
	setlocale(LC_ALL, "rus");

	float x;

	cout << "Введите число: ";
	cin >> x;
	double half_1 = (x + 5.0) / 2.0;
	double half_2 = (x - 5.0) / 2.0;
	cout << "Полученные части: " << endl << half_1 << endl << half_2 << endl;
	system("pause");
}