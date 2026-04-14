#pragma once
#include "../OS12_COM/Interface.h"
#define OS12HANDLE void*
#define PIADDER IAdder*
#define PIMULTIPLIER IMultiplier*
#define PIUNKNOWN IUnknown*


#define IERR(s)    std::cerr<<"ERROR:  "<<s<<std::endl
#define IRES(s,r)  std::cerr<<s<<r<<std::endl

namespace OS12 
{
	OS12HANDLE Init();
	namespace Adder 
	{
		double Add(OS12HANDLE, double, double);
		double Sub(OS12HANDLE, double, double);
	}

	namespace Multiplier
	{
		double Mul(OS12HANDLE, double, double);
		double Div(OS12HANDLE, double, double);
	}
	void Dispose(OS12HANDLE);
}