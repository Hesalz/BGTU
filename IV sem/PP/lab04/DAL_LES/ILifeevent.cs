namespace DAL_LES;

public interface ILifeEvent<T> : IDisposable
{
    List<T> GetAllLifeEvents();
    T? GetLifeEventById(int Id);
    bool DelLifeEvent(int id);
    bool AddLifeEvent(T lifeEvent);
    bool UpdLifeEvent(int id, T lifeEvent);
}