#include <iostream> 
using namespace std;
void second() // Вариант 2
{
	setlocale(LC_CTYPE, "Russian");
	cout << endl << "Задание №2" << endl;
	int A, position, n, razn;
	char tmp[33];
	cout << "Введите A ";
	cin >> A;
	_itoa_s(A, tmp, 2);
	cout << "Представление числа в двоичном коде: " << tmp << endl;
	cout << "Введите начальную позицию ";
	cin >> position;
	cout << "Введите число битов ";
	cin >> n;
	razn = position - n;
	while (position >= razn)
	{
		A |= 1 << position;
		_itoa_s(A, tmp, 2);
		position--;
	}
	cout << tmp << endl;
}

void main()
{
	using namespace std;
	setlocale(LC_ALL, "RUS");
	int A, A_mask = 07, B;
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
	B <<= 2;
	cout << " " << endl;
	cout << "Число B = " << B << endl;
	_itoa_s(B, tmp, 2);
	cout << "Представление числа в двоичном коде: " << tmp << endl;
	second();
}