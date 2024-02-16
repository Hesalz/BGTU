#include <iostream>
#include <cmath>
using namespace std;

int main() { // 4-ая доп. задача
    setlocale(LC_ALL, "rus");
    double r; 
    double p, q;
    cout << "Введите диагонали ромба\n";
    cin >> p >> q;
    cout << "Введите радиус шара\n";
    cin >> r;
    double diameter = 2 * r;
    bool abc = ((diameter <= p) && (diameter <= q));
    if (abc) {
        cout << "Шар пройдет через ромбообразное отверстие." << endl;
    }
    else {
        cout << "Шар не пройдет через ромбообразное отверстие." << endl;
    }
    return 0;
}
