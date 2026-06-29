using BookingApi.Models;

namespace BookingApi.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}