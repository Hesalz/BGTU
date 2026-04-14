namespace Lec03LibN
{
    public interface IFactory
    {
        IBonus getA(float costOneHour);
        IBonus getB(float costOneHour, float x);
        IBonus getC(float costOneHour, float x, float y);
    }
}
