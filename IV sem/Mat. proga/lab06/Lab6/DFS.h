// ---DFS.h  
// 
#pragma once
#include "Graph.h"
#include <vector>

struct DFS   // depth-first search поиск в глубину
{
    const static int NIL = -1;
    const static int INF = INT_MAX;  // Объявление INF
    const static int NINF = INT_MIN;
    enum Color { WHITE, GRAY, BLACK };
    const graph::AList* al;                // исходный граф
    Color* c;                              // цвет вершины
    int* d;                                // время обнаружения
    int* f;                                // время завершения обработки
    int* p;                                // предшествующая вершина
    int t;                                 // текущее время
    std::vector<int> topological_sort;     // результат топологической сортировки

    DFS(const graph::AList& al);
    DFS(const graph::AMatrix& am);
    void visit(int v);
    void init(const graph::AList& al);
    std::vector<int> getTopologicalSort(); // Объявление метода
    int get(int i);                        // Объявление метода get
};