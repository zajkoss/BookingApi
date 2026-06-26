using BookingApi.Commands;
using BookingApi.Data;
using BookingApi.Models;
using BookingApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Services;

public class BookingService : IBookingService
{
    private const string CACHE_KEY_ALL = "booking:all";
    private const string CACHE_KEY_ID = "booking:";
    private readonly AppDbContext _dbContext;
    private readonly IBookingRepository _bookingRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<BookingService> _logger;


    public BookingService(IBookingRepository bookingRepository, ICacheService cache, AppDbContext appDbContext,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _cache = cache;
        _dbContext = appDbContext;
        _logger = logger;
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
        var dtos = bookings.Select(b =>
            new BookingDto(b.Id, b.ResourceId, b.StartDate, b.EndDate, b.CreatedAt, b.BookedBy));
        _logger.LogInformation("Fetched {Count} bookings from database", dtos.Count()); // ze structured logging
        await _cache.SetAsync(CACHE_KEY_ALL, dtos);
        return dtos;
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching booking with id {Id}", id);
        if (id == Guid.Empty)
        {
            _logger.LogError("Id cannot be empty");
            throw new ArgumentException("Id cannot be empty");
        }

        var cached = await _cache.GetAsync<BookingDto>($"booking:{id}");
        if (cached != null)
        {
            _logger.LogInformation("Cache hit for booking with id {Id}", id);
            return cached;
        }

        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking is null) return null;
        _logger.LogInformation("Fetched booking with id {Id} from database", id);
        var dto = new BookingDto(booking.Id, booking.ResourceId, booking.StartDate, booking.EndDate, booking.CreatedAt,
            booking.BookedBy);
        await _cache.SetAsync($"booking:{id}", dto);
        return dto;
    }

    //TODO:  Nice to have Unit of work - separate class
    // TODO: move to IResourceRepository.GetWithLockAsync when UoW is implemented
    public async Task<BookingDto> CreateAsync(CreateBookingCommand command)
    {
        _logger.LogInformation("Creating booking");
        ArgumentNullException.ThrowIfNull(command);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var resource = await _dbContext.Resources
                .FromSqlRaw("SELECT * FROM \"Resources\" WHERE \"Id\" = {0} FOR UPDATE", command.ResourceId)
                .FirstOrDefaultAsync();

            if (resource is null)
            {
                _logger.LogError("Resource not found during creating booking");
                throw new InvalidOperationException("Resource not found");
            }

            var hasConflict =
                await _bookingRepository.HasConflictAsync(command.ResourceId, command.StartDate, command.EndDate);
            // if (hasConflict) throw new ConflictException("Resource not available");
            if (hasConflict)
            {
                _logger.LogError("Conflict during creating booking");
                throw new InvalidOperationException("Resource not available");
            }

            var newBooking = new Booking()
            {
                ResourceId = command.ResourceId,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                CreatedAt = DateTime.UtcNow,
                BookedBy = command.BookingBy
            };
            var booking = await _bookingRepository.CreateAsync(newBooking);
            await transaction.CommitAsync();

            _logger.LogInformation("Booking created");
            await _cache.RemoveAsync(CACHE_KEY_ALL);
            return new BookingDto(booking.Id, booking.ResourceId, booking.StartDate, booking.EndDate, booking.CreatedAt,
                booking.BookedBy);
        }
        catch
        {
            await transaction.RollbackAsync();
            _logger.LogError("Failed to create booking");
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty");
        _logger.LogInformation("Deleting booking with id {Id}", id);
        await _bookingRepository.DeleteAsync(id);
        await _cache.RemoveAsync(CACHE_KEY_ALL);
        _logger.LogInformation("Deleted booking with id {Id}", id);
    }
}