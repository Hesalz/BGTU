#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <string>
#include <Windows.h>
#include "../OS11_HTAPI/HT.h"
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

int main(int argc, char* argv[])
{
    if (argc != 2) {
        cout << "Usage: os11_02.exe path_to_storage" << endl;
        return -1;
    }

    const size_t cSize = strlen(argv[1]) + 1;
    wchar_t* wc = new wchar_t[cSize];
    mbstowcs(wc, argv[1], cSize);

    cout << "[CLIENT] Starting, waiting for server..." << endl;

    while (true) {
        HANDLE hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
        if (!hServerMutex) {
            cout << "[CLIENT] Server not running, retrying in 1 second..." << endl;
            Sleep(1000);
            continue;
        }
        CloseHandle(hServerMutex);

        HTHandle* ht = OpenExist(wc);
        if (!ht) {
            cout << "[CLIENT] Cannot open storage, retrying in 1 second..." << endl;
            Sleep(1000);
            continue;
        }

        cout << "[CLIENT] Connected to storage" << endl;

        string key, payload = "0";
        for (int i = 0; i < 50; ++i) {
            hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
            if (!hServerMutex) {
                cout << "[CLIENT] Server stopped during insertion!" << endl;
                break;
            }
            CloseHandle(hServerMutex);

            key = to_string(rand() % 50);
            Element* element = new Element(key.c_str(), (int)key.length() + 1, payload.c_str(), (int)payload.length() + 1);

            cout << "Inserting element: ";
            Print(element);

            if (Insert(ht, element)) {
                cout << "[CLIENT] SUCCESS: Inserted element with key: " << key << endl;
            }
            else {
                cout << "[CLIENT] ERROR: Failed to insert element with key: " << key << " - " << ht->LastErrorMessage << endl;
            }

            delete element;
            Sleep(1000);
        }

        Close(ht);
        cout << "[CLIENT] Storage closed, waiting for server restart..." << endl;
        Sleep(1000);
    }

    delete[] wc;
    return 0;
}