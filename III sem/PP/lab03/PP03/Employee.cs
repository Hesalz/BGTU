using Lec03LibN;

namespace PP03
{
    public class Employee
    {
        public IBonus bonus { get; private set; }
        public Employee(IBonus bonus)
        {
            this.bonus = bonus;
        }

        public float calcBonus (float numberHours)
        {
            return bonus.Calculate(numberHours);
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 1eb568908d67eee9be534943dc44ad717487737c
