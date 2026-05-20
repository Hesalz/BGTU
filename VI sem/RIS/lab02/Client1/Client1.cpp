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
    char ipaddr[15];        // ip-адрес координатора
    char resource[20];      // имя ресурса
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

//полная дата для записи в файл
string GetCurrentDateTime() {
    time_t now = time(NULL);                                             //время в сек
    struct tm* timeinfo = localtime(&now);                               //преобраз в лок
    char buffer[80];
    strftime(buffer, sizeof(buffer), "%Y-%m-%d %H:%M:%S", timeinfo);
    return string(buffer);
}
//только время для консоли
string GetCurrentTimeShort() {
    time_t now = time(NULL);
    struct tm* timeinfo = localtime(&now);
    char buffer[80];
    strftime(buffer, sizeof(buffer), "%H:%M:%S", timeinfo);
    return string(buffer);
}

//отправка сообщения и получение ответа
bool SendAndReceive(CA& ca, CA::STATUS expectedResponse) {
    //отправляем структуру серверу
    int sent = sendto(clientSocket, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&coordinatorAddr, sizeof(coordinatorAddr));

    if (sent == SOCKET_ERROR) {
        cout << "[ОШИБКА] Отправка сообщения: " << WSAGetLastError() << endl;
        return false;
    }

    //получаем ответ
    CA response;
    memset(&response, 0, sizeof(response));                  //обнуляем структуру
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;                                    //30 секунд таймаут
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(clientSocket, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        int error = WSAGetLastError();
        if (error != WSAETIMEDOUT) {
            cout << "[ОШИБКА] Получение ответа: " << error << endl;
        }
        return false;
    }
    //проверка что статус который и ожидался
    if (response.status == expectedResponse) {
        ca.status = response.status;
        return true;
    }

    return false;
}

//инициализация крит секции
CA InitCA(char ipaddr[15], char resource[20]) {
    CA ca;
    strncpy_s(ca.ipaddr, 15, ipaddr, _TRUNCATE);                //копируем айпи
    strncpy_s(ca.resource, 20, resource, _TRUNCATE);            //копир имя
    ca.status = CA::NOINIT;

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cout << "[InitCA] Ошибка WSAStartup" << endl;
        return ca;
    }

    clientSocket = socket(AF_INET, SOCK_DGRAM, 0);
    if (clientSocket == INVALID_SOCKET) {
        cout << "[InitCA] Ошибка создания сокета: " << WSAGetLastError() << endl;
        WSACleanup();
        return ca;
    }

    coordinatorAddr.sin_family = AF_INET;
    coordinatorAddr.sin_port = htons(2000);
    coordinatorAddr.sin_addr.s_addr = inet_addr(ipaddr);

    //запрос на координацию
    ca.status = CA::INIT;
    if (SendAndReceive(ca, CA::INIT)) {
        cout << "[InitCA] Критическая секция инициализирована для ресурса '" << resource << "'" << endl;
        isInitialized = true;
    }
    else {
        cout << "[InitCA] Ошибка инициализации" << endl;
        ca.status = CA::NOINIT;
        closesocket(clientSocket);
        WSACleanup();
    }

    return ca;
}

//войти в критическую секцию (блокирующая операция)
bool EnterCA(CA& ca) {
    if (!isInitialized) {
        cout << "[EnterCA] Ошибка: секция не инициализирована" << endl;
        return false;
    }

    cout << "[EnterCA] Запрос входа в критическую секцию..." << endl;

    //запрос на вход
    ca.status = CA::ENTER;

    int sent = sendto(clientSocket, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&coordinatorAddr, sizeof(coordinatorAddr));

    if (sent == SOCKET_ERROR) {
        cout << "[EnterCA] Ошибка отправки: " << WSAGetLastError() << endl;
        return false;
    }

    //ответ
    CA response;
    memset(&response, 0, sizeof(response));
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;
    setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(clientSocket, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        cout << "[EnterCA] Ошибка получения: " << WSAGetLastError() << endl;
        return false;
    }

    //обработка ответа
    if (response.status == CA::ENTER) {
        ca.status = CA::ENTER;
        cout << "[EnterCA] Вход в критическую секцию РАЗРЕШЕН!" << endl;
        return true;
    }
    else if (response.status == CA::WAIT) {
        cout << "[EnterCA] Секция занята, ожидание освобождения..." << endl;

        //ждем разрешения от координатора
        while (true) {
            CA grantMsg;
            memset(&grantMsg, 0, sizeof(grantMsg));

            received = recvfrom(clientSocket, (char*)&grantMsg, sizeof(grantMsg), 0,
                (SOCKADDR*)&fromAddr, &fromLen);

            if (received == SOCKET_ERROR) {
                cout << "[EnterCA] Ошибка ожидания: " << WSAGetLastError() << endl;
                return false;
            }

            if (grantMsg.status == CA::ENTER) {
                ca.status = CA::ENTER;
                cout << "[EnterCA] Вход в критическую секцию РАЗРЕШЕН!" << endl;
                return true;
            }
        }
    }

    return false;
}

