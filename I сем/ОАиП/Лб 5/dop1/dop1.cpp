#include <iostream>
using namespace std;

int main() { // 3-я доп. задача
    setlocale(LC_ALL, "rus");
    int a = 0, b = 0, p = 0, q = 0, r = 0, s = 0, place = 0, house1 = 0, house2 = 0;
    cout << "Введите размеры прямоугольного участка через пробел\n";
    cin >> a >> b;
    cout << "Введите размеры первого дома через пробел\n";
    cin >> p >> q;
    cout << "Введите размеры второго дома через пробел\n";
    cin >> r >> s;
    place = a * b;
    house1 = p * q;
    house2 = r * s;
    bool fisrthouse = (place > house1);
    if (fisrthouse) {
        place = place - house1;
    }
    else {
        cout << "На территории не помещается первый дом\n";
    }
    bool secondhouse = (place > house2);
    if (secondhouse) {
        cout << "На территории помещается два дома\n";
    }
    else {
        cout << "На территории не помещается два дома\n";
    }
    return 0;
}
