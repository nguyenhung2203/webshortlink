using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Domains;

public sealed class DomainService
{
    private const string DomainsFeatureKey = "domains.custom";

    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPlanCapabilityService _planCapabilityService;
    private readonly IAuditLogService _auditLogService;

    public DomainService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IPlanCapabilityService planCapabilityService,
        IAuditLogService auditLogService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _planCapabilityService = planCapabilityService;
        _auditLogService = auditLogService;
    }

    public async Task<IReadOnlyCollection<DomainListItemDto>> GetMineAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        return await _dbContext.Domains.AsNoTracking()
            .Where(x => x.UserId == current.UserId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new DomainListItemDto(x.Id, x.Host, x.IsVerified, x.VerificationToken, x.VerifiedAtUtc, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<DomainListItemDto> CreateAsync(CreateDomainRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, DomainsFeatureKey, cancellationToken);
        var domainLimit = await _planCapabilityService.GetLimitAsync(current.UserId, "domains.max_count", cancellationToken) ?? 0;
        var currentDomains = await _dbContext.Domains.CountAsync(x => x.UserId == current.UserId && !x.IsDeleted, cancellationToken);
        if (currentDomains >= domainLimit)
        {
            throw new AppException(ErrorCodes.PlanFeatureDenied, "Da vuot gioi han so luong custom domain.", StatusCodes.Status403Forbidden);
        }

        var normalizedHost = NormalizeHost(request.Host);
        var exists = await _dbContext.Domains.AnyAsync(x => x.Host == normalizedHost && !x.IsDeleted, cancellationToken);
        if (exists)
        {
            throw new AppException(ErrorCodes.Conflict, "Domain da ton tai.", StatusCodes.Status409Conflict);
        }

        var domain = new CustomDomain
        {
            Id = Guid.NewGuid(),
            UserId = current.UserId,
            Host = normalizedHost,
            VerificationToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()).ToLowerInvariant(),
            IsVerified = false,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        _dbContext.Domains.Add(domain);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DomainListItemDto(domain.Id, domain.Host, domain.IsVerified, domain.VerificationToken, domain.VerifiedAtUtc, domain.CreatedAtUtc);
    }

    public async Task<VerifyDomainResponseDto> VerifyAsync(Guid domainId, VerifyDomainRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var domain = await _dbContext.Domains.FirstOrDefaultAsync(x => x.Id == domainId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay domain.", StatusCodes.Status404NotFound);

        if (!string.Equals(domain.VerificationToken, request.VerificationToken?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Verification token khong hop le.");
        }

        domain.IsVerified = true;
        domain.VerifiedAtUtc = DateTime.UtcNow;
        domain.UpdatedAtUtc = DateTime.UtcNow;
        domain.UpdatedByUserId = current.UserId.ToString();
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.WriteAsync(
            AuditActorType.User,
            "USR-API-DOMAIN-VERIFY",
            nameof(CustomDomain),
            domain.Id.ToString(),
            current.UserId,
            current.IpAddress,
            new { domain.Host },
            cancellationToken);

        return new VerifyDomainResponseDto(true, domain.Host, "Xac minh domain thanh cong.");
    }

    public async Task<MessageResponseDto> DeleteAsync(Guid domainId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var domain = await _dbContext.Domains.FirstOrDefaultAsync(x => x.Id == domainId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay domain.", StatusCodes.Status404NotFound);

        var linkedLinks = await _dbContext.Links.AnyAsync(x => x.DomainId == domainId && !x.IsDeleted, cancellationToken);
        if (linkedLinks)
        {
            throw new AppException(ErrorCodes.Conflict, "Domain dang duoc su dung boi shortlink.", StatusCodes.Status409Conflict);
        }

        domain.IsDeleted = true;
        domain.DeletedAtUtc = DateTime.UtcNow;
        domain.DeletedByUserId = current.UserId.ToString();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Da xoa domain.");
    }

    public static string NormalizeHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Host khong hop le.");
        }

        var normalized = host.Trim().ToLowerInvariant();
        normalized = normalized.Replace("https://", string.Empty).Replace("http://", string.Empty);
        normalized = normalized.Trim('/').Split('/')[0];

        if (normalized.Contains(':'))
        {
            normalized = normalized.Split(':')[0];
        }

        if (!Uri.CheckHostName(normalized).Equals(UriHostNameType.Dns))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Host khong hop le.");
        }

        return normalized;
    }
}
