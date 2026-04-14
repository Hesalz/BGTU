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

bool GetServer(char* call, short port, sockaddr_in* from, int* flen) {
    SOCKET clientSocket;
    sockaddr_in broadcastAddr;
    char buffer[1024];

    clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        throw runtime_error("Ошибка создания сокета");
    }

    int broadcast = 1;
    if (setsockopt(clientSocket, SOL_SOCKET, SO_BROADCAST,
        (char*)&broadcast, sizeof(broadcast)) == SOCKET_ERROR) {
        closesocket(clientSocket);
        throw runtime_error("Ошибка установки режима широковещания");
    }

    int timeout = 5000;
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    broadcastAddr.sin_family = AF_INET;
    broadcastAddr.sin_addr.s_addr = INADDR_BROADCAST;
    broadcastAddr.sin_port = htons(port);

    if (sendto(clientSocket, call, strlen(call), 0,
        (sockaddr*)&broadcastAddr, sizeof(broadcastAddr)) == SOCKET_ERROR) {
        closesocket(clientSocket);
        throw runtime_error("Ошибка отправки широковещательного запроса");
    }

    cout << "Широковещательный запрос отправлен: " << call << endl;

    int fromSize = sizeof(sockaddr_in);
    int bytesReceived = recvfrom(clientSocket, buffer, sizeof(buffer) - 1, 0,
        (sockaddr*)from, &fromSize);

    if (bytesReceived == SOCKET_ERROR) {
        int error = WSAGetLastError();
        closesocket(clientSocket);
        if (error == WSAETIMEDOUT) {
            return false;
        }
        throw runtime_error("Ошибка приема данных");
    }

    buffer[bytesReceived] = '\0';

    closesocket(clientSocket);

    if (strcmp(buffer, call) == 0) {
        *flen = fromSize;
        return true;
    }

    return false;
}

int main() {
    setlocale(LC_ALL, "ru");
    WSADATA wsaData;

    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        handleError("Ошибка инициализации Winsock");
    }

    char call[] = "Hello";
    short port = 2000;
    sockaddr_in serverAddr;
    int serverAddrSize = sizeof(serverAddr);

    cout << "Поиск сервера в сети..." << endl;

    if (GetServer(call, port, &serverAddr, &serverAddrSize)) {
        char ip[INET_ADDRSTRLEN];
        inet_ntop(AF_INET, &serverAddr.sin_addr, ip, sizeof(ip));
        int port = ntohs(serverAddr.sin_port);

        cout << "Сервер найден!" << endl;
        cout << "IP: " << ip << ", Порт: " << port << endl;
    }
    else {
        cout << "Сервер не найден или откликнулся с неправильным позывным" << endl;
    }

    WSACleanup();
    return 0;
}