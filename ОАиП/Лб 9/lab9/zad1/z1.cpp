#include <iostream>
using namespace std;

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
}