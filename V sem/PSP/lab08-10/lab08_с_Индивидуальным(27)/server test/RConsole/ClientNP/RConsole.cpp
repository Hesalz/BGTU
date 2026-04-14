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
#pragma warning(disable : 4996) // Надо пофиксить отображению меню после выбора 7 пунтка

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
	case 7: return "schedule";
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
	char wbuf[1000] = "start";
	while (true) {
		int serverSendCommand = 0;
		puts("Input some server command\n" \
			"1 - start\n" \
			"2 - stop\n" \
			"3 - wait\n" \
			"4 - statistics\n" \
			"5 - shutdown\n" \
			"6 - exit\n" \
			"7 - schedule (send service schedule)");
		scanf("%d", &serverSendCommand);
		
		if (serverSendCommand == 7) {
			puts("Enter service schedule in format:");
			puts("schedule SERVICE1 HH:MM HH:MM SERVICE2 HH:MM HH:MM ...");
			puts("Example: schedule RAND 08:00 12:00 TIME 09:00 10:00 ECHO 00:00 23:59");
			puts("Enter schedule:");
			
			while (getchar() != '\n');
			
			char schedule[1000];
			if (fgets(schedule, sizeof(schedule), stdin) != NULL) {
				size_t len = strlen(schedule);
				if (len > 0 && schedule[len - 1] == '\n')
					schedule[len - 1] = '\0';
				
				if (strlen(schedule) == 0) {
					puts("Error: Empty schedule entered. Please enter a valid schedule.");
					continue;
				}
				
				char finalSchedule[1000];
				if (strncmp(schedule, "schedule", 8) != 0) {
					sprintf_s(finalSchedule, "schedule %s", schedule);
				} else {
					strcpy_s(finalSchedule, schedule);
				}
				
				strcpy_s(wbuf, finalSchedule);
				
				HANDLE hMailslot = CreateFileA(
					"\\\\.\\mailslot\\ServerSchedule",
					GENERIC_WRITE,
					FILE_SHARE_READ | FILE_SHARE_WRITE,
					NULL,
					OPEN_EXISTING,
					FILE_ATTRIBUTE_NORMAL,
					NULL
				);
				
				if (hMailslot != INVALID_HANDLE_VALUE) {
					DWORD bytesWritten = 0;
					if (WriteFile(hMailslot, finalSchedule, (DWORD)strlen(finalSchedule), &bytesWritten, NULL)) {
						printf("Schedule sent to Mailslot successfully (%d bytes)\n", bytesWritten);
					} else {
						int error = GetLastError();
						printf("Error: Failed to write schedule to Mailslot (error: %d)\n", error);
					}
					CloseHandle(hMailslot);
				} else {
					int error = GetLastError();
					printf("Error: Failed to open Mailslot (error: %d)\n", error);
					printf("Make sure the server is running and Mailslot is created\n");
				}
			} else {
				puts("Error: Failed to read schedule from input");
				continue;
			}
		} else {
			strcpy_s(wbuf, get_command_by_id(serverSendCommand));
		}
		
		system("cls");
		if (!WriteFile(hPipe, wbuf, sizeof(wbuf), &dwWrite, NULL)) {
			int error = GetLastError();
			printf("Error: Failed to send command to server (error: %d)\n", error);
			printf("Error message: %s\n", SetPipeError("WriteFile:", error).c_str());
			continue;
		}
		printf("send:  %s\n", wbuf);
		if (serverSendCommand == 6 || serverSendCommand == 5)
			break;
		
		if (!ReadFile(hPipe, rbuf, sizeof(rbuf), &dwRead, NULL)) {
			int error = GetLastError();
			printf("Error: Failed to read response from server (error: %d)\n", error);
			printf("Error message: %s\n", SetPipeError("ReadFile:", error).c_str());
			break;
		}
		printf("get:  %s\n", rbuf);
	}
	CloseHandle(hPipe);
	system("pause");
	return 0;
}

