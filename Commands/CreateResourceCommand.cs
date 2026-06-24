namespace BookingApi.Commands;
public record CreateResourceCommand(string Name, string? Description, int Capacity);