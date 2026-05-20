#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <WinSock2.h>
#include <Windows.h>
#include <iostream>
#include <fstream>
#include <vector>
#include <string>
#include <thread>
#include <algorithm>
#include <ctime>

#pragma comment(lib, "WS2_32.lib")

#ifndef SIO_UDP_CONNRESET
#define SIO_UDP_CONNRESET _WSAIOW(IOC_VENDOR, 12)
#endif

const int PORT = 5555;
const char* NODES_FILE = "nodes.txt";
const char* CONFIG_FILE = "config.txt";

SOCKET serverSocket;
std::string myIp;
std::string coordinatorIp;
std::vector<std::string> nodes;

bool iAmCoordinator = false;
bool electionStarted = false;
bool receivedOk = false;
int lostAnswers = 0;

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

// Удаление лишних пробелов и переносов строк
std::string trim(const std::string& value)
{
    size_t first = value.find_first_not_of(" \t\r\n");
    if (first == std::string::npos)
        return "";

    size_t last = value.find_last_not_of(" \t\r\n");
    return value.substr(first, last - first + 1);
}

// Перевод IP в число, чтобы можно было сравнивать IP-адреса
unsigned long ipToNumber(const std::string& ip)
{
    return ntohl(inet_addr(ip.c_str()));
}

// Загрузка списка серверов из nodes.txt
void loadNodes()
{
    nodes.clear();

    std::ifstream file(NODES_FILE);
    std::string line;

    if (!file.is_open())
    {
        std::cout << "[Ошибка] Не удалось открыть файл nodes.txt" << std::endl;
        return;
    }

    while (std::getline(file, line))
    {
        line = trim(line);

        if (!line.empty())
            nodes.push_back(line);
    }
}

// Чтение текущего координатора из config.txt
std::string readCoordinator()
{
    std::ifstream file(CONFIG_FILE);
    std::string line;

    if (file.is_open() && std::getline(file, line))
        return trim(line);

    return "";
}

// Запись нового координатора в config.txt
void writeCoordinator(const std::string& ip)
{
    std::ofstream file(CONFIG_FILE, std::ios::trunc);

    if (file.is_open())
        file << ip;
}

// Отправка UDP-сообщения на указанный IP
void sendUdpMessage(const std::string& ip, const std::string& message)
{
    sockaddr_in address{};
    address.sin_family = AF_INET;
    address.sin_port = htons(PORT);
    address.sin_addr.s_addr = inet_addr(ip.c_str());

    sendto(
        serverSocket,
        message.c_str(),
        (int)message.length(),
        0,
        (sockaddr*)&address,
        sizeof(address)
    );
}

// Формирование текущего времени в нужном формате
std::string getCurrentTime()
{
    time_t now = time(NULL);
    tm* local = localtime(&now);

    char buffer[64];

    strftime(
        buffer,
        sizeof(buffer),
        "%d%m%Y:%H:%M:%S",
        local
    );

    return std::string(buffer);
}

// Определение начального координатора
void defineStartCoordinator()
{
    coordinatorIp = readCoordinator();

    if (coordinatorIp.empty())
    {
        auto maxNode = std::max_element(
            nodes.begin(),
            nodes.end(),
            [](const std::string& a, const std::string& b)
            {
                return ipToNumber(a) < ipToNumber(b);
            }
        );

        if (maxNode != nodes.end())
        {
            coordinatorIp = *maxNode;
            writeCoordinator(coordinatorIp);
        }
    }

    iAmCoordinator = (myIp == coordinatorIp);

    if (iAmCoordinator)
        std::cout << "[Старт] Этот сервер является координатором: " << myIp << std::endl;
    else
        std::cout << "[Старт] Текущий координатор: " << coordinatorIp << std::endl;
}

// Рассылка сообщения о новом координаторе
void announceCoordinator()
{
    coordinatorIp = myIp;
    iAmCoordinator = true;
    electionStarted = false;
    receivedOk = false;
    lostAnswers = 0;

    writeCoordinator(myIp);

    std::cout << "[Выборы] Сервер " << myIp << " стал новым координатором" << std::endl;

    for (const std::string& ip : nodes)
    {
        if (ip != myIp)
            sendUdpMessage(ip, "COORDINATOR");
    }
}

// Запуск выборов по алгоритму забияки
void startElection()
{
    electionStarted = true;
    receivedOk = false;

    std::cout << "[Выборы] Координатор не отвечает. Начинаем выборы" << std::endl;

    unsigned long myNumber = ipToNumber(myIp);

    for (const std::string& ip : nodes)
    {
        if (ipToNumber(ip) > myNumber)
        {
            std::cout << "[Выборы] Отправлен запрос старшему серверу: " << ip << std::endl;
            sendUdpMessage(ip, "ELECTION");
        }
    }

    Sleep(2000);

    if (!receivedOk)
    {
        announceCoordinator();
    }
    else
    {
        std::cout << "[Выборы] Есть сервер старше. Ждём нового координатора" << std::endl;
        electionStarted = false;
        lostAnswers = 0;
    }
}

