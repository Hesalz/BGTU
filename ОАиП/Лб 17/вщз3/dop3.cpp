#include <iostream>
using namespace std;

// Вариант 7
void createAndFillArray(int* arr, int size) {
    srand(time(NULL));

    for (int i = 0; i < size; ++i) {
        arr[i] = rand() % 20; 
    }
}

void createAndFillMatrix(int** matrix, int rows, int cols) {
    srand(time(NULL)); 

    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            matrix[i][j] = rand() % 200 - 100;
        }
    }
}

int calculateProduct(const int* arr, int startIdx, int endIdx) {
    int product = 1;

    for (int i = startIdx; i <= endIdx; ++i) {
        product *= arr[i];
    }

    return product;
}

double processMatrix(int** matrix, int rows, int cols) {
    bool foundNegativeColumn = false;
    double negativeColumnAverage = 0.0;

    for (int j = 0; j < cols; ++j) {
        bool allNegative = true;
        double columnSum = 0.0;
        int negativeCount = 0;

        for (int i = 0; i < rows; ++i) {
            if (matrix[i][j] >= 0) {
                allNegative = false;
                break;
            }
            else {
                columnSum += matrix[i][j];
                negativeCount++;
            }
        }

        if (allNegative) {
            foundNegativeColumn = true;
            negativeColumnAverage = columnSum / negativeCount;
            break;
        }
    }

    if (foundNegativeColumn) {
        cout << "Есть столбец, все элементы которого отрицательны." << endl;
        cout << "Среднее арифметическое отрицательных элементов столбца: " << negativeColumnAverage << endl;
    }
    else {
        cout << "Нет столбца, все элементы которого отрицательны." << endl;
    }

    return negativeColumnAverage;
}

void subtractFromMatrix(int** matrix, int rows, int cols, double value) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            matrix[i][j] -= abs(value);
        }
    }
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

    int product = calculateProduct(array, 2, 7);
    cout << "Произведение элементов массива с индексами от 2 до 7: " << product << endl;

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

    double negativeColumnAverage = processMatrix(matrix, matrixRows, matrixCols);
    subtractFromMatrix(matrix, matrixRows, matrixCols, negativeColumnAverage);
    cout << "Полученная матрица после вычитания значения:" << endl;
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
