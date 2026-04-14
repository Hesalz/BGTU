namespace Lec03LibN
{
    internal class Bonus : IBonus
    {
        public float costOneHour { get; set; }

        public Bonus (float costOneHour)
        {
            this.costOneHour = costOneHour;
        }

        public float Calculate(float hoursOfWork)
        {
            return costOneHour * hoursOfWork;
        }
    }

    public class BonusAL2 : IBonus
    {
        public float costOneHour { get; set; }
        private float A { get; set; }

        public BonusAL2(float costOneHour, float A)
        {
            this.costOneHour = costOneHour;
            this.A = A;
        }

        public float Calculate(float hoursOfWork)
        {
            return (hoursOfWork + A) * costOneHour;
        }
    }

    public class BonusAL3 : IBonus
    {
        private float A { get; set; }
        private float B { get; set; }
        public float costOneHour { get; set; }

        public BonusAL3(float costOneHour, float A, float B)
        {
            this.costOneHour = costOneHour;
            this.A = A;
            this.B = B;
        }

        public float Calculate(float hoursOfWork)
        {
            return (hoursOfWork + A) * (costOneHour + B);
        }
    }

    public class BonusB : IBonus
    {
        private float x { get; set; }
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

    internal class BonusBL2 : IBonus
    {
        private float A;
        private float x;
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
        private float x { get; set; }
        private float A { get; set; }
        private float B { get; set; }

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
    internal class BonusC : IBonus
    {
        public float costOneHour { get; set; }
        private float x;
        private float y;

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

    internal class BonusCL2 : IBonus
    {
        private float A;
        private float x;
        private float y;
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
        private float x { get; set; }
        private float y { get; set; }
        private float A { get; set; }
        private float B { get; set; }

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
