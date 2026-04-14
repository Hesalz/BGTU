#include <iostream>
#include <windows.h>
#include <string>
#include <chrono>
#include <vector>
#define _CRT_SECURE_NO_WARNINGS
#pragma warning(disable : 4996)
#include <tchar.h>
using namespace std;

string SetErrorMsgText(string msg, int errorNumber) {
    string result = "";
    switch (errorNumber)
    {
    case ERROR_FILE_NOT_FOUND:
        result = msg + "Файл не найден";
        break;
    case ERROR_PATH_NOT_FOUND:
        result = msg + "Путь не найден";
        break;
    case ERROR_ACCESS_DENIED:
        result = msg + "Доступ запрещен";
        break;
    case ERROR_INVALID_HANDLE:
        result = msg + "Неверный дескриптор";
        break;
    case ERROR_BROKEN_PIPE:
        result = msg + "Канал разорван";
        break;
    case ERROR_PIPE_BUSY:
        result = msg + "Канал занят";
        break;
    default:
        result = msg + "Неизвестная ошибка: " + to_string(errorNumber);
        break;
    }
    return result;
}

//Задание 7
void SendMessageToServers(const char* serverName, const char* message) {
    HANDLE cH = CreateFileA(serverName, GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (cH == INVALID_HANDLE_VALUE) {
        cerr << "Failed to create file. Error: " << SetErrorMsgText("", GetLastError()) << endl;
        return;
    }
    else {
        cout << "File created successfully" << endl;
    }

    DWORD bytesSended;
    if (!WriteFile(cH, message, strlen(message) + 1, &bytesSended, NULL)) {
        cerr << "Failed to write to Mailslot. Error: " << SetErrorMsgText("", GetLastError()) << endl;
    }
    else {
        cout << "Message sent successfully. Bytes sent: " << bytesSended << endl;
    }

    CloseHandle(cH);
}

int main() {
    // Задание 4, 5: подключение к локальному серверу
    HANDLE cH = CreateFile(TEXT("\\\\.\\mailslot\\Box"), GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (cH == INVALID_HANDLE_VALUE) {
        cerr << "Failed to create file. Error: " << SetErrorMsgText("", GetLastError()) << endl;
        return 1;
    }
    else {
        cout << "File created successfully" << endl;
    }

    // Задание 9: оценка скорости пересылки 1000 сообщений
    auto start = chrono::high_resolution_clock::now();
    for (int i = 0; i < 1000; ++i) {
        char message[100];
        sprintf(message, "Message %d from client", i);
        DWORD bytesSended;

        if (!WriteFile(cH, message, strlen(message) + 1, &bytesSended, NULL)) {
            cerr << "Error sending message: " << SetErrorMsgText("", GetLastError()) << " - " << message << endl;
            break;
        }

        if (i % 100 == 0) {
            cout << "Message " << i << " sent" << endl;
        }
        Sleep(5);
    }

    auto end = chrono::high_resolution_clock::now();
    chrono::duration<double> duration = end - start;
    cout << "Time taken to send 1000 messages: " << duration.count() << " seconds" << endl;

    char message[100] = "Hello from Mailslot-client";
    DWORD bytesSended;
    if (!WriteFile(cH, message, strlen(message) + 1, &bytesSended, NULL)) {
        cerr << "Error sending message: " << SetErrorMsgText("", GetLastError()) << " - " << message << endl;
    }
    else {
        cout << "Test message sent successfully: " << message << endl;
    }
    CloseHandle(cH);

    // Задание 6, 7: отправка сообщений на удаленные серверы
    /*const char* nameServer1 = "\\\\.\\mailslot\\Box";
    const char* message1 = "Hello from client to MSI";
    cout << "\nSending to fisrt server..." << endl;
    SendMessageToServers(nameServer1, message1);

    const char* nameServer2 = "\\\\.\\mailslot\\Box";
    const char* message2 = "Hello from client to DESKTOP";
    cout << "\nSending to second server..." << endl;
    SendMessageToServers(nameServer2, message2);

    const char* nameServer3 = "\\\\.\\mailslot\\Box";
    const char* message3 = "Hello from client to OTHERPC";
    cout << "\nSending to third server..." << endl;
    SendMessageToServers(nameServer3, message3);*/

    return 0;
}