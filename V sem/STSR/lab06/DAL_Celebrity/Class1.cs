namespace DAL_Celebrity
{
    public interface IRepository<T1, T2> : IMix<T1, T2>, ICelebrity<T1>, ILifeevent<T2> { }

    public interface IMix<T1, T2>
    {
        List<T2> GetLifeeventsByCelebrityId(int celebrityId);
        T1? GetCelebrityByLifeeventId(int lifeeventId);
    }
    
    public interface ICelebrity<T>: IDisposable
    {
        List<T> GetAllCelebrities();
        T? GetCelebrityById(int Id);
        bool DelCelebrity(int id);
        bool AddCelebrity(T celebrity);
        bool? UpdCelebrity(int id, T celebrity);
        int GetCelebrityIdByName(string name);
    }

    public interface ILifeevent<T>: IDisposable
    {
        List<T> GetAllLifeevents();
        T? GetLifeeventById(int Id);
        bool DelLifeevent(int id);
        bool AddLifeevent(T lifeevent);
        bool UpdLifeevent(int id, T lifeevent);
    }
}
