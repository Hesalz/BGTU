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

static void LogWinsockError(const char* tag, const char* action) {
    cout << "[" << tag << "] " << action << " failed: " << WSAGetLastError() << endl;
}

bool SendAndReceive(SOCKET sock, CA& ca, CA::STATUS expectedResponse) {
    int sent = sendto(sock, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&serverAddr, sizeof(serverAddr));

    if (sent == SOCKET_ERROR) {
        LogWinsockError("CA", "sendto");
        return false;
    }

    CA response;
    memset(&response, 0, sizeof(response));  
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;                         
    setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(sock, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        int error = WSAGetLastError();
        if (error != WSAETIMEDOUT) {
            cout << "[CA] recvfrom failed: " << error << endl;
        }
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

    cout << "[InitCA] Initializing critical section for resource '" << resource << "' via " << ipaddr << ":2000" << endl;

    WSADATA wsaData;
    if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
        cout << "[InitCA] WSAStartup failed: " << WSAGetLastError() << endl;
        return ca;
    }

    sock = socket(AF_INET, SOCK_DGRAM, 0);
    if (sock == INVALID_SOCKET) {
        cout << "[InitCA] Socket creation failed: " << WSAGetLastError() << endl;
        WSACleanup();
        return ca;
    }

    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(2000);
    serverAddr.sin_addr.s_addr = inet_addr(ipaddr);

    ca.status = CA::INIT;
    if (SendAndReceive(sock, ca, CA::INIT)) {
        cout << "[InitCA] Critical section initialized" << endl;
    }
    else {
        cout << "[InitCA] Initialization failed" << endl;
        ca.status = CA::NOINIT;
        closesocket(sock);
        WSACleanup();
    }

    return ca;
}

bool EnterCA(SOCKET sock, CA& ca) {
    if (ca.status != CA::INIT && ca.status != CA::WAIT) return false;

    cout << "[EnterCA] Requesting entry to critical section..." << endl;
    ca.status = CA::ENTER;

    int sent = sendto(sock, (char*)&ca, sizeof(ca), 0,
        (SOCKADDR*)&serverAddr, sizeof(serverAddr));

    if (sent == SOCKET_ERROR) {
        LogWinsockError("EnterCA", "sendto");
        return false;
    }

    CA response;
    memset(&response, 0, sizeof(response));
    SOCKADDR_IN fromAddr;
    int fromLen = sizeof(fromAddr);

    int timeout = 30000;
    setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));

    int received = recvfrom(sock, (char*)&response, sizeof(response), 0,
        (SOCKADDR*)&fromAddr, &fromLen);

    if (received == SOCKET_ERROR) {
        LogWinsockError("EnterCA", "recvfrom");
        return false;
    }

    if (response.status == CA::ENTER) {
        ca.status = CA::ENTER;
        cout << "[EnterCA] Entry GRANTED" << endl;
        return true;
    }
    else if (response.status == CA::WAIT) {
        ca.status = CA::WAIT;
        cout << "[EnterCA] BUSY: waiting for grant..." << endl;
        while (true) {
            CA grantMsg;
            memset(&grantMsg, 0, sizeof(grantMsg));

            received = recvfrom(sock, (char*)&grantMsg, sizeof(grantMsg), 0,
                (SOCKADDR*)&fromAddr, &fromLen);

            if (received == SOCKET_ERROR) {
                LogWinsockError("EnterCA", "recvfrom(wait)");
                return false;
            }

            if (grantMsg.status == CA::ENTER) {
                ca.status = CA::ENTER;
                cout << "[EnterCA] Entry GRANTED" << endl;
                return true;
            }
        }
    }

    cout << "[EnterCA] Unexpected response status: " << response.status << endl;
    return false;
}

bool LeaveCA(SOCKET sock, CA& ca) {
    cout << "[LeaveCA] Leaving critical section..." << endl;
    ca.status = CA::LEAVE;

    if (SendAndReceive(sock, ca, CA::LEAVE)) {
        cout << "[LeaveCA] Leave confirmed" << endl;
        return true;
    }
    cout << "[LeaveCA] Leave failed" << endl;
    return false;
}

bool CloseCA(SOCKET sock, CA& ca) {
    cout << "[CloseCA] Closing critical section..." << endl;
    ca.status = CA::NOINIT;

    if (SendAndReceive(sock, ca, CA::NOINIT)) {
        cout << "[CloseCA] Close confirmed" << endl;
        closesocket(sock);
        WSACleanup();
        return true;
    }

    cout << "[CloseCA] Close failed" << endl;
    closesocket(sock);
    WSACleanup();
    return false;
}

