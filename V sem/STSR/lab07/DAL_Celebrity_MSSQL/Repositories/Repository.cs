
using DAL_Celebrity_MSSQL.Data;
using DAL_Celebrity_MSSQL.Interfaces;
using DAL_Celebrity_MSSQL.Models;
using Microsoft.EntityFrameworkCore;


namespace DAL_Celebrity_MSSQL.Repositories
{
    public class Repository : IRepository
    {

        readonly Context context;

        private readonly DbSet<Celebrity> _celebrities;
        private readonly DbSet<Lifeevent> _lifeevents;
        public Repository()
        {
            context = new Context();
            _celebrities = context.Set<Celebrity>();
            _lifeevents = context.Set<Lifeevent>();
        }

        public Repository(string connectionString)
        {
            context = new Context(connectionString);
            _celebrities = context.Set<Celebrity>();
            _lifeevents = context.Set<Lifeevent>();
        }

        public static IRepository Create()
        {
            return new Repository();
        }

        public static IRepository Create(string connectionString)
        {
            return new Repository(connectionString);
        }
        public bool AddCelebrity(Celebrity celebrity)
        {
            _celebrities.Add(celebrity);
            context.SaveChanges();
            return true;
            
        }

        public bool AddLifeevent(Lifeevent lifeevent)
        {
            _lifeevents.Add(lifeevent);
            context.SaveChanges();
            return true;
        }

        public bool DelCelebrity(int id)
        {
            var celebrity = _celebrities.Find(id);
            if (celebrity != null)
            {
                _celebrities.Attach(celebrity);
                _celebrities.Remove(celebrity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DelLifeevent(int id)
        {
            var lifeevent = _lifeevents.Find(id);
            if(lifeevent != null)
            {
                _lifeevents.Attach(lifeevent);
                _lifeevents.Remove(lifeevent);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Celebrity> GetAllCelebrities()
        {
            return _celebrities.ToList();
        }

        public List<Lifeevent> GetAllLifeevents()
        {
            return _lifeevents.ToList();
        }

        public Celebrity? GetCelebrityById(int Id)
        {
            return _celebrities.FirstOrDefault(c=> c.Id == Id);
        }

        public Celebrity? GetCelebrityByLifeeventId(int lifeeventId)
        {
            var id = _lifeevents.FirstOrDefault(l => l.Id == lifeeventId).CelebrityId;
            return _celebrities.FirstOrDefault(c => c.Id == id);
        }

        public int GetCelebrityIdByName(string name)
        {
            var celebrity_id = _celebrities.FirstOrDefault(c => c.FullName.Contains(name)).Id;
            if(celebrity_id != null)
            {
                return celebrity_id;
            }
            return 0;
        }

        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId)
        {
            var lifeevents = _lifeevents.Where(l => l.CelebrityId == celebrityId).ToList();
            return lifeevents;
        }

        public Lifeevent? GetLifeeventById(int Id)
        {
            return _lifeevents.FirstOrDefault(l => l.Id == Id);
        }

        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            var newCelebrity = _celebrities.FirstOrDefault(c => c.Id == id);
            if (newCelebrity != null)
            {
                newCelebrity.Update(celebrity);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdLifeevent(int id, Lifeevent lifeevent)
        {
            var newLifeevent = _lifeevents.FirstOrDefault(c => c.Id == id);
            if (newLifeevent != null)
            {
                newLifeevent.Update(lifeevent);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void Dispose()
        {

        }
    }
}
