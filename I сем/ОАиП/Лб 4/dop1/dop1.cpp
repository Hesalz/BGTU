#include <iostream>
#include <iomanip>
using namespace std;


int main() // 2 var
{
	setlocale(LC_ALL, "rus");
	char p, x;
	p = ' ';

	cout << "Введите символ " << endl;
	cin >> x;

	cout << setw(33) << setfill(p) << p << setw(3) << setfill(x) << x << endl;
	cout << setw(31) << setfill(p) << p << setw(7) << setfill(x) << x << endl;
	cout << setw(29) << setfill(p) << p << setw(11) << setfill(x) << x << endl;
	cout << setw(27) << setfill(p) << p << setw(15) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(25) << setfill(p) << p << setw(19) << setfill(x) << x << endl;
	cout << setw(27) << setfill(p) << p << setw(15) << setfill(x) << x << endl;
	cout << setw(29) << setfill(p) << p << setw(11) << setfill(x) << x << endl;
	cout << setw(31) << setfill(p) << p << setw(7)<< setfill(x) << x << endl;
	cout << setw(33) << setfill(p) << p << setw(3) << setfill(x) << x << endl;
}