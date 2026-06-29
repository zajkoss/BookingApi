namespace BookingApi.Commands;

public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName);
