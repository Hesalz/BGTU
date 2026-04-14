// ChildView.cpp: реализация класса CChildView
//
#define _USE_MATH_DEFINES

#include "stdafx.h"
#include "Lab02.h"
#include "ChildView.h"
#include <cmath>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// CChildView

CChildView::CChildView()
{
    Index = 0; // Добавляем инициализацию индекса
}

CChildView::~CChildView()
{
}

BEGIN_MESSAGE_MAP(CChildView, CWnd)
    ON_WM_PAINT()
    ON_COMMAND(ID_TESTS_F1, &CChildView::OnTestsF1)
    ON_COMMAND(ID_TESTS_F2, &CChildView::OnTestsF2)
    ON_COMMAND(ID_TESTS_F3, &CChildView::OnTestsF3)
END_MESSAGE_MAP()

// Обработчики сообщений CChildView

BOOL CChildView::PreCreateWindow(CREATESTRUCT& cs)
{
    if (!CWnd::PreCreateWindow(cs))
        return FALSE;

    cs.dwExStyle |= WS_EX_CLIENTEDGE;
    cs.style &= ~WS_BORDER;
    cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS,
        ::LoadCursor(nullptr, IDC_ARROW), reinterpret_cast<HBRUSH>(COLOR_WINDOW + 1), nullptr);

    return TRUE;
}

void CChildView::OnPaint()
{
    CPaintDC dc(this); // контекст устройства для рисования

    if (Index == 1)
    {
        Graph.Draw(dc, 0, 1);
    }
    else if (Index == 2)
    {
        Graph.Draw(dc, 0, 1);
    }
    else if (Index == 3)
    {
        const int N = 8, M = 100;
        double radius = 5.0;

        CMatrix X, Y;
        // Используем ваш собственный тип пера CMyPen (а не CPen)
        CMyPen PenOct, PenCirc;

        // Устанавливаем параметры CMyPen через ваш метод Set
        PenOct.Set(PS_SOLID, 3, RGB(255, 0, 0));   // восьмиугольник — красный
        PenCirc.Set(PS_SOLID, 2, RGB(0, 0, 255));  // окружность — синяя

        // 1) Рисуем восьмиугольник (и оси)
        X.RedimMatrix(N + 1);
        Y.RedimMatrix(N + 1);
        for (int i = 0; i <= N; ++i)
        {
            double angle = 2.0 * M_PI * i / N;
            X(i) = radius * cos(angle);
            Y(i) = radius * sin(angle);
        }

        RW.SetRect(200, 200, 600, 600);
        Graph.SetParams(X, Y, RW);
        Graph.SetPenLine(PenOct);
        Graph.SetPenAxis(PenAxis); // PenAxis — ваш член класса CMyPen, если нужно
        Graph.Draw(dc, 0, 0);      // рисуем рамку=0, оси=1

        // 2) Рисуем окружность (без повторной отрисовки осей)
        X.RedimMatrix(M + 1);
        Y.RedimMatrix(M + 1);
        for (int i = 0; i <= M; ++i)
        {
            double angle = 2.0 * M_PI * i / M;
            X(i) = radius * cos(angle);
            Y(i) = radius * sin(angle);
        }

        Graph.SetParams(X, Y, RW);
        Graph.SetPenLine(PenCirc);
        Graph.Draw(dc, 0, 0); // оси не рисуем второй раз
    }
}


double CChildView::MyF1(double x)
{
    double y = sin(x) / x;
    return y;
}

double CChildView::MyF2(double x)
{
    double y = sqrt(x) * sin(x);
    return y;
}

void CChildView::OnTestsF1()
{
    double xL = -3 * M_PI;
    double xH = -xL;
    double dx = M_PI / 36;
    int N = (int)((xH - xL) / dx);

    X.RedimMatrix(N + 1);
    Y.RedimMatrix(N + 1);

    for (int i = 0; i <= N; i++)
    {
        X(i) = xL + i * dx;
        Y(i) = MyF1(X(i));
    }

    PenLine.Set(PS_SOLID, 1, RGB(255, 0, 0));
    PenAxis.Set(PS_SOLID, 2, RGB(0, 0, 255));
    RW.SetRect(200, 200, 600, 600);
    Graph.SetParams(X, Y, RW);
    Graph.SetPenLine(PenLine);
    Graph.SetPenAxis(PenAxis);

    Index = 1;
    Invalidate();
}

void CChildView::OnTestsF2()
{
    double xL = 0;
    double xH = 6 * M_PI;
    double dx = M_PI / 36;
    int N = (int)((xH - xL) / dx);

    X.RedimMatrix(N + 1);
    Y.RedimMatrix(N + 1);

    for (int i = 0; i <= N; i++)
    {
        X(i) = xL + i * dx;
        Y(i) = MyF2(X(i));
    }

    PenLine.Set(PS_DASH, 3, RGB(255, 0, 0));
    PenAxis.Set(PS_SOLID, 2, RGB(0, 0, 0));
    RW.SetRect(200, 200, 600, 600);
    Graph.SetParams(X - 0.01, Y, RW);
    Graph.SetPenLine(PenLine);
    Graph.SetPenAxis(PenAxis);

    Index = 2;
    Invalidate();
}

void CChildView::OnTestsF3()
{
    // Создаем точки для восьмиугольника
    int N = 8;
    X.RedimMatrix(N + 1);
    Y.RedimMatrix(N + 1);

    double radius = 5.0; // Радиус в мировых координатах
    double centerX = 0.0;
    double centerY = 0.0;

    // Создаем точки восьмиугольника
    for (int i = 0; i <= N; i++)
    {
        double angle = 2.0 * M_PI * i / N;
        X(i) = centerX + radius * cos(angle);
        Y(i) = centerY + radius * sin(angle);
    }

    // Устанавливаем параметры пера для восьмиугольника
    PenLine.Set(PS_SOLID, 3, RGB(255, 0, 0)); // Красный, толщина 3
    PenAxis.Set(PS_SOLID, 2, RGB(0, 0, 255)); // Синий для осей

    // Устанавливаем область отображения (мировые координаты)
    RW.SetRect(200, 200, 600, 600);

    // Устанавливаем мировые координаты так, чтобы восьмиугольник был виден
    double margin = 1.0;
    double x_min = -radius - margin;
    double x_max = radius + margin;
    double y_min = -radius - margin;
    double y_max = radius + margin;

    // Создаем временные матрицы для установки параметров
    CMatrix tempX = X;
    CMatrix tempY = Y;
    Graph.SetParams(tempX, tempY, RW);

    // Принудительно устанавливаем нужную область мировых координат
    Graph.GetRS(RS); // Получаем текущую RS
    RS.SetRectD(x_min, y_max, x_max, y_min); // Устанавливаем нужные границы
    Graph.SetWindowRect(RW); // Обновляем матрицу преобразования

    Graph.SetPenLine(PenLine);
    Graph.SetPenAxis(PenAxis);

    Index = 3;
    Invalidate();
}