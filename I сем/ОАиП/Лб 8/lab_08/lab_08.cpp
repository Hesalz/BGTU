#include <iostream>
#include <cmath>
using namespace std;

void second() {
    setlocale(LC_ALL, "rus");
    cout << "Задание 1" << endl;
    float x[5] = { 9, 2.7, 4.1, 6, 12 }, p, y = 0, max;
    max = x[0];
    for (int i = 0; i < 5; i++) {
        y += pow(x[i], 2) + 1;
        (max <= x[i]) ? max = x[i] : max;
    }

    p = y + max;
    cout << p << endl << y << endl;
}

void dop1() {
    setlocale(LC_ALL, "Rus");
    cout << "Доп. задание 1" << endl;
    int k, sum = 0;
    cout << "Введите размер массива: " << endl;
    cin >> k;
    int *x = new int[k];
    cout << "Введите числа массива " << endl;
    for (int j = 0; j < k; j++) {
        cin >> x[j];
    }
    for (int i = 0; i < k; i++) {
        if (x[i] % 2 == 0) {
            sum += x[i];
        }
    }
    delete[] x;
    cout << "Сумма чётных чисел массива = " << sum << endl;
}

void dop2() {
    setlocale(LC_ALL, "Rus");
    cout << "Доп. задание 2" << endl;
    int k, last = 0;
    cout << "Введите размер массива: " << endl;
    cin >> k;
    int* x = new int[k];
    cout << "Введите числа массива " << endl;
    for (int j = 0; j < k; j++) {
        cin >> x[j];
    }
    for (int j = 0; j < k; j++) {
        if (x[j] < 0) {
            last = x[j];
        }
    }
    delete[] x;
    cout << "Порядковый номер последнего отрицательного элемента = " << last << endl;
}

void dop3() {
    setlocale(LC_ALL, "Rus");
    cout << "Доп. задание 3" << endl;
    int k, l = 0;
    float max, min;
    cout << "Введите размер массива: " << endl;
    cin >> k;
    float* x = new float[k];
    cout << "Введите числа массива " << endl;
    for (int d = 0; d < k; d++) {
        cin >> x[d];
    }
    min = x[0];
    max = x[0];
    for (int j = 0; j < k; j++) {
        if (x[j] > max) {
            max = x[j];
        }
        if (x[j] < min) {
            min = x[j];
        }
    }
    for (int o = 0; o < k; o++) {
        if ((x[o] > min) && (x[o] < max)) {
            l++;
        }
    }
    delete[] x;
    cout << "Количество элементов, стоящих между минимальным и максимальным значениями = " << l << endl;
}


int main()
{
    second();
    setlocale(LC_ALL, "rus");
    cout << "Задание 2" << endl;
    int m = 5, x[5] = { -2, 6, 1.1, 2.7, 4 }, z;
    z = 8 * x[2];
    for (int i = 0; i < m; i++) {
        z += pow((x[i] - 2), 2);
    }
    cout << z << endl;

    dop1();
    dop2();
    dop3();
}
