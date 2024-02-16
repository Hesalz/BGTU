#include <iostream>
using namespace std;

// Вариант 4 
void second() {
    cout << endl <<  "Задание №2" << endl;
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
            matrix[i][j] = rand() % 10;
        }
    }

    cout << "Сгенерированная матрица: " << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }

    int count = 0;
    for (int j = 0; j < m; ++j) {
        bool has_zero = false;
        for (int i = 0; i < n; ++i) {
            if (matrix[i][j] == 0) {
                has_zero = true;
                break;
            }
        }
        if (!has_zero) {
            ++count;
        }
    }

    cout << "Количество столбцов, не содержащих ни одного нулевого элемента: " << count << endl;

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
        arr[i] = ((rand() % 100) / 10.0) - 2;
    }

    printf("Сгенерированный массив:\n");
    for (int i = 0; i < n; ++i) {
        printf("arr[%d]: %.2lf\n", i, arr[i]);
    }

    int min_index = 0;
    for (int i = 1; i < n; ++i) {
        if (arr[i] < arr[min_index]) {
            min_index = i;
        }
    }

    int first_negative_index = -1;
    for (int i = 0; i < n; ++i) {
        if (arr[i] < 0) {
            first_negative_index = i;
            break;
        }
    }
    int second_negative_index = -1;
    for (int i = first_negative_index + 1; i < n; ++i) {
        if (arr[i] < 0) {
            second_negative_index = i;
            break;
        }
    }

    double sumbetween = 0.0;
    if (first_negative_index != -1 && second_negative_index != -1) {
        for (int i = first_negative_index + 1; i < second_negative_index; ++i) {
            sumbetween += arr[i];
        }
    }

    printf("Номер минимального элемента: %d\n", min_index);
    if (first_negative_index == -1 || second_negative_index == -1) {
        printf("В массиве нет двух отрицательных элементов.\n");
    }
    else {
        printf("Сумма элементов между первым и вторым отрицательными элементами: %.2lf\n", sumbetween);
    }
    free(arr);

    second();
}
