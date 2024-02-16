#include <iostream>
#include <fstream>
#include <sstream>
#include <Windows.h>
using namespace std;

int main() { // Вариант 2
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    ifstream file1("input.txt");
    ofstream file2("output.txt");

    if (!file1.is_open() || !file2.is_open()) {
        cerr << "Failed to open files." << endl;
        return 1;
    }

    char line[100];
    int wordCount = 0;

    while (file1.getline(line, 100)) {
        if (line[0] == 'A') {
            file2 << line << endl;

            istringstream iss(line);
            char word[20];
            while (iss >> word) {
                ++wordCount;
            }
        }
    }

    file1.close();
    file2.close();

    cout << "Количество слов в FILE2: " << wordCount << endl;

    return 0;
}
