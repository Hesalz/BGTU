#include <iostream>
#include <windows.h>
#include "ReminderManager.h"

void AddReminderFunction(ReminderManager& manager);
void EditReminderFunction(ReminderManager& manager);
void DeleteReminderFunction(ReminderManager& manager);
void ShowRemindersFunction(const ReminderManager& manager);

int main() {
    setlocale(LC_ALL, "Rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    ReminderManager manager;

    manager.loadFromFile("reminders.txt");

    int choice;
    do {
        std::cout << "1. Добавить напоминание\n";
        std::cout << "2. Редактировать напоминание\n";
        std::cout << "3. Удалить напоминание\n";
        std::cout << "4. Показать все напоминания\n";
        std::cout << "0. Выход\n";
        std::cout << "Введите свой выбор: ";
        std::cin >> choice;

        switch (choice) {
        case 1:
            AddReminderFunction(manager);
            break;
        case 2:
            EditReminderFunction(manager);
            break;
        case 3:
            DeleteReminderFunction(manager);
            break;
        case 4:
            ShowRemindersFunction(manager);
            break;
        case 0:
            std::cout << "Программа завершена." << std::endl;
            break;
        default:
            std::cout << "Неправильный выбор. Пожалуйста, выберите существующий вариант." << std::endl;
        }

    } while (choice != 0);

    manager.saveToFile("reminders.txt");

    return 0;
}
