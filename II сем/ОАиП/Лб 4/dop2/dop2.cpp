#include <iostream>
#include <string>
#include <algorithm>
#include <iomanip>
using namespace std;

struct Time {
	int hours;
	int minutes;
};

struct Train {
	string Place;
	int NumTrain;
	Time time;
};


string Places[7] = { "Минск","Гродно","Гомель","Пинск","Брест","Витебск", "Могилёв"};

int genRand(int from, int to) {
	return from + rand() % (to - from + 1);
}

int compTrain(const void* p1, const void* p2)
{
	return strcmp(((Train*)p1)->Place.c_str(), ((Train*)p2)->Place.c_str());
}

void displayTrain(Train* trains, int SIZE) {
	qsort(trains, SIZE, sizeof(trains[0]), compTrain);

	for (int i = 0; i < SIZE; ++i)
	{
		cout << setw(15) << "Номер поезда: " << trains[i].NumTrain << "\t";
		cout << setw(20) << "Место прибытия: " << trains[i].Place << "\t";
		cout << setw(30) << "Время прибытия на вокзал: " << trains[i].time.hours << " часов, " << trains[i].time.minutes << " минут " << endl;
		cout << endl;
	}
};

void displayTrainMode(Train* trains, int SIZE)
{
	qsort(trains, SIZE, sizeof(trains[0]), compTrain);

	int hours_user, minutes_user;

	cout << "Введите время отправления" << endl << "Введите часы: ";
	cin >> hours_user;
	cout << "Введите минуты: ";
	cin >> minutes_user;

	if (hours_user < 0 || hours_user > 23 || minutes_user < 0 || minutes_user >= 60)
	{
		cout << "Некорретное время" << endl;
		exit(1);
	}


	cout << "Ваше время : " << hours_user << " часов, " << minutes_user << " минут " << endl;
	int all_user_time = hours_user * 60 * 60 + minutes_user * 60;

	cout << endl;


	bool ones = false;

	for (int i = 0; i < SIZE; i++)
	{
		if (trains[i].time.hours * 60 + trains[i].time.minutes > hours_user * 60 + minutes_user)
		{
			cout << setw(15) << "Номер поезда: " << trains[i].NumTrain << "\t";
			cout << setw(20) << "Место прибытия: " << trains[i].Place << "\t";
			cout << setw(30) << "Время прибытия на вокзал: " << trains[i].time.hours << " часов, " << trains[i].time.minutes << " минут " << endl;
			cout << endl;
			ones = true;
		}
	}
	if (!ones)
	{
		cout << "Нет поездов." << endl;
	}
}
;

void genTrains(Train* trains, int SIZE)
{
	for (int i = 0; i < SIZE; i++)
	{
		trains[i].NumTrain = genRand(0, 70);
		trains[i].Place = Places[genRand(0, 6)];
		trains[i].time.hours = genRand(0, 23);
		trains[i].time.minutes = genRand(0, 59);
	}
}

void main()
{
	setlocale(LC_CTYPE, "rus");
	srand(time(NULL));
	const int SIZE = 5;
	Train trains[SIZE];
	genTrains(trains, SIZE);
	sort(trains, trains + SIZE, [](const Train& a, const Train& b) { return a.Place < b.Place; });
	displayTrain(trains, SIZE);
	displayTrainMode(trains, SIZE);
	short choose;
	cout << "\n1 - выход из программы.\n";
	cin >> choose;

	if (choose != 1) {
		main();
	}

}