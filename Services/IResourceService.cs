using BookingApi.DTOs;

namespace BookingApi.Services;
public interface IResourceService
{
    Task<IEnumerable<ResourceDto>> GetAllAsync();
}