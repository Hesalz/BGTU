#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <string>
#include "ErrorHandler.h"
#pragma comment(lib, "WS2_32.lib")

using namespace std;

int main() {
    setlocale(0, "Russian");

    WSADATA wsaData;
    SOCKET sS, cS;
    int port = 2000;

    try {
        if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
            throw SetErrorMsgText("Startup:", WSAGetLastError());

        if ((sS = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
            throw SetErrorMsgText("socket:", WSAGetLastError());

        SOCKADDR_IN serv;
        serv.sin_family = AF_INET;
        serv.sin_port = htons(port);
        serv.sin_addr.s_addr = INADDR_ANY;

        if (bind(sS, (LPSOCKADDR)&serv, sizeof(serv)) == SOCKET_ERROR)
            throw SetErrorMsgText("bind:", WSAGetLastError());

        if (listen(sS, SOMAXCONN) == SOCKET_ERROR)
            throw SetErrorMsgText("listen:", WSAGetLastError());

        cout << "Сервер запущен и ожидает подключений на порту " << port << "..." << endl;

        while (true) {
            cout << "\nОжидание подключения клиента..." << endl;

            SOCKADDR_IN client;
            int lclient = sizeof(client);
            if ((cS = accept(sS, (sockaddr*)&client, &lclient)) == INVALID_SOCKET)
                throw SetErrorMsgText("accept:", WSAGetLastError());

            cout << "Клиент подключился: "
                << inet_ntoa(client.sin_addr) << ":" << ntohs(client.sin_port) << endl;

            char buffer[1024];
            int messageCount = 0;
            clock_t startTime = clock();

            while (true) {
                int bytesRecv = recv(cS, buffer, sizeof(buffer) - 1, NULL);
                if (bytesRecv == SOCKET_ERROR)
                    throw SetErrorMsgText("recv:", WSAGetLastError());

                if (bytesRecv == 0) {
                    cout << "Клиент отправил сообщение нулевой длины. Завершение сеанса.\n" << endl;
                    break;
                }

                buffer[bytesRecv] = '\0';
                messageCount++;

                cout << "Получено [" << messageCount << "]: " << buffer << endl;

                if (send(cS, buffer, bytesRecv, NULL) == SOCKET_ERROR)
                    throw SetErrorMsgText("send:", WSAGetLastError());

                cout << "Отправлено эхо: " << buffer << endl;
            }

            clock_t endTime = clock();
            double elapsedTime = (double)(endTime - startTime) / CLOCKS_PER_SEC;

            cout << "Сеанс завершен. Обработано сообщений: " << messageCount << endl;
            cout << "Время обработки: " << elapsedTime << " секунд" << endl;
            if (messageCount > 0) {
                cout << "Среднее время на сообщение: " << (elapsedTime * 1000 / messageCount) << " мс" << endl;
            }

            closesocket(cS);
            cout << "Клиент отключен. Ожидание нового подключения..." << endl;
        }

        closesocket(sS);

        if (WSACleanup() == SOCKET_ERROR)
            throw SetErrorMsgText("Cleanup:", WSAGetLastError());
    }
    catch (string errorMsgText) {
        cout << "Ошибка: " << errorMsgText << endl;

        if (cS != INVALID_SOCKET) closesocket(cS);
        if (sS != INVALID_SOCKET) closesocket(sS);
        WSACleanup();
    }

    return 0;
}