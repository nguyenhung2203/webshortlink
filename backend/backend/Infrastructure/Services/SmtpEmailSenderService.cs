using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Infrastructure.Options;

namespace WebShortlink.Backend.Infrastructure.Services;

public sealed class SmtpEmailSenderService : IEmailSenderService, IDisposable
{
    private readonly SmtpOptions _options;
    private readonly ILogger<SmtpEmailSenderService> _logger;
    private readonly SmtpClient _smtpClient;

    public SmtpEmailSenderService(IOptions<SmtpOptions> options, ILogger<SmtpEmailSenderService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        _smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            EnableSsl = _options.EnableSsl
        };
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_options.Host) || string.IsNullOrWhiteSpace(_options.Username))
            {
                _logger.LogWarning("Chưa cấu hình SMTP Host/Username. Bỏ qua việc gửi email thật.");
                return;
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.FromEmail, _options.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            
            mailMessage.To.Add(toEmail);

            // Use token registration to cancel SmtpClient if needed
            using (cancellationToken.Register(() => _smtpClient.SendAsyncCancel()))
            {
                await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
            }
            
            _logger.LogInformation("SMTP: Email Sent To {ToEmail} Successfully.", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi email SMTP đến {ToEmail}.", toEmail);
            throw; // Reraise to inform caller that delivery failed
        }
    }

    public void Dispose()
    {
        _smtpClient.Dispose();
    }
}
