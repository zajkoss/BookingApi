using BookingApi.Commands;
using BookingApi.DTOs;

namespace BookingApi.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterUserCommand command);
    Task<string> LoginAsync(LoginUserCommand command);
}