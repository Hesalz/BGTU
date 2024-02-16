#include <iostream> 
using namespace std;

void second(){ // Вариант 15
    setlocale(LC_ALL, "RUS");
    cout << endl << "Задание №2" << endl;
    int A, n, p; char num[33];
    cout << "Введите число А " << endl;
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

void main() {
	setlocale(LC_CTYPE, "Russian");
	int A, i; char tmp[33];
	cout << "Введите число ";
	cin >> A;
	_itoa_s(A, tmp, 2);
	cout << "Число в двоичном виде = " << tmp << endl;
	if ((A & 1) == 0)
		cout << "Число кратно 2" << endl;
	else
		cout << "Число не кратно 2" << endl;
    second();
}