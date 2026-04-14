using Microsoft.EntityFrameworkCore;

namespace DAL_LES
{
    public class Context : DbContext
    {
        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<LifeEvent> LifeEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=PC; Initial Catalog=DAL_LES; TrustServerCertificate=True; Integrated Security=True",
                options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null)
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LifeEvent>()
                .HasOne(le => le.Celebrity)
                .WithMany(c => c.Lifeevents)
                .HasForeignKey(le => le.CelebrityId);
        }
    }
}