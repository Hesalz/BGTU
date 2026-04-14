#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

void handleError(const string& message) {
    cerr << message << ": " << WSAGetLastError() << endl;
    exit(1);
}

int main() {
    setlocale(LC_ALL, "Russian");

    WSADATA wsaData;
    SOCKET serverSocket;
    sockaddr_in serverAddr;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        handleError("Ошибка инициализации Winsock");
    }

    serverSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (serverSocket == INVALID_SOCKET) {
        handleError("Ошибка создания сокета");
    }

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(2000);

    if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
        handleError("Ошибка привязки сокета");
    }

    cout << "UDP Сервер запущен на порту 2000..." << endl;

    char buffer[1024];
    sockaddr_in clientAddr;
    int clientAddrSize = sizeof(clientAddr);

    while (true) {
        int bytesReceived = recvfrom(serverSocket, buffer, sizeof(buffer) - 1, 0,
            (sockaddr*)&clientAddr, &clientAddrSize);
        if (bytesReceived == SOCKET_ERROR) {
            int error = WSAGetLastError();
            if (error == WSAECONNRESET) {
                cout << "Клиент разорвал соединение (таймаут)" << endl;
                continue;
            }
            else {
                cout << "Ошибка приема данных: " << error << endl;
            }
            continue;
        }

        buffer[bytesReceived] = '\0';

        char clientIP[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &clientAddr.sin_addr, clientIP, INET_ADDRSTRLEN);
        cout << "Получено от " << clientIP << ":" << ntohs(clientAddr.sin_port) << " - " << buffer << endl;

        Sleep(5000);

        string response = "Ответ: " + string(buffer);
        int sendResult = sendto(serverSocket, response.c_str(), (int)response.length() + 1, 0,
            (sockaddr*)&clientAddr, clientAddrSize);

        if (sendResult == SOCKET_ERROR) {
            int error = WSAGetLastError();
            if (error == WSAECONNRESET) {
                cout << "Не удалось отправить ответ - клиент разорвал соединение" << endl;
            }
            else {
                cout << "Ошибка отправки ответа: " << error << endl;
            }
        }
        else {
            cout << "Отправлен ответ: " << response << endl;
        }

        cout << "----------------------------------------" << endl;
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}