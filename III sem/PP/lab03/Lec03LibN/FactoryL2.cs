namespace Lec03LibN
{
    public class FactoryL2 : IFactory
    {
        public float A;
        public FactoryL2(float A)
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
}
