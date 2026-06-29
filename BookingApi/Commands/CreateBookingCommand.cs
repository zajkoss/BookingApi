namespace BookingApi.Commands;

public record CreateBookingCommand(Guid ResourceId, DateTime StartDate, DateTime EndDate, string BookingBy);