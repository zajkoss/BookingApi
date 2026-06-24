using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Models;

namespace BookingApi.Services;
public interface IResourceService
{
    Task<IEnumerable<ResourceDto>> GetAllAsync();
    Task<ResourceDto> CreateAsync(CreateResourceCommand newResource);
    Task<ResourceDto?> GetByIdAsync(Guid id);
    Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceCommand resource);
    Task<ResourceDto?> DeactiveAsync(Guid id);

}