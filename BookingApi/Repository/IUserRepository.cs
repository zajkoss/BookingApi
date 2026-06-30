using BookingApi.Models;
using BookingApi.Repository.Common;

namespace BookingApi.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}