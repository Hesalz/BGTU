namespace Lec03LibN
{
    public class FactoryL1 : IFactory
    {
        public IBonus getA (float costOneHour)
        {
            return new BonusA (costOneHour);
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
}
