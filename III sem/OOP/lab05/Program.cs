using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace lab05
{
    interface IDriveable
    {
        void Drive();
        bool DoClone();
    }

    abstract class Vehicle
    {
        public abstract void StartEngine();
        public abstract bool DoClone();
        public abstract override string ToString();
    }

    class Engine
    {
        public string Model { get; set; }
        public int Horsepower { get; set; }

        public Engine(string model, int horsepower)
        {
            Model = model;
            Horsepower = horsepower;
        }

        public override string ToString()
        {
            return $"Двигатель: {Model}, Лошадиная сила: {Horsepower}";
        }
    }

    class Car : Vehicle, IDriveable
    {
        public string Brand { get; set; }
        public Engine CarEngine { get; set; }

        public Car(string brand, Engine engine)
        {
            Brand = brand;
            CarEngine = engine;
        }

        public override void StartEngine()
        {
            Console.WriteLine($"Запуск двигателя {Brand}.");
        }

        public void Drive()
        {
            Console.WriteLine($"{Brand} в движении.");
        }

        public override bool DoClone()
        {
            return true;
        }

        bool IDriveable.DoClone()
        {
            return false;
        }

        public override string ToString()
        {
            return $"Модель: {Brand}, {CarEngine}";
        }
    }

    class Printer
    {
        public void IAmPrinting(Vehicle vehicle)
        {
            Console.WriteLine(vehicle.ToString());
        }
    }
    class SentientBeing
    {
        public string Name { get; set; }

        public SentientBeing(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"Разумное существо с именем: {Name}";
        }
    }

    class Human : SentientBeing
    {
        public Human(string name) : base(name) { }

        public override string ToString()
        {
            return $"Человек с именем: {Name}";
        }
    }

    enum YearOfCreation
    {
        Unknown = 0,
        Year2000 = 2000,
        Year2005 = 2005,
        Year2010 = 2010,
        Year2015 = 2015
    }

    struct Specifications
    {
        public int PowerLevel;
        public int DefenseLevel;

        public Specifications(int powerLevel, int defenseLevel)
        {
            PowerLevel = powerLevel;
            DefenseLevel = defenseLevel;
        }

        public override string ToString()
        {
            return $"Мощность: {PowerLevel}, Защита: {DefenseLevel}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Human human = new Human("Виталик");
            Transformer transformer1 = new Transformer("Оптимус", "Прайм", YearOfCreation.Year2005, new Specifications(500, 300));
            Transformer transformer2 = new Transformer("Бамблби", "Камаро", YearOfCreation.Year2010, new Specifications(300, 200));
            Army army = new Army();
            army.AddUnit(human);
            army.AddUnit(transformer1);
            army.AddUnit(transformer2);
            ArmyController controller = new ArmyController(army);
            army.DisplayUnits();

            var unit = controller.FindByCreationYear(YearOfCreation.Year2005);
            Console.WriteLine($"Найденная боевая единица 2005 года: {unit}");
            controller.DisplayTransformersByPower(500);
            Console.WriteLine($"Количество боевых единиц в армии: {controller.CountUnits()}");

            // 4
            Engine carEngine = new Engine("V8", 500);
            Car car = new Car("Порше", carEngine);
            Printer printer = new Printer();
            printer.IAmPrinting(car);

            Console.WriteLine($"Клон автомобиля из абстрактного класса: {car.DoClone()}");
            Console.WriteLine($"Клон автомобиля из интерфейса: {(car as IDriveable).DoClone()}");

            transformer1.Drive();
            Console.WriteLine($"Клон трансформера: {transformer1.DoClone()}");

            if (transformer1 is IDriveable)
            {
                Console.WriteLine("Трансформер может водить");
            }

            Console.WriteLine(car.ToString());
            Console.WriteLine(human.ToString());
            Console.WriteLine(transformer1.ToString());
            Console.WriteLine(transformer2.ToString());
        }
    }
}
