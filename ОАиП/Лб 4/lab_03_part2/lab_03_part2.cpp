#include <iostream>
using namespace std;

int main() {
    setlocale(LC_ALL, "rus");
    int a, b;

    cout << "Введите значение a: ";
    cin >> a;

    cout << "Введите значение b: ";
    cin >> b;

    char temp = a;
    a = b;
    b = temp;

    cout << "После обмена значениями:" << endl;
    cout << "a = " << a << endl;
    cout << "b = " << b << endl;

    return 0;
}