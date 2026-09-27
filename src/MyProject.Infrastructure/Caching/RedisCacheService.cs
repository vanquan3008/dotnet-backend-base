using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Caching;

internal sealed class RedisCacheService(
    IDistributedCache cache,
    IOptions<RedisOptions> options) : ICacheService
{
    private readonly TimeSpan defaultExpiration =
        TimeSpan.FromMinutes(options.Value.DefaultExpirationMinutes);

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var json = await cache.GetStringAsync(key, cancellationToken);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? defaultExpiration
        };
        return cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            cacheOptions,
            cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return cache.RemoveAsync(key, cancellationToken);
    }
}
