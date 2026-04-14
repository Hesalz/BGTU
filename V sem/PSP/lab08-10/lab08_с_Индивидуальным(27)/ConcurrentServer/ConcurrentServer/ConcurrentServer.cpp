#include "stdafx.h"
#include <ws2tcpip.h>
#include "Winsock2.h"
#include "ErrorFunctions.h"
#include <string>
#include <list>
#include <time.h>
#include <iostream>
#include <cctype>
#include <cstdio>
#include <errno.h>
#include <windows.h>
#define _CRT_SECURE_NO_WARNINGS // Сервер падает по истечению времени расписания, надо исправлять.
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
HANDLE hAcceptServer, hConsolePipe, hGarbageCleaner, hDispatchServer, hResponseServer, hMailReader;
HANDLE hScheduleMailslot = INVALID_HANDLE_VALUE;
HANDLE hClientConnectedEvent = CreateEvent(NULL, FALSE, FALSE, L"ClientConnected");
DWORD WINAPI AcceptServer(LPVOID pPrm);
DWORD WINAPI ConsolePipe(LPVOID pPrm);
DWORD WINAPI GarbageCleaner(LPVOID pPrm);
DWORD WINAPI DispatchServer(LPVOID pPrm);
DWORD WINAPI ResponseServer(LPVOID pPrm);
DWORD WINAPI MailReader(LPVOID pPrm);
CRITICAL_SECTION scListContact;

struct Contact;
void SendReasonAndClose(Contact* c, const char* reason);

enum TalkersCommand {
    START, STOP, EXIT, STATISTICS, WAIT, SHUTDOWN, GETCOMMAND, LOAD_LIB, UNLOAD_LIB, SCHEDULE
};
volatile TalkersCommand  previousCommand = GETCOMMAND;

struct ScheduleEntry {
    char serviceName[20];
    int startHour;
    int startMinute;
    int endHour;
    int endMinute;
    
    ScheduleEntry() {
        serviceName[0] = '\0';
        startHour = 0;
        startMinute = 0;
        endHour = 23;
        endMinute = 59;
    }
    
    ScheduleEntry(const char* name, int sh, int sm, int eh, int em) {
        strcpy_s(serviceName, name);
        startHour = sh;
        startMinute = sm;
        endHour = eh;
        endMinute = em;
    }
};

std::list<ScheduleEntry> serviceSchedule;
CRITICAL_SECTION scSchedule;
volatile int mailCheckInterval = 60;

void NormalizeServiceNameForCompare(char* dest, const char* source, size_t destSize) {
    if (source == NULL || strlen(source) == 0) {
        dest[0] = '\0';
        return;
    }
    
    size_t len = strlen(source);
    if (len >= destSize) len = destSize - 1;
    
    for (size_t i = 0; i < len; i++) {
        dest[i] = tolower(source[i]);
    }
    dest[len] = '\0';
    
    if (!strcmp(dest, "echo")) {
        strcpy_s(dest, destSize, "echo");
    } else if (!strcmp(dest, "time")) {
        strcpy_s(dest, destSize, "time");
    } else if (!strcmp(dest, "random") || !strcmp(dest, "rand")) {
        strcpy_s(dest, destSize, "random");
    }
}

