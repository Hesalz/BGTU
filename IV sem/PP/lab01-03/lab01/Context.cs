using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class Context : DbContext
    {
        public Context() : base() 
        {
            Database.EnsureCreated();
        }
        public DbSet<WSRef> wSRefs { get; set; }
        public DbSet<Comment>? comments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=PC; Initial Catalog=SSSS; TrustServerCertificate=True; Integrated Security=True;");

        }
    }
}
