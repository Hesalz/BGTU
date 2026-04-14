using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("1. Вывод запущенных процессов:");
        PrintRunningProcesses();
        Console.WriteLine("\n2. Исследование домена приложения:");
        InspectAppDomain();
        Console.WriteLine("\n3. Работа с потоком и простыми числами:");
        WorkWithSingleThread();
        Console.WriteLine("\n4. Два потока (чётные и нечётные числа):");
        WorkWithTwoThreads();
        Console.WriteLine("\n5. Работа с Таймером:");
        WorkWithTimer();
    }

    static void PrintRunningProcesses()
    {
        foreach (var process in Process.GetProcesses())
        {
            try
            {
                Console.WriteLine($"ID: {process.Id}, Имя: {process.ProcessName}, Приоритет: {process.BasePriority}, " +
                                  $"Время запуска: {process.StartTime}, Состояние: {process.Responding}, Время использования ЦП: {process.TotalProcessorTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Невозможно получить доступ к процессу {process.ProcessName}: {ex.Message}");
            }
        }
    }

    static void InspectAppDomain()
    {
        var currentDomain = AppDomain.CurrentDomain;
        Console.WriteLine($"Текущий домен: {currentDomain.FriendlyName}");
        Console.WriteLine($"Корневой каталог: {currentDomain.BaseDirectory}");
        Console.WriteLine("Загруженные сборки:");
        foreach (var assembly in currentDomain.GetAssemblies())
        {
            Console.WriteLine($"- {assembly.FullName}");
        }

        Console.WriteLine("\nСоздание нового AssemblyLoadContext...");
        var newContext = new AssemblyLoadContext("NewContext", isCollectible: true);

        try
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            Console.WriteLine($"Загрузка сборки: {assemblyPath}");
            var assembly = newContext.LoadFromAssemblyPath(assemblyPath);

            Console.WriteLine($"Сборка загружена в новом контексте: {assembly.FullName}");
        }
        finally
        {
            Console.WriteLine("Выгрузка нового AssemblyLoadContext...");
            newContext.Unload();
        }

        Console.WriteLine("Новый AssemblyLoadContext выгружен.");
    }

    static void WorkWithSingleThread()
    {
        Console.Write("Введите n для расчёта простых чисел: ");
        int n;
        if (!int.TryParse(Console.ReadLine(), out n))
        {
            Console.WriteLine("Введено некорректное значение. Попробуйте снова.");
            return;
        }

        Thread thread = new Thread(() =>
        {
            using var writer = new StreamWriter("Primes.txt");
            for (int i = 1; i <= n; i++)
            {
                if (IsPrime(i))
                {
                    Console.WriteLine($"Поток: {Thread.CurrentThread.Name}, ID: {Thread.CurrentThread.ManagedThreadId}, Приоритет: {Thread.CurrentThread.Priority}, Простое число: {i}");
                    writer.WriteLine(i);
                    Thread.Sleep(100);
                }
            }
        })
        {
            Name = "PrimeThread",
            Priority = ThreadPriority.Normal
        };

        thread.Start();

        while (thread.IsAlive)
        {
            Console.WriteLine($"Поток {thread.Name} работает... Статус: {thread.ThreadState}, ID: {thread.ManagedThreadId}, Приоритет: {thread.Priority}");
            if (thread.ThreadState == System.Threading.ThreadState.Running)
            {
                thread.Suspend();
                Console.WriteLine("Поток приостановлен...");
                Thread.Sleep(2000);
                thread.Resume();
                Console.WriteLine("Поток возобновлен...");
            }

            Thread.Sleep(500);
        }

        Console.WriteLine("Поток завершен.");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    static void WorkWithTwoThreads()
    {
        int n = 20;
        var locker = new object();
        bool isEvenTurn = true;

        Thread evenThread = new Thread(() =>
        {
            for (int i = 2; i <= n; i += 2)
            {
                lock (locker)
                {
                    Console.WriteLine($"Чётное: {i}");
                    File.AppendAllText("Numbers.txt", $"Чётное: {i}\n");
                    isEvenTurn = false;
                    Thread.Sleep(200);
                    Monitor.Pulse(locker);
                    Monitor.Wait(locker);
                }
            }
        })
        {
            Name = "EvenThread",
            Priority = ThreadPriority.AboveNormal
        };

        Thread oddThread = new Thread(() =>
        {
            for (int i = 1; i <= n; i += 2)
            {
                lock (locker)
                {
                    while (isEvenTurn)
                    {
                        Monitor.Wait(locker);
                    }
                    Console.WriteLine($"Нечётное: {i}");
                    File.AppendAllText("Numbers.txt", $"Нечётное: {i}\n");
                    isEvenTurn = true;
                    Thread.Sleep(300);
                    Monitor.Pulse(locker);
                }
            }
        })
        {
            Name = "OddThread",
            Priority = ThreadPriority.Normal
        };

        evenThread.Start();
        oddThread.Start();

        evenThread.Join();
        oddThread.Join();
    }

    static void WorkWithTimer()
    {
        int counter = 0;
        Timer timer = null;

        timer = new Timer(state =>
        {
            Console.WriteLine($"Таймер выполнен {++counter} время(с)");

            if (counter == 5)
            {
                timer.Dispose();
            }
        }, null, 0, 1000);
        Thread.Sleep(6000);
    }
}
