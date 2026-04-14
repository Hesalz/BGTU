#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <string>
#include <Windows.h>
#include "../OS11_HTAPI/HT.h"

using namespace std;
using namespace HT;

int main(int argc, char* argv[])
{
    if (argc != 2) {
        cout << "Usage: start.exe path_to_storage" << endl;
        return -1;
    }

    const size_t cSize = strlen(argv[1]) + 1;
    wchar_t* wc = new wchar_t[cSize];
    mbstowcs(wc, argv[1], cSize);

    HANDLE hServerMutex = CreateMutex(NULL, TRUE, L"HT_SERVER_MUTEX");
    if (GetLastError() == ERROR_ALREADY_EXISTS) {
        cout << "Server already running!" << endl;
        return -1;
    }

    try {
        HTHandle* ht = Open(wc); 
        if (!ht) throw "Cannot open storage";

        cout << "Server started..." << endl;
        wcout << L"Storage file: " << wc << endl;

        while (true) {
            Sleep(ht->SecSnapshotInterval * 1000);
            Snap(ht); 
            cout << "=========SNAPSHOT=========" << endl;
        }

        Close(ht);
    }
    catch (const char* err) {
        cout << "Error: " << err << endl;
    }

    if (hServerMutex) CloseHandle(hServerMutex);
    delete[] wc;

    return 0;
}
