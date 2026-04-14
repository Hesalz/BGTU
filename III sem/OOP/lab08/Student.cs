using System;

public class Student
{
    public string Name { get; }
    public int Scholarship { get; private set; }

    public Student(string name, int scholarship)
    {
        Name = name;
        Scholarship = scholarship;
    }

    public void OnPromotion(int amount)
    {
        Scholarship += amount;
        Console.WriteLine($"Студенту {Name} повысили стипендию {amount}. Новая стипендия: {Scholarship}");
    }

    public void OnPenalty(int amount)
    {
        Scholarship -= amount;
        Console.WriteLine($"Студенту {Name} понизили стипендию на {amount}. Новая стипендия: {Scholarship}");
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Студент {Name}: стипендия {Scholarship}");
    }
}
