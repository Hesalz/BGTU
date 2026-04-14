#include "OS12_LIB.h"
#include <stdexcept>
#include <iostream>



OS12HANDLE OS12::Init()
{
    PIUNKNOWN pIUnknown = nullptr;
    try {
        if(!SUCCEEDED(CoInitialize(nullptr)))
            throw std::runtime_error("Error initialize OLE32");

        if(!SUCCEEDED(CoCreateInstance(CLSID_Math, NULL, CLSCTX_INPROC_SERVER, IID_IUnknown, (void**)&pIUnknown)))
            throw std::runtime_error("Error create instance CLSID_Math");

        return pIUnknown;
    }
    catch (std::runtime_error error) {
        IRES("Init: ", error.what());
        return nullptr;
    }
}

double OS12::Adder::Add(OS12HANDLE h, double x, double y)
{
    try {
        if (!SUCCEEDED(((PIUNKNOWN)h)->QueryInterface(IID_IAdder, (void**)&h)))
            throw std::runtime_error("Error get interface IID_IAdder");

        double result = 0.0;
        if (!SUCCEEDED(((PIADDER)h)->Add(x, y, result)))
            throw std::runtime_error("Error IAdder::Add");
        return result;
    }
    catch (std::runtime_error error) {
        IRES("Add: ", error.what());
    }
   
}

double OS12::Adder::Sub(OS12HANDLE h, double x, double y)
{
    try {
        if (!SUCCEEDED(((PIUNKNOWN)h)->QueryInterface(IID_IAdder, (void**)&h)))
            throw std::runtime_error("Error get interface IID_IAdder");

        double result = 0.0;
        if (!SUCCEEDED(((PIADDER)h)->Sub(x, y, result)))
            throw std::runtime_error("Error IAdder::Sub");
        return result;
    }
    catch (std::runtime_error error) {
        IRES("Sub: ", error.what());
    }
}

double OS12::Multiplier::Mul(OS12HANDLE h, double x, double y)
{
    try {
        if (!SUCCEEDED(((PIUNKNOWN)h)->QueryInterface(IID_IMultiplier, (void**)&h)))
            throw std::runtime_error("Error get interface IID_IMultiplier");

        double result = 0.0;
        if (!SUCCEEDED(((PIMULTIPLIER)h)->Mul(x, y, result)))
            throw std::runtime_error("Error Multiplier::Mul");
        return result;
    }
    catch (std::runtime_error error) {
        IRES("Mul: ", error.what());
    }
}
 
double OS12::Multiplier::Div(OS12HANDLE h, double x, double y)
{
    try {
        if (!SUCCEEDED(((PIUNKNOWN)h)->QueryInterface(IID_IMultiplier, (void**)&h)))
            throw std::runtime_error("Error get interface IID_IMultiplier");
        
        if(y == 0)
            throw std::runtime_error("Param of second equals to zero (x/0)");

        double result = 0.0;
        if (!SUCCEEDED(((PIMULTIPLIER)h)->Div(x, y, result)))
            throw std::runtime_error("Error Multiplier::Div");
        return result;
    }
    catch (std::runtime_error error) {
        IRES("Div: ", error.what());
    }
}

void OS12::Dispose(OS12HANDLE h) {
    ((PIUNKNOWN)h)->Release();
    CoFreeUnusedLibraries();
}