#include <iostream>
#include <fstream>
#include <Windows.h>
#include <cstring>

using namespace std;

const int MAX_NAME_LENGTH = 50;
const int MAX_ITEMS = 100;

struct Item {
    char clientName[MAX_NAME_LENGTH];
    char itemName[MAX_NAME_LENGTH];
    float appraisalValue;
    float loanAmount;
    char dateReceived[MAX_NAME_LENGTH];
    int storagePeriod;
};

void inputItem(Item& item) {
    cout << "Введите имя клиента: ";
    cin.ignore();
    cin.getline(item.clientName, MAX_NAME_LENGTH);

    cout << "Введите наименование товара: ";
    cin.getline(item.itemName, MAX_NAME_LENGTH);

    cout << "Введите оценочную стоимость: ";
    cin >> item.appraisalValue;

    cout << "Введите сумму, выданную под залог: ";
    cin >> item.loanAmount;

    cout << "Введите дату сдачи: ";
    cin.ignore();
    cin.getline(item.dateReceived, MAX_NAME_LENGTH);

    cout << "Введите срок хранения (в днях): ";
    cin >> item.storagePeriod;
}

void printItem(const Item& item) {
    cout << "Имя клиента: " << item.clientName << endl;
    cout << "Наименование товара: " << item.itemName << endl;
    cout << "Оценочная стоимость: " << item.appraisalValue << endl;
    cout << "Сумма, выданная под залог: " << item.loanAmount << endl;
    cout << "Дата сдачи: " << item.dateReceived << endl;
    cout << "Срок хранения: " << item.storagePeriod << " дней" << endl;
}

void writeItemToFile(const Item& item, const char* filename) {
    ofstream file(filename, ios::app | ios::binary);
    if (file.is_open()) {
        file.write(reinterpret_cast<const char*>(&item), sizeof(Item));
        file.close();
    }
    else {
        cout << "Ошибка открытия файла." << endl;
    }
}

void readItemsFromFile(const char* filename) {
    ifstream file(filename, ios::binary);
    if (file.is_open()) {
        Item item;
        while (file.read(reinterpret_cast<char*>(&item), sizeof(Item))) {
            printItem(item);
            cout << endl;
        }
        file.close();
    }
    else {
        cout << "Ошибка открытия файла." << endl;
    }
}

void searchItemByName(const char* filename, const char* itemName) {
    ifstream file(filename, ios::binary);
    if (file.is_open()) {
        Item item;
        bool found = false;
        while (file.read(reinterpret_cast<char*>(&item), sizeof(Item))) {
            if (strcmp(item.itemName, itemName) == 0) {
                printItem(item);
                found = true;
            }
        }
        file.close();
        if (!found) {
            cout << "Товар с наименованием '" << itemName << "' не найден." << endl;
        }
    }
    else {
        cout << "Ошибка открытия файла." << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    char filename[] = "data.txt";

    while (true) {
        cout << "Выберите действие:" << endl;
        cout << "1. Ввод данных о товаре" << endl;
        cout << "2. Вывод данных о всех товарах" << endl;
        cout << "3. Поиск товара по наименованию" << endl;
        cout << "4. Выход" << endl;

        int choice;
        cin >> choice;

        switch (choice) {
        case 1: {
            Item newItem;
            cout << "Введите данные о товаре:" << endl;
            inputItem(newItem);
            writeItemToFile(newItem, filename);
            break;
        }
        case 2: {
            cout << "Данные о товарах:" << endl;
            readItemsFromFile(filename);
            break;
        }
        case 3: {
            char searchName[MAX_NAME_LENGTH];
            cout << "Введите наименование товара для поиска: ";
            cin.ignore();
            cin.getline(searchName, MAX_NAME_LENGTH);
            searchItemByName(filename, searchName);
            break;
        }
        case 4: {
            cout << "Выход из программы." << endl;
            return 0;
        }
        default:
            cout << "Некорректный выбор. Попробуйте снова." << endl;
        }
    }

    return 0;
}
