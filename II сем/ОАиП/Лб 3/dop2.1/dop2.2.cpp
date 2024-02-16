#include <iostream>
#include <fstream>
#include <sstream>

using namespace std;

int main() {
    setlocale(LC_ALL, "rus");
    cout << "Введите строку символов, состоящую из групп цифр и нулей, разделенных пробелами: ";
    string inputString;
    getline(cin, inputString);

    ofstream fout("output.txt");
    if (fout.is_open()) {
        fout << inputString;
        fout.close();
        cout << "Строка успешно записана в файл output.txt" << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
        return 1;
    }

    ifstream fin("output.txt");
    if (fin.is_open()) {
        string line;
        getline(fin, line);
        fin.close();

        istringstream iss(line);
        string shortestGroup;
        int shortestLength = 100;
        string group;
        while (iss >> group) {
            if (group.length() < shortestLength) {
                shortestLength = group.length();
                shortestGroup = group;
            }
        }

        cout << "Самая короткая группа цифр: " << shortestGroup << endl;
    }
    else {
        cerr << "Ошибка открытия файла для чтения." << endl;
        return 1;
    }

    return 0;
}
