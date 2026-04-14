#include <iostream>
#include <windows.h>
#include <string>
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
    case ERROR_NOT_ENOUGH_MEMORY:
        result = msg + "Недостаточно памяти";
        break;
    case ERROR_INVALID_PARAMETER:
        result = msg + "Неверный параметр";
        break;
    case ERROR_IO_INCOMPLETE:
        result = msg + "Операция ввода-вывода не завершена";
        break;
    case ERROR_IO_PENDING:
        result = msg + "Операция ввода-вывода ожидает завершения";
        break;
    case ERROR_OPERATION_ABORTED:
        result = msg + "Операция прервана";
        break;
    case ERROR_SEM_TIMEOUT:
        result = msg + "Таймаут семафора";
        break;
    case ERROR_PIPE_BUSY:
        result = msg + "Канал занят";
        break;
    case ERROR_BROKEN_PIPE:
        result = msg + "Канал разорван";
        break;
    case ERROR_NO_DATA:
        result = msg + "Нет данных";
        break;
    case ERROR_HANDLE_EOF:
        result = msg + "Конец файла";
        break;
    case ERROR_INSUFFICIENT_BUFFER:
        result = msg + "Недостаточный размер буфера";
        break;
    default:
        result = msg + "Неизвестная ошибка: " + to_string(errorNumber);
        break;
    }
    return result;
}

int main() {
    // Задание 1, 3: создание mailslot с интервалом ожидания 3 минуты (180000 мс) и размером сообщения 500 байт
    HANDLE sH = CreateMailslot(TEXT("\\\\.\\mailslot\\Box"), 500, 180000, NULL);
    cout << "server listening..." << endl;
    if (sH == INVALID_HANDLE_VALUE) {
        cerr << SetErrorMsgText("create: ", GetLastError()) << endl;
        return 1;
    }

    // Задание 2: чтение сообщения
    char rbuf[512]; // Увеличим буфер для сообщений до 500 байт (Задание 8)
    DWORD bytesRead;
    if (!ReadFile(sH, rbuf, sizeof(rbuf) - 1, &bytesRead, NULL)) {
        DWORD error = GetLastError();
        if (error == ERROR_SEM_TIMEOUT) {
            cout << "Timeout expired while reading from Mailslot." << endl;
        }
        else {
            cerr << SetErrorMsgText("read: ", GetLastError()) << endl;
        }
    }
    else {
        rbuf[bytesRead] = '\0';
        cout << "Bytes received: " << bytesRead << endl;
        cout << "Received message: " << rbuf << endl;
    }

    // Задание 9: непрерывное чтение сообщений
    while (true) {
        if (!ReadFile(sH, rbuf, sizeof(rbuf), &bytesRead, NULL)) {
            DWORD error = GetLastError();
            if (error == ERROR_SEM_TIMEOUT) {
                cout << "Timeout expired while reading from Mailslot." << endl;
                break;
            }
            else {
                cerr << "Error reading from mailslot: " << SetErrorMsgText("", error) << endl;
            }
            continue;
        }
        rbuf[bytesRead] = '\0';
        cout << "Received message: " << rbuf << endl;
    }

    CloseHandle(sH);
    return 0;
}