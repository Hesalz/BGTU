#pragma once

#include <string>
#include <ctime>

struct Reminder {
    std::string text;
    tm datetime;
};