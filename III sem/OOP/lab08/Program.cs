using System;

class Program
{
    static void Main(string[] args)
    {
        Director director = new Director();
        Worker worker1 = new Worker("Иван", 30000);
        Worker worker2 = new Worker("Мария", 35000);
        Student student1 = new Student("Петр", 15000);
        Student student2 = new Student("Анна", 12000);

        director.Promotion += worker1.OnPromotion;
        director.Promotion += worker2.OnPromotion;
        director.Penalty += worker2.OnPenalty;

        director.Promotion += student1.OnPromotion;
        director.Penalty += student2.OnPenalty;

        Console.WriteLine("Начальное состояние:");
        worker1.DisplayInfo();
        worker2.DisplayInfo();
        student1.DisplayInfo();
        student2.DisplayInfo();

        Console.WriteLine("\nДиректор проводит действия:");
        director.RaiseSalary(5000);
        director.GivePenalty(2000);

        Console.WriteLine("\nСостояние после событий:");
        worker1.DisplayInfo();
        worker2.DisplayInfo();
        student1.DisplayInfo();
        student2.DisplayInfo();

        Console.WriteLine("\nОбработка строки:");
        string input = "  Строка   строка, строка строка СТРОКА!  ";
        var stringProcessor = new StringProcessor();
        string processed = stringProcessor.Process(input);
        Console.WriteLine($"Исходная строка: {input}");
        Console.WriteLine($"Обработанная строка: {processed}");
    }
}