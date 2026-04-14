namespace Lec03LibN
{
    public class BonusC : IBonus
    {
        public float costOneHour { get; set; }
        public float x;
        public float y;

        public BonusC(float costOneHour, float x, float y)
        {
            this.costOneHour = costOneHour;
            this.x = x;
            this.y = y;
        }

        public float Calculate(float hoursOfWork)
        {
            return costOneHour * hoursOfWork * x + y;
        }
    }

    public class BonusCL2 : IBonus
    {
        public float A;
        public float x;
        public float y;
        public float costOneHour { get; set; }

        public BonusCL2(float costOneHour, float x, float y, float A)
        {
            this.costOneHour = costOneHour;
            this.A = A;
            this.x = x;
            this.y = y;
        }

        public float Calculate(float hoursOfWork)
        {
            return (hoursOfWork + A) * costOneHour * x + y;
        }
    }

    public class BonusCL3 : IBonus
    {
        public float costOneHour { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float A { get; set; }
        public float B { get; set; }

        public BonusCL3(float costOneHour, float x, float y, float A, float B)
        {
            this.costOneHour = costOneHour;
            this.x = x;
            this.y = y;
            this.A = A;
            this.B = B;
        }

        public float Calculate(float hoursOfWork)
        {
            return (hoursOfWork + A) * (costOneHour + B) * x + y;
        }
    }
}
