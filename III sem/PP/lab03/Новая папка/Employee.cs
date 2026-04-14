using Lec03LibN;

namespace PP03
{
    internal class Employee
    {
        public IBonus bonus { get; private set; }
        public Employee(IBonus bonus) {

            this.bonus = bonus;
        }

        public float calcBonus (float numberHours)
        {
            return bonus.Calculate(numberHours);
        }
    }
}
