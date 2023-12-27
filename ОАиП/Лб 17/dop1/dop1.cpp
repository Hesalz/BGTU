#include <iostream>
using namespace std;

//Вариант 4
void createAndFillArray(int* arr, int size) {
    srand(time(NULL));

    for (int i = 0; i < size; ++i) {
        arr[i] = rand() % 200 + 1;
    }
}

void createAndFillMatrix(int** matrix, int rows, int cols) {
    srand(time(nullptr) + 1);

    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            matrix[i][j] = rand() % 200 - 100;
        }
    }
}

int calculateOddSum(const int* arr, int size) {
    int sum = 0;

    for (int i = 0; i < size; ++i) {
        if (arr[i] % 2 != 0) {
            sum += arr[i];
        }
    }

    return sum;
}

bool processMatrix(int** matrix, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        bool allPositive = true;

        for (int j = 0; j < cols; ++j) {
            if (matrix[i][j] <= 0) {
                allPositive = false;
                break;
            }
        }

        if (allPositive) {
            cout << "Найдена строка с положительными элементами: " << i + 1 << endl;

            if (i > 0) {
                for (int k = 0; k < cols; ++k) {
                    matrix[i - 1][k] = -matrix[i - 1][k];
                }
            }

            return true;
        }
    }

    cout << "Нет строк с положительными элементами." << endl;
    return false;
}

int main() {
    setlocale(LC_ALL, "rus");

    const int arraySize = 15;
    int* array = new int[arraySize];

    createAndFillArray(array, arraySize);

    cout << "Исходный массив: ";
    for (int i = 0; i < arraySize; ++i) {
        cout << array[i] << " ";
    }
    cout << endl;

    int oddSum = calculateOddSum(array, arraySize);
    cout << "Сумма нечетных элементов массива: " << oddSum << endl;
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
