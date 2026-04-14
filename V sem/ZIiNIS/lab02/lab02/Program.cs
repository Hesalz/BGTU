using System;
using System.Text;

namespace laba2
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- Пути к файлам с текстами ---
            string italianFile = "latin.txt";
            string mongolianFile = "cyrillic.txt";
            string binaryFile = "binary.txt";     

            string italianAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string mongolianAlphabet = "АБВГДЕЁЖЗИЙКЛМНООӨПРССТУУҮФХЦЧШЩЪЫЬЭЮЯ";
            string binaryAlphabet = "01";

            // --- Частоты букв ---
            Console.WriteLine("Частоты букв итальянского алфавита:");
            Entropy.PrintLetterFrequencies(italianFile, italianAlphabet);
            Console.WriteLine();

            Console.WriteLine("Частоты букв монгольского алфавита:");
            Entropy.PrintLetterFrequencies(mongolianFile, mongolianAlphabet);
            Console.WriteLine();

            // --- Энтропия ---
            double italianEntropy = Entropy.ComputeEntropyFromFile(italianFile, italianAlphabet);
            double mongolianEntropy = Entropy.ComputeEntropyFromFile(mongolianFile, mongolianAlphabet);
            double binaryEntropy = Entropy.ComputeEntropyFromFile(binaryFile, binaryAlphabet);

            Console.WriteLine($"Энтропия итальянского алфавита: {italianEntropy:F4}");
            Console.WriteLine($"Энтропия монгольского алфавита: {mongolianEntropy:F4}");
            Console.WriteLine($"Энтропия бинарного алфавита: {binaryEntropy:F4}");
            Console.WriteLine();

            // --- Количество информации ФИО ---
            string fio = "Babashinskiy Gleb Alexandrovich";
            string fioMongolian = "Бабашинский Глеб Александрович";

            double infoItalian = italianEntropy * fio.Length;
            double infoMongolian = mongolianEntropy * fioMongolian.Length;
            int asciiBits = Encoding.ASCII.GetBytes(fio).Length * 8;

            Console.WriteLine($"Количество информации ФИО (итальянский): {infoItalian:F4} бит");
            Console.WriteLine($"Количество информации ФИО (монгольский): {infoMongolian:F4} бит");
            Console.WriteLine($"Количество информации ФИО (ASCII): {asciiBits} бит");
            Console.WriteLine();

            // --- Количество информации с ошибками ---
            double[] errors = { 0.1, 0.5, 1.0 };
            foreach (double err in errors)
            {
                double info = Entropy.ChannelCapacity(err) * asciiBits;
                Console.WriteLine($"Количество информации (ASCII, Ошибочная передача единичного бита={err}): {info:F4} бит");
            }

            Console.WriteLine("\n--- Конец программы ---");
            Console.ReadKey();
        }
    }
}
