#include <iostream>
using namespace std;

// Вариант 4
void subtractRows(int** arrayB, int** array, int rows, int cols) {
    for (int j = 0; j < cols; j++) {
        arrayB[0][j] = array[0][j] - array[rows - 1][j];
    }
    for (int i = 1; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            arrayB[i][j] = array[i][j] - array[i - 1][j];
        }
    }
}

void printArray(int** arrayB, int rows, int cols) {
    cout << "Полученный массив:" << endl;
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            cout << arrayB[i][j] << " ";
        }
        cout << endl;
    }
}

int findSumOfNumbersInString(const char* str) {
    int sum = 0;
    for (const char* ptr = str; *ptr != '\0'; ptr++) {
        if (isdigit(*ptr)) {
            sum += *ptr - '0';
        }
    }

    return sum;
}

void fillmatrix(int** array, int rows, int cols, int n) {
    srand(time(NULL));
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            array[i][j] = rand() % 10;
            cout << array[i][j] << " ";
        }
        cout << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    int choice, n = 0;
    int** array = nullptr;
    int** arrayB = nullptr;
    cout << "Выберите задачу (1 или 2):";
    cin >> choice;
    cin.ignore();

    switch (choice) {
    case 1:
        cout << "Введите размер массива (n x n): ";
        cin >> n;
        cin.ignore();

        array = new int* [n];
        for (int i = 0; i < n; i++) {
            array[i] = new int[n];
        }

        arrayB = new int* [n];
        for (int i = 0; i < n; i++) {
            arrayB[i] = new int[n];
        }

        fillmatrix(array, n, n, n);
        subtractRows(arrayB, array, n, n);
        printArray(arrayB, n, n);

        break;
    case 2: {
        cout << "Введите строку: ";
        char inputString[100];
        cin.getline(inputString, 100);
        int sum = findSumOfNumbersInString(inputString);
        cout << "Сумма цифр в строке: " << sum << endl;
        break;
    }
    default:
        cout << "Неверный ввод." << endl;
    }

    for (int i = 0; i < n; i++) {
        delete[] array[i];
    }
    delete[] array;

    for (int i = 0; i < n; i++) {
        delete[] arrayB[i];
    }
    delete[] arrayB;

    return 0;
}
