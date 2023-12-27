#include <iostream>
using namespace std;
// Вариант 9
void second() // Парабола 
{
	setlocale(LC_ALL, "RUS");
	int n = 200, i = 1;
	double h, x, s1 = 0, s2 = 0, z, a = 3, b = 6;
	h = (b - a) / (2 * n);
	x = a + 2 * h;
	for (i; i < n; i++)
	{
		s2 += (pow(x, 3) + 3);
		x += h;
		s1 += (pow(x, 3) + 3);
		x += h;
	}
	z = (h / 3) * ((pow(a, 4) + 3) + (4 * (pow((a + h), 3) + 3)) + (4 * s1) + (2 * s2) + (pow(b, 3) + 3));
	cout << "z = " <<  z << endl;
}

void third() // метод Дихотомии
{
	setlocale(LC_ALL, "RUS");
	int b = 2;
	double e = 0.0001, x = 0, a = -1;
	x = (a + b) / 2;
	if (((sin(x) + pow(x,3)) * (sin(a) + pow(a,3))) <= 0) {
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
		if ((sin(x) + pow(x,3)) * (sin(a) + pow(a,3)) <= 0) {
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
	double h, x, s = 0, a = 3, b = 6, n = 200;
	h = (b - a) / n;
	x = a;
	for (; x < b - h; x += h)
	{
		s = s + h * ((pow(x, 3) + 3 + pow(x + h, 3) + 3) / 2);
	}
	cout << "s = " << s << endl;
	second();
	third();
}