struct DFS_File {
    string fileName;
    FILE* fileHandle;                          
    CA ca;
    SOCKET sock;
    bool isOpen;                           
};

typedef DFS_File* HDFS;

HDFS OpenDFSFIle(char* FileName, char* ServerIP) {
    HDFS hdfs = new DFS_File;                            
    hdfs->fileName = FileName;
    hdfs->isOpen = false;
    hdfs->fileHandle = nullptr;
    hdfs->sock = INVALID_SOCKET;

    cout << "[OpenDFSFile] Opening '" << FileName << "'" << endl;

    hdfs->ca = InitCA(ServerIP, FileName, hdfs->sock);

    if (hdfs->ca.status != CA::INIT) {
        cout << "[DFS] Initialization failed (status=" << hdfs->ca.status << ")" << endl;
        delete hdfs;
        return nullptr;
    }

    if (!EnterCA(hdfs->sock, hdfs->ca)) {
        cout << "[DFS] Failed to enter critical section" << endl;
        CloseCA(hdfs->sock, hdfs->ca);
        delete hdfs;
        return nullptr;
    }

    hdfs->fileHandle = fopen(FileName, "a+");                         
    if (!hdfs->fileHandle) {
        cout << "[DFS] Failed to open file: " << FileName << endl;
        LeaveCA(hdfs->sock, hdfs->ca);
        CloseCA(hdfs->sock, hdfs->ca);
        delete hdfs;
        return nullptr;
    }

    hdfs->isOpen = true;
    cout << "[OpenDFSFile] File opened successfully: " << FileName << endl;
    return hdfs;
}

int WriteDFSFIle(HDFS hdfs, void* buf, int bufsize) {
    if (!hdfs || !hdfs->isOpen || !hdfs->fileHandle) {
        return -1;
    }

    int written = fwrite(buf, 1, bufsize, hdfs->fileHandle);
    fflush(hdfs->fileHandle);                                   
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
        cout << "[CloseDFSFile] File closed: " << hdfs->fileName << endl;
    }

    if (hdfs->sock != INVALID_SOCKET) {
        LeaveCA(hdfs->sock, hdfs->ca);                
        CloseCA(hdfs->sock, hdfs->ca);                 
    }

    delete hdfs;                                   
}

static void DemonstrateWriteRead(int delayBeforeStartMs) {
    cout << "Demo: write 10 lines" << endl;

    char fileName[] = "Z:\\Lab_2.txt";
    char serverIP[] = "172.20.10.3";

    if (delayBeforeStartMs > 0) {
        cout << "[SYSTEM] Delay " << delayBeforeStartMs << " ms..." << endl;
        Sleep(delayBeforeStartMs);
    }

    HDFS hdfs = OpenDFSFIle(fileName, serverIP);                  

    if (!hdfs) {
        cout << "[ERROR] Failed to open file" << endl;
        return;
    }

    cout << "\n--- Writing 10 lines ---" << endl;
    for (int i = 1; i <= 10; i++) {
        char buffer[256];
        time_t now = time(NULL);
        struct tm* timeinfo = localtime(&now);
        char timeStr[80];
        strftime(timeStr, sizeof(timeStr), "%Y-%m-%d %H:%M:%S", timeinfo);

        sprintf(buffer, "[DFS record %d] %s\n", i, timeStr);
        int written = WriteDFSFIle(hdfs, buffer, (int)strlen(buffer));

        if (written > 0) {
            cout << "Wrote record " << i << ": " << buffer;
        }
        else {
            cout << "Write error at record " << i << endl;
        }

        Sleep(500);
    }

    cout << "\n--- Write completed ---" << endl;

    CloseDFSFIle(hdfs);

    cout << "\n--- Reading file ---" << endl;

    hdfs = OpenDFSFIle(fileName, serverIP);
    if (!hdfs) {
        cout << "[ERROR] Failed to open file for reading" << endl;
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

    cout << "\n--- Read completed (read " << lineCount << " lines) ---" << endl;

    CloseDFSFIle(hdfs);
}

int main(int argc, char* argv[]) {
    setlocale(LC_ALL, "C");

    cout << "DFS API demo" << endl;

    int delayBeforeStartMs = 0;
    if (argc > 1) {
        delayBeforeStartMs = atoi(argv[1]);
    }

    DemonstrateWriteRead(delayBeforeStartMs);
    system("pause");
    return 0;
}