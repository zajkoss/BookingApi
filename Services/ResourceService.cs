using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Models;
using BookingApi.Repository;

namespace BookingApi.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ICacheService _cache;

    public ResourceService(IResourceRepository resourceRepository, ICacheService cache)
    {
        _resourceRepository = resourceRepository;
        _cache = cache;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<ResourceDto>>("resources:all");
        if (cached is not null) return cached;
        
        var resources = await _resourceRepository.GetAllAsync();
        var dtos = resources
            .Select(r => new ResourceDto(r.Id, r.Name, r.Description, r.Capacity, r.IsActive))
            .ToList();
        
        await _cache.SetAsync("resources:all", dtos);
        return dtos;
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
        var savedResource = await _resourceRepository.CreateAsync(resource);
        await _cache.RemoveAsync("resources:all");
        return new ResourceDto(savedResource.Id, savedResource.Name, savedResource.Description, savedResource.Capacity,
            savedResource.IsActive);
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid id)
    {   
        if (id == Guid.Empty) return null;
        var cached = await _cache.GetAsync<ResourceDto>($"resources:{id}");
        if (cached is not null) return cached;
        var resource = await _resourceRepository.GetByIdAsync(id);
        await _cache.SetAsync($"resources:{id}", resource);
        var dto = new ResourceDto(resource.Id, resource.Name, resource.Description, resource.Capacity, resource.IsActive);
        await _cache.SetAsync($"resources:{id}", dto);
        return dto;
    }

    public async Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceCommand resource)
    {
        if (id == Guid.Empty) return null;
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if (byIdAsync is null) return null;
        byIdAsync.Name = resource.Name;
        byIdAsync.Description = resource.Description;
        byIdAsync.Capacity = resource.Capacity;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        await _cache.RemoveAsync("resources:all");
        await _cache.RemoveAsync($"resources:{id}");
        return new ResourceDto(id, updateAsync.Name, updateAsync.Description, updateAsync.Capacity,
            updateAsync.IsActive);
    }

    public async Task<ResourceDto?> DeactiveAsync(Guid id)
    {
        if (id == Guid.Empty) return null;
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if (byIdAsync is null) return null;
        byIdAsync.IsActive = false;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        await _cache.RemoveAsync("resources:all");
        await _cache.RemoveAsync($"resources:{id}");
        return new ResourceDto(id, updateAsync.Name, updateAsync.Description, updateAsync.Capacity,
            updateAsync.IsActive);
    }
}