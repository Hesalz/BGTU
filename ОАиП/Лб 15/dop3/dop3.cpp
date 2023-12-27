#include <iostream>
using namespace std;

//Вариант 1
void second() {
    cout << endl << "Задание №2" << endl;
    int n, m;
    cout << "Введите количество строк и столбцов матрицы: ";
    cin >> n >> m;

    if (n <= 0 || m <= 0) {
        cout << "Некорректные размеры матрицы." << endl;
        return;
    }

    int** matrix = new int* [n];
    for (int i = 0; i < n; ++i) {
        matrix[i] = new int[m];
    }

    srand(time(NULL));
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            matrix[i][j] = (rand() % 10) - 3;
        }
    }

    cout << "Сгенерированная матрица:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }

    int positive_column = -1;
    for (int j = 0; j < m; ++j) {
        bool all_positive = true;
        for (int i = 0; i < n; ++i) {
            if (matrix[i][j] <= 0) {
                all_positive = false;
                break;
            }
        }
        if (all_positive) {
            positive_column = j;
            break;
        }
    }

    if (positive_column > 0) {
        for (int i = 0; i < n; ++i) {
            matrix[i][positive_column - 1] = -matrix[i][positive_column - 1];
        }
        cout << "Первый столбец с положительными элементами найден. Знаки элементов предыдущего столбца изменены." << endl;
    }
    else {
        cout << "В матрице нет столбца, все элементы которого положительны." << endl;
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

    if (arr == NULL) {
        printf("Ошибка выделения памяти.\n");
        return;
    }

    srand(time(NULL));
    for (int i = 0; i < n; ++i) {
        arr[i] = (rand() % 100 - 50) / 10.0; 
    }

    printf("Сгенерированный массив:\n");
    for (int i = 0; i < n; ++i) {
        printf("arr[%d]: %.2lf\n", i, arr[i]);
    }

    int count_negative = 0;
    for (int i = 0; i < n; ++i) {
        if (arr[i] < 0) {
            count_negative++;
        }
    }

    double min_absolute_value = arr[0];
    int min_absolute_index = 0;

    for (int i = 1; i < n; ++i) {
        if (abs(arr[i]) < abs(min_absolute_value)) {
            min_absolute_value = arr[i];
            min_absolute_index = i;
        }
    }

    double sum_after_min_absolute = 0.0;
    for (int i = min_absolute_index + 1; i < n; ++i) {
        sum_after_min_absolute += fabs(arr[i]);
    }

    printf("Количество отрицательных элементов: %d\n", count_negative);
    printf("Сумма модулей элементов после минимального по модулю элемента: %.2lf\n", sum_after_min_absolute);
    free(arr);

    second();
}
