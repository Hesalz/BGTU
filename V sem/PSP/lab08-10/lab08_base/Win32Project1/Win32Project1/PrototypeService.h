#pragma once
#include "ErrorFunctions.h"
#include <time.h>
#include <string>
struct Contact
{
	enum TE {
		EMPTY,
		ACCEPT,
		CONTACT
	}    type;
	enum ST {
		WORK,
		ABORT,
		TIMEOUT,
		FINISH
	}      sthread;

	SOCKET      s;
	SOCKADDR_IN prms;
	int         lprms;
	HANDLE      hthread;
	HANDLE      htimer;
	HANDLE		serverHThtead;

	char msg[50];
	char srvname[15];

	Contact(TE t = EMPTY, const char* namesrv = "")
	{
		memset(&prms, 0, sizeof(SOCKADDR_IN));
		lprms = sizeof(SOCKADDR_IN);
		type = t;
		strcpy_s(srvname, namesrv);
		msg[0] = 0;
	};

	void SetST(ST sth, const char* m = "")
	{
		sthread = sth;
		strcpy_s(msg, m);
	}
};

string runServer;

const std::string currentDateTime() {
	time_t     now = time(0);
	tm  tstruct {};
	char       buf[80];
	localtime_s(&tstruct, &now);
	strftime(buf, sizeof(buf), "%Y-%m-%d.%X", &tstruct);
	return buf;
}

void CALLBACK ASStartMessage(ULONG_PTR)
{
	printf("RunServer: %s  \nstart time:  %s\n", runServer.c_str(), currentDateTime().c_str());
}

void CALLBACK ASFinishMessage(ULONG_PTR)
{
	printf("finish time: %s\n", currentDateTime().c_str());
}

void QueueUserAPCWrapper(PAPCFUNC functionName, Contact *contact) {
	QueueUserAPC(functionName, contact->hthread, 0);
}

void send_msg_to_client(Contact* contact)
{
	if (send(contact->s, contact->msg, sizeof(contact->msg), NULL) == SOCKET_ERROR)
		throw  SetErrorMsgText("send:", WSAGetLastError());
}
DWORD WINAPI EchoServer(LPVOID pPrm)
{
	DWORD rc = 0;
	Contact *contact = (Contact*)(pPrm);
	u_long nonblk = 0;
	try
	{
		runServer = "EchoServer";
		QueueUserAPCWrapper(ASStartMessage, contact);
		int lobuf, libuf;
		contact->sthread = contact->WORK;
		contact->type = contact->CONTACT;
		strcpy_s(contact->msg, "start transmission");
		send_msg_to_client(contact);


		if (ioctlsocket(contact->s, FIONBIO, &nonblk) == SOCKET_ERROR)
			throw SetErrorMsgText("ioctlsocket:", WSAGetLastError());

		while (true) {

			if ((libuf = recv(contact->s, contact->msg, sizeof(contact->msg), NULL)) == SOCKET_ERROR)
				throw  SetErrorMsgText("recv:", WSAGetLastError());
			send_msg_to_client(contact);
			if (atoi(contact->msg) == 0) break;
		}
	}
	catch (...)
	{

		puts("error in dll");
		contact->sthread = contact->ABORT;
		contact->type = contact->EMPTY;
		rc = contact->sthread;

		QueueUserAPCWrapper(ASFinishMessage, contact);
		CancelWaitableTimer(contact->htimer);
		ExitThread(rc);

	}

	contact->type = contact->ACCEPT;

	nonblk = 1;
	if (ioctlsocket(contact->s, FIONBIO, &nonblk) == SOCKET_ERROR)
		throw SetErrorMsgText("ioctlsocket:", WSAGetLastError());

	QueueUserAPCWrapper(ASFinishMessage, contact);
	CancelWaitableTimer(contact->htimer);
	ExitThread(rc);

}

DWORD WINAPI TimeServer(LPVOID pPrm)
{
	DWORD rc = 0;
	Contact *contact = (Contact*)(pPrm);
	runServer = "TimeServer";
	int lobuf, libuf;
	QueueUserAPCWrapper(ASStartMessage, contact);
	strcpy_s(contact->msg, currentDateTime().c_str());
	send_msg_to_client(contact);

	contact->type = contact->ACCEPT;

	QueueUserAPCWrapper(ASFinishMessage, contact);
	CancelWaitableTimer(contact->htimer);
	ExitThread(rc);
}

DWORD WINAPI RandomServer(LPVOID pPrm)
{
	DWORD rc = 0;
	Contact *contact = (Contact*)(pPrm);
	runServer = "Random";
	QueueUserAPCWrapper(ASStartMessage, contact);
	srand(time(NULL));
	int lobuf, libuf, randNumber;
	randNumber = rand() % 100 + 1;
	sprintf_s(contact->msg, "%d", randNumber);
	send_msg_to_client(contact);

	contact->type = contact->ACCEPT;

	QueueUserAPCWrapper(ASFinishMessage, contact);
	CancelWaitableTimer(contact->htimer);
	ExitThread(rc);
}