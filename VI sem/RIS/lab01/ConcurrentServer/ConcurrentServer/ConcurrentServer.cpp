#include "stdafx.h"
#include <ws2tcpip.h>
#include "Winsock2.h"
#include "ErrorFunctions.h"
#include <string>
#include <list>
#include <time.h>
#include <iostream>
#include <unordered_map>
#include <cmath>
#define _CRT_SECURE_NO_WARNINGS
#define AS_SQ 10
#define IP_SERVER "127.0.0.1"
const char* ucall = "Hello";
using namespace std;
#ifndef MSG_DONTWAIT
#define MSG_DONTWAIT 0x40
#endif
SOCKET sS = INVALID_SOCKET;
int serverPort;
char dllName[50];
char namedPipeName[10];
volatile long opened = 0;
volatile long connectionCount = 0;
volatile long sayNoCount = 0;
volatile long successConnections = 0;
volatile long currentActiveConnections = 0;
volatile bool acceptEnabled = false;
volatile bool rejectNew = false;
volatile bool waitMode = false;
HANDLE hAcceptServer, hConsolePipe, hGarbageCleaner, hDispatchServer, hResponseServer;
HANDLE hClientConnectedEvent = CreateEvent(NULL, FALSE, FALSE, L"ClientConnected");
DWORD WINAPI AcceptServer(LPVOID pPrm);
DWORD WINAPI ConsolePipe(LPVOID pPrm);
DWORD WINAPI GarbageCleaner(LPVOID pPrm);
DWORD WINAPI DispatchServer(LPVOID pPrm);
DWORD WINAPI ResponseServer(LPVOID pPrm);
CRITICAL_SECTION scListContact;

struct Contact;
void SendReasonAndClose(Contact* c, const char* reason);

enum TalkersCommand {
    START,
    STOP,
    EXIT,
    STATISTICS,
    WAIT,
    SHUTDOWN,
    GETCOMMAND,
    LOAD_LIB,
    UNLOAD_LIB,
    ALGO_OLD,
    ALGO_NEW,
    TIME_LOCAL,
    TIME_NTP
};
volatile TalkersCommand  previousCommand = GETCOMMAND;
volatile bool useNewCorrectionAlgo = false;
volatile bool useNtpTime = false;
const char* NTP_SERVER_HOST = "pool.ntp.org";
const unsigned short NTP_SERVER_PORT = 123;
volatile LONGLONG ntpTimeOffsetMs = 0;

LONGLONG GetLocalSystemTimeMs()
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

LONGLONG QueryNtpTimeMs()
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
        serverAddr.sin_port = htons(NTP_SERVER_PORT);

        hostent* he = gethostbyname(NTP_SERVER_HOST);
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
        printf("\n%s", err.c_str());
    }
    if (s != INVALID_SOCKET)
        closesocket(s);
    return result;
}

DWORD WINAPI NtpSyncThread(LPVOID)
{
    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
        return 1;

    while (true)
    {
        if (useNtpTime)
        {
            LONGLONG ntpMs = QueryNtpTimeMs();
            if (ntpMs > 0)
            {
                LONGLONG localMs = GetLocalSystemTimeMs();
                ntpTimeOffsetMs = ntpMs - localMs;
            }
        }
        Sleep(10000); 
    }

    WSACleanup();
    return 0;
}

struct Contact
{
    enum TE {
        EMPTY,
        ACCEPT,
        CONTACT
    } type;
    enum ST {
        WORK,
        ABORT,
        TIMEOUT,
        FINISH
    } sthread;
    SOCKET s;
    SOCKADDR_IN prms;
    int lprms;
    HANDLE hthread;
    HANDLE htimer;
    HANDLE serverHThtead;
    char msg[50];
    char srvname[15];
    time_t lastActivityTime;
    int warningsCount;
    Contact(TE t = EMPTY, const char* namesrv = "")
    {
        ZeroMemory(&prms, sizeof(SOCKADDR_IN));
        lprms = sizeof(SOCKADDR_IN);
        type = t;
        strcpy(srvname, namesrv);
        msg[0] = 0x00;
        lastActivityTime = time(NULL);
        warningsCount = 0;
    }
    void SetST(ST sth, const char* m = "")
    {
        sthread = sth;
        strcpy(msg, m);
    }
};
typedef list<Contact> ListContact;
ListContact contacts;
bool  GetRequestFromClient(char* name, short port, SOCKADDR_IN* from, int* flen);

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

