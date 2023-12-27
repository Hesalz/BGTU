#include <iostream>
using namespace std;

void second() {
    cout << endl << "Задание №2" << endl;
    int n, m;
    cout << "Введите количество строк матрицы (0 < n < 100): ";
    cin >> n;
    if (n > 100 || n <= 0) return;
    cout << "Введите количество строк матрицы (0 < m < 100): ";
    cin >> m;
    if (m > 100 || m <= 0) return;

    int** mat = new int*[n];
    for (int i = 0; i < n; i++) {
        mat[i] = new int[m];
    }

    srand(time(NULL));
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            mat[i][j] = rand() % 10;
        }
    }

    cout << "Элементы матрицы:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << mat[i][j] << " ";
        }
        cout << endl;
    }

    int minsum = 0;
    int maxsum = 0;

    for (int i = 0; i < n; ++i) {
        if (i % 2 == 0) {
            int max_of_even = mat[i][0];
            for (int j = 0; j < m; ++j) {
                if (mat[i][j] < max_of_even) {
                    max_of_even = mat[i][j];
                }
            }
            minsum += max_of_even;
        }
        else {
            int min_of_odd = mat[i][0];
            for (int j = 0; j < m; ++j) {
                if (mat[i][j] > min_of_odd) {
                    min_of_odd = mat[i][j];
                }
            }
            maxsum += min_of_odd;
        }
    }

    cout << "Сумма наименьших элементов нечетных строк : " << minsum << endl;
    cout << "Сумма наибольших элементов четных строк: " << maxsum << endl;

    for (int k = 0; k < n; k++) 
        delete[] mat [k];
    delete[] mat;
}

void main() {
	setlocale(LC_ALL, "rus");
    /*const int max_size = 100;*/
    double* arr;

    int n;
    printf("Введите размер массива (0 < n < 100): ");
    scanf_s("%d", &n);

   /* if (n <= 0 || n > max_size) {
        printf("Некорректный размер массива.\n", max_size);
        return;
    } */
    arr = (double*)malloc(n * sizeof(double));

    if (arr == NULL) {
        printf("Ошибка выделения памяти.\n");
        return;
    }
    srand(time(NULL)); 
    for (int i = 0; i < n; ++i) {
        arr[i] = ((rand() % 100) / 10.0) - 5.0;
    }

    printf("Сгенерированный массив:\n");
    for (int i = 0; i < n; ++i) {
        printf("arr[%d]: %.2lf\n", i, arr[i]);
    }

    int max_index = 0;
    for (int i = 1; i < n; ++i) {
        if (arr[i] > arr[max_index]) {
            max_index = i;
        }
    }

    double product = 1.0;
    double sum = 0.0;

    for (int i = 0; i < max_index; ++i) {
        if (arr[i] < 0) {
            product *= arr[i];
        }
        else {
            sum += arr[i];
        }
    }

    printf("Индекс максимального элемента массива: %d\n", max_index);
    printf("Произведение отрицательных элементов: %.2lf\n", product);
    printf("Сумма положительных элементов до максимального: %.2lf\n", sum);
    free(arr);

    second();
}