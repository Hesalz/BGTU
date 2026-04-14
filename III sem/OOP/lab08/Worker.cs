using System;

public class Worker
{
    public string Name { get; }
    public int Salary { get; private set; }

    public Worker(string name, int salary)
    {
        Name = name;
        Salary = salary;
    }

    public void OnPromotion(int amount)
    {
        Salary += amount;
        Console.WriteLine($"Работнику {Name} повысили зарплату на {amount}. Новая зарплата: {Salary}");
    }

    public void OnPenalty(int amount)
    {
        Salary -= amount;
        Console.WriteLine($"Работнику {Name} дали штраф на {amount}. Новая зарплата: {Salary}");
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Работник {Name}: зарплата {Salary}");
    }
}
