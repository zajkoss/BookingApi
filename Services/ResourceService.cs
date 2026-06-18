using BookingApi.DTOs;
using BookingApi.Models;

namespace BookingApi.Services;
public class ResourceService : IResourceService
{
    
    private List<Resource> resources = new()
    {
        new Resource { Id = Guid.NewGuid(), Name = "Room A", Description = "Conference room", Capacity = 10, IsActive = true },
        new Resource { Id = Guid.NewGuid(), Name = "Room B", Description = null, Capacity = 5, IsActive = true }
    };
    
    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        return resources
            .Select(r => new ResourceDto(r.Id, r.Name, r.Description, r.Capacity, r.IsActive))
            .ToList();

    }
}