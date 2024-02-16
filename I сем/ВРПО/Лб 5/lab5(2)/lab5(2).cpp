#include <iostream>

int getInc(int x) {
	static int k = 1;
	return x += k;
};

int main(int argc, char* argv[]) {
	for (int i = 0; i <= 5; i++) {
		int k = 2;
		std::cout << i << ":" << getInc(i) << std::endl;
	}

	system("pause");
	return 0;
}