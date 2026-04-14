#include "stdafx.h"
#include "Winsock2.h"
#pragma comment(lib, "WS2_32.lib") 
#include <string>
#include <iostream>
#include <ctime>
#include <chrono>
#include <conio.h>

#define IP_SERVER "26.180.211.124"

using namespace std;
using namespace chrono;
SOCKET  cC;
SOCKET  cS;

bool ICAN = true;
bool ntpSynced = false;

string SetErrorMsgText(string msgText, int code);

#pragma pack(push, 1)
struct GETSINCHRO
{
	char cmd[4];
	__int64  curvalue;
};

struct SETSINCHRO
{
	char cmd[4];
	__int64  correction;
};
#pragma pack(pop)


const char* GLOBAL_NTP_SERVER_HOST = "pool.ntp.org";
const unsigned short GLOBAL_NTP_SERVER_PORT = 123;

LONGLONG GetLocalSystemTimeMsClient()
{
	FILETIME ft;
	GetSystemTimeAsFileTime(&ft);
	ULARGE_INTEGER uli;
	uli.LowPart = ft.dwLowDateTime;
	uli.HighPart = ft.dwHighDateTime;
	const ULONGLONG EPOCH_DIFF_100NS = 116444736000000000ULL;
	ULONGLONG unix100ns = uli.QuadPart - EPOCH_DIFF_100NS;
	return (LONGLONG)(unix100ns / 10000ULL);
}