bool IsServiceAllowed(const char* serviceName) {
    EnterCriticalSection(&scSchedule);
    
    printf("IsServiceAllowed: Checking service %s, schedule size: %d\n", serviceName, (int)serviceSchedule.size());
    
    if (serviceSchedule.empty()) {
        printf("IsServiceAllowed: Schedule is empty, allowing all services\n");
        LeaveCriticalSection(&scSchedule);
        return true;
    }
    
    char normalizedService[20];
    NormalizeServiceNameForCompare(normalizedService, serviceName, sizeof(normalizedService));
    
    bool found = false;
    bool allowed = false;
    
    for (auto it = serviceSchedule.begin(); it != serviceSchedule.end(); ++it) {
        char normalizedSchedule[20];
        NormalizeServiceNameForCompare(normalizedSchedule, it->serviceName, sizeof(normalizedSchedule));
        
        if (!strcmp(normalizedSchedule, normalizedService)) {
            found = true;
            time_t now = time(NULL);
            struct tm timeinfo;
            localtime_s(&timeinfo, &now);
            int currentHour = timeinfo.tm_hour;
            int currentMinute = timeinfo.tm_min;
            
            int currentTime = currentHour * 60 + currentMinute;
            int startTime = it->startHour * 60 + it->startMinute;
            int endTime = it->endHour * 60 + it->endMinute;
            
            if (startTime <= endTime) {
                allowed = (currentTime >= startTime && currentTime <= endTime);
            } else {
                allowed = (currentTime >= startTime || currentTime <= endTime);
            }
            
            printf("IsServiceAllowed: Service %s (normalized: %s) found in schedule. Current time: %02d:%02d, Schedule: %02d:%02d-%02d:%02d, Allowed: %s\n",
                   serviceName, normalizedService, currentHour, currentMinute, 
                   it->startHour, it->startMinute, it->endHour, it->endMinute, 
                   allowed ? "YES" : "NO");
            break;
        }
    }
    
    if (!found) {
        allowed = false;
        printf("IsServiceAllowed: Service %s (normalized: %s) NOT found in schedule. Access DENIED.\n", 
               serviceName, normalizedService);
    }
    
    LeaveCriticalSection(&scSchedule);
    return allowed;
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

void SendReasonAndClose(Contact* c, const char* reason) {
    if (c == NULL || c->s == INVALID_SOCKET) {
        printf("SendReasonAndClose: Invalid contact or socket\n");
        return;
    }
    
    int sendRc = send(c->s, reason, (int)strlen(reason) + 1, NULL);
    if (sendRc == SOCKET_ERROR) {
        int error = WSAGetLastError();
        if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
            printf("SendReasonAndClose: Failed to send reason '%s' to client, error: %s (%d)\n", 
                   reason, GetErrorMsgText(error).c_str(), error);
        }
    } else {
        if (shutdown(c->s, SD_SEND) == SOCKET_ERROR) {
            int error = WSAGetLastError();
            if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
                printf("SendReasonAndClose: Failed to shutdown socket, error: %s (%d)\n", 
                       GetErrorMsgText(error).c_str(), error);
            }
        }
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
                    if (shutdown(c.s, SD_SEND) == SOCKET_ERROR) {
                        int error = WSAGetLastError();
                        if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
                            printf("Warning: Failed to shutdown socket in rejectNew, error: %s (%d)\n", 
                                   GetErrorMsgText(error).c_str(), error);
                        }
                    }
                    Sleep(500);
                }
                else {
                    int error = WSAGetLastError();
                    if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
                        printf("Failed to send ServerStopped to client, error: %s (%d)\n", 
                               GetErrorMsgText(error).c_str(), error);
                    }
                }
                if (closesocket(c.s) == SOCKET_ERROR) {
                    int error = WSAGetLastError();
                    if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                        printf("Warning: Failed to close socket in rejectNew, error: %s (%d)\n", 
                               GetErrorMsgText(error).c_str(), error);
                    }
                }
                InterlockedIncrement(&sayNoCount);
            }
            else if (waitMode) {
                printf("Rejecting new client %s:%d (server in wait)\n", inet_ntoa(c.prms.sin_addr), htons(c.prms.sin_port));
                const char* rejectMsg = "ServerWait";
                int sendResult = send(c.s, rejectMsg, (int)strlen(rejectMsg) + 1, NULL);
                if (sendResult != SOCKET_ERROR) {
                    if (shutdown(c.s, SD_SEND) == SOCKET_ERROR) {
                        int error = WSAGetLastError();
                        if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
                            printf("Warning: Failed to shutdown socket in waitMode, error: %s (%d)\n", 
                                   GetErrorMsgText(error).c_str(), error);
                        }
                    }
                    Sleep(500);
                }
                else {
                    int error = WSAGetLastError();
                    if (error != WSAENOTCONN && error != WSAECONNRESET && error != WSAESHUTDOWN) {
                        printf("Failed to send ServerWait to client, error: %s (%d)\n", 
                               GetErrorMsgText(error).c_str(), error);
                    }
                }
                if (closesocket(c.s) == SOCKET_ERROR) {
                    int error = WSAGetLastError();
                    if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                        printf("Warning: Failed to close socket in waitMode, error: %s (%d)\n", 
                               GetErrorMsgText(error).c_str(), error);
                    }
                }
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
        } else {
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

void NormalizeServiceName(char* serviceName) {
    if (strlen(serviceName) > 0) {
        for (int i = 0; i < (int)strlen(serviceName); i++) {
            serviceName[i] = tolower(serviceName[i]);
        }
        
        if (!strcmp(serviceName, "echo")) {
            strcpy_s(serviceName, 20, "Echo");
        } else if (!strcmp(serviceName, "time")) {
            strcpy_s(serviceName, 20, "Time");
        } else if (!strcmp(serviceName, "random") || !strcmp(serviceName, "rand")) {
            strcpy_s(serviceName, 20, "Random");
        } else {
            serviceName[0] = toupper(serviceName[0]);
        }
    }
}

bool ParseSchedule(const char* scheduleStr) {
    if (scheduleStr == NULL || strlen(scheduleStr) == 0) {
        printf("ParseSchedule: Empty schedule string\n");
        return false;
    }
    
    EnterCriticalSection(&scSchedule);
    
    try {
        serviceSchedule.clear();
        
        char buffer[1000];
        if (strlen(scheduleStr) >= sizeof(buffer)) {
            printf("ParseSchedule: Schedule string too long\n", sizeof(buffer) - 1);
            LeaveCriticalSection(&scSchedule);
            return false;
        }
        strcpy_s(buffer, scheduleStr);
        
        char* context = NULL;
        char* token = strtok_s(buffer, " ", &context);
        
        if (token && !strcmp(token, "schedule")) {
            token = strtok_s(NULL, " ", &context);
        }
        
        int entriesAdded = 0;
        while (token != NULL) {
            char serviceName[20];
            if (strlen(token) >= sizeof(serviceName)) {
                printf("ParseSchedule: Service name too long: %s (skipping)\n", token);
                token = strtok_s(NULL, " ", &context);
                continue;
            }
            strcpy_s(serviceName, token);
            NormalizeServiceName(serviceName);
            
            token = strtok_s(NULL, " ", &context);
            if (token == NULL) {
                printf("ParseSchedule: Incomplete entry for service %s (missing start time)\n", serviceName);
                break;
            }
            
            int startHour, startMinute;
            if (sscanf_s(token, "%d:%d", &startHour, &startMinute) != 2) {
                printf("ParseSchedule: Invalid start time format for service %s: %s (expected HH:MM)\n", serviceName, token);
                token = strtok_s(NULL, " ", &context);
                continue;
            }
            
            if (startHour < 0 || startHour > 23 || startMinute < 0 || startMinute > 59) {
                printf("ParseSchedule: Invalid start time for service %s: %02d:%02d (must be 00:00-23:59)\n", serviceName, startHour, startMinute);
                token = strtok_s(NULL, " ", &context);
                continue;
            }
            
            token = strtok_s(NULL, " ", &context);
            if (token == NULL) {
                printf("ParseSchedule: Incomplete entry for service %s (missing end time)\n", serviceName);
                break;
            }
            
            int endHour, endMinute;
            if (sscanf_s(token, "%d:%d", &endHour, &endMinute) != 2) {
                printf("ParseSchedule: Invalid end time format for service %s: %s (expected HH:MM)\n", serviceName, token);
                token = strtok_s(NULL, " ", &context);
                continue;
            }
            
            if (endHour < 0 || endHour > 23 || endMinute < 0 || endMinute > 59) {
                printf("ParseSchedule: Invalid end time for service %s: %02d:%02d (must be 00:00-23:59)\n", serviceName, endHour, endMinute);
                token = strtok_s(NULL, " ", &context);
                continue;
            }
            
            serviceSchedule.push_back(ScheduleEntry(serviceName, startHour, startMinute, endHour, endMinute));
            printf("Schedule added: %s %02d:%02d - %02d:%02d\n", serviceName, startHour, startMinute, endHour, endMinute);
            entriesAdded++;
            
            token = strtok_s(NULL, " ", &context);
        }
        
        if (entriesAdded == 0) {
            printf("ParseSchedule: No valid schedule entries found in: %s\n", scheduleStr);
            LeaveCriticalSection(&scSchedule);
            return false;
        }
        
        printf("ParseSchedule: Successfully parsed %d schedule entries\n", entriesAdded);
        printf("ParseSchedule: Current schedule contents:\n");
        for (auto it = serviceSchedule.begin(); it != serviceSchedule.end(); ++it) {
            printf("  - %s: %02d:%02d - %02d:%02d\n", it->serviceName, it->startHour, it->startMinute, it->endHour, it->endMinute);
        }
        LeaveCriticalSection(&scSchedule);
        return true;
    }
    catch (...) {
        printf("ParseSchedule: Exception occurred while parsing schedule\n");
        serviceSchedule.clear();
        LeaveCriticalSection(&scSchedule);
        return false;
    }
}

TalkersCommand set_param(char* param) {
    if (!strcmp(param, "start")) return START;
    if (!strcmp(param, "stop")) return STOP;
    if (!strcmp(param, "exit")) return EXIT;
    if (!strcmp(param, "wait")) return WAIT;
    if (!strcmp(param, "shutdown")) return SHUTDOWN;
    if (!strcmp(param, "statistics")) return STATISTICS;
    if (!strcmp(param, "getcommand")) return GETCOMMAND;
    if (strstr(param, "schedule")) return SCHEDULE;
    if (strstr(param, "UNLOAD_LIB")) return UNLOAD_LIB;
    if (strstr(param, "LOAD_LIB")) return LOAD_LIB;
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
                    sprintf_s(sendStastistics, "\nStatistics\ncount of connectings :    %d\ncount of denides:        %d\nsuccess end:             %d\ncount of active connections : %d\n", connectionCount, sayNoCount, successConnections, contactSize);
                    WriteFile(hPipe, sendStastistics, sizeof(sendStastistics), &dwWrite, NULL);
                }
                else if (param == SCHEDULE)
                {
                    printf("ConsolePipe: Received schedule command: %s\n", rbuf);
                    bool parseSuccess = ParseSchedule(rbuf);
                    char response[200];
                    if (parseSuccess) {
                        sprintf_s(response, "Schedule updated successfully");
                        printf("ConsolePipe: Schedule parsed successfully\n");
                    } else {
                        sprintf_s(response, "Schedule update failed: invalid format or empty schedule");
                        printf("ConsolePipe: Schedule parsing failed\n");
                    }
                    
                    if (!WriteFile(hPipe, response, strlen(response) + 1, &dwWrite, NULL)) {
                        int error = GetLastError();
                        printf("ConsolePipe: Failed to send schedule response, error: %d\n", error);
                    }
                }
                if (param != STATISTICS && param != SCHEDULE)
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
                            if (i->s != INVALID_SOCKET) {
                                if (closesocket(i->s) == SOCKET_ERROR) {
                                    int error = WSAGetLastError();
                                    if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                                        printf("Warning: Failed to close socket after ErrorInquiry, error: %s (%d)\n", 
                                               GetErrorMsgText(error).c_str(), error);
                                    }
                                }
                                i->s = INVALID_SOCKET;
                            }
                        }
                        else {
                            printf("DispatchServer: Checking schedule for service: %s\n", i->msg);
                            bool isAllowed = IsServiceAllowed(i->msg);
                            printf("DispatchServer: Service %s allowed: %s\n", i->msg, isAllowed ? "YES" : "NO");
                            
                            if (!isAllowed) {
                                time_t now = time(NULL);
                                struct tm timeinfo;
                                localtime_s(&timeinfo, &now);
                                char errorMsg[100];
                                sprintf_s(errorMsg, "ServiceNotAvailable: %s is not available at %02d:%02d", i->msg, timeinfo.tm_hour, timeinfo.tm_min);
                                
                                printf("Service %s rejected: not in schedule (current time: %02d:%02d)\n", 
                                       i->msg, timeinfo.tm_hour, timeinfo.tm_min);
                                
                                SendReasonAndClose(&(*i), errorMsg);
                                
                                i->sthread = i->ABORT;
                                i->type = i->EMPTY;
                                
                                if (i->s != INVALID_SOCKET) {
                                    if (closesocket(i->s) == SOCKET_ERROR) {
                                        int error = WSAGetLastError();
                                        if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                                            printf("Warning: Failed to close socket after schedule rejection, error: %s (%d)\n", 
                                                   GetErrorMsgText(error).c_str(), error);
                                        }
                                    }
                                    i->s = INVALID_SOCKET;
                                }
                                continue;
                            }
                            
                            printf("DispatchServer: Service %s allowed, proceeding with connection\n", i->msg);
                            
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
bool PutAnswerToClient(char* name, sockaddr* to, int* lto) {
    char msg[] = "Hello";
    if ((sendto(sSUDP, msg, sizeof(msg) + 1, NULL, to, *lto)) == SOCKET_ERROR)
        throw  SetErrorMsgText("sendto:", WSAGetLastError());
    return false;
}

