#include <iostream>
using namespace std;

//Вариант 8
void fillArray(int** array, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            array[i][j] = (rand() % 21) - 10;
        }
    }
}

void printArray(int** array, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            cout << array[i][j] << " ";
        }
        cout << endl;
    }
}

void countElements(int** array, int rows, int cols) {
    int negativeCount = 0, positiveCount = 0, zeroCount = 0;

    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (array[i][j] < 0) {
                negativeCount++;
            }
            else if (array[i][j] > 0) {
                positiveCount++;
            }
            else {
                zeroCount++;
            }
        }
    }

    cout << "Отрицательных элементов: " << negativeCount << endl;
    cout << "Положительных элементов: " << positiveCount << endl;
    cout << "Нулевых элементов: " << zeroCount << endl;
}

int countWords(const char* text) {
    int wordCount = 0;

    while (*text != '\0') {
        while (*text != '\0' && (*text == ' ' || *text == ',' || *text == '.' || *text == '!' || *text == '?' || *text == ';' || *text == ':' || *text == '"' || *text == '/' || *text == '0' || *text == '1' || *text == '2') || *text == '3' || *text == '4' || *text == '5' || *text == '6' || *text == '7' || *text == '8' || *text == '9') {
            text++;
        }
        if (*text != '\0') {
            wordCount++;

            while (*text != '\0' && !(*text == ' ' || *text == ',' || *text == '.' || *text == '!' || *text == '?' || *text == ';' || *text == ':' || *text == '"' || *text == '/' || *text == '0' || *text == '1' || *text == '2') || *text == '3' || *text == '4' || *text == '5' || *text == '6' || *text == '7' || *text == '8' || *text == '9') {
                text++;
            }
        }
    }

    return wordCount;
}

int main() {
    setlocale(LC_ALL, "rus");
    srand(time(0));

    int choice;
    cout << "Выберите задание (1 или 2): ";
    cin >> choice;

    switch (choice) {
    case 1: {
        int rows, cols;
        cout << "Введите количество строк: ";
        cin >> rows;
        cout << "Введите количество столбцов: ";
        cin >> cols;

        int** array = new int* [rows];
        for (int i = 0; i < rows; ++i) {
            array[i] = new int[cols];
        }

        fillArray(array, rows, cols);

        cout << "Исходный массив:" << endl;
        printArray(array, rows, cols);

        countElements(array, rows, cols);

        for (int i = 0; i < rows; ++i) {
            delete[] array[i];
        }
        delete[] array;

        break;
    }

    case 2: {
        char text[1000];

        cout << "Введите текст: ";
        cin.ignore();
        cin.getline(text, 1000);

        int wordCount = countWords(text);
        cout << "Количество слов в тексте: " << wordCount << endl;

        break;
    }

    default:
        cout << "Неверный выбор." << endl;
    }

    return 0;
}
