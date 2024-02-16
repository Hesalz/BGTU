#include <iostream> 
using namespace std;

void second() { // Вариант 9
	setlocale(LC_ALL, "RUS");
	cout << endl << "Задание №2" << endl;
	int A, n, p; char num[33];
	cout << "Введите число А ";
	cin >> A;
	_itoa_s(A, num, 2);
	cout << "Число в двоичном виде = " << num << endl;
	cout << "Введите c какого бита начинать замену ";
	cin >> p;
	cout << "Скольно заменяем битов? ";
	cin >> n;
	_itoa_s(A, num, 2);
	cout << "Число А в двоичном  виде: " << num << endl;
	unsigned int mask = 1 << p - 1;
	for (int i = 0; i <= n; i++) {
		A ^= mask;
		mask <<= 1;
	}
	_itoa_s(A, num, 2);
	cout << "Итоговое число: " << num << endl;
}
void main()
{
	using namespace std;
	setlocale(LC_ALL, "RUS");
	int A, B, A_mask = 07;
	char tmp[33];
	cout << "Введите число A: ";
	cin >> A;
	cout << "Введите число B: ";
	cin >> B;
	cout << " " << endl;
	cout << "Число A: " << A << endl;
	_itoa_s(A, tmp, 2);
	cout << "Представление числа в двоичном коде: " << tmp << endl;
	cout << " " << endl;
	cout << "Число B: " << B << endl;
	_itoa_s(B, tmp, 2);
	cout << "Представление числа в двоичном коде: " << tmp << endl;
	A >>= 2;
	A &= A_mask;
	B <<= 5;
	B |= A;
	B <<= 1;
	cout << " " << endl;
	cout << "Число B = " << B << endl;
	_itoa_s(B, tmp, 2);
	cout << "Представление числа в двоичном коде: " << tmp << endl;
	second();
}