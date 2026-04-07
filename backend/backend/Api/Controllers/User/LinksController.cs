using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Links;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user/links")]
public sealed class LinksController : ControllerBase
{
    private readonly LinkService _linkService;

    public LinksController(LinkService linkService)
    {
        _linkService = linkService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLinkRequestDto request, CancellationToken cancellationToken)
        => Ok(await _linkService.CreateAsync(request, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        => Ok(await _linkService.GetMineAsync(cancellationToken));

    [HttpGet("{linkId:guid}")]
    public async Task<IActionResult> GetDetail(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _linkService.GetMyDetailAsync(linkId, cancellationToken));

    [HttpPut("{linkId:guid}")]
    public async Task<IActionResult> Update(Guid linkId, [FromBody] UpdateLinkRequestDto request, CancellationToken cancellationToken)
        => Ok(await _linkService.UpdateAsync(linkId, request, cancellationToken));

    [HttpPatch("{linkId:guid}/status")]
    public async Task<IActionResult> Toggle(Guid linkId, [FromBody] ToggleLinkStatusRequestDto request, CancellationToken cancellationToken)
        => Ok(await _linkService.ToggleAsync(linkId, request, cancellationToken));

    [HttpDelete("{linkId:guid}")]
    public async Task<IActionResult> Delete(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _linkService.DeleteAsync(linkId, cancellationToken));
}
