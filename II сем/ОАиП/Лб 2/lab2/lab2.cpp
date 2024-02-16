#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <iostream>

void transposeMatrix(const char* inputFile, const char* outputFile) {
    FILE* input = fopen(inputFile, "r");
    if (input) {
        fprintf(stderr, "All is good with input file).\n");
    }
    
    else {
        fprintf(stderr, "Error opening input file.\n");
        return;
    }

    int numCols;
    fscanf(input, "%d", &numCols);

    double** matrix = (double**)malloc(numCols * sizeof(double*));
    for (int i = 0; i < numCols; ++i) {
        matrix[i] = (double*)malloc(numCols * sizeof(double));
    }

    for (int i = 0; i < numCols; ++i) {
        for (int j = 0; j < numCols; ++j) {
            fscanf(input, "%lf", &matrix[i][j]);
        }
    }

    fclose(input);

    double** transposedMatrix = (double**)malloc(numCols * sizeof(double*));
    for (int i = 0; i < numCols; ++i) {
        transposedMatrix[i] = (double*)malloc(numCols * sizeof(double));
        for (int j = 0; j < numCols; ++j) {
            transposedMatrix[i][j] = matrix[j][i];
        }
    }

    FILE* output = fopen(outputFile, "w");
    if (output) {
        fprintf(stderr, "All is good with output file).\n");
    }
    else {
        fprintf(stderr, "Error opening output file.\n");
        return;
    }

    for (int i = 0; i < numCols; ++i) {
        for (int j = 0; j < numCols; ++j) {
            fprintf(output, "%.2lf ", transposedMatrix[i][j]);
            if ((j + 1) % 4 == 0) {
                fprintf(output, "\n");
            }
        }
    }

    fclose(output);

    for (int i = 0; i < numCols; ++i) {
        free(matrix[i]);
        free(transposedMatrix[i]);
    }
    free(matrix);
    free(transposedMatrix);
}

int main() {
    transposeMatrix("D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/input_matrix.txt", "D:/БГТУ/Работы/ОАиП/Лб 2 (2 сем)/output_transposed_matrix.txt");
    return 0;
}
