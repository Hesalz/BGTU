#include "DFS.h"
#include <vector>
#include <climits>

void DFS::init(const graph::AList& al)
{
    this->al = &al;
    this->c = new Color[this->al->n_vertex];
    this->d = new int[this->al->n_vertex];
    this->f = new int[this->al->n_vertex];
    this->p = new int[this->al->n_vertex];
    this->t = 0;
    for (int i = 0; i < this->al->n_vertex; i++)
    {
        this->c[i] = WHITE;
        this->d[i] = this->f[i] = 0;
        this->p[i] = NIL;
    }
    for (int i = 0; i < this->al->n_vertex; i++)
        if (this->c[i] == WHITE)
        {
            this->visit(i);
        }
}

DFS::DFS(const graph::AList& al) { this->init(al); }
DFS::DFS(const graph::AMatrix& am) { this->init(*(new graph::AList(am))); }

void DFS::visit(int u)
{
    this->c[u] = GRAY;
    this->d[u] = ++(this->t);
    for (int j = 0; j < this->al->size(u); j++)
    {
        int v = this->al->get(u, j);
        if (this->c[v] == WHITE)
        {
            this->p[v] = u;
            this->visit(v);
        }
    }
    this->c[u] = BLACK;
    this->f[u] = ++(this->t);
    this->topological_sort.push_back(u);
}

std::vector<int> DFS::getTopologicalSort()
{
    std::reverse(this->topological_sort.begin(), this->topological_sort.end());
    return this->topological_sort;
}

int DFS::get(int i)
{
    int j = 0, min1 = INF, min2 = NINF, ntx = NIL;
    for (int j = 0; j <= i; j++) 
    {
        for (int k = 0; k < this->al->n_vertex; k++)
            if (this->f[k] < min1 && this->f[k] > min2)
            {
                min1 = this->f[k]; ntx = k;
            };
        min2 = min1;  min1 = INF;
    };
    return ntx;
}