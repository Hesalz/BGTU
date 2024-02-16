#include <iostream>
using namespace std;

//Вариант 6
void createAndFillArray(int* arr, int size) {
    srand(time(nullptr));

    for (int i = 0; i < size; ++i) {
        arr[i] = rand() % 100 - 50;
    }
}

void createAndFillMatrix(int** matrix, int matrixRows, int matrixCols) {
    srand(time(nullptr) + 1);

    for (int i = 0; i < matrixRows; ++i) {
        for (int j = 0; j < matrixCols; ++j) {
            matrix[i][j] = (rand() % 11) - 3;
        }
    }
}

void findMinMaxSum(const int* arr, int size, int& minSum, int& maxSum) {
    if (size <= 0) {
        cout << "Массив пуст." << endl;
        return;
    }

    minSum = arr[0];
    maxSum = arr[0];

    for (int i = 1; i < size; ++i) {
        if (arr[i] < minSum) {
            minSum = arr[i];
        }
        else if (arr[i] > maxSum) {
            maxSum = arr[i];
        }
    }

    cout << "Сумма минимального и максимального элементов массива: " << minSum + maxSum << endl;
}

void processMatrix(int** matrix, int rows, int cols) {
    bool allRowsContainZero = true;

    for (int i = 0; i < rows; ++i) {
        bool hasZero = false;

        for (int j = 0; j < cols; ++j) {
            if (matrix[i][j] == 0) {
                hasZero = true;
                break;
            }
        }

        if (!hasZero) {
            allRowsContainZero = false;
            break;
        }
    }

    if (!allRowsContainZero) {
        cout << "Не все строки матрицы содержат хотя бы один нулевой элемент." << endl;

        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                if (matrix[i][j] < 0) {
                    matrix[i][j] = 0;
                }
            }
        }

        cout << "Значения отрицательных элементов заменены на нули." << endl;
    }
    else {
        cout << "Все строки матрицы содержат хотя бы один нулевой элемент." << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    const int arraySize = 10;
    int* array = new int[arraySize];

    createAndFillArray(array, arraySize);
    cout << "Исходный массив: ";
    for (int i = 0; i < arraySize; ++i) {
        cout << array[i] << " ";
    }
    cout << endl;
    int minSum, maxSum;
    findMinMaxSum(array, arraySize, minSum, maxSum);
    delete[] array;

    const int matrixRows = 4;
    const int matrixCols = 4;
    int** matrix = new int* [matrixRows];
    for (int i = 0; i < matrixRows; ++i) {
        matrix[i] = new int[matrixCols];
    }


    createAndFillMatrix(matrix, matrixRows, matrixCols);
    cout << "Исходная матрица:" << endl;
    for (int i = 0; i < matrixRows; ++i) {
        for (int j = 0; j < matrixCols; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }

    processMatrix(matrix, matrixRows, matrixCols);

    cout << "Полученная матрица:" << endl;
    for (int i = 0; i < matrixRows; ++i) {
        for (int j = 0; j < matrixCols; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }
    for (int i = 0; i < matrixRows; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;

    return 0;
}
