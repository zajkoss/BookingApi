namespace BookingApi.DTOs;

public record UserDto(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt, bool IsActive);