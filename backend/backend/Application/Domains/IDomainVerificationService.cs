using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebShortlink.Backend.Application.Domains;

public interface IDomainVerificationService
{
    Task<bool> VerifyDomainOwnershipAsync(string host, string verificationToken, CancellationToken cancellationToken = default);
}
