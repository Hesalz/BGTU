using System;
using System.Collections.Generic;
using lab07;

namespace lab07
{
    class Program
    {
        static void Main()
        {

            var intCollection = new CollectionType<int>();
            intCollection.Add(10);
            intCollection.Add(20);
            intCollection.Add(30);
            Console.WriteLine($"Целочисленная коллекция: {intCollection}");

            intCollection.Remove(20);
            Console.WriteLine($"После удаления: {intCollection}");

            var floatCollection = new CollectionType<float>();
            floatCollection.Add(1.5f);
            floatCollection.Add(2.7f);
            Console.WriteLine($"Коллекция вещественных чисел: {floatCollection}");

            var found = floatCollection.FindByPredicate(x => x > 2);
            Console.WriteLine($"Найдено значение больше 2: {found}");

            var customCollection = new CollectionType<CustomType>();
            customCollection.Add(new CustomType("Евгений", 1));
            customCollection.Add(new CustomType("Владислав", 2));

            Console.WriteLine($"Пользовательская коллекция: {customCollection}");

            var collection = new CollectionType<int>();
            collection.Add(1);
            collection.Add(2);
            collection.Add(3);

            string filePath = "documents.txt";

            File<int>.SaveToFile(collection, filePath);

            var loadedCollection = File<int>.LoadFromFile(filePath, int.Parse);
            Console.WriteLine($"Загруженная коллекция: {string.Join(", ", loadedCollection.View())}");















            // Program 3 lab
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
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

            if (set1)
            {
                Console.WriteLine("Размер множества вне диапазона от 5 до 10.");
            }
            else
            {
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
}
