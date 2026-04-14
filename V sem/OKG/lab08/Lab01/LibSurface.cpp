#include "stdafx.h"

double Function1(double x, double y)
{
	return x * x + y * y;
};

double Function2(double x, double y)
{
	return x * x - y * y;
};

double Function3(double x, double y)
{
	double value = 9 - x * x - y * y;
	if (value >= 0)
		return sqrt(value);
	else {
		return 0;
	}
};

CPlot3D::CPlot3D()///////////////
{
	pFunc = NULL;
	ViewPoint.RedimMatrix(3);
	WinRect.SetRect(0, 0, 200, 200);
	ViewPoint(0) = 10, ViewPoint(1) = 30;
	ViewPoint(2) = 45;
};

void CPlot3D::SetFunction(pfunc2 pF, CRectD RS, double dx, double dy)//////////
{
	pFunc = pF;
	SpaceRect.SetRectD(RS.left, RS.top, RS.right, RS.bottom);

	MatrF.clear();			// очищаем матрицу для хранения координат точек поверхности в МСК
	CreateMatrF(dx, dy);	// заполняем матрицу к-ми точек поверхности
}

void CPlot3D::SetViewPoint(double r, double fi, double q)////////////
{
	ViewPoint(0) = r;
	ViewPoint(1) = fi;
	ViewPoint(2) = q;

	MatrView.clear();	// очищаем матрицу для хранения к-т точек проекции поверхности на плоскость XY ВСК
	CreateMatrView();	// заполняем матрицу к-ми точек проекции поверхности на плоскость XY ВСК

	MatrWindow.clear();
	CreateMatrWindow();
}

CMatrix CPlot3D::GetViewPoint()
{
	CMatrix P = ViewPoint;
	return P;
}

void CPlot3D::SetWinRect(CRect Rect)////////
{
	WinRect = Rect;

	MatrWindow.clear();		// очищаем матрицу для хранения оконных координат точек изображения 
	CreateMatrWindow();		// заполняем матрицу хранения оконных координат точек изображения 
}

void CPlot3D::CreateMatrF(double dx, double dy)//////////////
{
	double xL = SpaceRect.left;
	double xH = SpaceRect.right;
	double yL = SpaceRect.bottom;
	double yH = SpaceRect.top;
	CVecMatrix VecMatrix;
	CMatrix V(4);
	V(3) = 1;
	for (double x = xL; x <= xH; x += dx)
	{
		VecMatrix.clear();
		for (double y = yL; y <= yH; y += dy)
		{
			V(0) = x;
			V(1) = y;
			V(2) = pFunc(x, y);
			VecMatrix.push_back(V);
		}
		MatrF.push_back(VecMatrix);
	}
}

int CPlot3D::GetNumberRegion()
//Определяет номер области  для  рисования.
{
	CMatrix CartPoint = SphereToCart(ViewPoint);	// Декартовы координаты точки наблюдения (3x1)
	double xView = CartPoint(0);					// x - координата точки наблюдения
	double yView = CartPoint(1);					// y - координата точки наблюдения
	double zView = CartPoint(2);					// z - координата точки наблюдения

	double xL = SpaceRect.left;
	double xH = SpaceRect.right;
	double yL = SpaceRect.bottom;
	double yH = SpaceRect.top;

	//-- Определяем где находится точка наблюдения относительно диагоналей области RectF:
	
	//-- получаем уравнение диагонали y1=y1(x) [точки (xL,yL)-(xH,yH)]и находим значение y1=y1(xView)  	
	double y1 = yL + (yH - yL) * (xView - xL) / (xH - xL);

	//-- получаем уравнение диагонали y2=y2(x) [точки (xL,yH)-(xH,yL)]и находим значение y2=y2(xView)  	
	double y2 = yH - (yH - yL) * (xView - xL) / (xH - xL);

	if ((yView <= y1) && (yView <= y2)) 
		return 1;
	if ((yView > y2) && (yView < y1)) 
		return 2;
	if ((yView >= y1) && (yView >= y2)) 
		return 3;
	if ((yView > y1) && (yView < y2)) 
		return 4;
}