bool  GetRequestFromClient(char* name, short port, SOCKADDR_IN* from, int* flen)
{
    SOCKADDR_IN clnt;
    int lc = sizeof(clnt);
    ZeroMemory(&clnt, lc);
    char ibuf[500];
    int  lb = 0;
    int optval = 1;
    int TimeOut = 10;
    setsockopt(sSUDP, SOL_SOCKET, SO_BROADCAST, (char*)&optval, sizeof(int));
    setsockopt(sSUDP, SOL_SOCKET, SO_RCVTIMEO, (char*)&TimeOut, sizeof(TimeOut));
    while (true) {
        if ((lb = recvfrom(sSUDP, ibuf, sizeof(ibuf), NULL, (sockaddr*)&clnt, &lc)) == SOCKET_ERROR) return false;
        ibuf[lb] = '\0';
        cout << ibuf << endl;
        if (strcmp(name, ibuf) == 0) {
            *from = clnt;
            *flen = lc;
            return true;
        }
        puts("\nIncorrect callname");
    }
    return false;
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
    int numberOfClients = 0;
    char call[50] = "Hello";
    while (*(TalkersCommand*)pPrm != EXIT)
    {
        try
        {
            if (GetRequestFromClient(call, serverPort, &from, &lc))
            {
                if (opened) printf("\nconnected Client: %d, port: %d, address: %s\n", ++numberOfClients, htons(from.sin_port), inet_ntoa(from.sin_addr));
                PutAnswerToClient(call, (sockaddr*)&from, &lc);
                InterlockedIncrement(&sayNoCount);
            }
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
                if (idleTime >= 30.0)
                {
                    printf("AutoDisconnect: client %d is disconnected due to inactivity\n", i->s);
                    SendReasonAndClose(&(*i), "IdleTimeout");
                    if (i->s != INVALID_SOCKET) {
                        if (closesocket(i->s) == SOCKET_ERROR) {
                            int error = WSAGetLastError();
                            if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                                printf("Warning: Failed to close socket in AutoDisconnect (IdleTimeout), error: %s (%d)\n", 
                                       GetErrorMsgText(error).c_str(), error);
                            }
                        }
                        i->s = INVALID_SOCKET;
                    }
                    i->sthread = i->ABORT;
                    i->type = i->EMPTY;
                }
                else
                {
                    int newWarnings = (int)(idleTime / 18.0);
                    if (newWarnings > i->warningsCount)
                    {
                        i->warningsCount = newWarnings;
                        printf("Warning %d for client %d due to inactivity\n", i->warningsCount, i->s);
                        if (i->warningsCount >= 10)
                        {
                            printf("AutoDisconnect: client %d is forcibly disconnected. Too many warnings.\n", i->s);
                            SendReasonAndClose(&(*i), "TooManyWarnings");
                            if (i->s != INVALID_SOCKET) {
                                if (closesocket(i->s) == SOCKET_ERROR) {
                                    int error = WSAGetLastError();
                                    if (error != WSAENOTSOCK && error != WSAENOTCONN) {
                                        printf("Warning: Failed to close socket in AutoDisconnect (TooManyWarnings), error: %s (%d)\n", 
                                               GetErrorMsgText(error).c_str(), error);
                                    }
                                }
                                i->s = INVALID_SOCKET;
                            }
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

DWORD WINAPI MailReader(LPVOID pPrm)
{
    DWORD rc = 0;
    const char* mailslotName = "\\\\.\\mailslot\\ServerSchedule";
    
    hScheduleMailslot = CreateMailslotA(
        mailslotName,
        0,
        MAILSLOT_WAIT_FOREVER,
        NULL               
    );
    
    if (hScheduleMailslot == INVALID_HANDLE_VALUE) {
        int error = GetLastError();
        printf("MailReader: Failed to create Mailslot %s, error: %d\n", mailslotName, error);
        ExitThread(rc);
        return rc;
    }
    
    printf("MailReader: Mailslot %s created successfully\n", mailslotName);
    
    while (*((TalkersCommand*)pPrm) != EXIT)
    {
        DWORD bytesRead = 0;
        DWORD messageCount = 0;
        DWORD nextSize = 0;
        char buffer[1000];
        
        if (GetMailslotInfo(hScheduleMailslot, NULL, &nextSize, &messageCount, NULL)) {
            while (messageCount > 0) {
                if (ReadFile(hScheduleMailslot, buffer, sizeof(buffer) - 1, &bytesRead, NULL)) {
                    buffer[bytesRead] = '\0';
   
                    size_t len = strlen(buffer);
                    if (len > 0 && buffer[len - 1] == '\n')
                        buffer[len - 1] = '\0';
                    if (len > 0 && buffer[len - 1] == '\r')
                        buffer[len - 1] = '\0';
                    
                    printf("MailReader: Received message from Mailslot (%d bytes): %s\n", bytesRead, buffer);

                    if (strstr(buffer, "schedule") != NULL)
                    {
                        printf("MailReader: New schedule received from Mailslot\n");
                        bool parseSuccess = ParseSchedule(buffer);
                        if (!parseSuccess) {
                            printf("MailReader: Failed to parse schedule from Mailslot\n");
                        }
                    } else {
                        printf("MailReader: Mailslot message does not contain valid schedule command\n");
                    }
                } else {
                    int error = GetLastError();
                    if (error != ERROR_INSUFFICIENT_BUFFER) {
                        printf("MailReader: Error reading from Mailslot, error: %d\n", error);
                        break;
                    } else {
                        printf("MailReader: Message too large, skipping\n");
                        if (nextSize > 0 && nextSize < 10000) {
                            char* largeBuffer = new char[nextSize + 1];
                            ReadFile(hScheduleMailslot, largeBuffer, nextSize, &bytesRead, NULL);
                            delete[] largeBuffer;
                        }
                    }
                }
                
                if (!GetMailslotInfo(hScheduleMailslot, NULL, &nextSize, &messageCount, NULL)) {
                    break;
                }
            }
        } else {
            int error = GetLastError();
            if (error != ERROR_INSUFFICIENT_BUFFER) {
                printf("MailReader: Error getting Mailslot info, error: %d\n", error);
            }
        }
        
        Sleep(mailCheckInterval * 1000);
    }
    
    if (hScheduleMailslot != INVALID_HANDLE_VALUE) {
        CloseHandle(hScheduleMailslot);
        hScheduleMailslot = INVALID_HANDLE_VALUE;
    }
    
    puts("shutdown MailReader");
    ExitThread(rc);
    return rc;
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
    InitializeCriticalSection(&scSchedule);

    hAcceptServer = CreateThread(NULL, 0, AcceptServer, (LPVOID)&cmd, 0, NULL);
    hConsolePipe = CreateThread(NULL, 0, ConsolePipe, (LPVOID)&cmd, 0, NULL);
    hGarbageCleaner = CreateThread(NULL, 0, GarbageCleaner, (LPVOID)&cmd, 0, NULL);
    hDispatchServer = CreateThread(NULL, 0, DispatchServer, (LPVOID)&cmd, 0, NULL);
    hResponseServer = CreateThread(NULL, 0, ResponseServer, (LPVOID)&cmd, 0, NULL);
    HANDLE hAutoDisconnect = CreateThread(NULL, 0, AutoDisconnectThread, (LPVOID)&cmd, 0, NULL);
    hMailReader = CreateThread(NULL, 0, MailReader, (LPVOID)&cmd, 0, NULL);

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
    TerminateThread(hMailReader, 0);
    puts("shutdown MailReader");
    if (hScheduleMailslot != INVALID_HANDLE_VALUE) {
        CloseHandle(hScheduleMailslot);
        hScheduleMailslot = INVALID_HANDLE_VALUE;
    }
    CloseHandle(hDispatchServer);
    CloseHandle(hResponseServer);
    CloseHandle(hAutoDisconnect);
    CloseHandle(hMailReader);

    DeleteCriticalSection(&scListContact);
    DeleteCriticalSection(&scSchedule);
    FreeLibrary(st);
    return 0;
}