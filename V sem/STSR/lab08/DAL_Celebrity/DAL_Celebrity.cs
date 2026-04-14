using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Celebrity
{
    public interface IMix<T1, T2>
    {
        List<T2> GetLifeEventsByCelebrityId(int celebrityId); // получить  все события по Id знаменитости
        T1? GetCelebrityByLifeEventId(int lifeEventId);       // получить знаменитость по Id события 
    }

    public interface ICelebrity<T1> : IDisposable
    {
        List<T1> GetAllCelebrities();                   // получить все Знаменитости 
        T1? GetCelebrityById(int id);                   // получить Знаменитости по Id
        bool DeleteCelebrity(int id);                   // удалить Знаменитость по Id
        bool AddCelebrity(T1 celebrity);                // добавить Знаменитость
        bool UpdateCelebrity(int id, T1 celebrity);     // изменить Знаменитость по Id
        int GetCelebrityByName(string name);            // получить первый Id по вхождению подстроки
    }

    public interface ILifeEvent<T2>
    {
        List<T2> GetAllLifeEvents();                    // получить все события
        T2? GetLifeEventById(int lifeEventId);          // получить события по Id
        bool DeleteLifeEvent(int lifeEventId);          // удалить событие по Id
        bool AddLifeEvent(T2 lifeEvent);                // добавить  событие 
        bool UpdateLifeEvent(int id, T2 lifeEvent);     // изменить событие  по Id

    }
    // Универсальный интерфейс, объединяющий IMix, ICelebrity и ILifeEvent
    public interface IRepository<T1, T2> : IMix<T1, T2>, ICelebrity<T1>, ILifeEvent<T2>
    {

    }
}
