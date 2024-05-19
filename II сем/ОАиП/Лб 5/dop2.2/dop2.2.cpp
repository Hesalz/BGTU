#include <iostream>
#include <fstream>
#include <string>
#include <Windows.h>

using namespace std;

struct TrainData {
    int trainNumber;
    string destination;
    string daysOfWeek;
    string arrivalTime;
    string departureTime;
};

void inputTrainData(TrainData& train) {
    cout << "Введите номер поезда: ";
    cin >> train.trainNumber;
    cin.ignore(); 
    cout << "Введите пункт назначения: ";
    getline(cin, train.destination);
    cout << "Введите дни следования: ";
    getline(cin, train.daysOfWeek);
    cout << "Введите время прибытия: ";
    getline(cin, train.arrivalTime);
    cout << "Введите время отправления: ";
    getline(cin, train.departureTime);
}

void writeToFile(const TrainData& train, const string& filename) {
    ofstream outFile(filename, ios::app);
    if (outFile.is_open()) {
        outFile << train.trainNumber << "\t";
        outFile << train.destination << "\t";
        outFile << train.daysOfWeek << "\t";
        outFile << train.arrivalTime << "\t";
        outFile << train.departureTime << endl;
        outFile.close();
        cout << "Данные о поезде успешно записаны в файл." << endl;
    }
    else {
        cout << "Ошибка открытия файла для записи." << endl;
    }
}

void readFromFileAndPrint(const string& filename) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Содержимое файла:" << endl;
        int trainNumber;
        string destination, daysOfWeek, arrivalTime, departureTime;
        while (inFile >> trainNumber >> destination >> daysOfWeek >> arrivalTime >> departureTime) {
            cout << trainNumber << "\t";
            cout << destination << "\t";
            cout << daysOfWeek << "\t";
            cout << arrivalTime << "\t";
            cout << departureTime << endl;
        }
        inFile.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
    }
}

void searchByDestination(const string& filename, const string& destination) {
    ifstream inFile(filename);
    if (inFile.is_open()) {
        cout << "Результаты поиска по пункту назначения " << destination << ":" << endl;
        int trainNumber;
        string dest, days, arrival, departure;
        bool found = false;
        while (inFile >> trainNumber >> dest >> days >> arrival >> departure) {
            if (dest == destination) {
                cout << trainNumber << "\t";
                cout << dest << "\t";
                cout << days << "\t";
                cout << arrival << "\t";
                cout << departure << endl;
                found = true;
            }
        }
        if (!found) {
            cout << "Поезда с пунктом назначения " << destination << " не найдены." << endl;
        }
        inFile.close();
    }
    else {
        cout << "Ошибка открытия файла для чтения." << endl;
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);
    TrainData train;
    string filename = "trains.txt"; 

    int choice;
    string destination;

    do {
        cout << "\nМеню:" << endl;
        cout << "1. Ввод данных о поезде" << endl;
        cout << "2. Запись данных о поезде в файл" << endl;
        cout << "3. Вывод данных из файла на экран" << endl;
        cout << "4. Поиск поезда по пункту назначения" << endl;
        cout << "0. Выход из программы" << endl;
        cout << "Выберите действие: ";
        cin >> choice;
        cin.ignore();

        switch (choice) {
        case 1:
            inputTrainData(train);
            break;
        case 2:
            writeToFile(train, filename);
            break;
        case 3:
            readFromFileAndPrint(filename);
            break;
        case 4:
            cout << "Введите пункт назначения для поиска: ";
            getline(cin, destination);
            searchByDestination(filename, destination);
            break;
        case 0:
            break;
        default:
            cout << "Неверный выбор. Попробуйте еще раз." << endl;
        }
    } while (choice != 0);

    return 0;
}
