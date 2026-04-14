using Microsoft.EntityFrameworkCore;

namespace DAL_LES;

public interface IRepository : ICommon, ICelebrity, ILifeEvent
{
    Guid Id { get; }
}

public interface ICommon : ICommon<Celebrity, LifeEvent> { }
public interface ICelebrity : ICelebrity<Celebrity> { }
public interface ILifeEvent : ILifeEvent<LifeEvent> { }


public class Repository : IRepository
{
    private readonly Context context;
    public Guid Id { get; } = Guid.NewGuid();

    private Repository(Context context)
    {
        context = new Context();
        this.context = context;
    }

    public static Repository Create()
    {
        var context = new Context();
        try
        {
            if (!context.Database.CanConnect())
            {
                context.Database.Migrate();
            }
            return new Repository(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании/подключении БД: {ex.Message}");
            throw;
        }
    }

    public bool AddCelebrity(Celebrity celebrity)
    {
        try
        {
            context.Add(celebrity);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AddCelebrity: {ex.Message}");
            return false;
        }
    }

    public bool AddLifeEvent(LifeEvent lifeEvent)
    {
        try
        {
            context.Add(lifeEvent);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in AddLifeEvent: {ex.Message}");
            return false;
        }
    }

    public bool DelCelebrity(int id)
    {
        try
        {
            Celebrity celebrity = context.Set<Celebrity>().Find(id) ?? new Celebrity();
            context.Remove(celebrity);
            context.SaveChanges();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception in DelCelebrity: {ex.Message}");
            return false;
        }
    }

    public bool DelLifeEvent(int id)
    {
        try
        {
            LifeEvent lifeEvent = context.Set<LifeEvent>().Find(id) ?? new LifeEvent();
            context.Remove(lifeEvent);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in DelCelebrity: {ex.Message}");
            return false;
        }
    }
    

    public List<Celebrity> GetAllCelebrities()
    {
        return context.Celebrities.ToList();
    }

    public List<LifeEvent> GetAllLifeEvents()
    {
        return context.LifeEvents.ToList();
    }

    public Celebrity? GetCelebrityById(int id)
    {
        return context.Celebrities.FirstOrDefault(x => x.Id == id);
    }

    public Celebrity GetCelebrityByLifeEventId(int lifeEventId)
    {
        LifeEvent? lifeEvent = context.LifeEvents.FirstOrDefault(le => le.Id == lifeEventId);
        if (lifeEvent == null) return new Celebrity();

        return context.Celebrities.FirstOrDefault(c => c.Id == lifeEvent.CelebrityId) ?? new Celebrity();
    }

    public LifeEvent? GetLifeEventById(int id)
    {
        return context.LifeEvents.FirstOrDefault(x => x.Id == id);
    }

    public List<LifeEvent> GetLifeEventsByCelebrityId(int celebrityId)
    {
        return context.LifeEvents.Where(x => x.CelebrityId == celebrityId).ToList();
    }

    public bool UpdCelebrity(int id, Celebrity celebrity)
    {
        if (!context.Celebrities.Any(c => c.Id == id))
            return false;

        context.Celebrities.Update(celebrity);
        return context.SaveChanges() > 0;
    }

    public bool UpdLifeEvent(int id, LifeEvent lifeEvent)
    {
        if (!context.LifeEvents.Any(le => le.Id == id))
            return false;

        context.LifeEvents.Update(lifeEvent);
        return context.SaveChanges() > 0;
    }

    public void Dispose()
    {
        context.Dispose();
    }
}