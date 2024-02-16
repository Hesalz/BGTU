#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <cstring>

using namespace std;

int main() { // Доп. задание 3
    setlocale(LC_ALL, "rus");
    char sentence[] = "муравей усердно трудился как муравей";

    char modifiedSentence[1000] = "";
    char* words[100];
    int wordCount = 0;

    char* token = strtok(sentence, " ");

    while (token != 0) {
        bool isDuplicate = false;

        for (int i = 0; i < wordCount; i++) {
            if (strcmp(token, words[i]) == 0) {
                isDuplicate = true;
                break;
            }
        }

        if (!isDuplicate) {
            words[wordCount] = token;
            wordCount++;
            if (strlen(modifiedSentence) != 0) {
                strncat(modifiedSentence, " ", 1);
            }
            strncat(modifiedSentence, token, 999);
        }
        token = strtok(0, " ");
    }

    cout << "Предложение без повторяющихся слов: " << modifiedSentence << endl;

    return 0;
}
