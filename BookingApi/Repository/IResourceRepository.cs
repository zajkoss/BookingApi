using BookingApi.Models;

namespace BookingApi.Repository;

public interface IResourceRepository
{
    Task<IEnumerable<Resource>> GetAllAsync();
    Task<Resource> CreateAsync(Resource resource);
    Task<Resource?> GetByIdAsync(Guid id);
    Task<Resource?> UpdateAsync(Resource resource);

}