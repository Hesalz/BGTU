#include <iostream>

int getSum(int x, int y);
int getMul(int x, int y);

int parm1 = 2;
int parm2 = 3;

int main(int argc, char* argv[]) {
	std::cout << "getSum(" << parm1 << "," << parm2 << ") = " << getSum(2, 3) << std::endl;

	parm1 = 5;
	parm2 = 5;
	std::cout << "getMul(" << parm1 << "," << parm2 << ") = " << getMul(2, 3) << std::endl;

	system("pause");
	return 0;	
}