#include <iostream>
using namespace std;


float count(float x,int n) {
	if (n == 0) return 1;
	if (n == 1) return x;
	return  (x*x / n/(n-1)) * count(x, n - 2);
}


void ch1()
{
	int n;
	float x;
	cout << "Введите x ";
	cin >> x;
	cout << "Введите n ";
	cin >> n;
	if (n > -1) {
		printf_s("Результат = %.2f", count(x, n));
	}
	else {
		printf_s("n отрицательное");
	}
	
}


int getF(int n, int m) {
	if (n == 0) return 1;
	if (n < m) return -1;
	return 2 * getF(n - 1, m);
}

void ch2() {
	int n, m;
	cout << "Введите n ";
	cin >> n;
	cout << "Введите m ";
	cin >> m;
	if (n < 0) {
		printf_s("n отрицательное");
		return;
	}
	printf_s("Результат = %d", getF(n, m));
}

void main() {
	setlocale(LC_ALL,"ru");
	ch1();
	cout << endl;
	ch2();
}