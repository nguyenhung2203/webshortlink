namespace WebShortlink.Backend.Application.Abstractions;

public interface IEmailSenderService
{
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
