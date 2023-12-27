#include <iostream>
using namespace std;
//Вариант 4
double equation1(double x) {
    return pow(x, 3) + 2 * x - 1;
}

double equation2(double x) {
    return exp(x) - 2;
}

double solution(double (*equation)(double), double a, double b, double e) {
    double x;
    while ((b - a) >= e) {
        x = (a + b) / 2.0;

        if (equation(x) == 0.0)
            break;
        else if (equation(x) * equation(a) < 0)
            b = x;
        else
            a = x;
    }
    return x;
}

int main() {
    setlocale(LC_ALL, "RUS");
    double a1, b1, a2, b2, e = 0.001;

    cout << "Для уравнения x^3 + 2x - 1:\n";
    cout << "Введите начальное значение a: ";
    cin >> a1;
    cout << "Введите конечное значение b: ";
    cin >> b1;

    double root1 = solution(equation1, a1, b1, e);
    cout << "Корень уравнения: " << root1 << endl;

    cout << "\nДля уравнения e^x - 2:\n";
    cout << "Введите начальное значение a: ";
    cin >> a2;
    cout << "Введите конечное значение b: ";
    cin >> b2;

    double root2 = solution(equation2, a2, b2, e);
    cout << "Корень уравнения: " << root2 << endl;

    return 0;
}
