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
    SOCKET clientSocket;
    sockaddr_in serverAddr;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        handleError("Ошибка инициализации Winsock");
    }

    clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        handleError("Ошибка создания сокета");
    }

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(2000);
    inet_pton(AF_INET, "127.0.0.1", &serverAddr.sin_addr);

    int timeout = 2000;
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    cout << "UDP Клиент запущен..." << endl;
    cout << "Подключение к серверу.." << endl;
    cout << "Таймаут ожидания ответа: " << timeout << " секунды" << endl;

    int messageCount;
    cout << "\nВведите количество сообщений: ";
    cin >> messageCount;

    cout << "\nОтправка " << messageCount << " сообщений..." << endl;
    cout << "========================================" << endl;

    char buffer[1024];
    int serverAddrSize = sizeof(serverAddr);
    int lostCount = 0;
    int successCount = 0;

    for (int i = 0; i < messageCount; i++) {
        string message = "Message " + to_string(i + 1);

        int sendResult = sendto(clientSocket, message.c_str(), (int)message.length() + 1, 0,
            (sockaddr*)&serverAddr, serverAddrSize);
        if (sendResult == SOCKET_ERROR) {
            cout << "Ошибка отправки сообщения " << (i + 1) << endl;
            continue;
        }

        cout << "[" << (i + 1) << "] Отправлено серверу: " << message << endl;

        int bytesReceived = recvfrom(clientSocket, buffer, sizeof(buffer) - 1, 0,
            (sockaddr*)&serverAddr, &serverAddrSize);

        if (bytesReceived == SOCKET_ERROR) {
            int error = WSAGetLastError();
            if (error == WSAETIMEDOUT) {
                cout << "[" << (i + 1) << "] *** ПАКЕТ ПОТЕРЯН - Таймаут ожидания ответа ***" << endl;
                lostCount++;
            }
            else {
                cout << "[" << (i + 1) << "] Ошибка приема ответа: " << error << endl;
            }
        }
        else {
            buffer[bytesReceived] = '\0';
            cout << "[" << (i + 1) << "] Получен ответ от сервера: " << buffer << endl;
            successCount++;
        }

        cout << "----------------------------------------" << endl;
        Sleep(100);
    }

    cout << "\n=== СТАТИСТИКА ===" << endl;
    cout << "Всего отправлено: " << messageCount << endl;
    cout << "Успешно получено: " << successCount << endl;
    cout << "Потеряно: " << lostCount << endl;
    cout << "Успешность: " << (successCount * 100.0 / messageCount) << "%" << endl;

    closesocket(clientSocket);
    WSACleanup();
    return 0;
}