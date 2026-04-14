#include "Math.h"
long g_lObjs = 0;
long g_lLocks = 0;

Math::Math()
{
	 m_lRef = 1;
	 InterlockedIncrement(&g_lObjs);
};

Math::~Math() 
{
	InterlockedDecrement(&g_lObjs);
}

HRESULT STDMETHODCALLTYPE Math::QueryInterface(REFIID riid, void** ppv)
{
	if (riid == IID_IUnknown || riid == IID_IAdder)
		*ppv = (IAdder*)this;
	else if (riid == IID_IMultiplier)
		*ppv = (IMultiplier*)this;
	else
		return E_NOINTERFACE;

	if (*ppv) {
		AddRef();
		return (S_OK);
	}
	else
		return (E_NOINTERFACE);
};

STDMETHODIMP_(ULONG) Math::AddRef()
{
	InterlockedIncrement(&m_lRef);
	return m_lRef;
};

STDMETHODIMP_(ULONG) Math::Release()
{
	InterlockedDecrement(&m_lRef);
	if (m_lRef == 0)
	{
		delete this;
		return 0;
	}
	else
		return m_lRef;
};

STDMETHODIMP Math::Add(const double x, const double y, double& z) {
	z = x + y;
	return S_OK;
}

STDMETHODIMP Math::Sub(const double x, const double y, double& z) {
	z = x - y;
	return S_OK;
}

STDMETHODIMP Math::Mul(const double x, const double y, double& z) {
	z = x * y;
	return S_OK;
}

STDMETHODIMP Math::Div(const double x, const double y, double& z) {
	z = x / y;
	return S_OK;
}
