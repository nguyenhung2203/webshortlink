using WebShortlink.Backend.Application.Links;

namespace WebShortlink.Backend.Application.Abstractions;

public interface ILinkCacheService
{
    Task<CachedLinkDto?> GetAsync(string host, string slug, CancellationToken cancellationToken = default);
    Task SetAsync(CachedLinkDto dto, CancellationToken cancellationToken = default);
    Task RemoveAsync(string host, string slug, CancellationToken cancellationToken = default);
}
