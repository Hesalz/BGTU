#include <iostream>
#include <string>
#include <vector>
#include <algorithm>
#include <iomanip>

using namespace std;

struct Camp {
	string CampName;
	string CampPlace;
	string CampType;
	int voucher;
};

const short CampTypesSize = 3;

string CampNames[5] = { "Диспансер","Ультра","Сатурн","Луна","Солнышко" };

string CampPlaces[5] = { "Минск","Витебск","Брест","Могилёв","Гомель" };
string CampTypes[3] = { "Лекарственное","Психотерапия","Реабилитация" };

const short SIZE = 6;



Camp lagger[SIZE];

int genRand(int from, int to) {
	return from + rand() % (to - from + 1);
}


bool cmp(Camp p1, Camp p2)
{
	return p1.CampName.compare(p2.CampName) < 0;
}

void showInfo() {

	vector<Camp> typed;

	for (int i = 0; i < 3; i++)
	{

		for (int h = 0; h < SIZE; h++) {
			if (lagger[h].CampType.compare(CampTypes[i]) == 0)
			{
				typed.push_back(lagger[h]);
			}
		}

		sort(typed.begin(), typed.end(), cmp);

		for (int x = 0; x < typed.size(); x++) {
			cout << "Название лагеря: " << typed[x].CampName;
			cout << setw(23) << "Место лагеря: " << typed[x].CampPlace;
			cout << setw(23) << "Профиль лечения: " << typed[x].CampType;
			cout << setw(23) << "Количество путевок: " << typed[x].voucher << endl;
			cout << endl;
		}

		typed = {};
	}




}



void genCamps()
{


	for (int i = 0; i < SIZE; i++) {
		lagger[i].CampName = CampNames[genRand(0, 4)];
		lagger[i].CampPlace = CampPlaces[genRand(0, 4)];
		lagger[i].CampType = CampTypes[genRand(0, 2)];
		lagger[i].voucher = genRand(0, 12);

	}
	cout << "\nИнформация сгенирирована\n";
}


void search3() {
	int n;
	cout << "Введите номер: ";
	cin >> n;
	n = n - 1;
	cout << "Название лагеря: " << lagger[n].CampName;
	cout << setw(23) << "Место лагеря: " << lagger[n].CampPlace;
	cout << setw(23) << "Профиль лечения: " << lagger[n].CampType;
	cout << setw(23) << "Количество путевок: " << lagger[n].voucher << endl;
	cout << endl;

}


void main()
{

	setlocale(LC_CTYPE, "ru");
	short choose;

	cout << "Выберите, что сделать:\n1. Сгенерировать информацию;\n2. Вывести информацию о путёвках;\n3. Поиск по номеру;\n4. Выход\n";
	cin >> choose;
	switch (choose)
	{
	case 1: genCamps(); break;
	case 2: showInfo(); break;
	case 3: search3(); break;
	}
	if (choose != 4) {
		main();
	}
}