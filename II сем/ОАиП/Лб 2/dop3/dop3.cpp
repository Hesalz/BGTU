#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>

//Вариант 9
void removeDuplicates(const char* inputFile, const char* outputFile) {
    FILE* fileA = fopen(inputFile, "r");
    FILE* fileB = fopen(outputFile, "w");

    if (!fileA || !fileB) {
        fprintf(stderr, "Error opening input/output files.\n");
        return;
    }

    int num;
    bool encountered[100000] = { false };

    while (fscanf(fileA, "%d", &num) == 1) {
        if (!encountered[num]) {
            fprintf(fileB, "%d ", num);
            encountered[num] = true;
        }
    }

    fclose(fileA);
    fclose(fileB);
}

int main() {
    removeDuplicates("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/третий/fileA.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/третий/fileB.txt");

    return 0;
}
