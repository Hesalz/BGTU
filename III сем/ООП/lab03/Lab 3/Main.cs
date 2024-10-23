using System;
using System.Collections.Generic;
using lab3;

class Program {
    static void Main() {
        var production = new Set.Production(1, "SpaceX");
        var developer = new Set.Developer("Илон Маск", 123, "Software");

        var set1 = new Set(new List<int> { 3, 5, 7, 7 }, production, developer);
        var set2 = new Set(new List<int> { 3, 4, 5, 6, 7 }, production, developer);

        Console.WriteLine($"Множество 1: {set1}");
        Console.WriteLine($"Множество 2: {set2}");

        set1 += 9;
        Console.WriteLine($"Множество 1 после добавления элемента: {set1}");

        var unionSet = set1 + set2;
        Console.WriteLine($"Объединение множеств: {unionSet}");

        var intersectionSet = set1 * set2;
        Console.WriteLine($"Пересечение множеств: {intersectionSet}");

        Console.WriteLine($"Мощность множества 1: {(int)set1}");

        if (set1) {
            Console.WriteLine("Размер множества вне диапазона от 5 до 10.");
        }
        else {
            Console.WriteLine("Размер множества находится в диапазоне от 5 до 10.");
        }

        Console.WriteLine($"Сумма элементов первого множества: {StatisticOperations.Sum(set1)}");
        Console.WriteLine($"Разница между максимальным и минимальным элементом: {StatisticOperations.DifferenceBetweenMaxAndMin(set1)}");
        Console.WriteLine($"Количество элементов в множестве 1: {StatisticOperations.CountElements(set1)}");

        set1.RemoveDuplicates();
        Console.WriteLine($"Множество 1 после удаления дубликатов: {set1}");

        string exampleStr = "Hello World!";
        Console.WriteLine($"Строка с добавлением 'слово': {exampleStr.AddOccupied()}");
    }
}