//покинуть крит секцию
bool LeaveCA(CA& ca) {
    if (!isInitialized) {
        cout << "[LeaveCA] Ошибка: секция не инициализирована" << endl;
        return false;
    }

    cout << "[LeaveCA] Выход из критической секции..." << endl;

    ca.status = CA::LEAVE;

    if (SendAndReceive(ca, CA::LEAVE)) {
        cout << "[LeaveCA] Критическая секция освобождена" << endl;
        return true;
    }

    cout << "[LeaveCA] Ошибка при выходе" << endl;
    return false;
}

//закрыть крит секцию
bool CloseCA(CA& ca) {
    if (!isInitialized) {
        cout << "[CloseCA] Ошибка: секция не инициализирована" << endl;
        return false;
    }

    cout << "[CloseCA] Закрытие критической секции..." << endl;

    ca.status = CA::NOINIT;

    if (SendAndReceive(ca, CA::NOINIT)) {
        cout << "[CloseCA] Критическая секция закрыта" << endl;
        closesocket(clientSocket);
        WSACleanup();
        isInitialized = false;
        return true;
    }

    cout << "[CloseCA] Ошибка при закрытии" << endl;
    closesocket(clientSocket);
    WSACleanup();
    isInitialized = false;
    return false;
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "rus");

    //параметры командной
    string clientName = "B";
    int delayBeforeStart = 0;

    if (argc > 1) {
        clientName = argv[1];
        if (argc > 2) {
            delayBeforeStart = atoi(argv[2]);
        }
    }

    cout << "КЛИЕНТ " << clientName << " ЗАПУЩЕН" << endl;

    //задержка для клиента C
    if (clientName == "C" && delayBeforeStart == 0) {
        delayBeforeStart = 2000;
    }

    if (delayBeforeStart > 0) {
        cout << "[СИСТЕМА] Задержка " << delayBeforeStart << " мс..." << endl;
        Sleep(delayBeforeStart);
    }

    char ip[] = "26.180.211.124";
    char resource[] = "Z:\\Lab_2.txt";

    //инициализация
    CA ca = InitCA(ip, resource);

    if (ca.status == CA::INIT) {
        cout << endl << "   ПОПЫТКА ВХОДА В КРИТИЧЕСКУЮ СЕКЦИЮ   " << endl;

        //вход в секцию
        if (EnterCA(ca)) {
            cout << endl << "   НАЧАЛО ЗАПИСИ В ФАЙЛ   " << endl;

            FILE* file = fopen(resource, "a"); 

            if (file != nullptr) {
                for (int i = 0; i < 5; i++) {
                    string currentTime = GetCurrentDateTime();
                    fprintf(file, "[Client %s] %s - note %d/5\n",
                        clientName.c_str(), currentTime.c_str(), i + 1);
                    fflush(file);

                    cout << "[" << GetCurrentTimeShort() << "] Записана строка "
                        << (i + 1) << "/5" << endl;

                    if (i < 4) Sleep(5000);
                }
                fclose(file);
                cout << "    ЗАПИСЬ УСПЕШНО ЗАВЕРШЕНА    " << endl;
            }
            else {
                cout << "[ОШИБКА] Не удалось открыть файл: " << resource << endl;
                cout << "Код ошибки: " << errno << endl;
            }

            LeaveCA(ca);
        }
        else {
            cout << "[ОШИБКА] Не удалось войти в критическую секцию" << endl;
        }

        CloseCA(ca);
    }
    else {
        cout << "[ОШИБКА] Не удалось инициализировать критическую секцию" << endl;
        cout << "Убедитесь, что координатор запущен на порту 2000" << endl;
    }

    cout << endl << " КЛИЕНТ " << clientName << " ЗАВЕРШИЛ РАБОТУ " << endl;
    system("pause");
    return 0;
}