#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <cstring>

using namespace std;

int main() { // Доп. задание 5
    setlocale(LC_ALL, "rus");
    const char* firstSentence = "Первое предложение осьминог";
    const char* secondSentence = "Второе предложение после Первое.";

    char shortestWord[100];
    strcpy(shortestWord, firstSentence); 

    char firstSentenceCopy[100];
    strcpy(firstSentenceCopy, firstSentence);

    char* token = strtok(firstSentenceCopy, " ");

    while (token != 0) {
        if (strlen(token) < strlen(shortestWord) && !strstr(secondSentence, token)) {
            strcpy(shortestWord, token);
        }
        token = strtok(0, " ");
    }

    cout << "Самое короткое слово, которого нет во втором предложении: " << shortestWord << endl;

    return 0;
}
