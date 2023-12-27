#include <iostream>
using namespace std;
// Вариант 14
void second() // Парабола 
{
	setlocale(LC_ALL, "RUS");
	int n = 200, i = 1;
	double h, x, s1 = 0, s2 = 0, z, a = 1, b = 4;
	h = (b - a) / (2 * n);
	x = a + 2 * h;
	for (i; i < n; i++)
	{
		s2 += (pow(x, 4) + 4);
		x += h;
		s1 += (pow(x, 4) + 4);
		x += h;
	}
	z = (h / 3) * ((pow(1, 4) + 4) + (4 * (pow((a + h), 4) + 4)) + (4 * s1) + (2 * s2) + (pow(b, 4) + 4));
	cout << "z = " << z << endl;
}

void third() // метод Дихотомии
{
	setlocale(LC_ALL, "RUS");
	int b = 2;
	double e = 0.0001, x1, x = 0, a = -1;
	x = (a + b) / 2;
	if (((pow(x, 3)) + x - 2) * ((pow(a, 3)) + a - 2) <= 0) {
		b = x;
	}
	else {
		a = x;
	}
	if (abs(a - b) > 2 * e)
	{
		x = (a + b) / 2;
	}
	else {
		if (((pow(x, 3)) + x - 2) * ((pow(a, 3)) + a - 2) <= 0) {
			b = x;
		}
		else {
			a = x;
		}
	}
	cout << "x = " << x << endl;
}

void main() // Трапеция
{
	using namespace std;
	setlocale(LC_ALL, "RUS");
	double h, x, s = 0, a = 1, b = 4, n = 200;
	h = (b - a) / n;
	x = a;
	for (; x < b - h; x += h)
	{
		s = s + h * ((pow(x, 4) + 4 + pow(x + h, 4) + 4) / 2);
	}
	cout << "s = " << s << endl;
	second();
	third();
}