LONGLONG QueryGlobalNtpTimeMs()
{
	SOCKET s = INVALID_SOCKET;
	LONGLONG result = -1;
	try
	{
		s = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		if (s == INVALID_SOCKET)
			throw SetErrorMsgText("NTP socket:", WSAGetLastError());

		SOCKADDR_IN serverAddr;
		ZeroMemory(&serverAddr, sizeof(serverAddr));
		serverAddr.sin_family = AF_INET;
		serverAddr.sin_port = htons(GLOBAL_NTP_SERVER_PORT);

		hostent* he = gethostbyname(GLOBAL_NTP_SERVER_HOST);
		if (!he)
			throw string("gethostbyname failed for NTP server");
		serverAddr.sin_addr = *reinterpret_cast<in_addr*>(he->h_addr);

		unsigned char packet[48] = { 0 };
		packet[0] = 0x1B;

		int timeout = 3000;
		setsockopt(s, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

		if (sendto(s, (char*)packet, sizeof(packet), 0, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
			throw SetErrorMsgText("NTP sendto:", WSAGetLastError());

		SOCKADDR_IN fromAddr;
		int fromLen = sizeof(fromAddr);
		int rc = recvfrom(s, (char*)packet, sizeof(packet), 0, (sockaddr*)&fromAddr, &fromLen);
		if (rc == SOCKET_ERROR)
			throw SetErrorMsgText("NTP recvfrom:", WSAGetLastError());
		if (rc < 48)
			throw string("NTP response too short");

		unsigned long secs =
			(packet[40] << 24) | (packet[41] << 16) | (packet[42] << 8) | (packet[43]);
		unsigned long frac =
			(packet[44] << 24) | (packet[45] << 16) | (packet[46] << 8) | (packet[47]);

		const unsigned long NTP_TIMESTAMP_DELTA = 2208988800UL;
		if (secs < NTP_TIMESTAMP_DELTA)
			throw string("NTP time before 1970");
		unsigned long unixSecs = secs - NTP_TIMESTAMP_DELTA;
		double ms = (double)unixSecs * 1000.0 + (double)frac * 1000.0 / 4294967296.0;
		result = (LONGLONG)ms;
	}
	catch (string& err)
	{
		cout << err << endl;
	}
	if (s != INVALID_SOCKET)
		closesocket(s);
	return result;
}

void SyncOsTimeWithGlobalNtp()
{
	LONGLONG ntpMs = QueryGlobalNtpTimeMs();
	if (ntpMs <= 0)
	{
		cout << "Failed to get time from global NTP server" << endl;
		return;
	}

	const ULONGLONG EPOCH_DIFF_100NS = 116444736000000000ULL;
	ULONGLONG unix100ns = (ULONGLONG)ntpMs * 10000ULL;
	ULONGLONG filetimeValue = unix100ns + EPOCH_DIFF_100NS;

	FILETIME ft;
	ft.dwLowDateTime = (DWORD)(filetimeValue & 0xFFFFFFFF);
	ft.dwHighDateTime = (DWORD)(filetimeValue >> 32);

	SYSTEMTIME stUtc;
	if (!FileTimeToSystemTime(&ft, &stUtc))
	{
		cout << "FileTimeToSystemTime failed" << endl;
		return;
	}

	cout << "Setting OS time to global NTP time (UTC): "
		<< stUtc.wYear << "-" << stUtc.wMonth << "-" << stUtc.wDay << " "
		<< stUtc.wHour << ":" << stUtc.wMinute << ":" << stUtc.wSecond << endl;

	if (!SetSystemTime(&stUtc))
	{
		cout << "SetSystemTime failed (need admin rights?)" << endl;
	}
}

bool CheckTimeoutMessage()
{
	char peekBuf[64] = { 0 };
	fd_set readSet;
	fd_set errorSet;
	timeval tv;
	tv.tv_sec = 0;
	tv.tv_usec = 0;
	FD_ZERO(&readSet);
	FD_ZERO(&errorSet);
	FD_SET(cC, &readSet);
	FD_SET(cC, &errorSet);
	int sel = select(0, &readSet, NULL, &errorSet, &tv);
	if (sel > 0)
	{
		if (FD_ISSET(cC, &errorSet)) {
			puts("Connection error detected. Server may have closed the connection.");
			closesocket(cC);
			WSACleanup();
			return true;
		}
		if (FD_ISSET(cC, &readSet))
		{
			int r = recv(cC, peekBuf, sizeof(peekBuf), MSG_PEEK);
			if (r > 0)
			{
				recv(cC, peekBuf, sizeof(peekBuf), 0);
				peekBuf[r] = '\0';
				if (!strcmp(peekBuf, "ServerExit")) {
					puts("Server is shutting down (EXIT). Connection closed.");
					closesocket(cC);
					WSACleanup();
					return true;
				}
				else if (!strcmp(peekBuf, "ServerShutdown")) {
					puts("Server is shutting down (SHUTDOWN). Connection closed.");
					closesocket(cC);
					WSACleanup();
					return true;
				}
				printf("server disconnect: %s\n", peekBuf);
				return true;
			}
			else if (r == 0) {
				puts("Server has closed the connection. Exiting...");
				closesocket(cC);
				WSACleanup();
				return true;
			}
			else {
				int error = WSAGetLastError();
				if (error == WSAECONNRESET || error == WSAENOTCONN || error == WSAESHUTDOWN) {
					puts("Server has closed the connection. Exiting...");
					closesocket(cC);
					WSACleanup();
					return true;
				}
			}
		}
	}
	return false;
}



string  GetErrorMsgText(int code)
{
	string msgText;
	switch (code)
	{
	case WSAEINTR:          msgText = "WSAEINTR";         break;
	case WSAEACCES:         msgText = "WSAEACCES";        break;
	case WSAEFAULT:          msgText = "WSAEFAULT";         break;
	case WSAEINVAL:         msgText = "WSAEINVAL";        break;
	case WSAEMFILE:          msgText = "WSAEMFILE";         break;
	case WSAEWOULDBLOCK:         msgText = "WSAEWOULDBLOCK";        break;
	case WSAEINPROGRESS:          msgText = "WSAEINPROGRESS";         break;
	case WSAEALREADY:         msgText = "WSAEALREADY";        break;
	case WSAENOTSOCK:          msgText = "WSAENOTSOCK";         break;
	case WSAEDESTADDRREQ:         msgText = "WSAEDESTADDRREQ";        break;
	case WSAEMSGSIZE:          msgText = "WSAEMSGSIZE";         break;
	case WSAEPROTOTYPE:         msgText = "WSAEPROTOTYPE";        break;
	case WSAENOPROTOOPT:          msgText = "WSAENOPROTOOPT";         break;
	case WSAEPROTONOSUPPORT:         msgText = "WSAEPROTONOSUPPORT";        break;
	case WSAESOCKTNOSUPPORT:          msgText = "WSAESOCKTNOSUPPORT";         break;
	case WSAEOPNOTSUPP:         msgText = "WSAEOPNOTSUPP";        break;
	case WSAEPFNOSUPPORT:          msgText = "WSAEPFNOSUPPORT";         break;
	case WSAEAFNOSUPPORT:         msgText = "WSAEAFNOSUPPORT";        break;
	case WSAEADDRINUSE:          msgText = "WSAEADDRINUSE";         break;
	case WSAEADDRNOTAVAIL:         msgText = "WSAEADDRNOTAVAIL";        break;
	case WSAENETDOWN:          msgText = "WSAENETDOWN";         break;
	case WSAENETUNREACH:         msgText = "WSAENETUNREACH";        break;
	case WSAENETRESET:          msgText = "WSAENETRESET";         break;
	case WSAECONNABORTED:         msgText = "WSAECONNABORTED";        break;
	case WSAECONNRESET:          msgText = "WSAECONNRESET";         break;
	case WSAENOBUFS:         msgText = "WSAENOBUFS";        break;
	case WSAEISCONN:          msgText = "WSAEISCONN";         break;
	case WSAENOTCONN:         msgText = "WSAENOTCONN";        break;
	case WSAESHUTDOWN:          msgText = "WSAESHUTDOWN";         break;
	case WSAETIMEDOUT:         msgText = "WSAETIMEDOUT";        break;
	case WSAECONNREFUSED:          msgText = "WSAECONNREFUSED";         break;
	case WSAEHOSTDOWN:         msgText = "WSAEHOSTDOWN";        break;
	case WSAEHOSTUNREACH:          msgText = "WSAEHOSTUNREACH";         break;
	case WSAEPROCLIM:         msgText = "WSAEPROCLIM";        break;
	case WSASYSNOTREADY:          msgText = "WSASYSNOTREADY";         break;
	case WSAVERNOTSUPPORTED:         msgText = "WSAVERNOTSUPPORTED";        break;
	case WSANOTINITIALISED:          msgText = "WSANOTINITIALISED";         break;
	case WSAEDISCON:         msgText = "WSAEDISCON";        break;
	case WSATYPE_NOT_FOUND:          msgText = "WSATYPE_NOT_FOUND";         break;
	case WSAHOST_NOT_FOUND:         msgText = "WSAHOST_NOT_FOUND";        break;
	case WSATRY_AGAIN:          msgText = "WSATRY_AGAIN";         break;
	case WSANO_RECOVERY:         msgText = "WSANO_RECOVERY";        break;
	case WSANO_DATA:          msgText = "WSANO_DATA";         break;
	case WSA_INVALID_HANDLE:         msgText = "WSA_INVALID_HANDLE";        break;
	case WSA_INVALID_PARAMETER:          msgText = "WSA_INVALID_PARAMETER";         break;
	case WSA_IO_INCOMPLETE:         msgText = "WSA_IO_INCOMPLETE";        break;
	case WSA_IO_PENDING:          msgText = "WSA_IO_PENDING";         break;
	case WSA_NOT_ENOUGH_MEMORY:         msgText = "WSA_NOT_ENOUGH_MEMORY";        break;
	case WSA_OPERATION_ABORTED:          msgText = "WSA_OPERATION_ABORTED";         break;
	case WSAEINVALIDPROCTABLE:         msgText = "WSAEINVALIDPROCTABLE";        break;
	case WSAEINVALIDPROVIDER:          msgText = "WSAEINVALIDPROVIDER";         break;
	case WSAEPROVIDERFAILEDINIT:         msgText = "WSAEPROVIDERFAILEDINIT";        break;

	case WSASYSCALLFAILURE: msgText = "WSASYSCALLFAILURE"; break;
	default:                msgText = "***ERROR***";      break;
	};
	return msgText;
};

string  SetErrorMsgText(string msgText, int code)
{
	string error = msgText + GetErrorMsgText(code);
	error.append(" ").append(to_string(code));
	return error;
};

char* get_message(int msg)
{
	switch (msg)
	{
	case 1: 	return "Echo";
	case 2: 	return "Time";
	case 3: 	return "Random";
	case 4:		return "close";
	case 5:		return "exit";
	default:
		return "";
	}
}



bool GetServer(char* call, SOCKADDR_IN* from, int* flen, SOCKET* cC, SOCKADDR_IN* all) {
	char ibuf[200], obuf[200];
	int  libuf = 0, lobuf = 0;

	if ((lobuf = sendto(*cC, call, strlen(call) + 1, NULL, (sockaddr*)all, sizeof(*all))) == SOCKET_ERROR) throw  SetErrorMsgText("sendto:", WSAGetLastError());
	if ((libuf = recvfrom(*cC, ibuf, sizeof(ibuf), NULL, (sockaddr*)from, flen)) == SOCKET_ERROR) {
		if (WSAGetLastError() == WSAETIMEDOUT) return false;
		else throw  SetErrorMsgText("recv:", WSAGetLastError());
	}
	if (strcmp(call, ibuf) == 0) return true;
	else return false;
}



BOOL WINAPI HandlerRountime(DWORD eventCode)
{
	ICAN = false;
	Sleep(3000);
	switch (eventCode)
	{
	case CTRL_CLOSE_EVENT:
		int lobuf;
		if ((lobuf = send(cC, get_message(5), 6, NULL)) == SOCKET_ERROR)
			cout << "Error: send failed";
		Sleep(200);
		if (closesocket(cS) == SOCKET_ERROR)
			throw  SetErrorMsgText("closesocket:", WSAGetLastError());
		if (WSACleanup() == SOCKET_ERROR)
			throw  SetErrorMsgText("Cleanup:", WSAGetLastError());
		return false;
		break;
	default:
		break;


	}

}

void RunTimeSyncClientUDP(const SOCKADDR_IN* serverAddr)
{
	if (!serverAddr)
		throw string("RunTimeSyncClientUDP: serverAddr is NULL");

	SOCKET s = socket(AF_INET, SOCK_DGRAM, 0);
	if (s == INVALID_SOCKET)
		throw SetErrorMsgText("UDP socket:", WSAGetLastError());

	SOCKADDR_IN serv = *serverAddr;

	int Tc;
	int N;
	cout << "UDP time sync: enter period Tc (ms): ";
	cin >> Tc;
	cout << "UDP time sync: enter number of requests N: ";
	cin >> N;

	int udpStatMode = 1;
	cout << "UDP output mode: 1 - simple (correction + Cc), 2 - Cc-OStime stats (Unix-ms scale)\n";
	cin >> udpStatMode;
	const bool showCcOsStat = (udpStatMode == 2);

	__int64 Cc;

	if (ntpSynced)
	{
		Cc = GetLocalSystemTimeMsClient();
	}
	else
	{
		Cc = 0; 
	}

	long double sumCcMinusOs = 0.0L;
	long long ccOsSamples = 0;

	for (int i = 1; i <= N; ++i)
	{
		GETSINCHRO req{};
		memcpy(req.cmd, "SINC", 4);
		req.curvalue = Cc;

		int servLen = sizeof(serv);
		if (sendto(s, (char*)&req, sizeof(req), 0, (sockaddr*)&serv, servLen) == SOCKET_ERROR)
			throw SetErrorMsgText("UDP sendto:", WSAGetLastError());

		SETSINCHRO ans{};
		SOCKADDR_IN from{};
		int fromLen = sizeof(from);
		int rc = recvfrom(s, (char*)&ans, sizeof(ans), 0, (sockaddr*)&from, &fromLen);
		if (rc == SOCKET_ERROR)
			throw SetErrorMsgText("UDP recvfrom:", WSAGetLastError());

		if (rc != sizeof(ans) || strncmp(ans.cmd, "SINC", 4) != 0)
		{
			cout << "UDP time sync: invalid server response\n";
			break;
		}

		__int64 correction = ans.correction;

		Cc += correction;

		if (showCcOsStat)
		{
			LONGLONG osTimeMs = GetLocalSystemTimeMsClient();
			__int64 diffCcMinusOs = Cc - (__int64)osTimeMs;
			sumCcMinusOs += (long double)diffCcMinusOs;
			++ccOsSamples;
			long double avgCcMinusOs = sumCcMinusOs / (long double)ccOsSamples;

			cout << "[UDP] request #" << i
				<< " correction=" << correction
				<< " Cc=" << Cc
				<< " (Cc-OStimeMs)=" << diffCcMinusOs
				<< " avg(Cc-OStimeMs)=" << (double)avgCcMinusOs
				<< endl;
		}
		else
		{
			cout << "[UDP] request #" << i
				<< " correction=" << correction
				<< " Cc after correction=" << Cc << endl;
		}

		Sleep(Tc);
		Cc += Tc;
	}

	closesocket(s);
}

int _tmain(int argc, char* argv[])
{
	SetConsoleCtrlHandler(HandlerRountime, TRUE);
	WSADATA wsaData;
	setlocale(0, "rus");
	try
	{
		if (WSAStartup(MAKEWORD(2, 1), &wsaData) != 0)
			throw  SetErrorMsgText("Startup:", WSAGetLastError());
		if ((cS = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
			throw  SetErrorMsgText("socket:", WSAGetLastError());

		if ((cC = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
			throw  SetErrorMsgText("socket:", WSAGetLastError());

		SOCKADDR_IN serv;





		int connectionType = 0;
		int port;
		cout << "1 - Connect using server callname (broadcast)" << endl
			<< "2 - Connect using DNS name / IP" << endl;
		cin >> connectionType;

		while (connectionType != 1 && connectionType != 2) {
			cout << "Enter correct connection type (1 or 2)\n";
			cin >> connectionType;
		}

		bool connected = false;
		bool firstConnection = true;
		char call[200] = "";
		string dnsName = "";

		if (connectionType == 1) {
			cout << "Enter callname: ";
			cin >> call;
		}
		else if (connectionType == 2) {
			cout << "Enter DNS name or IP: ";
			cin >> dnsName;
		}

		while (!connected) {
			if (cC != INVALID_SOCKET) {
				closesocket(cC);
				cC = INVALID_SOCKET;
			}

			if ((cC = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
				throw SetErrorMsgText("socket:", WSAGetLastError());

			if (connectionType == 1) {

				SOCKET SocketUDP;
				int optval = 1;
				if ((SocketUDP = socket(AF_INET, SOCK_DGRAM, NULL)) == INVALID_SOCKET)
					throw SetErrorMsgText("Socket:", WSAGetLastError());
				if (setsockopt(SocketUDP, SOL_SOCKET, SO_BROADCAST, (char*)&optval, sizeof(int)) == SOCKET_ERROR)
					throw SetErrorMsgText("Opt:", WSAGetLastError());

				SOCKADDR_IN all;
				all.sin_family = AF_INET;
				all.sin_port = htons(2000);
				all.sin_addr.s_addr = INADDR_BROADCAST;
				SOCKADDR_IN from;

				memset(&from, 0, sizeof(from));
				int lc = sizeof(from);

				bool bsr = GetServer(call, &from, &lc, &SocketUDP, &all);
				if (!bsr) {
					cout << "Server not found, retrying..." << endl;
					Sleep(3000);
				}
				else {
					serv.sin_family = AF_INET;
					serv.sin_port = htons(2000);
					serv.sin_addr.s_addr = from.sin_addr.s_addr;

					if (closesocket(SocketUDP) == SOCKET_ERROR)
						throw SetErrorMsgText("Closesocket:", WSAGetLastError());

					if ((connect(cC, (sockaddr*)&serv, sizeof(serv))) == SOCKET_ERROR) {
						if (firstConnection) {
							cout << "Failed to connect, retrying..." << endl;
							firstConnection = false;
						}
						Sleep(3000);
						continue;
					}
				}
			}
			else if (connectionType == 2) {

				hostent* he = gethostbyname(dnsName.c_str());
				if (!he) {
					cout << "DNS lookup failed, retrying..." << endl;
					Sleep(3000);
					continue;
				}

				serv.sin_family = AF_INET;
				serv.sin_port = htons(2000);
				serv.sin_addr = *reinterpret_cast<in_addr*>(he->h_addr);

				if ((connect(cC, (sockaddr*)&serv, sizeof(serv))) == SOCKET_ERROR) {
					cout << "Failed to connect, retrying..." << endl;
					Sleep(3000);
				}
				else {
					
				}
			}

			DWORD timeout = 3000;
			setsockopt(cC, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

			char initialMessage[50];
			memset(initialMessage, 0, sizeof(initialMessage));
			int initialRecv = recv(cC, initialMessage, sizeof(initialMessage) - 1, NULL);

			timeout = 0;
			setsockopt(cC, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

			if (initialRecv > 0) {
				initialMessage[initialRecv] = '\0';
				if (!strcmp(initialMessage, "ServerStopped")) {
					if (firstConnection) {
						puts("Server is in STOP mode. Waiting for server to start...");
						firstConnection = false;
					}
					closesocket(cC);
					cC = INVALID_SOCKET;
					Sleep(3000);
					continue;
				}
				else if (!strcmp(initialMessage, "ServerWait")) {
					if (firstConnection) {
						puts("Server is in WAIT mode. Waiting for current clients to finish...");
						firstConnection = false;
					}
					closesocket(cC);
					cC = INVALID_SOCKET;
					Sleep(3000);
					continue;
				}
				else if (!strcmp(initialMessage, "ServerExit")) {
					puts("Server is shutting down (EXIT). Connection closed.");
					closesocket(cC);
					WSACleanup();
					system("pause");
					return -1;
				}
				else if (!strcmp(initialMessage, "ServerShutdown")) {
					puts("Server is shutting down (SHUTDOWN). Connection closed.");
					closesocket(cC);
					WSACleanup();
					system("pause");
					return -1;
				}
				connected = true;
			}
			else if (initialRecv == 0) {
				if (firstConnection) {
					puts("Server is in STOP mode. Waiting for server to start...");
					firstConnection = false;
				}
				closesocket(cC);
				cC = INVALID_SOCKET;
				Sleep(3000);
				continue;
			}
			else {
				connected = true;
			}
		}

		cout << "Connected successfully!" << endl;






		while (true) {
			if (CheckTimeoutMessage()) {
				return -1;
			}
			char message[50],
				obuf[50];
			int  libuf = 0,
				lobuf = 0;

			puts("Choose");
			int service;
			puts("1 - Echo\n2 - Time\n3 - Random\n4 - close socket\n5 - UDP time sync\n6 - Sync OS time with global NTP");

			while (!_kbhit()) {
				if (CheckTimeoutMessage()) return -1;
				Sleep(100);
			}
			scanf("%d", &service);

			if (service == 5)
			{
				RunTimeSyncClientUDP(&serv);
				continue;
			}

			if (service == 6)
			{
				SyncOsTimeWithGlobalNtp();
				ntpSynced = true;
				continue;
			}

			char* outMessage = new char[5];
			strcpy(outMessage, get_message(service));

			if (CheckTimeoutMessage()) {
				return -1;
			}

			if ((lobuf = send(cC, outMessage, strlen(outMessage) + 1, NULL)) == SOCKET_ERROR) {
				int error = WSAGetLastError();
				if (error == WSAECONNRESET || error == WSAENOTCONN || error == WSAESHUTDOWN) {
					puts("Server has closed the connection. Exiting...");
					closesocket(cC);
					WSACleanup();
					return -1;
				}
				throw  SetErrorMsgText("send:", error);
			}

			printf("sended: %s\n", outMessage);

			if (CheckTimeoutMessage()) {
				return -1;
			}
			if ((libuf = recv(cC, message, sizeof(message), NULL)) == SOCKET_ERROR) {
				int error = WSAGetLastError();
				if (error == WSAECONNRESET || error == WSAENOTCONN || error == WSAESHUTDOWN) {
					puts("Server has closed the connection. Exiting...");
					closesocket(cC);
					WSACleanup();
					return -1;
				}
				throw  SetErrorMsgText("recv:", error);
			}
			if (libuf == 0) {
				puts("Server has closed the connection. Exiting...");
				closesocket(cC);
				WSACleanup();
				return -1;
			}

			if (service == 4) {
				break;
			}

			if (!strcmp(message, "TimeOUT")) {
				puts("time out");
				return -1;
			}
			if (!strcmp(message, "ServerExit")) {
				puts("Server is shutting down (EXIT). Connection closed.");
				closesocket(cC);
				WSACleanup();
				return -1;
			}
			if (!strcmp(message, "ServerShutdown")) {
				puts("Server is shutting down (SHUTDOWN). Connection closed.");
				closesocket(cC);
				WSACleanup();
				return -1;
			}
			if (service == 1)
			{
				for (int j = 15; j >= 0; --j) {
					if (!ICAN) {
						if (closesocket(cS) == SOCKET_ERROR)
							throw  SetErrorMsgText("closesocket:", WSAGetLastError());
						if (WSACleanup() == SOCKET_ERROR)
							throw  SetErrorMsgText("Cleanup:", WSAGetLastError());
					}
					Sleep(1000);
					sprintf(outMessage, "%d", j);
					if ((lobuf = send(cC, outMessage, strlen(outMessage) + 1, NULL)) == SOCKET_ERROR)
						throw  SetErrorMsgText("send:", WSAGetLastError());
					printf("send: %s\n", outMessage);
					if ((libuf = recv(cC, message, sizeof(message), NULL)) == SOCKET_ERROR)
						throw  SetErrorMsgText("recv:", WSAGetLastError());
					printf("receive: %s\n", message);
				}
			}
			else if (service == 2 || service == 3) {
				printf("receive: %s\n", message);
			}
		}

		if (closesocket(cS) == SOCKET_ERROR)
			throw  SetErrorMsgText("closesocket:", WSAGetLastError());
		if (WSACleanup() == SOCKET_ERROR)
			throw  SetErrorMsgText("Cleanup:", WSAGetLastError());
	}
	catch (string errorMsgText)
	{
		printf("\n%s", errorMsgText.c_str());
	}
	system("pause");
	return 0;
}