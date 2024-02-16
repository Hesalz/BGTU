#pragma once

#include <vector>
#include "Reminder.h"

class ReminderManager {
public:
    void addReminder(const Reminder& reminder);
    void editReminder(int index, const Reminder& reminder);
    void deleteReminder(int index);
    void showReminders() const;
    void saveToFile(const std::string& filename) const;
    void loadFromFile(const std::string& filename);
    size_t getRemindersCount() const;
    const std::vector<Reminder>& getReminders() const;

private:
    std::vector<Reminder> reminders;
};