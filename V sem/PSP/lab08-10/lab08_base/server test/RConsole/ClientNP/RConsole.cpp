


#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <iostream>
#include <string>
#include <ctime>
using namespace std;
#define _CRT_SECURE_NO_WARNINGS
#define _itoa_s insted
#pragma warning(disable : 4996)

string  GetErrorMsgText(int code)
{
	string msgText;

	switch (code)
	{
	case WSAEINTR:          msgText = "WSAEINTR";         break;
	case WSAEACCES:         msgText = "WSAEACCES";        break;
	case WSAEFAULT:         msgText = "WSAEFAULT";        break;
	case WSAEINVAL:         msgText = "WSAEINVAL";        break;
	case WSAEMFILE:         msgText = "WSAEMFILE";        break;
	case WSAEWOULDBLOCK:    msgText = "WSAEWOULDBLOCK";   break;
	case WSAEINPROGRESS:    msgText = "WSAEINPROGRESS";   break;
	case WSAEALREADY:       msgText = "WSAEALREADY";      break;
	case WSAENOTSOCK:       msgText = "WSAENOTSOCK";      break;
	case WSAEDESTADDRREQ:   msgText = "WSAEDESTADDRREQ";  break;
	case WSAEMSGSIZE:       msgText = "WSAEMSGSIZE";      break;
	case WSAEPROTOTYPE:     msgText = "WSAEPROTOTYPE";    break;
	case WSAENOPROTOOPT:    msgText = "WSAENOPROTOOPT";   break;
	case WSAEPROTONOSUPPORT:msgText = "WSAEPROTONOSUPPORT"; break;
	default:                msgText = "***ERROR***";      break;
	};
	return msgText;
};
string  SetPipeError(string msgText, int code)
{
	return  msgText + GetErrorMsgText(code).append(to_string(code));
};

char* get_command_by_id(int id)
{
	char msg[50];
	char final_msg[70];
	switch (id)
	{
	case 1: return "start";
	case 2: return "stop";
	case 3: return "wait";
	case 4: return "statistics";
	case 5: return "shutdown";
	case 6: return "exit";
	default: return  "error";
	}
}
int _tmain(int argc, _TCHAR* argv[])
{
	setlocale(LC_ALL, "");
	char rbuf[200];

	DWORD dwRead;
	DWORD dwWrite;
	HANDLE hPipe;
	int n;
	try
	{
		if ((hPipe = CreateFileA(
			"\\\\.\\pipe\\BOX",
			GENERIC_READ | GENERIC_WRITE,
			FILE_SHARE_READ | FILE_SHARE_WRITE,
			NULL, OPEN_EXISTING, NULL,
			NULL)) == INVALID_HANDLE_VALUE)
			throw  SetPipeError("createfile:", GetLastError());
	}
	catch (string ErrorPipeText)
	{
		printf("\n%s\n", ErrorPipeText.c_str());
		return -1;
	}
	char wbuf[40] = "start";
	while (true) {
		int serverSendCommand = 0;
		puts("Input some server command\n" \
			"1 - start\n" \
			"2 - stop\n" \
			"3 - wait\n" \
			"4 - statistics\n" \
			"5 - shutdown\n" \
			"6 - exit");
		scanf("%d", &serverSendCommand);
		strcpy(wbuf, get_command_by_id(serverSendCommand));
		system("cls");
		WriteFile(hPipe, wbuf, sizeof(wbuf), &dwWrite, NULL);
		printf("send:  %s\n", wbuf);
		if (serverSendCommand == 6 || serverSendCommand == 5)
			break;
		ReadFile(hPipe, rbuf, sizeof(rbuf), &dwRead, NULL);
		printf("get:  %s\n", rbuf);
	}
	CloseHandle(hPipe);
	system("pause");
	return 0;
}

