#include <iostream>
#include <vector>
#include <algorithm>
#include <Windows.h>
#include <string>

using namespace std; // Вариант 2

enum Gender { Male, Female };

struct DateOfBirth {
    unsigned int day : 5;
    unsigned int month : 4;
    unsigned int year : 12;
};

struct Citizen {
    string fullName;
    DateOfBirth dob;
    string address;
    Gender gender;
};

vector<Citizen> citizens;

void addCitizen() {
    Citizen citizen;
    cout << "Введите Ф.И.О.: ";
    getline(cin, citizen.fullName);
    cout << "Введите день рождения (дд): ";
    int day, month, year;
    cin >> day;
    citizen.dob.day = day;
    cout << "Введите месяц рождения (мм): ";
    cin >> month;
    citizen.dob.month = month;
    cout << "Введите год рождения (гггг): ";
    cin >> year; 
    citizen.dob.year = year;
    cin.ignore();
    cout << "Введите адрес: ";
    getline(cin, citizen.address);
    cout << "Выберите пол (0 - м, 1 - ж): ";
    int genderChoice;
    cin >> genderChoice;
    citizen.gender = static_cast<Gender>(genderChoice);
    citizens.push_back(citizen);
    cout << "Горожанин успешно добавлен." << endl;
}

void displayCitizens() {
    if (citizens.empty()) {
        cout << "Список горожан пуст." << endl;
        return;
    }
    cout << "Список горожан:" << endl;
    for (const auto& citizen : citizens) {
        cout << "Ф.И.О.: " << citizen.fullName << endl;
        cout << "Дата рождения: " << citizen.dob.day << "." << citizen.dob.month << "." << citizen.dob.year << endl;
        cout << "Адрес: " << citizen.address << endl;
        cout << "Пол: " << (citizen.gender == Male ? "мужской" : "женский") << endl;
        cout << endl;
    }
}

void deleteCitizenByYearOfBirth(int year) {
    auto it = remove_if(citizens.begin(), citizens.end(), [year](const Citizen& citizen) {
        return citizen.dob.year == year;
        });
    citizens.erase(it, citizens.end());
    cout << "Горожане, родившиеся в " << year << " году, удалены." << endl;
}

void searchCitizenByYearOfBirth(int year) {
    bool found = false;
    cout << "Результаты поиска горожан, родившихся в " << year << " году:" << endl;
    for (const auto& citizen : citizens) {
        if (citizen.dob.year == year) {
            found = true;
            cout << "Ф.И.О.: " << citizen.fullName << endl;
            cout << "Дата рождения: " << citizen.dob.day << "." << citizen.dob.month << "." << citizen.dob.year << endl;
            cout << "Адрес: " << citizen.address << endl;
            cout << "Пол: " << (citizen.gender == Male ? "мужской" : "женский") << endl;
            cout << endl;
        }
    }
    if (!found) {
        cout << "Горожане, родившиеся в " << year << " году, не найдены." << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    int choice;
    int year;

    do {
        cout << "\nМеню:" << endl;
        cout << "1. Добавить горожанина" << endl;
        cout << "2. Вывести список всех горожан" << endl;
        cout << "3. Удалить горожан по году рождения" << endl;
        cout << "4. Найти горожан по году рождения" << endl;
        cout << "0. Выйти из программы" << endl;
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 1:
            cin.ignore();
            addCitizen();
            break;
        case 2:
            displayCitizens();
            break;
        case 3:
            cout << "Введите год рождения для удаления горожан: ";
            cin >> year;
            deleteCitizenByYearOfBirth(year);
            break;
        case 4:
            cout << "Введите год рождения для поиска горожан: ";
            cin >> year;
            searchCitizenByYearOfBirth(year);
            break;
        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте еще раз." << endl;
        }
    } while (choice != 0);

    return 0;
}
