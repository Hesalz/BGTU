#include <iostream>
#include <fstream>
#include <sstream>
#include <string>

using namespace std;

int main() {
    setlocale(LC_ALL, ".1251");
    cout << "Введите строку символов, состоящую из цифр и слов, разделенных пробелами: ";
    string inputString;
    getline(cin, inputString);

    ofstream fout("data.txt");
    if (fout.is_open()) {
        fout << inputString;
        fout.close();
        cout << "Строка успешно записана в файл data.txt" << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
        return 1;
    }

    ifstream fin("data.txt");
    if (fin.is_open()) {
        string line;
        int number;
        while (getline(fin, line)) {
            istringstream iss(line);
            string word;
            while (iss >> word) {
                if (isdigit(word[0])) {
                    number = stoi(word);
                    if (number % 2 != 0) {
                        cout << number << " ";
                    }
                }
            }
            cout << endl;
        }
        fin.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
        return 1;
    }

    return 0;
}
