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
        << "9. Удалить клиента\n"
        << "10. Изменить данные клиента\n"
        << "11. Полностью очистить базу данных\n"
        << "12. Сортировка по имени.\n"
        << "13. Сортировка по индексу (пузырьковая).\n"
        << "14. Сортировка по индексу (выбором).\n"
        << "15. Найти клиента по (части) фамилии.\n"
        << "16. Выход\n"
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

void deleteClient(int index) {
    auto it = clients.begin();
    while (it != clients.end()) {
        if (it->index == index) {
            it = clients.erase(it);
            cout << "Клиент удален.\n";
            for (auto& client : clients) {
                if (client.index > index) {
                    client.index--;
                }
            }
            nextIndex--;
            return;
        }
        else {
            ++it;
        }
    }
    cout << "Клиент с указанным номером не найден.\n";
}

void editClient(int index) {
    Client* client = findClientByIndex(index);
    if (client != nullptr) {
        cout << "Введите новое имя для клиента: ";
        cin.ignore();
        getline(cin, client->name);
        cout << "Данные клиента изменены.\n";
    }
    else {
        cout << "Клиент с указанным номером не найден.\n";
    }
}

void bubbleSortByName() {
    int n = clients.size();
    for (int i = 0; i < n; i++) {
        for (int j = i + 1; j < n; j++) {
            if (clients[i].name > clients[j].name) {
                swap(clients[i], clients[j]);
            }
        }
    }
}

void bubbleSortByIndex() {
    int n = clients.size();
    for (int i = 0; i < n; i++) {
        for (int j = i + 1; j < n; j++) {
            if (clients[i].index > clients[j].index) {
                swap(clients[i], clients[j]);
            }
        }
    }
}

void SelectionSort()
{
    int n = clients.size();
    int smallest;

    for (int i = 0; i < n; i++) {
        smallest = i;
        for (int j = i + 1; j < n; j++) {
            if (clients[j].index < clients[smallest].index)
                smallest = j;
        }
        swap(clients[smallest].index, clients[i].index);
    }
}

void searchClientByLastName(const string& lastName) {
    bool found = false;
    for (const auto& client : clients) {
        if (client.name.find(lastName) != string::npos) {
            cout << "Найден клиент: " << client.index << ". " << client.name << endl;
            found = true;
        }
    }
    if (!found) {
        cout << "Клиент с фамилией \"" << lastName << "\" не найден.\n";
    }
}



int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    int n, index;
    string t, filename;
    Menu();
    while (choice != 16) {
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
        case 9:
            system("cls");
            cout << "Введите номер клиента для удаления: ";
            cin >> n;
            deleteClient(n);
            system("pause");
            system("cls");
            Menu();
            break;
        case 10:
            system("cls");
            cout << "Введите номер клиента для изменения данных: ";
            cin >> n;
            editClient(n);
            system("pause");
            system("cls");
            Menu();
            break;
        case 11:
            system("cls");
            clients.clear();
            nextIndex = 1;
            cout << "База данных очищена.\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 12:
            system("cls");
            bubbleSortByName();
            cout << "Отсортировано по именам (пузырьковая сортировка).\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 13:
            system("cls");
            bubbleSortByIndex();
            cout << "Отсортировано по индексу (пузырьковая сортировка).\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 14:
            system("cls");
            SelectionSort();
            cout << "Отсортировано по индексу (сортировка выбором).\n";
            system("pause");
            system("cls");
            Menu();
            break;
        case 15:
            system("cls");
            cout << "Введите фамилию клиента или её часть для поиска: ";
            cin >> t;
            searchClientByLastName(t);
            system("pause");
            system("cls");
            Menu();
            break;

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