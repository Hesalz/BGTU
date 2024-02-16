#include "ReminderManager.h"
#include <iostream>

void DeleteReminderFunction(ReminderManager& manager) {
    int index;
    std::cout << "������� ������ ��� �������� �����������: ";
    std::cin >> index;

    if (index < 1 || index > manager.getReminders().size()) {
        std::cout << "�������� ������ ����������� ��� ������ ����������� �� ����������. ����������, �������� ���������� ������." << std::endl;
        return;
    }

    manager.deleteReminder(index - 1);
}
