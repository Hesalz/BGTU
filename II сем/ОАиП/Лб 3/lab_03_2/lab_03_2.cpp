#include <iostream>
#include <fstream>
#include <cctype>
#include <windows.h>

using namespace std;

void read(const char* data, const string& filename) {
    ofstream file(filename);
    if (file.is_open()) {
        file << data;
        cout << "Данные успешно записаны в файл " << filename << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
    }
}

void write(const string& inputFilename, const string& outputFilename) {
    ifstream input(inputFilename);
    ofstream output(outputFilename);
    if (input && output) {
        char ch;
        bool space = false;
        while (input.get(ch)) {
            if (!isdigit(ch) || isspace(ch)) {
                output << ch;
                space = (isspace(ch));
            }
        }
        cout << "Слова успешно записаны в файл " << outputFilename << endl;
    }
    else {
        cout << "Ошибка открытия файлов." << endl;
    }
}

int main() {
    setlocale(LC_ALL, ".1251");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    cout << "Введите строку символов, состоящую из цифр и слов, разделенных пробелами: ";
    const int MAX_LENGTH = 1000;
    char input[MAX_LENGTH], ch;
    int i = 0;
    while (cin.get(ch)) {
        if (ch == '\n') {
            input[i] = '\0';
            break;
        }
        input[i++] = ch;
    }

    read(input, "input.txt");
    write("input.txt", "output.txt");

    return 0;
}
