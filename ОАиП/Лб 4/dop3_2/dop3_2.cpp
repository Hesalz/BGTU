#include <iostream>

using namespace std;

int main() {
    setlocale(LC_ALL, "rus");
    double x1, y1, x2, y2;

    cout << "Введите координаты точки M1 (x1 y1): ";
    cin >> x1 >> y1;

    cout << "Введите координаты точки M2 (x2 y2): ";
    cin >> x2 >> y2;

    double d = sqrt(pow(x2 - x1, 2) + pow(y2 - y1, 2));

    cout << "Расстояние между точками M1 и M2: " << d << endl;

    return 0;
}
