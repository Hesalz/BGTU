#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdlib.h>
#include <iostream>

void removeDuplicates(const char* inputFileName, const char* outputFileName, int n) {
    FILE* inputFile = fopen(inputFileName, "r");
    if (inputFile == NULL) {
        printf("Ошибка открытия файла\n");
        return;
    }
    else printf("Ваш говнокод заработал( файл открыт и прочитан, как книжка )\n");

    int num;
    int* numbers = NULL;
    int count = 0;

    while (fscanf(inputFile, "%d", &num) == 1) {
        numbers = (int*)realloc(numbers, (count + 1) * sizeof(int));
        numbers[count++] = num;
    }

    fclose(inputFile);
    int* uniqueNumbers = NULL;
    int uniqueCount = 0;

    for (int i = 0; i < count; ++i) {
        int isDuplicate = 0;

        for (int j = 0; j < uniqueCount; ++j) {
            if (numbers[i] == uniqueNumbers[j]) {
                isDuplicate = 1;
                break;
            }
        }

        if (!isDuplicate) {
            uniqueNumbers = (int*)realloc(uniqueNumbers, (uniqueCount + 1) * sizeof(int));
            uniqueNumbers[uniqueCount++] = numbers[i];
        }
    }

    free(numbers);

    FILE* outputFile = fopen(outputFileName, "w");
    if (outputFile == NULL) {
        printf("Ошибка создания файла\n");
        return;
    }
    else printf("Ваш говнокод заработал( файл создан )\n");

    for (int i = 0; i < uniqueCount; ++i) {
        fprintf(outputFile, "%d ", uniqueNumbers[i]);
    }

    fclose(outputFile);
    free(uniqueNumbers);
}

int main() {
    setlocale(LC_ALL, "rus");
    removeDuplicates("D:/БГТУ/Работы/лб 2/input2.txt", "D:/БГТУ/Работы/лб 2/output2.txt", 2); // Сюда свой путь напишешь
    return 0;
}
