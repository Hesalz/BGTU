#include <iomanip> 
#include <iostream>
using namespace std;
int main() // 4 var
{
	setlocale(LC_CTYPE, "Russian");

	char c, p;

	p = ' ';
	cout << "Введите символ "; cin >> c;

	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;
	cout << setw(80) << setfill(p) << p << endl;

	cout << setw(50) << setfill(p) << p;
	cout << setw(20) << setfill(c) << c << endl;
	cout << setw(47) << setfill(p) << p;
	cout << setw(26) << setfill(c) << c << endl;

	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;
	cout << setw(46) << setfill(p) << p;
	cout << setw(28) << setfill(c) << c << endl;

	cout << setw(47) << setfill(p) << p;
	cout << setw(26) << setfill(c) << c << endl;
	cout << setw(50) << setfill(p) << p;
	cout << setw(20) << setfill(c) << c << endl;

	return 0;
}