#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <iostream>
#include <string>
#include <queue>
#include <map>
#include <winsock2.h>
#include <windows.h>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

struct CA {
    char ipaddr[15];        // ip-адрес координатора
    char resource[20];      // имя ресурса
    enum STATUS {
        NOINIT = 0,         // начальное состояние
        INIT = 1,           // выполнена инициализация
        ENTER = 2,          // выполнен вход в секцию
        LEAVE = 3,          // выполнен выход из секции
        WAIT = 4            // ожидание входа
    } status;
};

struct ClientInfo {
    string ip;
    int port;
    SOCKADDR_IN addr;
};

int main() {
    setlocale(LC_ALL, "C");
    cout << "Server STARTED" << endl;

    WSADATA wsaData;
    SOCKET serverSocket;
    SOCKADDR_IN serverAddr, clientAddr;
    int clientAddrSize = sizeof(clientAddr);

    // Очередь ожидания
    queue<ClientInfo> waitQueue;                                  //очередь клиентов
    ClientInfo currentOwner;
    bool isBusy = false;

    try {
        if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
            throw runtime_error("WSAStartup failed");
        }

        serverSocket = socket(AF_INET, SOCK_DGRAM, 0);
        if (serverSocket == INVALID_SOCKET) {
            throw runtime_error("Socket creation failed");
        }

        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(2000);
        serverAddr.sin_addr.s_addr = INADDR_ANY;

        if (bind(serverSocket, (SOCKADDR*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            throw runtime_error("Socket bind failed");
        }

        cout << "[SYSTEM] Server is listening on port 2000" << endl << endl;

        while (true) {
            CA ca;
            memset(&ca, 0, sizeof(ca));                                             //обнуление структуры для приема

            int bytesReceived = recvfrom(serverSocket, (char*)&ca, sizeof(ca), 0,
                (SOCKADDR*)&clientAddr, &clientAddrSize);

            if (bytesReceived == SOCKET_ERROR) {
                continue;
            }

            ClientInfo client;
            client.ip = inet_ntoa(clientAddr.sin_addr);
            client.port = ntohs(clientAddr.sin_port);
            client.addr = clientAddr;

            string resourceName = string(ca.resource);                               //строка с изменением ресурса

            switch (ca.status) {
            case CA::INIT: {
                cout << "[INIT] Client " << client.ip << ":" << client.port
                    << " initializes critical section for resource '" << resourceName << "'" << endl;

                //ответное соо
                CA response;
                strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);    //имя ресурса
                strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);        //копир айпи
                response.status = CA::INIT;                                  //статус ответа

                sendto(serverSocket, (char*)&response, sizeof(response), 0,
                    (SOCKADDR*)&clientAddr, clientAddrSize);
                break;
            }

            case CA::ENTER: {                                                       //запрос на вход
                cout << "[ENTER REQUEST] Client " << client.ip << ":" << client.port
                    << " requests entry to critical section (resource: '" << resourceName << "')" << endl;

                //если секция свободна
                if (!isBusy) {
                    isBusy = true;
                    currentOwner = client;

                    CA response;
                    strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                    strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                    response.status = CA::ENTER;                                  //разрешаем вход

                    sendto(serverSocket, (char*)&response, sizeof(response), 0,
                        (SOCKADDR*)&clientAddr, clientAddrSize);

                    cout << "[GRANTED] Client " << client.ip << ":" << client.port
                        << " ENTERED the critical section" << endl;
                }
                //если занято
                else {
                    waitQueue.push(client);                                         //в очередь

                    CA response;
                    strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                    strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                    response.status = CA::WAIT;                                     //сообщаем об ожидании

                    sendto(serverSocket, (char*)&response, sizeof(response), 0,
                        (SOCKADDR*)&clientAddr, clientAddrSize);

                    cout << "[WAIT] Client " << client.ip << ":" << client.port
                        << " (position in queue: " << waitQueue.size() << ")" << endl;
                }
                break;
            }

            case CA::LEAVE: {
                cout << "[LEAVE] Client " << client.ip << ":" << client.port
                    << " leaves the critical section" << endl;

                //проверка что выходит именно владелец
                if (isBusy && currentOwner.ip == client.ip && currentOwner.port == client.port) {
                    isBusy = false;

                    CA response;
                    strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                    strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                    response.status = CA::LEAVE;

                    sendto(serverSocket, (char*)&response, sizeof(response), 0,
                        (SOCKADDR*)&clientAddr, clientAddrSize);

                    cout << "[RELEASED] Critical section released" << endl;

                    //если есть ожидающие клиенты
                    if (!waitQueue.empty()) {
                        ClientInfo nextClient = waitQueue.front();                   //берем первого
                        waitQueue.pop();                                             //удаляем из очереди

                        CA grantMsg;
                        strncpy_s(grantMsg.resource, 20, ca.resource, _TRUNCATE);
                        strncpy_s(grantMsg.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                        grantMsg.status = CA::ENTER;

                        sendto(serverSocket, (char*)&grantMsg, sizeof(grantMsg), 0,
                            (SOCKADDR*)&nextClient.addr, sizeof(nextClient.addr));

                        currentOwner = nextClient;
                        isBusy = true;

                        cout << "[ACTIVATE] Grant sent to client "
                            << nextClient.ip << ":" << nextClient.port << endl;
                        cout << "[ENTER] Client " << nextClient.ip << ":" << nextClient.port
                            << " ENTERED the critical section" << endl;
                    }
                }
                break;
            }

            case CA::NOINIT: {
                cout << "[CLOSE] Client " << client.ip << ":" << client.port
                    << " closes critical section (resource: '" << resourceName << "')" << endl;

                CA response;
                strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                response.status = CA::NOINIT;

                sendto(serverSocket, (char*)&response, sizeof(response), 0,
                    (SOCKADDR*)&clientAddr, clientAddrSize);
                break;
            }

            default:
                cout << "[UNKNOWN] Received unknown status: " << ca.status << endl;
                break;
            }

            cout << endl;
        }

        closesocket(serverSocket);
        WSACleanup();
    }
    catch (const exception& e) {
        cerr << "ERROR: " << e.what() << endl;
        WSACleanup();
        return 1;
    }

    return 0;
}