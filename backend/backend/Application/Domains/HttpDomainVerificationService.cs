using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebShortlink.Backend.Application.Domains;

public class HttpDomainVerificationService : IDomainVerificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpDomainVerificationService> _logger;

    public HttpDomainVerificationService(HttpClient httpClient, ILogger<HttpDomainVerificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> VerifyDomainOwnershipAsync(string host, string verificationToken, CancellationToken cancellationToken = default)
    {
        var schemes = new[] { "https", "http" };

        foreach (var scheme in schemes)
        {
            var url = $"{scheme}://{host}/.well-known/webshortlink-verification.txt";
            
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    if (content.Trim().Equals(verificationToken.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    _logger.LogWarning("Domain verification failed for {Host} using {Scheme}: token mismatched.", host, scheme);
                }
                else
                {
                    _logger.LogInformation("Domain verification failed for {Host} using {Scheme}: HTTP {StatusCode}.", host, scheme, response.StatusCode);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Domain verification timeout for {Host} using {Scheme}.", host, scheme);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Domain verification error for {Host} using {Scheme}.", host, scheme);
            }
        }

        return false;
    }
}
