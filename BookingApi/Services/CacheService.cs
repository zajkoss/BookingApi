using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.CircuitBreaker;

namespace BookingApi.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ResiliencePipeline _pipeline;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
        _pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30)
            })
            .Build();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async ct =>
            {
                var data = await _cache.GetAsync(key.ToLower());
                if (data is null) return default;
                return JsonSerializer.Deserialize<T>(data);
            });
        }
        catch (BrokenCircuitException e)
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
        };

        try
        {
            _pipeline.ExecuteAsync(async ct =>
            {
                var data = await _cache.GetAsync(key.ToLower());
                await _cache.SetAsync(key.ToLower(), JsonSerializer.SerializeToUtf8Bytes(value), options);
            });
        }
        catch (BrokenCircuitException e)
        {
            //Nothing to do
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _pipeline.ExecuteAsync(async ct => { await _cache.RemoveAsync(key.ToLower()); });
        }
        catch (BrokenCircuitException e)
        {
            //Nothing to do
        }
    }
}