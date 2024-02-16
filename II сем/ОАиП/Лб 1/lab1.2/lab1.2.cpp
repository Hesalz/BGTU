#include <iostream>

int fmin(int first, int second, int rest...) {
    int min_val = first;

    int* ptr = &second;
    while (*ptr != 0) {
        min_val = (*ptr < min_val) ? *ptr : min_val;
        ptr++;
    }

    return min_val;
}

double fmin(double first, double second, double rest...) {
    double min_val = first;

    double* ptr = &second;
    while (*ptr != 0.0) {
        min_val = (*ptr < min_val) ? *ptr : min_val;
        ptr++;
    }

    return min_val;
}

int main() {
    setlocale(LC_ALL, "rus");
    int int_min = fmin(-10, 5, -3, 8, 0);
    int int2_min = fmin(14, 9, 11, 3, 4, 0);
    double double_min = fmin(3.14, 2.718, -1.618, 0.0);

    std::cout << "Минимальное из чисел типа int: " << int_min << std::endl;
    std::cout << "Минимальное из чисел типа int(2): " << int2_min << std::endl;
    std::cout << "Минимальное из чисел типа double: " << double_min << std::endl;

    return 0;
}
