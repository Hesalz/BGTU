#include <iostream>
#include <stdio.h>

using namespace std;

    int main() {
        setlocale(LC_ALL, "rus");
        int a, b;

        printf_s("������� �������� a:");
        scanf_s("%d", &a);

        printf_s("������� �������� b:");
        scanf_s("%d", &b);

        a = a + b;
        b = a - b;
        a = a - b;

        printf_s("����� ������ ����������:\n");
        printf_s("a = %d\n", a);
        printf_s("b = %d\n", b);

        return 0;
    }