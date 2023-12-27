#include <iostream>
#include <iomanip>
using namespace std;


int main() // 6 var
{
	setlocale(LC_ALL, "rus");
	char p, x;
	p = ' ';

	cout << "Введите символ " << endl;
	cin >> x;

	cout << setw(33) << setfill(p) << p << setw(1) << setfill(x) << x << endl;
	cout << setw(32) << setfill(p) << p << setw(3) << setfill(x) << x << endl;
	cout << setw(31) << setfill(p) << p << setw(5) << setfill(x) << x << endl;
	cout << setw(30) << setfill(p) << p << setw(7) << setfill(x) << x << endl;
	cout << setw(29) << setfill(p) << p << setw(9) << setfill(x) << x << endl;
	cout << setw(24) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(21) << setfill(p) << p << setw(25) << setfill(x) << x << endl;
	cout << setw(18) << setfill(p) << p << setw(31) << setfill(x) << x << endl;
	cout << setw(21) << setfill(p) << p << setw(25) << setfill(x) << x << endl;
	cout << setw(24) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(29) << setfill(p) << p << setw(9) << setfill(x) << x << endl;
	cout << setw(30) << setfill(p) << p << setw(7) << setfill(x) << x << endl;
	cout << setw(31) << setfill(p) << p << setw(5) << setfill(x) << x << endl;
	cout << setw(32) << setfill(p) << p << setw(3) << setfill(x) << x << endl;
	cout << setw(33) << setfill(p) << p << setw(1) << setfill(x) << x << endl;
}