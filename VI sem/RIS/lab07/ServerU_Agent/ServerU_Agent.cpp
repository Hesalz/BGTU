#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <stdio.h>
#include <string>
#include <fstream>
#include <WinSock2.h>

#pragma comment(lib, "WS2_32.lib")

static std::string trim(const std::string& s)
{
    size_t start = s.find_first_not_of(" \t\r\n");

    if (start == std::string::npos)
        return "";

    size_t end = s.find_last_not_of(" \t\r\n");

    return s.substr(start, end - start + 1);
}

static std::string loadCoordinator()
{
    std::ifstream file("config.txt");

    std::string line;

    if (file.is_open() && std::getline(file, line))
        return trim(line);

    return "";
}

int main(int argc, char* argv[])
{
    if (argc < 2)
    {
        printf("Usage: %s <AGENT_IP>\n", argv[0]);
        return -1;
    }

    WSAData wsadata;

    WSAStartup(MAKEWORD(2, 2), &wsadata);

    SOCKET s = socket(AF_INET, SOCK_DGRAM, 0);

    DWORD timeout = 3000;

    setsockopt(
        s,
        SOL_SOCKET,
        SO_RCVTIMEO,
        (const char*)&timeout,
        sizeof(timeout)
    );

    sockaddr_in addr = { 0 };

    addr.sin_family = AF_INET;
    addr.sin_port = htons(5555);
    addr.sin_addr.s_addr = inet_addr(argv[1]);

    if (bind(s, (sockaddr*)&addr, sizeof(addr)) == SOCKET_ERROR)
    {
        printf("Bind error\n");
        return -1;
    }

    printf("[Agent] Started on %s:5555\n", argv[1]);

    char buffer[256];

    while (true)
    {
        sockaddr_in client = { 0 };

        int clientSize = sizeof(client);

        int r = recvfrom(
            s,
            buffer,
            sizeof(buffer) - 1,
            0,
            (sockaddr*)&client,
            &clientSize
        );

        if (r == SOCKET_ERROR)
            continue;

        buffer[r] = '\0';

        if (strcmp(buffer, "gettime") != 0)
            continue;

        std::string coordIp = loadCoordinator();

        if (coordIp.empty())
        {
            printf("[Agent] No coordinator\n");
            continue;
        }

        SOCKET temp = socket(AF_INET, SOCK_DGRAM, 0);

        DWORD tout = 3000;

        setsockopt(
            temp,
            SOL_SOCKET,
            SO_RCVTIMEO,
            (const char*)&tout,
            sizeof(tout)
        );

        sockaddr_in coord = { 0 };

        coord.sin_family = AF_INET;
        coord.sin_port = htons(5555);
        coord.sin_addr.s_addr = inet_addr(coordIp.c_str());

        sendto(
            temp,
            "gettime",
            7,
            0,
            (sockaddr*)&coord,
            sizeof(coord)
        );

        sockaddr_in from = { 0 };

        int fromSize = sizeof(from);

        int rr = recvfrom(
            temp,
            buffer,
            sizeof(buffer) - 1,
            0,
            (sockaddr*)&from,
            &fromSize
        );

        if (rr == SOCKET_ERROR)
        {
            printf(
                "[Agent] Coordinator timeout: %s\n",
                coordIp.c_str()
            );

            closesocket(temp);

            continue;
        }

        buffer[rr] = '\0';

        printf(
            "[LOG] Client %s -> Coordinator %s\n",
            inet_ntoa(client.sin_addr),
            coordIp.c_str()
        );

        sendto(
            s,
            buffer,
            rr,
            0,
            (sockaddr*)&client,
            clientSize
        );

        closesocket(temp);
    }

    closesocket(s);

    WSACleanup();
}