#define _CRT_SECURE_NO_WARNINGS
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <stdio.h>
#include <time.h>
#include <thread>
#include <vector>
#include <string>
#include <fstream>
#include <algorithm>
#include <WinSock2.h>

#pragma comment(lib, "WS2_32.lib")

SOCKET s;

std::string my_ip;
std::string coordinator_ip;

bool is_coordinator = false;
bool election = false;
bool got_ok = false;

int missed = 0;

std::vector<std::string> nodes;

static std::string trim(const std::string& s)
{
    size_t start = s.find_first_not_of(" \t\r\n");

    if (start == std::string::npos)
        return "";

    size_t end = s.find_last_not_of(" \t\r\n");

    return s.substr(start, end - start + 1);
}

unsigned long ipToNum(const std::string& ip)
{
    return ntohl(inet_addr(ip.c_str()));
}

void sendMessage(
    const std::string& ip,
    const std::string& msg
)
{
    sockaddr_in addr = { 0 };

    addr.sin_family = AF_INET;
    addr.sin_port = htons(5555);
    addr.sin_addr.s_addr = inet_addr(ip.c_str());

    sendto(
        s,
        msg.c_str(),
        (int)msg.length(),
        0,
        (sockaddr*)&addr,
        sizeof(addr)
    );
}

void saveCoordinator()
{
    std::ofstream file("config.txt", std::ios::trunc);

    file << coordinator_ip;
}

void loadNodes()
{
    std::ifstream file("nodes.txt");

    std::string ip;

    while (std::getline(file, ip))
    {
        ip = trim(ip);

        if (!ip.empty())
            nodes.push_back(ip);
    }
}

void initializeCoordinator()
{
    std::ifstream file("config.txt");

    std::string fileCoordinator;

    if (file.is_open())
        std::getline(file, fileCoordinator);

    fileCoordinator = trim(fileCoordinator);

    if (!fileCoordinator.empty())
    {
        coordinator_ip = fileCoordinator;
    }
    else
    {
        auto it = std::max_element(
            nodes.begin(),
            nodes.end(),
            [](const std::string& a, const std::string& b)
            {
                return ipToNum(a) < ipToNum(b);
            }
        );

        coordinator_ip = *it;
    }

    if (coordinator_ip == my_ip)
    {
        is_coordinator = true;

        saveCoordinator();

        printf("[Server] I am coordinator\n");
    }
    else
    {
        is_coordinator = false;

        printf(
            "[Server] Coordinator: %s\n",
            coordinator_ip.c_str()
        );

        unsigned long myNum = ipToNum(my_ip);
        unsigned long coordNum = ipToNum(coordinator_ip);

        if (myNum > coordNum)
        {
            printf("[Bully] Higher node joined cluster. Starting election.\n");

            election = true;

            got_ok = false;

            for (auto& ip : nodes)
            {
                if (ipToNum(ip) > myNum)
                    sendMessage(ip, "election");
            }

            Sleep(3000);

            if (!got_ok)
            {
                is_coordinator = true;

                coordinator_ip = my_ip;

                saveCoordinator();

                printf("[Bully] I am new coordinator\n");

                for (auto& ip : nodes)
                {
                    if (ip != my_ip)
                        sendMessage(ip, "coordinator");
                }
            }

            election = false;
        }
    }
}

void electionThread()
{
    while (true)
    {
        Sleep(5000);

        if (is_coordinator)
            continue;

        sendMessage(coordinator_ip, "ping");

        missed++;

        if (missed < 3)
            continue;

        if (election)
            continue;

        printf("[Bully] Coordinator down\n");

        election = true;

        got_ok = false;

        unsigned long myNum = ipToNum(my_ip);

        for (auto& ip : nodes)
        {
            if (ipToNum(ip) > myNum)
                sendMessage(ip, "election");
        }

        Sleep(3000);

        if (!got_ok)
        {
            is_coordinator = true;

            coordinator_ip = my_ip;

            saveCoordinator();

            printf("[Bully] I am new coordinator\n");

            for (auto& ip : nodes)
            {
                if (ip != my_ip)
                    sendMessage(ip, "coordinator");
            }
        }

        election = false;

        missed = 0;
    }
}

void listenerThread()
{
    char buffer[256];

    while (true)
    {
        sockaddr_in from = { 0 };

        int fromSize = sizeof(from);

        int r = recvfrom(
            s,
            buffer,
            sizeof(buffer) - 1,
            0,
            (sockaddr*)&from,
            &fromSize
        );

        if (r <= 0)
            continue;

        buffer[r] = '\0';

        std::string msg(buffer);

        std::string sender = inet_ntoa(from.sin_addr);

        if (msg == "gettime")
        {
            if (!is_coordinator)
                continue;

            time_t t = time(NULL);

            tm now;

            localtime_s(&now, &t);

            char out[64];

            strftime(
                out,
                sizeof(out),
                "%d%m%Y:%H:%M:%S",
                &now
            );

            sendto(
                s,
                out,
                (int)strlen(out),
                0,
                (sockaddr*)&from,
                fromSize
            );

            printf(
                "[Server] Time sent to %s\n",
                sender.c_str()
            );
        }
        else if (msg == "ping")
        {
            sendMessage(sender, "pong");
        }
        else if (msg == "pong")
        {
            missed = 0;
        }
        else if (msg == "election")
        {
            sendMessage(sender, "ok");

            if (!election)
            {
                election = true;

                got_ok = false;

                unsigned long myNum = ipToNum(my_ip);

                for (auto& ip : nodes)
                {
                    if (ipToNum(ip) > myNum)
                        sendMessage(ip, "election");
                }

                std::thread([]()
                    {
                        Sleep(3000);

                        if (!got_ok)
                        {
                            is_coordinator = true;

                            coordinator_ip = my_ip;

                            saveCoordinator();

                            printf("[Bully] I am new coordinator\n");

                            for (auto& ip : nodes)
                            {
                                if (ip != my_ip)
                                    sendMessage(ip, "coordinator");
                            }
                        }

                        election = false;

                        missed = 0;

                    }).detach();
            }
        }
        else if (msg == "ok")
        {
            got_ok = true;
        }
        else if (msg == "coordinator")
        {
            coordinator_ip = sender;

            if (coordinator_ip != my_ip)
                is_coordinator = false;

            missed = 0;

            election = false;

            saveCoordinator();

            printf(
                "[Bully] New coordinator: %s\n",
                sender.c_str()
            );
        }
    }
}

int main(int argc, char* argv[])
{
    if (argc < 2)
    {
        printf("Usage: %s <IP>\n", argv[0]);

        return -1;
    }

    my_ip = argv[1];

    loadNodes();

    WSAData wsadata;

    WSAStartup(MAKEWORD(2, 2), &wsadata);

    s = socket(AF_INET, SOCK_DGRAM, 0);

    sockaddr_in addr = { 0 };

    addr.sin_family = AF_INET;
    addr.sin_port = htons(5555);
    addr.sin_addr.s_addr = inet_addr(my_ip.c_str());

    if (bind(
        s,
        (sockaddr*)&addr,
        sizeof(addr)
    ) == SOCKET_ERROR)
    {
        printf(
            "Bind error for %s\n",
            my_ip.c_str()
        );

        return -1;
    }

    initializeCoordinator();

    printf(
        "[Server] Started on %s\n",
        my_ip.c_str()
    );

    std::thread t1(listenerThread);
    std::thread t2(electionThread);

    t1.join();
    t2.join();

    closesocket(s);

    WSACleanup();
}