#include <iostream>
void main() // метод Дихотомии
{
	using namespace std;
	setlocale(LC_ALL, "RUS");
	int a = -1, b = 2;
	double e = 0.0001, x = 0;
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

	cout << x << endl;
}