#include <iostream>
using namespace std; // Вариант 12

void second(){
    cout << endl << "Задание №2" << endl;
    int A[10] = {}, B[10] = {}, t, n = 10;
    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        A[i] = rand() % 40 - 10;
        B[i] = rand() % 40 - 20;
    }
    cout << "Введите значение k: ";
    cin >> t;
    int countA = 0, countB = 0;
    int* pA = A;
    int* pB = B;

    for (int i = 0; i < n; i++) {
        if (pA[i] < t) countA++;
        if (pB[i] < t) countB++;
    }

    if (countA > countB) {
        cout << "Массив A имеет больше элементов меньше " << t << endl;
        cout << "Элементы массива А: ";
        for (int i = 0; i < n; i++) {
            cout << " " << pA[i];
        }
        cout << endl << "Элементы массива B: ";
        for (int i = 0; i < n; i++) {
            cout << " " << pB[i];
        }
    }
    else if (countB > countA) {
        cout << "Массив B имеет больше элементов меньше " << t << endl;
        cout << "Элементы массива B: ";
        for (int i = 0; i < n; i++) {
            cout << pB[i] << " ";
        }
        cout << endl << "Элементы массива А:";
        for (int i = 0; i < n; i++) {
            cout << " " << pA[i];
        }
    }
    else {
        cout << "Оба массива имеют одинаковое количество элементов меньше " << t << endl;
        cout << "Элементы массива B: ";
        for (int i = 0; i < n; i++) {
            cout << pB[i] << " ";
        }
        cout << endl << "Элементы массива А:";
        for (int i = 0; i < n; i++) {
            cout << " " << pA[i];
        }
    }
}

void main() {
    setlocale(LC_ALL, "rus");
    cout << "Задание №1" << endl;
    int n, plus = 0, minus = 0, equal = 0;
    cout << "Введите размер массивов (< 100): ";
    cin >> n;
    if (n > 100 || n <= 0) {
        cout << "Неверное значение";
        return;
    }
    int* A = new int[n];
    int* B = new int[n];
    int* pA = A;
    int* pB = B;
    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        A[i] = rand() % 100;
        B[i] = rand() % 100;
    }
    for (int k = 0; k < n; k++) {
        if (pA[k] > pB[k]) {
            plus++;
        }
        else if (pA[k] < pB[k]) {
            minus++;
        }
        else {
            equal++;
        }
    }
    cout << "Массив C: ";
    for (int i = 0; i < n; i++) {
        cout << pA[i] << " ";
    }
    cout << endl << "Массив D: ";
    for (int i = 0; i < n; i++) {
        cout << pB[i] << " ";
    }
    cout << endl;
    cout << "Количество элементов A[k] > B[k]: " << plus << endl;
    cout << "Количество элементов A[k] < B[k]: " << minus << endl;
    cout << "Количество элементов A[k] = B[k]: " << equal << endl;
    delete[] A;
    delete[] B;
    second();
}