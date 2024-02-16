#include <iostream>
#include <fstream>
#include <cctype>
#include <string>
#include <windows.h>

using namespace std;

int main() { // Вариант 4
    setlocale(LC_ALL, ".1251");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    ifstream file1("input.txt");
    ofstream file2("output.txt");

    if (!file1.is_open() || !file2.is_open()) {
        cerr << "Ошибка открытия файлов." << endl;
        return 1;
    }

    int lineA = 0;

    string line;
    while (getline(file1, line)) {
        bool containsDigit = false;
        for (char c : line) {
            if (isdigit(c)) {
                containsDigit = true;
                break;
            }
        }

        if (!containsDigit) {
            file2 << line << endl;
            if (!line.empty() && line[0] == 'A') {
                lineA++;
            }
        }
    }

    file1.close();
    file2.close();

    cout << "Количество строк, начинающихся с буквы 'A' в output: " << lineA << endl;

    return 0;
}
