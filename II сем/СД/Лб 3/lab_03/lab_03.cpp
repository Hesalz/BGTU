#include <iostream>
#include <string>
#include <fstream>
#include <Windows.h>

using namespace std;

void Menu() {
    cout << "1. Создать и записать данные в файл\n"
        << "2. Чтение файла\n"
        << "3. Удаление содержимого файла\n"
        << "4. Удаление файла\n"
        << "5. Выход\n"
        << "Введите ваш выбор: ";
}

int main() {
    setlocale(LC_ALL, ".1251");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    string FileName = "";
    string temp;
    bool isFileCreated = false; 

    Menu();
    int choice;
    cin >> choice;
    ofstream fout;
    ifstream fin;

    while (choice != 5) {
        switch (choice) {
        case 1:
            system("cls");
            cout << "Введите имя файла: ";
            cin.ignore();
            getline(cin, FileName);
            fout.open(FileName);
            if (fout.is_open()) {
                cout << "Введите данные, необходимые записать в файл:\n";
                getline(cin, temp);
                fout << temp;
                cout << "Данные успешно записаны в файл.\n";
                isFileCreated = true;
            }
            else {
                cout << "Не удалось создать файл.\n";
            }
            fout.close();
            system("pause");
            system("cls");
            Menu();
            break;
        case 2:
            system("cls");
            fin.open(FileName);
            if (fin.is_open()) {
                getline(fin, temp);
                cout << temp << endl;
            }
            else {
                cout << "Не удалось открыть файл.\n";
            }
            fin.close();
            system("pause");
            system("cls");
            Menu();
            break;
        case 3:
            system("cls");
            if (!isFileCreated) {
                cout << "Файл не был создан.\n";
            }
            else {
                fout.open(FileName, ios::trunc);
                cout << "Содержимое файла удалено.\n";
                fout.close();
            }
            system("pause");
            system("cls");
            Menu();
            break;
        case 4:
            system("cls");
            if (remove(FileName.c_str()) == 0) {
                cout << "Файл успешно удален.\n";
                isFileCreated = false;
            }
            else {
                cout << "Не удалось удалить файл.\n";
            }
            system("pause");
            system("cls");
            Menu();
            break;
        case 5:
            exit;
        default:
            system("cls");
            cout << "Неверный выбор.";
            system("pause");
            system("cls");
            Menu();
            break;
        }
        cin >> choice;
    }
    return 0;
}
