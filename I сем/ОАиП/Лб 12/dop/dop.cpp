#include <iostream>
using namespace std; // Вариант 4

void second() {
    cout << endl << endl << "Задание №2" << endl;
    int n, result = 0;
    cout << "Введите размер массива Z (< 100): ";
    cin >> n;
    if (n > 100 || n <= 0) {
        cout << "Неверное значение";
        return;
    }
    int* Z = new int[n];
    int* pZ = Z;

    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        Z[i] = rand() % 5;
    }

    bool* search = new bool[100];

    for (int i = 0; i < 100; i++) {
        search[i] = false;
    }

    for (int i = 0; i < n; i++) {
        if (!search[*pZ]) {
            search[*pZ] = true;
            result++;
        }
        pZ++;
    }

    cout << "Массив Z: ";
    for (int i = 0; i < n; i++) {
        cout << Z[i] << " ";
    }
    cout << endl << "Количество различных чисел в массиве Z: " << result << endl;

    delete[] Z;
    delete[] search;
}

void main() {
    setlocale(LC_ALL, "rus");
    cout << "Задание №1" << endl;
    int n, m, max = 0;
    cout << "Введите размер массива A (< 100): ";
    cin >> n;
    if (n > 100 || n <= 0) {
        cout << "Неверное значение";
        return;
    }
    cout << "Введите размер массив B (< 100): ";
    cin >> m;
    if (m > 100 || m <= 0) {
        cout << "Неверное значение";
        return;
    }
    int* A = new int[n];
    int* B = new int[m];
    int* pA = A;
    int* pB = B;
    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        A[i] = rand() % 100;
    }
    for (int i = 0;i < m;i++) {
        B[i] = rand() % 100;
    }
    for (int i = 0; i < n; i++) {
        if (pA[i] > max) max = pA[i];
    }
    bool cont = false;
    for (int i = 0; i < m; i++) {
        if (max == B[i]) {
            cont = true;
            break;
        }
    }
    cout << "Элементы массива A: ";
    for (int i = 0; i < n;i++) {
        cout << pA[i] << " ";
    }
    cout << endl << "Элементы массива B: ";
    for (int i = 0; i < m; i++) {
        cout << pB[i] << " ";
    }
    cout << endl;
    if (cont) {
        cout << "Максимальный элемент массива A (" << max << ") содержится в массиве B.";
    }
    else {
        cout << "Максимальный элемент массива A (" << max << ") не содержится в массиве B.";
    }
    delete[] A;
    delete[] B;
    second();
}