void SendReasonAndClose(Contact* c, const char* reason) {
    int sendRc = send(c->s, reason, (int)strlen(reason) + 1, NULL);
    if (sendRc != SOCKET_ERROR) {
        shutdown(c->s, SD_SEND);
        Sleep(500);
    }
}

bool AcceptCycle(int sq)
{
    if (sS == INVALID_SOCKET) return false;
    if (!acceptEnabled && !rejectNew && !waitMode) return false;
    bool rc = false;
    Contact c(Contact::ACCEPT, "EchoServer");
    int acceptCount = ((rejectNew || waitMode) && sq == 0) ? 1 : sq;
    while (acceptCount-- > 0 && !rc)
    {
        if ((c.s = accept(sS, (sockaddr*)&c.prms, &c.lprms)) == INVALID_SOCKET)
        {
            if (WSAGetLastError() != WSAEWOULDBLOCK)
                throw  SetErrorMsgText("accept:", WSAGetLastError());
        }
        else
        {
            rc = true;
            if (rejectNew) {
                printf("Rejecting new client %s:%d (server in stop)\n", inet_ntoa(c.prms.sin_addr), htons(c.prms.sin_port));
                const char* rejectMsg = "ServerStopped";
                int sendResult = send(c.s, rejectMsg, (int)strlen(rejectMsg) + 1, NULL);
                if (sendResult != SOCKET_ERROR) {
                    shutdown(c.s, SD_SEND);
                    Sleep(500); 
                }
                else {
                    printf("Failed to send ServerStopped to client, error: %d\n", WSAGetLastError());
                }
                closesocket(c.s);
                InterlockedIncrement(&sayNoCount);
            }
            else if (waitMode) {
                printf("Rejecting new client %s:%d (server in wait)\n", inet_ntoa(c.prms.sin_addr), htons(c.prms.sin_port));
                const char* rejectMsg = "ServerWait";
                int sendResult = send(c.s, rejectMsg, (int)strlen(rejectMsg) + 1, NULL);
                if (sendResult != SOCKET_ERROR) {
                    shutdown(c.s, SD_SEND);
                    Sleep(500);
                }
                else {
                    printf("Failed to send ServerWait to client, error: %d\n", WSAGetLastError());
                }
                closesocket(c.s);
                InterlockedIncrement(&sayNoCount);
            }
            else {
                EnterCriticalSection(&scListContact);
                contacts.push_front(c);
                LeaveCriticalSection(&scListContact);
                puts("contact connected");
                InterlockedIncrement(&connectionCount);
                InterlockedDecrement(&sayNoCount);
            }
        }
    }
    return rc;
}

