namespace Lec03LibN
{
    internal class SalaryForAllLvl : IFactory
    {
        public IBonus getA (float costOneHour)
        {
            return new Bonus (costOneHour);
        }

        public IBonus getB (float costOneHour, float x)
        {
         return new BonusB (costOneHour, x);
        }

        public IBonus getC (float costOneHour, float x, float y)
        {
            return new BonusC (costOneHour, x, y);
        }
    }

    internal class SalaryForL2 : IFactory
    {
        private readonly float A;
        public SalaryForL2(float A)
        {
            this.A = A;
        }

        public IBonus getA(float costOneHour)
        {
            return new BonusAL2(costOneHour, A);
        }

        public IBonus getB(float costOneHour, float x)
        {
            return new BonusBL2(costOneHour, x, A);
        }

        public IBonus getC(float costOneHour, float x, float y)
        {
            return new BonusCL2(costOneHour, x, y, A);
        }
    }
    public class SalaryForL3 : IFactory
    {
        private float A { get; set; }
        private float B { get; set; }
        public SalaryForL3(float A, float B)
        {
            this.A = A;
            this.B = B;
        }

        public IBonus getA(float costOneHour)
        {
            return new BonusAL3(costOneHour, A, B);
        }

        public IBonus getB(float costOneHour, float x)
        {
            return new BonusBL3(costOneHour, x, A, B);
        }

        public IBonus getC(float costOneHour, float x, float y)
        {
            return new BonusCL3(costOneHour, x, y, A, B);
        }
    }
}
