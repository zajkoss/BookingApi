using BookingApi.Commands;
using BookingApi.Repository;

namespace BookingApi.Services;

public class BookingService : IBookingService
{
    private const string CACHE_KEY_ALL = "booking:all";
    
    private readonly IBookingRepository _bookingRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<BookingService> _logger;
    

    public BookingService(IBookingRepository bookingRepository, ICacheService cache)
    {
        _bookingRepository = bookingRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all bookings");

        var cached = await _cache.GetAsync<List<BookingDto>>(CACHE_KEY_ALL);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit for all bookings");
            return cached;
        }
        var bookings = await _bookingRepository.GetAllAsync();
        var dtos = bookings.Select(b => new BookingDto(b.Id, b.ResourceId, b.StartDate, b.EndDate, b.CreatedAt,b.BookedBy));
        _logger.LogInformation("Fetched {Count} bookings from database", dtos.Count()); // ze structured logging
        await _cache.SetAsync(CACHE_KEY_ALL, dtos);
        return dtos;
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<BookingDto> CreateAsync(CreateBookingCommand command)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}