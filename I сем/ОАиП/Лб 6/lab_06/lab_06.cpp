#include <iostream>
#include <cmath>
using namespace std;

// while
void second() {

    setlocale(LC_ALL, "rus");
    double y, a, z, q, j;
    y = -1.55;
    a = 6;
    j = -1;
    cout << "Задание 2" << endl;
    while (j < 1.2) {
        z = sqrt(a + 1) - tan(j + y);
        q = exp(-j * 0.1 * y) + pow(3 * z, 2);
        cout << "Для [" << j << "] " << "z = " << z << " q = " << q << endl;
        j += 0.2;
    }
    cout << endl << endl;


}

// for while
void third() {

    setlocale(LC_ALL, "rus");
    
    double y = -1.55, j[] = { 3.3, -4, 0.9 };

    cout << "Задание 3" << endl;
    for (int i = 0; i < 3; i++) {
        double a = 1;
        while (a < 2.2) {
            double z = sqrt(a + 1) - tan(j[i] + y);
            double q = exp(-j[i] * 0.1 * y) + pow(3 * z, 2);

            cout << "Для i[" << i << "] и a[" << a << "]: " << "z = " << z << ", q = " << q << endl;
            a += 0.2;
        }
    }
    cout << endl << endl;
}

//for
int main() {
    setlocale(LC_ALL, "rus");
    double y = -1.55, j[] = { 8, -0.6, 1, 6, 4 };
    int a = 6;

    cout << "Задание 1" << endl;

    for (int i = 0; i < 5; ++i) {
        double z = sqrt(a + 1) - tan(j[i] + y);
        double q = exp(-j[i] * 0.1 * y) + pow(3 * z, 2);

        cout << "Для j[" << i << "]: z = " << z << ", q = " << q << endl;
    }
    cout << endl << endl;
    second();
    third();

    return 0;
}