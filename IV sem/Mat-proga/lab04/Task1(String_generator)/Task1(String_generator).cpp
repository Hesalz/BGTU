#include <iostream>
#include <ctime>
#include <Windows.h>

using namespace std;

#define FIRST_LEN 200
#define SECOND_LEN 300

char* GenerateRandomString(int size)
{
	char* str = (char*)malloc(sizeof(char) * size);
	for (int i = 0; i < size; i++) {
		str[i] = rand() % 26 + 'a'; 
	}
	return str;
}


int main()
{
	SetConsoleCP(1251);
	SetConsoleOutputCP(1251);
	srand(time(NULL));

	char* s1 = GenerateRandomString(FIRST_LEN);
	cout << "S1: " << endl;
	for (int i = 0; i < FIRST_LEN; i++) {
		if (i % 50 == 0)
		{
			cout << "\n";
		}
		cout << s1[i];
	}
	cout << endl << endl;

	char* s2 = GenerateRandomString(SECOND_LEN);
	cout << "S2: " << endl;
	for (int i = 0; i < SECOND_LEN; i++) {
		if (i % 50 == 0)
		{
			cout << "\n";
		}
		cout << s2[i];
	}
	cout << endl << endl;
}