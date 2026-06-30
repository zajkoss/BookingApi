using BookingApi.Commands;
using BookingApi.DTOs;

namespace BookingApi.Services;

public class CachedResourceService : IResourceService
{
    private readonly IResourceService _inner;
    private readonly ICacheService _cache;

    public CachedResourceService(IResourceService inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IEnumerable<ResourceDto>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<ResourceDto>>("resources:all");
        if (cached is not null) return cached;
        var resourceDtos = await _inner.GetAllAsync();
        await _cache.SetAsync("resources:all", resourceDtos);
        return resourceDtos;
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceCommand newResource)
    {
        var resourceDto = await _inner.CreateAsync(newResource);
        await _cache.RemoveAsync("resources:all");
        return resourceDto;
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid id)
    {
        var cached = await _cache.GetAsync<ResourceDto>($"resources:{id}");
        if (cached is not null) return cached;
        var dto = await _inner.GetByIdAsync(id);
        if (dto is not null)
            await _cache.SetAsync($"resources:{id}", dto);
        return dto;
    }

    public async Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceCommand resource)
    {
        var resourceDto = await _inner.UpdateAsync(id, resource);
        await _cache.RemoveAsync("resources:all");
        await _cache.RemoveAsync($"resources:{id}");
        return resourceDto;
    }

    public async Task<ResourceDto?> DeactiveAsync(Guid id)
    {
        var deactiveAsync = await _inner.DeactiveAsync(id);
        await _cache.RemoveAsync("resources:all");
        await _cache.RemoveAsync($"resources:{id}");
        return deactiveAsync;
    }
}