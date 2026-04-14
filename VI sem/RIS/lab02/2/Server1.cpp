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
    setlocale(LC_ALL, "rus");
    cout << "КООРДИНАТОР (UDP-сервер) ЗАПУЩЕН" << endl;

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
            throw runtime_error("Ошибка WSAStartup");
        }

        serverSocket = socket(AF_INET, SOCK_DGRAM, 0);
        if (serverSocket == INVALID_SOCKET) {
            throw runtime_error("Ошибка создания сокета");
        }

        serverAddr.sin_family = AF_INET;
        serverAddr.sin_port = htons(2000);
        serverAddr.sin_addr.s_addr = INADDR_ANY;

        if (bind(serverSocket, (SOCKADDR*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR) {
            throw runtime_error("Ошибка привязки сокета");
        }

        cout << "[СИСТЕМА] Сервер слушает порт 2000" << endl << endl;

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
                cout << "[ИНИЦИАЛИЗАЦИЯ] Клиент " << client.ip << ":" << client.port
                    << " инициализирует критическую секцию для ресурса '" << resourceName << "'" << endl;

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
                cout << "[ЗАПРОС ВХОДА] Клиент " << client.ip << ":" << client.port
                    << " запрашивает вход в критическую секцию (ресурс: '" << resourceName << "')" << endl;

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

                    cout << "[РАЗРЕШЕНИЕ] Клиент " << client.ip << ":" << client.port
                        << " ВОШЕЛ в критическую секцию" << endl;
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

                    cout << "[ОЖИДАНИЕ] Клиент " << client.ip << ":" << client.port
                        << " поставлен в очередь (позиция: " << waitQueue.size() << ")" << endl;
                }
                break;
            }

            case CA::LEAVE: {
                cout << "[ВЫХОД] Клиент " << client.ip << ":" << client.port
                    << " покидает критическую секцию" << endl;

                //проверка что выходит именно владелец
                if (isBusy && currentOwner.ip == client.ip && currentOwner.port == client.port) {
                    isBusy = false;

                    CA response;
                    strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                    strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                    response.status = CA::LEAVE;

                    sendto(serverSocket, (char*)&response, sizeof(response), 0,
                        (SOCKADDR*)&clientAddr, clientAddrSize);

                    cout << "[ОСВОБОЖДЕНИЕ] Критическая секция освобождена" << endl;

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

                        cout << "[АКТИВАЦИЯ] Разрешение отправлено клиенту "
                            << nextClient.ip << ":" << nextClient.port << endl;
                        cout << "[ВХОД] Клиент " << nextClient.ip << ":" << nextClient.port
                            << " ВОШЕЛ в критическую секцию" << endl;
                    }
                }
                break;
            }

            case CA::NOINIT: {
                cout << "[ЗАКРЫТИЕ] Клиент " << client.ip << ":" << client.port
                    << " закрывает критическую секцию (ресурс: '" << resourceName << "')" << endl;

                CA response;
                strncpy_s(response.resource, 20, ca.resource, _TRUNCATE);
                strncpy_s(response.ipaddr, 15, ca.ipaddr, _TRUNCATE);
                response.status = CA::NOINIT;

                sendto(serverSocket, (char*)&response, sizeof(response), 0,
                    (SOCKADDR*)&clientAddr, clientAddrSize);
                break;
            }

            default:
                cout << "[НЕИЗВЕСТНО] Получен неизвестный статус: " << ca.status << endl;
                break;
            }

            cout << endl;
        }

        closesocket(serverSocket);
        WSACleanup();
    }
    catch (const exception& e) {
        cerr << "ОШИБКА: " << e.what() << endl;
        WSACleanup();
        return 1;
    }

    return 0;
}