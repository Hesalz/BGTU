#include <iostream>
using namespace std;

void second() // Парабола 
{
	setlocale(LC_ALL, "RUS");
	int n = 200, i = 1;
	double h, x, s1 = 0, s2 = 0, z, a = 1, b = 6;
	h = (b - a) / (2 * n);
	x = a + 2 * h;
	for (i; i < n; i++)
	{
		s2 += (1 + pow(x, 3));
		x += h;
		s1 += (1 + pow(x, 3));
		x += h;
	}
	z = (h / 3) * (1 + pow(a, 3) + (4 * (1 + pow((a + h), 3))) + (4 * s1) + (2 * s2) + (1 + pow(b, 3)));
	cout << "z = " << z << endl;
}

void third() // метод Дихотомии
{
	using namespace std;
	setlocale(LC_ALL, "RUS");
	int b = 2;
	double e = 0.0001, x = 0, a =-1;
	x = (a + b) / 2;

	if (((pow(x, 3)) + 2 * x - 1) * ((pow(a, 3)) + 2 * a - 1) <= 0) {
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
		if (((pow(x, 3)) + 2 * x - 1) * ((pow(a, 3)) + 2 * a - 1) <= 0) {
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
	setlocale(LC_ALL, "RUS");
	double h, x, s = 0, a = 1, b = 6, n = 200;
	h = (b - a) / n;
	x = a;
	for (; x < b - h; x += h)
	{
		s = s + h * (1 + pow(x, 3) + (1 + pow(x, 3) + h)) / 2;
	}
	cout << "s = " << s << endl;
	second();
	third();
}