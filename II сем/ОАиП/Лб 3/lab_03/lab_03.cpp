#include <iostream>
#include <fstream>
#include <string>
#include <Windows.h>
using namespace std;

int main() {   
    setlocale(LC_ALL, "ru");
    ifstream in("FILE1.txt");
    ofstream out("FILE2.txt");
    if (!in || !out) {
        cout << "Ошибка открытия файлов" << endl;
        return 1;
    }

    int k = 0;
    cout << "Укажите число k: ";
    cin >> k;
    string line;
    int count = 0;
    while (getline(in, line)) {
        count++;
        if (count >= k && count < k + 3) {
            out << line << endl;
        }
    }

    in.close();
    out.close();

    ifstream in2("FILE2.txt");
    if (!in2) {
        cout << "\nОшибка открытия файла FILE2" << endl;
        return 2;
    }

    char vowels[] = "AaEeUuIiOo", ch;
    int vowel_count = 0;
    while (in2.get(ch)) {
        for (int i = 0; i < sizeof(vowels) - 1; i++) {
            if (ch == vowels[i]) {
                vowel_count++;
            }
        }
    }
    cout << "Количество гласных букв в строке: " << vowel_count << endl;
    in2.close();

    return 0;
}
