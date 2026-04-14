#include "ReminderManager.h"
#include <iostream>
#include <ctime>
#include <iomanip>

void EditReminderFunction(ReminderManager& manager) {
    int index;
    std::cout << "Выберите индекс напоминания для изменения: ";
    std::cin >> index;

    if (index < 1 || index > manager.getReminders().size()) {
        std::cout << "Неверный индекс напоминания или такого напоминания не существует. Пожалуйста, выберите корректный индекс." << std::endl;
        return;
    }

    Reminder reminder;
    std::cout << "Введите дату и время (ГГГГ-ММ-ДД ЧЧ:ММ): ";
    if (!(std::cin >> std::get_time(&reminder.datetime, "%Y-%m-%d %H:%M"))) {
        std::cout << "Неверный ввод. Введите дату и время в правильном формате." << std::endl;
        std::cin.clear();
        std::cin.ignore(INT_MAX, '\n');
        return;
    }
    std::cout << "Введите новый текст напоминания: ";
    std::cin.ignore();
    std::getline(std::cin, reminder.text);
    manager.editReminder(index - 1, reminder);
}
