#include <iostream>
#include <cstring>

using namespace std;

int main() { // Доп. задание 1
    char words[][10] = {"apple", "banana", "orange", "grape", "pear"};
    char ending[] = "e";
    char words_with_ending[5][10];
    int count = 0;

    for (int i = 0; i < 5; i++) {
        if (strlen(words[i]) >= strlen(ending) && strcmp(words[i] + strlen(words[i]) - strlen(ending), ending) == 0) {
            strcpy_s(words_with_ending[count], words[i]);
            count++;
        }
    }

    for (int i = 0; i < count; i++) {
        cout << words_with_ending[i] << endl;
    }

    return 0;
}
