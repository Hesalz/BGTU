using DAL_Celebrity;
using Microsoft.EntityFrameworkCore;
namespace DAL_Celebrity_MSSQL
{
    public interface IRepository : DAL_Celebrity.IRepository<Celebrity, Lifeevent> { }

    public class Celebrity
    {
        public Celebrity() { this.FullName = string.Empty; this.Nationality = string.Empty; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string? ReqPhotoPath { get; set; }
    }

    public class Lifeevent
    {
        public Lifeevent() { this.Description = string.Empty; }
        public int Id { get; set; }
        public int CelebrityId { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public string? ReqPhotoPath { get; set; }
    }

    public class Repository : IRepository
    {
        Context context;
        public Repository() { this.context = new Context(); }
        public Repository(string connectionstring) { this.context = new Context(connectionstring); }
        public static IRepository Create() { return new Repository(); }
        public static IRepository Create(string connectionstring) { return new Repository(connectionstring); }
        public List<Celebrity> GetAllCelebrities() { return this.context.Celebrities.ToList<Celebrity>(); }
        public Celebrity? GetCelebrityById(int Id)
        {
           return this.context.Celebrities.FirstOrDefault(c => c.Id == Id);
        }
        public bool AddCelebrity(Celebrity celebrity)
        {
            this.context.Celebrities.Add(celebrity);
            return this.context.SaveChanges() > 0;
        }

        public bool DelCelebrity(int id)
        {
            var cel = this.context.Celebrities.FirstOrDefault(c => c.Id == id);
            if(cel != null)
            {
                this.context.Celebrities.Remove(cel);
                return this.context.SaveChanges() > 0;
            }
            return false;
        }

