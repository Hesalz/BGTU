#include <iostream>
#include <cstdarg>

int findMax(int first, ...) {
    va_list args;
    va_start(args, first);

    int max_val = first;

    int current;
    while ((current = va_arg(args, int)) != 0) {
        max_val = (current < max_val) ? current : max_val;
    }

    va_end(args);
    return max_val;
}

int main() {
    std::cout << findMax(555, 5, 1, -2, 3, 7, 0) << std::endl;
    std::cout << findMax(-34, -4445, -2, 0) << std::endl;
    std::cout << findMax(-34, 2, 11, 1, -2, 0) << std::endl;
    return 0;
}
