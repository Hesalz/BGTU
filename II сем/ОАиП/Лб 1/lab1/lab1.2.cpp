#include <iostream>

// Рекурсивная функция для нахождения минимального значения из чисел типа int
int fmax(int value) {
    return value;
}

int fmax(int first, int second, int rest...) {
    int min_val = fmax(first, second);

    int* ptr = &rest;
    while (*ptr != -1) {
        min_val = fmax(min_val, *ptr);
        ++ptr;
    }

    return min_val;
}

// Рекурсивная функция для нахождения минимального значения из чисел типа double
double fmax(double value) {
    return value;
}

double fmax(double first, double second, double rest...) {
    double min_val = fmax(first, second);

    double* ptr = &rest;
    while (*ptr != -1.0) {
        min_val = fmax(min_val, *ptr);
        ++ptr;
    }

    return min_val;
}

int main2() {
    // Примеры использования функции с разным количеством параметров
    int int_min = fmax(10, 5, 8, -1);
    double double_min = fmax(3.14, 2.718, 1.618, -1.0);

    // Вывод результатов
    std::cout << "Минимальное из чисел типа int: " << int_min << std::endl;
    std::cout << "Минимальное из чисел типа double: " << double_min << std::endl;

    return 0;
}
