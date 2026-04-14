#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <vector>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

void handleError(const string& message) {
    cerr << message << ": " << WSAGetLastError() << endl;
    exit(1);
}

bool GetRequestFromClient(SOCKET s, char* name, sockaddr_in* from, int* flen) {
    char buffer[1024];
    int fromSize = sizeof(sockaddr_in);

    int timeout = 10000;
    setsockopt(s, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    while (true) {
        int bytesReceived = recvfrom(s, buffer, sizeof(buffer) - 1, 0,
            (sockaddr*)from, &fromSize);

        if (bytesReceived == SOCKET_ERROR) {
            int error = WSAGetLastError();
            if (error == WSAETIMEDOUT) {
                return false; 
            }
            throw runtime_error("Ошибка при приёме данных от клиента");
        }

        buffer[bytesReceived] = '\0';

        char ip[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &from->sin_addr, ip, sizeof(ip));
        int port = ntohs(from->sin_port);

        cout << "[Получено] От " << ip << ":" << port << " → " << buffer << endl;

        if (strcmp(buffer, name) == 0) {
            *flen = fromSize;
            return true;
        }
        else {
            cout << "   Неверный позывной. Ожидалось: " << name << endl;
        }
    }
}

bool PutAnswerToClient(SOCKET s, char* name, sockaddr_in* to, int* tlen) {
    if (sendto(s, name, strlen(name), 0, (sockaddr*)to, *tlen) == SOCKET_ERROR) {
        throw runtime_error("Ошибка отправки ответа клиенту");
    }

    char ip[INET_ADDRSTRLEN];
    inet_ntop(AF_INET, &to->sin_addr, ip, sizeof(ip));
    int port = ntohs(to->sin_port);

    cout << "Ответ отправлен → " << ip << ":" << port << endl;
    return true;
}

void CheckExistingServers(char* call, short port) {
    SOCKET checkSocket;
    sockaddr_in broadcastAddr;
    vector<string> servers;

    checkSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (checkSocket == INVALID_SOCKET) {
        cerr << "Ошибка создания сокета проверки" << endl;
        return;
    }

    int broadcast = 1;
    setsockopt(checkSocket, SOL_SOCKET, SO_BROADCAST, (char*)&broadcast, sizeof(broadcast));

    int timeout = 2000;
    setsockopt(checkSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    broadcastAddr.sin_family = AF_INET;
    broadcastAddr.sin_port = htons(port);
    broadcastAddr.sin_addr.s_addr = INADDR_BROADCAST; 

    sendto(checkSocket, call, strlen(call), 0, (sockaddr*)&broadcastAddr, sizeof(broadcastAddr));

    char buffer[1024];
    sockaddr_in from;
    int fromSize = sizeof(from);

    while (true) {
        int bytesReceived = recvfrom(checkSocket, buffer, sizeof(buffer) - 1, 0,
            (sockaddr*)&from, &fromSize);

        if (bytesReceived == SOCKET_ERROR) break;

        buffer[bytesReceived] = '\0';

        if (strcmp(buffer, call) == 0) {
            char ip[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &from.sin_addr, ip, sizeof(ip));
            servers.push_back(ip);
        }
    }

    closesocket(checkSocket);

    if (!servers.empty()) {
        cout << "\nВНИМАНИЕ: Найдено " << servers.size()
            << " сервер(ов) с таким же позывным в сети:\n";
        for (const auto& ip : servers)
            cout << "   - " << ip << endl;
        cout << endl;
    }
    else {
        cout << "Другие серверы с таким позывным не обнаружены\n\n";
    }
}

int main() {
    setlocale(LC_ALL, "ru");

    WSADATA wsaData;
    SOCKET serverSocket;
    sockaddr_in serverAddr;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
        handleError("Ошибка инициализации Winsock");

    char call[] = "Hello";
    short port = 2000;

    cout << "Проверка наличия других серверов в сети..." << endl;
    CheckExistingServers(call, port);

    serverSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (serverSocket == INVALID_SOCKET)
        handleError("Ошибка создания серверного сокета");

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(port);

    if (bind(serverSocket, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
        handleError("Ошибка привязки сокета");

    cout << "Сервер запущен на порту " << port << endl;
    cout << "Позывной: " << call << endl;
    cout << "Ожидание запросов от клиентов...\n\n";

    while (true) {
        sockaddr_in clientAddr;
        int clientAddrSize = sizeof(clientAddr);

        if (GetRequestFromClient(serverSocket, call, &clientAddr, &clientAddrSize)) {
            char ip[INET_ADDRSTRLEN];
            inet_ntop(AF_INET, &clientAddr.sin_addr, ip, sizeof(ip));
            int clientPort = ntohs(clientAddr.sin_port);

            cout << "Получен правильный позывной от " << ip << ":" << clientPort << endl;

            PutAnswerToClient(serverSocket, call, &clientAddr, &clientAddrSize);
        }
        else {
            cout << "Таймаут ожидания запроса клиента" << endl;
        }
    }

    closesocket(serverSocket);
    WSACleanup();
    return 0;
}
