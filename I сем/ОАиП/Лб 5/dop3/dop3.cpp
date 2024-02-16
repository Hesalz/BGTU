#include <iostream>
using namespace std;

int main() // 5-ая доп. задача
{
    setlocale(LC_ALL, "rus");
    int a, b, c, r, s, t;
    cout << "Введите размеры коробки\n";
    cin >> r >> s >> t;
    cout << "Введите размеры посылки\n";
    cin >> a >> b >> c;
    bool first ( ((a <= r) && (b <= s) && (c <= t)) || ((a <= r) && (b <= t) && (c <= s)) || ((a <= s) && (b <= r) && (c <= t)) || ((a <= s) && (b <= t) && (c <= r)) || ((a <= t) && (b <= r) && (c <= s)) || ((a <= t) && (b <= s) && (c <= r)) );
    if (first) {
        cout << "Посылка помещается в коробку\n";
    }
    else {
        cout << "Посылка не помещается в коробку\n";
    }
}