void openSocket() {
    if (sS != INVALID_SOCKET) return;
    SOCKADDR_IN serv;
    u_long nonblk = 1;
    if ((sS = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
        throw  SetErrorMsgText("socket:", WSAGetLastError());
    InterlockedIncrement(&opened);
    serv.sin_family = AF_INET;
    serv.sin_port = htons(serverPort);
    serv.sin_addr.s_addr = INADDR_ANY;
    if (bind(sS, (LPSOCKADDR)&serv, sizeof(serv)) == SOCKET_ERROR) {
        int err = WSAGetLastError();
        closesocket(sS);
        sS = INVALID_SOCKET;
        InterlockedDecrement(&opened);
        throw  SetErrorMsgText("bind:", err);
    }
    if (listen(sS, SOMAXCONN) == SOCKET_ERROR)
        throw  SetErrorMsgText("listen:", WSAGetLastError());
    if (ioctlsocket(sS, FIONBIO, &nonblk) == SOCKET_ERROR)
        throw SetErrorMsgText("ioctlsocket:", WSAGetLastError());
}

void closeSocket() {
    if (sS != INVALID_SOCKET) {
        if (closesocket(sS) == SOCKET_ERROR)
            throw  SetErrorMsgText("closesocket:", WSAGetLastError());
        sS = INVALID_SOCKET;
        InterlockedDecrement(&opened);
    }
}

void CommandsCycle(TalkersCommand& cmd)
{
    int  sq = 0;
    while (cmd != EXIT)
    {
        switch (cmd)
        {
        case START: cmd = GETCOMMAND;
            if (previousCommand != START) {
                sq = AS_SQ;
                puts("Start command");
                if (sS == INVALID_SOCKET) openSocket();
                acceptEnabled = true;
                rejectNew = false;
                waitMode = false;
                previousCommand = START;
            }
            else puts("start already in use");
            break;
        case STOP:  cmd = GETCOMMAND;
            if (previousCommand != STOP) {
                sq = AS_SQ;
                puts("Stop command");
                acceptEnabled = true;
                rejectNew = true;
                waitMode = false;
                previousCommand = STOP;
            }
            else puts("stop already in use");
            break;
        case WAIT:  cmd = GETCOMMAND;
            if (previousCommand != WAIT) {
                sq = AS_SQ;
                puts("Wait command: pause accepting new clients until current finish");
                acceptEnabled = true;
                rejectNew = false;
                waitMode = true;
                previousCommand = WAIT;
            }
            else puts("wait already in use");
            break;
        case EXIT:
            sq = 0;
            puts("EXIT command\n........shutting down...........");
            EnterCriticalSection(&scListContact);
            for (auto i = contacts.begin(); i != contacts.end(); i++) {
                if (i->type == i->CONTACT || i->type == i->ACCEPT) {
                    SendReasonAndClose(&(*i), "ServerExit");
                    closesocket(i->s);
                    i->sthread = i->ABORT;
                    i->type = i->EMPTY;
                }
            }
            LeaveCriticalSection(&scListContact);
            Sleep(1000);
            closeSocket();
            acceptEnabled = false;
            rejectNew = false;
            waitMode = false;
            cmd = EXIT;
            break;
        case SHUTDOWN:
            sq = 0;
            puts("SHUTDOWN command\n........shutting down...........");
            EnterCriticalSection(&scListContact);
            for (auto i = contacts.begin(); i != contacts.end(); i++) {
                if (i->type == i->CONTACT || i->type == i->ACCEPT) {
                    SendReasonAndClose(&(*i), "ServerShutdown");
                    closesocket(i->s);
                    i->sthread = i->ABORT;
                    i->type = i->EMPTY;
                }
            }
            LeaveCriticalSection(&scListContact);
            Sleep(1000);
            closeSocket();
            acceptEnabled = false;
            rejectNew = false;
            waitMode = false;
            while (true) {
                EnterCriticalSection(&scListContact);
                int contactSize = contacts.size();
                LeaveCriticalSection(&scListContact);
                if (contactSize == 0) break;
                Sleep(100);
            }
            printf("size of contacts 0\n");
            cmd = EXIT;
            break;
        case GETCOMMAND:  cmd = GETCOMMAND;
            break;
        };

        if (waitMode) {
            EnterCriticalSection(&scListContact);
            int contactSize = contacts.size();
            LeaveCriticalSection(&scListContact);
            if (contactSize == 0) {
                printf("active contacts: %d\n", contactSize);
                waitMode = false;
                puts("accepting resumed");
            }
        }
        if (acceptEnabled) {
            if (AcceptCycle(sq))
            {
                cmd = GETCOMMAND;
                if (!rejectNew && !waitMode) {
                    SetEvent(hClientConnectedEvent);
                }
            }
            else SleepEx(0, TRUE);
        }
        else {
            SleepEx(100, TRUE);
        }
    }
    if (cmd == EXIT) {
        ExitProcess(0);
    }
}

DWORD WINAPI AcceptServer(LPVOID pPrm)
{
    DWORD rc = 0;
    WSADATA wsaData;
    try
    {
        if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
            throw  SetErrorMsgText("Startup:", WSAGetLastError());
        CommandsCycle(*((TalkersCommand*)pPrm));
        if (WSACleanup() == SOCKET_ERROR)
            throw SetErrorMsgText("Cleanup:", WSAGetLastError());
    }
    catch (string errorMsgText)
    {
        printf("\n%s", errorMsgText.c_str());
    }
    puts("shutdown acceptServer");
    ExitThread(rc);
}

TalkersCommand set_param(char* param) {
    if (!strcmp(param, "start")) return START;
    if (!strcmp(param, "stop")) return STOP;
    if (!strcmp(param, "exit")) return EXIT;
    if (!strcmp(param, "wait")) return WAIT;
    if (!strcmp(param, "shutdown")) return SHUTDOWN;
    if (!strcmp(param, "statistics")) return STATISTICS;
    if (!strcmp(param, "getcommand")) return GETCOMMAND;
    if (strstr(param, "UNLOAD_LIB")) return UNLOAD_LIB;
    if (strstr(param, "LOAD_LIB")) return LOAD_LIB;
    if (!strcmp(param, "algo_old")) return ALGO_OLD;
    if (!strcmp(param, "algo_new")) return ALGO_NEW;
    if (!strcmp(param, "time_local")) return TIME_LOCAL;
    if (!strcmp(param, "time_ntp")) return TIME_NTP;
    return GETCOMMAND;
}

typedef void* (*FUNCTION)(char*, LPVOID);
FUNCTION ts;
volatile bool is_load_library = false;
std::list<HMODULE> list_of_dlls;
std::list<FUNCTION> list_of_functions;

DWORD WINAPI ConsolePipe(LPVOID pPrm)
{
    DWORD rc = 0;
    char rbuf[100];
    DWORD dwRead, dwWrite;
    HANDLE hPipe;
    try
    {
        char namedPipeConnectionString[50];
        sprintf(namedPipeConnectionString, "\\\\.\\pipe\\%s", namedPipeName);
        SECURITY_DESCRIPTOR sd;
        SECURITY_ATTRIBUTES sa;
        InitializeSecurityDescriptor(&sd, SECURITY_DESCRIPTOR_REVISION);
        SetSecurityDescriptorDacl(&sd, TRUE, NULL, FALSE);
        sa.nLength = sizeof(sa);
        sa.lpSecurityDescriptor = &sd;
        sa.bInheritHandle = FALSE;
        if ((hPipe = CreateNamedPipeA(namedPipeConnectionString, PIPE_ACCESS_DUPLEX, PIPE_TYPE_MESSAGE | PIPE_WAIT, 1, NULL, NULL, INFINITE, &sa)) == INVALID_HANDLE_VALUE)
            throw SetPipeError("create:", GetLastError());
        if (!ConnectNamedPipe(hPipe, NULL))
            throw SetPipeError("connect:", GetLastError());
        TalkersCommand& param = *((TalkersCommand*)pPrm);
        while (param != EXIT) {
            puts("Connecting to Named Pipe Client ...");
            ConnectNamedPipe(hPipe, NULL);
            while (ReadFile(hPipe, rbuf, sizeof(rbuf), &dwRead, NULL))
            {
                printf("main client message:  %s\n", rbuf);
                param = set_param(rbuf);
                if (param == ALGO_OLD || param == ALGO_NEW)
                {
                    useNewCorrectionAlgo = (param == ALGO_NEW);
                    const char* resp = useNewCorrectionAlgo
                        ? "correction algorithm: NEW (smoothed)"
                        : "correction algorithm: OLD (Cs - Cc)";
                    WriteFile(hPipe, resp, (DWORD)strlen(resp) + 1, &dwWrite, NULL);
                    continue;
                }
                if (param == TIME_LOCAL || param == TIME_NTP)
                {
                    useNtpTime = (param == TIME_NTP);
                    const char* resp = useNtpTime
                        ? "time source: GLOBAL NTP (Unix ms)"
                        : "time source: LOCAL counter (clock)";
                    WriteFile(hPipe, resp, (DWORD)strlen(resp) + 1, &dwWrite, NULL);
                    continue;
                }
                if (param == LOAD_LIB)
                {
                    is_load_library = true;
                    EnterCriticalSection(&scListContact);
                    list_of_dlls.push_front(LoadLibraryA(strstr(rbuf, "Win")));
                    list_of_functions.push_front((FUNCTION)GetProcAddress(list_of_dlls.front(), "SSS"));
                    LeaveCriticalSection(&scListContact);
                }
                else if (param == UNLOAD_LIB)
                {
                    is_load_library = false;
                    EnterCriticalSection(&scListContact);
                    list_of_dlls.pop_front();
                    list_of_functions.pop_front();
                    LeaveCriticalSection(&scListContact);
                }
                if (param == STATISTICS)
                {
                    EnterCriticalSection(&scListContact);
                    int contactSize = contacts.size();
                    LeaveCriticalSection(&scListContact);
                    char sendStastistics[200];
                    sprintf(sendStastistics, "\nStatistics\ncount of connectings :    %d\ncount of denides:        %d\nsuccess end:             %d\ncount of active connections : %d\n", connectionCount, sayNoCount, successConnections, contactSize);
                    WriteFile(hPipe, sendStastistics, sizeof(sendStastistics), &dwWrite, NULL);
                }
                if (param != STATISTICS)
                    WriteFile(hPipe, rbuf, strlen(rbuf) + 1, &dwWrite, NULL);
                if (param == EXIT || param == SHUTDOWN) {
                    break;
                }
            }
            DisconnectNamedPipe(hPipe);
            if (param == EXIT || param == SHUTDOWN) {
                break;
            }
        }
    }
    catch (string ErrorPipeText)
    {
        printf("\n%s", ErrorPipeText.c_str());
        return -1;
    }
    CloseHandle(hPipe);
    puts("shutdown ConsolePipe");
    ExitThread(rc);
}

DWORD WINAPI GarbageCleaner(LPVOID pPrm)
{
    DWORD rc = 0;
    while (*((TalkersCommand*)pPrm) != EXIT) {
        int listSize = 0;
        int howMuchClean = 0;
        EnterCriticalSection(&scListContact);
        if (contacts.size() != 0) {
            for (auto i = contacts.begin(); i != contacts.end();) {
                if (i->type == i->EMPTY) {
                    if (i->sthread == i->FINISH)
                        InterlockedIncrement(&successConnections);
                    if (i->sthread == i->ABORT || i->sthread == i->TIMEOUT)
                        InterlockedIncrement(&sayNoCount);
                    i = contacts.erase(i);
                    howMuchClean++;
                    listSize = contacts.size();
                }
                else ++i;
            }
        }
        LeaveCriticalSection(&scListContact);
    }
    puts("shutdown garbageCleaner");
    ExitThread(rc);
}

HMODULE st;
void CALLBACK ASWTimer(LPVOID Prm, DWORD, DWORD) {
    Contact* contact = (Contact*)(Prm);
    printf("ASWTimer is calling %p\n", contact->hthread);
    TerminateThread(contact->serverHThtead, NULL);
    SendReasonAndClose(contact, "ServiceTimeout");
    EnterCriticalSection(&scListContact);
    CancelWaitableTimer(contact->htimer);
    contact->type = contact->EMPTY;
    contact->sthread = contact->TIMEOUT;
    LeaveCriticalSection(&scListContact);
}

DWORD WINAPI DispatchServer(LPVOID pPrm)
{
    DWORD rc = 0;
    TalkersCommand& command = *(TalkersCommand*)pPrm;
    while (command != EXIT)
    {
        if (command != STOP) {
            WaitForSingleObject(hClientConnectedEvent, INFINITE);
            ResetEvent(hClientConnectedEvent);
            while (true) {
                EnterCriticalSection(&scListContact);
                for (auto i = contacts.begin(); i != contacts.end(); i++) {
                    if (i->type == i->ACCEPT) {
                        char serviceType[10];
                        int r = recv(i->s, serviceType, sizeof(serviceType), NULL);
                        if (r < 1) continue;
                        i->lastActivityTime = time(NULL);
                        i->warningsCount = 0;
                        cout << "New command - " << serviceType << endl;
                        strcpy(i->msg, serviceType);
                        if (!strcmp(i->msg, "close")) {
                            if ((send(i->s, "echo: close", strlen("echo: close") + 1, NULL)) == SOCKET_ERROR)
                                throw  SetErrorMsgText("send:", WSAGetLastError());
                            i->sthread = i->FINISH;
                            i->type = i->EMPTY;
                            continue;
                        }
                        if (!strcmp(i->msg, "exit")) {
                            SendReasonAndClose(&(*i), "ClientExit");
                            i->sthread = i->ABORT;
                            i->type = i->EMPTY;
                            continue;
                        }
                        if (strcmp(i->msg, "Echo") && strcmp(i->msg, "Time") && strcmp(i->msg, "Random")) {
                            SendReasonAndClose(&(*i), "ErrorInquiry");
                            i->sthread = i->ABORT;
                            i->type = i->EMPTY;
                            if (closesocket(i->s) == SOCKET_ERROR)
                                throw  SetErrorMsgText("closesocket:", WSAGetLastError());
                        }
                        else {
                            i->type = i->CONTACT;
                            i->hthread = hAcceptServer;
                            i->serverHThtead = ts(serviceType, (LPVOID) & (*i));
                            i->htimer = CreateWaitableTimer(0, FALSE, 0);
                            LARGE_INTEGER Li;
                            int seconds = 30;
                            Li.QuadPart = -(10000000 * seconds);
                            SetWaitableTimer(i->htimer, &Li, 0, ASWTimer, (LPVOID) & (*i), FALSE);
                            SleepEx(0, TRUE);
                        }
                    }
                    else if (i->type == i->CONTACT) {
                        char bufCheck[5];
                        int rCheck = recv(i->s, bufCheck, sizeof(bufCheck), MSG_PEEK | MSG_DONTWAIT);
                        if (rCheck > 0) {
                            i->lastActivityTime = time(NULL);
                            i->warningsCount = 0;
                        }
                    }
                }
                LeaveCriticalSection(&scListContact);
                Sleep(200);
            }
        }
    }
    puts("shutdown dispatchServer");
    ExitThread(rc);
}

SOCKET sSUDP;
bool PutAnswerToClient(const SETSINCHRO& answer, sockaddr* to, int* lto) {
    if ((sendto(sSUDP, (const char*)&answer, sizeof(answer), NULL, to, *lto)) == SOCKET_ERROR)
        throw  SetErrorMsgText("sendto:", WSAGetLastError());
    return true;
}

bool  GetRequestFromClient(GETSINCHRO* request, SOCKADDR_IN* from, int* flen)
{
    int lc = sizeof(SOCKADDR_IN);
    SOCKADDR_IN clnt;
    ZeroMemory(&clnt, lc);

    int TimeOut = 1000;
    setsockopt(sSUDP, SOL_SOCKET, SO_RCVTIMEO, (char*)&TimeOut, sizeof(TimeOut));

    int lb = recvfrom(sSUDP, (char*)request, sizeof(*request), NULL, (sockaddr*)&clnt, &lc);
    if (lb == SOCKET_ERROR)
        return false;

    if (lb != sizeof(*request) || strncmp(request->cmd, "SINC", 4) != 0)
        return false;

    *from = clnt;
    *flen = lc;
    return true;
}

DWORD WINAPI ResponseServer(LPVOID pPrm)
{
    DWORD rc = 0;
    WSADATA wsaData;
    SOCKADDR_IN serv;
    if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
        throw  SetErrorMsgText("Startup:", WSAGetLastError());
    if ((sSUDP = socket(AF_INET, SOCK_DGRAM, NULL)) == INVALID_SOCKET)
        throw  SetErrorMsgText("socket:", WSAGetLastError());
    serv.sin_family = AF_INET;
    serv.sin_port = htons(serverPort);
    serv.sin_addr.s_addr = INADDR_ANY;
    if (bind(sSUDP, (LPSOCKADDR)&serv, sizeof(serv)) == SOCKET_ERROR)
        throw  SetErrorMsgText("bind:", WSAGetLastError());
    SOCKADDR_IN from;
    int lc = sizeof(from);
    ZeroMemory(&from, lc);

    clock_t startClock = clock();
    __int64 sumRawCorrection = 0;
    __int64 sumUsedCorrection = 0;
    long long requestCount = 0;

    struct ClientAlgoState
    {
        bool synced = false;
        double ema = 0.0;
    };
    std::unordered_map<unsigned long long, ClientAlgoState> perClient;
    const double EMA_ALPHA = 0.2;

    while (*(TalkersCommand*)pPrm != EXIT)
    {
        try
        {
            GETSINCHRO request;
            if (!GetRequestFromClient(&request, &from, &lc))
                continue;

            LONGLONG CsMs;
            if (useNtpTime)
            {
                CsMs = GetLocalSystemTimeMs() + ntpTimeOffsetMs;
            }
            else
            {
                clock_t now = clock();
                CsMs = ((now - startClock) * 1000) / CLOCKS_PER_SEC;
            }
            __int64 Cs = CsMs;

            __int64 rawCorrection = Cs - request.curvalue;
            ++requestCount;
            sumRawCorrection += rawCorrection;

            __int64 correction;
            if (useNewCorrectionAlgo)
            {
                unsigned long long key =
                    ((unsigned long long)from.sin_addr.S_un.S_addr << 16) |
                    (unsigned long long)(unsigned short)from.sin_port;

                ClientAlgoState& st = perClient[key];
                if (!st.synced)
                {
                    correction = rawCorrection;
                    st.synced = true;
                    st.ema = 0.0;
                }
                else
                {
                    st.ema = st.ema + EMA_ALPHA * ((double)rawCorrection - st.ema);
                    correction = (__int64)llround(st.ema);
                }
            }
            else
            {
                correction = rawCorrection;
            }

            SETSINCHRO answer;
            memcpy(answer.cmd, "SINC", 4);
            answer.correction = correction;

            PutAnswerToClient(answer, (sockaddr*)&from, &lc);

            sumUsedCorrection += correction;
            double avgCorrection = (requestCount > 0) ? (double)sumUsedCorrection / requestCount : 0.0;

            printf("UDP client %s:%d, request #%lld, correction=%I64d, average correction=%.2f\n",
                inet_ntoa(from.sin_addr),
                htons(from.sin_port),
                requestCount,
                correction,
                avgCorrection);
        }
        catch (string errorMsgText)
        {
            printf("\n%s", errorMsgText.c_str());
        }
    }
    if (closesocket(sSUDP) == SOCKET_ERROR)
        throw  SetErrorMsgText("closesocket:", WSAGetLastError());
    if (WSACleanup() == SOCKET_ERROR)
        throw  SetErrorMsgText("Cleanup:", WSAGetLastError());
    ExitThread(rc);
}

DWORD WINAPI AutoDisconnectThread(LPVOID pPrm)
{
    while (*((TalkersCommand*)pPrm) != EXIT)
    {
        Sleep(1000);
        EnterCriticalSection(&scListContact);
        for (auto i = contacts.begin(); i != contacts.end(); i++)
        {
            if (i->type == i->CONTACT || i->type == i->ACCEPT)
            {
                double idleTime = difftime(time(NULL), i->lastActivityTime);
                if (idleTime >= 1000.0)
                {
                    printf("AutoDisconnect: client %d is disconnected due to inactivity\n", i->s);
                    SendReasonAndClose(&(*i), "IdleTimeout");
                    closesocket(i->s);
                    i->sthread = i->ABORT;
                    i->type = i->EMPTY;
                }
                else
                {
                    int newWarnings = (int)(idleTime / 100.0);
                    if (newWarnings > i->warningsCount)
                    {
                        i->warningsCount = newWarnings;
                        printf("Warning %d for client %d due to inactivity\n", i->warningsCount, i->s);
                        if (i->warningsCount >= 10)
                        {
                            printf("AutoDisconnect: client %d is forcibly disconnected. Too many warnings.\n", i->s);
                            SendReasonAndClose(&(*i), "TooManyWarnings");
                            closesocket(i->s);
                            i->sthread = i->ABORT;
                            i->type = i->EMPTY;
                        }
                    }
                }
            }
        }
        LeaveCriticalSection(&scListContact);
    }
    puts("shutdown AutoDisconnectThread");
    ExitThread(0);
}

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "rus");
    acceptEnabled = true;
    if (argc == 2) {
        serverPort = atoi(argv[1]);
    }
    else if (argc == 3) {
        serverPort = atoi(argv[1]);
        strcpy(dllName, argv[2]);
    }
    else if (argc == 4) {
        serverPort = atoi(argv[1]);
        strcpy(dllName, argv[2]);
        strcpy(namedPipeName, argv[3]);
    }
    else {
        serverPort = 2000;
        strcpy(dllName, "ServiceLibrary.dll");
        strcpy(namedPipeName, "BOX");
    }
    printf("server port %d\n", serverPort);

    st = LoadLibraryA(dllName);
    if (st == NULL) {
        printf("�� ������� ��������� DLL: %s. ������: %d\n", dllName, GetLastError());
        return -1;
    }

    ts = (FUNCTION)GetProcAddress(st, "SSS");
    if (ts == NULL) {
        printf("�� ������� ����� ������� SSS � DLL: %s. ������: %d\n", dllName, GetLastError());
        FreeLibrary(st);
        return -1;
    }

    volatile TalkersCommand cmd = START;
    InitializeCriticalSection(&scListContact);

    hAcceptServer = CreateThread(NULL, 0, AcceptServer, (LPVOID)&cmd, 0, NULL);
    hConsolePipe = CreateThread(NULL, 0, ConsolePipe, (LPVOID)&cmd, 0, NULL);
    hGarbageCleaner = CreateThread(NULL, 0, GarbageCleaner, (LPVOID)&cmd, 0, NULL);
    hDispatchServer = CreateThread(NULL, 0, DispatchServer, (LPVOID)&cmd, 0, NULL);
    hResponseServer = CreateThread(NULL, 0, ResponseServer, (LPVOID)&cmd, 0, NULL);
    HANDLE hAutoDisconnect = CreateThread(NULL, 0, AutoDisconnectThread, (LPVOID)&cmd, 0, NULL);

    SetThreadPriority(hGarbageCleaner, THREAD_PRIORITY_BELOW_NORMAL);
    SetThreadPriority(hDispatchServer, THREAD_PRIORITY_NORMAL);
    SetThreadPriority(hConsolePipe, THREAD_PRIORITY_NORMAL);
    SetThreadPriority(hResponseServer, THREAD_PRIORITY_NORMAL);
    SetThreadPriority(hAcceptServer, THREAD_PRIORITY_HIGHEST);

    WaitForSingleObject(hAcceptServer, INFINITE);
    CloseHandle(hAcceptServer);
    WaitForSingleObject(hConsolePipe, INFINITE);
    CloseHandle(hConsolePipe);
    WaitForSingleObject(hGarbageCleaner, INFINITE);
    CloseHandle(hGarbageCleaner);
    TerminateThread(hDispatchServer, 0);
    puts("shutdown dispatchServer");
    TerminateThread(hResponseServer, 0);
    puts("shutdown responseServer");
    TerminateThread(hAutoDisconnect, 0);
    puts("shutdown AutoDisconnectThread");
    CloseHandle(hDispatchServer);
    CloseHandle(hResponseServer);
    CloseHandle(hAutoDisconnect);

    DeleteCriticalSection(&scListContact);
    FreeLibrary(st);
    return 0;
}