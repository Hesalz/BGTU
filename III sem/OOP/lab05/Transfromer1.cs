using System;

namespace lab05
{
    partial class Transformer : Human, IDriveable
    {
        public string Model { get; set; }
        public YearOfCreation Year { get; set; }
        public Specifications Specs { get; set; }

        public Transformer(string name, string model, YearOfCreation year, Specifications specs) : base(name)
        {
            Model = model;
            Year = year;
            Specs = specs;
        }

        public void Drive()
        {
            Console.WriteLine($"{Name} (Трансформер {Model} за рулем");
        }

        public bool DoClone()
        {
            return true;
        }
    }
}
