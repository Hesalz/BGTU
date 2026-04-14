#define _CRT_SECURE_NO_WARNINGS
#include <windows.h>
#include <locale.h>
#include <stdio.h>
#include <stdlib.h>

#define BUFFER_SIZE 8192
#define INVALID_LINE -999

HANDLE g_hFile = INVALID_HANDLE_VALUE;
char* g_FileBuffer = NULL;
DWORD g_FileSize = 0;
char g_FilePath[MAX_PATH];

void PrintError(const char* msg)
{
    printf("Ошибка: %s (код %lu)\n", msg, GetLastError());
}

/* ===================== ВСПОМОГАТЕЛЬНЫЕ ===================== */

int CountLines()
{
    if (!g_FileBuffer) return 0;

    int count = 0;
    for (DWORD i = 0; i < g_FileSize; i++)
        if (g_FileBuffer[i] == '\n') count++;

    if (g_FileSize > 0 && g_FileBuffer[g_FileSize - 1] != '\n')
        count++;

    return count;
}

int GetLineBounds(int pos, DWORD* start, DWORD* end)
{
    int line = 1;
    DWORD s = 0;

    for (DWORD i = 0; i <= g_FileSize; i++)
    {
        if (i == g_FileSize || g_FileBuffer[i] == '\n')
        {
            if (line == pos)
            {
                *start = s;
                *end = i;
                return 1;
            }
            line++;
            s = i + 1;
        }
    }
    return 0;
}

/* ===================== ОСНОВНЫЕ ФУНКЦИИ ===================== */

BOOL MyOpenFile(LPSTR filePath)
{
    if (g_hFile != INVALID_HANDLE_VALUE)
        return FALSE;

    g_hFile = CreateFileA(
        filePath,
        GENERIC_READ | GENERIC_WRITE,
        0,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL
    );

    if (g_hFile == INVALID_HANDLE_VALUE)
        return FALSE;

    strcpy(g_FilePath, filePath);

    g_FileSize = GetFileSize(g_hFile, NULL);
    g_FileBuffer = (char*)malloc(g_FileSize + 1);
    if (!g_FileBuffer)
        return FALSE;

    DWORD read;
    SetFilePointer(g_hFile, 0, NULL, FILE_BEGIN);
    ReadFile(g_hFile, g_FileBuffer, g_FileSize, &read, NULL);
    g_FileBuffer[g_FileSize] = '\0';

    // Пропускаем BOM UTF-8, если он есть
    if (g_FileSize >= 3 &&
        (unsigned char)g_FileBuffer[0] == 0xEF &&
        (unsigned char)g_FileBuffer[1] == 0xBB &&
        (unsigned char)g_FileBuffer[2] == 0xBF)
    {
        memmove(g_FileBuffer, g_FileBuffer + 3, g_FileSize - 3);
        g_FileSize -= 3;
        g_FileBuffer[g_FileSize] = '\0';
    }

    return TRUE;
}

BOOL AddRow(HANDLE hFile, LPSTR row, INT pos)
{
    if (hFile == INVALID_HANDLE_VALUE || !row)
        return FALSE;

    int lines = CountLines();
    if (pos > lines + 1) return FALSE;

    DWORD insertPos = 0;

    if (pos == 0)
        insertPos = 0;
    else if (pos == -1)
        insertPos = g_FileSize;
    else
    {
        DWORD s, e;
        if (!GetLineBounds(pos, &s, &e))
            return FALSE;
        insertPos = s;
    }

    DWORD rowLen = (DWORD)strlen(row);
    DWORD newSize = g_FileSize + rowLen + 1;
    char* newBuf = (char*)malloc(newSize + 1);

    memcpy(newBuf, g_FileBuffer, insertPos);
    memcpy(newBuf + insertPos, row, rowLen);
    newBuf[insertPos + rowLen] = '\n';
    memcpy(newBuf + insertPos + rowLen + 1,
        g_FileBuffer + insertPos,
        g_FileSize - insertPos);

    g_FileSize = newSize;
    free(g_FileBuffer);
    g_FileBuffer = newBuf;
    g_FileBuffer[g_FileSize] = '\0';

    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
    SetEndOfFile(hFile);

    DWORD written;
    WriteFile(hFile, g_FileBuffer, g_FileSize, &written, NULL);

    return TRUE;
}

