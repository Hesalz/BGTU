#include <iostream>
#include <string>
#include <Windows.h>
using namespace std;

enum class Subject {
    Math,
    Physics,
    Chemistry,
    Biology,
    History,
    Geography,
    Literature
};

struct Student {
    string fullName;
    unsigned int classLevel : 4;
    char classLetter;
    double grades[7];
    double averageGrade;
};

string getSubjectName(Subject subject) {
    switch (subject) {
    case Subject::Math:
        return "Математика";
    case Subject::Physics:
        return "Физика";
    case Subject::Chemistry:
        return "Химия";
    case Subject::Biology:
        return "Биология";
    case Subject::History:
        return "История";
    case Subject::Geography:
        return "География";
    case Subject::Literature:
        return "Литература";
    default:
        return "Неизвестный предмет";
    }
}

void inputStudentData(Student& student) {
    cout << "\nВведите ФИО ученика: ";
    getline(cin, student.fullName);

    cout << "Введите класс (от 1 до 11): ";
    unsigned int classLevelInput;
    cin >> classLevelInput;
    student.classLevel = classLevelInput;

    cout << "Введите букву класса: ";
    cin >> student.classLetter;

    cout << "Введите оценки по следующим предметам:\n";
    for (int i = 0; i < 7; ++i) {
        cout << "Оценка по предмету " << getSubjectName(static_cast<Subject>(i)) << ": ";
        cin >> student.grades[i];
    }

    double sum = 0;
    for (int i = 0; i < 7; ++i) {
        sum += student.grades[i];
    }
    student.averageGrade = sum / 7;
}


void printStudentData(const Student& student) {
    cout << "ФИО: " << student.fullName << endl;
    cout << "Класс: " << student.classLevel << student.classLetter << endl;
    cout << "Оценки по предметам:\n";
    for (int i = 0; i < 7; i++) {
        cout << getSubjectName(static_cast<Subject>(i)) << ": " << student.grades[i] << endl;
    }
    cout << "Средний балл: " << student.averageGrade << endl;
}

string getLastName(const string& fullName) {
    size_t spaceIndex = fullName.find(' ');
    if (spaceIndex != string::npos) {
        return fullName.substr(0, spaceIndex);
    }
    else {
        return fullName;
    }
}

void searchStudent(const Student students[], int numStudents) {
    string searchName;
    cout << "\nВведите фамилию для поиска: ";
    getline(cin, searchName);

    bool found = false;
    for (int i = 0; i < numStudents; i++) {
        if (getLastName(students[i].fullName) == searchName) {
            printStudentData(students[i]);
            found = true;
        }
    }

    if (!found) {
        cout << "\nУченик с такой фамилией не найден!" << endl;
    }
}


void deleteStudent(Student students[], int& numStudents, int index) {
    if (index < 0 || index > numStudents) {
        cout << "Некорректный номер ученика.";
    }
    else {
        for (int i = index; i < numStudents; i++) {
            students[i] = students[i + 1];
        }
        numStudents--;
        cout << "Ученик успешно удален.";
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    const int MAX_STUDENTS = 100;
    Student students[MAX_STUDENTS];
    int numStudents = 0;

    int choice;
    do {
        cout << "\nМеню:\n";
        cout << "1. Добавить ученика\n";
        cout << "2. Вывести информацию об ученике\n";
        cout << "3. Удалить ученика\n";
        cout << "4. Поиск ученика по фамилии\n";
        cout << "5. Список учеников\n";
        cout << "0. Выйти\n";
        cout << "Выберите действие: ";
        cin >> choice;
        cin.ignore();

        switch (choice) {
        case 1: {
            if (numStudents < MAX_STUDENTS) {
                inputStudentData(students[numStudents]);
                numStudents++;
            }
            else {
                cout << "\nДостигнуто максимальное количество учеников!" << endl;
            }
            break;
        }
        case 2: {
            int index;
            cout << "\nВведите номер ученика: ";
            cin >> index;
            if (index >= 1 && index < numStudents + 1) {
                cout << "[" << index << "] ";
                printStudentData(students[index - 1]);
            }
            else {
                cout << "Неверный номер ученика!" << endl;
            }
            break;
        }
        case 3: {
            int index;
            cout << "\nВведите номер ученика: ";
            cin >> index;
            if (index >= 1 && index < numStudents + 1) {
                deleteStudent(students, numStudents, index - 1);
            }
            else {
                cout << "Неверный номер ученика!" << endl;
            }
            break;
        }
        case 4: {
            searchStudent(students, numStudents);
            break;
        }
        case 5:
            cout << "\nСписок учеников:\n";
            if (numStudents != 0) {
                for (int i = 0; i < numStudents; i++) {
                    cout << "[" << i + 1 << "] ";  printStudentData(students[i]); cout << endl;
                }
            }
            else { cout << "Список учеников пуст.\n"; }
            break;
        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте снова." << endl;
        }
    } while (choice != 0);

    return 0;
}
