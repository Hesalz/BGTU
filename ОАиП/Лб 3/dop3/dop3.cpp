#include <iostream>
using namespace std;

int main() {
    double z=0.0, y=0.0, n = 2, b = -0.12, x = 1.3e-4;
    z = 1 / (x - 1) + sin(x) - sqrt(n);
    y = (exp(-b) + 1) / (2 * z);

    cout << "z=" << z << endl;
    cout << "y=" << y << endl;

    return 0;
}
