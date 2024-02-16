#include <iostream>
#include <fstream>
#include <string>
#include <Windows.h>

#define MAX_CLIENTS 100
#define MAX_NAME_LENGTH 50

using namespace std;

struct BankClient {
    string fullname;
    string account_type;
    int account_number;
    float balance;
    string last_update;
};

BankClient clients[MAX_CLIENTS];
int num_clients = 0;

void menu(int &cho);
void addClient();
void displayClients();
void deleteClient(int account_number);
void searchClient(int account_number);
void writeToFile();
void readFromFile();

int main() {
    setlocale(LC_ALL, ".1251");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    int choice;
    do {
        menu(choice);
        switch (choice) {
        case 1:
            addClient();
            break;
        case 2:
            displayClients();
            break;
        case 3: {
            int account_number;
            cout << "Введите номер аккаунта для удаления: ";
            cin >> account_number;
            deleteClient(account_number);
            break;
        }
        case 4: {
            int account_number;
            cout << "Введите номер аккаунта для поиска: ";
            cin >> account_number;
            searchClient(account_number);
            break;
        }
        case 5:
            writeToFile();
            break;
        case 6:
            readFromFile();
            break;
        case 7:
            break;
        default:
            cout << "Неверный выбор.\n";
        }
    } while (choice != 7);

    return 0;
}

void menu(int &cho) {
    cout << "\nМеню:\n"
        << "1. Добавить клиента\n"
        << "2. Список клиентов\n"
        << "3. Удалить клиента\n"
        << "4. Найти клиента\n"
        << "5. Сохранить данные\n"
        << "6. Прочитать данные из файла\n"
        << "7. Выход\n"
        << "Введите ваш выбор: ";
    cin >> cho;
}

void addClient() {
    if (num_clients < MAX_CLIENTS) {
        cout << "Введите ФИО клиента:\n";
        cin.ignore();
        getline(cin, clients[num_clients].fullname);
        cout << "Тип счёта: ";
        getline(cin, clients[num_clients].account_type);
        cout << "Номер счёта: ";
        cin >> clients[num_clients].account_number;
        cout << "Баланс: ";
        cin >> clients[num_clients].balance;
        cout << "Последнее изменение: ";
        cin.ignore();
        getline(cin, clients[num_clients].last_update);
        num_clients++;
        cout << "Клиент успешно добавлен.\n";
    }
    else {
        cout << "Достигнут максимум клиентов.\n";
    }
}

void displayClients() {
    if (num_clients == 0) {
        cout << "Список клиентов пуст.\n";
    }
    else {
        cout << "Список клиентов:\n";
        for (int i = 0; i < num_clients; i++) {
            cout << "Клиент - [" << i + 1 << "]" << endl;
            cout << "ФИО: " << clients[i].fullname << endl;
            cout << "Тип счёта: " << clients[i].account_type << endl;
            cout << "Номер счёта: " << clients[i].account_number << endl;
            cout << "Баланс: " << clients[i].balance << endl;
            cout << "Последнее изменение: " << clients[i].last_update << endl << endl;
        }
    }
}

void deleteClient(int account_number) {
    int found = 0;
    for (int i = 0; i < num_clients; i++) {
        if (clients[i].account_number == account_number) {
            found = 1;
            for (int j = i; j < num_clients - 1; j++) {
                clients[j] = clients[j + 1];
            }
            num_clients--;
            cout << "Клиент с номером счёта " << account_number << " успешно удалён.\n";
            break;
        }
    }
    if (!found) {
        cout << "Клиент с номером счёта " << account_number << " не найден.\n";
    }
}

void searchClient(int account_number) {
    int found = 0;
    for (int i = 0; i < num_clients; i++) {
        if (clients[i].account_number == account_number) {
            found = 1;
            cout << "Найденный клиент:\n";
            cout << "ФИО: " << clients[i].fullname << endl;
            cout << "Тип счёта: " << clients[i].account_type << endl;
            cout << "Номер счёта: " << clients[i].account_number << endl;
            cout << "Баланс: " << clients[i].balance << endl;
            cout << "Последнее изменение: " << clients[i].last_update << endl;
            break;
        }
    }
    if (!found) {
        cout << "Клиент с номером счёта " << account_number << " не найден.\n";
    }
}

void writeToFile() {
    ofstream file("input.txt");
    if (!file.is_open()) {
        cout << "Ошибка открытия файла.\n";
        return;
    }
    for (int i = 0; i < num_clients; i++) {
        file << clients[i].fullname << endl;
        file << clients[i].account_type << endl;
        file << clients[i].account_number << endl;
        file << clients[i].balance << endl;
        file << clients[i].last_update << endl;
    }
    file.close();
    cout << "Данные успешно записаны в файл.\n";
}

void readFromFile() {
    ifstream file("input.txt");
    if (!file.is_open()) {
        cout << "Ошибка открытия файла.\n";
        return;
    }
    num_clients = 0;
    string line;
    while (getline(file, line)) {
        clients[num_clients].fullname = line;
        getline(file, clients[num_clients].account_type);
        file >> clients[num_clients].account_number;
        file >> clients[num_clients].balance;
        file.ignore();
        getline(file, clients[num_clients].last_update);
        num_clients++;
    }
    file.close();
    cout << "Данные успешно записаны в файл.\n";
}