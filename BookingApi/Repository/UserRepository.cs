using BookingApi.Data;
using BookingApi.Models;
using BookingApi.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}