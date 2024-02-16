#include <iostream>
using namespace std;
//Вариант 1
double equation1(double x) {
    return pow(x, 2) + 4 * x - 2;
}

double equation2(double x) {
    return sin(x) + 0.1;
}

double solution(double (*equation)(double), double a, double b, double e) {
    double x = 0;
    while (abs(a - b) >= 2 * e) {
        x = (a + b) / 2.0;

        if (equation(x) * equation(a) <= 0)
            b = x;
        else
            a = x;
    }
    return x;
}

int main() {
    setlocale(LC_ALL, "RUS");
    double a = 0, b = 0, e = 0.001;

    cout << "Уравнение cos(x) + x - 7\n";
    cout << "Введите начальное значение a: ";
    cin >> a;
    cout << "Введите конечное значение b: ";
    cin >> b;

    double root1 = solution(equation1, a, b, e);
    cout << "Корень уравнения: " << root1 << endl;

    cout << "\nУравнение exp(x) - 1 / x\n";
    cout << "Введите начальное значение a: ";
    cin >> a;
    cout << "Введите конечное значение b: ";
    cin >> b;

    double root2 = solution(equation2, a, b, e);
    cout << "Корень уравнения: " << root2 << endl;

    return 0;
}
