using Microsoft.Extensions.Logging;
using WebShortlink.Backend.Application.Abstractions;

namespace WebShortlink.Backend.Infrastructure.Services;

public sealed class ConsoleEmailSenderService : IEmailSenderService
{
    private readonly ILogger<ConsoleEmailSenderService> _logger;

    public ConsoleEmailSenderService(ILogger<ConsoleEmailSenderService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("========== SIMULATED EMAIL DELIVERY ==========");
        _logger.LogWarning("TO: {ToEmail}", toEmail);
        _logger.LogWarning("SUBJECT: {Subject}", subject);
        _logger.LogWarning("BODY:\n{Body}", body);
        _logger.LogWarning("=============================================");
        
        return Task.CompletedTask;
    }
}
