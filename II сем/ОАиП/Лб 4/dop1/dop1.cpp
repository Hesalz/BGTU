#include <iostream>
#include <fstream>
#include <string>

using namespace std;

struct Student {
	string name;
	int maths;
	int physics;
	int english;
	int oaip;
	int vrpo;
};

const short StudCount = 10;
Student students[StudCount]; 
string StudentsNames[11] = { "Василий Пупкин","Вадим Бедолага","Александр Труба","Яна Лебедь","Иван Иваныч","Тишка Тихонов", "Абсурд Дигирович", "Назар Виталь", "Александра Гоголь", "Полина Нерв", "Исходный Лавелас"};

void exams() {
	if (students->name != "")
	{
		int numStudents = 0;
		for (int i = 0; i < StudCount; ++i) {

			if ((students[i].maths == 5 || students[i].maths == 4) &&
				(students[i].physics == 5 || students[i].physics == 4) &&
				(students[i].english == 5 || students[i].english == 4) &&
				(students[i].oaip == 5 || students[i].oaip == 4) &&
				(students[i].vrpo == 5 || students[i].vrpo == 4))
			{
				cout << i << ") Студент: " << students[i].name << " сдал экзамены на 4 или 5" << endl;
				numStudents++;
			}
			else cout << i << ") Студент: " << students[i].name << " не сдал экзамены на 4 или 5" << endl;
		}
		cout << "Сотношение студентов, сдавших экзамен на 4 или 5, ко всем студентам " << ((double)numStudents / (double)StudCount) * 100 << " % " << endl;
	}
	else { cout << "Список студентов пуст.\n"; }
}

int genRand(int from, int to) {
	return from + rand() % (to - from + 1);
}

void  genStud() {
	for (int i = 0; i < StudCount; ++i) {
		students[i].name = StudentsNames[genRand(0, 10)];
		students[i].maths = genRand(2,5);
		students[i].physics = genRand(2, 5);
		students[i].english = genRand(2, 5);
		students[i].oaip = genRand(2, 5);
		students[i].vrpo = genRand(2, 5);
	} cout << "Студенты успешно сгенерированы.\n";
}


void displayInfo() {
	if (students[0].name != "") {
		for (int i = 0; i < StudCount; ++i) {
			cout << "Имя студента: " << students[i].name << endl;
			cout << "Математика: " << students[i].maths << endl;
			cout << "Физика: " << students[i].physics << endl;
			cout << "Английский: " << students[i].english << endl;
			cout << "ОАиП: " << students[i].oaip << endl;
			cout << "ВРПО: " << students[i].vrpo << endl;
			cout << endl;
		}
	}
	else { cout << "Список студентов пуст.\n"; }
}

void main() {
	setlocale(LC_CTYPE, "ru");
	srand(time(NULL));
	const int SIZE = 7;
	Student students[SIZE];
	short choose;

	cout << "\nВыберите, что сделать:\n1. Проверить, сколько студентов положительно сдали экзамены;\n2. Вывести информацию о студентах;\n3. Сгенирировать студентов;\n4. Выход\n";
	cin >> choose;
	switch (choose)
	{
	case 1: exams(); break;
	case 2: displayInfo(); break;
	case 3: genStud(); break;
	case 4: return;
	}

	if (choose != 4) main();
}