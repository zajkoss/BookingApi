using BookingApi.Commands;

namespace BookingApi.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync();
    Task<BookingDto?> GetByIdAsync(Guid id);
    Task<BookingDto> CreateAsync(CreateBookingCommand command);
    Task DeleteAsync(Guid id);
}