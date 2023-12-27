#include <iostream>
using namespace std;

int sumBelowDiagonal(int** array, int n) {
    int sum = 0;
    for (int i = 1; i < n; ++i) {
        for (int j = 0; j < i; ++j) {
            sum += array[i][j];
        }
    }
    return sum;
}

int countOccurrences(char* text, char ch) {
    int count = 0;
    int len = strlen(text);
    for (int i = 0; i < len; ++i) {
        if (text[i] == ch) {
            count++;
        }
    }
    return count;
}

void fillRandom(int** array, int n) {
    srand(time(NULL));
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            array[i][j] = rand() % 10;
        }
    }
}

void printMatrix(int** array, int n) {
    cout << "Исходная матрица:" << endl;
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            cout << array[i][j] << " ";
        }
        cout << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    int choice, n;
    int** arrayA = nullptr;
    cout << "Выберите задачу (1 или 2): ";
    cin >> choice;

    switch (choice) {
    case 1:
        cout << "Введите размерность массива A (n x n): ";
        cin >> n;

        arrayA = new int* [n];
        for (int i = 0; i < n; ++i) {
            arrayA[i] = new int[n];
        }

        fillRandom(arrayA, n);
        printMatrix(arrayA, n);
        cout << "Сумма элементов ниже главной диагонали: " << sumBelowDiagonal(arrayA, n) << endl;
       
        for (int i = 0; i < n; ++i) {
            delete[] arrayA[i];
        }
        delete[] arrayA;

        break;
    case 2: 
        char text[100];
        cout << "Введите текст: ";
        cin.ignore();
        cin.getline(text, sizeof(text));

        char ch;
        cout << "Введите символ для поиска: ";
        cin >> ch;

        cout << "Число вхождений символа в текст: " << countOccurrences(text, ch) << endl;
        break;

    default:
        cout << "Неверный выбор." << endl;
        break;
    }


    return 0;
}
