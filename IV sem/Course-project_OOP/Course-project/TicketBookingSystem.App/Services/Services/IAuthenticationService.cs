using TicketBookingSystem.Models;

namespace TicketBookingSystem.Services.Services;

public interface IAuthenticationService
{
    Task<User?> LoginAsync(string email, string password);
    Task<User> RegisterAsync(string email, string password, string firstName, string lastName, string? phoneNumber);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}


