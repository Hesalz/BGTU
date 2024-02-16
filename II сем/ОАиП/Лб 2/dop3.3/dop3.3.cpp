#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <iostream>

// 31
// 34
// 17
// 19
void copyLines(const char* inputFile, const char* outputFile, int minLength) {
    FILE* fileF1 = fopen(inputFile, "r");
    FILE* fileF2 = fopen(outputFile, "w");

    if (!fileF1 || !fileF2) {
        fprintf(stderr, "Error opening input/output files.\n");
        return;
    }

    char line[1000];

    while (fgets(line, sizeof(line), fileF1) != NULL) {
        int len = 0;
        int totalLength = 0;

        while (line[len] != '\0' && line[len] != '\n') {
            len++;
            totalLength++;
        }

        if (totalLength > minLength) {
            fprintf(fileF2, "%s", line);
        }
    }

    fclose(fileF1);
    fclose(fileF2);
}

int main() {
    setlocale(LC_ALL, "rus");
    int minLength;
    printf("Введите минимальное количество символов: ");
    scanf("%d", &minLength);

    copyLines("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/третий два/F1.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/третий два/F2.txt", minLength);

    return 0;
}

