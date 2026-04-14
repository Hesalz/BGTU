#include "INTERFACE.h"
#include "REGISTRY.h"
#include "Math.h"
#include "MathFactory.h"

HMODULE hmodule;
LONG Seq = 0;

BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        hmodule = hModule;
        break;
    case DLL_THREAD_ATTACH:
		break;
    case DLL_THREAD_DETACH:
		break;
    case DLL_PROCESS_DETACH:
		break;
    }
    return TRUE;
}

STDAPI DllInstall(BOOL b, PCWSTR s) 
{
	return S_OK;
}

STDAPI DllRegisterServer()
{
	return RegisterServer(
		hmodule,
		CLSID_Math,
		FriendlyName,
		VerIndProg,
		ProgID
	);
}

STDAPI DllUnregisterServer()
{
	return UnregisterServer(
		CLSID_Math,
		VerIndProg,
		ProgID
	);
}

STDAPI DllCanUnloadNow() 
{
	if ((g_lLocks == 0) && (g_lObjs == 0))
		return S_OK;
	else
		return S_FALSE;
}
STDAPI DllGetClassObject(const CLSID& clsid, const IID& iid, void** ppv) {
	HRESULT rc = E_UNEXPECTED;
	MathFactory* pF;
	if (clsid != CLSID_Math) rc = CLASS_E_CLASSNOTAVAILABLE;
	else if ((pF = new MathFactory()) == NULL) rc = E_OUTOFMEMORY;
	else {
		rc = pF->QueryInterface(iid, ppv);
		pF->Release();
	}
	return rc;
}