#include <iostream>
using namespace std;

int main() // 3 вариант 5 лаба
{
    setlocale(LC_ALL, "rus");
    float a, b, c, t;
    cout << "Введите a\n";
    cin >> a;
    cout << "Введите b\n";
    cin >> b;
    cout << "Введите c\n";
    cin >> c;
    (a != 0 && b != 0 && c != 0) ? t = pow(a * b * c, 1.0 / 3.0) : t = (a + b + c) / 3;
        cout << "t = " << t;
}