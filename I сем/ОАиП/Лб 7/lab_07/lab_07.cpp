#include <iostream>
using namespace std;

void second() {
    int n, count = 0;
    cout << "Введите число n: ";
    cin >> n;

    for (int i = 100; i <= 999; ++i) {
        int num1 = i / 100;
        int num2 = (i / 10) % 10;
        int num3 = i % 10;
        

        if (num1 + num2 + num3 == n) {
            count++;
            cout << i << "; ";
        }
    }

    cout << endl << "Количество трехзначных чисел, сумма цифр которых равна " << n << ": " << count << endl;
}

void dop1() { // Доп. задача 1
    for (int num = 100; num <= 999; num++) {
        int digit1 = num / 100;
        int digit2 = (num / 10) % 10;
        int digit3 = num % 10;

        if (digit1 < digit2 && digit2 < digit3) {
            if (sqrt(num) == static_cast<int>(sqrt(num))) {
                cout << "Найдено число: " << num << endl;
                break;
            }
        }
    }
}

void dop2() { // Доп. задача 3
    double c1, c2, half;
    cout << "Введите количество воды в первом и во втором сосуде (в литрах) через пробел : ";
    cin >> c1 >> c2;

    for (int i = 0; i < 12; i++) {
        half = c1 / 2;
        c1 -= half;
        c2 += half;

        half = c2 / 2;
        c2 -= half;
        c1 += half;
    }
    cout << "После 12 переливаний в сосудах будет воды в " << "первом сосуде: " << c1 << "; во втором: " << c2 << endl;
}

void dop3() { // Доп. задача 4
    int sum, temp;
    for (int num = 1000; num <= 9999; num++) {
        if (num % 2 == 0 && num % 7 == 0 && num % 11 == 0) {
            sum = 0;
            temp = num;
            while (temp > 0) {
                sum += temp % 10;
                temp /= 10;
            }
            if (sum == 30) {
                cout << "Номер автомобиля: " << num << endl;
            }
        }
    }
}

int main() {
	setlocale(LC_ALL, "rus");
	double x[] = { 8,1.9, 4, 1 }, a = 105e-4, m = 4, i = 7, p, s;

	for (int k = 0; k < 4; k++) {
		s = exp(-a * x[k]) - log(i / x[k] * m) / pow(log(m), 2);
		p = s > 2 * x[k] ? pow((-i * s), 2) : (sin(-6 * s));
		cout << "При x[" << k << "]: s = " << s << "; p = " << p << endl;
	}

	second();
    dop1();
    dop2();
    dop3();
}