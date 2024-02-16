#include <iostream>
using namespace std;

//Вариант 5
void setRandomMatrix(int** matrix, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            matrix[i][j] = rand() % 10;
        }
    }
}

void printMatrix(int** matrix, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            cout << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

int findRowWithMinMaxElement(int** matrix, int rows, int cols) {
    int* minElements = new int[rows];
    for (int i = 0; i < rows; ++i) {
        minElements[i] = INT_MAX;
        for (int j = 0; j < cols; ++j) {
            if (matrix[i][j] < minElements[i]) {
                minElements[i] = matrix[i][j];
            }
        }
    }
    int maxOfMinElements = INT_MIN;
    int rowWithMaxMinElement = -1;

    for (int i = 0; i < rows; ++i) {
        if (minElements[i] > maxOfMinElements) {
            maxOfMinElements = minElements[i];
            rowWithMaxMinElement = i;
        }
    }

    delete[] minElements;

    return rowWithMaxMinElement;
}

int countSymbolOccurrences(char* text, char searchSymbol) {
    int count = 0;
    for (int i = 0; text[i] != '\0'; ++i) {
        if (text[i] == searchSymbol) {
            count++;
        }
    }
    return count;
}

int processText(char** textArray, int numLines, char searchSymbol) {
    int totalOccurrences = 0;

    for (int i = 0; i < numLines; ++i) {
        totalOccurrences += countSymbolOccurrences(textArray[i], searchSymbol);
    }

    return totalOccurrences;
}

int main() {
    setlocale(LC_ALL, "rus");
    srand(time(0));
    int rows = 0, cols = 0;
    int** matrix = nullptr;
    int choice;
    cout << "Выберите задачу (1 или 2): ";
    cin >> choice;

    switch (choice) {

    case 1: {
        cout << "Введите количество строк: ";
        cin >> rows;
        cout << "Введите количество столбцов: ";
        cin >> cols;

        matrix = new int* [rows];
        for (int i = 0; i < rows; ++i) {
            matrix[i] = new int[cols];
        }

        setRandomMatrix(matrix, rows, cols);
        cout << "Сгенерированная матрица:" << endl;
        printMatrix(matrix, rows, cols);
        int rowWithMaxMinElement = findRowWithMinMaxElement(matrix, rows, cols);
        cout << "Номер строки с максимальным элементом среди минимальных элементов каждой строки: " << rowWithMaxMinElement + 1 << endl;
        break;
    }
    case 2: {
        int numLines;
        cout << "Введите количество строк текста: ";
        cin >> numLines;

        char** textArray = new char* [numLines];
        cout << "Введите строки текста:" << endl;

        for (int i = 0; i < numLines; ++i) {
            textArray[i] = new char[100];
            cin >> textArray[i];
        }

        char searchSymbol;
        cout << "Введите символ поиска: ";
        cin >> searchSymbol;

        int totalOccurrences = processText(textArray, numLines, searchSymbol);
        cout << "Суммарное количество вхождений символа: " << totalOccurrences << endl;

        for (int i = 0; i < numLines; ++i) {
            delete[] textArray[i];
        }
        delete[] textArray;
        break;
    }
    default:
        cout << "Некорректный выбор задачи." << endl;
    }

    for (int i = 0; i < rows; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;

    return 0;
}
