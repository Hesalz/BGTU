#include <iostream>
#include <iomanip>
#include "Boat.h"
#define NN (sizeof(v)/sizeof(int))
#define MM 5

int wmain()
{
    setlocale(LC_ALL, "rus");
    int V = 1500,
        v[] = { 100,  230,   321,  504,  718,  295, 105, 900, 741, 641, 344, 712, 215, 572, 491, 478, 234, 823, 899, 100, 329, 421, 712, 648, 871 },
        c[NN] = { 108,   50,    34,   28,   123,  101, 12, 73, 42, 84, 92, 100, 142, 74, 53, 69, 11, 23, 92, 112, 84, 37, 48, 58, 39 };
    short  r[MM];
    int cc = boat(
        V,   // [in]  максимальный вес груза
        MM,  // [in]  количество мест для контейнеров     
        NN,  // [in]  всего контейнеров  
        v,   // [in]  вес каждого контейнера  
        c,   // [in]  доход от перевозки каждого контейнера     
        r    // [out] результат: индексы выбранных контейнеров  
    );
    std::cout << std::endl << "- Задача о размещении контейнеров на судне";
    std::cout << std::endl << "- общее количество контейнеров    : " << NN;
    std::cout << std::endl << "- количество мест для контейнеров : " << MM;
    std::cout << std::endl << "- ограничение по суммарному весу  : " << V;
    std::cout << std::endl << "- вес контейнеров                 : ";
    for (int i = 0; i < NN; i++) std::cout << std::setw(3) << v[i] << " ";
    std::cout << std::endl << "- доход от перевозки              : ";
    for (int i = 0; i < NN; i++) std::cout << std::setw(3) << c[i] << " ";
    std::cout << std::endl << "- выбраны контейнеры (0,1,...,m-1): ";
    for (int i = 0; i < MM; i++) std::cout << r[i] << " ";
    std::cout << std::endl << "- доход от перевозки              : " << cc;
    std::cout << std::endl << "- общий вес выбранных контейнеров : ";
    int s = 0; for (int i = 0; i < MM; i++) s += v[r[i]]; std::cout << s;
    std::cout << std::endl << std::endl;
    system("pause");
    return 0;
}