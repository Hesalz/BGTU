#include <iostream>
using namespace std;

enum class Planet {
    Меркурий,
    Венера,
    Земля,
    Марс,
    Юпитер,
    Сатурн,
    Уран,
    Нептун
};

void displayPlanetName(Planet selectedPlanet) {
    switch (selectedPlanet) {
    case Planet::Меркурий:
        cout << "Меркурий";
        break;
    case Planet::Венера:
        cout << "Венера";
        break;
    case Planet::Земля:
        cout << "Земля";
        break;
    case Planet::Марс:
        cout << "Марс";
        break;
    case Planet::Юпитер:
        cout << "Юпитер";
        break;
    case Planet::Сатурн:
        cout << "Сатурн";
        break;
    case Planet::Уран:
        cout << "Уран";
        break;
    case Planet::Нептун:
        cout << "Нептун";
        break;
    default:
        cout << "Неверный выбор";
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    cout << "Выберите номер планеты (от 1 до 8): ";
    int choice;
    cin >> choice;

    Planet selectedPlanet = static_cast<Planet>(choice - 1);

    cout << "Выбранная планета: ";
    displayPlanetName(selectedPlanet);
    return 0;
}
