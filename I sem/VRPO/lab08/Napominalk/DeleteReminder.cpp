#include "ReminderManager.h"
#include <iostream>

void DeleteReminderFunction(ReminderManager& manager) {
    int index;
    std::cout << "Введите индекс для удаления напоминания: ";
    std::cin >> index;

    if (index < 1 || index > manager.getReminders().size()) {
        std::cout << "Неверный индекс напоминания или такого напоминания не существует. Пожалуйста, выберите корректный индекс." << std::endl;
        return;
    }

    manager.deleteReminder(index - 1);
}
