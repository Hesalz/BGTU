#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <WinSock2.h>
#include <Windows.h>
#include <iostream>
#include <string>

#pragma comment(lib, "WS2_32.lib")

#ifndef SIO_UDP_CONNRESET
#define SIO_UDP_CONNRESET _WSAIOW(IOC_VENDOR, 12)
#endif

const int PORT = 5555;

// Убираем ошибку UDP Connection Reset в Windows
void disableUdpReset(SOCKET s)
{
    BOOL flag = FALSE;
    DWORD returned = 0;

    WSAIoctl(
        s,
        SIO_UDP_CONNRESET,
        &flag,
        sizeof(flag),
        NULL,
        0,
        &returned,
        NULL,
        NULL
    );
}

// Вывод ошибки Winsock
void showError(const char* text)
{
    std::cout << text << ". Код ошибки: " << WSAGetLastError() << std::endl;
}

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Russian");

    if (argc < 2)
    {
        std::cout << "Запуск: ClientU.exe <IP_ПОСРЕДНИКА>" << std::endl;
        return -1;
    }

    std::string agentIp = argv[1];

    WSAData data;

    if (WSAStartup(MAKEWORD(2, 2), &data) != 0)
    {
        showError("Ошибка WSAStartup");
        return -1;
    }

    SOCKET clientSocket = socket(AF_INET, SOCK_DGRAM, 0);

    if (clientSocket == INVALID_SOCKET)
    {
        showError("Ошибка создания сокета");
        WSACleanup();
        return -1;
    }

    disableUdpReset(clientSocket);

    DWORD timeout = 3000;

    setsockopt(
        clientSocket,
        SOL_SOCKET,
        SO_RCVTIMEO,
        (const char*)&timeout,
        sizeof(timeout)
    );

    sockaddr_in agentAddress{};
    agentAddress.sin_family = AF_INET;
    agentAddress.sin_port = htons(PORT);
    agentAddress.sin_addr.s_addr = inet_addr(agentIp.c_str());

    int agentSize = sizeof(agentAddress);

    std::cout << "[Клиент] Запущен. Посредник: "
        << agentIp
        << ":"
        << PORT
        << std::endl;

    const char* request = "GET_TIME";
    char answer[256];

    while (true)
    {
        std::cout << "[Клиент] Отправляем запрос времени..." << std::endl;

        int sendResult = sendto(
            clientSocket,
            request,
            (int)strlen(request),
            0,
            (sockaddr*)&agentAddress,
            agentSize
        );

        if (sendResult == SOCKET_ERROR)
        {
            showError("Ошибка отправки запроса");
            Sleep(3000);
            continue;
        }

        int recvResult = recvfrom(
            clientSocket,
            answer,
            sizeof(answer) - 1,
            0,
            (sockaddr*)&agentAddress,
            &agentSize
        );

        if (recvResult == SOCKET_ERROR)
        {
            int error = WSAGetLastError();

            if (error == WSAETIMEDOUT)
            {
                std::cout << "[Клиент] Ответ не получен. Ожидание превышено" << std::endl;
            }
            else
            {
                showError("Ошибка получения ответа");
            }

            Sleep(3000);
            continue;
        }

        answer[recvResult] = '\0';

        std::cout << "[Клиент] Получено время: "
            << answer
            << std::endl;

        Sleep(5000);
    }

    closesocket(clientSocket);
    WSACleanup();

    return 0;
}