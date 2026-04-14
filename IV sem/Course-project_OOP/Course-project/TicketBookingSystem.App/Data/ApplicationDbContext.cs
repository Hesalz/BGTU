using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<EventPhoto> EventPhotos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email");
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("PasswordHash");
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("FirstName");
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("LastName");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("PhoneNumber");
            entity.Property(e => e.Role)
                .HasConversion<int>()
                .HasColumnName("Role");
            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200).HasColumnName("Title");
            entity.Property(e => e.Description).HasMaxLength(2000).HasColumnName("Description");
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Time).HasColumnName("Time");
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200).HasColumnName("Location");
            entity.Property(e => e.TotalTickets).HasColumnName("TotalTickets");
            entity.Property(e => e.Price).HasPrecision(18, 2).HasColumnName("Price");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            entity.Property(e => e.Categories).HasConversion<int>().HasColumnName("Categories");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventId).HasColumnName("EventId");
            entity.Property(e => e.BookingId).HasColumnName("BookingId");
            entity.Property(e => e.Status).HasConversion<int>().HasColumnName("Status");
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            entity.HasOne(e => e.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EventPhoto>(entity =>
        {
            entity.ToTable("EventPhotos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventId).HasColumnName("EventId");
            entity.Property(e => e.ImageData)
                .HasColumnType("varbinary(max)")
                .HasColumnName("ImageData");
            entity.Property(e => e.SortOrder).HasColumnName("SortOrder");
            entity.HasOne(e => e.Event)
                .WithMany(e => e.Photos)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.EventId).HasColumnName("EventId");
            entity.Property(e => e.TicketCount).HasColumnName("TicketCount");
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2).HasColumnName("TotalAmount");
            entity.Property(e => e.Status).HasConversion<int>().HasColumnName("Status");
            entity.Property(e => e.BookingDate).HasColumnName("BookingDate");
            entity.Property(e => e.ConfirmedAt).HasColumnName("ConfirmedAt");
            entity.Property(e => e.CancelledAt).HasColumnName("CancelledAt");
            entity.HasOne(e => e.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminUser = new User
        {
            Id = 1,
            Email = "admin@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<User>().HasData(adminUser);

        var defaultUser = new User
        {
            Id = 2,
            Email = "user@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            FirstName = "User",
            LastName = "Default",
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<User>().HasData(defaultUser);
    }
}


