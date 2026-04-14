#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include "Winsock2.h"
#include <windows.h>

#include <algorithm>
#include <iostream>
#include <string>
#pragma comment(lib, "WS2_32.lib")

int countServers = 0;

std::string GetErrorMsgText(int code)
{
    std::string msgText;
    switch (code)
    {
    case WSAEINTR: msgText = "Работа функции прервана"; break;
    case WSAEACCES: msgText = "Разрешение отвергнуто"; break;
    case WSAEFAULT: msgText = "Ошибочный адрес"; break;
    case WSAEINVAL: msgText = "Ошибка в аргументе"; break;
    case WSAEMFILE: msgText = "Открыто слишком много файлов"; break;
    case WSAEWOULDBLOCK: msgText = "Ресурс временно недоступен"; break;
    case WSAEINPROGRESS: msgText = "Операция в процессе развития"; break;
    case WSAEALREADY: msgText = "Операция уже выполняется"; break;
    case WSAENOTSOCK: msgText = "Сокет задан неправильно"; break;
    case WSAEDESTADDRREQ: msgText = "Требуется адрес расположения"; break;
    case WSAEMSGSIZE: msgText = "Сообщение слишком длинное"; break;
    case WSAEPROTOTYPE: msgText = "Неправильный тип протокола для сокета"; break;
    case WSAENOPROTOOPT: msgText = "Ошибка в опции протокола"; break;
    case WSAEPROTONOSUPPORT: msgText = "Протокол не поддерживается"; break;
    case WSAESOCKTNOSUPPORT: msgText = "Тип сокета не поддерживается"; break;
    case WSAEOPNOTSUPP: msgText = "Операция не поддерживается"; break;
    case WSAEPFNOSUPPORT: msgText = "Тип протоколов не поддерживается"; break;
    case WSAEAFNOSUPPORT: msgText = "Тип адресов не поддерживается протоколом"; break;
    case WSAEADDRINUSE: msgText = "Адрес уже используется"; break;
    case WSAEADDRNOTAVAIL: msgText = "Запрошенный адрес не может быть использован"; break;
    case WSAENETDOWN: msgText = "Сеть отключена"; break;
    case WSAENETUNREACH: msgText = "Сеть недостижима"; break;
    case WSAENETRESET: msgText = "Сеть разорвала соединение"; break;
    case WSAECONNABORTED: msgText = "Программный отказ связи"; break;
    case WSAECONNRESET: msgText = "Связь не восстановлена"; break;
    case WSAENOBUFS: msgText = "Не хватает памяти для буферов"; break;
    case WSAEISCONN: msgText = "Сокет уже подключен"; break;
    case WSAENOTCONN: msgText = "Сокет не подключен"; break;
    case WSAESHUTDOWN: msgText = "Нельзя выполнить send: сокет завершил работу"; break;
    case WSAETIMEDOUT: msgText = "Закончился отведенный интервал времени"; break;
    case WSAECONNREFUSED: msgText = "Соединение отклонено"; break;
    case WSAEHOSTDOWN: msgText = "Хост в неработоспособном состоянии"; break;
    case WSAEHOSTUNREACH: msgText = "Нет маршрута для хоста"; break;
    case WSAEPROCLIM: msgText = "Слишком много процессов"; break;
    case WSASYSNOTREADY: msgText = "Сеть недоступна"; break;
    case WSAVERNOTSUPPORTED: msgText = "Данная версия недоступна"; break;
    case WSANOTINITIALISED: msgText = "Не выполнена инициализация WS2_32.dll"; break;
    case WSAEDISCON: msgText = "Выполняется отключение"; break;
    case WSATYPE_NOT_FOUND: msgText = "Класс не найден"; break;
    case WSAHOST_NOT_FOUND: msgText = "Хост не найден"; break;
    case WSATRY_AGAIN: msgText = "Неавторизованный хост не найден"; break;
    case WSANO_RECOVERY: msgText = "Неопределенная ошибка"; break;
    case WSANO_DATA: msgText = "Нет записи запрошенного типа"; break;
    case WSA_INVALID_HANDLE: msgText = "Указанный дескриптор события с ошибкой"; break;
    case WSA_INVALID_PARAMETER: msgText = "Один или более параметров с ошибкой"; break;
    case WSA_IO_INCOMPLETE: msgText = "Объект ввода-вывода не в сигнальном состоянии"; break;
    case WSA_IO_PENDING: msgText = "Операция завершится позже"; break;
    case WSA_NOT_ENOUGH_MEMORY: msgText = "Не достаточно памяти"; break;
    case WSA_OPERATION_ABORTED: msgText = "Операция отвергнута"; break;
    case WSASYSCALLFAILURE: msgText = "Аварийное завершение системного вызова"; break;
    default:
        msgText = "Неизвестная ошибка: " + std::to_string(code);
        break;
    }
    return msgText;
}

std::string SetErrorMsgText(const std::string& msgText, int code)
{
    return msgText + ": " + GetErrorMsgText(code);
}

bool GetRequestFromClient(const char* name, SOCKET s, struct sockaddr* from, int* flen)
{
    char buf[50];
    int length;
    std::cout << "\nWait message..." << std::endl;

    if ((length = recvfrom(s, buf, sizeof(buf) - 1, NULL, from, flen)) == SOCKET_ERROR)
    {
        if (WSAGetLastError() == WSAETIMEDOUT)
            return false;
        else
            throw SetErrorMsgText("Recvfrom", WSAGetLastError());
    }

    buf[length] = '\0';
    std::cout << "Received: '" << buf << "' length: " << length << std::endl;

    return strcmp(buf, name) == 0;
}

