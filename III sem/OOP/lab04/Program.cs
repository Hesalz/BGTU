using System;

interface IDriveable {
    void Drive();
    bool DoClone();
}

abstract class Vehicle {
    public abstract void StartEngine();
    public abstract bool DoClone();
    public abstract override string ToString();
}

class Engine {
    public string Model { get; set; }
    public int Horsepower { get; set; }

    public Engine(string model, int horsepower)     {
        Model = model;
        Horsepower = horsepower;
    }

    public override string ToString() {
        return $"Двигатель: {Model}, Лоашдиная сила: {Horsepower}";
    }
}

class Car : Vehicle, IDriveable {
    public string Brand { get; set; }
    public Engine CarEngine { get; set; }

    public Car(string brand, Engine engine) {
        Brand = brand;
        CarEngine = engine;
    }

    public override void StartEngine() {
        Console.WriteLine($"Запуск двигателя {Brand}.");
    }

    public void Drive() {
        Console.WriteLine($"{Brand} в движении.");
    }

    public override bool DoClone() {
        return true;
    }

    bool IDriveable.DoClone() {
        return false;
    }

    public override string ToString() {
        return $"Модель: {Brand}, {CarEngine}";
    }
}

class SentientBeing {
    public string Name { get; set; }

    public SentientBeing(string name) {
        Name = name;
    }

    public override string ToString() {
        return $"Разумное существо с именем: {Name}";
    }
}

class Human : SentientBeing {
    public Human(string name) : base(name) { }

    public override string ToString() {
        return $"Человек с именем: {Name}";
    }
}

sealed class Transformer : Human, IDriveable {
    public string Model { get; set; }

    public Transformer(string name, string model) : base(name) {
        Model = model;
    }

    public void Drive() {
        Console.WriteLine($"{Name} (Трансформер {Model}) за рулем");
    }

    public bool DoClone() {
        return true;
    }

    public override string ToString() {
        return $"Трансформер: {Name}, Модель: {Model}";
    }
}

class Printer {
    public void IAmPrinting(Vehicle vehicle) {
        Console.WriteLine(vehicle.ToString());
    }
}

class Program {
    static void Main(string[] args) {
        
        Engine carEngine = new Engine("V8", 500);
        Car car = new Car("Порше", carEngine);
        Human human = new Human("Виталик");
        Transformer transformer = new Transformer("Оптимус", "Прайм");

        Vehicle[] vehicles = new Vehicle[] { car };

        Printer printer = new Printer();
        foreach (var v in vehicles) {
            printer.IAmPrinting(v);
        }

        Console.WriteLine($"Клон автомобиля из абстрактного класса: {car.DoClone()}");
        Console.WriteLine($"Клон автомобиля из интерфейса: {(car as IDriveable).DoClone()}");

        transformer.Drive();
        Console.WriteLine($"Клон трансформера: {transformer.DoClone()}");

        if (transformer is IDriveable) {
            Console.WriteLine("Трансформер может водить");
        }

        Console.WriteLine(car.ToString());
        Console.WriteLine(human.ToString());
        Console.WriteLine(transformer.ToString());
    }
}