void CPlot3D::CreateMatrView()////////////////
{
	// Создаём матрицу пересчёта из МСК в ВСК
	CMatrix MV = CreateViewCoord(ViewPoint(0), ViewPoint(1), ViewPoint(2));

	CVecMatrix VecMatrix;
	CMatrix VX(4), V(3);
	V(2) = 1;
	double xmin = DBL_MAX;
	double xmax = DBL_MIN;
	double ymin = DBL_MAX;
	double ymax = DBL_MIN;

	for (int i = 0; i < MatrF.size(); i++)
	{
		VecMatrix.clear();
		for (int j = 0; j < MatrF[i].size(); j++)
		{
			VX = MatrF[i][j];
			VX = MV * VX;
			V(0) = VX(0);
			V(1) = VX(1);
			VecMatrix.push_back(V);

			double x = V(0);
			double y = V(1);

			if (x < xmin) xmin = x;
			if (x > xmax) xmax = x;
			if (y < ymin) ymin = y;
			if (y > ymax) ymax = y;
		}
		MatrView.push_back(VecMatrix);
	}
	ViewRect.SetRectD(xmin, ymax, xmax, ymin);
}

void CPlot3D::CreateMatrWindow()/////////////
{
	// Матрица пересчёта из ВСК в ОСК
	CMatrix MW = SpaceToWindow(ViewRect, WinRect);

	CVecPoint VecPoint;
	CMatrix  V(3);
	for (int i = 0; i < MatrView.size(); i++)
	{
		VecPoint.clear();
		for (int j = 0; j < MatrView[i].size(); j++)
		{
			V = MatrView[i][j];
			V = MW * V;
			CPoint P((int)V(0), (int)V(1));
			VecPoint.push_back(P);
		}
		MatrWindow.push_back(VecPoint);
	}
}

void CPlot3D::Draw(CDC& dc)//////
{
	if (MatrWindow.empty())
	{
		TCHAR* error = _T("Массив данных для рисования в окне пуст! ");
		MessageBox(NULL, error, _T("Ошибка"), MB_ICONSTOP);
		return;
	}

	int kRegion = GetNumberRegion();

	int nRows = MatrWindow.size();
	int nCols = MatrWindow[0].size();

	CPoint pt[4];

	switch (kRegion)
	{
	case 1:
	{
		for (int j = nCols - 1; j > 0; j--)
			for (int i = 0; i < nRows - 1; i++)
			{
				pt[0] = MatrWindow[i][j];
				pt[1] = MatrWindow[i][j - 1];
				pt[2] = MatrWindow[i + 1][j - 1];
				pt[3] = MatrWindow[i + 1][j];
				dc.Polygon(pt, 4);
			}
		break;
	}
	case 2:
	{
		for (int i = 0; i < nRows - 1; i++)
			for (int j = 0; j < nCols - 1; j++)
			{
				pt[0] = MatrWindow[i][j];
				pt[1] = MatrWindow[i][j + 1];
				pt[2] = MatrWindow[i + 1][j + 1];
				pt[3] = MatrWindow[i + 1][j];
				dc.Polygon(pt, 4);
			}
		break;
	}
	case 3:
	{
		for (int j = 0; j < nCols - 1; j++)
			for (int i = 0; i < nRows - 1; i++)
			{
				pt[0] = MatrWindow[i][j];
				pt[1] = MatrWindow[i][j + 1];
				pt[2] = MatrWindow[i + 1][j + 1];
				pt[3] = MatrWindow[i + 1][j];
				dc.Polygon(pt, 4);
			}
		break;
	}
	case 4:
	{
		for (int i = nRows - 1; i > 0; i--)
			for (int j = 0; j < nCols - 1; j++)
			{
				pt[0] = MatrWindow[i][j];
				pt[1] = MatrWindow[i][j + 1];
				pt[2] = MatrWindow[i - 1][j + 1];
				pt[3] = MatrWindow[i - 1][j];
				dc.Polygon(pt, 4);
			}
		break;
	}
	}
}