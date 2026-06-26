using BookingApi.Models;
using BookingApi.Repository.Common;

namespace BookingApi.Repository;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<bool> HasConflictAsync(Guid resourceId, DateTime start, DateTime end);

}