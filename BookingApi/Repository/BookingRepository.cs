using BookingApi.Data;
using BookingApi.Models;
using BookingApi.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Repository;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> HasConflictAsync(Guid resourceId, DateTime start, DateTime end)
    {
        return await _dbSet.AnyAsync(b => b.ResourceId == resourceId && b.StartDate < end && b.EndDate > start);
    }
}
