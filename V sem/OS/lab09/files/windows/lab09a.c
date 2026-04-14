#define UNICODE
#define _UNICODE

#include <windows.h>
#include <stdio.h>

#define BUFFER_SIZE 4096

void PrintError(LPCWSTR msg)
{
    DWORD err = GetLastError();
    WCHAR buf[256];
    swprintf(buf, 256, L"%s. Код ошибки: %lu\n", msg, err);
    WriteConsoleW(GetStdHandle(STD_ERROR_HANDLE), buf, wcslen(buf), NULL, NULL);
}

void PrintFileTime(LPCWSTR title, FILETIME ft)
{
    SYSTEMTIME st;
    FileTimeToSystemTime(&ft, &st);

    WCHAR buf[128];
    DWORD written;
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), title, wcslen(title), &written, NULL);
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), L": ", 2, &written, NULL);

    swprintf(buf, 128, L"%02d.%02d.%04d %02d:%02d:%02d\n",
             st.wDay, st.wMonth, st.wYear,
             st.wHour, st.wMinute, st.wSecond);
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), buf, wcslen(buf), &written, NULL);
}

BOOL IsTextFile(HANDLE hFile)
{
    BYTE buffer[BUFFER_SIZE];
    DWORD readed;
    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
    if (!ReadFile(hFile, buffer, BUFFER_SIZE, &readed, NULL))
        return FALSE;
    for (DWORD i = 0; i < readed; i++)
    {
        BYTE b = buffer[i];
        if (b < 9 || (b > 13 && b < 32))
            return FALSE;
    }
    return TRUE;
}

void DetectBinaryType(HANDLE hFile)
{
    BYTE sig[4];
    DWORD readed;
    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
    if (!ReadFile(hFile, sig, 4, &readed, NULL) || readed < 2)
        return;

    if (sig[0] == 'M' && sig[1] == 'Z')
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Бинарный файл: PE (EXE/DLL)\n",
                      wcslen(L"Бинарный файл: PE (EXE/DLL)\n"), NULL, NULL);
    else if (sig[0] == 'P' && sig[1] == 'K')
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Бинарный файл: Архив (ZIP)\n",
                      wcslen(L"Бинарный файл: Архив (ZIP)\n"), NULL, NULL);
    else
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Бинарный файл: Неизвестный тип\n",
                      wcslen(L"Бинарный файл: Неизвестный тип\n"), NULL, NULL);
}

void PrintInfo(LPWSTR FileName)
{
    HANDLE hFile = CreateFileW(FileName, GENERIC_READ, FILE_SHARE_READ, NULL,
                               OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        PrintError(L"Не удалось открыть файл");
        return;
    }

    DWORD written;
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), L"Имя файла: ", wcslen(L"Имя файла: "), &written, NULL);
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), FileName, wcslen(FileName), &written, NULL);
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), L"\n", 1, &written, NULL);

    LARGE_INTEGER size;
    GetFileSizeEx(hFile, &size);

    WCHAR buf[256];
    swprintf(buf, 256, L"Размер файла: %I64u Б (%.2f КиБ, %.2f МиБ)\n",
             size.QuadPart, (double)size.QuadPart / 1024.0, (double)size.QuadPart / (1024.0*1024.0));
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), buf, wcslen(buf), &written, NULL);

    FILETIME ftCreate, ftAccess, ftWrite;
    GetFileTime(hFile, &ftCreate, &ftAccess, &ftWrite);

    PrintFileTime(L"Создан", ftCreate);
    PrintFileTime(L"Последний доступ", ftAccess);
    PrintFileTime(L"Последнее изменение", ftWrite);

    if (IsTextFile(hFile))
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Тип файла: Текстовый\n",
                      wcslen(L"Тип файла: Текстовый\n"), &written, NULL);
    else
    {
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Тип файла: Бинарный\n",
                      wcslen(L"Тип файла: Бинарный\n"), &written, NULL);
        DetectBinaryType(hFile);
    }

    CloseHandle(hFile);
}

void PrintText(LPWSTR FileName)
{
    HANDLE hFile = CreateFileW(FileName, GENERIC_READ, FILE_SHARE_READ, NULL,
                               OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        PrintError(L"Не удалось открыть файл");
        return;
    }

    if (!IsTextFile(hFile))
    {
        DWORD written;
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"Файл не является текстовым\n",
                      wcslen(L"Файл не является текстовым\n"), &written, NULL);
        CloseHandle(hFile);
        return;
    }

    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);

    BYTE buffer[BUFFER_SIZE];
    WCHAR out[BUFFER_SIZE];
    DWORD readed;
    DWORD written;

    while (ReadFile(hFile, buffer, BUFFER_SIZE, &readed, NULL) && readed > 0)
    {
        int chars = MultiByteToWideChar(CP_ACP, 0, (LPCSTR)buffer, readed, out, BUFFER_SIZE);
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE), out, chars, &written, NULL);
    }

    CloseHandle(hFile);
}

int wmain(int argc, wchar_t* argv[])
{
    if (argc != 2)
    {
        DWORD written;
        WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                      L"lab09a <путь_к_файлу>\n",
                      wcslen(L"lab09a <путь_к_файлу>\n"), &written, NULL);
        return 1;
    }

    PrintInfo(argv[1]);

    DWORD written;
    WriteConsoleW(GetStdHandle(STD_OUTPUT_HANDLE),
                  L"\n----- Содержимое файла -----\n",
                  wcslen(L"\n----- Содержимое файла -----\n"), &written, NULL);

    PrintText(argv[1]);

    return 0;
}
