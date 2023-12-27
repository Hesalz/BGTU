#include <iostream>
using namespace std; // Вариант 10

void second() {
    cout << endl << "Задание №2" << endl;
    int n;
    cout << "Введите размер массивов (< 100): ";
    cin >> n;
    if (n > 100 || n <= 0) {
        cout << "Неверное значение.";
        return;
    }
    int* A = new int[n];
    int* B = new int[n];
    int* C = new int[n];
    int* pA = A;
    int* pB = B;
    int* pC = C;

    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        A[i] = rand() % 100;
        B[i] = rand() % 100;
    }
    for (int i = 0; i < n; i++) {
        *pC = *pA + *pB;
        pA++;
        pB++;
        pC++;
    }

    cout << "Массив A: ";
    for (int i = 0; i < n; i++) {
        cout << A[i] << " ";
    }
    cout << endl;
    cout << "Массив B: ";
    for (int i = 0; i < n; i++) {
        cout << B[i] << " ";
    }
    cout << endl;
    cout << "Массив C: ";
    for (int i = 0; i < n; i++) {
        cout << C[i] << " ";
    }
    delete[] A;
    delete[] B;
    delete[] C;
}
void main() {
    setlocale(LC_ALL, "rus");
    cout << "Задание №1" << endl;
    int n, m, min = 101;
    bool isInB = false;
    cout << "Введите размер массива A (< 100): ";
    cin >> n;
    if (n > 100 || n <= 0) {
        cout << "Неверное значение.";
        return;
    }
    cout << "Введите размер массива B (< 100): ";
    cin >> m;
    if (m > 100 || m <= 0) {
        cout << "Неверное значение.";
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
    for (int i = 0; i < m; i++) {
        B[i] = rand() % 100;
    }


    for (int i = 0; i < n; i++) {
        isInB = false;
        pB = B;

        for (int j = 0; j < m; j++) {
            if (*pA == *pB) {
                isInB = true;
                break;
            }
            pB++;
        }

        if (!isInB && *pA < min) {
            min = *pA;
        }

        pA++;
    }
    cout << "Массив A: ";
    for (int i = 0; i < n;i++) {
        cout << A[i] << " ";
    }
    cout << endl << "Массив B: ";
    for (int i = 0; i < m;i++) {
        cout << B[i] << " ";
    }
        cout << endl << "Наименьшее число в массиве A, которое не входит в массив B: " << min << endl;
        delete[] A;
        delete[] B;
    second();
}
