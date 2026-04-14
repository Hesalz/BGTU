using System;
using System.Linq;

namespace lab3 {
    public static class StatisticOperations {
        public static int Sum(Set set) {
            return set.elements.Sum();
        }

        public static int DifferenceBetweenMaxAndMin(Set set) {
            return set.elements.Max() - set.elements.Min();
        }

        public static int CountElements(Set set) {
            return set.elements.Count;
        }

        public static string AddOccupied(this string str) {
            var words = str.Split(' ');
            return string.Join(" ", words.Select(word => word + " "));
        }

        public static void RemoveDuplicates(this Set set) {
            set.elements = set.elements.Distinct().ToList();
        }
    }
}
