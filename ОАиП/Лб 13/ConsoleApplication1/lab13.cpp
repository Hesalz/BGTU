#include <iostream>
#include <cstring>
using namespace std;
void second() {
    cout << endl << endl << "Second exercize" << endl;
    const char* inputString = "Текст с числом: +123, а также еще число: -456.";

    const char* currentChar = inputString;
    char numberBuffer[100];

    while (*currentChar) {
        if (*currentChar == ' ' || *currentChar == ',' || *currentChar == '.') {
            currentChar++;
        }
        else if (*currentChar == '+' || *currentChar == '-') {
            numberBuffer[0] = *currentChar;
            numberBuffer[1] = '\0';
            currentChar++;
            char* numberPtr = numberBuffer + 1;
            while (*currentChar >= '0' && *currentChar <= '9') {
                *numberPtr = *currentChar;
                numberPtr++;
                currentChar++;
            }
            *numberPtr = '\0';
            cout << "Найдено число: " << numberBuffer << endl;
        }
        else if (*currentChar >= '0' && *currentChar <= '9') {
            char* numberPtr = numberBuffer;
            while (*currentChar >= '0' && *currentChar <= '9') {
                *numberPtr = *currentChar;
                numberPtr++;
                currentChar++;
            }
            *numberPtr = '\0';
            cout << "Найдено число: " << numberBuffer << endl;
        }
        else {
            currentChar++;
        }
    }
}
void main() {
    setlocale(LC_ALL, "rus");
    cout << "First exercize" << endl;
    char str[] = "This is a program for *labaratory job* number thirteen.";
    int len = strlen(str);
    int first_star = -1, second_star = -1;

    for (int i = 0; i < len; i++) {
        if (str[i] == '*') {
            if (first_star == -1) {
                first_star = i;
            }
            else {
                second_star = i;
                break;
            }
        }
    }

    if (first_star != -1 && second_star != -1) {
        for (int i = first_star + 1; i < second_star; i++) {
            cout << str[i];
        }
    }
    else {
        cout << "Не найдено двух символов '*'";
    }
    second();
}
