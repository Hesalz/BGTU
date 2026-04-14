#pragma once
#include <objbase.h>
#include <initguid.h>

// {e4ffc6c9-e6ca-4cfe-84f0-7b656b8fd825}
DEFINE_GUID(CLSID_Math,
	0xe4ffc6c9, 0xe6ca, 0x4cfe, 0x84, 0xf0, 0x7b, 0x65, 0x6b, 0x8f, 0xd8, 0x25);

// {f8db8e0a-60eb-46f1-852f-14b996ee7495}
DEFINE_GUID(IID_IAdder,
	0xf8db8e0a, 0x60eb, 0x46f1, 0x85, 0x2f, 0x14, 0xb9, 0x96, 0xee, 0x74, 0x95);

// {bebd0c4b-cd13-4773-b237-06fabfd4a258}
DEFINE_GUID(IID_IMultiplier,
	0xbebd0c4b, 0xcd13, 0x4773, 0xb2, 0x37, 0x06, 0xfa, 0xbf, 0xd4, 0xa2, 0x58);

static LPCWSTR  FriendlyName = L"OS12_secxndary";
static LPCWSTR  VerIndProg = L"OS12_secxndary.1.0";
static LPCWSTR  ProgID = L"OS12_secxndary.1";

interface IAdder : IUnknown
{
	  STDMETHOD(Add(const double x,const double y,double& z))PURE;
	  STDMETHOD(Sub(const double x,const double y,double& z))PURE;
};

interface IMultiplier : IUnknown
{
	  STDMETHOD(Mul(const double x, const double y, double& z))PURE;
	  STDMETHOD(Div(const double x, const double y, double& z))PURE;
};

