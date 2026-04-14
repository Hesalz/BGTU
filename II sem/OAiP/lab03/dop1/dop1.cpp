#include <iostream>
#include <fstream>
using namespace std;

int main() { // Вариант 1
    setlocale(LC_ALL, "rus");
    int i = 0, k = 0;
    char buff[50];
    ofstream fout("output.txt");
    ifstream fin("inputdop1.txt");
    if (!fin.is_open())
        cout << "Файл не может быть открыт!\n";
    else
    {
        while (!fin.eof()) {
            k++;
            fin.getline(buff, 50);
            if (k % 2 == 0) {
                fout << buff << "\n";
            }
            i++;
        }
        fin.close();
        fout.close();

        ifstream inputFile("inputdop1.txt");
        if (inputFile.is_open()) {
            int inputSize = 0;
            char ch;
            while (inputFile.get(ch)) {
                ++inputSize;
            }
            cout << "Размер файла inputdop1.txt: " << inputSize << " байт" << endl;
            inputFile.close();
        }
        else {
            cout << "Ошибка при открытии файла inputdop1.txt для подсчета размера." << endl;
        }

        ifstream outputFile("output.txt");
        if (outputFile.is_open()) {
            int outputSize = 0;
            char ch;
            while (outputFile.get(ch)) {
                ++outputSize;
            }
            cout << "Размер файла output.txt: " << outputSize << " байт" << endl;
            outputFile.close();
        }
        else {
            cout << "Ошибка при открытии файла output.txt для подсчета размера." << endl;
        }
    }
    return 0;
}
