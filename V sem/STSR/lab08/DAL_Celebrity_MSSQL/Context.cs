using DAL_Celebrity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Celebrity_MSSQL
{
    public class Context : DbContext
    {
        public string? ConnectionString { get; private set; } = null;
        public Context(string connString) : base()
        {
            ConnectionString = connString;
        }

        public Context() : base()
        {

        }
        public DbSet<Celebrity> Celebrities { get; set; }
        public DbSet<LifeEvent> LifeEvents { get; set; }


        // для настройки параметров подключения к базе данных.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            if (this.ConnectionString is null)
            {
                this.ConnectionString = "Data Source=PC;Initial Catalog=LES01;Integrated Security=True;Trust Server Certificate=True";
            }

            optionsBuilder.UseSqlServer(this.ConnectionString);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Celebrity>().ToTable("Celebrities").HasKey(p => p.Id);
            modelBuilder.Entity<Celebrity>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<Celebrity>().Property(p => p.FullName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Celebrity>().Property(p => p.Nationality).IsRequired();
            modelBuilder.Entity<Celebrity>().Property(p => p.ReqPhotoPath).HasMaxLength(200);

            modelBuilder.Entity<LifeEvent>().ToTable("LifeEvents").HasKey(p => p.Id);
            modelBuilder.Entity<LifeEvent>().ToTable("LifeEvents");
            modelBuilder.Entity<LifeEvent>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<LifeEvent>().ToTable("LifeEvents").HasOne<Celebrity>().WithMany().HasForeignKey(p => p.CelebrityId);      
            modelBuilder.Entity<LifeEvent>().Property(p => p.CelebrityId).IsRequired();
            modelBuilder.Entity<LifeEvent>().Property(p => p.Date);
            modelBuilder.Entity<LifeEvent>().Property(p => p.Description).HasMaxLength(256);
            modelBuilder.Entity<LifeEvent>().Property(p => p.ReqPhotoPath).HasMaxLength(256);


            base.OnModelCreating(modelBuilder);
        }

    }
}
