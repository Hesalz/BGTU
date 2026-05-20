#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <WinSock2.h>
#include <Windows.h>
#include <iostream>
#include <fstream>
#include <string>

#pragma comment(lib, "WS2_32.lib")

#ifndef SIO_UDP_CONNRESET
#define SIO_UDP_CONNRESET _WSAIOW(IOC_VENDOR, 12)
#endif

const int PORT = 5555;
const char* CONFIG_FILE = "config.txt";

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

// Удаление пробелов и переносов строк
std::string trim(const std::string& value)
{
    size_t first = value.find_first_not_of(" \t\r\n");
    if (first == std::string::npos)
        return "";

    size_t last = value.find_last_not_of(" \t\r\n");
    return value.substr(first, last - first + 1);
}

// Чтение IP координатора из config.txt
std::string getCoordinatorIp()
{
    std::ifstream file(CONFIG_FILE);
    std::string line;

    if (file.is_open() && std::getline(file, line))
        return trim(line);

    return "";
}

// Отправка запроса координатору и получение ответа
bool askCoordinator(SOCKET socketHandle, const std::string& coordinatorIp, std::string& answer)
{
    sockaddr_in coordinatorAddress{};
    coordinatorAddress.sin_family = AF_INET;
    coordinatorAddress.sin_port = htons(PORT);
    coordinatorAddress.sin_addr.s_addr = inet_addr(coordinatorIp.c_str());

    if (coordinatorAddress.sin_addr.s_addr == INADDR_NONE)
    {
        std::cout << "[Ошибка] Некорректный IP координатора в config.txt: "
            << coordinatorIp
            << std::endl;

        return false;
    }

    const char* request = "GET_TIME";

    int sendResult = sendto(
        socketHandle,
        request,
        (int)strlen(request),
        0,
        (sockaddr*)&coordinatorAddress,
        sizeof(coordinatorAddress)
    );

    if (sendResult == SOCKET_ERROR)
    {
        showError("Ошибка отправки запроса координатору");
        return false;
    }

    char buffer[256];

    sockaddr_in fromServer{};
    int fromServerSize = sizeof(fromServer);

    int recvResult = recvfrom(
        socketHandle,
        buffer,
        sizeof(buffer) - 1,
        0,
        (sockaddr*)&fromServer,
        &fromServerSize
    );

    if (recvResult == SOCKET_ERROR)
    {
        int error = WSAGetLastError();

        if (error == WSAETIMEDOUT)
        {
            std::cout << "[Ошибка] Координатор "
                << coordinatorIp
                << " не ответил на запрос"
                << std::endl;
        }
        else
        {
            showError("Ошибка получения ответа от координатора");
        }

        return false;
    }

    buffer[recvResult] = '\0';
    answer = buffer;

    return true;
}

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Russian");

    if (argc < 2)
    {
        std::cout << "Запуск: ServerU_Agent.exe <IP_ПОСРЕДНИКА>" << std::endl;
        return -1;
    }

    std::string agentIp = argv[1];

    WSAData data;

    if (WSAStartup(MAKEWORD(2, 2), &data) != 0)
    {
        showError("Ошибка WSAStartup");
        return -1;
    }

    SOCKET agentSocket = socket(AF_INET, SOCK_DGRAM, 0);

    if (agentSocket == INVALID_SOCKET)
    {
        showError("Ошибка создания сокета");
        WSACleanup();
        return -1;
    }

    disableUdpReset(agentSocket);

    DWORD timeout = 2500;

    setsockopt(
        agentSocket,
        SOL_SOCKET,
        SO_RCVTIMEO,
        (const char*)&timeout,
        sizeof(timeout)
    );

    sockaddr_in agentAddress{};
    agentAddress.sin_family = AF_INET;
    agentAddress.sin_port = htons(PORT);
    agentAddress.sin_addr.s_addr = inet_addr(agentIp.c_str());

    if (bind(agentSocket, (sockaddr*)&agentAddress, sizeof(agentAddress)) == SOCKET_ERROR)
    {
        std::cout << "[Ошибка] Не удалось запустить посредника на IP "
            << agentIp
            << " и порту "
            << PORT
            << std::endl;

        closesocket(agentSocket);
        WSACleanup();
        return -1;
    }

    std::cout << "[Посредник] Запущен на "
        << agentIp
        << ":"
        << PORT
        << std::endl;

    char clientBuffer[256];

    while (true)
    {
        sockaddr_in clientAddress{};
        int clientSize = sizeof(clientAddress);

        int recvResult = recvfrom(
            agentSocket,
            clientBuffer,
            sizeof(clientBuffer) - 1,
            0,
            (sockaddr*)&clientAddress,
            &clientSize
        );

        if (recvResult == SOCKET_ERROR)
        {
            int error = WSAGetLastError();

            if (error == WSAETIMEDOUT)
                continue;

            showError("Ошибка получения запроса клиента");
            continue;
        }

        clientBuffer[recvResult] = '\0';

        std::string clientMessage = clientBuffer;
        std::string clientIp = inet_ntoa(clientAddress.sin_addr);

        if (clientMessage != "GET_TIME")
            continue;

        std::string coordinatorIp = getCoordinatorIp();

        if (coordinatorIp.empty())
        {
            std::cout << "[Ошибка] В config.txt не указан координатор" << std::endl;
            continue;
        }

        std::string timeAnswer;

        bool ok = askCoordinator(
            agentSocket,
            coordinatorIp,
            timeAnswer
        );

        if (!ok)
        {
            std::cout << "[Журнал] Клиент "
                << clientIp
                << " обратился к посреднику, но координатор "
                << coordinatorIp
                << " недоступен"
                << std::endl;

            continue;
        }

        sendto(
            agentSocket,
            timeAnswer.c_str(),
            (int)timeAnswer.length(),
            0,
            (sockaddr*)&clientAddress,
            clientSize
        );

        std::cout << "[Журнал] Клиент: "
            << clientIp
            << " | координатор: "
            << coordinatorIp
            << " | ответ: "
            << timeAnswer
            << std::endl;
    }

    closesocket(agentSocket);
    WSACleanup();

    return 0;
}