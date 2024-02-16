#include <iostream>
using namespace std;

int main() {
    double a = 5e-6, b = 40, x = 1.1, w=0.0, v=0.0;

    w = (a + b) * tan(x) / (x + 1);
    v = 0.5 * b - sqrt(w - a * b);

    cout << "w=" << w << endl;
    cout << "v=" << v << endl;

    return 0;
}