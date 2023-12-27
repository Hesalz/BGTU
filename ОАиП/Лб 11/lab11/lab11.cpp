#include <iostream>
using namespace std;

void second() {
    cout << endl;
    int A, B, n, m, p, q;
    cout << "Введите целое число A: ";
    cin >> A;
    cout << "Введите целое число B: ";
    cin >> B;
    cout << "Введите количество битов n: ";
    cin >> n;
    cout << "Введите количество битов m: ";
    cin >> m;
    cout << "Введите позицию p: ";
    cin >> p;
    cout << "Введите позицию q: ";
    cin >> q;

    int maskA = ((1 << n) - 1) << (p - n); 
    int maskB = ((1 << m) - 1) << (q - m); 
    int bitsFromA = (A & maskA) >> (p - n);

    B = (B & ~maskB) | (bitsFromA << (q - m));

    cout << "Результат B: " << B << endl;
}

void main() {
    setlocale(LC_ALL, "rus");
    int A, result;
    cout << "Введите целое число A: ";
    cin >> A;

    int mask = 0;
    for (int i = 2; i <= 14; i++) {
        mask |= (1 << (i - 1));
    }
    result = A ^ mask;

    cout << "Результат: " << result << endl;
    second();
}
