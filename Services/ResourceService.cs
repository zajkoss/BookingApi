using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Models;
using BookingApi.Repository;

namespace BookingApi.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;

    public ResourceService(IResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        var resources = await _resourceRepository.GetAllAsync();
        return resources
            .Select(r => new ResourceDto(r.Id, r.Name, r.Description, r.Capacity, r.IsActive))
            .ToList();
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceCommand newResource)
    {
        var resource = new Resource
        {
            Name = newResource.Name,
            Description = newResource.Description,
            Capacity = newResource.Capacity,
            IsActive = true 
        };
        var savedResource= await _resourceRepository.CreateAsync(resource);
        return new ResourceDto(savedResource.Id, savedResource.Name, savedResource.Description, savedResource.Capacity, savedResource.IsActive);
      
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid id)
    {
        var resource = await _resourceRepository.GetByIdAsync(id);
        return resource is null ? null : new ResourceDto(resource.Id, resource.Name, resource.Description, resource.Capacity, resource.IsActive);
    }

    public async Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceCommand resource)
    {
        if(id == Guid.Empty) return null;
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if(byIdAsync is null) return null;
        byIdAsync.Name = resource.Name;
        byIdAsync.Description = resource.Description;    
        byIdAsync.Capacity = resource.Capacity;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        return new ResourceDto(id, updateAsync.Name, updateAsync.Description, updateAsync.Capacity, updateAsync.IsActive);
    }

    public async Task<ResourceDto?> DeactiveAsync(Guid id)
    {
        if(id == Guid.Empty) return null;
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if(byIdAsync is null) return null;
        byIdAsync.IsActive = false;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        return new ResourceDto(id, updateAsync.Name, updateAsync.Description, updateAsync.Capacity, updateAsync.IsActive);
    }
}