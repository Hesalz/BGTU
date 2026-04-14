#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <iostream>
#include <string>
#include <winsock2.h>
#include <windows.h>
#include <ctime>
#include <cstring>

#pragma comment(lib, "ws2_32.lib")

using namespace std;

struct CA {
    char ipaddr[15];        // IP-адрес координатора
    char resource[20];      // имя ресурса (например, путь к файлу)
    enum STATUS {
        NOINIT,    // начальное состояние
        INIT,      // выполнена инициализация
        ENTER,     // выполнен вход в секцию
        LEAVE,     // выполнен выход из секции
        WAIT       // ожидание входа
    } status;
};

SOCKET clientSocket;
SOCKADDR_IN coordinatorAddr;
bool isInitialized = false;

// полная дата/время для записи в файл
string GetCurrentDateTime() {
    time_t now = time(NULL);                                             // время в секундах
    struct tm* timeinfo = localtime(&now);                               // локальное время
    char buffer[80];
    strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", timeinfo);
    return string(buffer);
}
// только время для консоли
string GetCurrentTimeShort() {
    time_t now = time(NULL);
    struct tm* timeinfo = localtime(&now);
    char buffer[80];
    strftime(buffer, sizeof(buffer), "%H:%M:%S", timeinfo);
    return string(buffer);
}

// отправка сообщения и получение ответа
bool SendAndReceive(CA& ca, CA::STATUS expectedResponse) {
    // отправляем структуру координатору
    int sent = sendto(clientSocket, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&coordinatorAddr, sizeof(coordinatorAddr));

    if (sent == SOCKET_ERROR) {
        cout << "[ERROR] Send failed: " << WSAGetLastError() << endl;
        return false;
    }

    // получаем ответ
    CA response;
    memset(&response, 0, sizeof(response));                  // обнуляем структуру
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;                                    // 30 секунд таймаут
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(clientSocket, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        int error = WSAGetLastError();
        if (error != WSAETIMEDOUT) {
            cout << "[ERROR] Receive failed: " << error << endl;
        }
        return false;
    }
    // проверка, что статус именно тот, который ожидали
    if (response.status == expectedResponse) {
        ca.status = response.status;
        return true;
    }

    return false;
}

// инициализация критической секции
CA InitCA(char ipaddr[15], char resource[20]) {
    CA ca;
    strncpy_s(ca.ipaddr, 15, ipaddr, _TRUNCATE);                // копируем IP
    strncpy_s(ca.resource, 20, resource, _TRUNCATE);            // копируем имя ресурса
    ca.status = CA::NOINIT;

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cout << "[InitCA] WSAStartup failed" << endl;
        return ca;
    }

    clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        cout << "[InitCA] Socket creation failed: " << WSAGetLastError() << endl;
        WSACleanup();
        return ca;
    }

    coordinatorAddr.sin_family = AF_INET;
    coordinatorAddr.sin_port = htons(2000);
    coordinatorAddr.sin_addr.s_addr = inet_addr(ipaddr);

    // запрос на координацию
    ca.status = CA::INIT;
    if (SendAndReceive(ca, CA::INIT)) {
        cout << "[InitCA] Critical section initialized for resource '" << resource << "'" << endl;
        isInitialized = true;
    }
    else {
        cout << "[InitCA] Initialization failed" << endl;
        ca.status = CA::NOINIT;
        closesocket(clientSocket);
        WSACleanup();
    }

    return ca;
}

