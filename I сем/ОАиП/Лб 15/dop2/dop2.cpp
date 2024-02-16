#include <iostream>
using namespace std;

// Вариант 13
void second() {
    cout << endl << "Задание №2" << endl;
    int n, m;
    cout << "Введите количество строк и столбцов матрицы: ";
    cin >> n >> m;

    if (n <= 0 || m <= 0) {
        cout << "Некорректные размеры матрицы." << endl;;
        return;
    }

    int** matrix = new int* [n];
    for (int i = 0; i < n; ++i) {
        matrix[i] = new int[m];
    }

    srand(time(NULL));
    for (int i = 0; i < 1; ++i) {
        for (int j = 0; j < m; ++j) {
            matrix[i][j] = 0;
        }
    }
    for (int i = 1; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            matrix[i][j] = rand() % 10;
        }
    }

    cout << "Исходная матрица:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }

    int zero_row = -1;
    for (int i = 0; i < n; ++i) {
        bool all_zeros = true;
        for (int j = 0; j < m; ++j) {
            if (matrix[i][j] != 0) {
                all_zeros = false;
                break;
            }
        }
        if (all_zeros) {
            zero_row = i;
            break;
        }
    }

    if (zero_row != -1) {
        for (int j = 0; j < m; ++j) {
            matrix[j][zero_row] = matrix[j][zero_row] / 2;
        }
        cout << "Первая строка с нулевыми элементами найдена. Элементы соответствующего столбца уменьшены вдвое." << endl;
    }
    else {
        cout << "В матрице нет строк, все элементы которых равны нулю." << endl;
    }

    cout << "Измененная матрица:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }

    for (int i = 0; i < n; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;

    return;
}

void main() {
    setlocale(LC_ALL, "rus");
    int n;
    printf("Введите размер массива: ");
    scanf_s("%d", &n);

    if (n <= 0) {
        printf("Некорректный размер массива. Пожалуйста, введите положительное число.\n");
        return;
    }

    double* arr = (double*)malloc(n * sizeof(double));

    srand(time(NULL));
    for (int i = 0; i < n; ++i) {
        arr[i] = ((rand() % 100) / 10.0) - 3; 
    }

    printf("Сгенерированный массив:\n");
    for (int i = 0; i < n; ++i) {
        printf("arr[%d]: %.2lf\n", i, arr[i]);
    }

    double sumodd = 0.0;
    for (int i = 1; i < n; i += 2) {
        sumodd += arr[i];
    }

    int first_negative_index = -1, last_negative_index = -1;
    for (int i = 0; i < n; ++i) {
        if (arr[i] < 0) {
            if (first_negative_index == -1) {
                first_negative_index = i;
            }
            last_negative_index = i;
        }
    }

    double sumbetween = 0.0;
    if (first_negative_index != -1 && last_negative_index != -1) {
        for (int i = first_negative_index + 1; i < last_negative_index; ++i) {
            sumbetween += arr[i];
        }
    }

    printf("Сумма элементов с нечетными номерами: %.2lf\n", sumodd);
    if (first_negative_index == -1 || last_negative_index == -1) {
        printf("В массиве нет отрицательных элементов.\n");
    }
    else {
        printf("Сумма элементов между первым и последним отрицательными элементами: %.2lf\n", sumbetween);
    }
    free(arr);
	second();
}