using Lab4_5.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

namespace Lab4_5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AppDbContext")
        {
            Database.SetInitializer<AppDbContext>(null);
        }

        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<CinemaImage> CinemaImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Cinema>()
                .HasMany(c => c.Images)
                .WithRequired(i => i.Cinema)
                .HasForeignKey(i => i.CinemaId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cinema>()
                .HasRequired(c => c.Category)
                .WithMany(c => c.Cinemas)
                .HasForeignKey(c => c.CategoryId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId)
                .Property(c => c.CategoryId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<User>()
                .Property(u => u.Theme)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}