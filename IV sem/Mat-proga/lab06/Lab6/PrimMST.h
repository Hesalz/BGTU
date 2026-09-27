#ifndef PRIM_MST_H
#define PRIM_MST_H

#include <vector>
#include "Graph.h"

class PrimMST {
public:
    PrimMST(graph::AList& g, std::vector<std::vector<int>>& weights);
    void findMST();
    void printMST();

private:
    int V; // Количество вершин
    graph::AList& graph; // Список смежности
    std::vector<std::vector<int>> weights; // Матрица весов
    std::vector<int> parent; // Массив родительских вершин
    std::vector<int> key; // Минимальные веса
    std::vector<bool> inMST; // Массив посещенных вершин

    int minKey();
};

#endif
