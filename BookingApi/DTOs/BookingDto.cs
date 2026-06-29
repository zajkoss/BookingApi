namespace BookingApi.Commands;

public record BookingDto(Guid Id, Guid ResourceId, DateTime StartDate, DateTime EndDate, DateTime CreatedAt, string BookingBy);