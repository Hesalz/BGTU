#include <iostream>
using namespace std;
int main() // 1 блок схема из лабы 2
{
    setlocale(LC_ALL, "rus");
    int x, y, z, max, t;
    cout << "Введите x\n";
    cin >> x;
    cout << "Введите y\n";
    cin >> y;
    cout << "Введите z\n";
    cin >> z;
    max = x + y + z;
    (x * y * z > max) ? max = x * y * z : max = x + y + z;
    t = max * 3;
    cout << "t = " << t;
}
