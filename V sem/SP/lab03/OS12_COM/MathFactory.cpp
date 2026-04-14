#include "MathFactory.h"
#include "Math.h"
#include "SEQLOG.h" //for using debug

MathFactory::MathFactory() {
	m_lRef = 1;
}

MathFactory::~MathFactory() {};


STDMETHODIMP_(HRESULT __stdcall) MathFactory::QueryInterface(REFIID riid, void** ppv)
{
	if (riid == IID_IUnknown)
		*ppv = (IUnknown*)this;
	else if (riid == IID_IClassFactory)
		*ppv = (IClassFactory*)this;
	else
		return E_NOINTERFACE;

	if (*ppv) {
		AddRef();
		return (S_OK);
	}
	else
		return (E_NOINTERFACE);
}

STDMETHODIMP_(ULONG) MathFactory::AddRef() {
    InterlockedIncrement(&m_lRef);
    return m_lRef;
}

STDMETHODIMP_(ULONG) MathFactory::Release() {
    InterlockedDecrement(&m_lRef);
    if (m_lRef == 0)
    {
        delete this;
        return 0;
    }
    else
        return m_lRef;
}


STDMETHODIMP MathFactory::CreateInstance(LPUNKNOWN pUnkOuter, REFIID riid, void** ppvObj) {

    HRESULT hr = E_UNEXPECTED;

    Math* pMath = nullptr;

    if (pUnkOuter != NULL)
        hr = CLASS_E_NOAGGREGATION;
    else if ((pMath = new Math()) == NULL)
        hr = E_OUTOFMEMORY;
    else {
        hr = pMath->QueryInterface(riid, ppvObj);
        pMath->Release();
    }

    if (FAILED(hr))
        delete pMath;

    return hr;
}

STDMETHODIMP  MathFactory::LockServer(BOOL fLock) {
    if (fLock)
        InterlockedIncrement(&g_lLocks);
    else
        InterlockedDecrement(&g_lLocks);

    return S_OK;
}