#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <Windows.h>
using namespace std;

struct Client {
    int index;
    string name;
};

int choice;
vector<Client> clients;
int nextIndex = 1;

void Menu() {
    cout << "1. Добавить нового клиента\n"
        << "2. Оценить товар и выдать сумму под залог\n"
        << "3. Просмотреть список активных залогов\n"
        << "4. Продажа товара\n"
        << "5. Посмотреть список клиентов\n"
        << "6. Найти клиента по номеру\n"
        << "7. Загрузить данные из файла\n"
        << "8. Сохранить данные в файл\n"
        << "9. Выход\n"
        << "Введите ваш выбор: ";
    cin >> choice;
};

Client* findClientByIndex(int index) {
    for (auto& client : clients) {
        if (client.index == index) {
            return &client;
        }
    }
    return nullptr;
}

void readFromFile(const string& filename) {
    ifstream file(filename);
    if (!file.is_open()) {
        cout << "Ошибка открытия файла.\n";
        return;
    }

    clients.clear();
    int maxIndex = 0;
    int index;
    string name;
    while (file >> index) {
        file.ignore();
        getline(file, name);
        clients.push_back({ index, name });
        maxIndex = max(maxIndex, index);
    }

    nextIndex = maxIndex + 1;
    file.close();
}

void writeToFile(const string& filename) {
    ofstream file(filename);
    if (!file.is_open()) {
        cout << "Ошибка открытия файла для записи.\n";
        return;
    }

    for (const auto& client : clients) {
        file << client.index << " " << client.name << endl;
    }

    file.close();
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    int n, index;
    string t, filename;
    Menu();
    while (choice != 9) {
        switch (choice) {
        case 1:
            system("cls");
            cout << "Введите ФИО клиента: ";
            cin.ignore();
            getline(cin, t);
            clients.push_back({ nextIndex, t });
            cout << "Данные введены.\n";
            ++nextIndex;
            system("pause");
            system("cls");
            Menu();
            break;
        case 2:
            system("cls");
            cout << "Введите имя товара: ";
            cin.ignore();
            getline(cin, t);
            cout << "Сумма под залог - 650BYN\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 3:
            system("cls");
            cout << "Список активных залогов:\n 1. Телефон\n 2. Часы\n 3. Золотая цепочка\n 4. Брошь\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 4:
            system("cls");
            cout << "Введите номер товара, который необходимо продать: ";
            cin >> n;
            cout << "Сумма залога: 330BYN\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 5:
            system("cls");
            if (!clients.empty()) {
                cout << "Список клиентов:\n";
                for (const auto& client : clients) {
                    cout << client.index << ". " << client.name << endl;
                }
            }
            else { cout << "Список клиентов пуст.\n"; }
            system("pause");
            system("cls");
            Menu();
            break;
        case 6:
            system("cls");
            if (!clients.empty())
            {
                do {
                    cout << "Введите номер клиента (0 - Выход): \n";
                    cin >> index;
                    if (index == 0) { break; }
                    Client* foundClient = findClientByIndex(index);
                    if (foundClient != nullptr) {
                        cout << "Найденный клиент: " << foundClient->name << endl;
                        break;
                    }
                    else { cout << "Клиент с указанным номером не найден.\n"; }
                } while (true);
            }
            else { cout << "Список клиентов пуст.\n"; }
            system("pause");
            system("cls");
            Menu();
            break;
        case 7:
            system("cls");
            cout << "Введите имя файла для загрузки данных: ";
            cin >> filename;
            readFromFile(filename);
            cout << "Данные загружены из файла.\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 8:
            system("cls");
            cout << "Введите имя файла для сохранения данных: ";
            cin >> filename;
            writeToFile(filename);
            cout << "Данные сохранены в файле.\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 9: exit(1);
        default:
            system("cls");
            cout << "Неверный выбор\n";
            system("pause");
            system("cls");
            Menu();
            break;
        }
    }
}