#include <iostream>
#include <string>
using namespace std;

void second() { // Вариант 1
        double nach; 
        double procent;
        double hochy;

        cout << "Введите начальную выручку: ";
        cin >> nach;
        cout << "Введите ежедневное увеличение выручки (%) : ";
        cin >> procent;
        cout << "Введите заданное значение выручки: ";
        cin >> hochy;

        double virychka = nach;
        int days = 0;

        while (virychka < hochy) {
            virychka += (virychka * procent / 100.0);
            days++;
        }

        cout << "Выручка фирмы в тот день: " << virychka << " тыс. руб." << endl;
        cout << "Количество дней для достижения результата: " << days << " дней" << endl << endl;
}

void third() { // Вариант 2
        int n; 
        double amortizac;
        double price = 0.0;

        cout << "Введите количество лет: ";
        cin >> n;
        cout << "Введите процент амортизации в год (%): ";
        cin >> amortizac;

        for (int i = 0; i < n; ++i) {

            double nachcost;
            cout << "Введите стоимость оборудования за " << i+1 << "-й год: ";
            cin >> nachcost;
            nachcost *= (1 - amortizac / 100.0);
            price += nachcost;
        }

        cout << "Общая стоимость накопленного оборудования за " << n << " лет: " << price << " рублей" << endl;
    }

int main() { // Вариант 6

    setlocale(LC_ALL, "rus");
    cout << "Введите целое число: ";
    string input;
    cin >> input;

    string result = "";

    for (char digit : input) {

        if (digit != '3' && digit != '6') {
            result += digit;
        }
    }

    cout << "Результат: " << result << endl << endl;

    second();
    third();
}
