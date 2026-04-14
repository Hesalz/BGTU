#pragma once
#include "stdafx.h"
#include <winsock.h>
#pragma comment(lib, "ws2_32.lib") 
#include <string>
//#include <iostream>

using namespace std;

string  GetErrorMsgText(int code);
string  SetErrorMsgText(string msgText, int code);

string  SetPipeError(string msgText, int code);