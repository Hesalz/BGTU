using DAL_LES;
namespace CashCelebrities;

public interface ICashCelebrities
{
    List<Celebrity> GetCelebrities();
    void PrintCashCelebrities();
}