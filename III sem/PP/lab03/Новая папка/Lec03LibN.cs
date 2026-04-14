namespace Lec03LibN
{
    public class Lec03LibN
    {
        static public IFactory getL1()
        {
            return new SalaryForAllLvl ();
        }

        static public IFactory getL2(float a)
        {
            return new SalaryForL2 (a);
        }
        static public IFactory getL3(float a, float b)
        {
            return new SalaryForL3 (a, b);
        }
    }
}
