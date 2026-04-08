using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class CustomDomain : SoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Host { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string VerificationToken { get; set; } = string.Empty;
    public DateTime? VerifiedAtUtc { get; set; }
    public bool IsGlobal { get; set; }

    public AppUser User { get; set; } = null!;
    public ICollection<Link> Links { get; set; } = new List<Link>();
}