BOOL RemRow(HANDLE hFile, INT pos)
{
    if (hFile == INVALID_HANDLE_VALUE) return FALSE;

    int lines = CountLines();
    if (lines == 0) return FALSE;

    int realPos = pos;
    if (pos == 0) realPos = 1;
    if (pos == -1) realPos = lines;
    if (realPos < 1 || realPos > lines) return FALSE;

    DWORD s, e;
    if (!GetLineBounds(realPos, &s, &e)) return FALSE;

    DWORD len = (e < g_FileSize && g_FileBuffer[e] == '\n') ? e - s + 1 : e - s;

    memmove(g_FileBuffer + s, g_FileBuffer + s + len, g_FileSize - (s + len));
    g_FileSize -= len;
    g_FileBuffer[g_FileSize] = '\0';

    SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
    SetEndOfFile(hFile);

    DWORD written;
    WriteFile(hFile, g_FileBuffer, g_FileSize, &written, NULL);

    return TRUE;
}

BOOL PrintRow(HANDLE hFile, INT pos)
{
    if (hFile == INVALID_HANDLE_VALUE) return FALSE;

    int lines = CountLines();
    int realPos = pos;
    if (pos == 0) realPos = 1;
    if (pos == -1) realPos = lines;
    if (realPos < 1 || realPos > lines) return FALSE;

    DWORD s, e;
    if (!GetLineBounds(realPos, &s, &e)) return FALSE;

    for (DWORD i = s; i < e; i++)
        putchar(g_FileBuffer[i]);
    putchar('\n');

    return TRUE;
}

BOOL PrintRows(HANDLE hFile)
{
    if (hFile == INVALID_HANDLE_VALUE) return FALSE;

    for (DWORD i = 0; i < g_FileSize; i++)
        putchar(g_FileBuffer[i]);
    if (g_FileSize > 0 && g_FileBuffer[g_FileSize - 1] != '\n')
        putchar('\n');

    return TRUE;
}

BOOL CloseFile(HANDLE hFile)
{
    if (hFile == INVALID_HANDLE_VALUE) return FALSE;

    CloseHandle(hFile);
    g_hFile = INVALID_HANDLE_VALUE;

    free(g_FileBuffer);
    g_FileBuffer = NULL;
    g_FileSize = 0;

    return TRUE;
}

/* ===================== МЕНЮ ===================== */

void Menu()
{
    int cmd;
    char buf[BUFFER_SIZE];
    int pos;

    while (1)
    {
        printf(
            "\nВыберите выполняемую операцию:\n"
            "1.Открыть файл.\n"
            "2.Вставить строку.\n"
            "3.Удалить строку.\n"
            "4.Вывести строку.\n"
            "5.Вывести файл.\n"
            "6.Закрыть файл.\n"
            "0.Выход.\n> ");

        scanf("%d", &cmd);
        getchar();

        switch (cmd)
        {
        case 1:
            printf("Путь к файлу: ");
            fgets(buf, BUFFER_SIZE, stdin);
            buf[strcspn(buf, "\n")] = 0;
            if (!MyOpenFile(buf)) PrintError("MyOpenFile");
            break;

        case 2:
            printf("Строка: ");
            fgets(buf, BUFFER_SIZE, stdin);
            buf[strcspn(buf, "\n")] = 0;
            printf("Позиция: ");
            scanf("%d", &pos); getchar();
            if (!AddRow(g_hFile, buf, pos)) PrintError("AddRow");
            break;

        case 3:
            printf("Позиция: ");
            scanf("%d", &pos); getchar();
            if (!RemRow(g_hFile, pos)) PrintError("RemRow");
            break;

        case 4:
            printf("Позиция: ");
            scanf("%d", &pos); getchar();
            if (!PrintRow(g_hFile, pos)) PrintError("PrintRow");
            break;

        case 5:
            if (!PrintRows(g_hFile)) PrintError("PrintRows");
            break;

        case 6:
            if (!CloseFile(g_hFile)) PrintError("CloseFile");
            break;

        case 0:
            CloseFile(g_hFile);
            return;
        }
    }
}

int main()
{
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    setlocale(LC_ALL, "Russian");
    Menu();
    return 0;
}
