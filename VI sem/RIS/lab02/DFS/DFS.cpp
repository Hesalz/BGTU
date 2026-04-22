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
    char ipaddr[15];
    char resource[20];
    enum STATUS {
        NOINIT, INIT, ENTER, LEAVE, WAIT
    } status;
};

SOCKADDR_IN serverAddr;

//отправляем запрос и ожидаем ответ
bool SendAndReceive(SOCKET sock, CA& ca, CA::STATUS expectedResponse) {
    int sent = sendto(sock, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&serverAddr, sizeof(serverAddr));

    if (sent == SOCKET_ERROR) {
        return false;
    }

    CA response;
    memset(&response, 0, sizeof(response));       //обнуляем память
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;                          //таймаут ожидания ответа
    setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(sock, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        return false;
    }

    if (response.status == expectedResponse) {
        ca.status = response.status;
        return true;
    }

    return false;
}

CA InitCA(char ipaddr[15], char resource[20], SOCKET& sock) {
    CA ca;
    strncpy_s(ca.ipaddr, 15, ipaddr, _TRUNCATE);
    strncpy_s(ca.resource, 20, resource, _TRUNCATE);
    ca.status = CA::NOINIT;

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        return ca;
    }

    sock = socket(AF_INET, SOCK_DGRAM, 0);
    if (sock == INVALID_SOCKET) {
        WSACleanup();
        return ca;
    }

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(2000);
    serverAddr.sin_addr.s_addr = inet_addr(ipaddr);

    ca.status = CA::INIT;
    if (SendAndReceive(sock, ca, CA::INIT)) {
        //успешно
    }
    else {
        ca.status = CA::NOINIT;
        closesocket(sock);
        WSACleanup();
    }

    return ca;
}

bool EnterCA(SOCKET sock, CA& ca) {
    if (ca.status != CA::INIT && ca.status != CA::WAIT) return false;

    ca.status = CA::ENTER;

    int sent = sendto(sock, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&serverAddr, sizeof(serverAddr));

    if (sent == SOCKET_ERROR) return false;

    CA response;
    memset(&response, 0, sizeof(response));
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;
    setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(sock, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) return false;

    if (response.status == CA::ENTER) {
        ca.status = CA::ENTER;
        return true;
    }
    else if (response.status == CA::WAIT) {
        ca.status = CA::WAIT;
        while (true) {
            CA grantMsg;
            memset(&grantMsg, 0, sizeof(grantMsg));

            received = recvfrom(sock, (char*)&grantMsg, sizeof(grantMsg), 0,
                (SOCKADDR*)&fromAddr, &fromLen);

            if (received == SOCKET_ERROR) return false;

            if (grantMsg.status == CA::ENTER) {
                ca.status = CA::ENTER;
                return true;
            }
        }
    }

    return false;
}

bool LeaveCA(SOCKET sock, CA& ca) {
    ca.status = CA::LEAVE;

    if (SendAndReceive(sock, ca, CA::LEAVE)) {
        return true;
    }
    return false;
}

bool CloseCA(SOCKET sock, CA& ca) {
    ca.status = CA::NOINIT;

    if (SendAndReceive(sock, ca, CA::NOINIT)) {
        closesocket(sock);
        WSACleanup();
        return true;
    }

    closesocket(sock);
    WSACleanup();
    return false;
}

//абстракция файла
struct DFS_File {
    string fileName;
    FILE* fileHandle;                          //указатель на открытый файл
    CA ca;
    SOCKET sock;
    bool isOpen;                              //открыт ли файл
};

typedef DFS_File* HDFS;

//чтоб создать объект файла
HDFS OpenDFSFIle(char* FileName, char* ServerIP) {
    HDFS hdfs = new DFS_File;                             //выделяем память
    hdfs->fileName = FileName;
    hdfs->isOpen = false;
    hdfs->fileHandle = nullptr;
    hdfs->sock = INVALID_SOCKET;

    //инициализация критической секции
    hdfs->ca = InitCA(ServerIP, FileName, hdfs->sock);

    if (hdfs->ca.status != CA::INIT) {
        cout << "[DFS] Ошибка инициализации (статус=" << hdfs->ca.status << ")" << endl;
        delete hdfs;
        return nullptr;
    }

    //вход в критическую секцию
    if (!EnterCA(hdfs->sock, hdfs->ca)) {
        cout << "[DFS] Не удалось войти в критическую секцию" << endl;
        CloseCA(hdfs->sock, hdfs->ca);
        delete hdfs;
        return nullptr;
    }

    //открытие файла
    hdfs->fileHandle = fopen(FileName, "a+");                         //a+ - append (добавление) + чтение.
    if (!hdfs->fileHandle) {
        cout << "[DFS] Не удалось открыть файл: " << FileName << endl;
        LeaveCA(hdfs->sock, hdfs->ca);
        CloseCA(hdfs->sock, hdfs->ca);
        delete hdfs;
        return nullptr;
    }

    //возвр дескриптор файла
    hdfs->isOpen = true;
    cout << "[DFS] Файл успешно открыт: " << FileName << endl;
    return hdfs;
}

