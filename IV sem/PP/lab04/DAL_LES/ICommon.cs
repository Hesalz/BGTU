namespace DAL_LES;

public interface ICommon<T1, T2> : IDisposable
{
    List<T2> GetLifeEventsByCelebrityId(int celebrityId);
    T1 GetCelebrityByLifeEventId(int lifeEventId);
}