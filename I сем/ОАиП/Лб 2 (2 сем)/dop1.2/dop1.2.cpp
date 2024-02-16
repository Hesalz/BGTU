#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <iostream>

void filterNumbers(const char* inputFileName, const char* outputFileName, int k) {
    FILE* inputFile = fopen(inputFileName, "r");
    if (!inputFile) {
        fprintf(stderr, "Error opening input file.\n");
        return;
    }

    FILE* outputFile = fopen(outputFileName, "w");
    if (!outputFile) {
        fprintf(stderr, "Error opening output file.\n");
        fclose(inputFile);
        return;
    }

    int number;

    while (fscanf(inputFile, "%d", &number) == 1) {
        if (number % k == 0) {
            fprintf(outputFile, "%d\n", number);
        }
    }

    fclose(inputFile);
    fclose(outputFile);
}

int main() {
    filterNumbers("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/первый два/f.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/первый два/g.txt", 2);

    return 0;
}
