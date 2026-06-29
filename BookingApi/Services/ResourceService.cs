using AutoMapper;
using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Models;
using BookingApi.Repository;

namespace BookingApi.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ICacheService _cache;
    private readonly ILogger<ResourceService> _logger;
    private readonly IMapper _mapper;

    public ResourceService(IResourceRepository resourceRepository, ICacheService cache, ILogger<ResourceService> logger, IMapper mapper)
    {
        _resourceRepository = resourceRepository;
        _cache = cache;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<ResourceDto>>("resources:all");
        if (cached is not null) return cached;
        
        var resources = await _resourceRepository.GetAllAsync();
        var dtos = resources
            .Select(r => _mapper.Map<ResourceDto>(r))
            .ToList();
        
        await _cache.SetAsync("resources:all", dtos);
        return dtos;
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceCommand newResource)
    {
        var resource = _mapper.Map<Resource>(newResource);
        resource.IsActive = true;
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
        var dto = _mapper.Map<ResourceDto>(resource);
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
        return _mapper.Map<ResourceDto>(updateAsync);
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
        return _mapper.Map<ResourceDto>(updateAsync);
    }
}