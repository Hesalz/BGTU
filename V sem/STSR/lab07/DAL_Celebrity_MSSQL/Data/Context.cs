using DAL_Celebrity_MSSQL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL_Celebrity_MSSQL.Data
{
    public class Context:DbContext
    {
        public string? ConnectionString { get; private set; } = null;

        public Context(string connectionString):base()
        {
            this.ConnectionString = connectionString;
        }

        public Context() : base()
        {   }

        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<Lifeevent> Lifeevents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = this.ConnectionString 
                    ?? "Data Source=PC;Initial Catalog=Celebrity;Integrated Security=true;TrustServerCertificate=true;";
                
                optionsBuilder.UseSqlServer(
                    connectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Celebrity>()
                .ToTable("Celebrities")
                .HasKey(c => c.Id);
            modelBuilder.Entity<Celebrity>()
                .Property(c => c.Id).IsRequired();
            modelBuilder.Entity<Celebrity>()
                .Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<Celebrity>()
                .Property(c => c.Nationality)
                .IsRequired()
                .HasMaxLength(2);
            modelBuilder.Entity<Celebrity>()
                .Property(c => c.ReqPhotoPath)
                .HasMaxLength(200);

            modelBuilder.Entity<Lifeevent>()
                .ToTable("Lifeevents")
                .HasKey(l => l.Id);
            modelBuilder.Entity<Lifeevent>()
                .Property(l => l.Id)
                .IsRequired();
            modelBuilder.Entity<Lifeevent>()
                .Property(l => l.CelebrityId)
                .IsRequired();
            modelBuilder.Entity<Lifeevent>()
                .Property(l => l.Date);
            modelBuilder.Entity<Lifeevent>()
                .Property (l => l.Description)
                .HasMaxLength(256);
            modelBuilder.Entity<Lifeevent>()
                .Property(l => l.ReqPhotoPath)
                .HasMaxLength(256);
            modelBuilder.Entity<Lifeevent>()
                .HasOne<Celebrity>()
                .WithMany()
                .HasForeignKey(l => l.CelebrityId);

            base.OnModelCreating(modelBuilder);
        }

    }
}