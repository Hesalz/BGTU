#include <iostream>
#include <windows.h>
#include <string>

using namespace std;

void setConsoleEncoding() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
}

void differenceBetweenLatinCharacters() {
    cout << "Введите буквы" << endl;
    string x;
    cin >> x;

    for (char c : x) {
        if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
            cout << "Разница между прописной и строчной буквой = 32" << endl;
        }
        else {
            cout << "Неверный ввод" << endl;
        }
    }
}

void differenceBetweenRussianCharacters() {
    cout << "Введите буквы" << endl;
    string x;
    cin >> x;

    for (char c : x) {
        if ((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я')) {
            cout << "Разница между прописной и строчной буквой = 32" << endl;
        }
        else {
            cout << "Неверный ввод" << endl;
        }
    }
}

void outputDigitCodes() {
    cout << "Введите цифры (0-9)" << endl;
    string x;
    cin >> x;

    for (char c : x) {
        if (c >= '0' && c <= '9') {
            int z = c - '0' + 48;
            cout << "Код цифры 1251 равен = " << z << endl;
        }
        else {
            cout << "Неверный ввод" << endl;
        }
    }
}

int main() {
    setConsoleEncoding();
    int a;
    while (true) {
        cout << "Выберите операцию (1- Разница кодов в латинском алфавите, 2- Разница кодов в русском алфавите, 3- Вывод кодов цифр 4-Выход)" << endl;
        cin >> a;

        switch (a) {
        case 1:
            differenceBetweenLatinCharacters();
            break;

        case 2:
            differenceBetweenRussianCharacters();
            break;

        case 3:
            outputDigitCodes();
            break;

        case 4:
            exit(0);

        default:
            cout << "Неверный ввод." << endl;
        }
    }

    return 0;
}
