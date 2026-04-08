using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Domains;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user/domains")]
public sealed class DomainsController : ControllerBase
{
    private readonly DomainService _domainService;

    public DomainsController(DomainService domainService)
    {
        _domainService = domainService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => Ok(await _domainService.GetMineAsync(cancellationToken));

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(CancellationToken cancellationToken)
        => Ok(await _domainService.GetAvailableAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDomainRequestDto request, CancellationToken cancellationToken)
        => Ok(await _domainService.CreateAsync(request, cancellationToken));

    [HttpPost("{domainId:guid}/verify")]
    public async Task<IActionResult> Verify(Guid domainId, [FromBody] VerifyDomainRequestDto request, CancellationToken cancellationToken)
        => Ok(await _domainService.VerifyAsync(domainId, request, cancellationToken));

    [HttpDelete("{domainId:guid}")]
    public async Task<IActionResult> Delete(Guid domainId, CancellationToken cancellationToken)
        => Ok(await _domainService.DeleteAsync(domainId, cancellationToken));
}
