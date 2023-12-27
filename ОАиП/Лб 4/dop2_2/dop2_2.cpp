#include <iostream>;

using namespace std;

int main()
{
	setlocale(LC_ALL, "rus");

	double x, y, z;
	double m, n;

	cout << "Введите x " << endl;
	cin >> x;

	cout << "Введите y " << endl;
	cin >> y;

	cout << "Введите z" << endl;
	cin >> z;

	m = (abs(x) + abs(y) + abs(z)) / 3;
	n = pow(abs(x * y * z), 1.0 / 3.0);

	cout << "Среднее арифметическое = " << m << endl;
	cout << "Среднее геометрическое = " << n << endl;


	return 0;
}