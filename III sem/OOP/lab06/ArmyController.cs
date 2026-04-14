using System;
using System.Linq;

namespace lab05
{
    partial class ArmyController
    {
        private Army army;

        public ArmyController(Army army)
        {
            this.army = army;
        }

        public SentientBeing FindByCreationYear(YearOfCreation year)
        {
            return army.Units.OfType<Transformer>().FirstOrDefault(t => t.Year == year);
        }

        public void DisplayTransformersByPower(int powerLevel)
        {
            var transformers = army.Units.OfType<Transformer>().Where(t => t.Specs.PowerLevel == powerLevel);

            Console.WriteLine($"Трансформеры с мощностью {powerLevel}:");
            foreach (var transformer in transformers)
            {
                Console.WriteLine(transformer.Name);
            }
        }

        public int CountUnits()
        {
            return army.Units.Count;
        }
    }
}
