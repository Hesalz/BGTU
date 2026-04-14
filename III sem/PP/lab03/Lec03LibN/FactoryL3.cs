namespace Lec03LibN
{
    public class FactoryL3 : IFactory
    {
        public float A { get; set; }
        public float B { get; set; }
        public FactoryL3(float A, float B)
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
