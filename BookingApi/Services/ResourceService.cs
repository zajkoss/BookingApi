using AutoMapper;
using BookingApi.Commands;
using BookingApi.DTOs;
using BookingApi.Exceptions;
using BookingApi.Models;
using BookingApi.Repository;

namespace BookingApi.Services;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly ILogger<ResourceService> _logger;
    private readonly IMapper _mapper;

    public ResourceService(IResourceRepository resourceRepository, ILogger<ResourceService> logger, IMapper mapper)
    {
        _resourceRepository = resourceRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        var resources = await _resourceRepository.GetAllAsync();
        if (resources is null) return Enumerable.Empty<ResourceDto>();
        var dtos = resources
            .Select(r => _mapper.Map<ResourceDto>(r))
            .ToList();
        
        return dtos;
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceCommand newResource)
    {
        var resource = _mapper.Map<Resource>(newResource);
        resource.IsActive = true;
        var savedResource = await _resourceRepository.CreateAsync(resource);
        return new ResourceDto(savedResource.Id, savedResource.Name, savedResource.Description, savedResource.Capacity,
            savedResource.IsActive);
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid id)
    {   
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty");
        var resource = await _resourceRepository.GetByIdAsync(id);
        if (resource is null) return null;
        var dto = _mapper.Map<ResourceDto>(resource);
        return dto;
    }

    public async Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceCommand resource)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty");
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if (byIdAsync is null) throw new NotFoundException($"Resource {id} not found");
        byIdAsync.Name = resource.Name;
        byIdAsync.Description = resource.Description;
        byIdAsync.Capacity = resource.Capacity;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        return _mapper.Map<ResourceDto>(updateAsync);
    }

    public async Task<ResourceDto?> DeactiveAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty");
        var byIdAsync = await _resourceRepository.GetByIdAsync(id);
        if (byIdAsync is null) throw new NotFoundException($"Resource {id} not found");
        if (!byIdAsync.IsActive) throw new ConflictException($"Resource {id} is already inactive");        
        byIdAsync.IsActive = false;
        var updateAsync = await _resourceRepository.UpdateAsync(byIdAsync);
        return _mapper.Map<ResourceDto>(updateAsync);
    }
}