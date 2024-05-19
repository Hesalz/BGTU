#define _CRT_SECURE_NO_WARNINGS

#include <iostream>
#include <string>
#include <Windows.h>
#include <vector>
#include <ctime>

using namespace std; // Вариант 1

enum class Position {
    MANAGER,
    ENGINEER,
    ACCOUNTANT,
};

struct Employee {
    string fullName;
    string education;
    string specialty;
    string department;
    Position position;
    float salary;
    struct {
        unsigned int day : 5;
        unsigned int month : 4;
        unsigned int year : 12;
    } hireDate;
};

void inputEmployee(Employee& emp) {
    cout << "Введите ФИО: ";
    getline(cin, emp.fullName);
    cout << "Введите образование: ";
    getline(cin, emp.education);
    cout << "Введите специальность: ";
    getline(cin, emp.specialty);
    cout << "Введите подразделение: ";
    getline(cin, emp.department);
    cout << "Выберите должность (0 - Менеджер, 1 - Инженер, 2 - Бухгалтер и т.д.): ";
    int pos;
    cin >> pos;
    emp.position = static_cast<Position>(pos);
    cout << "Введите оклад: ";
    cin >> emp.salary;
    cout << "Введите дату поступления на предприятие (день месяц год): ";
    int day, month, year;
    cin >> day >> month >> year;
    emp.hireDate.day = day;
    emp.hireDate.month = month;
    emp.hireDate.year = year;
}

void outputEmployee(const Employee& emp) {
    cout << "ФИО: " << emp.fullName << endl;
    cout << "Образование: " << emp.education << endl;
    cout << "Специальность: " << emp.specialty << endl;
    cout << "Подразделение: " << emp.department << endl;
    cout << "Должность: ";
    switch (emp.position) {
    case Position::MANAGER:
        cout << "Менеджер" << endl;
        break;
    case Position::ENGINEER:
        cout << "Инженер" << endl;
        break;
    case Position::ACCOUNTANT:
        cout << "Бухгалтер" << endl;
        break;
    }
    cout << "Оклад: " << emp.salary << endl;
    cout << "Дата поступления на предприятие: " << emp.hireDate.day << "." << emp.hireDate.month << "." << emp.hireDate.year << endl;
}

void deleteEmployeeByName(vector<Employee>& employees, const string& name) {
    for (auto it = employees.begin(); it != employees.end(); ++it) {
        if (it->fullName == name) {
            it = employees.erase(it);
            cout << "Сотрудник " << name << " удален" << endl;
            return;
        }
    }
    cout << "Сотрудник " << name << " не найден" << endl;
}

void printEmployeesByExperience(const vector<Employee>& employees, int experienceYears) {
    time_t currentTime = time(nullptr);
    struct tm* currentDate = localtime(&currentTime);

    cout << "Сотрудники с опытом работы " << experienceYears << " и более лет:" << endl;
    for (const auto& emp : employees) {
        int yearsOfWork = currentDate->tm_year + 1900 - emp.hireDate.year;
        if (currentDate->tm_mon < emp.hireDate.month || (currentDate->tm_mon == emp.hireDate.month && currentDate->tm_mday < emp.hireDate.day)) {
            yearsOfWork--;
        }
        if (yearsOfWork >= experienceYears) {
            outputEmployee(emp);
            cout << endl;
        }
    }
}



int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    vector<Employee> employees;
    int choice;

    do {
        cout << "\nМеню:" << endl;
        cout << "1. Добавить сотрудника" << endl;
        cout << "2. Вывести список сотрудников" << endl;
        cout << "3. Удалить сотрудника" << endl;
        cout << "4. Список сотрудников по стажу работы" << endl;
        cout << "0. Выход" << endl;
        cout << "Выберите действие: ";
        cin >> choice;
        cin.ignore();

        switch (choice) {
        case 1:
        {
            Employee emp;
            inputEmployee(emp);
            employees.push_back(emp);
            cout << "Сотрудник добавлен" << endl;
        }
        break;
        case 2:
            if (employees.empty()) {
                cout << "Список сотрудников пуст" << endl;
            }
            else {
                cout << "Список сотрудников:" << endl;
                for (const auto& emp : employees) {
                    outputEmployee(emp);
                    cout << endl;
                }
            }
            break;
        case 3:
        {
            string nameToDelete;
            cout << "Введите ФИО сотрудника для удаления: ";
            getline(cin, nameToDelete);
            deleteEmployeeByName(employees, nameToDelete);
        }
        break;
        case 4:
        {
            int experienceYears;
            cout << "Введите количество лет опыта работы для фильтрации: ";
            cin >> experienceYears;
            printEmployeesByExperience(employees, experienceYears);
        }
        break;

        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте еще раз." << endl;
        }
    } while (choice != 0);

    return 0;
}

