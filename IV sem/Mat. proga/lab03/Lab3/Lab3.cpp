#include <iostream>
#include <iomanip> 
#include "Salesman.h"
#define N 5
#define VARIANT 3

int main() // метод перестановки для проверки правильности
{
    setlocale(LC_ALL, "rus");
    int d[N][N] = { //0   1    2    3     4        
                  { INF, 2 * VARIANT, 21 + VARIANT, INF, VARIANT},    //  0
                  { VARIANT, INF,  15 + VARIANT,  68 - VARIANT, 84 - VARIANT},    //  1
                  { 2 + VARIANT,  3 * VARIANT, INF, 86, 49 + VARIANT},    //  2 
                  { 17 + VARIANT,  58 - VARIANT,  4 * VARIANT, INF, 3 * VARIANT},    //  3
                  { 93 - VARIANT,  66 + VARIANT,  52,  13 + VARIANT, INF} };   //  4  
    int r[N];                     // результат 
    int s = salesman(
        N,          // [in]  количество городов 
        (int*)d,          // [in]  массив [n*n] расстояний 
        r           // [out] массив [n] маршрут 0 x x x x  

    );
    std::cout << std::endl << "-- Задача коммивояжера -- ";
    std::cout << std::endl << "-- количество  городов: " << N;
    std::cout << std::endl << "-- матрица расстояний : ";
    for (int i = 0; i < N; i++)
    {
        std::cout << std::endl;
        for (int j = 0; j < N; j++)

            if (d[i][j] != INF) std::cout << std::setw(3) << d[i][j] << " ";

            else std::cout << std::setw(3) << "INF" << " ";
    }
    std::cout << std::endl << "-- оптимальный маршрут: ";
    for (int i = 0; i < N; i++) std::cout << r[i] + 1 << "-->"; std::cout << 0 + 1;
    std::cout << std::endl << "-- длина маршрута     : " << s;
    std::cout << std::endl;
    system("pause");
    return 0;
}