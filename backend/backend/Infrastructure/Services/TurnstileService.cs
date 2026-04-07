using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Infrastructure.Options;

namespace WebShortlink.Backend.Infrastructure.Services;

public sealed class TurnstileService : ITurnstileService
{
    private readonly HttpClient _httpClient;
    private readonly TurnstileOptions _options;

    public TurnstileService(HttpClient httpClient, IOptions<TurnstileOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task VerifyAsync(string? token, string? remoteIp, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new AppException(ErrorCodes.TurnstileFailed, "Thiếu Turnstile token.");
        }

        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = _options.SecretKey,
            ["response"] = token,
            ["remoteip"] = remoteIp ?? string.Empty
        });

        var response = await _httpClient.PostAsync(_options.VerifyUrl, content, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<TurnstileVerifyResponse>(cancellationToken: cancellationToken);
        if (!response.IsSuccessStatusCode || payload?.Success != true)
        {
            throw new AppException(ErrorCodes.TurnstileFailed, "Xác minh Turnstile thất bại.");
        }
    }

    private sealed record TurnstileVerifyResponse(bool Success);
}
