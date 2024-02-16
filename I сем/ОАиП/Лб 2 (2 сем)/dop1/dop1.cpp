#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <iostream>

//Вариант 1
void multiplyMatrices(const char* fileNameA, const char* fileNameB, const char* fileNameC) {
    FILE* fileA = fopen(fileNameA, "r");
    FILE* fileB = fopen(fileNameB, "r");

    if (!fileA || !fileB) {
        fprintf(stderr, "Error opening input files.\n");
        return;
    }

    int colsA, colsB;
    fscanf(fileA, "%d", &colsA);
    fscanf(fileB, "%d", &colsB);

    if (colsA <= 0 || colsB <= 0 || colsA != colsB) {
        fprintf(stderr, "Invalid matrix dimensions for multiplication.\n");
        fclose(fileA);
        fclose(fileB);
        return;
    }

    double** M1 = (double**)malloc(colsA * sizeof(double*));
    double** M2 = (double**)malloc(colsB * sizeof(double*));

    for (int i = 0; i < colsA; ++i) {
        M1[i] = (double*)malloc(colsA * sizeof(double));
    }

    for (int i = 0; i < colsB; ++i) {
        M2[i] = (double*)malloc(colsB * sizeof(double));
    }

    for (int i = 0; i < colsA; ++i) {
        for (int j = 0; j < colsA; ++j) {
            fscanf(fileA, "%lf", &M1[i][j]);
        }
    }

    for (int i = 0; i < colsB; ++i) {
        for (int j = 0; j < colsB; ++j) {
            fscanf(fileB, "%lf", &M2[i][j]);
        }
    }

    fclose(fileA);
    fclose(fileB);

    if (colsA != colsB) {
        fprintf(stderr, "Matrices cannot be multiplied.\n");

        for (int i = 0; i < colsA; ++i) {
            free(M1[i]);
        }

        for (int i = 0; i < colsB; ++i) {
            free(M2[i]);
        }

        free(M1);
        free(M2);

        return;
    }

    double** result = (double**)malloc(colsA * sizeof(double*));
    for (int i = 0; i < colsA; ++i) {
        result[i] = (double*)malloc(colsA * sizeof(double));
    }

    for (int i = 0; i < colsA; ++i) {
        for (int j = 0; j < colsA; ++j) {
            result[i][j] = 0;
            for (int k = 0; k < colsA; ++k) {
                result[i][j] += M1[i][k] * M2[k][j];
            }
        }
    }

    FILE* fileC = fopen(fileNameC, "w");
    if (!fileC) {
        fprintf(stderr, "Error opening output file.\n");

        for (int i = 0; i < colsA; ++i) {
            free(M1[i]);
            free(result[i]);
        }

        for (int i = 0; i < colsB; ++i) {
            free(M2[i]);
        }

        free(M1);
        free(M2);
        free(result);

        return;
    }

    fprintf(fileC, "%d\n", colsA);

    for (int i = 0; i < colsA; ++i) {
        for (int j = 0; j < colsA; ++j) {
            fprintf(fileC, "%.2lf ", result[i][j]);
        }
        fprintf(fileC, "\n");
    }

    fclose(fileC);

    for (int i = 0; i < colsA; ++i) {
        free(M1[i]);
        free(result[i]);
    }

    for (int i = 0; i < colsB; ++i) {
        free(M2[i]);
    }

    free(M1);
    free(M2);
    free(result);
}

int main() {
    multiplyMatrices("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/первый/fA.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/первый/fB.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/первый/fC.txt");

    return 0;
}