// войти в критическую секцию (блокирующая операция)
bool EnterCA(CA& ca) {
    if (!isInitialized) {
        cout << "[EnterCA] Error: critical section is not initialized" << endl;
        return false;
    }

    cout << "[EnterCA] Requesting entry to the critical section..." << endl;

    // запрос на вход
    ca.status = CA::ENTER;

    int sent = sendto(clientSocket, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&coordinatorAddr, sizeof(coordinatorAddr));

    if (sent == SOCKET_ERROR) {
        cout << "[EnterCA] Send failed: " << WSAGetLastError() << endl;
        return false;
    }

    // ответ
    CA response;
    memset(&response, 0, sizeof(response));
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(clientSocket, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        cout << "[EnterCA] Receive failed: " << WSAGetLastError() << endl;
        return false;
    }

    // обработка ответа
    if (response.status == CA::ENTER) {
        ca.status = CA::ENTER;
        cout << "[EnterCA] Entry to the critical section GRANTED!" << endl;
        return true;
    }
    else if (response.status == CA::WAIT) {
        cout << "[EnterCA] Section is busy, waiting for release..." << endl;

        // ждём разрешения от координатора
        while (true) {
            CA grantMsg;
            memset(&grantMsg, 0, sizeof(grantMsg));

            received = recvfrom(clientSocket, (char*)&grantMsg, sizeof(grantMsg), 0,
                (SOCKADDR*)&fromAddr, &fromLen);

            if (received == SOCKET_ERROR) {
                cout << "[EnterCA] Wait receive failed: " << WSAGetLastError() << endl;
                return false;
            }

            if (grantMsg.status == CA::ENTER) {
                ca.status = CA::ENTER;
                cout << "[EnterCA] Entry to the critical section GRANTED!" << endl;
                return true;
            }
        }
    }

    return false;
}

// покинуть критическую секцию
bool LeaveCA(CA& ca) {
    if (!isInitialized) {
        cout << "[LeaveCA] Error: critical section is not initialized" << endl;
        return false;
    }

    cout << "[LeaveCA] Leaving the critical section..." << endl;

    ca.status = CA::LEAVE;

    if (SendAndReceive(ca, CA::LEAVE)) {
        cout << "[LeaveCA] Critical section released" << endl;
        return true;
    }

    cout << "[LeaveCA] Leave failed" << endl;
    return false;
}

// закрыть критическую секцию
bool CloseCA(CA& ca) {
    if (!isInitialized) {
        cout << "[CloseCA] Error: critical section is not initialized" << endl;
        return false;
    }

    cout << "[CloseCA] Closing critical section..." << endl;

    ca.status = CA::NOINIT;

    if (SendAndReceive(ca, CA::NOINIT)) {
        cout << "[CloseCA] Critical section closed" << endl;
        closesocket(clientSocket);
        WSACleanup();
        isInitialized = false;
        return true;
    }

    cout << "[CloseCA] Close failed" << endl;
    closesocket(clientSocket);
    WSACleanup();
    isInitialized = false;
    return false;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "C");

    // задержка старта (мс), задаётся 1-м аргументом командной строки
    int delayBeforeStart = 0;

    if (argc > 1) {
        delayBeforeStart = atoi(argv[1]);
    }

    cout << "CLIENT STARTED" << endl;

    if (delayBeforeStart > 0) {
        cout << "[SYSTEM] Delay " << delayBeforeStart << " ms..." << endl;
        Sleep(delayBeforeStart);
    }

    char ip[] = "172.20.10.3";
    char resource[] = "Z:\\Lab_2.txt";

    // инициализация
    CA ca = InitCA(ip, resource);

    if (ca.status == CA::INIT) {
        cout << endl << "   TRYING TO ENTER CRITICAL SECTION   " << endl;

        // вход в секцию
        if (EnterCA(ca)) {
            cout << endl << "   START WRITING TO FILE   " << endl;

            FILE* file = fopen(resource, "a"); 

            if (file != nullptr) {
                for (int i = 0; i < 5; i++) {
                    string currentTime = GetCurrentDateTime();
                    fprintf(file, "[Client] %s - record %d/5\n",
                        currentTime.c_str(), i + 1);
                    fflush(file);

                    cout << "[" << GetCurrentTimeShort() << "] Wrote line "
                        << (i + 1) << "/5" << endl;

                    if (i < 4) Sleep(5000);
                }
                fclose(file);
                cout << "    WRITE COMPLETED SUCCESSFULLY    " << endl;
            }
            else {
                cout << "[ERROR] Failed to open file: " << resource << endl;
                cout << "Error code: " << errno << endl;
            }

            LeaveCA(ca);
        }
        else {
            cout << "[ERROR] Failed to enter critical section" << endl;
        }

        CloseCA(ca);
    }
    else {
        cout << "[ERROR] Failed to initialize critical section" << endl;
        cout << "Make sure the coordinator is running on port 2000" << endl;
    }

    cout << endl << " CLIENT FINISHED " << endl;
    system("pause");
    return 0;
}