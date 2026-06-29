using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace BookingApi.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetAsync(key.ToLower());
        if (data is null) return default;
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };
        await _cache.SetAsync(key.ToLower(), JsonSerializer.SerializeToUtf8Bytes(value), options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key.ToLower());
    }
}