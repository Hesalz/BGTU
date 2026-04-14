#include "ReminderManager.h"
#include <iostream>
#include <ctime>
#include <iomanip>

void AddReminderFunction(ReminderManager& manager) {
    Reminder reminder;
    std::cout << "Введите дату и время (ГГГГ-ММ-ДД ЧЧ:ММ): ";
    if (!(std::cin >> std::get_time(&reminder.datetime, "%Y-%m-%d %H:%M"))) {
        std::cout << "Неверный ввод. Введите дату и время в правильном формате." << std::endl;
        std::cin.clear();
        std::cin.ignore(INT_MAX, '\n');
        return;
    }
    std::cout << "Введите содержимое напоминания: ";
    std::cin.ignore();
    std::getline(std::cin, reminder.text);
    manager.addReminder(reminder);
}
