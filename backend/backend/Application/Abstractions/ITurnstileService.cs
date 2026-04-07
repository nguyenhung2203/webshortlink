namespace WebShortlink.Backend.Application.Abstractions;

public interface ITurnstileService
{
    Task VerifyAsync(string? token, string? remoteIp, CancellationToken cancellationToken = default);
}
