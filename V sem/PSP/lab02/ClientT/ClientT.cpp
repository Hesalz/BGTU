#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <iostream>
#include <winsock2.h>
#include <string>
#include <ctime>
#include "ErrorHandler.h"
#pragma comment(lib, "WS2_32.lib")

using namespace std;

int main() {
    setlocale(0, "Russian");

    WSADATA wsaData;
    SOCKET cC;

    int port = 2000;
    string ip = "127.0.0.1"; // 10.208.125.117

    try {
        if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0)
            throw SetErrorMsgText("Startup:", WSAGetLastError());

        if ((cC = socket(AF_INET, SOCK_STREAM, NULL)) == INVALID_SOCKET)
            throw SetErrorMsgText("socket:", WSAGetLastError());

        SOCKADDR_IN serv;
        serv.sin_family = AF_INET;
        serv.sin_port = htons(port);
        serv.sin_addr.s_addr = inet_addr(ip.c_str());

        if (connect(cC, (sockaddr*)&serv, sizeof(serv)) == SOCKET_ERROR)
            throw SetErrorMsgText("connect:", WSAGetLastError());

        cout << "Подключение к серверу установлено." << endl;

        int messageCount;
        cout << "Введите количество сообщений для отправки: ";
        cin >> messageCount;

        clock_t startTime = clock();

        string message = "Hello from Client 1";

        for (int i = 1; i <= messageCount; i++) {

            if (send(cC, message.c_str(), message.length(), NULL) == SOCKET_ERROR)
                throw SetErrorMsgText("send:", WSAGetLastError());

            cout << "Отправлено [" << i << "/" << messageCount << "]: " << message << endl;

            char buffer[1024];
            int bytesRecv = recv(cC, buffer, sizeof(buffer) - 1, NULL);
            if (bytesRecv == SOCKET_ERROR)
                throw SetErrorMsgText("recv:", WSAGetLastError());

            buffer[bytesRecv] = '\0';
            string receivedMsg(buffer);
            cout << "Получено эхо: " << receivedMsg << endl;

            if (i < messageCount) {
                size_t pos = receivedMsg.find_last_of(" ");
                if (pos != string::npos) {
                    int currentNumber = stoi(receivedMsg.substr(pos + 1));
                    message = "Hello from Client " + to_string(currentNumber + 1);
                }
            }

            if (i % 100 == 0) {
                cout << "Обработано " << i << " сообщений из " << messageCount << endl;
            }
        }

        if (send(cC, "", 0, NULL) == SOCKET_ERROR)
            throw SetErrorMsgText("send:", WSAGetLastError());

        clock_t endTime = clock();
        double elapsedTime = (double)(endTime - startTime) / CLOCKS_PER_SEC;

        cout << "\n=== РЕЗУЛЬТАТЫ ТЕСТИРОВАНИЯ ===" << endl;
        cout << "Общее количество сообщений: " << messageCount << endl;
        cout << "Общее время обмена: " << elapsedTime << " секунд" << endl;
        cout << "Среднее время на сообщение: " << (elapsedTime * 1000 / messageCount) << " мс" << endl;
        cout << "Сообщений в секунду: " << (messageCount / elapsedTime) << endl;

        closesocket(cC);

        if (WSACleanup() == SOCKET_ERROR)
            throw SetErrorMsgText("Cleanup:", WSAGetLastError());
    }
    catch (string errorMsgText) {
        cout << "Ошибка: " << errorMsgText << endl;
        if (cC != INVALID_SOCKET) closesocket(cC);
        WSACleanup();
    }

    return 0;
}