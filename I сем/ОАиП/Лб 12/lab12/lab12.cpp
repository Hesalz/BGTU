#include <iostream>
using namespace std;

void second() {
    cout << endl << endl << "Задание №2" << endl;
    int m, plus = 0, minus = 0, equal = 0;
    cout << "Введите размер массивов (< 100): ";
    cin >> m;
    if (m > 100 || m <= 0) {
        cout << "Неверное значение";
        return;
    }
    int* C = new int[m];
    int* D = new int[m];
    int* pC = C;
    int* pD = D;
    srand(time(NULL));
    for (int i = 0; i < m; i++) {
        C[i] = rand() % 100;
        D[i] = rand() % 100;
    }
    for (int k = 0; k < m; k++) {
        if (pC[k] > pD[k]) {
            plus++;
        }
        else if (pC[k] < pD[k]) {
            minus++;
        }
        else {
            equal++;
        }
    }
    cout << "Массив C: ";
    for (int i = 0; i < m; i++) {
        cout << pC[i] << " ";
    }
    cout << endl << "Массив D: ";
    for (int i = 0; i < m; i++) {
        cout << pD[i] << " ";
    }
    cout << endl;
    cout << "Количество элементов C[k] > D[k]: " << plus << endl;
    cout << "Количество элементов C[k] < D[k]: " << minus << endl;
    cout << "Количество элементов C[k] = D[k]: " << equal << endl;
    delete[] C;
    delete[] D;
}
    
void main() {
    setlocale(LC_ALL, "rus");
    cout << "Задание №1" << endl;
    int A[10] = {}, B[10] = {}, t, n = 10;
    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        A[i] = rand() % 40 - 10;
        B[i] = rand() % 40 - 20;
    }
    cout << "Введите значение t: ";
    cin >> t;
    int countA = 0, countB = 0;
    int *pA = A;
    int *pB = B;

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
            cout << " " << *(pB+i);
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
    }
    cout << endl << countA << " " << countB;
    second();
}