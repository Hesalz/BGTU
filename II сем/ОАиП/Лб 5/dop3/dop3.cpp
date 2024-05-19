#include <iostream>
#include <fstream>
#include <Windows.h>
#include <string>
#include <vector>

using namespace std; // Вариант 5

enum class Origin {
    Покупка,
    Кража,
    Подарок
};

struct Book {
    string author;
    string title;
    string publisher;
    string section;
    Origin origin;
    bool available;
    int year;
};

void inputBookData(Book& book) {
    cout << "Введите автора книги: ";
    getline(cin, book.author);
    cout << "Введите название книги: ";
    getline(cin, book.title);
    cout << "Введите издательство: ";
    getline(cin, book.publisher);
    cout << "Введите раздел библиотеки: ";
    getline(cin, book.section);
    cout << "Выберите происхождение книги (0 - Покупка, 1 - Кража, 2 - Подарок): ";
    int origin;
    cin >> origin;
    book.origin = static_cast<Origin>(origin);
    cout << "Введите год издания: ";
    cin >> book.year;
    cout << "Книга доступна? (1 - да, 0 - нет): ";
    cin >> book.available;
    cin.ignore();
}

void printBookData(const Book& book) {
    cout << "Автор: " << book.author << endl;
    cout << "Название: " << book.title << endl;
    cout << "Издательство: " << book.publisher << endl;
    cout << "Раздел библиотеки: " << book.section << endl;
    cout << "Происхождение: ";
    switch (book.origin) {
    case Origin::Покупка:
        cout << "Покупка" << endl;
        break;
    case Origin::Кража:
        cout << "Кража" << endl;
        break;
    case Origin::Подарок:
        cout << "Подарок" << endl;
        break;
    }
    cout << "Год издания: " << book.year << endl;
    cout << "Доступность: " << (book.available ? "Доступна" : "Недоступна") << endl;
}

void writeToFile(const Book& book, const string& filename) {
    ofstream outFile(filename, ios::app);
    if (outFile.is_open()) {
        outFile << book.author << "\t";
        outFile << book.title << "\t";
        outFile << book.publisher << "\t";
        outFile << book.section << "\t";
        outFile << static_cast<int>(book.origin) << "\t";
        outFile << book.year << "\t";
        outFile << book.available << endl;
        outFile.close();
        cout << "Данные о книге успешно записаны в файл." << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
    }
}

void readFromFileAndPrint(const string& filename) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Содержимое файла:" << endl;
        string author, title, publisher, section, originStr, availableStr;
        int origin, year;
        bool available;
        while (inFile >> author >> title >> publisher >> section >> origin >> year >> available) {
            if (origin == 0)
                originStr = "Покупка";
            else if (origin == 1)
                originStr = "Кража";
            else if (origin == 2)
                originStr = "Подарок";
            if (available)
                availableStr = "Доступна";
            else
                availableStr = "Недоступна";

            cout << "Автор: " << author << endl;
            cout << "Название: " << title << endl;
            cout << "Издательство: " << publisher << endl;
            cout << "Раздел библиотеки: " << section << endl;
            cout << "Происхождение: " << originStr << endl;
            cout << "Год издания: " << year << endl;
            cout << "Доступность: " << availableStr << endl;
            cout << endl;
        }
        inFile.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
    }
}

void searchByYear(const string& filename, int year) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Результаты поиска книг за год " << year << ":" << endl;
        string author, title, publisher, section, originStr, availableStr;
        int origin, bookYear;
        bool available;
        bool found = false;
        while (inFile >> author >> title >> publisher >> section >> origin >> bookYear >> available) {
            if (bookYear == year) {
                if (origin == 0)
                    originStr = "Покупка";
                else if (origin == 1)
                    originStr = "Кража";
                else if (origin == 2)
                    originStr = "Подарок";
                if (available)
                    availableStr = "Доступна";
                else
                    availableStr = "Недоступна";

                cout << "Автор: " << author << endl;
                cout << "Название: " << title << endl;
                cout << "Издательство: " << publisher << endl;
                cout << "Раздел библиотеки: " << section << endl;
                cout << "Происхождение: " << originStr << endl;
                cout << "Год издания: " << bookYear << endl;
                cout << "Доступность: " << availableStr << endl;
                cout << endl;
                found = true;
            }
        }
        if (!found) {
            cout << "Книги за указанный год не найдены." << endl;
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
    string filename = "data.txt";

    int choice, year;
    do {
        cout << "\nМеню:" << endl;
        cout << "1. Запись данных о книге в файл" << endl;
        cout << "2. Вывод данных из файла на экран" << endl;
        cout << "3. Поиск книг по году издания" << endl;
        cout << "0. Выход из программы" << endl;
        cout << "Выберите действие: ";
        cin >> choice;
        cin.ignore();

        switch (choice) {
        case 1: {
            Book book;
            inputBookData(book);
            writeToFile(book, filename);
            break;
        }
        case 2: {
            readFromFileAndPrint(filename);
            break;
        }
        case 3:
            cout << "Введите год издания: ";
            cin >> year;
            searchByYear(filename, year);
            break;
        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте еще раз." << endl;
        }
    } while (choice != 0);

    return 0;
}
