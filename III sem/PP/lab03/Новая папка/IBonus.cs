namespace Lec03LibN
{
    public interface IBonus
    {
        float costOneHour { get; set; }
        float Calculate(float hoursOfWork);

    }
}
