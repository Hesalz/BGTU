using TicketBookingSystem.Models;

namespace TicketBookingSystem.Services.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}


