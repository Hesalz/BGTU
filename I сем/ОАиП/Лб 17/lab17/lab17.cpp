#include <iostream>
using namespace std;

void printArray(int* arr, int size) {
    for (int i = 0; i < size; ++i) {
        cout << arr[i] << " ";
    }
    cout << endl;
}

void printMatrix(int** matrix, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

void findSumBetweenZeros(int* arr, int size, int& result) {
    int firstZeroIndex = -1;
    int lastZeroIndex = -1;

    for (int i = 0; i < size; ++i) {
        if (arr[i] == 0) {
            if (firstZeroIndex == -1) {
                firstZeroIndex = i;
            }
            else {
                lastZeroIndex = i;
            }
        }
    }

    if (firstZeroIndex != -1 && lastZeroIndex != -1) {
        result = 0;
        for (int i = firstZeroIndex + 1; i < lastZeroIndex; ++i) {
            result += arr[i];
        }
    }
    else {
        result = 0;
        cout << "Нет пары нулевых элементов." << endl;
    }
}

bool findPositiveRow(int** matrix, int rows, int cols, int& sum) {
    for (int i = 0; i < rows; ++i) {
        bool allPositive = true;
        int rowSum = 0;

        for (int j = 0; j < cols; ++j) {
            if (matrix[i][j] <= 0) {
                allPositive = false;
                break;
            }

            rowSum += matrix[i][j];
        }

        if (allPositive) {
            sum = rowSum;
            return true;
        }
    }

    return false;
}

int main() {
    setlocale(LC_ALL, "rus");
    srand(time(NULL));
    int arrSize;
    cout << "Введите размер массива: ";
    cin >> arrSize;

    int* arr = new int[arrSize];
    for (int i = 0; i < arrSize; ++i) {
        arr[i] = (rand() % 10) - 3;
    }

    cout << "Массив: ";
    printArray(arr, arrSize);

    int sumBetweenZeros;
    findSumBetweenZeros(arr, arrSize, sumBetweenZeros);
    cout << "Сумма элементов между первым и последним нулевыми элементами: " << sumBetweenZeros << endl;

    int rows, cols;
    cout << "Введите количество строк матрицы: ";
    cin >> rows;
    cout << "Введите количество столбцов матрицы: ";
    cin >> cols;

    int** matrix = new int* [rows];
    for (int i = 0; i < rows; ++i) {
        matrix[i] = new int[cols];
        for (int j = 0; j < cols; ++j) {
            matrix[i][j] = (rand() % 21) - 5;
        }
    }

    cout << "Матрица:" << endl;
    printMatrix(matrix, rows, cols);

    int sumPositiveRow;
    if (findPositiveRow(matrix, rows, cols, sumPositiveRow)) {
        cout << "Сумма положительных элементов строки: " << sumPositiveRow << endl;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                matrix[i][j] -= sumPositiveRow;
            }
        }

        cout << "Матрица после уменьшения на сумму положительных элементов строки:" << endl;
        printMatrix(matrix, rows, cols);
    }
    else {
        cout << "Нет строки с положительными элементами." << endl;
    }

    delete[] arr;
    for (int i = 0; i < rows; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;

    return 0;
}