        public bool? UpdCelebrity(int id, Celebrity celebrity)
        {
            var cel = this.context.Celebrities.FirstOrDefault(c => c.Id == id);
            if (cel != null)
            {
                cel.FullName = celebrity.FullName;
                cel.Nationality = celebrity.Nationality;
                cel.ReqPhotoPath = celebrity.ReqPhotoPath;
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public List<Lifeevent> GetAllLifeevents() { return this.context.Lifeevents.ToList<Lifeevent>(); }
        public Lifeevent? GetLifeeventById(int Id) { 
            return this.context.Lifeevents.FirstOrDefault(c => c.Id == Id);
        }
        public bool AddLifeevent(Lifeevent lifeevent) {
            this.context.Lifeevents.Add(lifeevent);
            return this.context.SaveChanges() > 0;
        }
        public bool DelLifeevent(int id) {
            var l = this.context.Lifeevents.FirstOrDefault(c => c.Id == id);
            if (l != null)
            {
                this.context.Lifeevents.Remove(l);
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public bool UpdLifeevent(int id, Lifeevent lifeevent) {
            var l = this.context.Lifeevents.FirstOrDefault(c => c.Id == id);
            if (l != null)
            {
                l.CelebrityId = lifeevent.CelebrityId;
                l.ReqPhotoPath = lifeevent.ReqPhotoPath;
                l.Description = lifeevent.Description;
                l.Date = lifeevent.Date;
                return this.context.SaveChanges() > 0;
            }
            return false;
        }
        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId) {
            return this.context.Lifeevents.Where(p => p.CelebrityId == celebrityId).ToList();
        }
        public Celebrity? GetCelebrityByLifeeventId(int lifeeventId) {
            var l = this.context.Lifeevents.FirstOrDefault(p => p.Id == lifeeventId);
            return this.context.Celebrities.FirstOrDefault(p => p.Id == l.CelebrityId);
        }
        public int GetCelebrityIdByName(string name) {
            return this.context.Celebrities.FirstOrDefault(p => p.FullName.Contains(name)).Id;
        }
        public void Dispose() { }
    }

    public class Context : DbContext
    {
        public string? ConnectionString { get; private set; } = null;

        public Context(string connstring) : base()
        {
            this.ConnectionString = connstring;
        }
        public Context() : base()
        {

        }

        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<Lifeevent> Lifeevents { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (this.ConnectionString is null) this.ConnectionString = "Data Source=PC;Initial Catalog=Celebrity;Integrated Security=True;Trust Server Certificate=True";
            optionsBuilder.UseSqlServer(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Celebrity>().ToTable("Celebrities").HasKey(p => p.Id);
            modelBuilder.Entity<Celebrity>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<Celebrity>().Property(p => p.FullName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Celebrity>().Property(p => p.Nationality).IsRequired().HasMaxLength(2);
            modelBuilder.Entity<Celebrity>().Property(p => p.ReqPhotoPath).HasMaxLength(200);

            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents").HasKey(p => p.Id);
            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents");
            modelBuilder.Entity<Lifeevent>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents").HasOne<Celebrity>().WithMany().HasForeignKey(p => p.CelebrityId);
            modelBuilder.Entity<Lifeevent>().Property(p => p.CelebrityId).IsRequired();
            modelBuilder.Entity<Lifeevent>().Property(p => p.Date);
            modelBuilder.Entity<Lifeevent>().Property(p => p.Description).HasMaxLength(256);
            modelBuilder.Entity<Lifeevent>().Property(p => p.ReqPhotoPath).HasMaxLength(256);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Init
    {
        static string connstring = "Data Source=PC;Initial Catalog=Celebrity;Integrated Security=True;Trust Server Certificate=True";
        public Init() { }
        public Init(string conn) { connstring = conn; }
        public static void Execute(bool delete = true, bool create = true)
        {
            Context context = new Context(connstring);
            if (delete) context.Database.EnsureDeleted();
            if(create) context.Database.EnsureCreated();

            Func<string, string> puri = (string f) => $"{f}";

            { // 1
                Celebrity c = new Celebrity() { FullName = "Noam Chomsky", Nationality = "US", ReqPhotoPath = puri("Chomsky.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 1, Date = new DateTime(1928, 12, 7), Description = "Дата рождения", ReqPhotoPath = null};
                Lifeevent l2 = new Lifeevent() { CelebrityId = 1, Date = new DateTime(1955, 1, 1), Description = "Издание книги \"Логическая структура лингвистической теории\"", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 2
                Celebrity c = new Celebrity() { FullName = "Tim Berners-Lee", Nationality = "UK", ReqPhotoPath = puri("Berners-Lee.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 2, Date = new DateTime(1955, 6, 8), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 2, Date = new DateTime(1989, 6, 8), Description = "В CERN предложил \"Гиппертекстовый проект\"", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 3
                Celebrity c = new Celebrity() { FullName = "Edgar Codd", Nationality = "US", ReqPhotoPath = puri("Codd.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 3, Date = new DateTime(1923, 8, 23), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 3, Date = new DateTime(2003, 4, 18), Description = "Дата смерти", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 4
                Celebrity c = new Celebrity() { FullName = "Donald Knuth", Nationality = "US", ReqPhotoPath = puri("Knuth.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 4, Date = new DateTime(1938, 1, 10), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 4, Date = new DateTime(1974, 1, 1), Description = "Премия Тьюринга", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 5
                Celebrity c = new Celebrity() { FullName = "Linus Torvalds", Nationality = "US", ReqPhotoPath = puri("Linus.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 5, Date = new DateTime(1969, 12, 28), Description = "Дата рождения. Финляндия.", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 5, Date = new DateTime(1991, 9, 17), Description = "Выложил исходный код OS Linux (версии 0.01)", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 6
                Celebrity c = new Celebrity() { FullName = "John Neumann", Nationality = "US", ReqPhotoPath = puri("Neumann.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 6, Date = new DateTime(1903, 12, 28), Description = "Дата рождения. Венгрия.", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 6, Date = new DateTime(1957, 2, 8), Description = "Дата смерти", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 7
                Celebrity c = new Celebrity() { FullName = "Edsger Dijkstra", Nationality = "NL", ReqPhotoPath = puri("Djkstra.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 7, Date = new DateTime(1930, 12, 28), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 7, Date = new DateTime(2002, 8, 6), Description = "Дата смерти", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 8
                Celebrity c = new Celebrity() { FullName = "Ada Lovelace", Nationality = "UK", ReqPhotoPath = puri("Lovelace.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 8, Date = new DateTime(1815, 12, 10), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 8, Date = new DateTime(1852, 11, 27), Description = "Дата смерти", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 9
                Celebrity c = new Celebrity() { FullName = "Charles Babbage", Nationality = "UK", ReqPhotoPath = puri("Babbage.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 9, Date = new DateTime(1791, 12, 26), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 9, Date = new DateTime(1871, 10, 18), Description = "Дата смерти", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            { // 10
                Celebrity c = new Celebrity() { FullName = "Andrew Tanenbaum", Nationality = "NL", ReqPhotoPath = puri("Tanenbaum.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 10, Date = new DateTime(1944, 3, 16), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent() { CelebrityId = 10, Date = new DateTime(1987, 1, 1), Description = "Создал OS MINIX - бесплатную Unix-подобную систему", ReqPhotoPath = null };
                context.Celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            context.SaveChanges();
        }
    }
}
