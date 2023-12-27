#include <iostream>
#include <locale>
using namespace std;

void dop1() {
	cout << endl << "Доп. задание 1";
	int n, a[98], week = 0, days = 0, maxRainfall = 0, weeklyRainfall = 0, currentWeek = 0;
	cout << endl << "Введите количество дней: ";
	cin >> n;

	if (n < 7) {
		cout << "Недостаточно данных для поиска недели." << endl;
		return;
	}

	for (int i = 0; i < n; ++i) {
		cout << "Введите количество осадков за день " << (i + 1) << ": ";
		cin >> a[i];
	}
	for (int i = 0; i < n; i++) {
		weeklyRainfall += a[i];
		days += 1;
		if (days == 7) {
			currentWeek += 1;
			if (weeklyRainfall > maxRainfall) {
				maxRainfall = weeklyRainfall;
				week = currentWeek;
			}
			days = 0;
			weeklyRainfall = 0;
		}
	}

	cout << "Наибольшее количество осадков выпало на " << week << "-ой неделе." << endl;
}

void dop2() {
	cout << endl << "Доп. задание 2" << endl;
	const int n = 10;
	int a[n], result = 0;
	srand((unsigned)time(NULL));
	cout << "Исходный массив: " << endl;
	for (int i = 0; i < n; i++) {
		a[i] = rand() % n;
	}
	for (int i = 0; i < n; i++) {
		cout << "a[" << i << "]" << " " << a[i] << endl;
	}
	for (int i = 0; i < n; i++) {
		if (a[i] == a[i + 1]) {
			result += 1;
		}
	}
	cout << "Количество пар соседних элементов массива с одинаковыми значениями: " << result << endl;
}

void dop3() {
	cout << endl << "Доп. задание 3" << endl;
	int a[10], n = 10, maxCount = 1, currentCount = 1;
	srand((unsigned)time(NULL));
	for (int i = 0; i < n; i++) {
		a[i] = rand() % n;
	}
	for (int i = 1; i < n; ++i) {
		if (a[i] == a[i - 1]) {
			currentCount++;
		}
		else {
			currentCount = 1;
		}

		if (currentCount > maxCount) {
			maxCount = currentCount;
		}
	}
	cout << "Исходный массив: " << endl;
	for (int i = 0; i < n; i++) {
		cout << "a[" << i << "]" << " " << a[i] << endl;
	}

	cout << endl << "Наибольшее число подряд идущих одинаковых элементов: " << maxCount << endl;
}

void second() {
	setlocale(LC_ALL, "rus");
	int w[100],e[100], n, j = 0;
	cout << endl << "Введите размер массива (<= 100): ";
	cin >> n;
	if (n > 100)
		return;
	cout << "Исходный массив: ";
	srand((unsigned)time(NULL));
	for (int i = 0; i < n; i++) {
		w[i] = rand() % 101 - 50;
		cout << w[i] << " ";
	}
	for (int i = 0; i < n; i++) {
		if (w[i] < 0) {
			e[j] = w[i];
			j++;
		}
	}
		for (int i = 0; i < n + 1; i++) {
		if (w[i] > 0) {
			e[j] = w[i];
			j++;
		}
	}
	cout << endl << "Полученный массив : ";
	for (int i = 0; i < n; i++) {
		cout << " " << e[i];
	}
	cout << endl;
}

void main() {
	setlocale(LC_ALL, "rus");
	int a[100], n, newsize = 0, b[200];
	cout << "Введите размер массива (<= 100):";
	cin >> n;
	if (n > 100)
		return;
	cout << "Исходный массив: ";
	srand((unsigned)time(NULL));
	for (int i = 0; i < n; i++) {
		a[i] = rand() % 100;
		cout << a[i] << " ";
	}
	for (int i = 0; i < n; i++) {
		if ((i + 1) % 7 != 0) {
			a[newsize++] = a[i];
		}
	}
	cout << endl << "Массив без индексов кратных 7: ";
	for (int i = 0; i < newsize; i++) {
		cout << " " << a[i];
	}
	newsize = 0;
	for (int i = 0; i < n; i++) {
		b[newsize++] = a[i];
		if (a[i] % 2 != 0) {
			b[newsize++] = 4;
		}
	}
	cout << endl << "Массив с добавлением элемента 4 после каждого нечетного элемента: ";
	for (int i = 0; i < newsize; i++) {
		cout << " " << b[i];
	}
	cout << endl;
	second();
	dop1();
	dop2();
	dop3();
}
