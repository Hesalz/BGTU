using System;

public class Director
{
    public event Action<int> Promotion;
    public event Action<int> Penalty;

    public void RaiseSalary(int amount)
    {
        Console.WriteLine($"Директор повышает зарплату на {amount}.");
        Promotion?.Invoke(amount);
    }

    public void GivePenalty(int amount)
    {
        Console.WriteLine($"Директор выносит штраф на {amount}.");
        Penalty?.Invoke(amount);
    }
}
