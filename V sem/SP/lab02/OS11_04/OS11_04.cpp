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
        cout << "Usage: OS11_04.exe path_to_storage" << endl;
        return -1;
    }

    const size_t cSize = strlen(argv[1]) + 1;
    wchar_t* wc = new wchar_t[cSize];
    mbstowcs(wc, argv[1], cSize);

    cout << "[CLIENT 04] Starting, waiting for server..." << endl;

    while (true) {
        HANDLE hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
        if (!hServerMutex) {
            cout << "[CLIENT 04] Server not running, waiting..." << endl;
            Sleep(1000);
            continue;
        }
        CloseHandle(hServerMutex);

        HTHandle* HT = OpenExist(wc);
        if (!HT) {
            cout << "[CLIENT 04] Cannot open storage, retrying in 1 second..." << endl;
            Sleep(1000);
            continue;
        }

        cout << "[CLIENT 04] Connected to storage" << endl;

        string key;
        for (int i = 0; i < 100; i++) {
            hServerMutex = OpenMutex(SYNCHRONIZE, FALSE, L"HT_SERVER_MUTEX");
            if (!hServerMutex) {
                cout << "[CLIENT 04] Server stopped during operation!" << endl;
                break;
            }
            CloseHandle(hServerMutex);

            Sleep(1000);
            key = to_string(rand() % 50);
            cout << "Trying to update key: " << key << endl;

            Element* searchKey = new Element(key.c_str(), (int)key.length() + 1);
            HT::Element* elFromHT = Get(HT, searchKey);

            if (!elFromHT) {
                cout << "[CLIENT 04] ERROR: Key not found - " << key << endl;
                delete searchKey;
                continue;
            }

            cout << "Found element: ";
            Print(elFromHT);

            int newPayload = atoi((char*)elFromHT->payload) + 1;
            string newPayloadStr = to_string(newPayload);

            cout << "Updating payload from " << (char*)elFromHT->payload << " to " << newPayloadStr << endl;

            if (Update(HT, elFromHT, newPayloadStr.c_str(), (int)newPayloadStr.length() + 1)) {
                cout << "[CLIENT 04] SUCCESS: Updated element with key: " << key
                    << ", new payload: " << newPayloadStr << endl;

                HT::Element* updatedElement = Get(HT, searchKey);
                if (updatedElement) {
                    cout << "Updated element: ";
                    Print(updatedElement);
                }
            }
            else {
                cout << "[CLIENT 04] ERROR: Failed to update key: " << key << " - " << HT->LastErrorMessage << endl;
            }

            delete searchKey;
            cout << "Current elements amount: " << HT->ElementCount << endl << endl;
        }

        Close(HT);
        cout << "[CLIENT 04] Storage closed, waiting for server restart..." << endl;
        Sleep(1000);
    }

    delete[] wc;
    return 0;
}