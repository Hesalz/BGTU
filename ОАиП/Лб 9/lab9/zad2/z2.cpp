#include <iostream>
using namespace std;

void main() // Парабола 
{
	setlocale(LC_ALL, "RUS");
	int n = 200, i = 1;
	double h, x, s1 = 0, s2 = 0, s, a = 1, b = 6;
	h = (b - a) / (2 * n);
	x = a + 2 * h;
	for (i; i < n; i++)
	{
		s2 += (1 + pow(x, 3));
		x += h;
		s1 += (1 + pow(x, 3));
		x += h;
	}
	s = (h / 3) * (1 + pow(a, 3) + (4 * (1 + pow((a + h), 3))) + (4 * s1) + (2 * s2) + (1 + pow(b, 3)));
	cout << s << endl;
}