#include <iostream>
using namespace std;

int main() { // var 6
    double  t = 0.0, u = 0.0, s = 0.0, y = 0.956, a = 5e-6, n = 4;

     t = 1 / sqrt(y) + 14 * a;
     u = (t + 1) / (a + 2);
     s = log((2 * n / 3) + exp(-n) / u);

     cout << "t=" << t <<  endl;
     cout << "u=" << u <<  endl;
     cout << "s=" << s <<  endl; 
    cout << 11 %% 10;
    return 0;
}
