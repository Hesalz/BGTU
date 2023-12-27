#include <iostream>
#include <windows.h>
using namespace std;

int main() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    int a, y;
    char x;

    cout << "Выберите операцию:" << endl << "(1 - Разница кодов в латинском алфавите, 2 - Разница кодов в русском алфавите, 3 - Вывод кодов цифр, 4 - Выход)" << endl;
    cin >> a;

    switch(a){
    case 1:
        cout << "Введите букву" << endl;
        cin >> x;
        if ((x >= 'A' && x <= 'Z') || (x >= 'a' && x <= 'z')) {
            cout << "Разница между прописной и строчной буквой = 32" << endl;
        }

        else
            cout << "Неверный ввод" << endl;
        break;

    case(2):
        cout << "Введите букву" << endl;
        cin >> x;
        if ((x >= 'А' && x <= 'Я') || (x >= 'а' && x <= 'я')) {
            cout << "Разница между прописной и строчной буквой = 32" << endl;
        }

        else
            cout << "Неверный ввод" << endl;
        break;

    case(3):
        cout << "Введите цифру (0-9)" << endl;
        cin >> y;
        if (y >= 0 && y <= 9) {
            int z = y + 48;
            cout << "Код цифры 1251 равен = " << z << endl;
        }

        else
            cout << "Неверный ввод" << endl;
        break;


    case(4):
        exit(0);

    default: 
        cout << "Неверный ввод." << endl;
    }

}