using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Infrastructure.Options;

namespace WebShortlink.Backend.Infrastructure.Queue;

public sealed class RedisAnalyticsQueue : IAnalyticsQueue
{
    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;
    private readonly ILogger<RedisAnalyticsQueue> _logger;

    public RedisAnalyticsQueue(IConnectionMultiplexer redis, IOptions<RedisOptions> options, ILogger<RedisAnalyticsQueue> logger)
    {
        _redis = redis;
        _options = options.Value;
        _logger = logger;
    }

    public async Task EnqueueAsync(ClickTrackingMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var db = _redis.GetDatabase();
            await db.ListRightPushAsync(_options.ClickQueueKey, JsonSerializer.Serialize(message));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong day duoc click event vao Redis queue. Redirect van tiep tuc.");
        }
    }

    public async Task<ClickTrackingMessage?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var db = _redis.GetDatabase();
            var value = await db.ListLeftPopAsync(_options.ClickQueueKey);
            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<ClickTrackingMessage>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong doc duoc click event tu Redis queue.");
            return null;
        }
    }
}
