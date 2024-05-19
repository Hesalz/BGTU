#include <iostream>
#include <Windows.h>
#include <fstream>
#include <string>

using namespace std;

struct TeacherExamData {
    string lastName;
    string examName;
    string examDate;
};

void inputTeacherExamData(TeacherExamData& data) {
    cout << "Введите фамилию преподавателя: ";
    getline(cin, data.lastName);
    cout << "Введите название экзамена: ";
    getline(cin, data.examName);
    cout << "Введите дату экзамена: ";
    getline(cin, data.examDate);
}

void writeToFile(const TeacherExamData& data, const string& filename) {
    ofstream outFile(filename, ios::app);
    if (outFile.is_open()) {
        outFile << data.lastName << "\t" << data.examName << "\t" << data.examDate << endl;
        cout << "Данные успешно записаны в файл." << endl;
        outFile.close();
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
    }
}

void readFromFileAndPrint(const string& filename) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Содержимое файла:" << endl;
        string line;
        while (getline(inFile, line)) {
            cout << line << endl;
        }
        inFile.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
    }
}

void searchByLastName(const string& filename, const string& lastName) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Результаты поиска по фамилии " << lastName << ":" << endl;
        string line;
        bool found = false;
        while (getline(inFile, line)) {
            size_t pos = line.find(lastName);
            if (pos != string::npos) {
                cout << line << endl;
                found = true;
            }
        }
        if (!found) {
            cout << "Преподаватель с фамилией " << lastName << " не найден." << endl;
        }
        inFile.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    TeacherExamData data;
    string filename = "data.txt";

    int choice;
    string lastName;

    do {
        cout << "\nМеню:" << endl;
        cout << "1. Ввод данных о преподавателе и экзамене" << endl;
        cout << "2. Запись данных в файл" << endl;
        cout << "3. Вывод данных из файла на экран" << endl;
        cout << "4. Поиск данных по фамилии преподавателя" << endl;
        cout << "0. Выход" << endl;
        cout << "Выберите действие: ";
        cin >> choice;
        cin.ignore();

        switch (choice) {
        case 1:
            inputTeacherExamData(data);
            break;
        case 2:
            writeToFile(data, filename);
            break;
        case 3:
            readFromFileAndPrint(filename);
            break;
        case 4:
            cout << "Введите фамилию преподавателя для поиска: ";
            getline(cin, lastName);
            searchByLastName(filename, lastName);
            break;
        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте еще раз." << endl;
        }
    } while (choice != 0);

    return 0;
}
