#include <iostream>
using namespace std;
int main() // var 3
{
	double d = 0.0, f = 0.0, i = -6, x = 4.5, z = 1.5e-6;
	d = tan(-x * i) / sqrt(x - z);
	f = sin(2*d) / d;
	cout << "d=" << d << endl; 
	cout << "f=" << f << endl;
	return 0;
}

