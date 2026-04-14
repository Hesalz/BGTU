#include <objbase.h>
#include <iostream>
#include "../OS12_COM/Interface.h"

#define IERR(s)    std::cout<<"error "<<s<<std::endl
#define IRES(s,r)  std::cout<<s<<r<<std::endl

IAdder* pIAdder = nullptr;
IMultiplier* pMultiplier = nullptr;

int main()
{
	IUnknown* pIUnknown = NULL;
	HRESULT hrInit = CoInitialize(NULL);
	if (FAILED(hrInit)) {
		std::cerr << "CoInitialize error: " << hrInit << std::endl;
		return 1;
	}

	HRESULT hr0 = CoCreateInstance(CLSID_Math, NULL, CLSCTX_INPROC_SERVER, IID_IUnknown, (void**)&pIUnknown);

	if (SUCCEEDED(hr0) && pIUnknown != NULL)
	{
		std::cout << "CoCreateInstance succeeded" << std::endl;
		if (SUCCEEDED(pIUnknown->QueryInterface(IID_IAdder, (void**)&pIAdder)))
		{
			{
				double z = 0.0;
				if (!SUCCEEDED(pIAdder->Add(2.0, 3.0, z)))  IERR("IAdder::Add");
				else IRES("IAdder::Add = ", z);
			}
			{
				double z = 0.0;
				if (!SUCCEEDED(pIAdder->Sub(2.0, 3.0, z)))  IERR("IAdder::Sub");
				else IRES("IAdder::Sub = ", z);
			}
			if (SUCCEEDED(pIAdder->QueryInterface(IID_IMultiplier, (void**)&pMultiplier)))
			{
				{
					double z = 0.0;
					if (!SUCCEEDED(pMultiplier->Mul(2.0, 3.0, z))) IERR("IMultiplier::Mul");
					else IRES("Multiplier::Mul = ", z);
				}
				{
					double z = 0.0;
					if (!SUCCEEDED(pMultiplier->Div(2.0, 3.0, z))) IERR("IMultiplier::Div");
					else IRES("IMultiplier::Div = ", z);
				}
				if (SUCCEEDED(pMultiplier->QueryInterface(IID_IAdder, (void**)&pIAdder)))
				{
					double z = 0.0;
					if (!SUCCEEDED(pIAdder->Add(2.0, 3.0, z))) IERR("IAdder::Add");
					else IRES("IAdder::Add = ", z);
					pIAdder->Release();
				}
				else  IERR("IMultiplier->IAdder");
				pMultiplier->Release();
			}
			else IERR("IAdder->IMultiplier");
			pIAdder->Release();
		}
		else  IERR("IAdder");
		pIUnknown->Release();
	}
	else  
	{
		std::cerr << "CoCreateInstance error: " << hr0 << " (0x" << std::hex << hr0 << std::dec << ")" << std::endl;
		std::cerr << "Error code meaning: ";
		switch (hr0) {
		case REGDB_E_CLASSNOTREG:
			std::cerr << "REGDB_E_CLASSNOTREG - Class not registered or DLL not found" << std::endl;
			break;
		case CLASS_E_CLASSNOTAVAILABLE:
			std::cerr << "CLASS_E_CLASSNOTAVAILABLE - Class not available" << std::endl;
			break;
		case CO_E_DLLNOTFOUND:
			std::cerr << "CO_E_DLLNOTFOUND - DLL not found" << std::endl;
			break;
		default:
			std::cerr << "Unknown error" << std::endl;
		}
	}
	
	CoFreeUnusedLibraries();
	CoUninitialize();

	return 0;
}


