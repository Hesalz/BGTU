#include <iostream>
#include <iomanip>
using namespace std;


int main()
{
	setlocale(LC_ALL, "rus");
	char p, c;
	p = ' ';

	cout << "Введите символ " << endl;
	cin >> c;

	cout << setw(25) << setfill(p) << p; 
	cout << setw(1) << setfill(c) << c << endl;
	cout << setw(24) << setfill(p) << p;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(23) << setfill(p) << p;
	cout << setw(5) << setfill(c) << c << endl;
	cout << setw(22) << setfill(p) << p;
	cout << setw(7) << setfill(c) << c << endl;
	cout << setw(21) << setfill(p) << p;
	cout << setw(9) << setfill(c) << c << endl;
	cout << setw(20) << setfill(p) << p;
	cout << setw(11) << setfill(c) << c << endl;
	cout << setw(19) << setfill(p) << p;
	cout << setw(13) << setfill(c) << c << endl; // 1
	cout << setw(23) << setfill(p) << p;
	cout << setw(5) << setfill(c) << c << endl;
	cout << setw(22) << setfill(p) << p;
	cout << setw(7) << setfill(c) << c << endl;
	cout << setw(21) << setfill(p) << p;
	cout << setw(9) << setfill(c) << c << endl;
	cout << setw(20) << setfill(p) << p;
	cout << setw(11) << setfill(c) << c << endl;
	cout << setw(19) << setfill(p) << p;
	cout << setw(13) << setfill(c) << c << endl;
	cout << setw(18) << setfill(p) << p;
	cout << setw(15) << setfill(c) << c << endl;
	cout << setw(17) << setfill(p) << p;
	cout << setw(17) << setfill(c) << c << endl; // 2
	cout << setw(22) << setfill(p) << p;
	cout << setw(7) << setfill(c) << c << endl;
	cout << setw(21) << setfill(p) << p;
	cout << setw(9) << setfill(c) << c << endl;
	cout << setw(20) << setfill(p) << p;
	cout << setw(11) << setfill(c) << c << endl;
	cout << setw(19) << setfill(p) << p;
	cout << setw(13) << setfill(c) << c << endl;
	cout << setw(18) << setfill(p) << p;
	cout << setw(15) << setfill(c) << c << endl;
	cout << setw(17) << setfill(p) << p;
	cout << setw(17) << setfill(c) << c << endl;
	cout << setw(16) << setfill(p) << p;
	cout << setw(19) << setfill(c) << c << endl;
	cout << setw(15) << setfill(p) << p;
	cout << setw(21) << setfill(c) << c << endl; // 3
	cout << setw(24) << setfill(p) << p;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(24) << setfill(p) << p;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(24) << setfill(p) << p;
	cout << setw(3) << setfill(c) << c << endl;
	cout << setw(24) << setfill(p) << p;
	cout << setw(3) << setfill(c) << c << endl;
	return 0;
}