// Поток проверки координатора
void coordinatorChecker()
{
    while (true)
    {
        Sleep(5000);

        if (iAmCoordinator)
            continue;

        if (coordinatorIp.empty())
        {
            lostAnswers = 3;
        }
        else
        {
            sendUdpMessage(coordinatorIp, "PING");
            lostAnswers++;

            std::cout << "[Проверка] Проверяем координатора "
                << coordinatorIp
                << ". Неудачных проверок: "
                << lostAnswers
                << std::endl;
        }

        if (lostAnswers >= 3 && !electionStarted)
        {
            startElection();
        }
    }
}

// Поток приёма сообщений
void messageListener()
{
    char buffer[256];

    while (true)
    {
        sockaddr_in sender{};
        int senderSize = sizeof(sender);

        int result = recvfrom(
            serverSocket,
            buffer,
            sizeof(buffer) - 1,
            0,
            (sockaddr*)&sender,
            &senderSize
        );

        if (result <= 0)
            continue;

        buffer[result] = '\0';

        std::string message = buffer;
        std::string senderIp = inet_ntoa(sender.sin_addr);

        // Запрос времени от посредника
        if (message == "GET_TIME")
        {
            if (iAmCoordinator)
            {
                std::string timeText = getCurrentTime();

                sendto(
                    serverSocket,
                    timeText.c_str(),
                    (int)timeText.length(),
                    0,
                    (sockaddr*)&sender,
                    senderSize
                );

                std::cout << "[Время] Отправлено время посреднику: "
                    << senderIp
                    << std::endl;
            }
        }

        // Проверка жив ли сервер
        else if (message == "PING")
        {
            sendUdpMessage(senderIp, "PONG");
        }

        // Ответ координатора на проверку
        else if (message == "PONG")
        {
            lostAnswers = 0;
        }

        // Сообщение о начале выборов
        else if (message == "ELECTION")
        {
            std::cout << "[Выборы] Получен запрос выборов от "
                << senderIp
                << std::endl;

            sendUdpMessage(senderIp, "OK");

            coordinatorIp = "";
            lostAnswers = 3;
        }

        // Ответ от сервера с большим IP
        else if (message == "OK")
        {
            receivedOk = true;
        }

        // Уведомление о новом координаторе
        else if (message == "COORDINATOR")
        {
            coordinatorIp = senderIp;
            iAmCoordinator = false;
            electionStarted = false;
            lostAnswers = 0;

            writeCoordinator(coordinatorIp);

            std::cout << "[Координатор] Новый координатор: "
                << coordinatorIp
                << std::endl;
        }
    }
}

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Russian");

    if (argc < 2)
    {
        std::cout << "Запуск: ServerU.exe <IP_СЕРВЕРА>" << std::endl;
        return -1;
    }

    myIp = argv[1];

    loadNodes();

    if (std::find(nodes.begin(), nodes.end(), myIp) == nodes.end())
    {
        std::cout << "[Ошибка] IP " << myIp << " отсутствует в nodes.txt" << std::endl;
        return -1;
    }

    WSAData data;

    if (WSAStartup(MAKEWORD(2, 2), &data) != 0)
    {
        showError("Ошибка WSAStartup");
        return -1;
    }

    serverSocket = socket(AF_INET, SOCK_DGRAM, 0);

    if (serverSocket == INVALID_SOCKET)
    {
        showError("Ошибка создания сокета");
        WSACleanup();
        return -1;
    }

    disableUdpReset(serverSocket);

    sockaddr_in serverAddress{};
    serverAddress.sin_family = AF_INET;
    serverAddress.sin_port = htons(PORT);
    serverAddress.sin_addr.s_addr = inet_addr(myIp.c_str());

    if (bind(serverSocket, (sockaddr*)&serverAddress, sizeof(serverAddress)) == SOCKET_ERROR)
    {
        std::cout << "[Ошибка] Не удалось занять IP "
            << myIp
            << " и порт "
            << PORT
            << std::endl;

        closesocket(serverSocket);
        WSACleanup();
        return -1;
    }

    defineStartCoordinator();

    std::cout << "[Сервер] UDP-сервер времени запущен: "
        << myIp
        << ":"
        << PORT
        << std::endl;

    std::thread listener(messageListener);
    std::thread checker(coordinatorChecker);

    listener.join();
    checker.join();

    closesocket(serverSocket);
    WSACleanup();

    return 0;
}