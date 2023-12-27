#include <iostream>
using namespace std;

void second() {
        cout << endl << "Задание №2" << endl;
        srand(time(NULL));
        int k, f, matrix[4][4];
        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                *(*(matrix + i) + j) = rand() % 20;
                cout << *(*(matrix + i) + j) << " ";
            }
            cout << endl;
        }

        cout << "Введите номер строки (k) для проверки: ";
        cin >> k;
        bool istrue = true;
        int* row = *(matrix + k);

        for (int j = 1; j < 4; j++) {
            if (*(row + j) > *(row + j - 1)) {
                istrue = false;
                break;
            }
        }

        if (istrue) {
            cout << "Все элементы в строке " << k << " упорядочены по убыванию." << endl;
        }
        else {
            cout << "Элементы в строке " << k << " не упорядочены по убыванию." << endl;
        }
}

void main() {
    setlocale(LC_ALL, "rus");
    cout << "Задание #1" << endl;

    srand(time(0));
    int matrix[4][4];
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            matrix[i][j] = rand() % 5;
        }
    }

    int sum = 0;
    cout << "Элементы матрицы:" << endl;
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            cout << matrix[i][j] << ' ';
            sum += matrix[i][j];
        }
        cout << endl;
    }
    if (sum % 2 == 0) {
        cout << "Сумма элементов матрицы чётная. Сумма: " << sum << endl;
    }
    else {
        cout << "Сумма элементов матрицы нечётная. Сумма: " << sum << endl;
    }
    second();
}
