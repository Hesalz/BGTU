using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Repositories;

namespace TicketBookingSystem.Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName, string? phoneNumber)
    {
        if (await _userRepository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email already exists");

        var user = new User
        {
            Email = email,
            PasswordHash = HashPassword(password),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Role = UserRole.Client,
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.AddAsync(user);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        if (!VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}


