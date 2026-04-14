#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <windows.h>
#include "../OS11_HTAPI/HT.h"
#include <string>
#pragma comment(lib, "OS11_HTAPI")

using namespace std;
using namespace HT;

bool checkValue(const char* input);

int main(int argc, char* argv[])
{
    char filename[256];
    int capacity, snapshotinterval, maxKeyLength, maxPayloadLength;

    try
    {
        cout << "=== HT STORAGE CREATION ===" << endl;

        if (argc != 6)
        {
            if (argc == 1)
            {
                cout << "Enter filename (e.g., datafile.ht): ";
                cin.getline(filename, sizeof(filename));

                cout << "Enter capacity: ";
                cin >> capacity;

                cout << "Enter snapshot interval: ";
                cin >> snapshotinterval;

                cout << "Enter max key length: ";
                cin >> maxKeyLength;

                cout << "Enter max payload length: ";
                cin >> maxPayloadLength;

                cin.ignore();
            }
            else
            {
                cerr << "Error: Invalid number of arguments!" << endl;
                cout << "Usage: program_name filename capacity snapshotinterval maxKeyLength maxPayloadLength" << endl;
                cout << "Or run without arguments for interactive mode" << endl;
                throw "Invalid arguments count";
            }
        }
        else
        {
            strcpy(filename, argv[1]);

            if (!checkValue(argv[2]) || !checkValue(argv[3]) || !checkValue(argv[4]) || !checkValue(argv[5]))
            {
                cerr << "Error: Enter correct numeric values for parameters!" << endl;
                throw "Invalid parameter values";
            }

            capacity = stoi(argv[2]);
            snapshotinterval = stoi(argv[3]);
            maxKeyLength = stoi(argv[4]);
            maxPayloadLength = stoi(argv[5]);
        }

        const size_t cSize = strlen(filename) + 1;
        wchar_t* wc = new wchar_t[cSize];
        mbstowcs(wc, filename, cSize);

        if (GetFileAttributes(wc) != INVALID_FILE_ATTRIBUTES)
        {
            cerr << "Error: File already exists!" << endl;
            throw "File exists";
        }

        cout << "Creating storage..." << endl;
        HTHandle* HT = Create(capacity, snapshotinterval, maxKeyLength, maxPayloadLength, wc);
        if (HT == NULL)
        {
            cerr << "Error: Storage creation failed!" << endl;
            throw "Storage creation failed";
        }

        cout << "\n=== STORAGE CREATED SUCCESSFULLY ===" << endl;
        cout << "filename = " << filename << endl;
        cout << "snapshotinterval = " << snapshotinterval << endl;
        cout << "capacity = " << capacity << endl;
        cout << "maxkeylength = " << maxKeyLength << endl;
        cout << "maxdatalength = " << maxPayloadLength << endl;

        Close(HT);
        delete[] wc;
    }
    catch (const char* err)
    {
        cerr << "Error: " << err << endl;
        cout << "Press Enter to exit...";
        cin.get();
        return -1;
    }
    catch (const exception& ex)
    {
        cerr << "Error: " << ex.what() << endl;
        cout << "Press Enter to exit...";
        cin.get();
        return -1;
    }

    cout << "\nStorage created successfully!" << endl;
    cout << "Press Enter to exit...";
    cin.get();

    return 0;
}

bool checkValue(const char* input)
{
    int i = 0;
    while (input[i] != '\0')
    {
        if (input[i] < '0' || input[i] > '9')
            return false;
        i++;
    }
    return true;
}