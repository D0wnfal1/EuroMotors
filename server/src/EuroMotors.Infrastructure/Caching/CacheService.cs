using System.Buffers;
using System.Text.Json;
using EuroMotors.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace EuroMotors.Infrastructure.Caching;

internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    private const string KeysPrefix = "cache_keys:";
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        await cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
        
        string prefix = GetPrefix(key);
        string keysKey = $"{KeysPrefix}{prefix}";
        
        IEnumerable<string> keys = await GetKeysByPrefixCoreAsync(prefix, cancellationToken);
        var keysList = keys.ToList();
        
        if (!keysList.Contains(key))
        {
            keysList.Add(key);
            await cache.SetAsync(keysKey, Serialize(keysList), CacheOptions.Create(TimeSpan.FromDays(30)), cancellationToken);
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        cache.RemoveAsync(key, cancellationToken);
        
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> keys = await GetKeysByPrefixCoreAsync(prefix, cancellationToken);
        
        foreach (string key in keys)
        {
            await RemoveAsync(key, cancellationToken);
        }
        
        await RemoveAsync($"{KeysPrefix}{prefix}", cancellationToken);
    }
    
    public async Task<IEnumerable<string>> GetKeysByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        return await GetKeysByPrefixCoreAsync(prefix, cancellationToken);
    }
    
    private async Task<IEnumerable<string>> GetKeysByPrefixCoreAsync(string prefix, CancellationToken cancellationToken)
    {
        string keysKey = $"{KeysPrefix}{prefix}";
        byte[]? bytes = await cache.GetAsync(keysKey, cancellationToken);
        
        if (bytes == null)
        {
            return Enumerable.Empty<string>();
        }
        
        return Deserialize<List<string>>(bytes) ?? new List<string>();
    }
    
    private static string GetPrefix(string key)
    {
        int separatorIndex = key.IndexOf(':');
        return separatorIndex > 0 ? key.Substring(0, separatorIndex) : key;
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes)!;
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }
}
