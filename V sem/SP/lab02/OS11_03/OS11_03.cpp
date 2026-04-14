#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include "../OS11_HTAPI/HT.h"
#include <string>
#include <Windows.h>
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

int main(int argc, char* argv[])
{
    setlocale(LC_ALL, "Rus");
    srand((unsigned int)time(nullptr));

    if (argc != 2) {
        cout << "Usage: OS11_03.exe path_to_storage" << endl;
        return -1;
    }

    const size_t cSize = strlen(argv[1]) + 1;
    wchar_t* wc = new wchar_t[cSize];
    mbstowcs(wc, argv[1], cSize);

    cout << "[CLIENT 03] Starting, waiting for server..." << endl;

    while (true) {
        HANDLE hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
        if (!hServerMutex) {
            cout << "[CLIENT 03] Server not running, waiting..." << endl;
            Sleep(1000);
            continue;
        }
        CloseHandle(hServerMutex);

        HTHandle* HT = OpenExist(wc);
        if (!HT) {
            cout << "[CLIENT 03] Cannot open storage, retrying in 1 second..." << endl;
            Sleep(1000);
            continue;
        }

        cout << "[CLIENT 03] Connected to storage" << endl;

        string key;
        for (int i = 0; i < 100; i++) {
            hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
            if (!hServerMutex) {
                cout << "[CLIENT 03] Server stopped during operation!" << endl;
                break;
            }
            CloseHandle(hServerMutex);

            Sleep(1000);
            key = to_string(rand() % 50);
            cout << "Trying to delete key: " << key << endl;

            Element* searchKey = new Element(key.c_str(), (int)key.length() + 1);
            HT::Element* elFromHT = Get(HT, searchKey);

            if (!elFromHT) {
                cout << "[CLIENT 03] ERROR: Key not found - " << key << endl;
                delete searchKey;
                continue;
            }

            cout << "Found element to delete: ";
            Print(elFromHT);

            if (Delete(HT, elFromHT)) {
                cout << "[CLIENT 03] SUCCESS: Deleted element with key: " << key << endl;
            }
            else {
                cout << "[CLIENT 03] ERROR: Failed to delete key: " << key << " - " << HT->LastErrorMessage << endl;
            }

            delete searchKey;
            cout << "Current elements amount: " << HT->ElementCount << endl;
        }

        Close(HT);
        cout << "[CLIENT 03] Storage closed, waiting for server restart..." << endl;
        Sleep(1000);
    }

    delete[] wc;
    return 0;
}