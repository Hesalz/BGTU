#include "PrimMST.h"
#include <iostream>
#include <vector>
#include <queue>
#include <limits>

PrimMST::PrimMST(graph::AList& g, std::vector<std::vector<int>>& w)
    : graph(g), weights(w), V(g.n_vertex) {
    parent.resize(V, -1);
    key.resize(V, std::numeric_limits<int>::max());
    inMST.resize(V, false);
}

// Алгоритм Прима с использованием приоритетной очереди
void PrimMST::findMST() {
    using pii = std::pair<int, int>; // (вес, вершина)
    std::priority_queue<pii, std::vector<pii>, std::greater<pii>> pq;

    key[0] = 0;
    pq.push({ 0, 0 }); // Начинаем с вершины 0

    while (!pq.empty()) {
        int u = pq.top().second;
        pq.pop();

        if (inMST[u]) continue;
        inMST[u] = true;

        // Обрабатываем всех соседей вершины u
        for (int i = 0; i < graph.size(u); i++) {
            int v = graph.get(u, i);
            int weight = weights[u][v];

            if (!inMST[v] && weight < key[v]) {
                key[v] = weight;
                parent[v] = u;
                pq.push({ key[v], v });
            }
        }
    }
}

void PrimMST::printMST() {
    std::cout << "\n-- Минимальное остовное дерево (MST) по алгоритму Прима --" << std::endl;
    for (int i = 1; i < V; i++) {
        std::cout << "Ребро: " << parent[i] << " - " << i << " | Вес: " << weights[i][parent[i]] << std::endl;
    }
}
