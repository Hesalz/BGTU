#include "ReminderManager.h"
#include <fstream>
#include <iomanip>
#include <iostream>
#include <string>

void ReminderManager::addReminder(const Reminder& reminder) {
    reminders.push_back(reminder);
}

void ReminderManager::editReminder(int index, const Reminder& reminder) {
    if (index >= 0 && index < reminders.size()) {
        reminders[index] = reminder;
    }
}

void ReminderManager::deleteReminder(int index) {
    if (index >= 0 && index < reminders.size()) {
        reminders.erase(reminders.begin() + index);
    }
}

void ReminderManager::showReminders() const {
    for (size_t i = 0; i < reminders.size(); ++i) {
        std::cout << "[" << i + 1 << "] " << "Date: " << std::put_time(&reminders[i].datetime, "%Y-%m-%d %H:%M") << " - " << reminders[i].text << std::endl;
    }

    if (reminders.size() == 0) {
        std::cout << "У вас ещё нет ни одного напоминания.\n";
    }
}

void ReminderManager::saveToFile(const std::string& filename) const {
    std::ofstream file(filename);
    for (const auto& reminder : reminders) {
        file << std::put_time(&reminder.datetime, "%Y-%m-%d %H:%M") << " " << reminder.text << std::endl;
    }
}

void ReminderManager::loadFromFile(const std::string& filename) {
    reminders.clear();
    std::ifstream file(filename);
    Reminder reminder;
    while (file >> std::get_time(&reminder.datetime, "%Y-%m-%d %H:%M")) {
        std::getline(file >> std::ws, reminder.text);
        reminders.push_back(reminder);
    }
}

size_t ReminderManager::getRemindersCount() const {
    return reminders.size();
}

const std::vector<Reminder>& ReminderManager::getReminders() const {
    return reminders;
}
