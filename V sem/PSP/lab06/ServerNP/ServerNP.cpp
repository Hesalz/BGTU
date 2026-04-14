#include <iostream>
#include "Winsock2.h"
#pragma comment(lib, "WS2_32.lib")
#pragma warning(disable : 4996)
using namespace std;

#define INADDR_ANY        (u_long)0x00000000 //любой адрес       +++ 
#define INADDR_LOOPBACK    0x7f000001        // внутренняя петля +++
#define INADDR_BROADCAST  (u_long)0xffffffff // широковещание    +++  
#define INADDR_NONE        0xffffffff        // нет адреса  
#define ADDR_ANY           INADDR_ANY        // любой адрес       

typedef struct sockaddr_in SOCKADDR_IN;    //                     +++
typedef struct sockaddr_in* PSOCKADDR_IN;
typedef struct sockaddr_in FAR* LPSOCKADDR_IN;

string GetErrorMsgText(int code)
{
    string msgText;
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
    case WSASYSCALLFAILURE: msgText = "Аварийное завершение системного вызова"; break;
    case 2: msgText = "Неудачное завершение"; break;
    case ERROR_INVALID_PARAMETER: msgText = "Значение параметра pimax превосходит PIPE_UNLIMITED_INSTANCES"; break;
    case ERROR_NO_DATA: msgText = "Канал закрывается"; break;
    case ERROR_PIPE_CONNECTED: msgText = "Процесс на другом конце канала уже подключён"; break;
    case ERROR_PIPE_LISTENING: msgText = "Ожидание подключения клиента"; break;
    case ERROR_CALL_NOT_IMPLEMENTED: msgText = "Функция не поддерживается системой"; break;
    default: msgText = "**ERROR**"; break;
    }
    return msgText;
};

string SetPipeError(string msgText, int code)
{
    return msgText + GetErrorMsgText(code);
}

int main()
{
    setlocale(LC_ALL, "Russian");
    HANDLE hPipe;

    try {

        SECURITY_DESCRIPTOR sd;
        InitializeSecurityDescriptor(&sd, SECURITY_DESCRIPTOR_REVISION);
        SetSecurityDescriptorDacl(&sd, TRUE, NULL, FALSE);

        SECURITY_ATTRIBUTES sa;
        sa.nLength = sizeof(sa);
        sa.lpSecurityDescriptor = &sd;
        sa.bInheritHandle = FALSE;

        if ((hPipe = CreateNamedPipe(L"\\\\.\\pipe\\Tube",
            PIPE_ACCESS_DUPLEX,
            PIPE_TYPE_MESSAGE | PIPE_WAIT,
            1, 512, 512,
            INFINITE, &sa)) == INVALID_HANDLE_VALUE)
            throw SetPipeError("create: ", GetLastError());


        while (true) {

            cout << "Waiting for client to connect..." << endl;

            if (!ConnectNamedPipe(hPipe, NULL)) {
                DWORD err = GetLastError();
                if (err != ERROR_PIPE_CONNECTED)
                    throw SetPipeError("connect: ", err);
            }

            cout << "Client connected" << endl;

            bool clientActive = true;

            while (clientActive) {

                char rbuf[50] = {};
                DWORD readBytes = 0;

                BOOL ok = ReadFile(hPipe, rbuf, sizeof(rbuf), &readBytes, NULL);

                if (!ok) {
                    DWORD err = GetLastError();

                    if (err == ERROR_BROKEN_PIPE || err == ERROR_NO_DATA) {
                        DisconnectNamedPipe(hPipe);
                        clientActive = false;
                        break;
                    }

                    throw SetPipeError("connect: ", err);
                }

                if (strcmp(rbuf, "\0") == 0) {
                    DisconnectNamedPipe(hPipe);
                    clientActive = false;
                    break;
                }

                cout << "Message: " << rbuf << endl;

                DWORD wbufl = 0;
                if (!WriteFile(hPipe, rbuf, sizeof(rbuf) - 1, &wbufl, NULL))
                    throw SetPipeError("write: ", GetLastError());

                cout << "message sent." << endl;
            }

            DisconnectNamedPipe(hPipe);
        }

        CloseHandle(hPipe);
    }
    catch (string errorMsgText) {
        cout << endl << "Error: " << errorMsgText;
        DisconnectNamedPipe(hPipe);
        CloseHandle(hPipe);
    }

    return 0;
}
