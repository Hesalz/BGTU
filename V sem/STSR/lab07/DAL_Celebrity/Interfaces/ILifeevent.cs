
namespace DAL_Celebrity.Interfaces
{
    public interface ILifeevent<T> : IDisposable
    {
        List<T> GetAllLifeevents();                       // получить все События 
        T? GetLifeeventById(int Id);                       // получить Событие по Id 
        bool DelLifeevent(int id);                        // удалить Событие  по Id 
        bool AddLifeevent(T lifeevent);                   // добавить Событие  
        bool UpdLifeevent(int id, T lifeevent);           // изменить обытие по  Id  
    }
}
