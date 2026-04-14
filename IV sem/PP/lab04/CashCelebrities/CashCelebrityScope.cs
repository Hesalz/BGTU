using DAL_LES;
namespace CashCelebrities;

public class CashCelebrityScope : ICashCelebrities
{
    private DateTime lastRefresh;
    private ICelebrity repo;
    private TimeSpan durationCache;
    private List<Celebrity> celebritiesCache;

    public CashCelebrityScope(TimeSpan duration, ICelebrity repo)
    {
        lastRefresh = DateTime.MinValue;
        this.repo = repo;
        durationCache = duration;
        celebritiesCache = new List<Celebrity>();
    }

    public List<Celebrity> GetCelebrities()
    {
        if(DateTime.Now - lastRefresh >= durationCache)
        {
            celebritiesCache = repo.GetAllCelebrities();
            lastRefresh = DateTime.Now;
        }
        return celebritiesCache ?? new List<Celebrity>();
    }

    public void PrintCashCelebrities()
    {
        Console.WriteLine($"Time of cache: {DateTime.Now}");
        Console.WriteLine($"Cached celbrities:\n");
        List<Celebrity> celebs = celebritiesCache.ToList();
        foreach(var c in celebs)
        {
            Console.WriteLine($"Celebrity {c.Id}: {c.FullName}, {c.Nationality}");
        }

    }
}