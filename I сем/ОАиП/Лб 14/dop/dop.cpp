#include <iostream>
#include <ctime>

using namespace std;

void third() { // 3
    cout << endl << "Задание №3" << endl;
    const int n = 4;
    double matrix[n][n];

    srand(time(0));
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            matrix[i][j] = static_cast<double>(rand()) / 1000;
        }
    }
    cout << "Исходная матрица:" << endl;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cout << matrix[i][j] << ' ';
        }
        cout << endl;
    }

    for (int i = 0; i < n; i++) {
        double maxElem = matrix[i][i];
        int maxRow = i;
        int maxCol = i;

        for (int j = i; j < n; j++) {
            for (int k = (j == i ? i + 1 : 0); k < n; k++) {
                if (matrix[j][k] > maxElem) {
                    maxElem = matrix[j][k];
                    maxRow = j;
                    maxCol = k;
                }
            }
        }
        if (maxRow != i || maxCol != i) {
            swap(matrix[i][i], matrix[maxRow][maxCol]);
        }
    }

    cout << "Результирующая матрица:" << endl;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cout << matrix[i][j] << ' ';
        }
        cout << endl;
    }

}

void second() { // 2
    cout << endl << "Задание №2" << endl;
    int n;
    cout << "Введите порядок латинского квадрата (n): ";
    cin >> n;

    int** latinSquare = new int* [n];
    for (int i = 0; i < n; i++) {
        latinSquare[i] = new int[n];
    }

    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            latinSquare[i][j] = (i + j) % n + 1;
        }
    }

    cout << "Латинский квадрат порядка " << n << ":" << endl;
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            cout << latinSquare[i][j] << ' ';
        }
        cout << endl;
    }

}

void main() { // 1
    setlocale(LC_ALL, "rus");
    cout << "Задание №1" << endl;
    const int n = 2;
    const int size = 2 * n;
    int matrix[size][size];

    srand(time(0));
    for (int i = 0; i < size; i++) {
        for (int j = 0; j < size; j++) {
            matrix[i][j] = rand() % 21 - 10;
        }
    }

    cout << "Исходная матрица:" << endl;
    for (int i = 0; i < size; i++) {
        for (int j = 0; j < size; j++) {
            cout << matrix[i][j] << ' ';
        }
        cout << endl;
    }

    int result[size][size];
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            for (int k = 0; k < n; k++) {
                for (int l = 0; l < n; l++) {
                    result[i * n + k][j * n + l] = matrix[k][l];
                }
            }
        }
    }

    cout << "Полученная матрица:" << endl;
    for (int i = 0; i < size; i++) {
        for (int j = 0; j < size; j++) {
            cout << result[i][j] << ' ';
        }
        cout << endl;
    }
    second();
    third();
}
