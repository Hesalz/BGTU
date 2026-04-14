using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        Task task1 = Task.Run(() => FindPrimes());
        Console.WriteLine($"Идентификатор задачи 1: {task1.Id}");
        task1.Wait();
        Console.WriteLine($"Задача 1 завершена: {task1.IsCompleted}");

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task task2 = Task.Run(() => FindPrimesWithCancellation(cancellationTokenSource.Token));
        Console.WriteLine($"Идентификатор задачи 2: {task2.Id}");
        Task.Delay(2000).ContinueWith(t => cancellationTokenSource.Cancel());
        task2.Wait();
        Console.WriteLine($"Задача 2 завершена: {task2.IsCompleted}");

        Task<double> task3 = Task.Run(() => Math.Sqrt(16));
        Task<double> task4 = Task.Run(() => Math.Sqrt(25));
        Task<double> task5 = Task.Run(() => Math.Sqrt(36));
        Task.WhenAll(task3, task4, task5).ContinueWith(t =>
        {
            Console.WriteLine("Результаты:");
            Console.WriteLine(task3.Result);
            Console.WriteLine(task4.Result);
            Console.WriteLine(task5.Result);
        }).Wait();

        Task<string> task6 = Task.Run(() => "Задача 6 завершена.");
        Task<string> task7 = Task.Run(() => "Задача 7 завершена.");
        Task.WhenAll(task6, task7).ContinueWith(t =>
        {
            Console.WriteLine("Все задачи завершены.");
            Console.WriteLine("Результат задачи 6: " + task6.Result);
            Console.WriteLine("Результат задачи 7: " + task7.Result);
        }).Wait();

        Parallel.For(0, 1000000, i =>
        {
            _ = i * 2;
        });

        Console.WriteLine("Задача 5 завершена.");

        Parallel.Invoke(
            () => { Console.WriteLine("Задача 6.1"); },
            () => { Console.WriteLine("Задача 6.2"); },
            () => { Console.WriteLine("Задача 6.3"); }
        );

        var warehouse = new BlockingCollection<string>(5);
        var suppliers = new[] { "Товар1", "Товар2", "Товар3", "Товар4", "Товар5" };
        var buyers = Enumerable.Range(1, 10).ToArray();

        var supplyTasks = suppliers.Select((product, index) => Task.Run(() =>
        {
            Thread.Sleep(TimeSpan.FromSeconds(index + 1));
            warehouse.Add(product);
            Console.WriteLine($"Поставщик добавил: {product}");
        })).ToArray();

        var buyTasks = buyers.Select(buyer => Task.Run(() =>
        {
            try
            {
                string product = warehouse.Take();
                Console.WriteLine($"Покупатель {buyer} купил {product}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Покупатель {buyer} не смог найти товар.");
            }
        })).ToArray();

        Task.WhenAll(supplyTasks).ContinueWith(t =>
        {
            warehouse.CompleteAdding();
        }).Wait();

        Task.WhenAll(buyTasks).Wait();

        AsyncMethod().Wait();
    }

    static void FindPrimes()
    {
        int n = 1000000;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var primes = SieveOfEratosthenes(n);
        stopwatch.Stop();
        Console.WriteLine($"Найдено {primes.Count()} простых чисел.");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} ms");
    }

    static void FindPrimesWithCancellation(CancellationToken token)
    {
        int n = 1000000;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var primes = SieveOfEratosthenesWithCancellation(n, token);
        stopwatch.Stop();
        if (primes.Any())
        {
            Console.WriteLine($"Найдено {primes.Count()} простых чисел.");
        }
        else
        {
            Console.WriteLine("Задача была отменена.");
        }
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} ms");
    }

    static bool[] SieveOfEratosthenes(int limit)
    {
        bool[] sieve = new bool[limit + 1];
        for (int i = 2; i <= limit; i++)
            sieve[i] = true;

        for (int i = 2; i * i <= limit; i++)
        {
            if (sieve[i])
            {
                for (int j = i * i; j <= limit; j += i)
                    sieve[j] = false;
            }
        }

        return sieve;
    }

    static bool[] SieveOfEratosthenesWithCancellation(int limit, CancellationToken token)
    {
        bool[] sieve = new bool[limit + 1];
        for (int i = 2; i <= limit; i++)
            sieve[i] = true;

        for (int i = 2; i * i <= limit; i++)
        {
            if (sieve[i])
            {
                for (int j = i * i; j <= limit; j += i)
                {
                    if (token.IsCancellationRequested)
                    {
                        return new bool[0];
                    }
                    sieve[j] = false;
                }
            }
        }

        return sieve;
    }

    static async Task AsyncMethod()
    {
        Console.WriteLine("Начало работы...");
        await Task.Delay(2000);
        Console.WriteLine("Задача завершена.");
    }
}
