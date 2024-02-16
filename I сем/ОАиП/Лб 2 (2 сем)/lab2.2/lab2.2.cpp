#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>

void interleaveFiles(const char* inputFileA, const char* inputFileB, const char* inputFileC, const char* outputFile) {
    FILE* inputA = fopen(inputFileA, "r");
    FILE* inputB = fopen(inputFileB, "r");
    FILE* inputC = fopen(inputFileC, "r");

    if (!inputA || !inputB || !inputC) {
        fprintf(stderr, "Error opening input files.\n");
        return;
    }
    else {
        fprintf(stderr, "All is good with opening files)\n");
    }

    FILE* output = fopen(outputFile, "w");
    if (!output) {
        fprintf(stderr, "Error opening output file.\n");
        return;
    }
    else {
        fprintf(stderr, "All is good with output file)\n");
    }

    int valueA, valueB, valueC;
    while (fscanf(inputA, "%d", &valueA) == 1 && fscanf(inputB, "%d", &valueB) == 1 && fscanf(inputC, "%d", &valueC) == 1) {
        fprintf(output, "%d %d %d ", valueA, valueB, valueC);
    }

    fclose(inputA);
    fclose(inputB);
    fclose(inputC);
    fclose(output);
}

int main() {
    interleaveFiles("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/NameA.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/NameB.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/NameC.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/NameD.txt");
    return 0;
}
