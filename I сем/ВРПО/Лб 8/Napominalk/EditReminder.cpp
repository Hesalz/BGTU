#include "ReminderManager.h"
#include <iostream>
#include <ctime>
#include <iomanip>

void EditReminderFunction(ReminderManager& manager) {
    int index;
    std::cout << "�������� ������ ����������� ��� ���������: ";
    std::cin >> index;

    if (index < 1 || index > manager.getReminders().size()) {
        std::cout << "�������� ������ ����������� ��� ������ ����������� �� ����������. ����������, �������� ���������� ������." << std::endl;
        return;
    }

    Reminder reminder;
    std::cout << "������� ���� � ����� (����-��-�� ��:��): ";
    if (!(std::cin >> std::get_time(&reminder.datetime, "%Y-%m-%d %H:%M"))) {
        std::cout << "�������� ����. ������� ���� � ����� � ���������� �������." << std::endl;
        std::cin.clear();
        std::cin.ignore(INT_MAX, '\n');
        return;
    }
    std::cout << "������� ����� ����� �����������: ";
    std::cin.ignore();
    std::getline(std::cin, reminder.text);
    manager.editReminder(index - 1, reminder);
}
