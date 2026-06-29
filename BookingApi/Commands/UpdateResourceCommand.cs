namespace BookingApi.Commands;
public record UpdateResourceCommand(string Name, string? Description, int Capacity);