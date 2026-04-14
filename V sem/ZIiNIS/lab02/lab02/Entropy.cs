using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace laba2
{
    public static class Entropy
    {
        // Подсчёт частот букв в файле
        public static void PrintLetterFrequencies(string filePath, string alphabet)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {filePath} не найден!");
                return;
            }

            Dictionary<char, int> counts = new Dictionary<char, int>();
            foreach (var ch in alphabet)
                counts[ch] = 0;

            string text = File.ReadAllText(filePath).ToUpper();
            int total = 0;
            foreach (var ch in text)
            {
                if (counts.ContainsKey(ch))
                {
                    counts[ch]++;
                    total++;
                }
            }

            Console.WriteLine("Буква\tКоличество\tЧастота (%)");
            foreach (var kvp in counts)
            {
                double freq = total > 0 ? (double)kvp.Value / total * 100 : 0;
                Console.WriteLine($"{kvp.Key}\t{kvp.Value}\t\t{freq:F2}");
            }
            Console.WriteLine($"Всего букв: {total}");
        }

        // Энтропия текста в файле
        public static double ComputeEntropyFromFile(string filePath, string alphabet)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл {filePath} не найден!");
                return 0;
            }

            Dictionary<char, int> counts = new Dictionary<char, int>();
            foreach (var ch in alphabet)
                counts[ch] = 0;

            string text = File.ReadAllText(filePath).ToUpper();
            int total = 0;
            foreach (var ch in text)
            {
                if (counts.ContainsKey(ch))
                {
                    counts[ch]++;
                    total++;
                }
            }

            double entropy = 0;
            foreach (var kvp in counts)
            {
                if (kvp.Value > 0)
                {
                    double p = (double)kvp.Value / total;
                    entropy += -p * Math.Log2(p);
                }
            }
            return entropy;
        }

        // Емкость канала с вероятностью ошибки
        public static double ChannelCapacity(double errorProbability)
        {
            if (errorProbability <= 0) return 1;       // без ошибок
            if (errorProbability >= 1) return 1;       // полностью инвертированный канал
            double q = errorProbability;
            double hq = -q * Math.Log2(q) - (1 - q) * Math.Log2(1 - q);
            return 1 - hq;
        }
    }
}
