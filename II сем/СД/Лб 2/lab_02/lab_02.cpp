#include <iostream>
#include <string>
using namespace std;

int choice; 

void Menu() {
	cout << "1. Добавить нового клиента\n"
		<< "2. Оценить товар и выдать сумму под залог\n"
		<< "3. Просмотреть список активных залогов\n"
		<< "4. Продажа товара\n"
		<< "5. Посмотреть список клиентов\n"
		<< "6. Выход\n"
		<< "Введите ваш выбор: ";
	cin >> choice;
};

int main() {
	setlocale(LC_ALL, "rus");
	int n;
	string t;
	Menu();
	while (choice != 6) {
		switch (choice) {
		case 1:
			system("cls");
			cout << "Введите ФИО клиента: ";
			cin.ignore();
			getline(cin, t);
			cout << "Данные введены.\n";
			system("pause");
			system("cls");
			Menu();
			break;
		case 2:
			system("cls");
			cout << "Введите имя товара: ";
			cin.ignore();
			getline(cin, t);
			cout << "Сумма под залог - 650BYN\n";
			system("pause");
			system("cls");
			Menu();
			break;
		case 3:
			system("cls");
			cout << "Список активных залогов:\n 1. Телефон\n 2. Часы\n 3. Золотая цепочка\n 4. Брошь\n";
			system("pause");
			system("cls");
			Menu();
			break;
		case 4:
			system("cls");
			cout << "Введите номера товара, который необходимо продать: ";
			cin >> n;
			cout << "Сумма залога: 330BYN\n";
			system("pause");
			system("cls");
			Menu();
			break;
		case 5:
			system("cls");
			cout << "Список клиентов:\n 1. Иванов Иван Иванович\n 2. Михаилов Михаил Михайлович\n 3. Стасов Стас Станиславович\n 4. Кириллов Кирилл Кирилович\n";
			system("pause");
			system("cls");
			Menu();
			break;
		case 6:
			exit;
		default:
			system("cls");
			cout << "Неверный выбор\n";
			system("pause");
			system("cls");
			Menu();
			break;
		}
	}
}