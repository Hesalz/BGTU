using System;
using System.Collections.Generic;

namespace lab05
{
    partial class Army
    {
        private List<SentientBeing> units = new List<SentientBeing>();

        public void AddUnit(SentientBeing unit)
        {
            if (unit != null)
            {
                units.Add(unit);
                Console.WriteLine($"{unit.Name} добавлен в армию.");
            }
            else
            {
                Console.WriteLine("Ошибка: Боец не может быть пустым.");
            }
        }

        public void RemoveUnit(SentientBeing unit)
        {
            if (unit != null && units.Contains(unit))
            {
                units.Remove(unit);
                Console.WriteLine($"{unit.Name} удален из армии.");
            }
            else
            {
                Console.WriteLine("Не найдено.");
            }
        }

        public SentientBeing GetUnit(int index)
        {
            if (index >= 0 && index < units.Count)
                return units[index];
            return null;
        }

        public void DisplayUnits()
        {
            Console.WriteLine("Армия:");
            foreach (var unit in units)
            {
                Console.WriteLine(unit.ToString());
            }
        }

        public List<SentientBeing> Units => units;
    }
}