bool PutAnswerToClient(const char* name, SOCKET s, struct sockaddr* to, int* lto)
{
    int length = strlen(name);
    if (sendto(s, name, length + 1, NULL, to, *lto) == SOCKET_ERROR)
    {
        throw SetErrorMsgText("Sendto", WSAGetLastError());
    }
    return true;
}

void GetServer(const char* call, short port, struct sockaddr* from, int* flen)
{
    SOCKET cC;
    SOCKADDR_IN all;
    int timeout = 5000;
    int optval = 1;
    char buf[50];

    try
    {
        if ((cC = socket(AF_INET, SOCK_DGRAM, NULL)) == INVALID_SOCKET)
            throw SetErrorMsgText("socket", WSAGetLastError());

        if (setsockopt(cC, SOL_SOCKET, SO_BROADCAST, (char*)&optval, sizeof(int)) == SOCKET_ERROR)
            throw SetErrorMsgText("setsockopt", WSAGetLastError());

        if (setsockopt(cC, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(int)) == SOCKET_ERROR)
            throw SetErrorMsgText("setsockopt", WSAGetLastError());

        all.sin_family = AF_INET;
        all.sin_port = htons(port);
        all.sin_addr.s_addr = INADDR_BROADCAST;

        if (sendto(cC, call, strlen(call) + 1, NULL, (sockaddr*)&all, sizeof(all)) == SOCKET_ERROR)
            throw SetErrorMsgText("sendto", WSAGetLastError());

        while (true)
        {
            int length = recvfrom(cC, buf, sizeof(buf) - 1, NULL, from, flen);
            if (length == SOCKET_ERROR)
            {
                if (WSAGetLastError() == WSAETIMEDOUT)
                    break;
                else
                    throw SetErrorMsgText("recvfrom", WSAGetLastError());
            }

            buf[length] = '\0';
            if (strcmp(call, buf) == 0)
            {
                countServers++;
                std::cout << "There's a server with the same callsign!" << std::endl;
                std::cout << "Count: " << countServers << std::endl;
                std::cout << "IP: " << inet_ntoa(((SOCKADDR_IN*)from)->sin_addr) << std::endl;
                std::cout << "Port: " << ntohs(((struct sockaddr_in*)from)->sin_port) << std::endl;
            }
        }

        std::cout << "Number of servers with the same callsign: " << countServers << std::endl;

        if (closesocket(cC) == SOCKET_ERROR)
            throw SetErrorMsgText("closesocket", WSAGetLastError());
    }
    catch (const std::string& errorMsgText)
    {
        if (cC != INVALID_SOCKET)
            closesocket(cC);
        throw;
    }
}

int main()
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    SOCKET sS;
    WSADATA wsaData;

    SOCKADDR_IN client;
    int clientSize = sizeof(client);
    const char name[] = "Hello";
    char hostname[32];

    try
    {
        if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
            throw SetErrorMsgText("Startup", WSAGetLastError());

        std::cout << "Checking for other servers..." << std::endl;
        GetServer(name, 2000, (sockaddr*)&client, &clientSize);

        if ((sS = socket(AF_INET, SOCK_DGRAM, NULL)) == INVALID_SOCKET)
            throw SetErrorMsgText("socket", WSAGetLastError());

        SOCKADDR_IN serv;
        serv.sin_family = AF_INET;
        serv.sin_port = htons(2000);
        serv.sin_addr.s_addr = INADDR_ANY;

        if (bind(sS, (LPSOCKADDR)&serv, sizeof(serv)) == SOCKET_ERROR)
            throw SetErrorMsgText("bind", WSAGetLastError());

        if (gethostname(hostname, sizeof(hostname)) == SOCKET_ERROR)
            throw SetErrorMsgText("gethostname", WSAGetLastError());

        std::cout << "Server name: " << hostname << std::endl;
        std::cout << "Waiting for client connections on port 2000..." << std::endl;

        while (true)
        {
            SOCKADDR_IN clientB;
            int clientSize = sizeof(clientB);

            try
            {
                if (GetRequestFromClient(name, sS, (SOCKADDR*)&clientB, &clientSize))
                {
                    std::cout << std::endl;
                    std::cout << "Client socket:" << std::endl;
                    std::cout << "IP: " << inet_ntoa(clientB.sin_addr) << std::endl;
                    std::cout << "Port: " << ntohs(clientB.sin_port) << std::endl;
                    std::cout << std::endl;

                    if (PutAnswerToClient(name, sS, (SOCKADDR*)&clientB, &clientSize))
                    {
                        std::cout << "Success! Response sent to client." << std::endl;
                    }
                }
                else
                {
                    std::cout << "Wrong call name!" << std::endl;
                }
            }
            catch (const std::string& e)
            {
                std::cout << "Error handling client: " << e << std::endl;
            }
        }

        if (closesocket(sS) == SOCKET_ERROR)
            throw SetErrorMsgText("closesocket", WSAGetLastError());

        if (WSACleanup() == SOCKET_ERROR)
            throw SetErrorMsgText("Cleanup", WSAGetLastError());
    }
    catch (const std::string& errorMsgText)
    {
        std::cout << std::endl << "Error: " << errorMsgText << std::endl;
        system("pause");
    }

    return 0;
}