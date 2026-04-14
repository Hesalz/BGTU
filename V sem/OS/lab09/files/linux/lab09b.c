#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/stat.h>
#include <locale.h>

#define BUFFER_SIZE 8192
#define INVALID_LINE -999

int g_fd = -1;
char *g_FileBuffer = NULL;
size_t g_FileSize = 0;
char g_FilePath[1024];

void PrintError(const char *msg)
{
    perror(msg);
}

/* ===================== ВСПОМОГАТЕЛЬНЫЕ ===================== */

int CountLines()
{
    if (!g_FileBuffer) return 0;

    int count = 0;
    for (size_t i = 0; i < g_FileSize; i++)
        if (g_FileBuffer[i] == '\n') count++;

    if (g_FileSize > 0 && g_FileBuffer[g_FileSize - 1] != '\n')
        count++;

    return count;
}

int GetLineBounds(int pos, size_t *start, size_t *end)
{
    int line = 1;
    size_t s = 0;

    for (size_t i = 0; i <= g_FileSize; i++)
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

int MyOpenFile(char *filePath)
{
    if (g_fd != -1) return 0;

    g_fd = open(filePath, O_RDWR);
    if (g_fd < 0) return 0;

    strcpy(g_FilePath, filePath);

    struct stat st;
    fstat(g_fd, &st);
    g_FileSize = st.st_size;

    g_FileBuffer = malloc(g_FileSize + 1);
    if (!g_FileBuffer) return 0;

    lseek(g_fd, 0, SEEK_SET);
    read(g_fd, g_FileBuffer, g_FileSize);
    g_FileBuffer[g_FileSize] = '\0';

    /* UTF-8 BOM */
    if (g_FileSize >= 3 &&
        (unsigned char)g_FileBuffer[0] == 0xEF &&
        (unsigned char)g_FileBuffer[1] == 0xBB &&
        (unsigned char)g_FileBuffer[2] == 0xBF)
    {
        memmove(g_FileBuffer, g_FileBuffer + 3, g_FileSize - 3);
        g_FileSize -= 3;
        g_FileBuffer[g_FileSize] = '\0';
    }

    return 1;
}

int AddRow(int pos, char *row)
{
    if (g_fd < 0) return 0;

    int lines = CountLines();
    if (pos > lines + 1) return 0;

    size_t insertPos = 0;

    if (pos == 0) insertPos = 0;
    else if (pos == -1) insertPos = g_FileSize;
    else
    {
        size_t s, e;
        if (!GetLineBounds(pos, &s, &e)) return 0;
        insertPos = s;
    }

    size_t rowLen = strlen(row);
    size_t newSize = g_FileSize + rowLen + 1;
    char *newBuf = malloc(newSize + 1);

    memcpy(newBuf, g_FileBuffer, insertPos);
    memcpy(newBuf + insertPos, row, rowLen);
    newBuf[insertPos + rowLen] = '\n';
    memcpy(newBuf + insertPos + rowLen + 1,
           g_FileBuffer + insertPos,
           g_FileSize - insertPos);

    free(g_FileBuffer);
    g_FileBuffer = newBuf;
    g_FileSize = newSize;
    g_FileBuffer[g_FileSize] = '\0';

    lseek(g_fd, 0, SEEK_SET);
    ftruncate(g_fd, 0);
    write(g_fd, g_FileBuffer, g_FileSize);

    return 1;
}

int RemRow(int pos)
{
    if (g_fd < 0) return 0;

    int lines = CountLines();
    if (lines == 0) return 0;

    int realPos = pos == 0 ? 1 : (pos == -1 ? lines : pos);
    if (realPos < 1 || realPos > lines) return 0;

    size_t s, e;
    if (!GetLineBounds(realPos, &s, &e)) return 0;

    size_t len = (e < g_FileSize && g_FileBuffer[e] == '\n') ? e - s + 1 : e - s;
    memmove(g_FileBuffer + s, g_FileBuffer + s + len, g_FileSize - (s + len));
    g_FileSize -= len;
    g_FileBuffer[g_FileSize] = '\0';

    lseek(g_fd, 0, SEEK_SET);
    ftruncate(g_fd, 0);
    write(g_fd, g_FileBuffer, g_FileSize);

    return 1;
}

int PrintRow(int pos)
{
    if (g_fd < 0) return 0;

    int lines = CountLines();
    int realPos = pos == 0 ? 1 : (pos == -1 ? lines : pos);
    if (realPos < 1 || realPos > lines) return 0;

    size_t s, e;
    if (!GetLineBounds(realPos, &s, &e)) return 0;

    fwrite(g_FileBuffer + s, 1, e - s, stdout);
    putchar('\n');
    return 1;
}

int PrintRows()
{
    if (g_fd < 0) return 0;
    fwrite(g_FileBuffer, 1, g_FileSize, stdout);
    if (g_FileSize && g_FileBuffer[g_FileSize - 1] != '\n')
        putchar('\n');
    return 1;
}

void CloseFile()
{
    if (g_fd >= 0)
    {
        close(g_fd);
        g_fd = -1;
    }
    free(g_FileBuffer);
    g_FileBuffer = NULL;
    g_FileSize = 0;
}

/* ===================== МЕНЮ ===================== */

void Menu()
{
    int cmd, pos;
    char buf[BUFFER_SIZE];

    while (1)
    {
        printf(
            "\n1.Открыть файл\n"
            "2.Вставить строку\n"
            "3.Удалить строку\n"
            "4.Вывести строку\n"
            "5.Вывести файл\n"
            "6.Закрыть файл\n"
            "0.Выход\n> ");

        scanf("%d", &cmd);
        getchar();

        switch (cmd)
        {
        case 1:
            printf("Путь: ");
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
            if (!AddRow(pos, buf)) PrintError("AddRow");
            break;

        case 3:
            printf("Позиция: ");
            scanf("%d", &pos); getchar();
            if (!RemRow(pos)) PrintError("RemRow");
            break;

        case 4:
            printf("Позиция: ");
            scanf("%d", &pos); getchar();
            if (!PrintRow(pos)) PrintError("PrintRow");
            break;

        case 5:
            if (!PrintRows()) PrintError("PrintRows");
            break;

        case 6:
            CloseFile();
            break;

        case 0:
            CloseFile();
            return;
        }
    }
}

int main()
{
    setlocale(LC_ALL, "");
    Menu();
    return 0;
}
