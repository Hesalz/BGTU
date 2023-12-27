#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <iostream>

void filterNumbers(const char* inputFile, const char* outputFile, int threshold) {
    FILE* fileF = fopen(inputFile, "r");
    FILE* fileG = fopen(outputFile, "w");

    if (!fileF || !fileG) {
        fprintf(stderr, "Error opening input/output files.\n");
        return;
    }

    int num;

    printf("Введите число: ");
    scanf("%d", &threshold);

    while (fscanf(fileF, "%d", &num) == 1) {
        if (num > threshold) {
            fprintf(fileG, "%d ", num);
        }
    }

    fclose(fileF);
    fclose(fileG);
}

int main() {
    setlocale(LC_ALL, "rus");
    int threshold = 0;
    filterNumbers("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/второй два/f.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/второй два/g.txt", threshold);

    return 0;
}
