namespace Lec03LibN
{
    public class BonusB : IBonus
    {
        public float x { get; set; }
        public float costOneHour { get; set; }

        public BonusB(float costOneHour, float x)
        {
            this.x = x;
            this.costOneHour = costOneHour;
        }

        public float Calculate(float hoursOfWork)
        {
            return costOneHour * hoursOfWork * x;
        }
    }

    public class BonusBL2 : IBonus
    {
        public float A;
        public float x;
        public float costOneHour { get; set; }

        public BonusBL2(float costOneHour, float x, float A)
        {
            this.costOneHour = costOneHour;
            this.A = A;
            this.x = x;
        }

        public float Calculate(float hoursOfWork)
        {
            return costOneHour * (hoursOfWork + A) * x;
        }
    }

    public class BonusBL3 : IBonus
    {
        public float costOneHour { get; set; }
        public float x { get; set; }
        public float A { get; set; }
        public float B { get; set; }

        public BonusBL3(float costOneHour, float x, float A, float B)
        {
            this.costOneHour = costOneHour;
            this.x = x;
            this.A = A;
            this.B = B;
        }

        public float Calculate(float hoursOfWork)
        {
            return (hoursOfWork + A) * (costOneHour + B) * x;
        }
    }
}
