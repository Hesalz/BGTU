#include <iostream>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <windows.h>

using namespace std;

int main() {
    setlocale(LC_ALL, ".1251");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    cout << "Введите строку, состоящую из цифр и слов, разделенных пробелами: ";
    string inputString;
    getline(cin, inputString);

    ofstream fout("input.txt");
    if (fout.is_open()) {
        fout << inputString;
        fout.close();
        cout << "Строка успешно записана в файл input.txt" << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
        return 1;
    }

    ifstream fin("input.txt");
    if (fin.is_open()) {
        string word;
        int maxWordLength = 0;
        string longestWordWithoutDigits;

        while (fin >> word) {
            bool hasDigit = false;
            for (char c : word) {
                if (isdigit(c)) {
                    hasDigit = true;
                    break;
                }
            }
            if (!hasDigit && word.length() > maxWordLength) {
                maxWordLength = static_cast<int>(word.length());
                longestWordWithoutDigits = word;
            }
        }
        fin.close();

        cout << "Самое длинное слово без цифр содержит " << maxWordLength << " символов." << endl;
        cout << "Это слово: " << longestWordWithoutDigits << endl;
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
        return 1;
    }

    return 0;
}
