
namespace DAL_Celebrity.Interfaces
{
    public interface IMix<T1, T2>
    {
        List<T2> GetLifeeventsByCelebrityId(int celebrityId);   // получить все События по Id Знаменитости
        T1? GetCelebrityByLifeeventId(int lifeeventId);         // получить Знаменитость по Id События
    }
}
