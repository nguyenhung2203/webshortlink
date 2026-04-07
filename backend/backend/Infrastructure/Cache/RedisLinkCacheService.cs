using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Links;
using WebShortlink.Backend.Infrastructure.Options;

namespace WebShortlink.Backend.Infrastructure.Cache;

public sealed class RedisLinkCacheService : ILinkCacheService
{
    private readonly IDistributedCache _cache;
    private readonly RedisOptions _options;
    private readonly ILogger<RedisLinkCacheService> _logger;

    public RedisLinkCacheService(IDistributedCache cache, IOptions<RedisOptions> options, ILogger<RedisLinkCacheService> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CachedLinkDto?> GetAsync(string host, string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = await _cache.GetStringAsync(BuildKey(host, slug), cancellationToken);
            return payload is null ? null : JsonSerializer.Deserialize<CachedLinkDto>(payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong doc duoc link cache cho host {Host}, slug {Slug}.", host, slug);
            return null;
        }
    }

    public async Task SetAsync(CachedLinkDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetStringAsync(
                BuildKey(dto.Host, dto.Slug),
                JsonSerializer.Serialize(dto),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_options.HotLinkTtlMinutes)
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong ghi duoc link cache cho host {Host}, slug {Slug}.", dto.Host, dto.Slug);
        }
    }

    public async Task RemoveAsync(string host, string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(BuildKey(host, slug), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong xoa duoc link cache cho host {Host}, slug {Slug}.", host, slug);
        }
    }

    private string BuildKey(string host, string slug) => $"{_options.InstanceName}link:{host.ToLowerInvariant()}:{slug.ToLowerInvariant()}";
}