int WriteDFSFIle(HDFS hdfs, void* buf, int bufsize) {
    if (!hdfs || !hdfs->isOpen || !hdfs->fileHandle) {
        return -1;
    }

    int written = fwrite(buf, 1, bufsize, hdfs->fileHandle);
    fflush(hdfs->fileHandle);                                    //принуд запись сразу на диск
    return written;
}

int ReadDFSFIle(HDFS hdfs, void* buf, int bufsize) {
    if (!hdfs || !hdfs->isOpen || !hdfs->fileHandle) {
        return -1;
    }

    int read = fread(buf, 1, bufsize, hdfs->fileHandle);

    if (read == 0 && feof(hdfs->fileHandle)) {
        return 0;
    }
    return read;
}

void CloseDFSFIle(HDFS hdfs) {
    if (!hdfs) return;

    if (hdfs->isOpen && hdfs->fileHandle) {
        fclose(hdfs->fileHandle);
        cout << "[DFS] Файл закрыт: " << hdfs->fileName << endl;
    }

    if (hdfs->sock != INVALID_SOCKET) {
        LeaveCA(hdfs->sock, hdfs->ca);                  //вых из секции
        CloseCA(hdfs->sock, hdfs->ca);                  //закрываем секцию
    }

    delete hdfs;                                      //освобождаем память
}

void DemonstrateWriteRead() {
    cout << "ДЕМОНСТРАЦИЯ ЗАПИСИ 10 СТРОК" << endl;

    char fileName[] = "Y:\\server.txt";
    char serverIP[] = "26.83.199.121";

    HDFS hdfs = OpenDFSFIle(fileName, serverIP);                     //откр файл через апи

    if (!hdfs) {
        cout << "[ОШИБКА] Не удалось открыть файл" << endl;
        return;
    }

    cout << "\n--- ЗАПИСЬ 10 СТРОК ---" << endl;
    for (int i = 1; i <= 10; i++) {
        char buffer[256];
        time_t now = time(NULL);
        struct tm* timeinfo = localtime(&now);
        char timeStr[80];
        strftime(timeStr, sizeof(timeStr), "%Y-%m-%d %H:%M:%S", timeinfo);

        sprintf(buffer, "[DFS Note %d] %s\n", i, timeStr);
        int written = WriteDFSFIle(hdfs, buffer, (int)strlen(buffer));

        if (written > 0) {
            cout << "Записана строка " << i << ": " << buffer;
        }
        else {
            cout << "Ошибка записи строки " << i << endl;
        }

        Sleep(500);
    }

    cout << "\n--- ЗАПИСЬ ЗАВЕРШЕНА ---" << endl;

    CloseDFSFIle(hdfs);

    cout << "\n--- ЧТЕНИЕ ФАЙЛА ---" << endl;

    hdfs = OpenDFSFIle(fileName, serverIP);
    if (!hdfs) {
        cout << "[ОШИБКА] Не удалось открыть файл для чтения" << endl;
        return;
    }

    char buffer[1024];
    int bytesRead;
    int lineCount = 0;

    fseek(hdfs->fileHandle, 0, SEEK_SET);

    while ((bytesRead = ReadDFSFIle(hdfs, buffer, sizeof(buffer) - 1)) > 0) {
        buffer[bytesRead] = '\0';
        cout << buffer;

        for (int i = 0; i < bytesRead; i++) {
            if (buffer[i] == '\n') lineCount++;
            if (lineCount >= 10) break;
        }
        if (lineCount >= 10) break;
    }

    cout << "\n--- ЧТЕНИЕ ЗАВЕРШЕНО (показано " << lineCount << " строк) ---" << endl;

    CloseDFSFIle(hdfs);
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "rus");

    cout << "ДЕМОНСТРАЦИЯ API DFS" << endl;

    DemonstrateWriteRead();

    cout << "ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА" << endl;
    system("pause");
    return 0;
}